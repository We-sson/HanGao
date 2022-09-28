

using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;



namespace Soceket_Connect
{

    public class Socket_Connect
    {



        public Socket_Connect()
        {

        }







        private ManualResetEvent Close_Waite { set; get; } = new ManualResetEvent(false);

        private ManualResetEvent Send_Read { set; get; } = new ManualResetEvent(false);
        private ManualResetEvent Connnect_Write { set; get; } = new ManualResetEvent(false);
        private ManualResetEvent Connnect_Read { set; get; } = new ManualResetEvent(false);
        private ManualResetEvent Send_Write { set; get; } = new ManualResetEvent(false);
        private ManualResetEvent Rece_Write { set; get; } = new ManualResetEvent(false);
        private ManualResetEvent Send_Waite { set; get; } = new ManualResetEvent(false);

        /// <summary>
        /// 通讯接收信息委托申声明
        /// </summary>
        /// <param name="_Receive_Info"></param>
        public delegate void Socket_Receive_delegate(Socket_Models_Receive _Receive_Info);
        /// <summary>
        /// 通讯接收信息委托属性
        /// </summary>
        public Socket_Receive_delegate Socket_Receive_Delegate { set; get; }

        /// <summary>
        /// 连接状态枚举
        /// </summary>
        public enum Socket_Tpye
        {
            Connect_OK,
            Connect_Cancel,
        }



        public double Socket_Time { set; get; } = 0;

        private bool _Is_Write_Client = false;

        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public bool Is_Write_Client
        {
            get { return _Is_Write_Client; }
            set
            {
                _Is_Write_Client = value;
                if (value)
                {
                    //Messenger.Send<string , string>(Socket_Tpye.Connect_OK.ToString(), Meg_Value_Eunm.Socket_Write_Tpye.ToString());
                }
                else
                {
                    //Messenger.Send<string, string>(Socket_Tpye.Connect_Cancel.ToString(), Meg_Value_Eunm.Socket_Write_Tpye.ToString());

                }
            }
        }



        private bool _Is_Read_Client = false;
        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public bool Is_Read_Client
        {
            get { return _Is_Read_Client; }
            set
            {
                _Is_Read_Client = value;
                if (value)
                {
                    //Messenger.Send<string, string>(Socket_Tpye.Connect_OK.ToString(), Meg_Value_Eunm.Socket_Read_Tpye.ToString());
                }
                else
                {
                    //Messenger.Send<string, string>(Socket_Tpye.Connect_Cancel.ToString(), Meg_Value_Eunm.Socket_Read_Tpye.ToString());

                }
            }
        }

        /// <summary>
        /// 设置IP
        /// </summary>
        private string _IP;

        public string KUKA_IP
        {
            get { return _IP; }
            set { _IP = value; }
        }



        /// <summary>
        /// 设置端口
        /// </summary>
        private string _Port;

        public string KUKA_Port
        {
            get { return _Port; }
            set { _Port = value; }
        }


        private static int _Write_Number_ID = 0;
        /// <summary>
        /// 写入变量唯一标识ID号
        /// </summary>
        public static int Write_Number_ID
        {
            set
            {
                _Write_Number_ID = value;
            }
            get
            {
                if (_Write_Number_ID > 65500)
                {
                    _Write_Number_ID = 0;
                }
                //bool a = false;


                _Write_Number_ID++;




                return _Write_Number_ID;
            }
        }

        private static int _Read_Number_ID;
        /// <summary>
        /// 读取变量唯一标识ID号
        /// </summary>
        public static int Read_Number_ID
        {
            set
            {
                _Read_Number_ID = value;
            }
            get
            {



                if (_Read_Number_ID > 65500)
                {
                    _Read_Number_ID = 0;
                }
                //do
                //{
                _Read_Number_ID++;

                //} while (Socket_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID) && On_Read_List.Any<Socket_Models_List>(l => l.Val_ID == _Read_Number_ID));

                return _Read_Number_ID;
            }

        }








        /// <summary>
        /// 写入锁
        /// </summary>
        private ReaderWriterLockSlim Write_Lock { set; get; } = new ReaderWriterLockSlim();
        /// <summary>
        /// 异步接受属性
        /// </summary>
        private Socket_Models_Receive Socket_KUKA_Receive = new Socket_Models_Receive();
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
        private IPEndPoint IP { set; get; }

        /// <summary>
        /// Socket连接
        /// </summary>
        public enum Socket_Client_Type
        {
            /// <summary>
            /// 同步连接
            /// </summary>
            Synchronized,
            /// <summary>
            /// 异步连接
            /// </summary>
            Asynchronous,
            /// <summary>
            /// 多线程连接
            /// </summary>
            Thread
        }



