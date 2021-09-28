using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.Socket_KUKA;
using 悍高软件.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;
using static 悍高软件.ViewModel.UserControl_Socket_Setup_ViewModel;
using static 悍高软件.ViewModel.UserControl_Socket_Var_Show_ViewModel;



namespace Soceket_Connect
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Connect : ViewModelBase
    {



        public Socket_Connect(string _IP, string _Port, Connect_Type _Type, Read_Write_Enum _Enum)
        {

            //实例初始化
            //IP = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));

            if (_Type == Connect_Type.Long)
            {
                if (Socket_Connect_Thread == null)
                {
                    Socket_Client_Thread(_Enum, _IP, _Port);

                }
            }
        }






        public static ManualResetEventSlim Thread_Read { set; get; } = new ManualResetEventSlim(false);

        public static ManualResetEvent Close_Waite { set; get; } = new ManualResetEvent(false);
        public static ManualResetEventSlim Quit_Waite { set; get; } = new ManualResetEventSlim(false);


        //public static ManualResetEventSlim Connnect_Read { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEvent Socket_Read { set; get; } = new ManualResetEvent(false);
        public static ManualResetEventSlim Send_Read { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Rece_Read { set; get; } = new ManualResetEventSlim(false);




        public static ManualResetEvent Connnect_Write { set; get; } = new ManualResetEvent(false);
        public static ManualResetEvent Send_Write { set; get; } = new ManualResetEvent(false);
        public static ManualResetEvent Rece_Write { set; get; } = new ManualResetEvent(false);
        public static ManualResetEvent Connect_IA = new ManualResetEvent(false);
        public static ManualResetEvent Send_IA = new ManualResetEvent(false);





        public static ManualResetEvent Send_Waite { set; get; } = new ManualResetEvent(false);



        /// <summary>
        /// 连接线程设置
        /// </summary>
        public static Thread Socket_Connect_Thread { set; get; }
        /// <summary>
        /// 连接线程设置
        /// </summary>
        public Thread Socket_Write_Thread { set; get; }


        /// <summary>
        /// 长连接启动属性
        /// </summary>
        public bool Socket_Long_Connect { set; get; } = false;




        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public volatile bool Is_Write_Client = false;


        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public volatile bool Is_Read_Client = false;

        /// <summary>
        /// 集合读取允许
        /// </summary>
        public bool Read_List { set; get; } = false;

        //public byte[] _data { set; get; }= Array.Empty<byte>();


        /// <summary>
        /// Socket连接ip和端口属性
        /// </summary>
        public Socket_Models_Connect Socket_Client { set; get; } = new Socket_Models_Connect() { };





        /// <summary>
        /// 写入锁
        /// </summary>
        public static ReaderWriterLockSlim Send_Lock = new ReaderWriterLockSlim();

        /// <summary>
        /// 断口连接独占锁
        /// </summary>
        public static ReaderWriterLockSlim Quit_Lock = new ReaderWriterLockSlim();
        /// <summary>
        /// 写入回调锁
        /// </summary>
        public static Mutex Send_IA_Lock = new Mutex();


        /// <summary>
        /// 接收回调锁
        /// </summary>
        public static Mutex Rece_IA_Lock = new Mutex();

        public static object _locker = new object();

        /// <summary>
        /// 读取锁
        /// </summary>
        public static Mutex Connect_Lock = new Mutex();


        /// <summary>
        /// 消息通道传输属性
        /// </summary>
        public Socket_Models_Send Socket_Send_Attribute { set; get; }

        /// <summary>
        /// 异步接受属性
        /// </summary>
        public Socket_Models_Receive Socket_KUKA_Receive { set; get; }



        /// <summary>
        /// Socket唯一写入连接标识
        /// </summary>
        public Socket Global_Socket_Write { set; get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);



        /// <summary>
        /// Socket唯一读取连接标识
        /// </summary>
        public Socket Global_Socket_Read { set; get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// IP设置属性
        /// </summary>
        public IPEndPoint IP { set; get; }

        /// <summary>
        /// 输入ID号返回对应byte组
        /// </summary>
        public byte[] Send_number_ID(int _ID)
        {

            var arr = new byte[_ID.ToString("x4").Length / 2];

            for (var i = 0; i < arr.Length; i++)
                arr[i] = (byte)Convert.ToInt32((_ID.ToString("x4")).Substring(i * 2, 2), 16);


            return arr;
        }


        /// <summary>
        /// 使用多线程连接
        /// </summary>
        /// <param name="_Enum"></param>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        public void Socket_Client_Thread(Read_Write_Enum _Enum, string _IP, string _Port)
        {
            if (_Enum == Read_Write_Enum.Read)
            {
                //读取用多线程连接
                Socket_Connect_Thread = new Thread(() => Socket_Client_KUKA(_Enum, _IP, _Port)) { Name = "Connect—KUKA", IsBackground = true };
                Socket_Connect_Thread.Start();
            }
            if (_Enum == Read_Write_Enum.Write)
            {
                //写入同步线程连接
                Socket_Client_KUKA(_Enum, _IP, _Port);
            }

        }


        /// <summary>
        /// TCP连接方法
        /// </summary>
        /// <param name="R_W_Enum"></param>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        public void Socket_Client_KUKA(Read_Write_Enum R_W_Enum, string _IP, string _Port)
        {



            //try
            //{




            //设置读写IP
            IP = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));



            //显示连接中状态


            //读取连接设置
            if (R_W_Enum == Read_Write_Enum.Read && !Is_Read_Client)
            {
            Messenger.Default.Send<int>(0, "Connect_Client_Socketing_Button_Show");
                //重置连接阻塞标识
                Socket_Read.Reset();



                //设置Socket
                Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //启动异步连接
                Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);




                //连接超时判断
                if (!Socket_Read.WaitOne(2000, true) || !Is_Read_Client)
                {
                    Socket_Receive_Error(R_W_Enum, "Error: -53 原因:读取连接超时！检查网络与IP设置是否正确。");
                    return;
                }
            Messenger.Default.Send<bool>(false, "Connect_Client_Button_IsEnabled");
            }

            if (R_W_Enum == Read_Write_Enum.Write && !Is_Write_Client)
            {

                //加线程锁，只允许单次进入
                //Connect_Lock.WaitOne();


                Connnect_Write.Reset();

                Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Global_Socket_Write.Connect(IP);

                Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);

                //连接超时判断




                //释放线程锁
                //Connect_Lock.ReleaseMutex();

            }
            //开始异步连接



            //禁止控件用户二次连接



            //}
            //catch (Exception e)
            //{

            //    User_Log_Add("Error:-5 " + e.Message);

            //}





        }





        /// <summary>
        /// TCP长时间连接设置
        /// </summary>
        /// <param name="_On">启动</param>
        /// <param name="KeepAliveTime">数据空闲时间开始</param>
        /// <param name="KeepAliveInterval">相隔发送时间</param>
        /// <returns></returns>
        public void KeepAlive(Socket _Socket, bool _On, int KeepAliveTime, int KeepAliveInterval)
        {
            if (_Socket != null)
            {

                byte[] buffer = new byte[12];
                BitConverter.GetBytes(Convert.ToInt32(_On)).CopyTo(buffer, 0);
                BitConverter.GetBytes(KeepAliveTime).CopyTo(buffer, 4);
                BitConverter.GetBytes(KeepAliveInterval).CopyTo(buffer, 8);
                _Socket.IOControl(IOControlCode.KeepAliveValues, buffer, null);
            }

        }




        /// <summary>
        /// 异步连接回调命令
        /// </summary>
        /// <param name="ar"></param>
        public void Client_Inf(IAsyncResult ar)
        {



            Read_Write_Enum _Enum = (Read_Write_Enum)ar.AsyncState;


            string Remote_IP = String.Empty;
            string Local_IP = String.Empty;

            //初始接收属性
            Socket_KUKA_Receive = new Socket_Models_Receive() { Read_Write_Type = _Enum };

            //try
            //{


            //连接成功再继续
            if (Global_Socket_Write.Connected && _Enum == Read_Write_Enum.Write)
            {



                //当前错误信息IP号
                //Remote_IP = Global_Socket_Write.RemoteEndPoint.ToString();
                Local_IP = Global_Socket_Write.LocalEndPoint.ToString();
                try
                {

                    //挂起读取异步连接
                    Global_Socket_Write.EndConnect(ar);
                    Is_Write_Client = true;

                }
                catch (Exception e)
                {

                    //User_Log_Add($"写入连接目标IP—失败！");
                    User_Log_Add($"Error: -51 原因:" + e.Message);
                    return;
                }



                //Keep-Alive保持连接设置
                //KeepAlive(Global_Socket_Write, true, 2000, 1000);


                //异步监听接收写入消息
                Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);
                //发送终端连接信息
                User_Log_Add("写入连接目标IP：" + Global_Socket_Write.RemoteEndPoint.ToString());
                User_Log_Add("写入本地连接IP：" + Global_Socket_Write.LocalEndPoint.ToString());

                //释放连接线程，保留

                //连接完成释放线程
                Connnect_Write.Set();


            }





            if (Global_Socket_Read.Connected && _Enum == Read_Write_Enum.Read)
            {


                //当前错误信息IP号
                Local_IP = Global_Socket_Read.LocalEndPoint.ToString();
                //Remote_IP = Global_Socket_Read.RemoteEndPoint.ToString();
                try
                {

                    //挂起读取异步连接
                    Global_Socket_Read.EndConnect(ar);
                    Is_Read_Client = true;

                }
                catch (Exception e)
                {

                    //User_Log_Add($"读取连接目标IP—失败！");
                    User_Log_Add($"Error: -50 原因:" + e.Message);
                    return;
                }


                //KeepAlive(Global_Socket_Read, true, 2000, 1000);

                //异步监听接收读取消息
                Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);



                //发送终端连接信息
                User_Log_Add("读取连接目标IP：" + Global_Socket_Read.RemoteEndPoint.ToString());
                User_Log_Add("读取本地连接IP：" + Global_Socket_Read.LocalEndPoint.ToString());





                //阻塞一定时间，等待其他程序集载入
                //Thread.Sleep(1000);

                //开启多线程监听集合内循环发送
                Messenger.Default.Send<bool>(true, "Socket_Read_Thread");


                //前端显示连接成功
                Messenger.Default.Send<int>(1, "Connect_Client_Socketing_Button_Show");




                //连接成功标识
                Socket_Read.Set();



            }





            //连接状态显示
            //if (Global_Socket_Read.Connected )
            //{

            //    Messenger.Default.Send<int>(1, "Connect_Client_Socketing_Button_Show");

            //}
            //else
            //{
            //    //连接失败后允许用户再次点击连接按钮
            //    Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");
            //    Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");


            //    User_Log_Add("Error:-16 " + Local_IP + "连接超时!");


            //}

            //}
            //catch (Exception e)
            //{


            //    Socket_Receive_Error("Error:-3 " + e.Message);

            //}






        }






        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar">Socket属性</param>
        public void Socke_Receive_Message(IAsyncResult ar)
        {





            //传入参数转换
            Socket_Models_Receive Socket_KUKA_Receive = ar.AsyncState as Socket_Models_Receive;

            Socket_Modesl_Byte _Byte = new Socket_Modesl_Byte() { };



            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
            {
                //等待发送完成标识
                Send_Read.Wait();

                //获取接收字节数量
                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Read.EndReceive(ar);

                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {
                    //接收异常退出
                    User_Log_Add("Error: -19 原因:" + GetType().Name + " 接收线程，库卡服务器断开！");
                    //Socket_Receive_Error("Error:-19 " + GetType().Name + " 接收线程，库卡服务器断开！");
                    //Quit_Waite.Set();
                    return;
                }

            }

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
            {
                Rece_IA_Lock.WaitOne();


                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Write.EndReceive(ar);
                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {
                    //接收异常退出

                    Socket_Receive_Error(Socket_KUKA_Receive.Read_Write_Type, "Error: -20 原因:" + GetType().Name + " 写入线程，库卡服务器断开！");

                    return;

                }
                Rece_IA_Lock.ReleaseMutex();

            }







            if (Socket_KUKA_Receive.Byte_Leng > 0)
            {



                //获取接收字节
                if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
                {

                    _Byte._data = Socket_KUKA_Receive.Byte_Read_Receive;
                    Socket_KUKA_Receive.Byte_Read_Receive = new byte[1024 * 2];
                }
                else if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
                {
                    _Byte._data = Socket_KUKA_Receive.Byte_Write_Receive;
                    Socket_KUKA_Receive.Byte_Write_Receive = new byte[1024 * 2];
                }


                //Array.Resize(ref _data, Socket_KUKA_Receive.Byte_Leng);

                _Byte._data = _Byte._data.Skip(0).Take(Socket_KUKA_Receive.Byte_Leng).ToArray();

                #region 排序

                //提出前俩位的id号
                _Byte._ID = Int32.Parse(BitConverter.ToString(_Byte._data.Skip(0).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取接收变量总长度
                _Byte._Val_Total_Length = Int32.Parse(BitConverter.ToString(_Byte._data.Skip(2).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取读取还是写入状态
                _Byte._Return_Tpye = Int32.Parse(BitConverter.ToString(_Byte._data.Skip(4).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取变量长度数据
                var b = _Byte._data.Skip(5).Take(2).ToArray();
                var bb = BitConverter.ToString(b).Replace("-", "");
                var bbb = Convert.ToInt64(bb, 16);
                _Byte._Val_Length = Int32.Parse(bb, System.Globalization.NumberStyles.HexNumber);

                //提取接收返回变量值
                _Byte.Message_Show = Encoding.ASCII.GetString(_Byte._data, 7, _Byte._Val_Length);

                //提取写入是否成功
                _Byte._Write_Type = Int32.Parse(BitConverter.ToString(_Byte._data.Skip(_Byte._Val_Total_Length + 3).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);


                #endregion

                if (_Byte._Return_Tpye == 1 && _Byte._Write_Type == 1)
                {
                    User_Log_Add(_Byte.Message_Show + " 写入成功！");
                }
                else if (_Byte._Return_Tpye == 1 && _Byte._Write_Type == 0)
                {
                    User_Log_Add($"Read Val:" + _Byte.Message_Show + " 写入失败！");

                }



                //回传接收消息到显示
                if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
                {

                    Messenger.Default.Send(_Byte, "Socket_Read_List");

                }




            }



            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write && Is_Write_Client)
            {
                //线程独占锁
                Rece_IA_Lock.WaitOne();



                //发送回调等待标识
                Send_IA.WaitOne();



                //关闭写入连接，重置标识
                Global_Socket_Write.Shutdown(SocketShutdown.Both);
                Global_Socket_Write.Close();
                Is_Write_Client = false;

                //释放接收等待状态
                Rece_Write.Set();

                Rece_IA_Lock.ReleaseMutex();
            }

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
            {
                //递归调用读取接收

                var _ = Global_Socket_Read.Connected;
                if (Is_Read_Client)
                {

                    Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);
                }

                //接收完成释放一次读取线程
                Send_Waite.Set();
            }

        }




        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="Message">发送处理好的字节流发送</param>
        /// <param name="_i">0是读取发送，1是写入发送</param>
        public void Socket_Send_Message_Method(Socket_Models_Send _S)
        {


            Byte[] Message = _S.Send_Byte;

            if (_S.Read_Write_Type == Read_Write_Enum.Write)
            {
                //互斥线程锁，保证每次只有一个线程接收消息
                Send_Lock.EnterWriteLock();

                User_Log_Add("剩余读取线程：" + Send_Lock.WaitingWriteCount.ToString());

                //同步线程启动
                Socket_Client_Thread(Read_Write_Enum.Write, Socket_Client_Setup.Text_Error.User_IP, Socket_Client_Setup.Text_Error.User_Port);



                //等待连接完成


                if (!Connnect_Write.WaitOne(300, false))
                {
                    Socket_Receive_Error(_S.Read_Write_Type, "Error: -54 原因:写入连接超时！检查网络与IP设置是否正确。");

                    //退出写入独占锁
                    Send_Lock.ExitWriteLock();
                    return;
                }
                //重置发送等待状态
                Send_Write.Reset();


                //重置发送回调等待
                Send_IA.Reset();


                //重置接收等待状态
                Rece_Write.Reset();
                //异步写入发送
                Global_Socket_Write.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Write);

                //等待发送完成标识
                Send_Write.WaitOne();

                Task.Delay(50);
                //等待接收完成关闭标识
                Rece_Write.WaitOne();







                //User_Log_Add("退出线程号：" + Thread.CurrentThread.ManagedThreadId);
                //接收信息互斥线程锁，保证每次只有一个线程接收消息
                Send_Lock.ExitWriteLock();


            }

            if (_S.Read_Write_Type == Read_Write_Enum.Read)
            {
                lock (Global_Socket_Read)
                {

                    //异步读取发送
                    Global_Socket_Read.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Read);
                }

            }


        }


        /// <summary>
        /// 发送信息异步回调
        /// </summary>
        /// <param name="Socket">异步参数</param>
        public void Socket_Send_Message(IAsyncResult ar)
        {
            //线程加锁
            //Send_IA_Lock.WaitOne();

            if (Global_Socket_Write == (Socket)ar.AsyncState)
            {

                Global_Socket_Write.EndSend(ar);



                //Connect_IA.Set();
                //释放发送等待状态
                Send_Write.Set();


                //发送回调完成后再关闭通讯口
                Send_IA.Set();


            }

            if (Global_Socket_Read == (Socket)ar.AsyncState)
            {

                Global_Socket_Read.EndSend(ar);

                //释放发送完成等待
                Send_Read.Set();

            }

            //线程释放锁
            //Send_IA_Lock.ReleaseMutex();

        }









        /// <summary>
        /// 处理读取变量字节流
        /// </summary>
        /// <param name="_var">读取名称</param>
        /// <param name="_ID">ID号</param>
        /// <returns></returns>
        public void Send_Read_Var(string _var, int _ID)
        {



            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(_var);




            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Send_number_ID(_ID));
            //传输数据总长度值
            _data.AddRange(Send_number_ID(_v.Length + 3));
            //读取标识 0x00 
            _data.AddRange(new byte[1] { 0x00 });
            //传输变量长度值
            _data.AddRange(Send_number_ID(_v.Length));
            //传输变量
            _data.AddRange(_v);
            //结束位号
            _data.AddRange(new byte[1] { 0x00 });

            //发送排序好的字节流发送
            Socket_Send_Message_Method(new Socket_Models_Send() { Send_Byte = _data.ToArray(), Read_Write_Type = Read_Write_Enum.Read });



        }


        /// <summary>
        /// 处理写入变量转换字节流
        /// </summary>
        /// <param name="_name">写入变量名</param>
        /// <param name="_var">写入变量值</param>
        public void Send_Write_Var(string _name, string _var)
        {


            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(_var);
            byte[] _n = Encoding.Default.GetBytes(_name);

            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Send_number_ID(Write_Number_ID));
            //传输数据总长度值
            _data.AddRange(Send_number_ID(_n.Length + _v.Length + 5));
            //写入标识 0x01
            _data.AddRange(new byte[1] { 0x01 });
            //传输变量长度值
            _data.AddRange(Send_number_ID(_n.Length));
            //传输变量
            _data.AddRange(_n);
            //传输写入值长度值
            _data.AddRange(Send_number_ID(_v.Length));
            //传输写入值
            _data.AddRange(_v);
            //结束位号
            _data.AddRange(new byte[1] { 0x00 });



            //发送排序好的字节流发送



            Socket_Send_Message_Thread(new Socket_Models_Send() { Send_Byte = _data.ToArray(), Read_Write_Type = Read_Write_Enum.Write });


        }

        /// <summary>
        /// 新建线程发送消息
        /// </summary>
        /// <param name="_S"></param>
        public void Socket_Send_Message_Thread(Socket_Models_Send _S)
        {



            //if (!Socket_Write_Thread.IsAlive && Socket_Write_Thread==null)
            //{
            //读取用多线程连接
            Socket_Write_Thread = new Thread(() => Socket_Send_Message_Method(_S)) { Name = "Write—KUKA", IsBackground = true };
            Socket_Write_Thread.Start();

            //}



        }




        /// <summary>
        /// 断开连接
        /// </summary>
        public void Socket_Close(Read_Write_Enum _Enum)
        {



            if (_Enum == Read_Write_Enum.Read)
            {

                //断开读取连接
                //读取标识重置
                Is_Read_Client = false;


                //等待发送变量退出后关闭连接


                if (Global_Socket_Read.Connected)
                {

                    Close_Waite.WaitOne();

                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                    Global_Socket_Read.Close();

                }






                //清除集合内容
                Messenger.Default.Send(true, "Clear_List");
                //User_Log_Add("断开读取连接");
            }

            if (_Enum == Read_Write_Enum.Write)
            {
                Is_Write_Client = false;

                if (Global_Socket_Write.Connected)
                {

                    //关闭写入连接，重置标识
                    Global_Socket_Write.Shutdown(SocketShutdown.Both);
                    Global_Socket_Write.Close();
                }

                //释放接收等待状态
                Rece_Write.Set();




            }





            //连接失败后允许用户再次点击连接按钮
            Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");
            Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");








        }



        /// <summary>
        /// 接收异常处理程序
        /// </summary>
        /// <param name="_Error">连接失败原因输入</param>
        public void Socket_Receive_Error(Read_Write_Enum _Enum, string _Error)
        {
            //线程独占锁
            //Quit_Lock.EnterWriteLock();

            //重置退出标记
            //Quit_Waite.Reset();


            Close_Waite.Reset();
            Close_Waite.Set();

            //连接失败后关闭连接

            Socket_Close(_Enum);
            User_Log_Add(_Error);

            //Quit_Lock.ExitWriteLock();
        }




    }


}










