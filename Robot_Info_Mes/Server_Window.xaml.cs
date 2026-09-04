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
    /// 汇总看板主窗口；列表、轮播及通信状态均由 <c>Robot_Info_VM</c> 提供。
    /// </summary>
    public partial class Server_Window : Window
    {
        /// <summary>
        /// 初始化看板页面及每个设备对应的 OEE 视图模板。
        /// </summary>
        public Server_Window()
        {
            InitializeComponent();
        }
    }
}
