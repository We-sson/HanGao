using Halcon_SDK_DLL.Halcon_Method;
using Roboto_Socket_Library.Model;
using System.Xml.Serialization;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.Xml_Date.Vision_XML.Vision_Model
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    [XmlRoot("Vision_Data")]
    public class Vision_Xml_Models
    {


        /// <summary>
        /// 相机设备2D3D切换类型
        /// </summary>
        public bool Camera_Devices_2D3D_Switch { set; get; } = true;



        public MVS_Camera_Parameter_Model Camera_Parameter_Data { set; get; } = new MVS_Camera_Parameter_Model();



        public MVS_Camera_Parameter_Model Camera_0_3DPoint_Parameter { set; get; } = new MVS_Camera_Parameter_Model() { TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON, TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE };
        public MVS_Camera_Parameter_Model Camera_1_3DPoint_Parameter { set; get; } = new MVS_Camera_Parameter_Model() { TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON, TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0, TriggerActivation = MVS_SDK_Base.Model.MV_CAM_TRIGGER_ACTIVATION.LevelHigh };


        public MVS_Camera_Parameter_Model Camera_0_3DFusionImage_Parameter { set; get; } = new MVS_Camera_Parameter_Model() { TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON, TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE };
        public MVS_Camera_Parameter_Model Camera_1_3DFusionImage_Parameter { set; get; } = new MVS_Camera_Parameter_Model() { TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON, TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0, TriggerActivation = MVS_SDK_Base.Model.MV_CAM_TRIGGER_ACTIVATION.LevelHigh };


        public H3DStereo_ParamData_Model H3DStereo_ParamData { set; get; } = new H3DStereo_ParamData_Model();


        public Find_Shape_Based_ModelXld Find_Shape_Data { set; get; } = new Find_Shape_Based_ModelXld() { };


        /// <summary>
        /// 预处理流程
        /// </summary>
        public ObservableCollection<Preprocessing_Process_Lsit_Model> Find_Preprocessing_Process_List { set; get; } = [];


        public ObservableCollection<Preprocessing_Process_Lsit_Model> Camera_0_3DPoint_Process_List { set; get; } = [];
        public ObservableCollection<Preprocessing_Process_Lsit_Model> Camera_1_3DPoint_Process_List { set; get; } = [];



        public ObservableCollection<Preprocessing_Process_Lsit_Model> Camera_0_3DFusionImage_Process_List { set; get; } = [];
        public ObservableCollection<Preprocessing_Process_Lsit_Model> Camera_1_3DFusionImage_Process_List { set; get; } = [];


        public ObservableCollection<Preprocessing_Process_Lsit_Model> Camera_3DModel_Process_List { set; get; } = [];




        [XmlAttribute()]
        public string ID { set; get; } = "0";

        [XmlAttribute("Date_Revise")]
        public string Date_Last_Revise { get; set; } = DateTime.Now.ToString();


        public ObservableCollection<Preprocessing_Process_Lsit_Model> Get_H3DStereo_Preprocessing_Process()
        {

            var cameraProcessList = H3DStereo_ParamData.Stereo_Preprocessing_CameraSwitch ? (H3DStereo_ParamData.H3DStereo_Image_Type == H3DStereo_Image_Type_Enum.点云图像
                  ? Camera_0_3DPoint_Process_List
                  : Camera_0_3DFusionImage_Process_List)
              : (H3DStereo_ParamData.H3DStereo_Image_Type == H3DStereo_Image_Type_Enum.点云图像
                  ? Camera_1_3DPoint_Process_List
                  : Camera_1_3DFusionImage_Process_List);

            return cameraProcessList ?? Find_Preprocessing_Process_List;

        }

    }



    /// <summary>
    /// 视觉数据参数文件集合
    /// </summary>
    [Serializable]

    public class Vision_Data
    {
        /// <summary>
        /// 视觉自动模式参数设置
        /// </summary>
        //public Vision_Auto_Config_Model Vision_Auto_Config { set; get; }=new Vision_Auto_Config_Model ();

        /// <summary>
        /// 相机设置参数列表
        /// </summary>
        public ObservableCollection<Vision_Xml_Models> Vision_List { set; get; } = [];


    }

    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Vision_Global_Parameters_Model
    {

        /// <summary>
        /// 查找模型次数,最少1次
        /// </summary>
        public int Vision_Run_Number { set; get; } = 3;

        /// <summary>
        /// 查找模型超时毫秒,最少1000毫秒
        /// </summary>
        public int Find_TimeOut_Millisecond { set; get; } = 3000;

        /// <summary>
        /// 查找模型位置平移允许最大误差范围
        /// </summary>
        public double Vision_Translation_Max_Offset { set; get; } = 20.0;

        /// <summary>
        /// 查找模型位置旋转允许最大误差范围
        /// </summary>
        public double Vision_Rotation_Max_Offset { set; get; } = 5.0;



        /// <summary>
        /// 视觉检查有效区域范围值,,超过会偏移相机位置
        /// </summary>
        public int Vision_Scope { set; get; } = 20;

        /// <summary>
        /// 自动连接相机
        /// </summary>
        public string Auto_Camera_Selected_Name { set; get; }

        /// <summary>
        /// 开启自动连接相机
        /// </summary>
        public bool Auto_Connect_Selected_Camera { set; get; } = false;


    }

    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Local_Network_Config_Model
    {


        /// <summary>
        /// 上位机网络服务器端口
        /// </summary>
        public int Local_Network_Port { set; get; } = 5000;
        /// <summary>
        /// 启动自动连接
        /// </summary>
        public bool Local_Network_Auto_Connect { set; get; } = true;

        /// <summary>
        /// 本地连接服务列表IP
        /// </summary>
        public List<string> Local_Network_IP_List { set; get; } = [];


        /// <summary>
        /// 连接库卡协议外的ip
        /// </summary>
        public Socket_Robot_Protocols_Enum Local_Network_Robot_Model { set; get; } = Socket_Robot_Protocols_Enum.KUKA;
    }

    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Robot_SDK_Config_Model
    {
        /// <summary>
        /// 连接库卡协议外的端口
        /// </summary>
        public int Robot_SDK_IP_Port { set; get; } = 7000;

        public string Robot_SDK_IP { set; get; } = "192.168.0.1";

        public bool Robot_SDK_Auto_Connect { set; get; } = true;
    }



    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Vision_Auto_Config_Model
    {








        public Robot_SDK_Config_Model Global_Robot_SDK_Config { set; get; } = new Robot_SDK_Config_Model();



        public Vision_Global_Parameters_Model Vision_Global_Parameters { set; get; } = new Vision_Global_Parameters_Model();


        public Local_Network_Config_Model Local_Network_Config { set; get; } = new Local_Network_Config_Model();

    }






}
