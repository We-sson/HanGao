
using UserControl = System.Windows.Controls.UserControl;


namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 可复用日志控制台；显示时间戳日志，并通过绑定命令在新增内容后滚动到底部。
    /// </summary>
    public partial class Control_Log : UserControl
    {
        /// <summary>
        /// 装载日志显示模板，控件本身不持有业务状态。
        /// </summary>
        public Control_Log()
        {
            InitializeComponent();
        }
    }
}
