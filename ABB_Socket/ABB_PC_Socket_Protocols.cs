
using Halcon_SDK_DLL.Model;

namespace ABB_Socket
{
    public  class ABB_PC_Socket_Protocols
    {
        public ABB_PC_Socket_Protocols()
        {

        }

        public byte[] ABB_PC_Socket<T1>(byte[] _Str)
        {

            Vision_Model_Enum _Model =  Vision_Model_Enum.HandEye_Calib_Date;


            switch (_Model)
            {
                case Vision_Model_Enum.Calibration_New:
                    //Calibration_Data_Receive _Calibration_New_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                    //_Str = Receive_Calibration_New_String(_Calibration_New_Receive, _St);

                    break;
                case Vision_Model_Enum.Calibration_Text:
                    //Calibration_Data_Receive _Calibration_Text_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                    //_Str = Receive_Calibration_Text_String(_Calibration_Text_Receive, _St);

                    break;

                case Vision_Model_Enum.Find_Model:

                    //Calibration_Data_Receive _Find_Receive = KUKA_Send_Receive_Xml.String_Xml<Calibration_Data_Receive>(_St);

                    //_Str = KUKA_Receive_Find_String(_Find_Receive, _St);
                    break;

                case Vision_Model_Enum.Vision_Ini_Data:

                    //Vision_Ini_Data_Receive _Vision_Receive = KUKA_Send_Receive_Xml.String_Xml<Vision_Ini_Data_Receive>(_St);

                    //_Str = KUKA_Receive_Vision_Ini_String(_Vision_Receive, _St);



                    break;

                case Vision_Model_Enum.HandEye_Calib_Date:

                    //HandEye_Calibration_Receive _HandEye_Receive = KUKA_Send_Receive_Xml.String_Xml<HandEye_Calibration_Receive>(_St);

                    //_Str = HandEye_Calibration_String(_HandEye_Receive, _St);

                    break;


            }



            return Array.Empty<byte>();
        }


    }
}
