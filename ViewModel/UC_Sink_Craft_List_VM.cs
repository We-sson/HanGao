using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static HanGao.Model.Sink_Models;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Sink_Craft_List_VM : ObservableRecipient
    {

        public UC_Sink_Craft_List_VM()
        {



        }






          public   static     ObservableCollection<Sink_Craft_Models> _Sink_Craft = new ObservableCollection<Sink_Craft_Models>  { 
            new Sink_Craft_Models (){ Sink_Ico= Sink_Type_Enum.LeftRight_One.GetStringValue()  , Sink_Title="水槽围边焊接工艺" , Sink_Subtitle="焊接工艺由机器人记录多个位置姿态,通过重复行走路径激光焊接完成!", },
            new Sink_Craft_Models (){ Sink_Ico="&#xE61B;", Sink_Title="水槽短边焊接工艺" , Sink_Subtitle="焊接工艺由多个位置姿态,连续激光焊接完成!", },
            new Sink_Craft_Models (){ Sink_Ico=Sink_Type_Enum.LeftRight_Two.GetStringValue(), Sink_Title="水槽围边焊接工艺" , Sink_Subtitle="焊接工艺由多个位置姿态,连续激光焊接完成!", },
            new Sink_Craft_Models (){ Sink_Ico="&#xE61B;", Sink_Title="水槽围边焊接工艺" , Sink_Subtitle="焊接工艺由多个位置姿态,连续激光焊接完成!", },



            };
        /// <summary>
        /// 水槽列表集合
        /// </summary>
        public static    ObservableCollection<Sink_Craft_Models> Sink_Craft
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




        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Craft_UI_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                FrameShow.ProgramEdit_Click = true;
                FrameShow.ProgramEdit_UI = true;
                //关闭弹窗
                Messenger.Send<UserControl, string>(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;






            });
        }


    }
}
