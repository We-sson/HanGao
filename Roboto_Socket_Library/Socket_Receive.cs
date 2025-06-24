
using Roboto_Socket_Library.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;


namespace Roboto_Socket_Library
{

    public class Socket_Receive
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
        public delegate T2 ReceiveMessage_delegate<T1, T2>(T1 _T);

        public delegate void Message_Byte_delegate<T1>(T1 _Meg);

        /// <summary>
        /// 机器人通讯
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; }



        public ReceiveMessage_delegate<Vision_Find_Data_Receive, Vision_Find_Data_Send>? Vision_Find_Model_Delegate { set; get; }


        /// <summary>
        /// 视觉程序初始化协议委托
        /// </summary>
        public ReceiveMessage_delegate<Vision_Ini_Data_Receive, Vision_Ini_Data_Send>? Vision_Ini_Data_Delegate { set; get; }


        /// <summary>
        /// 手眼标定数据协议委托
        /// </summary>
        public ReceiveMessage_delegate<HandEye_Calibration_Receive, HandEye_Calibration_Send>? HandEye_Calibration_Data_Delegate { set; get; }



        /// <summary>
        /// 视觉创建模型接受协议委托
        /// </summary>
        public ReceiveMessage_delegate<Vision_Creation_Model_Receive, Vision_Creation_Model_Send>? Vision_Creation_Model_Data_Delegate { set; get; }


        /// <summary>
        /// 视觉创建模型接受协议委托
        /// </summary>
        public ReceiveMessage_delegate<Robot_Mes_Info_Data_Receive, Robot_Mes_Info_Data_Send>? Mes_Info_Model_Data_Delegate { set; get; }




        public Message_Byte_delegate<byte[]>? Socket_Receive_Meg { set; get; }
        public Message_Byte_delegate<byte[]>? Socket_Send_Meg { set; get; }

        /// <summary>
        /// 通讯连接错误委托
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }


        public Socket_T_delegate<string>? Socket_ConnectInfo_delegate { set; get; }




        /// <summary>
        /// 通讯服务器
        /// </summary>
        public Socket? Socket_Sever { set; get; }


        private static byte[] buffer = new byte[1024 * 1024];
        private static int ConnectNumber = 0;

        /// <summary>
        /// 接收文件编码信息
        /// </summary>
        public string Receive_Information { set; get; } = string.Empty;


        /// <summary>
        /// 发生文本编码信息
        /// </summary>
        public string Send_Information { set; get; } = string.Empty;


        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        public void Server_Strat(string _IP, string _Port)
        {


            try
            {

                //创建套接字
                IPEndPoint ipe = new(IPAddress.Parse(_IP), int.Parse(_Port));

                Socket_Sever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //设置通讯口可重用端口
                Socket_Sever.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                //Socket_Sever.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.KeepAlive, true);
                //Socket_Sever.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.TcpKeepAliveTime, 30);

                //绑定端口和IP
                Socket_Sever.Bind(ipe);
                //设置监听
                Socket_Sever.Listen(10);

                //异步接收客户端
                Socket_Sever.BeginAccept(new AsyncCallback(ClienAppcet), Socket_Sever);


            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"IP：{_IP}，Port：{_Port}，开启服务失败！原因：" + e.Message);
            }


        }

        /// <summary>
        /// 服务器停止监听
        /// </summary>
        public void Sever_End()
        {

            try
            {

                Socket_Sever?.Shutdown(SocketShutdown.Both);
                Socket_Sever?.Close();
            }
            catch (Exception)
            {

                Socket_Sever?.Dispose();

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

                //添加本地监听
                _IPAddress.Add("127.0.0.1");

                return true;
            }
            catch (Exception _e)
            {

                throw new Exception("本地IP获取失败！，请检查网络配置。原因：" + _e.Message);

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
            Socket? ServerSocket = ar!.AsyncState! as Socket;
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
                    Socket_ErrorInfo_delegate?.Invoke(e.Message);

                    return;
                }

                Socket_ConnectInfo_delegate?.Invoke($"{ServerSocket.LocalEndPoint}:连接进来了");

                Console.WriteLine("第" + ConnectNumber + "连接进来了");

            }



            //通过递归来不停的接收客户端的连接
            ServerSocket?.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);

        }






        /// <summary>
        /// 异步消息接收
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage(IAsyncResult ar)
        {
            //客户端对象
            if (ar!.AsyncState is Socket client)
            {
                IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint!;
                try
                {
                    int length = client.EndReceive(ar);
                    string _S = string.Empty;

                    Byte[] Send_byte = Array.Empty<byte>();
                    //WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                    //每当服务器收到消息就会给客户端返回一个Server received data



                    if (length == 0)
                    {
                            Socket_ErrorInfo_delegate?.Invoke($"{clientipe}: 断开连接! ");
                        return;
                    }


                    //接触数据长度
                    byte[] _Reveice_Meg = buffer.Skip(0).Take(length).ToArray();
                    //委托显示接受数据
                    Socket_Receive_Meg?.Invoke(_Reveice_Meg);


                        //创建协议处理类型,处理协议头部解析类型
                    Robot_Socket_Protocol _Socket_Protocol = new (Socket_Robot, _Reveice_Meg);

                        ///根据协议类型处理对应内容
                        switch (_Socket_Protocol.Vision_Model_Type)
                        {
                            case Vision_Model_Enum.Calibration_New:
                                break;
                            case Vision_Model_Enum.Calibration_Text:
                                break;
                            case Vision_Model_Enum.Calibration_Add:
                                break;
                            case Vision_Model_Enum.Find_Model:


                                Vision_Find_Data_Receive? _Vision_Find_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Find_Data_Receive>();

                                Vision_Find_Data_Send? _Vision_Find_Send = Vision_Find_Model_Delegate?.Invoke(_Vision_Find_Rece!);

                                Send_byte = _Socket_Protocol.Socket_Send_Set_Data<Vision_Find_Data_Send>(_Vision_Find_Send ?? new Vision_Find_Data_Send()) ?? Array.Empty<byte>();

                                break;
                            case Vision_Model_Enum.Vision_Ini_Data:

                                Vision_Ini_Data_Receive? _Vision_Ini_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Ini_Data_Receive>();

                                Vision_Ini_Data_Send? _Vision_Ini_Send = Vision_Ini_Data_Delegate?.Invoke(_Vision_Ini_Rece!);

                                Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Vision_Ini_Send ?? new Vision_Ini_Data_Send()) ?? Array.Empty<byte>();

                                break;
                            case Vision_Model_Enum.HandEye_Calib_Date:


                                HandEye_Calibration_Receive? _HandEye_Rece = _Socket_Protocol.Socket_Receive_Get_Date<HandEye_Calibration_Receive>();

                                HandEye_Calibration_Send? _Hand_Send = HandEye_Calibration_Data_Delegate?.Invoke(_HandEye_Rece!);

                                Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Hand_Send ?? new HandEye_Calibration_Send()) ?? Array.Empty<byte>();


                                break;

                            case Vision_Model_Enum.Vision_Creation_Model:

                                Vision_Creation_Model_Receive? _Vision_Creation_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Creation_Model_Receive>();

                                Vision_Creation_Model_Send? __Vision_Creation_Send = Vision_Creation_Model_Data_Delegate?.Invoke(_Vision_Creation_Rece!);

                                Send_byte = _Socket_Protocol.Socket_Send_Set_Data(__Vision_Creation_Send ?? new Vision_Creation_Model_Send()) ?? Array.Empty<byte>();

                                break;

                            case Vision_Model_Enum.Mes_Info_Data:


                                Robot_Mes_Info_Data_Receive? _Mes_Info_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>();

                                Robot_Mes_Info_Data_Send? _Mes_Info_Send = Mes_Info_Model_Data_Delegate?.Invoke(_Mes_Info_Rece!);

                                Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Mes_Info_Send ?? new Robot_Mes_Info_Data_Send()) ?? Array.Empty<byte>();



                                break;

                        }


                        //Send_Information = _S;

                        if (Send_byte != Array.Empty<byte>())
                        {

                            //委托显示发送数据
                            Socket_Send_Meg?.Invoke(Send_byte);
                            client.Send(Send_byte);
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

                    Socket_ErrorInfo_delegate?.Invoke(e.Message);

                    //断开连接
                    //WriteLine(clientipe + " is disconnected，total connects " + (connectCount), ConsoleColor.Red);
                }
            }

        }

        ///// <summary>
        ///// 视觉功能模式
        ///// </summary>
        ///// <param name="_St"></param>
        ///// <returns></returns>
        //public string KUKA_EKL_Socket(string _St)
        //{
        //    if (_St != "")
        //    {

        //        //提取接收内容解析
        //        XElement _KUKA_Receive= XElement.Parse(_St);
        //        Vision_Model_Enum _Model = Enum.Parse<Vision_Model_Enum>( _KUKA_Receive.Attribute("Model")!.Value.ToString());

        //        string _Str = "";
        //        //将对应的功能反序列化处理
        //        switch (_Model)
        //        {
        //            case Vision_Model_Enum.Calibration_New:
        //                Calibration_Data_Receive _Calibration_New_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

        //                _Str = KUKA_Receive_Calibration_New_String(_Calibration_New_Receive, _St);

        //                break;
        //            case Vision_Model_Enum.Calibration_Text:
        //                Calibration_Data_Receive _Calibration_Text_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

        //                _Str = KUKA_Receive_Calibration_Text_String(_Calibration_Text_Receive, _St);

        //                break;

        //            case Vision_Model_Enum.Find_Model:

        //                Calibration_Data_Receive _Find_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

        //                _Str = KUKA_Receive_Find_String(_Find_Receive, _St);
        //                break;

        //            case Vision_Model_Enum.Vision_Ini_Data:

        //                Vision_Ini_Data_Receive _Vision_Receive = KUKA_Send_Receive_Xml.String_Xml<Vision_Ini_Data_Receive>(_St);

        //                _Str = KUKA_Receive_Vision_Ini_String(_Vision_Receive, _St);



        //                break;

        //            case Vision_Model_Enum.HandEye_Calib_Date:

        //                KUKA_HandEye_Calibration_Receive _HandEye_Receive = KUKA_Send_Receive_Xml.String_Xml<KUKA_HandEye_Calibration_Receive>(_St);

        //                _Str= HandEye_Calibration_String?.Invoke(_HandEye_Receive, _St);

        //                break;


        //        }

        //        return _Str;
        //    }
        //    else
        //    {
        //        return "";
        //    }

        //}






        //public void Dispose()
        //{
        //    GC.Collect();
        //    GC.SuppressFinalize(this);
        //}
    }




    public class KUKA_Send_Receive_Xml
    {
        public KUKA_Send_Receive_Xml()
        {




        }



        /// <summary>
        /// 结果属性转换字符串
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Type"></param>
        /// <returns></returns>
        public static string Property_Xml<T1>(T1 _Type)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = false;


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var str = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(str, settings))
            {
                var xmlSerializer = new XmlSerializer(typeof(T1));
                xmlSerializer.Serialize(xmlWriter, _Type, ns);
            }

            string _St = str.ToString();

            return _St;



        }



        /// <summary>
        /// 字符串转换属性
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static T1 String_Xml<T1>(string _Path) where T1 : class
        {



            using (XmlReader xmlReader = XmlReader.Create(new StringReader(_Path)))
            {

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T1));




                T1 obj = (T1)xmlSerializer!.Deserialize(xmlReader)!;


                return obj;
            }


        }


    }


}
