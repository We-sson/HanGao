using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;

/// <summary>验证 HP 演示配置的内嵌资源、已测轴心和运动链；不代替控制器零点校准。</summary>
internal static class RobotHpChecks
{
    public static void Run(string fixture)
    {
        const string modelId = "KR_10_R1420_HP";
        var configuration = RobotConfigurationStore.Discover().Single(model => model.Id == modelId).Configuration;
        Check(configuration.Joints.Select(joint => joint.HomeAngle).SequenceEqual(new double[] { 0, -90, 90, 0, 0, 0 })
            && configuration.Joints.All(joint => joint.HomeAngle == joint.ModelReferenceAngle), "HP home changed the original STEP posture.");
        foreach (var file in new[] { configuration.BaseFile }.Concat(configuration.Joints.Select(joint => joint.ModelFile)))
        {
            using var source = ModelResources.OpenRobot(modelId, file);
            Check(source.Length > 0, $"HP asset is empty: {file}.");
        }
        using var scene = RobotSceneContent.Load(configuration, modelId, cacheRoot: System.IO.Path.Combine(fixture, "cache"));
        Check(scene.JointGroups.Count == 6 && !scene.HasTool, "HP robot hierarchy is incomplete.");
        using var basePart = FindGroup(scene.Root, "Base");
        Check(basePart.GetCount() > 0, "HP base geometry is missing.");
        for (var index = 0; index < 6; index++)
        {
            using var part = FindGroup(scene.JointGroups[index], $"J{index + 1}");
            Check(part.GetCount() > 0, $"HP J{index + 1} geometry is missing.");
        }
        using var flange = FindGroup(scene.JointGroups[5], "FlangeFrame");
        using var mount = FindGroup(flange, "ToolMount");
        using var tcp = FindGroup(flange, "TcpFrame");
        scene.SetToolTcp(new PoseConfiguration { Z = 100 });
        scene.Root.UpdateTransform(Matrix4d.Identity);
        double[] homeDirections = [0, 0, -1, 0, 1, 0, 1, 0, 0];
        AssertFrame(flange, [890, 0, 1080], homeDirections);
        AssertFrame(tcp, [990, 0, 1080], homeDirections);

        // 期望值由实测 CAD 尺寸独立计算；各测试角度均在当前演示行程内。
        (double Delta, double[] Origin, double[] Directions)[] cases =
        [
            (90, [0, -890, 1080], [0, 0, -1, 1, 0, 0, 0, -1, 0]),
            (90, [780, 0, -290], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
            (-90, [130, 0, 1800], [1, 0, 0, 0, 1, 0, 0, 0, 1]),
            (90, [890, 0, 1080], [0, -1, 0, 0, 0, -1, 1, 0, 0]),
            (90, [810, 0, 1000], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
            (90, [890, 0, 1080], [0, -1, 0, 0, 0, -1, 1, 0, 0])
        ];
        for (var index = 0; index < cases.Length; index++)
        {
            var joint = configuration.Joints[index];
            using var origin = new GPnt(joint.AxisOrigin.X, joint.AxisOrigin.Y, joint.AxisOrigin.Z);
            using var direction = new GDir(joint.AxisDirection.X, joint.AxisDirection.Y, joint.AxisDirection.Z);
            using var axis = new GAx1(origin, direction);
            using var rotation = new GTrsf();
            rotation.SetRotation(axis, cases[index].Delta * Math.PI / 180);
            using var matrix = Matrix4d.From(rotation);
            scene.JointGroups[index].SetTransform(matrix);
            scene.Root.UpdateTransform(Matrix4d.Identity);
            AssertFrame(flange, cases[index].Origin, cases[index].Directions);
            AssertFrame(mount, cases[index].Origin, cases[index].Directions);
            var tcpOrigin = Enumerable.Range(0, 3).Select(coordinate =>
                cases[index].Origin[coordinate] + 100 * cases[index].Directions[6 + coordinate]).ToArray();
            AssertFrame(tcp, tcpOrigin, cases[index].Directions);
            scene.JointGroups[index].SetTransform(Matrix4d.Identity);
        }
        Console.WriteLine("PASS: KR 10 R1420 HP embedded assets, CAD home, flange/TCP and six joint motion chains");
    }

    private static GroupSceneNode FindGroup(GroupSceneNode parent, string name)
    {
        using var iterator = parent.CreateIterator();
        while (iterator.More())
        {
            using var node = iterator.Current();
            if (node.GetName() == name)
                return GroupSceneNode.Cast(node) ?? throw new InvalidOperationException($"{name} is not a group.");
            iterator.Next();
        }
        throw new InvalidOperationException($"HP group {name} is missing.");
    }

    private static void AssertFrame(SceneNode frame, double[] origin, double[] directions)
    {
        using var transform = frame.GetWorldTransform();
        for (var coordinate = 0; coordinate < 3; coordinate++)
        {
            Check(Math.Abs(transform.get(3, coordinate) - origin[coordinate]) < 0.001, $"HP {frame.GetName()} origin is incorrect.");
            for (var axis = 0; axis < 3; axis++)
                Check(Math.Abs(transform.get(axis, coordinate) - directions[axis * 3 + coordinate]) < 0.001,
                    $"HP {frame.GetName()} axis {axis}, component {coordinate} is incorrect.");
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
