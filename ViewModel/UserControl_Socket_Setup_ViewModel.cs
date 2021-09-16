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
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.Model.Socket_Setup_Models;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ViewModelBase
    {



        public UserControl_Socket_Setup_ViewModel()
        {
         
    







            //注册消息接收



            //连接按钮屏蔽方法
            Messenger.Default.Register<bool>(this, "Connect_Client_Button_IsEnabled", (_Bool)=> 
            {
                Socket_Client_Setup.Connect_Button_IsEnabled = _Bool;
            });



            //连接控制柜，网络连接状态显示方法
            Messenger.Default.Register<int>(this, "Connect_Client_Socketing_Button_Show", (_int)=> 
            {
                Socket_Client_Setup.Client_Button_Show(_int);
            });

  


            //通讯延时绑定
            Messenger.Default.Register<string>(this, "Connter_Time_Delay_Method",(_String)=> { Connter_Time_Delay = _String; });



            //客户端连接数量
            Messenger.Default.Register<int>(this, "ClientCount", (_int => { ClientCount = _int;  }));


            //显示
            Messenger.Default.Register<Visibility>(this, "Socket_Countion_Show", (_Vis => { Socket_Countion_Show = _Vis;  }));

         

        }


        public Socket_Setup_Models Socket_Client_Setup { set; get; } = new Socket_Setup_Models() { Connect_Socket_Type = Socket_Type.Client, Control_Name_String = "连接控制柜", Text_Error = new IP_Text_Error() { User_IP = "192.168.153.130", User_Port = "7000" } };


        public Socket_Setup_Models Socket_Server_Setup { set; get; } = new Socket_Setup_Models() { Connect_Socket_Type = Socket_Type.Server, Control_Name_String = "监听控制柜", Text_Error = new IP_Text_Error() { User_IP = "192.168.153.1", User_Port = "5000" } };





        //private string _Socket_Message = "准备接收....";
        ///// <summary>
        ///// 接收消息属性
        ///// </summary>
        //public string Socket_Message
        //{
        //    get
        //    {
        //        return _Socket_Message;
        //        //return _Socket_Message;
        //    }
        //    set
        //    {
        //        _Socket_Message = value;
        //    }
        //}

        /// <summary>
        /// 通讯延时显示
        /// </summary>
        public string Connter_Time_Delay { set; get; } = "0";


        public int ClientCount { set; get; } = 0;

        public Visibility Socket_Countion_Show { set; get; } = Visibility.Hidden;
   








        ////接收到信息显示到前端界面方法
        //public void Socket_Message_Show(string Message)
        //{

        //    Socket_Message = Message;

        //}
































    }
}
