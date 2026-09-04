using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端 Socket 发送报文监视弹层，供现场核对回包及看板上报内容。
    /// </summary>
    public partial class Client_Send_Monitor_View : UserControl
    {
        /// <summary>
        /// 装载只读发送报文视图。
        /// </summary>
        public Client_Send_Monitor_View()
        {
            InitializeComponent();
        }
    }
}
