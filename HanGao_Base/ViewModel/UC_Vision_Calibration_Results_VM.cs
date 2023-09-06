
using MVS_SDK_Base.Model;
using static HanGao.ViewModel.UC_Vision_Calibration_Image_VM;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration;
using static HanGao.ViewModel.User_Control_Log_ViewModel;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Calibration_Results_VM : ObservableRecipient
    {

        public UC_Vision_Calibration_Results_VM()
        {

        }






        /// <summary>
        /// Halcon标定参数设置句柄
        /// </summary>
        public static HCalibData Halcon_CalibSetup_ID { set; get; } = new HCalibData() { };




        public Calibration_Camera_Results_Model Calibration_Camera_Results { set; get; }=new Calibration_Camera_Results_Model () { };


        /// <summary>
        /// 检测标定图像数据集识别情况
        /// </summary>
        public ICommand Camera_Calibration_Checks_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {

                    HCalibData _CalibSetup_ID = new HCalibData();
                    int _ImageNO = 0;

             

                    //初始化标定数据
                    if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, 1) > 0)
                    {
                        //遍历标定保存图像
                        foreach (var _Calib in Calibration_Image_List)
                        {


                            HObject _CalibCoord = new HObject();
                            HXLDCont _CalibXLD = new HXLDCont();

                            //判断相机图像是否存在
                            if (_Calib.Camera_0.Calibration_Image != null || _Calib.Camera_1.Calibration_Image != null)
                            {

                                //判断相机0是否存在图像
                                if (_Calib.Camera_0.Calibration_Image != null)
                                {
                                    //查找标定图像中标定板位置和坐标
                                    Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Calib.Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma);
                                    _Calib.Camera_0.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                    _Calib.Camera_0.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                    _Calib.Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();
                                    _Calib.Image_No = _ImageNO;
                                }

                                if (_Calib.Camera_1.Calibration_Image != null)
                                {
                                    //查找标定图像中标定板位置和坐标
                                    Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Calib.Camera_1.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma);
                                    _Calib.Camera_1.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                    _Calib.Camera_1.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                    _Calib.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();
                                    _Calib.Image_No = _ImageNO;
                                }



                                _ImageNO++;

                            }

                        }

                    }
    
                });

            });
        }



        /// <summary>
        /// 检测标定图像数据集识别情况
        /// </summary>
        public ICommand Camera_Calibration_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {



                    //判断报错标定图像大于指定数量
                    if (Calibration_Image_List.Count > 10)
                    {

                        HCalibData _CalibSetup_ID = new HCalibData();

             
                        //初始化标定数据
                        if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List) > 0)
                        {

                            foreach (var _Calib in Calibration_Image_List)
                            {

                                HObject _CalibCoord = new HObject();
                                HXLDCont _CalibXLD = new HXLDCont();


                               
                                //判断相机0是否存在图像
                                if (_Calib.Camera_0.Calibration_Image != null)
                                {

                                    HObject _Imge = _Calib.Camera_0.Calibration_Image;
                                    //查找标定图像中标定板位置和坐标
                                    Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Calib.Camera_0.Calibration_Image,0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, _Calib.Image_No);
                         
                                }

                                if (_Calib.Camera_1.Calibration_Image != null)
                                {
                                    HObject _Imge = _Calib.Camera_1.Calibration_Image;

                                    //查找标定图像中标定板位置和坐标
                                    Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord,ref _CalibSetup_ID, (HImage)_Calib.Camera_1.Calibration_Image, 1, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, _Calib.Image_No);
                    
                                }





                            }


                            Calibration_Camera_Results.Error_Pixel= _CalibSetup_ID.CalibrateCameras();




                        }

                    }
                    else
                    {

                        User_Log_Add("标定图像必须大于10张", Log_Show_Window_Enum.Calibration);

                    }

                });

            });
        }



    }
}
