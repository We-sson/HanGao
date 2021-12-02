using Microsoft.Toolkit.Mvvm.Messaging;
using Nancy.Helpers;
 
using PropertyChanged;
using Soceket_Connect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Errorinfo;
using HanGao.Model;
using HanGao.Socket_KUKA;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static HanGao.Model.Socket_Setup_Models;
using static HanGao.ViewModel.Home_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Sideber_Show_ViewModel : ObservableRecipient
    {

        public UserControl_Sideber_Show_ViewModel()
        {
            //注册消息接收
            //WeakReferenceMessenger.Default.Register<bool?>(this, "Sidebar_Subtitle_Signal_Method_bool", Sidebar_Subtitle_Signal_Method_bool);




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



            Messenger.Register<Sideber_Models,string >(this,nameof( Meg_Value_Eunm.Sideber_Show), (O,Sm) => { Sideber_Var = Sm; });

        }


        //侧边栏内容
        public Sideber_Models Sideber_Var { set; get; } = Sideber_List[0];






        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand Click_OPen_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            { 
            
            //把参数类型转换控件
            UIElement e = Sm.Source as UIElement;


                Sideber_Var.Sideber_Open = false;
                



            });
        }

        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand Loaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                UIElement e = Sm.Source as UIElement;


               Messenger.Send<Socket_Setup_Models ,string >(new Socket_Setup_Models()
                {
                    Read = new Socket_Connect(IP_Client, Port_Client, Connect_Type.Long, Read_Write_Enum.Read),
                    Write = new Socket_Connect(IP_Client, Port_Client, Connect_Type.Short, Read_Write_Enum.Write),

                    Connect_Socket_Type = Socket_Type.Client,
                    Control_Name_String = "连接控制柜",
                    Text_Error = new IP_Text_Error() { User_IP = IP_Client, User_Port = Port_Client }
                }, nameof (Meg_Value_Eunm.Client_Initialization));


                Messenger.Send<Socket_Setup_Models,string >(new Socket_Setup_Models()
                {
                    Connect_Socket_Type = Socket_Type.Server,
                    Sever = new Socket_Sever(IP_Sever, Port_Sever),
                    Control_Name_String = "监听控制柜",
                    Text_Error = new IP_Text_Error() { User_IP = IP_Sever, User_Port = Port_Sever }
                }, nameof(Meg_Value_Eunm.Sever_Initialization));









            });
        }










    }
}
