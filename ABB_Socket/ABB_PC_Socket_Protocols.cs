
using Halcon_SDK_DLL.Model;

namespace ABB_Socket
{
    public  class ABB_PC_Socket_Protocols
    {
        public ABB_PC_Socket_Protocols()
        {

        }

        public byte[] ABB_PC_Socket<T1>(byte[] Receice_byte)
        {

            Vision_Model_Enum _Model =  Vision_Model_Enum.HandEye_Calib_Date;


            switch (_Model)
            {
                case Vision_Model_Enum.Calibration_New:
        

                    break;
                case Vision_Model_Enum.Calibration_Text:
    
                    break;

                case Vision_Model_Enum.Find_Model:

    
                    break;

                case Vision_Model_Enum.Vision_Ini_Data:

           



                    break;

                case Vision_Model_Enum.HandEye_Calib_Date:


                    //HandEye_Calibration_Receive _HandEye_Calib_Rece = new HandEye_Calibration_Receive();







                    break;


            }



            return Array.Empty<byte>();
        }


    }
}
