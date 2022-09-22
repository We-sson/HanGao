global using HanGao.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HalconDotNet;
using HanGao.View.FrameShow;
using HanGao.View.User_Control;
using HanGao.View.User_Control.Vision_Control;
using HanGao.View.UserMessage;
using HanGao.Xml_Date.Xml_Models;
using HanGao.Xml_Date.Xml_Write_Read;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using static HanGao.Model.List_Show_Models;
using static HanGao.Model.User_Read_Xml_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Vision_CameraSet_ViewModel : ObservableRecipient
    {
        public UC_Vision_CameraSet_ViewModel()
        {







        }



        /// <summary>
        /// UI相机显示列表
        /// </summary>
        public ObservableCollection<string> Camera_UI_List { set; get; } = new ObservableCollection<string>();

        /// <summary>
        /// 
        /// </summary>
        public CCamera Live_Camera { set; get; } = new CCamera();

        private int nret = CErrorDefine.MV_ALG_OK;
        /// <summary>
        /// 相机状态
        /// </summary>
        public int Nret
        {
            get { return nret; }
            set
            {
                nret = value;
                if (value != CErrorDefine.MV_ALG_OK)
                {

                }
            }
        }



        /// <summary>
        /// 初始化存储相机设备
        /// </summary>
        public List<CCameraInfo> Camera_List { set; get; } = new List<CCameraInfo>();

        /// <summary>
        /// 用户选择相机参数项
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
        /// 查找网络内相机
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
                            Nret = Live_Camera.SetIntValue("GevSCPSPacketSize", (uint)_PacketSize);
                            //创建失败方法
                            if (CErrorDefine.MV_OK != Nret)
                            {
                                MessageBox.Show("设置相机参数错误");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("获取数据包大小失败，相机数据包为：" + _PacketSize);
                            //获取数据包大小失败方法
                        }

                    }



                    //创建抓图回调函数
                    ImageCallback = new cbOutputExdelegate(ImageCallbackFunc);
                    Nret = Live_Camera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置回调方法失败");
                        return;
                    }

                    Nret = Live_Camera.StartGrabbing();
                    //开始捕捉
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置回调方法失败");
                        return;
                    }


                }
                else if ((bool)E.IsChecked == false)
                {



                    Nret = Live_Camera.StopGrabbing();
                    //停止抓图方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("关闭相机失败");
                        return;
                    }



                    Nret = Live_Camera.RegisterImageCallBackEx(null, IntPtr.Zero);
                    //创建失败方法
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("取消回调方法失败");
                        return;
                    }


                }

            });
        }



        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        public ICommand Camera_Image_Exposure_time_Set_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>(async (Sm) =>
            {

                await Task.Delay(1000);

                TextBox E = Sm.Source as TextBox;

                //MessageBox.Show(E.Text);

                if (float.Parse(E.Text) == 0)
                {
                    //设置曝光时间
                    Nret = Live_Camera.SetEnumValue(Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS);
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置相机自动曝光模式失败");
                        return;
                    }
                }
                else
                {
                    //设置曝光时间
                    Nret = Live_Camera.SetEnumValue(Camera_Parameters_Name_Enum.ExposureAuto.ToString(), (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF);
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置相机曝光模式失败");
                        return;
                    }
                    //设置曝光时间
                    Nret = Live_Camera.SetFloatValue(Camera_Parameters_Name_Enum.ExposureTime.ToString(), float.Parse(E.Text));
                    if (CErrorDefine.MV_OK != Nret)
                    {
                        MessageBox.Show("设置相机曝光时间失败");
                        return;
                    }
                }





            });
        }



        /// <summary>
        /// 连接相机命令
        /// </summary>
        public ICommand Connection_Camera_Comm
        {
            get => new AsyncRelayCommand<UC_Vision_CameraSet>(async (E) =>
            {


                await Task.Delay(100);


                //MessageBox.Show(E.Text);

                CCameraInfo _L = _Camera_List[Camera_UI_Select];
                //创建相机
                Nret = Live_Camera.CreateHandle(ref _L);
                //创建失败方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("创建相机失败");
                    return;
                }

                //打开相机
                Nret = Live_Camera.OpenDevice();
                //打卡失败方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("打开相机失败");
                    return;
                }

                //设置采集模式
                Nret = Live_Camera.SetEnumValue(Camera_Parameters_Name_Enum.AcquisitionMode.ToString(), (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                //创建失败方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("设置相机触发模式失败");
                    return;
                }


                //设置相机触发模式
                Nret = Live_Camera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                //创建失败方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("设置相机触发模式失败");
                    return;
                }

                //连接成功后关闭UI操作
                E.Connection_Camera.IsEnabled = false;



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



                //MessageBox.Show(E.Text);

                Nret = Live_Camera.CloseDevice();
                //停止抓图方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("关闭相机失败");
                    return;
                }

                Nret = Live_Camera.DestroyHandle();
                //停止抓图方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("关闭相机句柄失败");
                    return;
                }



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

                UserControl E = Sm.Source as UserControl;

                //MessageBox.Show(E.Text);

                Nret = Live_Camera.CloseDevice();
                //停止抓图方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("关闭相机失败");
                    return;
                }

                Nret = Live_Camera.DestroyHandle();
                //停止抓图方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("关闭相机句柄失败");
                    return;
                }




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





                Nret = Live_Camera.StartGrabbing();
                //开始捕捉
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("设置开启相机失败");
                    return;
                }

                Live_Camera.GetIntValue("PayloadSize", ref stParam);

                //帧图像信息
                Single_Image_Mode Single_Image = new Single_Image_Mode
                {
                    pData = new byte[stParam.CurValue]
                };


                Nret = Live_Camera.GetOneFrameTimeout(Single_Image.pData, (uint)stParam.CurValue, ref Single_Image.Single_ImageInfo, 1000);
                //抓图错误方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("相机抓图失败");
                    return;
                }


                Messenger.Send<Single_Image_Mode, string>(Single_Image, nameof(Meg_Value_Eunm.Single_Image_Show));


                await Task.Delay(500);

                Nret = Live_Camera.StopGrabbing();
                //停止抓图方法
                if (CErrorDefine.MV_OK != Nret)
                {
                    MessageBox.Show("关闭相机失败");
                    return;
                }




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

                        Camera_UI_List.Add(_GEGI.chManufacturerName + _GEGI.chModelName);


                    }



                }
                //查找到相关相机设备后，默认选择第一个相机
                if (_Camera_List.Count != 0)
                {

                    Camera_UI_Select = 0;

                    CCameraInfo _L = _Camera_List[Camera_UI_Select];

                    //检查相机设备可用情况
                    bool nBol = CSystem.IsDeviceAccessible(ref _L, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE);
                    if (nBol == false)
                    {
                        MessageBox.Show("选择设备被占用相机不可使用");
                        return;
                    }










                }



            });
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
            ExposureAuto,
            /// <summary>
            /// 曝光模式定时时的曝光时间(us)，双精度类型——默认500，最小27，最大 2.5e+06
            /// </summary>
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

}


