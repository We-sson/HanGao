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

            //接收修改参数属性
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.Sink_Craft_List_Value_Load), (O, S) =>
            {

              
                Sink_Craft = S.Sink_Craft;
                Sink = S;


            });





        }


        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models Sink { get; set; }





        public static ObservableCollection<Sink_Craft_Models> _Sink_Craft = new ObservableCollection<Sink_Craft_Models>();
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

                //把参数类型转换控件
                Button E = Sm.Source as Button;
                Sink_Craft_Models S = E.DataContext as Sink_Craft_Models;



                FrameShow.ProgramEdit_Enabled = true;
                FrameShow.ProgramEdit_UI = true;




                //传送工艺枚举属性
                Messenger.Send<Sink_Craft_Models, string>(S, nameof(Meg_Value_Eunm.Program_UI_Load));


                //传送用户选中的水槽属性
                //Messenger.Send<Sink_Models, string>(Sink, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Point_Load));
                

                //关闭弹窗
                Messenger.Send<UserControl, string>(null, nameof(Meg_Value_Eunm.User_Contorl_Message_Show));
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;






            });
        }


    }
}
