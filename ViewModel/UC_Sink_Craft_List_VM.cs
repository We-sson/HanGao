using GalaSoft.MvvmLight;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Sink_Craft_List_VM : ViewModelBase
    {

        public UC_Sink_Craft_List_VM()
        {

            //Sink_Craft = new ObservableCollection<UserControl>
            //{
            //    new UC_Sink_Short_Side(){},
            //    new  UC_Sink_Craft(){},        
            //    new UC_Sink_Short_Side(){},
            //    new  UC_Sink_Craft(){}
            //};

        }




        public static   ObservableCollection<UserControl> _Sink_Craft = new ObservableCollection<UserControl>    {
                new UC_Sink_Short_Side(){},
                new  UC_Sink_Craft(){},
                new UC_Sink_Short_Side(){},
                new  UC_Sink_Craft(){},
                            new UC_Sink_Short_Side(){},
                new  UC_Sink_Craft(){},
                new UC_Sink_Short_Side(){},
                new  UC_Sink_Craft(){}
            };
        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static   ObservableCollection<UserControl> Sink_Craft
        {
            get { return _Sink_Craft; }
            set { 
                _Sink_Craft = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Sink_Craft)));
            }
        }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    }
}
