using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AnyCAD.Foundation;
using CommunityToolkit.Mvvm.Input;
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
    /// <summary>加载前哪些轴已收到输入，未收到的轴仍使用各自初始角。</summary>
    private readonly bool[] _pendingAxes = new bool[6];
    /// <summary>场景创建入口，复用场景接口以验证加载失败与通信时序。</summary>
    private readonly Func<AnyCAD.WPF.RenderControl, IRobotSceneService> _sceneFactory;
    /// <summary>通知源的普通引用，通信回调不读取 WPF 依赖属性。</summary>
    private volatile IRobotViewerModel? _boundModel;
    /// <summary>等待 AnyCAD 渲染器就绪后继续加载。</summary>
    private readonly TaskCompletionSource<bool> _viewerReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    /// <summary>当前场景服务；释放后置空，阻止继续访问原生视口。</summary>
    private IRobotSceneService? _scene;
    /// <summary>正在进行的加载任务，公开加载与自动加载共用同一任务。</summary>
    private Task? _loadTask;
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
    public RobotViewer() : this(view => new RobotSceneService(view)) { }

    /// <summary>以指定场景创建器构造控件，供加载与线程回归测试使用。</summary>
    internal RobotViewer(Func<AnyCAD.WPF.RenderControl, IRobotSceneService> sceneFactory)
    {
        _sceneFactory = sceneFactory;
        ZoomAllCommand = new RelayCommand(() => _scene?.ZoomAll(), () => _loaded && !_disposed);
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

    /// <summary>保持当前观察方向，将完整模型居中并适配窗口；仅加载完成后可用。</summary>
    public IRelayCommand ZoomAllCommand { get; }

    /// <summary>按 Model.Options 加载固定组合；只能在 UI 线程调用一次。</summary>
    public Task LoadAsync()
    {
        if (!Dispatcher.CheckAccess()) throw new InvalidOperationException("LoadAsync 必须在 UI 线程调用。");
        if (_disposed) throw new ObjectDisposedException(nameof(RobotViewer));
        if (_loaded) throw new InvalidOperationException("控件已经加载，不能重复调用 LoadAsync。");
        if (_loading) return _loadTask!;
        var options = Model?.Options ?? throw new InvalidOperationException("必须设置 Model.Options。");
        _loading = true;
        return _loadTask = LoadSceneAsync(options);
    }

    /// <summary>一次完整加载成功后应用最新角度；失败时清理场景并允许再次尝试。</summary>
    private async Task LoadSceneAsync(RobotViewerOptions options)
    {
        SetStatus("正在加载三维模型…");
        try
        {
            await _viewerReady.Task;
            ObjectDisposedException.ThrowIf(_disposed, this);
            var robotId = ModelCatalog.GetId(options.Robot);
            var scene = _sceneFactory(RenderView);
            _scene = scene;
            scene.InitializeViewport();

            var robot = RobotConfigurationStore.LoadBuiltIn(robotId);
            await scene.LoadRobotAsync(robot, robotId);
            ObjectDisposedException.ThrowIf(_disposed, this);
            _joints = robot.Joints.Select((joint, index) => new JointViewModel(index, joint)).ToArray();

            var toolId = ModelCatalog.GetId(options.Tool);
            var tool = ToolConfigurationStore.LoadBuiltIn(toolId);
            await scene.LoadToolAsync(toolId, tool);
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (options.ToolTcp is not null) scene.SetToolTcp(options.ToolTcp);

            var workId = ModelCatalog.GetId(options.Work);
            var work = WorkConfigurationStore.LoadBuiltIn(workId);
            await scene.LoadWorkAsync(workId, work);
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (options.WorkPose is not null) scene.SetWorkPose(options.WorkPose);

            scene.SetCoordinateVisibility(
                options.ShowBaseCoordinates,
                options.ShowFlangeCoordinates,
                options.ShowTcpCoordinates);

            scene.SetView(RobotView.Isometric);
            scene.SetOrbit(options.OrbitSpeedDegreesPerSecond, options.OrbitStartAngleDegrees, options.OrbitElevationDegrees);

            // 完整加载前不消费通信帧；仅对未收到输入的轴填入初始角。
            var home = _joints.Select(joint => joint.HomeAngle).ToArray();
            double[] angles;
            lock (_updateSync)
            {
                for (var index = 0; index < home.Length; index++)
                    if (!_pendingAxes[index]) _pendingAngles[index] = home[index];
                _hasPendingAngles = false;
                _loaded = true;
                angles = (double[])_pendingAngles.Clone();
            }
            ApplyAngles(angles);

            SetStatus("模型已加载");
        }
        catch (Exception exception)
        {
            _loaded = false;
            _joints = [];
            _scene?.Dispose();
            _scene = null;
            if (!_disposed) SetStatus($"三维模型加载失败：{exception.Message}");
            throw;
        }
        finally { _loading = false; }
    }

    /// <summary>一次推送六轴角度，单位度；可在通信线程调用，只在 UI 帧内应用一次。</summary>
    public void SetJoints(IReadOnlyList<double> degrees)
    {
        if (degrees is null || degrees.Count < 6) throw new ArgumentException("需要 J1～J6 六个角度。", nameof(degrees));
        lock (_updateSync)
        {
            if (_disposed) return;
            for (var index = 0; index < _pendingAngles.Length; index++)
            {
                _pendingAngles[index] = degrees[index];
                _pendingAxes[index] = true;
            }
            ScheduleApply();
        }
    }

    /// <summary>只更新一个关节，其余轴保持最近一次的值；可在通信线程调用。</summary>
    public void SetJoint(int axis, double degrees)
    {
        if (axis is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(axis));
        lock (_updateSync)
        {
            if (_disposed) return;
            _pendingAngles[axis] = degrees;
            _pendingAxes[axis] = true;
            ScheduleApply();
        }
    }

    /// <summary>恢复等轴测视角并停止自转；不改变关节角度。</summary>
    public void ResetView()
    {
        if (_disposed || _scene is null) return;
        _scene.SetView(RobotView.Isometric);
    }

    /// <summary>释放场景和原生视口；必须在 UI 线程调用。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        lock (_updateSync) _disposed = true;
        _viewerReady.TrySetCanceled();
        RenderView.ViewerReady -= OnViewerReady;
        if (Model is not null) Model.PropertyChanged -= OnModelPropertyChanged;
        _boundModel = null;
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
        viewer._boundModel = eventArgs.NewValue as IRobotViewerModel;
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
        if (sender is not IRobotViewerModel model || !ReferenceEquals(model, _boundModel)) return;
        if (eventArgs.PropertyName == nameof(IRobotViewerModel.Joints)) ApplyBoundJoints(model.Joints);
        else if (eventArgs.PropertyName == nameof(IRobotViewerModel.Options))
        {
            // Options 涉及 WPF 控件与原生视口，只在界面线程处理。
            if (Dispatcher.CheckAccess()) StartLoadIfReady();
            else Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ReferenceEquals(model, _boundModel)) StartLoadIfReady();
            }));
        }
    }

    /// <summary>保存绑定角度；场景加载完成后由 LoadAsync 统一应用。</summary>
    private void ApplyBoundJoints(IReadOnlyList<double>? joints)
    {
        if (joints is null || joints.Count < 6) return;
        SetJoints(joints);
    }

    // 加载流程

    /// <summary>渲染器和 Model.Options 都就绪时开始加载，避免重复触发。</summary>
    private void StartLoadIfReady()
    {
        if (_disposed || _loaded || _loading || Model?.Options is null) return;
        if (!_viewerReady.Task.IsCompletedSuccessfully) return;
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
            if (_disposed || !_loaded || _updateQueued) return;
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
            if (_disposed || !_loaded || !_hasPendingAngles) return;
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
