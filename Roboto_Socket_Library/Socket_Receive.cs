
using Roboto_Socket_Library.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;


namespace Roboto_Socket_Library
{
    /// <summary>
    /// 面向机器人/视觉协议的 TCP 通信门面，可作为服务端接受多个客户端，也可作为客户端主动连接并发送请求。
    /// 网络层收到完整报文后交给 <see cref="Robot_Socket_Protocol"/> 解码，再通过对应业务委托取得应答并编码回传。
    /// </summary>
    public class Socket_Receive
    {
        /// <summary>
        /// 创建服务端实例并立即在指定地址开始监听，接收模式使用固定缓冲的一次一帧方案。
        /// </summary>
        /// <param name="_IP">要绑定的本地 IPv4 地址。</param>
        /// <param name="_Port">要绑定的 TCP 端口文本。</param>
        public Socket_Receive(string _IP, string _Port)
        {


            Server_Strat(_IP, _Port);



        }

        /// <summary>
        /// 创建服务端实例、选择接收策略并立即开始监听。
        /// </summary>
        /// <param name="_IP">要绑定的本地 IPv4 地址。</param>
        /// <param name="_Port">要绑定的 TCP 端口文本。</param>
        /// <param name="_UseXmlFragmentReceive">为 <see langword="true"/> 时按 XML 根元素边界持续读取，适合拆包/粘包场景。</param>
        public Socket_Receive(string _IP, string _Port,bool _UseXmlFragmentReceive)
        {

            UseXmlFragmentReceive = _UseXmlFragmentReceive;
            Server_Strat(_IP, _Port);



        }

        /// <summary>
        /// 创建未启动的通信实例，供调用方稍后选择 <see cref="Connect"/> 或 <see cref="Server_Strat"/>。
        /// </summary>
        public Socket_Receive()
        {


            //Server_Strat(_IP, _Port);



        }

        /// <summary>
        /// 声明“收到请求—执行业务—返回应答”的同步处理函数。
        /// </summary>
        /// <typeparam name="T1">解码后的请求类型。</typeparam>
        /// <typeparam name="T2">业务处理后返回的应答类型。</typeparam>
        /// <param name="_T">解码后的请求。</param>
        /// <param name="_socket">请求来源 Socket；需要按连接区分业务状态时使用。</param>
        /// <returns>要编码并发送给请求方的应答。</returns>
        public delegate T2 ReceiveMessage_delegate<T1, T2>(T1 _T, Socket? _socket = null);

        /// <summary>
        /// 声明只消费服务端回执、不产生下一帧应答的客户端处理函数。
        /// </summary>
        /// <typeparam name="T1">回执 DTO 类型。</typeparam>
        /// <param name="_T">解码后的回执。</param>
        public delegate void ClientMessage_delegate<T1>(T1 _T);



        /// <summary>
        /// 声明原始报文观察回调，用于日志或通信监视界面。
        /// </summary>
        /// <typeparam name="T1">原始消息类型，当前使用 <see cref="byte"/> 数组。</typeparam>
        /// <param name="_Meg">完整发送或接收报文。</param>
        public delegate void Message_Byte_delegate<T1>(T1 _Meg);




        /// <summary>
        /// 当前连接采用的机器人协议族，决定功能码识别和正文编码方式。
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; } = Socket_Robot_Protocols_Enum.KUKA;


        /// <summary>
        /// 视觉查找请求处理器：输入拍摄/路径请求，返回识别结果点位。
        /// </summary>
        public ReceiveMessage_delegate<Vision_Find_Data_Receive, Vision_Find_Data_Send>? Vision_Find_Model_Delegate { set; get; }


        /// <summary>
        /// 视觉程序初始化请求处理器。
        /// </summary>
        public ReceiveMessage_delegate<Vision_Ini_Data_Receive, Vision_Ini_Data_Send>? Vision_Ini_Data_Delegate { set; get; }


        /// <summary>
        /// 手眼标定请求处理器。
        /// </summary>
        public ReceiveMessage_delegate<HandEye_Calibration_Receive, HandEye_Calibration_Send>? HandEye_Calibration_Data_Delegate { set; get; }



        /// <summary>
        /// 视觉建模请求处理器。
        /// </summary>
        public ReceiveMessage_delegate<Vision_Creation_Model_Receive, Vision_Creation_Model_Send>? Vision_Creation_Model_Data_Delegate { set; get; }


        /// <summary>
        /// 机器人生产/MES 状态请求处理器。
        /// </summary>
        public ReceiveMessage_delegate<Robot_Mes_Info_Data_Receive, Robot_Mes_Info_Data_Send>? Robot_Info_Model_Data_Delegate { set; get; }



