using System.Windows;
using System.Windows.Input;
using Macad.Occt;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>检查切换标准视角与鼠标 Z-up 绕模型中心转盘旋转约束。</summary>
internal static class CameraViewChecks
{
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

    public static void RunOrbitChecks()
    {
        var pivot = new Pnt(150, -80, 420);
        // 包含平移偏移（center != pivot），验证绕模型包围盒中心旋转时半径与相对位置均保持不变。
        var startEye = new Pnt(1650, -1080, 1420);
        var startCenter = new Pnt(250, -30, 500);
        var startUp = new Dir(0, 0, 1);

        double expectedEyeDist = Distance(startEye, pivot);
        double expectedCenterDist = Distance(startCenter, pivot);

        // 1. 纯水平偏航应严格绕 pivot 的 Z 轴旋转且保持 Z 不变。
        var (yawEye, yawCenter, yawUp) = RobotSceneService.OrbitCameraAroundPivot(
            startEye, startCenter, startUp, pivot, yawDegrees: 33.5, pitchDegrees: 0);
        double rad = 33.5 * Math.PI / 180.0;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);
        var refEye = new Pnt(
            pivot.X + (startEye.X - pivot.X) * cos - (startEye.Y - pivot.Y) * sin,
            pivot.Y + (startEye.X - pivot.X) * sin + (startEye.Y - pivot.Y) * cos,
            startEye.Z);
        var refCenter = new Pnt(
            pivot.X + (startCenter.X - pivot.X) * cos - (startCenter.Y - pivot.Y) * sin,
            pivot.Y + (startCenter.X - pivot.X) * sin + (startCenter.Y - pivot.Y) * cos,
            startCenter.Z);
        Check(NearPoint(yawEye, refEye, 1e-6) && NearPoint(yawCenter, refCenter, 1e-6),
            "纯水平偏航应与绕 Z 轴精确旋转结果完全一致。");
        Check(yawUp.Z > 0, "水平旋转后相机 Up.Z 必须保持向上。");

        // 2. 连续 40 步斜向拖拽：锁定支点距离、零滚转与 Z-up 约束。
        var eye = startEye;
        var center = startCenter;
        var up = startUp;
        for (int i = 0; i < 40; i++)
        {
            double yaw = (i % 2 == 0 ? 11.3 : -7.9);
            double pitch = (i < 20 ? -2.2 : 1.7);
            (eye, center, up) = RobotSceneService.OrbitCameraAroundPivot(eye, center, up, pivot, yaw, pitch);
            Check(Math.Abs(Distance(eye, pivot) - expectedEyeDist) < 1e-6, "旋转后 Eye 到旋转中心的半径发生漂移。");
            Check(Math.Abs(Distance(center, pivot) - expectedCenterDist) < 1e-6, "旋转后 Center 到旋转中心的半径发生漂移。");

            var viewDir = new Vec(eye, center).ToDir().ToVec();
            var upVec = up.ToVec();
            var right = viewDir.Crossed(upVec);
            Check(up.Z > 0, $"第 {i} 步旋转后出现相机倒置：Up.Z={up.Z}。");
            Check(Math.Abs(viewDir.Dot(upVec)) < 1e-9, "旋转后相机 Up 与视线不再正交。");
            Check(Math.Abs(right.Z) < 1e-9, $"旋转后出现非零相机滚转：right.Z={right.Z}。");
        }

        // 3. ±89° 极点限位，不翻转也不发散。
        var (topClampedEye, _, topClampedUp) = RobotSceneService.OrbitCameraAroundPivot(
            startEye, pivot, startUp, pivot, yawDegrees: 0, pitchDegrees: -500);
        var topBack = new Vec(pivot, topClampedEye).ToDir();
        double topElev = Math.Atan2(topBack.Z, Math.Sqrt(topBack.X * topBack.X + topBack.Y * topBack.Y)) * 180.0 / Math.PI;
        Check(topElev <= 89.0001 && topElev > 88.5 && topClampedUp.Z > 0, $"上极点仰角限位异常：{topElev}°。");

