using System.Windows;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;
using IOPath = System.IO.Path;

namespace RobotSimulator;

public partial class App : System.Windows.Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            AppLog.Write("应用启动，初始化 AnyCAD GlobalInstance。");
            GlobalInstance.Initialize();
            ModelingEngine.EnableSnapShape();

            var modelsRoot = IOPath.Combine(AppContext.BaseDirectory, "Models");
            var robotModels = RobotConfigurationStore.Discover(modelsRoot);

            AppLog.Write($"发现 {robotModels.Count} 个机器人型号：{string.Join("、", robotModels.Select(model => model.Id))}");
            var window = new MainWindow(robotModels);
            MainWindow = window;
            window.Show();
        }
        catch (Exception exception)
        {
            AppLog.Write($"应用启动失败：{exception}");
            System.Windows.MessageBox.Show(exception.ToString(), "应用启动失败", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        AppLog.Write("应用退出，释放 AnyCAD GlobalInstance。");
        GlobalInstance.Destroy();
    }
}
