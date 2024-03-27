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
            ///通讯解析必备参数
            Socket_Robot = _robo;
            Receice_byte = new List<byte>(_receive);
            Vision_Model_Type = Socket_Get_Vision_Model();
        }

        private KUKA_EKL_Socket_Protocols KUKA_Socket_Protocols { get; set; } = new KUKA_EKL_Socket_Protocols();

        private ABB_PC_Socket_Protocols ABB_Socket_Protocols { set; get; } = new ABB_PC_Socket_Protocols();



        /// <summary>
        /// 通讯对接机器人类型
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; } = Socket_Robot_Protocols_Enum.通用;


        /// <summary>
        /// 视觉接收协议模式
        /// </summary>
        public Vision_Model_Enum Vision_Model_Type { set; get; } = Vision_Model_Enum.Vision_Ini_Data;



        /// <summary>
        /// 接收数据原始byte
        /// </summary>
        public List<byte> Receice_byte = new List<byte>();

        /// <summary>
        /// 解析头部数据是属于那个方法
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Vision_Model_Enum Socket_Get_Vision_Model()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:


                    //提取接收内容解析
                    XElement _KUKA_Receive = XElement.Parse(Encoding.UTF8.GetString(Receice_byte.ToArray()));
                    return Enum.Parse<Vision_Model_Enum>(_KUKA_Receive.Attribute("Model")!.Value.ToString());


                case Socket_Robot_Protocols_Enum.ABB:


                    //装换接收总数字节
                    var INI = BitConverter.ToInt16(Receice_byte.Skip(0).Take(2).ToArray());

                    if (INI != Receice_byte.Count() - 2)
                    {
                        throw new Exception("通讯协议存在丢包，请检查网络！");
                    }

                    //装换接收功能
                    var mode = BitConverter.ToInt16(Receice_byte.Skip(2).Take(2).ToArray());


                    if (!Enum.IsDefined((Vision_Model_Enum)mode))
                    {
                        throw new Exception("通讯协议无该功能码，请联系开发者！");
                    }


                    return (Vision_Model_Enum)mode;




                case Socket_Robot_Protocols_Enum.川崎:


                    //**********
                    break;
                case Socket_Robot_Protocols_Enum.通用:

                    //**********

                    break;

            }


            return Vision_Model_Enum.Calibration_New;

        }






        /// <summary>
        /// 通讯数据汇总方法解析
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

                case Vision_Model_Enum.Vision_Creation_Model:

                    return (T1)(Object)Vision_Creation_Model_Receive_Protocol();



                default:
                    throw new Exception("现有通讯协议无法解析，请联系开发者！");

            }


            return default;


        }


        /// <summary>
        /// 通讯数据汇总发生解析
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[]? Socket_Send_Set_Data<T1>(T1 _Propertie)
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

                    return Vision_Find_Send_Protocol((_Propertie as Vision_Find_Data_Send)!);

                case Vision_Model_Enum.Vision_Ini_Data:

                    return Vision_Ini_Send_Procotol((_Propertie as Vision_Ini_Data_Send)!);

                case Vision_Model_Enum.HandEye_Calib_Date:

                    return HandEye_Calibration_Send_Protocol((_Propertie as HandEye_Calibration_Send)!);


                case Vision_Model_Enum.Vision_Creation_Model:
                    return Vision_Creation_Model_Send_Procotol((_Propertie as Vision_Creation_Model_Send)!);


                default:
                    throw new Exception("现有通讯协议无法解析，请联系开发者！");

            }


            return default;
        }



        /// <summary>
        /// 手眼标定通讯解压协议解析
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 手眼标定发送协议
        /// </summary>
        /// <param name="_Propertie"></param>
        /// <returns></returns>
        private byte[] HandEye_Calibration_Send_Protocol(HandEye_Calibration_Send _Propertie)
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:

                    var bt = Encoding.UTF8.GetBytes(KUKA_Send_Receive_Xml.Property_Xml<HandEye_Calibration_Send>(_Propertie)); 

                    return bt;
                case Socket_Robot_Protocols_Enum.ABB:

                    List<byte> _Send_ABB_Byte = new List<byte>();

                    //装换数据
                    var st = BitConverter.GetBytes(_Propertie.IsStatus);
                    var mes = Encoding.UTF8.GetBytes(_Propertie.Message_Error);
                    var mes_num = BitConverter.GetBytes(mes.Length);
                    var xx = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.X)));
                    var yy = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Y)));
                    var zz = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Z)));
                    var Rxx = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Rx)));
                    var Ryy = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Ry)));
                    var Rzz = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Rz)));



                    //拼接

                    _Send_ABB_Byte.AddRange(st);
                    _Send_ABB_Byte.AddRange(mes_num);
                    _Send_ABB_Byte.AddRange(mes);
                    _Send_ABB_Byte.AddRange(xx);
                    _Send_ABB_Byte.AddRange(yy);
                    _Send_ABB_Byte.AddRange(zz);
                    _Send_ABB_Byte.AddRange(Rxx);
                    _Send_ABB_Byte.AddRange(Ryy);
                    _Send_ABB_Byte.AddRange(Rzz);


                    var _num = BitConverter.GetBytes(_Send_ABB_Byte.Count);

                    _Send_ABB_Byte.InsertRange(0, _num);
                    return _Send_ABB_Byte.ToArray();
                case Socket_Robot_Protocols_Enum.川崎:



                    break;
                case Socket_Robot_Protocols_Enum.通用:



                    break;

            }








            return Array.Empty<byte>();
        }




        /// <summary>
        /// 视觉创建模式协议发送方法
        /// </summary>
        /// <param name="_Propertie"></param>
        /// <returns></returns>
        private byte[] Vision_Creation_Model_Send_Procotol(Vision_Creation_Model_Send _Propertie)
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:



                    break;
                case Socket_Robot_Protocols_Enum.ABB:


                    List<byte> _Send_ABB_Byte = new List<byte>();

                    //装换数据
                    var st = BitConverter.GetBytes(_Propertie.IsStatus);
                    var mes = Encoding.UTF8.GetBytes(_Propertie.Message_Error);
                    var mes_num = BitConverter.GetBytes(mes.Length);
                    var xx = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.X)));
                    var yy = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Y)));
                    var zz = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Z)));
                    var Rxx = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Rx)));
                    var Ryy = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Ry)));
                    var Rzz = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Rz)));



                    //拼接

                    _Send_ABB_Byte.AddRange(st);
                    _Send_ABB_Byte.AddRange(mes_num);
                    _Send_ABB_Byte.AddRange(mes);
                    _Send_ABB_Byte.AddRange(xx);
                    _Send_ABB_Byte.AddRange(yy);
                    _Send_ABB_Byte.AddRange(zz);
                    _Send_ABB_Byte.AddRange(Rxx);
                    _Send_ABB_Byte.AddRange(Ryy);
                    _Send_ABB_Byte.AddRange(Rzz);


                    var _num = BitConverter.GetBytes(_Send_ABB_Byte.Count);

                    _Send_ABB_Byte.InsertRange(0, _num);
                    return _Send_ABB_Byte.ToArray();
                case Socket_Robot_Protocols_Enum.川崎:



                    break;
                case Socket_Robot_Protocols_Enum.通用:



                    break;

            }

            return Array.Empty<byte>();
        }

        /// <summary>
        /// 视觉创建模式协议接受方法
        /// </summary>
        /// <returns></returns>
        private Vision_Creation_Model_Receive Vision_Creation_Model_Receive_Protocol()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:



                    break;
                case Socket_Robot_Protocols_Enum.ABB:

                    Vision_Creation_Model_Receive _ABB_Creation_Model_Rece = new Vision_Creation_Model_Receive();


                    //解析协议
                    //int _Pos_Model = BitConverter.ToInt16(Receice_byte.Skip(4).Take(2).ToArray());

                    //if (!Enum.IsDefined((Vision_Creation_Model_Pos_Enum)_Pos_Model))
                    //{
                    //    throw new Exception("通讯协议无该功能码，请联系开发者！");
                    //}

                    int _Robot_Type = BitConverter.ToInt16(Receice_byte.Skip(4).Take(2).ToArray());

                    if (!Enum.IsDefined((Robot_Type_Enum)_Robot_Type))
                    {
                        throw new Exception("通讯协议无该机器人类型，请联系开发者！");
                    }


                    var xx = Receice_byte.Skip(6).Take(4).ToArray();
                    var yy = Receice_byte.Skip(10).Take(4).ToArray();
                    var zz = Receice_byte.Skip(14).Take(4).ToArray();
                    var Rxx = Receice_byte.Skip(18).Take(4).ToArray();
                    var Ryy = Receice_byte.Skip(22).Take(4).ToArray();
                    var Rzz = Receice_byte.Skip(26).Take(4).ToArray();
                    var xxx = Receice_byte.Skip(30).Take(4).ToArray();
                    var yyy = Receice_byte.Skip(34).Take(4).ToArray();
                    var zzz = Receice_byte.Skip(38).Take(4).ToArray();
                    var Rxxx = Receice_byte.Skip(42).Take(4).ToArray();
                    var Ryyy = Receice_byte.Skip(46).Take(4).ToArray();
                    var Rzzz = Receice_byte.Skip(50).Take(4).ToArray();
                    double cx = BitConverter.ToSingle(xx);
                    double cy = BitConverter.ToSingle(yy);
                    double cz = BitConverter.ToSingle(zz);
                    double cRx = BitConverter.ToSingle(Rxx);
                    double cRy = BitConverter.ToSingle(Ryy);
                    double cRz = BitConverter.ToSingle(Rzz);
                    double ox = BitConverter.ToSingle(xxx);
                    double oy = BitConverter.ToSingle(yyy);
                    double oz = BitConverter.ToSingle(zzz);
                    double oRx = BitConverter.ToSingle(Rxxx);
                    double oRy = BitConverter.ToSingle(Ryyy);
                    double oRz = BitConverter.ToSingle(Rzzz);

                    _ABB_Creation_Model_Rece.Vision_Model = Vision_Model_Type;
                  
                    _ABB_Creation_Model_Rece.Robot_Type = (Robot_Type_Enum)_Robot_Type;
                    _ABB_Creation_Model_Rece.Camera_Pos.X = Math.Round(cx, 4).ToString();
                    _ABB_Creation_Model_Rece.Camera_Pos.Y = Math.Round(cy, 4).ToString();
                    _ABB_Creation_Model_Rece.Camera_Pos.Z = Math.Round(cz, 4).ToString();
                    _ABB_Creation_Model_Rece.Camera_Pos.Rx = Math.Round(cRx, 4).ToString();
                    _ABB_Creation_Model_Rece.Camera_Pos.Ry = Math.Round(cRy, 4).ToString();
                    _ABB_Creation_Model_Rece.Camera_Pos.Rz = Math.Round(cRz, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.X = Math.Round(ox, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.Y = Math.Round(oy, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.Z = Math.Round(oz, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.Rx = Math.Round(oRx, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.Ry = Math.Round(oRy, 4).ToString();
                    _ABB_Creation_Model_Rece.Origin_Pos.Rz = Math.Round(oRz, 4).ToString();


                    return _ABB_Creation_Model_Rece;
                case Socket_Robot_Protocols_Enum.川崎:



                    break;
                case Socket_Robot_Protocols_Enum.通用:



                    break;

            }


            return new Vision_Creation_Model_Receive();

        }


        /// <summary>
        /// 视觉初始化数据接收协议解析
        /// </summary>
        /// <returns></returns>
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




        private byte[]? Vision_Ini_Send_Procotol(Vision_Ini_Data_Send _Propertie)
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

            return Array.Empty<byte>();
        }



        private byte[] Vision_Find_Send_Protocol(Vision_Find_Data_Send _Propertie)
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

            return Array.Empty<byte>();
        }

        /// <summary>
        /// 视觉查找数据接收解析
        /// </summary>
        /// <returns></returns>
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







    }
}
