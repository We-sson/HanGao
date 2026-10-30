using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RobotSimulator.Models;

/// <summary>独立工装的模型和安装位姿，坐标相对于机器人基坐标。</summary>
public sealed class WorkConfiguration
{
    public required string WorkName { get; init; }
    public required string ModelFile { get; init; }
    /// <summary>整体安装位姿：XYZ 为 mm，ABC 为度；全零保留 STEP 原有摆放。</summary>
    public required PoseConfiguration Pose { get; init; }
}

/// <summary>每套工装独立保存安装位置，可搭配不同机器人与 Tool。</summary>
public sealed record WorkModelDefinition(string Id, string ConfigurationPath, WorkConfiguration Configuration)
{
    public string DisplayName => Configuration.WorkName;
}

/// <summary>工装采用与 STEP 同名的 work.json；首次生成默认配置，已有配置不覆盖。</summary>
public static class WorkConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static WorkConfiguration LoadBuiltIn(string workId)
    {
        var modelFile = ModelResources.GetWorkModelFile(workId);
        using var stream = ModelResources.OpenWork(workId, GetConfigurationFileName(modelFile));
        var configuration = Read(stream);
        if (!string.Equals(configuration.ModelFile, modelFile, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"工装 {workId} 的配置必须引用同名模型 {modelFile}。");
        return configuration;
    }

    public static WorkConfiguration Load(string path)
    {
        using var stream = File.OpenRead(path);
        var configuration = Read(stream);
        ValidateFileName(path, configuration);
        return configuration;
    }

    public static IReadOnlyList<WorkModelDefinition> Discover(string workRoot) => ModelResources.WorkIds.Select(id =>
    {
        var defaults = LoadBuiltIn(id);
        var path = Path.Combine(workRoot, id, GetConfigurationFileName(defaults.ModelFile));
        if (!File.Exists(path)) Save(path, defaults);
        return new WorkModelDefinition(id, path, Load(path));
    }).ToArray();

    public static void Save(string path, WorkConfiguration configuration)
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

    private static WorkConfiguration Read(Stream stream)
    {
        var configuration = JsonSerializer.Deserialize<WorkConfiguration>(stream, SerializerOptions)
            ?? throw new InvalidDataException("工装配置内容为空。");
        Validate(configuration);
        return configuration;
    }

    private static string GetConfigurationFileName(string modelFile) => $"{Path.GetFileNameWithoutExtension(modelFile)}.work.json";

    private static void ValidateFileName(string path, WorkConfiguration configuration)
    {
        var expected = GetConfigurationFileName(configuration.ModelFile);
        if (!string.Equals(Path.GetFileName(path), expected, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"工装配置必须与模型同名：{configuration.ModelFile} 对应 {expected}。");
    }

    internal static void Validate(WorkConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.WorkName) || string.IsNullOrWhiteSpace(configuration.ModelFile))
            throw new InvalidDataException("工装名称和模型资源文件名不能为空。");
        if (configuration.ModelFile.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || !configuration.ModelFile.EndsWith(".STEP", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(configuration.ModelFile)))
            throw new InvalidDataException("工装模型必须是本目录中的 STEP 文件名。");
        var pose = configuration.Pose;
        if (pose is null || !double.IsFinite(pose.X) || !double.IsFinite(pose.Y) || !double.IsFinite(pose.Z)
            || !double.IsFinite(pose.A) || !double.IsFinite(pose.B) || !double.IsFinite(pose.C))
            throw new InvalidDataException("工装安装位置的 XYZ/ABC 必须是有限数值。");
    }
}
