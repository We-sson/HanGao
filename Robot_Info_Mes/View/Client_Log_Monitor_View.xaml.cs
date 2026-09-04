using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端运行日志弹层；只负责呈现 <c>User_Log_Models</c> 中累积的日志文本。
    /// </summary>
    public partial class Client_Log_Monitor_View : UserControl
    {
        /// <summary>
        /// 装载日志控制台及自动滚动行为。
        /// </summary>
        public Client_Log_Monitor_View()
        {
            InitializeComponent();
        }
    }
}
