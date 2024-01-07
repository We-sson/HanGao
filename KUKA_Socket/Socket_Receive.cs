using HanGao.Socket_KUKA;
using KUKA_Socket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using static Soceket_Connect.Socket_Connect;

namespace Soceket_KUKA
{

    public class Socket_Receive : IDisposable
    {

        public Socket_Receive(string _IP, string _Port)
        {


            Server_Strat(_IP, _Port);



        }


        /// <summary>
        /// 接收委托类型声明
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="_T"></param>
        /// <param name="_S"></param>
        /// <returns></returns>
        public delegate string ReceiveMessage_delegate<T1, T2>(T1 _T, T2 _S);

        /// <summary>
        /// 机器人通讯
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; }

        /// <summary>
        /// 声明接收信息委托
        /// </summary>
        public ReceiveMessage_delegate<Calibration_Data_Receive, string> KUKA_Receive_Calibration_New_String { set; get; }
        public ReceiveMessage_delegate<Calibration_Data_Receive, string> KUKA_Receive_Calibration_Add_String { set; get; }
        public ReceiveMessage_delegate<Calibration_Data_Receive, string> KUKA_Receive_Calibration_Text_String { set; get; }

        public ReceiveMessage_delegate<Calibration_Data_Receive, string> KUKA_Receive_Find_String { set; get; }
        public ReceiveMessage_delegate<Vision_Ini_Data_Receive, string> KUKA_Receive_Vision_Ini_String { set; get; }



        public ReceiveMessage_delegate<KUKA_HandEye_Calibration_Receive, string> HandEye_Calibration_String { set; get; }

        


        /// <summary>
        /// 通讯连接错误委托
        /// </summary>
        public Socket_T_delegate<string > Socket_ErrorInfo_delegate { set; get; }




        /// <summary>
        /// 通讯服务器
        /// </summary>
        public Socket Socket_Sever { set; get; }


        private static byte[] buffer = new byte[1024 * 1024];
        private static int ConnectNumber = 0;

        /// <summary>
        /// 接收文件编码信息
        /// </summary>
        public string Receive_Information { set; get; }=string.Empty;


        /// <summary>
        /// 发生文本编码信息
        /// </summary>
        public string Send_Information { set; get; }=string.Empty;  


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
            //设置通讯口可重用端口
            Socket_Sever.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
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

            try
            {

            Socket_Sever.Shutdown(SocketShutdown.Both);
            Socket_Sever.Close();
            }
            catch (Exception)
            {

            Socket_Sever.Dispose();
              
            }

        }



        /// <summary>
        /// 查找本机所有IP地址
        /// </summary>
        /// <returns></returns>
        public static bool GetLocalIP(ref List<string> _IPAddress)
        {

            try
            {
                IPAddress[] _ipArray;
                _ipArray = Dns.GetHostAddresses(Dns.GetHostName());

                _IPAddress = new List<string>();
                foreach (var _ip in _ipArray)
                {
                    if (_ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _IPAddress.Add(_ip.ToString());


                    }

                }

                return true;
            }
            catch (Exception)
            {

                return false;

            }


        }


        /// <summary>
        /// 异步接收连接方法
        /// </summary>
        /// <param name="ar"></param>
        private void ClienAppcet(IAsyncResult ar)
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
                catch (Exception e)
                {
                    Socket_ErrorInfo_delegate(e.Message);

                    return;
                }

                Socket_ErrorInfo_delegate("第" + ConnectNumber + "连接进来了");

                Console.WriteLine("第" + ConnectNumber + "连接进来了");

            }



            if (null != ServerSocket)
            {
                //通过递归来不停的接收客户端的连接
                ServerSocket.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);
            }

        }






        /// <summary>
        /// 异步消息接收
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage(IAsyncResult ar)
        {
            Socket client = ar.AsyncState as Socket; //客户端对象
            if (client != null)
            {
                IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint;
                try
                {
                    int length = client.EndReceive(ar);
                    string _S = string.Empty;
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    //WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                    //每当服务器收到消息就会给客户端返回一个Server received data
                   



                    if (message == "")
                    {
                        Socket_ErrorInfo_delegate("设备IP: " + clientipe.Address.ToString() + " 断开连接! ");
                        return;
                    }

                    Receive_Information = message;

                    switch (Socket_Robot)
                    {
                        case Socket_Robot_Protocols_Enum.KUKA:
                             _S = KUKA_EKL_Socket(message);

                            break;
                        case Socket_Robot_Protocols_Enum.ABB:



                            break;
                        case Socket_Robot_Protocols_Enum.川崎:
                            break;
                    }

                    Send_Information = _S;

                    if (_S!=string.Empty)
                    {

                    client.Send(Encoding.UTF8.GetBytes(_S));
                    //通过递归不停的接收该客户端的消息
                    client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                    }
                    else
                    {
                        throw new Exception("现有通讯协议无法解析，请联系开发者！");
                    }
                }
                catch (Exception e)
                {

                    //设置计数器
                    ConnectNumber--;

                    Socket_ErrorInfo_delegate(e.Message);

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
        public string KUKA_EKL_Socket(string _St)
        {
            if (_St != "")
            {

                //提取接收内容解析
                XElement _KUKA_Receive= XElement.Parse(_St);
                Vision_Model_Enum _Model = Enum.Parse<Vision_Model_Enum>( _KUKA_Receive.Attribute("Model").Value.ToString());

                string _Str = "";
                //将对应的功能反序列化处理
                switch (_Model)
                {
                    case Vision_Model_Enum.Calibration_New:
                        Calibration_Data_Receive _Calibration_New_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                        _Str = KUKA_Receive_Calibration_New_String(_Calibration_New_Receive, _St);

                        break;
                    case Vision_Model_Enum.Calibration_Text:
                        Calibration_Data_Receive _Calibration_Text_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                        _Str = KUKA_Receive_Calibration_Text_String(_Calibration_Text_Receive, _St);

                        break;

                    case Vision_Model_Enum.Find_Model:

                        Calibration_Data_Receive _Find_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                        _Str = KUKA_Receive_Find_String(_Find_Receive, _St);
                        break;

                    case Vision_Model_Enum.Vision_Ini_Data:

                        Vision_Ini_Data_Receive _Vision_Receive = KUKA_Send_Receive_Xml.String_Xml<Vision_Ini_Data_Receive>(_St);

                        _Str = KUKA_Receive_Vision_Ini_String(_Vision_Receive, _St);

                 

                        break;

                    case Vision_Model_Enum.HandEye_Calib_Date:

                        KUKA_HandEye_Calibration_Receive _HandEye_Receive = KUKA_Send_Receive_Xml.String_Xml<KUKA_HandEye_Calibration_Receive>(_St);
                       
                        _Str= HandEye_Calibration_String(_HandEye_Receive, _St);

                        break;


                }

                return _Str;
            }
            else
            {
                return "";
            }

        }


        public string ABB_PC_Socket(string _Str)
        {



            return "";
        }



        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
