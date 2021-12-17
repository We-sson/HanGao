using Microsoft.Toolkit.Mvvm.Messaging;

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
using HanGao.Socket_KUKA;
using HanGao.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Var_Show_ViewModel : ObservableRecipient
    {



        public UserControl_Socket_Var_Show_ViewModel()
        {

            //开始读取集合发送线程
            Messenger.Register<dynamic ,string >(this,nameof( Meg_Value_Eunm.Socket_Read_Thread), (O,_Bool) =>
            {
                if (_Bool)
                {
                    Socket_Read_Thread = new Thread(Receive_Read_Theam) { Name = "Read", IsBackground = true };
                    Socket_Read_Thread.Start();
                    User_Log_Add("启动变量发送线程");

                }


            });
            //读取变量集合发送
            Messenger.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Clear_List), (O,_Bool) =>
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
           Messenger.Register<ObservableCollection<Socket_Models_List>,string >(this, nameof(Meg_Value_Eunm.List_Connect), (O,_List) =>
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
            Messenger.Register<Socket_Modesl_Byte,string >(this, nameof(Meg_Value_Eunm.Socket_Read_List), (O, _Byte) =>
            {
                lock (Socket_Read_List)
                {

                    if (Socket_Read_List.Count > 0)
                    {


                        for (int i = 0; i < Socket_Read_List.Count; i++)
                        {

                            if (Socket_Read_List[i].Val_ID == _Byte.Byte_ID && Socket_Read_List[i].Val_Var != _Byte.Message_Show)
                            {
                                Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                                Socket_Read_List[i].Val_Var = _Byte.Message_Show;
                                //MessageBox.Show(Socket_Read_List[i].Val_Var);
                                //把属于自己的区域回传
                                Socket_Models_List a = Socket_Read_List[i];

                                Messenger.Send<Socket_Models_List,string  >(a, Socket_Read_List[i].Send_Area);
                                return;
                            }

                        }








                    }


                }


            });


            //释放发送线程

        }




        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        public static ObservableCollection<Socket_Models_List> Socket_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>() { };





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


                    for (int i = 0; i < Socket_Read_List.Count; i++)
                    {

                        if (Socket_Read_List[i].Val_ID == _Byte.Byte_ID && Socket_Read_List[i].Val_Var != _Byte.Message_Show)
                        {
                            Socket_Read_List[i].Val_Update_Time = DateTime.Now.ToLocalTime();
                            Socket_Read_List[i].Val_Var = _Byte.Message_Show;
                            //把属于自己的区域回传
                            Messenger.Send<Socket_Models_List,string >(Socket_Read_List[i], Socket_Read_List[i].Send_Area);
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






            while (true)
            {

                try
                {


                    var bool_A = Socket_Read_List.Count > 0;
                    //var bool_B = Global_Socket_Read.Poll(-1, SelectMode.SelectRead);



                    if (bool_A)
                    {

                        for (int i = 0; i < Socket_Read_List.Count; i++)
                        {




                            Send_Waite.Reset();
                            //当前时间
                            Delay_time = DateTime.Now;
                            if (Socket_Read_List[i].Val_OnOff)
                            {
                                int _ID = Socket_Read_List[i].Val_ID;
                                //重置发送等待标识
                                Send_Read.Reset();

                                //发送变量集合内容
                                Socket_Client_Setup.Read.Send_Read_Var(Socket_Read_List[i].Val_Name, _ID);

                                //等待发送完成


                                if (!Send_Waite.WaitOne(3000, false) || !Socket_Client_Setup.Read.Is_Read_Client)
                                {
                                    Socket_Client_Setup.Read.Socket_Receive_Error(Read_Write_Enum.Read, "发送超时无应答，退出线程发送！");
                                    return;
                                }

     
                            }


                        }

                        //发送通讯延迟
                       Messenger.Send<string,string >((DateTime.Now - Delay_time).TotalMilliseconds.ToString().Split('.')[0], nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));

                    }
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
                    User_Log_Add($"Error: -08 原因:" + e.Message);
                    //User_Log_Add("-1.5，退出发送线程");
                    //Clear_List();
                    return;
                }
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
