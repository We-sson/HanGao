using MVS_SDK_Base.Model;
using static Halcon_SDK_DLL.Halcon_Calibration_SDK;
using static HanGao.ViewModel.UC_Vision_Calibration_Image_VM;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;

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
        private static int _Calibration_Results_State_Val = 10;

        public static int Calibration_Results_State_Val
        {
            get { return _Calibration_Results_State_Val; }
            set { _Calibration_Results_State_Val = value; }
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



                        HCalibData _CalibSetup_ID = new HCalibData();
                        Calibration_Load_Type _Load_Type = Calibration_Load_Type.None;

                        //获得相机标定内参
                        //Caliration_AllCamera_Results_Model _All_Camera_Results = All_Camera_Results;
                        //UI线程委托
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _Load_Type = Enum.Parse<Calibration_Load_Type>(E.Name);
                        });


                        //判断报错标定图像大于指定数量
                        if (Calibration_List.Count > 10)
                        {

                            if (Calibration_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count == Calibration_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count)
                            {

                                User_Log_Add("进行双相机标定!", Log_Show_Window_Enum.Calibration);

                                All_Camera_Results = Cailbration_Camera_Method(All_Camera_Results, Calibration_Load_Type.All_Camera);

                            }
                            else
                            {
                                //检查标定图像集合数量数量是否合理
                                if (Calibration_List.Where((_w) => _w.Camera_0.Calibration_Image != null).ToList().Count > 10 || Calibration_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count > 10)
                                {

                                    All_Camera_Results = Cailbration_Camera_Method(All_Camera_Results, _Load_Type);

                                }
                                else
                                {
                                    User_Log_Add(_Load_Type + " : 标定有效图像列表必须大于10张", Log_Show_Window_Enum.Calibration);

                                }

                                ////检查标定图像集合数量数量是否合理
                                //if (Calibration_List.Where((_w) => _w.Camera_1.Calibration_Image != null).ToList().Count > 10)
                                //{
                                //    Cailbration_Camera_Method(ref _All_Camera_Results, _Load_Type);

                                //}
                                //else
                                //{
                                //    User_Log_Add(Calibration_List[0].Camera_1.Carme_Name + " : 标定有效图像列表必须大于10张", Log_Show_Window_Enum.Calibration);

                                //}

                            }




                        }
                        else
                        {

                            User_Log_Add("标定图像必须大于10张", Log_Show_Window_Enum.Calibration);

                        }


                        ////赋值回程序
                        //All_Camera_Results = _All_Camera_Results;



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






        public static Caliration_AllCamera_Results_Model Cailbration_Camera_Method(Caliration_AllCamera_Results_Model _All_Camera_Results, Calibration_Load_Type _Calib_Load_Type)
        {





            HCalibData _CalibSetup_ID = new HCalibData();
            Calibration_Results_State_Val = 0;


            switch (_Calib_Load_Type)
            {
                case Calibration_Load_Type.None:


                    break;
                case Calibration_Load_Type.All_Camera:




                    //初始化标定数据
                    if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, _Calib_Load_Type) > 0)
                    {



                        for (int i = 0; i < Calibration_List.Count; i++)
                        {


                            HXLDCont _CalibXLD_0 = new HXLDCont();
                            HXLDCont _CalibXLD_1 = new HXLDCont();
                            HObject _Imge_1 = new HObject();
                            HObject _Imge_0 = new HObject();
                            HObject _CalibCoord_0 = new HObject();
                            HObject _CalibCoord_1 = new HObject();
                         
                            _Imge_0 = Calibration_List[i].Camera_0.Calibration_Image;
                            _Imge_1 = Calibration_List[i].Camera_1.Calibration_Image;

                            //计算标定板信息
                            if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD_0, ref _CalibCoord_0, ref _CalibSetup_ID, (HImage)_Imge_0, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No).GetResult())
                            {
                            

                                Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();
                                Calibration_List[i].Camera_0.Calibration_Region = _CalibXLD_0.CopyObj(1, -1);
                                Calibration_List[i].Camera_0.Calibration_XLD = _CalibCoord_0.CopyObj(1, -1);
               

                                //获得信息
                            }



                            if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD_1, ref _CalibCoord_1, ref _CalibSetup_ID, (HImage)_Imge_1, 1, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No).GetResult())
                            {




                                Calibration_List[i].Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();
                                Calibration_List[i].Camera_1.Calibration_Region = _CalibXLD_1.CopyObj(1, -1);
                                Calibration_List[i].Camera_1.Calibration_XLD = _CalibCoord_1.CopyObj(1, -1);
                           

                            }


                            //_Calib.Image_No
                            //Calibration_Image_List.Count
                            Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                        }









                        //});
                    }
                    else
                    {

                        User_Log_Add("标定图像初始化识别！", Log_Show_Window_Enum.Calibration);

                        return _All_Camera_Results;
                    }










                    break;
                case Calibration_Load_Type.Camera_0 or Calibration_Load_Type.Camera_1:




                    //初始化标定数据
                    if (Set_Camera_Calibration_Par(ref _CalibSetup_ID, _Calib_Load_Type) > 0)
                    {



                        for (int i = 0; i < Calibration_List.Count; i++)
                        {


                            HXLDCont _CalibXLD = new HXLDCont();
                            HObject _Imge = new HObject();
                            HObject _CalibCoord = new HObject();
                            List<HObjectModel3D> _CarmeraModel = new List<HObjectModel3D>();
                            //判断相机是否存在图像
                            switch (_Calib_Load_Type)
                            {

                                case Calibration_Load_Type.Camera_0:

                                    _Imge = Calibration_List[i].Camera_0.Calibration_Image;


                                    break;
                                case Calibration_Load_Type.Camera_1:

                                    _Imge = Calibration_List[i].Camera_1.Calibration_Image;

                                    break;
                            }

                            if (_Imge != null)
                            {

                                //计算标定板信息



                                if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, (HImage)_Imge, 0, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma, Calibration_List[i].Image_No).GetResult())
                                {






                           


                                    switch (_Calib_Load_Type)
                                    {

                                        case Calibration_Load_Type.Camera_0:


                                            Calibration_List[i].Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();
                                            Calibration_List[i].Camera_0.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                            Calibration_List[i].Camera_0.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                                     
                                            break;
                                        case Calibration_Load_Type.Camera_1:


                                            Calibration_List[i].Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定计算成功.ToString();
                                            Calibration_List[i].Camera_1.Calibration_Region = _CalibXLD.CopyObj(1, -1);
                                            Calibration_List[i].Camera_1.Calibration_XLD = _CalibCoord.CopyObj(1, -1);
                          

                                            break;
                                    }

                                }


                            }
                            else
                            {

                                User_Log_Add("标定图像列表为空！请检查。", Log_Show_Window_Enum.Calibration);

                                return _All_Camera_Results;
                            }



                            //_Calib.Image_No
                            //Calibration_Image_List.Count
                            Calibration_Results_State_Val = (100 / Calibration_List.Count) * i;

                        }

                        //});
                    }


                    break;

            }





            //标定相机后赋值到全局调用
            Halcon_CalibSetup_ID = _CalibSetup_ID;




            return Calibration_Results(_All_Camera_Results, _CalibSetup_ID, _Calib_Load_Type);

        }

        /// <summary>
        /// 标定获得相机内参结果
        /// </summary>
        /// <param name="_All_Camera_Results"></param>
        /// <param name="_CalibSetup_ID"></param>
        /// <param name="_Calib_Load_Type"></param>
        /// <returns></returns>
        public static Caliration_AllCamera_Results_Model Calibration_Results(Caliration_AllCamera_Results_Model _All_Camera_Results, HCalibData _CalibSetup_ID, Calibration_Load_Type _Calib_Load_Type)
        {



            //计算标定误差
            double Results_Error_Val = _CalibSetup_ID.CalibrateCameras();

        



            //获得标定结果相机内参
            switch (_Calib_Load_Type)
            {

                case Calibration_Load_Type.All_Camera:


                    _All_Camera_Results.Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                    {
                        Result_Error_Val = Results_Error_Val,
                        Camera_Result_Pama = Set_Cailbration_Camera_Param(_CalibSetup_ID, 0)
                    };
                    _All_Camera_Results.Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                    {
                        Result_Error_Val = Results_Error_Val,
                        Camera_Result_Pama = Set_Cailbration_Camera_Param(_CalibSetup_ID, 1)
                    };

                    foreach (var _H3DModel in Calibration_List)
                    {


                        _H3DModel.Camera_0.Calibration_3D_Model = Get_Calibration_Camera_3DModel(_CalibSetup_ID, _H3DModel.Image_No,0);

                        _H3DModel.Camera_1.Calibration_3D_Model = Get_Calibration_Camera_3DModel(_CalibSetup_ID, _H3DModel.Image_No,1);


                    }
                    //获得信息


                    break;


                case Calibration_Load_Type.Camera_0:


                    _All_Camera_Results.Camera_0_Results = new Calibration_Camera_Data_Results_Model()
                    {
                        Result_Error_Val = Results_Error_Val,
                        Camera_Result_Pama = Set_Cailbration_Camera_Param(_CalibSetup_ID, 0)
                    };

                    foreach (var _H3DModel in Calibration_List)
                    {


                        _H3DModel.Camera_0.Calibration_3D_Model = Get_Calibration_Camera_3DModel(_CalibSetup_ID, _H3DModel.Image_No, 0);

   
                    }


                    break;
                case Calibration_Load_Type.Camera_1:
                    _All_Camera_Results.Camera_1_Results = new Calibration_Camera_Data_Results_Model()
                    {
                        Result_Error_Val = Results_Error_Val,
                        Camera_Result_Pama = Set_Cailbration_Camera_Param(_CalibSetup_ID, 0)
                    };


                    foreach (var _H3DModel in Calibration_List)
                    {


                        _H3DModel.Camera_1.Calibration_3D_Model = Get_Calibration_Camera_3DModel(_CalibSetup_ID, _H3DModel.Image_No, 0);


                    }
                    break;


            }


            return _All_Camera_Results;


        }





        /// <summary>
        /// 创初始化标定对象
        /// </summary>
        /// <param name="_camerLits"></param>
        /// <returns></returns>
        public static int Set_Camera_Calibration_Par(ref HCalibData _CalibSetup_ID, Calibration_Load_Type _CType)
        {




            _CalibSetup_ID.Dispose();
            int _camera_number = 0;





            switch (_CType)
            {
                case Calibration_Load_Type.All_Camera:


                    //读取标定相机数量
                    _camera_number = MVS_Camera_Info_List.Where((_w) => _w.Camera_Calibration.Camera_Calibration_Setup == Camera_Calibration_Mobile_Type_Enum.Start_Calibration).ToList().Count;

                    //初始化标定相机数量
                    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);
                    //设置校准对象描述文件
                    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);


                    int _number = 0;

                    //设置使用的摄像机类型
                    foreach (var _camera in MVS_Camera_Info_List)
                    {
                        if (_camera.Camera_Calibration.Camera_Calibration_Setup == Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                        {
                            HCamPar Camera_CamPar = new HCamPar(Halcon_Calibration_SDK.Get_Cailbration_Camera_Param(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                            ////设置标定相机内参初始化,俩种方法
                            _CalibSetup_ID.SetCalibDataCamParam(_number, new HTuple(), Camera_CamPar);

                            //HOperatorSet.SetCalibDataCamParam(
                            //    Halcon_CalibSetup_ID,
                            //    _number,
                            //    new HTuple(),
                            //    Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                            _number++;


                        }
                    }


                    break;
                case Calibration_Load_Type.Camera_0 or Calibration_Load_Type.Camera_1:

                    //文件标定方式一个位
                    _camera_number = 1;
                    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);

                    //设置校准对象描述文件
                    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);


                    //设置使用的摄像机类型

                    //获取初始化相机内参
                    HCamPar File_CamPar = new HCamPar(Halcon_Calibration_SDK.Get_Cailbration_Camera_Param(Select_Calibration.Camera_Calibration.Camera_Calibration_Paramteters));

                    ////设置标定相机内参初始化,俩种方法
                    _CalibSetup_ID.SetCalibDataCamParam(0, new HTuple(), File_CamPar);





                    break;

            }


            return _camera_number;
        }



    }
}
