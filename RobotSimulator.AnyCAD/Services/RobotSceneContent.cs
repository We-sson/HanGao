using AnyCAD.Foundation;
using RobotSimulator.Models;

namespace RobotSimulator.Services;

/// <summary>固定的机器人场景及可独立替换的 Tool，二者分别持有和释放原生资源。</summary>
public sealed class RobotSceneContent : IDisposable
{
    /// <summary>机器人基座和六轴部件，切换 Tool 时保持不变。</summary>
    private readonly List<CadModelPart> _parts = [];
    /// <summary>场景层级和坐标箭头的原生包装。</summary>
    private readonly List<IDisposable> _cadResources = [];
    /// <summary>当前安装的工具，替换时立即释放旧工具。</summary>
    private CadModelPart? _toolPart;
    /// <summary>固定于机器人基坐标的工装，与 Tool 分别管理生命周期。</summary>
    private CadModelPart? _workPart;
    private GroupSceneNode _workMount = null!;
    /// <summary>法兰下面的工具安装节点，局部变换为单位矩阵。</summary>
    private GroupSceneNode _toolMount = null!;
    /// <summary>法兰下面的 TCP 变换节点。</summary>
    private GroupSceneNode _tcpFrame = null!;
    /// <summary>固定的基坐标显示箭头。</summary>
    private AxisWidget _baseAxes = null!;
    /// <summary>随法兰运动的显示箭头。</summary>
    private AxisWidget _flangeAxes = null!;
    /// <summary>工具中心的显示箭头。</summary>
    private AxisWidget _tcpAxes = null!;
    /// <summary>防止重复释放。</summary>
    private bool _disposed;

    /// <summary>整台机器人的场景根节点。</summary>
    public GroupSceneNode Root { get; } = new();
    /// <summary>按照 J1～J6 排列的旋转节点。</summary>
    public IReadOnlyList<GroupSceneNode> JointGroups { get; private set; } = [];
    /// <summary>当前是否安装了独立工具模型。</summary>
    public bool HasTool => _toolPart is not null;
    public bool HasWork => _workPart is not null;

    /// <summary>返回机器人、Tool 和工装各部件独立的包围盒，供按实际投影范围适配相机。</summary>
    public IReadOnlyList<AABox> GetPartBoundingBoxes()
    {
        var boxes = new List<AABox>();
        void Add(SceneNode node) => boxes.Add(node.GetWorldBBox());
        foreach (var part in _parts) Add(part.Node);
        if (_toolPart is not null) Add(_toolPart.Node);
        if (_workPart is not null) Add(_workPart.Node);
        return boxes;
    }

    /// <summary>计算机器人、Tool 和工装几何的世界包围盒，不包含坐标轴等显示辅助对象。</summary>
    public AABox GetModelBoundingBox()
    {
        var minX = double.MaxValue;
        var minY = double.MaxValue;
        var minZ = double.MaxValue;
        var maxX = double.MinValue;
        var maxY = double.MinValue;
        var maxZ = double.MinValue;
        void Include(SceneNode node)
        {
            using var box = node.GetWorldBBox();
            using var min = box.getMinimum();
            using var max = box.getMaximum();
            minX = Math.Min(minX, min.x);
            minY = Math.Min(minY, min.y);
            minZ = Math.Min(minZ, min.z);
            maxX = Math.Max(maxX, max.x);
            maxY = Math.Max(maxY, max.y);
            maxZ = Math.Max(maxZ, max.z);
        }

        foreach (var part in _parts) Include(part.Node);
        if (_toolPart is not null) Include(_toolPart.Node);
        if (_workPart is not null) Include(_workPart.Node);
        return minX > maxX ? Root.GetWorldBBox() : new AABox(minX, minY, minZ, maxX, maxY, maxZ);
    }

    /// <summary>通过 Load 创建场景，避免暴露未完整初始化的坐标节点。</summary>
    private RobotSceneContent() { }

