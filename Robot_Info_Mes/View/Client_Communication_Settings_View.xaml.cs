using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端通信参数弹层；字段通过继承来的 DataContext 直接绑定全局运行配置。
    /// </summary>
    public partial class Client_Communication_Settings_View : UserControl
    {
        /// <summary>
        /// 装载通信参数表单，业务提交由绑定命令处理，代码隐藏层不保存额外状态。
        /// </summary>
        public Client_Communication_Settings_View()
        {
            InitializeComponent();
        }
    }
}
