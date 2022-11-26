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


        /// <summary>
        /// Halcon  Xld直线参数属性
        /// </summary>
        public class Line_Contour_Xld_Model
        {


            public List<HObject> HPoint_Group { set; get; } = new List<HObject>();
            public HObject Lin_Xld_Region { set; get; } = new HObject();
            public HObject Xld_Region { set; get; } = new HObject();

            public HTuple RowBegin { set; get; } = new HTuple();
            public HTuple ColBegin { set; get; } = new HTuple();
            public HTuple RowEnd { set; get; } = new HTuple();
            public HTuple ColEnd { set; get; } = new HTuple();
            public HTuple Nr { set; get; } = new HTuple();
            public HTuple Nc { set; get; } = new HTuple();
            public HTuple Dist { set; get; } = new HTuple();

        }

        /// <summary>
        /// Halcon  Xld圆弧参数属性
        /// </summary>
        public class Cir_Contour_Xld_Model
        {
            public List<HObject> HPoint_Group { set; get; } = new List<HObject>();
            public HObject Cir_Xld_Region { set; get; } = new HObject();
            public HObject Xld_Region { set; get; } = new HObject();


            public HTuple Row { set; get; } = new HTuple();
            public HTuple Column { set; get; } = new HTuple();
            public HTuple Radius { set; get; } = new HTuple();
            public HTuple StartPhi { set; get; } = new HTuple();
            public HTuple EndPhi { set; get; } = new HTuple();
            public HTuple PointOrder { set; get; } = new HTuple();

        }


        ///// <summary>
        /////  一般形状匹配创建模型参数
        ///// </summary>
        //public class Halcon_Create_Shape_ModelXld
        //{
        //    /// <summary>
        //    /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        //    /// </summary>
        //    public string NumLevels { set; get; } = "auto";
        //    /// <summary>
        //    /// 图案的最小旋转。默认值：-0.39 建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        //    /// </summary>
        //    public double AngleStart { set; get; } = 0;
        //    /// <summary>
        //    /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39
        //    /// </summary>
        //    public double AngleExtent { set; get; } = 360;
        //    /// <summary>
        //    /// 角度的步长（分辨率）。默认值： “自动”建议值：“自动”, 0.0175, 0.0349, 0.0524, 0.0698, 0.0873
        //    /// </summary>
        //    public string AngleStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 用于生成模型的优化类型和可选方法。默认值： “自动”
        //    /// </summary>
        //    public Optimization_Enum Optimization { set; get; } = Optimization_Enum.auto;
        //    /// <summary>
        //    /// 匹配指标。默认值： “ignore_local_polarity”
        //    /// </summary>
        //    public Metric_Enum Metric { set; get; } = Metric_Enum.ignore_local_polarity;
        //    /// <summary>
        //    /// 搜索图像中对象的最小对比度。默认值：5,建议值：1、2、3、5、7、10、20、30、40
        //    /// </summary>
        //    public int MinContrast { set; get; } = 5;


        //    /// <summary>
        //    /// 模型类型
        //    /// </summary>
        //    public Shape_Model_Type_Enum Model_Type { set; get; }
        //}


        /// <summary>
        /// 形状匹配查找模型结果参数
        /// </summary>
        public class Halcon_Find_Shape_Out_Parameter
        {
            /// <summary>
            /// 模型实例的行坐标
            /// </summary>
            public double Row { set; get; } = 0;
            /// <summary>
            /// 模型实例的列坐标
            /// </summary>
            public double Column { set; get; } = 0;
            /// <summary>
            /// 模型实例的旋转角度
            /// </summary>
            public double Angle { set; get; } = 0;
            /// <summary>
            /// 模型和找到的实例相似值
            /// </summary>
            public double Score { set; get; } = 0;
            /// <summary>
            /// 查找耗时
            /// </summary>
            public int Find_Time { set; get; } = 0;


            /// <summary>
            /// 找到的模型实例的分数
            /// </summary>
            public HTuple HomMat2D { set; get; } = new HTuple();

        }

        ///// <summary>
        ///// 可变形形状匹配查找模型结果参数
        ///// </summary>
        //public class Halcon_Find_Deformable_Out_Parameter
        //{

        //    /// <summary>
        //    /// 模型和找到的实例相似值
        //    /// </summary>
        //    public double Score { set; get; } = 0;
        //    /// <summary>
        //    /// 找到的模型实例的分数
        //    /// </summary>
        //    public HTuple HomMat2D { set; get; } = new HTuple();


        //    /// <summary>
        //    /// 查找耗时
        //    /// </summary>
        //    public int Find_Time { set; get; } = 0;
        //}


        /// <summary>
        /// 创建形状匹配模板总类型参数
        /// </summary>
        public class Create_Shape_Based_ModelXld
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public Shape_Based_Model_Enum Shape_Based_Model { set; get; }


            /// <summary>
            /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
            /// </summary>
            public string NumLevels { set; get; } = "auto";
            /// <summary>
            /// 图案的最小旋转。默认值：-0.39 建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
            /// </summary>
            public double AngleStart { set; get; } = 0;
            /// <summary>
            /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39
            /// </summary>
            public double AngleExtent { set; get; } = 360;
            /// <summary>
            /// 角度的步长（分辨率）。默认值： “自动”建议值：“自动”, 0.0175, 0.0349, 0.0524, 0.0698, 0.0873
            /// </summary>
            public string AngleStep { set; get; } = "auto";
            /// <summary>
            /// 阵列在行方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
            /// </summary>
            public double ScaleRMin { set; get; } = 1.0;
            /// <summary>
            /// 不使用此参数。默认值：[]
            /// </summary>
            public double ScaleRMax { set; get; } = 0;
            /// <summary>
            /// 在行方向上缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
            /// </summary>
            public string ScaleRStep { set; get; } = "auto";
            /// <summary>
            /// 不使用此参数。默认值：[]
            /// </summary>
            public double ScaleCMax { set; get; } = 0;
            /// <summary>
            /// 阵列在列方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
            /// </summary>
            public double ScaleCMin { set; get; } = 1.0;
            /// <summary>
            /// 在列方向上缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
            /// </summary>
            public string ScaleCStep { set; get; } = "auto";
            /// <summary>
            /// 用于生成模型的优化类型和可选方法。默认值： “自动”
            /// </summary>
            public Optimization_Enum Optimization { set; get; } = Optimization_Enum.auto;
            /// <summary>
            /// 匹配指标。默认值： “ignore_local_polarity”
            /// </summary>
            public Metric_Enum Metric { set; get; } = Metric_Enum.ignore_local_polarity;
            /// <summary>
            /// 搜索图像中对象的最小对比度。默认值：5,建议值：1、2、3、5、7、10、20、30、40
            /// </summary>
            public double MinContrast { set; get; } = 5;
            /// <summary>
            /// 泛型参数名称。默认值：[]
            /// </summary>
            public GenParam_Enum GenParamName { set; get; }
            /// <summary>
            /// 泛型参数的值。默认值：[]
            /// </summary>
            public GenParam_Enum GenParamVal { set; get; }


            /// <summary>
            /// 图案的最小比例。 默认值：0.9,建议值：0.5、0.6、0.7、0.8、0.9、1.0
            /// </summary>
            public double ScaleMin { set; get; } = 0;
            /// <summary>
            /// 图案的最大比例。 默认值：1.1,建议值：1.0、1.1、1.2、1.3、1.4、1.5
            /// </summary>
            public double ScaleMax { set; get; } = 0;
            /// <summary>
            /// 缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
            /// </summary>
            public string ScaleStep { set; get; } = "auto";



        }


        public class Find_Shape_Based_ModelXld
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public Shape_Based_Model_Enum Shape_Based_Model { set; get; }


            /// <summary>
            /// 模型的最小旋转。默认值：-0.39,建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
            /// </summary>
            public double AngleStart { set; get; } = 0;
            /// <summary>
            /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39、0.0
            /// </summary>
            public double AngleExtent { set; get; } = 360;
            /// <summary>
            /// 阵列在行方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
            /// </summary>
            public double ScaleRMin { set; get; } = 1.0;
            /// <summary>
            /// 不使用此参数。默认值：[]
            /// </summary>
            public double ScaleRMax { set; get; } = 0;
            /// <summary>
            /// 不使用此参数。默认值：[]
            /// </summary>
            public double ScaleCMax { set; get; } = 0;
            /// <summary>
            /// 阵列在列方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
            /// </summary>
            public double ScaleCMin { set; get; } = 1.0;
            /// <summary>
            /// 要查找的模型实例的最低分数。 默认值：0.5,建议值：0.3、 0.4、 0.5、 0.6、 0.7、 0.8、 0.9、 1.0
            /// </summary>
            public double MinScore { set; get; } = 0.8;
            /// <summary>
            /// 要找到的模型的实例数（对于所有匹配项，为 0）。默认值：1,建议值：0、1、2、3、4、5、10、20
            /// </summary>
            public int NumMatches { set; get; } = 1;
            /// <summary>
            /// 要查找的模型实例的最大重叠。默认值：0.5,建议值：0.0， 0.1， 0.2， 0.3， 0.4， 0.5， 0.6， 0.7， 0.8， 0.9， 1.0
            /// </summary>
            public double MaxOverlap { set; get; } = 0;
            /// <summary>
            /// 匹配中使用的金字塔级别数（如果|，则使用的最低金字塔级别numLevels|= 2）。默认值：0
            /// </summary>
            public int NumLevels { set; get; } = 3;
            /// <summary>
            /// 搜索启发式的“贪婪”（0：安全但慢;1：快但可能会错过匹配）。默认值：0.9
            /// </summary>
            public double Greediness { set; get; } = 0.9;

            /// <summary>
            /// 亚像素精度（如果不等于）“无”.默认值： “least_squares”
            /// </summary>
            public Subpixel_Values_Enum SubPixel { set; get; } = Subpixel_Values_Enum.least_squares;

            /// <summary>
            /// 最大灰度值功能使能
            /// </summary>
            public bool ScaleImageMax_Enable { set; get; } = true;
            /// <summary>
            /// 最大灰度值功能显示
            /// </summary>
            public bool ScaleImageMax_Disp { set; get; } = true;
            /// <summary>
            /// 增强图像功能使能
            /// </summary>
            public bool Emphasize_Enable { set; get; } = true;
            /// <summary>
            /// 增强图像功能显示
            /// </summary>
            public bool Emphasize_Disp { set; get; } = true;
            /// <summary>
            ///  增强图像的低通掩码的宽度。
            /// </summary>
            public double Emphasize_MaskWidth { set; get; } = 30;
            /// <summary>
            ///  增强图像的低通掩码的高度。
            /// </summary>
            public double Emphasize_MaskHeight { set; get; } = 30;
            /// <summary>
            /// 增强图像的对比强调的强度
            /// </summary>
            public double Emphasize_Factor { set; get; } = 15;

            /// <summary>
            /// 矩形掩码的中值滤波器使能
            /// </summary>
            public bool MedianRect_Enable { set; get; } = true;

            /// <summary>
            /// /矩形掩码的中值滤波器显示
            /// </summary>
            public bool MedianRect_Disp { set; get; } = true;

            /// <summary>
            /// 矩形掩码的中值滤波器使能,滤镜掩码的宽度。
            /// </summary>
            public double MedianRect_MaskWidth { set; get; } = 50;
            /// <summary>
            /// 矩形掩码的中值滤波器使能,滤镜掩码的高度。
            /// /// </summary>
            public double MedianRect_MaskHeight { set; get; } = 50;


        }



        ///// <summary>
        /////  一般形状匹配查找模型参数
        ///// </summary>
        //public class Halcon_Find_Shape_ModelXld
        //{

        //    /// <summary>
        //    /// 模型的最小旋转。默认值：-0.39,建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        //    /// </summary>
        //    public double AngleStart { set; get; } = 0;
        //    /// <summary>
        //    /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39、0.0
        //    /// </summary>
        //    public double AngleExtent { set; get; } = 360;
        //    /// <summary>
        //    /// 要查找的模型实例的最低分数。 默认值：0.5,建议值：0.3、 0.4、 0.5、 0.6、 0.7、 0.8、 0.9、 1.0
        //    /// </summary>
        //    public double MinScore { set; get; } = 0.8;
        //    /// <summary>
        //    /// 要找到的模型的实例数（对于所有匹配项，为 0）。默认值：1,建议值：0、1、2、3、4、5、10、20
        //    /// </summary>
        //    public int NumMatches { set; get; } = 1;
        //    /// <summary>
        //    /// 要查找的模型实例的最大重叠。默认值：0.5,建议值：0.0， 0.1， 0.2， 0.3， 0.4， 0.5， 0.6， 0.7， 0.8， 0.9， 1.0
        //    /// </summary>
        //    public double MaxOverlap { set; get; } = 0;
        //    /// <summary>
        //    /// 亚像素精度（如果不等于）“无”.默认值： “least_squares”
        //    /// </summary>
        //    public Subpixel_Values_Enum SubPixel { set; get; } = Subpixel_Values_Enum.least_squares;
        //    /// <summary>
        //    /// 匹配中使用的金字塔级别数（如果|，则使用的最低金字塔级别numLevels|= 2）。默认值：0
        //    /// </summary>
        //    public int NumLevels { set; get; } = 3;
        //    /// <summary>
        //    /// 搜索启发式的“贪婪”（0：安全但慢;1：快但可能会错过匹配）。默认值：0.9
        //    /// </summary>
        //    public double Greediness { set; get; } = 0.9;

        //}

        ///// <summary>
        ///// 可变形形状匹配查找模型参数
        ///// </summary>
        //public class Halcon_Find_Deformable_model
        //{
        //    /// <summary>
        //    /// 模型的最小旋转。默认值：-0.39,建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        //    /// </summary>
        //    public double AngleStart { set; get; } = 0;
        //    /// <summary>
        //    /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39、0.0
        //    /// </summary>
        //    public double AngleExtent { set; get; } = 360;
        //    /// <summary>
        //    /// 阵列在行方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
        //    /// </summary>
        //    public double ScaleRMin { set; get; } = 1.0;
        //    /// <summary>
        //    /// 不使用此参数。默认值：[]
        //    /// </summary>
        //    public double ScaleRMax { set; get; } = 0;
        //    /// <summary>
        //    /// 不使用此参数。默认值：[]
        //    /// </summary>
        //    public double ScaleCMax { set; get; } = 0;
        //    /// <summary>
        //    /// 阵列在列方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
        //    /// </summary>
        //    public double ScaleCMin { set; get; } = 1.0;
        //    /// <summary>
        //    /// 要查找的模型实例的最低分数。 默认值：0.5,建议值：0.3、 0.4、 0.5、 0.6、 0.7、 0.8、 0.9、 1.0
        //    /// </summary>
        //    public double MinScore { set; get; } = 0.8;
        //    /// <summary>
        //    /// 要找到的模型的实例数（对于所有匹配项，为 0）。默认值：1,建议值：0、1、2、3、4、5、10、20
        //    /// </summary>
        //    public int NumMatches { set; get; } = 1;
        //    /// <summary>
        //    /// 要查找的模型实例的最大重叠。默认值：0.5,建议值：0.0， 0.1， 0.2， 0.3， 0.4， 0.5， 0.6， 0.7， 0.8， 0.9， 1.0
        //    /// </summary>
        //    public double MaxOverlap { set; get; } = 0;
        //    /// <summary>
        //    /// 匹配中使用的金字塔级别数（如果|，则使用的最低金字塔级别numLevels|= 2）。默认值：0
        //    /// </summary>
        //    public int NumLevels { set; get; } = 3;
        //    /// <summary>
        //    /// 搜索启发式的“贪婪”（0：安全但慢;1：快但可能会错过匹配）。默认值：0.9
        //    /// </summary>
        //    public double Greediness { set; get; } = 0.9;
        //}



        /// <summary>
        /// 查找九点标定模型参数
        /// </summary>
        public class Halcon_Find_Calibration_Model
        {
            /// <summary>
            ///滤波模式    .三种模式
            /// </summary>
            public int Filtering_Model { set; get; }

            /// <summary>
            /// 滤镜掩码的宽度。默认值：15 值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101   典型值范围：3 ≤ maskWidth ≤ 4095
            /// </summary>
            public double MaskWidth { set; get; }

            /// <summary>
            /// 滤镜掩模的高度。 默认值：15  值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101  典型值范围：3 ≤ maskHeight ≤ 4095
            /// </summary>
            public double MaskHeight { set; get; }
            /// <summary>
            /// 滤镜遮罩类型。默认值： “圆  值列表：“圆”, “正方形”值列表（对于计算设备）：“正方形”
            /// </summary>
            public MedianImage_MaskType_Enum MaskType_Model { set; get; }

            /// <summary>
            /// 滤镜掩码的半径。默认值：1  值列表（用于计算设备）：1、2  建议值：1， 2， 3， 4， 5， 6， 7， 8， 9， 11， 15， 19， 25， 31， 39， 47， 59  典型值范围：1 ≤ radius ≤ 4095
            /// </summary>
            public double Radius { set; get; }
            /// <summary>
            /// 边境处理。默认值： “镜像”值列表（对于计算设备）：“镜像”建议值：“镜像”, “循环”, “续”, 0, 30, 60, 90, 120, 150, 180, 210, 240, 255
            /// </summary>
            public MedianImage_Margin_Enum Margin_Model { set; get; }

            /// <summary>
            /// 滤镜掩码的宽度。默认值：15  值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101  典型值范围：3 ≤ maskWidth ≤ 201
            /// </summary>
            public double Emphasize_MaskWidth { set; get; }
            /// <summary>
            /// 滤镜掩模的高度。 默认值：15  值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101  典型值范围：3 ≤ maskHeight ≤ 201
            /// </summary>
            public double Emphasize_MaskHeight { set; get; }

            /// <summary>
            /// 对比强调的强度。 默认值：1.0建议值：0.3， 0.5， 0.7， 1.0， 1.4， 1.8， 2.0典型值范围：（sqrt）0.0 ≤ factor ≤ 20.0
            /// </summary>
            public double Factor { set; get; }
            /// <summary>
            /// 灰度值的较低阈值或“Min”.默认值：128.0 建议值：0.0， 10.0， 30.0， 64.0， 128.0， 200.0， 220.0， 255.0，“Min”
            /// </summary>
            public double MinGray { set; get; }

            /// <summary>
            /// 灰度值的上限阈值或“Max”.默认值：255.0  建议值：0.0， 10.0， 30.0， 64.0， 128.0， 200.0， 220.0， 255.0，“Max”
            /// </summary>
            public double MaxGray { set; get; }

            /// <summary>
            /// 最小面积
            /// </summary>
            public double Min_Area { set; get; }

            /// <summary>
            /// 最大面积
            /// </summary>
            public double Max_Area { set; get; }

        }

        ///// <summary>
        ///// 可变形形状匹配创建模型参数
        ///// </summary>
        //public class Halcon_Create_Planar_Uncalib_Deformable_ModelXld
        //{
        //    /// <summary>
        //    /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        //    /// </summary>
        //    public string NumLevels { set; get; } = "auto";
        //    /// <summary>
        //    /// 图案的最小旋转。默认值：-0.39 建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        //    /// </summary>
        //    public double AngleStart { set; get; } = 0;
        //    /// <summary>
        //    /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39
        //    /// </summary>
        //    public double AngleExtent { set; get; } = 360;
        //    /// <summary>
        //    /// 角度的步长（分辨率）。默认值： “自动”建议值：“自动”, 0.0175, 0.0349, 0.0524, 0.0698, 0.0873
        //    /// </summary>
        //    public string AngleStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 阵列在行方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
        //    /// </summary>
        //    public double ScaleRMin { set; get; } = 1.0;
        //    /// <summary>
        //    /// 不使用此参数。默认值：[]
        //    /// </summary>
        //    public double ScaleRMax { set; get; } = 0;
        //    /// <summary>
        //    /// 在行方向上缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
        //    /// </summary>
        //    public string ScaleRStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 不使用此参数。默认值：[]
        //    /// </summary>
        //    public double ScaleCMax { set; get; } = 0;
        //    /// <summary>
        //    /// 阵列在列方向上的最小比例。默认值：1.0,建议值：0.5、0.6、0.7、0.8、0.9、1.0
        //    /// </summary>
        //    public double ScaleCMin { set; get; } = 1.0;
        //    /// <summary>
        //    /// 在列方向上缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
        //    /// </summary>
        //    public string ScaleCStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 用于生成模型的优化类型和可选方法。默认值： “自动”
        //    /// </summary>
        //    public Optimization_Enum Optimization { set; get; } = Optimization_Enum.auto;
        //    /// <summary>
        //    /// 匹配指标。默认值： “ignore_local_polarity”
        //    /// </summary>
        //    public Metric_Enum Metric { set; get; } = Metric_Enum.ignore_local_polarity;
        //    /// <summary>
        //    /// 搜索图像中对象的最小对比度。默认值：5,建议值：1、2、3、5、7、10、20、30、40
        //    /// </summary>
        //    public double MinContrast { set; get; } = 5;
        //    /// <summary>
        //    /// 泛型参数名称。默认值：[]
        //    /// </summary>
        //    public GenParam_Enum GenParamName { set; get; }
        //    /// <summary>
        //    /// 泛型参数的值。默认值：[]
        //    /// </summary>
        //    public GenParam_Enum GenParamVal { set; get; }

        //    /// <summary>
        //    /// 模型类型
        //    /// </summary>
        //    public Shape_Model_Type_Enum Model_Type { set; get; }




        //}










        ///// <summary>
        ///// 各向同性缩放的形状模型参数
        ///// </summary>
        //public class Halcon_Create_Scaled_Shape_ModelXld
        //{
        //    /// <summary>
        //    /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        //    /// </summary>
        //    public string NumLevels { set; get; } = "auto";
        //    /// <summary>
        //    /// 图案的最小旋转。默认值：-0.39 建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        //    /// </summary>
        //    public double AngleStart { set; get; } = 0;
        //    /// <summary>
        //    /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39
        //    /// </summary>
        //    public double AngleExtent { set; get; } = 360;
        //    /// <summary>
        //    /// 角度的步长（分辨率）。默认值： “自动”建议值：“自动”, 0.0175, 0.0349, 0.0524, 0.0698, 0.0873
        //    /// </summary>
        //    public string AngleStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 图案的最小比例。 默认值：0.9,建议值：0.5、0.6、0.7、0.8、0.9、1.0
        //    /// </summary>
        //    public double ScaleMin { set; get; } = 0;
        //    /// <summary>
        //    /// 图案的最大比例。 默认值：1.1,建议值：1.0、1.1、1.2、1.3、1.4、1.5
        //    /// </summary>
        //    public double ScaleMax { set; get; } = 0;
        //    /// <summary>
        //    /// 缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
        //    /// </summary>
        //    public string ScaleStep { set; get; } = "auto";
        //    /// <summary>
        //    /// 用于生成模型的优化类型和可选方法。默认值： “自动”
        //    /// </summary>
        //    public Optimization_Enum Optimization { set; get; } = Optimization_Enum.auto;
        //    /// <summary>
        //    /// 匹配指标。默认值： “ignore_local_polarity”
        //    /// </summary>
        //    public Metric_Enum Metric { set; get; } = Metric_Enum.ignore_local_polarity;
        //    /// <summary>
        //    /// 搜索图像中对象的最小对比度。默认值：5,建议值：1、2、3、5、7、10、20、30、40
        //    /// </summary>
        //    public double MinContrast { set; get; } = 5;


        //    /// <summary>
        //    /// 模型类型
        //    /// </summary>
        //    public Shape_Model_Type_Enum Model_Type { set; get; }

        //}






        /// <summary>
        /// 对于特别大的模型，通过设置来减少模型点的数量可能很有用optimization更改为不同于“无”.如果optimization = “无”，则存储所有模型点。在所有其他情况下，点数根据optimization.如果点数减少，则可能需要FindScaledShapeModel将参数设置为较小的值，例如 0.7 或 0.8。对于小型模型，模型点数量的减少不会导致搜索速度加快，因为在这种情况下，通常必须检查模型的更多潜在实例
        /// </summary>
        public enum Optimization_Enum
        {
            auto,
            no_pregeneration,
            none,
            point_reduction_high,
            point_reduction_low,
            point_reduction_medium,
            pregeneration
        }

        /// <summary>
        /// 参数metric确定在图像中识别模型的条件。
        /// </summary>
        public enum Metric_Enum
        {
            ignore_color_polarity,
            ignore_global_polarity,
            ignore_local_polarity,
            use_polarity
        }


        /// <summary>
        /// 获得泛指名称
        /// </summary>
        public enum GenParam_Enum
        {
            part_size,
            big,
            medium,
            small
        }

        public enum Shape_Model_Type_Enum
        {
            Create_Shape_Model,
            Create_Planar_Model,
            Create_Local_Model,
            Create_Scaled_Model
        }


        /// <summary>
        /// 亚像素精度枚举
        /// </summary>
        public enum Subpixel_Values_Enum
        {
            none,
            interpolation,
            least_squares,
            least_squares_high,
            least_squares_very_high,
        }

        /// <summary>
        /// 查找匹配模型类型
        /// </summary>
        public enum Find_Model_Enum
        {
            Shape_Model,
            Planar_Deformable_Model,
            Local_Deformable_Model,
            Scale_Model

        }

        /// <summary>
        /// 中值滤波器滤镜遮罩类型枚举
        /// </summary>
        public enum MedianImage_MaskType_Enum
        {
            circle,
            square

        }

        /// <summary>
        /// 中值滤波器边境处理类型枚举。
        /// </summary>
        public enum MedianImage_Margin_Enum
        {
            mirrored,
            cyclic,
            continued
        }




        /// <summary>
        /// 形状匹配模板类型枚举
        /// </summary>
        public enum Shape_Based_Model_Enum
        {
            /// <summary>
            /// 一般形状匹配模板
            /// </summary>
            shape_model,
            /// <summary>
            /// 线性变形匹配模板
            /// </summary>
            planar_deformable_model,
            /// <summary>
            /// 局部可变形模板
            /// </summary>
            local_deformable_model,
            /// <summary>
            /// 和比例缩放模板
            /// </summary>
            Scale_model
        }



    }
}
