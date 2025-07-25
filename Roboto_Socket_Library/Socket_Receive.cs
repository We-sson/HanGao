
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
        public Socket_Receive()
        {


            //Server_Strat(_IP, _Port);



        }

        /// <summary>
        /// 接收委托类型声明
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="_T"></param>
        /// <param name="_S"></param>
        /// <returns></returns>
        public delegate T2 ReceiveMessage_delegate<T1, T2>(T1 _T, Socket? _socket = null);


        public delegate void ClientMessage_delegate<T1>(T1 _T);





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
        public ReceiveMessage_delegate<Robot_Mes_Info_Data_Receive, Robot_Mes_Info_Data_Send>? Robot_Info_Model_Data_Delegate { set; get; }



        /// <summary>
        /// 信息接受服务器委托
        /// </summary>
        public ReceiveMessage_delegate<Mes_Server_Info_Data_Receive, Mes_Server_Info_Data_Send>? Mes_Server_Info_Data_Delegate { set; get; }




        /// <summary>
        /// 看板接受消息委托
        /// </summary>
        public ClientMessage_delegate<Mes_Server_Info_Data_Send>? Mes_Receive_Info_Data_Delegate { set; get; }








        public ReceiveMessage_delegate<Mes_Server_Info_Data_Receive, Mes_Server_Info_Data_Send>? Mes_Client_Info_Data_Delegate { set; get; }









        public Message_Byte_delegate<byte[]>? Socket_Receive_Meg { set; get; }
        public Message_Byte_delegate<byte[]>? Socket_Send_Meg { set; get; }

        /// <summary>
        /// 通讯连接错误委托
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }


        public Socket_T_delegate<string>? Socket_ConnectInfo_delegate { set; get; }


        private ManualResetEvent Connnect_State { set; get; } = new ManualResetEvent(false);


        /// <summary>
        /// 通讯服务器
        /// </summary>
        public Socket? Socket_Sever { set; get; }



        public Socket? Socket_Client { set; get; }

        //public bool Client_Connect { set; get; }

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



        private readonly ManualResetEvent Timeout_Event = new ManualResetEvent(false);


        /// <summary>
        /// 带有超时连接时间服务器连接方法
        /// </summary>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        /// <param name="TimeOut"></param>
        /// <returns></returns>
        public bool Connect(string _IP, string _Port, int TimeOut = 500)
        {
            try
            {
                Timeout_Event.Reset();
                //创建套接字
                IPEndPoint ipe = new(IPAddress.Parse(_IP), int.Parse(_Port));

                Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);        //创建与远程主机的连接
                //Socket_Client.Connect(ipe);
                Socket_Client?.BeginConnect(ipe, new AsyncCallback(Client_Inf), Socket_Client);

                if (Timeout_Event.WaitOne(TimeOut))
                {


                    Socket_ConnectInfo_delegate?.Invoke($"IP：{_IP}，Port：{_Port}，连接服务器成功！", Socket_Client);
                    return true;

                }
                else
                {
                    Socket_ConnectInfo_delegate?.Invoke($"Error:-1,IP：{_IP}，Port：{_Port}，连接服务器超时退出！", Socket_Client);
                    //Socket_Client?.Shutdown(SocketShutdown.Both);
                    Socket_Client?.Close();
                    Socket_Client?.Dispose();


                    return false;

                }

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"Error:-2,IP：{_IP}，Port：{_Port}，开启服务失败！原因：" + e.Message, Socket_Client);
                Socket_Client?.Close();
                Socket_Client?.Dispose();
                return false;

            }


        }







        /// <summary>
        /// 异步连接回调命令
        /// </summary>
        /// <param name="ar"></param>
        private void Client_Inf(IAsyncResult ar)
        {

            try
            {
                //Task.Delay(10);
                //挂起读取异步连接
                Socket_Client?.EndConnect(ar!);
                //异步接收客户端
                //Socket_Client?.BeginAccept(new AsyncCallback(ClienAppcet), Socket_Client);

                //Client_Connect = true;
                Timeout_Event.Set();

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"Error: -51 原因:" + e.Message, Socket_Client);
                //Client_Connect = false;
                Socket_Client?.Close();
                Socket_Client?.Dispose();

                //Socket_Client?.Shutdown(SocketShutdown.Both);



                return;
            }


        }




        public void Send_Val<T1>(Socket_Robot_Protocols_Enum _Robot_Protocols, Vision_Model_Enum _Model, T1 _val)
        {

            try
            {


                Robot_Socket_Protocol _Socket_Protoco = new Robot_Socket_Protocol(_Robot_Protocols, _Model);
                Byte[] Send_byte = Array.Empty<byte>();

                //_Socket_Protoco.Socket_Send_Set_Data(_val);

                Send_byte = _Socket_Protoco.Socket_Send_Set_Data(_val ?? new object()) ?? Array.Empty<byte>();


                if (Send_byte != Array.Empty<byte>() && ((bool?)Socket_Client?.Connected ?? false))
                {

                    //委托显示发送数据
                    Socket_Send_Meg?.Invoke(Send_byte);
                    Socket_Client?.Send(Send_byte);
                    //通过递归不停的接收该客户端的消息
                    Socket_Client?.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Client_ReceiveMessage), Socket_Client);
                }
                else
                {
                    throw new Exception("Error:-3,现有通讯协议无法解析，请联系开发者！");
                }

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"Error: -51 原因:" + e.Message, Socket_Client);

            }
        }




        /// <summary>
        /// 异步消息接收
        /// </summary>
        /// <param name="ar"></param>
        private void Client_ReceiveMessage(IAsyncResult ar)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            //客户端对象
            if (ar!.AsyncState is Socket client)
            {
                try
                {
                    IPEndPoint? clientipe = (IPEndPoint)client?.RemoteEndPoint!;
                    int length = client?.EndReceive(ar) ?? 0;
                    string _S = string.Empty;

                    Byte[] Send_byte = Array.Empty<byte>();
                    //WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                    //每当服务器收到消息就会给客户端返回一个Server received data



                    if (length == 0)
                    {
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-4,{clientipe}: 断开连接! ", client);
                        client?.Close();
                        client?.Dispose();
                        //Client_Connect = false;
                        return;
                    }


                    //接触数据长度
                    byte[] _Reveice_Meg = buffer.Skip(0).Take(length).ToArray();
                    //委托显示接受数据
                    Socket_Receive_Meg?.Invoke(_Reveice_Meg);


                    //创建协议处理类型,处理协议头部解析类型
                    Robot_Socket_Protocol _Socket_Protocol = new(Socket_Robot, _Reveice_Meg);

                    ///根据协议类型处理对应内容
                    switch (_Socket_Protocol.Vision_Model)
                    {


                        case Vision_Model_Enum.Mes_Server_Info_Rece_Data:


                            Mes_Server_Info_Data_Send? _Mes_Server_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Mes_Server_Info_Data_Send>();

                            Mes_Receive_Info_Data_Delegate?.Invoke(_Mes_Server_Rece!);


                            break;


                    }

                }
                catch (Exception e)
                {

                    //设置计数器
                    //ConnectNumber--;
                    Socket_ErrorInfo_delegate?.Invoke("Error:-5," + e.Message, client);
                    client?.Close();
                    client?.Dispose();

                    //断开连接
                    //WriteLine(clientipe + " is disconnected，total connects " + (connectCount), ConsoleColor.Red);
                }
            }

        }





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

                Socket_ErrorInfo_delegate?.Invoke($"Error:-7,IP：{_IP}，Port：{_Port}，开启服务失败！原因：" + e.Message, Socket_Sever);

                //Socket_Sever.Close();
                //Socket_Sever.Dispose();


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

                throw new Exception("Error:-8,本地IP获取失败！，请检查网络配置。原因：" + _e.Message);

            }


        }


        /// <summary>
        /// 异步接收连接方法
        /// </summary>
        /// <param name="ar"></param>
        private void ClienAppcet(IAsyncResult ar)
        {
            //每当连接进来的客户端数量增加时链接数量自增1

            try
            {



                ConnectNumber++;
                //服务端对象获取
                Socket? ServerSocket = ar.AsyncState as Socket;
                Socket? client = ServerSocket?.EndAccept(ar);
                if (null != ServerSocket)
                {
                    try
                    {


                        //得到接受进来的socket客户端
                        //开始异步接收客户端数据
                        client?.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                    }
                    catch (Exception e)
                    {
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-15" + e.Message, client);
                        ServerSocket?.Close();
                        ServerSocket?.Dispose();
                        return;
                    }

                    Socket_ConnectInfo_delegate?.Invoke($"{ServerSocket.LocalEndPoint}:连接进来了", client);

                    Console.WriteLine("第" + ConnectNumber + "连接进来了");

                }



                //通过递归来不停的接收客户端的连接
                ServerSocket?.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);
            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"Error:-16" + e.Message);


            }
        }






        /// <summary>
        /// 异步消息接收
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage(IAsyncResult ar)
        {

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //客户端对象
            if (ar!.AsyncState is Socket client)
            {
                try
                {
                    IPEndPoint clientipe = (IPEndPoint)client?.RemoteEndPoint!;
                    int length = client?.EndReceive(ar) ?? 0;
                    string _S = string.Empty;

                    Byte[] Send_byte = Array.Empty<byte>();
                    //WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                    //每当服务器收到消息就会给客户端返回一个Server received data



                    if (length == 0)
                    {
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-9,{clientipe}: 断开连接! ", client);
                        client?.Close();
                        client?.Dispose();
                        //Client_Connect = false;
                        return;
                    }


                    //接触数据长度
                    byte[] _Reveice_Meg = buffer.Skip(0).Take(length).ToArray();
                    //委托显示接受数据
                    Socket_Receive_Meg?.Invoke(_Reveice_Meg);


                    //创建协议处理类型,处理协议头部解析类型
                    Robot_Socket_Protocol _Socket_Protocol = new(Socket_Robot, _Reveice_Meg);

                    ///根据协议类型处理对应内容
                    switch (_Socket_Protocol.Vision_Model)
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

                            Robot_Mes_Info_Data_Send? _Mes_Info_Send = Robot_Info_Model_Data_Delegate?.Invoke(_Mes_Info_Rece!);

                            Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Mes_Info_Send ?? new Robot_Mes_Info_Data_Send()) ?? Array.Empty<byte>();



                            break;


                        case Vision_Model_Enum.Mes_Server_Info_Send_Data:


                            Mes_Server_Info_Data_Receive? _Mes_Server_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Mes_Server_Info_Data_Receive>();

                            Mes_Server_Info_Data_Send? _Mes_Server_Send = Mes_Server_Info_Data_Delegate?.Invoke(_Mes_Server_Rece!, client);

                            Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Mes_Server_Send ?? new Mes_Server_Info_Data_Send()) ?? Array.Empty<byte>();


                            break;




                    }


                    //Send_Information = _S;

                    if (Send_byte != Array.Empty<byte>())
                    {

                        //委托显示发送数据
                        Socket_Send_Meg?.Invoke(Send_byte);
                        client?.Send(Send_byte);
                        //通过递归不停的接收该客户端的消息
                        client?.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                    }
                    else
                    {
                        throw new Exception("Error:-10,现有通讯协议无法解析，请联系开发者！");
                    }
                }
                catch (Exception e)
                {

                    //设置计数器
                    Socket_ErrorInfo_delegate?.Invoke("Error:-11," + e.Message, client);
                    ConnectNumber--;
                    client?.Close();
                    client?.Dispose();

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





    public enum Socket_Type_Enum
    {
        Server,
        Client
    }





}
