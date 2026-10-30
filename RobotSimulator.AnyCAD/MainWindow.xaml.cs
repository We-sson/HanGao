using System.Windows;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

namespace RobotSimulator;

/// <summary>主窗口只负责组装 ViewModel 和三维服务、显示提示；业务事件通过 XAML 命令触发。</summary>
public partial class MainWindow : Window
{
    /// <summary>接收已发现的型号，为本窗口创建场景服务，并设置所有界面绑定的数据源。</summary>
    public MainWindow(IReadOnlyList<RobotModelDefinition> robotModels, IReadOnlyList<ToolModelDefinition> toolModels)
    {
        InitializeComponent();

        var viewModel = new MainViewModel(robotModels, toolModels, new RobotSceneService(RenderView));
        // 弹窗和真正关闭窗口属于视图职责，ViewModel 只发出请求，不直接依赖窗口控件。
        viewModel.ErrorOccurred += (message, title) =>
            System.Windows.MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        viewModel.CloseRequested += () => Dispatcher.BeginInvoke(Close);
        DataContext = viewModel;
#if DEBUG
        RobotSetupButton.Visibility = Visibility.Visible;
        RobotSetupButton.Click += (_, _) =>
        {
            if (viewModel.IsPlaying) viewModel.PlayCommand.Execute(null);
            new Views.RobotSetupWindow { Owner = this }.ShowDialog();
        };
#endif
    }
}