        /// <summary>
        /// 输入ID号返回对应byte组
        /// </summary>
        private byte[] Send_number_ID(int _ID)
        {

            var arr = new byte[_ID.ToString("x4").Length / 2];

            for (var i = 0; i < arr.Length; i++)
                arr[i] = (byte)Convert.ToInt32((_ID.ToString("x4")).Substring(i * 2, 2), 16);


            return arr;
        }


        /// <summary>
        /// 线程连接方式
        /// </summary>
        /// <param name="_Enum"></param>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        private void Socket_Client_Thread(Socket_Client_Type _Enum, Read_Write_Enum R_W_Enum)
        {




            switch (_Enum)
            {
                case Socket_Client_Type.Synchronized:

                    Socket_Client_KUKA(R_W_Enum);

                    break;
                case Socket_Client_Type.Asynchronous:

                    Task.Run(async () =>
                    {

                        //写入同步线程连接
                        Socket_Client_KUKA(R_W_Enum);

                        await Task.Delay(5);

                    });

                    break;

                case Socket_Client_Type.Thread:
                    //读取用多线程连接
                    new Thread(() => Socket_Client_KUKA(R_W_Enum)) { Name = "Connect—KUKA", IsBackground = true }.Start();

                    break;
            }

        }


        /// <summary>
        /// TCP连接方法
        /// </summary>
        /// <param name="R_W_Enum"></param>
        /// <param name="_IP"></param>
        /// <param name="_Port"></param>
        private void Socket_Client_KUKA(Read_Write_Enum R_W_Enum)
        {





            //设置读写IP
            IP = new IPEndPoint(IPAddress.Parse(KUKA_IP), int.Parse(KUKA_Port));




            if (R_W_Enum == Read_Write_Enum.One_Read || R_W_Enum == Read_Write_Enum.Read)
            {


                //重置连接阻塞标识
                Connnect_Read.Reset();

                //设置Socket
                Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //启动异步连接
                Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);




                //连接超时判断
                if (!Connnect_Read.WaitOne(10000, true) || !Is_Read_Client)
                {
                    Socket_Receive_Error(R_W_Enum, "Error: -53 原因:读取连接超时！检查网络与IP设置是否正确。");
                    return;
                }







            }
            else if (R_W_Enum == Read_Write_Enum.Write)
            {

                Connnect_Write.Reset();

                Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);


