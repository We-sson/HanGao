using Roboto_Socket_Library.Model;
using Roboto_Socket_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Throw;

namespace Roboto_Socket_Library
{
    /// <summary>
    /// 管理 KUKA 变量服务的 TCP 读写通信。
    /// 读取和写入使用两个独立 Socket，通过异步回调与等待事件协调“连接—发送—收包—解码”的顺序。
    /// </summary>
    /// <remarks>
    /// 该类型维护共享请求状态，不适合由多个外部线程同时执行批量读写；公开批量方法会在内部串行化操作。
    /// </remarks>
    public class Socket_Connect 
    {
        /// <summary>
        /// 创建尚未连接的 KUKA 变量通信对象；调用读写方法前需设置 <see cref="Connect_IP"/> 和 <see cref="Connect_Port"/>。
        /// </summary>
        public Socket_Connect()
        {

        }
        /// <summary>发生错误后用于唤醒关闭路径的信号。</summary>
        private ManualResetEvent Close_Waite { set; get; } = new ManualResetEvent(false);

        /// <summary>读取请求的异步发送完成信号。</summary>
        private ManualResetEvent Send_Read { set; get; } = new ManualResetEvent(false);

        /// <summary>写通道异步连接完成信号。</summary>
        private ManualResetEvent Connnect_Write { set; get; } = new ManualResetEvent(false);

        /// <summary>读通道异步连接完成信号。</summary>
        private ManualResetEvent Connnect_Read { set; get; } = new ManualResetEvent(false);

        /// <summary>写请求的异步发送完成信号。</summary>
        private ManualResetEvent Send_Write { set; get; } = new ManualResetEvent(false);

        /// <summary>写连接关闭路径的完成信号；保留给外部等待流程。</summary>
        private ManualResetEvent Rece_Write { set; get; } = new ManualResetEvent(false);

        /// <summary>任一请求收到完整响应后的信号。</summary>
        private ManualResetEvent Send_Waite { set; get; } = new ManualResetEvent(false);





        /// <summary>
        /// 读取响应解析完成时触发；参数同时包含变量值和调用方附带的业务上下文。
        /// </summary>
        public Socket_T_delegate<KUKA_SDK_Models>? Socket_Receive_Delegate { set; get; } 


        /// <summary>
        /// 总连接状态变化时触发。
        /// </summary>
        public Socket_T_delegate<bool>? Socket_Connect_State_delegate { set; get; }

        // 历史版本用于通知外部启动循环线程，当前通信流程不再调用。
        //public Socket_T_delegate<bool> Socket_CycleThread_delegate { set; get; }


        /// <summary>
        /// 连接、收发或协议解析失败时触发。
        /// </summary>
        public Socket_T_delegate<string>? Socket_ErrorInfo_delegate { set; get; }



        /// <summary>
        /// 旧版连接结果枚举；保留以兼容引用该嵌套类型的调用方。
        /// </summary>
        public enum Socket_Tpye
        {
            /// <summary>连接成功。</summary>
            Connect_OK,

            /// <summary>连接已取消或失败。</summary>
            Connect_Cancel,
        }

        /// <summary>
        /// 预留的通信耗时记录，单位由调用方约定；当前类不主动写入。
        /// </summary>
        public double Socket_Time { set; get; } = 0;

        // 共享连接状态的后备字段；只能通过属性 setter 更新，以保证状态委托同步触发。
        private bool _Is_Connect_Client = false;

        /// <summary>
        /// 表示当前批处理使用的连接是否可继续工作。
        /// 设置该值时会同步通知 <see cref="Socket_Connect_State_delegate"/>。
        /// </summary>
        public bool Is_Connect_Client
        {
            get => _Is_Connect_Client;
            set
            {
                _Is_Connect_Client = value;
                // 状态通知集中在 setter，确保连接、异常和关闭路径使用同一出口。
                Socket_Connect_State_delegate?.Invoke(value,null);

            }
        }





