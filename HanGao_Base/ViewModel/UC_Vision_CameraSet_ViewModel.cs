using Halcon_SDK_DLL;
using HanGao.View.User_Control.Vision_Control;
using HanGao.Xml_Date.Vision_XML.Vision_Model;
using MvCamCtrl.NET;
using MVS_SDK_Base.Model;
using System.CodeDom;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static MVS_SDK_Base.Model.MVS_Model;
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {
            Dictionary<int, string> _E = new();
            //相机设置错误信息委托显示
            MVS_Camera.MVS_ErrorInfo_delegate += (string _Error) =>
            {
                User_Log_Add(_Error);
            };
            //UI启动初始话相机连接
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Initialization_Camera), (O, _S) =>
            {
                ////使用多线程读取
                //new Thread(new ThreadStart(new Action(() =>
                //{
                Task.Run(() =>
                {
                    //Initialization_Camera_Thread();
                });
                //})))
                //{ IsBackground = true, Name = "Initialization_Camera_Thread" }.Start();
            });
            //UI关闭,强制断开相机连接
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Close_Camera), (O, _S) =>
            {
                Close_Camera();
            });
            //接收用户选择参数
            Messenger.Register<object , string>(this, nameof(Meg_Value_Eunm.Vision_Data_Xml_List), (O, _V) =>
            {
                Camera_Parameter_Val= UC_Visal_Function_VM.Find_Data_List.Vision_List.Where(_W=>(int.Parse( _W.ID)==(int)_V)).FirstOrDefault ().Camera_Parameter_Data;
                //Camera_Parameter_Val = _Data.Camera_Parameter_Data;
                Camera_Data_ID_UI = (int)_V;
                User_Log_Add("相机参数" + Camera_Data_ID_UI + "号已加载到参数列表中！");
            });
            Initialization_Camera_Thread();
        }
        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static MVS_Camera_Parameter_Model _Camera_Parameter_Val { get; set; } = new MVS_Camera_Parameter_Model();
        /// <summary>
        /// 相机参数
        /// </summary>
        public static MVS_Camera_Parameter_Model Camera_Parameter_Val
        {
            get { return _Camera_Parameter_Val; }
            set
            {
                _Camera_Parameter_Val = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Parameter_Val)));
            }
        }
        /// <summary>
        /// 相机信息
        /// </summary>
        public MVS_Camera_Info_Model Camera_Info { set; get; } = new MVS_Camera_Info_Model();
        //private static int _Camera_Data_ID_UI { get; set; } = -1;
        /// <summary>
        /// 当前相机参数号数
        /// </summary>
        public int Camera_Data_ID_UI { set; get; }
        //{
        //    get { return _Camera_Data_ID_UI; }
        //    set
        //    {
        //        _Camera_Data_ID_UI = value;
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Camera_Data_ID_UI)));
        //    }
        //}
        /// <summary>
        /// UI相机显示参数
        /// </summary>
        public ObservableCollection<string> Camera_UI_List { set; get; } = new ObservableCollection<string>();
        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        public static MVS MVS_Camera { set; get; } = new MVS();
        /// <summary>
        /// 图像回调字段
        /// </summary>
        private cbOutputExdelegate ImageCallback;
        //public static Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 初始化连接
        /// </summary>
        public void Initialization_Camera_Thread()
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (Initialization_Camera())
                    {
                        if (Display_Status(Connect_Camera()).GetResult())
                        {
                            return;
                        }
                    }
                    Thread.Sleep(1000);
                    User_Log_Add("第" + i + "/30次重试连接相机！多次失败检查相机IP");
                }
            });
        }
        /// <summary>
        /// 用户选择相机数
        /// </summary>
        public int Camera_UI_Select { set; get; } = 0;
        /// <summary>
        /// 相机连接成功
        /// </summary>
        public bool Camera_Connect_OK { set; get; } = false;
        /// <summary>
        /// 定义回调类型
        /// </summary>
        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            HImage_Display_Model MVS_TOHalcon = new HImage_Display_Model();
            HImage _Image = new HImage();
            ///转换海康图像类型
            if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _Image, pFrameInfo.nWidth, pFrameInfo.nHeight, pData)).GetResult())
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
                if ((bool)E.IsChecked)
                {
                    //设置GEGI网络包大小
                    _State = MVS_Camera.Set_Camera_GEGI_GevSCPSPacketSize();
                    //创建抓图回调函数
                    _State = MVS_Camera.RegisterImageCallBackEx(ImageCallback = new cbOutputExdelegate(ImageCallbackFunc));
                    //开始取流
                    _State = MVS_Camera.StartGrabbing();
                    if (_State != true)
                    {
                        E.IsChecked = false;
                    }
                }
                else if ((bool)E.IsChecked == false)
                {
                    _State = MVS_Camera.StopGrabbing();
                }
            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_ExposureTime_Val_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Slider E = Sm.Source as Slider;
                if ((E.Value) == 0)
                {
                    //设置曝光自动模式
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureAuto, MVS_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.ExposureAuto), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS));
                }
                else
                {
                    //设置曝光手动模式
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureAuto, MVS_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.ExposureAuto), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF));
                    //设置曝光时间
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.ExposureTime, MVS_Camera.Camera.SetFloatValue(nameof(Camera_Parameters_Name_Enum.ExposureTime), (float)E.Value));
                    Camera_Parameter_Val.ExposureTime = E.Value;
                }
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Gain_Val_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Slider E = Sm.Source as Slider;
                if ((E.Value) == 0)
                {
                    //设置曝光自动模式
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.GainAuto, MVS_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.GainAuto), (uint)MV_CAM_GAIN_MODE.MV_GAIN_MODE_CONTINUOUS));
                }
                else
                {
                    //设置曝光手动模式
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.GainAuto, MVS_Camera.Camera.SetEnumValue(nameof(Camera_Parameters_Name_Enum.GainAuto), (uint)MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF));
                    //设置曝光时间
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.Gain, MVS_Camera.Camera.SetFloatValue(nameof(Camera_Parameters_Name_Enum.Gain), (float)E.Value));
                    Camera_Parameter_Val.Gain = E.Value;
                }
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 浮点数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Float_Val_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Slider E = Sm.Source as Slider;
                MVS_Camera.Set_Camera_Val((Camera_Parameters_Name_Enum)Enum.Parse(typeof(Camera_Parameters_Name_Enum), E.Name), MVS_Camera.Camera.SetFloatValue(E.Name, (float)E.Value));
                Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, E.Value);
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 整数数类型参数设置
        /// </summary>
        public ICommand Set_Camera_Int_Val_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Slider E = Sm.Source as Slider;
                MVS_Camera.Set_Camera_Val((Camera_Parameters_Name_Enum)Enum.Parse(typeof(Camera_Parameters_Name_Enum), E.Name), MVS_Camera.Camera.SetIntValue(E.Name, (Int32)E.Value));
                Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, (Int32)E.Value);
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 枚举类型参数设置
        /// </summary>
        public ICommand Set_Camera_Enum_Val_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                ComboBox E = Sm.Source as ComboBox;
                MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.AcquisitionMode, MVS_Camera.Camera.SetEnumValue(E.Name, (uint)(MV_CAM_ACQUISITION_MODE)Camera_Parameter_Val.AcquisitionMode));
                Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, Camera_Parameter_Val.AcquisitionMode);
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 连接相机
        /// </summary>
        /// <returns></returns>
        public MPR_Status_Model Connect_Camera()
        {
            //打开相机
            if (Display_Status(MVS_Camera.Open_Camera()).GetResult())
            {
                MVS_Camera_Info_Model _Info = new MVS_Camera_Info_Model();
                MVS_Camera.Get_Camrea_Info_Method(ref _Info);
                Camera_Info = _Info;
                Messenger.Send<MVS_Camera_Info_Model, string>(Camera_Info, nameof(Meg_Value_Eunm.MVS_Camera_Info_Show));
                //Message
                //设置相机总参数
                if (Display_Status(MVS_Camera.Set_Camrea_Parameters_List(Camera_Parameter_Val)).GetResult())
                {
                    Camera_Connect_OK = true;
                    return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "相机设置参数成功！" };
                }
                else
                {
                    //User_Log_Add("相机设置参数错误，请检查参数！");
                    return new MPR_Status_Model(MVE_Result_Enum.相机连接失败);
                }
            }
            return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = MVS_Camera.Camera + "相机连接成功！" };
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public bool Close_Camera()
        {
            //关闭相机
            MVS_Camera.CloseDevice();
            //断开连接后可以再次连接相机
            return Camera_Connect_OK = false;
        }
        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_CameraSet>(async (E) =>
            {
                //获得UI设置的参数
                Camera_Parameter_Val.ExposureTime = E.Camera_ExposureTime_UI.Value;
                Camera_Parameter_Val.Gain = E.Camera_Gain_UI.Value;
                Camera_Parameter_Val.DigitalShift = E.DigitalShift.Value;
                Camera_Parameter_Val.Gamma = E.Gamma.Value;
                Camera_Parameter_Val.Sharpness = (int)E.Sharpness.Value;
                Camera_Parameter_Val.BlackLevel = (int)E.BlackLevel.Value;
                Camera_Parameter_Val.AcquisitionMode = (MV_CAM_ACQUISITION_MODE)E.AcquisitionMode.SelectedIndex;
                Connect_Camera();
                //连接成功后关闭UI操作
                await Task.Delay(100);
            });
        }
        /// <summary>
        /// 断开相机命令
        /// </summary>
        public ICommand Disconnection_Camera_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_CameraSet>(async (E) =>
            {
                await Task.Delay(100);
                Close_Camera();
            });
        }
        /// <summary>
        /// 单帧获取图像功能
        /// </summary>
        public ICommand Single_Camera_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                Button E = Sm.Source as Button;
                HImage _Image = new HImage();
                Get_Image(ref _Image, Get_Image_Model_Enum.相机采集, UC_Visal_Function_VM.Features_Window.HWindow);
                await Task.Delay(50);
            });
        }
        /// <summary>
        /// 相机测试闪光灯功能
        /// </summary>
        public ICommand Camera_Flash_Open_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {
                CheckBox E = Sm.Source as CheckBox;
                if ((bool)E.IsChecked)
                {
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.LineInverter, MVS_Camera.Camera.SetBoolValue(nameof(Camera_Parameters_Name_Enum.LineInverter), true));
                }
                else
                {
                    MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.LineInverter, MVS_Camera.Camera.SetBoolValue(nameof(Camera_Parameters_Name_Enum.LineInverter), false));
                }
                await Task.Delay(50);
            });
        }
        /// <summary>
        /// 根据采集方式获取图像
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Get_Model"></param>
        /// <param name="_Window"></param>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static HPR_Status_Model Get_Image(ref HImage _Image, Get_Image_Model_Enum _Get_Model, HWindow _Window, string _path = "")
        {
            //HObject _image = new HObject();
            //HOperatorSet.GenEmptyObj(out _Image);
            _Image.Dispose();
            _Window.ClearWindow();
            switch (_Get_Model)
            {
                case Get_Image_Model_Enum.相机采集:
                    if (!Display_Status(GetOneFrameTimeout(ref _Image, _Window)).GetResult())
                    {
                        return new HPR_Status_Model(HVE_Result_Enum.图像文件读取失败);
                    }
                    break;
                case Get_Image_Model_Enum.图像采集:
                    if (!Display_Status(Halcon_SDK.HRead_Image(ref _Image, _path)).GetResult())
                    {
                        return new HPR_Status_Model(HVE_Result_Enum.图像文件读取失败);
                    }
                    break;
            }
            //获得图像保存到内存，随时调用
            //_image = _Image;
            UC_Visal_Function_VM.Load_Image = _Image.CopyObj(1, -1);
            _Window.DispObj(_Image);
            //保存图像当当前目录下
            if (Global_Seting.IsVisual_image_saving)
            {
                if (!Display_Status(Halcon_SDK.Save_Image(_Image)).GetResult())
                {
                    return new HPR_Status_Model(HVE_Result_Enum.样品图像保存失败);
                }
            }
            //使用完清楚内存
            //_Image.Dispose();
            return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "采集图像方法成功！" };
        }
        /// <summary>
        /// 获得一图像显示到指定窗口
        /// </summary>
        /// <param name="_HWindow"></param>
        public static HPR_Status_Model GetOneFrameTimeout(ref HImage _HImage, HWindow _Window)
        {
            //设置相机总参数
            if (Display_Status(MVS_Camera.Set_Camrea_Parameters_List(Camera_Parameter_Val)).GetResult())
            {
                //获得一帧图片信息
                MVS_Image_Mode _MVS_Image = MVS_Camera.GetOneFrameTimeout();
                //转换Halcon图像变量
                if (Display_Status(Halcon_SDK.Mvs_To_Halcon_Image(ref _HImage, _MVS_Image.FrameEx_Info.ImageInfo.Width, _MVS_Image.FrameEx_Info.ImageInfo.Height, _MVS_Image.PData)).GetResult())
                {
                    //发送显示图像位置
                    _Window.DispObj(_HImage);
                    return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = MVS_Camera.Camera.ToString() + "相机图像采集成功！" };
                }
                else
                {
                    return new HPR_Status_Model(HVE_Result_Enum.Halcon转换海康图像错误);
                }
            }
            else
            {
                return new HPR_Status_Model(HVE_Result_Enum.相机采集失败);
            }
        }
        /// <summary>
        /// 初始化相机
        /// </summary>
        public bool Initialization_Camera()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //清楚相机UI列表
                Camera_UI_List.Clear();
            });
            //查找相机设备对象
            int Camera_List_Number = MVS_Camera.Find_Camera_Devices().Count;
            Application.Current.Dispatcher.Invoke(() =>
            {
                //查询读取相机设备类型赋值到UI层
                Camera_UI_List = new ObservableCollection<string>(MVS_Camera.Get_Camera_List_Name());
            });
            ////查找到相关相机设备后，默认选择第一个相机
            //if (Camera_List_Number != 0)
            //{
            if (Camera_UI_List.Count == 0)
            {
                User_Log_Add("无法查找到相机,检查相机IP和本地IP是否同网段!");
                return false;
            }
            else
            {
                //默认选择首相机
                Camera_UI_Select = 0;
                return MVS_Camera.Check_IsDeviceAccessible(Camera_UI_Select);
            }
        }
        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Initialize_GIGE_Camera_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_CameraSet>(async (Sm) =>
            {
                //把参数类型转换控件
                Initialization_Camera();
                await Task.Delay(50);
            });
        }
    }
    /// <summary>
    /// 海康相机Int参数类型UI显示模型
    /// </summary>
    public class MVS_Int_UI_Type
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public int Val { set; get; }
        /// <summary>
        /// 最大值
        /// </summary>
        public int Max { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public int Min { set; get; }
    }
    /// <summary>
    /// 海康相机Float参数类型UI显示模型
    /// </summary>
    public class MVS_Float_UI_Type
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public double Val { set; get; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double Min { set; get; }
    }
    /// <summary>
    /// 海康相机Enum参数类型UI显示模型
    /// </summary>
    public class MVS_Enum_UI_Type
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public Enum Val { set; get; }
        /// <summary>
        /// 枚举类型
        /// </summary>
        public Enum EType { get; set; }
    }
    /// <summary>
    /// 海康相机ROI参数，UI显示模型
    /// </summary>
    public class MVS_ROI_UI_Type
    {
        /// <summary>
        /// 图像的最大宽度（以像素为单位）
        /// </summary>
        public int WidthMax { set; get; }
        /// <summary>
        /// 图像的最大高度（以像素为单位）
        /// </summary>
        public int HeightMax { set; get; }
        /// <summary>
        /// 设备提供的图像宽度（像素）
        /// </summary>
        public int Width { set; get; }
        /// <summary>
        /// 设备提供的图像的高度（像素）
        /// </summary>
        public int Height { set; get; }
        /// <summary>
        /// 从原点到AOI的垂直偏移（像素）,整数类型——默认0，最小0，最大3072
        /// </summary>
        public int OffsetX { set; get; }
        /// <summary>
        /// 从原点到AOI的水平偏移（像素）,整数类型——默认0，最小0，最大2048
        /// </summary>
        public int OffsetY { set; get; }
        /// <summary>
        /// 水平翻转设备发送的图像。翻转后应用感兴趣区域，布尔类型——默认False
        /// </summary>
        public bool ReverseX { set; get; }
    }
    /// <summary>
    /// UI界面相机参数
    /// </summary>
    public class Camrea_Parameters_UI_Model
    {
        public double Exposure { set; get; } = 30000;
        public double Gain { set; get; } = 10;
        public double DigitalShift { set; get; } = 0;
        public double Gamma { set; get; } = 0.5;
        public int Sharpness { set; get; } = 10;
        public int BlackLevel { set; get; } = 100;
        public int ROI_Height_Max { set; get; } = 2048;
        public int ROI_Width_Max { set; get; } = 3072;
        public int ROI_Height_X { set; get; } = 0;
        public int ROI_Width_Y { set; get; } = 0;
        /// <summary>
        /// 水平翻转设备发送的图像。翻转后应用感兴趣区域，布尔类型——默认False
        /// </summary>
        public bool ROI_ReverseX { set; get; }
        public ACQUISITION_MODE_Enum ACQUISITION_MODE { set; get; } = ACQUISITION_MODE_Enum.持续采集模式;
    }
}
