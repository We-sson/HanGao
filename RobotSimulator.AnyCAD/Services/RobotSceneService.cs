using AnyCAD.Foundation;
using AnyCAD.WPF;
using RobotSimulator.Models;
using RobotSimulator.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace RobotSimulator.Services;

public readonly record struct RobotLoadProgress(int Completed, int Total, string Message, bool FromCache)
{
    public double Percent => Total == 0 ? 0 : Completed * 100.0 / Total;
}

/// <summary>
/// A fully parsed and tessellated robot scene.
/// </summary>
public sealed class RobotSceneContent : IDisposable
{
    private const double DisplayDeflectionMillimeters = 1.0;
    private const string CacheVersion = "v5-color-groups-1mm";
    private static readonly JsonSerializerOptions CacheJsonOptions = new() { WriteIndented = false };
    private readonly List<IDisposable> _cadResources = [];
    private bool _disposed;

    private RobotSceneContent(GroupSceneNode root, List<GroupSceneNode> jointGroups, bool hasTool)
    {
        Root = root;
        JointGroups = jointGroups;
        HasTool = hasTool;
    }

    public GroupSceneNode Root { get; }

    public IReadOnlyList<GroupSceneNode> JointGroups { get; }

    public bool HasTool { get; private set; }

    public static RobotSceneContent Load(
        RobotConfiguration configuration,
        string modelDirectory,
        Action<RobotLoadProgress>? report = null)
    {
        var requiredParts = new List<(string Name, string FileName)>
        {
            ("Base", configuration.BaseFile)
        };
        requiredParts.AddRange(configuration.Joints.Select(joint => (joint.Name, joint.ModelFile)));

        var toolPath = IOPath.Combine(modelDirectory, configuration.ToolFile);
        var hasTool = IOFile.Exists(toolPath);
        if (hasTool)
        {
            requiredParts.Add(("Tool", configuration.ToolFile));
        }

        var root = new GroupSceneNode();
        var jointGroups = new List<GroupSceneNode>(6);
        var content = new RobotSceneContent(root, jointGroups, hasTool: false);

        try
        {
            var loadedNodes = new Dictionary<string, SceneNode>(StringComparer.OrdinalIgnoreCase);
            var completed = 0;
            foreach (var (name, fileName) in requiredParts)
            {
                report?.Invoke(new RobotLoadProgress(
                    completed,
                    requiredParts.Count,
                    $"正在读取 {name}：{fileName}",
                    false));

                var result = content.LoadPart(name, IOPath.Combine(modelDirectory, fileName));
                loadedNodes.Add(name, result.Node);
                completed++;
                report?.Invoke(new RobotLoadProgress(
                    completed,
                    requiredParts.Count,
                    result.Message,
                    result.FromCache));
            }

            root.AddNode(loadedNodes["Base"]);

            GroupSceneNode parent = root;
            foreach (var joint in configuration.Joints)
            {
                var group = new GroupSceneNode();
                group.SetPickable(false);
                group.AddNode(loadedNodes[joint.Name]);
                parent.AddNode(group);
                jointGroups.Add(group);
                parent = group;
            }

            if (hasTool)
            {
                var toolGroup = new GroupSceneNode();
                toolGroup.SetName("ToolMount");
                toolGroup.SetPickable(false);
                toolGroup.AddNode(loadedNodes["Tool"]);
                using var toolPose = CreatePoseMatrix(configuration.ToolModelPose);
                toolGroup.SetTransform(toolPose);
                parent.AddNode(toolGroup);
            }

            root.SetPickable(false);
            content.HasTool = hasTool;
            return content;
        }
        catch
        {
            content.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Root.Dispose();

        // Shapes reference their XDE document, so release wrappers before documents.
        for (var index = _cadResources.Count - 1; index >= 0; index--)
        {
            _cadResources[index].Dispose();
        }
        _cadResources.Clear();
    }

    private PartLoadResult LoadPart(string name, string path)
    {
        if (!IOFile.Exists(path))
        {
            throw new System.IO.FileNotFoundException($"找不到 {name} 模型。", path);
        }

        var cacheDirectory = GetCacheDirectory(name, path);
        if (TryLoadBrepCache(name, cacheDirectory, out var cachedNode, out var cachedFaceCount))
        {
            return new PartLoadResult(cachedNode, true, $"{name} 已从 BREP 缓存载入：{cachedFaceCount} 个面");
        }

        var document = new XdeDocumentI();
        if (!document.Open(path))
        {
            document.Dispose();
            throw new System.IO.InvalidDataException($"AnyCAD 无法读取 {name} 模型：{path}");
        }
        _cadResources.Add(document);

        var partGroup = new GroupSceneNode();
        partGroup.SetName(name);
        partGroup.SetPickable(false);
        var shapeCount = document.GetShapeCount();
        if (shapeCount == 0)
        {
            partGroup.Dispose();
            throw new System.IO.InvalidDataException($"{name} STEP 中没有可显示的形状：{path}");
        }

        var faceCount = 0u;
        var colorCount = new HashSet<(int R, int G, int B)>();
        var cacheManifest = new PartCacheManifest { Version = CacheVersion };
        var shapesForCache = new List<TopoShape>(shapeCount);

        var colorGroups = new Dictionary<(int R, int G, int B), FaceColorGroup>();
        for (var shapeIndex = 0; shapeIndex < shapeCount; shapeIndex++)
        {
            using var shapeNode = document.GetShapeNode(shapeIndex);
            var shape = document.GetShape(shapeNode);
            _cadResources.Add(shape);

            using var neutralGray = new Vector3(0.72f, 0.72f, 0.72f);
            using var shapeColor = document.GetColor(shapeNode, neutralGray);
            colorCount.Add(ToColorKey(shapeColor));

            using var faces = shape.GetChildren(EnumTopoShapeType.Topo_FACE);
            for (var faceIndex = 0; faceIndex < faces.Count; faceIndex++)
            {
                using var face = faces[faceIndex];
                using var faceColor = document.GetFaceColor(face, shapeColor);
                var colorKey = ToColorKey(faceColor);
                colorCount.Add(colorKey);
                if (!colorGroups.TryGetValue(colorKey, out var group))
                {
                    group = new FaceColorGroup([faceColor.x, faceColor.y, faceColor.z]);
                    colorGroups.Add(colorKey, group);
                }
                group.Faces.Add(face);
            }
        }

        var colorIndex = 0;
        foreach (var group in colorGroups.Values)
        {
            var colorShape = ShapeBuilder.MakeCompound(group.Faces)
                ?? throw new InvalidOperationException($"AnyCAD 无法生成 {name} 的颜色分组形状。");
            _cadResources.Add(colorShape);
            shapesForCache.Add(colorShape);

            var cachedShape = new CachedShape
            {
                BrepFile = $"color-{colorIndex++}.brep",
                ShapeColor = group.Color
            };
            var displayNode = CreateDisplayNode(name, colorShape, cachedShape);
            faceCount += displayNode.GetFaceCount();
            partGroup.AddNode(displayNode);
            cacheManifest.Shapes.Add(cachedShape);
            group.Dispose();
        }

        var cacheSaved = TrySaveBrepCache(cacheDirectory, shapesForCache, cacheManifest);
        return new PartLoadResult(
            partGroup,
            false,
            cacheSaved
                ? $"{name} 已解析：{faceCount} 个面，{colorCount.Count} 种 STEP 颜色（缓存已生成）"
                : $"{name} 已解析：{faceCount} 个面，{colorCount.Count} 种 STEP 颜色");
    }

    private BrepSceneNode CreateDisplayNode(string name, TopoShape shape, CachedShape cachedShape)
    {
        // Every cached shape contains faces of one STEP color, so optimized mesh
        // generation is safe and no longer depends on AnyCAD's internal face order.
        var displayNode = BrepSceneNode.Create(shape, DisplayDeflectionMillimeters, true)
            ?? throw new InvalidOperationException($"AnyCAD 无法生成 {name} 的显示节点。");

        displayNode.SetPickable(false);
        displayNode.SetSubShapePickable(false);
        displayNode.SetCastShadow(false);

        using var shapeColor = ToVector(cachedShape.ShapeColor);
        displayNode.SetColor(shapeColor);
        displayNode.RequestUpdate();
        return displayNode;
    }

    private static Vector3 ToVector(float[] values)
    {
        // XDE returns STEP colors in linear RGB, while AnyCAD's default CAD
        // material accepts display/sRGB values (ColorTable.Hex follows sRGB).
        return new Vector3(
            LinearToSrgb(values[0]),
            LinearToSrgb(values[1]),
            LinearToSrgb(values[2]));
    }

    private static Matrix4d CreatePoseMatrix(PoseConfiguration pose)
    {
        // KUKA ABC convention: Rz(A) * Ry(B) * Rx(C).
        var a = pose.A * Math.PI / 180.0;
        var b = pose.B * Math.PI / 180.0;
        var c = pose.C * Math.PI / 180.0;
        var ca = Math.Cos(a);
        var sa = Math.Sin(a);
        var cb = Math.Cos(b);
        var sb = Math.Sin(b);
        var cc = Math.Cos(c);
        var sc = Math.Sin(c);

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

    private static float LinearToSrgb(float value)
    {
        var linear = Math.Clamp(value, 0.0f, 1.0f);
        return linear <= 0.0031308f
            ? 12.92f * linear
            : 1.055f * MathF.Pow(linear, 1.0f / 2.4f) - 0.055f;
    }

    private static string GetCacheDirectory(string name, string sourcePath)
    {
        var info = new System.IO.FileInfo(sourcePath);
        var normalizedPath = IOPath.GetFullPath(sourcePath).ToUpperInvariant();
        var pathHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalizedPath)))[..12];
        var cacheRoot = IOPath.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RobotSimulator.AnyCAD",
            "ModelCache");
        System.IO.Directory.CreateDirectory(cacheRoot);

        return IOPath.Combine(
            cacheRoot,
            $"{name}-{pathHash}-{info.Length:x}-{info.LastWriteTimeUtc.Ticks:x}-{CacheVersion}");
    }

    private bool TryLoadBrepCache(string name, string cacheDirectory, out SceneNode cachedNode, out uint faceCount)
    {
        cachedNode = null!;
        faceCount = 0;
        var manifestPath = IOPath.Combine(cacheDirectory, "colors.json");
        if (!IOFile.Exists(manifestPath))
        {
            return false;
        }

        GroupSceneNode? partGroup = null;
        try
        {
            var manifest = JsonSerializer.Deserialize<PartCacheManifest>(
                IOFile.ReadAllText(manifestPath),
                CacheJsonOptions);
            if (manifest is null || manifest.Version != CacheVersion || manifest.Shapes.Count == 0)
            {
                return false;
            }

            partGroup = new GroupSceneNode();
            partGroup.SetName(name);
            partGroup.SetPickable(false);

            foreach (var cachedShape in manifest.Shapes)
            {
                var brepPath = IOPath.Combine(cacheDirectory, cachedShape.BrepFile);
                var shape = ShapeIO.Open(brepPath)
                    ?? throw new System.IO.InvalidDataException($"BREP 缓存无效：{brepPath}");
                _cadResources.Add(shape);

                var displayNode = CreateDisplayNode(name, shape, cachedShape);
                faceCount += displayNode.GetFaceCount();
                partGroup.AddNode(displayNode);
            }

            cachedNode = partGroup;
            return true;
        }
        catch (Exception exception)
        {
            partGroup?.Dispose();
            AppLog.Write($"BREP 缓存读取失败，将重新解析 STEP：{cacheDirectory}；{exception.Message}");
            return false;
        }
    }

    private static bool TrySaveBrepCache(
        string cacheDirectory,
        IReadOnlyList<TopoShape> shapes,
        PartCacheManifest manifest)
    {
        try
        {
            System.IO.Directory.CreateDirectory(cacheDirectory);
            for (var index = 0; index < shapes.Count; index++)
            {
                var brepPath = IOPath.Combine(cacheDirectory, manifest.Shapes[index].BrepFile);
                if (!ShapeIO.Save(shapes[index], brepPath))
                {
                    AppLog.Write($"BREP 缓存写入失败：{brepPath}");
                    return false;
                }
            }

            IOFile.WriteAllText(
                IOPath.Combine(cacheDirectory, "colors.json"),
                JsonSerializer.Serialize(manifest, CacheJsonOptions));
            return true;
        }
        catch (Exception exception)
        {
            AppLog.Write($"BREP 缓存写入失败：{cacheDirectory}；{exception.Message}");
            return false;
        }
    }

    private static (int R, int G, int B) ToColorKey(Vector3 color)
    {
        return (
            (int)Math.Round(color.x * 255),
            (int)Math.Round(color.y * 255),
            (int)Math.Round(color.z * 255));
    }

    private readonly record struct PartLoadResult(SceneNode Node, bool FromCache, string Message);

    private sealed class PartCacheManifest
    {
        public string Version { get; set; } = string.Empty;

        public List<CachedShape> Shapes { get; set; } = [];
    }

    private sealed class CachedShape
    {
        public string BrepFile { get; set; } = string.Empty;

        public float[] ShapeColor { get; set; } = [];
    }

    private sealed class FaceColorGroup(float[] color) : IDisposable
    {
        public float[] Color { get; } = color;

        public TopoShapeList Faces { get; } = [];

        public void Dispose()
        {
            Faces.Dispose();
        }
    }
}

