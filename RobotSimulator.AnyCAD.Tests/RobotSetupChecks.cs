#if DEBUG
using System.IO;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

internal static class RobotSetupChecks
{
    public static void Run(string fixture)
    {
        foreach (var image in new[] { "model-assembly.png", "joint-axis.png", "angle-reference.png", "flange-frame.png" })
        {
            var uri = new Uri($"/RobotSimulator.AnyCAD;component/Resources/SetupGuide/{image}", UriKind.Relative);
            using var source = System.Windows.Application.GetResourceStream(uri)?.Stream;
            Check(source is { Length: > 0 }, $"Missing embedded guide image: {image}.");
        }
        var directory = Path.Combine(fixture, "NewRobot");
        Directory.CreateDirectory(directory);
        foreach (var file in new[] { "Base.STEP", "J1.STEP", "J2.STEP", "J3.STEP", "J4.STEP", "J5.STEP", "J6.STEP" })
            File.WriteAllText(Path.Combine(directory, file), "ISO-10303-21;");
        var vm = new RobotSetupViewModel();
        vm.SelectDirectory(directory);
        Check(vm.ModelFiles.Count == 7 && vm.Joints.Count == 6 && vm.BaseFile == "Base.STEP", "Model mapping was not detected.");
        Check(vm.Joints.All(joint => joint.Minimum.Length == 0 && joint.OriginX.Length == 0)
            && vm.FlangeA.Length == 0, "Unmeasured calibration was silently filled in.");
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1, "Valid model mapping did not advance.");
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1 && vm.ValidationMessage.Contains("J1"), "Incomplete joint calibration was accepted.");

        var configuration = RobotConfigurationStore.LoadBuiltIn("KR_10_R1440-2");
        var output = RobotImportService.Save(directory, configuration);
        Check(output == Path.Combine(directory, "NewRobot.robot.json"), "Robot source config did not match its directory.");
        vm.SelectDirectory(directory);
        Check(vm.Joints[1].HomeAngle == "-90" && vm.FlangeZ == "1055", "Existing calibration was not loaded.");
        vm.NextCommand.Execute(null);
        vm.Joints[0].DirectionZ = "0";
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1 && vm.SelectedJoint.Name == "J1" && vm.ValidationMessage.Contains("J1"),
            "An invalid current axis advanced to the next joint.");
        vm.SetAxisDirectionCommand.Execute("-Z");
        Check(vm.SelectedJoint.DirectionX == "0" && vm.SelectedJoint.DirectionY == "0" && vm.SelectedJoint.DirectionZ == "-1",
            "A direction preset did not fill all three components.");
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1 && vm.SelectedJoint.Name == "J2" && vm.JointConnection.Contains("J1.STEP")
            && vm.JointConnection.Contains("J2.STEP"), "Joint-by-joint navigation did not identify the mating parts.");
        vm.SelectedJoint.HomeAngle = "0";
        vm.UseExportPoseCommand.Execute(null);
        Check(vm.SelectedJoint.HomeAngle == "-90", "Keeping the export pose did not copy the recorded angle.");
        vm.PreviousJointCommand.Execute(null);
        Check(vm.SelectedJoint.Name == "J1", "Previous-joint navigation lost the current step.");
        for (var joint = 0; joint < 6; joint++) vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 2, "Six valid joint checks did not advance to the flange.");
        vm.BackCommand.Execute(null);
        vm.Joints[1].Minimum = "NaN";
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1 && vm.SelectedJoint.Name == "J2", "Full validation did not focus an earlier invalid joint.");
        vm.Joints[1].Minimum = "-185";
        for (var joint = 1; joint < 6; joint++) vm.NextCommand.Execute(null);
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 3 && vm.CanSave, "Valid configuration did not complete the wizard.");
        vm.ModelName = "New Robot";
        Check(!vm.CanSave && vm.HasUnsavedChanges && vm.ValidationMessage.Length == 0, "Editing left a stale validation result.");
        vm.ValidateCommand.Execute(null);
        Check(vm.CanSave, "Valid changes failed validation.");
        vm.Save();
        Check(!vm.HasUnsavedChanges, "A saved draft was still marked as unsaved.");
        Check(RobotImportService.LoadExisting(directory)?.ModelName == "New Robot", "Robot save roundtrip failed.");
        var saved = File.ReadAllBytes(output);
        vm.Joints[0].DirectionX = vm.Joints[0].DirectionY = vm.Joints[0].DirectionZ = "0";
        vm.ValidateCommand.Execute(null);
        Check(!vm.CanSave && vm.ValidationMessage.Contains("J1"), "A zero joint direction was accepted.");
        ExpectInvalid(() => vm.Save());
        Check(saved.SequenceEqual(File.ReadAllBytes(output)), "Invalid calibration destroyed the existing source config.");
        vm.SelectDirectory(directory);
        vm.Joints[0].Minimum = "NaN";
        ExpectInvalid(() => vm.Save());
        Check(saved.SequenceEqual(File.ReadAllBytes(output)), "Non-finite input changed the existing source config.");
        vm.SelectDirectory(directory);
        vm.Joints[0].ModelFile = vm.BaseFile;
        ExpectInvalid(() => vm.Save());
        vm.SelectDirectory(directory);
        vm.Joints[0].ModelFile = "../J1.STEP";
        ExpectInvalid(() => vm.Save());
        vm.SelectDirectory(directory);
        File.Delete(Path.Combine(directory, "J6.STEP"));
        ExpectInvalid(() => vm.Save());
        Check(saved.SequenceEqual(File.ReadAllBytes(output)) && !Directory.EnumerateFiles(directory, "*.tmp").Any(),
            "A missing model damaged the source config or left a temporary file.");

        var incomplete = Path.Combine(fixture, "IncompleteRobot");
        Directory.CreateDirectory(incomplete);
        File.WriteAllText(Path.Combine(incomplete, "Base.STEP"), "ISO-10303-21;");
        vm.SelectDirectory(incomplete);
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 0 && !vm.CanSave && vm.ValidationMessage.Length > 0,
            "An incomplete import advanced to calibration.");
        Check(!File.Exists(vm.OutputPath), "Import wrote an unvalidated robot config.");

        var custom = Path.Combine(fixture, "CustomRobot");
        Directory.CreateDirectory(custom);
        foreach (var file in new[] { "Stand", "Shoulder", "Arm", "Elbow", "Wrist", "Hand", "Flange" })
            File.WriteAllText(Path.Combine(custom, file + ".STEP"), "ISO-10303-21;");
        vm.SelectDirectory(custom);
        vm.BaseFile = "Stand.STEP";
        vm.NextCommand.Execute(null);
        Check(vm.StepIndex == 1, "Custom STEP names could not reach the joint mapping page.");
        var firstJoint = vm.Joints[0];
        firstJoint.ModelFile = "Shoulder.STEP";
        firstJoint.Minimum = "-180"; firstJoint.Maximum = "180";
        firstJoint.HomeAngle = firstJoint.ModelReferenceAngle = "0";
        firstJoint.OriginX = firstJoint.OriginY = firstJoint.OriginZ = "0";
        vm.SetAxisDirectionCommand.Execute("+Z");
        vm.NextCommand.Execute(null);
        Check(vm.SelectedJoint.Name == "J2" && vm.StepIndex == 1,
            "Checking J1 incorrectly required the other five joints to be mapped first.");

        Console.WriteLine("PASS: developer robot wizard mapping, measured inputs, roundtrip, validation and non-destructive source saves");
    }

    private static void ExpectInvalid(Action action)
    {
        try { action(); }
        catch (InvalidDataException) { return; }
        throw new InvalidOperationException("An invalid robot configuration was accepted.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