        /// <summary>
        /// 看板服务器处理客户端周期上传快照的函数；可利用来源 Socket 区分产线。
        /// </summary>
        public ReceiveMessage_delegate<Mes_Server_Info_Data_Receive, Mes_Server_Info_Data_Send>? Mes_Server_Info_Data_Delegate { set; get; }




        /// <summary>
        /// 看板客户端收到服务器回执后的通知函数，不再生成应答。
        /// </summary>
        public ClientMessage_delegate<Mes_Server_Info_Data_Send>? Mes_Receive_Info_Data_Delegate { set; get; }


        /// <summary>
        /// 是否使用流式 XML Fragment 读取模式。
        /// 开启后，只有读取到完整顶层 XML 元素才交给协议层，可正确处理 TCP 拆包和粘包。
        /// </summary>
        public bool UseXmlFragmentReceive { get; set; } = false;


        /// <summary>
        /// 预留的看板客户端请求处理器；当前分派流程使用 <see cref="Mes_Server_Info_Data_Delegate"/>。
        /// </summary>
        public ReceiveMessage_delegate<Mes_Server_Info_Data_Receive, Mes_Server_Info_Data_Send>? Mes_Client_Info_Data_Delegate { set; get; }





        /// <summary>
        /// 固定缓冲接收模式下，每个连接分配的缓冲大小。
        /// 256 KB 可容纳现有几十 KB 的 XML 报文，同时避免旧版 4 MB/连接造成的大对象堆压力。
        /// </summary>
        public const int Receive_Buffer_Size = 256 * 1024;   // 256KB

        /// <summary>收到一帧原始报文后触发，主要用于日志和监视界面。</summary>
        public Message_Byte_delegate<byte[]>? Socket_Receive_Meg { set; get; }

        /// <summary>发送一帧原始报文前触发，主要用于日志和监视界面。</summary>
        public Message_Byte_delegate<byte[]>? Socket_Send_Meg { set; get; }

