using System.IO;

namespace RobotSimulator.Models;

/// <summary>内嵌资源入口；机器人、工具和工装使用独立目录与资源前缀。</summary>
public static class ModelResources
{
    /// <summary>包含全部模型和开发配置的当前程序集。</summary>
    private static readonly System.Reflection.Assembly ResourceAssembly = typeof(ModelResources).Assembly;
    /// <summary>统一路径分隔符后的资源索引。</summary>
    private static readonly Dictionary<string, string> ResourceNames = ResourceAssembly.GetManifestResourceNames()
        .Where(name => name.StartsWith("RobotModels/", StringComparison.Ordinal) || name.StartsWith("ToolModels/", StringComparison.Ordinal)
            || name.StartsWith("WorkModels/", StringComparison.Ordinal))
        .ToDictionary(name => name.Replace('\\', '/'), StringComparer.OrdinalIgnoreCase);

    /// <summary>由 robot.json 识别的内置机器人目录列表。</summary>
    public static IReadOnlyList<string> RobotIds { get; } = GetIds("RobotModels/", ".robot.json");
    /// <summary>由显示缓存识别的工具目录，每个目录包含一个模型和同名配置。</summary>
    public static IReadOnlyList<string> ToolIds { get; } = GetIds("ToolModels/", ".cad");
    /// <summary>每套工装包含一个装配显示缓存和同名安装配置。</summary>
    public static IReadOnlyList<string> WorkIds { get; } = GetIds("WorkModels/", ".cad");

    /// <summary>以原始 CAD 文件名定位显示缓存，配置 JSON 本身仍按原名打开。</summary>
    public static Stream OpenRobot(string id, string fileName) => Open($"RobotModels/{id}/{RuntimeFileName(fileName)}");

    /// <summary>打开工具内嵌文件，同一个工具资源可安装到任何机器人。</summary>
    public static Stream OpenTool(string id, string fileName) => Open($"ToolModels/{id}/{RuntimeFileName(fileName)}");
    /// <summary>打开内嵌工装显示缓存或其配置。</summary>
    public static Stream OpenWork(string id, string fileName) => Open($"WorkModels/{id}/{RuntimeFileName(fileName)}");

    internal static string GetToolModelFile(string id) => GetSingleModelFile("ToolModels/", id, "Tool");
    internal static string GetWorkModelFile(string id) => GetSingleModelFile("WorkModels/", id, "工装");

    private static string GetSingleModelFile(string resourcePrefix, string id, string kind)
    {
        var prefix = $"{resourcePrefix}{id}/";
        var files = ResourceNames.Keys.Where(name => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            && name.EndsWith(".cad", StringComparison.OrdinalIgnoreCase)).Select(name => name[prefix.Length..]).ToArray();
        if (files.Length != 1 || files[0].Contains('/'))
            throw new InvalidDataException($"{kind} {id} 的资源目录必须只包含一个 CAD 显示缓存。");
        return Path.ChangeExtension(files[0], ".STEP");
    }

    /// <summary>配置保留源文件名，运行时读取同名预转换显示缓存。</summary>
    private static string RuntimeFileName(string fileName) => Path.GetExtension(fileName).Equals(".STEP", StringComparison.OrdinalIgnoreCase)
        ? Path.ChangeExtension(fileName, ".cad") : fileName;

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
