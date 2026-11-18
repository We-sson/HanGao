using System.Collections;
using System.Reflection;
using Macad.Occt;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>检查内嵌 CAD 显示缓存（.cad）、逐面颜色索引与关节/位姿变换，不打开真实三维视口。</summary>
internal static class CadResourceChecks
{
    public static void Run()
    {
        Check(ModelResources.RobotIds.Count > 0 && ModelResources.ToolIds.Count > 0 && ModelResources.WorkIds.Count > 0,
            "内置模型资源为空。");

        var loadedParts = 0;
        foreach (var robotId in ModelResources.RobotIds)
        {
            var configuration = RobotConfigurationStore.LoadBuiltIn(robotId);
            var files = new[] { configuration.BaseFile }.Concat(configuration.Joints.Select(joint => joint.ModelFile)).ToArray();
            Check(files.Length == 7, $"{robotId} 应该包含底座和六个关节模型。");
            foreach (var file in files)
            {
                using var source = ModelResources.OpenRobot(robotId, file);
                CheckPart($"{robotId}/{file}", CadModelPart.Load(file, source));
                loadedParts++;
            }
        }

        foreach (var toolId in ModelResources.ToolIds)
        {
            var configuration = ToolConfigurationStore.LoadBuiltIn(toolId);
            using var source = ModelResources.OpenTool(toolId, configuration.ModelFile);
            CheckPart($"ToolModels/{toolId}", CadModelPart.Load(configuration.ToolName, source));
            loadedParts++;
        }

        foreach (var workId in ModelResources.WorkIds)
        {
            var configuration = WorkConfigurationStore.LoadBuiltIn(workId);
            using var source = ModelResources.OpenWork(workId, configuration.ModelFile);
            CheckPart($"WorkModels/{workId}", CadModelPart.Load(configuration.WorkName, source));
            loadedParts++;
        }

        var names = (IDictionary?)typeof(ModelResources)
            .GetField("ResourceNames", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)
            ?? throw new InvalidOperationException("无法读取内嵌资源索引。");
        var resourceNames = names.Keys.Cast<string>().ToArray();
        var expected = 7 * ModelResources.RobotIds.Count + ModelResources.ToolIds.Count + ModelResources.WorkIds.Count;
        Check(loadedParts == expected, $"应该加载 {expected} 个显示缓存，实际 {loadedParts} 个。");
        Check(resourceNames.Count(name => name.EndsWith(".cad", StringComparison.OrdinalIgnoreCase)) == expected,
            $"内嵌 CAD 显示缓存数量应为 {expected}。");
        Check(resourceNames.All(name => !name.EndsWith(".glb", StringComparison.OrdinalIgnoreCase)
                                        && !name.EndsWith(".STEP", StringComparison.OrdinalIgnoreCase)),
            "仍然内嵌了旧的 STEP/GLB 模型资源。");

        CheckTransforms();
        Console.WriteLine("PASS: embedded .cad display caches, per-face colors and joint/pose transforms");
    }

    static void CheckPart(string name, CadModelPart part)
    {
        Check(part.Package.FaceCount > 0, $"{name} 没有可显示的面。");
        var faces = new TopTools_IndexedMapOfShape();
        TopExp.MapShapes(part.Package.Shape, TopAbs_ShapeEnum.FACE, faces);
        Check(faces.Size() == part.Package.FaceCount, $"{name} 的颜色索引与 BREP 面数不一致。");

        Check(!part.Bounds.IsVoid() && !part.Bounds.IsWhole(), $"{name} 的包围盒无效。");
        var min = part.Bounds.CornerMin();
        var max = part.Bounds.CornerMax();
        double[] size = [max.X - min.X, max.Y - min.Y, max.Z - min.Z];
        Check(size.All(value => double.IsFinite(value) && value > 0), $"{name} 的包围盒尺寸无效。");
        Check(size.Max() < 1e6, $"{name} 仍包含无限面。");

        for (var face = 1; face <= part.Package.FaceCount; face++)
        {
            var slot = part.Package.ColorSlot(face);
            Check(part.Package.ColorCount == 0
                    ? slot == 0
                    : slot >= 0 && slot < part.Package.ColorCount,
                $"{name} 第 {face} 个面的颜色槽位越界。");
        }
    }

