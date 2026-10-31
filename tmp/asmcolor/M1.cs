using System.Diagnostics;
using System.IO;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;

internal static class M1Probe
{
    public static void Run(string mode)
    {
        GlobalInstance.Initialize(true);
        var process = Process.GetCurrentProcess();
        void Memory(string label)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            process.Refresh();
            Console.WriteLine($"{label,-12} ws={process.WorkingSet64 / 1048576} MB private={process.PrivateMemorySize64 / 1048576} MB");
        }

        if (mode == "flat")
        {
            var cacheRoot = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\perfcache3";
            using var flatStream = ModelResources.OpenWork("双点焊工装", "双点焊工装.STEP");
            using var part = CadModelPart.Load("Work", flatStream, cacheRoot);
            var flatMinX = double.MaxValue; var flatMinY = double.MaxValue; var flatMinZ = double.MaxValue;
            var flatMaxX = double.MinValue; var flatMaxY = double.MinValue; var flatMaxZ = double.MinValue;
            using var group = GroupSceneNode.Cast(part.Node);
            using var iterator = group.CreateIterator();
            while (iterator.More())
            {
                using var child = iterator.Current();
                using var brep = BrepSceneNode.Cast(child);
                if (brep is not null)
                {
                    using var local = brep.GetBoundingBox();
                    if (local.isFinite() && !local.isNull())
                    {
                        var low = local.getMinimum();
                        var high = local.getMaximum();
                        flatMinX = Math.Min(flatMinX, low.x); flatMaxX = Math.Max(flatMaxX, high.x);
                        flatMinY = Math.Min(flatMinY, low.y); flatMaxY = Math.Max(flatMaxY, high.y);
                        flatMinZ = Math.Min(flatMinZ, low.z); flatMaxZ = Math.Max(flatMaxZ, high.z);
                    }
                }
                iterator.Next();
            }
            Memory("flat");
            Console.WriteLine($"flat bbox: ({flatMinX:F1},{flatMinY:F1},{flatMinZ:F1}) - ({flatMaxX:F1},{flatMaxY:F1},{flatMaxZ:F1})");
            return;
        }

