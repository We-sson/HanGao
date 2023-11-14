using Generic_Extension;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using PropertyChanged;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
            LineInverter

        }

        /// <summary>
        /// 相机参数类型
        /// </summary>
        [Serializable]
        public class MVS_Camera_Parameter_Model
        {
            public MVS_Camera_Parameter_Model(MVS_Camera_Parameter_Model _Param)
            {
                AcquisitionMode = _Param.AcquisitionMode;
                AcquisitionFrameRateEnable = _Param.AcquisitionFrameRateEnable;
                ExposureTime = _Param. ExposureTime;
                ExposureAuto = _Param.ExposureAuto;
                ExposureMode = _Param.ExposureMode;
                TriggerMode = _Param.TriggerMode;
                TriggerActivation = _Param.TriggerActivation;
                TriggerDelay = _Param.TriggerDelay;
                GainAuto = _Param. GainAuto;
                Gain = _Param.Gain;
                DigitalShiftEnable = _Param.DigitalShiftEnable;
                DigitalShift = _Param.DigitalShift;
                BlackLevelEnable = _Param.BlackLevelEnable;
                BlackLevel = _Param.BlackLevel;
                GammaEnable = _Param.GammaEnable;
                Gamma = _Param.Gamma;
                ReverseX = _Param.ReverseX;
                OffsetX = _Param.OffsetX;
                OffsetY = _Param.OffsetY;
                LineSelector = _Param.LineSelector;
                LineMode = _Param.LineMode;
                LineInverter = _Param.LineInverter;
                StrobeEnable = _Param.StrobeEnable;
                WidthMax = _Param.WidthMax;
                HeightMax = _Param.HeightMax;
                ResultingFrameRate = _Param.ResultingFrameRate;
            }

            public MVS_Camera_Parameter_Model()
            {

            }

            /// <summary>
            /// 设备采集的采集模式、枚举类型值 ——默认持续采集模式，"MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS"
            /// </summary>
            [StringValue("设置相机触发模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_ACQUISITION_MODE AcquisitionMode { set; get; } = MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS;
            /// <summary>
            /// 每个帧突发开始触发信号采集的帧数、整数类型——默认1，最大1023
            /// </summary>
            [StringValue("设置每个帧突发开始触发信号采集的帧数失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool AcquisitionFrameRateEnable { set; get; } = true;
            /// <summary>
            /// 曝光模式定时时的曝光时间
            /// </summary>
            [StringValue("设置曝光模式定时时的曝光时间失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double ExposureTime { set; get; } = 30000;
            /// <summary>
            /// 设置定时曝光模式时的自动曝光模式，枚举类型——默认连续模式，"MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF"
            /// </summary>
            [StringValue("设置控制抓取帧的采集频率失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_EXPOSURE_AUTO_MODE ExposureAuto { set; get; } = MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF;
            /// <summary>
            /// 设置曝光（或快门）的工作模式,枚举类型——默认定时模式，"MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED"
            /// </summary>
            [StringValue("设置曝光（或快门）的工作模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_EXPOSURE_MODE ExposureMode { set; get; } = MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED;
            /// <summary>
            /// 控制所选触发器是否处于活动状态、枚举类型——默认Off，"MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF"
            /// </summary>
            [StringValue("设置控制所选触发器是否处于活动状态失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_TRIGGER_MODE TriggerMode { set; get; } = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;
            /// <summary>
            /// 控制所选触发器是否处于活动状态，枚举类型——默认，"MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0"
            /// </summary>
            [StringValue("设置控制所选触发器是否处于活动状态")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_TRIGGER_SOURCE TriggerActivation { set; get; } = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE1;
            /// <summary>
            /// 指定在激活触发接收之前要应用的延迟（以us为单位）
            /// </summary>
            [StringValue("设置指定在激活触发接收之前要应用的延迟失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double TriggerDelay { set; get; } = 0.00;
            /// <summary>
            /// 设置自动增益控制（AGC）模式，枚举类型——默认，"MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF"
            /// </summary>
            [StringValue("设置自动增益控制（AGC）模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_GAIN_MODE GainAuto { set; get; } = MV_CAM_GAIN_MODE.MV_GAIN_MODE_ONCE;
            /// <summary>
            /// 应用于图像的增益，单位为dB，Float类型，默认0.00
            /// </summary>
            [StringValue("设置图像的增益失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double Gain { set; get; } = 10.00;
            /// <summary>
            /// 使能/禁用数字移位调节，布尔类型——默认false
            /// </summary>
            [StringValue("设置使能/禁用数字移位调节失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool DigitalShiftEnable { set; get; } = true;
            /// <summary>
            /// 设置选定的数字移位控制，Float类型——默认0
            /// </summary>
            [StringValue("设置数字移位控制失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double DigitalShift { set; get; } = 0.00;
            /// <summary>
            /// 使能/禁用黑电平调整，布尔类型——默认true；
            /// </summary>
            [StringValue("设置使能/禁用黑电平调整失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool BlackLevelEnable { set; get; } = true;
            /// <summary>
            /// 模拟黑电平百分比，整数类型——默认200，最小0，最大4095
            /// </summary>
            [StringValue("设置模拟黑电平百分比失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public int BlackLevel { set; get; } = 100;
            /// <summary>
            /// 使能/禁用伽马校正，布尔类型——默认true；
            /// </summary>
            [StringValue("设置使能/禁用伽马校正失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool GammaEnable { set; get; } = true;
            /// <summary>
            /// 控制像素强度的伽马校正，双精度类型——默认0.5，最小0，最大4
            /// </summary>
            [StringValue("设置控制像素强度的伽马校正失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public double Gamma { set; get; } = 0.5;
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
            [StringValue("设置水平翻转设备发送的图像失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool ReverseX { set; get; } = false;

            //[StringValue("设置设备提供的图像宽度（像素）失败")]
            //public int Width { set; get; } = 3072;

            //[StringValue("设置设备提供的图像的高度（像素）失败")]
            //public int Height { set; get; } = 2048;

            [StringValue("设置从原点到AOI的垂直偏移（像素）失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public int OffsetX { set; get; } = 0;

            [StringValue("设置从原点到AOI的水平偏移（像素）失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public int OffsetY { set; get; } = 0;

            [StringValue("设置选择要配置的外部设备连接器的物理线（或管脚）失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_LINESELECTOR_MODE LineSelector { set; get; } = MV_CAM_LINESELECTOR_MODE.Lin1;

            [StringValue("设置线路模式失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public MV_CAM_LINEMODE_MODE LineMode { set; get; } = MV_CAM_LINEMODE_MODE.Strobe;

            [StringValue("设置控制所选输入或输出线的信号反转失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool LineInverter { set; get; } = false;


            [StringValue("设置使能输出信号输出到所选线路失败")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Write)]
            public bool StrobeEnable { set; get; } = true;


            /// <summary>
            /// 图像的最大宽度
            /// </summary>
            [StringValue("获得图像的最大宽度（以像素为单位）~失败!")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Read)]
            public int WidthMax { set; get; } = 0;
            /// <summary>
            /// 图像的最大高度
            /// </summary>
            [StringValue("获得图像的最大高度（以像素为单位）~失败!")]
            [Camera_ReadWrite(Camera_Parameter_RW_Type.Read)]

            public int HeightMax { set; get; } = 0;
            /// <summary>
            /// 最大采集帧速率
            /// </summary>
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
            /// </summary>
            public double Calibrated_Accuracy { set; get; } = 0.00;



            /// <summary>
            /// 标定结状态
            /// </summary>
            public Camera_Calibration_File_Type_Enum Camera_Calibration_State { set; get; } = Camera_Calibration_File_Type_Enum.无;


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
            /// 相机设备当前状态
            /// </summary>
            public MV_CAM_Device_Status_Enum Camer_Status { set; get; } = MV_CAM_Device_Status_Enum.Null;

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
            /// 整数IP地址转换字符串
            /// </summary>
            /// <param name="_Ipaddress"></param>
            /// <returns></returns>
            private string IP_intTOString(uint _Ipaddress)
            {

                string _IPString = ((_Ipaddress & 0xFF000000) >> 24).ToString() + "." + ((_Ipaddress & 0x00FF0000) >> 16).ToString() + "." + ((_Ipaddress & 0x0000FF00) >> 8).ToString() + "." + ((_Ipaddress & 0x000000FF)).ToString();


                return _IPString;

            }


            ///// <summary>
            ///// 相机对应内参
            ///// </summary>
            //public HCamPar HCamera_Param { set; get; } = new HCamPar();


            public  bool Get_HCamPar_File()
            {
                try
                {

                    string _File = Directory.GetCurrentDirectory() + "\\Calibration_File\\" + Camera_Info.SerialNumber + ".dat";

                    if (File.Exists(_File))
                    {

                       
                        HCamPar _CamP=  new HCamPar();
                        _CamP.ReadCamPar(_File);
                       
                        Camera_Calibration.Camera_Calibration_Paramteters = new Halcon_Camera_Calibration_Parameters_Model(_CamP);
                        Camera_Calibration.Camera_Calibration_State = Camera_Calibration_File_Type_Enum.内参标定;


                        return true;

                    }



                    //throw new Exception(Camera_Info.SerialNumber + "：相机没有内参信息，请把内参文件存放在："+_File);
                    return false;


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
    /// 相机标定状态
    /// </summary>
    public enum Camera_Calibration_Mobile_Type_Enum
    {
        /// <summary>
        /// 不用校准
        /// </summary>
        UnCalibration,
        /// <summary>
        /// 开始校准
        /// </summary>
        Start_Calibration
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
        无,
        内参标定,
        双目标定,
    }

}
