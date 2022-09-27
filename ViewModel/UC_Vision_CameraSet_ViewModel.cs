global using HanGao.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HalconDotNet;
using HanGao.Extension_Method;
using HanGao.View.FrameShow;
using HanGao.View.User_Control;
using HanGao.View.User_Control.Vision_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Xml_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using Nancy.Extensions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static HanGao.Model.List_Show_Models;
using static HanGao.Model.User_Read_Xml_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using MVS_SDK_Base;

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
        public MVS_Camera_Parameter_Model Camera_Parameter_Val { set; get; } = new MVS_Camera_Parameter_Model();


        /// <summary>
        /// UI相机显示参数
        /// </summary>
        public ObservableCollection<string> Camera_UI_List { set; get; } = new ObservableCollection<string>();

        /// <summary>
        ///  用户选择相机对象
        /// </summary>
        public CCamera Live_Camera { set; get; } = new CCamera();


        /// <summary>
        /// 相机对象参数
        /// </summary>
        public Camrea_Parameters_UI_Model Camera_Parameters_UI { set; get; } = new Camrea_Parameters_UI_Model();



        //private int nret = CErrorDefine.MV_ALG_OK;
        ///// <summary>
        ///// 相机状态
        ///// </summary>
        //public int Nret
        //{
        //    get { return nret; }
        //    set
        //    {
        //        nret = value;
        //        if (value != CErrorDefine.MV_ALG_OK)
        //        {

        //        }
        //    }
        //}



        /// <summary>
        /// 初始化存储相机设备
        /// </summary>
        public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();

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
                        int _PacketSize = Live_Camera.GIGE_GetOptimalPacketSize();
                        if (_PacketSize > 0)
                        {

                            //设置曝光模式
                            Set_Camera_State(
                                Camera_Parameters_Name_Enum.GevSCPSPacketSize,
                                Live_Camera.SetIntValue("GevSCPSPacketSize", (uint)_PacketSize)
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
                                                    Camera_Parameters_Name_Enum.RegisterImageCallBackEx,
                                                    Live_Camera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero)
                                                    );

                    //开始取流
                    Set_Camera_State(Camera_Parameters_Name_Enum.StartGrabbing, Live_Camera.StartGrabbing());



                }
                else if ((bool)E.IsChecked == false)
                {



                    //相机停止取流
                    Set_Camera_State(Camera_Parameters_Name_Enum.StopGrabbing, Live_Camera.StopGrabbing());


                    //回调方法设置为空
                    Set_Camera_State(
                                Camera_Parameters_Name_Enum.RegisterImageCallBackEx,
                               Live_Camera.RegisterImageCallBackEx(null, IntPtr.Zero)
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
                    Set_Camera_State(Camera_Parameters_Name_Enum.ExposureAuto, Live_Camera.SetEnumValue(Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS));


                }
                else
                {
                    //设置曝光模式
                    Set_Camera_State(Camera_Parameters_Name_Enum.ExposureAuto, Live_Camera.SetEnumValue(Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF));


                    //设置曝光时间
                    Set_Camera_State(Camera_Parameters_Name_Enum.ExposureTime, Live_Camera.SetFloatValue(Camera_Parameters_Name_Enum.ExposureTime.ToString(), (float)E.Value));

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
                    Camera_ErrorInfo(_Val_Type, Live_Camera.SetEnumValue(_Val_Type.Name, Convert.ToUInt32(_val)));




                    break;
                case Type _T when _T == typeof(int):

                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, Live_Camera.SetIntValue(_Val_Type.Name, (int)_val));


                    break;
                case Type _T when _T == typeof(double):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, Live_Camera.SetFloatValue(_Val_Type.Name, Convert.ToSingle(_val)));


                    break;

                case Type _T when _T == typeof(string):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, Live_Camera.SetStringValue(_Val_Type.Name, _val.ToString()));


                    break;
                case Type _T when _T == typeof(bool):
                    //设置相机参数
                    Camera_ErrorInfo(_Val_Type, Live_Camera.SetBoolValue(_Val_Type.Name, (bool)_val));


                    break;
            }
        }

        /// <summary>
        ///  设置相机状态码显示
        /// </summary>
        /// <param name="_name">相机参数名称枚举</param>
        /// <param name="_key">相机状态码</param>
        private bool Set_Camera_State(Camera_Parameters_Name_Enum _name, object _key)
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
                Set_Camera_State(Camera_Parameters_Name_Enum.CreateHandle, Live_Camera.CreateHandle(ref _L));


                //打开相机
                Set_Camera_State(Camera_Parameters_Name_Enum.OpenDevice, Live_Camera.OpenDevice());



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
                Set_Camera_State(Camera_Parameters_Name_Enum.CloseDevice, Live_Camera.CloseDevice());

                //销毁相机句柄 
                Set_Camera_State(Camera_Parameters_Name_Enum.DestroyHandle, Live_Camera.DestroyHandle());


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
                Set_Camera_State(Camera_Parameters_Name_Enum.StartGrabbing, Live_Camera.StartGrabbing());


                //获取图像缓存大小
                Set_Camera_State(Camera_Parameters_Name_Enum.PayloadSize, Live_Camera.GetIntValue("PayloadSize", ref stParam));

                //创建帧图像信息
                Single_Image_Mode Single_Image = new Single_Image_Mode
                {
                    pData = new byte[stParam.CurValue]
                };

                //抓取一张图片
                if (Set_Camera_State(Camera_Parameters_Name_Enum.GetOneFrameTimeout, Live_Camera.GetOneFrameTimeout(Single_Image.pData, (uint)stParam.CurValue, ref Single_Image.Single_ImageInfo, 1000)))
                {

       

                Messenger.Send<Single_Image_Mode, string>(Single_Image, nameof(Meg_Value_Eunm.Single_Image_Show));


                await Task.Delay(500);
                //相机停止取流
                Set_Camera_State(Camera_Parameters_Name_Enum.StopGrabbing, Live_Camera.StopGrabbing());

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
                    Camera_List.Add(_Camera_List[i]);

                    if (_Camera_List[i].nTLayerType == CSystem.MV_GIGE_DEVICE)
                    {

                        //转换
                        CGigECameraInfo _GEGI = Camera_List[i] as CGigECameraInfo;

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
                    Set_Camera_State(Camera_Parameters_Name_Enum.IsDeviceAccessible, CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE));



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


    /// <summary>
    /// 相机功能参数名称
    /// </summary>
    public enum Camera_Parameters_Name_Enum
    {
        /// <summary>
        /// 设备采集的采集模式、枚举类型值 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE"
        /// </summary>
        AcquisitionMode,
        /// <summary>
        /// 每个帧突发开始触发信号采集的帧数、整数类型——默认1，最大1023
        /// </summary>
        AcquisitionBurstFrameCount,
        /// <summary>
        /// 控制抓取帧的采集频率、双精度类型——默认1，最小0.4，最大500
        /// </summary>
        AcquisitionFrameRate,
        /// <summary>
        /// 控制所选触发器是否处于活动状态、枚举类型——默认Off，"MV_CAM_TRIGGER_MODE"
        /// </summary>
        TriggerMode,
        /// <summary>
        /// 指定用作触发源的内部信号或物理输入线路。所选触发器的触发模式必须设置为“开”。枚举类型——线路0，"MV_CAM_TRIGGER_SOURCE"
        /// </summary>
        TriggerSource,
        /// <summary>
        /// 指定在激活触发接收之前要应用的延迟（以us为单位），双精度类型——默认0，最小0，最大3.2e+07
        /// </summary>
        TriggerDelay,
        /// <summary>
        /// 设置所选行去缓冲时间的值（us），整数类型——默认0，最小0，最大1000000
        /// </summary>
        LineDebouncerTime,
        /// <summary>
        /// 设置定时曝光模式时的自动曝光模式，枚举类型——默认连续模式，"MV_CAM_EXPOSURE_AUTO_MODE"
        /// </summary>
        [StringValue("设置相机自动曝光模式失败")]
        ExposureAuto,
        /// <summary>
        /// 曝光模式定时时的曝光时间(us)，双精度类型——默认500，最小27，最大 2.5e+06
        /// </summary>
        [StringValue("设置相机曝光时间失败")]
        ExposureTime,
        /// <summary>
        /// 应用于图像的增益，单位为dB，双精度类型——默认0，最小0，最大20.0322
        /// </summary>
        Gain,
        /// <summary>
        /// 设置自动增益控制（AGC）模式，枚举类型——模式连续模式，"MV_CAM_GAIN_MODE"
        /// </summary>
        GainAuto,
        /// <summary>
        /// 设置选定的数字移位控制，双精度类型——默认0，最小-6，最大6
        /// </summary>
        DigitalShift,
        /// <summary>
        /// 使能/禁用数字移位调节，布尔类型——默认False
        /// </summary>
        DigitalShiftEnable,
        /// <summary>
        /// 设置选定的亮度控制，整数类似——默认100，最小0，最大255
        /// </summary>
        Brightness,
        /// <summary>
        /// 模拟黑电平百分比，整数类型——默认200，最小0，最大4095
        /// </summary>
        BlackLevel,
        /// <summary>
        /// 使能/禁用黑电平调整，布尔类型——默认True
        /// </summary>
        BlackLevelEnable,
        /// <summary>
        /// 控制像素强度的伽马校正，双精度类型——默认0.5，最小0，最大4
        /// </summary>
        Gamma,
        /// <summary>
        /// 使能/禁用伽马校正，布尔类型——默认True
        /// </summary>
        GammaEnable,
        /// <summary>
        /// 图像的锐度，整数类型——默认10，最小0，最大100
        /// </summary>
        Sharpness,
        /// <summary>
        /// 使能/禁用锐度调节，布尔类型——默认False
        /// </summary>
        SharpnessEnable,
        /// <summary>
        /// 设置设备图像宽度（像素），整数类型——默认512，最小376，最大3072
        /// </summary>
        Width,
        /// <summary>
        /// 设置设备图像高度（像素），整数类型——默认512，最小320，最大2048
        /// </summary>
        Height,
        /// <summary>
        /// 从原点到AOI的垂直偏移（像素）,整数类型——默认0，最小0，最大3072
        /// </summary>
        OffsetX,
        /// <summary>
        /// 从原点到AOI的水平偏移（像素）,整数类型——默认0，最小0，最大2048
        /// </summary>
        OffsetY,
        /// <summary>
        /// 水平翻转设备发送的图像。翻转后应用感兴趣区域，布尔类型——默认False
        /// </summary>
        ReverseX,
        /// <summary>
        /// 创建设备句柄方法。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("创建设备句柄失败")]
        CreateHandle,
        /// <summary>
        ///  打开设备方法。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("打开设备失败")]
        OpenDevice,

        /// <summary>
        /// 关闭设备方法。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("关闭设备失败")]
        CloseDevice,
        /// <summary>
        /// 销毁相机句柄。成功，返回MV_OK；失败，返回错误码
        /// </summary>
        [StringValue("销毁相机句柄失败")]
        DestroyHandle,
        /// <summary>
        /// 相机开始取流。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("相机开始取流失败")]
        StartGrabbing,
        /// <summary>
        /// 相机停止取流。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("相机停止取流失败")]
        StopGrabbing,
        /// <summary>
        /// 获取图像缓存大小。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("获取图像缓存大小失败")]
        PayloadSize,
        /// <summary>
        /// 采用超时机制获取一帧图片，SDK内部等待直到有数据时返回 。成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("获取一帧图片，或超时失败")]
        GetOneFrameTimeout,
        /// <summary>
        /// 检查相机设备是否可达。可达，返回true；不可达，返回false
        /// </summary>
        [StringValue("选择相机设备被占用相机不可使用")]
        IsDeviceAccessible,
        /// <summary>
        /// 探测网络最佳包大小(只对GigE相机有效)，成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("设置相机探测网络最佳包大小参数失败")]
        GevSCPSPacketSize,
        /// <summary>
        /// 注册图像数据回调方法，成功，返回MV_OK；失败，返回错误码 
        /// </summary>
        [StringValue("设置注册图像数据回调失败")]
        RegisterImageCallBackEx,

    }


    public class MVS_Camera_Parameter_Model
    {
        /// <summary>
        /// 设备采集的采集模式、枚举类型值 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS"
        /// </summary>
        [StringValue("设置相机触发模式失败")]
        public Enum AcquisitionMode { set; get; } = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;
        /// <summary>
        /// 每个帧突发开始触发信号采集的帧数、整数类型——默认1，最大1023
        /// </summary>
        [StringValue("设置每个帧突发开始触发信号采集的帧数失败")]
        public int AcquisitionBurstFrameCount { set; get; } = 1;
        /// <summary>
        /// 控制抓取帧的采集频率、双精度类型——默认1，最小0.4，最大500
        /// </summary>
        [StringValue("设置控制抓取帧的采集频率失败")]
        public double AcquisitionFrameRate { set; get; } = 13.6;
        /// <summary>
        /// 设置定时曝光模式时的自动曝光模式，枚举类型——默认连续模式，"MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF"
        /// </summary>
        [StringValue("设置控制抓取帧的采集频率失败")]
        public Enum ExposureAuto { set; get; } = MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF;
        /// <summary>
        /// 曝光模式定时时的曝光时间
        /// </summary>
        [StringValue("设置曝光模式定时时的曝光时间失败")]
        public double ExposureTime { set; get; } = 500;
        /// <summary>
        /// 设置曝光（或快门）的工作模式,枚举类型——默认定时模式，"MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED"
        /// </summary>
        [StringValue("设置曝光（或快门）的工作模式失败")]
        public Enum ExposureMode { set; get; } = MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED;
        /// <summary>
        /// 控制所选触发器是否处于活动状态、枚举类型——默认Off，"MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF"
        /// </summary>
        [StringValue("设置控制所选触发器是否处于活动状态失败")]

        public Enum TriggerMode { set; get; } = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;
        /// <summary>
        /// 控制所选触发器是否处于活动状态，枚举类型——默认，"MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0"
        /// </summary>
        [StringValue("设置控制所选触发器是否处于活动状态")]
        public Enum TriggerActivation { set; get; } = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
        /// <summary>
        /// 指定在激活触发接收之前要应用的延迟（以us为单位）
        /// </summary>
        [StringValue("设置指定在激活触发接收之前要应用的延迟失败")]
        public double TriggerDelay { set; get; } = 0.00;
        /// <summary>
        /// 应用于图像的增益，单位为dB，Float类型，默认0.00
        /// </summary>
        [StringValue("设置图像的增益失败")]
        public double Gain { set; get; } = 0.00;
        /// <summary>
        /// 设置自动增益控制（AGC）模式，枚举类型——默认，"MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF"
        /// </summary>
        [StringValue("设置自动增益控制（AGC）模式失败")]
        public Enum GainAuto { set; get; } = MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF;
        /// <summary>
        /// 使能/禁用数字移位调节，布尔类型——默认false
        /// </summary>
        [StringValue("设置使能/禁用数字移位调节失败")]
        public bool DigitalShiftEnable { set; get; } = true;
        /// <summary>
        /// 设置选定的数字移位控制，Float类型——默认0
        /// </summary>
        [StringValue("设置数字移位控制失败")]
        public double DigitalShift { set; get; } = 0.00;
        /// <summary>
        /// 使能/禁用黑电平调整，布尔类型——默认true；
        /// </summary>
        [StringValue("设置使能/禁用黑电平调整失败")]
        public bool BlackLevelEnable { set; get; } = true;
        /// <summary>
        /// 模拟黑电平百分比，整数类型——默认200，最小0，最大4095
        /// </summary>
        [StringValue("设置模拟黑电平百分比失败")]
        public int BlackLevel { set; get; } = 100;
        /// <summary>
        /// 使能/禁用伽马校正，布尔类型——默认true；
        /// </summary>
        [StringValue("设置使能/禁用伽马校正失败")]
        public bool GammaEnable { set; get; } = true;
        /// <summary>
        /// 控制像素强度的伽马校正，双精度类型——默认0.5，最小0，最大4
        /// </summary>
        [StringValue("设置控制像素强度的伽马校正失败")]
        public double Gamma { set; get; } = 0.5;
        /// <summary>
        /// 图像的锐度，布尔类型——默认false；
        /// </summary>
        [StringValue("设置使能/禁用伽马校正失败")]
        public bool SharpnessEnable { set; get; } = true;
        /// <summary>
        /// 图像的锐度，整数类型——默认10，最小0，最大100
        /// </summary>
        [StringValue("设置图像的锐度失败")]
        public int Sharpness { set; get; } = 10;

        /// <summary>
        /// 水平翻转设备发送的图像。翻转后应用感兴趣区域，布尔类型——默认false
        /// </summary>
        [StringValue("设置水平翻转设备发送的图像失败")]
        public bool ReverseX { set; get; } = false;

    }





}