                //连接超时判断
                if (!Connnect_Write.WaitOne(1000, false) || !Is_Write_Client)
                {
                    Socket_Receive_Error(R_W_Enum, "Error: -53 原因:写入连接超时！检查网络与IP设置是否正确。");
                    return;
                }



            }



        }




        /// <summary>
        /// 异步连接回调命令
        /// </summary>
        /// <param name="ar"></param>
        private void Client_Inf(IAsyncResult ar)
        {



            Read_Write_Enum _Enum = (Read_Write_Enum)ar.AsyncState;



            //string Local_IP;


            if (_Enum == Read_Write_Enum.Write)
            {



                //当前错误信息IP号
                //Local_IP = Global_Socket_Write.LocalEndPoint.ToString();



                try
                {
                    //Task.Delay(10);
                    //挂起读取异步连接
                    Global_Socket_Write.EndConnect(ar);
                    Is_Write_Client = true;




                }
                catch 
                {

                     
                    //User_Log_Add($"Error: -51 原因:" + e.Message);


                    return;
                }

                //发送终端连接信息
                //User_Log_Add("写入连接目标IP：" + Global_Socket_Write.RemoteEndPoint.ToString());
                //User_Log_Add("写入本地连接IP：" + Global_Socket_Write.LocalEndPoint.ToString());

                //释放连接线程，保留





                //连接完成释放线程
                Connnect_Write.Set();






            }

            if (_Enum == Read_Write_Enum.Read || _Enum == Read_Write_Enum.One_Read)
            {

                //当前错误信息IP号
                //Local_IP = Global_Socket_Read.LocalEndPoint.ToString();
                //Remote_IP = Global_Socket_Read.RemoteEndPoint.ToString();
                try
                {
                    //Task.Delay(10);
                    //Thread.Sleep(20);
                    //挂起读取异步连接
                    Global_Socket_Read.EndConnect(ar);
                    Is_Read_Client = true;

                }
                catch 
                {


                    //User_Log_Add($"Error: -50 原因:" + e.Message);
                    return;
                }




                //发送终端连接信息
                //User_Log_Add("读取连接目标IP：" + Global_Socket_Read.RemoteEndPoint.ToString());
                //User_Log_Add("读取本地连接IP：" + Global_Socket_Read.LocalEndPoint.ToString());


                //连接成功释放阻塞
                Connnect_Read.Set();


            }












        }






        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar">Socket属性</param>
        private void Socke_Receive_Message(IAsyncResult ar)
        {





            //传入参数转换
            Socket_Models_Receive _Receive = ar.AsyncState as Socket_Models_Receive;

            //  Socket_Modesl_Byte _Byte = new Socket_Modesl_Byte() { };



            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
            {
                //等待发送完成标识
                Send_Read.WaitOne(10000);

                //获取接收字节数量
                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Read.EndReceive(ar);

                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {
                    //接收异常退出
                    //User_Log_Add("Error: -19 原因:" + GetType().Name + " 接收消息异常，库卡服务器断开！");
                    Socket_Receive_Error(Socket_KUKA_Receive.Read_Write_Type, "Error: -20 原因:" + GetType().Name + " 写入线程，库卡服务器断开！");

                    return;
                }

            }

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
            {
                // Rece_IA_Lock.WaitOne();


                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Write.EndReceive(ar);
                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {
                    //接收异常退出

                    Socket_Receive_Error(Socket_KUKA_Receive.Read_Write_Type, "Error: -20 原因:" + GetType().Name + " 写入线程，库卡服务器断开！");

                    return;

                }
                // Rece_IA_Lock.ReleaseMutex();

            }







            if (Socket_KUKA_Receive.Byte_Leng > 0)
            {



                //获取接收字节
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


                #region 排序
                //Array.Resize(ref _data, Socket_KUKA_Receive.Byte_Leng);



                #endregion


                //回传接收消息到显示
                if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
                {

                    _Receive.Reveice_Inf.Val_Var = Socket_KUKA_Receive.Receive_Byte.Message_Show;




                    //设置了变量值接收位置的，委托传输
                    if (_Receive.Reveice_Inf.Send_Area != "")
                    {
                        //委托方法
                        Socket_Receive_Delegate(_Receive);
                    

                    }
 


                }




            }



            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
            {


                Is_Write_Client = false;

                //释放接收等待状态
                Send_Waite.Set();



            }

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read || Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.One_Read)
            {
                //释放发送线程 
                Send_Waite.Set();

            }





        }




        /// <summary>
        /// 消息发送
        /// </summary>
        private void Socket_Send_Message_Method(Socket_Models_Receive _S)
        {


            Byte[] Message = _S.Send_Byte;
            //Socket_KUKA_Receive = _S;




            if (_S.Read_Write_Type == Read_Write_Enum.Write && Global_Socket_Write.Connected == true)
            {



                //重置发送等待状态
                Send_Write.Reset();
                Send_Waite.Reset();



                //重置接收等待状态
                //  Rece_Write.Reset();

                //异步监听接收写入消息
                Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), _S);



                Thread.Sleep(10);

                //异步写入发送
                Global_Socket_Write.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Write);

                ////等待发送完成标识
                //Send_Write.WaitOne();

                //Task.Delay(50);
                ////等待接收完成关闭标识
                //Rece_Write.WaitOne();


                if (!Send_Waite.WaitOne(1000) && !Send_Write.WaitOne(1000))
                {
                    Socket_Receive_Error(_S.Read_Write_Type, "Error: -54 原因:写入连接超时！检查网络与IP设置是否正确。");

                    //退出写入独占锁

                    return;
                }

            }
            if ((_S.Read_Write_Type == Read_Write_Enum.Read || _S.Read_Write_Type == Read_Write_Enum.One_Read) && Global_Socket_Read.Connected == true)
            {

                //复位连接发生线程堵塞
                Send_Read.Reset();
                Send_Waite.Reset();



                //异步监听接收读取消息
                Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), _S);

                Thread.Sleep(15);

                Global_Socket_Read.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Read);

                if (!Send_Waite.WaitOne(150000) && !Send_Read.WaitOne(1500000))
                {

                    Socket_Receive_Error(Read_Write_Enum.Read, "接收超时无应答，退出线程发送！");
                    return;
                }

            }
















        }


        /// <summary>
        /// 发送信息异步回调
        /// </summary>
        /// <param name="Socket">异步参数</param>
        private void Socket_Send_Message(IAsyncResult ar)
        {


            if (Global_Socket_Write == (Socket)ar.AsyncState)
            {

                Global_Socket_Write.EndSend(ar);

                //释放发送等待状态
                Send_Write.Set();

            }

            if (Global_Socket_Read == (Socket)ar.AsyncState)
            {

                Global_Socket_Read.EndSend(ar);

                //释放发送完成等待
                Send_Read.Set();

            }


        }



        /// <summary>
        /// 周期写入
        /// </summary>
        /// <param name="Sml"></param>
        public void Cycle_Write_Send(string _ValName, string _WriteVar)
        {

            lock (Socket_KUKA_Receive)
            {

                Write_Lock.EnterWriteLock();


                Socket_Models_List Sml = new Socket_Models_List() { Val_Name = _ValName, Write_Value = _WriteVar, Val_ID = Read_Number_ID };
                Socket_KUKA_Receive = new Socket_Models_Receive() { Send_Byte = Write_Var_To_Byte(Sml), Read_Write_Type = Read_Write_Enum.Write, Reveice_Inf = Sml };



                //创建连接
                Socket_Client_Thread(Socket_Client_Type.Synchronized, Read_Write_Enum.Write);



                if (Global_Socket_Write.Connected || Is_Write_Client)
                {
                    //发送消息
                    Socket_Send_Message_Method(Socket_KUKA_Receive);

                    // 关闭连接
                    Socket_Close(Read_Write_Enum.Write);

                }




                Write_Lock.ExitWriteLock();

            }
        }







        /// <summary>
        /// 读取变量周期方法
        /// </summary>
        /// <param name="Sml">周期传输集合</param>
        public void Cycle_Real_Send(ObservableCollection<Socket_Models_List> Sml)
        {


            //加锁
            lock (Socket_KUKA_Receive)
            {
                Socket_KUKA_Receive = new Socket_Models_Receive();
                Socket_Client_Thread(Socket_Client_Type.Synchronized, Read_Write_Enum.One_Read);


                if (Global_Socket_Read.Connected)
                {

                    //发生集合内的对象
                    for (int i = 0; i < Sml.Count; i++)
                    {


                        Socket_KUKA_Receive = new Socket_Models_Receive() {  Send_Byte = Read_Var_To_Byte(Sml[i]), Read_Write_Type = Read_Write_Enum.One_Read, Reveice_Inf = Sml[i] };

                        Socket_Send_Message_Method(Socket_KUKA_Receive);


                    }


                    // 关闭连接
                    Socket_Close(Read_Write_Enum.One_Read);
                }

            }
        }

        /// <summary>
        /// 读取变量循环方法
        /// </summary>
        /// <param name="Sml"></param>
        public void Loop_Real_Send(ObservableCollection<Socket_Models_List> Socket_Read_List)
        {
            //加锁
            lock (Socket_KUKA_Receive)
            {

                Socket_KUKA_Receive = new Socket_Models_Receive();


                Socket_Client_Thread(Socket_Client_Type.Synchronized, Read_Write_Enum.One_Read);

                //Messenger.Send<dynamic, string>(false, nameof(Meg_Value_Eunm.Connect_Client_Button_IsEnabled));
                do
                {

                    DateTime timeB = DateTime.Now;  //获取当前时间



                    //发生集合内的对象
                    for (int i = 0; i < Socket_Read_List.Count; i++)
                    {



                        Socket_KUKA_Receive = new Socket_Models_Receive() { Send_Byte = Read_Var_To_Byte(Socket_Read_List[i]), Read_Write_Type = Read_Write_Enum.One_Read, Reveice_Inf = Socket_Read_List[i] };
                        if (Is_Read_Client)
                        {


                        Socket_Send_Message_Method(Socket_KUKA_Receive);
                        }


                    }

                    //发送通讯延迟
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    Socket_Time = (DateTime.Now - timeB).TotalSeconds;
                   
                    //WeakReferenceMessenger.Default.Send<dynamic , string>((DateTime.Now - timeB).TotalSeconds, nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));
                    //});
                } while (Is_Read_Client);

                // 关闭连接
                Socket_Close(Read_Write_Enum.One_Read);

          
            }




        }

        /// <summary>
        /// 读取格式专值
        /// </summary>
        /// <param name="Smr"></param>
        private void Real_Byte_To_Var(ref Socket_Models_Receive Smr)
        {

            if (Smr.Read_Write_Type == Read_Write_Enum.Read || Smr.Read_Write_Type == Read_Write_Enum.One_Read)
            {

                Smr.Receive_Byte.Byte_data = Smr.Byte_Read_Receive.Skip(0).Take(Smr.Byte_Leng).ToArray();
            }
            else
            {
                Smr.Receive_Byte.Byte_data = Smr.Byte_Write_Receive.Skip(0).Take(Smr.Byte_Leng).ToArray();
            }


            //提出前俩位的id号
            Smr.Receive_Byte.Byte_ID = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(0).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            //提取接收变量总长度
            Smr.Receive_Byte.Byte_Val_Total_Length = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(2).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            //提取读取还是写入状态
            Smr.Receive_Byte.Byte_Return_Tpye = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(4).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

            //提取变量长度数据
            var b = Smr.Receive_Byte.Byte_data.Skip(5).Take(2).ToArray();
            var bb = BitConverter.ToString(b).Replace("-", "");
            //var bbb = Convert.ToInt64(bb, 16);
            Smr.Receive_Byte.Byte_Val_Length = Int32.Parse(bb, System.Globalization.NumberStyles.HexNumber);

            //提取接收返回变量值
            Smr.Receive_Byte.Message_Show = Encoding.ASCII.GetString(Smr.Receive_Byte.Byte_data, 7, Smr.Receive_Byte.Byte_Val_Length);


            //MessageBox.Show(Smr.Receive_Byte.Message_Show);

            //提取写入是否成功
            Smr.Receive_Byte.Byte_Write_Type = Int32.Parse(BitConverter.ToString(Smr.Receive_Byte.Byte_data.Skip(Smr.Receive_Byte.Byte_Val_Total_Length + 3).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);



            if (Smr.Receive_Byte.Byte_Return_Tpye == 1 && Smr.Receive_Byte.Byte_Write_Type == 1)
            {
                //User_Log_Add(Smr.Reveice_Inf.Val_Name + " = " + Smr.Receive_Byte.Message_Show);
                //User_Log_Add(" 变量值写入成功！");

            }
            else if (Smr.Receive_Byte.Byte_Return_Tpye == 1 && Smr.Receive_Byte.Byte_Write_Type == 0)
            {
                //User_Log_Add(Smr.Reveice_Inf.Val_Name + " = " + Smr.Receive_Byte.Message_Show);
                //User_Log_Add(" 变量值写入失败！");


            }



        }


        /// <summary>
        /// 处理读取变量字节流
        /// </summary>
        /// <param name="_var">读取名称</param>
        /// <param name="_ID">ID号</param>
        /// <returns></returns>
        private byte[] Read_Var_To_Byte(Socket_Models_List Var)
        {





            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(Var.Val_Name);


            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Send_number_ID(Var.Val_ID));
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


            return _data.ToArray();




        }

        /// <summary>
        /// 处理写入变量转换字节流
        /// </summary>
        /// <param name="_name">写入变量名</param>
        /// <param name="_var">写入变量值</param>
        private byte[] Write_Var_To_Byte(Socket_Models_List Var)
        {





            //临时存放变量
            List<byte> _data = new List<byte>();
            //变量转换byte
            byte[] _v = Encoding.Default.GetBytes(Var.Write_Value);
            byte[] _n = Encoding.Default.GetBytes(Var.Val_Name);

            //传输数据排列，固定顺序不可修改

            //传输数据唯一标识
            _data.AddRange(Send_number_ID(Read_Number_ID));
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
            return _data.ToArray();


        }



        /// <summary>
        /// 断开连接
        /// </summary>
        public void Socket_Close(Read_Write_Enum _Enum)
        {



            if (_Enum == Read_Write_Enum.Read)
            {



                //等待发送变量退出后关闭连接


                if (Global_Socket_Read.Connected)
                {

                    //Close_Waite.WaitOne();

                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                    Global_Socket_Read.Close();

                }

                //断开读取连接
                //读取标识重置
                Is_Read_Client = false;





                //清除集合内容
                //Messenger.Send<dynamic, string>(true, nameof(Meg_Value_Eunm.Clear_List));
                //User_Log_Add("断开读取连接");

                //连接失败后允许用户再次点击连接按钮
                //Messenger.Send<dynamic, string>(true, nameof(Meg_Value_Eunm.Connect_Client_Button_IsEnabled));
                //Messenger.Send<dynamic, string>(-1, nameof(Meg_Value_Eunm.Connect_Client_Socketing_Button_Show));

            }


            if (_Enum == Read_Write_Enum.One_Read)
            {


                if (Global_Socket_Read.Connected)
                {

                    //Close_Waite.WaitOne();

                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                    Global_Socket_Read.Close();

                }
                ////断开读取连接
                ////读取标识重置
                //Is_Read_Client = false;

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
        }



        /// <summary>
        /// 接收异常处理程序
        /// </summary>
        /// <param name="_Error">连接失败原因输入</param>
        public void Socket_Receive_Error(Read_Write_Enum _Enum, string _Error)
        {
            Close_Waite.Reset();
            Close_Waite.Set();

            //连接失败后关闭连接

            Socket_Close(_Enum);
            //User_Log_Add(_Error);

        }




    }


}










