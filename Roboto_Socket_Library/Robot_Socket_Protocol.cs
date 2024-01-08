using ABB_Socket;
using Halcon_SDK_DLL.Model;
using KUKA_Socket;
using Roboto_Socket_Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Roboto_Socket_Library
{
    public  class Robot_Socket_Protocol
    {

        public Robot_Socket_Protocol(Socket_Robot_Protocols_Enum _robo, byte[] _receive ) 
        {
            Socket_Robot = _robo;
            Receice_byte = new List<byte>(_receive);

            Vision_Model_Type= Socket_Get_Vision_Model();
        }

        private  KUKA_EKL_Socket_Protocols KUKA_Socket_Protocols { get; set; } = new KUKA_EKL_Socket_Protocols();

        private  ABB_PC_Socket_Protocols ABB_Socket_Protocols { set; get; } = new ABB_PC_Socket_Protocols();


        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; } = Socket_Robot_Protocols_Enum.通用;

        private  Vision_Model_Enum Vision_Model_Type { set; get; } = Vision_Model_Enum.Vision_Ini_Data;


        public List<byte> Receice_byte = new List<byte>();

        public byte[] Socket_Protocols<T1>(byte[] _Str, Socket_Robot_Protocols_Enum Socket_Robot)
        {

            Vision_Model_Enum _Model = Vision_Model_Enum.HandEye_Calib_Date;







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

                   // HandEye_Calibration_Receive _HandEye_Receive = KUKA_Send_Receive_Xml.String_Xml<HandEye_Calibration_Receive>(_St);

                   // _Str = HandEye_Calibration_Data_Delegate(_HandEye_Receive, _St);





                    break;


            }



            return Array.Empty<byte>();
        }




        private  Vision_Model_Enum Socket_Get_Vision_Model()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:




                    break;
                case Socket_Robot_Protocols_Enum.ABB:


                 var aa=   Encoding.UTF8.GetString(Receice_byte.ToArray(),0, 1);
                  var ee=  Encoding.UTF8.GetString(Receice_byte.ToArray(), Receice_byte.Count-1, 1);


                    if (aa=="[" && ee=="]")
                    {
                        var tt = Receice_byte.Skip(1).Take(4).ToArray();
                      int e=  BitConverter.ToInt32(tt);

                        return (Vision_Model_Enum)e;
                    }else
                    {
                        throw new Exception("通讯协议无法解析，请联系开发者！");
                    }

                case Socket_Robot_Protocols_Enum.川崎:
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    break;
   
            }




            return Vision_Model_Enum.Calibration_New;

        }







        public Type? Get_Data_Type()
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





                    break;
                case Vision_Model_Enum.Vision_Ini_Data:

                    return typeof(Vision_Ini_Data_Receive);


        
                case Vision_Model_Enum.HandEye_Calib_Date:


                    return typeof(HandEye_Calibration_Receive);

             
            }


            return default;
        }


        public T1 Socket_Receive_Get_Date<T1>()
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
                    break;
                case Vision_Model_Enum.Vision_Ini_Data:


                    Vision_Ini_Data_Receive _Vision_In_Data = new Vision_Ini_Data_Receive();

                    //*******Convert

                    return (T1)(Object)_Vision_In_Data;
                case Vision_Model_Enum.HandEye_Calib_Date:

                    HandEye_Calibration_Receive _HandEye_Calib_Rece = new HandEye_Calibration_Receive();


                    int _Calib_Model = BitConverter.ToInt32(Receice_byte.Skip(5).Take(4).ToArray());

                     var xx=   Receice_byte.Skip(9).Take(4).ToArray();
                    var yy= Receice_byte.Skip(13).Take(4).ToArray();
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


                    _HandEye_Calib_Rece.Calibration_Model = (HandEye_Calibration_Type_Enum)_Calib_Model;


                    return (T1)(Object)_HandEye_Calib_Rece;

                
        
            }




            return  (T1)new  Object();




        }


        public byte[]? Socket_Send_Set_Data<T1>(T1 _Send_Data)
        {




            return Array.Empty<byte>();
        }


    }
}
