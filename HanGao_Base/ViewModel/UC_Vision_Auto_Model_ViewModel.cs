
using HanGao.View.User_Control.Vision_hand_eye_Calibration;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;

using KUKA_Socket;
using Roboto_Socket_Library;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;
using static Roboto_Socket_Library.Socket_Receive;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Auto_Model_ViewModel : ObservableRecipient
    {
        public UC_Vision_Auto_Model_ViewModel()
        {



            ////读取存储参数文件
            //Vision_Auto_Config_Model _Date = new Vision_Auto_Config_Model();
            //Read_Xml_File(ref _Date);
            //Vision_Auto_Cofig = _Date;

            ////视觉接收设置参数
            //Static_KUKA_Receive_Vision_Ini_String += (Vision_Ini_Data_Receive _S) =>
            //{ 
            //    //UC_Vision_Robot_Protocol_ViewModel.Receive_Socket_String = _RStr;

            //    Vision_Ini_Data_Send _Send = new Vision_Ini_Data_Send();


            //    _Send.IsStatus = 1;
            //    _Send.Initialization_Data.Vision_Scope = Vision_Auto_Cofig.Vision_Scope.ToString();
            //    _Send.Message_Error = HVE_Result_Enum.Vision_Ini_Data_OK.ToString ();
            //    //属性转换xml流
            //    string _SendSteam = KUKA_Send_Receive_Xml.Property_Xml(_Send);
             
            //    UC_Vision_Robot_Protocol_ViewModel.Send_Socket_String = _SendSteam;
            //    //return _SendSteam;
            //    return _Send;
            //};

            //Initialization_Sever_Start();
        }








    }




}
