using AnyCAD.Foundation;
using System.Security.Cryptography;
using System.Text.Json;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace RobotSimulator.Services;

/// <summary>一个独立 CAD 部件的几何、颜色和原生资源；工具可以单独替换和释放。</summary>
public sealed class CadModelPart : IDisposable
{
    /// <summary>网格弦高误差，单位 mm，兼顾显示细节和交互速度。</summary>
    private const double DisplayDeflectionMillimeters = 1.0;
    /// <summary>机器人、Tool 和工装统一按装配体提取颜色，旧缓存需要重建。</summary>
    private const string CacheVersion = "v8-assembly-colors-1mm";
    /// <summary>XDE 文档的创建与销毁使用应用级注册表，须串行进入。</summary>
    private static readonly object XdeDocumentSync = new();
    /// <summary>颜色缓存清单使用紧凑 JSON。</summary>
    private static readonly JsonSerializerOptions CacheJsonOptions = new() { WriteIndented = false };
    /// <summary>原生几何及文档按反序释放，使文档比引用它的几何活得更久。</summary>
    private readonly List<IDisposable> _cadResources = [];
    /// <summary>内容哈希缓存的根目录。</summary>
    private readonly string _cacheRoot;
    /// <summary>防止重复释放原生资源。</summary>
    private bool _disposed;

    /// <summary>该部件的显示节点，调用方可将它挂接到机器人或工具安装节点。</summary>
    public SceneNode Node { get; private set; } = null!;
    /// <summary>本次是否直接读取了 BREP 缓存。</summary>
    public bool FromCache { get; private set; }
    /// <summary>供日志和加载进度显示的说明。</summary>
    public string LoadMessage { get; private set; } = string.Empty;

    /// <summary>保存缓存目录，实际读取通过 Load 完成。</summary>
    private CadModelPart(string cacheRoot) => _cacheRoot = cacheRoot;

    /// <summary>读取一份独立资源，异常时释放已创建的原生对象，不影响原先正在显示的工具。</summary>
    /// <param name="progress">可选的分阶段进度回调，用于在长时间解析时更新界面。</param>
    public static CadModelPart Load(string name, System.IO.Stream source, string? cacheRoot = null,
        Action<RobotLoadProgress>? progress = null)
    {
        var part = new CadModelPart(cacheRoot ?? IOPath.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RobotSimulator.AnyCAD", "ModelCache"));
        try
        {
            var result = part.LoadPart(name, source, progress);
            part.Node = result.Node;
            part.FromCache = result.FromCache;
            part.LoadMessage = result.Message;
            return part;
        }
        catch { part.Dispose(); throw; }
    }

    /// <summary>先释放显示节点，再反序释放几何和文档；机器人切换和工具替换分别管理各自生命周期。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Node?.Dispose();
        for (var index = _cadResources.Count - 1; index >= 0; index--)
        {
            if (_cadResources[index] is XdeDocumentI document)
            {
                lock (XdeDocumentSync) document.Dispose();
            }
            else _cadResources[index].Dispose();
        }
        _cadResources.Clear();
    }

