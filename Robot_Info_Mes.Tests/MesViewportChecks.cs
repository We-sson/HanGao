using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Macad.Interaction;
using Robot_Info_Mes;
using Robot_Info_Mes.Model;
using Robot_Info_Mes.ViewModel;
using RobotSimulator.Controls;
using RobotSimulator.Models;
using RobotSimulator.ViewModels;

/// <summary>用真实采集端 XAML、Macad 视口和本机回环验证模型、动作与节拍。</summary>
internal static class MesViewportChecks
{
    public static void Run()
    {
        var dispatcher = Dispatcher.CurrentDispatcher;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
        var frame = new DispatcherFrame();
        var task = RunAsync();
        _ = task.ContinueWith(_ => dispatcher.BeginInvoke(() => frame.Continue = false), TaskScheduler.Default);
        Dispatcher.PushFrame(frame);
        task.GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        foreach (var name in new[] { "Global_Color", "Window_UserConect", "UserControl_Style", "LiveCharts2_Style" })
        {
            app.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"/Robot_Info_Mes;component/View/{name}.xaml", UriKind.Relative)
            });
        }

        var config = new File_Int_Model();
        config.Mes_Digital_Simulation.Robot_Model = RobotModelKind.KR_10_R1440_2;
        config.Mes_Digital_Simulation.Work_Model = WorkKind.DoubleSpotWelding;
        config.Mes_Digital_Simulation.Orbit_Speed = 0;
        var vm = new Robot_Info_VM(config);
        app.Resources["ViewModelLocator"] = new OfflineLocator(vm);

        var window = new Client_Window
        {
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowState = WindowState.Normal,
            ShowInTaskbar = false,
            Width = 1440,
            Height = 1000,
            Left = -4000,
            Top = -2000
        };
        app.MainWindow = window;
        window.Show();
        try
        {
            var tab = Descendants(window).OfType<TabControl>()
                .Single(control => control.Items.OfType<TabItem>().Any(item => Equals(item.Header, "三维仿真")));
            var viewer = (RobotViewer)window.FindName("SimulationViewer");
            // 第一次打开三维后立即切走，覆盖视口还未加入可视树的时机。
            var interrupted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            void LeaveBeforeViewportAttach(object sender, RoutedEventArgs e)
            {
                viewer.Loaded -= LeaveBeforeViewportAttach;
                window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    tab.SelectedIndex = 0;
                    interrupted.TrySetResult();
                }));
            }
            viewer.Loaded += LeaveBeforeViewportAttach;
            tab.SelectedIndex = 1;
            await interrupted.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await window.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
            Check(!viewer.IsLoaded, "快速切回当月数据后，三维控件尚未离开可视树。");
            tab.SelectedIndex = 1;
            await WaitAsync(() => viewer.ZoomAllCommand.CanExecute(null),
                () => $"MES 三维模型未加载：{vm.Digital_Simulation?.StatusMessage}，控件可见 {viewer.IsVisible}，绑定 {viewer.Model is not null}。");
            await WaitAsync(() => InteractiveContext.Current?.ViewportController?.V3dView?.Window()?.IsMapped() == true,
                () => "MES 三维视口的原生窗口未显示。");
            var view = InteractiveContext.Current!.ViewportController!.V3dView!;
            var output = Path.Combine(AppContext.BaseDirectory, "mes-smoke");
            Directory.CreateDirectory(output);
            var homeImage = Path.Combine(output, "mes-robot-home.png");
            view.Redraw();
            Check(view.Dump(homeImage) && new FileInfo(homeImage).Length > 10_000, "MES 三维视口没有渲染模型。");

            await MesRobotCommunicationChecks.RunAsync(vm, async (angles, cycle) =>
            {
                var joints = (IReadOnlyList<JointViewModel>)typeof(RobotViewer)
                    .GetField("_joints", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(viewer)!;
                await WaitAsync(() => joints.Select((joint, index) => Math.Abs(joint.Angle - angles[index]) < 0.1).All(matches => matches),
                    () => $"MES 三维模型没有到达通信角度，当前角度：{string.Join(", ", joints.Select(joint => joint.Angle))}。");
                var expected = cycle.ToString("F0", CultureInfo.CurrentCulture);
                await WaitAsync(() => Descendants(window).OfType<TextBlock>().Any(text =>
                    text.GetBindingExpression(TextBlock.TextProperty)?.ParentBinding.Path?.Path
                        == "Mes_Robot_Info_Model_Data.Robot_Work_AB_Cycle.Timer_Sec" && text.Text == expected),
                    () => $"MES 节拍显示未更新到 {expected} 秒。");
            });

            var movedImage = Path.Combine(output, "mes-robot-moved.png");
            view.Redraw();
            Check(view.Dump(movedImage) && !File.ReadAllBytes(homeImage).AsSpan().SequenceEqual(File.ReadAllBytes(movedImage)),
                "MES 视口的运动截图与初始画面相同。");
            tab.SelectedIndex = 0;
            await WaitAsync(() => !viewer.IsVisible, () => "切回当月数据后三维视口仍显示。");
            tab.SelectedIndex = 1;
            await WaitAsync(() => viewer.IsVisible && InteractiveContext.Current?.ViewportController?.V3dView?.Window()?.IsMapped() == true,
                () => "重新打开三维标签后模型窗口未恢复。");
            Console.WriteLine($"PASS: MES client rendered model, moved on TCP input, displayed cycle and restored its viewport: {output}");
            await MesClientEdgeChecks.RunAsync(vm, viewer);
            await MesBoardCommunicationChecks.RunAsync();
            await SocketLifecycleChecks.RunAsync();
        }
        finally
        {
            window.DataContext = null;
            window.Close();
        }
    }

    private static async Task WaitAsync(Func<bool> condition, Func<string> failure)
    {
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (!condition() && DateTime.UtcNow < deadline) await Task.Delay(50);
        Check(condition(), failure());
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            yield return child;
            foreach (var nested in Descendants(child)) yield return nested;
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    public sealed record OfflineLocator(Robot_Info_VM Robot_Info_VM);
}
