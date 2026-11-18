using System.IO;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>验证 XML 缺轴、明确的零角度、异常数值与 DTO 复制不会混淆。</summary>
internal static class RobotAxisValidationChecks
{
    public static void Run()
    {
        var serializer = new XmlSerializer(typeof(Robot_Axis_Model), new XmlRootAttribute("Robot_Axis"));
        Robot_Axis_Model Read(string attributes)
        {
            using var reader = new StringReader($"<Robot_Axis {attributes} />");
            return (Robot_Axis_Model)serializer.Deserialize(reader)!;
        }

        var zero = Read("J1=\"0\" J2=\"0\" J3=\"0\" J4=\"0\" J5=\"0\" J6=\"0\"");
        Check(zero.HasCompleteFiniteAxes(), "明确提供六个零角度的报文应为有效帧。");
        Check(new Robot_Axis_Model(zero).HasCompleteFiniteAxes(), "复制丢失了完整角度标记。");
        var partial = Read("J1=\"12\" J2=\"-45\" J3=\"90\" J4=\"0\" J5=\"0\"");
        Check(!partial.HasCompleteFiniteAxes(), "缺少 J6 的 XML 被当成了 J6 为零的完整帧。");
        Check(!new Robot_Axis_Model(partial).HasCompleteFiniteAxes(), "复制把缺轴帧变成了完整帧。");
        Check(!Read(string.Empty).HasCompleteFiniteAxes(), "空轴节点被当成了完整零位。");
        Check(!Read("J1=\"NaN\" J2=\"0\" J3=\"0\" J4=\"0\" J5=\"0\" J6=\"0\"").HasCompleteFiniteAxes(),
            "NaN 角度没有被拒绝。");
        Check(!Read("J1=\"0\" J2=\"0\" J3=\"INF\" J4=\"0\" J5=\"0\" J6=\"0\"").HasCompleteFiniteAxes(),
            "无穷角度没有被拒绝。");

        var assigned = new Robot_Axis_Model { J1 = 1, J2 = 2, J3 = 3, J4 = 4, J5 = 5, J6 = 6 };
        Check(assigned.HasCompleteFiniteAxes(), "逐轴赋值的完整角度不应被拒绝。");
        using var writer = new StringWriter();
        serializer.Serialize(writer, assigned);
        using var roundtrip = new StringReader(writer.ToString());
        var restored = (Robot_Axis_Model)serializer.Deserialize(roundtrip)!;
        Check(restored.HasCompleteFiniteAxes() && restored.J1 == 1 && restored.J6 == 6,
            "DTO 序列化回读没有保留有效角度。");
        Console.WriteLine("PASS: complete finite robot axes, explicit zero, missing XML attributes and copy/serialization");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
