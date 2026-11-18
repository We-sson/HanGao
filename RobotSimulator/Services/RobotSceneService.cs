using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
    const double OrbitApplyInterval = 1.0 / 30.0;
    const double CameraResumeDelay = 0.25;
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
    bool _userAdjusting;
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
    (double Ox, double Oy, double Oz, double Ux, double Uy, double Uz)? _pendingEye;

    public RobotSceneService(ViewportPanel panel)
    {
        _panel = panel ?? throw new ArgumentNullException(nameof(panel));
    }

    public bool IsUserAdjustingView => _userAdjusting || (_orbitActive && _orbitPaused);

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
        RequestRedraw();
    }

    public void ApplyJoint(JointViewModel joint)
    {
        ArgumentNullException.ThrowIfNull(joint);
        if (_content is null || _joints is null) return;
        _content.SetJoints(_joints);
        RequestRedraw();
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

        var (ox, oy, oz, ux, uy, uz) = view switch
        {
            RobotView.Front => (0.0, -1.0, 0.0, 0.0, 0.0, 1.0),
            RobotView.Top => (0.0, 0.0, 1.0, 0.0, 1.0, 0.0),
            _ => (1.0, -1.0, 1.0, 0.0, 0.0, 1.0),
        };
        ApplyEyeDirection(ox, oy, oz, ux, uy, uz);
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

    /// <summary>
    /// Macad 视口使用原生 <see cref="IViewportMouseControl"/>，不会回调本项目；
    /// 因此每帧比较相机状态，识别用户旋转/平移/缩放，用于暂停自动环绕与示例动作。
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
            if (_pendingZoomAll) ZoomAll();
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
            _pendingZoomAll = false;
            return;
        }
        ApplyEyeDirectionCore(view, ox, oy, oz, ux, uy, uz);
    }

    void ApplyEyeDirectionCore(V3d_View view, double ox, double oy, double oz, double ux, double uy, double uz)
    {
        var length = Math.Sqrt(ox * ox + oy * oy + oz * oz);
        if (length <= 1e-12) return;

        var box = _content?.GetModelBounds();
        var center = box is not null && !box.IsVoid() ? BoxCenter(box) : Pnt.Origin;
        var distance = box is not null && !box.IsVoid() ? Math.Max(10.0, BoxDiagonal(box)) : 2000.0;

        var direction = new Vec(ox / length, oy / length, oz / length);
        var eye = center.Translated(direction.Multiplied(distance));

        var camera = view.Camera();
        camera.SetEyeAndCenter(eye, center);
        camera.SetUp(new Dir(ux, uy, uz));
        camera.SetProjectionType(Graphic3d_Camera.Projection.Orthographic);
        view.SetCamera(camera);
        view.FitAll(FitMargin, false);
        view.ZFitAll();
        _orbitCenter = center;
        UpdateCoordinateScale();
        view.Redraw();
        RememberCamera(view);
    }

    void ApplyOrbitPose()
    {
        var view = ReadyView;
        if (view is null)
        {
            _pendingEye = null;
            _pendingZoomAll = false;
            return;
        }

        var box = _content?.GetModelBounds();
        var center = box is not null && !box.IsVoid() ? BoxCenter(box) : Pnt.Origin;
        var distance = box is not null && !box.IsVoid() ? Math.Max(10.0, BoxDiagonal(box)) : 2000.0;

        var azimuth = _orbitStartAngle * Math.PI / 180.0;
        var elevation = Math.Clamp(_orbitElevation, -85.0, 85.0) * Math.PI / 180.0;
        var eye = new Pnt(
            center.X + Math.Cos(elevation) * Math.Cos(azimuth) * distance,
            center.Y + Math.Cos(elevation) * Math.Sin(azimuth) * distance,
            center.Z + Math.Sin(elevation) * distance);

        var camera = view.Camera();
        camera.SetEyeAndCenter(eye, center);
        camera.SetUp(new Dir(0, 0, 1));
        camera.SetProjectionType(Graphic3d_Camera.Projection.Orthographic);
        view.SetCamera(camera);
        view.FitAll(FitMargin, false);
        view.ZFitAll();
        _orbitCenter = center;
        UpdateCoordinateScale();
        view.Redraw();
        RememberCamera(view);
    }

    void TickOrbit(V3d_View view)
    {
        if (!_orbitActive || _orbitSpeed == 0) return;

        var now = Stopwatch.GetTimestamp();
        if (_orbitPaused)
        {
            var idle = (now - _lastInteraction) / (double)Stopwatch.Frequency;
            if (idle < CameraResumeDelay) return;
            _orbitPaused = false;
        }

        if (_lastOrbitFrame == 0)
        {
            _lastOrbitFrame = now;
            return;
        }

        var delta = (now - _lastOrbitFrame) / (double)Stopwatch.Frequency;
        _lastOrbitFrame = now;
        _orbitElapsed += Math.Clamp(delta, 0.0, 0.05);
        if (_orbitElapsed < OrbitApplyInterval) return;

        var degrees = _orbitSpeed * _orbitElapsed;
        _orbitElapsed = 0;
        var controller = InteractiveContext.Current?.ViewportController;
        if (controller is null) return;
        controller.Rotate(degrees, 0, 0);
        RememberCamera(view);
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
    }
}
