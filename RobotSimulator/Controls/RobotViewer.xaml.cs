using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using Macad.Interaction.Panels;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace RobotSimulator.Controls;

/// <summary>固定机器人、Tool 和工装组合的只读三维显示控件；宿主只需绑定一个 Model。</summary>
public partial class RobotViewer : UserControl, IDisposable
{
    /// <summary>串行化通信线程的角度写入和 UI 线程的取帧。</summary>
    private readonly object _updateSync = new();
    /// <summary>最新待应用角度，避免高频通信时堆积多帧。</summary>
    private readonly double[] _pendingAngles = new double[6];
    /// <summary>加载前哪些轴已收到输入，未收到的轴仍使用各自初始角。</summary>
    private readonly bool[] _pendingAxes = new bool[6];
    /// <summary>场景创建入口，复用场景接口以验证加载失败与通信时序。</summary>
    private readonly Func<ViewportPanel, IRobotSceneService> _sceneFactory;
    /// <summary>通知源的普通引用，通信回调不读取 WPF 依赖属性。</summary>
    private volatile IRobotViewerModel? _boundModel;
    /// <summary>进入可视树后开始加载场景。</summary>
    private readonly TaskCompletionSource<bool> _viewerReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    /// <summary>宿主窗口关闭时释放场景，标签页临时卸载时保留场景。</summary>
    private Window? _ownerWindow;
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
    /// <summary>视口隐藏期间只记录最新角度，不排界面更新。</summary>
    private bool _updatesSuspended;
    /// <summary>当前显示角度，按渲染帧逐步逼近通信目标。</summary>
    private readonly double[] _displayAngles = new double[6];
    /// <summary>当前显示角速度，单位度/秒；六个轴共用速度、加速度上限。</summary>
    private readonly double[] _displayVelocities = new double[6];
    /// <summary>是否已经完成第一次角度应用；首次加载直接贴合目标。</summary>
    private bool _hasAppliedAngles;
    /// <summary>是否正在订阅渲染帧进行平滑。</summary>
    private bool _smoothing;
    /// <summary>上一渲染帧时间；null 表示下一帧只记录时间。</summary>
    private TimeSpan? _lastRenderTime;
    /// <summary>最近一次把角度写入三维场景的渲染时间。</summary>
    private TimeSpan _lastApplyTime;
    /// <summary>到达目标的容差，单位度。</summary>
    public double JointPositionToleranceDegrees { get; set; } = JointMotion.DefaultToleranceDegrees;
    /// <summary>是否已按 Model.Options 完成加载。</summary>
    private bool _loaded;
    /// <summary>是否正在加载，防止重复触发。</summary>
    private bool _loading;
    /// <summary>加载期间收到新参数；当前加载结束后立即重建。</summary>
    private bool _reloadPending;
    /// <summary>当前进行中的加载所用参数，用于识别加载期间参数是否被替换。</summary>
    private RobotViewerOptions? _loadingOptions;
    /// <summary>释放标记，阻止加载或更新继续访问已销毁的控件。</summary>
    private bool _disposed;
    /// <summary>最近一次应用的相机环绕参数；仅这些参数变化时才重置相机。</summary>
    private (double Speed, double Start, double Elevation)? _appliedOrbit;

    // 生命周期与初始化

    /// <summary>创建视口并连接加载和可见性事件。</summary>
    public RobotViewer() : this(view => new RobotSceneService(view)) { }

    /// <summary>以指定场景创建器构造控件，供加载与线程回归测试使用。</summary>
    internal RobotViewer(Func<ViewportPanel, IRobotSceneService> sceneFactory)
    {
        _sceneFactory = sceneFactory;
        ZoomAllCommand = new RelayCommand(() => _scene?.ZoomAll(), () => _loaded && !_disposed);
        // ViewportPanel 依赖 Macad 全局交互上下文，必须在 XAML 创建视口之前准备好。
        MacadViewportBootstrap.EnsureContext();
        InitializeComponent();
        Behaviors.ViewportDoubleClickTrigger.SetCommand(RenderView, ZoomAllCommand);
        // 面板延后到控件加载完成再挂回，避免宿主窗口首次 Show 期间建原生视口。
        DeferredViewportHost.Defer(this, (Grid)Content, RenderView, () => _disposed);
        RenderView.Loaded += OnViewportLoaded;
        RenderView.Unloaded += OnViewportUnloaded;
        RenderView.IsVisibleChanged += OnViewportVisibilityChanged;
    }

