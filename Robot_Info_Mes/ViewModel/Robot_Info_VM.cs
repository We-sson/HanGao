

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using PropertyChanged;
using Robot_Info_Mes.Model;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using SkiaSharp;
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
    public class Robot_Info_VM : ObservableRecipient
    {

        public Robot_Info_VM()
        {
            ///初始化
            ///
            Initialization_Local_Network_Robot_Socket();

            Welding_Power_Series = new ObservableCollection<ISeries>(Welding_Power_Data.AddValue(Weding_Power_Val, "%").BuildSeries());
        }



        public Texte_Model Model { set; get; } = new();






        public Socket_Robot_Info_Parameters_Model Robot_Info_Parameters { set; get; } = new Socket_Robot_Info_Parameters_Model() { };

        public GaugeBuilder Welding_Power_Data { get; set; } =
new GaugeBuilder()
{
    LabelsSize = 25,
    InnerRadius = 55,
    BackgroundInnerRadius = 55,
    LabelsPosition = PolarLabelsPosition.ChartCenter,
    Background = new SolidColorPaint(new SKColor(222, 222, 222)),
}.WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}");

        public ObservableCollection<ISeries> Welding_Power_Series { get; set; }  

        public ObservableValue Weding_Power_Val { set; get; } = new ObservableValue { Value = 50 };

        /// <summary>
        /// 消息显示
        /// </summary>
        public User_Log_Models User_Log { set; get; } = new User_Log_Models();

        /// <summary>
        /// 初始化启动视觉通讯是否开启
        /// </summary>
        public void Initialization_Local_Network_Robot_Socket()
        {


            Initialization_Sever_Start();
            User_Log_Add("开启所有IP服务器连接：" + Robot_Info_Parameters.Sever_Socket_Port.ToString());


        }




        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void Initialization_Sever_Start()
        {
            List<string> _List = [];
            if (Socket_Receive.GetLocalIP(ref _List))
            {
                Robot_Info_Parameters.Local_IP_UI = new ObservableCollection<string>(_List) { };

                ///启动服务器添加接收事件
                foreach (var _Sever in Robot_Info_Parameters.Local_IP_UI)
                {
                    Robot_Info_Parameters.Receive_List.Add(new Socket_Receive(_Sever, Robot_Info_Parameters.Sever_Socket_Port.ToString())
                    {
                        Socket_Robot = Robot_Info_Parameters.Socket_Robot_Model,
                        //Vision_Ini_Data_Delegate = Robot_Info_Parameters,

                        //Vision_Find_Model_Delegate = Vision_Find_Shape_Receive_Method,
                        Mes_Info_Model_Data_Delegate= Robot_Mes_Info_Receive_Method,
                        Socket_ErrorInfo_delegate = Socket_Log_Show,
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
                        Initialization_Sever_Start();
                        User_Log_Add("开启所有IP服务器连接：" + Robot_Info_Parameters.Sever_Socket_Port.ToString());

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

        public Robot_Mes_Info_Data_Send Robot_Mes_Info_Receive_Method(Robot_Mes_Info_Data_Receive _Receive)
        {
            Robot_Mes_Info_Data_Send _Send = new ();



            _Send.Socket_Polling_Time =(int)(Robot_Info_Parameters.Socket_Polling_Time*1000);
            _Send.IsStatus = 1;
            //if (Convert.ToBoolean(_Receive.Calibration))
            //{


            //    _Send = Vision_Calibration_Model_Receive_Method(_Receive);

            //}
            //else
            //{
            //    _Send = Vision_Find_Data_Receive_Method(_Receive);
            //}



            return _Send;
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



        public  void Socket_Log_Show(string _log)
        {
            User_Log_Add(_log);
        }



    }


}
