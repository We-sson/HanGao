using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace RobotSimulator.Controls;

/// <summary>固定机器人、Tool 和工装组合的只读三维显示控件；宿主只需绑定一个 Model。</summary>
public partial class RobotViewer : UserControl, IDisposable
{
    /// <summary>AnyCAD 全局引擎只初始化一次，允许宿主重复创建控件。</summary>
    private static readonly object EngineSync = new();
    /// <summary>当前进程是否已经初始化过 AnyCAD。</summary>
    private static bool _engineInitialized;
    /// <summary>串行化通信线程的角度写入和 UI 线程的取帧。</summary>
    private readonly object _updateSync = new();
    /// <summary>最新待应用角度，避免高频通信时堆积多帧。</summary>
    private readonly double[] _pendingAngles = new double[6];
    /// <summary>等待 AnyCAD 渲染器就绪后继续加载。</summary>
    private readonly TaskCompletionSource<bool> _viewerReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    /// <summary>当前场景服务；释放后置空，阻止继续访问原生视口。</summary>
    private RobotSceneService? _scene;
    /// <summary>与场景六轴顺序一致的关节状态。</summary>
    private IReadOnlyList<JointViewModel> _joints = [];
    /// <summary>是否有尚未应用到场景的角度。</summary>
    private bool _hasPendingAngles;
    /// <summary>是否已经排入一次 UI 队列，避免同一帧重复排队。</summary>
    private bool _updateQueued;
    /// <summary>是否已按 Model.Options 完成加载。</summary>
    private bool _loaded;
    /// <summary>是否正在加载，防止重复触发。</summary>
    private bool _loading;
    /// <summary>释放标记，阻止加载或更新继续访问已销毁的控件。</summary>
    private bool _disposed;

    // 生命周期与初始化

    /// <summary>创建视口并订阅渲染器就绪事件。</summary>
    public RobotViewer()
    {
        InitializeEngine();
        InitializeComponent();
        RenderView.ViewerReady += OnViewerReady;
        if (RenderView.IsReady) OnViewerReady();
        Unloaded += (_, _) => Dispose();
    }

    /// <summary>在创建渲染控件前初始化 AnyCAD；重复调用 Initialize 已验证为安全。</summary>
    private static void InitializeEngine()
    {
        lock (EngineSync)
        {
            if (_engineInitialized) return;
            GlobalInstance.Initialize();
            ModelingEngine.EnableSnapShape();
            _engineInitialized = true;
        }
    }

    // 宿主绑定

