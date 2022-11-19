using HanGao.View.User_Control.Vision_Control;
using KUKA_Socket.Models;
using Ookii.Dialogs.Wpf;
using Soceket_KUKA;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Auto_Model_ViewModel : ObservableRecipient
    {
        public UC_Vision_Auto_Model_ViewModel()
        {


            Local_IP_UI = new ObservableCollection<string>(KUKA_Receive.GetLocalIP()) { };

            KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(),Local_Port_UI);
            Receive_Start_Type = false ;



            KUKA_Receive.KUKA_Receive_String += (string _S) =>
            {

                Calibration_Data_Receive _Receive= KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_S);
                if (_Receive.Model == Vision_Model_Enum.Find_Model)
                {




                }


                return default;

            };


        }



        public static   Socket_Receive KUKA_Receive { set; get; } = new Socket_Receive();

        public KUKA_Send_Receive_Xml KUKA_Xml { set; get; } = new KUKA_Send_Receive_Xml();


        public ObservableCollection<string > Local_IP_UI { set; get; }


        public int IP_UI_Select { set; get; } = 1;

        public string Local_Port_UI { set; get; } = "5000";


        public KUKA_Send_Receive_Xml KUKA_Send_Receive { set; get; } = new KUKA_Send_Receive_Xml() { };


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
                    KUKA_Receive.Server_Strat(Local_IP_UI[IP_UI_Select].ToString(), Local_Port_UI);
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
