using System.ComponentModel;
using System.Text.Json;
using System.Windows.Threading;
using AnyCAD.Foundation;
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
        var fixture = Path.Combine(Path.GetTempPath(), "RobotSimulator.AnyCAD.Tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(fixture);
        try
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            TestConfiguration(fixture);
#if DEBUG
            RobotSetupChecks.Run(fixture);
#endif
            Console.WriteLine("PASS: separate embedded catalogs, fixed robot parameters, Tool-only output and atomic Tool saves");
            RunAsync(() => TestViewModelAsync(fixture));
            Console.WriteLine("PASS: independent robot/Tool selection, shared TCP, Tool-only reload, failure recovery and animation lifecycle");
            GlobalInstance.Initialize();
            try
            {
                RobotColorChecks.Run(fixture);
                RobotHpChecks.Run(fixture);
                TestIrb2600Scene(fixture);
                TestNativeScene(fixture);
                TestKr35Scene(fixture);
                Console.WriteLine("PASS: geometry/colors, independent Tool replacement, two robot flange frames, six joint axes and cache reuse");
            }
            finally { GlobalInstance.Destroy(); }
            return 0;
        }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
        finally
        {
            if (Path.GetDirectoryName(Path.GetFullPath(fixture)) == Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar)
                && Path.GetFileName(fixture).StartsWith("RobotSimulator.AnyCAD.Tests-", StringComparison.Ordinal))
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
        var service = new FakeSceneService();
        var vm = new MainViewModel(robots, [first, second], service);
        var errors = new List<string>();
        vm.ErrorOccurred += (message, _) => errors.Add(message);
        Check(!vm.CanOperate && !vm.CanEditTool, "Controls were enabled before the viewer was ready.");
        await vm.ViewerReadyCommand.ExecuteAsync(null);
        Check(vm.IsLoaded && service.RobotLoadCount == 1 && service.ToolLoadCount == 1, "Initial independent scene loading failed.");
        Check(service.Visibility == (true, true, true), "Coordinate visibility was not initialized.");
        vm.ToolTcpZ = 123.45;
        Check(service.LastTcp?.Z == 123.45, "Live TCP edits did not update the scene.");
        var tcpUpdates = service.TcpUpdates;
        vm.ToolTcpX = double.NaN;
        Check(service.TcpUpdates == tcpUpdates, "Invalid TCP reached the scene.");
        vm.ToolTcpX = 0;
        vm.SaveToolConfigurationCommand.Execute(null);
        Check(ToolConfigurationStore.Load(first.ConfigurationPath).Tcp.Z == 123.45, "The Tool TCP was not saved.");
        var anotherRobot = robots.Single(robot => robot.Id == Kr35ModelId);
        vm.SelectedRobot = anotherRobot;
        await vm.SelectRobotCommand.ExecuteAsync(anotherRobot);
        Check(service.RobotLoadCount == 2 && vm.SelectedTool == first && vm.ToolTcpZ == 123.45,
            "Robot switching changed the selected Tool or its TCP.");
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
        var oldAngle = vm.Joints[0].Angle;
        vm.Joints[0].Angle = double.NaN;
        Check(vm.Joints[0].Angle == oldAngle, "Invalid joint angle was accepted.");
        vm.ShowBaseCoordinates = false;
        vm.ShowFlangeCoordinates = false;
        vm.UpdateCoordinateVisibilityCommand.Execute(null);
        Check(service.Visibility == (false, false, true), "Coordinate controls are no longer independent.");

        var window = new System.Windows.Window
        {
            Width = 32, Height = 32, Left = -5000, Top = -5000, ShowInTaskbar = false,
            WindowStartupLocation = System.Windows.WindowStartupLocation.Manual
        };
        EventHandler? observer = null;
        try
        {
            window.Show();
            var before = service.ApplyAllCount;
            var input = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            observer = (_, _) =>
            {
                System.Windows.Media.CompositionTarget.Rendering -= observer;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, () => input.TrySetResult(service.ApplyAllCount));
            };
            System.Windows.Media.CompositionTarget.Rendering += observer;
            vm.PlayCommand.Execute(null);
            Check(await input.Task.WaitAsync(TimeSpan.FromSeconds(3)) == before, "Animation ran ahead of mouse input.");
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
            Check(closeRequested && errors.Count == 1, "Deferred closing did not resume after loading.");
            vm.ClosingCommand.Execute(new CancelEventArgs());
            Check(service.Disposed, "The scene service was not disposed.");
        }
        finally
        {
            System.Windows.Media.CompositionTarget.Rendering -= observer;
            if (vm.IsPlaying) vm.PlayCommand.Execute(null);
            window.Close();
        }
        var emptyService = new FakeSceneService();
        var noTool = new MainViewModel(robots, [], emptyService);
        await noTool.ViewerReadyCommand.ExecuteAsync(null);
        Check(noTool.IsLoaded && !noTool.CanEditTool && emptyService.ToolLoadCount == 0, "Robot-only operation failed.");
        noTool.ClosingCommand.Execute(new CancelEventArgs());
        var failedService = new FakeSceneService { FailInitialization = true };
        var failedViewer = new MainViewModel(robots, [], failedService);
        var initializationErrors = 0;
        failedViewer.ErrorOccurred += (_, _) => initializationErrors++;
        await failedViewer.ViewerReadyCommand.ExecuteAsync(null);
        Check(initializationErrors == 1 && !failedViewer.IsLoaded, "Viewport initialization errors escaped the view model.");
        failedViewer.ClosingCommand.Execute(new CancelEventArgs());
    }

    /// <summary>确认机器人不带内置 Tool，单独安装或替换时保留六轴并继续复用颜色缓存。</summary>
    private static void TestNativeScene(string fixture)
    {
        var toolPath = Path.Combine(fixture, "Tool.STEP");
        using (var source = ModelResources.OpenTool(ToolId, "Tool.STEP"))
        using (var destination = File.Create(toolPath)) source.CopyTo(destination);
        using var original = ShapeIO.Open(toolPath);
        var expected = MeshStats.FromShape(original);
        var configuration = RobotConfigurationStore.LoadBuiltIn(ModelId);
        var cacheRoot = Path.Combine(fixture, "cache");
        using (var scene = RobotSceneContent.Load(configuration, ModelId, cacheRoot: cacheRoot))
        {
            Check(!scene.HasTool, "Robot loading unexpectedly included a Tool.");
            var part = LoadTool(cacheRoot);
            Check(!part.FromCache, "The isolated first Tool load did not parse STEP.");
            scene.ReplaceTool(part);
            scene.ReplaceTool(part);
            Check(scene.HasTool, "Reapplying the same Tool released its active geometry.");
            VerifyCoordinateFrames(scene);
            VerifyToolScene(scene, expected);
            scene.JointGroups[5].SetTransform(Matrix4d.Identity);
            using var offset = Matrix4d.makeTranslate(12, -7, 3);
            scene.JointGroups[0].SetTransform(offset);
            scene.ReplaceTool(LoadTool(cacheRoot));
            scene.SetToolTcp(new PoseConfiguration { Z = 100 });
            scene.Root.UpdateTransform(Matrix4d.Identity);
            using var flange = FindGroup(scene.JointGroups[5], "FlangeFrame");
            using var mount = FindGroup(flange, "ToolMount");
            Check(mount.GetCount() == 1, "Tool replacement retained old geometry nodes.");
            AssertFrame(mount, [942, -7, 1058], [0, 0, -1, 0, 1, 0, 1, 0, 0]);
            scene.ReplaceTool(null);
            Check(!scene.HasTool && mount.GetCount() == 0, "Tool removal did not clear its geometry.");
            scene.ReplaceTool(LoadTool(cacheRoot));
            scene.Root.UpdateTransform(Matrix4d.Identity);
            AssertFrame(mount, [942, -7, 1058], [0, 0, -1, 0, 1, 0, 1, 0, 0]);
        }
        var progress = new List<RobotLoadProgress>();
        using var cached = RobotSceneContent.Load(configuration, ModelId, progress.Add, cacheRoot);
        Check(progress.Count(state => state.FromCache) == 7 && !cached.HasTool, "Robot caches were not independent of Tool.");
        using var cachedTool = LoadTool(cacheRoot);
        Check(cachedTool.FromCache, "The Tool cache was not reused.");
    }

    /// <summary>从共享工具资源库读取一个独立几何对象，由调用方决定安装或释放。</summary>
    private static CadModelPart LoadTool(string cacheRoot)
    {
        using var source = ModelResources.OpenTool(ToolId, "Tool.STEP");
        return CadModelPart.Load("Tool", source, cacheRoot);
    }

    /// <summary>创建测试用的独立工具配置。</summary>
    private static ToolConfiguration MakeTool(string name, double z) => new()
    {
        ToolName = name, ModelFile = "Tool.STEP", Tcp = new PoseConfiguration { Z = z }
    };

    /// <summary>IRB 的名义零位保持装配，实测轴线驱动法兰与工具；不代替控制器标定。</summary>
    private static void TestIrb2600Scene(string fixture)
    {
        const string modelId = "IRB2600_20_165";
        var configuration = RobotConfigurationStore.Discover().Single(model => model.Id == modelId).Configuration;
        Check(configuration.Joints.All(joint => joint.HomeAngle == 0 && joint.ModelReferenceAngle == 0),
            "The IRB 2600 nominal home changed the exported STEP posture.");
        var ranges = configuration.Joints.Select(joint => (joint.Minimum, joint.Maximum));
        Check(ranges.SequenceEqual(new (double, double)[] { (-180, 180), (-95, 155), (-180, 75), (-400, 400), (-120, 120), (-400, 400) }),
            "The IRB 2600 ranges differ from the supplied ABB datasheet.");
        var cacheRoot = Path.Combine(fixture, "cache");
        using (var scene = RobotSceneContent.Load(configuration, modelId, cacheRoot: cacheRoot))
        using (var flange = FindGroup(scene.JointGroups[5], "FlangeFrame"))
        using (var mount = FindGroup(flange, "ToolMount"))
        using (var tcp = FindGroup(flange, "TcpFrame"))
        {
            Check(scene.JointGroups.Count == 6 && !scene.HasTool, "The IRB 2600 hierarchy is incomplete.");
            using var basePart = FindGroup(scene.Root, "Base");
            Check(basePart.GetCount() > 0, "The IRB 2600 base geometry is missing.");
            for (var index = 0; index < 6; index++)
            {
                using var part = FindGroup(scene.JointGroups[index], $"J{index + 1}");
                Check(part.GetCount() > 0, $"The IRB 2600 J{index + 1} geometry is missing.");
            }
            scene.SetToolTcp(new PoseConfiguration { Z = 100 });
            scene.Root.UpdateTransform(Matrix4d.Identity);
            double[] homeDirections = [0, 0, -1, 0, 1, 0, 1, 0, 0];
            AssertFrame(flange, [1030, 0, 1260], homeDirections);
            AssertFrame(tcp, [1130, 0, 1260], homeDirections);

            // 期望位置由 CAD 实测尺寸独立计算；J3 取 -90°，保持在数据表行程内。
            (double Delta, double[] Origin, double[] Directions)[] cases =
            [
                (90, [0, 1030, 1260], [0, 0, -1, -1, 0, 0, 0, 1, 0]),
                (90, [965, 0, -435], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
                (-90, [35, 0, 2025], [1, 0, 0, 0, 1, 0, 0, 0, 1]),
                (90, [1030, 0, 1260], [0, 1, 0, 0, 0, 1, 1, 0, 0]),
                (90, [945, 0, 1175], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
                (90, [1030, 0, 1260], [0, 1, 0, 0, 0, 1, 1, 0, 0])
            ];
            for (var index = 0; index < cases.Length; index++)
            {
                var joint = configuration.Joints[index];
                using var origin = new GPnt(joint.AxisOrigin.X, joint.AxisOrigin.Y, joint.AxisOrigin.Z);
                using var direction = new GDir(joint.AxisDirection.X, joint.AxisDirection.Y, joint.AxisDirection.Z);
                using var axis = new GAx1(origin, direction);
                using var rotation = new GTrsf();
                rotation.SetRotation(axis, cases[index].Delta * Math.PI / 180);
                using var matrix = Matrix4d.From(rotation);
                scene.JointGroups[index].SetTransform(matrix);
                scene.Root.UpdateTransform(Matrix4d.Identity);
                AssertFrame(flange, cases[index].Origin, cases[index].Directions);
                AssertFrame(mount, cases[index].Origin, cases[index].Directions);
                var tcpOrigin = Enumerable.Range(0, 3).Select(coordinate =>
                    cases[index].Origin[coordinate] + 100 * cases[index].Directions[6 + coordinate]).ToArray();
                AssertFrame(tcp, tcpOrigin, cases[index].Directions);
                scene.JointGroups[index].SetTransform(Matrix4d.Identity);
            }
        }
        var progress = new List<RobotLoadProgress>();
        using var cached = RobotSceneContent.Load(configuration, modelId, progress.Add, cacheRoot);
        Check(progress.Count(state => state.FromCache) == 7, "The IRB 2600 caches were not reused.");
        Console.WriteLine("PASS: IRB 2600 embedded config, seven STEP parts, nominal home, six joint chains, flange/TCP and cache reuse");
    }

    private static void TestKr35Scene(string fixture)
    {
        var configuration = RobotConfigurationStore.LoadBuiltIn(Kr35ModelId);
        Check(configuration.Joints.Select(joint => joint.HomeAngle).SequenceEqual(new double[] { 0, -90, 90, 0, 0, 0 })
            && configuration.Joints.All(joint => joint.HomeAngle == joint.ModelReferenceAngle),
            "The KR 35 default pose must preserve the exported CAD assembly.");
        var cacheRoot = Path.Combine(fixture, "cache");
        using (var scene = RobotSceneContent.Load(configuration, Kr35ModelId, cacheRoot: cacheRoot))
        using (var flange = FindGroup(scene.JointGroups[5], "FlangeFrame"))
        using (var mount = FindGroup(flange, "ToolMount"))
        using (var tcp = FindGroup(flange, "TcpFrame"))
        using (var baseAxes = FindNode(scene.Root, "BaseAxes"))
        {
            scene.ReplaceTool(LoadTool(cacheRoot));
            scene.SetToolTcp(new PoseConfiguration { Z = 100 });
            Check(scene.HasTool && scene.JointGroups.Count == 6, "The KR 35 scene is incomplete.");
            scene.Root.UpdateTransform(Matrix4d.Identity);
            double[] homeOrigin = [1153.3, 0, 1483.5];
            double[] homeDirections = [0, 0, -1, 0, 1, 0, 1, 0, 0];
            AssertFrame(flange, homeOrigin, homeDirections);
            AssertFrame(mount, homeOrigin, homeDirections);
            AssertFrame(tcp, [1253.3, 0, 1483.5], homeDirections);

            (double Delta, double[] Origin, double[] Directions)[] cases =
            [
                (90, [0, -1153.3, 1483.5], [0, 0, -1, 1, 0, 0, 0, -1, 0]),
                (90, [1150, 0, -419.8], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
                (-90, [30, 0, 2266.8], [1, 0, 0, 0, 1, 0, 0, 0, 1]),
                (90, homeOrigin, [0, -1, 0, 0, 0, -1, 1, 0, 0]),
                (90, [1022.8, 0, 1353], [-1, 0, 0, 0, 1, 0, 0, 0, -1]),
                (90, homeOrigin, [0, -1, 0, 0, 0, -1, 1, 0, 0])
            ];
            for (var index = 0; index < cases.Length; index++)
            {
                var joint = configuration.Joints[index];
                using var origin = new GPnt(joint.AxisOrigin.X, joint.AxisOrigin.Y, joint.AxisOrigin.Z);
                using var direction = new GDir(joint.AxisDirection.X, joint.AxisDirection.Y, joint.AxisDirection.Z);
                using var axis = new GAx1(origin, direction);
                using var rotation = new GTrsf();
                rotation.SetRotation(axis, cases[index].Delta * Math.PI / 180);
                using var matrix = Matrix4d.From(rotation);
                scene.JointGroups[index].SetTransform(matrix);
                scene.Root.UpdateTransform(Matrix4d.Identity);
                AssertFrame(flange, cases[index].Origin, cases[index].Directions);
                AssertFrame(mount, cases[index].Origin, cases[index].Directions);
                var tcpOrigin = Enumerable.Range(0, 3)
                    .Select(coordinate => cases[index].Origin[coordinate] + 100 * cases[index].Directions[6 + coordinate]).ToArray();
                AssertFrame(tcp, tcpOrigin, cases[index].Directions);
                AssertFrame(baseAxes, [0, 0, 0], [1, 0, 0, 0, 1, 0, 0, 0, 1]);
                Check(!scene.Root.GetInfiniteSize(), "The KR 35 scene was excluded from camera clipping bounds.");
                scene.JointGroups[index].SetTransform(Matrix4d.Identity);
            }
        }

        var progress = new List<RobotLoadProgress>();
        using var cached = RobotSceneContent.Load(configuration, Kr35ModelId, progress.Add, cacheRoot);
        Check(!cached.HasTool && progress.Count(state => state.FromCache) == 7,
            "Reloading the KR 35 model did not reuse all seven robot caches.");
    }

    private static void VerifyCoordinateFrames(RobotSceneContent scene)
    {
        using var baseAxes = FindNode(scene.Root, "BaseAxes");
        using var flange = FindGroup(scene.JointGroups[5], "FlangeFrame");
        using var mount = FindGroup(flange, "ToolMount");
        using var tcp = FindGroup(flange, "TcpFrame");
        using var flangeAxes = FindNode(flange, "FlangeAxes");
        using var tcpAxes = FindNode(tcp, "TcpAxes");
        foreach (var axes in new[] { baseAxes, flangeAxes, tcpAxes })
        {
            using var widget = AxisWidget.Cast(axes);
            using var parts = widget.GetNodes();
            using var iterator = parts.CreateIterator();
            var arrowParts = 0;
            while (iterator.More())
            {
                using var part = iterator.Current();
                using var primitive = PrimitiveSceneNode.Cast(part);
                using var material = primitive?.GetMaterial();
                if (material is not null)
                {
                    Check(material.IsFixedSize() && !material.GetDepthWrite(),
                        "Coordinate arrows must keep their screen size and must not hide model surfaces through the depth buffer.");
                    arrowParts++;
                }
                iterator.Next();
            }
            Check(arrowParts >= 3, "Coordinate arrow render materials are missing.");
        }
        scene.Root.UpdateTransform(Matrix4d.Identity);
        Check(!scene.Root.GetInfiniteSize(), "Coordinate helpers excluded the robot from camera fitting and clipping bounds.");
        using (var robotBounds = scene.Root.GetWorldBBox())
        {
            using var basePoint = new Vector3d(0, 0, 0);
            using var flangePoint = new Vector3d(930, 0, 1055);
            Check(robotBounds.isFinite() && !robotBounds.isNull() && robotBounds.contains(basePoint) && robotBounds.contains(flangePoint),
                "The scene bounds must contain both the base and flange for camera orbiting.");
        }
        AssertFrame(flange, [930, 0, 1055], [0, 0, -1, 0, 1, 0, 1, 0, 0]);
        AssertFrame(mount, [930, 0, 1055], [0, 0, -1, 0, 1, 0, 1, 0, 0]);
        AssertFrame(baseAxes, [0, 0, 0], [1, 0, 0, 0, 1, 0, 0, 0, 1]);

        scene.SetToolTcp(new PoseConfiguration { X = 10, Y = 20, Z = 30, A = 90 });
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(tcp, [960, 20, 1045], [0, 1, 0, 0, 0, 1, 1, 0, 0]);
        AssertFrame(mount, [930, 0, 1055], [0, 0, -1, 0, 1, 0, 1, 0, 0]);

        using var origin = new GPnt(0, 0, 0);
        using var direction = new GDir(0, 0, 1);
        using var axis = new GAx1(origin, direction);
        using var rotation = new GTrsf();
        rotation.SetRotation(axis, Math.PI / 2);
        using var matrix = Matrix4d.From(rotation);
        scene.JointGroups[0].SetTransform(matrix);
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(flange, [0, 930, 1055], [0, 0, -1, -1, 0, 0, 0, 1, 0]);
        AssertFrame(mount, [0, 930, 1055], [0, 0, -1, -1, 0, 0, 0, 1, 0]);
        AssertFrame(tcp, [-20, 960, 1045], [-1, 0, 0, 0, 0, 1, 0, 1, 0]);
        AssertFrame(baseAxes, [0, 0, 0], [1, 0, 0, 0, 1, 0, 0, 0, 1]);

        using var wristOrigin = new GPnt(830, 0, 1055);
        using var wristDirection = new GDir(-1, 0, 0);
        using var wristAxis = new GAx1(wristOrigin, wristDirection);
        rotation.SetRotation(wristAxis, Math.PI / 2);
        using var wristMatrix = Matrix4d.From(rotation);
        scene.JointGroups[5].SetTransform(wristMatrix);
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(flange, [0, 930, 1055], [1, 0, 0, 0, 0, -1, 0, 1, 0]);
        AssertFrame(mount, [0, 930, 1055], [1, 0, 0, 0, 0, -1, 0, 1, 0]);
        Check(!scene.Root.GetInfiniteSize(), "Moving joints marked the scene bounds as infinite.");
        AssertFrame(tcp, [10, 960, 1035], [0, 0, -1, -1, 0, 0, 0, 1, 0]);

        scene.SetCoordinateVisibility(false, false, true);
        Check(!baseAxes.GetVisible() && !flangeAxes.GetVisible() && tcpAxes.GetVisible() && flange.GetVisible(),
            "Hiding the flange axes also hid the TCP frame.");
        scene.SetCoordinateVisibility(true, true, false);
        Check(baseAxes.GetVisible() && flangeAxes.GetVisible() && !tcpAxes.GetVisible(),
            "The TCP visibility control changed the other frames.");
        scene.SetCoordinateVisibility(true, true, true);

        scene.JointGroups[0].SetTransform(Matrix4d.Identity);
        scene.JointGroups[5].SetTransform(Matrix4d.Identity);
        scene.SetToolTcp(new PoseConfiguration { B = 90 });
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(tcp, [930, 0, 1055], [-1, 0, 0, 0, 1, 0, 0, 0, -1]);
        scene.SetToolTcp(new PoseConfiguration { C = 90 });
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(tcp, [930, 0, 1055], [0, 0, -1, 1, 0, 0, 0, -1, 0]);
        scene.SetToolTcp(new PoseConfiguration());
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(tcp, [930, 0, 1055], [0, 0, -1, 0, 1, 0, 1, 0, 0]);
    }

    private static void AssertFrame(SceneNode frame, double[] origin, double[] directions)
    {
        using var transform = frame.GetWorldTransform();
        for (var coordinate = 0; coordinate < 3; coordinate++)
        {
            Check(Near(transform.get(3, coordinate), origin[coordinate]), $"{frame.GetName()} origin is incorrect.");
            for (var axis = 0; axis < 3; axis++)
                Check(Near(transform.get(axis, coordinate), directions[axis * 3 + coordinate]),
                    $"{frame.GetName()} axis {axis}, component {coordinate}: expected {directions[axis * 3 + coordinate]}, got {transform.get(axis, coordinate)}.");
        }
    }

    private static void VerifyToolScene(RobotSceneContent scene, MeshStats expected)
    {
        Check(scene.HasTool && scene.JointGroups.Count == 6, "Tool or joint hierarchy is missing.");
        using var flange = FindGroup(scene.JointGroups[5], "FlangeFrame");
        using var mount = FindGroup(flange, "ToolMount");
        scene.Root.UpdateTransform(Matrix4d.Identity);
        AssertFrame(mount, [930, 0, 1055], [0, 0, -1, 0, 1, 0, 1, 0, 0]);

        using var tool = FindGroup(mount, "Tool");
        Check(tool.GetCount() > 1, "Tool STEP colors were not split into separate display groups.");
        var combined = new MeshStats();
        using (var iterator = tool.CreateIterator())
        {
            while (iterator.More())
            {
                using var node = iterator.Current();
                using var brep = BrepSceneNode.Cast(node)
                    ?? throw new InvalidOperationException("Tool color group is not a BREP node.");
                using var shape = brep.GetTopoShape();
                combined.Add(shape);
                iterator.Next();
            }
        }
        Check(combined.SameAs(expected), $"Tool geometry changed when splitting colors: {combined} vs {expected}.");

        using var offset = Matrix4d.makeTranslate(12, -7, 3);
        scene.JointGroups[5].SetTransform(offset);
        scene.Root.UpdateTransform(Matrix4d.Identity);
        using var world = mount.GetWorldTransform();
        Check(Near(world.get(3, 0), 942) && Near(world.get(3, 1), -7) && Near(world.get(3, 2), 1058),
            "Tool did not follow the J6 scene node.");
    }

    private static GroupSceneNode FindGroup(GroupSceneNode parent, string name)
    {
        using var node = FindNode(parent, name);
        return GroupSceneNode.Cast(node) ?? throw new InvalidOperationException($"{name} is not a group.");
    }

    private static SceneNode FindNode(GroupSceneNode parent, string name)
    {
        using var iterator = parent.CreateIterator();
        while (iterator.More())
        {
            var node = iterator.Current();
            if (node.GetName() == name)
            {
                return node;
            }
            node.Dispose();
            iterator.Next();
        }
        throw new InvalidOperationException($"Group {name} was not found.");
    }

    private static bool Near(double actual, double expected) => Math.Abs(actual - expected) < 0.001;
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
        if (error is not null) throw error;
    }

    private sealed class MeshStats
    {
        private readonly double[] _minimum = [double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity];
        private readonly double[] _maximum = [double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity];
        private readonly double[] _sum = new double[3];
        private long _count;
        private long _faceCount;

        public static MeshStats FromShape(TopoShape shape)
        {
            var result = new MeshStats();
            result.Add(shape);
            return result;
        }

        public void Add(TopoShape shape)
        {
            using var representation = GRepShape.Create(shape, null, null, 1.0, false);
            Check(representation.Build(), "Could not mesh a Tool shape.");
            using var iterator = new GRepIterator();
            Check(iterator.Initialize(representation, EnumShapeFilter.Face), "Could not inspect Tool faces.");
            while (iterator.More())
            {
                _faceCount++;
                using var positions = iterator.GetPositions();
                if (positions is not null)
                {
                    for (uint index = 0; index + 2 < positions.GetItemCount(); index += 3)
                    {
                        for (uint axis = 0; axis < 3; axis++)
                        {
                            var value = positions.GetValue(index + axis);
                            _minimum[axis] = Math.Min(_minimum[axis], value);
                            _maximum[axis] = Math.Max(_maximum[axis], value);
                            _sum[axis] += value;
                        }
                        _count++;
                    }
                }
                iterator.Next();
            }
        }

        public bool SameAs(MeshStats other) => _faceCount == other._faceCount
            && Enumerable.Range(0, 3).All(axis => Math.Abs(_minimum[axis] - other._minimum[axis]) < 0.01
                && Math.Abs(_maximum[axis] - other._maximum[axis]) < 0.01
                && Math.Abs(_sum[axis] / _count - other._sum[axis] / other._count) < 3);

        public override string ToString() => $"{_faceCount} faces, {_count} points, bounds [{string.Join(", ", _minimum)}] to [{string.Join(", ", _maximum)}], mean [{string.Join(", ", _sum.Select(sum => sum / _count))}]";
    }

    /// <summary>只记录调用次数和参数，检查工具更换不会触发机器人重新加载。</summary>
    private sealed class FakeSceneService : IRobotSceneService
    {
        public int RobotLoadCount { get; private set; }
        public int ToolLoadCount { get; private set; }
        public int ApplyAllCount { get; private set; }
        public int TcpUpdates { get; private set; }
        public bool Disposed { get; private set; }
        public bool FailNextTool { get; set; }
        public bool FailInitialization { get; set; }
        public PoseConfiguration? LastTcp { get; private set; }
        public (bool, bool, bool) Visibility { get; private set; }
        public TaskCompletionSource<bool>? PendingToolLoad { get; set; }

        public void InitializeViewport()
        {
            if (FailInitialization) throw new InvalidOperationException("测试视口初始化失败。");
        }
        public Task LoadRobotAsync(RobotConfiguration configuration, string modelId, IProgress<RobotLoadProgress>? progress = null)
        {
            RobotLoadCount++;
            return Task.CompletedTask;
        }
        public async Task LoadToolAsync(string toolId, ToolConfiguration configuration, IProgress<RobotLoadProgress>? progress = null)
        {
            ToolLoadCount++;
            if (FailNextTool) { FailNextTool = false; throw new System.IO.InvalidDataException("测试工具加载失败。"); }
            if (PendingToolLoad is { } pending) { await pending.Task; PendingToolLoad = null; }
        }
        public void ApplyAll(IEnumerable<JointViewModel> joints)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            ApplyAllCount++;
        }
        public void ApplyJoint(JointViewModel joint) => ObjectDisposedException.ThrowIf(Disposed, this);
        public void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp) => Visibility = (showBase, showFlange, showTcp);
        public void SetToolTcp(PoseConfiguration pose)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            LastTcp = pose;
            TcpUpdates++;
        }
        public void SetView(RobotView view) { }
        public void ZoomAll() { }
        public void Dispose() => Disposed = true;
    }
}