        /// <summary>
        /// KUKA 变量服务器的 IPv4 地址文本。
        /// </summary>
        public string Connect_IP { set; get; } = string.Empty;



        /// <summary>
        /// KUKA 变量服务器的 TCP 端口文本。
        /// </summary>
        public string Connect_Port { set; get; } = string.Empty;



        // 历史实现为写请求单独维护静态 ID；当前读写统一由 Val_Number_ID 或调用方提供 ID。
        //private static int _Write_Number_ID = 0;
        ///// <summary>
        ///// 写入变量唯一标识ID号
        ///// </summary>
        //public static int Write_Number_ID
        //{
        //    set
        //    {
        //        _Write_Number_ID = value;
        //    }
        //    get
        //    {
        //        if (_Write_Number_ID > 65500)
        //        {
        //            _Write_Number_ID = 0;
        //        }
        //        //bool a = false;


        //        _Write_Number_ID++;




        //        return _Write_Number_ID;
        //    }
        //}



        private int _Val_Number_ID;
        /// <summary>
        /// 获取下一个请求标识 ID。
        /// getter 本身会递增计数，并在超过 65500 后回绕到 1，以保留在两字节协议范围内。
        /// </summary>
        public int Val_Number_ID
        {
            set
            {
                _Val_Number_ID = value;
            }
            get
            {



                if (_Val_Number_ID > 65500)
                {
                    _Val_Number_ID = 0;
                }
                //do
                //{
                _Val_Number_ID++;

                //} while (Socket_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID) && On_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID));

                return _Val_Number_ID;
            }

        }






        // 历史读写锁方案；当前公开批量方法直接锁定 Socket_KUKA_Receive。
        //private ReaderWriterLockSlim Write_Lock { set; get; } = new ReaderWriterLockSlim();
        /// <summary>
        /// 当前正在执行的请求及共享接收缓冲。
        /// </summary>
        private KUKA_SDK_Models Socket_KUKA_Receive = new KUKA_SDK_Models();
        /// <summary>
        /// 专供写变量请求使用的 TCP Socket。
        /// </summary>
        public Socket Global_Socket_Write { set; get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// <summary>
        /// 专供循环读取或单次读取使用的 TCP Socket。
        /// </summary>
        public Socket Global_Socket_Read { set; get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 端点不缓存为属性，连接时根据最新的 IP/端口即时创建。
        //private IPEndPoint IP { set; get; } = new IPEndPoint(IPAddress.Parse(Connect_IP), int.Parse(Connect_Port));

        /// <summary>
        /// 旧版连接执行方式枚举；当前实现固定采用异步连接加同步等待。
        /// </summary>
        public enum Socket_Client_Type
        {
            /// <summary>
            /// 同步连接。
            /// </summary>
            Synchronized,
            /// <summary>
            /// 异步连接。
            /// </summary>
            Asynchronous,
            /// <summary>
            /// 由独立线程发起连接。
            /// </summary>
            Thread
        }



        /// <summary>
        /// 将请求 ID 编码为协议要求的两字节大端序数组。
        /// </summary>
        /// <param name="_ID">0 到 65535 范围内的协议标识。</param>
        /// <returns>高字节在前、低字节在后的两字节数组。</returns>
        private byte[] Send_number_ID(int _ID)
        {
            // 先格式化为固定四位十六进制，再按书写顺序切成两个字节，从而得到网络字节序。
            var arr = new byte[_ID.ToString("x4").Length / 2];

            for (var i = 0; i < arr.Length; i++)
                arr[i] = (byte)Convert.ToInt32((_ID.ToString("x4")).Substring(i * 2, 2), 16);


            return arr;
        }





        /// <summary>
        /// 按请求类型创建并连接对应的读或写 Socket。
        /// </summary>
        /// <param name="R_W_Enum">决定使用读通道还是写通道；<see cref="Read_Write_Enum.One_Read"/> 复用读通道。</param>
        /// <remarks>方法以异步方式发起连接，但会等待回调信号，因此对调用方表现为带超时的同步连接。</remarks>
        private void Socket_Client_KUKA(Read_Write_Enum R_W_Enum)
        {





            // 每次连接时解析最新配置，避免修改 Connect_IP/Connect_Port 后仍使用旧端点。
            IPEndPoint IP = new IPEndPoint(IPAddress.Parse(Connect_IP), int.Parse(Connect_Port));




            if (R_W_Enum == Read_Write_Enum.One_Read || R_W_Enum == Read_Write_Enum.Read)
            {
                // 读和单次读共用同一连接完成事件及 Socket。
                Connnect_Read.Reset();
                Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);




                // 循环读取可能用于设备启动阶段，因此允许 10 秒连接窗口。
                if (!Connnect_Read.WaitOne(10000, true) || !Is_Connect_Client)
                {
                    Socket_Receive_Error(R_W_Enum, "Error: -53 原因:读取连接超时！检查网络与IP设置是否正确。");
                    return;
                }







            }
            else if (R_W_Enum == Read_Write_Enum.Write)
            {
                // 写操作建立短连接，使用独立事件，避免读通道的回调误唤醒此处。
                Connnect_Write.Reset();
                Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);


                // 写入面向即时控制，当前协议约定只等待 1 秒。
                if (!Connnect_Write.WaitOne(1000, false) || !Is_Connect_Client)
                {
                    Socket_Receive_Error(R_W_Enum, "Error: -53 原因:写入连接超时！检查网络与IP设置是否正确。");
                    return;
                }



            }



        }




