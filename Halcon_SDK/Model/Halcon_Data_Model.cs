using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcon_SDK_DLL.Model
{
    public class Halcon_Data_Model
    {
        public Halcon_Data_Model()
        {

        }



        /// <summary>
        /// Halcon窗口名称
        /// </summary>
        public enum Halcon_Window_Name
        {
            Live_Window,
            Features_Window,
            Results_Window_1,
            Results_Window_2,
            Results_Window_3,
            Results_Window_4
        }




        public class HImage_Display_Model
        {
            /// <summary>
            /// 海康威视图像信息
            /// </summary>
            public HObject Image { set; get; } = new HObject();

            /// <summary>
            /// 图像显示位置Halcon控件
            /// </summary>
            public HWindow Image_Show_Halcon = null;







        }



        /// <summary>
        /// 查找模型结果数据类型
        /// </summary>
        public class Find_Planar_Out_Model
        {
            public double row { set; get; } = 0;
            public double column { set; get; } = 0;
            public double angle { set; get; } = 0;
            public double score { set; get; } = 0;


        }
    }
}
