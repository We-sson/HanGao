using Generic_Extension;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MVS_SDK_Base.Model
{
    public class MVS_Model
    {
        /// <summary>
        /// 相机功能参数名称
        /// </summary>
        public enum Camera_Parameters_Name_Enum
        {
            /// <summary>
            /// 设备采集的采集模式、枚举类型值 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE"
            /// </summary>
            [StringValue("设备采集的采集模式失败")]
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
            [StringValue("设置相机图像的增益失败")]
            Gain,
            /// <summary>
            /// 设置自动增益控制（AGC）模式，枚举类型——模式连续模式，"MV_CAM_GAIN_MODE"
            /// </summary>
            [StringValue("设置自动增益控制（AGC）模式失败")]

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
            [StringValue("设置从原点到AOI的水平偏移（像素）失败")]
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
            /// 获取最佳的packet size，该接口目前只支持GigE设备 ,成功，返回MV_OK；失败，返回错误码 
            /// </summary>
            [StringValue("获取相机探测网络最佳包大小参数失败")]
            GIGE_GetOptimalPacketSize,
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

        /// <summary>
        /// 相机参数类型
        /// </summary>
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
            public double ExposureTime { set; get; } = 30000;
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
            /// 设置自动增益控制（AGC）模式，枚举类型——默认，"MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF"
            /// </summary>
            [StringValue("设置自动增益控制（AGC）模式失败")]
            public Enum GainAuto { set; get; } = MV_CAM_GAIN_MODE.MV_GAIN_MODE_ONCE;
            /// <summary>
            /// 应用于图像的增益，单位为dB，Float类型，默认0.00
            /// </summary>
            [StringValue("设置图像的增益失败")]
            public double Gain { set; get; } = 10.00;
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

            [StringValue("设置设备提供的图像宽度（像素）失败")]
            public int Width { set; get; } = 3072;

            [StringValue("设置设备提供的图像的高度（像素）失败")]
            public int Height { set; get; } = 2048;

            [StringValue("设置从原点到AOI的垂直偏移（像素）失败")]

            public int OffsetX { set; get; } = 0;

            [StringValue("设置从原点到AOI的水平偏移（像素）失败")]
            public int OffsetY { set; get; } = 0;




        }




        /// <summary>
        /// 相机属性参数
        /// </summary>
        public class MVS_Camera_Info_Model
        {
            /// <summary>
            /// 有关设备的制造商信息
            /// </summary>
            [StringValue("获得有关设备的制造商信息~失败!")]
            public string DeviceManufacturerInfo { set; get; } = "";
            /// <summary>
            /// 设备的型号名称
            /// </summary>
            [StringValue("获得设备的型号名称~失败!")]
            public string DeviceModelName { set; get; } = "";
            /// <summary>
            /// 设备序列号
            /// </summary>
            [StringValue("获得设备序列号。此字符串是设备的唯一标识符~失败!")]
            public string DeviceSerialNumber { set; get; } = "";
            /// <summary>
            /// 图像的最大宽度
            /// </summary>
            [StringValue("获得图像的最大宽度（以像素为单位）~失败!")]
            public int WidthMax { set; get; } = 0;
            /// <summary>
            /// 图像的最大高度
            /// </summary>
            [StringValue("获得图像的最大高度（以像素为单位）~失败!")]
            public int HeightMax { set; get; } = 0;
            /// <summary>
            /// 最大采集帧速率
            /// </summary>
            [StringValue("获得允许的最大采集帧速率的“绝对”值~失败!")]
            public double ResultingFrameRate { set; get; } = 0;
            /// <summary>
            /// 网络接口的当前IP地址
            /// </summary>
            [StringValue("获得给定网络接口的当前IP地址~失败!")]
            public int GevCurrentIPAddress { set; get; } = 0;
        }


        /// <summary>
        /// 图像显示模型
        /// </summary>
        public class MVS_Image_Mode
        {
            /// <summary>
            /// 图像缓存,只读
            /// </summary>
            public byte[] pData_Buffer;

            private IntPtr _PData = IntPtr.Zero;
            /// <summary>
            /// 图像缓存开始句柄
            /// </summary>
            public IntPtr PData
            {
                get
                {
                    if (pData_Buffer != null)
                    {

                        return _PData = Marshal.UnsafeAddrOfPinnedArrayElement((Array)pData_Buffer, 0);
                    }
                    return _PData;
                }
                set { _PData = value; }
            }


            /// <summary>
            /// 图像信息(仅包含图像基本信息，不含图像缓存)
            /// </summary>
            public CFrameoutEx FrameEx_Info = new CFrameoutEx();


            /// <summary>
            /// 图像信息(仅包含图像基本信息，不含图像缓存)
            /// </summary>
            public CFrameout Frame_Info = new CFrameout();


            /// <summary>
            /// 用户信息句柄
            /// </summary>
            public IntPtr pUser = IntPtr.Zero;


        }







    }

    /// <summary>
    /// 采集模式中文枚举名
    /// </summary>
    public enum ACQUISITION_MODE_Enum
    {
        单帧模式,
        多帧模式,
        持续采集模式
    }


}
