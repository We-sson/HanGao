
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
        /// Halcon标定参数设置句柄
        /// </summary>
        public static HCalibData Halcon_CalibSetup_ID { set; get; } = null;



        /// <summary>
        /// 相机标定结果 
        /// </summary>
        public Calibration_Camera_Results_Model Calibration_Camera_Results { set; get; } = new Calibration_Camera_Results_Model() { };


     
        /// <summary>
        /// 标定双相机内参数属性
        /// </summary>
        public Caliration_AllCamera_Results_Model All_Camera_Results { set; get; } = new Caliration_AllCamera_Results_Model();
        
        
  
      


        /// <summary>
        /// 相机图像标定结果
        /// </summary>
        public bool Calibration_Results_State { set; get; } = false;
        /// <summary>
        /// 相机图像标定过程值
        /// </summary>
        public int Calibration_Results_State_Val { set; get; } = 10;





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
                        if (Calibration_List.Count > 10)
                        {

                            HCalibData _CalibSetup_ID = new HCalibData();

                            Calibration_Load_Type _Load_Type = Calibration_Load_Type.None;
                            //UI线程委托
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _Load_Type = Enum.Parse<Calibration_Load_Type>(E.Name);
                            });

                            //判断标定图像列表双相机都有相同图像
                            if (Calibration_List.Where((_w)=>_w.Camera_0.Calibration_Image!=null).ToList().Count== Calibration_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count)
                            {
                                //双相机一起标定,提高精度

                            //初始化标定数据
                            if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, Calibration_Load_Type.All_Camera) > 0)
                            {
                                for (int i = 0; i < Calibration_List.Count; i++)
                                {


                                    HObject _CalibCoord = new HObject();
                                    HXLDCont _CalibXLD = new HXLDCont();

                                    //判断相机0是否存在图像
                                    if (Calibration_List[i].Camera_0.Calibration_Image != null)
                                    {

                                        HObject _Imge = Calibration_List[i].Camera_0.Calibration_Image;
                                        //查找标定图像中标定板位置和坐标
                                        Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_List[i].Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No);
                                        Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();


                                    }

                                    if (Calibration_List[i].Camera_1.Calibration_Image != null)
                                    {
                                        HObject _Imge = Calibration_List[i].Camera_1.Calibration_Image;
                                        //查找标定图像中标定板位置和坐标
                                        Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_List[i].Camera_1.Calibration_Image, 1, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No);
                                        Calibration_List[i].Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();

                                    }


                                    //_Calib.Image_No
                                    //Calibration_Image_List.Count
                                    Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                                }

                                //标定相机后赋值到全局调用
                                Halcon_CalibSetup_ID = _CalibSetup_ID;


                                //计算标定误差
                                double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();


                      

                                //Calibration_Camera_Results.Error_Pixel = _CalibSetup_ID.CalibrateCameras();


                                Application.Current.Dispatcher.Invoke(() => 
                                {
                                
                                
                                

                                switch (Enum.Parse<Calibration_Load_Type>(E.Name))
                                {
                                    case Calibration_Load_Type.Camera_0:


                                        Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                                        {
                                            Result_Error_Val = Results_Error_Val,
                                            Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                        };




                                        break;
                                    case Calibration_Load_Type.Camera_1:
                                        Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                                        {
                                            Result_Error_Val = Results_Error_Val,
                                            Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 1)
                                        };
                                        break;


                                }
                                });
                            }

                            }
                            else
                            {




                                //初始化标定数据
                                if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, Calibration_Load_Type.Camera_0) > 0)
                                {
                                    for (int i = 0; i < Calibration_List.Count; i++)
                                    {


                                        HObject _CalibCoord = new HObject();
                                        HXLDCont _CalibXLD = new HXLDCont();

                                        //判断相机0是否存在图像
                                        if (Calibration_List[i].Camera_0.Calibration_Image != null)
                                        {

                                            HObject _Imge = Calibration_List[i].Camera_0.Calibration_Image;
                                            //查找标定图像中标定板位置和坐标
                                            Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_List[i].Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No);
                                            Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();


                                        }

                                        //_Calib.Image_No
                                        //Calibration_Image_List.Count
                                        Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                                    }




                                    //标定相机后赋值到全局调用
                                    Halcon_CalibSetup_ID = _CalibSetup_ID;



                                    //计算标定误差
                                    double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();




                                    //Calibration_Camera_Results.Error_Pixel = _CalibSetup_ID.CalibrateCameras();


                                    Application.Current.Dispatcher.Invoke(() =>
                                    {




                                        switch (Enum.Parse<Calibration_Load_Type>(E.Name))
                                        {

                                            case Calibration_Load_Type.All_Camera:

                                                Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };
                                                Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 1)
                                                };
                                                break;


                                            case Calibration_Load_Type.Camera_0:


                                                Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };




                                                break;
                                            case Calibration_Load_Type.Camera_1:
                                                Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };
                                                break;


                                        }
                                    });
                                }


                                //初始化标定数据
                                if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, Calibration_Load_Type.Camera_1) > 0)
                                {
                                    for (int i = 0; i < Calibration_List.Count; i++)
                                    {


                                        HObject _CalibCoord = new HObject();
                                        HXLDCont _CalibXLD = new HXLDCont();

                                        //判断相机0是否存在图像
                                        if (Calibration_List[i].Camera_0.Calibration_Image != null)
                                        {

                                            HObject _Imge = Calibration_List[i].Camera_0.Calibration_Image;
                                            //查找标定图像中标定板位置和坐标
                                            Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)Calibration_List[i].Camera_0.Calibration_Image, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No);
                                            Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();


                                        }

                                        //_Calib.Image_No
                                        //Calibration_Image_List.Count
                                        Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                                    }






                                    //标定相机后赋值到全局调用
                                    Halcon_CalibSetup_ID = _CalibSetup_ID;



                                    //计算标定误差
                                    double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();




                                    //Calibration_Camera_Results.Error_Pixel = _CalibSetup_ID.CalibrateCameras();


                                    Application.Current.Dispatcher.Invoke(() =>
                                    {




                                        switch (Enum.Parse<Calibration_Load_Type>(E.Name))
                                        {

                                            case Calibration_Load_Type.All_Camera:

                                                Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };
                                                Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 1)
                                                };
                                                break;


                                            case Calibration_Load_Type.Camera_0:


                                                Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };




                                                break;
                                            case Calibration_Load_Type.Camera_1:
                                                Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                                                {
                                                    Result_Error_Val = Results_Error_Val,
                                                    Camera_Result_Pama = Halcon_Calibration_SDK.Set_Cailbration_Camera_Param(Halcon_CalibSetup_ID, 0)
                                                };
                                                break;


                                        }
                                    });
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
