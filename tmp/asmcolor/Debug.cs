using System.IO;
using AnyCAD.Foundation;
using RobotSimulator.Models;

internal static class DebugProbe
{
    public static void Run()
    {
        GlobalInstance.Initialize(true);
        using var source = ModelResources.OpenWork("双点焊工装", "双点焊工装.STEP");
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"dbg-{Guid.NewGuid():N}.STEP");
        using (var file = File.Create(path)) source.CopyTo(file);
        using var document = new XdeDocumentI();
        if (!document.Open(path)) throw new InvalidOperationException("open failed");
        File.Delete(path);

        void Walk(XdeNode node, int depth)
        {
            if (depth > 128) return;
            using var reference = document.IsReference(node) ? document.GetReferencedShape(node) : null;
            var definition = reference ?? node;
            if (document.IsAssembly(definition))
            {
                using var children = new XdeNodeIterator(definition);
                while (children.More())
                {
                    using var child = children.Value();
                    Walk(child, depth + 1);
                    children.Next();
                }
                return;
            }

            using var definitionShape = document.GetShape(definition);
            using var instanceShape = document.GetShape(node);
            using var definitionFaces = definitionShape?.GetChildren(EnumTopoShapeType.Topo_FACE);
            using var instanceFaces = instanceShape?.GetChildren(EnumTopoShapeType.Topo_FACE);
            Console.WriteLine($"leaf reference={reference is not null} definitionFaces={definitionFaces?.Count} instanceFaces={instanceFaces?.Count}");

            var directDefinition = definitionShape is null ? null : BrepSceneNode.Create(definitionShape, 1.0, true);
            Console.WriteLine($"Create(definitionShape)={(directDefinition is not null)}");
            directDefinition?.Dispose();

            var directInstance = instanceShape is null ? null : BrepSceneNode.Create(instanceShape, 1.0, true);
            Console.WriteLine($"Create(instanceShape)={(directInstance is not null)}");
            directInstance?.Dispose();

            if (definitionFaces is not null && definitionFaces.Count > 0)
            {
                using var list = new TopoShapeList();
                for (var index = 0; index < definitionFaces.Count; index++)
                    list.Add(definitionFaces[index]);
                var compound = ShapeBuilder.MakeCompound(list);
                Console.WriteLine($"MakeCompound={(compound is not null)}");
                if (compound is not null)
                {
                    using var compoundFaces = compound.GetChildren(EnumTopoShapeType.Topo_FACE);
                    Console.WriteLine($"compoundFaces={compoundFaces.Count}");
                    var nodeFromCompound = BrepSceneNode.Create(compound, 1.0, true);
                    Console.WriteLine($"Create(compound)={(nodeFromCompound is not null)}");
                    nodeFromCompound?.Dispose();
                }

                // 复现 M1 的写法：逐个 using 释放面包装后再合并。
                using (var listB = new TopoShapeList())
                {
                    for (var index = 0; index < definitionFaces.Count; index++)
                    {
                        using var face = definitionFaces[index];
                        listB.Add(face);
                    }
                    var compoundB = ShapeBuilder.MakeCompound(listB);
                    var nodeB = compoundB is null ? null : BrepSceneNode.Create(compoundB, 1.0, true);
                    Console.WriteLine($"dispose-then-compound Create={(nodeB is not null)}");
                    nodeB?.Dispose();
                }

                // 复现 M1 的写法：逐面调用 GetFaceColor 后再合并。
                using (var listC = new TopoShapeList())
                using (var fallback = new Vector3(0.72f, 0.72f, 0.72f))
                {
                    for (var index = 0; index < definitionFaces.Count; index++)
                    {
                        using var face = definitionFaces[index];
                        using var faceColor = document.GetFaceColor(face, fallback);
                        listC.Add(face);
                    }
                    var compoundC = ShapeBuilder.MakeCompound(listC);
                    var nodeC = compoundC is null ? null : BrepSceneNode.Create(compoundC, 1.0, true);
                    Console.WriteLine($"color-then-compound Create={(nodeC is not null)}");
                    nodeC?.Dispose();
                }
            }
        }

        Walk(document.GetShapeNode(0), 0);
    }
}
