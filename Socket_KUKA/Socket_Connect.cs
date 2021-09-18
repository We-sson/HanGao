using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections;
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



        public Socket_Connect()
        {
            //初始化


        }


        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock { set; get; } = new Mutex();
        public static Mutex Receive_ReturnString_Lock = new Mutex();



        public static ManualResetEventSlim Connnect_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Send_Waite { set; get; } = new ManualResetEventSlim(false);
        public static ManualResetEventSlim Receive_Waite { set; get; } = new ManualResetEventSlim(false);


        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Connect_Lock = new Mutex();


        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public static bool Is_Write_Client { set; get; } = false;

        /// <summary>
        /// 写入连接成功属性
        /// </summary>
        public static bool Is_Read_Client { set; get; } = false;

        /// <summary>
        /// 集合读取允许
        /// </summary>
        public static bool Read_List { set; get; } = false;

        //public byte[] _data { set; get; }= Array.Empty<byte>();


        /// <summary>
        /// Socket连接ip和端口属性
        /// </summary>
        public static Socket_Models_Connect Socket_Client { set; get; } = new Socket_Models_Connect() { };



        /// <summary>
        /// 异步接受属性
        /// </summary>
        public Socket_Models_Receive Socket_KUKA_Receive { set; get; } = new Socket_Models_Receive() { };



        /// <summary>
        /// Socket唯一写入连接标识
        /// </summary>
        public static Socket Global_Socket_Write { set; get; }



        /// <summary>
        /// Socket唯一读取连接标识
        /// </summary>
        public static Socket Global_Socket_Read { set; get; }




        public  static  IPEndPoint IP { set; get; } 




        /// <summary>
        /// Socket连接方法
        /// </summary>
        public  void Socket_Client_KUKA(Read_Write_Enum R_W_Enum, string _Ip, int _Port)
        {



            if (!Is_Read_Client || !Is_Write_Client)
            {



                //设置连接IP断口 
                IP = new IPEndPoint(IPAddress.Parse(_Ip), _Port);

                try
                {
                    //显示连接中状态


                    Messenger.Default.Send<int>(0, "Connect_Client_Socketing_Button_Show");


                    if (R_W_Enum== Read_Write_Enum.Read)
                    {


                    Is_Read_Client = true;
                    Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    //Global_Socket_Read.BeginConnect(IP, new AsyncCallback(Client_Inf), Global_Socket_Read);

                    }


                    if (R_W_Enum== Read_Write_Enum.Write)
                    {

                    Is_Write_Client = true;

                    Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Global_Socket_Write.BeginConnect(IP, new AsyncCallback(Client_Inf), Global_Socket_Write);

                    }








                    //开始异步连接


                    Connnect_Waite.Set();


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
            Connnect_Waite.Wait();
            if (Is_Read_Client || Is_Write_Client)
            {


                Socket _Connect = (Socket)ar.AsyncState;

                //通讯连接判断
                try
                {

                    lock (this)
                    {


                        if (Global_Socket_Write == _Connect)
                        {
                            //挂起写入异步连接


                            Global_Socket_Write.EndConnect(ar);
                            Socket_KUKA_Receive = new Socket_Models_Receive() { Read_Write_Type = Read_Write_Enum.Write };

                            //Keep-Alive保持连接设置
                            KeepAlive(Global_Socket_Write, true, 2000, 1000);


                            //异步监听接收写入消息
                            Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);


                            //发送终端连接信息
                            User_Log_Add("写入连接IP：" + Global_Socket_Write.LocalEndPoint.ToString());


                            Send_Waite.Set();

                        }

                        else if (Global_Socket_Read == _Connect)
                        {
                            //挂起读取异步连接

                            lock (Global_Socket_Read)
                            {
                                Global_Socket_Read.EndConnect(ar);
                                Socket_KUKA_Receive = new Socket_Models_Receive() { Read_Write_Type = Read_Write_Enum.Read };


                                //KeepAlive(Global_Socket_Read, true, 2000, 1000);

                                //异步监听接收读取消息
                                Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);


                                //发送终端连接信息
                                User_Log_Add("读取连接IP：" + Global_Socket_Read.LocalEndPoint.ToString());

                            }


                            //允许读取集合内容
                            Read_List = true;


                            //开启多线程监听集合内循环发送
                            Messenger.Default.Send<bool>(true, "Socket_Read_Thread");





                        }

                    }




                    //连接状态显示
                    if (Global_Socket_Read.Connected || Global_Socket_Write.Connected)
                    {

                        Messenger.Default.Send<int>(1, "Connect_Client_Socketing_Button_Show");

                    }
                    else
                    {
                        //连接失败后允许用户再次点击连接按钮
                        Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");
                        Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");
                        User_Log_Add("Error:-16 " + _Connect.RemoteEndPoint.ToString() + "连接超时!");


                    }

                }
                catch (Exception e)
                {


                    Socket_Receive_Error("Error:-3 " + e.Message);

                }


                Receive_Waite.Set();
            }


        }






        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar">Socket属性</param>
        public void Socke_Receive_Message(IAsyncResult ar)
        {


            if (Is_Read_Client)
            {


                //传入参数转换
                Socket_Models_Receive Socket_KUKA_Receive = ar.AsyncState as Socket_Models_Receive;

                Socket_Modesl_Byte _Byte = new Socket_Modesl_Byte() { };
                //try
                //{

                lock (this)
                {

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
                    else if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
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


                    //if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
                    //{

                    //    //User_Log_Add("-2.1，等待接收线程");
                    //    //Monitor.Enter(The_Lock);
                    //    //User_Log_Add("-2.2，进入接收线程");
                    //}


                    //连接状态不正常时退出,并运行错误操作
                    //try
                    //{




                    if (Socket_KUKA_Receive.Byte_Leng > 0)
                    {



                        lock (_Byte)
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
                                MessageBox.Show($"Read Val:" + _Byte.Message_Show + " 写入失败！");

                            }




                            if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
                            {

                                Messenger.Default.Send(_Byte, "Socket_Read_List");





                                //接收到的消息写入到集合里面

                                //for (int i = 0; i < Socket_Read_List.Count; i++)
                                //{
                                //    var id = Socket_Read_List[i].Val_ID;
                                //    //根据ID号对应写入接收到的内容
                                //    if (Socket_Read_List[i].Val_ID == _ID)
                                //    {
                                //        //更新内容时间
                                //        //更新参数值
                                //        var a = Socket_Read_List[i].Val_Var;
                                //        if (a != Message_Show)
                                //        {
                                //            Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                                //            Socket_Read_List[i].Val_Var = Message_Show;
                                //            //把属于自己的区域回传
                                //            Messenger.Default.Send<Socket_Models_List>(Socket_Read_List[i], Socket_Read_List[i].Send_Area);
                                //            return;
                                //        }
                                //    }
                                //}


                                //User_Log_Add("-2.3，接收到的消息：" + Message_Show);


                                //User_Log_Add("-2.4，释放发送线程");
                                //Monitor.Pulse(The_Lock);

                                //User_Log_Add("-2.5，已释放发送线程锁");
                                //Monitor.Exit(The_Lock);

                            }


                        }

                    }

                    //}
                    //catch (Exception e)
                    //{
                    //    Socket_Receive_Error("Error:-24 " + e.Message);
                    //    //User_Log_Add("Error: -24 " + e.Message);
                    //}



                    //try
                    //{





                    if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Write)
                    {


                        //递归调用写入接收
                        Global_Socket_Write.BeginReceive(Socket_KUKA_Receive.Byte_Write_Receive, 0, Socket_KUKA_Receive.Byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);
                    }
                    else if (Socket_KUKA_Receive.Read_Write_Type == Read_Write_Enum.Read)
                    {


                        //递归调用读取接收
                        Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);
                        var _ = Global_Socket_Read.Connected;
                        Global_Socket_Read.BeginReceive(Socket_KUKA_Receive.Byte_Read_Receive, 0, Socket_KUKA_Receive.Byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), Socket_KUKA_Receive);

                        Send_Waite.Set();
                    }

                    //}
                    //catch (Exception e)
                    //{
                    //    Socket_Receive_Error("Error:-25 " + e.Message);
                    //    //User_Log_Add("Error: -25 " + e.Message);
                    //}


                }



            }




        }






        /// <summary>
        /// 断开所有连接
        /// </summary>
        public static void Socket_Close()
        {
            if (Is_Read_Client && Is_Read_Client)
            {





                //停止读取集合内容
                Read_List = false;
                Is_Read_Client = false;
                Is_Write_Client = false;



                //清除集合内容
                Messenger.Default.Send(true, "Clear_List");


                //断开所以连接
                if (Global_Socket_Write.Connected)
                {
                    Global_Socket_Write.Shutdown(SocketShutdown.Both);

                }
                Global_Socket_Write.Close();
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










    }


}










