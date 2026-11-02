using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using AnyCAD.Foundation;
using RobotSimulator.Services;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

/// <summary>用小型真实几何检查不可写缓存降级，以及损坏缓存的原生资源回滚。</summary>
internal static class CadCacheRecoveryChecks
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly ConstructorInfo PartConstructor = typeof(CadModelPart).GetConstructor(
        PrivateInstance, null, [typeof(string)], null)!;
    private static readonly MethodInfo ReadCache = typeof(CadModelPart).GetMethod("TryLoadBrepCache", PrivateInstance)!;
    private static readonly FieldInfo Resources = typeof(CadModelPart).GetField("_cadResources", PrivateInstance)!;
    private static readonly string CacheVersion = (string)typeof(CadModelPart)
        .GetField("CacheVersion", BindingFlags.Static | BindingFlags.NonPublic)!.GetRawConstantValue()!;

    public static void Run(string fixture)
    {
        var root = Path.Combine(fixture, "cad-cache-recovery");
        Directory.CreateDirectory(root);
        using var axes = new GAx2();
        using var box = ShapeBuilder.MakeBox(axes, 1, 2, 3)
            ?? throw new InvalidOperationException("Cannot create the cache recovery box.");
        var step = Path.Combine(root, "box.STEP");
        Check(ShapeIO.Save(box, step), "Cannot save the cache recovery STEP.");

        // 同名文件使缓存目录无法创建；STEP 仍须完整加载，且不能覆盖该文件。
        var blockedCache = Path.Combine(root, "blocked-cache");
        File.WriteAllText(blockedCache, "keep this file");
        int coldResourceCount;
        using (var source = File.OpenRead(step))
        using (var part = CadModelPart.Load("Box", source, blockedCache))
        {
            Check(!part.FromCache, "An unavailable cache was reported as a cache hit.");
            CheckBox(part);
            coldResourceCount = GetResources(part).Count;
        }
        Check(File.ReadAllText(blockedCache) == "keep this file", "Cache fallback overwrote an existing file.");

        CheckDamagedCache(root, step, box, coldResourceCount, invalidColor: false);
        CheckDamagedCache(root, step, box, coldResourceCount, invalidColor: true);
        Console.WriteLine("PASS: unavailable cache fallback, missing BREP/invalid color resource rollback and warm cache recovery");
    }

    private static void CheckDamagedCache(string root, string step, TopoShape box, int coldResourceCount, bool invalidColor)
    {
        var cacheRoot = Path.Combine(root, invalidColor ? "invalid-color" : "missing-brep");
        var contentHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(step)));
        var cacheDirectory = Path.Combine(cacheRoot, $"Box-{contentHash}-{CacheVersion}");
        Directory.CreateDirectory(cacheDirectory);
        Check(ShapeIO.Save(box, Path.Combine(cacheDirectory, "color-0.brep")), "Cannot save the first recovery BREP.");
        var shapes = new List<(string BrepFile, float[] Color)> { ("color-0.brep", [1, 0, 0]) };
        if (invalidColor)
        {
            // 最后一组在创建网格后因颜色数组不完整而失败，前面的并行结果也必须释放。
            Check(ShapeIO.Save(box, Path.Combine(cacheDirectory, "color-1.brep")), "Cannot save the second recovery BREP.");
            Check(ShapeIO.Save(box, Path.Combine(cacheDirectory, "color-2.brep")), "Cannot save the third recovery BREP.");
            shapes.Add(("color-1.brep", [0, 1, 0]));
            shapes.Add(("color-2.brep", []));
        }
        else shapes.Add(("missing.brep", [0, 1, 0]));
        File.WriteAllText(Path.Combine(cacheDirectory, "colors.json"), JsonSerializer.Serialize(new
        {
            Version = CacheVersion,
            Shapes = shapes.Select(shape => new { shape.BrepFile, ShapeColor = shape.Color })
        }));

        using (var part = (CadModelPart)PartConstructor.Invoke([cacheRoot]))
        {
            var retained = new RetainedResource();
            var resources = GetResources(part);
            resources.Add(retained);
            object?[] arguments = ["Box", cacheDirectory, CacheVersion, null, 0u];
            Check(!(bool)ReadCache.Invoke(part, arguments)! && arguments[3] is null && (uint)arguments[4]! == 0,
                "A damaged cache did not return an empty failed result.");
            Check(resources.Count == 1 && ReferenceEquals(resources[0], retained) && !retained.Disposed,
                "Cache failure retained new geometry or released a resource it did not own.");
        }

        using (var source = File.OpenRead(step))
        using (var recovered = CadModelPart.Load("Box", source, cacheRoot))
        {
            Check(!recovered.FromCache, "A damaged cache bypassed STEP recovery.");
            CheckBox(recovered);
            Check(GetResources(recovered).Count == coldResourceCount,
                "STEP recovery retained resources from the failed cache attempt.");
        }
        using (var source = File.OpenRead(step))
        using (var warm = CadModelPart.Load("Box", source, cacheRoot))
        {
            Check(warm.FromCache, "STEP recovery did not repair its cache.");
            CheckBox(warm);
        }
    }

    private static List<IDisposable> GetResources(CadModelPart part) => (List<IDisposable>)Resources.GetValue(part)!;

    private static void CheckBox(CadModelPart part)
    {
        using var group = GroupSceneNode.Cast(part.Node)
            ?? throw new InvalidOperationException("Recovered box has no color group.");
        Check(group.GetCount() == 1, "Cache recovery changed the box color groups.");
        using var iterator = group.CreateIterator();
        using var child = iterator.Current();
        using var brep = BrepSceneNode.Cast(child)
            ?? throw new InvalidOperationException("Recovered box has no BREP node.");
        using var shape = brep.GetTopoShape();
        using var faces = shape.GetChildren(EnumTopoShapeType.Topo_FACE);
        Check(faces.Count == 6 && brep.GetFaceCount() == 6, "Cache recovery changed the box geometry.");
    }

    private sealed class RetainedResource : IDisposable
    {
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
