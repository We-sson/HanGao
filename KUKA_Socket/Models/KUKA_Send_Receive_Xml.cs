using System;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KUKA_Socket.Models
{
    public class KUKA_Send_Receive_Xml
    {
        public KUKA_Send_Receive_Xml()
        {





            Property_Xml(KUKA_Receive_Text);
            Property_Xml(KUKA_Send_Text);

            string Str=  "<KUKA_Receive><Calibration_Model Area=\"F_45\" Work_Number=\"1\"><Calibration_Point><Pos_1 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_1><Pos_2 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_2><Pos_3 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_3><Pos_4 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_4><Pos_5 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_5><Pos_6 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_6><Pos_7 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_7><Pos_8 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_8><Pos_9 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_9></Calibration_Point><Camera_Point><Pos X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos></Camera_Point></Calibration_Model></KUKA_Receive>";
 

            Calibration_Data_Receive aa =  String_Xml<Calibration_Data_Receive>(Str);
        }


        Calibration_Data_Receive KUKA_Receive_Text = new Calibration_Data_Receive()
        {
            Calibration_Model = new Calibration_Models() { 
                Area = Calibration_Area_Enum.F_45,
                Work_Number = 1,
                Calibration_Point = new Calibration_Point_Models()
                {
                    Pos_1 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_2 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_3 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_4 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_5 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_6 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_7 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_8 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                    Pos_9 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
                },
                Camera_Point = new Camera_Point_Models()
                {
                    Pos = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, }
                }
            },

        };


        Calibration_Data_Send KUKA_Send_Text = new Calibration_Data_Send()
        {
            IsStatus = 1,
            Messer_Error = "Find time timeout"
        };


        public string Property_Xml<T1>(T1 _Type)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = false ;


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var str = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(str, settings))
            {
                var xmlSerializer = new XmlSerializer(typeof(T1));
                xmlSerializer.Serialize(xmlWriter, _Type, ns);
            }

            string _St = str.ToString();

            return _St;



        }




        public T1  String_Xml<T1>(string  _Path) where T1 : class
        {

    
                T1 obj;


            //XmlDocument Property_Str = new XmlDocument();
            //Property_Str.LoadXml(_Path)

                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(_Path)))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T1));
                        obj = (T1)xmlSerializer.Deserialize(xmlReader);
            
                    }
            
                return obj;

 






        }


    }


    [Serializable]
    [XmlType("KUKA_Receive")]
    public class Calibration_Data_Receive
    {

        public Calibration_Models Calibration_Model { set; get; }


    }


    [Serializable]
    [XmlType("KUKA_Send")]
    public class Calibration_Data_Send
    {

        public string Messer_Error { set; get; }

        public int IsStatus { set; get; }


    }







    [Serializable]
    public class Calibration_Models
    {
        [XmlAttribute]
        public Calibration_Area_Enum Area { set; get; }
        [XmlAttribute]
        public int Work_Number { set; get; }


        public Calibration_Point_Models Calibration_Point { set; get; }

        public Camera_Point_Models Camera_Point { set; get; }
    }


    [Serializable]
    public class Calibration_Point_Models
    {

        public Point_Models Pos_1 { set; get; }
        public Point_Models Pos_2 { set; get; }
        public Point_Models Pos_3 { set; get; }
        public Point_Models Pos_4 { set; get; }
        public Point_Models Pos_5 { set; get; }
        public Point_Models Pos_6 { set; get; }
        public Point_Models Pos_7 { set; get; }
        public Point_Models Pos_8 { set; get; }
        public Point_Models Pos_9 { set; get; }


    }
    [Serializable]
    public class Camera_Point_Models
    {

        public Point_Models Pos { set; get; }

    }

    [Serializable]
    public class Point_Models
    {
        [XmlAttribute]
        public double X { set; get; }
        [XmlAttribute]
        public double Y { set; get; }
        [XmlAttribute]
        public double Z { set; get; }
        [XmlAttribute]
        public double A { set; get; }
        [XmlAttribute]
        public double B { set; get; }
        [XmlAttribute]
        public double C { set; get; }




    }



    public enum Calibration_Area_Enum
    {
        F_45,
        F_135,
        F_225,
        F_315
    }


}