    /// <summary>读取一个部件：先尝试内容哈希对应的 BREP 缓存，缺失时解析 STEP、提取颜色并生成缓存。</summary>
    private PartLoadResult LoadPart(string name, System.IO.Stream source, Action<RobotLoadProgress>? progress)
    {
        progress?.Invoke(new RobotLoadProgress(0, 1, "正在校验模型缓存…", false));
        // 使用文件内容而非路径或程序时间作为缓存标识，普通代码重新构建不会让模型缓存失效。
        var contentHash = Convert.ToHexString(SHA256.HashData(source));
        var cacheDirectory = GetCacheDirectory(name, contentHash, CacheVersion);
        if (TryLoadBrepCache(name, cacheDirectory, CacheVersion, out var cachedNode, out var cachedFaceCount))
        {
            return new PartLoadResult(cachedNode, true, $"{name} 已从 BREP 缓存载入：{cachedFaceCount} 个面");
        }

        progress?.Invoke(new RobotLoadProgress(0, 1, "正在解析 STEP 文件…", false));
        XdeDocumentI document;
        lock (XdeDocumentSync) document = new XdeDocumentI();
        _cadResources.Add(document);
        // XDE 只能从路径读取；先写临时 STEP 并关闭文件流，解析结束后立即删除临时文件。
        var path = IOPath.Combine(IOPath.GetTempPath(), $"RobotSimulator-{Guid.NewGuid():N}.STEP");
        try
        {
            using (var temporaryFile = new System.IO.FileStream(path, System.IO.FileMode.CreateNew))
            {
                source.Position = 0;
                source.CopyTo(temporaryFile);
            }
            if (!document.Open(path))
                throw new System.IO.InvalidDataException($"AnyCAD 无法读取内置 {name} 模型。");
        }
        finally
        {
            IOFile.Delete(path);
        }

        var partGroup = new GroupSceneNode();
        Node = partGroup;
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

        // 将同色的面收集到同一组，既保留原始颜色，也避免复合网格优化后按面索引错配颜色。
        var colorGroups = new Dictionary<(int R, int G, int B), FaceColorGroup>();
        try
        {
            progress?.Invoke(new RobotLoadProgress(0, 1, "正在提取颜色和面…", false));
            for (var shapeIndex = 0; shapeIndex < shapeCount; shapeIndex++)
            {
                using var shapeNode = document.GetShapeNode(shapeIndex);
                using var fallback = new Vector3(0.72f, 0.72f, 0.72f);
                // 单零件导出也可能保留装配层级，颜色挂在子产品上；统一递归读取，避免退回灰色。
                CollectAssemblyColors(document, shapeNode, null, fallback, colorGroups, 0);
            }

            foreach (var key in colorGroups.Keys)
            {
                colorCount.Add(key);
            }

            // 每种颜色单独生成显示节点及 BREP 文件，缓存只保存几何和颜色，加载时仍需生成显示网格。
            progress?.Invoke(new RobotLoadProgress(0, 1, "正在生成显示网格…", false));
            var colorValues = colorGroups.Values.ToArray();
            var colorShapes = new TopoShape[colorValues.Length];
            var cachedShapes = new CachedShape[colorValues.Length];
            for (var index = 0; index < colorValues.Length; index++)
            {
                var colorShape = ShapeBuilder.MakeCompound(colorValues[index].Faces)
                    ?? throw new InvalidOperationException($"AnyCAD 无法生成 {name} 的颜色分组形状。");
                _cadResources.Add(colorShape);
                shapesForCache.Add(colorShape);
                colorShapes[index] = colorShape;
                cachedShapes[index] = new CachedShape
                {
                    BrepFile = $"color-{index}.brep",
                    ShapeColor = colorValues[index].Color
                };
            }

            // 各颜色组互不共享几何，可并行 tessellate；节点挂接仍留在当前线程。
            var displayNodes = new BrepSceneNode[colorValues.Length];
            Parallel.For(0, colorValues.Length, index =>
                displayNodes[index] = CreateDisplayNode(name, colorShapes[index], cachedShapes[index]));

            for (var index = 0; index < displayNodes.Length; index++)
            {
                _cadResources.Add(displayNodes[index]);
                faceCount += displayNodes[index].GetFaceCount();
                partGroup.AddNode(displayNodes[index]);
                cacheManifest.Shapes.Add(cachedShapes[index]);
            }

            progress?.Invoke(new RobotLoadProgress(0, 1, "正在写入模型缓存…", false));
            var cacheSaved = TrySaveBrepCache(cacheDirectory, shapesForCache, cacheManifest);
            return new PartLoadResult(
                partGroup,
                false,
                cacheSaved
                    ? $"{name} 已解析：{faceCount} 个面，{colorCount.Count} 种 STEP 颜色（缓存已生成）"
                    : $"{name} 已解析：{faceCount} 个面，{colorCount.Count} 种 STEP 颜色");
        }
        finally
        {
            // 临时面列表在解析失败时也要及时释放，工具可以反复更换而不积累原生资源。
            foreach (var group in colorGroups.Values) group.Dispose();
        }
    }

