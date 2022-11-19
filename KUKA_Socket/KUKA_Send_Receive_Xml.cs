using System;
using System.ComponentModel;
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





            //Property_Xml(KUKA_Receive_Text);
            //Property_Xml(KUKA_Send_Text);

            //string Str = "<KUKA_Receive Model=\"Find_Model\"><Vision_Model ID=\"\" Work_Area=\"\" Vision_Area=\"F_45\" Calibration_Number=\"1\"><Vision_Point><Pos_1 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_1><Pos_2 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_2><Pos_3 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_3><Pos_4 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_4><Pos_5 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_5><Pos_6 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_6><Pos_7 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_7><Pos_8 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_8><Pos_9 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_9></Vision_Point><Camera_Point><Pos_1 X=\"1765.000000\" Y=\"0.000000\" Z=\"1784.000000\" A=\"0.000000\" B=\"90.000000\" C=\"0.000000\"></Pos_1><Pos_2 X=\"\" Y=\"\" Z=\"\" A=\"\" B=\"\" C=\"\"></Pos_2></Camera_Point></Vision_Model></KUKA_Receive>";


            //Calibration_Data_Receive aa = String_Xml<Calibration_Data_Receive>(Str);
        }


        //Calibration_Data_Receive KUKA_Receive_Text = new Calibration_Data_Receive()
        //{
        //    Model = Vision_Model_Enum.Find_Model,
        //    Vision_Model = new Calibration_Models()
        //    {
        //        Vision_Area = Calibration_Area_Enum.F_45,
        //        Calibration_Number = 1,
        //        Vision_Point = new Calibration_Point_Models()
        //        {
        //            Pos_1 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_2 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_3 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_4 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_5 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_6 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_7 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_8 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //            Pos_9 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, },
        //        },
        //        Camera_Point = new Camera_Point_Models()
        //        {
        //            Pos_1 = new Point_Models() { X = 100.123, Y = 100.123, Z = 100.123, A = 100.123, B = 100.123, C = 100.123, }
        //        }
        //    },

        //};


        //Calibration_Data_Send KUKA_Send_Text = new Calibration_Data_Send()
        //{
        //    IsStatus = "1",
        //    Messer_Error = "Find time timeout",
        //    Vision_Point = new Calibration_Point_Models()

        //};


        public static  string Property_Xml<T1>(T1 _Type)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = false;


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




        public static    T1 String_Xml<T1>(string _Path) where T1 : class
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


}
