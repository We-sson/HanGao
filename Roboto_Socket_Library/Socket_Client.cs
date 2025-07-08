using Roboto_Socket_Library.Model;
using System.Net;
using System.Net.Sockets;

namespace Roboto_Socket_Library
{
    public class Socket_Client_Model
    {
        public Socket_Client_Model()
        {


        }

        public Socket_Client_Model(string _IP, int _Port)
        {


        }

        public string IP = "127.0.0.1";
        public int Port = 5000;


        public Socket? Socket_Client { set; get; }



        /// <summary>
        /// 通讯连接错误委托
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }


        public Socket_T_delegate<string>? Socket_ConnectInfo_delegate { set; get; }




        public bool IsConnect { set; get; } = false;






        public bool Connect()
        {
            try
            {

                IPAddress _IP = IPAddress.Parse(IP);
                IPEndPoint iPEndPoint = new IPEndPoint(_IP, Port);


                Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);        //创建与远程主机的连接
                Socket_Client.Connect(iPEndPoint);
                IsConnect = true;


                Socket_ConnectInfo_delegate?.Invoke($"IP：{IP}，Port：{Port}，连接服务器成功！", Socket_Client);
                return true;

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"IP：{IP}，Port：{Port}，开启服务失败！原因：" + e.Message, Socket_Client);
                Socket_Client?.Close();
                IsConnect = false;
                return false;

            }


        }













    }
}
