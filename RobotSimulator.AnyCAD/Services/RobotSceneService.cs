using System.Windows.Media;
using AnyCAD.Foundation;
using AnyCAD.WPF;
using RobotSimulator.Models;
using RobotSimulator.ViewModels;

namespace RobotSimulator.Services;

/// <summary>一次部件读取的进度快照，由后台读取器上报给界面。</summary>
/// <param name="Completed">已完成的部件数量，不是面数量。</param>
/// <param name="Total">当前加载阶段的部件数：机器人阶段为七个部件，工具阶段为一个。</param>
/// <param name="Message">当前读取步骤或缓存命中说明。</param>
/// <param name="FromCache">当前完成的部件是否从 BREP 缓存读取。</param>
public readonly record struct RobotLoadProgress(int Completed, int Total, string Message, bool FromCache)
{
    /// <summary>提供给界面进度条的百分比；没有部件时返回 0，避免除零。</summary>
    public double Percent => Total == 0 ? 0 : Completed * 100.0 / Total;
}

/// <summary>将 ViewModel 的场景操作转换为 AnyCAD 控件调用，管理后台加载、相机和显示刷新。</summary>
public sealed class RobotSceneService : IRobotSceneService
{
    /// <summary>当前窗口的 AnyCAD WPF 渲染控件，场景显示和相机操作都通过它完成。</summary>
    private readonly RenderControl _renderControl;
    /// <summary>当前已挂接的机器人场景；切换或重载时替换并释放旧场景。</summary>
    private RobotSceneContent? _content;
    /// <summary>视口背景的原生对象，在服务释放时一起释放。</summary>
    private Background? _background;
    /// <summary>避免每次重载都重复创建背景和设置视图。</summary>
    private bool _viewportInitialized;
    /// <summary>服务释放标记，阻止后台任务结束后再向已关闭的视图挂接场景。</summary>
    private bool _disposed;
    /// <summary>用户正在用鼠标调整视角时暂停自转，避免自动相机覆盖手动操作。</summary>
    private bool _orbitPaused;
    /// <summary>环绕速度，单位度/秒；0 表示不旋转。</summary>
    private double _orbitSpeed;
    /// <summary>当前环绕方位角，单位度。</summary>
    private double _orbitAzimuth = -45;
    /// <summary>环绕仰角，单位度；35.264 对应标准等轴测视角。</summary>
    private double _orbitElevation = 35.264;
    /// <summary>按模型包围盒适配后的相机距离，环绕时保持该距离。</summary>
    private double _orbitDistance;
    /// <summary>环绕中心缓存的模型包围盒中心，避免每帧重新遍历场景。</summary>
    private double _orbitCenterX;
    private double _orbitCenterY;
    private double _orbitCenterZ;
    /// <summary>各部件包围盒角点缓存，用于计算旋转一圈的最大投影范围。</summary>
    private readonly List<(double X, double Y, double Z)> _fitCorners = [];
    /// <summary>是否已订阅 WPF 渲染帧；与 AnyCAD 控件内部使用同一渲染事件。</summary>
    private bool _frameSubscribed;
    /// <summary>上一渲染帧时间，单位秒。</summary>
    private double? _lastFrameSeconds;
    /// <summary>最近一次原生相机变化时间，单位秒；用于判断用户是否仍在调整视角。</summary>
    private double _lastCameraChangeSeconds;
    /// <summary>用户停止操作视角后恢复自转的等待时间，单位秒。</summary>
    private const double CameraResumeDelaySeconds = 0.25;
    /// <summary>原生标准视角动画结束后重新适配窗口的等待时间，单位秒。</summary>
    private const double ViewFitDelaySeconds = 0.15;
    /// <summary>程序主动修改相机时忽略原生 CameraChanged 通知。</summary>
    private bool _suppressCameraChanged;
    /// <summary>AnyCAD 原生相机变化监听器；字段持有实例，避免原生回调指向已回收对象。</summary>
    private CameraChangeListener? _cameraListener;
    /// <summary>等待原生标准视角动画结束后，按最终方向重新适配窗口。</summary>
    private bool _pendingViewFit;
    /// <summary>标准视角动画期间请求的环绕参数，动画结束后再应用，避免两套相机动画互相覆盖。</summary>
    private (double Speed, double Start, double Elevation)? _pendingOrbit;
    /// <summary>最近一次程序主动设置相机后的眼点、观察点和上方向。</summary>
    private readonly double[] _expectedCameraPose = new double[9];
    /// <summary>是否已经记录过程序设置的相机姿态。</summary>
    private bool _hasExpectedCameraPose;

