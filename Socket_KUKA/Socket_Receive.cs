using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.ViewModel;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Socket_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;
using static 悍高软件.ViewModel.UserControl_Right_Socket_Connection_ViewModel;




namespace Soceket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Receive : ViewModelBase
    {



        private static Socket_Models_Receive _Receive = new Socket_Models_Receive() { };
        /// <summary>
        /// 接收消息属性
        /// </summary>
        public static Socket_Models_Receive Receive
        {
            get
            {
                return _Receive;
            }
            set
            {
                _Receive = value;
            }

        }

        //private byte[] _data { set; get; } 












        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock = new Mutex();
        public static Mutex Receive_ReturnString_Lock = new Mutex();

        public int Receive_Int { set; get; } = 0;



        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar">Socket属性</param>
        public void Socke_Receive_Message(IAsyncResult ar)
        {




            //传入参数转换
            Socket_Models_Receive _Receive = ar.AsyncState as Socket_Models_Receive;

            try
            {



            if (_Receive.Read_int == 0)
            {

                Receive_Int = Global_Socket_Read.EndReceive(ar);
                if (Receive_Int == 0)
                {

                    Socket_Receive_Error("Error:-19 " + GetType().Name+ " 接收消息线程异常！");
                    
                }

                
            }
            else if (_Receive.Read_int == 1)
            {
                Receive_Int = Global_Socket_Write.EndReceive(ar);
                if (Receive_Int == 0)
                {
                    Socket_Receive_Error("Error:-20 " + GetType().Name+" 接收消息线程异常！");
                    
                }
    
            }

            }
            catch (Exception e)
            {
                Socket_Receive_Error("Error:-14 " + "接收消息线程异常断开！");
                //User_Log_Add("Error: -14 " + e.Message);
                return;
            }

            if ((Global_Socket_Write.Poll(10, SelectMode.SelectWrite) == false && Global_Socket_Read.Poll(10, SelectMode.SelectWrite) == false)) { Socket_Receive_Error("Error:-23 " + GetType().Name + " 接收消息线程异常！"); return; }

            //互斥线程锁，保证每次只有一个线程接收消息
            Receive_Lock.WaitOne();

            //连接属性断开后为空后退出接收
            //if (Global_Socket_Write == null || Global_Socket_Read == null) { Socket_Receive_Error("Error:-22 " + GetType().Name + " 接收消息线程异常！"); return; }



            if (_Receive.Read_int == 0)
            {

                //User_Log_Add("-2.1，等待接收线程");
                Monitor.Enter(The_Lock);
                //User_Log_Add("-2.2，进入接收线程");
            }


            //连接状态不正常时退出,并运行错误操作
            try
            {
                //if ((Global_Socket_Write.Poll(10, SelectMode.SelectWrite) == false && Global_Socket_Read.Poll(10, SelectMode.SelectWrite) == false)) { Socket_Receive_Error("Error:-23 " + GetType().Name+ " 接收消息线程异常！"); return; }




                byte[] _data = Array.Empty<byte>();


                if (_Receive.Read_int == 0)
                {
                   

                    Array.Resize(ref _data, Receive_Int);

                    _data = byte_Read_Receive;
                }
                else if (_Receive.Read_int == 1)
                {
              
                    Array.Resize(ref _data, Receive_Int);

                    _data = _Receive.byte_Write_Receive;
                }


                //提出前俩位的id号
                int _ID = Int32.Parse(BitConverter.ToString(_data.Skip(0).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取接收变量总长度
                int _Val_Total_Length = Int32.Parse(BitConverter.ToString(_data.Skip(2).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取读取还是写入状态
                int _Return_Tpye = Int32.Parse(BitConverter.ToString(_data.Skip(4).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取变量长度数据
                int _Val_Length = Int32.Parse(BitConverter.ToString(_data.Skip(5).Take(2).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);

                //提取接收返回变量值
                string Message_Show = Encoding.ASCII.GetString(_data, 7, _Val_Length);

                //提取写入是否成功
                int _Write_Type = Int32.Parse(BitConverter.ToString(_data.Skip(_Val_Total_Length + 3).Take(1).ToArray()).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);


                if (_Return_Tpye == 1 && _Write_Type == 1)
                {
                    //MessageBox.Show($"Read Val:"+ Message_Show + " 写入成功！");

                }
                else if (_Return_Tpye == 1 && _Write_Type == 0)
                {
                    MessageBox.Show($"Read Val:" + Message_Show + " 写入失败！");


                }




                if (_Receive.Read_int == 0)
                {


                    //接收到的消息写入到集合里面
                    for (int i = 0; i < Socket_Read_List.Count; i++)
                    {


                        //根据ID号对应写入接收到的内容
                        if (Socket_Read_List[i].Val_ID == _ID)
                        {

                            //更新内容时间


                            //更新参数值
                            if (Socket_Read_List[i].Val_Var != Message_Show)
                            {


                                Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                                Socket_Read_List[i].Val_Var = Message_Show;

                                //把属于自己的区域回传
                                Messenger.Default.Send<Socket_Models_List>(Socket_Read_List[i], Socket_Read_List[i].Send_Area);

                            }

                        }


                    }
                    //User_Log_Add("-2.3，接收到的消息：" + Message_Show);


                    //User_Log_Add("-2.4，释放发送线程");
                    Monitor.Pulse(The_Lock);

                    //User_Log_Add("-2.5，已释放发送线程锁");
                    Monitor.Exit(The_Lock);

                }

            }
            catch (Exception e)
            {
                Socket_Receive_Error("Error:-24 " + e.Message);
                //User_Log_Add("Error: -24 " + e.Message);
            }




            try
            {




                if (_Receive.Read_int == 1)
                {
                    //Socket_Connect.Global_Socket_Write.EndReceive(ar);

                    //递归调用写入接收
                    Global_Socket_Write.BeginReceive(byte_Write_Receive, 0, byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), new Socket_Models_Receive() { byte_Write_Receive = byte_Write_Receive, Read_int = 1 });
                }
                else if (_Receive.Read_int == 0)
                {
                    //Socket_Connect.Global_Socket_Read.EndReceive(ar);

                    //递归调用读取接收
                    Global_Socket_Read.BeginReceive(byte_Read_Receive, 0, byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), new Socket_Models_Receive() { byte_Read_Receive = byte_Read_Receive, Read_int = 0 });

                }
            }
            catch (Exception e)
            {
                Socket_Receive_Error("Error:-25 " + e.Message);
                //User_Log_Add("Error: -25 " + e.Message);
            }




            //接收信息互斥线程锁，保证每次只有一个线程接收消息
            Receive_Lock.ReleaseMutex();
        }




        /// <summary>
        /// 接收异常处理程序
        /// </summary>
        public static void Socket_Receive_Error(string _Error)
        {

            //Messenger.Default.Send<bool?>(false, "Sidebar_Subtitle_Signal_Method_bool");

            ////连接失败后允许用户再次点击连接按钮
            //Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");
            //Messenger.Default.Send<int>(-1, "Connect_Socketing_Method");
            Socket_Close();
            User_Log_Add(_Error);

        }




    }
}
