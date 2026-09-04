using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端工艺基准参数弹层，用于编辑节拍、班次时长、产量及稼动率目标。
    /// </summary>
    public partial class Client_Process_Settings_View : UserControl
    {
        /// <summary>
        /// 装载工艺参数表单；数值同步和持久化由绑定及视图模型命令完成。
        /// </summary>
        public Client_Process_Settings_View()
        {
            InitializeComponent();
        }
    }
}
