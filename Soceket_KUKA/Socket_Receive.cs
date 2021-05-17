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
namespace Soceket_KUKA
{
   public  class Socket_Receive
    {
        public  Socket_Receive()
        {

        }

        private static string  _Rece_Message="准备接收";
        /// <summary>
        /// 接受消息属性
        /// </summary>
        public static  string  Rece_Message
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



        public static  void Socke_ReceiveMessage(IAsyncResult ar)
        {


            try
            {

            var Message = Socket_Connect.Global_Socket.EndReceive(ar);


            var Message_Show = Encoding.ASCII.GetString(Socket_Connect.byte_Receive, 0, Message);
                Rece_Message = Message_Show;
            MessageBox.Show(Message_Show);



                //递归调用
                Socket_Connect.Global_Socket.BeginReceive(Socket_Connect.byte_Receive, 0, Socket_Connect.byte_Receive.Length, SocketFlags.None, new AsyncCallback(Socket_Receive.Socke_ReceiveMessage), Socket_Connect.Global_Socket);

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }


        }





    }
}
