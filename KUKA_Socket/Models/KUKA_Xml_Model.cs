using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KUKA_Socket.Models
{
    public  class KUKA_Xml_Model
    {




    }



    [Serializable]
    [XmlType("KUKA_Receive")]
    public class Calibration_Data_Receive
    {

        public Calibration_Models Vision_Model { set; get; } = new Calibration_Models() { };


        public Vision_Model_Enum Model { set; get; } = Vision_Model_Enum.Find_Model;

    }


    [Serializable]
    [XmlType("KUKA_Send")]
    public class Calibration_Data_Send
    {




        public string Messer_Error { set; get; }
        [XmlAttribute]
        public string IsStatus { set; get; }


        public Calibration_Point_Models Vision_Point { set; get; } = new Calibration_Point_Models();
    }







    [Serializable]
    public class Calibration_Models
    {
        [XmlAttribute]
        public Calibration_Area_Enum Vision_Area { set; get; }
        [XmlAttribute]
        public string Calibration_Number { set; get; }

        [XmlAttribute]
        public string Work_Area { set; get; }

        [XmlAttribute("ID")]
        public string Find_ID { set; get; }






        public Calibration_Point_Models Vision_Point { set; get; } = new Calibration_Point_Models();

        public Camera_Point_Models Camera_Point { set; get; }
    }


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
    [Serializable]
    public class Camera_Point_Models
    {

        public Point_Models Pos_1 { set; get; }
        public Point_Models Pos_2 { set; get; }
    }

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



    public enum Calibration_Area_Enum
    {
        F_45,
        F_135,
        F_225,
        F_315
    }



    public enum Vision_Model_Enum
    {
        Find_Model,
        Calibration_Point
    }


}