        /// <summary>
        /// 连接、收发、XML 或业务协议处理失败时触发。
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }

        /// <summary>
        /// 主动连接成功、连接超时或服务端接受新客户端时触发。
        /// </summary>
        public Socket_T_delegate<string>? Socket_ConnectInfo_delegate { set; get; }

        /// <summary>客户端异步收包路径中，用于通知同步发送方已处理回执。</summary>
        private ManualResetEvent Send_State { set; get; } = new ManualResetEvent(false);

        /// <summary>供调用方等待服务端接收事件的公开同步信号；当前内部流程不主动设置。</summary>
        public ManualResetEvent Rece_Event { set; get; } = new ManualResetEvent(false);


        /// <summary>
        /// 服务端监听 Socket。
        /// </summary>
        public Socket? Socket_Sever { set; get; }

        /// <summary>
        /// 主动连接模式下使用的客户端 Socket。
        /// </summary>
        public Socket? Socket_Client { set; get; }

        //public bool Client_Connect { set; get; }

        //private  byte[] buffer { set; get; } = new byte[1024 * 2048];
        /// <summary>
        /// 服务端累计维护的连接计数；接受连接时增加，部分断开/异常路径会减少。
        /// </summary>
        public int ConnectNumber { set; get; } = 0;

        /// <summary>
        /// 预留给界面展示的最近接收文本；当前收发主流程通过原始字节委托发布数据。
        /// </summary>
        public string Receive_Information { set; get; } = string.Empty;


        /// <summary>
        /// 预留给界面展示的最近发送文本；当前收发主流程通过原始字节委托发布数据。
        /// </summary>
        public string Send_Information { set; get; } = string.Empty;

        /// <summary>主动连接回调成功后释放 <see cref="Connect"/> 的等待。</summary>
        private readonly ManualResetEvent Timeout_Event = new ManualResetEvent(false);

        /// <summary>主动连接回调无论成功或失败都设置，确保超时分支可等待回调收尾。</summary>
        private readonly ManualResetEvent Timeout_End = new ManualResetEvent(false);


        /// <summary>
        /// 在限定时间内主动连接远程 TCP 服务器。
        /// </summary>
        /// <param name="_IP">远程服务器 IPv4 地址。</param>
        /// <param name="_Port">远程服务器端口文本。</param>
        /// <param name="TimeOut">等待连接成功的毫秒数。</param>
        /// <returns>在时限内完成连接返回 <see langword="true"/>，否则返回 <see langword="false"/>。</returns>
        /// <remarks>超时后仍会等待底层连接回调收尾，因此 <paramref name="TimeOut"/> 不是方法总耗时的硬上限。</remarks>
        public bool Connect(string _IP, string _Port, int TimeOut = 500)
        {
            try
            {
                // 两个事件都可能保留上一次调用的有信号状态，连接前必须同时复位。
                Timeout_Event.Reset();
                Timeout_End.Reset();

                // 当前实现明确创建 IPv4/TCP Socket，并关闭 Nagle 以降低小报文往返延迟。
                IPEndPoint ipe = new(IPAddress.Parse(_IP), int.Parse(_Port));

                Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket_Client.NoDelay = true;
                Socket_Client?.BeginConnect(ipe, new AsyncCallback(Client_Inf), Socket_Client);

                if (Timeout_Event.WaitOne(TimeOut))
                {
                    // 回调已确认 Connected，再把可用 Socket 和目标地址交给上层。
                    Socket_ConnectInfo_delegate?.Invoke($"IP：{_IP}，Port：{_Port}，连接服务器成功！", Socket_Client);




                    return true;

                }
                else
                {
                    // 超时消息沿用连接信息通道；等待回调 finally，避免连接尝试尚未收尾就返回。
                    Socket_ConnectInfo_delegate?.Invoke($"Error:-1,IP：{_IP}，Port：{_Port}，连接服务器超时退出！", Socket_Client);
                    //Socket_Client?.Shutdown(SocketShutdown.Both);
                    //Socket_Client?.Close();
                    //Socket_Client?.Dispose();
                    Timeout_End.WaitOne();

                    return false;

                }

            }
            catch (Exception e)
            {
                // 地址解析、Socket 创建和同步等待期间的异常统一在此关闭并释放客户端资源。
                Socket_ErrorInfo_delegate?.Invoke($"Error:-2,IP：{_IP}，Port：{_Port}，开启服务失败！原因：" + e.Message, Socket_Client);
                Socket_Client?.Close();
                Socket_Client?.Dispose();
                return false;

            }


        }



        /// <summary>
        /// 关闭并释放一个客户端连接（幂等，可重复调用）。
        /// 必须在错误委托之后调用，因为上层错误处理可能仍需读取 <see cref="Socket.RemoteEndPoint"/> 来匹配客户端。
        /// </summary>
        /// <param name="_client">要清理的服务端客户端状态；为空或已经清理时直接返回。</param>
        private void Close_Client(Receive_State? _client)
        {
            Socket? _s = _client?.Client_Socket;
            if (_s == null) return;

            // 先置空实现幂等；Shutdown、Close 或 Dispose 中任一步失败都不阻止后续清理。
            _client!.Client_Socket = null;
            try { _s.Shutdown(SocketShutdown.Both); } catch { }
            try { _s.Close(); } catch { }
            try { _s.Dispose(); } catch { }
        }



        /// <summary>
        /// 完成主动客户端的异步连接，并释放连接等待事件。
        /// </summary>
        /// <param name="ar">由 <c>Socket.BeginConnect</c> 传入的异步结果。</param>
        private void Client_Inf(IAsyncResult ar)
        {

            try
            {
                // Connected 为真时先释放成功等待者；EndConnect 随后确认并结束底层异步操作。
                if (Socket_Client?.Connected ?? false)
                {
                    Timeout_Event.Set();

                }
                Socket_Client?.EndConnect(ar!);

                //Task.Delay(10);
                //挂起读取异步连接
                //异步接收客户端
                //Socket_Client?.BeginAccept(new AsyncCallback(ClienAppcet), Socket_Client);

                //Client_Connect = true;

            }
            catch (Exception e)
            {
                // 失败时不设置成功事件，由 Connect 的超时/失败分支返回 false。
                Socket_ErrorInfo_delegate?.Invoke($"Error:-51 原因:" + e.Message, Socket_Client);
                //Client_Connect = false;
                //Socket_Client?.Close();
                //Socket_Client?.Dispose();

                //Socket_Client?.Shutdown(SocketShutdown.Both);



                return;
            }
            finally
            {
                // 无论连接结果如何，都通知 Connect 回调已经结束，防止超时分支永久等待。
                Timeout_End.Set();
            }


        }
        /// <summary>
        /// 在已连接的客户端 Socket 上发送一个业务请求，并同步等待、解析服务器回执。
        /// </summary>
        /// <typeparam name="T1">与 <paramref name="_Model"/> 对应的发送 DTO 类型。</typeparam>
        /// <param name="_Robot_Protocols">用于编码本次请求的机器人协议族。</param>
        /// <param name="_Model">本次请求的业务功能码。</param>
        /// <param name="_val">要序列化的业务数据。</param>
        /// <param name="TimeOut">同步接收回执的毫秒超时。</param>
        /// <remarks>
        /// 该方法采用“一次发送、一次接收”的请求/应答模式，调用前必须先成功执行 <see cref="Connect"/>；
        /// <paramref name="_Robot_Protocols"/> 还应与实例的 <see cref="Socket_Robot"/> 保持一致，否则请求和回执会按不同协议解释。
        /// </remarks>
        public void Send_Val<T1>(Socket_Robot_Protocols_Enum _Robot_Protocols, Vision_Model_Enum _Model, T1 _val, int TimeOut = 1000)
        {
            // 缓冲声明在 try 外，确保所有异常路径都能在 finally 中归还共享池。
            byte[]? buffer = null;
            try
            {
                // 发送方向已知功能码，因此构造协议对象时无需从报文头反推模式。
                Robot_Socket_Protocol _Socket_Protoco = new Robot_Socket_Protocol(_Robot_Protocols, _Model);
                Byte[] Send_byte = Array.Empty<byte>();

                // 从共享池租用临时缓冲，避免每次请求都分配大数组。
                buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(Receive_Buffer_Size);
                Socket_Client!.ReceiveTimeout = TimeOut;



                // 将 DTO 编码为所选机器人协议的完整请求帧。
                Send_byte = _Socket_Protoco.Socket_Send_Set_Data(_val ?? new object()) ?? Array.Empty<byte>();


                if (Send_byte != Array.Empty<byte>() && ((bool?)Socket_Client?.Connected ?? false))
                {
                    Send_State.Reset();

                    // 原始发送观察回调先于实际 Send，便于日志保持请求顺序。
                    Socket_Send_Meg?.Invoke(Send_byte);
                    Socket_Client?.Send(Send_byte);

                    // 同步接收与本请求对应的一次回执；ReceiveTimeout 负责终止无响应等待。
                    int length = Socket_Client?.Receive(buffer, buffer.Length, SocketFlags.None) ?? 0;


                    if (length == 0)
                    {
                        // TCP 返回 0 表示对端正常关闭，不能当作空业务回执继续解析。
                        throw new Exception("Error:-65,接受信息为空，重复发送");

                    }

                    // 只复制本次有效字节，防止池化缓冲剩余区域进入协议解析。
                    byte[] _Reveice_Meg = buffer.Skip(0).Take(length).ToArray();

                    // 回执方向由报文头决定，协议对象会先识别 Vision_Model 再反序列化。
                    Robot_Socket_Protocol _Socket_Protocol = new(Socket_Robot, _Reveice_Meg);

                    // 客户端当前只消费看板服务器回执；其他功能的客户端回执尚未在此分派。
                    switch (_Socket_Protocol.Vision_Model)
                    {


                        case Vision_Model_Enum.Mes_Server_Info_Rece_Data:


                            // 上位机到上位机看板链路：这里接收的是 Server 对 Client 上传包的回执。
                            Mes_Server_Info_Data_Send? _Mes_Server_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Mes_Server_Info_Data_Send>();

                            Mes_Receive_Info_Data_Delegate?.Invoke(_Mes_Server_Rece!);


                            break;


                    }


                    // 业务回调完成后再发布原始接收帧，保证观察者看到的是已处理数据。
                    Socket_Receive_Meg?.Invoke(_Reveice_Meg);

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
            finally
            {
                // 归还池化缓冲，避免高频请求产生大对象堆分配。
                if (buffer != null) System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
            }
        }




        /// <summary>
        /// 处理主动客户端模式下的异步回执。
        /// </summary>
        /// <param name="ar">AsyncState 需要是包含客户端 Socket 和缓冲的 <see cref="Receive_State"/>。</param>
        /// <remarks>
        /// 当前 <see cref="Send_Val{T1}"/> 使用同步 Receive，本方法为旧版 BeginReceive 调用保留。
        /// </remarks>
        private void Client_ReceiveMessage(IAsyncResult ar)
        {
            // 回执处理直接影响同步调用等待时间，因此历史实现提升了当前回调线程优先级。
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // 只有状态类型正确时才结束接收，避免把其他异步操作的状态误当客户端处理。
            if (ar!.AsyncState is Receive_State client)
            {
                try
                {
                    IPEndPoint? clientipe = (IPEndPoint)client.Client_Socket?.RemoteEndPoint!;
                    int length = client.Client_Socket?.EndReceive(ar) ?? 0;
                    if (length == 0)
                    {
                        // 主动客户端收到 0 字节表示服务器关闭了连接；先上报来源再结束回调。
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-4,{clientipe}: 断开连接! ", client.Client_Socket);
                        //client?.Close();
                        //client?.Dispose();
                        //Client_Connect = false;
                        return;
                    }


                    // 截取有效回执，并根据报文中的功能码选择 DTO。
                    byte[] _Reveice_Meg = client.buffer.Skip(0).Take(length).ToArray();
                    Robot_Socket_Protocol _Socket_Protocol = new(Socket_Robot, _Reveice_Meg);

                    // 当前客户端异步路径只处理看板服务器确认回执。
                    switch (_Socket_Protocol.Vision_Model)
                    {


                        case Vision_Model_Enum.Mes_Server_Info_Rece_Data:


                            // 上位机到上位机看板链路：这里接收的是 Server 对 Client 上传包的回执。
                            Mes_Server_Info_Data_Send? _Mes_Server_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Mes_Server_Info_Data_Send>();

                            Mes_Receive_Info_Data_Delegate?.Invoke(_Mes_Server_Rece!);


                            break;


                    }

                    // 业务回执已经消费，释放可能等待本次请求完成的线程并发布原始帧。
                    Send_State.Set();
                    Socket_Receive_Meg?.Invoke(_Reveice_Meg);
                }
                catch (Exception e)
                {
                    // 保留 Socket 供上层读取 RemoteEndPoint 并定位失败连接。
                    Socket_ErrorInfo_delegate?.Invoke("Error:-5," + e.Message, client.Client_Socket);
                    //client?.Close();
                    //client?.Dispose();

                    //断开连接
                    //WriteLine(clientipe + " is disconnected，total connects " + (connectCount), ConsoleColor.Red);
                }
            }

        }





        /// <summary>
        /// 创建监听 Socket 并启动异步接受客户端循环。
        /// </summary>
        /// <param name="_IP">要绑定的本地 IPv4 地址。</param>
        /// <param name="_Port">要绑定的 TCP 端口文本。</param>
        public void Server_Strat(string _IP, string _Port)
        {


            try
            {
                // 地址和端口在启动时解析，配置问题会由统一错误委托上报。
                IPEndPoint ipe = new(IPAddress.Parse(_IP), int.Parse(_Port));

                Socket_Sever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 服务重启时允许尽快重新绑定仍处于系统回收期的地址。
                Socket_Sever.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                //Socket_Sever.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.KeepAlive, true);
                //Socket_Sever.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.TcpKeepAliveTime, 30);

                // 绑定后用较大的待连接队列承接多机器人同时上线。
                Socket_Sever.Bind(ipe);
                Socket_Sever.Listen(100);

                // BeginAccept 每次只接受一个连接，回调末尾会再次挂起形成持续循环。
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
        /// 停止服务端监听并释放监听 Socket。
        /// </summary>
        public void Sever_End()
        {

            try
            {
                // Shutdown 终止挂起的收发；若监听 Socket 状态不允许 Shutdown，则由 catch 直接 Dispose。
                Socket_Sever?.Shutdown(SocketShutdown.Both);
                Socket_Sever?.Close();
            }
            catch (Exception)
            {

                Socket_Sever?.Dispose();

            }

        }



        /// <summary>
        /// 枚举本机可用于 IPv4 监听的地址，并追加回环地址。
        /// </summary>
        /// <param name="_IPAddress">成功时替换为新列表，而不是在原列表上追加。</param>
        /// <returns>枚举完成返回 <see langword="true"/>；失败会抛出带上下文的异常。</returns>
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

                // 无论 DNS 是否返回回环地址，都显式提供本机测试入口。
                _IPAddress.Add("127.0.0.1");

                return true;
            }
            catch (Exception _e)
            {

                throw new Exception("Error:-8,本地IP获取失败！，请检查网络配置。原因：" + _e.Message);

            }


        }


        /// <summary>
        /// 完成一次客户端接受，并按配置启动固定缓冲或流式 XML 接收循环。
        /// </summary>
        /// <param name="ar">AsyncState 中保存监听 Socket。</param>
        private void ClienAppcet(IAsyncResult ar)
        {
            try
            {
                // 计数在 EndAccept 前递增，沿用现有“接受尝试次数”语义。
                ConnectNumber++;
                Socket? ServerSocket = ar.AsyncState as Socket;
                Socket? client = ServerSocket?.EndAccept(ar);
                Receive_State _Receive = new Receive_State() { Client_Socket = client };
                if (null != ServerSocket)
                {
                    try
                    {

                        if (UseXmlFragmentReceive)
                        {
                            // 流式 XmlReader 负责跨多个 TCP 包拼出完整顶层元素。
                            _ = ReceiveXmlFragmentLoopAsync(_Receive);

                        }
                        else
                        {
                            // 固定缓冲模式假设一次 EndReceive 对应一帧完整业务报文。
                            client?.BeginReceive(_Receive.buffer, 0, _Receive.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), _Receive);

                        }    




                    }
                    catch (Exception e)
                    {
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-15" + e.Message, client);
                        // 接收循环无法启动时立即释放已接受的客户端，避免孤立连接堆积。
                        Close_Client(_Receive);
                        return;
                    }

                    Socket_ConnectInfo_delegate?.Invoke($"{ServerSocket.LocalEndPoint}:连接进来了", client);

                    Console.WriteLine("第" + ConnectNumber + "连接进来了");

                }



                // 当前客户端已交给独立接收流程，监听器立即继续接受下一连接。
                ServerSocket?.BeginAccept(new AsyncCallback(ClienAppcet), ServerSocket);
            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke($"Error:-16" + e.Message);


            }
        }






        /// <summary>
        /// 固定缓冲模式下完成一批服务端数据接收，并继续挂接下一次接收。
        /// </summary>
        /// <param name="ar">AsyncState 中保存该客户端独有的 <see cref="Receive_State"/>。</param>
        private void ReceiveMessage(IAsyncResult ar)
        {

            // 协议响应要求较低延迟，历史实现将回调线程提升为最高优先级。
            Thread.CurrentThread.Priority = ThreadPriority.Highest;






            if (ar!.AsyncState is Receive_State client)
            {
                try
                {
                    IPEndPoint clientipe = (IPEndPoint)client.Client_Socket?.RemoteEndPoint!;
                    int length = client.Client_Socket?.EndReceive(ar) ?? 0;
                    if (length == 0)
                    {
                        // 0 字节是 TCP 有序关闭信号；先通知业务层，再释放 Socket。
                        Socket_ErrorInfo_delegate?.Invoke($"Error:-9,{clientipe}: 断开连接! ", client.Client_Socket);
                        //client?.Close();
                        //client?.Dispose();
                        //Client_Connect = false;
                        Close_Client(client);
                        return;
                    }


                    // 先续挂接收，缩短处理当前报文期间的网络空窗。
                    client.Client_Socket?.BeginReceive(client.buffer, 0, client.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);

                    // 当前缓冲会被下一次接收复用，因此续挂后立即复制本帧有效数据，再进入较慢的业务处理。
                    byte[] _Reveice_Meg = client.buffer.Skip(0).Take(length).ToArray();

                    // 固定缓冲路径把这批字节视为完整报文，并进入统一业务分派。
                    ProcessReceivedMessage(client, _Reveice_Meg);

                }
                catch (Exception e)
                {
                    // 错误委托必须先执行，因为 Close_Client 后 RemoteEndPoint 将不可读取。
                    Socket_ErrorInfo_delegate?.Invoke("Error:-11," + e.Message, client.Client_Socket);

                    ConnectNumber--;
                    Close_Client(client);

                    //断开连接
                    //WriteLine(clientipe + " is disconnected，total connects " + (connectCount), ConsoleColor.Red);
                }
            }


        }

        /// <summary>
        /// 统一处理一帧完整的服务端入站报文：识别功能、反序列化、调用业务处理器并发送应答。
        /// </summary>
        /// <param name="client">报文来源客户端，用于发送响应和传给需要区分连接的业务委托。</param>
        /// <param name="_Reveice_Meg">已经截取到准确长度的完整协议报文。</param>
        /// <exception cref="Exception">功能码未知、协议不支持或未能生成应答时抛出。</exception>
        private void ProcessReceivedMessage(Receive_State client, byte[] _Reveice_Meg)
        {
            // 所有业务分支最终都把响应编码到同一缓冲，便于统一记录和发送。
            byte[] Send_byte = Array.Empty<byte>();

            // 构造协议对象时先从报文头推断功能码，后续泛型解析必须与该功能对应。
            Robot_Socket_Protocol _Socket_Protocol = new(Socket_Robot, _Reveice_Meg);

            // 各已实现分支遵循相同流水线：解码请求 -> 调业务委托 -> 编码返回 DTO。
            switch (_Socket_Protocol.Vision_Model)
            {
                case Vision_Model_Enum.Calibration_New:
                    // 旧标定新增协议只保留功能码，尚未接入 DTO 和业务处理器。
                    break;
                case Vision_Model_Enum.Calibration_Text:
                    // 旧标定测试协议尚未实现。
                    break;
                case Vision_Model_Enum.Calibration_Add:
                    // 旧标定追加协议尚未实现。
                    break;
                case Vision_Model_Enum.Find_Model:
                    // 机器人提交查找上下文，视觉业务返回最多八个识别点位。
                    Vision_Find_Data_Receive? _Vision_Find_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Find_Data_Receive>();

                    Vision_Find_Data_Send? _Vision_Find_Send = Vision_Find_Model_Delegate?.Invoke(_Vision_Find_Rece!);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data<Vision_Find_Data_Send>(_Vision_Find_Send ?? new Vision_Find_Data_Send()) ?? Array.Empty<byte>();

                    break;
                case Vision_Model_Enum.Vision_Ini_Data:
                    // 初始化请求通常只携带功能码，业务层返回视觉范围与允许偏差。
                    Vision_Ini_Data_Receive? _Vision_Ini_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Ini_Data_Receive>();

                    Vision_Ini_Data_Send? _Vision_Ini_Send = Vision_Ini_Data_Delegate?.Invoke(_Vision_Ini_Rece!);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Vision_Ini_Send ?? new Vision_Ini_Data_Send()) ?? Array.Empty<byte>();

                    break;
                case Vision_Model_Enum.HandEye_Calib_Date:
                    // 标定请求携带当前机器人位姿，业务层返回状态、消息和结果位姿。
                    HandEye_Calibration_Receive? _HandEye_Rece = _Socket_Protocol.Socket_Receive_Get_Date<HandEye_Calibration_Receive>();

                    HandEye_Calibration_Send? _Hand_Send = HandEye_Calibration_Data_Delegate?.Invoke(_HandEye_Rece!);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Hand_Send ?? new HandEye_Calibration_Send()) ?? Array.Empty<byte>();


                    break;

                case Vision_Model_Enum.Vision_Creation_Model:
                    // 建模请求携带相机点、原点和机器人类型，业务层返回创建结果。
                    Vision_Creation_Model_Receive? _Vision_Creation_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Vision_Creation_Model_Receive>();

                    Vision_Creation_Model_Send? __Vision_Creation_Send = Vision_Creation_Model_Data_Delegate?.Invoke(_Vision_Creation_Rece!);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data(__Vision_Creation_Send ?? new Vision_Creation_Model_Send()) ?? Array.Empty<byte>();

                    break;

                case Vision_Model_Enum.Mes_Info_Data:
                    // 机器人上传生产状态，业务层返回是否接受及下一次轮询周期。
                    Robot_Mes_Info_Data_Receive? _Mes_Info_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>();

                    Robot_Mes_Info_Data_Send? _Mes_Info_Send = Robot_Info_Model_Data_Delegate?.Invoke(_Mes_Info_Rece!);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Mes_Info_Send ?? new Robot_Mes_Info_Data_Send()) ?? Array.Empty<byte>();



                    break;


                case Vision_Model_Enum.Mes_Server_Info_Send_Data:
                    // 上位机到上位机看板链路：这里接收的是 Client 周期上传给 Server 的看板快照。
                    Mes_Server_Info_Data_Receive? _Mes_Server_Rece = _Socket_Protocol.Socket_Receive_Get_Date<Mes_Server_Info_Data_Receive>();

                    Mes_Server_Info_Data_Send? _Mes_Server_Send = Mes_Server_Info_Data_Delegate?.Invoke(_Mes_Server_Rece!, client.Client_Socket);

                    Send_byte = _Socket_Protocol.Socket_Send_Set_Data(_Mes_Server_Send ?? new Mes_Server_Info_Data_Send()) ?? Array.Empty<byte>();


                    break;

                case Vision_Model_Enum.Unknown:
                    // 未识别的头部不能安全选择 DTO，直接交给外层异常路径关闭或记录连接。
                    throw new Exception("Error:-9,现有通讯协议无法解析，请联系开发者！");




            }


            // 有应答时先通知监视器，再同步写回同一客户端，保持请求/响应日志顺序。
            if (Send_byte != Array.Empty<byte>())
            {
                Socket_Send_Meg?.Invoke(Send_byte);
                client.Client_Socket?.Send(Send_byte);
            }
            else
            {
                // 未实现的功能分支会保留空应答；显式报错而不是让机器人无限等待。
                throw new Exception("Error:-10,现有通讯协议无法解析，请联系开发者！");
            }

            // 原始入站帧在业务和应答完成后发布，表示这一帧已经成功处理。
            Socket_Receive_Meg?.Invoke(_Reveice_Meg);

        }

        /// <summary>
        /// 在一个客户端网络流上持续读取独立的顶层 XML 元素，并逐帧交给统一协议处理。
        /// </summary>
        /// <param name="client">拥有网络流的客户端状态。</param>
        /// <remarks>
        /// <see cref="ConformanceLevel.Fragment"/> 允许同一 TCP 连接连续传输多个无共同根节点的 XML 文档；
        /// <c>XElement.LoadAsync</c> 仅在当前根元素完整到达后返回，因此同时解决拆包与粘包问题。
        /// </remarks>
        private async Task ReceiveXmlFragmentLoopAsync(Receive_State client)
        {
            Socket socket = client.Client_Socket
                ?? throw new InvalidOperationException("客户端 Socket 为空。");

            try
            {
                // NetworkStream 不拥有 Socket，退出 using 后由 Close_Client 统一决定何时关闭连接。
                using NetworkStream stream = new(socket, ownsSocket: false);
                using XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings
                {
                    Async = true,
                    CloseInput = false,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    // 禁止 DTD 和外部解析器，避免网络报文触发外部实体访问。
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                });

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    // 子元素由 XElement.LoadAsync 随当前根节点一次性读取，不应单独作为报文处理。
                    if (reader.NodeType != XmlNodeType.Element || reader.Depth != 0)
                    {
                        continue;
                    }

                    // 只有完整根节点结束后 LoadAsync 才返回，半包会继续异步等待网络数据。
                    XElement element = await XElement.LoadAsync(
                        reader,
                        LoadOptions.PreserveWhitespace,
                        CancellationToken.None).ConfigureAwait(false);

                    // 统一重新编码成 UTF-8，供现有 Robot_Socket_Protocol 字节接口复用。
                    byte[] completeMessage = Encoding.UTF8.GetBytes(
                        element.ToString(SaveOptions.DisableFormatting));

                    ProcessReceivedMessage(client, completeMessage);
                }

                //Socket_ErrorInfo_delegate?.Invoke(
                //    "Error:-9,客户端断开连接！",
                //    socket);
            }
            catch (XmlException ex)
            {
                // XML 结构错误通常无法在同一流上可靠恢复，记录后关闭此客户端。
                Socket_ErrorInfo_delegate?.Invoke(
                    "Error:-11,XML 报文不完整或格式错误：" + ex.Message,
                    socket);
                Close_Client(client);

            }
            catch (Exception ex)
            {
                // 网络断开和业务处理异常共用清理路径，避免遗留客户端句柄。
                Socket_ErrorInfo_delegate?.Invoke(
                    "Error:-11," + ex.Message,
                    socket);
                Close_Client(client);

            }
 
        }








        // 以下代码是早期“传入整段 XML 字符串并在单方法内分派”的实现草稿。
        // 当前由 ProcessReceivedMessage + Robot_Socket_Protocol 取代，保留仅用于追溯旧协议设计，不参与编译。
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






        // 历史版本曾通过主动 GC 充当 Dispose；当前资源在 Sever_End/Close_Client 中显式释放。
        //public void Dispose()
        //{
        //    GC.Collect();
        //    GC.SuppressFinalize(this);
        //}
    }




    /// <summary>
    /// 提供机器人 XML 报文的无命名空间序列化与反序列化。
    /// DTO 上的 <see cref="XmlTypeAttribute"/> 和 <see cref="XmlAttributeAttribute"/> 决定线上节点名称与属性布局。
    /// </summary>
    public class KUKA_Send_Receive_Xml
    {
        /// <summary>创建无状态 XML 转换器。</summary>
        public KUKA_Send_Receive_Xml()
        {




        }



        /// <summary>
        /// 将 DTO 序列化为不带 XML 声明、命名空间前缀和缩进的紧凑 XML 字符串。
        /// </summary>
        /// <typeparam name="T1">DTO 的声明类型，必须与线上根节点契约一致。</typeparam>
        /// <param name="_Type">要序列化的请求或应答对象。</param>
        /// <returns>可继续按 UTF-8 编码发送的 XML 文本。</returns>
        public string Property_Xml<T1>(T1 _Type)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            // 机器人报文只需要根元素正文，省略声明和格式空白以缩短帧长度。
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = false;


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            // 注册空前缀/空命名空间，避免 XmlSerializer 输出 xsi/xsd 声明。
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
        /// 将完整 XML 报文反序列化为指定 DTO。
        /// </summary>
        /// <typeparam name="T1">目标 DTO 类型。</typeparam>
        /// <param name="_Path">包含单个完整根元素的 XML 文本。</param>
        /// <returns>由 XML 内容填充的 DTO 实例。</returns>
        public static T1 String_Xml<T1>(string _Path) where T1 : class
        {



            using (XmlReader xmlReader = XmlReader.Create(new StringReader(_Path)))
            {
                // 每种 DTO 使用自身的 XmlType/XmlAttribute 元数据解释报文。
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T1));




                T1 obj = (T1)xmlSerializer!.Deserialize(xmlReader)!;


                return obj;
            }


        }


    }
    /// <summary>
    /// 保存服务端单个客户端的 Socket、接收缓冲和最近接收长度，作为异步回调的状态对象。
    /// </summary>
    public class Receive_State
    {
        /// <summary>该状态所属的客户端 Socket；关闭后置为 <see langword="null"/> 防止重复清理。</summary>
        public Socket? Client_Socket { set; get; }

        /// <summary>
        /// 固定缓冲接收模式的每连接缓冲；大小由 <see cref="Socket_Receive.Receive_Buffer_Size"/> 统一控制。
        /// </summary>
        public byte[] buffer { set; get; } = new byte[Socket_Receive.Receive_Buffer_Size];

        /// <summary>预留的累计或最近接收长度；当前回调使用局部 length 变量。</summary>
        public int Receive_Length { set; get; } = 0;

    }

    /// <summary>
    /// 标识通信实例在调用场景中充当监听服务端还是主动客户端。
    /// </summary>
    public enum Socket_Type_Enum
    {
        /// <summary>监听并接受远程连接。</summary>
        Server,

        /// <summary>主动连接远程服务。</summary>
        Client
    }





}
