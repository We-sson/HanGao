using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.Xml_Date.Vision_XML.Vision_Model
{
    [Serializable]
    [XmlRoot("Vision")]
    public  class Vision_Xml_Models
    {


        public  MVS_Camera_Parameter_Model Camera_Parameter_Data { set; get; } = new MVS_Camera_Parameter_Model();


        public Find_Shape_Based_ModelXld Find_Shape_Data { set; get; } = new Find_Shape_Based_ModelXld() { };

        [XmlAttribute()]
        public string  ID { set; get; } = "0";

        [XmlAttribute("Date_Revise")]
        public string Date_Last_Revise { get; set; }



    }



    /// <summary>
    /// 视觉数据参数文件集合
    /// </summary>
    public class Vision_Data
    {
        [XmlAttribute()]
        public int Stat_Network_Number { set; get; } = 1;

        public ObservableCollection<Vision_Xml_Models> Vision_List { set; get; } =new ObservableCollection<Vision_Xml_Models>() {  };


    }






}