    /// <summary>只读取内嵌的机器人基座和六轴，工具由独立加载流程安装。</summary>
    public static RobotSceneContent Load(RobotConfiguration configuration, string modelId,
        Action<RobotLoadProgress>? report = null, string? cacheRoot = null)
    {
        var content = new RobotSceneContent();
        try
        {
            var files = new List<(string Name, string File)> { ("Base", configuration.BaseFile) };
            files.AddRange(configuration.Joints.Select(joint => (joint.Name, joint.ModelFile)));
            var nodes = new Dictionary<string, SceneNode>();
            var loadSync = new object();
            // 各部件使用独立 XDE 文档，最多四路解析；全部完成后再统一组装运动链。
            Parallel.For(0, files.Count, new ParallelOptions { MaxDegreeOfParallelism = Math.Min(4, Environment.ProcessorCount) }, index =>
            {
                var (name, file) = files[index];
                lock (loadSync)
                    report?.Invoke(new RobotLoadProgress(nodes.Count, files.Count, $"正在并行读取 {name}：{file}", false));
                using var source = ModelResources.OpenRobot(modelId, file);
                var part = CadModelPart.Load(name, source, cacheRoot);
                lock (loadSync)
                {
                    content._parts.Add(part);
                    nodes.Add(name, part.Node);
                    report?.Invoke(new RobotLoadProgress(nodes.Count, files.Count, part.LoadMessage, part.FromCache));
                }
            });
            content.Root.SetPickable(false);
            content.Root.AddNode(nodes["Base"]);
            content._workMount = new GroupSceneNode();
            content._cadResources.Add(content._workMount);
            content._workMount.SetName("WorkMount");
            content._workMount.SetPickable(false);
            content.Root.AddNode(content._workMount);
            var groups = new List<GroupSceneNode>(6);
            var parent = content.Root;
            foreach (var joint in configuration.Joints)
            {
                var group = new GroupSceneNode();
                content._cadResources.Add(group);
                group.SetPickable(false);
                group.AddNode(nodes[joint.Name]);
                parent.AddNode(group);
                groups.Add(group);
                parent = group;
            }
            content.JointGroups = groups;
            content.CreateCoordinateFrames(parent, configuration);
            return content;
        }
        catch { content.Dispose(); throw; }
    }

    /// <summary>挂接已成功解析的新工具；只替换 Tool 节点，不重新读取机器人或改变关节角度。</summary>
    public void ReplaceTool(CadModelPart? tool)
    {
        if (ReferenceEquals(tool, _toolPart)) return;
        _toolMount.Clear();
        var previous = _toolPart;
        _toolPart = tool;
        if (tool is not null) _toolMount.AddNode(tool.Node);
        previous?.Dispose();
    }

    /// <summary>只替换固定工装，挂在基坐标下，不进入六轴运动链。</summary>
    public void ReplaceWork(CadModelPart? work)
    {
        if (ReferenceEquals(work, _workPart)) return;
        _workMount.Clear();
        var previous = _workPart;
        _workPart = work;
        if (work is not null) _workMount.AddNode(work.Node);
        previous?.Dispose();
    }

    /// <summary>机器人更换时转移现有附属模型的所有权，复用几何并保留安装参数。</summary>
    public void MoveAttachmentsTo(RobotSceneContent target)
    {
        if (ReferenceEquals(target, this)) return;
        using var workPose = _workMount.GetTransform();
        using var tcpPose = _tcpFrame.GetTransform();
        target._workMount.SetTransform(workPose);
        target._tcpFrame.SetTransform(tcpPose);
        if (_toolPart is { } tool)
        {
            if (!_toolMount.RemoveNode(tool.Node.GetUuid())) throw new InvalidOperationException("无法从原机器人移出 Tool。");
            // RemoveNode 保留父节点引用，显式解除后才能挂接到新机器人。
            tool.Node.SetParent(null);
            _toolPart = null;
            target.ReplaceTool(tool);
        }
        if (_workPart is { } work)
        {
            if (!_workMount.RemoveNode(work.Node.GetUuid())) throw new InvalidOperationException("无法从原机器人移出工装。");
            work.Node.SetParent(null);
            _workPart = null;
            target.ReplaceWork(work);
        }
    }

    /// <summary>基坐标下的整体安装位姿；全零保持 STEP 原有装配坐标。</summary>
    public void SetWorkPose(PoseConfiguration pose)
    {
        using var transform = CreatePoseMatrix(pose);
        _workMount.SetTransform(transform);
    }

