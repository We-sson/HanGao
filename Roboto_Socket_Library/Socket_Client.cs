using Roboto_Socket_Library.Model;
using System.Net;
using System.Net.Sockets;

namespace Roboto_Socket_Library
{
    /// <summary>
    /// 提供最小化的同步 TCP 客户端连接能力。
    /// 该类型只负责建立连接和发布连接结果，不包含协议编码、收发循环或自动重连逻辑。
    /// </summary>
    public class Socket_Client_Model
    {
        /// <summary>
        /// 创建使用默认地址 <c>127.0.0.1:5000</c> 的客户端配置。
        /// </summary>
        public Socket_Client_Model()
        {


        }

        /// <summary>
        /// 保留给旧调用方的兼容构造函数。
        /// 当前实现不会把参数写入 <see cref="IP"/> 和 <see cref="Port"/>；连接前仍需显式设置这两个成员。
        /// </summary>
        /// <param name="_IP">预期的服务器 IP（当前未使用）。</param>
        /// <param name="_Port">预期的服务器端口（当前未使用）。</param>
        public Socket_Client_Model(string _IP, int _Port)
        {


        }

        /// <summary>
        /// 要连接的服务器 IPv4 地址文本。
        /// </summary>
        public string IP = "127.0.0.1";

        /// <summary>
        /// 要连接的服务器 TCP 端口。
        /// </summary>
        public int Port = 5000;

        /// <summary>
        /// 已创建的客户端 Socket；连接失败时可能仍为已关闭的实例。
        /// </summary>
        public Socket? Socket_Client { set; get; }



        /// <summary>
        /// 通讯连接错误委托
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }

        /// <summary>
        /// 连接成功时触发，消息中包含目标地址，并将已连接 Socket 一并传给调用方。
        /// </summary>
        public Socket_T_delegate<string>? Socket_ConnectInfo_delegate { set; get; }



        /// <summary>
        /// 记录最近一次 <see cref="Connect"/> 调用是否成功。
        /// </summary>
        public bool IsConnect { set; get; } = false;





        /// <summary>
        /// 使用当前 <see cref="IP"/> 和 <see cref="Port"/> 同步连接服务器。
        /// </summary>
        /// <returns>连接成功返回 <see langword="true"/>；解析地址或连接失败返回 <see langword="false"/>。</returns>
        /// <remarks>该调用会阻塞当前线程，超时策略由操作系统的同步 Socket 连接行为决定。</remarks>
        public bool Connect()
        {
            try
            {
                // 先将文本配置转换成网络端点；格式错误与连接错误统一由下方 catch 上报。
                IPAddress _IP = IPAddress.Parse(IP);
                IPEndPoint iPEndPoint = new IPEndPoint(_IP, Port);

                // 每次调用都创建新的 IPv4/TCP Socket，成功后把实例交给后续业务继续收发。
                Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket_Client.Connect(iPEndPoint);
                IsConnect = true;


                Socket_ConnectInfo_delegate?.Invoke($"IP：{IP}，Port：{Port}，连接服务器成功！", Socket_Client);
                return true;

            }
            catch (Exception e)
            {
                // 先通知上层失败原因，再关闭本次创建的 Socket，确保状态与实际连接一致。
                Socket_ErrorInfo_delegate?.Invoke($"IP：{IP}，Port：{Port}，开启服务失败！原因：" + e.Message, Socket_Client);
                Socket_Client?.Close();
                IsConnect = false;
                return false;

            }


        }













    }
}