        /// <summary>
        /// 完成读/写 Socket 的异步连接，并唤醒发起连接的等待线程。
        /// </summary>
        /// <param name="ar">AsyncState 中保存发起连接时的 <see cref="Read_Write_Enum"/>。</param>
        /// <remarks>失败时先发布详细错误；外层等待将按各自超时路径完成清理。</remarks>
        private void Client_Inf(IAsyncResult ar)
        {
            // BeginConnect 只携带枚举值，回调据此选择正确的 Socket 和完成事件。
            Read_Write_Enum _Enum = Enum.Parse<Read_Write_Enum>(ar?.AsyncState?.ToString() ?? string.Empty) ;

            if (_Enum == Read_Write_Enum.Write)
            {



                try
                {
                    // EndConnect 既确认连接结果，也结束底层异步操作。
                    Global_Socket_Write.EndConnect(ar!);
                    Is_Connect_Client = true;




                }
                catch (Exception e)
                {


                    Socket_ErrorInfo_delegate?.Invoke($"Error: -51 原因:" + e.Message, Global_Socket_Write);


                    return;
                }

                // 只在 EndConnect 成功后释放写通道等待者。
                Connnect_Write.Set();

            }

            if (_Enum == Read_Write_Enum.Read || _Enum == Read_Write_Enum.One_Read)
            {
                try
                {
                    // 读取和单次读取都由同一 Socket 完成连接。
                    Global_Socket_Read.EndConnect(ar!);
                    Is_Connect_Client = true;

                }
                catch (Exception e)
                {


                    Socket_ErrorInfo_delegate?.Invoke($"Error: -50 原因:" + e.Message, Global_Socket_Read);
                    return;
                }
                // 通知 Socket_Client_KUKA 不必继续等待连接。
                Connnect_Read.Set();


            }
            
        }






