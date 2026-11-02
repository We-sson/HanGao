using System.Reflection;
using System.Windows.Threading;
using AnyCAD.Foundation;
using AnyCAD.WPF;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>检查标准视角停止自转，并保留静止视口的鼠标重绘能力。</summary>
internal static class CameraViewChecks
{
    /// <summary>可配合现有轻量场景服务检查三个命令与页面速度一致。</summary>
    public static void Run(MainViewModel viewModel)
    {
        var originalSpeed = viewModel.OrbitSpeed;
        try
        {
            foreach (var command in new[] { viewModel.FrontCommand, viewModel.TopCommand, viewModel.IsometricCommand })
            {
                viewModel.OrbitSpeed = 15;
                command.Execute(null);
                Check(viewModel.OrbitSpeed == 0, "切换标准视角后，页面环绕速度仍不为 0。");
            }
        }
        finally { viewModel.OrbitSpeed = originalSpeed; }
    }

    /// <summary>在已加载的真实视口上检查后续计时帧；由具备图形环境的回归入口调用。</summary>
    public static void RunNative(RobotSceneService scene, RenderControl renderControl)
    {
        Check(scene.IsLoaded && renderControl.IsReady, "相机回归检查需要已加载模型的真实视口。");
        foreach (var view in new[] { RobotView.Front, RobotView.Top, RobotView.Isometric })
        {
            scene.SetOrbit(15, -45, 35.264);
            scene.SetView(view);
            var expected = ReadDirection(renderControl);
            var orbitTimer = (DispatcherTimer?)typeof(RobotSceneService)
                .GetField("_orbitTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(scene);
            Check(orbitTimer?.IsEnabled == true, "停止自转时不应停止鼠标重绘计时器。");

            // 运行多个真实计时帧，防止只检查设置后的瞬间而漏掉下一帧恢复旧方向。
            var frame = new DispatcherFrame();
            var stop = new DispatcherTimer(DispatcherPriority.ApplicationIdle) { Interval = TimeSpan.FromMilliseconds(100) };
            stop.Tick += (_, _) => frame.Continue = false;
            try
            {
                stop.Start();
                Dispatcher.PushFrame(frame);
            }
            finally { stop.Stop(); }

            var actual = ReadDirection(renderControl);
            Check(Math.Abs(actual.X - expected.X) < 1e-6 && Math.Abs(actual.Y - expected.Y) < 1e-6
                && Math.Abs(actual.Z - expected.Z) < 1e-6, $"{view} 在后续计时帧被旧环绕方向覆盖。");
        }
        CheckManualOrbit(scene, renderControl);
    }

    /// <summary>用 SDK 导航模拟拖动和平移，再连续转两圈，检查构图、比例和距离不漂移。</summary>
    private static void CheckManualOrbit(RobotSceneService scene, RenderControl render)
    {
        var type = typeof(RobotSceneService);
        scene.SetView(RobotView.Isometric);
        scene.SetOrbit(15, -45, 35.264);
        var timer = (DispatcherTimer)type.GetField("_orbitTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(scene)!;
        timer.Stop();
        using var context = render.Viewer.GetContext();
        context.Pan(100, 50);
        context.Orbit(80, 20, false);
        type.GetField("_orbitPaused", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(scene, true);
        type.GetMethod("ResumeOrbit", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(scene, null);
        // 几何支点用真实包围盒中心独立计算，不相信服务内部已被平移覆盖的字段。
        var content = (RobotSceneContent)type.GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(scene)!;
        using var bounds = content.GetModelBoundingBox();
        using var minimum = bounds.getMinimum();
        using var maximum = bounds.getMaximum();
        using var center = new Vector3((minimum.x + maximum.x) / 2, (minimum.y + maximum.y) / 2, (minimum.z + maximum.z) / 2);
        using var initialScreen = render.Viewer.WorldPointToSceenPoint(center);
        var camera = render.Camera;
        var initialDistance = camera.GetDistance();
        var initialZoom = camera.GetOrthoZoomFactor();
        Console.WriteLine($"manual orbit: distance={initialDistance:F3} zoom={initialZoom:F6} near={camera.GetNear():F3} far={camera.GetFar():F3}");
        var clock = (System.Diagnostics.Stopwatch)type.GetField("_orbitClock", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(scene)!;
        var frame = type.GetMethod("OnOrbitFrame", BindingFlags.Instance | BindingFlags.NonPublic)!;
        for (var index = 0; index < 48; index++)
        {
            type.GetField("_lastOrbitSeconds", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(scene, clock.Elapsed.TotalSeconds - 1);
            frame.Invoke(scene, [null, EventArgs.Empty]);
            using var screen = render.Viewer.WorldPointToSceenPoint(center);
            Check(Math.Abs(screen.x - initialScreen.x) < .5 && Math.Abs(screen.y - initialScreen.y) < .5,
                $"平移后续转发生构图漂移：第 {index} 帧，支点屏幕位置 {initialScreen.x},{initialScreen.y} -> {screen.x},{screen.y}。");
            Check(Math.Abs(camera.GetDistance() / initialDistance - 1) < 1e-6
                && Math.Abs(camera.GetOrthoZoomFactor() / initialZoom - 1) < 1e-6,
                "续转改变了用户选择的相机距离或正交比例。");
            using var eye = camera.GetPosition();
            using var target = camera.GetTarget();
            var distance = camera.GetDistance();
            var depthMin = double.MaxValue;
            var depthMax = double.MinValue;
            for (var corner = 0; corner < 8; corner++)
            {
                var x = (corner & 1) == 0 ? minimum.x : maximum.x;
                var y = (corner & 2) == 0 ? minimum.y : maximum.y;
                var z = (corner & 4) == 0 ? minimum.z : maximum.z;
                var depth = ((x - eye.x) * (target.x - eye.x) + (y - eye.y) * (target.y - eye.y)
                    + (z - eye.z) * (target.z - eye.z)) / distance;
                depthMin = Math.Min(depthMin, depth);
                depthMax = Math.Max(depthMax, depth);
            }
            Check(depthMin >= camera.GetNear() - .1 && depthMax <= camera.GetFar() + .1,
                $"旋转到第 {index} 帧时模型超出裁切范围：模型深度 {depthMin}..{depthMax}，相机 {camera.GetNear()}..{camera.GetFar()}。");
        }
    }

    private static (double X, double Y, double Z) ReadDirection(RenderControl renderControl)
    {
        using var position = renderControl.Camera.GetPosition();
        using var target = renderControl.Camera.GetTarget();
        var x = position.x - target.x;
        var y = position.y - target.y;
        var z = position.z - target.z;
        var distance = Math.Sqrt(x * x + y * y + z * z);
        Check(double.IsFinite(distance) && distance > 1e-6, "相机方向无效。");
        return (x / distance, y / distance, z / distance);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
