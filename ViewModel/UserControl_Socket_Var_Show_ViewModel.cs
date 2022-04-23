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


       



            IsActive = true;
            //开始读取集合发送线程
            Messenger.Register<dynamic ,string >(this,nameof( Meg_Value_Eunm.Socket_Read_Thread), (O,_Bool) =>
            {
                //if (_Bool)
                //{
                //    Socket_Read_Thread = new Thread(Receive_Read_Theam) { Name = "Read", IsBackground = true };
                //    Socket_Read_Thread.Start();
                //    User_Log_Add("启动变量发送线程");

                //}


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
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
            
                        Socket_Read_List.Add(item);
                        }));



                    }

                }






            });




            Messenger.Register<ObservableCollection<Socket_Models_List>,string >(this, nameof(Meg_Value_Eunm.One_List_Connect),(O,_List) =>
            {

                //ObservableCollection<Socket_Models_List> O_List = new ObservableCollection<Socket_Models_List>();

                List_Lock.Reset();


                            Socket_Client_Setup.One_Read.Socket_Client_Thread(Read_Write_Enum.One_Read, Socket_Client_Setup.IP, Socket_Client_Setup.Port);

                foreach (var item in _List)
                {

                    if (!On_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {

                            On_Read_List.Add(item);

                        //On_Read_List= O_List;
                        }));


                            Socket_Client_Setup.One_Read.Send_Read_Var(new Socket_Models_Receive() { Read_Write_Type = Read_Write_Enum.One_Read, Reveice_Target_Inf = item });
                        List_Lock.WaitOne(1000);
                    }
                }

               //Socket_Client_Setup.One_Read.Socket_Client_Thread(Read_Write_Enum.One_Read, Socket_Client_Setup.IP, Socket_Client_Setup.Port);









            });


            //接收消息更新列表变量值,弃用
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
        /// 写入锁
        /// </summary>
        // public static ManualResetEvent Read_List_Lock = new ManualResetEvent(false );
        public static ManualResetEvent List_Lock = new ManualResetEvent(false   );
        public static ReaderWriterLockSlim Read_List = new ReaderWriterLockSlim();


        public static ObservableCollection<Socket_Models_List> On_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>(); 


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
        public  static  void Receive_Read_Theam(Socket_Models_Receive _Socket_Receive_Inf)
        {

            var aa = _Socket_Receive_Inf.Read_Write_Type;


                    if (Socket_Read_List.Count > 0 && Socket_Client_Setup.Read.Is_Read_Client)
                    {
               

                 DateTime Delay_time = DateTime.Now;

                        for (int i = 0; i < Socket_Read_List.Count; i++)
                        {

                            //当前时间
                            if (Socket_Read_List[i].Val_OnOff )
                            {

                               //重置发生等待阻塞
                                Send_Waite.Reset();
                                Send_Read.Reset();



                                int _ID = Socket_Read_List[i].Val_ID;
                                //重置发送等待标识

                        //将需要发送的变量信息写入回调参数忠
                            _Socket_Receive_Inf.Reveice_Target_Inf = Socket_Read_List[i];
                        
                        //发送变量集合内容

                        //if (Socket_Read_List[i].Value_One_Read== Read_Type_Enum.One_Read)
                        //{

                        //    //_Socket_Receive_Inf.Read_Write_Type = Read_Write_Enum.One_Read;
                        //    //Socket_Client_Setup.One_Read.Send_Read_Var(_Socket_Receive_Inf);
                        //    ;  Socket_Read_List[i].Val_OnOff = false;

                        //}

                        Socket_Client_Setup.Read.Send_Read_Var(_Socket_Receive_Inf);

                        //等待发送完,增加延时减少发送压力

                        //Thread.Sleep(5);

                        


                        if ( !Socket_Client_Setup.Read.Is_Read_Client)
                                {
                                    Socket_Client_Setup.Read.Socket_Receive_Error(Read_Write_Enum.Read, "发送超时无应答，退出线程发送！");
                                    return;
                                }


                             
     
                            }




                        }

                var Socke_Time = (DateTime.Now - Delay_time).TotalMilliseconds;
                if (Socke_Time > 0)
                {

                    //发送通讯延迟
                    WeakReferenceMessenger.Default.Send<string,string >(Socke_Time.ToString().Split('.')[0], nameof(Meg_Value_Eunm.Connter_Time_Delay_Method));
               
                }







                //Read_List_Lock.Set ();

                //Read_List.ExitWriteLock();
            }



            //List_Lock.Set();
                //Thread.Sleep(5);



            //Socket_Models_List _List;
            //do
            //{



            // _List = Socket_Read_List.FirstOrDefault(L => L.Value_One_Read == Read_Type_Enum.One_Read);
            //if (_List != null)
            //{ 
            //Application.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    Socket_Read_List.Remove(_List);
            //}));
            //}

            //} while ( _List !=null);
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



            Receive_Read_Theam(_Socket_Receive_Inf);


         }


        




        /// <summary>
        /// 清除读取集合内容
        /// </summary>
        public void Clear_List()
        {

        }


    }
}
