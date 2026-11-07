using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using AnyCAD.Foundation;
using AnyCAD.WPF;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors;
using RobotSimulator.Behaviors;
using RobotSimulator.Services;

/// <summary>向测试视口发送真实窗口消息，验证双击经过行为和命令后居中，不操作桌面鼠标。</summary>
internal static class ViewerDoubleClickChecks
{
    /// <summary>只向本测试创建的窗口发送输入消息。</summary>
    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern IntPtr SendMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam);

    /// <summary>检查左键双击居中、方向保留、其它按键过滤和移除行为后的订阅清理。</summary>
    public static void Run(RobotSceneService scene, RenderControl render)
    {
        var calls = 0;
        var trigger = new ViewerDoubleClickTrigger();
        trigger.Actions.Add(new InvokeCommandAction { Command = new RelayCommand(() => { scene.ZoomAll(); calls++; }) });
        var triggers = Interaction.GetTriggers(render);
        triggers.Add(trigger);
        try
        {
            var pending = new Queue<DependencyObject>();
            pending.Enqueue(render);
            HwndHost? host = null;
            while (pending.TryDequeue(out var node))
            {
                if (node is HwndHost native) { host = native; break; }
                for (var index = 0; index < VisualTreeHelper.GetChildrenCount(node); index++)
                    pending.Enqueue(VisualTreeHelper.GetChild(node, index));
            }
            Check(host is not null && host.Handle != IntPtr.Zero, "测试视口没有原生窗口。");
            var handle = host!.Handle;
            var point = new IntPtr((render.Viewer.GetHeight() / 2 << 16) | render.Viewer.GetWidth() / 2);
            scene.SetView(RobotView.Isometric);
            // 原生标准视角动画约 300ms，结束后再记录双击应恢复的比例。
            Wait(600);
            var camera = render.Camera;
            var zoom = camera.GetOrthoZoomFactor();
            using var centeredTarget = camera.GetTarget();
            using var context = render.Viewer.GetContext();
            context.Pan(160, 80);
            camera.SetOrthoZoomFactor(zoom / 2);
            camera.UpdateProjectionMatrix();
            using var eye = camera.GetPosition();
            using var target = camera.GetTarget();
            var distance = camera.GetDistance();

            SendMessage(handle, 0x0201, new IntPtr(1), point); // 左键单击。
            SendMessage(handle, 0x0202, IntPtr.Zero, point);
            SendMessage(handle, 0x0206, new IntPtr(2), point); // 右键双击。
            SendMessage(handle, 0x0205, IntPtr.Zero, point);
            Drain();
            Check(calls == 0, "单击或右键双击错误触发了居中。");

            SendMessage(handle, 0x0203, new IntPtr(1), point);
            SendMessage(handle, 0x0202, IntPtr.Zero, point);
            Drain();
            Check(calls == 1, "左键双击没有且仅有一次执行居中命令。");
            Check(Math.Abs(camera.GetOrthoZoomFactor() / zoom - 1) < 1e-5, "双击没有恢复适合窗口的比例。");
            using var actualEye = camera.GetPosition();
            using var actualTarget = camera.GetTarget();
            var actualDistance = camera.GetDistance();
            Check(Math.Abs((actualEye.x - actualTarget.x) / actualDistance - (eye.x - target.x) / distance) < 1e-6
                && Math.Abs((actualEye.y - actualTarget.y) / actualDistance - (eye.y - target.y) / distance) < 1e-6
                && Math.Abs((actualEye.z - actualTarget.z) / actualDistance - (eye.z - target.z) / distance) < 1e-6,
                "双击居中改变了当前观察方向。");
            Check(Math.Abs(actualTarget.x - centeredTarget.x) < .01
                && Math.Abs(actualTarget.y - centeredTarget.y) < .01
                && Math.Abs(actualTarget.z - centeredTarget.z) < .01, "双击后观察点没有返回模型中心。");
            using var center = new Vector3(centeredTarget.x, centeredTarget.y, centeredTarget.z);
            using var screen = render.Viewer.WorldPointToSceenPoint(center);
            Check(Math.Abs(screen.x - render.Viewer.GetWidth() / 2.0) < 1
                && Math.Abs(screen.y - render.Viewer.GetHeight() / 2.0) < 1, "模型没有恢复到视口中心。");

            // 双击已经排队但视口行为被移除时，不允许继续访问旧场景。
            SendMessage(handle, 0x0203, new IntPtr(1), point);
            triggers.Remove(trigger);
            Drain();
            SendMessage(handle, 0x0203, new IntPtr(1), point);
            SendMessage(handle, 0x0202, IntPtr.Zero, point);
            Drain();
            Check(calls == 1, "移除行为后仍执行了旧的居中命令。");
            Console.WriteLine("PASS: native left double-click centers scene through command, preserves direction and detaches safely");
        }
        finally { triggers.Remove(trigger); }
    }

    /// <summary>等待原生相机动画和渲染帧继续运行。</summary>
    private static void Wait(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var stop = new DispatcherTimer(DispatcherPriority.ApplicationIdle) { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        stop.Tick += (_, _) =>
        {
            stop.Stop();
            frame.Continue = false;
        };
        try
        {
            stop.Start();
            Dispatcher.PushFrame(frame);
        }
        finally { stop.Stop(); }
    }

    /// <summary>处理双击排入的命令及其绘制请求。</summary>
    private static void Drain()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, () => frame.Continue = false);
        Dispatcher.PushFrame(frame);
    }

    /// <summary>把手势或相机的回归失败作为明确错误报告。</summary>
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
