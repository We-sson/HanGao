using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RobotSimulator.Models;

/// <summary>开发时确定并内嵌的固定机器人参数，不包含 Tool，不生成外部机器人配置。</summary>
public sealed class RobotConfiguration
{
    /// <summary>机器人型号名称。</summary>
    public required string ModelName { get; init; }
    /// <summary>当前机器人资源目录中的基座 STEP 文件名。</summary>
    public required string BaseFile { get; init; }
    /// <summary>CAD 全局坐标下的法兰安装中心，单位 mm。</summary>
    public required VectorConfiguration FlangeOrigin { get; init; }
    /// <summary>法兰在 CAD 原始姿态下的 ABC 朝向，单位度，同时决定 Tool 安装方向。</summary>
    public required OrientationConfiguration FlangeOrientation { get; init; }
    /// <summary>固定的 J1～J6 参数，顺序决定场景父子运动链。</summary>
    public required IReadOnlyList<JointConfiguration> Joints { get; init; }
}

/// <summary>三维位姿：XYZ 为 mm，ABC 为度，旋转顺序为 Rz(A)·Ry(B)·Rx(C)。</summary>
public sealed class PoseConfiguration
{
    /// <summary>沿所属坐标系 X 轴的平移，单位 mm。</summary>
    public double X { get; set; }
    /// <summary>沿所属坐标系 Y 轴的平移，单位 mm。</summary>
    public double Y { get; set; }
    /// <summary>沿所属坐标系 Z 轴的平移，单位 mm。</summary>
    public double Z { get; set; }
    /// <summary>组合旋转中绕 Z 轴的 A 角，单位度。</summary>
    public double A { get; set; }
    /// <summary>组合旋转中绕 Y 轴的 B 角，单位度。</summary>
    public double B { get; set; }
    /// <summary>组合旋转中绕 X 轴的 C 角，单位度。</summary>
    public double C { get; set; }
}

/// <summary>开发时确定的固定法兰方向。</summary>
public sealed class OrientationConfiguration
{
    /// <summary>绕 Z 轴的 A 角，单位度。</summary>
    public double A { get; init; }
    /// <summary>绕 Y 轴的 B 角，单位度。</summary>
    public double B { get; init; }
    /// <summary>绕 X 轴的 C 角，单位度。</summary>
    public double C { get; init; }
}

/// <summary>内置机器人的型号记录，没有外部配置路径。</summary>
/// <param name="Id">Resources/Robots 中的机器人目录名。</param>
/// <param name="Configuration">只从内嵌资源读取的固定参数。</param>
public sealed record RobotModelDefinition(string Id, RobotConfiguration Configuration)
{
    /// <summary>机器人下拉框显示名称。</summary>
    public string DisplayName => Configuration.ModelName;
}

/// <summary>开发时校准的固定关节参数，运行时只改变当前角度。</summary>
public sealed class JointConfiguration
{
    /// <summary>依次为 J1～J6 的关节名称。</summary>
    public required string Name { get; init; }
    /// <summary>当前机器人目录中的关节 STEP 文件名。</summary>
    public required string ModelFile { get; init; }
    /// <summary>最小示教器绝对角度，单位度。</summary>
    public double Minimum { get; init; }
    /// <summary>最大示教器绝对角度，单位度。</summary>
    public double Maximum { get; init; }
    /// <summary>启动和复位角度，单位度。</summary>
    public double HomeAngle { get; init; }
    /// <summary>原始 STEP 的示教器角度；实际模型转角等于当前角度减去此值。</summary>
    public double ModelReferenceAngle { get; init; }
    /// <summary>CAD 全局坐标下旋转轴线上的一点，单位 mm。</summary>
    public required VectorConfiguration AxisOrigin { get; init; }
    /// <summary>按右手规则决定正转方向的非零向量。</summary>
    public required VectorConfiguration AxisDirection { get; init; }
}

