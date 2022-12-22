using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;
using static HanGao.ViewModel.User_Control_Log_ViewModel;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Auto_Model_ViewModel : ObservableRecipient
    {
        public UC_Vision_Auto_Model_ViewModel()
        {



            //读取存储文件参数
            Vision_Data _Date = new Vision_Data();
            Vision_Xml_Method.Read_Xml(ref _Date);
            Local_Port_UI = _Date.Stat_Network_Port;

            Initialization_Sever_Start();



        }





        /// <summary>
        /// 库卡通讯服务器属性
        /// </summary>
        public List<Socket_Receive> KUKA_Receive { set; get; } = new List<Socket_Receive>();

        //public KUKA_Send_Receive_Xml KUKA_Xml { set; get; } = new KUKA_Send_Receive_Xml();

        /// <summary>
        /// 电脑网口设备IP网址
        /// </summary>
        public ObservableCollection<string> Local_IP_UI { set; get; }

        /// <summary>
        ///默认开启网络端口号
        /// </summary>
        //public int IP_UI_Select { set; get; } = 1;




        /// <summary>
        /// 静态委托接收处理相机标定事件
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Calibration_String { set; get; }
        /// <summary>
        /// 静态委托处理查找模型特征
        /// </summary>
        public static Socket_Receive.ReceiveMessage_delegate<Calibration_Data_Receive, string> Static_KUKA_Receive_Find_String { set; get; }



        /// <summary>
        /// 服务其网络端口
        /// </summary>
        public int Local_Port_UI { set; get; } = 5000;




        //public KUKA_Send_Receive_Xml KUKA_Send_Receive { set; get; } = new KUKA_Send_Receive_Xml() { };

        //服务器开始状态
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

                    //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString ());
                    Receive_Start_Type = false;

                }
                else
                {
                    //KUKA_Receive.Sever_End();
                    Initialization_Sever_STOP();
                    Receive_Start_Type = true;
                }

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

                foreach (var _Sever in Local_IP_UI)
                {

                    KUKA_Receive.Add(new Socket_Receive(_Sever, Local_Port_UI.ToString()) { KUKA_Receive_Calibration_String = Static_KUKA_Receive_Calibration_String, KUKA_Receive_Find_String = Static_KUKA_Receive_Find_String, Socket_ErrorInfo_delegate = User_Log_Add });

                }



                //KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString());
                Receive_Start_Type = false;
                User_Log_Add("开启所有网络服务器设备端口:"+ Local_Port_UI .ToString());

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
