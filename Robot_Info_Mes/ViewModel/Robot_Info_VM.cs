

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
        }



        public Texte_Model  Model {set;get;}=new  ();






        public Socket_Robot_Info_Parameters_Model Robot_Info_Parameters { set; get; } = new Socket_Robot_Info_Parameters_Model() { };



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
                        //Socket_ErrorInfo_delegate = Socket_Log_Show,
                        //Socket_Receive_Meg = Vision_Socked_Receive_information.Data_Converts_Str_Method,
                        //Socket_Send_Meg = Vision_Socked_Send_information.Data_Converts_Str_Method,
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





        /// <summary>
        /// 全局使用输出方法
        /// </summary>
        public void User_Log_Add(string Log, MessageBoxImage _MessType= MessageBoxImage.None)
        {

            Task.Run(() =>
            {


                try
                {

                    User_Log.User_Log = Log;

               Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(Log, "操作提示....", MessageBoxButton.OK, _MessType); });

                }
                catch (Exception e)
                {

                    Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(e.Message, "操作提示....", MessageBoxButton.OK, _MessType); });

                }

            });





        }







    }


}
