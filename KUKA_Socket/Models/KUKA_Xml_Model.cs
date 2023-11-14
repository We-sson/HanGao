using System;
using System.Xml.Serialization;

namespace KUKA_Socket.Models
{
    public class KUKA_Xml_Model
    {




    }








    /// <summary>
    /// 标定查找接收协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Receive")]
    public class Vision_Ini_Data_Receive
    {

        [XmlAttribute()]
        public Vision_Model_Enum Model { set; get; }

    }

    /// <summary>
    /// 相机标定发送协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Send")]
    public class Vision_Ini_Data_Send
    {

        public string Message_Error { set; get; }
        [XmlAttribute]
        public int IsStatus { set; get; }


        public Initialization_Data Initialization_Data { set; get; }=new Initialization_Data ();



    }


    /// <summary>
    /// 手眼相机标定发送协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Send")]
    public class KUKA_HandEye_Calibration_Send
    {
        /// <summary>
        /// 标定消息错误
        /// </summary>
        public string Message_Error { set; get; }
        /// <summary>
        /// 标定状态
        /// </summary>
        [XmlAttribute]
        public int IsStatus { set; get; }

    }

    /// <summary>
    /// 手眼相机标定接收协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Receive")]
    public class KUKA_HandEye_Calibration_Receive
    {

        /// <summary>
        /// 接收模式
        /// </summary>
        [XmlAttribute]
        public Vision_Model_Enum Model { set; get; }
        [XmlAttribute]
        public HandEye_Calibration_Type_Enum Calibration_Model { set; get; }

        public Point_Models Actual_Point { set; get; }



    }


    /// <summary>
    /// 标定查找接收协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Receive")]
    public class Calibration_Data_Receive
    {

        public Calibration_Model_Receive Calibration_Model { set; get; }

        public Find_Model_Receive Find_Model { set; get; }

        public Calibration_Point_Models Vision_Point { set; get; }

        public Camera_Point_Models Camera_Point { set; get; }

        [XmlAttribute]
        public Vision_Model_Enum Model { set; get; }

    }

    /// <summary>
    /// 相机标定发送协议格式
    /// </summary>
    [Serializable]
    [XmlType("KUKA_Send")]
    public class Calibration_Data_Send
    {

        public string Message_Error { set; get; }
        [XmlAttribute]
        public int IsStatus { set; get; }


        public Calibration_Point_Models Vision_Point { set; get; } = new Calibration_Point_Models();



    }


    /// <summary>
    /// 相机标定接收协议格式
    /// </summary>
    [Serializable]
    public class Calibration_Model_Receive
    {
        [XmlAttribute]
        public string Vision_Area { set; get; }


        [XmlAttribute]
        public string Work_Area { set; get; }

        [XmlAttribute]
        public string Calibration_Mark { set; get; }

    }
    /// <summary>
    /// 查找模型接收协议格式
    /// </summary>
    [Serializable]
    public class Find_Model_Receive
    {
        [XmlAttribute]
        public string Find_Data { set; get; }
        [XmlAttribute]
        public string Vision_Area { set; get; }
        [XmlAttribute]
        public string Work_Area { set; get; }
    }






    /// <summary>
    /// 标定点数据格式内容
    /// </summary>
    [Serializable]
    public class Calibration_Point_Models
    {

        public Point_Models Pos_1 { set; get; } = new Point_Models();
        public Point_Models Pos_2 { set; get; } = new Point_Models();
        public Point_Models Pos_3 { set; get; } = new Point_Models();
        public Point_Models Pos_4 { set; get; } = new Point_Models();
        public Point_Models Pos_5 { set; get; } = new Point_Models();
        public Point_Models Pos_6 { set; get; } = new Point_Models();
        public Point_Models Pos_7 { set; get; } = new Point_Models();
        public Point_Models Pos_8 { set; get; } = new Point_Models();
        public Point_Models Pos_9 { set; get; } = new Point_Models();


    }


    /// <summary>
    /// 标定相机点格式内容
    /// </summary>
    [Serializable]
    public class Camera_Point_Models
    {

        public Point_Models Pos_1 { set; get; }
        public Point_Models Pos_2 { set; get; }
    }


    /// <summary>
    /// 位置点各个方向格式内容
    /// </summary>
    [Serializable]
    public class Point_Models
    {
        [XmlAttribute]
        public string X { set; get; } = "0";
        [XmlAttribute]
        public string Y { set; get; } = "0";
        [XmlAttribute]
        public string Z { set; get; } = "0";
        [XmlAttribute]
        public string A { set; get; } = "0";
        [XmlAttribute]
        public string B { set; get; } = "0";
        [XmlAttribute]
        public string C { set; get; } = "0";


    }

    /// <summary>
    /// 初始化数据内容格式
    /// </summary>
    [Serializable]
    public class Initialization_Data
    {
        public string Vision_Scope { set; get; }

    }



    /// <summary>
    /// 视觉识别功能
    /// </summary>
    public enum Vision_Model_Enum
    {
   
        Calibration_New,
        Calibration_Text,
        Calibration_Add,
        Find_Model,
        Vision_Ini_Data,
        HandEye_Calib_Date,
    }

    /// <summary>
    /// 通讯机器人协议枚举
    /// </summary>
    public enum Socket_Robot_Protocols_Enum
    {
        KUKA,
        ABB,
        川崎

    }


    /// <summary>
    /// 手眼标定过程状态枚举
    /// </summary>
    public enum HandEye_Calibration_Type_Enum
    {
        Calibration_Start,
        Calibration_Progress,
        Calibration_End


    }


}
