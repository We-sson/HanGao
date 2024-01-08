using System.Xml.Linq;
using Halcon_SDK_DLL.Model;

namespace KUKA_Socket
{
    public  class KUKA_EKL_Socket_Protocols
    {


        public KUKA_EKL_Socket_Protocols()
        {

        }




        /// <summary>
        /// 视觉功能模式
        /// </summary>
        /// <param name="_St"></param>
        /// <returns></returns>
        public string KUKA_EKL_Socket<T1>(string _St)
        {
            if (_St != "")
            {

                //提取接收内容解析
                XElement _KUKA_Receive = XElement.Parse(_St);
                Vision_Model_Enum _Model = Enum.Parse<Vision_Model_Enum>(_KUKA_Receive.Attribute("Model")!.Value.ToString());

                //string _Str = "";
                //将对应的功能反序列化处理
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

                return string.Empty;
            }
            else
            {
                return string.Empty;
            }

        }










    }



}
