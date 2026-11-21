using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Macad.Interaction;
using Macad.Interaction.Panels;
using Macad.Occt;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>
/// 真实窗口烟雾检查：新建 ViewportPanel，用 RobotSceneService 加载机器人、Tool 与工装，
/// 等待 OCCT 渲染后把视口截图到输出目录，并检查截图里确实有机器人和模型边线（不只有渐变背景）。
/// </summary>
internal static class ViewportSmokeChecks
{
    const string RobotId = "KR_10_R1440-2";
    const string ToolId = "Tool";

    public static async Task RunAsync()
    {
        var outputDirectory = Path.Combine(AppContext.BaseDirectory, "viewport-smoke");
        Directory.CreateDirectory(outputDirectory);

        var window = new Window
        {
            Title = "Macad viewport smoke",
            Width = 960,
            Height = 720,
            Left = -4000,
            Top = -2000,
            WindowStartupLocation = WindowStartupLocation.Manual,
            ShowInTaskbar = false,
            Background = Brushes.Black,
        };
        var panel = new ViewportPanel();
        var host = new Grid();
        host.Children.Add(panel);
        window.Content = host;
        Application.Current.MainWindow = window;
        DeferredViewportHost.Defer(window, host, panel);
        window.Show();

        RobotSceneService? scene = null;
        try
        {
            await WaitAsync(() => InteractiveContext.Current?.ViewportController?.V3dView?.Window()?.IsMapped() == true,
                "Macad 视口窗口未就绪。");

            scene = new RobotSceneService(panel);
            scene.InitializeViewport();
            Check(panel.MouseControl is RobotViewportMouseControl, "视口没有使用项目约定的左键旋转手势。");

            var robot = RobotConfigurationStore.LoadBuiltIn(RobotId);
            await scene.LoadRobotAsync(robot, RobotId);

            var joints = robot.Joints.Select((joint, index) => new JointViewModel(index, joint)).ToArray();
            foreach (var joint in joints) joint.Angle = joint.HomeAngle;
            scene.ApplyAll(joints);

            var tool = ToolConfigurationStore.LoadBuiltIn(ToolId);
            await scene.LoadToolAsync(ToolId, tool);
            CheckToolMount(scene, joints, tool.Tcp);

            var workId = ModelResources.WorkIds.First(id => id.Contains("R边", StringComparison.Ordinal));
            var work = WorkConfigurationStore.LoadBuiltIn(workId);
            await scene.LoadWorkAsync(workId, work);

            scene.SetCoordinateVisibility(true, true, true);
            await Task.Delay(1500);

            var view = InteractiveContext.Current?.ViewportController?.V3dView
                ?? throw new InvalidOperationException("Macad 视口未就绪，无法截图。");

            view.Redraw();

            var dumpPath = Path.Combine(outputDirectory, "viewport-smoke.png");
            view.Dump(dumpPath);
            Check(File.Exists(dumpPath), "视口截图没有生成。");
            Check(new FileInfo(dumpPath).Length > 10_000, "视口截图内容过小，可能是空画面。");

            var stats = Inspect(dumpPath);
            Check(stats.Width >= 600 && stats.Height >= 400, $"视口截图尺寸异常：{stats.Width}x{stats.Height}。");
            Check(stats.RowsWithModel >= stats.Height / 10,
                $"视口截图几乎只有背景：{stats.RowsWithModel}/{stats.Height} 行含模型。");
            Check(stats.OrangePixels >= 1000, $"截图里的橙色机器人过少：{stats.OrangePixels} 像素。");
            Check(stats.DarkPixels >= 2000, $"截图里的模型边线过少：{stats.DarkPixels} 像素。");

            Console.WriteLine(
                $"PASS: Macad viewport renders robot, tool and work " +
                $"({stats.Width}x{stats.Height}, {stats.RowsWithModel}/{stats.Height} rows, " +
                $"{stats.OrangePixels} robot px, {stats.DarkPixels} edge px): {dumpPath}");

            // 验证真实 V3d_View 上的鼠标转盘旋转严格围绕模型包围盒中心 _orbitCenter 且保持 Z-up 无滚转。
            var pivot = (Pnt)typeof(RobotSceneService)
                .GetField("_orbitCenter", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(scene)!;
            var beforeCam = view.Camera();
            var beforeEye = beforeCam.Eye();
            var beforeCenter = beforeCam.Center();
            double radiusBefore = new Vec(pivot, beforeEye).Magnitude();
            double centerRadiusBefore = new Vec(pivot, beforeCenter).Magnitude();

            scene.OrbitByPointer(-25, -10);

            var afterCam = view.Camera();
            var afterEye = afterCam.Eye();
            var afterCenter = afterCam.Center();
            var afterUp = afterCam.Up();
            double radiusAfter = new Vec(pivot, afterEye).Magnitude();
            double centerRadiusAfter = new Vec(pivot, afterCenter).Magnitude();
            var afterRight = new Vec(afterEye, afterCenter).ToDir().ToVec().Crossed(afterUp.ToVec());
            Check(new Vec(beforeEye, afterEye).Magnitude() > 1.0, "真实视口调用 OrbitByPointer 后相机位置未移动。");
            Check(Math.Abs(radiusAfter - radiusBefore) < 1e-4, "真实视口鼠标旋转后 Eye 到模型中心距离未保持不变。");
            Check(Math.Abs(centerRadiusAfter - centerRadiusBefore) < 1e-4, "真实视口鼠标旋转后 Center 到模型中心半径未保持不变。");
            Check(afterUp.Z > 0 && Math.Abs(afterRight.Z) < 1e-6, "真实视口鼠标旋转后未保持 Z-up 无滚转。");
            Console.WriteLine("PASS: real viewport OrbitByPointer rotates around _orbitCenter with Z-up constraint");

            // 验证未按键移出视口触发 Cancel 时，不误暂停自动环绕。
            scene.SetOrbit(15, -45, 35.264);
            Check(!scene.IsUserAdjustingView, "开启自动环绕后不应立即处于用户调整状态。");
            panel.MouseControl.Cancel();
            Check(!scene.IsUserAdjustingView, "未按键移出视口触发 Cancel 后，自动环绕被误暂停。");
            Console.WriteLine("PASS: idle viewport mouse leave (Cancel) does not pause auto-orbit");
        }
        finally
        {
            scene?.Dispose();
            window.Close();
        }
    }

    static void CheckToolMount(RobotSceneService scene, JointViewModel[] joints, PoseConfiguration originalTcp)
    {
        const BindingFlags fields = BindingFlags.Instance | BindingFlags.NonPublic;
        var content = typeof(RobotSceneService).GetField("_content", fields)!.GetValue(scene);
        var display = (AIS_ColoredShape)typeof(RobotSceneContent).GetField("_toolDisplay", fields)!.GetValue(content)!;

        // KR 10 R1440-2 的法兰安装中心和方向，按原始 CAD 姿态验证实际显示对象。
        CheckPoint(Pnt.Origin.Transformed(display.LocalTransformation()), 930, 0, 1055);
        CheckPoint(new Pnt(100, 0, 0).Transformed(display.LocalTransformation()), 930, 0, 955);
        CheckPoint(new Pnt(0, 0, 100).Transformed(display.LocalTransformation()), 1030, 0, 1055);

        joints[0].Angle = joints[0].HomeAngle + 90;
        scene.ApplyJoint(joints[0]);
        CheckPoint(Pnt.Origin.Transformed(display.LocalTransformation()), 0, -930, 1055);
        CheckPoint(new Pnt(0, 0, 100).Transformed(display.LocalTransformation()), 0, -1030, 1055);

        scene.SetToolTcp(new PoseConfiguration { X = 100, Z = 200, A = 45 });
        CheckPoint(Pnt.Origin.Transformed(display.LocalTransformation()), 0, -930, 1055);
        CheckPoint(new Pnt(0, 0, 100).Transformed(display.LocalTransformation()), 0, -1030, 1055);

        joints[0].Angle = joints[0].HomeAngle;
        scene.ApplyJoint(joints[0]);
        scene.SetToolTcp(originalTcp);
        Console.WriteLine("PASS: Tool mount position and direction, joint motion and independent TCP");
    }

    static void CheckPoint(Pnt actual, double x, double y, double z)
    {
        const double tolerance = 1e-6;
        Check(Math.Abs(actual.X - x) < tolerance && Math.Abs(actual.Y - y) < tolerance && Math.Abs(actual.Z - z) < tolerance,
            $"Tool 安装变换错误。期望 ({x}, {y}, {z})，实际 ({actual.X:F3}, {actual.Y:F3}, {actual.Z:F3}) mm。");
    }

    static async Task WaitAsync(Func<bool> condition, string message)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            if (condition()) return;
            await Task.Delay(50);
        }
        throw new InvalidOperationException(message);
    }

    /// <summary>统计截图：背景是垂直线性渐变，同一行颜色一致；有模型的行会出现明显横向色差，机器人本体为橙色，BREP 边线为深色。</summary>
    static (int Width, int Height, int RowsWithModel, int OrangePixels, int DarkPixels) Inspect(string path)
    {
        var decoder = new PngBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        var frame = decoder.Frames[0];
        var width = frame.PixelWidth;
        var height = frame.PixelHeight;
        var converted = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0);
        var stride = width * 4;
        var pixels = new byte[stride * height];
        converted.CopyPixels(pixels, stride, 0);

        var rowsWithModel = 0;
        var orangePixels = 0;
        var darkPixels = 0;
        for (var y = 0; y < height; y++)
        {
            var rowStart = y * stride;
            int minBlue = 255, maxBlue = 0, minGreen = 255, maxGreen = 0, minRed = 255, maxRed = 0;
            for (var x = 0; x < width; x += 2)
            {
                var offset = rowStart + x * 4;
                var blue = pixels[offset];
                var green = pixels[offset + 1];
                var red = pixels[offset + 2];
                minBlue = Math.Min(minBlue, blue); maxBlue = Math.Max(maxBlue, blue);
                minGreen = Math.Min(minGreen, green); maxGreen = Math.Max(maxGreen, green);
                minRed = Math.Min(minRed, red); maxRed = Math.Max(maxRed, red);
                if (red > 140 && green < 130 && blue < 100) orangePixels++;
                if (red < 80 && green < 80 && blue < 80) darkPixels++;
            }

            var spread = Math.Max(maxRed - minRed, Math.Max(maxGreen - minGreen, maxBlue - minBlue));
            if (spread > 10) rowsWithModel++;
        }

        return (width, height, rowsWithModel, orangePixels, darkPixels);
    }

    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
