using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HanGao.Model
{

    [AddINotifyPropertyChangedInterface]
  public   class List_Show_Models
    {

        /// <summary>
        /// 显示消息弹窗控件内容
        /// </summary
        public UserControl Message_Control { set; get; }

        /// <summary>
        /// 弹窗是否显示
        /// </summary>
        public Visibility List_Show_Bool { set; get; } = Visibility.Collapsed;

        /// <summary>
        /// 水槽型号名称
        /// </summary>
        public string List_Show_Name { set; get; } = "";

        /// <summary>
        /// 工作区好
        /// </summary>
        public string List_Chick_NO { set; get; } = "0";


        /// <summary>
        /// 用户选择的结果
        /// </summary>
        public string User_Check { set; get; } = "";

        //存放需要更换UI参数内容
        public Sink_Models Model { set; get; } = new Sink_Models( Sink_Models.Photo_Sink_enum.左右单盆);
    }
}
