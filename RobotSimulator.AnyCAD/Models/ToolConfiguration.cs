using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RobotSimulator.Models;

/// <summary>属于工具本身的独立配置，可用于任意机器人；现场只需调整 TCP。</summary>
public sealed class ToolConfiguration
{
    /// <summary>工具下拉框显示名称。</summary>
    public required string ToolName { get; init; }
    /// <summary>当前工具资源目录中的 STEP 文件名，模型仍然内嵌。</summary>
    public required string ModelFile { get; init; }
    /// <summary>相对于安装法兰的工具中心，XYZ 为 mm，ABC 为度。</summary>
    public required PoseConfiguration Tcp { get; init; }
}

/// <summary>独立工具记录，切换机器人时仍使用同一份工具配置。</summary>
/// <param name="Id">Resources/Tools 中的工具目录名。</param>
/// <param name="ConfigurationPath">程序目录中可编辑 tool.json 的路径。</param>
/// <param name="Configuration">发现工具时读取的配置，重载时再次读取外部文件。</param>
public sealed record ToolModelDefinition(string Id, string ConfigurationPath, ToolConfiguration Configuration)
{
    /// <summary>工具下拉框显示的友好名称。</summary>
    public string DisplayName => Configuration.ToolName;
}

/// <summary>只生成、读取和保存独立 tool.json，不处理机器人参数或旧格式。</summary>
public static class ToolConfigurationStore
{
    /// <summary>严格采用当前工具结构，并缩进输出以便人工修改。</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    /// <summary>读取内置工具默认设置，用于首次生成或配置文件被删除后的恢复。</summary>
    public static ToolConfiguration LoadBuiltIn(string toolId)
    {
        var modelFile = ModelResources.GetToolModelFile(toolId);
        using var stream = ModelResources.OpenTool(toolId, GetConfigurationFileName(modelFile));
        var configuration = Read(stream);
        if (!string.Equals(configuration.ModelFile, modelFile, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"Tool {toolId} 的配置必须引用同名模型 {modelFile}。");
        return configuration;
    }

    /// <summary>读取工具外部配置，调用方负责提示格式和参数错误。</summary>
    public static ToolConfiguration Load(string path)
    {
        using var stream = File.OpenRead(path);
        var configuration = Read(stream);
        ValidateFileName(path, configuration);
        return configuration;
    }

    /// <summary>枚举内置工具，只为缺失的配置生成默认文件，已有 TCP 保持原样。</summary>
    public static IReadOnlyList<ToolModelDefinition> Discover(string toolsRoot) => ModelResources.ToolIds.Select(id =>
    {
        var defaults = LoadBuiltIn(id);
        var path = Path.Combine(toolsRoot, id, GetConfigurationFileName(defaults.ModelFile));
        if (!File.Exists(path)) Save(path, defaults);
        return new ToolModelDefinition(id, path, Load(path));
    }).ToArray();

    /// <summary>先校验，再写临时文件并替换正式文件，避免失败时破坏原配置。</summary>
    public static void Save(string path, ToolConfiguration configuration)
    {
        Validate(configuration);
        ValidateFileName(path, configuration);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var temporaryPath = path + $".{Guid.NewGuid():N}.tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(configuration, SerializerOptions));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally { File.Delete(temporaryPath); }
    }

    /// <summary>解析当前工具结构并校验，不执行旧字段转换。</summary>
    private static ToolConfiguration Read(Stream stream)
    {
        var configuration = JsonSerializer.Deserialize<ToolConfiguration>(stream, SerializerOptions)
            ?? throw new InvalidDataException("Tool 配置内容为空。");
        Validate(configuration);
        return configuration;
    }

    private static string GetConfigurationFileName(string modelFile) => $"{Path.GetFileNameWithoutExtension(modelFile)}.tool.json";

    private static void ValidateFileName(string path, ToolConfiguration configuration)
    {
        var expected = GetConfigurationFileName(configuration.ModelFile);
        if (!string.Equals(Path.GetFileName(path), expected, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"Tool 配置必须与模型同名：{configuration.ModelFile} 对应 {expected}。");
    }

    /// <summary>检查名称、模型文件名和 TCP 数值，拒绝将 NaN 或无穷大写入配置。</summary>
    internal static void Validate(ToolConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.ToolName) || string.IsNullOrWhiteSpace(configuration.ModelFile))
            throw new InvalidDataException("Tool 名称和模型资源文件名不能为空。");
        if (configuration.ModelFile.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || !configuration.ModelFile.EndsWith(".STEP", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(configuration.ModelFile)))
            throw new InvalidDataException("Tool 模型必须是本目录中的 STEP 文件名。");
        var tcp = configuration.Tcp;
        if (tcp is null || !double.IsFinite(tcp.X) || !double.IsFinite(tcp.Y) || !double.IsFinite(tcp.Z)
            || !double.IsFinite(tcp.A) || !double.IsFinite(tcp.B) || !double.IsFinite(tcp.C))
            throw new InvalidDataException("Tool TCP 的 XYZ/ABC 必须是有限数值。");
    }
}
