using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using HanGao.Xml_Date.Xml_Write_Read;



namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Auto_Model_ViewModel : ObservableRecipient
    {
        public UC_Vision_Auto_Model_ViewModel()
        {



            //读取存储参数文件
            Vision_Auto_Cofig_Model _Date = new Vision_Auto_Cofig_Model();
            Read_Xml_File(ref _Date);
            Vision_Auto_Cofig = _Date;

            //视觉接收设置参数
            Static_KUKA_Receive_Vision_Ini_String += (Vision_Ini_Data_Receive _S, string _RStr) =>
            { 
                UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;

                Vision_Ini_Data_Send _Send = new Vision_Ini_Data_Send();


                _Send.IsStatus = 1;
                _Send.Initialization_Data.Vision_Scope = Vision_Auto_Cofig.Vision_Scope.ToString();
                _Send.Message_Error = HVE_Result_Enum.Vision_Ini_Data_OK.ToString ();
                //属性转换xml流
                string _SendSteam = KUKA_Send_Receive_Xml.Property_Xml(_Send);
                UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _SendSteam;
                return _SendSteam;
            };

            Initialization_Sever_Start();
        }



        /// <summary>
        /// 库卡通讯服务器属性
        /// </summary>
        public List<Socket_Receive> KUKA_Receive { set; get; } = new List<Socket_Receive>();

  
        /// <summary>
        /// 电脑网口设备IP网址
        /// </summary>
        public ObservableCollection<string> Local_IP_UI { set; get; }






        /// <summary>
        /// 静态委托接收处理相机标定事件
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Calibration_New_String { set; get; }
        /// <summary>
        /// 静态委托接收处理相机标定点添加事件
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Calibration_Add_String { set; get; }
        /// <summary>
        /// 静态委托接收处理相机标定精度测试事件
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Calibration_Text_String { set; get; }
        /// <summary>
        /// 静态委托处理查找模型特征
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Find_String { set; get; }
        /// <summary>
        /// 静态委托处理查找模型特征
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Vision_Ini_Data_Receive, string> Static_KUKA_Receive_Vision_Ini_String { set; get; }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        private static   Vision_Auto_Cofig_Model _Vision_Auto_Cofig { set; get; } = new Vision_Auto_Cofig_Model();

        /// <summary>
        /// 视觉自动参数属性
        /// </summary>
        public static Vision_Auto_Cofig_Model Vision_Auto_Cofig
        {

            get
            {
                return _Vision_Auto_Cofig;
            }
            set
            {
                _Vision_Auto_Cofig = value;
                //OnStaticPropertyChanged();
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Vision_Auto_Cofig)));

            }
        }





        /// <summary>
        /// 服务器开始状态
        /// </summary>
        public bool Receive_Start_Type { set; get; } = true;


        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Server_End_Comm
        {
            get => new RelayCommand<UC_Vision_Auto_Model>((Sm) =>
            {
                if (Receive_Start_Type)
                {

                    Initialization_Sever_Start();
                    Receive_Start_Type = false;

                }
                else
                {
                    Initialization_Sever_STOP();
                    Receive_Start_Type = true;
                }

            });
        }


        public ICommand Save_Config_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {


                Vision_Xml_Method.Save_Xml(Vision_Auto_Cofig);


            });
        }

        /// <summary>
        /// 初始化服务器全部ip启动
        /// </summary>
        public void Initialization_Sever_Start()
        {


            List<string> _List = new List<string>();
            if (Socket_Receive.GetLocalIP(ref _List))
            {


                Local_IP_UI = new ObservableCollection<string>(_List) { };


                ///启动服务器添加接收事件
                foreach (var _Sever in Local_IP_UI)
                {

                    KUKA_Receive.Add(new Socket_Receive(_Sever, Vision_Auto_Cofig.Stat_Network_Port.ToString()) {
                        KUKA_Receive_Calibration_New_String = Static_KUKA_Receive_Calibration_New_String,
                        KUKA_Receive_Calibration_Add_String=Static_KUKA_Receive_Calibration_Add_String,
                        KUKA_Receive_Calibration_Text_String=Static_KUKA_Receive_Calibration_Text_String,
                        KUKA_Receive_Find_String = Static_KUKA_Receive_Find_String, 
                        KUKA_Receive_Vision_Ini_String=Static_KUKA_Receive_Vision_Ini_String,
                        Socket_ErrorInfo_delegate = User_Log_Add });

                }



                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Receive_Start_Type = false;
                User_Log_Add("开启所有网络服务器设备端口:"+ Vision_Auto_Cofig.Stat_Network_Port.ToString());

            }

        }


        /// <summary>
        /// 初始化服务器全部停止
        /// </summary>
        public void Initialization_Sever_STOP()
        {

            foreach (var _Sock in KUKA_Receive)
            {

              
                _Sock.Sever_End();
            }
            User_Log_Add("停止所有服务器连接!");


        }




    }




}
