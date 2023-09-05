
using MVS_SDK_Base.Model;
using static HanGao.ViewModel.UC_Vision_Calibration_Image_VM;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Calibration_Results_VM : ObservableRecipient
    {

        public UC_Vision_Calibration_Results_VM()
        {

        }












        /// <summary>
        /// 检测标定图像数据集识别情况
        /// </summary>
        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {

                    HCalibData _CalibSetup_ID = new HCalibData();

                    if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, 1) >= 1)
                    {

                        foreach (var _Calib in Calibration_Image_List)
                        {

                           
                            HObject _CalibCoord = new HObject();
                            HXLDCont _CalibXLD = new HXLDCont();

                            if (_Calib.Camera_0.Calibration_Image!=null)
                            {

                         Halcon_Method. FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, _CalibSetup_ID, (HImage)_Calib.Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma);
                            _Calib.Camera_0.Calibration_Region= _CalibXLD.CopyObj(1, -1);
                            _Calib.Camera_0.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                _Calib.Camera_0.Calibration_State= Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();
                            }

                            if (_Calib.Camera_1.Calibration_Image != null)
                            {
                                Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, _CalibSetup_ID, (HImage)_Calib.Camera_1.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma);
                                _Calib.Camera_1.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                _Calib.Camera_1.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                _Calib.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();

                            }
                        }



                    }







                });

            });
        }




    }
}
