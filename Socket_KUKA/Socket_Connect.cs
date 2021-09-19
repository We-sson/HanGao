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

            //初始化
            IP = new IPEndPoint(IPAddress.Parse(_IP), int.Parse(_Port));

            if (_Type == Connect_Type.Long)
            {
                Socket_Client_KUKA(_Enum);
            }
        }



        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock { set; get; } = new Mutex();
        public static Mutex Receive_ReturnString_Lock = new Mutex();



        public static ManualResetEventSlim Connnect_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Connnect_Write_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Send_Write_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Rece_Write_Waite { set; get; } = new ManualResetEventSlim(false);



        public static ManualResetEventSlim Send_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Receive_Waite { set; get; } = new ManualResetEventSlim(false);


        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Connect_Lock = new Mutex();

        /// <summary>
        /// 长连接启动属性
        /// </summary>
        public bool Socket_Long_Connect { set; get; } = false;

        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public bool Is_Write_Client { set; get; } = false;

        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public bool Is_Read_Client { set; get; } = false;

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
        /// 资源互锁
        /// </summary>
        public static Mutex Wrist_Lock = new Mutex();




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
        public Socket Global_Socket_Write { set; get; }



        /// <summary>
        /// Socket唯一读取连接标识
        /// </summary>
        public Socket Global_Socket_Read { set; get; }




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
        /// Socket连接方法
        /// </summary>
        public void Socket_Client_KUKA(Read_Write_Enum R_W_Enum)
        {



            if (!Is_Read_Client || !Is_Write_Client)
            {



                //设置连接IP断口 

                try
                {
                    //显示连接中状态


                    Messenger.Default.Send<int>(0, "Connect_Client_Socketing_Button_Show");


                    if (R_W_Enum == Read_Write_Enum.Read && !Is_Read_Client)
                    {


                        Is_Read_Client = true;
                        Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                        Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);


                    }


                    if (R_W_Enum == Read_Write_Enum.Write && !Is_Write_Client)
                    {

                        Is_Write_Client = true;

                        Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), R_W_Enum);

                    }








                    //开始异步连接


                    //Connnect_Waite.Set();


                    //禁止控件用户二次连接
                    Messenger.Default.Send<bool>(false, "Connect_Client_Button_IsEnabled");








                }
                catch (Exception e)
                {

                    User_Log_Add("Error:-5 " + e.Message);

                }





            }




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
            //Connnect_Waite.Wait();


            Read_Write_Enum _Enum = (Read_Write_Enum)ar.AsyncState;


            string Remote_IP = String.Empty;
            string Local_IP = String.Empty;

            //初始接收属性
            Socket_KUKA_Receive = new Socket_Models_Receive() { Read_Write_Type = _Enum };
            //通讯连接判断
            //try
            //{



            if (Is_Write_Client)
            {

                if (_Enum == Read_Write_Enum.Write)
                {
                    //挂起写入异步连接


                    Global_Socket_Write.EndConnect(ar);

                    //Keep-Alive保持连接设置
                    //KeepAlive(Global_Socket_Write, true, 2000, 1000);


                    //异步监听接收写入消息
                    Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);

                    //当前错误信息IP号
                    Remote_IP = Global_Socket_Write.RemoteEndPoint.ToString();
                    Local_IP = Global_Socket_Write.LocalEndPoint.ToString();
                    //发送终端连接信息
                    User_Log_Add("写入连接IP：" + Global_Socket_Write.LocalEndPoint.ToString());

                    //释放一次线程，保留
                    Connnect_Write_Waite.Set();

                }
            }





            if (Is_Read_Client)
            {

                if (_Enum == Read_Write_Enum.Read)
                {
                    //挂起读取异步连接


                    Global_Socket_Read.EndConnect(ar);


                    //KeepAlive(Global_Socket_Read, true, 2000, 1000);

                    //异步监听接收读取消息
                    Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);


                    //当前错误信息IP号
                    Local_IP = Global_Socket_Read.LocalEndPoint.ToString();

                    Remote_IP = Global_Socket_Read.RemoteEndPoint.ToString();
                    //发送终端连接信息
                    User_Log_Add("读取连接IP：" + Global_Socket_Read.LocalEndPoint.ToString());


                    //开启多线程监听集合内循环发送
                    Messenger.Default.Send<bool>(true, "Socket_Read_Thread");



                    //前端显示连接成功
                    Messenger.Default.Send<int>(1, "Connect_Client_Socketing_Button_Show");


                }

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



            Receive_Waite.Set();



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




            //try
            //{



            //Receive_Lock.WaitOne();

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
            {

                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Read.EndReceive(ar);
                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {

                    Socket_Receive_Error("Error:-19 " + GetType().Name + " 接收线程，库卡服务器断开！");
                    //重新连接
                    //Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), Global_Socket_Read);
                    return;
                }


            }






            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
            {
                Socket_KUKA_Receive.Byte_Leng = Global_Socket_Write.EndReceive(ar);
                if (Socket_KUKA_Receive.Byte_Leng == 0)
                {

                    Socket_Receive_Error("Error:-20 " + GetType().Name + " 写入线程，库卡服务器断开！");
                    //重新连接

                    //Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), Global_Socket_Write);

                    return;

                }

            }







            if (Socket_KUKA_Receive.Byte_Leng > 0)
            {



                lock (this)
                {



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

            }



            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
            {

                Rece_Write_Waite.Wait();

                Global_Socket_Write.Shutdown(SocketShutdown.Both);
                Task.Delay(5);
                Global_Socket_Write.Close();
                Is_Write_Client = false;
                //递归调用写入接收
                //Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);
            }

            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
            {


                //递归调用读取接收

                var _ = Global_Socket_Read.Connected;
                Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);

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








            //互斥线程锁，保证每次只有一个线程接收消息
            Wrist_Lock.WaitOne();







            Byte[] Message = _S.Send_Byte;
            //Read_Write_Enum _i = _S.Read_Write_Type;
            //byte_Send = Encoding.UTF8.GetBytes((string ) Message);

            lock (Message)
            {




                //try
                //{
                //发送消息到服务器
                if (_S.Read_Write_Type == Read_Write_Enum.Write)
                {
                    //Socket_Connect _Client = new Socket_Connect();
                    //_Client.Socket_Client_KUKA(Read_Write_Enum.Write);
                    Socket_Client_KUKA(Read_Write_Enum.Write);

                    //等待连接完成
                    Connnect_Write_Waite.Wait();
                    Thread.Sleep(5);
                    //异步写入发送
                    Global_Socket_Write.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Write);


                    //等待发送完成
                    Send_Write_Waite.Wait();

                    Rece_Write_Waite.Set();
                }
                
                if (_S.Read_Write_Type == Read_Write_Enum.Read)
                {
                    //异步读取发送
                    Global_Socket_Read.BeginSend(Message, 0, Message.Length, SocketFlags.None, new AsyncCallback(Socket_Send_Message), Global_Socket_Read);
                }



                //}
                //catch (Exception e)
                //{



                //    Socket_Receive_Error("Error:-6 " + e.Message);

                //}

            }
            //接收信息互斥线程锁，保证每次只有一个线程接收消息
            Wrist_Lock.ReleaseMutex();

        }


        /// <summary>
        /// 发送信息异步回调
        /// </summary>
        /// <param name="Socket">异步参数</param>
        public void Socket_Send_Message(IAsyncResult ar)
        {



            if (Global_Socket_Write == (Socket)ar.AsyncState)
            {

                lock (Global_Socket_Write)
                {

                    Global_Socket_Write.EndSend(ar);
                    //释放发送信号
                    Send_Write_Waite.Set();

                }
            }
            
            if (Global_Socket_Read == (Socket)ar.AsyncState)
            {
                lock (Global_Socket_Read)
                {

                    Global_Socket_Read.EndSend(ar);
                }
            }



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



            Socket_Send_Message_Method(new Socket_Models_Send() { Send_Byte = _data.ToArray(), Read_Write_Type = Read_Write_Enum.Write });
            Task.Run(() => 
            {
            //发送排序好的字节流发送
            });

        }





        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void Socket_Close()
        {
            if (Is_Read_Client)
            {


                //Is_Write_Client = false;

                Is_Read_Client = false;


                //清除集合内容
                Messenger.Default.Send(true, "Clear_List");


                //断开所以连接
                //if (Global_Socket_Write.Connected)
                //{
                //    Global_Socket_Write.Shutdown(SocketShutdown.Both);

                //}
                //Global_Socket_Write.Close();
                if (Global_Socket_Read.Connected)
                {

                    Global_Socket_Read.Shutdown(SocketShutdown.Both);
                }
                Global_Socket_Read.Close();


                //Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //停止循环线程阻塞
                Send_Waite.Set();

                User_Log_Add("断开全部连接");


                //连接失败后允许用户再次点击连接按钮
                Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");
                Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");


            }




        }



        /// <summary>
        /// 接收异常处理程序
        /// </summary>
        public void Socket_Receive_Error(string _Error)
        {



            ////连接失败后允许用户再次点击连接按钮
            //Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");
            //Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");
            Socket_Close();
            User_Log_Add(_Error);

        }






    }


}