        /// <summary>
        /// 完成一次 KUKA 响应接收，解析协议帧并发布读取结果。
        /// </summary>
        /// <param name="ar">AsyncState 中保存发送该请求时的 <see cref="KUKA_SDK_Models"/>。</param>
        private void Socke_Receive_Message(IAsyncResult ar)
        {

            try
            {




                lock (ar)
                {
                    // 取回请求上下文，后面把解码值写入该对象并通过委托返回。
                    KUKA_SDK_Models _Receive = (ar?.AsyncState as KUKA_SDK_Models) ?? new KUKA_SDK_Models(); ;

                    if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
                    {
                        // 接收在发送前就已挂起；若响应极快，先等待发送回调完成再结束接收操作。
                        Send_Read.WaitOne(10000);
                        Socket_KUKA_Receive.Byte_Leng = Global_Socket_Read.EndReceive(ar!);

                        if (Socket_KUKA_Receive.Byte_Leng == 0)
                        {
                            // EndReceive 返回 0 表示服务器已关闭读连接。
                            Socket_Receive_Error(Socket_KUKA_Receive.Read_Write_Type, "Error: -20 原因:" + GetType().Name + " 写入线程，库卡服务器断开！");

                            return;
                        }

                    }

                    if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
                    {
                        // 写请求的应答从独立写 Socket 读取，避免与周期读响应互相串包。
                        Socket_KUKA_Receive.Byte_Leng = Global_Socket_Write.EndReceive(ar!);
                        if (Socket_KUKA_Receive.Byte_Leng == 0)
                        {
                            // 写连接被对端关闭时走统一错误和关闭路径。
                            Socket_Receive_Error(Socket_KUKA_Receive.Read_Write_Type, "Error: -20 原因:" + GetType().Name + " 写入线程，库卡服务器断开！");

                            return;

                        }
                        // Rece_IA_Lock.ReleaseMutex();

                    }







                    if (Socket_KUKA_Receive.Byte_Leng > 0)
                    {
                        // 依据通道选择有效缓冲，并按 KUKA 帧布局解出 ID、长度、值和结果标志。
                        if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
                        {

                            //_Byte.Byte_data = Socket_KUKA_Receive.Byte_Read_Receive;
                            Real_Byte_To_Var(ref Socket_KUKA_Receive);
                            // Socket_KUKA_Receive.Byte_Read_Receive = new byte[1024 * 1024];
                        }
                        else if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
                        {
                            // _Byte.Byte_data = Socket_KUKA_Receive.Byte_Write_Receive;
                            Real_Byte_To_Var(ref Socket_KUKA_Receive);

                            // Socket_KUKA_Receive.Byte_Write_Receive = new byte[1024 * 1024];
                        }



                        // 只有读请求包含需要回传给业务层的变量值；写请求只检查成功标志。
                        if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
                        {

                            //_Receive.Reveice_Inf.Val_Var = Socket_KUKA_Receive.Receive_Byte.Message_Show;

                            _Receive.Receive_Var = Socket_KUKA_Receive.Receive_Byte.Message_Show;
                            // 回传最初请求对象，使业务层可通过 Reveice_Inf 知道该值属于哪个变量。
                            Socket_Receive_Delegate?.Invoke(_Receive,null);

                        }
                    }



                    if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
                    {
                        // 写连接按批次使用；此处标记当前单次交互已结束并唤醒发送方。
                        Is_Connect_Client = false;
                        Send_Waite.Set();

                    }

                    if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
                    {
                        // 允许循环读取方法继续处理列表中的下一个变量。
                        Send_Waite.Set();

                    }



                }
            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke(e.Message);

            }
        }




