using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using AnyCAD.WPF;
using Microsoft.Xaml.Behaviors;
using RobotSimulator.Behaviors;
using RobotSimulator.Controls;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>用真实 WPF 控件检查通信线程、加载期间最新帧和加载失败后的恢复。</summary>
internal static class RobotViewerChecks
{
    public static async Task RunAsync()
    {
        var scene = new RecordingScene { RobotGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        var model = new Digital_Simulation(new DigitalSimulationConfiguration
        {
            Robot_Model = RobotModelKind.KR_10_R1440_2,
            // 关闭平滑，测试直接检查最新一帧角度。
            Joint_Response_Frequency = 0,
        });
        model.SetJoints([1, 2, 3, 4, 5, 6]);
        Check(model.Joints.SequenceEqual(new double[] { 1, 2, 3, 4, 5, 6 }), "SetJoints 没有保存 J1～J6。");
        model.ResetJointsToHome();
        Check(model.Joints.SequenceEqual(new double[] { 0, -90, 90, 0, 0, 0 }), "ResetJointsToHome 没有回到内置初始角。");
        var stepAngle = 0.0;
        var stepVelocity = 0.0;
        JointMotion.Step(ref stepAngle, ref stepVelocity, 30, 1.0 / 60, 2.0, 360, 1440, 0.02);
        Check(stepAngle > 0 && stepAngle < 30, "速度控制没有逐步逼近目标。");
        for (var index = 0; index < 180; index++)
            JointMotion.Step(ref stepAngle, ref stepVelocity, 30, 1.0 / 60, 2.0, 360, 1440, 0.02);
        Check(Math.Abs(stepAngle - 30) <= 0.02 && Math.Abs(stepVelocity) <= 0.5,
            "速度控制在 3 秒内没有停在目标。");
        using (var viewer = new RobotViewer(_ => scene) { Model = model })
        {
            Check(!viewer.ZoomAllCommand.CanExecute(null), "模型加载前不应允许双击居中。");
            var load = viewer.LoadAsync();
            Check(ReferenceEquals(load, viewer.LoadAsync()), "自动和公开入口应共用同一次加载。");
            NotifyReady(viewer);
            await WaitUntilAsync(() => scene.RobotLoadCount == 1);
            await Task.Run(() => model.Joints = [1, -45, 70, 4, 5, 6]);
            await DrainAsync();
            Check(scene.ApplyCount == 0, "完整加载前不应消费关节帧。");
            await Task.Run(() => viewer.SetJoints([12, -55, 80, 14, 15, 16]));
            await DrainAsync();
            scene.RobotGate!.TrySetResult(true);
            await load;
            Check(scene.Angles.SequenceEqual(new double[] { 12, -55, 80, 14, 15, 16 }), "加载完成没有应用最新角度。");
            var render = (RenderControl)viewer.FindName("RenderView");
            var action = Interaction.GetTriggers(render).OfType<ViewerDoubleClickTrigger>().Single()
                .Actions.OfType<InvokeCommandAction>().Single();
            Check(ReferenceEquals(action.Command, viewer.ZoomAllCommand), "嵌入视口的双击没有绑定控件自身的居中命令。");
            Check(action.Command.CanExecute(null), "模型加载后居中命令未启用。");
            action.Command.Execute(null);
            Check(scene.ZoomAllCount == 1, "居中命令没有复用场景适配功能。");

            var before = scene.ApplyCount;
            // 多次通信通知到达同一界面帧时，只需要最后一帧。
            await Task.Run(() =>
            {
                for (var index = 0; index < 100; index++) model.Joints = [index, -55, 80, 14, 15, 16];
            });
            await DrainAsync();
            Check(scene.Angles[0] == 99 && scene.ApplyCount > before && scene.ApplyCount - before < 100,
                "后台绑定更新丢失或未合并待处理帧。");
            viewer.Dispose();
            Check(!viewer.ZoomAllCommand.CanExecute(null), "控件释放后不应允许双击居中。");
        }

        // 在线替换 Model：已加载控件应释放旧场景、按新参数重建，并保留最近一帧角度。
        var reloadScenes = new System.Collections.Generic.List<RecordingScene>();
        using (var viewer = new RobotViewer(_ =>
               {
                   var scene = new RecordingScene();
                   reloadScenes.Add(scene);
                   return scene;
               })
               { Model = new Digital_Simulation(new DigitalSimulationConfiguration()) })
        {
            var load = viewer.LoadAsync();
            NotifyReady(viewer);
            await load;
            Check(reloadScenes.Count == 1 && reloadScenes[0].RobotLoadCount == 1, "首次加载没有创建场景。");
            viewer.SetJoints([11, -22, 33, 44, 55, 66]);
            await DrainAsync();

            viewer.Model = new Digital_Simulation(new DigitalSimulationConfiguration
            {
                Robot_Model = RobotModelKind.KR_10_R1440_2,
                Orbit_Speed = 5
            });
            await WaitUntilAsync(() => reloadScenes.Count == 2 && reloadScenes[1].RobotLoadCount == 1);
            Check(reloadScenes[0].Disposed, "重新加载前没有释放旧场景。");
            Check(reloadScenes[1].Angles.SequenceEqual(new double[] { 11, -22, 33, 44, 55, 66 }),
                "重新加载没有保留最近一帧角度。");
        }

        // 非模型参数变化直接应用到已加载场景，不重建模型。
        var liveConfig = new DigitalSimulationConfiguration();
        var liveScene = new RecordingScene();
        using (var viewer = new RobotViewer(_ => liveScene) { Model = new Digital_Simulation(liveConfig) })
        {
            var load = viewer.LoadAsync();
            NotifyReady(viewer);
            await load;
            var robotLoads = liveScene.RobotLoadCount;
            var orbits = liveScene.OrbitCount;
            liveConfig.Orbit_Speed = 5;
            Check(liveScene.OrbitCount > orbits, "自转参数变化没有实时应用。");
            var visibilities = liveScene.VisibilityCount;
            liveConfig.Show_Base_Axes = false;
            Check(liveScene.VisibilityCount > visibilities, "显示开关变化没有实时应用。");
            Check(liveScene.RobotLoadCount == robotLoads, "非模型参数变化不应重建场景。");
            // 仿真开关关闭时无视机器人状态；开启后焊接光跟随通信状态。
            var weldVisibilities = liveScene.WeldVisibilityCount;
            liveConfig.Show_Weld_Beam = false;
            Check(liveScene.WeldVisibilityCount > weldVisibilities && !liveScene.WeldBeamVisible,
                "关闭焊接光开关后仍显示焊接光。");
            liveConfig.Show_Weld_Beam = true;
            Check(liveScene.WeldBeamVisible, "重新开启焊接光开关后没有显示焊接光。");
            var digitalModel = (Digital_Simulation)viewer.Model!;
            digitalModel.WeldActive = false;
            Check(!liveScene.WeldBeamVisible, "机器人焊接状态关闭后仍显示焊接光。");
            digitalModel.WeldActive = true;
            Check(liveScene.WeldBeamVisible, "机器人焊接状态开启后没有显示焊接光。");
            // 机器人选择变化只更新实时参数，加载快照保持原值，宿主据此判断保存时是否重建。
            var loadedRobot = viewer.Model is Digital_Simulation digital ? digital.LoadOptions.Robot : default;
            liveConfig.Robot_Model = RobotModelKind.KR_10_R1440_2;
            Check(viewer.Model is Digital_Simulation current
                  && current.LoadOptions.Robot == loadedRobot && current.Options.Robot == liveConfig.Robot_Model,
                "机器人选择变化没有保留加载快照。");
        }

        var partialScene = new RecordingScene();
        using (var viewer = new RobotViewer(_ => partialScene) { Model = new Digital_Simulation(
                   new DigitalSimulationConfiguration { Robot_Model = RobotModelKind.KR_10_R1440_2 }) })
        {
            viewer.SetJoint(0, 25);
            var load = viewer.LoadAsync();
            NotifyReady(viewer);
            await load;
            Check(partialScene.Angles.SequenceEqual(new double[] { 25, -90, 90, 0, 0, 0 }),
                "加载前只更新一轴不应把其它轴的初始角清零。");
        }

        var failedScene = new RecordingScene { FailInitialization = true };
        var recoveredScene = new RecordingScene();
        var attempts = 0;
        using (var viewer = new RobotViewer(_ => attempts++ == 0 ? failedScene : recoveredScene)
               { Model = new Digital_Simulation(new DigitalSimulationConfiguration()) })
        {
            var load = viewer.LoadAsync();
            NotifyReady(viewer);
            try { await load; throw new InvalidOperationException("预期首次初始化失败。"); }
            catch (InvalidDataException) { }
            Check(failedScene.Disposed, "失败场景没有释放。");
            var corrected = new Digital_Simulation(new DigitalSimulationConfiguration());
            viewer.Model = corrected;
            await WaitUntilAsync(() => corrected.StatusMessage == "模型已加载");
            Check(attempts == 2 && recoveredScene.ApplyCount >= 1, "修正 Model 后没有重新加载。");
        }

        var closingScene = new RecordingScene { RobotGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        using (var viewer = new RobotViewer(_ => closingScene) { Model = new Digital_Simulation(new DigitalSimulationConfiguration()) })
        {
            var load = viewer.LoadAsync();
            NotifyReady(viewer);
            await WaitUntilAsync(() => closingScene.RobotLoadCount == 1);
            viewer.Dispose();
            closingScene.RobotGate!.TrySetResult(true);
            try { await load; throw new InvalidOperationException("关闭后不应继续装配场景。"); }
            catch (ObjectDisposedException) { }
            Check(closingScene.Disposed && closingScene.ApplyCount == 0, "关闭期间仍访问已释放场景。");
        }
        Console.WriteLine("PASS: RobotViewer worker-thread binding, latest-frame loading, single-axis home, retry and closing lifecycle");
    }

    /// <summary>独立视口只加载一台机器人，验证标准视角在多个后续计时帧中保持。</summary>
    public static async Task RunCameraAsync()
    {
        var render = new RenderControl();
        var ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        render.ViewerReady += () => ready.TrySetResult(true);
        var window = new Window { Content = render, Width = 640, Height = 480,
            Left = -5000, Top = -5000, ShowInTaskbar = false, ShowActivated = false };
        using var scene = new RobotSceneService(render);
        try
        {
            window.Show();
            if (render.IsReady) ready.TrySetResult(true);
            await ready.Task.WaitAsync(TimeSpan.FromSeconds(15));
            scene.InitializeViewport();
            await scene.LoadRobotAsync(RobotConfigurationStore.LoadBuiltIn("KR_10_R1440-2"), "KR_10_R1440-2");
            await scene.LoadToolAsync("Tool", ToolConfigurationStore.LoadBuiltIn("Tool"));
            await scene.LoadWorkAsync("R边焊接工装", WorkConfigurationStore.LoadBuiltIn("R边焊接工装"));
            CameraViewChecks.RunNative(scene, render);
            ViewerDoubleClickChecks.Run(scene, render);
            Console.WriteLine("PASS: real front/top/isometric views use native camera animation and stay stable after mouse interaction");
        }
        finally { scene.Dispose(); window.Close(); }
    }

    /// <summary>模拟渲染器就绪通知，其余加载与通知逻辑均使用真实控件代码。</summary>
    private static void NotifyReady(RobotViewer viewer) => typeof(RobotViewer)
        .GetMethod("OnViewerReady", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(viewer, null);

    private static async Task DrainAsync() => await Dispatcher.CurrentDispatcher.InvokeAsync(
        () => { }, DispatcherPriority.ApplicationIdle);

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition())
        {
            if (DateTime.UtcNow > deadline) throw new TimeoutException("等待控件状态超时。");
            await Task.Delay(5);
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    /// <summary>记录真实控件的调用；加载门用于稳定复现通信先于模型准备完成的情况。</summary>
    private sealed class RecordingScene : IRobotSceneService
    {
        public TaskCompletionSource<bool>? RobotGate { get; init; }
        public bool FailInitialization { get; init; }
        public int RobotLoadCount { get; private set; }
        public int ApplyCount { get; private set; }
        public int ZoomAllCount { get; private set; }
        public int OrbitCount { get; private set; }
        public int VisibilityCount { get; private set; }
        public int WeldVisibilityCount { get; private set; }
        public bool WeldBeamVisible { get; private set; } = true;
        public double[] Angles { get; private set; } = [];
        public bool Disposed { get; private set; }
        public bool IsUserAdjustingView => false;
        public void InitializeViewport() { if (FailInitialization) throw new InvalidDataException("初始化失败用例"); }
        public async Task LoadRobotAsync(RobotConfiguration configuration, string modelId, IProgress<RobotLoadProgress>? progress = null)
        {
            RobotLoadCount++;
            if (RobotGate is not null) await RobotGate.Task;
        }
        public Task LoadToolAsync(string toolId, ToolConfiguration configuration, IProgress<RobotLoadProgress>? progress = null) => Task.CompletedTask;
        public Task LoadWorkAsync(string workId, WorkConfiguration configuration, IProgress<RobotLoadProgress>? progress = null) => Task.CompletedTask;
        public void ApplyAll(IEnumerable<JointViewModel> joints)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            Angles = joints.Select(joint => joint.Angle).ToArray();
            ApplyCount++;
        }
        public void ApplyJoint(JointViewModel joint) { }
        public void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp) => VisibilityCount++;
        public void SetWeldBeamVisible(bool show)
        {
            WeldVisibilityCount++;
            WeldBeamVisible = show;
        }
        public void SetToolTcp(PoseConfiguration pose) { }
        public void SetWorkPose(PoseConfiguration pose) { }
        public void SetView(RobotView view) { }
        public void ZoomAll() => ZoomAllCount++;
        public void SetOrbit(double speedDegreesPerSecond, double startAngleDegrees, double elevationDegrees) => OrbitCount++;
        public void Dispose() => Disposed = true;
    }
}
