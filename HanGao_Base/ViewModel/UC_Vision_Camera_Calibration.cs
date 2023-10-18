using MVS_SDK_Base.Model;
using System.Drawing;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_Camera_Calibration : ObservableRecipient
    {
        public UC_Vision_Camera_Calibration() { }











        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        public static Halcon_Camera_Calibration_Model _Halcon_Calibration_Setupl { get; set; } = new Halcon_Camera_Calibration_Model();
        /// <summary>
        /// 全局标定设置参数
        /// </summary>
        public static Halcon_Camera_Calibration_Model Halcon_Calibration_Setup
        {
            get { return _Halcon_Calibration_Setupl; }
            set
            {
                _Halcon_Calibration_Setupl = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Halcon_Calibration_Setup)));
            }
        }



        public bool Get_Calibration_State { set; get; } = true;

        public static  int Calibration_Image_0_No = 0;
        public static  int Calibration_Image_1_No = 0;


        /// <summary>
        /// Halcon标定参数设置句柄
        /// </summary>
        //public HCalibData Halcon_CalibSetup_ID { set; get; } = new HCalibData() { };



        /// <summary>
        /// 标定图像保存列表动作
        /// </summary>
        public ICommand Calibration_Image_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;



                Task.Run(() =>
                {



                    Calibration_Image_Save_List();

                });



            });
        }


        /// <summary>
        /// 相机标定采集开始
        /// </summary>
        public ICommand Camera_Calibration_Start_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;

                if ((bool)E.IsChecked)
                {

                    Halcon_Calibration_Start(Halcon_Calibration_Setup);


                    //if (_State != true)
                    //{
                    //    E.IsChecked = false;
                    //}
                }
                else if ((bool)E.IsChecked == false)
                {
                    Halcon_Calibration_End(Halcon_Calibration_Setup);
                }

            });
        }

        /// <summary>
        /// 保存相机图像到集合中
        /// </summary>
        public void Calibration_Image_Save_List()
        {


            foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
            {


                if (_camer.Camera_Calibration.Camera_Calibration_Setup == Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                {
                    //相机连接后继续
                    if (_camer.Camer_Status == MV_CAM_Device_Status_Enum.Connecting)
                    {

                        //获得一帧图片信息
                        MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_camer);
                        HImage _HImage = new HImage();


                        //Halcon_SDK _Window = UC_Vision_CameraSet_ViewModel.GetWindowHandle(_camer.Show_Window);


                        if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)).GetResult())
                        {
                            //Calibration_Image_List_Model _Image = new Calibration_Image_List_Model() { };

                            //_Image.Image_No = UC_Vision_Calibration_Image_VM.Calibration_Image_No;
                            //_Image.Camera_No = (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type;

                            Calibration_Load_Image(_HImage, _camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, _camer.Camera_Info.SerialNumber.ToString());

                        }

                    }
                }


            }


        }

        /// <summary>
        /// 添加标定图像方法
        /// </summary>
        /// <param name="_HImage"></param>
        /// <param name="_Camera_Calibration_Type"></param>
        /// <param name="_Carme_Name"></param>
        public static    void Calibration_Load_Image(HImage _HImage, Camera_Calibration_MainOrSubroutine_Type_Enum _Camera_Calibration_Type,string _Carme_Name)
        {
            Calibration_Image_List_Model _Image = new Calibration_Image_List_Model() { };
            //设置图像号数

            //根据写入图像号数设置对应图像
            switch ((int)_Camera_Calibration_Type)
            {
                case (int)Camera_Calibration_MainOrSubroutine_Type_Enum.Main:
                    _Image.Image_No = Calibration_Image_0_No;
                    _Image.Camera_No = 0;
                    _Image.Camera_0.Calibration_Image = _HImage.CopyObj(1, -2);
                    _Image.Camera_0.Carme_Name = _Carme_Name;
                    _Image.Camera_0.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像已加载.ToString();
                    Calibration_Image_0_No++;

                    break;
                case (int)Camera_Calibration_MainOrSubroutine_Type_Enum.Subroutine:
                    _Image.Image_No = Calibration_Image_1_No;
                    _Image.Camera_No = 1;
                    _Image.Camera_1.Calibration_Image = _HImage.CopyObj(1, -2);
                    _Image.Camera_1.Carme_Name = _Carme_Name;
                    _Image.Camera_1.Calibration_State = Camera_Calibration_Results_Type_Enum.标定图像已加载.ToString();
                    Calibration_Image_1_No++;

                    break;

            }

            //发送设置好的图像到标定图像集合中
            StrongReferenceMessenger.Default.Send<Calibration_Image_List_Model, string>(_Image, nameof(Meg_Value_Eunm.Calibration_Image_ADD));

            //增加图像号数
            //UC_Vision_Calibration_Image_VM.Calibration_Image_No++;

        }



        public void Halcon_Calibration_End(Halcon_Camera_Calibration_Model _Parameters)
        {
            Get_Calibration_State = false;
            foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
            {
                if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                {
                    //相机连接后继续
                    if (_camer.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                    {

                        //设置相机总参数
                        //if (MVS.Set_Camrea_Parameters_List(_camer.Camera, UC_Vision_CameraSet_ViewModel.Camera_Parameter_Val).GetResult())
                        //{

                        MVS.StopGrabbing(_camer);


                        //}

                    }
                }



            };



        }








        /// <summary>
        /// 相机标定开始
        /// </summary>
        /// <param name="_Parameters"></param>
        public void Halcon_Calibration_Start(Halcon_Camera_Calibration_Model _Parameters)
        {
            HCalibData _CalibSetup_ID = new HCalibData();

            ///读取标定相机数量




            Get_Calibration_State = true;


            if (Set_Camera_Calibration_Par(ref _CalibSetup_ID,  Calibration_Load_Type.All_Camera) > 0)
            {

                foreach (var _camer in UC_Vision_CameraSet_ViewModel.MVS_Camera_Info_List)
                {
                    if (_camer.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                    {
                        //相机连接后继续
                        if (_camer.Camer_Status == MVS_SDK_Base.Model.MV_CAM_Device_Status_Enum.Connecting)
                        {

                            //设置相机总参数
                            if (MVS.Set_Camrea_Parameters_List(_camer.Camera, UC_Vision_CameraSet_ViewModel.Camera_Parameter_Val).GetResult())
                            {

                                if (MVS.StartGrabbing(_camer))
                                {




                                    Task.Run(() =>
                                    {



                                        //bool Get_Calibration_Image = false;

                                        while (Get_Calibration_State)
                                        {





                                            HImage _HImage = new HImage();
                                            //获得一帧图片信息
                                            MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(_camer);


                                            if (_MVS_Image != null)
                                            {

                                                if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)).GetResult())
                                                {

                                                    //发送显示图像位置

                                                    Display_HObiet(_HImage, null, null, null, _camer.Show_Window);

                                                    if (Vision_Calibration_Home_VM.Halcon_ShowMaxGray)
                                                    {
                                                        HRegion _Region = new HRegion();
                                                        if (Halcon_Method.Get_Image_MaxThreshold(ref _Region, _HImage).GetResult())
                                                        {

                                                            Display_HObiet(null, _Region, new HObject(), KnownColor.Red.ToString(), _camer.Show_Window);
                                                        }



                                                    }


                                                    if (Vision_Calibration_Home_VM.Halcon_ShowMinGray)
                                                    {
                                                        HRegion _Region = new HRegion();
                                                        if (Halcon_Method.Get_Image_MinThreshold(ref _Region, _HImage).GetResult())
                                                        {


                                                            Display_HObiet(null, _Region, new HObject(), KnownColor.Blue.ToString(), _camer.Show_Window);

                                                        }

                                                    }

                                                    try
                                                    {
                                                        if (Vision_Calibration_Home_VM.Halcon_ShowHObject)
                                                        {


                                                            HObject _CalibCoord = new HObject();
                                                            HXLDCont _CalibXLD = new HXLDCont();


                                                            if (Halcon_Method.FindCalib_3DCoord(ref _CalibXLD, ref _CalibCoord, ref _CalibSetup_ID, _HImage, (int)_camer.Camera_Calibration.Camera_Calibration_MainOrSubroutine_Type, 0, Halcon_Calibration_Setup.Halcon_Calibretion_Sigma).GetResult())
                                                            {



                                                                //HRegion _Coord = new HRegion(_CalibCoord);

                                                                Display_HObiet(null, _CalibXLD, null, KnownColor.Green.ToString(), _camer.Show_Window);
                                                                Display_HObiet(null, null, _CalibCoord, null, _camer.Show_Window);
                                                            }


                                                        }


                                                    }
                                                    catch (Exception e)
                                                    {

                                                        User_Log_Add(e.Message, Log_Show_Window_Enum.Calibration);
                                                        Display_HObiet(_HImage, new HObject(), new HObject(), null, _camer.Show_Window);

                                                    }


                                                }


                                            }

                                        }

                                    });

                                }

                            }

                        }
                    }



                };

            }
        }

        /// <summary>
        /// 创初始化标定对象
        /// </summary>
        /// <param name="_camerLits"></param>
        /// <returns></returns>
        //public static int Set_Camera_Calibration_Par(ref HCalibData _CalibSetup_ID, int _camera_number)
        //{




        //    _CalibSetup_ID.Dispose();

        //    //初始化标定相机数量
        //    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);

        //    //设置校准对象描述文件
        //    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);

        //    int _number = 0;
        //    //设置使用的摄像机类型

        //    HCamPar _CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(Select_Calibration.Camera_Calibration.Camera_Calibration_Paramteters));



        //    ////设置标定相机内参初始化,俩种方法
        //    _CalibSetup_ID.SetCalibDataCamParam(
        //        _number,
        //        new HTuple(),
        //       _CamPar);

        //    //HOperatorSet.SetCalibDataCamParam(
        //    //    Halcon_CalibSetup_ID,
        //    //    _number,
        //    //    new HTuple(),
        //    //    Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

        //    return _camera_number;
        //}

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

                     _camera_number = MVS_Camera_Info_List.Where((_w) => _w.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration).ToList().Count;

                    //初始化标定相机数量
                    _CalibSetup_ID = new HCalibData(Halcon_Calibration_Setup.Calibration_Setup_Model.ToString(), _camera_number, 1);
                    //设置校准对象描述文件
                    _CalibSetup_ID.SetCalibDataCalibObject(0, Halcon_Calibration_Setup.Halcon_CaltabDescr_Address);


                    int _number = 0;

                    //设置使用的摄像机类型
                    foreach (var _camera in MVS_Camera_Info_List)
            {
                if (_camera.Camera_Calibration.Camera_Calibration_Setup == MVS_SDK_Base.Model.Camera_Calibration_Mobile_Type_Enum.Start_Calibration)
                {
                    HCamPar Camera_CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(_camera.Camera_Calibration.Camera_Calibration_Paramteters));

                    ////设置标定相机内参初始化,俩种方法
                    _CalibSetup_ID.SetCalibDataCamParam(_number,new HTuple(),Camera_CamPar);

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
                    HCamPar File_CamPar = new HCamPar(Halcon_Calibration_SDK.Halcon_Get_Camera_Area_Scan(Select_Calibration.Camera_Calibration.Camera_Calibration_Paramteters));

                    ////设置标定相机内参初始化,俩种方法
                    _CalibSetup_ID.SetCalibDataCamParam(0,new HTuple(), File_CamPar);
                  




                    break;

            }


            return _camera_number;
        }




        /// <summary>
        /// 显示控件对象
        /// </summary>
        /// <param name="_HImage"></param>
        /// <param name="_Region"></param>
        /// <param name="_XLD"></param>
        /// <param name="_DrawColor"></param>
        /// <param name="_Show"></param>
        public static void Display_HObiet(HObject _HImage, HObject _Region, HObject _XLD, string _DrawColor, Window_Show_Name_Enum _Show)
        {
            if (_DrawColor != null)
            {

                SetHDrawColor(_DrawColor, DisplaySetDraw_Enum.fill, _Show);
            }

            if (_HImage != null)
            {
                SetDisplayHObject(_HImage, Display_HObject_Type_Enum.Image, _Show);

            }
            if (_Region != null)
            {

                SetDisplayHObject(_Region, Display_HObject_Type_Enum.Region, _Show);
            }

            if (_XLD != null)
            {

                SetDisplayHObject(_XLD, Display_HObject_Type_Enum.XLD, _Show);
            }



        }





        public static void SetDisplayHObject(HObject _Dispaly, Display_HObject_Type_Enum _Type, Window_Show_Name_Enum _Window)
        {
            StrongReferenceMessenger.Default.Send<DisplayHObject_Model, string>(new DisplayHObject_Model()
            { Display = _Dispaly, Display_Type = _Type, Show_Window = _Window }, nameof(Meg_Value_Eunm.DisplayHObject));

        }
        public static void SetHDrawColor(string HColor, DisplaySetDraw_Enum HDraw, Window_Show_Name_Enum _Window)
        {
            StrongReferenceMessenger.Default.Send<DisplayHObject_Model, string>(new DisplayHObject_Model()
            { SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw }, Display_Type = Display_HObject_Type_Enum.SetDrawColor, Show_Window = _Window }, nameof(Meg_Value_Eunm.DisplayHObject));

        }



        public static void SetDisplay3DModel(Display3DModel_Model _3DModel)
        {


            StrongReferenceMessenger.Default.Send<Display3DModel_Model, string>(_3DModel, nameof(Meg_Value_Eunm.Display_3DModel));


        }


    }
}