public sealed class RobotSceneService : IDisposable
{
    private readonly RenderControl _renderControl;
    private RobotSceneContent? _content;
    private Background? _background;
    private bool _viewportInitialized;
    private bool _disposed;

    public RobotSceneService(RenderControl renderControl)
    {
        _renderControl = renderControl;
    }

    public bool IsLoaded => _content is not null && _content.JointGroups.Count == 6;

    public void InitializeViewport()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_viewportInitialized)
        {
            return;
        }

        _background = Background.Create(EnumBackgroundTheme.LightBlue);
        _renderControl.SceneManager.SetBackground(_background);
        // Cube-only is the compact option; AxisAndCube produces the oversized RGB triad.
        _renderControl.SetViewCube(EnumViewCoordinateType.Cube);
        _renderControl.ShowCoordinateGrid(false);
        _viewportInitialized = true;
        _renderControl.RequestDraw(EnumUpdateFlags.Scene);
    }

    public async Task<bool> LoadAsync(
        RobotConfiguration configuration,
        string modelDirectory,
        IProgress<RobotLoadProgress>? progress = null)
    {
        var content = await Task.Run(() => RobotSceneContent.Load(
            configuration,
            modelDirectory,
            state => progress?.Report(state)));

        if (_disposed)
        {
            content.Dispose();
            throw new ObjectDisposedException(nameof(RobotSceneService));
        }

        Attach(content);
        return content.HasTool;
    }

    public void Attach(RobotSceneContent content)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Clear();
        _content = content;

        InitializeViewport();
        _renderControl.ShowSceneNode(content.Root);
        _renderControl.ZoomAll(1.15f);
    }

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

    public void ApplyJoint(JointViewModel joint)
    {
        if (!IsLoaded)
        {
            return;
        }

        ApplyJointTransform(joint, requestDraw: true);
    }

    public void Clear()
    {
        ClearCore(isShuttingDown: false);
    }

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
            // AnyCAD.WPF may release its internal viewer before WPF raises the
            // final window lifecycle callback. The native scene is already gone.
            AppLog.Write($"关闭时 AnyCAD 视图已先释放，跳过重复 ClearScene：{exception.Message}");
        }
        finally
        {
            content.Dispose();
        }
    }

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
        // The split STEP files retain the pose of the source assembly. KUKA axis
        // values are controller coordinates, so only their delta from that CAD
        // reference pose may be applied to the scene graph.
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
