using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using PropertyChanged;
using Soceket_Connect;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using 悍高软件.Errorinfo;
using 悍高软件.Model;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.Model.Socket_Setup_Models;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ViewModelBase
    {



        public UserControl_Socket_Setup_ViewModel()
        {


            //设置初始ip和端口
            IP_Client = "192.168.153.130";
            Port_Client = "7000";
            IP_Sever = "192.168.153.1";
            Port_Sever = "5000";



            Socket_Client_Setup = new Socket_Setup_Models()
            {
                Read = new Socket_Connect(IP_Client, Port_Client, Connect_Type.Long, Read_Write_Enum.Read),
                Write = new Socket_Connect(IP_Client, Port_Client, Connect_Type.Short, Read_Write_Enum.Write),
                Connect_Socket_Type = Socket_Type.Client,
                Control_Name_String = "连接控制柜",
                Text_Error = new IP_Text_Error() { User_IP = IP_Client, User_Port = Port_Client }
            };




            Socket_Server_Setup = new Socket_Setup_Models()
            {
                Connect_Socket_Type = Socket_Type.Server,
                Sever = new Socket_Sever(IP_Sever, Port_Sever),
                Control_Name_String = "监听控制柜",
                Text_Error = new IP_Text_Error() { User_IP = IP_Sever, User_Port = Port_Sever }
            };


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
        public string IP_Client { set; get; }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public string Port_Client { set; get; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string IP_Sever { set; get; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public string Port_Sever { set; get; }



        public static Socket_Setup_Models Socket_Client_Setup { set; get; }


        public static Socket_Setup_Models Socket_Server_Setup { set; get; }







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