    /// <summary>独立控制三组箭头，不隐藏法兰父节点，以免连带隐藏工具和 TCP。</summary>
    public void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp)
    {
        _baseAxes.SetVisible(showBase);
        _flangeAxes.SetVisible(showFlange);
        _tcpAxes.SetVisible(showTcp);
    }

    /// <summary>设置法兰局部坐标下的 TCP，仅移动坐标箭头，不移动 Tool 模型。</summary>
    public void SetToolTcp(PoseConfiguration pose)
    {
        using var transform = CreatePoseMatrix(pose);
        _tcpFrame.SetTransform(transform);
    }

    /// <summary>创建基坐标及法兰坐标，并让 Tool 与 TCP 都成为法兰的子节点。</summary>
    private void CreateCoordinateFrames(GroupSceneNode lastJoint, RobotConfiguration configuration)
    {
        _baseAxes = CreateAxes("BaseAxes", 64);
        Root.AddNode(_baseAxes);
        var flange = new GroupSceneNode();
        _cadResources.Add(flange);
        flange.SetName("FlangeFrame");
        flange.SetPickable(false);
        var origin = configuration.FlangeOrigin;
        var orientation = configuration.FlangeOrientation;
        using var transform = CreatePoseMatrix(new PoseConfiguration
        {
            X = origin.X, Y = origin.Y, Z = origin.Z,
            A = orientation.A, B = orientation.B, C = orientation.C
        });
        flange.SetTransform(transform);
        lastJoint.AddNode(flange);
        _flangeAxes = CreateAxes("FlangeAxes", 48);
        flange.AddNode(_flangeAxes);

        // 工具原点与法兰重合，模型 XYZ 直接对齐法兰 XYZ，无需为每台机器人复制工具。
        _toolMount = new GroupSceneNode();
        _cadResources.Add(_toolMount);
        _toolMount.SetName("ToolMount");
        _toolMount.SetPickable(false);
        flange.AddNode(_toolMount);
        _tcpFrame = new GroupSceneNode();
        _cadResources.Add(_tcpFrame);
        _tcpFrame.SetName("TcpFrame");
        _tcpFrame.SetPickable(false);
        flange.AddNode(_tcpFrame);
        _tcpAxes = CreateAxes("TcpAxes", 64);
        _tcpFrame.AddNode(_tcpAxes);
        SetToolTcp(new PoseConfiguration());
    }

    /// <summary>先释放场景节点，再释放各独立部件和坐标资源，允许重复调用。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Root.Dispose();
        _toolPart?.Dispose();
        _workPart?.Dispose();
        foreach (var part in _parts) part.Dispose();
        for (var index = _cadResources.Count - 1; index >= 0; index--) _cadResources[index].Dispose();
        _parts.Clear();
        _cadResources.Clear();
    }

    /// <summary>创建无文字的 XYZ 箭头；length 表示固定屏幕尺寸的标尺，材质负责抵消相机缩放。</summary>
    private AxisWidget CreateAxes(string name, float length)
    {
        using var lengths = new Vector3(length, length, length);
        var axes = AxisWidget.Create(4.0f, lengths, true, false)
            ?? throw new InvalidOperationException($"AnyCAD 无法创建 {name} 坐标轴。");
        _cadResources.Add(axes);
        axes.SetName(name);
        axes.SetPickable(false);
        axes.SetChildrenPickable(false);
        // 不能设为无限尺寸：此标记会向上传播，使整个机器人退出相机范围计算，造成模型被裁掉。
        axes.SetInfiniteSize(false);
        axes.ShowText(false);
        using var parts = axes.GetNodes();
        ConfigureAxisMaterials(parts);
        return axes;
    }

    /// <summary>递归找到箭头内部材质，启用固定屏幕尺寸，并禁止箭头修改深度缓冲。</summary>
    private static void ConfigureAxisMaterials(SceneNode node)
    {
        using var primitive = PrimitiveSceneNode.Cast(node);
        using var material = primitive?.GetMaterial();
        if (material is not null)
        {
            // SDK 在内部材质上处理固定尺寸；禁止写深度，避免箭头把后绘制的模型表面挡掉。
            material.SetFixedSize(true);
            material.SetDepthWrite(false);
            material.RequestUpdate();
        }

        using var group = GroupSceneNode.Cast(node);
        if (group is null) return;
        using var iterator = group.CreateIterator();
        while (iterator.More())
        {
            using var child = iterator.Current();
            ConfigureAxisMaterials(child);
            iterator.Next();
        }
    }

    /// <summary>把 XYZ/ABC 位姿转换为 AnyCAD 的 4×4 变换矩阵，供法兰、Tool 和 TCP 共用。</summary>
    private static Matrix4d CreatePoseMatrix(PoseConfiguration pose)
    {
        // KUKA ABC 约定为 Rz(A)·Ry(B)·Rx(C)；配置使用度，三角函数需要先换算为弧度。
        var a = pose.A * Math.PI / 180.0;
        var b = pose.B * Math.PI / 180.0;
        var c = pose.C * Math.PI / 180.0;
        // ca/sa 分别表示 cos(A)/sin(A)，cb/sb 和 cc/sc 分别对应 B、C，供下面展开矩阵公式使用。
        var ca = Math.Cos(a);
        var sa = Math.Sin(a);
        var cb = Math.Cos(b);
        var sb = Math.Sin(b);
        var cc = Math.Cos(c);
        var sc = Math.Sin(c);

        // 左侧 3×3 为组合旋转矩阵，最后一列为 XYZ 平移，矩阵行序由原生接口约定。
        using var transform = new GTrsf();
        transform.SetValues(
            ca * cb,
            ca * sb * sc - sa * cc,
            ca * sb * cc + sa * sc,
            pose.X,
            sa * cb,
            sa * sb * sc + ca * cc,
            sa * sb * cc - ca * sc,
            pose.Y,
            -sb,
            cb * sc,
            cb * cc,
            pose.Z);
        return Matrix4d.From(transform);
    }


}
