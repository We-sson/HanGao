using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
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
    /// <summary>相机更新计时器；静止时也保留鼠标操作重绘。</summary>
    private DispatcherTimer? _orbitTimer;
    /// <summary>用户正在用鼠标调整视角时暂停自转，避免自动相机覆盖手动操作。</summary>
    private bool _orbitPaused;
    /// <summary>本次按键是否从三维视口内开始；从界面其它位置按下的拖动不影响自转。</summary>
    private bool _viewportPressed;
    /// <summary>上一帧是否有鼠标按键按下，用于识别按下的瞬间。</summary>
    private bool _wasMousePressed;
    /// <summary>环绕计时使用的单调时钟，保证速度不受帧率抖动影响。</summary>
    private readonly Stopwatch _orbitClock = new();
    /// <summary>上一次环绕计时对应的时间，单位秒。</summary>
    private double _lastOrbitSeconds;
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
        // 只显示紧凑的方向立方体，避免 AxisAndCube 附带的大型 RGB 指示器占用视图。
        _renderControl.SetViewCube(EnumViewCoordinateType.Cube);
        _renderControl.ShowCoordinateGrid(false);
        _viewportInitialized = true;
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

        // 只停止自转，保留计时器对鼠标拖动的重绘，避免下一帧覆盖标准视角。
        _orbitSpeed = 0;

        // SetStandardView 在加载流程中会留下无效相机，改为直接计算标准方向的相机位置。
        using var driver = new CameraDriver();
        driver.ComputeCameraDirection(standardView);
        using var direction = driver.GetDirection();
        using var up = driver.GetUp();
        var camera = _renderControl.Camera;
        using var target = camera.GetTarget();
        var distance = camera.GetDistance();
        using var position = new Vector3d(
            target.x - direction.x * distance,
            target.y - direction.y * distance,
            target.z - direction.z * distance);
        camera.LookAt(position, target, up);
        ZoomAll();
    }

    /// <summary>按模型世界包围盒居中并填满窗口，保留少量边距避免贴边。</summary>
    public void ZoomAll()
    {
        if (_content is null)
        {
            return;
        }

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
        _orbitSpeed = speedDegreesPerSecond;
        _orbitAzimuth = startAngleDegrees;
        _orbitElevation = Math.Clamp(elevationDegrees, -85, 85);
        SetOrbitCamera(_orbitAzimuth);
        // 环绕时按整圈最坏角度适配，静止时按当前角度适配。
        FitOrthoToParts(coverFullOrbit: _orbitSpeed != 0);

        // 计时器即使速度为 0 也保持运行：拖动视角时需要它持续请求相机重绘，否则原生视口
        // 的拖动只有约 40 帧/秒且帧间隔不均，表现为鼠标旋转时卡时不卡。
        if (_orbitTimer is null)
        {
            _orbitTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(16) };
            _orbitTimer.Tick += OnOrbitFrame;
        }
        _lastOrbitSeconds = _orbitClock.Elapsed.TotalSeconds;
        _orbitClock.Start();
        _orbitTimer.Start();
    }

    /// <summary>停止环绕计时器；可重复调用，计时器保留以便再次启动。</summary>
    private void StopOrbit()
    {
        _orbitTimer?.Stop();
        _orbitClock.Stop();
        _orbitPaused = false;
        _viewportPressed = false;
        _wasMousePressed = false;
    }

    /// <summary>本次按住鼠标是否从三维视口内开始；按下后拖到视口外仍保持暂停，直到松开。</summary>
    private bool IsViewportInteraction()
    {
        var pressed = Mouse.LeftButton == MouseButtonState.Pressed || Mouse.MiddleButton == MouseButtonState.Pressed
            || Mouse.RightButton == MouseButtonState.Pressed;
        if (!pressed)
        {
            _viewportPressed = false;
        }
        else if (!_wasMousePressed && IsCursorOverViewport())
        {
            _viewportPressed = true;
        }
        _wasMousePressed = pressed;
        return _viewportPressed;
    }

    /// <summary>用屏幕坐标判断光标是否在三维视口范围内；视口由原生窗口接收输入，WPF 的 IsMouseOver 不可用。</summary>
    private bool IsCursorOverViewport()
    {
        if (!_renderControl.IsLoaded)
        {
            return false;
        }

        var topLeft = _renderControl.PointToScreen(new System.Windows.Point(0, 0));
        var bottomRight = _renderControl.PointToScreen(
            new System.Windows.Point(_renderControl.ActualWidth, _renderControl.ActualHeight));
        var cursor = System.Windows.Forms.Cursor.Position;
        return cursor.X >= topLeft.X && cursor.X < bottomRight.X
            && cursor.Y >= topLeft.Y && cursor.Y < bottomRight.Y;
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
    }

    /// <summary>按实际经过时间推进方位角；AnyCAD 视口不转发 WPF 鼠标事件，因此直接查询按键状态。</summary>
    private void OnOrbitFrame(object? sender, EventArgs e)
    {
        if (_disposed || _content is null)
        {
            return;
        }

        var now = _orbitClock.Elapsed.TotalSeconds;
        var elapsed = now - _lastOrbitSeconds;
        _lastOrbitSeconds = now;
        if (IsViewportInteraction())
        {
            _orbitPaused = true;
            // 相机由鼠标控制，这里只强制重绘，让拖动也按计时器节奏出帧。
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
            return;
        }
        ResumeOrbit();
        if (_orbitSpeed == 0)
        {
            return;
        }

        var delta = _orbitSpeed * elapsed;
        _orbitAzimuth = (_orbitAzimuth + delta) % 360;
        ApplyOrbitCamera(delta);
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
        camera.LookAt(nextPosition, nextTarget, nextUp);
        camera.SetOrthoZoomFactor(zoom);
        camera.UpdateProjectionMatrix();
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
        camera.LookAt(eye, target, up);
        // 显式保留用户选择的比例，适配采样和自转都不逐帧缩放。
        camera.SetOrthoZoomFactor(ortho);
    }

    /// <summary>最终关闭服务，释放机器人、背景并清除初始化标记，允许重复调用。</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        StopOrbit();
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
}
