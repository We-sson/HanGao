using System.IO;

namespace RobotSimulator.Models;

/// <summary>内嵌资源入口；机器人库和工具库使用独立目录和资源前缀。</summary>
public static class ModelResources
{
    /// <summary>包含全部模型和开发配置的当前程序集。</summary>
    private static readonly System.Reflection.Assembly ResourceAssembly = typeof(ModelResources).Assembly;
    /// <summary>统一路径分隔符后的资源索引。</summary>
    private static readonly Dictionary<string, string> ResourceNames = ResourceAssembly.GetManifestResourceNames()
        .Where(name => name.StartsWith("RobotModels/", StringComparison.Ordinal) || name.StartsWith("ToolModels/", StringComparison.Ordinal))
        .ToDictionary(name => name.Replace('\\', '/'), StringComparer.OrdinalIgnoreCase);

    /// <summary>由 robot.json 识别的内置机器人目录列表。</summary>
    public static IReadOnlyList<string> RobotIds { get; } = GetIds("RobotModels/", ".robot.json");
    /// <summary>由 STEP 识别的内置工具目录列表，每个目录包含一个模型和同名配置。</summary>
    public static IReadOnlyList<string> ToolIds { get; } = GetIds("ToolModels/", ".STEP");

    /// <summary>打开机器人内嵌文件；调用方负责释放数据流。</summary>
    public static Stream OpenRobot(string id, string fileName) => Open($"RobotModels/{id}/{fileName}");

    /// <summary>打开工具内嵌文件，同一个工具资源可安装到任何机器人。</summary>
    public static Stream OpenTool(string id, string fileName) => Open($"ToolModels/{id}/{fileName}");

    internal static string GetToolModelFile(string id)
    {
        var prefix = $"ToolModels/{id}/";
        var files = ResourceNames.Keys.Where(name => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            && name.EndsWith(".STEP", StringComparison.OrdinalIgnoreCase)).Select(name => name[prefix.Length..]).ToArray();
        if (files.Length != 1 || files[0].Contains('/'))
            throw new InvalidDataException($"Tool {id} 的资源目录必须只包含一个 STEP 模型。");
        return files[0];
    }

    /// <summary>提取并排序指定类型的资源目录标识。</summary>
    private static IReadOnlyList<string> GetIds(string prefix, string suffix) => ResourceNames.Keys
        .Where(name => name.StartsWith(prefix, StringComparison.Ordinal) && name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        .Select(name => name.Split('/')[1]).Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToArray();

    /// <summary>按完整资源名称读取，不回退到外部 STEP 文件。</summary>
    private static Stream Open(string name)
    {
        if (!ResourceNames.TryGetValue(name.Replace('\\', '/'), out var resourceName))
            throw new FileNotFoundException($"程序中未包含模型资源：{name}");
        return ResourceAssembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"无法读取模型资源：{name}");
    }
}
