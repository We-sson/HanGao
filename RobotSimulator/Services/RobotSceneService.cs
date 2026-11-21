using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Macad.Interaction;
using Macad.Interaction.Panels;
using Macad.Occt;
using RobotSimulator.Models;
using RobotSimulator.ViewModels;

namespace RobotSimulator.Services;

/// <summary>加载进度：Completed/Total 为已处理部件数，Message 为当前部件说明。</summary>
public readonly record struct RobotLoadProgress(int Completed, int Total, string Message)
{
    public double Percent => Total == 0 ? 0 : Completed * 100.0 / Total;
}

/// <summary>
/// Macad（OpenCascade AIS）渲染栈的三维场景服务：相机、环绕动画、坐标轴、Tool/Work、焊接光。
/// 一个实例对应一个 <see cref="ViewportPanel"/>，Tab 切换后视口窗口会重建，
/// 因此不缓存 <c>V3d_View</c>，每次从 <see cref="InteractiveContext.Current"/> 现取。
/// </summary>
public sealed class RobotSceneService : IRobotSceneService
{
    const double OrbitApplyInterval = 1.0 / 24.0;
    const double MinRedrawCooldown = 0.016;
    const double CameraResumeDelay = 2.0;
    const double AxisBaseLength = 64.0;
    const double AxisFlangeLength = 48.0;
    const double AxisTcpLength = 64.0;
    const double FitMargin = 0.08;

    readonly ViewportPanel _panel;
    RobotSceneContent? _content;
    IReadOnlyList<JointViewModel>? _joints;
    V3d_View? _configuredView;
    RenderQuality _quality = RenderQuality.High;
    bool _eventsBound;
    bool _disposed;

    bool _orbitActive;
    bool _orbitPaused;
    bool _pointerHeld;
    bool _userAdjusting;
    bool _lastReportedAdjusting;
    double _orbitSpeed;
    double _orbitStartAngle;
    double _orbitElevation;
    double _orbitElapsed;
    long _lastOrbitFrame;
    long _lastInteraction;
    Pnt _orbitCenter = Pnt.Origin;

    bool _cameraTracked;
    Pnt _cameraEye;
    Pnt _cameraCenter;
    double _cameraScale;
    long _lastCameraChange;

    bool _pendingZoomAll;
    bool _pendingOrbitPose;
    bool _sceneDirty;
    long _lastRedrawTimestamp;
    (double Ox, double Oy, double Oz, double Ux, double Uy, double Uz)? _pendingEye;

    public RobotSceneService(ViewportPanel panel)
    {
        _panel = panel ?? throw new ArgumentNullException(nameof(panel));
    }

    public bool IsUserAdjustingView => _pointerHeld || _userAdjusting || (_orbitActive && _orbitPaused);

    internal event Action? AdjustingStateChanged;

