using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Windows.Threading;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

/// <summary>新资源结构的回归入口，覆盖配置隔离、独立工具切换、动画和原生模型。</summary>
internal static class Program
{
    private const string ModelId = "KR_10_R1440-2";
    private const string Kr35ModelId = "KR_35_R1820_2";
    private const string ToolId = "Tool";

    [STAThread]
    private static int Main()
    {
        var fixture = Path.Combine(Path.GetTempPath(), "RobotSimulator.Tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(fixture);
        try
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            TestConfiguration(fixture);
            TestWorkConfiguration(fixture);
            ConfigurationRecoveryChecks.Run(fixture);
            CameraViewChecks.RunOrbitChecks();
            CameraViewChecks.RunMouseControlChecks();
#if DEBUG
            RobotSetupChecks.Run(fixture);
#endif
            Console.WriteLine("PASS: separate embedded catalogs, fixed robot parameters, Tool-only output and atomic Tool saves");
            RunAsync(() => TestViewModelAsync(fixture));
            Console.WriteLine("PASS: independent robot/Tool selection, shared TCP, Tool-only reload, failure recovery and animation lifecycle");
            // Macad 三维视口依赖 WPF Application 与全局 InteractiveContext；画质/动画用例会开关隐藏窗口，
            // 因此显式关闭窗口不结束应用。
            _ = new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
            MacadViewportBootstrap.EnsureContext();
            CadResourceChecks.Run();
            RunAsync(RobotViewerChecks.RunAsync);
            RunAsync(ViewportSmokeChecks.RunAsync);
            RunAsync(MainWindowChecks.RunAsync);
            return 0;
        }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
        finally
        {
            if (Path.GetDirectoryName(Path.GetFullPath(fixture)) == Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar)
                && Path.GetFileName(fixture).StartsWith("RobotSimulator.Tests-", StringComparison.Ordinal))
                Directory.Delete(fixture, recursive: true);
        }
    }

    /// <summary>机器人发现不写文件，只有 Tool 会生成可编辑配置，保存失败保留原数据。</summary>
    private static void TestConfiguration(string fixture)
    {
        var robots = RobotConfigurationStore.Discover();
        Check(robots.Any(model => model.Id == ModelId) && robots.Any(model => model.Id == Kr35ModelId), "Built-in robots are missing.");
        Check(!Directory.EnumerateFiles(fixture, "*", System.IO.SearchOption.AllDirectories).Any(), "Robot discovery wrote external data.");
        foreach (var robot in robots)
        {
            foreach (var file in new[] { robot.Configuration.BaseFile }.Concat(robot.Configuration.Joints.Select(joint => joint.ModelFile)))
            {
                using var source = ModelResources.OpenRobot(robot.Id, file);
                Check(source.Length > 0, "Robot asset is empty.");
            }
            var missing = false;
            try { using var duplicate = ModelResources.OpenRobot(robot.Id, "Tool.STEP"); }
            catch (System.IO.FileNotFoundException) { missing = true; }
            Check(missing, "The robot library still contains a duplicate Tool.");
        }
        Check(typeof(JointViewModel).GetProperty(nameof(JointViewModel.Minimum))!.SetMethod is null
            && typeof(JointViewModel).GetProperty(nameof(JointViewModel.ModelReferenceAngle))!.SetMethod is null,
            "Robot calibration parameters are still editable at runtime.");
        var toolsRoot = Path.Combine(fixture, "published", "Tools");
        var tool = ToolConfigurationStore.Discover(toolsRoot).Single(model => model.Id == ToolId);
        Check(Path.GetFileName(tool.ConfigurationPath) == Path.GetFileNameWithoutExtension(tool.Configuration.ModelFile) + ".tool.json",
            "The Tool configuration does not share the model basename.");
        var wrongPath = Path.Combine(toolsRoot, "WrongName.tool.json");
        var wrongNameRejected = false;
        try { ToolConfigurationStore.Save(wrongPath, tool.Configuration); }
        catch (System.IO.InvalidDataException) { wrongNameRejected = true; }
        Check(wrongNameRejected && !File.Exists(wrongPath), "A differently named Tool configuration was saved.");
        File.Copy(tool.ConfigurationPath, wrongPath);
        wrongNameRejected = false;
        try { ToolConfigurationStore.Load(wrongPath); }
        catch (System.IO.InvalidDataException) { wrongNameRejected = true; }
        Check(wrongNameRejected, "A differently named Tool configuration was loaded.");
        File.Delete(wrongPath);
        var files = Directory.GetFiles(Path.Combine(fixture, "published"), "*", System.IO.SearchOption.AllDirectories);
        Check(files.Length == ModelResources.ToolIds.Count && files.All(file => file.EndsWith(".tool.json")), "Output contains robot parameters or STEP files.");
        using var geometry = ModelResources.OpenTool(ToolId, tool.Configuration.ModelFile);
        Check(geometry.Length > 0, "The shared Tool geometry is missing.");
        ToolConfigurationStore.Save(tool.ConfigurationPath, MakeTool(tool.DisplayName, 246.8));
        Check(ToolConfigurationStore.Discover(toolsRoot).Single().Configuration.Tcp.Z == 246.8, "Discovery replaced user TCP edits.");
        using (var json = JsonDocument.Parse(File.ReadAllText(tool.ConfigurationPath)))
            Check(json.RootElement.EnumerateObject().Select(property => property.Name).Order().SequenceEqual(new[] { "modelFile", "tcp", "toolName" }),
                "The Tool configuration contains unrelated robot data.");
        var saved = File.ReadAllBytes(tool.ConfigurationPath);
        var rejected = false;
        try { ToolConfigurationStore.Save(tool.ConfigurationPath, MakeTool(tool.DisplayName, double.NaN)); }
        catch (System.IO.InvalidDataException) { rejected = true; }
        Check(rejected && saved.SequenceEqual(File.ReadAllBytes(tool.ConfigurationPath)), "Invalid TCP overwrote the saved configuration.");
        Check(!Directory.GetFiles(toolsRoot, "*.tmp", System.IO.SearchOption.AllDirectories).Any(), "Temporary configuration was left behind.");
        File.Delete(tool.ConfigurationPath);
        Check(ToolConfigurationStore.Discover(toolsRoot).Single().Configuration.Tcp.Z == ToolConfigurationStore.LoadBuiltIn(ToolId).Tcp.Z,
            "A deleted Tool configuration was not restored.");
    }

    /// <summary>用轻量场景服务验证界面编排，重点确认换工具不重读机器人、换机器人不丢工具 TCP。</summary>
    private static async Task TestViewModelAsync(string fixture)
    {
        var robots = RobotConfigurationStore.Discover();
        var first = ToolConfigurationStore.Discover(Path.Combine(fixture, "vm-tools")).Single();
        var secondConfig = MakeTool("第二个工具", 222);
        var secondPath = Path.Combine(fixture, "second", "Tool.tool.json");
        ToolConfigurationStore.Save(secondPath, secondConfig);
        var second = new ToolModelDefinition("SecondTool", secondPath, secondConfig);
        var work = WorkConfigurationStore.Discover(Path.Combine(fixture, "vm-work")).Single(model => model.Id == "双点焊工装");
        var secondWorkConfig = MakeWork("第二套工装", 222);
        var secondWorkPath = Path.Combine(fixture, "second-work", "双点焊工装.work.json");
        WorkConfigurationStore.Save(secondWorkPath, secondWorkConfig);
        var secondWork = new WorkModelDefinition("SecondWork", secondWorkPath, secondWorkConfig);
        var service = new FakeSceneService();
        var vm = new MainViewModel(robots, [first, second], [work, secondWork], service);
        var errors = new List<string>();
        vm.ErrorOccurred += (message, _) => errors.Add(message);
        Check(!vm.CanOperate && !vm.CanEditTool, "Controls were enabled before the viewer was ready.");
        await vm.ViewerReadyCommand.ExecuteAsync(null);
        Check(vm.IsLoaded && service.RobotLoadCount == 1 && service.ToolLoadCount == 1 && service.WorkLoadCount == 1,
            "Initial independent scene loading failed.");
        Check(service.Visibility == (true, true, true), "Coordinate visibility was not initialized.");
        CameraViewChecks.Run(vm);
        vm.ToolTcpZ = 123.45;
        Check(service.LastTcp?.Z == 123.45, "Live TCP edits did not update the scene.");
        var tcpUpdates = service.TcpUpdates;
        vm.ToolTcpX = double.NaN;
        Check(service.TcpUpdates == tcpUpdates, "Invalid TCP reached the scene.");
        vm.ToolTcpX = 0;
        vm.WorkX = 125;
        vm.WorkA = 90;
        Check(service.LastWorkPose?.X == 125 && service.LastWorkPose.A == 90, "Live Work placement did not update.");
        var workUpdates = service.WorkPoseUpdates;
        vm.WorkB = double.NaN;
        Check(service.WorkPoseUpdates == workUpdates, "Invalid Work placement reached the scene.");
        vm.WorkB = 0;
        vm.SaveWorkConfigurationCommand.Execute(null);
        Check(WorkConfigurationStore.Load(work.ConfigurationPath).Pose.X == 125, "Work placement was not saved.");
        vm.SaveToolConfigurationCommand.Execute(null);
        Check(ToolConfigurationStore.Load(first.ConfigurationPath).Tcp.Z == 123.45, "The Tool TCP was not saved.");
        var anotherRobot = robots.Single(robot => robot.Id == Kr35ModelId);
        vm.SelectedRobot = anotherRobot;
        await vm.SelectRobotCommand.ExecuteAsync(anotherRobot);
        Check(service.RobotLoadCount == 2 && service.ToolLoadCount == 1 && vm.SelectedTool == first && vm.ToolTcpZ == 123.45,
            "Robot switching changed the selected Tool or its TCP.");
        Check(service.WorkLoadCount == 1 && vm.SelectedWork == work && vm.WorkX == 125 && vm.WorkA == 90,
            "Robot switching lost the selected Work or its edited placement.");
        var joints = vm.Joints;
        vm.Joints[0].Angle = 30;
        vm.SelectedTool = second;
        await vm.SelectToolCommand.ExecuteAsync(second);
        Check(service.RobotLoadCount == 2 && ReferenceEquals(joints, vm.Joints) && vm.Joints[0].Angle == 30 && vm.ToolTcpZ == 222,
            "Tool switching reloaded the robot or reset its pose.");
        vm.ToolTcpZ = 333;
        vm.SaveToolConfigurationCommand.Execute(null);
        Check(ToolConfigurationStore.Load(secondPath).Tcp.Z == 333 && ToolConfigurationStore.Load(first.ConfigurationPath).Tcp.Z == 123.45,
            "Saving one Tool changed another Tool's settings.");
        vm.SelectedTool = first;
        await vm.SelectToolCommand.ExecuteAsync(first);
        Check(vm.ToolTcpZ == 123.45 && service.RobotLoadCount == 2, "Returning to a Tool did not restore its own TCP.");
        ToolConfigurationStore.Save(first.ConfigurationPath, MakeTool(first.DisplayName, 456.7));
        await vm.ReloadToolCommand.ExecuteAsync(null);
        Check(vm.ToolTcpZ == 456.7 && service.RobotLoadCount == 2 && ReferenceEquals(joints, vm.Joints) && vm.Joints[0].Angle == 30,
            "Tool reload touched the fixed robot or its pose.");
        service.FailNextTool = true;
        vm.SelectedTool = second;
        await vm.SelectToolCommand.ExecuteAsync(second);
        Check(vm.SelectedTool == first && vm.ToolTcpZ == 456.7 && vm.IsLoaded && !vm.IsBusy && errors.Count == 1,
            "A failed Tool selection did not preserve the active Tool and robot.");
        var toolLoads = service.ToolLoadCount;
        vm.SelectedWork = secondWork;
        await vm.SelectWorkCommand.ExecuteAsync(secondWork);
        Check(service.RobotLoadCount == 2 && service.ToolLoadCount == toolLoads && ReferenceEquals(joints, vm.Joints)
            && vm.Joints[0].Angle == 30 && vm.ToolTcpZ == 456.7 && vm.WorkX == 222,
            "Switching Work reloaded the robot or Tool, or reset its pose.");
        vm.WorkX = 333;
        vm.SaveWorkConfigurationCommand.Execute(null);
        Check(WorkConfigurationStore.Load(secondWorkPath).Pose.X == 333
            && WorkConfigurationStore.Load(work.ConfigurationPath).Pose.X == 125, "Saving Work changed another fixture.");
        vm.SelectedWork = work;
        await vm.SelectWorkCommand.ExecuteAsync(work);
        Check(vm.WorkX == 125 && vm.WorkA == 90, "Returning to Work lost its own placement.");
        WorkConfigurationStore.Save(work.ConfigurationPath, MakeWork(work.DisplayName, 456.7));
        await vm.ReloadWorkCommand.ExecuteAsync(null);
        Check(vm.WorkX == 456.7 && service.RobotLoadCount == 2 && service.ToolLoadCount == toolLoads
            && ReferenceEquals(joints, vm.Joints) && vm.Joints[0].Angle == 30, "Reloading Work changed the robot or Tool.");
        service.FailNextWork = true;
        vm.SelectedWork = secondWork;
        await vm.SelectWorkCommand.ExecuteAsync(secondWork);
        Check(vm.SelectedWork == work && vm.WorkX == 456.7 && vm.IsLoaded && !vm.IsBusy && errors.Count == 2,
            "Failed Work loading did not preserve the previous fixture and release the UI.");
        var previousRobot = vm.SelectedRobot;
        var previousJoints = vm.Joints;
        var workLoads = service.WorkLoadCount;
        service.PendingRobotLoad = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pendingRobot = service.PendingRobotLoad;
        vm.SelectedRobot = robots.Single(robot => robot.Id == ModelId);
        var switching = vm.SelectRobotCommand.ExecuteAsync(vm.SelectedRobot);
        Check(vm.IsBusy && vm.IsLoaded && vm.ShowLoadingOverlay && ReferenceEquals(vm.Joints, previousJoints),
            "Robot switching hid or reset the previous scene before the new robot was ready.");
        pendingRobot.SetException(new System.IO.InvalidDataException("测试机器人读取失败"));
        await switching;
        Check(vm.SelectedRobot == previousRobot && ReferenceEquals(vm.Joints, previousJoints) && vm.Joints[0].Angle == 30
            && vm.IsLoaded && !vm.IsBusy && errors.Count == 3 && service.ToolLoadCount == toolLoads
            && service.WorkLoadCount == workLoads, "Failed robot switching lost the previous scene or reloaded attachments.");
        var oldAngle = vm.Joints[0].Angle;
        vm.Joints[0].Angle = double.NaN;
        Check(vm.Joints[0].Angle == oldAngle, "Invalid joint angle was accepted.");
        vm.ShowBaseCoordinates = false;
        vm.ShowFlangeCoordinates = false;
        vm.UpdateCoordinateVisibilityCommand.Execute(null);
        Check(service.Visibility == (false, false, true), "Coordinate controls are no longer independent.");
        vm.ShowWeldBeam = false;
        vm.UpdateCoordinateVisibilityCommand.Execute(null);
        Check(!service.WeldBeamVisible, "The weld beam visibility control did not reach the scene.");

        var window = new System.Windows.Window
        {
            Width = 32, Height = 32, Left = -5000, Top = -5000, ShowInTaskbar = false,
            WindowStartupLocation = System.Windows.WindowStartupLocation.Manual
        };
        try
        {
            window.Show();
            var before = service.ApplyAllCount;
            vm.PlayCommand.Execute(null);
            await Task.Delay(250);
            Check(vm.IsPlaying && service.ApplyAllCount > before, "Animation did not update the scene.");
            vm.PlayCommand.Execute(null);
            var stopped = service.ApplyAllCount;
            await Task.Delay(120);
            Check(!vm.IsPlaying && service.ApplyAllCount == stopped, "Animation continued after stopping.");
            vm.PlayCommand.Execute(null);
            service.PendingToolLoad = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pending = service.PendingToolLoad;
            var loading = vm.ReloadToolCommand.ExecuteAsync(null);
            Check(vm.IsBusy && !vm.CanEditTool && !vm.IsPlaying, "Loading did not stop animation and lock configuration editing.");
            var closing = new CancelEventArgs();
            var closeRequested = false;
            vm.CloseRequested += () => closeRequested = true;
            vm.ClosingCommand.Execute(closing);
            Check(closing.Cancel && !service.Disposed, "Closing interrupted a native load.");
            pending.SetResult(true);
            await loading;
            Check(closeRequested && errors.Count == 3, "Deferred closing did not resume after loading.");
            vm.ClosingCommand.Execute(new CancelEventArgs());
            Check(service.Disposed, "The scene service was not disposed.");
        }
        finally
        {
            if (vm.IsPlaying) vm.PlayCommand.Execute(null);
            window.Close();
        }
        var emptyService = new FakeSceneService();
        var noTool = new MainViewModel(robots, [], [], emptyService);
        await noTool.ViewerReadyCommand.ExecuteAsync(null);
        Check(noTool.IsLoaded && !noTool.CanEditTool && emptyService.ToolLoadCount == 0, "Robot-only operation failed.");
        noTool.ClosingCommand.Execute(new CancelEventArgs());
        var failedService = new FakeSceneService { FailInitialization = true };
        var failedViewer = new MainViewModel(robots, [], [], failedService);
        var initializationErrors = 0;
        failedViewer.ErrorOccurred += (_, _) => initializationErrors++;
        await failedViewer.ViewerReadyCommand.ExecuteAsync(null);
        Check(initializationErrors == 1 && !failedViewer.IsLoaded, "Viewport initialization errors escaped the view model.");
        failedViewer.ClosingCommand.Execute(new CancelEventArgs());
        var failedWorkService = new FakeSceneService { FailNextWork = true };
        var failedWorkViewer = new MainViewModel(robots, [first], [work], failedWorkService);
        await failedWorkViewer.ViewerReadyCommand.ExecuteAsync(null);
        Check(failedWorkViewer.IsLoaded && failedWorkService.ToolLoadCount == 1 && failedWorkViewer.WorkStatus.Contains("加载失败"),
            "Initial Work failure disabled the robot or prevented Tool loading.");
        failedWorkViewer.ClosingCommand.Execute(new CancelEventArgs());
        var failedToolService = new FakeSceneService { FailNextTool = true };
        var failedToolViewer = new MainViewModel(robots, [first], [work], failedToolService);
        await failedToolViewer.ViewerReadyCommand.ExecuteAsync(null);
        Check(failedToolViewer.IsLoaded && failedToolService.WorkLoadCount == 1 && failedToolViewer.ToolStatus.Contains("加载失败"),
            "Initial Tool failure prevented independent Work loading.");
        failedToolViewer.ClosingCommand.Execute(new CancelEventArgs());
    }

    private static WorkConfiguration MakeWork(string name, double x) => new()
    {
        WorkName = name, ModelFile = "双点焊工装.STEP", Pose = new PoseConfiguration { X = x }
    };

    private static void TestWorkConfiguration(string fixture)
    {
        var root = Path.Combine(fixture, "work-config", "Work");
        var work = WorkConfigurationStore.Discover(root).Single(model => model.Id == "双点焊工装");
        Check(Path.GetFileName(work.ConfigurationPath) == "双点焊工装.work.json", "Work config/model basenames differ.");
        Check(work.Configuration.Pose.X == 0 && work.Configuration.Pose.Y == 0 && work.Configuration.Pose.Z == 0
            && work.Configuration.Pose.A == 0 && work.Configuration.Pose.B == 0 && work.Configuration.Pose.C == 0,
            "Default Work placement must preserve exported CAD coordinates.");
        using var source = ModelResources.OpenWork(work.Id, work.Configuration.ModelFile);
        Check(source.Length > 0, "Work STEP is not embedded.");
        WorkConfigurationStore.Save(work.ConfigurationPath, MakeWork(work.DisplayName, 123));
        Check(WorkConfigurationStore.Discover(root).Single(model => model.Id == "双点焊工装").Configuration.Pose.X == 123, "Discovery overwrote saved Work placement.");
        var saved = File.ReadAllBytes(work.ConfigurationPath);
        foreach (var value in new[] { double.NaN, double.PositiveInfinity })
        {
            var rejected = false;
            try { WorkConfigurationStore.Save(work.ConfigurationPath, MakeWork(work.DisplayName, value)); }
            catch (System.IO.InvalidDataException) { rejected = true; }
            Check(rejected && saved.SequenceEqual(File.ReadAllBytes(work.ConfigurationPath)), "Invalid Work pose overwrote saved data.");
        }
        var wrongPath = Path.Combine(root, "WrongName.work.json");
        var wrongNameRejected = false;
        try { WorkConfigurationStore.Save(wrongPath, work.Configuration); }
        catch (System.IO.InvalidDataException) { wrongNameRejected = true; }
        Check(wrongNameRejected && !File.Exists(wrongPath), "A differently named Work config was saved.");
        Check(Directory.GetFiles(root, "*", System.IO.SearchOption.AllDirectories).All(file => file.EndsWith(".work.json"))
            && !Directory.EnumerateFiles(root, "*.tmp", System.IO.SearchOption.AllDirectories).Any(), "Work discovery exported geometry or left temporary files.");
        Console.WriteLine("PASS: Work embedded catalog, matching config filename, zero placement and atomic independent saves");
    }

    /// <summary>创建测试用的独立工具配置。</summary>
    private static ToolConfiguration MakeTool(string name, double z) => new()
    {
        ToolName = name, ModelFile = "Tool.STEP", Tcp = new PoseConfiguration { Z = z }
    };

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void RunAsync(Func<Task> test)
    {
        var frame = new DispatcherFrame();
        Exception? error = null;
        async void Run()
        {
            try { await test(); }
            catch (Exception exception) { error = exception; }
            finally { frame.Continue = false; }
        }
        Run();
        if (frame.Continue) Dispatcher.PushFrame(frame);
        if (error is not null) ExceptionDispatchInfo.Capture(error).Throw();
    }

    /// <summary>只记录调用次数和参数，检查工具更换不会触发机器人重新加载。</summary>
    private sealed class FakeSceneService : IRobotSceneService
    {
        public bool IsUserAdjustingView { get; set; }
        public int RobotLoadCount { get; private set; }
        public int ToolLoadCount { get; private set; }
        public int WorkLoadCount { get; private set; }
        public int WorkPoseUpdates { get; private set; }
        public bool FailNextWork { get; set; }
        public PoseConfiguration? LastWorkPose { get; private set; }
        public int ApplyAllCount { get; private set; }
        public int TcpUpdates { get; private set; }
        public bool Disposed { get; private set; }
        public bool FailNextTool { get; set; }
        public bool FailInitialization { get; set; }
        public PoseConfiguration? LastTcp { get; private set; }
        public (bool, bool, bool) Visibility { get; private set; }
        public TaskCompletionSource<bool>? PendingToolLoad { get; set; }
        public TaskCompletionSource<bool>? PendingRobotLoad { get; set; }

        public void InitializeViewport()
        {
            if (FailInitialization) throw new InvalidOperationException("测试视口初始化失败。");
        }
        public async Task LoadRobotAsync(RobotConfiguration configuration, string modelId, IProgress<RobotLoadProgress>? progress = null)
        {
            RobotLoadCount++;
            if (PendingRobotLoad is { } pending)
            {
                try { await pending.Task; }
                finally { PendingRobotLoad = null; }
            }
        }
        public async Task LoadToolAsync(string toolId, ToolConfiguration configuration, IProgress<RobotLoadProgress>? progress = null)
        {
            ToolLoadCount++;
            if (FailNextTool) { FailNextTool = false; throw new System.IO.InvalidDataException("测试工具加载失败。"); }
            if (PendingToolLoad is { } pending) { await pending.Task; PendingToolLoad = null; }
        }
        public Task LoadWorkAsync(string workId, WorkConfiguration configuration, IProgress<RobotLoadProgress>? progress = null)
        {
            WorkLoadCount++;
            if (FailNextWork) { FailNextWork = false; throw new System.IO.InvalidDataException("测试工装加载失败。"); }
            return Task.CompletedTask;
        }
        public void SetWorkPose(PoseConfiguration pose)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            LastWorkPose = pose;
            WorkPoseUpdates++;
        }
        public void ApplyAll(IEnumerable<JointViewModel> joints)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            ApplyAllCount++;
        }
        public void ApplyJoint(JointViewModel joint) => ObjectDisposedException.ThrowIf(Disposed, this);
        public void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp) => Visibility = (showBase, showFlange, showTcp);
        public bool WeldBeamVisible { get; private set; } = true;
        public void SetWeldBeamVisible(bool show) => WeldBeamVisible = show;
        public void SetToolTcp(PoseConfiguration pose)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            LastTcp = pose;
            TcpUpdates++;
        }
        public void SetView(RobotView view) { }

        public void SetOrbit(double speedDegreesPerSecond, double startAngleDegrees, double elevationDegrees) { }
        public void ZoomAll() { }
        public void SetRenderQuality(RenderQuality quality) { }
        public void Dispose() => Disposed = true;
    }
}
