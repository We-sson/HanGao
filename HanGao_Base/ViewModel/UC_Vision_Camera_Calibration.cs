

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Camera_Calibration : ObservableRecipient
    {
        public UC_Vision_Camera_Calibration() { }








        public static Halcon_Camera_Calibration_Model Halcon_Calibration_Setup { get; set; } = new Halcon_Camera_Calibration_Model();




        public static HCalibData Halcon_CalibSetup_ID { set; get; } = new HCalibData() { };


        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button Window_UserContol = Sm.Source as Button;

                Halcon_Calibration_Start(Halcon_Calibration_Setup);


            });
        }


        public static void Halcon_Calibration_Start(Halcon_Camera_Calibration_Model _Parameters)
        {
            ///读取标定相机数量
            int _camer_number = Set_Camera_Calibration_Par(UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List);

            if (_camer_number > 0)
            {


            }


        }


        private static int Set_Camera_Calibration_Par(ObservableCollection<MVS_Camera_Info_Model> _camerLits)
        {
            int _camera_number = 0;



            //设置使用的摄像机类型
            foreach (var _camera in _camerLits)
            {
                if (_camera.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Emun.Start_Calibration)
                {
                    HCamPar _CamPar = new HCamPar();
                    _CamPar.SetCalibDataCamParam(Halcon_CalibSetup_ID, _camera_number, Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));
                    Halcon_CalibSetup_ID.SetCalibDataCamParam(_camera_number, new HTuple(), _CamPar);
                    _camera_number++;


                }
            }
            //设置使用过的校准对象
            Halcon_CalibSetup_ID.SetCalibDataCalibObject(Halcon_Calibration_Setup.Haclon_Calibration_number, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address.FullName);


            return _camera_number;
        }



    }
}
