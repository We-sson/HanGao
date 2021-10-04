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
using System.Threading.Tasks;
using System.Windows;
using HanGao.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Var_Show_ViewModel;

namespace HanGao.Socket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Sever : ViewModelBase
    {
        public Socket_Sever(string _IP,string _Port)
        {

            //初始化
            Address = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));
            //Socket_Server_KUKA();

        }



        #region 字段
        private static int _ClientCount;
        private static bool _IsRuning ;

        #endregion

        #region 属性

        /// <summary>
        /// 服务器启动状态
        /// </summary>
        public static bool IsRuning
        {
            set
            {
                _IsRuning = value;
                if (value)
                {
                    Messenger.Default.Send<Visibility>(Visibility.Visible, "Socket_Countion_Show");

                }
                else
                {

                    Messenger.Default.Send<Visibility>(Visibility.Hidden, "Socket_Countion_Show");
                }


            }
            get
            {
                return _IsRuning;
            }
        }
        /// <summary>
        /// 客户端连接数量
        /// </summary>
        public static int ClientCount
        {
            set
            {
                _ClientCount = value;
                Messenger.Default.Send<int>(value, "ClientCount");
            }
            get
            {
                return _ClientCount;
            }
        }

        public static  IPEndPoint Address { set; get; }

        /// <summary>
        /// 服务器唯一连接标识
        /// </summary>
        public static Socket Socket_Server { set; get; }


        /// <summary>
        /// KUKA客户端列表
        /// </summary>
        public static List<Socket_Models_Server> KUKA_Client_List { set; get; } = new List<Socket_Models_Server>();



        #endregion






        #region 方法


        /// <summary>
        /// 服务器开启连接
        /// </summary>
        /// <param name="_Ip"></param>
        /// <param name="_Port"></param>
        public void Socket_Server_KUKA()
        {
            if (!IsRuning)
            {
                IsRuning = true;
                //Address = new IPEndPoint(IPAddress.Parse(_Ip), _Port);
                Socket_Server = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket_Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Socket_Server.Bind(Address);

                Socket_Server.Listen(10);

                Socket_Server.BeginAccept(new AsyncCallback(KUKA_Client_Connect), Socket_Server);



            }
        }


        /// <summary>
        /// 有客户端连接处理方法
        /// </summary>
        /// <param name="ar"></param>
        public void KUKA_Client_Connect(IAsyncResult ar)
        {
            if (IsRuning)
            {
                Socket _Server = (Socket)ar.AsyncState;
                Socket _KUKA_Client = _Server.EndAccept(ar);


                Socket_Models_Server State = new Socket_Models_Server() { Server_Kuka_Client = _KUKA_Client };
                lock (_KUKA_Client)
                {

                    //接收连接对象后添加到列表中
                    KUKA_Client_List.Add(State);
                    //MessageBox.Show(KUKA_Client_List.Count.ToString());
                    //增加客户端连接时通知前端显示
                    ClientCount++;

                    State.Server_Recv_Byte = new byte[_KUKA_Client.ReceiveBufferSize];

                }
                //接收客户端发送信息
                _KUKA_Client.BeginReceive(State.Server_Recv_Byte, 0, State.Server_Recv_Byte.Length, SocketFlags.None, new AsyncCallback(KUKA_Client_Received), State);

                //接收其他客户端连接
                Socket_Server.BeginAccept(new AsyncCallback(KUKA_Client_Connect), Socket_Server);


            }

        }

        /// <summary>
        /// 接收客户端发送信息处理
        /// </summary>
        /// <param name="ar"></param>
        public void KUKA_Client_Received(IAsyncResult ar)
        {
            if (IsRuning)
            {
                Socket_Models_Server State = (Socket_Models_Server)ar.AsyncState;
                Socket _KUKA_Client = State.Server_Kuka_Client;

        

                lock (State)
                {

                int Recv_Byte = _KUKA_Client.EndReceive(ar);
                if (Recv_Byte == 0)
                {
                    ClientCount--;
                    Messenger.Default.Send<int>(ClientCount, "ClientCount");
                    //接收数据0的时候处理
                    KUKA_Client_Close(State);
                    return;
                }

        
       
                    //处理接收的数据
                    KUKA_Received_Val(State);


                }

                //接收客户端发送信息
                _KUKA_Client.BeginReceive(State.Server_Recv_Byte, 0, State.Server_Recv_Byte.Length, SocketFlags.None, new AsyncCallback(KUKA_Client_Received), State);


            }

        }


        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="_Byte"></param>
        public void KUKA_Received_Val(Socket_Models_Server SM_Server)
        {

            var a = SM_Server.Server_Kuka_Client.RemoteEndPoint.ToString();

            MessageBox.Show(a + Encoding.ASCII.GetString(SM_Server.Server_Recv_Byte));

        }


        /// <summary>
        /// 异步制定客户端发送数据
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Date"></param>
        public void Server_Send(Socket Client, byte[] Date)
        {

            Client.BeginSend(Date, 0, Date.Length, SocketFlags.None, new AsyncCallback(Server_SendEnd), Client);

        }


        /// <summary>
        /// 数据发送完成处理
        /// </summary>
        /// <param name="ar"></param>
        public void Server_SendEnd(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);

        }






        /// <summary>
        /// 关闭其中选定客户端连接
        /// </summary>
        /// <param name="_Server"></param>
        public   void KUKA_Client_Close(Socket_Models_Server _Server)
        {
            if (_Server != null)
            {
                _Server.Server_Send_Data = null;
                _Server.Server_Recv_Byte = null;

                KUKA_Client_List.Remove(_Server);

                _Server.Server_Closer();
            }


        }




        /// <summary>
        /// 服务器连接停止
        /// </summary>
        public  void Socket_Server_Stop()
        {



   
            
            if (IsRuning)
            {
                IsRuning = false;
                ClientCount = 0;
                Messenger.Default.Send<int>(ClientCount, "ClientCount");


                foreach (var item in KUKA_Client_List.ToArray())
                {
                    KUKA_Client_Close(item);
                    
                }
  
                   
                    //Socket_Server.Shutdown(SocketShutdown.Both);
                 
                Socket_Server.Close();
                
            }


        }



        #endregion



    }
}
