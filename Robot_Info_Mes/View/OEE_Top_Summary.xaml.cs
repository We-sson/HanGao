using System.Windows.Controls;

namespace Robot_Info_Mes.View
{
    /// <summary>单台设备的顶部概览：设备信息、实时生产数据、效率指标与状态累计时间。</summary>
    public partial class OEE_Top_Summary : UserControl
    {
        /// <summary>初始化 XAML 组件；数据全部由外部 DataContext 提供。</summary>
        public OEE_Top_Summary()
        {
            InitializeComponent();
        }
    }
}
