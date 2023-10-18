
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
        public static HCalibData Halcon_CalibSetup_ID { set; get; } = null;



        /// <summary>
        /// 相机标定结果 
        /// </summary>
        public Calibration_Camera_Results_Model Calibration_Camera_Results { set; get; } = new Calibration_Camera_Results_Model() { };


        public double Calibration_Camera_0_Results_Error_Val { set; get; } = 0;



        public double Calibration_Camera_1_Results_Error_Val { set; get; } = 0;


        /// <summary>
        /// 相机图像标定结果
        /// </summary>
        public bool Calibration_Results_State { set; get; } = false;
        /// <summary>
        /// 相机图像标定过程值
        /// </summary>
        public int Calibration_Results_State_Val { set; get; } = 10;
        /// <summary>
        /// 相机图像标定结果
        /// </summary>
        public bool Calibration_Checks_State { set; get; } = false;
        /// <summary>
        /// 相机图像标定过程值
        /// </summary>
        public int Calibration_Checks_State_Val { set; get; } = 50;


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
                    Calibration_Image_No = 0;
                    Calibration_Checks_State = true;


                    try
                    {




                        //初始化标定数据
                        if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, Calibration_Load_Type.Camera_0) > 0)
                        {



                            //清理标定数据状态
                            foreach (var _type in Calibration_Image_List)
                            {
                                _type.Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.None.ToString();
                                _type.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.None.ToString();
                            }

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
                                        if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Calib.Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma).GetResult())
                                        {

                                            _Calib.Camera_0.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                            _Calib.Camera_0.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                            _Calib.Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();
                                            _Calib.Image_No = Calibration_Image_No;
                                        }
                                        else
                                        {
                                            _Calib.Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别失败.ToString();
                                            _Calib.Image_No = Calibration_Image_No;
                                        }
                                    }

                                    if (_Calib.Camera_1.Calibration_Image != null)
                                    {
                                        if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Calib.Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma).GetResult())
                                        {
                                            //查找标定图像中标定板位置和坐标
                                            _Calib.Camera_1.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                            _Calib.Camera_1.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                            _Calib.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别成功.ToString();
                                            _Calib.Image_No = Calibration_Image_No;
                                        }
                                        else
                                        {
                                            _Calib.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像识别失败.ToString();
                                            _Calib.Image_No = Calibration_Image_No;
                                        }
                                    }


                                    //整理图像列表序号
                                    Calibration_Image_No++;
                                    //计算识别进度
                                    Calibration_Checks_State_Val = (100 / Calibration_Image_List.Count) * Calibration_Image_No;

                                }

                            }

                        }


                    }
                    catch (Exception)
                    {

                        User_Log_Add("有标定图像检测失败,请移除失败图像 !", Log_Show_Window_Enum.Calibration);

                    }
                    finally
                    {
                        Calibration_Checks_State_Val = 100;
                        Calibration_Checks_State = false;
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

                    try
                    {



                        Calibration_Results_State = true;
                        Calibration_Results_State_Val = 0;

                        //判断报错标定图像大于指定数量
                        if (Calibration_Image_List.Count > 10)
                        {

                            HCalibData _CalibSetup_ID = new HCalibData();

                            Calibration_Load_Type _Load_Type = Calibration_Load_Type.None;
                            //UI线程委托
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _Load_Type = Enum.Parse<Calibration_Load_Type>(E.Name);
                            });


                            //初始化标定数据
                            if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, _Load_Type) > 0)
                                {
                                    for (int i = 0; i < Calibration_Image_List.Count; i++)
                                    {


                                        HObject _CalibCoord = new HObject();
                                        HXLDCont _CalibXLD = new HXLDCont();

                                        //判断相机0是否存在图像
                                        if (Calibration_Image_List[i].Camera_0.Calibration_Image != null)
                                        {

                                            HObject _Imge = Calibration_Image_List[i].Camera_0.Calibration_Image;
                                            //查找标定图像中标定板位置和坐标
                                            Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_Image_List[i].Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_Image_List[i].Image_No);
                                        }

                                        if (Calibration_Image_List[i].Camera_1.Calibration_Image != null)
                                        {
                                            HObject _Imge = Calibration_Image_List[i].Camera_1.Calibration_Image;
                                            //查找标定图像中标定板位置和坐标
                                            Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_Image_List[i].Camera_1.Calibration_Image, 1, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_Image_List[i].Image_No);

                                        }


                                        //_Calib.Image_No
                                        //Calibration_Image_List.Count
                                        Calibration_Results_State_Val = (100 / Calibration_Image_List.Count) * i;

                                    }

                                    //标定相机后赋值到全局调用
                                    Halcon_CalibSetup_ID = _CalibSetup_ID;


                                //计算标定误差
                                double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();

                                //Calibration_Camera_Results.Error_Pixel = _CalibSetup_ID.CalibrateCameras();

                                switch (Enum.Parse<Calibration_Load_Type>(E.Name))
                                    {
                                        case Calibration_Load_Type.Camera_0:
                                            Calibration_Camera_0_Results_Error_Val = Results_Error_Val;



                                        //读取标定内参进行保存
                                        HTuple _HCamera = Halcon_CalibSetup_ID.GetCalibData("model", "general", "camera_setup_model");
                                        HCameraSetupModel _HCam = new HCameraSetupModel(_HCamera.H);
                                      //
                                         HTuple  _Camera_Param = _HCam.GetCameraSetupParam(0, "params");



                                        break;
                                        case Calibration_Load_Type.Camera_1:
                                            Calibration_Camera_1_Results_Error_Val= Results_Error_Val;
                                            break;
                                 
                                    }
                                }


                    



                        }
                        else
                        {

                            User_Log_Add("标定图像必须大于10张", Log_Show_Window_Enum.Calibration);

                        }

                    }
                    catch (Exception e)
                    {

                        User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);

                    }

                    finally
                    {
                        //复位标定数据状态
                        Calibration_Results_State_Val = 100;
                        Calibration_Results_State = false;
                    }


                });

            });
        }



    }
}
