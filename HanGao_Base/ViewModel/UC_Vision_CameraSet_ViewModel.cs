using HanGao.View.User_Control.Vision_Control;
using MVS_SDK_Base.Model;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_Camera_Calibration_VM
;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {





            //Dictionary<int, string> _E = new();
            //相机设置错误信息委托显示
            MPR_Status_Model.MVS_ErrorInfo_delegate += (string _Error) =>
            {
                User_Log_Add(_Error, Log_Show_Window_Enum.Home);
            };


            //UI关闭,强制断开相机连接
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Close_Camera), (O, _S) =>
            {
                MVS.Close_Camera(Select_Camera);
            });
            //接收用户选择参数
            Messenger.Register<Vision_Xml_Models, string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            {


                Camera_Parameter_Val = _V.Camera_Parameter_Data;
                User_Log_Add("相机参数" + _V.ID + "号已加载到参数列表中！", Log_Show_Window_Enum.Home);



            });


    


            Initialization_Camera_Thread();
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;



        public static MVS_Camera_Parameter_Model _Camera_Parameter_Val { get; set; } = new MVS_Camera_Parameter_Model();
        /// <summary>
        /// 相机参数
        /// </summary>
        /// 
        public static MVS_Camera_Parameter_Model Camera_Parameter_Val
        {
            get { return _Camera_Parameter_Val; }
            set
            {
                _Camera_Parameter_Val = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Parameter_Val)));
            }
        }




        /// <summary>
        /// 相机信息
        /// </summary>
        public static MVS_Camera_Info_Model _Select_Camera { set; get; } = new MVS_Camera_Info_Model();

        public static MVS_Camera_Info_Model Select_Camera
        {
            get { return _Select_Camera; }
            set
            {
                _Select_Camera = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Select_Camera)));
            }
        }
        /// <summary>
        /// 相机信息
        /// </summary>
        public static MVS_Camera_Info_Model _Select_Calibration { set; get; } = new MVS_Camera_Info_Model();

        public static MVS_Camera_Info_Model Select_Calibration
        {
            get { return _Select_Calibration; }
            set
            {
                _Select_Calibration = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Select_Calibration)));
            }
        }



        /// <summary>
        /// 查找相机列表
        /// </summary>
        private static ObservableCollection<MVS_Camera_Info_Model> _MVS_Camera_Info_List { set; get; } = new ObservableCollection<MVS_Camera_Info_Model>();
        public static ObservableCollection<MVS_Camera_Info_Model> MVS_Camera_Info_List
        {
            get { return _MVS_Camera_Info_List; }
            set
            {
                _MVS_Camera_Info_List = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MVS_Camera_Info_List)));
            }
        }



        /// <summary>
        /// 图像回调字段
        /// </summary>
        private cbOutputExdelegate ImageCallback;
        //public static Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 初始化连接
        /// </summary>
        public static void Initialization_Camera_Thread()
        {
            Task.Run(() =>
            {


                while (true)
                {

                    try
                    {



                    Initialization_Camera();

                    Thread.Sleep(1000);
                    }
                    catch (Exception _e)
                    {
                        User_Log_Add("查找相机失败！" + _e.Message, Log_Show_Window_Enum.Home);

                        continue;
                    }
                }

            });
        }




        /// <summary>
        /// 用户选择相机数
        /// </summary>
        //public int Camera_UI_Select { set; get; } = 0;
        /// <summary>
        /// 相机连接成功
        /// </summary>
        public bool Camera_Connect_OK { set; get; } = false;
        /// <summary>
        /// 定义回调类型
        /// </summary>
        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            //HImage_Display_Model MVS_TOHalcon = new HImage_Display_Model();
            HImage _Image = new HImage();
            ///转换海康图像类型
            if (Halcon_SDK.Mvs_To_Halcon_Image(ref _Image, pFrameInfo.nWidth, pFrameInfo.nHeight, pData))
            {
                //传送控件显示
                Messenger.Send<HImage_Display_Model, string>(new HImage_Display_Model()
                {
                    Image = _Image,
                    Image_Show_Halcon = UC_Visal_Function_VM.Live_Window.HWindow
                }, nameof(Meg_Value_Eunm.HWindow_Image_Show));
            }
            _Image.Dispose();
        }
        /// <summary>
        /// 相机实时采集图像功能
        /// </summary>
        public ICommand Live_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;
                bool _State = false;

                try
                {


                    if ((bool)E.IsChecked)
                    {


                        //设置GEGI网络包大小
                        _State = MVS.Set_Camera_GEGI_GevSCPSPacketSize(Select_Camera);
                        //创建抓图回调函数
                        _State = MVS.RegisterImageCallBackEx(Select_Camera, ImageCallback = new cbOutputExdelegate(ImageCallbackFunc));
                        //开始取流
                        _State = MVS.StartGrabbing(Select_Camera);


                        if (_State != true)
                        {
                            E.IsChecked = false;
                        }


                        User_Log_Add("开启实时相机图像成功！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


                    }
                    else if ((bool)E.IsChecked == false)
                    {
                        _State = MVS.StopGrabbing(Select_Camera);

                        User_Log_Add("关闭实时相机图像！", Log_Show_Window_Enum.Home, MessageBoxImage.Question);


                    }

                }
                catch (Exception _e)
                {

                    E.IsChecked = false;
                    User_Log_Add("开启实时相机失败！原因：" + _e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                }


            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_ExposureTime_Val_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Slider E = Sm.Source as Slider;



                if ((E.Value) == 0)
                {
                    //设置曝光自动模式
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureAuto, Select_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.ExposureAuto), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS));
                }
                else
                {
                    //设置曝光手动模式
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureAuto, Select_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.ExposureAuto), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF));
                    //设置曝光时间
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureTime, Select_Camera.Camera.SetFloatValue(nameof(Camera_Parameters_Name_Enum.ExposureTime), (float)E.Value));
                    //Select_Camera.Camera_Parameter.ExposureTime = E.Value;
                }


            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Gain_Val_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Slider E = Sm.Source as Slider;


                if ((E.Value) == 0)
                {
                    //设置曝光自动模式
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.GainAuto, Select_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.GainAuto), (uint)MV_CAM_GAIN_MODE.MV_GAIN_MODE_CONTINUOUS));
                }
                else
                {
                    //设置曝光手动模式
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.GainAuto, Select_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.GainAuto), (uint)MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF));
                    //设置曝光时间
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.Gain, Select_Camera.Camera.SetFloatValue(nameof(Camera_Parameters_Name_Enum.Gain), (float)E.Value));
                    //Camera_Parameter_Val.Gain = E.Value;
                }


            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Float_Val_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Slider E = Sm.Source as Slider;


                MVS.Set_Camera_Val((Camera_Parameters_Name_Enum)Enum.Parse(typeof(Camera_Parameters_Name_Enum), E.Name), Select_Camera.Camera.SetFloatValue(E.Name, (float)E.Value));
                //Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, E.Value);


            });
        }
        /// <summary>
        /// 整数数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Int_Val_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Slider E = Sm.Source as Slider;



                MVS.Set_Camera_Val((Camera_Parameters_Name_Enum)Enum.Parse(typeof(Camera_Parameters_Name_Enum), E.Name), Select_Camera.Camera.SetIntValue(E.Name, (Int32)E.Value));
                //Select_Camera.Camera_Parameter.GetType().GetProperty(E.Name).SetValue(Select_Camera.Camera_Parameter, (Int32)E.Value);


            });
        }
        /// <summary>
        /// 枚举类型参数设置
        /// </summary>
        public ICommand Set_Camera_Enum_Val_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;


                MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.AcquisitionMode, Select_Camera.Camera.SetEnumValue(E.Name, (uint)E.SelectedIndex));
                //Select_Camera.Camera_Parameter.GetType().GetProperty(E.Name).SetValue(Select_Camera.Camera_Parameter, E.SelectedIndex);


            });
        }








        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((E) =>
            {



                if (MVS.Connect_Camera(Select_Camera).GetResult())
                {


                    //E.Camera_ExposureTime_UI.Value = Select_Camera.Camera_Parameter.ExposureTime;
                    //E.Camera_Gain_UI.Value = Select_Camera.Camera_Parameter.Gain;
                    //E.DigitalShift.Value = Select_Camera.Camera_Parameter.DigitalShift;
                    //E.Gamma.Value = Select_Camera.Camera_Parameter.Gamma;
                    //E.BlackLevel.Value = Select_Camera.Camera_Parameter.BlackLevel;
                    //E.AcquisitionMode.SelectedIndex = (int)Select_Camera.Camera_Parameter.AcquisitionMode;
                    //获得UI设置的参数

                }

                //连接成功后关闭UI操作


            });
        }
        /// <summary>
        /// 断开相机命令
        /// </summary>
        public ICommand Disconnection_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((E) =>
            {


                MVS.Close_Camera(Select_Camera);

            });
        }
        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Single_Camera_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button E = Sm.Source as Button;

                Task.Run(() =>
                {

                    try
                    {


                    HImage _Image = new HImage();

                    Get_Image(ref _Image, Get_Image_Model_Enum.相机采集, Select_Camera.Show_Window);

                    User_Log_Add(Select_Camera.Camera_Info.SerialNumber.ToString() + "相机采集图像成功到窗口："+ Select_Camera.Show_Window, Log_Show_Window_Enum.Home, MessageBoxImage.Question);

                    }
                    catch (Exception _e)
                    {

                    User_Log_Add(Select_Camera.Camera.ToString() + "相机采集图像失败！原因："+_e.Message, Log_Show_Window_Enum.Home, MessageBoxImage.Error);

                    }

                });




            });
        }
        /// <summary>
        /// 相机测试闪光灯功能
        /// </summary>
        public ICommand Camera_Flash_Open_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;


                if ((bool)E.IsChecked)
                {
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.LineInverter, Select_Camera.Camera.SetBoolValue(nameof(Camera_Parameters_Name_Enum.LineInverter), true));
                }
                else
                {
                    MVS.Set_Camera_Val(Camera_Parameters_Name_Enum.LineInverter, Select_Camera.Camera.SetBoolValue(nameof(Camera_Parameters_Name_Enum.LineInverter), false));
                }


            });
        }


        /// <summary>
        /// 根据选项显示指定窗口
        /// </summary>
        /// <param name="_Window"></param>
        /// <returns></returns>
        public static Halcon_SDK GetWindowHandle(Window_Show_Name_Enum _Window)
        {

            switch (_Window)
            {
                case Window_Show_Name_Enum.Live_Window:
                    return UC_Visal_Function_VM.Live_Window;

                case Window_Show_Name_Enum.Features_Window:
                    return UC_Visal_Function_VM.Features_Window;


                case Window_Show_Name_Enum.Results_Window_1:
                    return UC_Visal_Function_VM.Results_Window_1;


                case Window_Show_Name_Enum.Results_Window_2:
                    return UC_Visal_Function_VM.Results_Window_2;


                case Window_Show_Name_Enum.Results_Window_3:
                    return UC_Visal_Function_VM.Results_Window_3;
                case Window_Show_Name_Enum.Results_Window_4:
                    return UC_Visal_Function_VM.Results_Window_4;
                    //case Window_Show_Name_Enum.Calibration_Window_1:



                    //    return Vision_Calibration_Home_VM.Calibration_Window_1;

                    //case Window_Show_Name_Enum.Calibration_Window_2:
                    //    return Vision_Calibration_Home_VM.Calibration_Window_2;


                    //case Window_Show_Name_Enum.Calibration_3D_Results:
                    //    return Vision_Calibration_Home_VM.Calibration_3D_Results;



            }

            return null;
            //_W.SetPart(0, 0, -2, -2);


        }


        /// <summary>
        /// 根据采集方式获取图像
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Get_Model"></param>
        /// <param name="_Window"></param>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Get_Image(ref HImage _Image, Get_Image_Model_Enum _Get_Model, Window_Show_Name_Enum _HW, string _path = "")
        {
            //HObject _image = new HObject();
            //HOperatorSet.GenEmptyObj(out _Image);
            _Image.Dispose();

            Halcon_SDK _Window = GetWindowHandle(_HW);
            _Window.HWindow.ClearWindow();

            switch (_Get_Model)
            {
                case Get_Image_Model_Enum.相机采集:
                    if (!(GetOneFrameTimeout(ref _Image, _Window.HWindow, Camera_Parameter_Val)))
                    {
                        return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);
                    }
                    break;
                case Get_Image_Model_Enum.图像采集:
                    if (!Display_Status(Halcon_SDK.HRead_Image(ref _Image, _path)).GetResult())
                    {
                        return new HPR_Status_Model<bool>(HVE_Result_Enum.图像文件读取失败);
                    }
                    break;
            }
            //获得图像保存到内存，随时调用
            //_image = _Image;
            UC_Visal_Function_VM.Load_Image = _Image.CopyObj(1, -1);
            _Window.DisplayImage = _Image;
            //保存图像当当前目录下
            if (Global_Seting.IsVisual_image_saving)
            {
                if (!Display_Status(Halcon_SDK.Save_Image(_Image)).GetResult())
                {
                    return new HPR_Status_Model<bool>(HVE_Result_Enum.样品图像保存失败);
                }
            }
            //使用完清楚内存
            //_Image.Dispose();
            return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "采集图像方法成功！" };
        }
        /// <summary>
        /// 获得一图像显示到指定窗口
        /// </summary>
        /// <param name="_HWindow"></param>
        public static bool GetOneFrameTimeout(ref HImage _HImage, HWindow _Window, MVS_Camera_Parameter_Model _Camera_Parameter)
        {
            //设置相机总参数
            if (MVS.Set_Camrea_Parameters_List(Select_Camera.Camera, Camera_Parameter_Val))
            {
                MVS.StartGrabbing(Select_Camera);

                //获得一帧图片信息
                MVS_Image_Mode _MVS_Image = MVS.GetOneFrameTimeout(Select_Camera);

                MVS.StopGrabbing(Select_Camera);
                //转换Halcon图像变量
                if ((Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData)))
                {
                    //发送显示图像位置
                    _Window.DispObj(_HImage);


                    //User_Log_Add(Select_Camera.Camera.ToString() + "相机图像采集成功！", Log_Show_Window_Enum.Home);


                    return true;
                }
            }
            else
            {



                throw new Exception("从" + Select_Camera.Camera.ToString() + "相机获得图像失败！");
            }

            return false;
        }
        /// <summary>
        /// 查找相机状态
        /// </summary>
        public static void Initialization_Camera()
        {



            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    //清楚相机UI列表
            //    Camera_UI_List.Clear();
            //});
            //查找相机设备对象
            //int Camera_List_Number = MVS.Find_Camera_Devices().Count;

            //查询在线相机可用情况,添相机列表

            //MVS_Camera_Info_List MVS.Find_Camera_Devices());

            ///创建临时集合
            ObservableCollection<CGigECameraInfo> _ECameraInfo_List = new ObservableCollection<CGigECameraInfo>(MVS.Find_Camera_Devices());


            ObservableCollection<MVS_Camera_Info_Model> _Camer_Info = new ObservableCollection<MVS_Camera_Info_Model>(MVS_Camera_Info_List);




            //查找网络中相机对象
            foreach (var _List in _ECameraInfo_List)
            {


                MVS_Camera_Info_Model _Info = MVS_Camera_Info_List.Where(_W => _W.Camera_Info.SerialNumber == _List.chSerialNumber).FirstOrDefault();

                if (_Info == null)
                {

                    
                    Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Add(new MVS_Camera_Info_Model(_List)); });

                }
            }


            //查找没在线的相机对象删除
            for (int i = 0; i < MVS_Camera_Info_List.Count; i++)
            {


                CGigECameraInfo _info = _ECameraInfo_List.Where(_W => _W.chSerialNumber == MVS_Camera_Info_List[i].Camera_Info.SerialNumber).FirstOrDefault();

                if (_info == null)
                {

                    Application.Current.Dispatcher.Invoke(() => { MVS_Camera_Info_List.Remove(MVS_Camera_Info_List[i]); });
                    


                    i--;
                }


            }




            //查询列表中相机设备可用情况
            foreach (var _Camera in MVS_Camera_Info_List)
            {
                if (_Camera.Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                {

                    if (MVS.Check_IsDeviceAccessible(_Camera.MVS_CameraInfo))
                    {
                        _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Null;
                    }
                    else
                    {
                        _Camera.Camer_Status = MV_CAM_Device_Status_Enum.Possess;
                    }
                }

            }

        }
        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Set_Camera_Parameter_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((Sm) =>
            {


                //把参数类型转换控件
                //Initialization_Camera();


            });
        }



        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Set_Camera_Calibration_Parameter_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                ComboBox _ComboxBox = Sm.Source as ComboBox;

                //把参数类型转换控件
                //Initialization_Camera();

                MVS_Camera_Info_Model _Select_Camera = _ComboxBox.DataContext as MVS_Camera_Info_Model;

                switch ((Camera_Calibration_MainOrSubroutine_Type_Enum)_ComboxBox.SelectedIndex)
                {
                    case Camera_Calibration_MainOrSubroutine_Type_Enum.Main:
                        Camera_Calibration_Paramteters_0 = new Halcon_Camera_Calibration_Parameters_Model(_Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters);


                        break;
                    case Camera_Calibration_MainOrSubroutine_Type_Enum.Subroutine:


                        Camera_Calibration_Paramteters_1 = new Halcon_Camera_Calibration_Parameters_Model(_Select_Camera.Camera_Calibration.Camera_Calibration_Paramteters);


                        break;
                }



            });
        }

    }




}
