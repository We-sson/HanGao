using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using RobotSimulator.Controls;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var model = new TestModel
        {
            Options = new RobotViewerOptions
            {
                Robot = RobotModelKind.Kr20R1810_2,
                Tool = ToolKind.Default,
                Work = WorkKind.RSideWelding,
            }
        };
        var viewer = new RobotViewer { Model = model };
        var window = new Window { Content = viewer, Width = 1000, Height = 700, Title = "Control test" };
        window.Loaded += async (_, _) =>
        {
            try
            {
                var watch = Stopwatch.StartNew();
                while (model.StatusMessage != string.Empty && watch.Elapsed.TotalSeconds < 60)
                    await Task.Delay(500);
                Console.WriteLine($"status='{model.StatusMessage}' elapsed={watch.ElapsedMilliseconds} ms");
                if (model.StatusMessage != string.Empty) { app.Shutdown(1); return; }

                model.Joints = [10, -20, 30, 40, 50, 60];
                await Task.Delay(800);
                model.Joints = [20, -10, 20, 30, 40, 50];
                await Task.Delay(800);
                Console.WriteLine("joints updated OK");
                app.Shutdown(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                app.Shutdown(1);
            }
        };
        app.Run(window);
        return 0;
    }
}

internal sealed class TestModel : INotifyPropertyChanged, IRobotViewerModel
{
    private IReadOnlyList<double>? _joints;
    private string _statusMessage = "正在加载三维模型…";

    public event PropertyChangedEventHandler? PropertyChanged;

    public RobotViewerOptions? Options { get; init; }

    public IReadOnlyList<double>? Joints
    {
        get => _joints;
        set
        {
            _joints = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Joints)));
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
        }
    }
}
