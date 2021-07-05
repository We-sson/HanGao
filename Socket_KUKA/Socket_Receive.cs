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





        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="ar">Socket属性</param>
        public void Socke_Receive_Message(IAsyncResult ar)
        {


            //互斥线程锁，保证每次只有一个线程接收消息
            Receive_Lock.WaitOne();
            User_Control_Log_ViewModel.User_Log_Add("-2.1，等待接收线程");
            Monitor.Enter(Socket_Connect.The_Lock);
            User_Control_Log_ViewModel.User_Log_Add("-2.2，进入接收线程");



            //连接属性断开后为空后退出接收
            if (Socket_Connect.Global_Socket_Write == null || Socket_Connect.Global_Socket_Read == null) { Socket_Receive_Error(); return; }
            //连接状态不正常时退出,并运行错误操作
            try
            {
                if ((Socket_Connect.Global_Socket_Write.Poll(50, SelectMode.SelectWrite) == false && Socket_Connect.Global_Socket_Read.Poll(50, SelectMode.SelectWrite) == false)) { Socket_Receive_Error(); return; }



                //传入参数转换
                Socket_Models_Receive _Receive = ar.AsyncState as Socket_Models_Receive;



                byte[] _data = new byte[0];











                if (_Receive.Read_int == 0)
                {


                    Array.Resize(ref _data, Socket_Connect.Global_Socket_Read.EndReceive(ar));

                    _data = Socket_Models_Connect.byte_Read_Receive;
                }
                else if (_Receive.Read_int == 1)
                {

                    Array.Resize(ref _data, Socket_Connect.Global_Socket_Write.EndReceive(ar));

                    _data = _Receive.byte_Write_Receive;
                }



                //缩减字节大小





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
                    MessageBox.Show("Read Val OK!");

                }
                else if (_Return_Tpye == 1 && _Write_Type == 0)
                {
                    MessageBox.Show("Read Val NO!");

                }





                //接收到的消息写入到集合里面
                for (int i = 0; i < UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Count; i++)
                {



                    if (UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_ID == _ID)
                    {

                        UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                        UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_Var = Message_Show;


                        //把属于自己的区域回传
                        Messenger.Default.Send<Socket_Models_List>(UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i], UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Send_Area);

                    }


                }
           











                if (_Receive.Read_int == 1)
                {
                    //Socket_Connect.Global_Socket_Write.EndReceive(ar);

                    //递归调用写入接收
                    Socket_Connect.Global_Socket_Write.BeginReceive(Socket_Models_Connect.byte_Write_Receive, 0, Socket_Models_Connect.byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), new Socket_Models_Receive() { byte_Write_Receive = Socket_Models_Connect.byte_Write_Receive, Read_int = 1 });
                }
                else if (_Receive.Read_int == 0)
                {
                    //Socket_Connect.Global_Socket_Read.EndReceive(ar);

                    //递归调用读取接收
                    Socket_Connect.Global_Socket_Read.BeginReceive(Socket_Models_Connect.byte_Read_Receive, 0, Socket_Models_Connect.byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(Socke_Receive_Message), new Socket_Models_Receive() { byte_Read_Receive = Socket_Models_Connect.byte_Read_Receive, Read_int = 0 });

                }








            }
            catch (Exception)
            {
                Socket_Receive_Error();
                User_Control_Log_ViewModel.User_Log_Add("-2.3，退出接收线程");
                Monitor.Exit(Socket_Connect.The_Lock);
                //MessageBox.Show(er.Message);
            }



            User_Control_Log_ViewModel.User_Log_Add("-2.3，释放发送线程");
            Monitor.PulseAll(Socket_Connect.The_Lock);
            User_Control_Log_ViewModel.User_Log_Add("-2.4，已释放发送线程锁");
            Monitor.Exit(Socket_Connect.The_Lock);

            //接收信息互斥线程锁，保证每次只有一个线程接收消息
            Receive_Lock.ReleaseMutex();
        }




        /// <summary>
        /// 连接异常处理程序
        /// </summary>
        public static void Socket_Receive_Error()
        {

            Messenger.Default.Send<bool?>(true, "Connect_Button_IsEnabled_Method");
            Messenger.Default.Send<bool?>(false, "Sidebar_Subtitle_Signal_Method_bool");


        }



        /// <summary>
        /// 循环读取所需变量值
        /// </summary>
        /// <param name="_Ob"></param>
        /// <returns></returns>
        public string Receive_Return_String(object _Obj)
        {
            //string Name_Val = _Obj as string;

            Receive_ReturnString_Lock.WaitOne();
            try
            {
                while (Socket_Connect.Global_Socket_Read.Connected)
                {
                    //UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List

                }

            }

            catch (Exception)
            {


            }

            Receive_ReturnString_Lock.ReleaseMutex();

            return string.Empty;

        }

    }
}
