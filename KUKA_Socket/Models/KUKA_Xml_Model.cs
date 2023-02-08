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




        [XmlAttribute()]
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
   
        Calibration_Point,
        Find_Model,
        Vision_Ini_Data,
    }

    /// <summary>
    /// 标定错误消息返回
    /// </summary>
    public enum Calibration_Error_Message_Enum
    {
        Vision_Ini_Data_OK,
        No_Error,
        Find_time_timeout,
        Camera_Connection_Time_Out,
        Error_No_Camera_GetImage,
        Error_No_Read_Shape_Mode_File,
        Error_No_Read_Math2D_File,
        Error_No_Camera_Set_Parameters,
        Error_No_Can_Find_the_model,
        Error_No_Find_ID_Number,
        Error_No_SinkInfo,
        Error_Find_Exceed_Error_Val
    }

}
