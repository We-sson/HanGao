using GalaSoft.MvvmLight;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Sink_Type_VM: ViewModelBase
    {
       public UC_Sink_Type_VM(string  _Sink_Type,string _Sink_Ico)
        {
            Sink_Type = _Sink_Type;
              Sink_Ico = _Sink_Ico;
        }


        /// <summary>
        /// 水槽类型
        /// </summary>
        public string Sink_Type { set; get; } = "左右普通单盆";

        /// <summary>
        /// 水槽图标
        /// </summary>
        public string Sink_Ico { set; get; } = "&#xe61b;";



        /// <summary>
        /// 图标字体位置
        /// </summary>
        public string Font_Family { set; get; } = "/Resources/#iconfont";


        /// <summary>
        /// 控件选择状态
        /// </summary>
        public bool Check_State { set; get; } = false;

    }
}
