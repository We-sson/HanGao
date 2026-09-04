using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 单台设备的 OEE 状态卡；既用于客户端本机，也作为服务器设备列表的项目模板。
    /// </summary>
    public partial class OEE_UI : UserControl
    {
        /// <summary>
        /// 装载设备信息、实时指标、状态计时及趋势图；数据全部由外部 DataContext 提供。
        /// </summary>
        public OEE_UI()
        {
            InitializeComponent();
        }
    }
}
