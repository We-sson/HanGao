using System.Windows;
using RobotSimulator.Models;
using RobotSimulator.Services;
using IOPath = System.IO.Path;

namespace RobotSimulator;

/// <summary>应用入口：发现型号配置并创建主窗口。</summary>
public partial class App : System.Windows.Application
{
    /// <summary>WPF 启动事件；发现内置型号并显示窗口。</summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            AppLog.Write($"应用启动。程序目录：{AppContext.BaseDirectory}；启动工作目录：{Environment.CurrentDirectory}");

            // 机器人参数只读内嵌资源；Tool 和工装各自生成可编辑配置。
            var robotModels = RobotConfigurationStore.Discover();
            var configurationWarnings = new List<string>();
            var toolModels = ToolConfigurationStore.Discover(IOPath.Combine(AppContext.BaseDirectory, "Tools"), configurationWarnings.Add);
            var workModels = WorkConfigurationStore.Discover(IOPath.Combine(AppContext.BaseDirectory, "Work"), configurationWarnings.Add);

            AppLog.Write($"发现 {robotModels.Count} 个机器人型号：{string.Join("、", robotModels.Select(model => model.Id))}");
            var window = new MainWindow(robotModels, toolModels, workModels);
            MainWindow = window;
            window.Show();
            if (configurationWarnings.Count > 0)
            {
                foreach (var warning in configurationWarnings) AppLog.Write(warning);
                System.Windows.MessageBox.Show(window,
                    $"以下 Tool/工装配置未加载，已有文件保持原样。请修复配置后重新启动；其余型号可继续使用。\n\n{string.Join("\n\n", configurationWarnings)}",
                    "部分附件配置未加载", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception exception)
        {
            AppLog.Write($"应用启动失败：{exception}");
            System.Windows.MessageBox.Show(exception.ToString(), "应用启动失败", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    /// <summary>WPF 退出事件。</summary>
    private void Application_Exit(object sender, ExitEventArgs e)
    {
        MacadViewportBootstrap.Shutdown();
        AppLog.Write("应用退出。");
    }
}