    /// <summary>宿主数据；设置后自动加载模型并跟随 Joints/StatusMessage 变化。</summary>
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model), typeof(IRobotViewerModel), typeof(RobotViewer),
        new PropertyMetadata(null, OnModelPropertyChanged));

    /// <summary>三维控件所需的全部宿主数据。</summary>
    public IRobotViewerModel? Model
    {
        get => (IRobotViewerModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    // 公开操作

    /// <summary>按 Model.Options 加载固定组合；只能在 UI 线程调用一次。</summary>
    public async Task LoadAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RobotViewer));
        if (_loaded) throw new InvalidOperationException("控件已经加载，不能重复调用 LoadAsync。");
        if (!Dispatcher.CheckAccess()) throw new InvalidOperationException("LoadAsync 必须在 UI 线程调用。");
        var options = Model?.Options ?? throw new InvalidOperationException("必须设置 Model.Options。");

        SetStatus("正在加载三维模型…");
        try
        {
            await _viewerReady.Task;
            var robotId = ModelCatalog.GetId(options.Robot);
            _scene = new RobotSceneService(RenderView);
            _scene.InitializeViewport();

            var robot = RobotConfigurationStore.LoadBuiltIn(robotId);
            await _scene.LoadRobotAsync(robot, robotId);
            _joints = robot.Joints.Select((joint, index) => new JointViewModel(index, joint)).ToArray();

            var toolId = ModelCatalog.GetId(options.Tool);
            var tool = ToolConfigurationStore.LoadBuiltIn(toolId);
            await _scene.LoadToolAsync(toolId, tool);
            if (options.ToolTcp is not null) _scene.SetToolTcp(options.ToolTcp);

            var workId = ModelCatalog.GetId(options.Work);
            var work = WorkConfigurationStore.LoadBuiltIn(workId);
            await _scene.LoadWorkAsync(workId, work);
            if (options.WorkPose is not null) _scene.SetWorkPose(options.WorkPose);

            _scene.SetCoordinateVisibility(
                options.ShowBaseCoordinates,
                options.ShowFlangeCoordinates,
                options.ShowTcpCoordinates);

            // 加载前绑定过的角度优先，否则使用配置中的初始角。
            var home = _joints.Select(joint => joint.HomeAngle).ToArray();
            double[] angles;
            lock (_updateSync)
            {
                if (!_hasPendingAngles) Array.Copy(home, _pendingAngles, _pendingAngles.Length);
                _hasPendingAngles = false;
                angles = (double[])_pendingAngles.Clone();
            }
            ApplyAngles(angles);

            // 启动时固定为等轴测视角，并按配置启动相机环绕。
            _scene.SetView(RobotView.Isometric);
            _scene.SetOrbit(options.OrbitSpeedDegreesPerSecond, options.OrbitStartAngleDegrees, options.OrbitElevationDegrees);
            _loaded = true;
            SetStatus("模型已加载");
        }
        catch (Exception exception)
        {
            SetStatus($"三维模型加载失败：{exception.Message}");
            throw;
        }
    }

    /// <summary>一次推送六轴角度，单位度；可在通信线程调用，只在 UI 帧内应用一次。</summary>
    public void SetJoints(IReadOnlyList<double> degrees)
    {
        if (degrees is null || degrees.Count < 6) throw new ArgumentException("需要 J1～J6 六个角度。", nameof(degrees));
        lock (_updateSync)
        {
            for (var index = 0; index < _pendingAngles.Length; index++)
                _pendingAngles[index] = degrees[index];
        }
        ScheduleApply();
    }

    /// <summary>只更新一个关节，其余轴保持最近一次的值；可在通信线程调用。</summary>
    public void SetJoint(int axis, double degrees)
    {
        if (axis is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(axis));
        lock (_updateSync)
        {
            _pendingAngles[axis] = degrees;
        }
        ScheduleApply();
    }

    /// <summary>恢复等轴测视角；不改变关节角度。</summary>
    public void ResetView()
    {
        if (_disposed || _scene is null) return;
        _scene.SetView(RobotView.Isometric);
    }

    /// <summary>释放场景和原生视口；必须在 UI 线程调用。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        RenderView.ViewerReady -= OnViewerReady;
        if (Model is not null) Model.PropertyChanged -= OnModelPropertyChanged;
        _scene?.Dispose();
        _scene = null;
    }

    // 模型绑定回调

    /// <summary>Model 变化时切换事件订阅，并立即应用已有的关节和搭配。</summary>
    private static void OnModelPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
    {
        var viewer = (RobotViewer)dependencyObject;
        if (eventArgs.OldValue is IRobotViewerModel oldModel)
            oldModel.PropertyChanged -= viewer.OnModelPropertyChanged;
        if (eventArgs.NewValue is IRobotViewerModel newModel)
        {
            newModel.PropertyChanged += viewer.OnModelPropertyChanged;
            viewer.ApplyBoundJoints(newModel.Joints);
            viewer.StartLoadIfReady();
        }
    }

    /// <summary>Model 的 Joints 更新时推送角度；Options 变化时重新尝试加载。</summary>
    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(IRobotViewerModel.Joints)) ApplyBoundJoints(Model?.Joints);
        else if (eventArgs.PropertyName == nameof(IRobotViewerModel.Options)) StartLoadIfReady();
    }

    /// <summary>保存绑定角度；场景加载完成后由 LoadAsync 统一应用。</summary>
    private void ApplyBoundJoints(IReadOnlyList<double>? joints)
    {
        if (joints is null || joints.Count < 6) return;
        if (_loaded)
        {
            SetJoints(joints);
            return;
        }
        lock (_updateSync)
        {
            for (var index = 0; index < _pendingAngles.Length; index++)
                _pendingAngles[index] = joints[index];
            _hasPendingAngles = true;
        }
    }

    // 加载流程

    /// <summary>渲染器和 Model.Options 都就绪时开始加载，避免重复触发。</summary>
    private void StartLoadIfReady()
    {
        if (_disposed || _loaded || _loading || Model?.Options is null) return;
        if (!_viewerReady.Task.IsCompletedSuccessfully) return;
        _loading = true;
        _ = LoadCoreAsync();
    }

    /// <summary>依赖属性触发的加载；异常只写入 StatusMessage，不向 UI 线程抛出。</summary>
    private async Task LoadCoreAsync()
    {
        try
        {
            await LoadAsync();
        }
        catch
        {
            // LoadAsync 已经记录了失败原因。
        }
    }

    /// <summary>把就绪通知排到界面队列，避免在 AnyCAD 初始化过程中加载模型。</summary>
    private void OnViewerReady()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(OnViewerReady));
            return;
        }
        _viewerReady.TrySetResult(true);
        StartLoadIfReady();
    }

    // 关节更新

    /// <summary>合并高频调用：已有排队时只更新待应用角度，不再重复排队。</summary>
    private void ScheduleApply()
    {
        lock (_updateSync)
        {
            _hasPendingAngles = true;
            if (_updateQueued) return;
            _updateQueued = true;
        }
        Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(ApplyPendingAngles));
    }

    /// <summary>在 UI 线程取最新一帧角度并替换场景变换，六轴只请求一次绘制。</summary>
    private void ApplyPendingAngles()
    {
        double[] angles;
        lock (_updateSync)
        {
            _updateQueued = false;
            if (!_hasPendingAngles) return;
            _hasPendingAngles = false;
            angles = (double[])_pendingAngles.Clone();
        }
        ApplyAngles(angles);
    }

    /// <summary>把角度写入关节状态并批量更新场景；加载完成前调用会被忽略。</summary>
    private void ApplyAngles(double[] angles)
    {
        if (_disposed || _scene is null || _joints.Count != 6) return;
        for (var index = 0; index < _joints.Count; index++)
            _joints[index].Angle = angles[index];
        _scene.ApplyAll(_joints);
    }

    // 状态回写

    /// <summary>把加载状态写回 Model，供宿主界面绑定。</summary>
    private void SetStatus(string message)
    {
        if (Model is not null) Model.StatusMessage = message;
    }
}
