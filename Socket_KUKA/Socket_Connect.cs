using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.Socket_KUKA;
using 悍高软件.ViewModel;

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
        public static Mutex Connect_Lock = new Mutex();






        private Socket_Models_Connect _Socket_Client = new Socket_Models_Connect() { };
        /// <summary>
        /// Socket连接ip和端口属性
        /// </summary>
        public Socket_Models_Connect Socket_Client
        {
            get
            {
                return _Socket_Client;
            }
            set
            {
                _Socket_Client = value;
            }
        }

        /// <summary>
        /// 异步接受属性
        /// </summary>
        public Socket_Receive _Socket_Receive { set; get; } = new Socket_Receive() { };


        private static Socket _Global_Socket_Write;
        /// <summary>
        /// Socket唯一写入连接标识
        /// </summary>
        public static Socket Global_Socket_Write
        {
            get
            {
                return _Global_Socket_Write;
            }
            set
            {
                _Global_Socket_Write = value;
            }
        }

        private static Socket _Global_Socket_Read;
        /// <summary>
        /// Socket唯一连接标识
        /// </summary>
        public static Socket Global_Socket_Read
        {
            get
            {
                return _Global_Socket_Read;
            }
            set
            {
                _Global_Socket_Read = value;
            }
        }






        /// <summary>
        /// Socket连接方法
        /// </summary>
        public void Socket_Client_KUKA(string _Ip, int _Port)
        {
            try
            {

                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_Ip), _Port);
                Global_Socket_Write = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Global_Socket_Read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Global_Socket_Write.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Global_Socket_Read.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                try
                {






                    //异步连接
                    Global_Socket_Write.BeginConnect(ip, new AsyncCallback(Client_Inf), Global_Socket_Write);
                    Global_Socket_Read.BeginConnect(ip, new AsyncCallback(Client_Inf), Global_Socket_Read);


                    //显示连接成功
                    Messenger.Default.Send<bool>(true, "Connect_Socketing_Method");
                    //禁止控件用户二次连接
                    Messenger.Default.Send<bool>(false, "Connect_Button_IsEnabled_Method");



                }
                catch (Exception e)
                {
                    //开饭连接按钮
                    Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");

                    MessageBox.Show(e.Message);

                }




            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                return;
            }






        }








        /// <summary>
        /// 异步连接回调命令
        /// </summary>
        /// <param name="ar"></param>
        public void  Client_Inf (IAsyncResult ar)
        {


            Socket Client = (Socket)ar.AsyncState;
            //ar.AsyncWaitHandle.WaitOne(1000);




            //挂起异步连接
            try
            {
                if (Global_Socket_Write == ar.AsyncState)
                {
                    //挂起写入异步连接
                    Global_Socket_Write.EndConnect(ar);
                    //异步监听接收写入消息
                    Global_Socket_Write.BeginReceive(Socket_Models_Connect.byte_Write_Receive, 0, Socket_Models_Connect.byte_Write_Receive.Length, SocketFlags.None, new AsyncCallback(_Socket_Receive.Socke_Receive_Message), new Socket_Models_Receive() { byte_Write_Receive = Socket_Models_Connect.byte_Write_Receive, Read_int = 1 });

                }
                else if (Global_Socket_Read == ar.AsyncState)
                {
                    //挂起读取异步连接
                    Global_Socket_Read.EndConnect(ar);
                    //异步监听接收读取消息
                    Global_Socket_Read.BeginReceive(Socket_Models_Connect.byte_Read_Receive, 0, Socket_Models_Connect.byte_Read_Receive.Length, SocketFlags.None, new AsyncCallback(_Socket_Receive.Socke_Receive_Message), new Socket_Models_Receive() { byte_Read_Receive = Socket_Models_Connect.byte_Read_Receive, Read_int = 0 });


                    //开启多线程监听集合内循环发送
                    Thread Socket_Read_Thread = new Thread(Receive_Read_Theam) { IsBackground = true };
                    Socket_Read_Thread.Start();
                    //Task T = Task.Run(() =>
                    // {
                    // Receive_Read_Theam();
                    // }
                    // );



                }


                //MessageBox.Show(Client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {

                //Messenger.Default.Send<bool>(false, "Connect_Button_IsEnabled_Method");
                MessageBox.Show("连接失败" + e.Message);
                //出现报错后运行再次连接
                Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");
                Messenger.Default.Send<bool>(false, "Connect_Socketing_Method");

                return;
            }



            //连接成功后，指示灯闪烁
            Messenger.Default.Send<bool?>(true, "Sidebar_Subtitle_Signal_Method_bool");

            









        }


        /// <summary>
        /// 断开所有连接
        /// </summary>
        public static async Task Socket_Close()
        {
             if (Global_Socket_Write != null && Global_Socket_Read != null)
            {

                //关闭Socket之前，首选需要把双方的Socket Shutdown掉
                //Global_Socket_Write.Shutdown(SocketShutdown.Both);
                //Global_Socket_Read.Shutdown(SocketShutdown.Both);

                //Shutdown掉Socket后主线程停止10ms，保证Socket的Shutdown完成
                await Task.Delay(10);


                Global_Socket_Write.Close();
                Global_Socket_Read.Close();

                //Global_Socket_Write = null;
                //Global_Socket_Read = null;

                Messenger.Default.Send<bool>(true, "Connect_Button_IsEnabled_Method");
                Messenger.Default.Send<bool>(false, "Connect_Socketing_Method");
                //断开连接后，关闭指示灯闪烁
                Messenger.Default.Send<bool?>(false, "Sidebar_Subtitle_Signal_Method_bool");

            }

            return;


        }





        /// <summary>
        /// 读取集合循环发送
        /// </summary>
        /// <param name="_Obj"></param>
        public  void  Receive_Read_Theam()
        {

            //Socket_Receive.Receive_Lock.WaitOne();
            //Thread.Sleep(5000);


            


            try
            {

                
                    //Socket_Receive.Receive_Lock.WaitOne(100);
                    //Connect_Lock.WaitOne(100);

                    
                    if (UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Count > 0)
                    {

                        for (int i = 0; i < UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Count; i++)
                        {
                            if (UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_OnOff == true)
                            {
                                UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_ID = i;
                                Socket_Send.Send_Read_Var(UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_Name, i);



                                //MessageBox.Show("发送完成变量：" + UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List[i].Val_Name);
                            }


                            

                        }




                    }



                 Task.Delay(200).Wait();
                  Receive_Read_Theam();
            }
            catch (Exception)
            {

         

            }
            finally
            {
                //异常退出时清空读取集合变量值
                foreach (var List in UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List)
                {
                    List.Val_Var = string.Empty;
                }
            }


            //Socket_Receive.Receive_Lock.ReleaseMutex();

        }


    }





}