        /// <summary>
        /// 在正确通道上预先挂接响应接收，再异步发送一帧 KUKA 请求。
        /// </summary>
        /// <param name="_S">包含完整发送帧、请求类型及业务上下文的请求状态。</param>
        /// <remarks>
        /// 先调用 BeginReceive 再 BeginSend，可避免本机发送后服务器立即回包而接收尚未就绪。
        /// 方法会等待发送或响应信号，批量调用因此保持请求与响应一一对应。
        /// </remarks>
        private void Socket_Send_Message_Method(KUKA_SDK_Models _S)
        {

            try
            {



                lock (_S)
                {
                    // Send_Byte 已由读/写组帧方法生成，此处不再修改线上内容。
                    Byte[] Message = _S.Send_Byte;

                    if (_S.Read_Write_Type == Read_Write_Enum.Write && Global_Socket_Write.Connected == true)
                    {
                        // 清除上一次请求留下的信号，保证当前请求确实等到自己的回调。
                        Send_Write.Reset();
                        Send_Waite.Reset();

                        // 先监听写应答，再短暂让出时间片后发出请求。
                        Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), _S);

                        Thread.Sleep(10);

                        // AsyncState 传 Socket，发送完成回调据此设置 Send_Write。
                        Global_Socket_Write.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Write);


                        if (!Send_Waite.WaitOne(1000) && !Send_Write.WaitOne(1000))
                        {
                            // 当前条件要求“既没收到响应、也没完成发送”才判为超时。
                            Socket_Receive_Error(_S.Read_Write_Type, "Error: -54 原因:写入连接超时！检查网络与IP设置是否正确。");

                            return;
                        }

                    }
                    if ((_S.Read_Write_Type == Read_Write_Enum.Read || _S.Read_Write_Type == Read_Write_Enum.One_Read) && Global_Socket_Read.Connected == true)
                    {
                        // 读请求同样清除发送和响应信号，避免前一变量的完成状态串到当前变量。
                        Send_Read.Reset();
                        Send_Waite.Reset();

                        // 挂接读响应后再发送变量读取帧。
                        Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), _S);

                        Thread.Sleep(15);