    private void OnViewportLoaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window is not null && _ownerWindow != window)
        {
            if (_ownerWindow is not null) _ownerWindow.Closed -= OnOwnerClosed;
            _ownerWindow = window;
            _ownerWindow.Closed += OnOwnerClosed;
        }

        OnViewerReady();
        SetUpdatesSuspended(!RenderView.IsVisible);
    }

    private void OnViewportUnloaded(object sender, RoutedEventArgs e) => SetUpdatesSuspended(true);

    private void OnViewportVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (Model?.Options is { } options) ApplyRenderQuality(options.RenderQuality);
        SetUpdatesSuspended(!RenderView.IsVisible);
    }

    private void OnOwnerClosed(object? sender, EventArgs e) => Dispose();

    private void SetUpdatesSuspended(bool suspended)
    {
        lock (_updateSync)
        {
            if (_updatesSuspended == suspended) return;
            _updatesSuspended = suspended;
            if (suspended && _smoothing) _hasPendingAngles = true;
        }

        if (suspended)
            StopSmoothing();
        else
            ApplyPendingAngles();
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
        _loadingOptions = options;
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
            var liveOptions = Model?.Options ?? options;
            ApplyRenderQuality(liveOptions.RenderQuality);

            var robot = RobotConfigurationStore.LoadBuiltIn(robotId);
            await scene.LoadRobotAsync(robot, robotId);
            ObjectDisposedException.ThrowIf(_disposed, this);
            _joints = robot.Joints.Select((joint, index) => new JointViewModel(index, joint)).ToArray();
            // 先用初始角完成机器人装配，避免加载 Tool/工装期间显示散开的 STEP 零件。
            scene.ApplyAll(_joints);

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

            liveOptions = Model?.Options ?? options;
            scene.SetCoordinateVisibility(
                liveOptions.ShowBaseCoordinates,
                liveOptions.ShowFlangeCoordinates,
                liveOptions.ShowTcpCoordinates);
            scene.SetWeldBeamVisible(liveOptions.ShowWeldBeam);

            // 初始加载直接定位到配置的环绕起始角；SetView 的原生动画会与紧接着的
            // SetOrbit 起始方向冲突，因此只在用户主动切换标准视角时使用动画。
            var orbit = (liveOptions.OrbitSpeedDegreesPerSecond, liveOptions.OrbitStartAngleDegrees, liveOptions.OrbitElevationDegrees);
            scene.SetOrbit(orbit.OrbitSpeedDegreesPerSecond, orbit.OrbitStartAngleDegrees, orbit.OrbitElevationDegrees);
            _appliedOrbit = orbit;

            // 完整加载前不消费通信帧；仅对未收到输入的轴填入初始角。
            var home = _joints.Select(joint => joint.HomeAngle).ToArray();
            double[] angles;
            lock (_updateSync)
            {
                for (var index = 0; index < home.Length; index++)
                    if (!_pendingAxes[index]) _pendingAngles[index] = home[index];
                _hasPendingAngles = false;
                _loaded = true;
                angles = CaptureJointTargetsLocked();
            }
            Array.Copy(angles, _displayAngles, _displayAngles.Length);
            Array.Clear(_displayVelocities, 0, _displayVelocities.Length);
            _hasAppliedAngles = true;
            StopSmoothing();
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
        finally
        {
            _loading = false;
            _loadingOptions = null;
            if (_reloadPending)
            {
                _reloadPending = false;
                StartLoadIfReady();
            }
        }
    }

    /// <summary>从通信线程推送六根轴的有限角度，单位度；有任一轴异常时保留上一帧。</summary>
    public void SetJoints(IReadOnlyList<double> degrees)
    {
        if (degrees is null || degrees.Count < 6) throw new ArgumentException("需要 J1～J6 六个角度。", nameof(degrees));
        Span<double> angles = stackalloc double[6];
        for (var index = 0; index < angles.Length; index++)
        {
            var angle = degrees[index];
            if (!double.IsFinite(angle)) return;
            angles[index] = angle;
        }
        lock (_updateSync)
        {
            if (_disposed) return;
            angles.CopyTo(_pendingAngles);
            Array.Fill(_pendingAxes, true);
            ScheduleApply();
        }
    }

    /// <summary>只更新一个关节，其余轴保持最近一次的值；可在通信线程调用。</summary>
    public void SetJoint(int axis, double degrees)
    {
        if (axis is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(axis));
        if (!double.IsFinite(degrees)) return;
        lock (_updateSync)
        {
            if (_disposed) return;
            _pendingAngles[axis] = degrees;
            _pendingAxes[axis] = true;
            ScheduleApply();
        }
    }

    /// <summary>释放场景和原生视口；必须在 UI 线程调用。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        lock (_updateSync) _disposed = true;
        StopSmoothing();
        _viewerReady.TrySetCanceled();
        RenderView.Loaded -= OnViewportLoaded;
        RenderView.Unloaded -= OnViewportUnloaded;
        RenderView.IsVisibleChanged -= OnViewportVisibilityChanged;
        if (_ownerWindow is not null) _ownerWindow.Closed -= OnOwnerClosed;
        _ownerWindow = null;
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
            if (Dispatcher.CheckAccess()) ApplyLiveOptions(model.Options);
            else Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ReferenceEquals(model, _boundModel)) ApplyLiveOptions(model.Options);
            }));
        }
    }

    /// <summary>实时应用画质、相机自转、坐标箭头和焊接光显示。</summary>
    private void ApplyLiveOptions(RobotViewerOptions? options)
    {
        if (_disposed || options is null) return;
        ApplyRenderQuality(options.RenderQuality);
        if (!_loaded || _scene is null)
        {
            // 加载完成时会读取最新的显示设置，只有装配相关参数变化才需要再加载。
            if (_loadingOptions is { } loading
                && (loading.Robot, loading.Tool, loading.Work, loading.ToolTcp, loading.WorkPose)
                    == (options.Robot, options.Tool, options.Work, options.ToolTcp, options.WorkPose)) return;
            StartLoadIfReady();
            return;
        }
        // 只有环绕参数真的变化时才重置相机；焊接状态、坐标开关变化不能把自动旋转拉回起点。
        var orbit = (options.OrbitSpeedDegreesPerSecond, options.OrbitStartAngleDegrees, options.OrbitElevationDegrees);
        if (_appliedOrbit != orbit)
        {
            _scene.SetOrbit(orbit.OrbitSpeedDegreesPerSecond, orbit.OrbitStartAngleDegrees, orbit.OrbitElevationDegrees);
            _appliedOrbit = orbit;
        }
        _scene.SetCoordinateVisibility(
            options.ShowBaseCoordinates,
            options.ShowFlangeCoordinates,
            options.ShowTcpCoordinates);
        _scene.SetWeldBeamVisible(options.ShowWeldBeam);
    }

    /// <summary>切换画质会重建渲染缓冲区，隐藏时推迟到视口再次可见。</summary>
    private void ApplyRenderQuality(RenderQuality quality)
    {
        if (_disposed || !RenderView.IsVisible) return;
        _scene?.SetRenderQuality(quality);
    }

    /// <summary>保存绑定角度；场景加载完成后由 LoadAsync 统一应用。</summary>
    private void ApplyBoundJoints(IReadOnlyList<double>? joints)
    {
        if (joints is null || joints.Count < 6) return;
        SetJoints(joints);
    }

    // 加载流程

    /// <summary>渲染器和 Model.Options 都就绪时开始加载；已加载时先释放旧场景再按新参数重建。</summary>
    private void StartLoadIfReady()
    {
        if (_disposed || Model?.Options is null) return;
        if (!_viewerReady.Task.IsCompletedSuccessfully) return;
        if (_loading)
        {
            // 只有参数对象被替换才需要在本次加载结束后重建；渲染器就绪通知不算。
            if (!ReferenceEquals(Model?.Options, _loadingOptions)) _reloadPending = true;
            return;
        }
        if (_loaded) ResetForReload();
        _ = LoadCoreAsync();
    }

    /// <summary>释放当前场景并回到未加载状态；保留最近一帧角度，重载后继续使用。</summary>
    private void ResetForReload()
    {
        StopSmoothing();
        _scene?.Dispose();
        _scene = null;
        _joints = [];
        lock (_updateSync)
        {
            _loaded = false;
            _hasPendingAngles = false;
            _updateQueued = false;
            _hasAppliedAngles = false;
        }
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

    /// <summary>首次进入可视树时加载场景；标签页返回时继续使用原场景。</summary>
    private void OnViewerReady()
    {
        if (_viewerReady.TrySetResult(true)) StartLoadIfReady();
    }

    // 关节更新

    /// <summary>合并高频调用：已有排队时只更新待应用角度，不再重复排队。</summary>
    private void ScheduleApply()
    {
        lock (_updateSync)
        {
            _hasPendingAngles = true;
            if (_disposed || !_loaded || _updatesSuspended || _updateQueued) return;
            _updateQueued = true;
        }
        // 仿真应用只是显示，排到后台优先级，让通信回执、输入和渲染先走。
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(ApplyPendingAngles));
    }

    /// <summary>在 UI 线程取最新一帧角度；首次直接贴合，之后按渲染帧平滑逼近。</summary>
    private void ApplyPendingAngles()
    {
        lock (_updateSync)
        {
            _updateQueued = false;
            if (_disposed || !_loaded || _updatesSuspended || !_hasPendingAngles) return;
            _hasPendingAngles = false;
        }

        // 首次应用或关闭平滑时直接贴合，避免加载瞬间从零位滑入。
        var options = Model?.Options;
        if (!_hasAppliedAngles
            || options is null
            || options.JointResponseFrequencyHz <= 0
            || options.JointMaxSpeedDegreesPerSecond <= 0)
        {
            double[] angles;
            lock (_updateSync) angles = CaptureJointTargetsLocked();
            Array.Copy(angles, _displayAngles, _displayAngles.Length);
            Array.Clear(_displayVelocities, 0, _displayVelocities.Length);
            _hasAppliedAngles = true;
            ApplyAngles(_displayAngles);
            return;
        }

        StartSmoothing();
    }

    /// <summary>开始按 WPF 渲染帧逼近最新目标；已有循环时不重复订阅。</summary>
    private void StartSmoothing()
    {
        if (_smoothing || _disposed || _updatesSuspended) return;
        _smoothing = true;
        _lastRenderTime = null;
        _lastApplyTime = TimeSpan.Zero;
        CompositionTarget.Rendering += OnSmoothingFrame;
    }

    /// <summary>停止渲染帧订阅，避免静止时持续更新场景。</summary>
    private void StopSmoothing()
    {
        if (!_smoothing) return;
        _smoothing = false;
        _lastRenderTime = null;
        CompositionTarget.Rendering -= OnSmoothingFrame;
    }

    /// <summary>每个渲染帧把显示角度向最新目标逼近；到达后停止。</summary>
    private void OnSmoothingFrame(object? sender, EventArgs e)
    {
        if (_disposed || _scene is null || _joints.Count != 6)
        {
            StopSmoothing();
            return;
        }
        if (e is not RenderingEventArgs renderArgs) return;
        if (_lastRenderTime is null)
        {
            _lastRenderTime = renderArgs.RenderingTime;
            return;
        }

        var deltaSeconds = (renderArgs.RenderingTime - _lastRenderTime.Value).TotalSeconds;
        _lastRenderTime = renderArgs.RenderingTime;
        if (deltaSeconds <= 0) return;
        // 窗口卡顿或断线恢复时限制单帧步长，避免一帧跳很远。
        deltaSeconds = Math.Min(deltaSeconds, 0.05);

        double[] target;
        lock (_updateSync) target = CaptureJointTargetsLocked();

        var options = Model?.Options;
        var response = options?.JointResponseFrequencyHz ?? 0;
        var maxSpeed = Math.Max(options?.JointMaxSpeedDegreesPerSecond ?? 0, 0);
        var maxAcceleration = Math.Max(options?.JointMaxAccelerationDegreesPerSecondSquared ?? 1e-3, 1e-3);
        var tolerance = Math.Max(JointPositionToleranceDegrees, 0);

        var settled = true;
        for (var index = 0; index < _displayAngles.Length; index++)
        {
            JointMotion.Step(ref _displayAngles[index], ref _displayVelocities[index], target[index],
                deltaSeconds, response, maxSpeed, maxAcceleration, tolerance);
            var joint = _joints[index];
            var angle = Math.Clamp(_displayAngles[index], joint.Minimum, joint.Maximum);
            if (angle != _displayAngles[index]) _displayVelocities[index] = 0;
            _displayAngles[index] = angle;
            if (Math.Abs(target[index] - _displayAngles[index]) > tolerance
                || Math.Abs(_displayVelocities[index]) > 0.5)
                settled = false;
        }

        // 插值每帧都算，但三维位置按 JointApplyRateHz 限速写入：
        // 减少视口刷新次数，给节拍等界面绑定留出时间。
        var applyRate = options?.JointApplyRateHz ?? 0;
        var applyDue = applyRate <= 0
            || (renderArgs.RenderingTime - _lastApplyTime).TotalSeconds >= 1.0 / applyRate;
        if (applyDue || settled)
        {
            _lastApplyTime = renderArgs.RenderingTime;
            ApplyAngles(_displayAngles);
        }
        if (settled) StopSmoothing();
    }

    /// <summary>持锁读取目标，并在插值前限制到当前选中机型的关节行程。</summary>
    private double[] CaptureJointTargetsLocked()
    {
        var angles = (double[])_pendingAngles.Clone();
        for (var index = 0; index < angles.Length; index++)
            angles[index] = Math.Clamp(angles[index], _joints[index].Minimum, _joints[index].Maximum);
        return angles;
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
