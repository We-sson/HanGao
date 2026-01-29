

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using Robot_Info_Mes.Model;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;

namespace Robot_Info_Mes.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public partial class Robot_Info_VM : ObservableRecipient
    {

        public Robot_Info_VM()
        {
            ///初始化
            ///

            if (!IsInDesignMode)
            {


                Window_Version = Application.ResourceAssembly.GetName().Version!.ToString();


                File_Int_Parameters = File_Xml_Model.Read_Xml_File<File_Int_Model>();



                switch (File_Int_Parameters.Window_Startup_Type)
                {
                    case Window_Startup_Type_Enum.Server:




                        ///使用默认xml格式接受信息
                        File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model = Socket_Robot_Protocols_Enum.KUKA;

                        ///看板端
                        Initialization_Mes_Sever_Start();


                        Int_Server_Run_Time();

                        Int_Server_KanBan_View_Data();

                        break;
                    case Window_Startup_Type_Enum.Client:

                        ///接受机器人信息

                        Mes_Robot_Info_Model_Data = File_Xml_Model.Read_Xml_File<Mes_Robot_Info_Model>();
                        Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                        //  Initialization_Local_Network_Robot_Socket();


                        Initialization_Robot_Sever_Start();

                        Int_Run_TIme();


                        User_Log_Add("已读取本机设备信息文件！" + File_Xml_Model.GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path));
                        User_Log_Add("已经初始化软件！" + File_Xml_Model.GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.File_Path));



                        break;

                }





            }


        }
        /// <summary>
        /// 开发设计状态
        /// </summary>
        private bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }


        /// <summary>
        /// 系统版本
        /// </summary>
        public string Window_Version { set; get; } = string.Empty;




        /// <summary>
        /// 页面报表控件显示
        /// </summary>
        public Work_Factor_Seried_Model Work_Factor_Seried { set; get; } = new Work_Factor_Seried_Model();










        //public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();




        /// <summary>
        /// 机器人通讯交互参数
        /// </summary>
        public Socket_Robot_Info_Parameters_Model Robot_Info_Parameters { set; get; } = new Socket_Robot_Info_Parameters_Model() { };




        /// <summary>
        /// 看板服务器参数
        /// </summary>
        public Socket_Mes_Info_Parameters_Model Mes_Info_Parameters { set; get; } = new Socket_Mes_Info_Parameters_Model();





        /// <summary>
        /// 上位机接受机器人参数
        /// </summary>
        public Mes_Robot_Info_Model Mes_Robot_Info_Model_Data { set; get; } = new();


        /// <summary>
        /// 文件初始化参数
        /// </summary>
        public File_Int_Model File_Int_Parameters { set; get; } = new();




        /// <summary>
        /// 看板设备列表
        /// </summary>
        public ObservableCollection<Mes_Server_Info_List_Model> Mes_Server_Model_List { set; get; } = new ObservableCollection<Mes_Server_Info_List_Model>();


        /// <summary>
        /// 看板选中列
        /// </summary>
        public Mes_Server_Info_List_Model Mes_Server_Model_Select { set; get; } = new Mes_Server_Info_List_Model();



        /// <summary>
        /// 看板循环查看列号
        /// </summary>
        public int Mes_Server_Model_List_View { set; get; } = 0;



        /// <summary>
        ///服务器启动轮播操作
        /// </summary>
        public ICommand Server_Window_Scrolling_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ListBox? _Contol = Sm!.Source as ListBox;
                try
                {



                    //ScrollViewer? ScrollViewer = _Contol!.Template.FindName("PART_ScrollViewer", _Contol) as ScrollViewer;

                    ///看板总列表轮播
                    _Contol!.ScrollIntoView(Mes_Server_Model_List[Mes_Server_Model_List_View]);
                    _Contol.SelectedIndex = Mes_Server_Model_List_View;



                    DispatcherTimer _timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.KanBan_ALLList_Cycle_View_Time)
                    };
                    _timer.Tick += (s, e) =>
                    {




                        Mes_Server_Model_List_View++;


                        if (Mes_Server_Model_List.Count == 0) return;
                        if (Mes_Server_Model_List_View > Mes_Server_Model_List.Count - 1)
                        {
                            Mes_Server_Model_List_View = 0;
                        }





                        _Contol!.ScrollIntoView(Mes_Server_Model_List[Mes_Server_Model_List_View]);
                        _Contol.SelectedIndex = Mes_Server_Model_List_View;








                    };
                    _timer.Start();








                    //User_Log_Add("保存设置参数成功！重启软件生效。");




                }
                catch (Exception _e)
                {

                    User_Log_Add(_e.Message, MessageBoxImage.Error);

                }



            });
        }





        /// <summary>
        /// 消息显示
        /// </summary>
        public User_Log_Models User_Log { set; get; } = new User_Log_Models();

        /// <summary>
        /// 初始化启动视觉通讯是否开启
        /// </summary>
        public void Initialization_Local_Network_Robot_Socket()
        {


            User_Log_Add("开启所有IP服务器连接：" + File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port.ToString());


        }


        /// <summary>
        /// 初始化机器人运行时间
        /// </summary>
        public void Int_Run_TIme()
        {

            //重启软件后当天时间继续计时
            Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Error_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Error_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Debug_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Debug_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Work_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_UI;
            Mes_Robot_Info_Model_Data.Robot_Run_Time.Time_Offset = Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_UI;

            Mes_Robot_Info_Model_Data.Robot_Run_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Offline_Time.Reset();






            //Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.AutoReset = true;
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time);
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Tick += (s, e) =>
            {


                //当程序连续开的时候经过12点清除数据
                Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                ///记录文件保存时间
                Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                ///计算可用稼动率
                Work_Factor_Seried.Work_Availability_Factor.Value = Work_Factor_Seried.Get_Work_Availability_Factor(Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Millisecond, Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_Millisecond);


                //计算性能稼动率
                Work_Factor_Seried.Work_Performance_Factor.Value = Work_Factor_Seried.Get_Work_Performance_Factor(File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Sec);
                ///保存时间文件



                File_Xml_Model.Save_Xml(Mes_Robot_Info_Model_Data);


                User_Log_Add("信息文件定时已到：" + File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time + "s，进行文件保存！");


            };
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Start();


            ///周期发送看板更新
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.Sever_Cycle_Update_Time);
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Tick += (s, e) =>
            //{


            //    //User_Log_Add("信息文件定时已到：" + Mes_Run_Parameters.Socket_Polling_Time + "s，进行文件保存！");


            //};
            //Mes_Robot_Info_Model_Data.Server_Cycle_Update_Data.Start();



            Socket_Cycle_KanBan_Info();

            ///汇总上传信息
            Mes_Info_Parameters.Socket_Client.Mes_Receive_Info_Data_Delegate = Mes_Receive_Info_Data_Method;
            Mes_Info_Parameters.Socket_Client.Socket_ErrorInfo_delegate = Socket_Cycle_Update_ErrorLog_Show;
            Mes_Info_Parameters.Socket_Client.Socket_ConnectInfo_delegate = Socket_ConnectLog_Show;
            Mes_Info_Parameters.Socket_Client.Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method;
            Mes_Info_Parameters.Socket_Client.Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method;
        }



        /// <summary>
        /// 设置看板时间参数
        /// </summary>
        public void Int_Server_KanBan_View_Data()
        {

            foreach (var item in Mes_Server_Model_List)
            {
                item.Work_Factor_Seried.KanBan_List_Cycle_View_Time = File_Int_Parameters.Mes_Run_Parameters.KanBan_List_Cycle_View_Time;
                item.Work_Factor_Seried.Mes_Data_View_Int();
            }


        }



        public void Int_Server_Run_Time()
        {



            Mes_Server_Model_List = File_Xml_Model.Read_Xml_File<ObservableCollection<Mes_Server_Info_List_Model>>();






            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Interval = TimeSpan.FromSeconds(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time);
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Tick += (s, e) =>
            {


                //当程序连续开的时候经过12点清除数据
                //Mes_Robot_Info_Model_Data.Check_Day_Int_Time();

                ///记录文件保存时间
                //Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                ///计算可用稼动率
                //Work_Factor_Seried.Work_Availability_Factor.Value = Work_Factor_Seried.Get_Work_Availability_Factor(Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Millisecond, Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_Millisecond);


                ////计算性能稼动率
                //Work_Factor_Seried.Work_Performance_Factor.Value = Work_Factor_Seried.Get_Work_Performance_Factor(File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Sec);
                /////保存时间文件



                ///检查通讯超时保存时间的更新断开连接
                foreach (var item in Mes_Server_Model_List)
                {
                    if ((DateTime.Now - item.Mes_Robot_Info_Model_Data.Socket_Last_Update_Time).TotalSeconds > File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time)
                    {
                        item.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;
                    }

                }

                File_Xml_Model.Save_Xml(new ObservableCollection<Mes_Server_Info_List_Model>(Mes_Server_Model_List));


                User_Log_Add("信息文件定时已到：" + File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time + "s，进行文件保存！");


            };
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Start();








        }

        /// <summary>
        /// 通讯循环周期发送看板信息
        /// </summary>
        public void Socket_Cycle_KanBan_Info()
        {
            Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                while (true)
                {


                    //if (Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected)
                    //{


                    if ((Mes_Info_Parameters.Socket_Client.Socket_Client) == null || (!(bool?)(Mes_Info_Parameters.Socket_Client.Socket_Client?.Connected) ?? false))
                    {

                        Mes_Info_Parameters.Socket_Client.Connect(File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_IP, File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_Port);
                        //连接成功释放一下信号
                        Mes_Info_Parameters.Socket_Client.Rece_Event.Set();

                    }

                    if (((bool?)(Mes_Info_Parameters.Socket_Client.Socket_Client?.Connected) ?? false) && Mes_Info_Parameters.Socket_Client.Rece_Event.WaitOne())
                    {
                        Mes_Info_Parameters.Socket_Client.Rece_Event.Reset();
                        //增加验收，避免过快通讯
                        Thread.Sleep((int)File_Int_Parameters.Mes_Run_Parameters.Sever_Cycle_Update_Time * 1000);
                        ///发送完成一次后复位信号

                        Mes_Server_Info_Data_Receive _Send = new Mes_Server_Info_Data_Receive()
                        {
                            Vision_Model = Vision_Model_Enum.Mes_Server_Info_Send_Data,
                            Socket_Update_Time = DateTime.Now,
                            Robot_Mes_Info_Data = Mes_Robot_Info_Model_Data.Robot_Info_Data,
                            Mes_Server_Date = new Mes_Server_Date_Model()
                            {
                                Robot_Debug_All_Time = Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI,
                                Robot_Error_All_Time = Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Timer_UI,
                                Robot_Run_All_Time = Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI,
                                Robot_Work_All_Time = Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Timer_UI,

                                Robot_Debug_Time = Mes_Robot_Info_Model_Data.Robot_Debug_Time.Timer_UI,
                                Robot_Error_Time = Mes_Robot_Info_Model_Data.Robot_Error_Time.Timer_UI,
                                Robot_Run_Time = Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_UI,
                                Robot_Work_Time = Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_UI,

                                Robot_Work_ABCD_Number = Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number,
                                Robot_Work_AB_Cycle = Mes_Robot_Info_Model_Data.Robot_Work_AB_Cycle.Timer_UI,
                                Robot_Work_CD_Cycle = Mes_Robot_Info_Model_Data.Robot_Work_CD_Cycle.Timer_UI,

                                Work_Availability_Factor = Work_Factor_Seried.Work_Availability_Factor.Value ?? 0,
                                Work_Cycle_Load_Factor = Work_Factor_Seried.Work_Cycle_Load_Factor.Value ?? 0,
                                Work_Performance_Factor = Work_Factor_Seried.Work_Performance_Factor.Value ?? 0,


                                ///工艺时间参数
                                Robot_Work_ABCD_Number_Max = File_Int_Parameters.Mes_Standard_Time.Robot_Work_ABCD_Number_Max,
                                Robot_Work_Time_Max = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours,

                                Work_Standard_Time = File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time,
                                Work_Availability_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Availability_Factor_Max,
                                Work_Performance_Factor_Max = File_Int_Parameters.Mes_Standard_Time.Work_Performance_Factor_Max,


                                Robot_Work_ABCD_Cycle_Mean = Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Cycle_Mean,


                            }


                        };


                        Mes_Info_Parameters.Socket_Client.Send_Val<Mes_Server_Info_Data_Receive>(Socket_Robot_Protocols_Enum.KUKA, Vision_Model_Enum.Mes_Server_Info_Rece_Data, _Send, (int)File_Int_Parameters.Mes_Run_Parameters.Mes_Server_Info_Rece_Time * 1000);



                    }


                }

                //}

            });




        }



        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void Initialization_Robot_Sever_Start()
        {
            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Robot_Info_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                ///启动服务器添加接收事件
                foreach (var _Sever in Robot_Info_Parameters.Local_IP_UI)
                {
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port)
                    {
                        Socket_Robot = File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model,

                        Robot_Info_Model_Data_Delegate = Robot_Mes_Info_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_Robot_ErrorLog_Show,
                        Socket_ConnectInfo_delegate = Socket_ConnectLog_Show,
                        Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method,
                        Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method,
                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Robot_Info_Parameters.Sever_IsRuning = true;
            }

        }






        public void Initialization_Mes_Sever_Start()
        {
            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Robot_Info_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                ///启动服务器添加接收事件
                foreach (var _Sever in Robot_Info_Parameters.Local_IP_UI)
                {
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, File_Int_Parameters.Mes_Run_Parameters.Sever_Mes_Info_Port.ToString())
                    {
                        Socket_Robot = File_Int_Parameters.Mes_Run_Parameters.Socket_Robot_Model,

                        Mes_Server_Info_Data_Delegate = Mes_Server_Info_Data_Method,
                        Socket_ErrorInfo_delegate = Socket_Mes_ErrorLog_Show,
                        Socket_ConnectInfo_delegate = Socket_ConnectLog_Show,

                        Socket_Receive_Meg = Robot_Info_Parameters.Receive_information.Data_Converts_Str_Method,
                        Socket_Send_Meg = Robot_Info_Parameters.Send_information.Data_Converts_Str_Method,
                    });
                }

                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Robot_Info_Parameters.Sever_IsRuning = true;
            }

        }


        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Server_End_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ToggleButton? _Contol = Sm!.Source as ToggleButton;
                try
                {




                    if (Robot_Info_Parameters.Sever_IsRuning)
                    {
                        Initialization_Robot_Sever_Start();
                        User_Log_Add("开启所有IP服务器连接：" + File_Int_Parameters.Mes_Run_Parameters.Sever_Socket_Port.ToString());

                    }
                    else
                    {
                        //Initialization_Sever_STOP();
                        Robot_Info_Parameters.Server_List_End();
                        User_Log_Add("停止所有IP服务器连接!", MessageBoxImage.Stop);

                    }
                }
                catch (Exception _e)
                {
                    Robot_Info_Parameters.Sever_IsRuning = false;
                    _Contol!.IsChecked = false;
                    User_Log_Add("开启服务器接受失败！原因：" + _e.Message, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Save_Date_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button? _Contol = Sm!.Source as Button;
                try
                {



                    File_Xml_Model.Save_Xml(File_Int_Parameters);


                    User_Log_Add("保存设置参数成功！重启软件生效。");




                }
                catch (Exception _e)
                {

                    User_Log_Add("保存设置参数失败！原因：" + _e.Message, MessageBoxImage.Error);

                }



            });
        }


        /// <summary>
        /// 接受机器人得信息进行处理
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Robot_Mes_Info_Data_Send Robot_Mes_Info_Receive_Method(Robot_Mes_Info_Data_Receive _Receive, Socket? _socket)
        {


            lock (Mes_Robot_Info_Model_Data)
            {
                Robot_Mes_Info_Data_Send _Send = new();


                Set_Robot_Info_Data(_Receive);








                _Send.Socket_Polling_Time = (int)(File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time * 1000);
                _Send.IsStatus = 1;


                return _Send;
            }

        }



        /// <summary>
        /// 看板接受各个上位机的机器人信息进行更新显示
        /// </summary>
        /// <param name="_Receive"></param>
        /// <returns></returns>
        public Mes_Server_Info_Data_Send Mes_Server_Info_Data_Method(Mes_Server_Info_Data_Receive _Receive, Socket? _Socket)
        {
            Mes_Server_Info_Data_Send _Send = new();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            lock (Mes_Server_Model_List)
            {


                foreach (var _Server in Mes_Server_Model_List)
                {


                    if (_Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Robot_Process_Int == _Receive.Robot_Mes_Info_Data.Robot_Process_Int)
                    {
                        //保存连接IP
                        _Server.Connetc_Mes_IP = (IPEndPoint?)_Socket!.RemoteEndPoint;

                        ///指标更新
                        _Server.Work_Factor_Seried.Work_Cycle_Load_Factor.Value = _Receive.Mes_Server_Date.Work_Cycle_Load_Factor;
                        _Server.Work_Factor_Seried.Work_Availability_Factor.Value = _Receive.Mes_Server_Date.Work_Availability_Factor;
                        _Server.Work_Factor_Seried.Work_Performance_Factor.Value = _Receive.Mes_Server_Date.Work_Performance_Factor;

                        //计算通讯周期
                        _Server.Mes_Robot_Info_Model_Data.Socket_Cycle_Time = DateTime.Now - _Server.Mes_Robot_Info_Model_Data.Socket_Last_Update_Time;

                        _Server.Mes_Robot_Info_Model_Data.Socket_Last_Update_Time = DateTime.Now;
                        _Server.Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_AB_Cycle.Timer_UI = _Receive.Mes_Server_Date.Robot_Work_AB_Cycle;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_CD_Cycle.Timer_UI = _Receive.Mes_Server_Date.Robot_Work_CD_Cycle;


                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number = _Receive.Mes_Server_Date.Robot_Work_ABCD_Number;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Cycle_Mean = _Receive.Mes_Server_Date.Robot_Work_ABCD_Cycle_Mean;


                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Work_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Debug_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Debug_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Error_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Error_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Run_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Run_Time;


                        _Server.Mes_Robot_Info_Model_Data.Robot_Work_All_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Work_All_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Debug_All_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Debug_All_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Error_All_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Error_All_Time;
                        _Server.Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Timer_UI = _Receive.Mes_Server_Date.Robot_Run_All_Time;


                        _Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Mes_Programs_Name = _Receive.Robot_Mes_Info_Data.Mes_Programs_Name;


                        _Server.Mes_Robot_Info_Model_Data.Robot_Info_Data.Mes_Robot_Mode = _Receive.Robot_Mes_Info_Data.Mes_Robot_Mode;

                        ///工艺参数写入
                        //_Server.Work_Factor_Seried.Work_Cycle_Load_Factor_Max= new ObservableValue(  _Receive.Mes_Server_Date.Work_Standard_Time );
                        //_Server.Work_Factor_Seried.Robot_Work_ABCD_Number_Max = new ObservableValue(_Receive.Mes_Server_Date.Robot_Work_ABCD_Number_Max);
                        //_Server.Work_Factor_Seried.Robot_Work_Time_Max = new ObservableValue(_Receive.Mes_Server_Date.Robot_Work_Time_Max);
                        //_Server.Work_Factor_Seried.Work_Availability_Factor_Max = new ObservableValue(_Receive.Mes_Server_Date.Work_Availability_Factor_Max);
                        //_Server.Work_Factor_Seried.Work_Performance_Factor_Max = new ObservableValue(_Receive.Mes_Server_Date.Work_Performance_Factor_Max);


                        _Server.Work_Factor_Seried.Mes_Data_View_List_Add(_Receive);
                        _Server.Work_Factor_Seried.Mes_Data_View_Max_Add(_Receive);








                        _Server.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;

                    }



                }


                ///更新列表排序
                for (int i = 0; i < Mes_Server_Model_List.Count; i++)
                {
                    if (Mes_Server_Model_List[i].Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected)
                    {
                        ///把已经连接的项目往上走
                        for (int _i = 0; _i < Mes_Server_Model_List.Count; _i++)
                        {
                            if (Mes_Server_Model_List[_i].Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected)
                            {
                                ///只能往上排序
                                if (i > _i)
                                {

                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Mes_Server_Model_List.Move(i, _i);
                                    });

                                }

                            }

                        }

                    }



                }


            }


            return _Send;


        }



        /// <summary>
        /// 看板信息
        /// </summary>
        /// <param name="_Receive"></param>
        public void Mes_Receive_Info_Data_Method(Mes_Server_Info_Data_Send _Receive)
        {




            User_Log_Add($"上传看板信息更新时间：{_Receive.Socket_Update_Time}");


            ///接受完成消息在发送信息
            Mes_Info_Parameters.Socket_Client.Rece_Event.Set();


            //Thread.Sleep(100);

        }



        /// <summary>
        /// 设置机器人信息数据
        /// </summary>
        /// <param name="_Data"></param>
        public void Set_Robot_Info_Data(Robot_Mes_Info_Data_Receive _Data)
        {
            ///计算通讯周期耗时
            Mes_Robot_Info_Model_Data.Socket_Cycle_Time = DateTime.Now - Mes_Robot_Info_Model_Data.Socket_Last_Update_Time;

            Mes_Robot_Info_Model_Data.Socket_Last_Update_Time = DateTime.Now;



            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;




            Mes_Robot_Info_Model_Data.Robot_Info_Data = new Robot_Mes_Info_Data_Receive(_Data);








            Work_Factor_Seried.Work_Cycle_Load_Factor.Value = Work_Factor_Seried.Get_Work_Cycle_Load_Factor(_Data.Robot_Process_Int,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_A_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_B_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_C_Cycle,
                                                                                                                                                                               ref Mes_Robot_Info_Model_Data.Robot_Work_D_Cycle,
                                                                                                                                                                                 File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time);



        }


        private readonly object _userLogLock = new();

        /// <summary>
        /// 全局使用输出方法
        /// </summary>


        public void User_Log_Add(string log, MessageBoxImage messType = MessageBoxImage.None)
        {

            try
            {
         Application.Current.Dispatcher.BeginInvoke(() =>
            {
                lock (_userLogLock)
                {
                    User_Log.User_Log = log;

                    if (messType != MessageBoxImage.None)
                    {
                        MessageBox.Show(log, "操作提示....", MessageBoxButton.OK, messType);
                    }
                }
            });



            }
            catch (Exception)
            {

               ///出现报错放弃
            }
        }




        /// <summary>
        /// 通讯错误信息
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_Robot_ErrorLog_Show(string _log, Socket? _Socket)
        {


            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;


            User_Log_Add(_log);
        }


        /// <summary>
        /// 看板通讯错误显示
        /// </summary>
        /// <param name="_log"></param>
        /// <param name="_Socket"></param>
        public void Socket_Cycle_Update_ErrorLog_Show(string _log, Socket? _Socket)
        {

            try
            {

                Mes_Info_Parameters.Socket_Client?.Socket_Client?.Disconnect(false);


                User_Log_Add(_log);

            }
            catch (Exception e)
            {

                User_Log_Add(_log + e.Message);
            }

        }

        /// <summary>
        /// 看板服务器通讯断开信息
        /// </summary>
        /// <param name="_log"></param>
        /// <param name="_Socket"></param>
        public void Socket_Mes_ErrorLog_Show(string _log, Socket? _Socket)
        {
            //加锁防止外部修改列表
            lock (Mes_Server_Model_List)
            {


            foreach (var _Server in Mes_Server_Model_List)
            {
                ///如果是当前连接的服务器
                if (_Socket != null)
                {

                    if (_Server.Connetc_Mes_IP == (IPEndPoint?)_Socket.RemoteEndPoint)
                    {
                        _Server.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;

                    }

                }

            }

            }

            User_Log_Add(_log);
        }


        /// <summary>
        /// 通讯连接信息
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_ConnectLog_Show(string _log, Socket? _Socket)
        {


            User_Log_Add(_log);
        }


    }


}
