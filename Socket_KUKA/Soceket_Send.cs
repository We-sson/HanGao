using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace 悍高软件.Socket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public  class Soceket_Send : ViewModelBase
    {

        public Soceket_Send()
        {

            //注册消息接收
            Messenger.Default.Register<string>(this, "Socket_Send_Message_Method", Socket_Send_Message_Method);


        }

        public static  byte[] byte_Send = new byte[1024 * 1024 * 2];


        public static   void Socket_Send_Message_Method(string Message)
        {


            byte_Send = Encoding.UTF8.GetBytes(Message);


            try
            {
                //发送消息到服务器
            Socket_Connect.Global_Socket.BeginSend(byte_Send, 0, byte_Send.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Socket_Connect.Global_Socket);
            }
            catch (Exception e)
            {

                Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");

                MessageBox.Show(e.Message);

            }


        }



        public static  void Socket_Send_Message(IAsyncResult Socket)
        {


            MessageBox.Show("发送完成！");




        }





    }
}
