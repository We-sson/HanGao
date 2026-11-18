using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Macad.Interaction;
using Macad.Interaction.Panels;
using RobotSimulator;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

/// <summary>
/// 真实主窗口检查：主窗口第一次 Show 之后视口面板才挂回，就绪命令必须仍然触发、模型必须真的载入视口，
/// 且视口浮层要拿到主窗口的 ViewModel（ViewportPanel 会用自己的 ViewportPanelModel 覆盖 DataContext）。
/// </summary>
internal static class MainWindowChecks
{
    public static async Task RunAsync()
    {
        EnsureApplicationResources();
        var warnings = new List<string>();
        var window = new MainWindow(
            RobotConfigurationStore.Discover(),
            ToolConfigurationStore.Discover(Path.Combine(AppContext.BaseDirectory, "Tools"), warnings.Add),
            WorkConfigurationStore.Discover(Path.Combine(AppContext.BaseDirectory, "Work"), warnings.Add))
        {
            Left = -4000,
            Top = -2000,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
        };
        // 真实启动时 App 先赋值 MainWindow 再 Show，视口宿主用 MainWindow 计算初次位置，这里保持同样的顺序。
        Application.Current.MainWindow = window;
        try
        {
            window.Show();
            var viewModel = (MainViewModel)window.DataContext;
            var renderView = (ViewportPanel)window.FindName("RenderView");
            Check(renderView.OverlayContent is FrameworkElement overlay && ReferenceEquals(overlay.DataContext, viewModel),
                "视口浮层没有接到主窗口的 ViewModel，加载进度和就绪状态会失效。");
            await WaitAsync(() => viewModel.IsLoaded, "主窗口视口没有完成首次模型载入。", TimeSpan.FromSeconds(60));

            var view = InteractiveContext.Current?.ViewportController?.V3dView
                ?? throw new InvalidOperationException("Macad 视口未就绪，无法截图。");
            await Task.Delay(500);
            view.Redraw();
            var outputDirectory = Path.Combine(AppContext.BaseDirectory, "mainwindow-smoke");
            Directory.CreateDirectory(outputDirectory);
            var dumpPath = Path.Combine(outputDirectory, "mainwindow-viewport.png");
            view.Dump(dumpPath);
            Check(File.Exists(dumpPath) && new FileInfo(dumpPath).Length > 10_000, "主窗口视口截图没有生成。");
            var (bodyPixels, edgePixels) = CountRobotPixels(dumpPath);
            // 默认机器人是 ABB IRB2600，机身为 215 灰白，不能沿用 KR10 的橙色判据。
            Check(bodyPixels >= 2000, $"主窗口视口里没有渲染出机器人白色机身（{bodyPixels} 像素）。");
            Check(edgePixels >= 2000, $"主窗口视口里没有渲染出模型边线（{edgePixels} 像素）。");
            Console.WriteLine($"PASS: main window loads robot and tool into Macad viewport ({viewModel.SelectedRobot?.Id}, {bodyPixels} body px, {edgePixels} edge px): {dumpPath}");
        }
        finally
        {
            window.Close();
            await Dispatcher.CurrentDispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
        }
    }

    /// <summary>主窗口直接引用 App.xaml 的资源和默认样式，测试宿主没有 App.xaml，只能先补齐这些键。</summary>
    static void EnsureApplicationResources()
    {
        var resources = Application.Current.Resources;
        resources["BooleanToVisibilityConverter"] = new BooleanToVisibilityConverter();
        resources["PanelBrush"] = new SolidColorBrush(Color.FromRgb(0x17, 0x1A, 0x20));
        resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0x22, 0x26, 0x2E));
        resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF4, 0x7B, 0x20));
        resources["MutedBrush"] = new SolidColorBrush(Color.FromRgb(0x9A, 0xA3, 0xB2));
    }

    static async Task WaitAsync(Func<bool> condition, string message, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (condition()) return;
            await Task.Delay(50);
        }
        throw new InvalidOperationException(message);
    }

    /// <summary>机器人本体是无彩色的浅灰，边线是深色；背景是蓝灰渐变，用这两类像素确认画面不是空的。</summary>
    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static (int Body, int Edge) CountRobotPixels(string path)
    {
        var decoder = new PngBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        var frame = decoder.Frames[0];
        var converted = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0);
        var stride = converted.PixelWidth * 4;
        var pixels = new byte[stride * converted.PixelHeight];
        converted.CopyPixels(pixels, stride, 0);
        var body = 0;
        var edge = 0;
        for (var offset = 0; offset < pixels.Length; offset += 4)
        {
            var blue = pixels[offset];
            var green = pixels[offset + 1];
            var red = pixels[offset + 2];
            var max = Math.Max(red, Math.Max(green, blue));
            var min = Math.Min(red, Math.Min(green, blue));
            if (min >= 195 && max - min <= 12) body++;
            else if (max < 80) edge++;
        }
        return (body, edge);
    }
}