        var (botClampedEye, _, botClampedUp) = RobotSceneService.OrbitCameraAroundPivot(
            startEye, pivot, startUp, pivot, yawDegrees: 0, pitchDegrees: 500);
        var botBack = new Vec(pivot, botClampedEye).ToDir();
        double botElev = Math.Atan2(botBack.Z, Math.Sqrt(botBack.X * botBack.X + botBack.Y * botBack.Y)) * 180.0 / Math.PI;
        Check(botElev >= -89.0001 && botElev < -88.5 && botClampedUp.Z > 0, $"下极点仰角限位异常：{botElev}°。");

        // 4. 从正顶视图交互（Up=(0,1,0)）退出不万向节死锁、不翻转。
        var topEye = new Pnt(100, 200, 2200);
        var topCenter = new Pnt(100, 200, 200);
        var topUp = new Dir(0, 1, 0);
        var (downDragEye, _, downDragUp) = RobotSceneService.OrbitCameraAroundPivot(topEye, topCenter, topUp, topCenter, 0, -10);
        Check(double.IsFinite(downDragEye.X) && Math.Abs(Distance(downDragEye, topCenter) - 2000.0) < 1e-6 && double.IsFinite(downDragUp.Z),
            "正顶视图继续俯仰应安全保持稳定。");

        var (exitEye, _, exitUp) = RobotSceneService.OrbitCameraAroundPivot(topEye, topCenter, topUp, topCenter, 0, 10);
        var exitBack = new Vec(topCenter, exitEye).ToDir();
        double exitElev = Math.Atan2(exitBack.Z, Math.Sqrt(exitBack.X * exitBack.X + exitBack.Y * exitBack.Y)) * 180.0 / Math.PI;
        Check(Math.Abs(exitElev - 80.0) < 0.5 && exitUp.Z > 0, $"从正顶视图向外拖出角度异常：{exitElev}°。");

        var (spinEye, _, spinUp) = RobotSceneService.OrbitCameraAroundPivot(topEye, topCenter, topUp, topCenter, 30, 0);
        Check(Math.Abs(spinUp.Z) < 1e-6 && Math.Abs(new Vec(topCenter, spinEye).Magnitude() - 2000.0) < 1e-6,
            "正顶视图纯偏航旋转异常。");

        Console.WriteLine("PASS: Z-up turntable orbit around pivot (radius lock, zero roll, +/-89° clamp, top-view stability)");
    }

    public static void RunMouseControlChecks()
    {
        var events = new List<bool>();
        var control = new RobotViewportMouseControl(held => events.Add(held));

        // 1. 未按任何视口操作键时移动、移出(Cancel)、松键或按非操作扩展键，均不应误触发交互状态。
        control.MouseMove(new Point(10, 10), null!, ModifierKeys.None);
        control.Cancel();
        control.MouseUp(new Point(10, 10), MouseButton.Left, null!, ModifierKeys.None);
        control.MouseDown(new Point(10, 10), MouseButton.XButton1, 1, null!, ModifierKeys.None);
        Check(events.Count == 0, $"空闲鼠标移动或移出视口误触发了交互通知：{string.Join(",", events)}。");

        // 2. 在视口内按下左键后移出视口(Cancel)应只通知一次结束，重复 Cancel 不得再次触发。
        control.MouseDown(new Point(20, 20), MouseButton.Left, 1, null!, ModifierKeys.None);
        control.Cancel();
        control.Cancel();
        Check(events.SequenceEqual(new[] { true, false }), $"左键按下后 Cancel 通知序列异常：{string.Join(",", events)}。");

        // 3. 右键正常按下并松开后，再移出视口(Cancel)不应额外触发通知。
        events.Clear();
        control.MouseDown(new Point(30, 30), MouseButton.Right, 1, null!, ModifierKeys.None);
        control.MouseUp(new Point(40, 40), MouseButton.Right, null!, ModifierKeys.None);
        control.Cancel();
        Check(events.SequenceEqual(new[] { true, false }), $"右键松开后 Cancel 通知序列异常：{string.Join(",", events)}。");

        Console.WriteLine("PASS: idle mouse leave/cancel does not trigger false pointer interaction");
    }

    static double Distance(Pnt a, Pnt b) => new Vec(a, b).Magnitude();

    static bool NearPoint(Pnt a, Pnt b, double tol)
        => Math.Abs(a.X - b.X) <= tol && Math.Abs(a.Y - b.Y) <= tol && Math.Abs(a.Z - b.Z) <= tol;

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
