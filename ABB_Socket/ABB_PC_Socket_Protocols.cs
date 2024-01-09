
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


                    int _Calib_Model = BitConverter.ToInt32(Receice_byte.Skip(5).Take(4).ToArray());

                    var xx = Receice_byte.Skip(9).Take(4).ToArray();
                    var yy = Receice_byte.Skip(13).Take(4).ToArray();
                    var zz = Receice_byte.Skip(17).Take(4).ToArray();
                    var Rxx = Receice_byte.Skip(21).Take(4).ToArray();
                    var Ryy = Receice_byte.Skip(25).Take(4).ToArray();
                    var Rzz = Receice_byte.Skip(29).Take(4).ToArray();
                    double x = BitConverter.ToSingle(xx);
                    double y = BitConverter.ToSingle(yy);
                    double z = BitConverter.ToSingle(zz);
                    double Rx = BitConverter.ToSingle(Rxx);
                    double Ry = BitConverter.ToSingle(Ryy);
                    double Rz = BitConverter.ToSingle(Rzz);





                    break;


            }



            return Array.Empty<byte>();
        }


    }
}
