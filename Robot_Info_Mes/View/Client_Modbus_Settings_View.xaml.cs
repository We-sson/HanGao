using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View;

/// <summary>
/// Modbus TCP 参数、自动寄存器布局和当前发布值监视弹层。
/// </summary>
public partial class Client_Modbus_Settings_View : UserControl
{
    /// <summary>装载视图；校验、保存和服务重启全部由 Robot_Info_VM 处理。</summary>
    public Client_Modbus_Settings_View()
    {
        InitializeComponent();
    }
}
