using GalaSoft.MvvmLight;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Pop_Ups_VM:ViewModelBase
    {


        public string Subtitle_Name { set; get; } = "Parameter Setting";


        public string Title_Name { set; get; } = "水槽类型选择";


        public int Number_Pages { set; get; } = 5;

        public int Page { set; get; } = 1;

        //public UserControl Pop_UserControl { set; get; } = new UC_Sink_Size() { };
        public UserControl Pop_UserControl { set; get; } = new UC_Sink_Size() { };

    }
}