/// <summary>固定的三分量数据，用于机器人轴线位置或方向。</summary>
public sealed class VectorConfiguration
{
    /// <summary>X 坐标或方向分量。</summary>
    public double X { get; init; }
    /// <summary>Y 坐标或方向分量。</summary>
    public double Y { get; init; }
    /// <summary>Z 坐标或方向分量。</summary>
    public double Z { get; init; }
}

/// <summary>只读机器人配置入口，不提供外部加载、保存、覆盖或旧版迁移。</summary>
public static class RobotConfigurationStore
{
    /// <summary>开发配置严格符合当前字段结构，多余字段直接报错。</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    /// <summary>从内嵌资源读取固定参数并校验，不读取程序目录中的机器人 JSON。</summary>
    public static RobotConfiguration LoadBuiltIn(string modelId)
    {
        using var stream = ModelResources.OpenRobot(modelId, $"{modelId}.robot.json");
        var configuration = JsonSerializer.Deserialize<RobotConfiguration>(stream, SerializerOptions)
            ?? throw new InvalidDataException("内置机器人配置内容为空。");
        Validate(configuration);
        return configuration;
    }

    /// <summary>枚举内置机器人，不创建任何外部配置文件。</summary>
    public static IReadOnlyList<RobotModelDefinition> Discover()
    {
        var models = ModelResources.RobotIds.Select(id => new RobotModelDefinition(id, LoadBuiltIn(id))).ToArray();
        if (models.Length == 0) throw new InvalidDataException("程序未包含机器人资源，请在 Resources/Robots 中添加型号并重新构建。");
        return models;
    }

    /// <summary>开发配置有误时尽早给出原因，避免非法数据进入原生几何接口。</summary>
    internal static void Validate(RobotConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.ModelName) || string.IsNullOrWhiteSpace(configuration.BaseFile)
            || configuration.Joints is null || configuration.Joints.Count != 6)
            throw new InvalidDataException("机器人必须具有型号名称、基座和 J1～J6 六个关节。");
        for (var index = 0; index < configuration.Joints.Count; index++)
        {
            var joint = configuration.Joints[index];
            if (joint is null || joint.Name != $"J{index + 1}" || string.IsNullOrWhiteSpace(joint.ModelFile))
                throw new InvalidDataException($"第 {index + 1} 个关节的名称或模型文件无效。");
            if (!double.IsFinite(joint.Minimum) || !double.IsFinite(joint.Maximum)
                || !double.IsFinite(joint.HomeAngle) || !double.IsFinite(joint.ModelReferenceAngle)
                || joint.Minimum >= joint.Maximum || joint.HomeAngle < joint.Minimum || joint.HomeAngle > joint.Maximum)
                throw new InvalidDataException($"{joint.Name} 的行程、初始角或 CAD 参考角无效。");
            ValidateVector(joint.AxisOrigin, $"{joint.Name} 轴心");
            ValidateVector(joint.AxisDirection, $"{joint.Name} 方向");
            var direction = joint.AxisDirection;
            var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
            if (!double.IsFinite(length) || length < 1e-9)
                throw new InvalidDataException($"{joint.Name} 的方向向量不能为零或无限大。");
        }
        ValidateVector(configuration.FlangeOrigin, "法兰中心");
        var orientation = configuration.FlangeOrientation;
        if (orientation is null || !double.IsFinite(orientation.A) || !double.IsFinite(orientation.B) || !double.IsFinite(orientation.C))
            throw new InvalidDataException("法兰方向的 ABC 必须是有限数值。");
    }

    /// <summary>检查全局坐标或方向中的 XYZ。</summary>
    private static void ValidateVector(VectorConfiguration? vector, string name)
    {
        if (vector is null || !double.IsFinite(vector.X) || !double.IsFinite(vector.Y) || !double.IsFinite(vector.Z))
            throw new InvalidDataException($"{name} 的 XYZ 必须是有限数值。");
    }
}
