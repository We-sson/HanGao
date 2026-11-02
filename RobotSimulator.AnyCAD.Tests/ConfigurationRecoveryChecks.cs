using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using RobotSimulator.Models;
#if DEBUG
using RobotSimulator.Services;
#endif

internal static class ConfigurationRecoveryChecks
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static void Run(string fixture)
    {
        CheckToolRecovery(Path.Combine(fixture, "recovery-tools"));
        CheckWorkRecovery(Path.Combine(fixture, "recovery-work"));
        CheckRequiredRobotNumbers(Path.Combine(fixture, "required-robot-numbers"));
        Console.WriteLine("PASS: corrupt attachment isolation, preserved files, recovery and required robot calibration numbers");
    }

    private static void CheckToolRecovery(string root)
    {
        var originalModels = ToolConfigurationStore.Discover(root);
        var damaged = originalModels.First();
        var original = File.ReadAllBytes(damaged.ConfigurationPath);
        var invalidConfiguration = JsonNode.Parse(Encoding.UTF8.GetString(original))!.AsObject();
        invalidConfiguration["toolName"] = string.Empty;
        foreach (var text in new[] { "{", invalidConfiguration.ToJsonString() })
        {
            var damagedBytes = Encoding.UTF8.GetBytes(text);
            File.WriteAllBytes(damaged.ConfigurationPath, damagedBytes);
            var warnings = new List<string>();
            var remaining = ToolConfigurationStore.Discover(root, warnings.Add);
            Check(remaining.Count == originalModels.Count - 1 && remaining.All(model => model.Id != damaged.Id),
                "A corrupt Tool was replaced with defaults or prevented other Tools from being discovered.");
            Check(warnings.Count == 1 && warnings[0].Contains(Path.GetFullPath(damaged.ConfigurationPath))
                && warnings[0].Split('\n').Last().Length > 0, "The Tool warning omitted its path or failure reason.");
            Check(damagedBytes.SequenceEqual(File.ReadAllBytes(damaged.ConfigurationPath)), "Discovery changed the corrupt Tool file.");
        }
        File.WriteAllBytes(damaged.ConfigurationPath, original);
        var recoveryWarnings = new List<string>();
        Check(ToolConfigurationStore.Discover(root, recoveryWarnings.Add).Count == originalModels.Count && recoveryWarnings.Count == 0,
            "Repairing a Tool configuration did not restore discovery.");

        // Tool/Work 的部分位姿仍允许省略零分量；只有机器人测量值要求逐项显式填写。
        File.WriteAllText(damaged.ConfigurationPath,
            $"{{\"toolName\":\"部分 TCP\",\"modelFile\":{JsonSerializer.Serialize(damaged.Configuration.ModelFile)},\"tcp\":{{\"z\":25}}}}");
        var partialTcp = ToolConfigurationStore.Load(damaged.ConfigurationPath).Tcp;
        Check(partialTcp.Z == 25 && partialTcp.X == 0 && partialTcp.A == 0, "Partial Tool TCP no longer defaults omitted components to zero.");
    }

    private static void CheckWorkRecovery(string root)
    {
        var originalModels = WorkConfigurationStore.Discover(root);
        Check(originalModels.Count > 1, "The recovery fixture needs at least two independent Work models.");
        var damaged = originalModels[0];
        var healthy = originalModels[1];
        WorkConfigurationStore.Save(healthy.ConfigurationPath, new WorkConfiguration
        {
            WorkName = healthy.DisplayName, ModelFile = healthy.Configuration.ModelFile, Pose = new PoseConfiguration { X = 125 }
        });
        var healthyBytes = File.ReadAllBytes(healthy.ConfigurationPath);
        var original = File.ReadAllBytes(damaged.ConfigurationPath);
        var invalidConfiguration = JsonNode.Parse(Encoding.UTF8.GetString(original))!.AsObject();
        invalidConfiguration["pose"] = null;
        foreach (var text in new[] { "{", invalidConfiguration.ToJsonString() })
        {
            var damagedBytes = Encoding.UTF8.GetBytes(text);
            File.WriteAllBytes(damaged.ConfigurationPath, damagedBytes);
            var warnings = new List<string>();
            var remaining = WorkConfigurationStore.Discover(root, warnings.Add);
            Check(remaining.Count == originalModels.Count - 1 && remaining.All(model => model.Id != damaged.Id)
                && remaining.Single(model => model.Id == healthy.Id).Configuration.Pose.X == 125,
                "A corrupt Work configuration prevented a healthy fixture from retaining its saved placement.");
            Check(warnings.Count == 1 && warnings[0].Contains(Path.GetFullPath(damaged.ConfigurationPath))
                && warnings[0].Split('\n').Last().Length > 0, "The Work warning omitted its path or failure reason.");
            Check(damagedBytes.SequenceEqual(File.ReadAllBytes(damaged.ConfigurationPath))
                && healthyBytes.SequenceEqual(File.ReadAllBytes(healthy.ConfigurationPath)), "Discovery changed an existing Work file.");
        }
        File.WriteAllBytes(damaged.ConfigurationPath, original);
        var recoveryWarnings = new List<string>();
        Check(WorkConfigurationStore.Discover(root, recoveryWarnings.Add).Count == originalModels.Count && recoveryWarnings.Count == 0,
            "Repairing a Work configuration did not restore discovery.");

        File.WriteAllText(damaged.ConfigurationPath,
            $"{{\"workName\":\"部分安装位姿\",\"modelFile\":{JsonSerializer.Serialize(damaged.Configuration.ModelFile)},\"pose\":{{\"x\":25}}}}");
        var partialPose = WorkConfigurationStore.Load(damaged.ConfigurationPath).Pose;
        Check(partialPose.X == 25 && partialPose.Y == 0 && partialPose.B == 0, "Partial Work pose no longer defaults omitted components to zero.");
    }

    private static void CheckRequiredRobotNumbers(string directory)
    {
        // 读取全部内置型号，确保现有有效配置完整，显式零值也能正常读取。
        var models = RobotConfigurationStore.Discover();
        var original = JsonSerializer.Serialize(models.Single(model => model.Id == "KR_10_R1440-2").Configuration, JsonOptions);
        Directory.CreateDirectory(directory);
        foreach (var property in new[] { "minimum", "maximum", "homeAngle", "modelReferenceAngle" })
            CheckMissingNumber(original, json => json["joints"]![1]!.AsObject(), property, directory);
        foreach (var property in new[] { "x", "y", "z" })
        {
            CheckMissingNumber(original, json => json["flangeOrigin"]!.AsObject(), property, directory);
            CheckMissingNumber(original, json => json["joints"]![1]!["axisOrigin"]!.AsObject(), property, directory);
            CheckMissingNumber(original, json => json["joints"]![1]!["axisDirection"]!.AsObject(), property, directory);
        }
        foreach (var property in new[] { "a", "b", "c" })
            CheckMissingNumber(original, json => json["flangeOrientation"]!.AsObject(), property, directory);
    }

    private static void CheckMissingNumber(string original, Func<JsonObject, JsonObject> selectObject, string property, string directory)
    {
        var json = JsonNode.Parse(original)!.AsObject();
        Check(selectObject(json).Remove(property), $"The fixture did not contain calibration field {property}.");
        var text = json.ToJsonString();
        try
        {
#if DEBUG
            var path = RobotImportService.GetConfigurationPath(directory);
            File.WriteAllText(path, text);
            RobotImportService.LoadExisting(directory);
#else
            JsonSerializer.Deserialize<RobotConfiguration>(text, JsonOptions);
#endif
        }
        catch (JsonException exception)
        {
            Check(exception.Message.Contains(property, StringComparison.OrdinalIgnoreCase), "The missing calibration field was not identified.");
#if DEBUG
            Check(File.ReadAllText(RobotImportService.GetConfigurationPath(directory)) == text, "Rejected source calibration was modified.");
#endif
            return;
        }
        throw new InvalidOperationException($"Missing robot calibration field {property} was silently accepted as zero.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
