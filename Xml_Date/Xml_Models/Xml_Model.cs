using HanGao.Extension_Method;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using static HanGao.Model.Sink_Models;

namespace HanGao.Xml_Date.Xml_Models
{
    /// <summary>
    /// xml文件头目
    /// </summary>
    [Serializable]
    [XmlRoot("Sink_Date")]
    public class Xml_Model
    {
        [XmlAttribute("Date_Revise")]
        public string  Date_Last_Modify { get; set; }
        [XmlAttribute("Max_SinkLIst")]
        public int Sink_List_number { get; set; }

        /// <summary>
        /// 水槽XML列表
        /// </summary>
        [XmlElement(ElementName = "Sink")]
        public List<Xml_Sink_Model> Sink_List { get; set; }



    }

    /// <summary>
    /// Xml文件，水槽属性类型说明
    /// </summary>
    public class Xml_Sink_Model
    {
        [XmlAttribute]
        public int Sink_Model { get; set; }
        public double Sink_Size_Long { get; set; }
        public double Sink_Size_Width { get; set; }
        public double Sink_Size_R { get; set; }
        public double Sink_Size_Down_Distance { get; set; }
        public double Sink_Size_Left_Distance { get; set; }
        public double Sink_Size_Short_Side { get; set; }
        public double Sink_Size_Pots_Thick { get; set; }
        public double Sink_Size_Panel_Thick { get; set; }
        [XmlAttribute]
        public Sink_Type_Enum Sink_Type { get; set; }

        [XmlElement(ElementName = "Surround_Craft")]
        public Xml_SInk_Surround_Craft Surround_Craft { get; set; } = new Xml_SInk_Surround_Craft() { };

        public string[] Short_Side_Craft { get; set; }
    }

    /// <summary>
    /// Xml文件，围边水槽工艺焊接部位
    /// </summary>
    [Serializable]
    public class Xml_SInk_Surround_Craft
    {

        public Xml_Surround_Craft_Data L0_Welding_Craft { get; set; } 
        public Xml_Surround_Craft_Data C45_Welding_Craft { get; set; }
        public Xml_Surround_Craft_Data L90_Welding_Craft { get; set; } 
        public Xml_Surround_Craft_Data C135_Welding_Craft { get; set; }
        public Xml_Surround_Craft_Data L180_Welding_Craft { get; set; } 
        public Xml_Surround_Craft_Data C225_Welding_Craft { get; set; } 
        public Xml_Surround_Craft_Data L270_Welding_Craft { get; set; } 
        public Xml_Surround_Craft_Data C315_Welding_Craft { get; set; } 
    }


    /// <summary>
    /// 围边焊接方向创建数据
    /// </summary>
    public class Xml_Surround_Craft_Data
    {
        [XmlIgnore]
        public Distance_Type_Enum Distance_Type;
        [XmlIgnore]
        public bool Write_Mode=false;

        [XmlElement]
        public List<Xml_Craft_Date> Craft_Date { get; set; } = new List<Xml_Craft_Date>();

        private int _maxArray;

        [XmlAttribute]
        public int MaxArray
        {
            get { return _maxArray; }
            set
            {
                if (Write_Mode)
                {

                switch (Distance_Type)
                {
                    case Distance_Type_Enum.LIN:
                        for (int i = 1; i < value+1; i++)
                        {
                            Craft_Date.Add(new Xml_Craft_Date() { NO = i, Craft_Type = Craft_Type_Enum.L_LIN_POS });
                        }
                        break;
                    case Distance_Type_Enum.CIR:
                        Craft_Date.Add(new Xml_Craft_Date() { NO = 1, Craft_Type = Craft_Type_Enum.C_LIN_POS });
                        Craft_Date.Add(new Xml_Craft_Date() { NO = 2, Craft_Type = Craft_Type_Enum.C_CIR_POS });
                        Craft_Date.Add(new Xml_Craft_Date() { NO = 3, Craft_Type = Craft_Type_Enum.C_CIR_POS });
                        break;
                }

                }



                _maxArray = value;
            }
        }



    }

    /// <summary>
    /// 围边工艺
    /// </summary>

    public class Xml_Craft_Date
    {
        [XmlAttribute]
        public int NO { get; set; }

        [XmlAttribute]
        public Craft_Type_Enum Craft_Type { get; set; } = Craft_Type_Enum.Null;

        [XmlAttribute]
        public string Welding_Name { get; set; } = "...";
        [XmlAttribute]
        [ReadWriteAttribute( ReadWrite_Enum.Write)]
        public int Welding_Power { get; set; } = 80;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public double Welding_Speed { get; set; } = 0.045;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public double Welding_Angle { get; set; } = 23;
        [XmlAttribute]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public int Welding_CDIS { get; set; } = 10;

        [XmlElement(ElementName = "Welding_Offset")]
        [ReadWriteAttribute(ReadWrite_Enum.Write)]
        public Welding_Pos_Date Welding_Offset { get; set; } = new Welding_Pos_Date() { };

        [XmlElement(ElementName = "Welding_Pos")]
        public Welding_Pos_Date Welding_Pos { get; set; } = new Welding_Pos_Date() { };
    }






    [Serializable]
    public class Welding_Pos_Date
    {
        [XmlAttribute]
        public double X { get; set; } = 0.000;
        [XmlAttribute]
        public double Y { get; set; } = 0.000;
        [XmlAttribute]
        public double Z { get; set; } = 0.000;
        [XmlAttribute]
        public double A { get; set; } = 0.000;
        [XmlAttribute]
        public double B { get; set; } = 0.000;
        [XmlAttribute]
        public double C { get; set; } = 0.000;


    }
    /// <summary>
    /// 工艺点类型
    /// </summary>
    [Flags]
    public enum Craft_Type_Enum
    {
        [StringValue("#L_LIN_POS")]
        L_LIN_POS,
        [StringValue("#C_LIN_POS")]
        C_LIN_POS,
        [StringValue("#C_CIR_POS")]
        C_CIR_POS,
        [StringValue("")]
        Null
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple =false ,Inherited =false )]
    public   class ReadWriteAttribute : Attribute
    {
         public  ReadWrite_Enum ReadWrite_Type;


        public ReadWriteAttribute(ReadWrite_Enum _Enum)
        {
            ReadWrite_Type=_Enum;
        }

        public  ReadWrite_Enum GetReadWriteType()
        {
            return ReadWrite_Type;
        }

    }


    public enum ReadWrite_Enum
    {
        Read,
        Write,
        Null
    }




    public enum Distance_Type_Enum
    {
        LIN,
        CIR
    }

}
