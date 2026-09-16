using System.Text.Json;
using System.IO;

namespace RobotSimulator.Models;

public sealed class RobotConfiguration
{
    public string ModelName { get; set; } = "KUKA KR 10 R1440-2";

    public string BaseFile { get; set; } = "Base.STEP";

    public string ToolFile { get; set; } = "Tool.STEP";

    /// <summary>
    /// Transform from Tool.STEP local coordinates to the robot's CAD reference coordinates.
    /// Identity means the tool was exported in the same assembly coordinate system.
    /// </summary>
    public PoseConfiguration ToolModelPose { get; set; } = new();

    /// <summary>
    /// KUKA $TOOL frame relative to the A6 flange: X/Y/Z in mm and A/B/C in degrees.
    /// </summary>
    public PoseConfiguration ToolTcp { get; set; } = new();

    public List<JointConfiguration> Joints { get; set; } = [];
}

public sealed class PoseConfiguration
{
    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public double A { get; set; }

    public double B { get; set; }

    public double C { get; set; }
}

public sealed record RobotModelDefinition(
    string Id,
    string ConfigurationPath,
    string ModelDirectory,
    RobotConfiguration Configuration)
{
    public string DisplayName => Configuration.ModelName;
}

public sealed class JointConfiguration
{
    public string Name { get; set; } = string.Empty;

    public string ModelFile { get; set; } = string.Empty;

    public double Minimum { get; set; }

    public double Maximum { get; set; }

    public double HomeAngle { get; set; }

    /// <summary>
    /// KUKA controller angle represented by the untransformed STEP assembly.
    /// The displayed rotation is ControllerAngle - ModelReferenceAngle.
    /// </summary>
    public double ModelReferenceAngle { get; set; }

    public VectorConfiguration AxisOrigin { get; set; } = new();

    public VectorConfiguration AxisDirection { get; set; } = new();
}

public sealed class VectorConfiguration
{
    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }
}

public static class RobotConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static RobotConfiguration Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("找不到机器人配置文件。", path);
        }

        var json = File.ReadAllText(path);
        var configuration = JsonSerializer.Deserialize<RobotConfiguration>(json, SerializerOptions)
            ?? throw new InvalidDataException("机器人配置文件内容为空。");

        Validate(configuration);
        return configuration;
    }

    public static IReadOnlyList<RobotModelDefinition> Discover(string modelsRoot)
    {
        if (!Directory.Exists(modelsRoot))
        {
            throw new DirectoryNotFoundException($"找不到机器人模型根目录：{modelsRoot}");
        }

        var definitions = new List<RobotModelDefinition>();
        foreach (var modelDirectory in Directory.GetDirectories(modelsRoot).OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var id = Path.GetFileName(modelDirectory);
            var configurationPath = Path.Combine(modelDirectory, $"{id}.robot.json");
            if (!File.Exists(configurationPath))
            {
                continue;
            }

            definitions.Add(new RobotModelDefinition(
                id,
                configurationPath,
                modelDirectory,
                Load(configurationPath)));
        }

        if (definitions.Count == 0)
        {
            throw new InvalidDataException(
                $"未发现机器人配置。每个型号目录内必须包含与目录同名的 <型号>.robot.json：{modelsRoot}");
        }

        return definitions;
    }

    public static void Save(string path, RobotConfiguration configuration)
    {
        Validate(configuration);
        File.WriteAllText(path, JsonSerializer.Serialize(configuration, SerializerOptions));
    }

    private static void Validate(RobotConfiguration configuration)
    {
        if (configuration.Joints.Count != 6)
        {
            throw new InvalidDataException("配置必须包含 J1 到 J6 六个关节。");
        }

        for (var index = 0; index < configuration.Joints.Count; index++)
        {
            var joint = configuration.Joints[index];
            var expectedName = $"J{index + 1}";

            if (!string.Equals(joint.Name, expectedName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"第 {index + 1} 个关节必须命名为 {expectedName}。");
            }

            if (joint.Minimum >= joint.Maximum)
            {
                throw new InvalidDataException($"{joint.Name} 的最小角度必须小于最大角度。");
            }

            if (joint.HomeAngle < joint.Minimum || joint.HomeAngle > joint.Maximum)
            {
                throw new InvalidDataException($"{joint.Name} 的初始角度必须位于机械行程内。");
            }

            if (!double.IsFinite(joint.ModelReferenceAngle))
            {
                throw new InvalidDataException($"{joint.Name} 的 CAD 模型参考角必须是有限数值。");
            }

            var directionLength = Math.Sqrt(
                joint.AxisDirection.X * joint.AxisDirection.X
                + joint.AxisDirection.Y * joint.AxisDirection.Y
                + joint.AxisDirection.Z * joint.AxisDirection.Z);

            if (directionLength < 1e-9)
            {
                throw new InvalidDataException($"{joint.Name} 的旋转轴方向不能为零向量。");
            }
        }

        ValidatePose(configuration.ToolModelPose, "Tool 模型安装位姿");
        ValidatePose(configuration.ToolTcp, "Tool TCP");
    }

    private static void ValidatePose(PoseConfiguration pose, string name)
    {
        if (!double.IsFinite(pose.X)
            || !double.IsFinite(pose.Y)
            || !double.IsFinite(pose.Z)
            || !double.IsFinite(pose.A)
            || !double.IsFinite(pose.B)
            || !double.IsFinite(pose.C))
        {
            throw new InvalidDataException($"{name} 的 X/Y/Z/A/B/C 必须是有限数值。");
        }
    }
}
