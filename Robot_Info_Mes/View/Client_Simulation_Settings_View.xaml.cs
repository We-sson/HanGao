using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 客户端仿真参数弹层，用于编辑机器人组合、相机自转和关节跟随参数。
    /// </summary>
    public partial class Client_Simulation_Settings_View : UserControl
    {
        /// <summary>
        /// 装载仿真参数表单；数值同步和持久化由绑定及视图模型命令完成。
        /// </summary>
        public Client_Simulation_Settings_View()
        {
            InitializeComponent();
        }
    }
}
