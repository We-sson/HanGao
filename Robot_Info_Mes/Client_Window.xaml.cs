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
        }
    }
}
