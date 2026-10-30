using System.Windows;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;
using IOPath = System.IO.Path;

namespace RobotSimulator;

/// <summary>应用入口：准备 AnyCAD 引擎和型号配置，创建主窗口，并在退出时释放原生引擎。</summary>
public partial class App : System.Windows.Application
{
    /// <summary>WPF 启动事件；先初始化 CAD 引擎，再发现内置型号并显示窗口。</summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            AppLog.Write($"应用启动，初始化 AnyCAD GlobalInstance。程序目录：{AppContext.BaseDirectory}；启动工作目录：{Environment.CurrentDirectory}");
            // 让 AnyCAD 原生资源中的相对路径从程序目录解析，支持快捷方式和外部启动器。
            System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            GlobalInstance.Initialize();
            ModelingEngine.EnableSnapShape();

            // 机器人参数只读内嵌资源；Tool 和工装各自生成可编辑配置。
            var robotModels = RobotConfigurationStore.Discover();
            var toolModels = ToolConfigurationStore.Discover(IOPath.Combine(AppContext.BaseDirectory, "Tools"));
            var workModels = WorkConfigurationStore.Discover(IOPath.Combine(AppContext.BaseDirectory, "Work"));

            AppLog.Write($"发现 {robotModels.Count} 个机器人型号：{string.Join("、", robotModels.Select(model => model.Id))}");
            var window = new MainWindow(robotModels, toolModels, workModels);
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

    /// <summary>WPF 退出事件；窗口场景释放后，再销毁 AnyCAD 全局原生资源。</summary>
    private void Application_Exit(object sender, ExitEventArgs e)
    {
        AppLog.Write("应用退出，释放 AnyCAD GlobalInstance。");
        GlobalInstance.Destroy();
    }
}
