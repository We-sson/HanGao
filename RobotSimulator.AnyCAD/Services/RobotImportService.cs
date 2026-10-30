using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RobotSimulator.Models;

namespace RobotSimulator.Services;

/// <summary>开发工具的源文件入口，不参与仿真运行时的机器人发现。</summary>
public static class RobotImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static string GetConfigurationPath(string directory) =>
        Path.Combine(directory, $"{new DirectoryInfo(directory).Name}.robot.json");

    public static IReadOnlyList<string> GetModelFiles(string directory)
    {
        if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("请选择存在的机器人源模型目录。");
        return Directory.EnumerateFiles(directory)
            .Where(path => Path.GetExtension(path).Equals(".STEP", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName).OfType<string>().Order(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public static RobotConfiguration? LoadExisting(string directory)
    {
        var path = GetConfigurationPath(directory);
        if (!File.Exists(path)) return null;
        using var stream = File.OpenRead(path);
        var configuration = JsonSerializer.Deserialize<RobotConfiguration>(stream, JsonOptions)
            ?? throw new InvalidDataException("机器人配置为空，请检查源文件。");
        RobotConfigurationStore.Validate(configuration);
        return configuration;
    }

    public static void ValidateFiles(string directory, string baseFile, IEnumerable<string> jointFiles)
    {
        var files = new[] { baseFile }.Concat(jointFiles).ToArray();
        if (files.Length != 7 || files.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 7)
            throw new InvalidDataException("请分别为 Base 和 J1～J6 选择七个不同的 STEP 文件。");
        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file) || Path.GetFileName(file) != file
                || !Path.GetExtension(file).Equals(".STEP", StringComparison.OrdinalIgnoreCase)
                || !File.Exists(Path.Combine(directory, file)))
                throw new InvalidDataException($"源目录中缺少 STEP 文件：{file}。");
            if (new FileInfo(Path.Combine(directory, file)).Length == 0)
                throw new InvalidDataException($"STEP 文件为空：{file}。");
        }
    }

    public static string Save(string directory, RobotConfiguration configuration)
    {
        RobotConfigurationStore.Validate(configuration);
        ValidateFiles(directory, configuration.BaseFile, configuration.Joints.Select(joint => joint.ModelFile));
        var path = GetConfigurationPath(directory);
        var temporaryPath = path + $".{Guid.NewGuid():N}.tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(configuration, JsonOptions));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally { File.Delete(temporaryPath); }
        return path;
    }
}
