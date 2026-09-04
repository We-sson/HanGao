using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端 Socket 接收报文监视弹层，供现场排查机器人上报内容。
    /// </summary>
    public partial class Client_Receive_Monitor_View : UserControl
    {
        /// <summary>
        /// 装载只读接收报文视图。
        /// </summary>
        public Client_Receive_Monitor_View()
        {
            InitializeComponent();
        }
    }
}
