using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Errorinfo;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Right_Socket_Connection_ViewModel: ViewModelBase  
    {



        public UserControl_Right_Socket_Connection_ViewModel()
        {

            //注册消息接收

            Messenger.Default.Register<string>(this, "Socket_Message_Show", Socket_Message_Show);

            Messenger.Default.Register<bool>(this, "Connect_Button_IsEnabled_Method", Connect_Button_IsEnabled_Method);

        }






        private bool _Connect_Button_IsEnabled=true;
        /// <summary>
        /// 连接按钮连接后禁止重复连接
        /// </summary>
        public bool Connect_Button_IsEnabled
        {
            get
            {
                return _Connect_Button_IsEnabled;
            }
            set
            {
                _Connect_Button_IsEnabled = value;
            }
        }






        private   string  _Socket_Message="准备接收...." ;
        /// <summary>
        /// 接收消息属性
        /// </summary>
        public  string Socket_Message
        {
            get
            {
                return _Socket_Message;
                //return _Socket_Message;
            }
            set
            {
                _Socket_Message = value;
            }
        }

        private string _User_IP ;
        /// <summary>
        /// 用户输入IP
        /// </summary>
        public string User_IP
        {
            get
            {
                return _User_IP;
                //return _Socket_Message;
            }
            set
            {
                _User_IP = value;
            }
        }




        //接收到信息显示到前端界面方法
        public void Connect_Button_IsEnabled_Method(bool Bool_Try)
        {

            Connect_Button_IsEnabled = Bool_Try;

        }


        //接收到信息显示到前端界面方法
        public void Socket_Message_Show(string Message)
        {

            Socket_Message = Message;

        }






        public ICommand Socket_Send_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Send);
        }
        /// <summary>
        /// Socket发送事件命令
        /// </summary>
        private void Socket_Send(UserControl_Right_Socket_Connection Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;

           //把输入框的消息发送过去
            Messenger.Default.Send<string>(Sm.Socket_Send.Text, "Socket_Send_Message_Method");

            Soceket_Send.Socket_Send_Message_Method(Sm.Socket_Send.Text);

       



        }












        public ICommand Socket_Connection_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Connection);
        }
        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        private void Socket_Connection(UserControl_Right_Socket_Connection Sm)
        {
            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;
            
       Socket_Connect Soceket_KUKA_Client = new Socket_Connect();


            //Socket_Receive.

 
            //创建连接
            Soceket_KUKA_Client.Socket_Client_KUKA(Sm.TB1.Text, int.Parse(Sm.TB2.Text));












        }



        public ICommand Socket_Close_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Clos);
        }
        /// <summary>
        /// Socket关闭事件命令
        /// </summary>
        private void Socket_Clos(UserControl_Right_Socket_Connection Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;

            //把输入框的消息发送过去
            //Messenger.Default.Send<string>(Sm.Socket_Send.Text, "Socket_Send_Message_Method");

            //Soceket_Send.Socket_Send_Message_Method(Sm.Socket_Send.Text);


            Socket_Connect.Socket_Close();

            

        }






    }
}
