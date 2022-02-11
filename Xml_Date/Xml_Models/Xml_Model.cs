using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HanGao.Model.Sink_Models;

namespace HanGao.Xml_Date.Xml_Models
{
    [Serializable]
    public  class Xml_Model
    {

        public DateTime Date_Last_Modify  { get; set; }

        public int Sink_List_number { get; set; }

        public Xml_Sink_Model Sink { get; set; }



    }
    [Serializable]
    public class Xml_Sink_Model
    {
        public double Sink_Model { get; set; }
        public double Sink_Size_Long { get; set; }
        public double Sink_Size_Width { get; set; }
        public double Sink_Size_R { get; set; }
        public double Sink_Size_Pots_Thick { get; set; }
        public double Sink_Size_Panel_Thick { get; set; }
        public string  Sink_Type { get; set; }

    }

}
