using HanGao.View.User_Control.Vision_Control;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Auto_Model_ViewModel : ObservableRecipient
    {
        public UC_Vision_Auto_Model_ViewModel()
        {


            Local_IP_UI = new ObservableCollection<string>(KUKA_Receive.GetLocalIP()) { };

            KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(),Local_Port_UI.ToString());
            Receive_Start_Type = false ;





        }


        /// <summary>
        /// 库卡通讯服务器属性
        /// </summary>
        public static   Socket_Receive KUKA_Receive { set; get; } = new Socket_Receive();

        //public KUKA_Send_Receive_Xml KUKA_Xml { set; get; } = new KUKA_Send_Receive_Xml();

        /// <summary>
        /// 电脑网口设备IP网址
        /// </summary>
        public ObservableCollection<string > Local_IP_UI { set; get; }

        /// <summary>
        ///默认开启网络端口号
        /// </summary>
        public int IP_UI_Select { set; get; } = 1;


        /// <summary>
        /// 服务其网络端口
        /// </summary>
        public int  Local_Port_UI { set; get; } = 5000;




        //public KUKA_Send_Receive_Xml KUKA_Send_Receive { set; get; } = new KUKA_Send_Receive_Xml() { };


        public bool Receive_Start_Type { set; get; } = true ;


        /// <summary>
        /// 模板存储位置选择
        /// </summary>
        public ICommand Server_End_Comm
        {
            get => new RelayCommand<UC_Vision_Auto_Model>((Sm) =>
            {
                if (Receive_Start_Type)
                {
                    KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI.ToString ());
                    Receive_Start_Type = false ;

                }
                else
                {
                KUKA_Receive.Sever_End();

                Receive_Start_Type = true ;
                }

            });
        }




    }
}
