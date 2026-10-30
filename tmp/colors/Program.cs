using System.IO;
using AnyCAD.Foundation;

GlobalInstance.Initialize(true);

foreach (var path in args)
{
    Console.WriteLine($"== {System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path))}/{System.IO.Path.GetFileName(path)} ==");
    using var document = new XdeDocumentI();
    if (!document.Open(path)) { Console.WriteLine("  open failed"); continue; }
    var shapeCount = document.GetShapeCount();
    Console.WriteLine($"  shapeCount={shapeCount}");
    for (var index = 0; index < shapeCount; index++)
    {
        using var node = document.GetShapeNode(index);
        using var fallback = new Vector3(0.72f, 0.72f, 0.72f);
        using var nodeColor = document.GetColor(node, fallback);
        using var shape = document.GetShape(node);
        using var faces = shape.GetChildren(EnumTopoShapeType.Topo_FACE);
        var keys = new HashSet<(int, int, int)>();
        var buckets = new Dictionary<(int, int, int), int>();
        for (var faceIndex = 0; faceIndex < faces.Count; faceIndex++)
        {
            using var face = faces[faceIndex];
            using var faceColor = document.GetFaceColor(face, nodeColor);
            var key = ((int)Math.Round(faceColor.x * 255), (int)Math.Round(faceColor.y * 255), (int)Math.Round(faceColor.z * 255));
            keys.Add(key);
            buckets[key] = buckets.GetValueOrDefault(key) + 1;
        }
        Console.WriteLine($"  shape{index}: nodeColor=({nodeColor.x:F3},{nodeColor.y:F3},{nodeColor.z:F3}) faces={faces.Count} distinctNodeFaces={keys.Count}");
        foreach (var (key, count) in buckets.OrderByDescending(pair => pair.Value).Take(6))
            Console.WriteLine($"      rgb({key.Item1},{key.Item2},{key.Item3}) x{count}");
    }
}
