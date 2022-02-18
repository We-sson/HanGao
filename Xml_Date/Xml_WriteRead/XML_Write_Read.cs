using HanGao.Model;
using HanGao.ViewModel;
using HanGao.Xml_Date.Xml_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HanGao.Xml_Date.Xml_WriteRead
{
    public class XML_Write_Read
    {

        public XML_Write_Read()
        {

            XML_To_object();
            //创建模板
            //ToXmlString();

        }
        /// <summary>
        /// 新建空模板序列化
        /// </summary>
        public static void ToXmlString()
        {
            var Sink = new Xml_Model
            {



                Date_Last_Modify = DateTime.Now.ToString(),
                Sink_List = new List<Xml_Sink_Model>()
                {



                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 2.85,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 23.8,
                    Sink_Size_Left_Distance = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Model.Sink_Models.Sink_Type_Enum.LeftRight_One,
                     Surround_Craft=new Xml_SInk_Surround_Craft()
                     {
                          L0_Welding_Craft=new Xml_Surround_Craft_Data()
                          {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                          },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                     }
                    },
                    new Xml_Sink_Model()
                    {
                    Sink_Model = 952154,
                    Sink_Size_Long = 400,
                    Sink_Size_Panel_Thick = 2.85,
                    Sink_Size_Pots_Thick =0.75,
                    Sink_Size_Short_Side = 23,
                    Sink_Size_Down_Distance = 23.8,
                    Sink_Size_Left_Distance = 23.8,
                    Sink_Size_R = 10,
                    Sink_Size_Width = 454,
                    Sink_Type = Model.Sink_Models.Sink_Type_Enum.LeftRight_One,
                     Surround_Craft=new Xml_SInk_Surround_Craft()
                     {
                          L0_Welding_Craft=new Xml_Surround_Craft_Data()
                          {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                          },
                           C45_Welding_Craft=new Xml_Surround_Craft_Data()
                           {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                           },
                            L90_Welding_Craft=new Xml_Surround_Craft_Data()
                            {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                            },
                             C135_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,

                             },
                             L180_Welding_Craft=new Xml_Surround_Craft_Data()
                             {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                             },
                              C225_Welding_Craft=new Xml_Surround_Craft_Data()
                              {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                              },
                               L270_Welding_Craft=new Xml_Surround_Craft_Data()
                               {
                               Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.LIN,
                               MaxArray = 10,
                               },
                                C315_Welding_Craft=new Xml_Surround_Craft_Data()
                                {
                                 Write_Mode = true,
                               Distance_Type = Distance_Type_Enum.CIR,
                               MaxArray = 3,
                                }
                     }
                    }

                },



                Sink_List_number = 2


            };



            var Xml = new XmlSerializer(typeof(Xml_Model));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            if (!Directory.Exists(@"Date"))
                Directory.CreateDirectory(@"Date");
            using (var XmlContent = new StreamWriter(@"Date\XmlDate.xml"))
            {
                Xml.Serialize(XmlContent, Sink, ns);
                var xmlContent = XmlContent.ToString();
                

            }

        }

        /// <summary>
        /// 水槽总数据
        /// </summary>
        public static Xml_Model Sink_Date { set; get; }



        /// <summary>
        /// 保存修改后的水槽尺寸
        /// </summary>
        /// <param name="sink"></param>
        public static void Write_Xml(Sink_Models sink)
        {

            foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
            {
                if (item.Sink_Model==sink.Sink_Model)
                {
                    item.Sink_Type=sink.Sink_Type;
                    item.Sink_Size_Long = sink.Sink_Process.Sink_Size_Long;
                    item.Sink_Size_Width = sink.Sink_Process.Sink_Size_Width;
                    item.Sink_Size_R = sink.Sink_Process.Sink_Size_R;
                    item.Sink_Size_Pots_Thick = sink.Sink_Process.Sink_Size_Pots_Thick;
                    item.Sink_Size_Panel_Thick = sink.Sink_Process.Sink_Size_Panel_Thick;
                    item.Sink_Size_Down_Distance = sink.Sink_Process.Sink_Size_Down_Distance;
                    item.Sink_Size_Left_Distance = sink.Sink_Process.Sink_Size_Left_Distance;


                }



            }



            Sink_Date.Date_Last_Modify = DateTime.Now.ToString();

            var Xml = new XmlSerializer(typeof(Xml_Model));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            if (!Directory.Exists(@"Date"))
                Directory.CreateDirectory(@"Date");
            using var XmlContent = new StreamWriter(@"Date\XmlDate.xml");
            Xml.Serialize(XmlContent, Sink_Date, ns);
            var xmlContent = XmlContent.ToString();



        }




        /// <summary>
        /// 读取文件内容方序列化
        /// </summary>
        public static void XML_To_object()
        {
            var xmlSerializer = new XmlSerializer(typeof(Xml_Model));
            using var reader = new StreamReader(@"Date\XmlDate.xml");
            Sink_Date = (Xml_Model)xmlSerializer.Deserialize(reader);

            foreach (var item in XML_Write_Read.Sink_Date.Sink_List)
            {
                List_Show.SinkModels.Add(new Sink_Models(item.Sink_Type)
                {
                    Sink_Model = item.Sink_Model,
                    Sink_Type = item.Sink_Type,
                    Sink_Process = new Sink_Size_Models()
                    {
                        Sink_Size_Long = item.Sink_Size_Long,
                        Sink_Size_Width = item.Sink_Size_Width,
                        Sink_Size_R = item.Sink_Size_R,
                        Sink_Size_Pots_Thick = item.Sink_Size_Pots_Thick,
                        Sink_Size_Down_Distance = item.Sink_Size_Down_Distance,
                        Sink_Size_Left_Distance = item.Sink_Size_Left_Distance,
                        Sink_Size_Panel_Thick = item.Sink_Size_Panel_Thick,
                        Sink_Size_Short_Side = item.Sink_Size_Short_Side,
                    }
                });
            }
        }
    }
}