    static void CheckTransforms()
    {
        var configuration = RobotConfigurationStore.LoadBuiltIn("KR_10_R1440-2");
        var jointConfig = configuration.Joints[0];
        var joint = new JointViewModel(0, jointConfig);
        CheckIdentity(RobotSceneContent.JointTransform(joint), "关节处于模型参考角时变换不是单位矩阵。");

        joint.Angle = jointConfig.ModelReferenceAngle + 90;
        var rotation = RobotSceneContent.JointTransform(joint);
        var origin = new Pnt(jointConfig.AxisOrigin.X, jointConfig.AxisOrigin.Y, jointConfig.AxisOrigin.Z);
        var axis = Normalize(jointConfig.AxisDirection.X, jointConfig.AxisDirection.Y, jointConfig.AxisDirection.Z);
        var helper = Math.Abs(axis.X) > 0.9 ? (X: 0.0, Y: 1.0, Z: 0.0) : (X: 1.0, Y: 0.0, Z: 0.0);
        var perpendicular = Normalize(axis.Y * helper.Z - axis.Z * helper.Y, axis.Z * helper.X - axis.X * helper.Z,
            axis.X * helper.Y - axis.Y * helper.X);

        var probe = new Pnt(origin.X + perpendicular.X * 10, origin.Y + perpendicular.Y * 10, origin.Z + perpendicular.Z * 10);
        var rotated = probe.Transformed(rotation);
        Check(Near(Distance(rotated, origin), 10), "关节旋转改变了点到转轴的距离。");
        var onAxis = origin.Transformed(rotation);
        Check(Near(onAxis.X, origin.X) && Near(onAxis.Y, origin.Y) && Near(onAxis.Z, origin.Z), "关节旋转移动了转轴上的点。");
        Check(!Near(rotated.X, probe.X) || !Near(rotated.Y, probe.Y) || !Near(rotated.Z, probe.Z), "关节角度变化没有产生旋转。");

        CheckIdentity(RobotSceneContent.PoseTransform(new PoseConfiguration()), "零姿态变换不是单位矩阵。");
        var translated = Pnt.Origin.Transformed(RobotSceneContent.PoseTransform(new PoseConfiguration { X = 10, Y = 20, Z = 30 }));
        Check(Near(translated.X, 10) && Near(translated.Y, 20) && Near(translated.Z, 30), "姿态平移没有写入世界变换。");
        var turned = new Pnt(1, 0, 0).Transformed(RobotSceneContent.PoseTransform(new PoseConfiguration { A = 90 }));
        Check(Near(turned.X, 0) && Near(turned.Y, 1) && Near(turned.Z, 0), "A 角没有按 Z 轴旋转。");
    }

    static void CheckIdentity(Trsf transform, string message)
    {
        foreach (var point in new[] { Pnt.Origin, new Pnt(1, 0, 0), new Pnt(0, 1, 0), new Pnt(0, 0, 1) })
        {
            var moved = point.Transformed(transform);
            Check(Near(moved.X, point.X) && Near(moved.Y, point.Y) && Near(moved.Z, point.Z), message);
        }
    }

    static (double X, double Y, double Z) Normalize(double x, double y, double z)
    {
        var length = Math.Sqrt(x * x + y * y + z * z);
        Check(length > 1e-12, "关节轴方向为 0。");
        return (x / length, y / length, z / length);
    }

    static double Distance(Pnt first, Pnt second)
    {
        var dx = first.X - second.X;
        var dy = first.Y - second.Y;
        var dz = first.Z - second.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    static bool Near(double first, double second) => Math.Abs(first - second) <= 1e-6;

    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
