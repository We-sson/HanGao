using Roboto_Socket_Library.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;



namespace Roboto_Socket_Library
{
    /// <summary>
    /// 旧版通用 TCP 监听器，负责维护多个客户端连接并提供定向发送能力。
    /// 接收回调目前只保留扩展入口，不负责机器人协议解析；完整协议服务由 <see cref="Socket_Receive"/> 提供。
    /// </summary>
    public class Socket_Sever 
    {
        /// <summary>
        /// 使用指定本地地址创建服务器配置，但不立即开始监听。
        /// </summary>
        /// <param name="_IP">要绑定的本地 IPv4 地址。</param>
        /// <param name="_Port">要绑定的 TCP 端口文本。</param>
        public Socket_Sever(string _IP,string _Port)
        {
            // 构造阶段只解析并保存端点，让调用方有机会先订阅状态或配置其他属性。
            Address = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));

        }



        #region 字段


        #endregion

        #region 属性

        /// <summary>
        /// 指示监听器是否处于运行状态；异步回调据此决定是否继续接收。
        /// </summary>
        public  bool IsRuning { set; get; }

        /// <summary>
        /// 当前记录的客户端连接数量。
        /// </summary>
        public  int ClientCount { set; get; }

        /// <summary>
        /// 服务器要绑定的本地 IP 和端口。
        /// </summary>
        public   IPEndPoint Address { set; get; }

        /// <summary>
        /// 服务器监听 Socket；它只接受连接，不代表任何一个具体客户端。
        /// </summary>
        public  Socket? Socket_Server { set; get; }


        /// <summary>
        /// 已接受且尚未移除的客户端状态列表。
        /// </summary>
        public  List<Socket_Models_Server> KUKA_Client_List { set; get; } = new List<Socket_Models_Server>();



        #endregion






        #region 方法


        /// <summary>
        /// 创建监听 Socket、绑定 <see cref="Address"/> 并启动异步接受循环。
        /// </summary>
        /// <remarks>重复调用时，如果服务器已经运行，则不会再次绑定端口。</remarks>
        public void Robot_Socket_Server()
        {
            if (!IsRuning)
            {
                IsRuning = true;
                // ReuseAddress 便于服务重启后尽快重新绑定同一端口。
                Socket_Server = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket_Server?.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Socket_Server?.Bind(Address);

                // 待处理连接队列上限为 10；后续连接由接受回调继续排队。
                Socket_Server?.Listen(10);
                Socket_Server?.BeginAccept(new AsyncCallback(KUKA_Client_Connect), Socket_Server);



            }
        }


        /// <summary>
        /// 完成一次客户端接受，将连接加入状态列表，并为该客户端启动接收循环。
        /// </summary>
        /// <param name="ar">由 <c>Socket.BeginAccept</c> 传入的异步结果。</param>
        public void KUKA_Client_Connect(IAsyncResult ar)
        {
            if (IsRuning)
            {
                // AsyncState 是发起 BeginAccept 的监听 Socket。
                Socket _Server = (Socket)ar.AsyncState!;
                Socket _KUKA_Client = _Server.EndAccept(ar);


                Socket_Models_Server State = new Socket_Models_Server() { Server_Client = _KUKA_Client };
                lock (_KUKA_Client)
                {
                    // 先登记连接并准备与系统接收缓冲同样大小的应用层缓冲。
                    KUKA_Client_List.Add(State);
                    ClientCount++;
                    State.Server_Recv_Byte = new byte[_KUKA_Client.ReceiveBufferSize];
                }

                // 每个客户端维护独立的接收状态；监听 Socket 同时继续接受其他客户端。
                _KUKA_Client.BeginReceive(State.Server_Recv_Byte, 0, State.Server_Recv_Byte.Length, SocketFlags.None, new AsyncCallback(KUKA_Client_Received), State);
                Socket_Server?.BeginAccept(new AsyncCallback(KUKA_Client_Connect), Socket_Server);


            }

        }

        /// <summary>
        /// 完成一次客户端接收，处理有效数据，并重新挂起下一次接收。
        /// </summary>
        /// <param name="ar">AsyncState 中包含本次接收所属的 <see cref="Socket_Models_Server"/>。</param>
        public void KUKA_Client_Received(IAsyncResult ar)
        {
            if (IsRuning)
            {
                Socket_Models_Server State = (Socket_Models_Server)ar.AsyncState!;
                Socket _KUKA_Client = State.Server_Client!;

        

                lock (State)
                {
                    int Recv_Byte = _KUKA_Client.EndReceive(ar);
                    if (Recv_Byte == 0)
                    {
                        // TCP 返回 0 表示对端已正常关闭，必须移出列表，避免继续 BeginReceive。
                        ClientCount--;
                        KUKA_Client_Close(State);
                        return;
                    }

                    // 将业务处理集中到独立入口，便于派生或后续接入协议解析。
                    KUKA_Received_Val(State);


                }

                // APM 回调只处理一批字节，因此处理完后重新挂起，形成持续接收链。
                _KUKA_Client.BeginReceive(State.Server_Recv_Byte, 0, State.Server_Recv_Byte.Length, SocketFlags.None, new AsyncCallback(KUKA_Client_Received), State);


            }

        }


        /// <summary>
        /// 接收数据的业务扩展点。
        /// </summary>
        /// <param name="SM_Server">包含来源 Socket 与当前接收缓冲的客户端状态。</param>
        /// <remarks>当前实现仅读取远端地址，不消费缓冲内容；调用方若需要协议解析，应在此处扩展。</remarks>
        public void KUKA_Received_Val(Socket_Models_Server SM_Server)
        {
            // 读取 RemoteEndPoint 可用于日志或按客户端路由；变量保留给后续处理逻辑。
            var a = SM_Server.Server_Client!.RemoteEndPoint!.ToString();

            //MessageBox.Show(a + Encoding.ASCII.GetString(SM_Server.Server_Recv_Byte));

        }


        /// <summary>
        /// 向指定客户端异步发送一帧数据。
        /// </summary>
        /// <param name="Client">目标客户端 Socket。</param>
        /// <param name="Date">要发送的完整字节数组。</param>
        public void Server_Send(Socket Client, byte[] Date)
        {

            Client.BeginSend(Date, 0, Date.Length, SocketFlags.None, new AsyncCallback(Server_SendEnd), Client);

        }


        /// <summary>
        /// 完成异步发送，调用 <c>Socket.EndSend</c> 释放本次异步操作资源。
        /// </summary>
        /// <param name="ar">AsyncState 中保存了发送所用的客户端 Socket。</param>
        public void Server_SendEnd(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState!).EndSend(ar!);

        }






        /// <summary>
        /// 清空并移除一个客户端状态，然后关闭其 Socket。
        /// </summary>
        /// <param name="_Server">要关闭的客户端状态。</param>
        public   void KUKA_Client_Close(Socket_Models_Server _Server)
        {
            if (_Server != null)
            {
                // 先清理可见状态并移出集合，再执行网络关闭，避免后续逻辑继续选中该客户端。
                _Server.Server_Send_Data = string.Empty;
                _Server.Server_Recv_Byte = Array.Empty<byte>();

                KUKA_Client_List.Remove(_Server);

                _Server.Server_Closer();
            }


        }




        /// <summary>
        /// 停止监听，并关闭当前列表中的所有客户端连接。
        /// </summary>
        public  void Socket_Server_Stop()
        {



   
            
            if (IsRuning)
            {
                // 先翻转运行标志，阻止已排队的异步回调继续安排新的接收。
                IsRuning = false;
                ClientCount = 0;

                // 使用快照遍历，因为 KUKA_Client_Close 会同步修改原列表。
                foreach (var item in KUKA_Client_List.ToArray())
                {
                    KUKA_Client_Close(item);
                    
                }
  
                   
                // 监听 Socket 未建立收发会话，直接 Close 即可结束 Accept。
                Socket_Server?.Close();
                
            }


        }



        #endregion



    }
}
