

using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HanGao.Socket_KUKA;
using System.Net;
using System.Collections.Generic;
using static Soceket_Connect.Socket_Connect;

namespace Soceket_KUKA
{
 
    public class Socket_Receive
    {

        public Socket_Receive( )
        {

         




        }


        public void Server_Strat(string _IP, string _Port)
        {
            //创建套接字
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口和IP
            socket.Bind(ipe);
            //设置监听
            socket.Listen(10);

            //异步接收客户端
            socket.BeginAccept(new AsyncCallback(ClienAppcet), socket);
        }

        public   delegate void ReceiveMessage_delegate<T>(T _T);
        public static  ReceiveMessage_delegate<dynamic > KUKA_Receive_Delegate { set; get; }


        private static byte[] buffer = new byte[1024*1024];
        private static int ConnectNumber = 0;


        public List<string > GetLocalIP()
        {
        
            try
            {
                IPAddress[] _ipArray;
                _ipArray = Dns.GetHostAddresses(Dns.GetHostName());
         
               List< string >_IPAddress=new List<string > ();
                foreach (var _ip in _ipArray)
                {
                    if (_ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _IPAddress.Add(_ip.ToString());


                    }

                }

                return _IPAddress;
            }
            catch (Exception)
            {
                
                //MessageBox.Show(ex.StackTrace + "\r\n" + ex.Message, "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                //Log.WriteLog(ex);
            }
            //if (localIp == null)
            //{
            //    localIp = IPAddress.Parse("127.0.0.1");
            //}
            return default;
        }


        private static void ClienAppcet(IAsyncResult ar)
        {
            //每当连接进来的客户端数量增加时链接数量自增1
            ConnectNumber++;
            //服务端对象获取
            Socket ServerSocket = ar.AsyncState as Socket;
            if (null != ServerSocket)
            {
                //得到接受进来的socket客户端
                Socket client = ServerSocket.EndAccept(ar);

                Console.WriteLine("第" + ConnectNumber + "连接进来了");

                //开始异步接收客户端数据
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            }



            if (null != ServerSocket)
            {
                //通过递归来不停的接收客户端的连接
                ServerSocket.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);
            }

        }







        private static void ReceiveMessage(IAsyncResult ar)
        {
            Socket client = ar.AsyncState as Socket; //客户端对象
            if (client != null)
            {
                IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint;
                try
                {
                    int length = client.EndReceive(ar);

                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    //WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                    //每当服务器收到消息就会给客户端返回一个Server received data
                    client.Send(Encoding.UTF8.GetBytes("Server received data"));
                    //通过递归不停的接收该客户端的消息
                    client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                }
                catch (Exception)
                {
                    //设置计数器
                    ConnectNumber--;
                    //断开连接
                    //WriteLine(clientipe + " is disconnected，total connects " + (connectCount), ConsoleColor.Red);
                }
            }

        }







    }
}
