using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot_Info_Mes
{
    /// <summary>
    /// 采集端主窗口；具体交互由 XAML 绑定到 <c>Robot_Info_VM</c>，此处只负责装载视图。
    /// </summary>
    public partial class Client_Window : Window
    {
        /// <summary>
        /// 初始化客户端页面及其资源、绑定和行为触发器。
        /// </summary>
        public Client_Window()
        {
            InitializeComponent();
        }
    }
}