                        Global_Socket_Read.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Read);

                        if (!Send_Waite.WaitOne(150000) && !Send_Read.WaitOne(1500000))
                        {
                            // 读取允许设备侧执行较长任务；超时后统一关闭读连接并终止循环。
                            Socket_Receive_Error(Read_Write_Enum.Read, "接收超时无应答，退出线程发送！");
                            return;
                        }

                    }

                }

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke(e.Message);

            }

        }


        /// <summary>
        /// 完成异步发送，并释放对应通道的发送等待信号。
        /// </summary>
        /// <param name="ar">AsyncState 是发起发送的读或写 Socket。</param>
        private void Socket_Send_Message(IAsyncResult ar)
        {


            if (Global_Socket_Write == (Socket?)ar.AsyncState)
            {
                // EndSend 完成本次写通道异步操作，并让等待者知道字节已交给系统。
                Global_Socket_Write.EndSend(ar);

                //释放发送等待状态
                Send_Write.Set();

            }

            if (Global_Socket_Read == (Socket?)ar.AsyncState)
            {
                // 读请求发送完成与响应完成是两个独立信号。
                Global_Socket_Read.EndSend(ar);

                //释放发送完成等待
                Send_Read.Set();

            }


        }



        /// <summary>
        /// 在一个短期写连接中依次写入一批机器人变量。
        /// </summary>
        /// <param name="Sml">按发送顺序排列的变量名、值、ID 和业务上下文集合。</param>
        public void Cycle_Write_Send(List<Socket_SendInfo_Model> Sml)
        {

            try
            {



                lock (Socket_KUKA_Receive)
                {
                    // 整批数据只建立一次写连接，以降低频繁握手开销。
                    Socket_Client_KUKA(Read_Write_Enum.Write);


                    if (Global_Socket_Write.Connected || Is_Connect_Client)
                    {

                        foreach (var item in Sml)
                        {
                            // 每个变量生成独立请求帧，并保留业务上下文用于错误定位。
                            Socket_KUKA_Receive = new KUKA_SDK_Models() { Send_Byte = Write_Var_To_Byte(item.Write_Var, item.Var_Name, item.Var_ID), Read_Write_Type = Read_Write_Enum.Write, Reveice_Inf = item.Reveice_Inf };
                            Socket_Send_Message_Method(Socket_KUKA_Receive);
                            // 等待本项发送回调，防止下一项覆盖共享请求状态。
                            Send_Write.WaitOne(5000);
                        }

                        // 全部写请求结束后关闭短期写通道。
                        Socket_Close(Read_Write_Enum.Write);
                    }

                    //Write_Lock.ExitWriteLock();

                }



            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke(e.Message);

            }
        }







        /// <summary>
        /// 建立一次读连接，按顺序读取一批变量，然后关闭连接。
        /// </summary>
        /// <param name="Sml">本批次需要读取的变量集合。</param>
        public void Cycle_Real_Send(List<Socket_SendInfo_Model> Sml)
        {


            //加锁
            try
            {


                lock (Socket_KUKA_Receive)
                {
                    // One_Read 表示整批读取后关闭，而不是持续轮询。
                    Socket_Client_KUKA(Read_Write_Enum.One_Read);



                    if (Global_Socket_Read.Connected)
                    {

                        // Socket_Send_Message_Method 会等待当前响应，因此集合按给定顺序串行读取。
                        foreach (var item in Sml)
                        {
                            // 组装变量读取帧并携带业务上下文供响应委托使用。
                            Socket_KUKA_Receive = new KUKA_SDK_Models() { Send_Byte = Read_Var_To_Byte(item.Var_Name, item.Var_ID), Read_Write_Type = Read_Write_Enum.One_Read, Reveice_Inf = item.Reveice_Inf };

                            Socket_Send_Message_Method(Socket_KUKA_Receive);


                        }



                        // 一批读取完成，主动释放本次读连接。
                        Socket_Close(Read_Write_Enum.One_Read);
                    }

                }
            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke(e.Message);

            }
        }

        /// <summary>
        /// 在持久读连接上循环轮询变量列表，直到连接状态被置为失败或外部关闭。
        /// </summary>
        /// <param name="Socket_Read_List">每轮按顺序读取的变量集合。</param>
        public void Loop_Real_Send(List<Socket_SendInfo_Model> Socket_Read_List)
        {
            //加锁

            try
            {


                lock (Socket_KUKA_Receive)
                {
                    // 清除上一批请求状态后，只为整个循环建立一次读连接。
                    Socket_KUKA_Receive = new KUKA_SDK_Models();
                    Socket_Client_KUKA(Read_Write_Enum.Read);



                    while (Is_Connect_Client)
                    {
                        // 记录轮询起点供历史耗时统计扩展；当前实现尚未使用该值。
                        DateTime timeB = DateTime.Now;  //获取当前时间



                        // 一轮内逐项请求，响应回调完成后才进入下一项。
                        foreach (var item in Socket_Read_List)
                        {


                            // 将变量描述封装成网络请求状态。
                            Socket_KUKA_Receive = new KUKA_SDK_Models() { Send_Byte = Read_Var_To_Byte(item.Var_Name, item.Var_ID), Read_Write_Type = Read_Write_Enum.Read, Reveice_Inf = item.Reveice_Inf };
                            if (Is_Connect_Client)
                            {


                                Socket_Send_Message_Method(Socket_KUKA_Receive);

                            }


                            if (!Is_Connect_Client)
                            {
                                // 响应或发送失败会翻转状态；立即关闭读 Socket，避免继续遍历失效连接。
                                Socket_Close(Read_Write_Enum.One_Read);
                            }

                        }


                    }





                }

            }
            catch (Exception e)
            {

                Socket_ErrorInfo_delegate?.Invoke(e.Message);

            }


        }

        /// <summary>
        /// 按 KUKA 变量协议解析当前请求的响应帧。
        /// </summary>
        /// <param name="Smr">包含有效接收长度和通道缓冲的请求状态；解析结果写回其 <c>Receive_Byte</c>。</param>
        /// <remarks>
        /// 布局为：请求 ID(2) + 数据总长(2) + 操作类型(1) + 值长度(2) + 值(N) + 写入结果(1)。
        /// 多字节整数按协议的大端书写顺序解析。
        /// </remarks>
        private void Real_Byte_To_Var(ref KUKA_SDK_Models Smr)
        {
            // 只复制 EndReceive 报告的有效部分，避免旧缓冲内容参与本次解析。
            if (Smr.Read_Write_Type == Read_Write_Enum.Read || Smr.Read_Write_Type == Read_Write_Enum.One_Read)
            {

                Smr.Receive_Byte.Byte_data = Smr.Byte_Read_Receive.Skip(0).Take(Smr.Byte_Leng).ToArray();
            }
            else
            {
                Smr.Receive_Byte.Byte_data = Smr.Byte_Write_Receive.Skip(0).Take(Smr.Byte_Leng).ToArray();
            }


            // [0..1] 请求 ID，用于核对响应属于哪个请求。
            Smr.Receive_Byte.Byte_ID = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(0).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            // [2..3] 从操作类型开始计算的协议数据总长度。
            Smr.Receive_Byte.Byte_Val_Total_Length = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(2).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            // [4] 返回的操作类型：协议值用于区分读/写响应。
            Smr.Receive_Byte.Byte_Return_Tpye = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(4).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            // [5..6] 后续变量文本的长度。
            var b = Smr.Receive_Byte.Byte_data.Skip(5).Take(2).ToArray();
            var bb = BitConverter.ToString(b).Replace("-", "");
            Smr.Receive_Byte.Byte_Val_Length = Int32.Parse(bb, System.Globalization.NumberStyles.HexNumber);

            // [7..] KUKA 服务以 ASCII 返回变量值或错误说明。
            Smr.Receive_Byte.Message_Show = Encoding.ASCII.GetString(Smr.Receive_Byte.Byte_data, 7, Smr.Receive_Byte.Byte_Val_Length);


            //MessageBox.Show(Smr.Receive_Byte.Message_Show);

            // 最后一个状态字节位于“总长度 + 3”处（前面还有 2 字节 ID 和长度字段的一部分）。
            Smr.Receive_Byte.Byte_Write_Type = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(Smr.Receive_Byte.Byte_Val_Total_Length + 3).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);



            if (Smr.Receive_Byte.Byte_Return_Tpye == 1 && Smr.Receive_Byte.Byte_Write_Type == 1)
            {
                // 写入成功无需向错误委托发布消息，调用方由正常完成信号继续。
            }
            else if (Smr.Receive_Byte.Byte_Return_Tpye == 1 && Smr.Receive_Byte.Byte_Write_Type == 0)
            {
                // 协议明确返回写失败时，同时上报设备消息和人类可读说明。
                Socket_ErrorInfo_delegate?.Invoke(Smr.Receive_Byte.Message_Show);
                Socket_ErrorInfo_delegate?.Invoke(" 变量值写入失败！");


            }



        }


        /// <summary>
        /// 将变量读取请求编码成 KUKA 二进制协议帧。
        /// </summary>
        /// <param name="Val_Name">机器人端变量名称。</param>
        /// <param name="Val_ID">用于匹配响应的请求 ID。</param>
        /// <returns>可直接写入 TCP 流的完整读取帧。</returns>
        private byte[] Read_Var_To_Byte(string Val_Name, int Val_ID)
        {





            // 变量名使用系统默认编码，以保持与既有机器人端配置兼容。
            List<byte> _data = new List<byte>();
            byte[] _v = Encoding.Default.GetBytes(Val_Name);


            // 线上字段顺序是协议契约，不能调整：ID | 总长 | 读标志 | 名称长 | 名称 | 结束位。
            // 请求唯一标识（2 字节，大端）。
            _data.AddRange(Send_number_ID(Val_ID));
            // 负载总长 = 操作标志 1 + 名称长度字段 2 + 名称 N。
            _data.AddRange(Send_number_ID(_v.Length + 3));
            // 0x00 表示读取变量。
            _data.AddRange(new byte[1] { 0x00 });
            // 变量名长度（2 字节）。
            _data.AddRange(Send_number_ID(_v.Length));
            // 变量名正文。
            _data.AddRange(_v);
            // 0x00 是协议帧结束标志。
            _data.AddRange(new byte[1] { 0x00 });


            return _data.ToArray();




        }

        /// <summary>
        /// 将变量写入请求编码成 KUKA 二进制协议帧。
        /// </summary>
        /// <param name="Write_Value">要写入的文本值。</param>
        /// <param name="Val_Name">机器人端变量名称。</param>
        /// <param name="Val_ID">用于匹配响应的请求 ID。</param>
        /// <returns>可直接写入 TCP 流的完整写请求帧。</returns>
        private byte[] Write_Var_To_Byte(string Write_Value, string Val_Name, int Val_ID)
        {





            // 名称和值沿用系统默认编码，与读取请求和机器人端保持一致。
            List<byte> _data = new List<byte>();
            byte[] _v = Encoding.Default.GetBytes(Write_Value);
            byte[] _n = Encoding.Default.GetBytes(Val_Name);

            // 固定布局：ID | 总长 | 写标志 | 名称长 | 名称 | 值长 | 值 | 结束位。
            // 请求唯一标识（2 字节，大端）。
            _data.AddRange(Send_number_ID(Val_ID));
            // 负载总长 = 标志 1 + 两个长度字段 4 + 名称 N + 值 M。
            _data.AddRange(Send_number_ID(_n.Length + _v.Length + 5));
            // 0x01 表示写入变量。
            _data.AddRange(new byte[1] { 0x01 });
            // 变量名长度和正文。
            _data.AddRange(Send_number_ID(_n.Length));
            _data.AddRange(_n);
            // 写入值长度和正文。
            _data.AddRange(Send_number_ID(_v.Length));
            _data.AddRange(_v);
            // 协议帧结束标志。
            _data.AddRange(new byte[1] { 0x00 });



            // 返回连续帧，实际发送由 Socket_Send_Message_Method 负责。
            return _data.ToArray();


        }



        /// <summary>
        /// 按请求类型关闭读或写 Socket，并同步更新等待状态。
        /// </summary>
        /// <param name="_Enum">决定关闭读通道、单次读通道或写通道。</param>
        public void Socket_Close(Read_Write_Enum _Enum)
        {

            if (_Enum == Read_Write_Enum.Read)
            {
                // 持久读取被显式结束时向上层发布提示并翻转循环条件。
                Socket_ErrorInfo_delegate?.Invoke("断开读取连接");

                if (Global_Socket_Read.Connected)
                {

                    //Close_Waite.WaitOne();

                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                    Global_Socket_Read.Close();

                }

                //断开读取连接
                //读取标识重置
                Is_Connect_Client = false;


            }


            if (_Enum == Read_Write_Enum.One_Read)
            {
                // 单次读结束只关闭共用读 Socket，不发布“断开循环读取”消息。
                if (Global_Socket_Read.Connected)
                {
                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                    Global_Socket_Read.Close();

                }


            }






            if (_Enum == Read_Write_Enum.Write)
            {
                // 写批次结束后先停止继续发送，再释放写 Socket。
                Is_Connect_Client = false;

                if (Global_Socket_Write.Connected)
                {

                    //关闭写入连接，重置标识
                    Global_Socket_Write.Shutdown(SocketShutdown.Both);
                    Global_Socket_Write.Close();
                }

                // 唤醒可能仍在等待写关闭完成的调用方。
                Rece_Write.Set();

            }
        }



        /// <summary>
        /// 发布通信错误、解除关闭等待，并关闭发生错误的通道。
        /// </summary>
        /// <param name="_Enum">发生错误的读写通道。</param>
        /// <param name="_Error">连接失败原因输入</param>
        public void Socket_Receive_Error(Read_Write_Enum _Enum, string _Error)
        {
            Socket_ErrorInfo_delegate?.Invoke(_Error);
            // Reset/Set 形成一次明确的错误完成脉冲，同时保持 ManualResetEvent 为可通过状态。
            Close_Waite.Reset();
            Close_Waite.Set();

            // 所有错误统一通过 Socket_Close 清理，避免各回调重复实现资源释放。
            Socket_Close(_Enum);
                
        }

        // 历史版本曾暴露空 Dispose；当前调用方应通过 Socket_Close 明确关闭读写通道。
        //public void Dispose()
        //{
        //    //GC.Collect();
        //    //GC.SuppressFinalize(this);
        //}
    }


}










