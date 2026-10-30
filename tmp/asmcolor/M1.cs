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
        if (mode == "flat")
        {
            var cacheRoot = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\perfcache3";
            using var flatStream = ModelResources.OpenWork("双点焊工装", "双点焊工装.STEP");
            using var part = CadModelPart.Load("Work", flatStream, cacheRoot);
            var flatBox = part.Node.ComputeBoundingBox(Matrix4d.Identity);
            var flatMin = flatBox.getMinimum();
            var flatMax = flatBox.getMaximum();
            process.Refresh();
            Console.WriteLine($"flat: ws={process.WorkingSet64 / 1048576} MB private={process.PrivateMemorySize64 / 1048576} MB");
            Console.WriteLine($"flat bbox: ({flatMin.x:F1},{flatMin.y:F1},{flatMin.z:F1}) - ({flatMax.x:F1},{flatMax.y:F1},{flatMax.z:F1})");
            return;
        }

        using var source = ModelResources.OpenWork("双点焊工装", "双点焊工装.STEP");
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"m1-{Guid.NewGuid():N}.STEP");
        using (var file = File.Create(path)) source.CopyTo(file);
        using var document = new XdeDocumentI();
        if (!document.Open(path)) throw new InvalidOperationException("open failed");
        File.Delete(path);

        var root = new GroupSceneNode();
        var definitions = new Dictionary<ulong, List<BrepSceneNode>>();
        var kept = new List<IDisposable>();
        var instances = 0;
        long uniqueFaces = 0;
        var failedGroups = 0;

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
                kept.Add(definitionShape);
                using var fallback = new Vector3(0.72f, 0.72f, 0.72f);
                using var shapeColor = document.GetColor(definition, fallback);
                var solidColors = SolidFaceColors(definitionShape, shapeColor);
                var groups = new Dictionary<(int, int, int), List<TopoShape>>();
                using var faces = definitionShape.GetChildren(EnumTopoShapeType.Topo_FACE);
                uniqueFaces += faces.Count;
                for (var index = 0; index < faces.Count; index++)
                {
                    using var face = faces[index];
                    using var bodyColor = solidColors.TryGetValue(face.HashCode(), out var rgb)
                        ? new Vector3(rgb.R, rgb.G, rgb.B) : null;
                    using var faceColor = document.GetFaceColor(face, bodyColor ?? shapeColor);
                    var colorKey = ((int)Math.Round(faceColor.x * 255), (int)Math.Round(faceColor.y * 255), (int)Math.Round(faceColor.z * 255));
                    if (!groups.TryGetValue(colorKey, out var list)) groups[colorKey] = list = [];
                    list.Add(face);
                }

                definitionNodes = [];
                foreach (var (colorKey, colorFaces) in groups)
                {
                    using var list = new TopoShapeList();
                    foreach (var face in colorFaces) list.Add(face);
                    var compound = ShapeBuilder.MakeCompound(list);
                    if (compound is null) { failedGroups++; continue; }
                    kept.Add(compound);
                    var display = BrepSceneNode.Create(compound, 1.0, true);
                    if (display is null)
                    {
                        failedGroups++;
                        Console.WriteLine($"create failed: def={key} color={colorKey} faces={colorFaces.Count}");
                        continue;
                    }
                    kept.Add(display);
                    using var color = new Vector3(colorKey.Item1 / 255f, colorKey.Item2 / 255f, colorKey.Item3 / 255f);
                    display.SetColor(color);
                    display.RequestUpdate();
                    definitionNodes.Add(display);
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
                instanceGroup.AddNode(definitionNode.Clone());
            root.AddNode(instanceGroup);
            instances++;
        }

        Walk(document.GetShapeNode(0), null, 0);

        var box = root.ComputeBoundingBox(Matrix4d.Identity);
        var minPoint = box.getMinimum();
        var maxPoint = box.getMaximum();
        process.Refresh();
        Console.WriteLine($"instanced: definitions={definitions.Count} instances={instances} uniqueFaces={uniqueFaces} nodes={definitions.Values.Sum(v => v.Count)} failedGroups={failedGroups}");
        Console.WriteLine($"instanced: ws={process.WorkingSet64 / 1048576} MB private={process.PrivateMemorySize64 / 1048576} MB");
        Console.WriteLine($"instanced bbox: ({minPoint.x:F1},{minPoint.y:F1},{minPoint.z:F1}) - ({maxPoint.x:F1},{maxPoint.y:F1},{maxPoint.z:F1})");
    }
}
