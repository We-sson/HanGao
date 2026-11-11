using System.Windows;

namespace Robot_Info_Mes
{
    /// <summary>采集端主窗口；加载、关节推送和关闭释放全部由 XAML 绑定与行为处理。</summary>
    public partial class Client_Window : Window
    {
        /// <summary>初始化客户端页面及其资源、绑定和行为触发器。</summary>
        public Client_Window()
        {
            InitializeComponent();
            Loaded += (_, _) => UpdateSimulationStatusPopup();
            Activated += (_, _) => UpdateSimulationStatusPopup();
            Deactivated += (_, _) => SimulationStatusPopup.IsOpen = false;
            Closed += (_, _) => SimulationStatusPopup.IsOpen = false;
            SimulationViewer.SizeChanged += (_, _) => UpdateSimulationStatusPopup();
        }

        /// <summary>只在主窗口处于前台时显示状态浮层，避免它浮到其他软件上方。</summary>
        private void UpdateSimulationStatusPopup()
        {
            // Popup 以视口左上角为原点；文字整体上移，落在三维画面左下角内部。
            SimulationStatusPopup.VerticalOffset = Math.Max(0, SimulationViewer.ActualHeight - 48);
            SimulationStatusPopup.IsOpen = IsActive && IsVisible && SimulationViewer.IsVisible;
        }
    }
}
