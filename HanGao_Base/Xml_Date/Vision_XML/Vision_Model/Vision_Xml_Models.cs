using Halcon_SDK_DLL.Halcon_Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.Xml_Date.Vision_XML.Vision_Model
{
    [Serializable]
    [XmlRoot("Vision_Data")]
    public  class Vision_Xml_Models
    {


        public  MVS_Camera_Parameter_Model Camera_Parameter_Data { set; get; } = new MVS_Camera_Parameter_Model();


        public Find_Shape_Based_ModelXld Find_Shape_Data { set; get; } = new Find_Shape_Based_ModelXld() { };


        /// <summary>
        /// 预处理流程
        /// </summary>
        public ObservableCollection<Preprocessing_Process_Lsit_Model> Find_Preprocessing_Process_List { set; get; } = new ObservableCollection<Preprocessing_Process_Lsit_Model>();
    


        [XmlAttribute()]
        public string  ID { set; get; } = "0";

        [XmlAttribute("Date_Revise")]
        public string Date_Last_Revise { get; set; } = DateTime.Now.ToString();



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
      public ObservableCollection<Vision_Xml_Models> Vision_List { set; get; } =new ObservableCollection<Vision_Xml_Models>() {  };


    }






    [Serializable]
    public class Vision_Auto_Config_Model
    {
        /// <summary>
        /// 连接库卡协议外的ip
        /// </summary>
        public string Connect_KUKA_IP { set; get; } = "192.168.0.1";

        /// <summary>
        /// 连接库卡协议外的端口
        /// </summary>
        public int Connect_KUKA_Port { set; get; } = 7000;

        /// <summary>
        /// 上位机网络服务器端口
        /// </summary>
        public int Stat_Network_Port { set; get; } = 5000;

        /// <summary>
        /// 查找模型次数,最少1次
        /// </summary>
         public int Find_Run_Number { set; get; } = 1;

        /// <summary>
        /// 查找模型超时毫秒,最少1000毫秒
        /// </summary>
        public int Find_TimeOut_Millisecond { set; get; } = 3000;

        /// <summary>
        /// 查找模型位置允许误差范围
        /// </summary>
        public double Find_Allow_Error { set; get; } = 5.0;

        /// <summary>
        /// 视觉检查有效区域范围值,,超过会偏移相机位置
        /// </summary>
        public int Vision_Scope { set; get; } = 20;


    }






}