        using var source = ModelResources.OpenWork("双点焊工装", "双点焊工装.STEP");
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"m1-{Guid.NewGuid():N}.STEP");
        using (var file = File.Create(path)) source.CopyTo(file);
        using var document = new XdeDocumentI();
        if (!document.Open(path)) throw new InvalidOperationException("open failed");
        File.Delete(path);
        Memory("document");

        var root = new GroupSceneNode();
        var definitions = new Dictionary<ulong, List<BrepSceneNode>>();
        var kept = new List<IDisposable>();
        var instances = 0;
        long uniqueFaces = 0;
        var failedGroups = 0;
        var debugged = false;
        var minX = double.MaxValue; var minY = double.MaxValue; var minZ = double.MaxValue;
        var maxX = double.MinValue; var maxY = double.MinValue; var maxZ = double.MinValue;

        void Merge(AABox box, GTrsf? trsf)
        {
            if (box.isNull() || !box.isFinite()) return;
            var low = box.getMinimum();
            var high = box.getMaximum();
            for (var index = 0; index < 8; index++)
            {
                var x = (index & 1) == 0 ? low.x : high.x;
                var y = (index & 2) == 0 ? low.y : high.y;
                var z = (index & 4) == 0 ? low.z : high.z;
                trsf?.Transforms(ref x, ref y, ref z);
                minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
                minZ = Math.Min(minZ, z); maxZ = Math.Max(maxZ, z);
            }
        }

        Dictionary<ulong, (float R, float G, float B)> SolidFaceColors(TopoShape shape, Vector3 fallback)
        {
            var colors = new Dictionary<ulong, (float R, float G, float B)>();
            using var solids = shape.GetChildren(EnumTopoShapeType.Topo_SOLID);
            for (var index = 0; index < solids.Count; index++)
            {
                using var solid = solids[index];
                using var color = document.GetFaceColor(solid, fallback);
                using var solidFaces = solid.GetChildren(EnumTopoShapeType.Topo_FACE);
                for (var faceIndex = 0; faceIndex < solidFaces.Count; faceIndex++)
                {
                    using var face = solidFaces[faceIndex];
                    colors[face.HashCode()] = (color.x, color.y, color.z);
                }
            }
            return colors;
        }

        void Walk(XdeNode node, GTrsf? parent, int depth)
        {
            if (depth > 128) throw new InvalidOperationException("depth");
            using var reference = document.IsReference(node) ? document.GetReferencedShape(node) : null;
            var definition = reference ?? node;
            using var definitionShape = document.GetShape(definition)
                ?? throw new InvalidOperationException("definition shape");
            var key = definitionShape.HashCode();

            if (!definitions.TryGetValue(key, out var definitionNodes))
            {
                definitionNodes = [];
                if (!document.IsAssembly(definition))
                {
                    kept.Add(definitionShape);
                    using var fallback = new Vector3(0.72f, 0.72f, 0.72f);
                    using var shapeColor = document.GetColor(definition, fallback);
                    var solidColors = SolidFaceColors(definitionShape, shapeColor);
                    var groups = new Dictionary<(int, int, int), List<TopoShape>>();
                    using var faces = definitionShape.GetChildren(EnumTopoShapeType.Topo_FACE);
                    uniqueFaces += faces.Count;
                    for (var index = 0; index < faces.Count; index++)
                    {
                        // 面包装由 faces 集合统一释放，循环内不能提前 Dispose，否则后续合并不出几何。
                        var face = faces[index];
                        using var bodyColor = solidColors.TryGetValue(face.HashCode(), out var rgb)
                            ? new Vector3(rgb.R, rgb.G, rgb.B) : null;
                        using var faceColor = document.GetFaceColor(face, bodyColor ?? shapeColor);
                        var colorKey = ((int)Math.Round(faceColor.x * 255), (int)Math.Round(faceColor.y * 255), (int)Math.Round(faceColor.z * 255));
                        if (!groups.TryGetValue(colorKey, out var list)) groups[colorKey] = list = [];
                        list.Add(face);
                    }

                    if (!debugged)
                    {
                        debugged = true;
                        var direct = BrepSceneNode.Create(definitionShape, 1.0, true);
                        Console.WriteLine($"first leaf: def={key} faces={faces.Count} groups={groups.Count} direct={(direct is not null)}");
                        direct?.Dispose();
                    }

                    foreach (var (colorKey, colorFaces) in groups)
                    {
                        using var list = new TopoShapeList();
                        foreach (var face in colorFaces) list.Add(face);
                        var compound = ShapeBuilder.MakeCompound(list);
                        if (compound is null)
                        {
                            failedGroups++;
                            if (failedGroups <= 10) Console.WriteLine($"compound failed: def={key} color={colorKey} faces={colorFaces.Count}");
                            continue;
                        }
                        kept.Add(compound);
                        var display = BrepSceneNode.Create(compound, 1.0, true);
                        if (display is null)
                        {
                            display = BrepSceneNode.Create(compound, 1.0, false);
                            if (failedGroups < 10) Console.WriteLine($"optimized failed, plain={(display is not null)}: def={key} color={colorKey} faces={colorFaces.Count}");
                        }
                        if (display is null)
                        {
                            failedGroups++;
                            if (failedGroups <= 10) Console.WriteLine($"create failed: def={key} color={colorKey} faces={colorFaces.Count}");
                            continue;
                        }
                        kept.Add(display);
                        using var color = new Vector3(colorKey.Item1 / 255f, colorKey.Item2 / 255f, colorKey.Item3 / 255f);
                        display.SetColor(color);
                        display.RequestUpdate();
                        definitionNodes.Add(display);
                    }
                }
                definitions[key] = definitionNodes;
            }

            using var transform = new GTrsf();
            if (parent is not null) transform.Multiply(parent);
            using var location = node.GetLocation();
            transform.Multiply(location);

            if (document.IsAssembly(definition))
            {
                using var children = new XdeNodeIterator(definition);
                while (children.More())
                {
                    using var child = children.Value();
                    Walk(child, transform, depth + 1);
                    children.Next();
                }
                return;
            }

            var instanceGroup = new GroupSceneNode();
            instanceGroup.SetTransform(Matrix4d.From(transform));
            foreach (var definitionNode in definitionNodes)
            {
                using var local = definitionNode.GetBoundingBox();
                Merge(local, transform);
                instanceGroup.AddNode(definitionNode.Clone());
            }
            root.AddNode(instanceGroup);
            instances++;
        }

        Walk(document.GetShapeNode(0), null, 0);

        Memory("tree");
        Console.WriteLine($"instanced: definitions={definitions.Count} instances={instances} uniqueFaces={uniqueFaces} nodes={definitions.Values.Sum(v => v.Count)} failedGroups={failedGroups}");
        Console.WriteLine($"instanced bbox: ({minX:F1},{minY:F1},{minZ:F1}) - ({maxX:F1},{maxY:F1},{maxZ:F1})");
    }
}
