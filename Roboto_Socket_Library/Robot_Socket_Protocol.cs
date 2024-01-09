using ABB_Socket;
using Halcon_SDK_DLL.Model;
using KUKA_Socket;
using Roboto_Socket_Library.Model;
using System.Text;
using System.Xml.Linq;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Roboto_Socket_Library
{
    public class Robot_Socket_Protocol
    {

        public Robot_Socket_Protocol(Socket_Robot_Protocols_Enum _robo, byte[] _receive)
        {
            Socket_Robot = _robo;
            Receice_byte = new List<byte>(_receive);

            Vision_Model_Type = Socket_Get_Vision_Model();
        }

        private KUKA_EKL_Socket_Protocols KUKA_Socket_Protocols { get; set; } = new KUKA_EKL_Socket_Protocols();

        private ABB_PC_Socket_Protocols ABB_Socket_Protocols { set; get; } = new ABB_PC_Socket_Protocols();


        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; } = Socket_Robot_Protocols_Enum.通用;

        public Vision_Model_Enum Vision_Model_Type { set; get; } = Vision_Model_Enum.Vision_Ini_Data;


        public List<byte> Receice_byte = new List<byte>();

        public byte[] Socket_Protocols<T1>(byte[] _Str, Socket_Robot_Protocols_Enum Socket_Robot)
        {

            Vision_Model_Enum _Model = Vision_Model_Enum.HandEye_Calib_Date;







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







                    break;


            }



            return Array.Empty<byte>();
        }




        private Vision_Model_Enum Socket_Get_Vision_Model()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:


                    //提取接收内容解析
                    XElement _KUKA_Receive = XElement.Parse(Encoding.UTF8.GetString(Receice_byte.ToArray()));
                    return Enum.Parse<Vision_Model_Enum>(_KUKA_Receive.Attribute("Model")!.Value.ToString());


                case Socket_Robot_Protocols_Enum.ABB:


                    var aa = Encoding.UTF8.GetString(Receice_byte.ToArray(), 0, 1);
                    var ee = Encoding.UTF8.GetString(Receice_byte.ToArray(), Receice_byte.Count - 1, 1);


                    if (aa == "[" && ee == "]")
                    {
                        var tt = Receice_byte.Skip(1).Take(4).ToArray();
                        int e = BitConverter.ToInt32(tt);

                        return (Vision_Model_Enum)e;
                    }
                    else
                    {
                        throw new Exception("通讯协议无法解析，请联系开发者！");
                    }

                case Socket_Robot_Protocols_Enum.川崎:


                    //**********
                    break;
                case Socket_Robot_Protocols_Enum.通用:

                    //**********

                    break;

            }


            return Vision_Model_Enum.Calibration_New;

        }


        private Type? Get_Data_Type()
        {
            //T1? _Get_data = default;


            switch (Vision_Model_Type)
            {
                case Vision_Model_Enum.Calibration_New:
                    break;
                case Vision_Model_Enum.Calibration_Text:
                    break;
                case Vision_Model_Enum.Calibration_Add:
                    break;
                case Vision_Model_Enum.Find_Model:



                    return typeof(Vision_Find_Data_Receive);



                case Vision_Model_Enum.Vision_Ini_Data:

                    return typeof(Vision_Ini_Data_Receive);



                case Vision_Model_Enum.HandEye_Calib_Date:


                    return typeof(HandEye_Calibration_Receive);


            }


            return default;
        }


        public T1? Socket_Receive_Get_Date<T1>()
        {


            switch (Vision_Model_Type)
            {
                case Vision_Model_Enum.Calibration_New:
                    break;
                case Vision_Model_Enum.Calibration_Text:
                    break;
                case Vision_Model_Enum.Calibration_Add:
                    break;
                case Vision_Model_Enum.Find_Model:

                    return (T1)(Object)Vision_Find_Receive_Protocol;

                case Vision_Model_Enum.Vision_Ini_Data:

                    return (T1)(Object)Vision_Ini_Receive_Protocol();

                case Vision_Model_Enum.HandEye_Calib_Date:

                    return (T1)(Object)HandEye_Calibration_Receive_Protocol();

                default:
                    throw new Exception("现有通讯协议无法解析，请联系开发者！");

            }


            return default;


        }

        private HandEye_Calibration_Receive HandEye_Calibration_Receive_Protocol()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:


                    HandEye_Calibration_Receive _Kuka_HandEye_Calib_Rece = KUKA_Send_Receive_Xml.String_Xml<HandEye_Calibration_Receive>(Encoding.UTF8.GetString(Receice_byte.ToArray()));


                    return _Kuka_HandEye_Calib_Rece;


                case Socket_Robot_Protocols_Enum.ABB:


                    HandEye_Calibration_Receive _ABB_HandEye_Calib_Rece = new HandEye_Calibration_Receive();

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



                    _ABB_HandEye_Calib_Rece.Vision_Model = Vision_Model_Type;
                    _ABB_HandEye_Calib_Rece.Calibration_Model = (HandEye_Calibration_Type_Enum)_Calib_Model;
                    _ABB_HandEye_Calib_Rece.ACT_Point.X = Math.Round(x, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Y = Math.Round(y, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Z = Math.Round(z, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Rx = Math.Round(Rx, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Ry = Math.Round(Ry, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Rz = Math.Round(Rz, 4).ToString();



                    return _ABB_HandEye_Calib_Rece;
                case Socket_Robot_Protocols_Enum.川崎:




                    break;
                case Socket_Robot_Protocols_Enum.通用:




                    break;

            }




            return new HandEye_Calibration_Receive();

        }


        private Vision_Ini_Data_Receive Vision_Ini_Receive_Protocol()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:



                    break;
                case Socket_Robot_Protocols_Enum.ABB:



                    break;
                case Socket_Robot_Protocols_Enum.川崎:



                    break;
                case Socket_Robot_Protocols_Enum.通用:



                    break;

            }


            return new Vision_Ini_Data_Receive();

        }


        private Vision_Find_Data_Receive Vision_Find_Receive_Protocol()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:



                    break;
                case Socket_Robot_Protocols_Enum.ABB:



                    break;
                case Socket_Robot_Protocols_Enum.川崎:



                    break;
                case Socket_Robot_Protocols_Enum.通用:



                    break;

            }


            return new Vision_Find_Data_Receive();

        }


        public byte[]? Socket_Send_Set_Data<T1>(T1 _Send_Data)
        {




            return Array.Empty<byte>();
        }


    }
}