    /// <summary>递归展开 CAD 装配实例，保留位置、实例颜色和逐面颜色，再按颜色合并。</summary>
    /// <param name="document">保留装配关系和颜色信息的 XDE 文档。</param>
    /// <param name="node">当前装配或零件实例。</param>
    /// <param name="parentTransform">上层装配累计的变换；根层为空。</param>
    /// <param name="inheritedColor">当前节点未指定颜色时使用的上层颜色。</param>
    /// <param name="colorGroups">最终颜色分组，坐标均统一到模型原始坐标系。</param>
    /// <param name="depth">当前递归深度，用来阻止异常装配无限展开。</param>
    private void CollectAssemblyColors(
        XdeDocumentI document,
        XdeNode node,
        GTrsf? parentTransform,
        Vector3 inheritedColor,
        Dictionary<(int R, int G, int B), FaceColorGroup> colorGroups,
        int depth)
    {
        if (depth > 128)
        {
            throw new System.IO.InvalidDataException("CAD 装配层级过深或包含循环引用。");
        }

        using var reference = document.IsReference(node) ? document.GetReferencedShape(node) : null;
        if (reference?.IsNullLabel() == true)
        {
            throw new System.IO.InvalidDataException("CAD 装配体包含无效的零件引用。");
        }

        // 实例可以覆盖零件定义的颜色；没有覆盖时，依次使用定义颜色和上层默认颜色。
        var definition = reference ?? node;
        using var definitionColor = document.GetColor(definition, inheritedColor);
        using var occurrenceColor = reference is null ? null : document.GetColor(node, definitionColor);
        var shapeColor = occurrenceColor ?? definitionColor;

        if (document.IsAssembly(definition))
        {
            using var transform = new GTrsf();
            if (parentTransform is not null)
            {
                transform.Multiply(parentTransform);
            }
            // 累计上层装配和当前实例的位置，保证展开后各零件仍按 SW 装配关系摆放。
            using var location = node.GetLocation();
            transform.Multiply(location);

            using var children = new XdeNodeIterator(definition);
            while (children.More())
            {
                using var child = children.Value();
                CollectAssemblyColors(document, child, transform, shapeColor, colorGroups, depth + 1);
                children.Next();
            }
            return;
        }

        var instanceShape = document.GetShape(node)
            ?? throw new System.IO.InvalidDataException("CAD 装配零件没有几何形状。");
        _cadResources.Add(instanceShape);
        var definitionShape = reference is null ? instanceShape : document.GetShape(definition)
            ?? throw new System.IO.InvalidDataException("CAD 装配零件定义没有几何形状。");
        if (reference is not null)
        {
            _cadResources.Add(definitionShape);
        }

        // 几何取实例的位置，颜色优先查定义中的对应面；配对检查可避免把颜色赋给错误的面。
        using var faces = instanceShape.GetChildren(EnumTopoShapeType.Topo_FACE);
        using var definitionFaces = reference is null ? null : definitionShape.GetChildren(EnumTopoShapeType.Topo_FACE);
        if (definitionFaces is not null && definitionFaces.Count != faces.Count)
        {
            throw new System.IO.InvalidDataException("CAD 装配零件与颜色定义的面数量不一致。");
        }

        // 有上层变换时先局部收集，再变换到模型坐标，避免同色但位置不同的实例混淆。
        var groups = parentTransform is null
            ? colorGroups
            : new Dictionary<(int R, int G, int B), FaceColorGroup>();
        var solidFaceColors = GetSolidFaceColors(document, definitionShape, shapeColor);
        try
        {
            for (var index = 0; index < faces.Count; index++)
            {
                using var face = faces[index];
                using var definitionFace = definitionFaces is null ? null : definitionFaces[index];
                if (definitionFace is not null && !face.IsPartnerShape(definitionFace))
                {
                    throw new System.IO.InvalidDataException("CAD 装配零件与颜色定义的面不匹配。");
                }

                var colorFace = definitionFace ?? face;
                using var bodyColor = solidFaceColors.TryGetValue(colorFace.HashCode(), out var rgb)
                    ? new Vector3(rgb.R, rgb.G, rgb.B) : null;
                using var faceColor = document.GetFaceColor(colorFace, bodyColor ?? shapeColor);
                var key = ToColorKey(faceColor);
                if (!groups.TryGetValue(key, out var group))
                {
                    group = new FaceColorGroup([faceColor.x, faceColor.y, faceColor.z]);
                    groups.Add(key, group);
                }
                group.Faces.Add(face);
            }

            if (parentTransform is not null)
            {
                foreach (var (key, group) in groups)
                {
                    var shape = ShapeBuilder.MakeCompound(group.Faces)
                        ?? throw new InvalidOperationException("AnyCAD 无法生成 CAD 装配的颜色分组形状。");
                    _cadResources.Add(shape);
                    var transformed = TransformTool.Transform(shape, parentTransform, false)
                        ?? throw new InvalidOperationException("AnyCAD 无法定位 CAD 装配零件。");
                    _cadResources.Add(transformed);
                    if (!colorGroups.TryGetValue(key, out var target))
                    {
                        target = new FaceColorGroup(group.Color);
                        colorGroups.Add(key, target);
                    }
                    target.Faces.Add(transformed);
                }
            }
        }
        finally
        {
            if (parentTransform is not null)
            {
                foreach (var group in groups.Values)
                {
                    group.Dispose();
                }
            }
        }
    }

