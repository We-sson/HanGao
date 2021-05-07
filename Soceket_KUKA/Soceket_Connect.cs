using PropertyChanged;
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
    [AddINotifyPropertyChangedInterface]
    public class Socket_Connect
    {


        public Socket_Connect()
        {
            //初始化

           
        }

        public Socket_Models Socket_Client
        {
            get
            {
                return Socket_Client;
            }
            set
            {
                Socket_Client = value;
            }
        }








        public bool Socket_Client_KUKA(string _Ip ,int _Port)
        {


        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_Ip), _Port);
        Socket Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Thread _Start_Client = new Thread(Start_Client);
            _Start_Client.IsBackground = true;
            _Start_Client.Start(Client);


            try
            {
                //Client.BeginConnect(ip, new AsyncCallback(Connect_ok), Client);
                Client.Connect(ip);
            }
            catch (Exception)
            {
                MessageBox.Show("连接失败!");
                throw;
            }


            //Socket_Client.IP = "127.0.0.1:7000";
            return true;


        }


       private void Start_Client(object S)
        {
            
        }



  












    }

   
 


  
}
