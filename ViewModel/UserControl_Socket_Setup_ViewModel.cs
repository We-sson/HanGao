﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

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

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ViewModelBase, INotifyPropertyChanged
    {



        public UserControl_Socket_Setup_ViewModel()
        {


            //设置初始ip和端口
            IP_Client = "192.168.153.130";
            Port_Client = "7000";
            IP_Sever = "192.168.153.1";
            Port_Sever = "5000";



            Messenger.Default.Register<Socket_Setup_Models>(this, "Client_Initialization", (_S) =>
            {
                Socket_Client_Setup = _S;
            });

            Messenger.Default.Register<Socket_Setup_Models>(this, "Sever_Initialization", (_S) =>
            {
                Socket_Server_Setup = _S;
            });


            //注册消息接收



            //连接按钮屏蔽方法
            Messenger.Default.Register<bool>(this, "Connect_Client_Button_IsEnabled", (_Bool) =>
            {
                Socket_Client_Setup.Connect_Button_IsEnabled = _Bool;
            });



            //连接控制柜，网络连接状态显示方法
            Messenger.Default.Register<int>(this, "Connect_Client_Socketing_Button_Show", (_int) =>
            {
                Socket_Client_Setup.Client_Button_Show(_int);
            });




            //通讯延时绑定
            Messenger.Default.Register<string>(this, "Connter_Time_Delay_Method", (_String) => { Connter_Time_Delay = _String; });



            //客户端连接数量
            Messenger.Default.Register<int>(this, "ClientCount", (_int => { ClientCount = _int; }));


            //显示
            Messenger.Default.Register<Visibility>(this, "Socket_Countion_Show", (_Vis => { Socket_Countion_Show = _Vis; }));



        }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public static string IP_Client { set; get; }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public static string Port_Client { set; get; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string IP_Sever { set; get; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string Port_Sever { set; get; }



        private static Socket_Setup_Models _Socket_Client_Setup = new Socket_Setup_Models();
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


        private static Socket_Setup_Models _Socket_Server_Setup = new Socket_Setup_Models();
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
        public string Connter_Time_Delay { set; get; } = "0";


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