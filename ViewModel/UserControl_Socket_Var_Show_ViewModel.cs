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
using System.Collections.Specialized;
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
using static HanGao.Extension_Method.SetReadTypeAttribute;
using static HanGao.ViewModel.UC_Surround_Point_VM;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Var_Show_ViewModel : ObservableRecipient
    {



        public UserControl_Socket_Var_Show_ViewModel()
        {

            Socket_Read_List.CollectionChanged += Socket_Read_List_CollectionChanged;

            IsActive = true;
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


                //User_Log_Add("剩余读取线程：" + Read_List.WaitingWriteCount.ToString());
                Read_List.EnterWriteLock();
                //写入集合中
                foreach (var item in _List)
                {
                   

                    if (!Socket_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
            
                        Socket_Read_List.Add(item);
                        }));



                    }
                    else
                    {
                        //查找相同名称和只读取一次属性
                  var a=  Socket_Read_List.Where<Socket_Models_List>(l => l.Value_One_Read ==  Read_Type_Enum.One_Read).FirstOrDefault()  ;
                        if (a !=null)
                        {
                        a.Val_OnOff = true;

                        } 
                    }
                   
          


                }

                Read_List.ExitWriteLock();
            });


            //接收消息更新列表变量值
            Messenger.Register<Socket_Modesl_Byte,string >(this, nameof(Meg_Value_Eunm.Socket_Read_List), (O, _Byte) =>
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



                            Messenger.Send<Socket_Models_List, string>(a, Socket_Read_List[i].Send_Area);


                           return;

                        }

                    }

            });


        }






        /// <summary>
        /// 集合列表更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Socket_Read_List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {


            //if (Socket_Read_Thread!=null )
            //{
                
            //    Read_List_Lock.WaitOne  ();

            //}
  



    }



        /// <summary>
        /// 写入锁
        /// </summary>
        public static ManualResetEvent Read_List_Lock = new ManualResetEvent(false );
        public static ReaderWriterLockSlim Read_List = new ReaderWriterLockSlim();

        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        private static  ObservableCollection<Socket_Models_List> _Socket_Read_List=new ObservableCollection<Socket_Models_List> ();

        public static  ObservableCollection<Socket_Models_List> Socket_Read_List
        {
            get 
            {

  

                return _Socket_Read_List; 
    


   
            }
            set 
            {

               

                _Socket_Read_List =  value;


            }
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


                //try
                //{


                  Delay_time = DateTime.Now;


                    if (Socket_Read_List.Count > 0)
                    {

                //Read_List_Lock.Reset();
                Read_List.EnterWriteLock();


                        for (int i = 0; i < Socket_Read_List.Count; i++)
                        {




                            //当前时间
                            if (Socket_Read_List[i].Val_OnOff )
                            {




                                Send_Waite.Reset();
                                int _ID = Socket_Read_List[i].Val_ID;
                                //重置发送等待标识
                                Send_Read.Reset();




                                //发送变量集合内容
                                Socket_Client_Setup.Read.Send_Read_Var(Socket_Read_List[i].Val_Name, _ID);


                                    //等待发送完,增加延时减少发送压力

                                    //Thread.Sleep(5);


                            if (Socket_Read_List[i].Value_One_Read == Read_Type_Enum.One_Read)
                            {

                                Socket_Read_List[i].Val_OnOff = false;

                            }

                                if (!Send_Waite.WaitOne(15000000,true ) || !Socket_Client_Setup.Read.Is_Read_Client)
                                {
                                    Socket_Client_Setup.Read.Socket_Receive_Error(Read_Write_Enum.Read, "发送超时无应答，退出线程发送！");
                                    return;
                                }


                             
     
                            }




                        }

                        //发送通讯延迟
                       Messenger.Send<string,string >((DateTime.Now - Delay_time).TotalMilliseconds.ToString().Split('.')[0], nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));








                   //Read_List_Lock.Set ();

                    Read_List.ExitWriteLock();
                }


                //    }
                //catch (Exception e)
                //{
                //    //异常处理
                //    User_Log_Add($"Error: -08 原因:" + e.Message);
                //    //User_Log_Add("-1.5，退出发送线程");
                //    //Clear_List();
                //    return;
                //}
                //finally
                //{

                ////Read_List_Lock.ExitWriteLock();
                //}




            }




        }




        /// <summary>
        /// 清除读取集合内容
        /// </summary>
        public void Clear_List()
        {

        }


    }
}
