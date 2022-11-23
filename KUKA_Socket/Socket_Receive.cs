using HanGao.Socket_KUKA;
using KUKA_Socket.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Soceket_KUKA
{

    public class Socket_Receive
    {

        public Socket_Receive()
        {






        }



        public   delegate  string  ReceiveMessage_delegate<T1,T2>(T1 _T,T2 _S);
        public   ReceiveMessage_delegate<Calibration_Data_Receive,string> KUKA_Receive_Calibration_String { set; get; }
        public ReceiveMessage_delegate<Calibration_Data_Receive, string> KUKA_Receive_Find_String { set; get; }




        private  Socket Socket_Sever { set; get; }


        private static byte[] buffer = new byte[1024 * 1024];
        private static int ConnectNumber = 0;

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        public void Server_Strat(string _IP, string _Port)
        {
            //创建套接字
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));

            Socket_Sever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //绑定端口和IP
            Socket_Sever.Bind(ipe);
            //设置监听
            Socket_Sever.Listen(10);

            //异步接收客户端
            Socket_Sever.BeginAccept(new AsyncCallback(ClienAppcet), Socket_Sever);
  
          


        }

        /// <summary>
        /// 服务器停止监听
        /// </summary>
        public void Sever_End()
        {
         

          
            Socket_Sever.Close();


        }



        /// <summary>
        /// 查找本机所有IP地址
        /// </summary>
        /// <returns></returns>
        public List<string> GetLocalIP()
        {

            try
            {
                IPAddress[] _ipArray;
                _ipArray = Dns.GetHostAddresses(Dns.GetHostName());

                List<string> _IPAddress = new List<string>();
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


        /// <summary>
        /// 异步接收连接方法
        /// </summary>
        /// <param name="ar"></param>
        private  void ClienAppcet(IAsyncResult ar)
        {
            //每当连接进来的客户端数量增加时链接数量自增1
            ConnectNumber++;
            //服务端对象获取
            Socket ServerSocket = ar.AsyncState as Socket;
            if (null != ServerSocket)
            {

                try
                {

                //得到接受进来的socket客户端
                Socket client = ServerSocket.EndAccept(ar);
                //开始异步接收客户端数据
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                }
                catch (Exception)
                {

                    return ;
                }

                Console.WriteLine("第" + ConnectNumber + "连接进来了");

            }



            if (null != ServerSocket)
            {
                //通过递归来不停的接收客户端的连接
                ServerSocket.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);
            }

        }







        private  void ReceiveMessage(IAsyncResult ar)
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




                    string _S=Vision_Model(message);

          


                    client.Send(Encoding.UTF8.GetBytes(_S));
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

        /// <summary>
        /// 视觉功能模式
        /// </summary>
        /// <param name="_St"></param>
        /// <returns></returns>
        public  string   Vision_Model(string _St)
        {

            Calibration_Data_Receive _Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);
            string _Str="";
                switch (_Receive.Model)
                {
                    case Vision_Model_Enum.Calibration_Point:

                    _Str= KUKA_Receive_Calibration_String(_Receive, _St);

                    break;
                    case Vision_Model_Enum.Find_Model:


                    _Str= KUKA_Receive_Find_String(_Receive, _St);
                    break;

                }

            return _Str;

        }




    }
}
