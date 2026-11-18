using System.Windows;
using System.Windows.Controls;
using RobotSimulator.Behaviors;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

namespace RobotSimulator;

/// <summary>主窗口只负责组装 ViewModel 和三维服务、显示提示；业务事件通过 XAML 命令触发。</summary>
public partial class MainWindow : Window
{
    /// <summary>接收已发现的型号，为本窗口创建场景服务，并设置所有界面绑定的数据源。</summary>
    public MainWindow(IReadOnlyList<RobotModelDefinition> robotModels, IReadOnlyList<ToolModelDefinition> toolModels,
        IReadOnlyList<WorkModelDefinition> workModels)
    {
        // Macad 视口依赖全局交互上下文，必须在 XAML 创建 ViewportPanel 之前准备好。
        MacadViewportBootstrap.EnsureContext();
        InitializeComponent();
        // ViewportFrame 里只有视口一层容器；面板延后到窗口显示完成再挂回，避免首次 Show 期间建原生视口。
        DeferredViewportHost.Defer(this, (Grid)ViewportFrame.Child, RenderView);

        var scene = new RobotSceneService(RenderView);
        var viewModel = new MainViewModel(robotModels, toolModels, workModels, scene);
        // 弹窗和真正关闭窗口属于视图职责，ViewModel 只发出请求，不直接依赖窗口控件。
        viewModel.ErrorOccurred += (message, title) =>
            System.Windows.MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        viewModel.CloseRequested += () => Dispatcher.BeginInvoke(Close);
        DataContext = viewModel;
        ViewportDoubleClickTrigger.SetCommand(RenderView, viewModel.ZoomAllCommand);
        // ViewportPanel 构造时把 ViewportPanelModel 设为自身 DataContext，XAML 命令和浮层绑定都拿不到本窗口的 ViewModel；
        // 视口就绪改成这里订阅，浮层内容也显式接到同一个 ViewModel，加载进度才能显示。
        if (RenderView.OverlayContent is FrameworkElement overlay) overlay.DataContext = viewModel;
        RenderView.Loaded += (_, _) => viewModel.ViewerReadyCommand.Execute(null);
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
