

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using Robot_Info_Mes.Model;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using Application = System.Windows.Application;
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
            Initialization_File_Info();

            switch (File_Int_Parameters.Window_Startup_Type)
            {
                case Window_Startup_Type_Enum.Server:



                    break;
                case Window_Startup_Type_Enum.Client:
                    Initialization_Local_Network_Robot_Socket();

                    Int_Run_TIme();


                    break;

            }



            //Welding_Power_Series = new ObservableCollection<ISeries>(Welding_Power_Data
            //   . AddValue(Weding_Power_Val, "%", new SolidColorPaint(new SKColor(75, 101, 153)))

            //    .BuildSeries());






        }





        public Work_Factor_Seried_Model Work_Factor_Seried { set; get; } = new Work_Factor_Seried_Model();







        public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();





        public Socket_Robot_Info_Parameters_Model Robot_Info_Parameters { set; get; } = new Socket_Robot_Info_Parameters_Model() { };




        public Mes_Robot_Info_Model Mes_Robot_Info_Model_Data { set; get; } = new();



        public File_Int_Model File_Int_Parameters { set; get; } = new();





        public ObservableCollection<Mes_Server_Info_List_Model> Mes_Server_Model_List { set; get; } =
            new ObservableCollection<Mes_Server_Info_List_Model>()
            {
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_7,
                    },
                      
                }
            },
                      new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_8,
                    },
                       
                }
            },
                new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.R_Side_9,
                    },
                           
                }
            },
             new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_7,
                    },
                        
                }
            },
            new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_8,
                    },
                           
                }
            },
             new ()
            {
                Mes_Robot_Info_Model_Data=new Mes_Robot_Info_Model(){
                    Robot_Info_Data=new  (){
                        Robot_Process_Int= Robot_Process_Int_Enum.Panel_Surround_9,
                    },
                      
                }
            },

            };







        /// <summary>
        /// 消息显示
        /// </summary>
        public User_Log_Models User_Log { set; get; } = new User_Log_Models();

        /// <summary>
        /// 初始化启动视觉通讯是否开启
        /// </summary>
        public void Initialization_Local_Network_Robot_Socket()
        {


            Initialization_Robot_Sever_Start();
            User_Log_Add("开启所有IP服务器连接：" + Mes_Run_Parameters.Sever_Socket_Port.ToString());


        }



        public void Int_Run_TIme()
        {

            Mes_Robot_Info_Model_Data.Robot_Run_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Run_All_Time.Start();
            Mes_Robot_Info_Model_Data.Robot_Offline_Time.Reset();






            //Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.AutoReset = true;
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Interval = TimeSpan.FromSeconds(Mes_Run_Parameters.Socket_Polling_Time);
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Tick += (s, e) =>
            {
                ///记录文件保存时间
                Mes_Robot_Info_Model_Data.File_Update_Time = DateTime.Now;
                ///计算可用稼动率
                Work_Factor_Seried.Work_Availability_Factor.Value = Work_Factor_Seried.Get_Work_Availability_Factor(Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Hours, File_Int_Parameters.Mes_Standard_Time.Work_Standard_Hours);


                //计算性能稼动率
                Work_Factor_Seried.Work_Performance_Factor.Value = Work_Factor_Seried.Get_Work_Performance_Factor(File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time, Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number, Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer_Sec);
                ///保存时间文件


                File_Xml_Model.Save_Xml(Mes_Robot_Info_Model_Data);


                User_Log_Add("信息文件定时已到：" + Mes_Run_Parameters.Socket_Polling_Time + "s，进行文件保存！");


            };
            Mes_Robot_Info_Model_Data.Socket_Cycle_Check_Update.Start();

        }

        /// <summary>
        /// 初始化文件读取
        /// </summary>
        public void Initialization_File_Info()
        {


            Mes_Robot_Info_Model_Data = File_Xml_Model.Read_Xml_File<Mes_Robot_Info_Model>();
            File_Int_Parameters = File_Xml_Model.Read_Xml_File<File_Int_Model>();

            ///检查数据是否当天有效
            Mes_Robot_Info_Model_Data.Check_Day_Int_Time();


            User_Log_Add("已读取本机设备信息文件！" + File_Xml_Model.GetXml_Path<File_Int_Model>(Get_Xml_File_Enum.File_Path));
            User_Log_Add("已经初始化软件！" + File_Xml_Model.GetXml_Path<Mes_Robot_Info_Model>(Get_Xml_File_Enum.File_Path));




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
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, Mes_Run_Parameters.Sever_Socket_Port.ToString())
                    {
                        Socket_Robot = Mes_Run_Parameters.Socket_Robot_Model,
                        //Vision_Ini_Data_Delegate = Robot_Info_Parameters,

                        //Vision_Find_Model_Delegate = Vision_Find_Shape_Receive_Method,
                        Mes_Info_Model_Data_Delegate = Robot_Mes_Info_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_ErrorLog_Show,
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
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, Mes_Run_Parameters.Sever_Socket_Port.ToString())
                    {
                        Socket_Robot = Mes_Run_Parameters.Socket_Robot_Model,
                        //Vision_Ini_Data_Delegate = Robot_Info_Parameters,

                        //Vision_Find_Model_Delegate = Vision_Find_Shape_Receive_Method,
                        Mes_Info_Model_Data_Delegate = Robot_Mes_Info_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_ErrorLog_Show,
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
                        User_Log_Add("开启所有IP服务器连接：" + Mes_Run_Parameters.Sever_Socket_Port.ToString());

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



        public Robot_Mes_Info_Data_Send Robot_Mes_Info_Receive_Method(Robot_Mes_Info_Data_Receive _Receive)
        {
            Robot_Mes_Info_Data_Send _Send = new();


            Set_Robot_Info_Data(_Receive);








            _Send.Socket_Polling_Time = (int)(Mes_Run_Parameters.Socket_Polling_Time * 1000);
            _Send.IsStatus = 1;


            return _Send;
        }



        public void Set_Robot_Info_Data(Robot_Mes_Info_Data_Receive _Data)
        {

            Mes_Robot_Info_Model_Data.Socket_Last_Update_Time = DateTime.Now;

            Mes_Robot_Info_Model_Data.Robot_Info_Data = new Robot_Mes_Info_Data_Receive(_Data);




            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;




            Work_Factor_Seried.Work_Cycle_Load_Factor.Value = Work_Factor_Seried.Get_Work_Cycle_Load_Factor(_Data.Robot_Process_Int,
                                                                                                                                                                                 Mes_Robot_Info_Model_Data.Robot_Work_A_Cycle,
                                                                                                                                                                                 Mes_Robot_Info_Model_Data.Robot_Work_B_Cycle,
                                                                                                                                                                                 Mes_Robot_Info_Model_Data.Robot_Work_C_Cycle,
                                                                                                                                                                                 Mes_Robot_Info_Model_Data.Robot_Work_D_Cycle,
                                                                                                                                                                                 File_Int_Parameters.Mes_Standard_Time.Work_Standard_Time);



        }



        /// <summary>
        /// 全局使用输出方法
        /// </summary>
        public void User_Log_Add(string Log, MessageBoxImage _MessType = MessageBoxImage.None)
        {

            Task.Run(() =>
            {


                try
                {

                    User_Log.User_Log = Log;


                    if (_MessType != MessageBoxImage.None)
                    {


                        Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(Log, "操作提示....", MessageBoxButton.OK, _MessType); });
                    }

                }
                catch (Exception e)
                {

                    Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(e.Message, "操作提示....", MessageBoxButton.OK, _MessType); });

                }

            });





        }


        /// <summary>
        /// 通讯错误信息
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_ErrorLog_Show(string _log)
        {


            Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;


            User_Log_Add(_log);
        }
        /// <summary>
        /// 通讯连接信息
        /// </summary>
        /// <param name="_log"></param>
        public void Socket_ConnectLog_Show(string _log)
        {


            User_Log_Add(_log);
        }


    }


}