    static V3d_View? ReadyView
    {
        get
        {
            try
            {
                var view = InteractiveContext.Current?.ViewportController?.V3dView;
                if (view is null) return null;
                var window = view.Window();
                return window is not null && window.IsMapped() ? view : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    static AIS_InteractiveContext? AisContext
    {
        get
        {
            try
            {
                return InteractiveContext.Current?.WorkspaceController?.AisContext;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public void InitializeViewport()
    {
        MacadViewportBootstrap.EnsureContext();
        var context = InteractiveContext.Current;
        var workspace = context.WorkspaceController
            ?? throw new InvalidOperationException("Macad 工作区尚未就绪。");

        // 只读仿真视口使用项目约定的手势：左键绕模型中心 Z-up 转盘旋转、右键平移、滚轮缩放，不启用 Macad 的框选/选择。
        _panel.MouseControl = new RobotViewportMouseControl(OnPointerInteraction, OrbitByPointer)
        {
            ViewportController = context.ViewportController,
        };

        workspace.V3dViewer.SetDefaultLights();
        workspace.V3dViewer.SetLightOn();
        workspace.AisContext.SetAutoActivateSelection(false);

        if (!_eventsBound)
        {
            _panel.IsVisibleChanged += OnPanelVisibilityChanged;
            _panel.Loaded += OnPanelLoaded;
            CompositionTarget.Rendering += OnRendering;
            _eventsBound = true;
        }
    }

    void OnPanelVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        if (!_panel.IsVisible) return;
        _configuredView = null;
        _pendingZoomAll = true;
        _cameraTracked = false;
        _userAdjusting = false;
    }

    void OnPanelLoaded(object sender, RoutedEventArgs args)
    {
        _configuredView = null;
        _pendingZoomAll = true;
        _cameraTracked = false;
        _userAdjusting = false;
    }

    public async Task LoadRobotAsync(
        RobotConfiguration configuration,
        string modelId,
        IProgress<RobotLoadProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        InitializeViewport();

        var parts = await Task.Run(() => RobotSceneContent.LoadParts(configuration, modelId, progress));
        ObjectDisposedException.ThrowIf(_disposed, this);

        var context = AisContext ?? throw new InvalidOperationException("Macad 交互上下文尚未就绪。");
        Attach(RobotSceneContent.Create(context, parts, configuration));
    }

    public async Task LoadToolAsync(
        string toolId,
        ToolConfiguration configuration,
        IProgress<RobotLoadProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        InitializeViewport();

        var owner = _content ?? throw new InvalidOperationException("请先加载机器人。");
        progress?.Report(new RobotLoadProgress(0, 1, $"正在读取 Tool：{configuration.ToolName}"));

        var part = await Task.Run(() =>
        {
            using var source = ModelResources.OpenTool(toolId, configuration.ModelFile);
            return CadModelPart.Load("Tool", source);
        });

        if (_disposed || !ReferenceEquals(owner, _content))
            throw new ObjectDisposedException(nameof(RobotSceneService));

        owner.ReplaceTool(part);
        owner.SetToolTcp(configuration.Tcp);
        ZoomAll();
        progress?.Report(new RobotLoadProgress(1, 1, part.LoadMessage));
    }

    public async Task LoadWorkAsync(
        string workId,
        WorkConfiguration configuration,
        IProgress<RobotLoadProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        InitializeViewport();

        var owner = _content ?? throw new InvalidOperationException("请先加载机器人。");
        progress?.Report(new RobotLoadProgress(0, 1, $"正在读取 Work：{configuration.WorkName}"));

        var part = await Task.Run(() =>
        {
            using var source = ModelResources.OpenWork(workId, configuration.ModelFile);
            return CadModelPart.Load("Work", source);
        });

        if (_disposed || !ReferenceEquals(owner, _content))
            throw new ObjectDisposedException(nameof(RobotSceneService));

        owner.ReplaceWork(part);
        owner.SetWorkPose(configuration.Pose);
        ZoomAll();
        progress?.Report(new RobotLoadProgress(1, 1, part.LoadMessage));
    }

    void Attach(RobotSceneContent content)
    {
        InitializeViewport();
        var previous = _content;
        if (ReferenceEquals(previous, content)) return;

        previous?.AdoptAttachmentsTo(content);
        _content = content;
        if (previous is not null) previous.Dispose();
        ZoomAll();
    }

    public void ApplyAll(IEnumerable<JointViewModel> joints)
    {
        ArgumentNullException.ThrowIfNull(joints);
        _joints = joints as IReadOnlyList<JointViewModel> ?? joints.ToList();
        _content?.SetJoints(_joints);
        // 合并到渲染节拍统一刷新，避免与 TickOrbit 同帧重复触发两遍三维重绘而拖慢上位机 UI 绑定。
        _sceneDirty = true;
    }

    public void ApplyJoint(JointViewModel joint)
    {
        ArgumentNullException.ThrowIfNull(joint);
        if (_content is null || _joints is null) return;
        _content.SetJoints(_joints);
        _sceneDirty = true;
    }

    public void SetCoordinateVisibility(bool showBaseAxes, bool showFlangeAxes, bool showTcpAxes)
    {
        _content?.SetCoordinateVisibility(showBaseAxes, showFlangeAxes, showTcpAxes);
        RequestRedraw();
    }

    public void SetWeldBeamVisible(bool visible)
    {
        _content?.SetWeldBeamVisible(visible);
        RequestRedraw();
    }

    public void SetToolTcp(PoseConfiguration pose)
    {
        _content?.SetToolTcp(pose);
        RequestRedraw();
    }

    public void SetWorkPose(PoseConfiguration pose)
    {
        _content?.SetWorkPose(pose);
        RequestRedraw();
    }

    public void SetView(RobotView view)
    {
        _orbitSpeed = 0;
        _orbitActive = false;
        _orbitPaused = false;
        _orbitElapsed = 0;
        _pendingOrbitPose = false;

        var (ox, oy, oz, ux, uy, uz) = view switch
        {
            RobotView.Front => (0.0, -1.0, 0.0, 0.0, 0.0, 1.0),
            RobotView.Top => (0.0, 0.0, 1.0, 0.0, 1.0, 0.0),
            _ => (1.0, -1.0, 1.0, 0.0, 0.0, 1.0),
        };
        ApplyEyeDirection(ox, oy, oz, ux, uy, uz);
        NotifyAdjustingStateIfChanged();
    }

    public void ZoomAll()
    {
        _pendingZoomAll = true;
        if (ReadyView is null) return;
        _pendingZoomAll = false;

        var view = ReadyView!;
        var box = _content?.GetModelBounds();
        if (box is not null && !box.IsVoid())
        {
            view.FitAll(box, FitMargin, false);
            _orbitCenter = BoxCenter(box);
        }
        else
        {
            view.FitAll(FitMargin, false);
            _orbitCenter = Pnt.Origin;
        }
        view.ZFitAll();
        UpdateCoordinateScale();
        view.Redraw();
        RememberCamera(view);
    }

    public void SetOrbit(double speedDegreesPerSecond, double startAngleDegrees, double elevationDegrees)
    {
        if (!double.IsFinite(speedDegreesPerSecond)
            || !double.IsFinite(startAngleDegrees)
            || !double.IsFinite(elevationDegrees))
            throw new ArgumentException("相机环绕参数必须为有限数值。");

        _orbitSpeed = speedDegreesPerSecond;
        _orbitStartAngle = startAngleDegrees;
        _orbitElevation = elevationDegrees;
        _orbitElapsed = 0;
        _lastOrbitFrame = 0;
        _orbitPaused = false;
        _userAdjusting = false;
        _orbitActive = speedDegreesPerSecond != 0;
        ApplyOrbitPose();
        NotifyAdjustingStateIfChanged();
    }

    public void SetRenderQuality(RenderQuality quality)
    {
        _quality = quality;
        var view = ReadyView;
        if (view is null) return;
        ApplyQuality(view, quality);
        view.Redraw();
    }

    // ── 内部：原生鼠标操作检测 ─────────────────────────────────────────

    void OnPointerInteraction(bool pointerHeld)
    {
        var now = Stopwatch.GetTimestamp();
        _pointerHeld = pointerHeld;
        _orbitPaused = true;
        _orbitElapsed = 0;
        _lastInteraction = now;
        _lastOrbitFrame = now;
        _userAdjusting = true;
        _lastCameraChange = now;
        NotifyAdjustingStateIfChanged();
    }

    void NotifyAdjustingStateIfChanged()
    {
        var current = IsUserAdjustingView;
        if (current == _lastReportedAdjusting) return;
        _lastReportedAdjusting = current;
        AdjustingStateChanged?.Invoke();
    }

    static bool IsAnyMouseButtonPressed()
        => Mouse.LeftButton == MouseButtonState.Pressed
           || Mouse.RightButton == MouseButtonState.Pressed
           || Mouse.MiddleButton == MouseButtonState.Pressed;

    /// <summary>
    /// 在按键交互之外继续比对相机状态，兜底覆盖非鼠标控制路径导致的视角变化。
    /// </summary>
    bool DetectUserCameraChange(V3d_View view)
    {
        if (!_cameraTracked)
        {
            RememberCamera(view);
            return false;
        }

        var camera = view.Camera();
        var now = Stopwatch.GetTimestamp();
        if (!SamePoint(camera.Eye(), _cameraEye)
            || !SamePoint(camera.Center(), _cameraCenter)
            || Math.Abs(camera.Scale() - _cameraScale) > Math.Max(1e-9, Math.Abs(_cameraScale) * 1e-9))
        {
            _userAdjusting = true;
            _lastCameraChange = now;
            RememberCamera(view);
            return true;
        }

        if (_userAdjusting
            && (now - _lastCameraChange) / (double)Stopwatch.Frequency > CameraResumeDelay)
            _userAdjusting = false;
        return false;
    }

    void RememberCamera(V3d_View view)
    {
        var camera = view.Camera();
        _cameraEye = camera.Eye();
        _cameraCenter = camera.Center();
        _cameraScale = camera.Scale();
        _cameraTracked = true;
    }

    static bool SamePoint(Pnt a, Pnt b)
        => Math.Abs(a.X - b.X) <= 1e-6
           && Math.Abs(a.Y - b.Y) <= 1e-6
           && Math.Abs(a.Z - b.Z) <= 1e-6;

    // ── 视口/相机 ─────────────────────────────────────────────────────

    void OnRendering(object? sender, EventArgs args)
    {
        if (_disposed || !_panel.IsVisible) return;

        var view = ReadyView;
        if (view is null) return;

        if (!ReferenceEquals(view, _configuredView))
        {
            ApplyViewSettings(view);
            _configuredView = view;
            if (_pendingEye is { } eye)
            {
                _pendingEye = null;
                ApplyEyeDirectionCore(view, eye.Ox, eye.Oy, eye.Oz, eye.Ux, eye.Uy, eye.Uz);
            }
            else if (_pendingOrbitPose)
            {
                ApplyOrbitPose();
            }
            if (_pendingZoomAll) ZoomAll();
        }

        if (_pointerHeld && !IsAnyMouseButtonPressed())
        {
            _pointerHeld = false;
            var releaseNow = Stopwatch.GetTimestamp();
            _lastInteraction = releaseNow;
            _lastCameraChange = releaseNow;
        }

        if (DetectUserCameraChange(view))
        {
            _orbitPaused = true;
            _orbitElapsed = 0;
            _lastInteraction = Stopwatch.GetTimestamp();
            UpdateCoordinateScale();
            RequestRedraw();
        }

        TickOrbit(view);
        if (_sceneDirty)
        {
            var now = Stopwatch.GetTimestamp();
            if ((now - _lastRedrawTimestamp) / (double)Stopwatch.Frequency >= OrbitApplyInterval)
            {
                _sceneDirty = false;
                RequestRedraw();
                _lastRedrawTimestamp = Stopwatch.GetTimestamp();
            }
        }
        NotifyAdjustingStateIfChanged();
    }

    void ApplyViewSettings(V3d_View view)
    {
        view.SetImmediateUpdate(false);
        ApplyQuality(view, _quality);
        view.SetBgGradientColors(
            CadColors.Srgb(151, 165, 186),
            CadColors.Srgb(213, 225, 243),
            Aspect_GradientFillMethod.Vertical);

        var camera = view.Camera();
        camera.SetProjectionType(Graphic3d_Camera.Projection.Orthographic);
        view.SetCamera(camera);
        view.Redraw();
        RememberCamera(view);
    }

    void ApplyEyeDirection(double ox, double oy, double oz, double ux, double uy, double uz)
    {
        var view = ReadyView;
        if (view is null)
        {
            _pendingEye = (ox, oy, oz, ux, uy, uz);
            _pendingOrbitPose = false;
            _pendingZoomAll = false;
            return;
        }
        ApplyEyeDirectionCore(view, ox, oy, oz, ux, uy, uz);
    }

    void ApplyEyeDirectionCore(V3d_View view, double ox, double oy, double oz, double ux, double uy, double uz)
    {
        var length = Math.Sqrt(ox * ox + oy * oy + oz * oz);
        if (length <= 1e-12) return;

        ApplyCameraPose(view, new Vec(ox / length, oy / length, oz / length), new Dir(ux, uy, uz), fitModelBox: false);
    }

    void ApplyOrbitPose()
    {
        var view = ReadyView;
        if (view is null)
        {
            _pendingOrbitPose = true;
            _pendingEye = null;
            _pendingZoomAll = false;
            return;
        }
        _pendingOrbitPose = false;
        _pendingZoomAll = false;

        var azimuth = _orbitStartAngle * Math.PI / 180.0;
        var elevation = Math.Clamp(_orbitElevation, -85.0, 85.0) * Math.PI / 180.0;
        var direction = new Vec(
            Math.Cos(elevation) * Math.Cos(azimuth),
            Math.Cos(elevation) * Math.Sin(azimuth),
            Math.Sin(elevation));
        ApplyCameraPose(view, direction, new Dir(0, 0, 1), fitModelBox: true);
    }

    /// <summary>按给定单位观测方向与上方向定位正交相机并适配模型包围盒。</summary>
    void ApplyCameraPose(V3d_View view, Vec unitOffsetDirection, Dir up, bool fitModelBox)
    {
        var box = _content?.GetModelBounds();
        var center = box is not null && !box.IsVoid() ? BoxCenter(box) : Pnt.Origin;
        var distance = box is not null && !box.IsVoid() ? Math.Max(10.0, BoxDiagonal(box)) : 2000.0;
        var eye = center.Translated(unitOffsetDirection.Multiplied(distance));

        var camera = view.Camera();
        camera.SetEyeAndCenter(eye, center);
        camera.SetUp(up);
        camera.SetProjectionType(Graphic3d_Camera.Projection.Orthographic);
        view.SetCamera(camera);
        if (fitModelBox && box is not null && !box.IsVoid()) view.FitAll(box, FitMargin, false);
        else view.FitAll(FitMargin, false);
        view.ZFitAll();
        _orbitCenter = center;
        UpdateCoordinateScale();
        _sceneDirty = false;
        view.Redraw();
        _lastRedrawTimestamp = Stopwatch.GetTimestamp();
        RememberCamera(view);
    }

    void TickOrbit(V3d_View view)
    {
        if (!_orbitActive || _orbitSpeed == 0) return;

        var now = Stopwatch.GetTimestamp();
        if (_pointerHeld)
        {
            _lastInteraction = now;
            _lastOrbitFrame = now;
            _orbitElapsed = 0;
            return;
        }
        if (_orbitPaused)
        {
            var idle = (now - _lastInteraction) / (double)Stopwatch.Frequency;
            if (idle < CameraResumeDelay)
            {
                _lastOrbitFrame = now;
                return;
            }
            _orbitPaused = false;
            _lastOrbitFrame = now;
            _orbitElapsed = 0;
            return;
        }

        if (_lastOrbitFrame == 0)
        {
            _lastOrbitFrame = now;
            return;
        }

        var delta = (now - _lastOrbitFrame) / (double)Stopwatch.Frequency;
        _lastOrbitFrame = now;
        _orbitElapsed += Math.Clamp(delta, 0.0, 0.1);
        if (_orbitElapsed < OrbitApplyInterval) return;
        if (_lastRedrawTimestamp != 0
            && (now - _lastRedrawTimestamp) / (double)Stopwatch.Frequency < MinRedrawCooldown)
            return;

        var degrees = _orbitSpeed * _orbitElapsed;
        _orbitElapsed = 0;
        if (!double.IsFinite(degrees) || Math.Abs(degrees) <= 1e-9) return;

        var camera = view.Camera();
        var center = camera.Center();
        var eye = camera.Eye();
        var dx = eye.X - center.X;
        var dy = eye.Y - center.Y;
        var dz = eye.Z - center.Z;
        if (dx * dx + dy * dy <= Math.Max(1e-6, dz * dz * 1e-4)) return;

        ApplyCameraOrbit(view, degrees, 0.0);
    }

    internal void OrbitByPointer(double yawDegrees, double pitchDegrees)
    {
        var view = ReadyView;
        if (view is null) return;
        ApplyCameraOrbit(view, yawDegrees, pitchDegrees);
    }

    /// <summary>绕 <see cref="_orbitCenter"/> 旋转相机并刷新像素比例、重绘与冷却时间戳，供鼠标拖拽和自动环绕共用。</summary>
    void ApplyCameraOrbit(V3d_View view, double yawDegrees, double pitchDegrees)
    {
        var camera = view.Camera();
        var (nextEye, nextCenter, nextUp) = OrbitCameraAroundPivot(
            camera.Eye(),
            camera.Center(),
            camera.Up(),
            _orbitCenter,
            yawDegrees,
            pitchDegrees);

        camera.SetEyeAndCenter(nextEye, nextCenter);
        camera.SetUp(nextUp);
        view.SetCamera(camera);
        UpdateCoordinateScale();
        _sceneDirty = false;
        view.Redraw();
        // 在 Redraw 完成之后记录冷却起点，确保即使低配工控机单帧较慢，UI 消息队列与生产数据绑定也能喘息执行。
        _lastRedrawTimestamp = Stopwatch.GetTimestamp();
        RememberCamera(view);
    }

    /// <summary>
    /// 绕 <paramref name="pivot"/> 执行 Z-up 转盘旋转（先沿水平右轴俯仰并限制仰角在 ±89°，再绕世界 Z 轴偏航，最后消除滚转）。
    /// </summary>
    internal static (Pnt Eye, Pnt Center, Dir Up) OrbitCameraAroundPivot(
        Pnt eye,
        Pnt center,
        Dir currentUp,
        Pnt pivot,
        double yawDegrees,
        double pitchDegrees)
    {
        const double maxElevationRadians = 89.0 * Math.PI / 180.0;
        if (!double.IsFinite(yawDegrees) || !double.IsFinite(pitchDegrees))
            return (eye, center, currentUp);

        var backVec = new Vec(center, eye);
        if (backVec.Magnitude() <= 1e-9)
            return (eye, center, currentUp);

        var backDir = backVec.ToDir();
        var viewDir = backDir.Reversed();

        // 从视角方向与世界 +Z 计算水平屏幕右轴；正顶/正底视角的水平分量为零，改用 viewDir × currentUp。
        Dir rightDir;
        var rx = viewDir.Y;
        var ry = -viewDir.X;
        if (Math.Sqrt(rx * rx + ry * ry) > 1e-6)
        {
            rightDir = new Dir(rx, ry, 0);
        }
        else
        {
            var crossVec = viewDir.ToVec().Crossed(currentUp.ToVec());
            rightDir = crossVec.Magnitude() > 1e-6 ? crossVec.ToDir() : Dir.DX;
        }

        var elevation = Math.Atan2(backDir.Z, Math.Sqrt(backDir.X * backDir.X + backDir.Y * backDir.Y));
        var pitchRad = pitchDegrees * Math.PI / 180.0;
        double appliedPitchRad;
        if (pitchRad < 0)
            appliedPitchRad = elevation >= maxElevationRadians ? 0.0 : elevation - Math.Min(elevation - pitchRad, maxElevationRadians);
        else if (pitchRad > 0)
            appliedPitchRad = elevation <= -maxElevationRadians ? 0.0 : elevation - Math.Max(elevation - pitchRad, -maxElevationRadians);
        else
            appliedPitchRad = 0.0;

        var nextEye = eye;
        var nextCenter = center;
        var nextUp = currentUp;

        if (Math.Abs(appliedPitchRad) > 1e-12)
        {
            var pitchTrsf = new Trsf(new Ax1(pivot, rightDir), appliedPitchRad);
            nextEye = nextEye.Transformed(pitchTrsf);
            nextCenter = nextCenter.Transformed(pitchTrsf);
            nextUp = nextUp.Transformed(pitchTrsf);
        }

        var yawRad = yawDegrees * Math.PI / 180.0;
        if (Math.Abs(yawRad) > 1e-12)
        {
            var yawTrsf = new Trsf(new Ax1(pivot, Dir.DZ), yawRad);
            nextEye = nextEye.Transformed(yawTrsf);
            nextCenter = nextCenter.Transformed(yawTrsf);
            nextUp = nextUp.Transformed(yawTrsf);
        }

        var nextViewVec = new Vec(nextEye, nextCenter);
        if (nextViewVec.Magnitude() <= 1e-9)
            return (eye, center, currentUp);

        var nextViewDir = nextViewVec.ToDir();
        if (Math.Sqrt(nextViewDir.Y * nextViewDir.Y + nextViewDir.X * nextViewDir.X) > 1e-4)
        {
            var horizRight = new Vec(nextViewDir.Y, -nextViewDir.X, 0);
            var upVec = horizRight.Crossed(nextViewDir.ToVec());
            nextUp = upVec.Magnitude() > 1e-9 ? upVec.ToDir() : nextUp;
        }
        else
        {
            var nextUpVec = nextUp.ToVec();
            var viewUnit = nextViewDir.ToVec();
            var dot = nextUpVec.Dot(viewUnit);
            var ortho = new Vec(
                nextUpVec.X - dot * viewUnit.X,
                nextUpVec.Y - dot * viewUnit.Y,
                nextUpVec.Z - dot * viewUnit.Z);
            if (ortho.Magnitude() > 1e-9)
                nextUp = ortho.ToDir();
        }

        return (nextEye, nextCenter, nextUp);
    }

    // ── 坐标轴/像素尺寸 ────────────────────────────────────────────────

    void UpdateCoordinateScale()
    {
        var content = _content;
        if (content is null) return;
        var controller = InteractiveContext.Current?.ViewportController;
        if (controller is null) return;
        var pixelSize = controller.PixelSize;
        if (!double.IsFinite(pixelSize) || pixelSize <= 0) return;
        content.UpdateCoordinateScale(pixelSize);
    }

    void RequestRedraw()
    {
        if (_panel.IsVisible) InteractiveContext.Current?.WorkspaceController?.Invalidate();
    }

    /// <summary>
    /// 画质档同时决定多重采样数与渲染分辨率倍率：光栅化线条和模型轮廓在 4× MSAA 以上才接近
    /// SolidWorks 的平滑观感，因此最低档也保留 2×；极致档再叠加 1.5 倍超采样。
    /// </summary>
    static void ApplyQuality(V3d_View view, RenderQuality quality)
    {
        var parameters = view.ChangeRenderingParams();
        parameters.NbMsaaSamples = MsaaSamples(quality);
        parameters.RenderResolutionScale = quality == RenderQuality.Ultra ? 1.5f : 1.0f;
    }

    static int MsaaSamples(RenderQuality quality) => quality switch
    {
        RenderQuality.Performance => 2,
        RenderQuality.Balanced => 4,
        RenderQuality.High => 8,
        RenderQuality.Ultra => 8,
        _ => 4,
    };

    static Pnt BoxCenter(Bnd_Box box)
    {
        var min = box.CornerMin();
        var max = box.CornerMax();
        return new Pnt((min.X + max.X) / 2.0, (min.Y + max.Y) / 2.0, (min.Z + max.Z) / 2.0);
    }

    static double BoxDiagonal(Bnd_Box box)
    {
        var min = box.CornerMin();
        var max = box.CornerMax();
        var dx = max.X - min.X;
        var dy = max.Y - min.Y;
        var dz = max.Z - min.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_eventsBound)
        {
            _panel.IsVisibleChanged -= OnPanelVisibilityChanged;
            _panel.Loaded -= OnPanelLoaded;
            CompositionTarget.Rendering -= OnRendering;
            _eventsBound = false;
        }

        _content?.Dispose();
        _content = null;
        _joints = null;
        _configuredView = null;
        AdjustingStateChanged = null;
    }
}
