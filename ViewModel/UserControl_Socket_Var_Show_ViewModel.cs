using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
 
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using 悍高软件.Socket_KUKA;
using 悍高软件.ViewModel;
using static Soceket_Connect.Socket_Connect;


using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.ViewModel.UserControl_Socket_Setup_ViewModel;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;



namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Var_Show_ViewModel : ViewModelBase
    {
        public UserControl_Socket_Var_Show_ViewModel()
        {
            //MessageBox.Show($"线程ID：" + Thread.CurrentThread.ManagedThreadId.ToString());

            //读取变量集合发送
            Messenger.Default.Register<bool>(this, "Clear_List", (_Bool) =>
            {
                if (_Bool)
                {

                    foreach (var List in Socket_Read_List)
                    {
                        List.Val_Var = string.Empty;
                    }
                }
            });


            // 发送内容集合接收写入
            Messenger.Default.Register<ObservableCollection<Socket_Models_List>>(this, "List_Connect", (_List) =>
            {
                //写入集合中
                foreach (var item in _List)
                {

                    if (!Socket_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {

                        Socket_Read_List.Add(item);
                    }

                }
            });


            //接收消息更新列表变量值
            Messenger.Default.Register<Socket_Modesl_Byte>(this, "Socket_Read_List", List_Var_Show);


            //开始读取集合发送线程
            Messenger.Default.Register<bool>(this, "Socket_Read_Thread", (_Bool) =>
            {
                if (_Bool)
                {
                    Socket_Read_Thread = new Thread(Receive_Read_Theam) { Name = "Read", IsBackground = true };
                    Socket_Read_Thread.Start();


                }
                else
                {
                    if (!Socket_Read_Thread.IsAlive)
                    {

                        Socket_Read_Thread.Abort();
                    }

                }

            });

        }




        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        public static ObservableCollection<Socket_Models_List> Socket_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>() { };




        //public  ManualResetEventSlim Send_Waite { set; get; } = new ManualResetEventSlim(false);

        private static int _Write_Number_ID=0;
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
                bool a = false;
                do
                {
                 a = Socket_Read_List.Any<Socket_Models_List>(_ => _.Val_ID == _Write_Number_ID);
                    _Write_Number_ID++;
                }
                while (a);




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

                _Read_Number_ID++;

                return _Read_Number_ID;
            }
        }


        public DateTime Delay_time { set; get; }

        /// <summary>
        /// 循环读取线程设置
        /// </summary>
        public Thread Socket_Read_Thread { set; get; }


        public void List_Var_Show(Socket_Modesl_Byte _Byte)
        {
            //MessageBox.Show($"线程ID：" + Thread.CurrentThread.ManagedThreadId.ToString());
           

            lock (Socket_Read_List)
            {

                if (Socket_Read_List.Count > 0)
                {


                    for (int i = 0; i <Socket_Read_List.Count; i++)
                    {

                        if (Socket_Read_List[i].Val_ID == _Byte._ID && Socket_Read_List[i].Val_Var != _Byte.Message_Show)
                        {
                            Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                            Socket_Read_List[i].Val_Var = _Byte.Message_Show;
                            //MessageBox.Show(Socket_Read_List[i].Val_Var);
                            //把属于自己的区域回传
                            Messenger.Default.Send<Socket_Models_List>(Socket_Read_List[i], Socket_Read_List[i].Send_Area);
                            return;
                        }

                    }

      






                }


            }


        }






        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand Click_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                //UIElement e = Sm.Source as UIElement;
                //Send_Waite.Set();
                MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());

                //for (int i = 0; i < Socket_Read_List.Count; i++)
                //{
                //    Socket_Read_List[i].Val_Var = "0.011";

                //    Task.Run(() => 
                //    {
                //    MessageBox.Show(Socket_Read_List[i].Val_Var);
                //    });
                //}

            });
        }






        /// <summary>
        /// 读取集合循环发送
        /// </summary>
        /// <param name="_Obj"></param>
        public void Receive_Read_Theam()
        {

            //Receive_Waite.Wait();
            //The_Lock = new object();





            while (Socket_Client_Setup.Read.Is_Read_Client)
            {
                //User_Log_Add("-1.1，准备发送线程");
                //Monitor.Enter(The_Lock);

                //User_Log_Add("-1.2，进入发送线程");






                try
                {


                    var bool_A = Socket_Read_List.Count > 0;
                    //var bool_B = Global_Socket_Read.Poll(-1, SelectMode.SelectRead);













                    foreach (var item in Socket_Read_List)
                    {
                        //当前时间
                        Delay_time = DateTime.Now;
                        if (item.Val_OnOff == true)
                        {

                            int _ID = item.Val_ID;
                            Socket_Client_Setup.Read.Send_Read_Var(item.Val_Name, _ID);

                            //User_Log_Add("-1.3，等待下一个变量发送");
                            //Monitor.Wait(The_Lock);
                            Send_Waite.Wait();
                            //User_Log_Add("-1.4，解除接收等待");
                            Thread.Sleep(1);
                            if (!Socket_Client_Setup.Read.Is_Read_Client) { return; }

                        }


                        Send_Waite.Reset();
                    }

                    Messenger.Default.Send<string>((DateTime.Now - Delay_time).TotalMilliseconds.ToString().Split('.')[0], "Connter_Time_Delay_Method");

                    //for (int i = 0; i < Socket_Read_List.Count; i++)
                    //{
                    //    if (Socket_Read_List[i].Val_OnOff == true)
                    //    {


                    //        Socket_Read_List[i].Val_ID = i;
                    //        Socket_Send.Send_Read_Var(Socket_Read_List[i].Val_Name, i);



                    //    }
                    //}


                }
                catch (Exception e)
                {
                    //异常处理
                    User_Log_Add($"Error:-8 " + e.Message);
                    User_Log_Add("-1.5，退出发送线程");
                    //Monitor.Exit(The_Lock);
                    Clear_List();
                    return;
                }



                //发送延时毫秒
            }



        }




        /// <summary>
        /// 清除读取集合内容
        /// </summary>
        public void Clear_List()
        {
            foreach (var List in Socket_Read_List)
            {
                List.Val_Var = string.Empty;
            }
        }


    }
}
