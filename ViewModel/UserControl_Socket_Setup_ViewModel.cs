using Microsoft.Toolkit.Mvvm.Messaging;

using PropertyChanged;
using Soceket_Connect;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using HanGao.Errorinfo;
using HanGao.Model;
using HanGao.Socket_KUKA;
using HanGao.View.User_Control;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static HanGao.Model.Socket_Setup_Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ObservableRecipient
    {



        public UserControl_Socket_Setup_ViewModel()
        {


            ////设置初始ip和端口
            //IP_Client = "192.168.153.130";
            //Port_Client = "7000";
            //IP_Sever = "192.168.153.1";
            //Port_Sever = "5000";



            WeakReferenceMessenger.Default.Register<Socket_Setup_Models,string>(this, nameof(Meg_Value_Eunm.Client_Initialization) , (O,_S) =>
            {
                Socket_Client_Setup = _S;
            });

            WeakReferenceMessenger.Default.Register<Socket_Setup_Models,string >(this, nameof(Meg_Value_Eunm.Sever_Initialization) , (O,_S) =>
            {
                Socket_Server_Setup = _S;
            });


            //注册消息接收



            //连接按钮屏蔽方法
            WeakReferenceMessenger.Default.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Connect_Client_Button_IsEnabled) , (O,_Bool) =>
            {
                Socket_Client_Setup.Connect_Button_IsEnabled = _Bool;
            });



            //连接控制柜，网络连接状态显示方法
            WeakReferenceMessenger.Default.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Connect_Client_Socketing_Button_Show) , (O,_int) =>
            {
                Socket_Client_Setup.Client_Button_Show(_int);
            });




            //通讯延时绑定
            WeakReferenceMessenger.Default.Register<string,string >(this, nameof(Meg_Value_Eunm.Connter_Time_Delay_Method) , (O,_String) => { Connter_Time_Delay = _String; });



            //客户端连接数量
            WeakReferenceMessenger.Default.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.ClientCount) , (O,_int )=> { ClientCount = _int; });


            //显示
            WeakReferenceMessenger.Default.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Socket_Countion_Show) , (O,_Visibility )=> { Socket_Countion_Show = _Visibility; });



        }



        /// <summary>
        /// 客户端IP
        /// </summary>
        private static  string _IP_Client = "192.168.153.131";

        public static string IP_Client
        {
            get { return _IP_Client; }
            set { _IP_Client = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(IP_Client))); }
        }



        /// <summary>
        /// 客户端端口
        /// </summary>
        private static string _Port_Client = "7000";

        public static string Port_Client
        {
            get { return _Port_Client; }
            set { _Port_Client = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Port_Client))); }
        }



        /// <summary>
        /// 服务器IP
        /// </summary>
        private static string _IP_Sever = "192.168.153.1";

        public static string IP_Sever
        {
            get { return _IP_Sever; }
            set { _IP_Sever = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(IP_Sever))); }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        private static string _Port_Sever = "5000";

        public static string Port_Sever
        {
            get { return _Port_Sever; }
            set { _Port_Sever = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Port_Sever))); }
        }
   



        private static Socket_Setup_Models _Socket_Client_Setup = new Socket_Setup_Models()
        {
            IP = IP_Client,
            Port = Port_Client,
            One_Read =  new Socket_Connect(),
            Read = new Socket_Connect(),
            Write = new Socket_Connect(),
            Connect_Socket_Type = Socket_Type.Client,
            Control_Name_String = "连接控制柜",
        };
        /// <summary>
        /// 客户端静态属性
        /// </summary>
        public static Socket_Setup_Models Socket_Client_Setup
        {
            get => _Socket_Client_Setup;
            set
            {

                _Socket_Client_Setup = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Client_Setup)));

            }
        }


        private static Socket_Setup_Models _Socket_Server_Setup = new Socket_Setup_Models() 
        {
            IP = IP_Sever,
            Port = Port_Sever,
            Connect_Socket_Type = Socket_Type.Server,
            Sever = new Socket_Sever(IP_Sever, Port_Sever),
            Control_Name_String = "监听控制柜",
        };
        /// <summary>
        /// 服务器静态属性
        /// </summary>
        public static Socket_Setup_Models Socket_Server_Setup
        {
            get => _Socket_Server_Setup;
            set
            {

                _Socket_Server_Setup = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Server_Setup)));

            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
         public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        /// <summary>
        /// 通讯延时显示
        /// </summary>
        public static  string Connter_Time_Delay { set; get; } = "0";


        /// <summary>
        /// 客户端连接数量
        /// </summary>
        public int ClientCount { set; get; } = 0;


        /// <summary>
        /// 连接按钮显示
        /// </summary>
        public Visibility Socket_Countion_Show { set; get; } = Visibility.Hidden;



    }
}
