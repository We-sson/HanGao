using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.Xml_Date.Vision_XML.Vision_Model
{
    public  class Vission_Xml_Models
    {


        public  MVS_Camera_Parameter_Model Camera_Parameter_Data { set; get; } = new MVS_Camera_Parameter_Model();


        public Find_Shape_Based_ModelXld Find_Shape_Data { set; get; } = new Find_Shape_Based_ModelXld() { };

        public int List_Number { set; get; } = 0;
    }
}
