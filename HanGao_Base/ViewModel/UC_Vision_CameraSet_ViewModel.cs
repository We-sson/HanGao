


using Halcon_SDK_DLL;
using HanGao.View.User_Control.Vision_Control;
using MVS_SDK_Base.Model;
using System.CodeDom;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;
using static HanGao.ViewModel.User_Control_Log_ViewModel;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {

            Dictionary<int, string> _E = new();


            //添加枚举到UI下拉显示
            foreach (var E in Enum.GetValues(typeof(ACQUISITION_MODE)))
            {
                _E.Add((int)(ACQUISITION_MODE)Enum.Parse(typeof(ACQUISITION_MODE), E.ToString()), E.ToString());
            }
            AcquisitionMode_ComboBox_UI = _E;



            //相机设置错误信息委托显示
            MVS_Camera.MVS_ErrorInfo_delegate += (string _Error) =>
            {
                User_Log_Add(_Error);
         
            };


            ////创建无参的线程
            //Thread thread1 = new Thread(new ThreadStart(Initialization_Camera_Thread));

     





        }



        /// <summary>
        /// 设备采集的采集模式UI绑定 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS"
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> AcquisitionMode_ComboBox_UI { private set; get; }





        /// <summary>
        /// 相机参数
        /// </summary>
        public static MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();




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


        public static Halcon_SDK SHalcon { set; get; } = new Halcon_SDK();

        /// <summary>
        /// 相机对象参数
        /// </summary>
        public Camrea_Parameters_UI_Model Camera_Parameters_UI { set; get; } = new Camrea_Parameters_UI_Model();


        public void Initialization_Camera_Thread()
        {

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //清除输入框内的数值

                for (int i = 0; i < 30; i++)
                {
                    if (Initialization_Camera())
                    {
                        if (Connect_Camera())
                        {
                            return;
                        }
                    }
                    Thread.Sleep(2000);
                    User_Log_Add("第" + i + "次重试连接相机！多次失败检查相机IP");
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
        /// 查找相机枚举集合
        /// </summary>
        private List<CCameraInfo> _Camera_List = new();


        /// <summary>
        /// 定义回调类型
        /// </summary>
        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {

            WeakReferenceMessenger.Default.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = SHalcon.Mvs_To_Halcon_Image(pFrameInfo.nWidth, pFrameInfo.nHeight, pData), Image_Show_Halcon = UC_Visal_Function_VM.Live_Window.HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));

        }






        /// <summary>
        /// 读取图像
        /// </summary>
        public ICommand Read_Image_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {







            });
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


                MVS_Camera.Set_Camera_Val(Camera_Parameters_Name_Enum.AcquisitionMode, MVS_Camera.Camera.SetEnumValue(E.Name, (uint)(MV_CAM_ACQUISITION_MODE)E.SelectedIndex));


                Camera_Parameter_Val.GetType().GetProperty(E.Name).SetValue(Camera_Parameter_Val, (MV_CAM_ACQUISITION_MODE)E.SelectedIndex);


                await Task.Delay(100);



            });
        }


        public bool Connect_Camera()
        {
            //打开相机
            if (MVS_Camera.Open_Camera())
            {
                //设置相机总参数
                if (MVS_Camera.Set_Camrea_Parameters_List(Camera_Parameter_Val))
                {
                   return Camera_Connect_OK = true;
                  
                }
            }

            return false;

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


                //关闭相机
                MVS_Camera.CloseDevice();

                //断开连接后可以再次连接相机
              
                Camera_Connect_OK = false;

            });
        }




        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        public ICommand Window_Unloaded_Camera_Close_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {

                await Task.Delay(100);


            });
        }





        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        public ICommand Camera_Image_Gain_Set_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {

                await Task.Delay(500);

                TextBox E = Sm.Source as TextBox;

                //MessageBox.Show(E.Text);






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


                UC_Visal_Function_VM.Load_Image = GetOneFrameTimeout(UC_Visal_Function_VM.Live_Window.HWindow);



                await Task.Delay(50);
            });
        }



        /// <summary>
        /// 获得一图像显示到指定窗口
        /// </summary>
        /// <param name="_HWindow"></param>
        public static HObject GetOneFrameTimeout(HWindow _HWindow)
        {

            //设置相机总参数
            if (MVS_Camera.Set_Camrea_Parameters_List(Camera_Parameter_Val) != true) { return default; }


            //获得一帧图片信息
            MVS_Image_Mode _Image = MVS_Camera.GetOneFrameTimeout();

            //转换Halcon图像变量
            HObject Image = SHalcon.Mvs_To_Halcon_Image(_Image.FrameEx_Info.ImageInfo.Width, _Image.FrameEx_Info.ImageInfo.Height, _Image.PData);
            //发送显示图像位置
            //WeakReferenceMessenger.Default.Send<HImage_Display_Model, string>(new HImage_Display_Model() { Image = Image, Image_Show_Halcon = _HWindow }, nameof(Meg_Value_Eunm.HWindow_Image_Show));

            _HWindow.DispObj(Image);


            return Image;


        }



        /// <summary>
        /// 初始化相机
        /// </summary>
        public bool Initialization_Camera()
        {

            lock (Camera_UI_List)
            {



                //清楚相机UI列表
                Camera_UI_List.Clear();



                //查找相机设备对象
                int Camera_List_Number = MVS_Camera.Find_Camera_Devices().Count;


                //查询读取相机设备类型赋值到UI层
                Camera_UI_List = new ObservableCollection<string>(MVS_Camera.Get_Camera_List_Name());


                //查找到相关相机设备后，默认选择第一个相机
                if (Camera_List_Number != 0)
                {




                    //默认选择首相机
                    Camera_UI_Select = 0;

                    return MVS_Camera.Check_IsDeviceAccessible(Camera_UI_Select);




                



                }



                return false;



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




        /// <summary>
        /// 采集模式中文枚举名
        /// </summary>
        public enum ACQUISITION_MODE
        {
            单帧模式,
            多帧模式,
            持续采集模式
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

        public MVS_Float_UI_Type Exposure_UI { set; get; } = new MVS_Float_UI_Type() { Val = 30000, Max = 50000, Min = 0 };

        public MVS_Float_UI_Type Gain_UI { set; get; } = new MVS_Float_UI_Type() { Val = 10, Max = 20.000, Min = 0 };

        public MVS_Float_UI_Type DigitalShift_UI { set; get; } = new MVS_Float_UI_Type() { Val = 0, Max = 6, Min = -6 };

        public MVS_Float_UI_Type Gamma_UI { set; get; } = new MVS_Float_UI_Type() { Val = 0.5, Max = 4, Min = 0 };

        public MVS_Int_UI_Type Sharpness_UI { set; get; } = new MVS_Int_UI_Type() { Val = 10, Max = 100, Min = 0 };

        public MVS_Int_UI_Type BlackLevel_UI { set; get; } = new MVS_Int_UI_Type() { Val = 100, Max = 4095, Min = 0 };

        public MVS_ROI_UI_Type ROI_UI { set; get; } = new MVS_ROI_UI_Type() { HeightMax = 2048, WidthMax = 3072, Height = 2048, Width = 3072, OffsetX = 0, OffsetY = 0, ReverseX = false };
    }






}


