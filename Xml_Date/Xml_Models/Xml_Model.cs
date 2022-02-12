using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static HanGao.Model.Sink_Models;

namespace HanGao.Xml_Date.Xml_Models
{
    [Serializable]
    [XmlRoot("Sink_Date")]
    public  class Xml_Model
    {
        [XmlAttribute("Date_Revise")]
        public DateTime Date_Last_Modify  { get; set; }
        [XmlAttribute("Max_SinkLIst")]
        public int Sink_List_number { get; set; }

        public List<Xml_Sink_Model> Sink_List { get; set; }



    }
    [Serializable]
    [xmol]
    public class Xml_Sink_Model
    {
        [XmlAttribute("Sink_Model")]
        public double Sink_Model { get; set; }
        public double Sink_Size_Long { get; set; }
        public double Sink_Size_Width { get; set; }
        public double Sink_Size_R { get; set; }
        public double Sink_Size_Pots_Thick { get; set; }
        public double Sink_Size_Panel_Thick { get; set; }
        [XmlAttribute("Sink_Type")]
        public string  Sink_Type { get; set; }

    }

}
