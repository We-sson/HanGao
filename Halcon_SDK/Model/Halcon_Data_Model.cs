using HalconDotNet;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Media3D;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

namespace Halcon_SDK_DLL.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Data_Model
    {
        [AddINotifyPropertyChangedInterface]
        /// <summary>
        /// 窗口显示内容模型
        /// </summary>
        public class DisplayHObject_Model
        {
            public HObject Display { set; get; } = new HObject();

            public Window_Show_Name_Enum Show_Window { set; get; } = Window_Show_Name_Enum.Features_Window;

            public Display_HObject_Type_Enum Display_Type { set; get; } = Display_HObject_Type_Enum.Image;

            public DisplayDrawColor_Model SetDisplay { set; get; } = new DisplayDrawColor_Model();
        }



        /// <summary>
        /// 三维模型显示属性模型
        /// </summary>
        public class Display3DModel_Model
        {
            public Display3DModel_Model()
            {

            }
            public Display3DModel_Model(List<HObjectModel3D> objectModel3D)
            {
                _ObjectModel3D = objectModel3D;
                //_CamParam = new HTuple();
                //_PoseIn = new HTuple();
                //_PoseOut = new HTuple();
                //_GenParamName = new HTuple();
                //_GenParamValue = new HTuple();
                //_Title = new HTuple();
                //_Label = new HTuple();
                //_Information = new HTuple();
            }

            public List<HObjectModel3D> _ObjectModel3D { set; get; } = new List<HObjectModel3D>();
            //public HTuple _CamParam { set; get; } = new HTuple();

            public HTuple _PoseIn { set; get; } = new HTuple();
            //public HTuple _PoseOut { set; get; } = new HTuple();
            //public HTuple _GenParamName { set; get; } = new HTuple();
            //public HTuple _GenParamValue { set; get; } = new HTuple();
            //public HTuple _Title { set; get; } = new HTuple();
            //public HTuple _Label { set; get; } = new HTuple();
            //public HTuple _Information { set; get; } = new HTuple();

        }

        /// <summary>
        /// 设置显示窗口颜色
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class DisplayDrawColor_Model
        {
            public DisplaySetDraw_Enum SetDraw { set; get; } = DisplaySetDraw_Enum.fill;


            private string _SetColor = KnownColor.Red.ToString().ToLower();

            public string SetColor
            {
                get { return _SetColor; }
                set { _SetColor = value.ToLower(); }
            }

        }


        /// <summary>
        /// Halcon窗口名称
        /// </summary>
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
        /// 形状匹配查找模型结果参数
        /// </summary>
        public class Find_Shape_Results_Model
        {
            /// <summary>
            /// 模型实例的行坐标
            /// </summary>
            public List<double> Row { set; get; } = new List<double>();
            /// <summary>
            /// 模型实例的列坐标
            /// </summary>
            public List<double> Column { set; get; } = new List<double>();
            /// <summary>
            /// 模型实例的旋转角度
            /// </summary>
            public List<double> Angle { set; get; } = new List<double>();
            /// <summary>
            /// 模型和找到的实例相似值
            /// </summary>
            public List<double> Score { set; get; } = new List<double>();
            /// <summary>
            /// 查找耗时
            /// </summary>
            public double Find_Time { set; get; } = 0;
            /// <summary>
            /// 查找模型结果
            /// </summary>
            public List<bool> FInd_Results { set; get; } = new List<bool>();
            /// <summary>
            /// 存储结果点
            /// </summary>
            public List<Point3D> Vision_Pos { set; get; } = new List<Point3D>();
            /// <summary>
            /// 存储结果点
            /// </summary>
            public List<Point3D> Robot_Pos { set; get; } = new List<Point3D>();
            //public double Right_Angle { set; get; } = 0;
            /// <summary>
            /// 找到的模型实例的分数
            /// </summary>
            public List<HTuple> HomMat2D { set; get; } = new List<HTuple>();
            /// <summary>
            /// 页面显示结过集合
            /// </summary>
            public List<string> Text_Arr_UI { set; get; } = new List<string>() { };
            /// <summary>
            /// 显示结果界面
            /// </summary>
            public HWindow DispWiindow { set; get; } = new HWindow();
        }
        public class Pos_List_Model
        {
            /// <summary>
            /// 存储结果点
            /// </summary>
            public List<Point3D> Vision_Pos { set; get; } = new List<Point3D>();
            /// <summary>
            /// 存储结果点
            /// </summary>
            public List<Point3D> Robot_Pos { set; get; } = new List<Point3D>();
            /// <summary>
            /// 直角角度
            /// </summary>
            public double Right_Angle { set; get; } = 0;
        }
        /// <summary>
        /// 创建形状匹配模板总类型参数
        /// </summary>
        public class Create_Shape_Based_ModelXld
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public Shape_Based_Model_Enum Shape_Based_Model { set; get; } = Shape_Based_Model_Enum.Ncc_Model;
            /// <summary>
            /// UI绑定查找模型区域名字
            /// </summary>
            public ShapeModel_Name_Enum ShapeModel_Name { set; get; } = ShapeModel_Name_Enum.F_45;
            /// <summary>
            /// 模型创建ID号
            /// </summary>
            public int Create_ID { set; get; } = 1;
            /// <summary>
            /// 金字塔层的最大数量默认值：“自动”值列表：1， 2， 3， 4， 5， 6， 7， 8， 9， 10，“自动”
            /// </summary>
            public string NumLevels { set; get; } = "auto";
            /// <summary>
            /// 图案的最小旋转。默认值：-0.39 建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
            /// </summary>
            public double AngleStart { set; get; } = -10;
            /// <summary>
            /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39
            /// </summary>
            public double AngleExtent { set; get; } = 20;
            /// <summary>
            /// 角度的步长（分辨率）。默认值： “自动”建议值：“自动”, 0.0175, 0.0349, 0.0524, 0.0698, 0.0873
            /// </summary>
            public double AngleStep { set; get; } = 0.01;
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
            public double ScaleMin { set; get; } = 0.9;
            /// <summary>
            /// 图案的最大比例。 默认值：1.1,建议值：1.0、1.1、1.2、1.3、1.4、1.5
            /// </summary>
            public double ScaleMax { set; get; } = 1.1;
            /// <summary>
            /// 缩放步长（分辨率）。默认值： “自动”建议值：“自动”, 0.01, 0.02, 0.05, 0.1, 0.15, 0.2
            /// </summary>
            public string ScaleStep { set; get; } = "auto";
            /// <summary>
            /// 用圆形结构元素扩张区域。
            /// </summary>
            public double DilationCircle { set; get; } = 25;
        }
        /// <summary>
        /// 查找形状匹配模板总类型参数
        /// </summary>
        [Serializable]
        public class Find_Shape_Based_ModelXld
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public Shape_Based_Model_Enum Shape_Based_Model { set; get; } = Shape_Based_Model_Enum.Ncc_Model;
            /// <summary>
            /// UI绑定查找模型区域名字
            /// </summary>
            public ShapeModel_Name_Enum ShapeModel_Name { set; get; } = ShapeModel_Name_Enum.F_45;
            /// <summary>
            /// 模型创建ID号
            /// </summary>
            public int FInd_ID { set; get; } = 1;
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
            public double ScaleRMax { set; get; } = 1.0;
            /// <summary>
            /// 不使用此参数。默认值：[]
            /// </summary>
            public double ScaleCMax { set; get; } = 1.0;
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
            /// 最大容许角度畸变 可以限制,作为默认值已设置3.14159/2，为 0，则根本不允许失真。
            /// </summary>
            public double Angle_change_restriction { set; get; } = 1.57;
            /// <summary>
            ///  可以限制各向异性缩放（较小的比例因子除以 更大的比例因子）。 此参数的值范围从默认值 0.0，其中 允许任意失真，到 1.0，其中不允许失真。 
            /// </summary>
            public double Aniso_scale_change_restriction { set; get; } = 0.3;
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
            public bool Illuminate_Enable { set; get; } = true;
            /// <summary>
            /// 增强图像功能显示
            /// </summary>
            public bool Illuminate_Disp { set; get; } = true;
            /// <summary>
            ///  增强图像的低通掩码的宽度。
            /// </summary>
            public int Illuminate_MaskWidth { set; get; } = 101;
            /// <summary>
            ///  增强图像的低通掩码的高度。
            /// </summary>
            public int Illuminate_MaskHeight { set; get; } = 101;
            /// <summary>
            /// 增强图像的对比强调的强度
            /// </summary>
            public double Illuminate_Factor { set; get; } = 0.8;
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
            public int Emphasize_MaskWidth { set; get; } = 15;
            /// <summary>
            ///  增强图像的低通掩码的高度。
            /// </summary>
            public int Emphasize_MaskHeight { set; get; } = 15;
            /// <summary>
            /// 增强图像的对比强调的强度
            /// </summary>
            public double Emphasize_Factor { set; get; } = 5;
            /// <summary>
            /// /计算带有矩形掩码的中值滤波器是使能
            /// </summary>
            public bool Median_image_Enable { set; get; } = true;
            /// <summary>
            /// 计算带有矩形掩码的中值滤波器显示
            /// </summary>
            public bool Median_image_Disp { set; get; } = true;
            /// <summary>
            /// 滤镜遮罩类型。默认值： “圆  值列表：“圆”, “正方形”值列表（对于计算设备）：“正方形”
            /// </summary>
            public MedianImage_MaskType_Enum MaskType_Model { set; get; } = MedianImage_MaskType_Enum.circle;
            /// <summary>
            /// 滤镜掩码的半径。默认值：1  值列表（用于计算设备）：1、2  建议值：1， 2， 3， 4， 5， 6， 7， 8， 9， 11， 15， 19， 25， 31， 39， 47， 59  典型值范围：1 ≤ radius ≤ 4095
            /// </summary>
            public int Median_image_Radius { set; get; } = 5;
            /// <summary>
            /// 边境处理。默认值： “镜像”值列表（对于计算设备）：“镜像”建议值：“镜像”, “循环”, “续”, 0, 30, 60, 90, 120, 150, 180, 210, 240, 255
            /// </summary>
            public MedianImage_Margin_Enum Margin_Model { set; get; } = MedianImage_Margin_Enum.mirrored;
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
            public int MedianRect_MaskWidth { set; get; } = 3;
            /// <summary>
            /// 矩形掩码的中值滤波器使能,滤镜掩码的高度。
            /// /// </summary>
            public int MedianRect_MaskHeight { set; get; } = 3;
            /// <summary>
            /// 灰度开运算使能
            /// </summary>
            public bool GrayOpeningRect_Enable { set; get; } = true;
            /// <summary>
            /// 灰度开运算显示
            /// </summary>
            public bool GrayOpeningRect_Disp { set; get; } = true;
            /// <summary>
            /// 灰度开运算滤镜掩码的高度
            /// </summary>
            public int GrayOpeningRect_MaskHeight { set; get; } = 16;
            /// <summary>
            /// 灰度开运算滤镜掩码的宽度
            /// </summary>
            public int GrayOpeningRect_MaskWidth { set; get; } = 16;
            /// <summary>
            /// 灰度开运算使能
            /// </summary>
            public bool GrayClosingRect_Enable { set; get; } = true;
            /// <summary>
            /// 灰度开运算显示
            /// </summary>
            public bool GrayClosingRect_Disp { set; get; } = true;
            /// <summary>
            /// 灰度开运算滤镜掩码的高度
            /// </summary>
            public int GrayClosingRect_MaskHeight { set; get; } = 16;
            /// <summary>
            /// 灰度开运算滤镜掩码的宽度
            /// </summary>
            public int GrayClosingRect_MaskWidth { set; get; } = 16;
        }
        /// <summary>
        /// 查找九点标定模型参数
        /// </summary>
        public class Halcon_Find_Calibration_Model
        {
            /// <summary>
            /// 用户选择采集图片方式
            /// </summary>
            public Get_Image_Model_Enum Get_Image_Model { set; get; } = Get_Image_Model_Enum.相机采集;
            /// <summary>
            /// /计算带有矩形掩码的中值滤波器是使能
            /// </summary>
            public bool Median_image_Enable { set; get; } = true;
            /// <summary>
            /// 计算带有矩形掩码的中值滤波器显示
            /// </summary>
            public bool Median_image_Disp { set; get; } = true;
            /// <summary>
            /// 滤镜遮罩类型。默认值： “圆  值列表：“圆”, “正方形”值列表（对于计算设备）：“正方形”
            /// </summary>
            public MedianImage_MaskType_Enum MaskType_Model { set; get; } = MedianImage_MaskType_Enum.circle;
            /// <summary>
            /// 滤镜掩码的半径。默认值：1  值列表（用于计算设备）：1、2  建议值：1， 2， 3， 4， 5， 6， 7， 8， 9， 11， 15， 19， 25， 31， 39， 47， 59  典型值范围：1 ≤ radius ≤ 4095
            /// </summary>
            public int Median_image_Radius { set; get; } = 5;
            /// <summary>
            /// 边境处理。默认值： “镜像”值列表（对于计算设备）：“镜像”建议值：“镜像”, “循环”, “续”, 0, 30, 60, 90, 120, 150, 180, 210, 240, 255
            /// </summary>
            public MedianImage_Margin_Enum Margin_Model { set; get; } = MedianImage_Margin_Enum.mirrored;
            /// <summary>
            /// 增强图像的对比度使能
            /// </summary>
            public bool Emphasize_Enable { set; get; } = true;
            /// <summary>
            /// 增强图像的对比度显示参数
            /// </summary>
            public bool Emphasize_Disp { set; get; } = true;
            /// <summary>
            /// 滤镜掩码的宽度。默认值：15  值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101  典型值范围：3 ≤ maskWidth ≤ 201
            /// </summary>
            public double Emphasize_MaskWidth { set; get; } = 50;
            /// <summary>
            /// 滤镜掩模的高度。 默认值：15  值列表（用于计算设备）：3、5  建议值：3， 5， 7， 9， 11， 13， 15， 17， 19， 21， 31， 49， 51， 61， 71， 81， 91， 101  典型值范围：3 ≤ maskHeight ≤ 201
            /// </summary>
            public double Emphasize_MaskHeight { set; get; } = 50;
            /// <summary>
            /// 对比强调的强度。 默认值：1.0建议值：0.3， 0.5， 0.7， 1.0， 1.4， 1.8， 2.0典型值范围：（sqrt）0.0 ≤ factor ≤ 20.0
            /// </summary>
            public double Emphasize_Factor { set; get; } = 20;
            /// <summary>
            /// 灰度值选区 
            /// </summary>
            public bool Gray_Disp { set; get; } = true;
            /// <summary>
            /// 灰度值的较低阈值或“Min”.默认值：128.0 建议值：0.0， 10.0， 30.0， 64.0， 128.0， 200.0， 220.0， 255.0，“Min”
            /// </summary>
            public double MinGray { set; get; } = 0;
            /// <summary>
            /// 灰度值的上限阈值或“Max”.默认值：255.0  建议值：0.0， 10.0， 30.0， 64.0， 128.0， 200.0， 220.0， 255.0，“Max”
            /// </summary>
            public double MaxGray { set; get; } = 100;
            /// <summary>
            /// 开运算消除边缘
            /// </summary>
            public double OpeningCircle_Radius { set; get; } = 2;
            /// <summary>
            /// 最小面积
            /// </summary>
            public double Min_Area { set; get; } = 50000;
            /// <summary>
            /// 最大面积
            /// </summary>
            public double Max_Area { set; get; } = 2000000;
        }




        /// <summary>
        /// Halcon标定相机参数
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Halcon_Camera_Calibration_Parameters_Model
        {

            /// <summary>
            /// 相机标定类型
            /// </summary>
            public Halocn_Camera_Calibration_Enum Camera_Calibration_Model { set; get; } = Halocn_Camera_Calibration_Enum.area_scan_polynomial;

            /// <summary>
            /// 单个像高度：微米
            /// </summary>
            public double Sy { set; get; } = 2.2;
            /// <summary>
            /// 单个像宽度：微米
            /// </summary>
            public double Sx { set; get; } = 2.2;

            /// <summary>
            /// 焦距：mm
            /// </summary>
            public double Focus { set; get; } = 16;

            /// <summary>
            /// 畸变系数：1/㎡
            /// </summary>
            public double Kappa { set; get; } = 0;

            /// <summary>
            /// 径向2阶：1/㎡
            /// </summary>
            public double K1 { set; get; } = 0;
            /// <summary>
            /// 径向4阶：1/㎡
            /// </summary>
            public double K2 { set; get; } = 0;
            /// <summary>
            /// 径向6阶：1/㎡
            /// </summary>
            public double K3 { set; get; } = 0;

            /// <summary>
            /// 切向2阶：1/㎡
            /// </summary>
            public double P1 { set; get; } = 0;
            /// <summary>
            /// 切向2阶：1/㎡
            /// </summary>
            public double P2 { set; get; } = 0;





            /// <summary>
            /// 中间高度度位置
            /// </summary>
            public double Cy { set; get; } = 250;
            /// <summary>
            /// 中间宽度位置
            /// </summary>
            public double Cx { set; get; } = 250;

            /// <summary>
            /// 最大高度像素
            /// </summary>
            public int Image_Width { set; get; } = 500;
            /// <summary>
            /// 最大宽度像素
            /// </summary>
            public int Image_Height { set; get; } = 500;
        }

    }

    /// <summary>
    /// 相机标定属性模型
    /// </summary>
    public class Halcon_Camera_Calibration_Model
    {




        /// <summary>
        /// 标定相机类型
        /// </summary>
        //public Halcon_Camera_Calibration_Parameters_Model Calibration_Paramteters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();


        /// <summary>
        /// 相机标定类型
        /// </summary>
        public Halcon_Calibration_Setup_Model_Enum Calibration_Setup_Model { set; get; } = Halcon_Calibration_Setup_Model_Enum.calibration_object;


        /// <summary>
        /// 标定板位置
        /// </summary>
        public string Halcon_CaltabDescr_Address { set; get; }
        /// <summary>
        /// 标定板厚度
        /// </summary>
        public double Halcon_CaltabThickness { set; get; } = 0;

        /// <summary>
        ///  标定相机数量 
        /// </summary>
        public int Haclon_Camera_number { set; get; } = 0;

        /// <summary>
        /// 标定板图像数量
        /// </summary>
        public int Haclon_Calibration_Image_number { get; } = 10;


        /// <summary>
        /// 标定板默认数量：1
        /// </summary>
        public int Haclon_Calibration_number { get; } = 1;

        /// <summary>
        /// 标定板识别滤波
        /// </summary>
        public double Halcon_Calibretion_Sigma { set; get; } = 1;

        /// <summary>
        /// 查找标定板模式：False-单帧模式
        /// </summary>
        public bool Halcon_Find_Calib_Model { set; get; } = false;


        /// <summary>
        /// 查找标定板图像显示
        /// </summary>
        public bool Halcon_Calib_XLD_Show { set; get; } = true;
    }





    public class Match_Models_List_Model
    {
        /// <summary>
        /// 模型号
        /// </summary>
        public int Match_ID { set; get; }
        /// <summary>
        /// 模型文件
        /// </summary>
        public FileInfo Match_File { set; get; }
        /// <summary>
        /// 模型区域
        /// </summary>
        public ShapeModel_Name_Enum Match_Area { set; get; }
        /// <summary>
        /// 模型匹配类型
        /// </summary>
        public Shape_Based_Model_Enum Match_Model { set; get; }
        /// <summary>
        /// 模型文件格式类型
        /// </summary>
        public Match_FileName_Type_Enum File_Type { set; get; }
        /// <summary>
        /// 模型文件号
        /// </summary>
        public int Match_No { set; get; }
        /// <summary>
        /// 匹配模型预存类型
        /// </summary>
        public Halcon_Method Model { set; get; } = new Halcon_Method();
        /// <summary>
        /// 获得模型文件名
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return Match_ID + (int)Match_Model + Match_No + "." + File_Type;
        }
    }
    /// <summary>
    /// 集合算子运行情况属性
    /// </summary>
    public class HPR_Status_Model
    {

        /// <summary>
        /// 泛型类型委托声明
        /// </summary>
        /// <param name="_Connect_State"></param>
        public delegate void HVS_T_delegate<T>(T _Tl);

        /// <summary>
        /// 算法设置错误委托属性
        /// </summary>
        public static HVS_T_delegate<string> HVS_ErrorInfo_delegate { set; get; }



        public HPR_Status_Model(HVE_Result_Enum _Status)
        {
            Result_Status = _Status;


        }
        /// <summary>
        /// 运行错误状态
        /// </summary>
        public HVE_Result_Enum Result_Status { set; get; }
        /// <summary>
        /// 运行错误详细信息
        /// </summary>
        private string _Result_Error_Info;

        public string Result_Error_Info
        {
            get { return _Result_Error_Info; }
            set
            {
                _Result_Error_Info = value;

                HVS_ErrorInfo_delegate(GetResult_Info());

            }
        }
        /// <summary>
        /// 获得算法运行状态
        /// </summary>
        /// <returns></returns>
        public bool GetResult()
        {
            if (Result_Status == HVE_Result_Enum.Run_OK) { return true; } else { return false; }
        }
        /// <summary>
        /// 获得算法运行状态信息
        /// </summary>
        /// <param name="_Erroe"></param>
        /// <returns></returns>
        public string GetResult_Info()
        {
            return Result_Status.ToString() + " " + Result_Error_Info;
        }
    }


    /// <summary>
    /// 标定图像集合模型参数
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Calibration_Image_List_Model
    {
        /// <summary>
        /// 标定图像序号
        /// </summary>
        public int Image_No { set; get; } = 0;

        /// <summary>
        /// 主相机
        /// </summary>
        public Calibration_Image_Camera_Model Camera_0 { set; get; } = new Calibration_Image_Camera_Model() { };
        /// <summary>
        /// 副相机
        /// </summary>
        public Calibration_Image_Camera_Model Camera_1 { set; get; } = new Calibration_Image_Camera_Model() { };


        public int Camera_No { set; get; } = 0;


    }

    /// <summary>
    /// 相机标定误差模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Calibration_Camera_Results_Model
    {

        public double Error_Pixel { set; get; } = 0;


        public double RMS_Translational { set; get; } = 0;

        public double RMS_Rotational { set; get; } = 0;

        public double Maximum_Translational { set; get; } = 0;

        public double Maximum_Rotational { set; get; } = 0;

        public string Calibration_Results_Save_File { set; get; } = "";
    }






    /// <summary>
    /// 标定相机集合类型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Caliration_AllCamera_Results_Model
    {

        public Calibration_Camera_Data_Results_Model Camera_0_Results { set; get; } = new Calibration_Camera_Data_Results_Model();

        public Calibration_Camera_Data_Results_Model Camera_1_Results { set; get; } = new Calibration_Camera_Data_Results_Model();

    }





    [AddINotifyPropertyChangedInterface]
    public class Calibration_Camera_Data_Results_Model
    {
        /// <summary>
        /// 标定结果像素误差
        /// </summary>
        public double Result_Error_Val { set; get; } = 0;

        /// <summary>
        /// 标定结果保存位置
        /// </summary>
        public string Result_File_Address { set; get; } = Directory.GetCurrentDirectory() + "\\Calibration_File";

        /// <summary>
        /// 相机标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Parameters_Model Camera_Result_Pama { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();


    


    }




    [AddINotifyPropertyChangedInterface]
    public class Calibration_Image_Camera_Model
    {




        /// <summary>
        /// 标定精度
        /// </summary>
        public double Calibration_Accuracy { set; get; } = 0;


        /// <summary>
        /// 标定图像
        /// </summary>
        public HObject Calibration_Image { set; get; } = null;


        /// <summary>
        /// 标定板特征
        /// </summary>
        public HObject Calibration_Region { set; get; } = new HObject();

        /// <summary>
        /// 标定板坐标系
        /// </summary>
        public HObject Calibration_XLD { set; get; } = new HObject();



        //相机名称图像的
        public string Carme_Name { set; get; }

        /// <summary>
        /// 标定状态
        /// </summary>
        public string Calibration_State { set; get; } = "None";




    }




    /// <summary>
    /// 相机标定
    /// </summary>
    public enum Halocn_Camera_Calibration_Enum
    {
        /// <summary>
        /// 面扫相机_畸形式
        /// </summary>
        area_scan_division,
        /// <summary>
        /// 面扫相机_多项式
        /// </summary>
        area_scan_polynomial
    }



    /// <summary>
    /// Halcon 标定模式枚举
    /// </summary>
    public enum Halcon_Calibration_Setup_Model_Enum
    {
        calibration_object,
        hand_eye_moving_cam,
        hand_eye_scara_moving_cam,
        hand_eye_scara_stationary_cam,
        hand_eye_stationary_cam
    }





    /// <summary>
    /// 查找模型位置预处理方法名称枚举
    /// </summary>
    public enum Find_Shape_Function_Name_Enum
    {
        /// <summary>
        /// 增强图像的对比度。
        /// </summary>
        Emphasize,
        /// <summary>
        /// 计算带有矩形掩码的中值滤波器。
        /// </summary>
        MedianRect,
        /// <summary>
        /// 最大灰度值分布在值范围0到255 中。
        /// </summary>
        ScaleImageMax,
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
        continued,
    }
    /// <summary>
    /// 匹配模型后缀类型
    /// </summary>
    public enum Match_FileName_Type_Enum
    {
        ncm,
        dxf,
        dfm,
        shm
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
        Scale_model,
        /// <summary>
        /// 相关性匹配模板
        /// </summary>
        Ncc_Model,
        /// <summary>
        /// Halcon通用保存格式
        /// </summary>
        Halcon_DXF
    }
    public enum FilePath_Type_Model_Enum
    {
        /// <summary>
        /// 文件路径获取
        /// </summary>
        Get,
        /// <summary>
        /// 文件路径保存
        /// </summary>
        Save,
    }
    /// <summary>
    /// 匹配模型位置名称
    /// </summary>
    public enum ShapeModel_Name_Enum
    {
        F_45,
        F_135,
        F_225,
        F_315
    }
    /// <summary>
    /// 工装号数
    /// </summary>
    public enum Work_Name_Enum
    {
        Null,
        Work_1,
        Work_2,
        Work_3,
        Work_4
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
    /// 获得采集图片方式
    /// </summary>
    public enum Get_Image_Model_Enum
    {
        相机采集,
        图像采集,
        触发采集
    }
    public enum HFIle_Type_Enum
    {
        /// <summary>
        /// 将打开已存在的输入文件以文本格式进行读取。
        /// </summary>
        input,
        /// <summary>
        /// 将打开一个新的输出文件以文本格式写入。
        /// </summary>
        output,
        /// <summary>
        /// 一个已经存在的输出文件被打开，在文件的最后以文本格式写入。
        /// </summary>
        append,
        /// <summary>
        /// 一个已经存在的输入文件被打开以读取二进制格式。
        /// </summary>
        input_binary,
        /// <summary>
        /// 一个新的输出文件被打开，用于写入二进制格式的文件。
        /// </summary>
        output_binary,
        /// <summary>
        /// 一个已经存在的输出文件被打开，在文件的最后以二进制格式写入。
        /// 对于文本文件，传递给fileType的元组可以通过以下编码设置之一进行扩展：
        /// </summary>
        append_binary,
        /// <summary>
        /// 文件中的字符串是以UTF-8编码的。这是默认的，所以对于UTF-8编码的文件和所有只使用纯7位US-ASCII字符的文件，这个值可以省略。
        /// </summary>
        utf8_encoding,
        /// <summary>
        /// 文件中的字符串以本地8位编码进行编码，这取决于系统当前的locale设置。有效的编码定义，例如，在Windows下由1252（微软的Latin-1方言）或932（Shift-JIS）代码页定义，或在Linux下由en_US.utf8、de_DE.iso885915或ja_JP.sjis等区域性编码定义。
        /// </summary>
        locale_encoding,
        ignore_encoding
    }
    /// <summary>
    /// 标定错误消息返回
    /// </summary>
    public enum HVE_Result_Enum
    {
        Run_OK,
        Vision_Ini_Data_OK,
        Find_time_timeout,
        Find_Calibration_Error,
        Camera_Connection_Time_Out,
        Error_No_Camera_GetImage,
        Error_No_Read_Shape_Mode_File,
        Error_No_Read_Math2D_File,
        Error_No_Camera_Set_Parameters,
        Error_No_Can_Find_the_model,
        Error_No_Find_ID_Number,
        Error_No_SinkInfo,
        Error_Find_Exceed_Error_Val,
        Error_Save_Math2D_File_Error,
        Error_Match_Math2D_Error,
        图像预处理错误,
        Halcon转换海康图像错误,
        相机采集失败,
        图像文件读取失败,
        读取图像文件格式错误,
        图像保存失败,
        样品图像保存失败,
        文件路径提取失败,
        创建匹配模型失败,
        读取模型文件失败,
        读取全部模型文件失败,
        Ncc匹配文件缺失,
        XLD对象映射失败,
        读取矩阵文件失败,
        保存矩阵文件失败,
        计算实际误差失败,
        标定查找9点位置区域失败,
        添加的直线类型不足2点数据_重新添加,
        添加的圆弧类型不足3点数据_重新添加,
        添加直线类型失败,
        添加圆弧类型失败,
        添加数据失败,
        XLD数据集合不足1组以上,
        XLD数据集合创建失败,
        Halcon文件类型读取失败,
        提取匹配结果的XLD模型数量与计算数量不匹配,
        提取匹配结果的XLD模型失败,
        查找模型匹配失败,
        根据匹配模型结果计算交点信息失败,
        匹配模型文件内存清除失败,
        XLD匹配结果映射失败,
        显示最大灰度失败,
        显示最小灰度失败,
        标定板图像识别错误,
    }




    /// <summary>
    /// 显示类型
    /// </summary>
    public enum Display_HObject_Type_Enum
    {
        Image,
        Region,
        XLD,
        SetDrawColor,


    }
    /// <summary>
    /// 窗口显示变量
    /// </summary>
    public enum Window_Show_Name_Enum
    {
        Live_Window,
        Features_Window,
        Results_Window_1,
        Results_Window_2,
        Results_Window_3,
        Results_Window_4,
        Calibration_Window_1,
        Calibration_Window_2,
        Calibration_3D_Results,
    }

    /// <summary>
    /// 区域显示设置枚举
    /// </summary>
    public enum DisplaySetDraw_Enum
    {
        fill,
        margin
    }
}
