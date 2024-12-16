using Generic_Extension;
using Halcon_SDK_DLL;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using MVS_SDK;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

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
            //[StringValue("选择相机设备被占用相机不可使用")]
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

            /// <summary>
            /// 使能输出信号输出到所选线路
            /// </summary>
            [StringValue("设置使能输出信号输出到所选线路失败")]
            LineInverter,

            [StringValue("获取一帧图片数据，或超时失败")]
            GetImageBuffer,

            [StringValue("清除缓存图片数据失败")]
            FreeImageBuffer,


            [StringValue("设置缓存图片数量失败")]
            SetImageNodeNum,



            [StringValue("软触发取图失败")]
            Command_TriggerSoftware
        }

        /// <summary>
        /// 相机参数类型
        /// </summary>
        [Serializable]
        [AddINotifyPropertyChangedInterface]
        public class MVS_Camera_Parameter_Model
        {
            public MVS_Camera_Parameter_Model(MVS_Camera_Parameter_Model _Param)
            {
                AcquisitionMode = _Param.AcquisitionMode;
                AcquisitionFrameRateEnable = _Param.AcquisitionFrameRateEnable;
                ExposureTime = _Param.ExposureTime;
                ExposureAuto = _Param.ExposureAuto;
                //ExposureMode = _Param.ExposureMode;
                TriggerMode = _Param.TriggerMode;
                TriggerActivation = _Param.TriggerActivation;
                //TriggerDelay = _Param.TriggerDelay;
                //TriggerCacheEnable = _Param.TriggerCacheEnable;
                GainAuto = _Param.GainAuto;
                Gain = _Param.Gain;
                DigitalShiftEnable = _Param.DigitalShiftEnable;
                DigitalShift = _Param.DigitalShift;
                BlackLevelEnable = _Param.BlackLevelEnable;
                BlackLevel = _Param.BlackLevel;
                GammaEnable = _Param.GammaEnable;
                Gamma = _Param.Gamma;
                ReverseX = _Param.ReverseX;
                //OffsetX = _Param.OffsetX;
                //OffsetY = _Param.OffsetY;
                LineSelector = _Param.LineSelector;
                LineMode = _Param.LineMode;
                LineInverter = _Param.LineInverter;
                StrobeEnable = _Param.StrobeEnable;
                Camera_Lighting_Control = _Param.Camera_Lighting_Control;
                //ResultingFrameRate = _Param.ResultingFrameRate;

            }

            public MVS_Camera_Parameter_Model()
            {

            }


            private Camera_Lighting_Control_Type_Enum _Camera_Lighting_Control = Camera_Lighting_Control_Type_Enum.自动;

            /// <summary>
            /// 相机控制光源选项
            /// </summary>
            public Camera_Lighting_Control_Type_Enum Camera_Lighting_Control
            {
                get { return _Camera_Lighting_Control; }
                set
                {
                    _Camera_Lighting_Control = value;
                    switch (value)
                    {
                        case Camera_Lighting_Control_Type_Enum.开:
                            LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                            StrobeEnable = false;
                            LineInverter = true;
                            break;
                        case Camera_Lighting_Control_Type_Enum.关:
                            LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                            StrobeEnable = false;
                            LineInverter = false;
                            break;
                        case Camera_Lighting_Control_Type_Enum.自动:

                            LineSelector = MV_CAM_LINESELECTOR_MODE.Lin1;
                            StrobeEnable = true;
                            LineInverter = false;
                            break;

                    }
                }
            }





            /// <summary>
            /// 设备采集的采集模式、枚举类型值 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS"
            /// </summary>
            [Display(Order = 1)]
            [StringValue("设置相机触发模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_ACQUISITION_MODE AcquisitionMode { set; get; } = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_SINGLE;
            /// <summary>
            /// 每个帧突发开始触发信号采集的帧数、整数类型——默认1，最大1023
            /// </summary>
            [Display(Order = 2)]
            [StringValue("设置每个帧突发开始触发信号采集的帧数失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool AcquisitionFrameRateEnable { set; get; } = true;



            /// <summary>
            /// 曝光模式定时时的曝光时间
            /// </summary>
            [Display(Order = 3)]
            [StringValue("设置曝光模式定时时的曝光时间失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double ExposureTime { set; get; } = 20000;
            /// <summary>
            /// 设置定时曝光模式时的自动曝光模式，枚举类型——默认连续模式，"MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF"
            /// </summary>
            [Display(Order = 4)]
            [StringValue("设置控制抓取帧的采集频率失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_EXPOSURE_MODE ExposureAuto { set; get; } = MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED;
            /// <summary>
            /// 设置曝光（或快门）的工作模式,枚举类型——默认定时模式，"MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED"
            /// </summary>
            //[StringValue("设置曝光（或快门）的工作模式失败")]
            //[Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            //public MV_CAM_EXPOSURE_MODE ExposureMode { set; get; } = MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED;



            /// <summary>
            /// 控制所选触发器是否处于活动状态、枚举类型——默认Off，"MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF"
            /// </summary>
            [Display(Order = 5)]
            [StringValue("设置控制所选触发器是否处于活动状态失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_TRIGGER_MODE TriggerMode { set; get; } = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;


            /// <summary>
            ///  指定用作触发源的内部信号或物理输入线路。所选触发器的触发模式必须设置为“开”。
            /// </summary>
            [Display(Order = 6)]
            [StringValue("设置触发源路径失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_TRIGGER_SOURCE TriggerSource { set; get; } = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;


            [Display(Order = 7)]
            [StringValue("设置触发器的激活模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_TRIGGER_ACTIVATION TriggerActivation { set; get; } = MV_CAM_TRIGGER_ACTIVATION.LevelHigh;



            //[StringValue("设置启用触发器缓存失败")]
            //[Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            //public bool TriggerCacheEnable { set; get; } = false;
            /// <summary>
            /// 指定在激活触发接收之前要应用的延迟（以us为单位）
            /// </summary>
            //[Display(Order = 8)]
            //[StringValue("设置指定在激活触发接收之前要应用的延迟失败")]
            //[Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            //public double TriggerDelay { set; get; } = 0.00;
            /// <summary>
            /// 设置自动增益控制（AGC）模式，枚举类型——默认，"MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF"
            /// </summary>
            [Display(Order = 9)]
            [StringValue("设置自动增益控制（AGC）模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_GAIN_MODE GainAuto { set; get; } = MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF;
            /// <summary>
            /// 应用于图像的增益，单位为dB，Float类型，默认0.00
            /// </summary>
            [Display(Order = 10)]
            [StringValue("设置图像的增益失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double Gain { set; get; } = 10.00;
            /// <summary>
            /// 使能/禁用数字移位调节，布尔类型——默认false
            /// </summary>
            [Display(Order = 11)]
            [StringValue("设置使能/禁用数字移位调节失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool DigitalShiftEnable { set; get; } = true;
            /// <summary>
            /// 设置选定的数字移位控制，Float类型——默认0
            /// </summary>
            [Display(Order = 12)]
            [StringValue("设置数字移位控制失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double DigitalShift { set; get; } = 0.00;
            /// <summary>
            /// 使能/禁用黑电平调整，布尔类型——默认true；
            /// </summary>
            [Display(Order = 13)]
            [StringValue("设置使能/禁用黑电平调整失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool BlackLevelEnable { set; get; } = true;
            /// <summary>
            /// 模拟黑电平百分比，整数类型——默认200，最小0，最大4095
            /// </summary>
            [Display(Order = 14)]
            [StringValue("设置模拟黑电平百分比失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public int BlackLevel { set; get; } = 200;
            /// <summary>
            /// 使能/禁用伽马校正，布尔类型——默认true；
            /// </summary>
            [Display(Order = 15)]
            [StringValue("设置使能/禁用伽马校正失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool GammaEnable { set; get; } = true;
            /// <summary>
            /// 控制像素强度的伽马校正，双精度类型——默认0.5，最小0，最大4
            /// </summary>
            [Display(Order = 16)]
            [StringValue("设置控制像素强度的伽马校正失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double Gamma { set; get; } = 0.75;
            /// <summary>
            /// 图像的锐度，布尔类型——默认false；
            /// </summary>
            //[StringValue("设置使能/禁用伽马校正失败")]
            //public bool SharpnessEnable { set; get; } = true;
            /// <summary>
            /// 图像的锐度，整数类型——默认10，最小0，最大100
            /// </summary>
            //[StringValue("设置图像的锐度失败")]
            //public int Sharpness { set; get; } = 10;

            /// <summary>
            /// 水平翻转设备发送的图像。翻转后应用感兴趣区域，布尔类型——默认false
            /// </summary>
            [Display(Order = 17)]
            [StringValue("设置水平翻转设备发送的图像失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool ReverseX { set; get; } = false;

            //[StringValue("设置设备提供的图像宽度（像素）失败")]
            //public int Width { set; get; } = 3072;

            //[StringValue("设置设备提供的图像的高度（像素）失败")]
            //public int Height { set; get; } = 2048;

            //[StringValue("设置从原点到AOI的垂直偏移（像素）失败")]
            //[Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            //public int OffsetX { set; get; } = 0;

            //[StringValue("设置从原点到AOI的水平偏移（像素）失败")]
            //[Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            //public int OffsetY { set; get; } = 0;

            [Display(Order = 18)]
            [StringValue("设置选择要配置的外部设备连接器的物理线（或管脚）失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_LINESELECTOR_MODE LineSelector { set; get; } = MV_CAM_LINESELECTOR_MODE.Lin1;

            [Display(Order = 19)]
            [StringValue("设置线路模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_LINEMODE_MODE LineMode { set; get; } = MV_CAM_LINEMODE_MODE.Strobe;

            [Display(Order = 20)]
            [StringValue("设置使能输出信号输出到所选线路失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool StrobeEnable { set; get; } = true;

            [Display(Order = 21)]
            [StringValue("设置控制所选输入或输出线的信号反转失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool LineInverter { set; get; } = false;


            /// <summary>
            /// 图像的最大宽度
            /// </summary>
            [Display(Order = 22)]

            [StringValue("获得图像的最大宽度（以像素为单位）~失败!")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Read)]
            public int WidthMax { set; get; } = 0;
            /// <summary>
            /// 图像的最大高度
            /// </summary>
            [Display(Order = 23)]

            [StringValue("获得图像的最大高度（以像素为单位）~失败!")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Read)]

            public int HeightMax { set; get; } = 0;
            /// <summary>
            /// 最大采集帧速率
            /// </summary>
            [Display(Order = 24)]

            [StringValue("获得允许的最大采集帧速率的“绝对”值~失败!")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Read)]
            public double ResultingFrameRate { set; get; } = 0;





        }
        /// <summary>
        /// 相机标定类型
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Camera_Calibration_Info_Model
        {
            /// <summary>
            /// 标定精度
            ///// </summary>
            //public double Calibrated_Accuracy { set; get; } 



            /// <summary>
            /// 标定结状态
            /// </summary>
            public Camera_Calibration_File_Type_Enum Camera_Calibration_State { set; get; } = Camera_Calibration_File_Type_Enum.无标定;


            /// <summary>
            /// 相机标定操作
            /// </summary>
            public Camera_Calibration_Mobile_Type_Enum Camera_Calibration_Setup { set; get; } = Camera_Calibration_Mobile_Type_Enum.UnCalibration;

            /// <summary>
            /// 相机标定主副相机
            /// </summary>
            public Camera_Calibration_MainOrSubroutine_Type_Enum Camera_Calibration_MainOrSubroutine_Type { set; get; } = Camera_Calibration_MainOrSubroutine_Type_Enum.None;

            /// <summary>
            /// 相机标定参数
            /// </summary>
            public Halcon_Camera_Calibration_Parameters_Model Camera_Calibration_Paramteters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();


            /// <summary>
            /// 相机手眼标定位置
            /// </summary>
            public Point_Model HandEye_ToolinCamera { set; get; }

            /// <summary>
            /// 设备图像来源设置
            /// </summary>
            public Image_Diver_Model_Enum HaneEye_Calibration_Diver_Model { get; set; } = Image_Diver_Model_Enum.Online;


        }


        /// <summary>
        /// 读取相机信息模型
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Get_Camera_Info_Model
        {


            /// <summary>
            /// 网络接口的当前IP地址
            /// </summary>
            [StringValue("获得给定网络接口的当前IP地址~失败!")]
            public int GevCurrentIPAddress { set; get; } = 0;

            /// <summary>
            /// 当前IP 
            /// </summary>
            [StringValue("获得当前IP地址~失败!")]
            public string CurrentIp { set; get; } = "";

            /// <summary>
            /// 当前子网掩码
            /// </summary>
            [StringValue("获得当前子网掩码~失败!")]
            public string CurrentSubNetMask { set; get; } = "";

            /// <summary>
            /// 默认网关
            /// </summary>
            [StringValue("获得当前默认网关~失败!")]
            public string DefultGateWay { set; get; } = "";

            /// <summary>
            /// 网卡IP 
            /// </summary>
            [StringValue("获得当前默认网关~失败!")]
            public string NetExport { set; get; } = "";


            /// <summary>
            /// 相机厂商
            /// </summary>
            [StringValue("获得相机厂商~失败!")]
            public string ManufacturerName { set; get; } = "";

            /// <summary>
            /// 相机型号
            /// </summary>
            [StringValue("获得相机型号~失败!")]
            public string ModelName { set; get; } = "";
            /// <summary>
            /// 相机版本
            /// </summary>
            [StringValue("获得相机版本~失败!")]
            public string DeviceVersion { set; get; } = "";

            /// <summary>
            /// 相机厂家信息
            /// </summary>
            [StringValue("获得相机厂商信息~失败!")]
            public string ManufacturerSpecificInfo { set; get; } = "";
            /// <summary>
            /// 相机唯一序列号
            /// </summary>
            [StringValue("获得相机序列号~失败!")]
            public string SerialNumber { set; get; } = "";
            /// <summary>
            /// 使用者设备IP
            /// </summary>
            [StringValue("获得相机设备主机ip ~失败!")]
            public string HostIp { set; get; } = "";
            /// <summary>
            /// 相机类型
            /// </summary>
            [StringValue("获得相机类型 ~失败!")]
            public MV_CAM_DeviceType_Enum DeviceType { set; get; }

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


        }



        /// <summary>
        /// 相机属性参数
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class MVS_Camera_Info_Model
        {

            /// <summary>
            /// 相机对象初始化
            /// </summary>
            /// <param name="_Camera"></param>
            public MVS_Camera_Info_Model(CCameraInfo _Camera)
            {
                MVS_CameraInfo = _Camera;
                //只支持GIGE相机
                if (_Camera.nTLayerType == CSystem.MV_GIGE_DEVICE)
                {
                    //转换
                    CGigECamera = _Camera as CGigECameraInfo;
                }

                Get_HCamPar_File();


            }
            public MVS_Camera_Info_Model()
            {



            }

            /// <summary>
            /// 读取相机信息
            /// </summary>
            public Get_Camera_Info_Model Camera_Info { set; get; } = new Get_Camera_Info_Model();

            /// <summary>
            /// 相机标定属性
            /// </summary>
            public Camera_Calibration_Info_Model Camera_Calibration { set; get; } = new Camera_Calibration_Info_Model();





            /// <summary>
            /// 相机设备句柄,首次检测设置
            /// </summary>
            public CCameraInfo MVS_CameraInfo;


            /// <summary>
            ///  用户选择相机对象操作
            /// </summary>
            public CCamera Camera { set; get; } = new CCamera();


            /// <summary>
            /// 图像需要显示位置
            /// </summary>
            public Window_Show_Name_Enum Show_Window { set; get; } = Window_Show_Name_Enum.Features_Window;

            /// <summary>
            /// 相机是否实时模式
            /// </summary>
            public bool Camera_Live { set; get; } = false;




            /// <summary>
            /// 相机检测延时
            /// </summary>
            public int Camera_Check_Delay { set; get; } = 0;


            /// <summary>
            /// 泛型类型委托声明
            /// </summary>
            /// <param name="_Connect_State"></param>
            public delegate void MVS_T_delegate<T>(T _Tl);




            /// <summary>
            /// 相机设置错误委托属性
            /// </summary>
            public MVS_T_delegate<string> ErrorInfo_delegate { set; get; }


            /// <summary>
            /// 相机设备当前状态
            /// </summary>
            public MV_CAM_Device_Status_Enum Camer_Status { set; get; } = MV_CAM_Device_Status_Enum.Null;


            /// <summary>
            /// 相机工作状态
            /// </summary>
            public MV_CAM_Camera_Work_State_Enum Camera_Work_State { set; get; } = MV_CAM_Camera_Work_State_Enum.Camera_Ready;


            private CGigECameraInfo _CGigECamera;
            /// <summary>
            /// 相机原参数信息
            /// </summary>
            public CGigECameraInfo CGigECamera
            {
                get { return _CGigECamera; }
                set
                {
                    _CGigECamera = value;


                    Camera_Info.DeviceVersion = _CGigECamera.chDeviceVersion;
                    Camera_Info.ModelName = _CGigECamera.chModelName;
                    Camera_Info.ManufacturerSpecificInfo = _CGigECamera.chManufacturerSpecificInfo;
                    Camera_Info.SerialNumber = _CGigECamera.chSerialNumber;
                    Camera_Info.ManufacturerName = _CGigECamera.chManufacturerName;


                    Camera_Info.CurrentIp = IP_intTOString(_CGigECamera.nCurrentIp);
                    Camera_Info.CurrentSubNetMask = IP_intTOString(_CGigECamera.nCurrentSubNetMask);
                    Camera_Info.DefultGateWay = IP_intTOString(_CGigECamera.nDefultGateWay);
                    Camera_Info.NetExport = IP_intTOString(_CGigECamera.nNetExport);
                    Camera_Info.HostIp = IP_intTOString(_CGigECamera.nHostIp);
                    Camera_Info.DeviceType = (MV_CAM_DeviceType_Enum)_CGigECamera.nDeviceType;


                }
            }


            /// <summary>
            /// 获得相机信息方法
            /// </summary>
            /// <param name="_Info"></param>
            /// <returns></returns>
            public MVS_Camera_Parameter_Model Get_Camrea_Parameters()
            {


                MVS_Camera_Parameter_Model _Camera_Parameter_Model = new();


                foreach (PropertyInfo _Type in _Camera_Parameter_Model.GetType().GetProperties())
                {

                    try
                    {



                        object _Val = new();

                        //读取标记读取属性
                        Camera_ReadWriteAttribute _CameraRW_Type = (Camera_ReadWriteAttribute)_Type.GetCustomAttribute(typeof(Camera_ReadWriteAttribute));



                        if (_CameraRW_Type != null && _CameraRW_Type.GetCamera_ReadWrite_Type() == Camera_Parameter_RW_Type.Read)
                        {

                            if (Get_Camera_Info_Val(Camera, _Type, _Type.Name, ref _Val).GetResult())
                            {
                                _Type.SetValue(_Camera_Parameter_Model, _Val);
                            }

                        }
                    }


                    catch (Exception _e)
                    {
                        throw new Exception("相机参数名 : " + _Type.Name + " 失败！" + "详细：" + _e.Message);
                        //new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "获取相机全部参数名成功！" };

                    }

                }


                return _Camera_Parameter_Model;

            }





            /// <summary>
            /// 设置总相机相机俩表
            /// </summary>
            /// <param name="_Camera_List"></param>
            public void Set_Camrea_Parameters_List(MVS_Camera_Parameter_Model _Parameter)
            {

                try
                {




                    var properties = _Parameter.GetType().GetProperties()
                        .OrderBy(p => ((DisplayAttribute)p.GetCustomAttribute(typeof(DisplayAttribute)))
                        ?.Order ?? int.MaxValue);

                    //遍历设置参数,调整参数顺序
                    foreach (PropertyInfo _Type in properties)
                    {
                        Camera_ReadWriteAttribute _CameraRW_Type = (Camera_ReadWriteAttribute)_Type.GetCustomAttribute(typeof(Camera_ReadWriteAttribute));

                        if (_CameraRW_Type != null && _CameraRW_Type.GetCamera_ReadWrite_Type() == Camera_Parameter_RW_Type.Write)
                        {

                            ///针对个别参数设置跳过
                            if (_Type.Name == nameof(MVS_Camera_Parameter_Model.TriggerActivation))
                            {

                                if ((MV_CAM_TRIGGER_SOURCE)properties.FirstOrDefault((_) => _.Name == nameof(MVS_Camera_Parameter_Model.TriggerSource)).GetValue(_Parameter) == MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
                                {
                                    continue;
                                }


                            }


                            if (!Set_Camera_Parameters_Val(Camera, _Type, _Type.Name, _Type.GetValue(_Parameter)))
                            {


                                throw new Exception(MVE_Result_Enum.相机参数设置错误 + "_参数名：" + _Type.Name);
                            }

                        }
                    }
                }


                catch (Exception e)
                {
                    throw new Exception(MVE_Result_Enum.相机参数设置错误 + " 原因：" + e.Message);

                }


            }

            /// <summary>
            /// 获得一图像显示到指定窗口
            /// </summary>
            /// <param name="_HWindow"></param>
            public HImage GetOneFrameTimeout(MVS_Camera_Parameter_Model _Camera_parameter)
            {


                try
                {
                    HImage _HImage = new();

                    if (Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                    {
                        Connect_Camera();
                    }

                    //设置为单帧模式
                    //_Camera_parameter.AcquisitionMode = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_SINGLE;

                    //Set_Camrea_Parameters_List(Camera_parameter);
                    if (_Camera_parameter != null)
                    {

                        //设置相机总参数
                        Set_Camrea_Parameters_List(_Camera_parameter);
                    }

                    StartGrabbing();

                    //获得一帧图片信息
                    MVS_Image_Mode _MVS_Image = MVS_GetOneFrameTimeout();

                    StopGrabbing();
                    //转换Halcon图像变量
                    _HImage = new Halcon_External_Method_Model().Mvs_To_Halcon_Image(_MVS_Image.FrameEx_Info.pcImageInfoEx.Width, _MVS_Image.FrameEx_Info.pcImageInfoEx.Height, _MVS_Image.PData);



                    return _HImage;

                }
                catch (Exception _e)
                {

                    throw new Exception(Camera_Info.SerialNumber + "相机获得图像失败！原因：" + _e.Message);

                }






            }






            /// <summary>
            /// 使用回调函数取图方法
            /// </summary>
            /// <param name="CommVale">是否使用命令提前</param>
            /// <param name="_Timeout">取图等待超时</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public MVS_Image_Mode MSV_GetImageCallback(bool CommVale = false, int _Timeout = 10000)
            {
                MVS_Image_Mode Image_Data = new();

                ManualResetEvent Camera_Signal = new ManualResetEvent(false);


                try
                {



                    // 注册回调函数，直接使用 Lambda 表达式定义逻辑
                    Camera.RegisterImageCallBackEx((IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser) =>
                    {
                        DateTime now = DateTime.Now;
                        Image_Data.Callback_pFrameInfo = pFrameInfo;
                        Image_Data.PData = pData;

                        Debug.WriteLine($"{Camera_Info.SerialNumber} 当前时间：{now:yyyy-MM-dd HH:mm:ss.fff}");

                        Camera_Signal.Set();

                    }, IntPtr.Zero);


                    //注册回调后开始取流
                    StartGrabbing();



                    // 如果需要，发送软触发命令
                    if (CommVale)
                    {
                        if (!Set_Camera_Val(Camera_Parameters_Name_Enum.Command_TriggerSoftware, Camera.SetCommandValue("TriggerSoftware")))
                        {
                            throw new Exception("软触发命令失败，请检查配置设置！");
                        }
                    }


                    if (!Camera_Signal.WaitOne(_Timeout))
                    {

                        throw new TimeoutException("软触发等待图像超时，请检查相机配置！");
                    }
                    return Image_Data;


                }
                catch (Exception e)
                {


                    throw new Exception("使用回调函数取图失败！原因：" + e.Message);
                }
                finally
                {
                    StopGrabbing();
                    Camera.RegisterImageCallBackEx(null, IntPtr.Zero);// 释放回调
                }

            }




            //public MVS_Image_Mode MVS_GetImageBuffer(int _Timeout = 1000)
            //{

            //    MVS_Image_Mode Image_Data = new();

            //    CFrameout pcFrame = new CFrameout();



            //    //while (true) 
            //    //{


            //    Set_Camera_Val(Camera_Parameters_Name_Enum.GetImageBuffer, Camera.GetImageBuffer(ref pcFrame, _Timeout));
            //    DateTime now = DateTime.Now;
            //    Debug.WriteLine($"{Camera_Info.SerialNumber} 当前时间：{now:yyyy-MM-dd HH:mm:ss.fff}");
            //    if (pcFrame.Image != null)
            //    {
            //        Image_Data.Frame_Info = pcFrame;

            //    }
            //    else
            //    {
            //        //continue;
            //    }
            //    return Image_Data;


            //    //Image_Data.pData_Buffer = pcFrame.Image.ImageData;
            //    //Image_Data.PData = pcFrame.Image.ImageAddr;

            //    //}



            //    //return Image_Data;

            //}



            /// <summary>
            /// 获得一帧图像方法
            /// </summary>
            /// <param name="_Timeout"></param>
            /// <returns></returns>
            public MVS_Image_Mode MVS_GetOneFrameTimeout(int _Timeout = 10000)
            {



                try
                {




                    CIntValue stParam = new();

                    Camera.ClearImageBuffer();

                    ////开始取流

                    //获取图像缓存大小
                    Set_Camera_Val(Camera_Parameters_Name_Enum.PayloadSize, Camera.GetIntValue("PayloadSize", ref stParam));




                    //创建帧图像信息
                    MVS_Image_Mode Frame_Image = new()
                    {

                        pData_Buffer = new byte[stParam.CurValue]
                    };

                    lock (Frame_Image)
                    {


                        //抓取一张图片
                        if (Set_Camera_Val(Camera_Parameters_Name_Enum.GetOneFrameTimeout, Camera.GetOneFrameTimeout(Frame_Image.pData_Buffer, (uint)stParam.CurValue, ref Frame_Image.FrameEx_Info, _Timeout)))
                        {
                            //StopGrabbing(_CameraInfo);
                            Camera.ClearImageBuffer();
                            return Frame_Image;
                        }

                    }


                }
                catch (Exception _e)
                {

                    throw new Exception("相机设备获取图像数据失败！" + " 原因：" + _e.Message);
                }

                return new MVS_Image_Mode();


            }



            public void Start_ImageCallback_delegate(MVS_Camera_Parameter_Model _Parameter, cbOutputExdelegate _Func)
            {

                //相机未连接会连接再操作
                if (Camer_Status != MV_CAM_Device_Status_Enum.Connecting)
                {
                    Connect_Camera();
                }

                //清楚取流后设置相机采集参数
                StopGrabbing();
                Set_Camrea_Parameters_List(_Parameter);

                //设置GEGI网络包大小
                Set_Camera_GEGI_GevSCPSPacketSize();
                //创建抓图回调函数
                RegisterImageCallBackEx(_Func);
                //开始取流
                StartGrabbing();



            }


            public void Stop_ImageCallback_delegate()
            {
                //开始取流流
                StopGrabbing();
                //清楚回调函数
                RegisterImageCallBackEx(null);


            }


            /// <summary>
            /// 相机开始取流方法
            /// </summary>
            /// <returns></returns>
            public void StartGrabbing()
            {


                try
                {


                    if (!Set_Camera_Val(Camera_Parameters_Name_Enum.StartGrabbing, Camera.SetImageNodeNum(1)))
                    {
                        throw new Exception();

                    }


                    //if (!Set_Camera_Val(Camera_Parameters_Name_Enum.StartGrabbing, Camera.SetGrabStrategy(MV_GRAB_STRATEGY.MV_GrabStrategy_LatestImagesOnly)))
                    //{
                    //    throw new Exception();

                    //}

                    if (!Set_Camera_Val(Camera_Parameters_Name_Enum.StartGrabbing, Camera.StartGrabbing()))
                    {
                        throw new Exception("开始取流失败！");

                    }


                }
                catch (Exception _e)
                {

                    throw new Exception("相机取流失败！原因 ：" + _e.Message);
                }

            }



            /// <summary>
            /// 相机停止取流
            /// </summary>
            /// <returns></returns>
            public void StopGrabbing()
            {


                try
                {


                    Set_Camera_Val(Camera_Parameters_Name_Enum.StopGrabbing, Camera.StopGrabbing());

                    //清空回调
                    //Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, Camera.RegisterImageCallBackEx(null, IntPtr.Zero));


                }
                catch (Exception _e)
                {

                    throw new Exception("相机停止取流失败！原因 ：" + _e.Message);
                }


                //停止取流

            }

            /// <summary>
            /// 利用反射设置相机属性参数
            /// </summary>
            /// <param name="_Val_Type"></param>
            /// <param name="_name"></param>
            /// <param name="_val"></param>
            private bool Set_Camera_Parameters_Val(CCamera _Camera, PropertyInfo _Val_Type, string _name, object _val)
            {
                //初始化设置相机状态
                bool _Parameters_Type = false;

                //对遍历参数类型分类
                switch (_Val_Type.PropertyType)
                {
                    case Type _T when _T.BaseType == typeof(Enum):

                        //设置相机参数
                        _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetEnumValue(_name, Convert.ToUInt32(_val)));




                        break;
                    case Type _T when _T == typeof(Int32):

                        //设置相机参数
                        _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetIntValue(_name, (int)_val));


                        break;
                    case Type _T when _T == typeof(double):
                        //设置相机参数
                        _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetFloatValue(_name, Convert.ToSingle(_val)));


                        break;

                    case Type _T when _T == typeof(string):
                        //设置相机参数
                        _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetStringValue(_name, _val.ToString()));


                        break;
                    case Type _T when _T == typeof(bool):
                        //设置相机参数
                        _Parameters_Type = Set_Camera_Val(_Val_Type, _Camera.SetBoolValue(_name, (bool)_val));


                        break;
                }

                return _Parameters_Type;

            }

            /// <summary>
            /// 关闭相机
            /// </summary>
            /// <returns></returns>
            public void Close_Camera()
            {
                if (Camera != null)
                {

                    //关闭相机
                    Camera.CloseDevice();
                    //Camera.DestroyHandle();
                    Camer_Status = MV_CAM_Device_Status_Enum.Null;

                    //return new MPR_Status_Model(MVE_Result_Enum.关闭相机成功) { Result_Error_Info = _Select_Camera.Camera_Info.ModelName };
                }


                //断开连接后可以再次连接相机

            }



            /// <summary>
            /// 利用反射设置相机属性参数
            /// </summary>
            /// <param name="_Val_Type"></param>
            /// <param name="_name"></param>
            /// <param name="_val"></param>
            public MPR_Status_Model Get_Camera_Info_Val(CCamera _Camera, PropertyInfo _Val_Type, string _name, ref object _Value)
            {
                //初始化设置相机状态
                bool _Parameters_Type = false;


                //对遍历参数类型分类
                switch (_Val_Type.PropertyType)
                {
                    case Type _T when _T.BaseType == typeof(Enum):

                        CEnumValue _EnumValue = new();

                        //设置相机参数
                        _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetEnumValue(_name, ref _EnumValue));




                        //_Value = _EnumValue.CurValue;
                        _Value = Enum.Parse(_T, _EnumValue.CurValue.ToString());
                        break;
                    case Type _T when _T == typeof(Int32):

                        CIntValue _IntValue = new();


                        //设置相机参数
                        _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetIntValue(_name, ref _IntValue));
                        _Value = (int)_IntValue.CurValue;


                        //IP地址提取方法
                        //var b = (_IntValue.CurValue) >> 24;
                        //var bb = (_IntValue.CurValue) >> 16;
                        //var bbb = (_IntValue.CurValue & 0x0000FF00) >> 8;
                        //var bbbb = _IntValue.CurValue & 0x000000FF;

                        break;
                    case Type _T when _T == typeof(double):
                        //设置相机参数
                        CFloatValue _DoubleValue = new();



                        _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetFloatValue(_name, ref _DoubleValue));
                        _Value = _DoubleValue.CurValue;


                        break;

                    case Type _T when _T == typeof(string):
                        //设置相机参数

                        CStringValue _StringValue = new();

                        _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetStringValue(_name, ref _StringValue));

                        _Value = _StringValue.CurValue;


                        break;
                    case Type _T when _T == typeof(bool):
                        //设置相机参数

                        bool _BoolValue = false;

                        _Parameters_Type = Get_Camera_Val(_Val_Type, _Camera.GetBoolValue(_name, ref _BoolValue));

                        _Value = _BoolValue;

                        break;
                }

                if (_Parameters_Type)
                {
                    return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = "获取相机参数名：" + _name + " 数值：" + _Value + " 成功！" };
                }
                else
                {
                    return new MPR_Status_Model(MVE_Result_Enum.获得相机参数设置错误) { Result_Error_Info = "_参数名：" + _name + "数值：" + _Value };
                }



            }

            /// <summary>
            ///  设置参数相机状态码委托返回显示
            /// </summary>
            /// <param name="_name">相机参数名称枚举</param>
            /// <param name="_key">相机状态码</param>
            public bool Set_Camera_Val<T1, T2>(T1 _name, T2 _key)
            {
                var aa = _name.GetType();


                //不同名称类型分别处理
                switch (_name)
                {
                    case T1 _ when _name is Camera_Parameters_Name_Enum:



                        Enum _Ename = _name as Enum;


                        switch (_key)
                        {
                            case T2 _ when _key is int Tint:
                                //创建失败方法
                                if (CErrorDefine.MV_OK != Tint)
                                {
                                    ErrorInfo_delegate?.Invoke("参数 : " + _name + " | 数值 : " + _Ename.GetStringValue());
                                    return false;
                                }

                                break;
                            case T2 _ when _key is bool Tbool:
                                //创建失败方法
                                if (false == Tbool)
                                {
                                    ErrorInfo_delegate?.Invoke("参数 : " + _name + " | 数值 : " + _Ename.GetStringValue());
                                    return false;
                                }

                                break;

                        }



                        break;


                    case T1 _ when _name is PropertyInfo:



                        PropertyInfo _Tname = _name as PropertyInfo;

                        StringValueAttribute _ErrorInfo = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                        switch (_key)
                        {
                            case T2 _ when _key is int Tint:
                                //创建失败方法
                                if (CErrorDefine.MV_OK != Tint)
                                {
                                    var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                    ErrorInfo_delegate?.Invoke("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                    return false;
                                }

                                break;
                            case T2 _ when _key is bool Tbool:
                                //创建失败方法
                                if (false == Tbool)
                                {
                                    var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                    ErrorInfo_delegate?.Invoke("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                    return false;
                                }

                                break;

                        }





                        break;
                }


                return true;


            }



            /// <summary>
            /// 读取相机参数方法
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="_name"></param>
            /// <param name="_key"></param>
            /// <returns></returns>
            public bool Get_Camera_Val<T1, T2>(T1 _name, T2 _key)
            {

                if (_name is PropertyInfo _Tname)
                {



                    StringValueAttribute _ErrorInfo = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                    switch (_key)
                    {
                        case T2 _ when _key is int Tint:
                            //创建失败方法
                            if (CErrorDefine.MV_OK != Tint)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                ErrorInfo_delegate?.Invoke("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;
                        case T2 _ when _key is bool Tbool:
                            //创建失败方法
                            if (false == Tbool)
                            {
                                var a = (StringValueAttribute)_Tname.GetCustomAttribute(typeof(StringValueAttribute));

                                ErrorInfo_delegate?.Invoke("参数 : " + _Tname.Name + " | 数值 : " + _ErrorInfo.StringValue);
                                return false;
                            }

                            break;

                    }

                    return true;
                }




                return false;
            }




            /// <summary>
            /// 连接相机
            /// </summary>
            /// <returns></returns>
            public void Connect_Camera()
            {


                try
                {



                    //打开相机
                    Open_Camera();


                    MVS_Camera_Parameter_Model _Parameter = new();


                    //获得相机属性
                    _Parameter = Get_Camrea_Parameters();

                    //获得图像最大像素
                    Camera_Info.HeightMax = _Parameter.HeightMax;
                    Camera_Info.WidthMax = _Parameter.WidthMax;

                    if (Camera_Calibration.Camera_Calibration_State == Camera_Calibration_File_Type_Enum.无标定)
                    {
                        Camera_Calibration.Camera_Calibration_Paramteters.Image_Width = _Parameter.WidthMax;
                        Camera_Calibration.Camera_Calibration_Paramteters.Image_Height = _Parameter.HeightMax;
                        Camera_Calibration.Camera_Calibration_Paramteters.Cx = _Parameter.WidthMax * 0.5;
                        Camera_Calibration.Camera_Calibration_Paramteters.Cy = _Parameter.HeightMax * 0.5;
                    }

                    //标记相机连接成功
                    Camer_Status = MV_CAM_Device_Status_Enum.Connecting;





                }
                catch (Exception _e)
                {
                    Close_Camera();

                    throw new Exception("相机连接失败原因 ：" + _e.Message);

                }




            }


            /// <summary>
            /// 打开相机列表中的对应数好
            /// </summary>
            /// <param name="_Camera_Number"></param>
            public void Open_Camera()
            {





                //创建相机
                if (Set_Camera_Val(Camera_Parameters_Name_Enum.CreateHandle, Camera.CreateHandle(ref MVS_CameraInfo)) != true)
                {
                    throw new Exception("创建相机句柄失败！");

                    //return new MPR_Status_Model(MVE_Result_Enum.创建相机句柄失败);
                }


                //打开相机
                if (Set_Camera_Val(Camera_Parameters_Name_Enum.OpenDevice, Camera.OpenDevice()) != true)
                {
                    throw new Exception("打开相机失败！");

                    //return new MPR_Status_Model(MVE_Result_Enum.打开相机失败);
                }


                //打开相机失败返回值
                //return new MPR_Status_Model(MVE_Result_Enum.Run_OK) { Result_Error_Info = _CameraInfo.Camera_Info.SerialNumber + "相机打开成功！" };


            }





            /// <summary>
            /// 设置探测网络最佳包大小(只对GigE相机有效)
            /// </summary>
            /// <returns></returns>
            public bool Set_Camera_GEGI_GevSCPSPacketSize()
            {

                if (MVS_CameraInfo.nTLayerType == CSystem.MV_GIGE_DEVICE)
                {

                    int nPacketSize = Camera.GIGE_GetOptimalPacketSize();

                    if (nPacketSize > 0)
                    {
                        Set_Camera_Val(Camera_Parameters_Name_Enum.GIGE_GetOptimalPacketSize, CErrorDefine.MV_OK);
                        Set_Camera_Val(Camera_Parameters_Name_Enum.GevSCPSPacketSize, Camera.SetIntValue(nameof(Camera_Parameters_Name_Enum.GevSCPSPacketSize), (uint)nPacketSize));
                    }
                    else
                    {
                        Set_Camera_Val(Camera_Parameters_Name_Enum.GIGE_GetOptimalPacketSize, CErrorDefine.MV_E_RESOURCE);

                    }

                }


                return true;
            }


            /// <summary>
            /// 检查相机列表中选择相机是否可用
            /// </summary>
            /// <param name="_Camera_Number"></param>
            public bool Check_IsDeviceAccessible()
            {

                //读取选择相机信息
                //CameraInfo = Camera_List[_Camera_Number];


                //检查相机设备可用情况
                return CSystem.IsDeviceAccessible(ref MVS_CameraInfo, MV_ACCESS_MODE.MV_ACCESS_EXCLUSIVE);





            }


            /// <summary>
            /// 设置图像获取委托方法
            /// </summary>
            /// <param name="_delegate"></param>
            /// <returns></returns>
            public bool RegisterImageCallBackEx(cbOutputExdelegate _delegate)
            {

                return Set_Camera_Val(Camera_Parameters_Name_Enum.RegisterImageCallBackEx, Camera.RegisterImageCallBackEx(_delegate, IntPtr.Zero));

            }



            public bool FreeImageBuffer()
            {
                CFrameout _Frame = new();
                Camera.FreeImageBuffer(ref _Frame);

                return true;

            }


            /// <summary>
            /// 整数IP地址转换字符串
            /// </summary>
            /// <param name="_Ipaddress"></param>
            /// <returns></returns>
            private static string IP_intTOString(uint _Ipaddress)
            {

                string _IPString = ((_Ipaddress & 0xFF000000) >> 24).ToString() + "." + ((_Ipaddress & 0x00FF0000) >> 16).ToString() + "." + ((_Ipaddress & 0x0000FF00) >> 8).ToString() + "." + ((_Ipaddress & 0x000000FF)).ToString();


                return _IPString;

            }


            ///// <summary>
            ///// 相机对应内参
            ///// </summary>
            //public HCamPar HCamera_Param { set; get; } = new HCamPar();

            /// <summary>
            /// 读取相机内参文件
            /// </summary>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public void Get_HCamPar_File()
            {
                try
                {

                    string _Calib_File = Directory.GetCurrentDirectory() + "\\Calibration_File";

                    ///检查标定文件夹
                    if (!Directory.Exists(_Calib_File))
                    {
                        DirectoryInfo directoryInfo = new(_Calib_File);
                        directoryInfo.Create();
                    }

                    string _File = _Calib_File + "\\" + Camera_Info.SerialNumber + ".dat";

                    string _HandEye_File = _Calib_File + "\\HandEyeToolinCam_" + Camera_Info.SerialNumber + ".dat";


                    //加载手眼相机内参文件
                    if (File.Exists(_File))
                    {


                        HCamPar _CamP = new();
                        _CamP.ReadCamPar(_File);

                        Camera_Calibration.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_CamP);
                        Camera_Calibration.Camera_Calibration_State = Camera_Calibration_File_Type_Enum.内参标定;


                        //return true;

                    }



                    ///加载手眼文件
                    if (File.Exists(_HandEye_File))
                    {
                        HPose _HandEyeP = new();
                        _HandEyeP.ReadPose(_HandEye_File);

                        Camera_Calibration.HandEye_ToolinCamera = new Point_Model(_HandEyeP);
                        Camera_Calibration.Camera_Calibration_Setup = Camera_Calibration_Mobile_Type_Enum.Calibrated_OK;
                    }



                    //throw new Exception(Camera_Info.SerialNumber + "：相机没有内参信息，请把内参文件存放在："+_File);
                    //return false;


                }
                catch (Exception _e)
                {

                    throw new Exception(Camera_Info.SerialNumber + "：相机内参读取失败！原因：" + _e.Message);
                }

            }
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
            public CFrameoutEx FrameEx_Info = new();


            /// <summary>
            /// 图像信息(仅包含图像基本信息，不含图像缓存)
            /// </summary>
            public CFrameout Frame_Info = new();



            /// <summary>
            /// 回调参数的
            /// </summary>
            public MV_FRAME_OUT_INFO_EX Callback_pFrameInfo = new MV_FRAME_OUT_INFO_EX();


            /// <summary>
            /// 用户信息句柄
            /// </summary>
            public IntPtr pUser = IntPtr.Zero;


        }





    }


    /// <summary>
    /// 相机属性读取写入标识方法
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class Camera_ReadWriteAttribute : Attribute
    {

        public Camera_ReadWriteAttribute(Camera_Parameter_RW_Type _Enum)
        {
            ReadWrite_Type = _Enum;
        }
        public Camera_Parameter_RW_Type ReadWrite_Type;

        public Camera_Parameter_RW_Type GetCamera_ReadWrite_Type()
        {
            return ReadWrite_Type;
        }

    }


    /// <summary>
    /// 相机参数读写标识
    /// </summary>
    public enum Camera_Parameter_RW_Type
    {
        Read,
        Write,
        ReadorWrite
    }

    /// <summary>
    /// 相机错误消息返回
    /// </summary>
    public enum MVE_Result_Enum
    {
        Run_OK,
        相机参数设置错误,
        相机连接失败,
        创建相机句柄失败,
        打开相机失败,
        获得相机参数设置错误,
        关闭相机成功

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


    /// <summary>
    /// 线路选择器枚举
    /// </summary>
    public enum MV_CAM_LINESELECTOR_MODE
    {
        Lin0,
        Lin1,
        Lin2

    }

    /// <summary>
    /// 线路模式枚举
    /// </summary>
    public enum MV_CAM_LINEMODE_MODE
    {
        Strobe = 8
    }

    /// <summary>
    /// 相机设备类型
    /// </summary>
    public enum MV_CAM_DeviceType_Enum
    {
        普通设备,
        虚拟采集卡上的设备,
        自研网卡上的设备
    }


    /// <summary>
    /// 相机当前状态
    /// </summary>
    public enum MV_CAM_Device_Status_Enum
    {
        /// <summary>
        /// 设备空闲
        /// </summary>
        Null,
        /// <summary>
        /// 设备占用
        /// </summary>
        Possess,
        /// <summary>
        /// 设备连接中
        /// </summary>
        Connecting
    }


    /// <summary>
    /// 相机工作状态
    /// </summary>
    public enum MV_CAM_Camera_Work_State_Enum
    {
        /// <summary>
        /// 相机准备中
        /// </summary>
        [Description("相机准备中")]
        Camera_Ready,
        /// <summary>
        /// 相机获取图像中
        /// </summary>
        [Description("相机获取图像中")]
        Camera_GetImage,
        /// <summary>
        /// 相机错误
        /// </summary>
        [Description("相机错误")]
        Camera_Error,


    }




    /// <summary>
    /// 相机标定状态
    /// </summary>
    public enum Camera_Calibration_Mobile_Type_Enum
    {
        /// <summary>
        /// 不用校准
        /// </summary>
        [Description("未标定")]
        UnCalibration,
        /// <summary>
        /// 开始校准
        /// </summary>
        [Description("正在标定")]
        Start_Calibration,
        /// <summary>
        /// 已经标定完成
        /// </summary>
        [Description("已经标定")]
        Calibrated_OK

    }


    /// <summary>
    /// 标定选择相机主副关系
    /// </summary>
    public enum Camera_Calibration_MainOrSubroutine_Type_Enum
    {
        /// <summary>
        /// 不标定时为空
        /// </summary>
        None = -1,
        /// <summary>
        /// 标定主相机
        /// </summary>
        Main,
        /// <summary>
        /// 标定副相机
        /// </summary>
        Subroutine



    }
    /// <summary>
    /// 标定结果状态
    /// </summary>
    public enum Camera_Calibration_Results_Type_Enum
    {

        None = 0,
        标定图像已加载,
        标定图像识别测试成功,
        标定图像识别失败,
        标定计算成功,
        标定计算失败,
        标定模型生成成功,
        标定模型生成失败,

    }


    /// <summary>
    /// 相机标定文件类别枚举
    /// </summary>
    public enum Camera_Calibration_File_Type_Enum
    {
        无标定,
        内参标定,
        双目标定,
    }



    /// <summary>
    /// 相机灯光控制
    /// </summary>
    public enum Camera_Lighting_Control_Type_Enum
    {
        开,
        关,
        自动
    }



    /// <summary>
    /// 触发极性
    ///指定触发器的激活模式
    /// </summary>
    public enum MV_CAM_TRIGGER_ACTIVATION
    {
        /// <summary>
        /// 上升沿
        /// </summary>
        RisingEdge,
        /// <summary>
        /// 下降沿
        /// </summary>
        FallingEdge,
        /// <summary>
        /// 高电平
        /// </summary>
        LevelHigh,

        /// <summary>
        /// 低电平
        /// </summary>
        LevelLow,

        /// <summary>
        /// 上升或下降沿
        /// </summary>
        AnyEdge,

    }




}