    /// <summary>绑定当前窗口的渲染控件，此时尚不读取模型或访问未就绪的原生视口。</summary>
    public RobotSceneService(RenderControl renderControl)
    {
        _renderControl = renderControl;
    }

    /// <summary>已挂接完整六轴场景时为 true，不要求存在可选 Tool。</summary>
    public bool IsLoaded => _content is not null && _content.JointGroups.Count == 6;

    /// <summary>用户是否正在视口内调整视角；示例动作据此暂停，把渲染让给鼠标。</summary>
    public bool IsUserAdjustingView => _orbitPaused;

    /// <summary>设置浅蓝 CAD 背景、紧凑视角立方体并关闭网格，在模型显示前执行一次。</summary>
    public void InitializeViewport()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_viewportInitialized)
        {
            return;
        }

        _background = Background.Create(EnumBackgroundTheme.LightBlue);
        _renderControl.SceneManager.SetBackground(_background);
        // Axis/AxisAndCube 在当前 SDK 下会导致模型深度/裁切异常，Cube 显示正常。
        _renderControl.SetViewCube(EnumViewCoordinateType.Cube);
        _renderControl.ShowCoordinateGrid(false);
        _viewportInitialized = true;

        // 与 AnyCAD 控件内部相同的 WPF 渲染事件；不再另建 DispatcherTimer，
        // 避免原生视口接收鼠标时计时器被阻塞后累计时间造成画面停顿和突然跳动。
        CompositionTarget.Rendering += OnCompositionFrame;
        _frameSubscribed = true;

        // 使用 AnyCAD 原生 CameraChanged 事件识别鼠标旋转、平移和缩放，
        // 不再轮询全局按键与屏幕坐标。
        _cameraListener = new CameraChangeListener(this);
        using (var manager = AppEventListenerManager.Instance())
        {
            manager.Add(_cameraListener);
        }

        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
    }

    /// <summary>后台读取固定机器人参数对应的基座和六轴，完成后挂接场景。</summary>
    public async Task LoadRobotAsync(
        RobotConfiguration configuration,
        string modelId,
        IProgress<RobotLoadProgress>? progress = null)
    {
        var content = await Task.Run(() => RobotSceneContent.Load(
            configuration,
            modelId,
            state => progress?.Report(state)));

        // 如果加载结束前服务已经关闭，则直接释放新场景，不再触碰已销毁的渲染控件。
        if (_disposed)
        {
            content.Dispose();
            throw new ObjectDisposedException(nameof(RobotSceneService));
        }

        Attach(content);
    }

    /// <summary>后台读取独立工具，成功后再替换旧工具，保留机器人当前角度和所有部件。</summary>
    public async Task LoadToolAsync(string toolId, ToolConfiguration configuration, IProgress<RobotLoadProgress>? progress = null)
    {
        ToolConfigurationStore.Validate(configuration);
        var owner = _content ?? throw new InvalidOperationException("请先加载机器人。");
        progress?.Report(new RobotLoadProgress(0, 1, $"正在读取 Tool：{configuration.ToolName}", false));
        var tool = await Task.Run(() =>
        {
            using var source = ModelResources.OpenTool(toolId, configuration.ModelFile);
            return CadModelPart.Load("Tool", source, null, state => progress?.Report(state));
        });
        if (_disposed || _content != owner)
        {
            tool.Dispose();
            throw new ObjectDisposedException(nameof(RobotSceneService));
        }
        owner.ReplaceTool(tool);
        owner.SetToolTcp(configuration.Tcp);
        owner.Root.UpdateTransform(Matrix4d.Identity);
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        ZoomAll();
        progress?.Report(new RobotLoadProgress(1, 1, tool.LoadMessage, tool.FromCache));
    }

    /// <summary>工装复用装配模型读取器，成功后才替换，固定在机器人基坐标下。</summary>
    public async Task LoadWorkAsync(string workId, WorkConfiguration configuration, IProgress<RobotLoadProgress>? progress = null)
    {
        WorkConfigurationStore.Validate(configuration);
        var owner = _content ?? throw new InvalidOperationException("请先加载机器人。");
        progress?.Report(new RobotLoadProgress(0, 1, $"正在读取工装：{configuration.WorkName}", false));
        var work = await Task.Run(() =>
        {
            using var source = ModelResources.OpenWork(workId, configuration.ModelFile);
            return CadModelPart.Load("Work", source, null, state => progress?.Report(state));
        });
        if (_disposed || _content != owner)
        {
            work.Dispose();
            throw new ObjectDisposedException(nameof(RobotSceneService));
        }
        owner.ReplaceWork(work);
        owner.SetWorkPose(configuration.Pose);
        owner.Root.UpdateTransform(Matrix4d.Identity);
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        ZoomAll();
        progress?.Report(new RobotLoadProgress(1, 1, work.LoadMessage, work.FromCache));
    }

    /// <summary>新机器人准备完成后局部替换，保留 Tool、工装与相机；首次安装时适配窗口。</summary>
    public void Attach(RobotSceneContent content)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        InitializeViewport();
        var previous = _content;
        _renderControl.ShowSceneNode(content.Root);
        previous?.MoveAttachmentsTo(content);
        _content = content;
        content.Root.UpdateTransform(Matrix4d.Identity);
        if (previous is not null)
        {
            _renderControl.RemoveSceneNode(previous.Root.GetUuid());
            previous.Dispose();
        }
        else ZoomAll();
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
    }

    /// <summary>批量更新各关节变换，最后只更新一次世界变换并请求一帧绘制，减少动画开销。</summary>
    public void ApplyAll(IEnumerable<JointViewModel> joints)
    {
        if (!IsLoaded)
        {
            return;
        }

        foreach (var joint in joints)
        {
            ApplyJointTransform(joint, requestDraw: false);
        }

        _content!.Root.UpdateTransform(Matrix4d.Identity);
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
    }

    /// <summary>更新单轴变换并立即请求绘制，供手动拖动滑块或输入角度使用。</summary>
    public void ApplyJoint(JointViewModel joint)
    {
        if (!IsLoaded)
        {
            return;
        }

        ApplyJointTransform(joint, requestDraw: true);
    }

    /// <summary>在已加载场景中应用三组坐标箭头的显示状态，并请求界面重绘。</summary>
    public void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp)
    {
        if (IsLoaded)
        {
            _content!.SetCoordinateVisibility(showBase, showFlange, showTcp);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    /// <summary>显示或隐藏 TCP 焊接光；加载前调用会被忽略。</summary>
    public void SetWeldBeamVisible(bool show)
    {
        if (IsLoaded)
        {
            _content!.SetWeldBeamVisible(show);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    /// <summary>更新 TCP 局部位姿后重新计算世界变换，让箭头立即跟随新参数显示。</summary>
    public void SetToolTcp(PoseConfiguration pose)
    {
        if (IsLoaded)
        {
            _content!.SetToolTcp(pose);
            _content.Root.UpdateTransform(Matrix4d.Identity);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    public void SetWorkPose(PoseConfiguration pose)
    {
        if (IsLoaded)
        {
            _content!.SetWorkPose(pose);
            _content.Root.UpdateTransform(Matrix4d.Identity);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    /// <summary>停止自转并切换到标准方向，围绕当前目标点旋转相机后适配窗口。</summary>
    public void SetView(RobotView view)
    {
        var standardView = view switch
        {
            RobotView.Isometric => EnumStandardView.View3D,
            RobotView.Front => EnumStandardView.Front,
            RobotView.Top => EnumStandardView.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(view))
        };

        _orbitSpeed = 0;
        _orbitPaused = false;
        _pendingViewFit = false;
        _pendingOrbit = null;

        // 先按当前方向适配模型，再交给 AnyCAD 原生标准视角动画旋转。
        // 动画结束后由渲染帧按最终方向重新适配窗口，避免旋转过程中反复改变构图。
        ZoomAll();
        _renderControl.SetStandardView(standardView, true);
        _pendingViewFit = true;
    }

    /// <summary>按模型世界包围盒居中并填满窗口，保留少量边距避免贴边。</summary>
    public void ZoomAll()
    {
        if (_content is null)
        {
            return;
        }

        WithoutCameraChanged(() =>
        {
            using var box = _content.GetModelBoundingBox();
            _renderControl.Viewer.ZoomToBox(box, 1.0f);
            using var min = box.getMinimum();
            using var max = box.getMaximum();

            // 环绕中心使用模型包围盒中心，旋转时整体画面更稳定。
            _orbitCenterX = (min.x + max.x) / 2;
            _orbitCenterY = (min.y + max.y) / 2;
            _orbitCenterZ = (min.z + max.z) / 2;
            _orbitDistance = _renderControl.Camera.GetDistance();
            // 原生鼠标旋转同样绕几何中心，平移只改变构图，不把旋转支点移到空处。
            using var pivot = new Vector3(_orbitCenterX, _orbitCenterY, _orbitCenterZ);
            _renderControl.ViewContext.SetUserOrbitPivot(pivot);
            RefreshFitCorners();

            FitOrthoToParts(coverFullOrbit: false);
        });
        RememberCameraPose();
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
    }

    /// <summary>缓存各部件包围盒的八个角点，供整圈旋转的最坏投影计算。</summary>
    private void RefreshFitCorners()
    {
        _fitCorners.Clear();
        if (_content is null)
        {
            return;
        }

        foreach (var partBox in _content.GetPartBoundingBoxes())
        {
            using (partBox)
            {
                using var partMin = partBox.getMinimum();
                using var partMax = partBox.getMaximum();
                _fitCorners.Add((partMin.x, partMin.y, partMin.z));
                _fitCorners.Add((partMin.x, partMin.y, partMax.z));
                _fitCorners.Add((partMin.x, partMax.y, partMin.z));
                _fitCorners.Add((partMin.x, partMax.y, partMax.z));
                _fitCorners.Add((partMax.x, partMin.y, partMin.z));
                _fitCorners.Add((partMax.x, partMin.y, partMax.z));
                _fitCorners.Add((partMax.x, partMax.y, partMin.z));
                _fitCorners.Add((partMax.x, partMax.y, partMax.z));
            }
        }
    }

    /// <summary>按部件投影调整正交缩放；环绕时采样整圈角度，保证任何角度都不切边。</summary>
    private void FitOrthoToParts(bool coverFullOrbit)
    {
        if (_content is null || _fitCorners.Count == 0)
        {
            return;
        }

        // 投影接口返回原生像素，不能与 WPF 的 DIP 尺寸混算。
        var controlWidth = _renderControl.Viewer.GetWidth();
        var controlHeight = _renderControl.Viewer.GetHeight();
        if (controlWidth <= 2 || controlHeight <= 2)
        {
            return;
        }

        var camera = _renderControl.Camera;
        var baseOrtho = camera.GetOrthoZoomFactor();
        var maxFraction = 0.0;
        if (coverFullOrbit && _orbitSpeed != 0)
        {
            // 每 15° 采样一次，取旋转一圈中投影最大的角度作为适配依据。
            for (var index = 0; index < 24; index++)
            {
                SetOrbitCamera(index * 15.0);
                maxFraction = Math.Max(maxFraction, MeasureFitFraction(controlWidth, controlHeight));
            }
            SetOrbitCamera(_orbitAzimuth);
        }
        else
        {
            maxFraction = MeasureFitFraction(controlWidth, controlHeight);
        }

        if (maxFraction > 0)
        {
            // 留 3% 边距；正交缩放因子越大模型越小，因此乘以最大投影占比。
            camera.SetOrthoZoomFactor(baseOrtho * maxFraction / 0.97);
            camera.UpdateProjectionMatrix();
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    /// <summary>把缓存的角点投影到屏幕，返回宽或高相对控件尺寸的最大占比。</summary>
    private double MeasureFitFraction(double controlWidth, double controlHeight)
    {
        var screenMinX = double.MaxValue;
        var screenMinY = double.MaxValue;
        var screenMaxX = double.MinValue;
        var screenMaxY = double.MinValue;
        foreach (var (x, y, z) in _fitCorners)
        {
            using var point = new Vector3(x, y, z);
            using var screen = _renderControl.Viewer.WorldPointToSceenPoint(point);
            screenMinX = Math.Min(screenMinX, screen.x);
            screenMinY = Math.Min(screenMinY, screen.y);
            screenMaxX = Math.Max(screenMaxX, screen.x);
            screenMaxY = Math.Max(screenMaxY, screen.y);
        }

        // 非对称装配的投影不一定关于包围盒中心对称，分别检查两侧边界。
        var screenWidth = 2 * Math.Max(Math.Abs(screenMinX - controlWidth / 2), Math.Abs(screenMaxX - controlWidth / 2));
        var screenHeight = 2 * Math.Max(Math.Abs(screenMinY - controlHeight / 2), Math.Abs(screenMaxY - controlHeight / 2));
        if (screenWidth <= 0 || screenHeight <= 0)
        {
            return 0;
        }

        return Math.Max(screenWidth / controlWidth, screenHeight / controlHeight);
    }

    /// <summary>设置相机绕模型包围盒中心的环绕速度、起始方位角和仰角；速度为 0 时只定位不旋转。</summary>
    public void SetOrbit(double speedDegreesPerSecond, double startAngleDegrees, double elevationDegrees)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // 原生标准视角动画期间改速度或角度时先记录，动画结束后再应用，
        // 否则两套相机动画会互相覆盖。
        if (_pendingViewFit)
        {
            _pendingOrbit = (speedDegreesPerSecond, startAngleDegrees, elevationDegrees);
            return;
        }
        ApplyOrbit(speedDegreesPerSecond, startAngleDegrees, elevationDegrees);
    }

    /// <summary>立即应用环绕速度和起始方向。</summary>
    private void ApplyOrbit(double speedDegreesPerSecond, double startAngleDegrees, double elevationDegrees)
    {
        _orbitSpeed = speedDegreesPerSecond;
        _orbitAzimuth = startAngleDegrees;
        _orbitElevation = Math.Clamp(elevationDegrees, -85, 85);
        _orbitPaused = false;
        _pendingViewFit = false;
        WithoutCameraChanged(() =>
        {
            SetOrbitCamera(_orbitAzimuth);
            // 环绕时按整圈最坏角度适配，静止时按当前角度适配。
            FitOrthoToParts(coverFullOrbit: _orbitSpeed != 0);
        });
        _renderControl.RequestDraw(EnumUpdateFlags.Camera);
    }

    /// <summary>记录手动操作后的相机朝向；构图中心可以平移，但几何旋转支点保持不变。</summary>
    private void ResumeOrbit()
    {
        if (!_orbitPaused)
        {
            return;
        }

        var camera = _renderControl.Camera;
        using var position = camera.GetPosition();
        using var target = camera.GetTarget();
        var offsetX = position.x - target.x;
        var offsetY = position.y - target.y;
        var offsetZ = position.z - target.z;
        var distance = Math.Sqrt(offsetX * offsetX + offsetY * offsetY + offsetZ * offsetZ);
        if (distance > 1e-6)
        {
            _orbitDistance = distance;
            _orbitAzimuth = Math.Atan2(offsetY, offsetX) * 180 / Math.PI;
            _orbitElevation = Math.Clamp(Math.Asin(offsetZ / distance) * 180 / Math.PI, -85, 85);
        }
        _orbitPaused = false;
        // 记录用户停止操作时的相机姿态，忽略仍在队列中的最后几个 CameraChanged。
        RememberCameraPose();
    }

    /// <summary>
    /// 按 WPF 渲染帧推进自转；这是 AnyCAD 控件默认使用的渲染事件。
    /// 不使用 EnableAnimation/SetAnimationCallback，因为它会把渲染切到 30ms 定时器，
    /// 降低机器人关节动作的整体帧率。
    /// </summary>
    private void OnCompositionFrame(object? sender, EventArgs e)
    {
        if (_disposed)
        {
            return;
        }
        if (e is not RenderingEventArgs renderArgs)
        {
            return;
        }

        var now = renderArgs.RenderingTime.TotalSeconds;
        var elapsed = _lastFrameSeconds is null ? 0 : now - _lastFrameSeconds.Value;
        _lastFrameSeconds = now;
        // 窗口卡顿或断线恢复时限制单帧步长，避免一帧跳很远。
        elapsed = Math.Clamp(elapsed, 0, 0.05);

        if (_content is null)
        {
            return;
        }

        // 原生标准视角动画完成后，按最终方向重新适配窗口。
        // 不能只调用 FitOrthoToParts：它基于当前缩放做比例修正，动画后的基准已不是适配值。
        if (_pendingViewFit && now - _lastCameraChangeSeconds >= ViewFitDelaySeconds)
        {
            _pendingViewFit = false;
            ZoomAll();
            if (_pendingOrbit is { } orbit)
            {
                _pendingOrbit = null;
                ApplyOrbit(orbit.Speed, orbit.Start, orbit.Elevation);
            }
        }

        if (_orbitPaused)
        {
            if (now - _lastCameraChangeSeconds < CameraResumeDelaySeconds)
            {
                return;
            }
            ResumeOrbit();
        }

        if (_orbitSpeed == 0 || elapsed <= 0)
        {
            return;
        }

        var delta = _orbitSpeed * elapsed;
        _orbitAzimuth = (_orbitAzimuth + delta) % 360;
        ApplyOrbitCamera(delta);
    }

    /// <summary>
    /// AnyCAD 原生 CameraChanged 回调：只有相机确实偏离程序设置值时才认定用户操作。
    /// 原生事件可能异步到达，不能只依赖同步的抑制标志，否则自转会被自己的事件反复暂停。
    /// </summary>
    private void OnCameraChanged()
    {
        if (_disposed || _suppressCameraChanged || IsCameraAtExpectedPose())
        {
            return;
        }

        _orbitPaused = true;
        _lastCameraChangeSeconds = _lastFrameSeconds ?? 0;
    }

    /// <summary>记录程序主动设置后的相机姿态，供后续 CameraChanged 判断是否来自用户。</summary>
    private void RememberCameraPose()
    {
        var camera = _renderControl.Camera;
        using var position = camera.GetPosition();
        using var target = camera.GetTarget();
        using var up = camera.GetUp();
        _expectedCameraPose[0] = position.x;
        _expectedCameraPose[1] = position.y;
        _expectedCameraPose[2] = position.z;
        _expectedCameraPose[3] = target.x;
        _expectedCameraPose[4] = target.y;
        _expectedCameraPose[5] = target.z;
        _expectedCameraPose[6] = up.x;
        _expectedCameraPose[7] = up.y;
        _expectedCameraPose[8] = up.z;
        _hasExpectedCameraPose = true;
    }

    /// <summary>当前相机是否仍是程序最近一次设置的位置。</summary>
    private bool IsCameraAtExpectedPose()
    {
        if (!_hasExpectedCameraPose)
        {
            return false;
        }

        var camera = _renderControl.Camera;
        using var position = camera.GetPosition();
        using var target = camera.GetTarget();
        using var up = camera.GetUp();
        const double tolerance = 1e-3;
        return Math.Abs(position.x - _expectedCameraPose[0]) <= tolerance
            && Math.Abs(position.y - _expectedCameraPose[1]) <= tolerance
            && Math.Abs(position.z - _expectedCameraPose[2]) <= tolerance
            && Math.Abs(target.x - _expectedCameraPose[3]) <= tolerance
            && Math.Abs(target.y - _expectedCameraPose[4]) <= tolerance
            && Math.Abs(target.z - _expectedCameraPose[5]) <= tolerance
            && Math.Abs(up.x - _expectedCameraPose[6]) <= tolerance
            && Math.Abs(up.y - _expectedCameraPose[7]) <= tolerance
            && Math.Abs(up.z - _expectedCameraPose[8]) <= tolerance;
    }

    /// <summary>临时忽略本服务主动修改相机产生的 CameraChanged 通知。</summary>
    private void WithoutCameraChanged(Action action)
    {
        var previous = _suppressCameraChanged;
        _suppressCameraChanged = true;
        try
        {
            action();
        }
        finally
        {
            _suppressCameraChanged = previous;
        }
    }

    /// <summary>眼点、观察点和上方向一起绕模型转动，保留手动平移后的构图和缩放。</summary>
    private void ApplyOrbitCamera(double deltaDegrees)
    {
        if (_content is null)
        {
            return;
        }

        var camera = _renderControl.Camera;
        using var position = camera.GetPosition();
        using var target = camera.GetTarget();
        using var up = camera.GetUp();
        var angle = deltaDegrees * Math.PI / 180;
        var cosine = Math.Cos(angle);
        var sine = Math.Sin(angle);
        using var nextPosition = new Vector3d(
            _orbitCenterX + cosine * (position.x - _orbitCenterX) - sine * (position.y - _orbitCenterY),
            _orbitCenterY + sine * (position.x - _orbitCenterX) + cosine * (position.y - _orbitCenterY), position.z);
        using var nextTarget = new Vector3d(
            _orbitCenterX + cosine * (target.x - _orbitCenterX) - sine * (target.y - _orbitCenterY),
            _orbitCenterY + sine * (target.x - _orbitCenterX) + cosine * (target.y - _orbitCenterY), target.z);
        using var nextUp = new Vector3d(cosine * up.x - sine * up.y, sine * up.x + cosine * up.y, up.z);
        var zoom = camera.GetOrthoZoomFactor();
        // 自转需要固定角速度和稳定的模型构图；ViewContext.Orbit 以鼠标像素为输入且会移动
        // 相机目标，所以这里继续用 Camera.LookAt 连续旋转，只把用户操作交给原生 CameraChanged。
        WithoutCameraChanged(() =>
        {
            camera.LookAt(nextPosition, nextTarget, nextUp);
            camera.SetOrthoZoomFactor(zoom);
            camera.UpdateProjectionMatrix();
        });
        RememberCameraPose();
        // 自转只改相机，用 Camera 标志通知引擎刷新视图。
        _renderControl.RequestDraw(EnumUpdateFlags.Camera);
    }

    /// <summary>按指定方位角把相机放到环绕位置，保留当前正交缩放。</summary>
    private void SetOrbitCamera(double azimuthDegrees)
    {
        var camera = _renderControl.Camera;
        var ortho = camera.GetOrthoZoomFactor();
        var distance = _orbitDistance > 0 ? _orbitDistance : camera.GetDistance();
        var azimuth = azimuthDegrees * Math.PI / 180;
        var elevation = _orbitElevation * Math.PI / 180;
        using var eye = new Vector3d(
            _orbitCenterX + distance * Math.Cos(elevation) * Math.Cos(azimuth),
            _orbitCenterY + distance * Math.Cos(elevation) * Math.Sin(azimuth),
            _orbitCenterZ + distance * Math.Sin(elevation));
        using var target = new Vector3d(_orbitCenterX, _orbitCenterY, _orbitCenterZ);
        using var up = new Vector3d(0, 0, 1);
        WithoutCameraChanged(() =>
        {
            camera.LookAt(eye, target, up);
            // 显式保留用户选择的比例，适配采样和自转都不逐帧缩放。
            camera.SetOrthoZoomFactor(ortho);
        });
        RememberCameraPose();
    }

    /// <summary>最终关闭服务，释放机器人、背景并清除初始化标记，允许重复调用。</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_frameSubscribed)
        {
            CompositionTarget.Rendering -= OnCompositionFrame;
            _frameSubscribed = false;
        }
        if (_cameraListener is not null)
        {
            using (var manager = AppEventListenerManager.Instance())
            {
                manager.Remove(_cameraListener);
            }
            _cameraListener.Dispose();
            _cameraListener = null;
        }
        _orbitPaused = false;
        _pendingViewFit = false;
        _pendingOrbit = null;
        _lastFrameSeconds = null;
        _hasExpectedCameraPose = false;
        ClearCore();
        _background?.Dispose();
        _background = null;
        _viewportInitialized = false;
    }

    /// <summary>先从渲染控件移除场景，再释放原生资源；关闭时兼容 SDK 已先销毁视口的情况。</summary>
    private void ClearCore()
    {
        var content = _content;
        _content = null;
        if (content is null)
        {
            return;
        }

        try
        {
            if (_renderControl.IsReady)
            {
                _renderControl.ClearScene();
            }
        }
        catch (NullReferenceException exception)
        {
            // 关闭窗口时 SDK 可能先释放内部视口，此时场景已被移除，只需继续释放持有的资源。
            AppLog.Write($"关闭时 AnyCAD 视图已先释放，跳过重复 ClearScene：{exception.Message}");
        }
        finally
        {
            content.Dispose();
        }
    }

    /// <summary>围绕 CAD 全局轴线应用“当前角度 - CAD 参考角”的旋转，父子层级负责下游随动。</summary>
    /// <param name="joint">当前轴的角度、参考角和旋转轴线参数。</param>
    /// <param name="requestDraw">单轴操作时立即刷新；批量操作传 false，由外层统一刷新。</param>
    private void ApplyJointTransform(JointViewModel joint, bool requestDraw)
    {
        var length = Math.Sqrt(
            joint.AxisDirectionX * joint.AxisDirectionX
            + joint.AxisDirectionY * joint.AxisDirectionY
            + joint.AxisDirectionZ * joint.AxisDirectionZ);

        if (length < 1e-9)
        {
            return;
        }

        using var origin = new GPnt(joint.AxisOriginX, joint.AxisOriginY, joint.AxisOriginZ);
        using var direction = new GDir(
            joint.AxisDirectionX / length,
            joint.AxisDirectionY / length,
            joint.AxisDirectionZ / length);
        using var axis = new GAx1(origin, direction);
        using var transform = new GTrsf();
        // STEP 已经带有原装配姿态，因此只旋转相对 CAD 参考角的差值，不能直接应用示教器绝对角。
        var modelRotation = joint.Angle - joint.ModelReferenceAngle;
        transform.SetRotation(axis, modelRotation * Math.PI / 180.0);
        using var matrix = Matrix4d.From(transform);

        var group = _content!.JointGroups[joint.Index];
        group.SetTransform(matrix);
        group.RequestUpdate();

        if (requestDraw)
        {
            _content.Root.UpdateTransform(Matrix4d.Identity);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
    }

    /// <summary>AnyCAD 原生相机变化通知；识别鼠标操作，替代轮询按键和屏幕坐标。</summary>
    private sealed class CameraChangeListener : AppEventListener
    {
        private readonly RobotSceneService _owner;

        public CameraChangeListener(RobotSceneService owner)
        {
            _owner = owner;
        }

        /// <summary>只处理相机变化，其他应用事件保持默认行为。</summary>
        public override void OnMessage(EnumAppEventType evt, string msg, AppEventArgs content)
        {
            if (evt == EnumAppEventType.CameraChanged)
            {
                _owner.OnCameraChanged();
            }
        }
    }
}
