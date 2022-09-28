


using HanGao.View.User_Control.Vision_Control;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {

            Dictionary<int, string> _E = new Dictionary<int, string>();


            //添加枚举到UI下拉显示
            foreach (var E in Enum.GetValues(typeof(ACQUISITION_MODE)))
            {
                _E.Add((int)(ACQUISITION_MODE)Enum.Parse(typeof(ACQUISITION_MODE), E.ToString()), E.ToString());
            }
            AcquisitionMode_ComboBox_UI = _E;


        }



        /// <summary>
        /// 设备采集的采集模式UI绑定 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS"
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> AcquisitionMode_ComboBox_UI { private set; get; }


        /// <summary>
        /// 相机参数
        /// </summary>
        public MVS.MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS.MVS_Camera_Parameter_Model();


        /// <summary>
        /// UI相机显示参数
        /// </summary>
        public ObservableCollection<string> Camera_UI_List { set; get; } = new ObservableCollection<string>();

        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        public MVS MVS_Camera { set; get; } = new MVS();






        /// <summary>
        /// 相机对象参数
        /// </summary>
        public Camrea_Parameters_UI_Model Camera_Parameters_UI { set; get; } = new Camrea_Parameters_UI_Model();






        ///// <summary>
        ///// 初始化存储相机设备
        ///// </summary>
        //public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();

        /// <summary>
        /// 用户选择相机数
        /// </summary>
        public int Camera_UI_Select { set; get; } = 0;


        /// <summary>
        /// 查找相机枚举集合
        /// </summary>
        private List<CCameraInfo> _Camera_List = new List<CCameraInfo>();


        /// <summary>
        /// 定义回调类型
        /// </summary>
        private cbOutputExdelegate ImageCallback;

        private void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {


            Messenger.Send<MVS_Image_delegate_Mode, string>(new MVS_Image_delegate_Mode() { pData = pData, pFrameInfo = pFrameInfo, pUser = pUser }, nameof(Meg_Value_Eunm.Live_Window_Image_Show));

            // MessageBox.Show("Get one frame: Width[" + Convert.ToString(pFrameInfo.nWidth) + "] , Height[" + Convert.ToString(pFrameInfo.nHeight) + "] , FrameNum[" + Convert.ToString(pFrameInfo.nFrameNum) + "]");





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

                if ((bool)E.IsChecked)
                {
                    CCameraInfo _L = _Camera_List[Camera_UI_Select];

                    //GEGI相机专属设置
                    if (_L.nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {
                        int _PacketSize = MVS_Camera.Live_Camera.GIGE_GetOptimalPacketSize();
                        if (_PacketSize > 0)
                        {

                            //设置曝光模式
                            Set_Camera_State(
                               MVS.Camera_Parameters_Name_Enum.GevSCPSPacketSize,
                              MVS_Camera.Live_Camera.SetIntValue("GevSCPSPacketSize", (uint)_PacketSize)
                                );

                        }
                        else
                        {
                            MessageBox.Show("获取数据包大小失败，相机数据包为：" + _PacketSize);
                            //获取数据包大小失败方法
                        }

                    }



                    //创建抓图回调函数
                    ImageCallback = new cbOutputExdelegate(ImageCallbackFunc);
                    Set_Camera_State(
                                                   MVS.Camera_Parameters_Name_Enum.RegisterImageCallBackEx,
                                                   MVS_Camera.Live_Camera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero)
                                                    );

                    //开始取流
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.StartGrabbing, MVS_Camera.Live_Camera.StartGrabbing());



                }
                else if ((bool)E.IsChecked == false)
                {



                    //相机停止取流
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.StopGrabbing, MVS_Camera.Live_Camera.StopGrabbing());


                    //回调方法设置为空
                    Set_Camera_State(
                               MVS.Camera_Parameters_Name_Enum.RegisterImageCallBackEx,
                               MVS_Camera.Live_Camera.RegisterImageCallBackEx(null, IntPtr.Zero)
                                );



                }

            });
        }



        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
    
        public ICommand Camera_Exposure_Set_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {


                Slider E = Sm.Source as Slider;

                //MessageBox.Show(E.Text);

                if ((E.Value) == 0)
                {
                    //设置曝光模式
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.ExposureAuto, MVS_Camera.Live_Camera.SetEnumValue(MVS.Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS));


                }
                else
                {
                    //设置曝光模式
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.ExposureAuto, MVS_Camera.Live_Camera.SetEnumValue(MVS.Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF));


                    //设置曝光时间
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.ExposureTime, MVS_Camera.Live_Camera.SetFloatValue(MVS.Camera_Parameters_Name_Enum.ExposureTime.ToString(), (float)E.Value));

                }


                await Task.Delay(100);



            });
        }


        /// <summary>
        /// 提取参数报错信息显示
        /// </summary>
        /// <param name="_Val_Type">相机参数属性</param>
        /// <param name="_key">相机错误码</param>
        private void Camera_ErrorInfo(PropertyInfo _Val_Type, int _key)
        {

            if (CErrorDefine.MV_OK != _key)
            {
                foreach (var _Attri in _Val_Type.GetCustomAttributes())
                {
                    if (_Attri is StringValueAttribute)
                    {
                        StringValueAttribute Errorinfo = (StringValueAttribute)_Attri;
                        MessageBox.Show(Errorinfo.StringValue);
                    }

                }
                return;
            }

        }

        /// <summary>
        /// 利用反射设置相机参数
        /// </summary>
        /// <param name="_Val_Type"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        private void Set_Camera_Val(PropertyInfo _Val_Type, string _name, object _val)
        {

            switch (_Val_Type.PropertyType)
            {
                case Type _T when _T == typeof(Enum):

                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, MVS_Camera.Live_Camera.SetEnumValue(_Val_Type.Name, Convert.ToUInt32(_val)));




                    break;
                case Type _T when _T == typeof(int):

                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, MVS_Camera.Live_Camera.SetIntValue(_Val_Type.Name, (int)_val));


                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, MVS_Camera.Live_Camera.SetFloatValue(_Val_Type.Name, Convert.ToSingle(_val)));


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, MVS_Camera.Live_Camera.SetStringValue(_Val_Type.Name, _val.ToString()));


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, MVS_Camera.Live_Camera.SetBoolValue(_Val_Type.Name, (bool)_val));


                    break;
            }
        }

        /// <summary>
        ///  设置相机状态码显示
        /// </summary>
        /// <param name="_name">相机参数名称枚举</param>
        /// <param name="_key">相机状态码</param>
        private bool Set_Camera_State(MVS.Camera_Parameters_Name_Enum _name, object _key)
        {


            switch (_key.GetType())
            {
                case Type _T when _key.GetType() == typeof(int):


                    //创建失败方法
                    if (CErrorDefine.MV_OK != (int)_key)
                    {
                        MessageBox.Show(_name.GetStringValue());
                        return false;
                    }

                    break;
                case Type _T when _key.GetType() == typeof(bool):
                    //创建失败方法
                    if (false == (bool)_key)
                    {
                        MessageBox.Show(_name.GetStringValue());
                        return false;
                    }

                    break;

            }

            return true;



        }







        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_CameraSet>(async (E) =>
            {




                //MessageBox.Show(E.Text);

                CCameraInfo _L = _Camera_List[Camera_UI_Select];


                //创建相机
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.CreateHandle, MVS_Camera.Live_Camera.CreateHandle(ref _L));


                //打开相机
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.OpenDevice, MVS_Camera.Live_Camera.OpenDevice());



                //遍历设置参数
                foreach (PropertyInfo _Type in Camera_Parameter_Val.GetType().GetProperties())
                {

                    Set_Camera_Val(_Type, _Type.Name, _Type.GetValue(Camera_Parameter_Val));

                }



                //连接成功后关闭UI操作
                E.Connection_Camera.IsEnabled = false;

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
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.CloseDevice, MVS_Camera.Live_Camera.CloseDevice());

                //销毁相机句柄 
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.DestroyHandle, MVS_Camera.Live_Camera.DestroyHandle());


                //断开连接后可以再次连接相机
                E.Connection_Camera.IsEnabled = true;


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

                CIntValue stParam = new CIntValue();


                //开始取流
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.StartGrabbing, MVS_Camera.Live_Camera.StartGrabbing());


                //获取图像缓存大小
                Set_Camera_State(MVS.Camera_Parameters_Name_Enum.PayloadSize, MVS_Camera.Live_Camera.GetIntValue("PayloadSize", ref stParam));

                //创建帧图像信息
                Single_Image_Mode Single_Image = new Single_Image_Mode
                {
                    pData = new byte[stParam.CurValue]
                };

                //抓取一张图片
                if (Set_Camera_State(MVS.Camera_Parameters_Name_Enum.GetOneFrameTimeout, MVS_Camera.Live_Camera.GetOneFrameTimeout(Single_Image.pData, (uint)stParam.CurValue, ref Single_Image.Single_ImageInfo, 1000)))
                {



                    Messenger.Send<Single_Image_Mode, string>(Single_Image, nameof(Meg_Value_Eunm.Single_Image_Show));


                    await Task.Delay(500);
                    //相机停止取流
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.StopGrabbing, MVS_Camera.Live_Camera.StopGrabbing());

                };

            });
        }




        /// <summary>
        /// 查找网络内相机
        /// </summary>
        public ICommand Initialize_GIGE_Camera_Comm
        {
            get => new RelayCommand<UC_Vision_CameraSet>((Sm) =>
            {
                //把参数类型转换控件

                // ch:创建设备列表 | en:Create Device List
                GC.Collect();
                int nRet = CErrorDefine.MV_OK;



                Task<int> _T = Task.Run(() =>
                 {

                     nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE, ref _Camera_List);

                     return nRet;
                 });
                //获得设备枚举

                //查找相机设备失败动作
                if (_T.Result != 0)
                {
                    return;
                }



                Camera_UI_List.Clear();
                //查询读取相机设备类型赋值到UI层
                for (int i = 0; i < _Camera_List.Count; i++)
                {
                    //添加到属性
                    MVS_Camera.Camera_List.Add(_Camera_List[i]);

                    if (_Camera_List[i].nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {

                        //转换
                        CGigECameraInfo _GEGI = MVS_Camera.Camera_List[i] as CGigECameraInfo;

                        //将相机信息名称添加到UI列表上
                        Camera_UI_List.Add(_GEGI.chManufacturerName + _GEGI.chModelName);


                    }



                }
                //查找到相关相机设备后，默认选择第一个相机
                if (_Camera_List.Count != 0)
                {
                    //默认选择首相机
                    Camera_UI_Select = 0;

                    //读取选择相机信息
                    CCameraInfo _L = _Camera_List[Camera_UI_Select];


                    //检查相机设备可用情况
                    Set_Camera_State(MVS.Camera_Parameters_Name_Enum.IsDeviceAccessible, CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE));



                }



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
    /// 海康图像回调参数模型
    /// </summary>
    public class MVS_Image_delegate_Mode
    {
        public IntPtr pData;
        public MV_FRAME_OUT_INFO_EX pFrameInfo;
        public IntPtr pUser;

    }


    /// <summary>
    /// 图像显示模型
    /// </summary>
    public class Single_Image_Mode
    {

        public byte[] pData;
        public CFrameoutEx Single_ImageInfo = new CFrameoutEx();
        public IntPtr Get_IntPtr()
        {
            if (pData != null)
            {

                return Marshal.UnsafeAddrOfPinnedArrayElement((Array)pData, 0);
            }
            return IntPtr.Zero;
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

        public MVS_Float_UI_Type Exposure_UI { set; get; } = new MVS_Float_UI_Type() { Val = 500, Max = 40000, Min = 0 };

        public MVS_Float_UI_Type Gain_UI { set; get; } = new MVS_Float_UI_Type() { Val = 0, Max = 20.000, Min = 0 };

        public MVS_Float_UI_Type DigitalShift_UI { set; get; } = new MVS_Float_UI_Type() { Val = 0, Max = 6, Min = -6 };

        public MVS_Float_UI_Type Gamma_UI { set; get; } = new MVS_Float_UI_Type() { Val = 0.5, Max = 4, Min = 0 };

        public MVS_Int_UI_Type Sharpness_UI { set; get; } = new MVS_Int_UI_Type() { Val = 10, Max = 100, Min = 0 };

        public MVS_Int_UI_Type BlackLevel_UI { set; get; } = new MVS_Int_UI_Type() { Val = 100, Max = 4095, Min = 0 };

        public MVS_ROI_UI_Type ROI_UI { set; get; } = new MVS_ROI_UI_Type() { HeightMax = 2048, WidthMax = 3072, Height = 2048, Width = 3072, OffsetX = 0, OffsetY = 0, ReverseX = false };
    }






}


