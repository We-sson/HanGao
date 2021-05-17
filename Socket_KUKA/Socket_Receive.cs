using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using 悍高软件.ViewModel;

namespace Soceket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public  class Socket_Receive : ViewModelBase
    {
        public  Socket_Receive()
        {

        }

        private  string  _Rece_Message="准备接收";
        /// <summary>
        /// 接受消息属性
        /// </summary>
        public   string  Rece_Message
        {
            get
            {
                return _Rece_Message;
            }
            set
            {

                _Rece_Message = value;
            }
        }
























        //异步接收消息方法
        public static  void Socke_ReceiveMessage(IAsyncResult ar)
        {


            try
            {

                if (Socket_Connect.Global_Socket == null) { return; }
            
               
                //接收到得信息解码显示
            string Message_Show = Encoding.ASCII.GetString(Socket_Connect.byte_Receive, 0, Socket_Connect.Global_Socket.EndReceive(ar));


                //接收到的消息显示出来
                Messenger.Default.Send<string>(Message_Show, "Socket_Message_Show");

                //UserControl_Right_Socket_Connection_ViewModel. Socket_Message = Message_Show;



            MessageBox.Show(Message_Show);



                //递归调用
                Socket_Connect.Global_Socket.BeginReceive(Socket_Connect.byte_Receive, 0, Socket_Connect.byte_Receive.Length, SocketFlags.None, new AsyncCallback(Socket_Receive.Socke_ReceiveMessage), Socket_Connect.Global_Socket);

            }
            catch (Exception e)
            {
                //Messenger.Default.Send<bool?>(true, "Connect_Button_IsEnabled_Method");

                MessageBox.Show(e.Message);
            }


        }





    }
}
