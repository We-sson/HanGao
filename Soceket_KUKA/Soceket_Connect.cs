﻿using PropertyChanged;
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

namespace Soceket_Connect
{
    //[AddINotifyPropertyChangedInterface]
    public class Socket_Connect
    {


        public Socket_Connect()
        {
            //初始化

           
        }

        private static bool _Socket_OK;
        /// <summary>
        /// 连接成功状态
        /// </summary>
        public static bool Socket_OK 
        {
            get
            {
                return _Socket_OK = false;
            }
            set
            {
                _Socket_OK = value;
            }
        }




        private Socket_Models _Socket_Client;
        /// <summary>
        /// Socket连接ip和端口属性
        /// </summary>
        public Socket_Models Socket_Client
        {
            get
            {
                return _Socket_Client;
            }
            set
            {
                _Socket_Client = value;
            }
        }

        private static Socket _Global_Socket;
        /// <summary>
        /// Socket唯一连接标识
        /// </summary>
        public static Socket Global_Socket
        {
            get
            {
                return _Global_Socket;
            }
            set
            {
                _Global_Socket = value;
            }
        }








        /// <summary>
        /// Socket连接方法
        /// </summary>
        public bool Socket_Client_KUKA(string _Ip ,int _Port)
        {

        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_Ip), _Port);
         Global_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


           


            Thread _Start_Client = new Thread(() => Start_Client(ip)) {  IsBackground=true };
            _Start_Client.Start();

            return true;


        }






        /// <summary>
        /// 多线程连接
        /// </summary>
       private void Start_Client(IPEndPoint IP)
        {
            try
            {
                //异步连接
                Global_Socket.BeginConnect(IP, new AsyncCallback(Client_Inf), Global_Socket);
            //S.Connect(IP);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                
            }



        }


        

           public  static byte[] byte_Receive = new byte[1024 * 1024 * 2];

        /// <summary>
        /// 异步连接回调命令
        /// </summary>
        /// <param name="ar"></param>
        public  void Client_Inf( IAsyncResult ar)
        {


            Socket Client = (Socket)ar.AsyncState;
            ar.AsyncWaitHandle.WaitOne(1000);



            //挂起异步连接
            try
            {
                Global_Socket.EndConnect(ar);
            //MessageBox.Show(Client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }


            //连接成功后，前台禁止连接
            Socket_OK = true;




            //开始异步接收消息
            Global_Socket.BeginReceive(byte_Receive, 0, byte_Receive.Length,SocketFlags.None, new AsyncCallback(Socket_Receive.Socke_ReceiveMessage), Global_Socket);
            //Client.BeginReceive();



        }

  












    }

   
 


  
}