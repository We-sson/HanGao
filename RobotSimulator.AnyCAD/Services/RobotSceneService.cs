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

    /// <summary>绑定当前窗口的渲染控件，此时尚不读取模型或访问未就绪的原生视口。</summary>
    public RobotSceneService(RenderControl renderControl)
    {
        _renderControl = renderControl;
    }

    /// <summary>已挂接完整六轴场景时为 true，不要求存在可选 Tool。</summary>
    public bool IsLoaded => _content is not null && _content.JointGroups.Count == 6;

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
            return CadModelPart.Load("Tool", source);
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

    /// <summary>移除旧场景、接管新场景，并把相机调整到能看到整个机器人的范围。</summary>
    public void Attach(RobotSceneContent content)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Clear();
        _content = content;

        InitializeViewport();
        _renderControl.ShowSceneNode(content.Root);
        _renderControl.ZoomAll(1.15f);
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

    /// <summary>把界面视角枚举映射到 AnyCAD 标准视图，再自动适配整个模型。</summary>
    public void SetView(RobotView view)
    {
        var standardView = view switch
        {
            RobotView.Isometric => EnumStandardView.View3D,
            RobotView.Front => EnumStandardView.Front,
            RobotView.Top => EnumStandardView.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(view))
        };
        _renderControl.SetStandardView(standardView, true);
        ZoomAll();
    }

    /// <summary>按整体包围盒适配相机并留出边距，不改变任何关节姿态。</summary>
    public void ZoomAll() => _renderControl.ZoomAll(1.15f);

    /// <summary>移除并释放当前机器人，用于重载或切换型号；保留视口设置供新场景使用。</summary>
    public void Clear()
    {
        ClearCore(isShuttingDown: false);
    }

    /// <summary>最终关闭服务，释放机器人、背景并清除初始化标记，允许重复调用。</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ClearCore(isShuttingDown: true);
        _background?.Dispose();
        _background = null;
        _viewportInitialized = false;
    }

    /// <summary>先从渲染控件移除场景，再释放原生资源；关闭时兼容 SDK 已先销毁视口的情况。</summary>
    private void ClearCore(bool isShuttingDown)
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
        catch (NullReferenceException exception) when (isShuttingDown)
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
