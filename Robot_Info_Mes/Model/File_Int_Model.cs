using Roboto_Socket_Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Robot_Info_Mes.Model
{


    [Serializable]
    public   class File_Int_Model
    {


        public Window_Startup_Type_Enum Window_Startup_Type { set; get; } = Window_Startup_Type_Enum.Client;


        public Mes_Run_Parameters_Model Mes_Run_Parameters { set; get; } = new();





    }



    public enum Window_Startup_Type_Enum
    {
        /// <summary>
        /// 看板页面
        /// </summary>
        Server,
        /// <summary>
        /// 机器人下位机
        /// </summary>
        Client


    }




    public class File_Xml_Model
    {

    }



}