    /// <summary>读取复合形状中各实体的继承色；未归属实体的散面仍使用原有的根颜色。</summary>
    private static Dictionary<ulong, (float R, float G, float B)> GetSolidFaceColors(
        XdeDocumentI document, TopoShape shape, Vector3 fallback)
    {
        var colors = new Dictionary<ulong, (float R, float G, float B)>();
        using var solids = shape.GetChildren(EnumTopoShapeType.Topo_SOLID);
        for (var index = 0; index < solids.Count; index++)
        {
            using var solid = solids[index];
            // SDK 的 GetFaceColor 同样支持实体；面上的显式颜色在最终读取时覆盖该继承色。
            using var color = document.GetFaceColor(solid, fallback);
            using var faces = solid.GetChildren(EnumTopoShapeType.Topo_FACE);
            for (var faceIndex = 0; faceIndex < faces.Count; faceIndex++)
            {
                using var face = faces[faceIndex];
                colors[face.HashCode()] = (color.x, color.y, color.z);
            }
        }
        return colors;
    }

    /// <summary>将一个同色 BREP 几何生成可显示的网格节点，关闭拾取和实时阴影以减少交互负担。</summary>
    private BrepSceneNode CreateDisplayNode(string name, TopoShape shape, CachedShape cachedShape)
    {
        // 一个节点只含一种颜色，网格优化即使调整面顺序，也不会再导致颜色错位。
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

    /// <summary>把缓存中的线性 RGB 转换成显示材质使用的 sRGB 三分量颜色。</summary>
    private static Vector3 ToVector(float[] values)
    {
        // XDE 返回线性颜色，默认 CAD 材质使用 sRGB；直接照搬会让颜色明显偏暗。
        return new Vector3(
            LinearToSrgb(values[0]),
            LinearToSrgb(values[1]),
            LinearToSrgb(values[2]));
    }

    /// <summary>按 sRGB 标准曲线转换一个颜色分量，并把输入限制在 0～1。</summary>
    private static float LinearToSrgb(float value)
    {
        var linear = Math.Clamp(value, 0.0f, 1.0f);
        return linear <= 0.0031308f
            ? 12.92f * linear
            : 1.055f * MathF.Pow(linear, 1.0f / 2.4f) - 0.055f;
    }

    /// <summary>组合“部件名 + 文件内容哈希 + 缓存版本”目录，区分型号几何并复用相同的资源。</summary>
    private string GetCacheDirectory(string name, string contentHash, string cacheVersion)
    {
        System.IO.Directory.CreateDirectory(_cacheRoot);
        return IOPath.Combine(_cacheRoot, $"{name}-{contentHash}-{cacheVersion}");
    }

    /// <summary>尝试读取几何和颜色缓存；不完整、格式不匹配或损坏时返回 false，让调用方重新解析 STEP。</summary>
    private bool TryLoadBrepCache(string name, string cacheDirectory, string cacheVersion, out SceneNode cachedNode, out uint faceCount)
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
            if (manifest is null || manifest.Version != cacheVersion || manifest.Shapes.Count == 0)
            {
                return false;
            }

            partGroup = new GroupSceneNode();
            partGroup.SetName(name);
            partGroup.SetPickable(false);

            var cachedShapes = manifest.Shapes.ToArray();
            var cacheShapes = new TopoShape[cachedShapes.Length];
            for (var index = 0; index < cachedShapes.Length; index++)
            {
                var brepPath = IOPath.Combine(cacheDirectory, cachedShapes[index].BrepFile);
                cacheShapes[index] = ShapeIO.Open(brepPath)
                    ?? throw new System.IO.InvalidDataException($"BREP 缓存无效：{brepPath}");
                _cadResources.Add(cacheShapes[index]);
            }

            // 各颜色组互不共享几何，可并行 tessellate；节点挂接仍留在当前线程。
            var displayNodes = new BrepSceneNode[cachedShapes.Length];
            Parallel.For(0, cachedShapes.Length, index =>
                displayNodes[index] = CreateDisplayNode(name, cacheShapes[index], cachedShapes[index]));
            foreach (var displayNode in displayNodes)
            {
                _cadResources.Add(displayNode);
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

    /// <summary>先保存各颜色组的 BREP，成功后写颜色清单；缓存写入失败只影响下次加载速度。</summary>
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

    /// <summary>将浮点颜色量化成三通道整数，作为颜色分组字典的键，减少浮点微小差异。</summary>
    private static (int R, int G, int B) ToColorKey(Vector3 color)
    {
        return (
            (int)Math.Round(color.x * 255),
            (int)Math.Round(color.y * 255),
            (int)Math.Round(color.z * 255));
    }

    /// <summary>一个部件加载完成后的结果，包含显示节点、缓存命中情况和进度提示。</summary>
    /// <param name="Node">已生成并等待加入机器人层级的显示节点。</param>
    /// <param name="FromCache">是否跳过 STEP 解析而读取了 BREP 缓存。</param>
    /// <param name="Message">提供给界面和日志的部件加载说明。</param>
    private readonly record struct PartLoadResult(SceneNode Node, bool FromCache, string Message);

    /// <summary>colors.json 对应的缓存清单，将缓存格式版本和同色几何文件关联起来。</summary>
    private sealed class PartCacheManifest
    {
        /// <summary>缓存格式版本，与当前读取器期望值不一致时重新生成缓存。</summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>该部件所有颜色分组的几何文件及颜色记录。</summary>
        public List<CachedShape> Shapes { get; set; } = [];
    }

    /// <summary>一个颜色分组的缓存信息，几何存 BREP 文件，颜色存在线性 RGB 数组中。</summary>
    private sealed class CachedShape
    {
        /// <summary>相对于部件缓存目录的 BREP 文件名。</summary>
        public string BrepFile { get; set; } = string.Empty;

        /// <summary>该几何组共用的线性 RGB 颜色，顺序为 R、G、B，每项通常为 0～1。</summary>
        public float[] ShapeColor { get; set; } = [];
    }

    /// <summary>解析期间临时收集同色的面；创建显示几何后释放面列表包装。</summary>
    /// <param name="color">当前分组共用的线性 RGB 颜色。</param>
    private sealed class FaceColorGroup(float[] color) : IDisposable
    {
        /// <summary>该组所有面共用的颜色，不因装配位置变化而改变。</summary>
        public float[] Color { get; } = color;

        /// <summary>待合并的同色面或同色复合几何，合并时保留各自空间位置。</summary>
        public TopoShapeList Faces { get; } = [];

        /// <summary>释放临时原生面列表；最终几何和文档由场景资源列表管理。</summary>
        public void Dispose()
        {
            Faces.Dispose();
        }
    }
}
