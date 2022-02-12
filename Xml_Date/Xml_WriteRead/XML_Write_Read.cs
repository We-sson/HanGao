using HanGao.Xml_Date.Xml_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HanGao.Xml_Date.Xml_WriteRead
{
    public  class XML_Write_Read
    {

        public XML_Write_Read()
        {

            ToXmlString();


        }

        public static void ToXmlString()
        {
            var Sink = new Xml_Model
            {

            

                Date_Last_Modify = DateTime.Now,
                Sink = new List<Xml_Sink_Model>()
                {
                


                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 23.8,
                    Sink_Size_Pots_Thick = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Model.Sink_Models.Sink_Type_Enum.LeftRight_One.ToString()
                    },


                     new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 23.8,
                    Sink_Size_Pots_Thick = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Model.Sink_Models.Sink_Type_Enum.LeftRight_One.ToString()
                    },
                    
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 23.8,
                    Sink_Size_Pots_Thick = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Model.Sink_Models.Sink_Type_Enum.LeftRight_One.ToString()
                    }
                },

       
                
                Sink_List_number = 10
                

            };

            var Xml = new XmlSerializer(typeof(Xml_Model));
            using (var XmlContent=new StreamWriter(@"XmlDate.xml"))
            {
                Xml.Serialize(XmlContent, Sink);
                var xmlContent=XmlContent.ToString();
                Console.WriteLine(xmlContent);

            }

        }
    }
}
