using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Sideber_Show_ViewModel : ViewModelBase
    {

        public UserControl_Sideber_Show_ViewModel()
        {
            //注册消息接收
            //Messenger.Default.Register<bool?>(this, "Sidebar_Subtitle_Signal_Method_bool", Sidebar_Subtitle_Signal_Method_bool);




            //if (IsInDesignMode)
            //{
            //    // Code runs in Blend --> create design time data.
            //    Sidebar_MainTitle = "连接状态";


            //    Subtitle_Position = new Thickness() { Top = 40 };
            //}
            //else
            //{
            //    // Code runs "for real"
            //}

            Messenger.Default.Register<Sideber_Models>(this, "Sideber_Show", (Sm)=> { Sideber_Var = Sm; });

        }


        public Sideber_Models Sideber_Var { set; get; } = new Sideber_Models() { Sideber_Open=false };






        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand Click_OPen_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>((Sm)=> 
            { 
            
            //把参数类型转换控件
            UIElement e = Sm.Source as UIElement;


                Sideber_Var.Sideber_Open = false;
                


            //    if (e.Uid == "Open")
            //{
            //    Sideber_Var.Sideber_Open = true;
            //        //侧边栏打开主页面模糊
            //        //Messenger.Default.Send<Double>(0, "Open_Effect");
            //        ////侧边栏打开后主页黑化禁止用户操作
            //        //Messenger.Default.Send<Visibility>(Visibility.Visible, "Home_Visibility_Show");


            //}
            //else if (e.Uid == "Close")
            //{
            //    Sideber_Var.Sideber_Open = false;
            //     Sideber_Var.Sidebar_Control = null;
            //    //Messenger.Default.Send<Double>(0, "Open_Effect");
            //    //Messenger.Default.Send<Visibility>(Visibility.Collapsed, "Home_Visibility_Show");

            //    }
            });
        }












    }
}
