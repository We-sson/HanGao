using System.Reflection;

namespace RobotSimulator.Models;

/// <summary>固定机器人、Tool 和工装组合；宿主在本地配置一次，运行时只推送关节角度。</summary>
public sealed class RobotViewerOptions
{
    /// <summary>内置机器人型号。</summary>
    public RobotModelKind Robot { get; init; } = RobotModelKind.KR_20_R1810_2;

    /// <summary>内置 Tool；未配置时使用枚举的第一项。</summary>
    public ToolKind Tool { get; init; } = ToolKind.Default;

    /// <summary>内置工装；未配置时使用枚举的第一项。</summary>
    public WorkKind Work { get; init; } = WorkKind.RSideWelding;

    /// <summary>覆盖 Tool 配置中的 TCP；为空时使用内置配置。</summary>
    public PoseConfiguration? ToolTcp { get; init; }

    /// <summary>覆盖工装配置中的安装位姿；为空时使用内置配置。</summary>
    public PoseConfiguration? WorkPose { get; init; }

    /// <summary>是否显示机器人基坐标箭头。</summary>
    public bool ShowBaseCoordinates { get; init; } = true;

    /// <summary>是否显示法兰坐标箭头。</summary>
    public bool ShowFlangeCoordinates { get; init; } = true;

    /// <summary>是否显示 TCP 坐标箭头。</summary>
    public bool ShowTcpCoordinates { get; init; } = true;

    /// <summary>相机环绕速度，度/秒；0 表示不旋转，默认加载完成后缓慢旋转。</summary>
    public double OrbitSpeedDegreesPerSecond { get; init; } = 15;

    /// <summary>相机环绕起始方位角，单位度；默认 -45 对应标准等轴测方向。</summary>
    public double OrbitStartAngleDegrees { get; init; } = -45;

    /// <summary>相机环绕仰角，单位度；默认值对应标准等轴测方向。</summary>
    public double OrbitElevationDegrees { get; init; } = 35.264;
}

/// <summary>内置机器人型号；枚举名用于代码选择，资源标识由特性固定。</summary>
public enum RobotModelKind
{
    [ModelId("KR_10_R1420_HP")] KR_10_R1420_HP,
    [ModelId("KR_10_R1440-2")] KR_10_R1440_2,
    [ModelId("KR_20_R1810_2")] KR_20_R1810_2,
    [ModelId("KR_35_R1820_2")] KR_35_R1820_2,
    [ModelId("IRB2600_20_165")] IRB2600_20_165,
}

/// <summary>内置 Tool；当前只有默认工具。</summary>
public enum ToolKind
{
    [ModelId("Tool")] Default,
}

/// <summary>内置工装。</summary>
public enum WorkKind
{
    [ModelId("R边焊接工装")] RSideWelding,
    [ModelId("双点焊工装")] DoubleSpotWelding,
}

/// <summary>把枚举成员映射到内嵌资源目录标识，避免宿主手写字符串。</summary>
internal static class ModelCatalog
{
    /// <summary>读取枚举成员上的资源标识；缺少特性说明枚举与资源目录脱节。</summary>
    public static string GetId(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        return field?.GetCustomAttribute<ModelIdAttribute>()?.Id
            ?? throw new InvalidOperationException($"枚举 {value} 没有配置模型资源标识。");
    }
}

/// <summary>标记枚举成员对应的内嵌模型目录名。</summary>
[AttributeUsage(AttributeTargets.Field)]
internal sealed class ModelIdAttribute(string id) : Attribute
{
    /// <summary>Resources 下对应目录的名称。</summary>
    public string Id { get; } = id;
}
