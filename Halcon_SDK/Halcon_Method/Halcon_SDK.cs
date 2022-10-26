using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Halcon_SDK_DLL
{
    public class Halcon_SDK
    {

        public Halcon_SDK()
        {

        }


        /// <summary>
        /// Halcon窗口句柄
        /// </summary>
        public HWindow HWindow { set; get; } = new HWindow();


        /// <summary>
        /// Halcon控件属性
        /// </summary>
        public HSmartWindowControlWPF Halcon_UserContol { set; get; } = new HSmartWindowControlWPF() { };


        /// <summary>
        /// 海康获取图像指针转换Halcon图像
        /// </summary>
        /// <param name="_Width"></param>
        /// <param name="_Height"></param>
        /// <param name="_pData"></param>
        /// <returns></returns>
        public HImage Mvs_To_Halcon_Image(int _Width, int _Height, IntPtr _pData)
        {
            HImage image = new HImage();
            //转换halcon图像格式
            image.GenImage1("byte", _Width, _Height, _pData);

            return image;
        }

        /// <summary>
        /// 图像文件地址转换Halcon图像
        /// </summary>
        /// <param name="_local"></param>
        /// <returns></returns>
        public static  HObject Local_To_Halcon_Image(string _local)
        {

            //新建空属性
            HOperatorSet.GenEmptyObj(out HObject ho_Image);

            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, _local);

            return ho_Image;


        }



        public  static HTuple List_ConcatObj<T1>(T1 _List)
        {



            return new HTuple();
        }










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


    /// <summary>
    ///  一般形状匹配创建模型参数
    /// </summary>
    public class Halcon_Create_Shape_ModelXld
    {
        /// <summary>
        /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        /// </summary>
        public string  NumLevels { set; get; } = "auto";
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
        public string  AngleStep { set; get; } = "auto";
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
        public int MinContrast { set; get; } = 5;


        /// <summary>
        /// 模型类型
        /// </summary>
        public Shape_Model_Type_Enum Model_Type { set; get; }
    }

    /// <summary>
    /// 可变形形状匹配创建模型参数
    /// </summary>
    public class Halcon_Create_Planar_Uncalib_Deformable_ModelXld
    {
        /// <summary>
        /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        /// </summary>
        public string  NumLevels { set; get; } = "auto";
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
        public string  AngleStep { set; get; } = "auto";
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
        public string  ScaleRStep { set; get; } = "auto";
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
        public string  ScaleCStep { set; get; } = "auto";
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
        /// 模型类型
        /// </summary>
        public Shape_Model_Type_Enum Model_Type { set; get; }


    }

    /// <summary>
    /// 各向同性缩放的形状模型参数
    /// </summary>
    public class Halcon_Create_Scaled_Shape_ModelXld
    {
        /// <summary>
        /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
        /// </summary>
        public string  NumLevels { set; get; } = "auto";
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
        public string  AngleStep { set; get; } = "auto";
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
        public string  ScaleStep { set; get; } = "auto";
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
        /// 模型类型
        /// </summary>
        public Shape_Model_Type_Enum Model_Type { set; get; }

    }



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

    public  enum Subpixel_Values_Enum
    {
         none, 
        interpolation, 
        least_squares, 
        least_squares_high, 
        least_squares_very_high, 
    }


}
