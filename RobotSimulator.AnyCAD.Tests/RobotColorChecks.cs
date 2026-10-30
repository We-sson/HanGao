using System.Text.Json;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

/// <summary>用原始 STEP 的实体色及面覆盖色，检查冷加载和缓存加载均保留机器人颜色。</summary>
internal static class RobotColorChecks
{
    public static void Run(string fixture)
    {
        // 计数来自 STEP 实体拓扑；显示网格可能跳过退化面，因此不使用网格面数。
        CheckPart(fixture, "Base", [(0.2, 0.2, 0.2, 5202), (0.4, 0.4, 0.4, 2)]);
        CheckPart(fixture, "J4", [(0.2, 0.2, 0.2, 116), (1, 127.0 / 255, 0, 169)]);
        Console.WriteLine("PASS: HP base and KUKA letter solid colors, face overrides, geometry counts and warm-cache colors");
    }

    private static void CheckPart(string fixture, string name, (double R, double G, double B, int Faces)[] expected)
    {
        var cache = Path.Combine(fixture, "robot-colors", name);
        using (var source = ModelResources.OpenRobot("KR_10_R1420_HP", name + ".STEP"))
        using (var cold = CadModelPart.Load(name, source, cache))
        {
            Check(!cold.FromCache, $"{name} did not exercise STEP color import.");
            CheckColors(cold.Node, cache, expected);
        }
        using (var source = ModelResources.OpenRobot("KR_10_R1420_HP", name + ".STEP"))
        using (var warm = CadModelPart.Load(name, source, cache))
        {
            Check(warm.FromCache, $"{name} did not reuse its color cache.");
            CheckColors(warm.Node, cache, expected);
        }
    }

    private static void CheckColors(SceneNode node, string cache, (double R, double G, double B, int Faces)[] expected)
    {
        var manifest = Directory.GetFiles(cache, "colors.json", System.IO.SearchOption.AllDirectories).Single();
        using var json = JsonDocument.Parse(File.ReadAllText(manifest));
        Check(json.RootElement.GetProperty("Version").GetString() != "v5-color-groups-1mm",
            "Old robot caches can still hide the repaired solid colors.");
        var shapes = json.RootElement.GetProperty("Shapes").EnumerateArray().ToArray();
        using var group = GroupSceneNode.Cast(node) ?? throw new InvalidOperationException("Robot part is not a color group.");
        Check(group.GetCount() == expected.Length && shapes.Length == expected.Length,
            $"Unexpected {node.GetName()} color groups; a solid color or face override was lost.");
        var found = new HashSet<int>();
        using var iterator = group.CreateIterator();
        var index = 0;
        while (iterator.More())
        {
            using var child = iterator.Current();
            using var brep = BrepSceneNode.Cast(child) ?? throw new InvalidOperationException("Color group has no BREP geometry.");
            using var topology = brep.GetTopoShape();
            using var faces = topology.GetChildren(EnumTopoShapeType.Topo_FACE);
            var actual = shapes[index++].GetProperty("ShapeColor").EnumerateArray().Select(value => value.GetDouble()).ToArray();
            var match = Array.FindIndex(expected, color => Near(actual[0], Linear(color.R))
                && Near(actual[1], Linear(color.G)) && Near(actual[2], Linear(color.B)));
            Check(match >= 0 && found.Add(match), $"Unexpected or duplicate {node.GetName()} color in cache.");
            Check(faces.Count == expected[match].Faces,
                $"{node.GetName()} color geometry changed: {faces.Count} faces instead of {expected[match].Faces}.");
            iterator.Next();
        }
        Check(found.Count == expected.Length, $"A {node.GetName()} source color is missing.");
    }

    private static double Linear(double srgb) => srgb <= 0.04045 ? srgb / 12.92 : Math.Pow((srgb + 0.055) / 1.055, 2.4);
    private static bool Near(double actual, double expected) => Math.Abs(actual - expected) < 0.00001;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
