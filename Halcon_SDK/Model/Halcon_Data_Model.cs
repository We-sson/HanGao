using HalconDotNet;

using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;

namespace Halcon_SDK_DLL.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Data_Model
    {
        [AddINotifyPropertyChangedInterface]
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
        ///
        [AddINotifyPropertyChangedInterface]
        public class Display3DModel_Model
        {
            public Display3DModel_Model()
            {
            }

            public Display3DModel_Model(List<HObjectModel3D> objectModel3D)
            {
                _ObjectModel3D = objectModel3D;
                //_CamParam = new ();
                //_PoseIn = new ();
                //_PoseOut = new ();
                //_GenParamName = new ();
                //_GenParamValue = new ();
                //_Title = new ();
                //_Label = new ();
                //_Information = new ();
            }

            public List<HObjectModel3D> _ObjectModel3D { set; get; } = new List<HObjectModel3D>();
            //public HTuple _CamParam { set; get; } = new ();

            /// <summary>
            /// 模型位置单位：M
            /// </summary>
            public HPose? _PoseIn { set; get; }

            //public HTuple _PoseOut { set; get; } = new ();
            //public HTuple _GenParamName { set; get; } = new ();
            //public HTuple _GenParamValue { set; get; } = new ();
            //public HTuple _Title { set; get; } = new ();
            //public HTuple _Label { set; get; } = new ();
            //public HTuple _Information { set; get; } = new ();
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
            public HWindow? Image_Show_Halcon;
        }

        /// <summary>
        /// 形状匹配查找模型结果参数
        /// </summary>
        [AddINotifyPropertyChangedInterface]
        public class Find_Shape_Results_Model
        {

            /// <summary>
            /// 匹配结果2d矩阵列表
            /// </summary>
            public ObservableCollection<HHomMat2D> Results_HomMat2D_List { set; get; } = new ();


            /// <summary>
            /// 匹配结果xld显示列表
            /// </summary>
            public ObservableCollection<HXLDCont> Results_HXLD_List { set; get; } = new ();


            public HObject HXLD_Results_All { set; get; } = new HObject();

            /// <summary>
            /// 匹配结果点
            /// </summary>
            public Point_Model Results_ModelInCam_Pos { set; get; } = new Point_Model();
            //public HImage Image_Rectified { set; get; } = new HImage();

            public Point_Model Results_Image_Pos { set; get; } = new Point_Model();




            public Point_Model Results_ModelInBase_Pos { set; get; } = new Point_Model();
            public Point_Model Results_PlanOffset_Pos { set; get; } = new Point_Model();



            public ObservableCollection<Point_Model> Results_PathInBase_Pos { set; get; } = new();


            /// <summary>
            /// 模型实例的行坐标 =Y
            /// </summary>
            public ObservableCollection<double> Find_Row { set; get; } = new ();

            /// <summary>
            /// 模型实例的列坐标 =X
            /// </summary>
            public ObservableCollection<double> Find_Column { set; get; } = new ();

            /// <summary>
            /// 模型实例的旋转角度
            /// </summary>
            public ObservableCollection<double> Find_Angle { set; get; } = new ();

            /// <summary>
            /// 模型和找到的实例相似值
            /// </summary>
            public ObservableCollection<double> Find_Score { set; get; } = new ();

            /// <summary>
            /// 查找耗时
            /// </summary>
            public ObservableCollection<double> Find_Time { set; get; } = new ();



            /// <summary>
            /// 查找模型结果
            /// </summary>
            //public List<bool> Find_Results { set; get; } = new List<bool>();

            /// <summary>
            /// 存储结果点
            /// </summary>
            public ObservableCollection<Point3D> Vision_Pos { set; get; } = new ();

            /// <summary>
            /// 存储结果点
            /// </summary>
            public ObservableCollection<Point3D> Robot_Pos { set; get; } = new ();

            //public double Right_Angle { set; get; } = 0;
            /// <summary>
            /// 找到的模型实例的分数
            /// </summary>
            public ObservableCollection<HTuple> HomMat2D { set; get; } = new ();

            /// <summary>
            /// 页面显示结过集合
            /// </summary>
            public ObservableCollection<string> Text_Arr_UI { set; get; } = new ObservableCollection<string>() { };

            /// <summary>
            /// 显示结果界面
            /// </summary>
            public Window_Show_Name_Enum DispWindow { set; get; } = Window_Show_Name_Enum.Features_Window;


            /// <summary>
            /// 模型匹配状态
            /// </summary>
            public Find_Shape_Results_State_Enum Find_Shape_Results_State { set; get; } = Find_Shape_Results_State_Enum.Match_None;


            public ObservableCollection<string> Set_Results_Data_List()
            {
                List<string> _DataList = new();
                
                Text_Arr_UI.Add("匹配模型Base坐标偏移结果：" + Results_PlanOffset_Pos.ToString());
                Text_Arr_UI.Add("匹配模型Base原点坐标：" + Results_ModelInBase_Pos.ToString());
                Text_Arr_UI.Add("匹配模型到相机坐标：" + Results_ModelInCam_Pos.ToString());
                Text_Arr_UI.Add("匹配像素坐标结果：" + Results_Image_Pos.ToString());
                Text_Arr_UI.Add("详情各特征结果：");


                for (int i = 0; i < Results_HXLD_List.Count; i++)
                {

                    Text_Arr_UI.Add($"匹配模型—{i}号 | 像素坐标 X: {Find_Column[i]:F3}mm,Y: {Find_Row[i]:F3}mm, Angle: {(180 / Math.PI) * Find_Angle[i]:F4}度, 相似度：{Find_Score[i]:F3}, 匹配耗时：{Find_Time[i]:F3}秒");


                }

                Text_Arr_UI.Add("----------------------------分割线--------------------------------");

                for (int i = 0; i < Results_PathInBase_Pos.Count; i++)
                {
                    Text_Arr_UI.Add($"结果路径坐标—{i}号 | 坐标 X: {Results_PathInBase_Pos[i].X:F3}mm,Y: {Results_PathInBase_Pos[i].Y:F3}mm, Z:  {Results_PathInBase_Pos[i].Z:F3}mm, Rx: {Results_PathInBase_Pos[i].Rx:F3}度, Ry: {Results_PathInBase_Pos[i].Ry:F3}, Rz: {Results_PathInBase_Pos[i].Rz:F3}");
                }

                return new ObservableCollection<string> (_DataList);
            }







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
            public string ShapeModel_Name { set; get; } ="默认";

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
            public int MinContrast { set; get; } = 5;

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
            public Find_Shape_Based_ModelXld()
            {

            }
            public Find_Shape_Based_ModelXld(Find_Shape_Based_ModelXld _)
            {
                Shape_Based_Model = _.Shape_Based_Model;
                ShapeModel_Name = _.ShapeModel_Name;
                FInd_ID = _.FInd_ID;
                AngleStart = _.AngleStart;
                AngleExtent = _.AngleExtent;
                ScaleRMin = _.ScaleRMin;
                ScaleRMax = _.ScaleRMax;
                ScaleCMax = _.ScaleCMax;
                ScaleCMin = _.ScaleCMin;
                MinScore = _.MinScore;
                NumMatches = _.NumMatches;
                MaxOverlap = _.MaxOverlap;
                NumLevels = _.NumLevels;
                Greediness = _.Greediness;
                SubPixel = _.SubPixel;
                NCC_SubPixel = _.NCC_SubPixel;
                Angle_change_restriction = _.Angle_change_restriction;
                Aniso_scale_change_restriction = _.Aniso_scale_change_restriction;
                Compulsory_Image_Rectified = _.Compulsory_Image_Rectified;
                Time_Out = _.Time_Out;
                Auto_Image_Rectified = _.Auto_Image_Rectified;
                 
            }

            /// <summary>
            /// 模板类型
            /// </summary>
            public Shape_Based_Model_Enum Shape_Based_Model { set; get; } = Shape_Based_Model_Enum.Ncc_Model;

            /// <summary>
            /// UI绑定查找模型区域名字
            /// </summary>
            public ShapeModel_Name_Enum ShapeModel_Name { set; get; } = ShapeModel_Name_Enum.F_45;

            /// <summary>
            /// 模型创建查找ID号
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
            public int NumLevels { set; get; } = 5;

            /// <summary>
            /// 搜索启发式的“贪婪”（0：安全但慢;1：快但可能会错过匹配）。默认值：0.9
            /// </summary>
            public double Greediness { set; get; } = 0.9;

            /// <summary>
            /// 亚像素精度（如果不等于）“无”.默认值： “least_squares”
            /// </summary>
            public Subpixel_Values_Enum SubPixel { set; get; } = Subpixel_Values_Enum.least_squares;



            /// <summary>
            /// NCC匹配是否亚精度
            /// </summary>
            public bool NCC_SubPixel { set; get; } = true;

            /// <summary>
            /// 是否强制校正图像
            /// </summary>
            public bool Compulsory_Image_Rectified { set; get; } = false;

            /// <summary>
            /// 图像自动校正
            /// </summary>
            public bool Auto_Image_Rectified { set; get; } = false ;

            /// <summary>
            /// 匹配查找世界限制超时
            /// </summary>
            public int Time_Out { set; get; } = 5000;
            /// <summary>
            /// 最大容许角度畸变 可以限制,作为默认值已设置3.14159/2，为 0，则根本不允许失真。
            /// </summary>
            public double Angle_change_restriction { set; get; } = 1.57;

            /// <summary>
            ///  可以限制各向异性缩放（较小的比例因子除以 更大的比例因子）。 此参数的值范围从默认值 0.0，其中 允许任意失真，到 1.0，其中不允许失真。
            /// </summary>
            public double Aniso_scale_change_restriction { set; get; } = 0.3;

            ///// <summary>
            ///// 最大灰度值功能使能
            ///// </summary>
            //public bool ScaleImageMax_Enable { set; get; } = true;

            ///// <summary>
            ///// 最大灰度值功能显示
            ///// </summary>
            //public bool ScaleImageMax_Disp { set; get; } = true;

            ///// <summary>
            ///// 增强图像功能使能
            ///// </summary>
            //public bool Illuminate_Enable { set; get; } = true;

            ///// <summary>
            ///// 增强图像功能显示
            ///// </summary>
            //public bool Illuminate_Disp { set; get; } = true;

            ///// <summary>
            /////  增强图像的低通掩码的宽度。
            ///// </summary>
            //public int Illuminate_MaskWidth { set; get; } = 101;

            ///// <summary>
            /////  增强图像的低通掩码的高度。
            ///// </summary>
            //public int Illuminate_MaskHeight { set; get; } = 101;

            ///// <summary>
            ///// 增强图像的对比强调的强度
            ///// </summary>
            //public double Illuminate_Factor { set; get; } = 0.8;

            ///// <summary>
            ///// 增强图像功能使能
            ///// </summary>
            //public bool Emphasize_Enable { set; get; } = true;

            ///// <summary>
            ///// 增强图像功能显示
            ///// </summary>
            //public bool Emphasize_Disp { set; get; } = true;

            ///// <summary>
            /////  增强图像的低通掩码的宽度。
            ///// </summary>
            //public int Emphasize_MaskWidth { set; get; } = 15;

            ///// <summary>
            /////  增强图像的低通掩码的高度。
            ///// </summary>
            //public int Emphasize_MaskHeight { set; get; } = 15;

            ///// <summary>
            ///// 增强图像的对比强调的强度
            ///// </summary>
            //public double Emphasize_Factor { set; get; } = 5;

            ///// <summary>
            ///// /计算带有矩形掩码的中值滤波器是使能
            ///// </summary>
            //public bool Median_image_Enable { set; get; } = true;

            ///// <summary>
            ///// 计算带有矩形掩码的中值滤波器显示
            ///// </summary>
            //public bool Median_image_Disp { set; get; } = true;

            ///// <summary>
            ///// 滤镜遮罩类型。默认值： “圆  值列表：“圆”, “正方形”值列表（对于计算设备）：“正方形”
            ///// </summary>
            //public MedianImage_MaskType_Enum MaskType_Model { set; get; } = MedianImage_MaskType_Enum.circle;

            ///// <summary>
            ///// 滤镜掩码的半径。默认值：1  值列表（用于计算设备）：1、2  建议值：1， 2， 3， 4， 5， 6， 7， 8， 9， 11， 15， 19， 25， 31， 39， 47， 59  典型值范围：1 ≤ radius ≤ 4095
            ///// </summary>
            //public int Median_image_Radius { set; get; } = 5;

            ///// <summary>
            ///// 边境处理。默认值： “镜像”值列表（对于计算设备）：“镜像”建议值：“镜像”, “循环”, “续”, 0, 30, 60, 90, 120, 150, 180, 210, 240, 255
            ///// </summary>
            //public MedianImage_Margin_Enum Margin_Model { set; get; } = MedianImage_Margin_Enum.mirrored;

            ///// <summary>
            ///// 矩形掩码的中值滤波器使能
            ///// </summary>
            //public bool MedianRect_Enable { set; get; } = true;

            ///// <summary>
            ///// /矩形掩码的中值滤波器显示
            ///// </summary>
            //public bool MedianRect_Disp { set; get; } = true;

            ///// <summary>
            ///// 矩形掩码的中值滤波器使能,滤镜掩码的宽度。
            ///// </summary>
            //public int MedianRect_MaskWidth { set; get; } = 3;

            ///// <summary>
            ///// 矩形掩码的中值滤波器使能,滤镜掩码的高度。
            ///// /// </summary>
            //public int MedianRect_MaskHeight { set; get; } = 3;

            ///// <summary>
            ///// 灰度开运算使能
            ///// </summary>
            //public bool GrayOpeningRect_Enable { set; get; } = true;

            ///// <summary>
            ///// 灰度开运算显示
            ///// </summary>
            //public bool GrayOpeningRect_Disp { set; get; } = true;

            ///// <summary>
            ///// 灰度开运算滤镜掩码的高度
            ///// </summary>
            //public int GrayOpeningRect_MaskHeight { set; get; } = 16;

            ///// <summary>
            ///// 灰度开运算滤镜掩码的宽度
            ///// </summary>
            //public int GrayOpeningRect_MaskWidth { set; get; } = 16;

            ///// <summary>
            ///// 灰度开运算使能
            ///// </summary>
            //public bool GrayClosingRect_Enable { set; get; } = true;

            ///// <summary>
            ///// 灰度开运算显示
            ///// </summary>
            //public bool GrayClosingRect_Disp { set; get; } = true;

            ///// <summary>
            ///// 灰度开运算滤镜掩码的高度
            ///// </summary>
            //public int GrayClosingRect_MaskHeight { set; get; } = 16;

            ///// <summary>
            ///// 灰度开运算滤镜掩码的宽度
            ///// </summary>
            //public int GrayClosingRect_MaskWidth { set; get; } = 16;
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
            public Halcon_Camera_Calibration_Parameters_Model()
            {
            }

            public Halcon_Camera_Calibration_Parameters_Model(HCamPar hCamPar)
            {
                HCamPar = new HCamPar(hCamPar);
            }

            public Halcon_Camera_Calibration_Parameters_Model(HTuple hCamPar)
            {
                HCamPar = new HCamPar(hCamPar);
            }

            public Halcon_Camera_Calibration_Parameters_Model(Halcon_Camera_Calibration_Parameters_Model _Parameters_Model)
            {
                Sy = _Parameters_Model.Sy;
                Sx = _Parameters_Model.Sx;
                Focus = _Parameters_Model.Focus;
                Kappa = _Parameters_Model.Kappa;
                K1 = _Parameters_Model.K1;
                K2 = _Parameters_Model.K2;
                K3 = _Parameters_Model.K3;
                P1 = _Parameters_Model.P1;
                P2 = _Parameters_Model.P2;
                Cy = _Parameters_Model.Cy;
                Cx = _Parameters_Model.Cx;
                Image_Height = _Parameters_Model.Image_Height;
                Image_Width = _Parameters_Model.Image_Width;
            }

            private void Set_HCamPar(HCamPar hCamPar)
            {
                //HCamPar = hCamPar;

                switch (Enum.Parse<Halocn_Camera_Calibration_Enum>(hCamPar[0]))
                {
                    case Halocn_Camera_Calibration_Enum.area_scan_division:
                        Camera_Calibration_Model = Halocn_Camera_Calibration_Enum.area_scan_division;
                        Focus = hCamPar[1] * 1000;
                        Kappa = hCamPar[2];
                        Sx = hCamPar[3] * 1000000;
                        Sy = hCamPar[4] * 1000000;
                        Cx = hCamPar[5];
                        Cy = hCamPar[6];
                        Image_Width = hCamPar[7];
                        Image_Height = hCamPar[8];

                        break;

                    case Halocn_Camera_Calibration_Enum.area_scan_polynomial:
                        Camera_Calibration_Model = Halocn_Camera_Calibration_Enum.area_scan_polynomial;
                        Focus = hCamPar[1] * 1000;
                        K1 = hCamPar[2];
                        K2 = hCamPar[3];
                        K3 = hCamPar[4];
                        P1 = hCamPar[5];
                        P2 = hCamPar[6];
                        Sx = hCamPar[7] * 1000000;
                        Sy = hCamPar[8] * 1000000;
                        Cx = hCamPar[9];
                        Cy = hCamPar[10];
                        Image_Width = hCamPar[11];
                        Image_Height = hCamPar[12];

                        break;
                }
            }

            public HCamPar Get_HCamPar()
            {
                HCamPar hCamPar = new ();

                switch (Camera_Calibration_Model)
                {
                    case Halocn_Camera_Calibration_Enum.area_scan_division:

                        hCamPar[0] = Camera_Calibration_Model.ToString();
                        hCamPar[1] = Focus / 1000;
                        hCamPar[2] = Kappa;
                        hCamPar[3] = Sx / 1000000;
                        hCamPar[4] = Sy / 1000000;
                        hCamPar[5] = Cx;
                        hCamPar[6] = Cy;
                        hCamPar[7] = Image_Width;
                        hCamPar[8] = Image_Height;

                        break;

                    case Halocn_Camera_Calibration_Enum.area_scan_polynomial:

                        hCamPar[0] = Camera_Calibration_Model.ToString();
                        hCamPar[1] = Focus / 1000;
                        hCamPar[2] = K1;
                        hCamPar[3] = K2;
                        hCamPar[4] = K3;
                        hCamPar[5] = P1;
                        hCamPar[6] = P2;
                        hCamPar[7] = Sx / 1000000;
                        hCamPar[8] = Sy / 1000000;
                        hCamPar[9] = Cx;
                        hCamPar[10] = Cy;
                        hCamPar[11] = Image_Width;
                        hCamPar[12] = Image_Height;

                        break;
                }

                return hCamPar;
            }

            /// <summary>
            /// 标定内参参数变量
            /// </summary>
            private HCamPar _HCamPar = new ();

            public HCamPar HCamPar
            {
                get
                {
                    _HCamPar = Get_HCamPar();
                    return _HCamPar;
                }
                set
                {
                    _HCamPar = value;
                    Set_HCamPar(value);
                }
            }

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
    /// 图像查找标定板结果类型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class FindCalibObject_Results : IDisposable
    {
        public FindCalibObject_Results()
        {
        }

        /// <summary>
        /// 拷贝变量
        /// </summary>
        /// <param name="_Results"></param>
        public FindCalibObject_Results(FindCalibObject_Results _Results)
        {
            _Image = _Results._Image;
            _CalibXLD = _Results._CalibXLD;
            _CalibRegion = _Results._CalibRegion;
            _DrawColor = _Results._DrawColor;
            hv_Row = _Results.hv_Row;
            hv_Column = _Results.hv_Column;
            hv_I = _Results.hv_I;
            hv_Pose = _Results.hv_Pose;
        }

        public HObject _Image { set; get; } = new HObject();
        public HObject _CalibXLD { set; get; } = new HObject();
        public HObject _CalibRegion { set; get; } = new HObject();

        //public HRegion ShowGray { set; get; } = new HRegion();

        public string _DrawColor { set; get; } = KnownColor.Blue.ToString();

        public HTuple hv_Row = new ();
        public HTuple hv_Column = new ();
        public HTuple hv_I = new ();
        public HTuple hv_Pose = new ();

        public void Dispose()
        {
            //ShowGray.Dispose();

            _CalibXLD.Dispose();
            //_CalibCoord.Dispose();

            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_I.Dispose();
            hv_Pose.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 相机标定属性模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Camera_Calibration_Model
    {
        public Halcon_Camera_Calibration_Model()
        {
            Read_Calibration_Plate_File();
        }

        /// <summary>
        /// 标定相机类型
        /// </summary>
        //public Halcon_Camera_Calibration_Parameters_Model Calibration_Paramteters { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();

        /// <summary>
        /// 相机标定类型
        /// </summary>
        public Halcon_Calibration_Setup_Model_Enum Calibration_Setup_Model { set; get; } = Halcon_Calibration_Setup_Model_Enum.calibration_object;

        /// <summary>
        /// 手眼标定校正模式
        /// </summary>
        public HandEye_Optimization_Method_Enum HandEye_Optimization_Method { set; get; } = HandEye_Optimization_Method_Enum.stochastic;

        /// <summary>
        /// 标定结果保存文件夹
        /// </summary>
        public string HandEye_Result_Fold_Address { set; get; } = Directory.GetCurrentDirectory() + "\\Calibration_File\\";

        /// <summary>
        /// 标定板厚度mm
        /// </summary>
        public double Halcon_CaltabThickness { set; get; } = 2;

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
        /// 手眼标定的旋转容差：单位度
        /// </summary>
        public double HandEye_Calibration_Check_Rotation { set; get; } = 5;

        /// <summary>
        /// 手眼标定的平移容差：单位mm
        /// </summary>
        public double HandEye_Calibration_Check_Translation { set; get; } = 10;

        /// <summary>
        /// 标定板列表
        /// </summary>
        public List<FileInfo> Calibration_Plate_List { set; get; } = new List<FileInfo>();

        /// <summary>
        /// 选择标定板类型
        /// </summary>
        public FileInfo? Selected_Calibration_Pate_Address { set; get; }

        /// <summary>
        /// 读取标定板类型文件方法
        /// </summary>
        public void Read_Calibration_Plate_File()
        {
            //固定标定板存放位置
            string _address = Directory.GetCurrentDirectory() + "\\Calibration_File\\CalTabFile";

            //判断位置是否存在
            if (!Directory.Exists(_address)) Directory.CreateDirectory(_address);

            //获得的文件夹内文件
            FileInfo[] Files = new DirectoryInfo(_address).GetFiles();

            //读取标定板文件
            foreach (var file in Files)
            {
                if (file.Extension.Equals(".cpd") || file.Extension.Equals(".descr"))
                {
                    //添加到列表中
                    Calibration_Plate_List.Add(file);
                }
            }
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Match_Models_List_Model
    {
        /// <summary>
        /// 模型号
        /// </summary>
        public int Match_ID { set; get; }

        /// <summary>
        /// 模型文件
        /// </summary>
        public FileInfo? Match_File { set; get; }

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
        public Halcon_Method_Model Model { set; get; } = new Halcon_Method_Model();

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
    [AddINotifyPropertyChangedInterface]
    public class HPR_Status_Model<T1>
    {
        /// <summary>
        /// 泛型类型委托声明
        /// </summary>
        /// <param name="_Connect_State"></param>
        public delegate void HVS_T_delegate<T>(T _Tl);

        /// <summary>
        /// 算法设置错误委托属性
        /// </summary>
        public static HVS_T_delegate<string>? HVS_ErrorInfo_delegate { set; get; }

        public HPR_Status_Model(HVE_Result_Enum _Status)
        {
            Result_Status = _Status;
        }

        public HPR_Status_Model()
        {
        }

        /// <summary>
        /// 运行错误状态
        /// </summary>
        public HVE_Result_Enum Result_Status { set; get; } = HVE_Result_Enum.Run_OK;

        /// <summary>
        /// 运行错误详细信息
        /// </summary>
        private string _Result_Error_Info = string.Empty;

        public string Result_Error_Info
        {
            get { return _Result_Error_Info; }
            set
            {
                _Result_Error_Info = value;

                HVS_ErrorInfo_delegate?.Invoke(GetResult_Info());
            }
        }

        /// <summary>
        /// 结果值
        /// </summary>
        public T1? ResultVal { set; get; }

        /// <summary>
        /// 获得算法运行状态
        /// </summary>
        /// <returns></returns>
        public bool GetResult()
        {
            if (Result_Status == HVE_Result_Enum.Run_OK)
            {
                return true;
            }
            else
            {
                return false;
            }
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
    public class Calibration_Image_List_Model : IDisposable
    {
        /// <summary>
        /// 标定图像序号
        /// </summary>
        public int Image_No { set; get; } = 0;

        /// <summary>
        /// 主相机
        /// </summary>
        public Calibration_Image_Camera_Model Camera_0 { set; get; } = new Calibration_Image_Camera_Model();

        /// <summary>
        /// 副相机
        /// </summary>
        public Calibration_Image_Camera_Model Camera_1 { set; get; } = new Calibration_Image_Camera_Model();

        /// <summary>
        /// 标定板位置
        /// </summary>
        public Point_Model Calibration_Plate_Pos { set; get; } = new Point_Model();

        public Point_Model HandEye_Robot_Pos { set; get; } = new Point_Model();

        /// <summary>
        /// 相机控制类型
        /// </summary>
        public Camera_Connect_Control_Type_Enum Camera_No { set; get; } = Camera_Connect_Control_Type_Enum.Camera_0;

        //public void Set_HImage(HObject _Image)
        //{
        //    switch (Camera_No)
        //    {
        //        case Camera_Connect_Control_Type_Enum.双目相机:
        //            break;
        //        case Camera_Connect_Control_Type_Enum.Camera_0:
        //            Camera_0.Calibration_Image = _Image;
        //            break;
        //        case Camera_Connect_Control_Type_Enum.Camera_1:
        //            Camera_1.Calibration_Image = _Image;

        //            break;

        //    }
        //}

        //public void Set_Features_Results(HObject _Region, HObject _XLD)
        //{
        //    switch (Camera_No)
        //    {
        //        case Camera_Connect_Control_Type_Enum.双目相机:

        //            break;
        //        case Camera_Connect_Control_Type_Enum.Camera_0:

        //            Camera_0.Calibration_Region = _Region;
        //            Camera_0.Calibration_XLD = _XLD;

        //            break;
        //        case Camera_Connect_Control_Type_Enum.Camera_1:
        //            Camera_1.Calibration_Region = _Region;
        //            Camera_1.Calibration_XLD = _XLD;

        //            break;

        //    }
        //}

        public void Set_Parameter_Val(Calibration_Image_Camera_Model _Model)
        {
            switch (Camera_No)
            {
                case Camera_Connect_Control_Type_Enum.双目相机:

                    break;

                case Camera_Connect_Control_Type_Enum.Camera_0:

                    Camera_0 = _Model;

                    break;

                case Camera_Connect_Control_Type_Enum.Camera_1:
                    Camera_1 = _Model;

                    break;
            }
        }

        public void Dispose()
        {
            Camera_0?.Dispose();

            Camera_1?.Dispose();

            //GC.Collect();
            //GC.SuppressFinalize(this);
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Calibration_Plate_Pos_Model
    {
        /// <summary>
        /// 标定图像序号
        /// </summary>
        public int Calibration_No { set; get; } = 0;

        public Point_Model Plate_Point { set; get; } = new Point_Model();

        /// <summary>
        /// 标定状态
        /// </summary>
        public string Calibration_State { set; get; } = "";
    }

    [AddINotifyPropertyChangedInterface]
    public class Point_Model
    {
        public Point_Model()
        {

        }


        public Point_Model(double _X = 0, double _Y = 0, double _Z = 0, double _Rx = 0, double _Ry = 0, double _Rz = 0, Robot_Type_Enum Robot_Type = Robot_Type_Enum.通用)
        {
            switch (Robot_Type)
            {
                case Robot_Type_Enum.KUKA:
                    //设置机器人当前位置

                    X = _X;
                    Y = _Y;
                    Z = _Z;
                    Rx = _Rz;
                    Ry = _Ry;
                    Rz = _Rx;
                    HType = Halcon_Pose_Type_Enum.abg;

                    break;
                case Robot_Type_Enum.ABB:
                    //_Robot_Pos = new Point_Model() { X = double.Parse(_S.ACT_Point.X), Y = double.Parse(_S.ACT_Point.Y), Z = double.Parse(_S.ACT_Point.Z), Rx = double.Parse(_S.ACT_Point.Rx), Ry = double.Parse(_S.ACT_Point.Ry), Rz = double.Parse(_S.ACT_Point.Rz), HType = Halcon_Pose_Type_Enum.abg };
                    X = _X;
                    Y = _Y;
                    Z = _Z;
                    Rx = _Rx;
                    Ry = _Ry;
                    Rz = _Rz;
                    HType = Halcon_Pose_Type_Enum.abg;
                    break;
                case Robot_Type_Enum.川崎:
                    break;
                case Robot_Type_Enum.通用:
                    X = _X;
                    Y = _Y;
                    Z = _Z;
                    Rx = _Rx;
                    Ry = _Ry;
                    Rz = _Rz;
                    HType = Halcon_Pose_Type_Enum.gba;
                    break;

            }
        }


        /// <summary>
        /// 转换对应机器人旋转姿态
        /// </summary>
        /// <param name="_type"></param>
        public void Convert_Pose(Robot_Type_Enum _type)
        {
            HPose = new Point_Model(X, Y, Z, Rx, Ry, Rz, _type).HPose;
        }

        public override string ToString()
        {
            return $"坐标 X: {X:F4}, Y: {Y:F4}, Z: {Z:F4}, Rx: {Rx:F4}, Ry: {Ry:F4}, Rz: {Rz:F4}, 类型: {HType}";
        }

        public Point_Model(Point_Model _Point)
        {
            HPose = new HPose(_Point.HPose);
            HType = _Point.HType;
        }
        public Point_Model(HPose _Pose)
        {
            HPose = new HPose(_Pose);
        }

        private double _x;

        public double X
        {
            get { return _x; }
            set { _x = value; HPose[0] = value / 1000.000; }
        }
        private double _y;

        public double Y
        {
            get { return _y; }
            set { _y = value; HPose[1] = value / 1000.000; }
        }

        private double _z;

        public double Z
        {
            get { return _z; }
            set { _z = value; HPose[2] = value / 1000.000; }
        }

        private double _Rx;

        public double Rx
        {
            get { return _Rx; }
            set { _Rx = value; HPose[3] = value; }
        }

        private double _Ry;

        public double Ry
        {
            get { return _Ry; }
            set { _Ry = value; HPose[4] = value; }
        }

        private double _Rz;

        public double Rz
        {
            get { return _Rz; }
            set { _Rz = value; HPose[5] = value; }
        }



        /// <summary>
        /// 读取位置点类型
        /// </summary>
        private Halcon_Pose_Type_Enum _HType = Halcon_Pose_Type_Enum.gba;

        public Halcon_Pose_Type_Enum HType
        {
            get { return _HType; }
            set { _HType = value; HPose[6] = (int)value; }
        }




        private HPose _HPose = new (0, 0, 0, 0, 0, 0, "Rp+T", "gba", "point");

        public HPose HPose
        {
            get
            {
                return _HPose;
            }
            set
            {
                Set_Vale(value);
                _HPose = value;
            }
        }

        /// <summary>
        /// 设置位置显示方法
        /// </summary>
        /// <param name="_Pose"></param>
        private void Set_Vale(HPose _Pose)
        {
            if (_Pose.RawData.Length != 0)
            {
                if (_Pose.RawData.Length == 7)
                {
                    HType = (Halcon_Pose_Type_Enum)(int)_Pose[6];
                }
                X = _Pose[0] * 1000;
                Y = _Pose[1] * 1000;
                Z = _Pose[2] * 1000;
                Rx = _Pose[3];
                Ry = _Pose[4];
                Rz = _Pose[5];
            }
        }

        /// <summary>
        /// 获得对应机器人的位姿
        /// </summary>
        /// <param name="_Robot"></param>
        /// <returns></returns>
        public Point_Model Get_HPos(Robot_Type_Enum _Robot)
        {
            Point_Model _Pos = new();

            switch (_Robot)
            {
                case Robot_Type_Enum.KUKA:

                    _Pos = new Point_Model() { X = X, Y = Y, Z = Z, Rx = Rz, Ry = Ry, Rz = Rx };

                    break;

                case Robot_Type_Enum.ABB:

                    _Pos = new Point_Model() { X = X, Y = Y, Z = Z, Rx = Rx, Ry = Ry, Rz = Rz };

                    //需要四元数转换
                    break;

                case Robot_Type_Enum.川崎:

                    //_Pos.CreatePose(X / 1000, Y / 1000, Z / 1000, Rx, Ry, Rz, "Rp+T", "gba", "point");

                    break;

                case Robot_Type_Enum.通用:

                    _Pos = new Point_Model() { X = X, Y = Y, Z = Z, Rx = Rx, Ry = Ry, Rz = Rz };

                    break;
            }

            //HTuple _Pos=new ();

            return _Pos;
        }


        /// <summary>
        /// 设置点类型
        /// </summary>
        /// <param name="_Robot"></param>
        public void Set_HPos_Type(Robot_Type_Enum? _Robot)
        {
            switch (_Robot)
            {
                case Robot_Type_Enum.KUKA:
                    HType = Halcon_Pose_Type_Enum.abg;

                    break;
                case Robot_Type_Enum.ABB:
                    HType = Halcon_Pose_Type_Enum.abg;

                    break;
                case Robot_Type_Enum.川崎:


                    break;
                case Robot_Type_Enum.通用:
                    HType = Halcon_Pose_Type_Enum.gba;

                    break;

            }
        }



        /// <summary>
        /// 保存位姿以单位：m
        /// </summary>
        /// <param name="_File"></param>
        /// <param name="_name"></param>
        /// <exception cref="Exception"></exception>
        public void Pos_Save(string _File, string _name)
        {
            try
            {
                //窗口同步模型下运行保存
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Checked_SaveFile(_File, _name))
                    {
                        //HPose _Pos = new HPose(X/1000, Y/1000, Z / 1000, A, B, C, "Rp+T", "gba", "point");
                        HPose.WritePose(_File + _name + ".dat");
                    }
                });
            }
            catch (Exception _e)
            {
                throw new Exception(_name + "：位姿文件保存失败！原因：" + _e.Message);
            }
        }

        /// <summary>
        /// 检查保存文件是否存在？
        /// </summary>
        /// <returns></returns>
        private static bool Checked_SaveFile(string _File, string _name)
        {
            ////检查文件夹，创建
            if (!Directory.Exists(_File)) Directory.CreateDirectory(_File);

            //添加名称
            string _File_Address = _File + _name;

            if (File.Exists(_File_Address += ".dat"))
            {
                if (MessageBox.Show("位资文件：" + _name + " 已存在，是否覆盖？", "标定提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK) return false; return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 相机标定误差模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class HandEye_RMS_Max_Error_Model
    {
        public double RMS_Translational { set; get; } = 0;

        public double RMS_Rotational { set; get; } = 0;

        public double Maximum_Translational { set; get; } = 0;

        public double Maximum_Rotational { set; get; } = 0;

        public void Set_Data(HTuple _Data)
        {
            RMS_Translational = _Data.TupleSelect(0) * 1000;
            RMS_Rotational = _Data.TupleSelect(1);
            Maximum_Translational = _Data.TupleSelect(2) * 1000;
            Maximum_Rotational = _Data.TupleSelect(3);
        }
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
        public Calibration_Camera_Data_Results_Model(Calibration_Camera_Data_Results_Model _Results_Model)
        {
            Camera_Calib_Error = _Results_Model.Camera_Calib_Error;
            Camera_Result_Pama = _Results_Model.Camera_Result_Pama;
            Calibration_Name = _Results_Model.Calibration_Name;
        }

        public Calibration_Camera_Data_Results_Model()
        {
        }

        /// <summary>
        /// 相机标定流程状态
        /// </summary>
        public Camera_Calinration_Process_Enum Camera_Calinration_Process_Type { set; get; } = Camera_Calinration_Process_Enum.Uncalibrated;

        /// <summary>
        /// 相机结果保存文件夹
        /// </summary>
        public string Result_Fold_Address { set; get; } = Directory.GetCurrentDirectory() + "\\Calibration_File";

        /// <summary>
        /// 相机标定参数
        /// </summary>
        public Halcon_Camera_Calibration_Parameters_Model Camera_Result_Pama { set; get; } = new Halcon_Camera_Calibration_Parameters_Model();

        /// <summary>
        /// 标定文件者名称
        /// </summary>
        public string Calibration_Name { set; get; } = string.Empty;

        /// <summary>
        /// 保存相机内参文件地址
        /// </summary>
        private string Save_File_Address { set; get; } = string.Empty;

        /// <summary>
        /// 手眼校准成功后，将返回使用校正工具姿势的完整变换链的姿势误差
        /// </summary>
        public HandEye_RMS_Max_Error_Model HandEye_Tool_in_Pase_Pose_Corrected_Error { set; get; } = new HandEye_RMS_Max_Error_Model();

        /// <summary>
        /// 手眼校准成功后，将返回使用校正工具姿势的完整变换链的姿势误差。
        /// </summary>
        public HandEye_RMS_Max_Error_Model HandEye_Calib_Error_Corrected_Tool { set; get; } = new HandEye_RMS_Max_Error_Model();

        /// <summary>
        /// 校准标记中心反投影到摄像机图像的均方根误差（RMSE），通过使用校正工具姿势的姿势链
        /// </summary>
        public double Camera_Calib_Error_Corrected_Tool { set; get; } = 0;

        /// <summary>
        /// 摄像机系统优化后投影的均方根误差（RMSE）。通常情况下，在执行手眼校准（calibrate_hand_eye）后会查询此误差，在此过程中会对摄像机系统进行内部校准，但不会返回摄像机校准的误差
        /// </summary>
        public HandEye_RMS_Max_Error_Model HandEye_Calib_Error { set; get; } = new HandEye_RMS_Max_Error_Model();

        /// <summary>
        /// 以机器人基准坐标表示的机器人工具输入位置的标准平移偏差
        /// </summary>
        public double HandEye_Tool_Translation_Deviation { set; get; } = 0;

        /// <summary>
        /// 以机器人基准坐标表示的机器人工具输入位置的标准旋转偏差
        /// </summary>
        public double HandEye_Tool_Rotational_Deviation { set; get; } = 0;

        /// <summary>
        /// 标定结果像素误差:单位像素
        /// </summary>
        public double Camera_Calib_Error { set; get; } = 0;

        /// <summary>
        ///  使用校正工具姿势的完整转换链的姿态误差
        /// </summary>
        //public double Camera_Calib_Error_Corrected_Tool { set; get; } = 0;

        /// <summary>
        /// 机器人工具在相机坐标系中的姿态
        /// </summary>
        public Point_Model HandEye_Tool_in_Cam_Pos { set; get; } = new Point_Model();

        /// <summary>
        /// 相机坐标系中机器人工具的姿态的标准偏差
        /// </summary>
        public Point_Model HandEye_Tool_in_Cam_Pose_Deviations { set; get; } = new Point_Model();

        /// <summary>
        /// 相机在机器人基坐标中的姿态
        /// </summary>
        public Point_Model HandEye_Cam_In_Base_Pos { set; get; } = new Point_Model();

        public Point_Model HandEye_Obj_In_Base_Pose { set; get; } = new Point_Model();

        public Point_Model HandEye_Obj_In_Base_Pose_Deviations { set; get; } = new Point_Model();

        /// <summary>
        /// 检查保存文件是否存在？
        /// </summary>
        /// <returns></returns>
        public bool Checked_SaveFile()
        {
            ////检查文件夹，创建
            if (!Directory.Exists(Result_Fold_Address)) Directory.CreateDirectory(Result_Fold_Address);

            //添加名称
            Save_File_Address = Result_Fold_Address + "\\" + Calibration_Name;

            if (File.Exists(Save_File_Address += ".dat"))
            {
                if (MessageBox.Show("相机内参文件：" + Calibration_Name + " 已存在，是否覆盖？", "标定提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK) return false; return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 保存相机参数
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Save_Camera_Parameters()
        {
            Camera_Calinration_Process_Type.Throw(Calibration_Name + "：未进行手眼标定！").IfEquals(Camera_Calinration_Process_Enum.Uncalibrated);

            if (Camera_Result_Pama.HCamPar != null)
            {
                //窗口同步模式运行
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Checked_SaveFile())
                    {
                        Camera_Result_Pama.HCamPar.WriteCamPar(Save_File_Address);
                    }
                });
            }
            else
            {
                throw new Exception("未进行相机标定，无法保存！");
            }
        }

        /// <summary>
        /// 手眼标定结果保存
        /// </summary>
        /// <param name="_Camera_Enum"></param>
        /// <param name="_Results"></param>
        public void HandEye_Results_Save()
        {
            //保存相机内参
            Save_Camera_Parameters();
            //保存相机在工具坐标
            HandEye_Tool_in_Cam_Pos.Pos_Save(Result_Fold_Address, "HandEyeToolinCam_" + Calibration_Name);
        }
    }

    /// <summary>
    /// 创建模型类型显示属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_Model_Group_Model
    {
        /// <summary>
        /// 模型是否可读取
        /// </summary>
        public bool IsRead { set; get; } = false;

        /// <summary>
        /// 模型是否创建
        /// </summary>
        public bool IsEnable { set; get; } = true;

        /// <summary>
        /// 模型类型
        /// </summary>
        public Shape_Based_Model_Enum Shape_Based_Model { set; get; }
    }

    /// <summary>
    /// 模型文件参数属性
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_File_UI_Model
    {
        /// <summary>
        /// 模型文件id号
        /// </summary>
        public int File_ID { set; get; } = -1;

        /// <summary>
        /// 45区域是否可读属性
        /// </summary>
        public bool IsRead_F45 { set; get; } = false;

        /// <summary>
        /// 135区域是否可读属性
        /// </summary>
        public bool IsRead_F135 { set; get; } = false;

        /// <summary>
        /// 225区域是否可读属性
        /// </summary>
        public bool IsRead_F225 { set; get; } = false;

        /// <summary>
        /// 315区域是否可读属性
        /// </summary>
        public bool IsRead_F315 { set; get; } = false;
    }

    /// <summary>
    /// 选择模型序号显示详细信息
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Shape_FileFull_UI_Model
    {
        public string File_Name { set; get; } = string.Empty;
        public string File_Directory { set; get; } = Environment.CurrentDirectory + "\\ShapeModel";
    }



    /// <summary>
    /// 创建模板画画模型
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Vision_Create_Model_Drawing_Model
    {

        public Vision_Create_Model_Drawing_Model()
        {

        }


        /// <summary>
        ///绘图_类型
        /// </summary>
        public Drawing_Type_Enme Drawing_Type { set; get; } = Drawing_Type_Enme.Draw_None;

        /// <summary>
        /// 绘画数据
        /// </summary>
        public ObservableCollection<Point3D> Drawing_Data { set; get; } = new ObservableCollection<Point3D>();



        /// <summary>
        /// 集合对象
        /// </summary>
        public HXLDCont Drawing_XLD { set; get; } = new HXLDCont();


        public Enum? Craft_Type_Enum { set; get; }

        /// <summary>
        /// 数据计算xld类型存放
        /// </summary>
        public HXLDCont Model_XLD { set; get; } = new HXLDCont();


        /// <summary>
        /// XLD轮廓创建状态
        /// </summary>
        public XLD_Contours_Creation_Status Craft_XLd_Creation_Status { set; get; } = XLD_Contours_Creation_Status.None;
    }

    [AddINotifyPropertyChangedInterface]
    public class Calibration_Image_Camera_Model : IDisposable
    {
        public Calibration_Image_Camera_Model()
        {
        }

        /// <summary>
        /// 标定精度
        /// </summary>
        public double Calibration_Accuracy { set; get; } = 0;

        /// <summary>
        /// 标定图像
        /// </summary>
        public HObject? Calibration_Image { set; get; }

        /// <summary>
        /// 标定板特征
        /// </summary>
        public HObject Calibration_Region { set; get; } = new HObject();

        /// <summary>
        /// 标定板坐标系
        /// </summary>
        public HObject Calibration_XLD { set; get; } = new HObject();

        /// <summary>
        /// 标定模型
        /// </summary>
        public List<HObjectModel3D> Calibration_3D_Model { set; get; } = new List<HObjectModel3D>();

        //相机名称图像的
        public string Carme_Name { set; get; } = string.Empty;

        /// <summary>
        /// 标定状态
        /// </summary>
        public Camera_Calibration_Image_State_Enum Calibration_State { set; get; } = Camera_Calibration_Image_State_Enum.None;

        /// <summary>
        /// 清理模型内存
        /// </summary>
        public void Dispose()
        {
            Calibration_Image?.Dispose();
            Calibration_Region?.Dispose();
            Calibration_XLD?.Dispose();

            foreach (var _model in Calibration_3D_Model)
            {
                _model.ClearObjectModel3d();
                _model.Dispose();
            }
            Calibration_3D_Model.Clear();

            //GC.Collect();
            //GC.SuppressFinalize(this);
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Shape_Mode_File_Model 
    {
        public Shape_Mode_File_Model()
        {

        }


        /// <summary>
        /// 创建模型类型
        /// </summary>
        public Shape_Based_Model_Enum Shape_Model { set; get; }


        /// <summary>
        /// 创建模型类型所在区域
        /// </summary>
        public string Shape_Area { set; get; } = "";


        /// <summary>
        /// 创建模型所应用的工艺
        /// </summary>
        public Match_Model_Craft_Type_Enum Shape_Craft { set; get; }

        /// <summary>
        /// 路径点应用于机器人姿态
        /// </summary>
        public Robot_Type_Enum Shape_Robot_Type { set; get; }


        /// <summary>
        /// 创建模型句柄集合
        /// </summary>
        public List<HTuple> Shape_Handle_List { set; get; } = new ();


        /// <summary>
        /// 选择的模型句柄
        /// </summary>
        public HTuple? Selected_Shape_Handle { set; get; } 


        /// <summary>
        /// 创建模型xld句柄
        /// </summary>
        public List<HXLDCont> Shape_XLD_Handle_List { set; get; } = new ();

        /// <summary>
        /// 选择XLD对象模型
        /// </summary>
        public HXLDCont? Selected_Shape_XLD_Handle { set; get; } 


        /// <summary>
        /// 模型识别前图像校正图像
        /// </summary>
        public HImage Shape_Image_Rectified { set; get; } = new();

        /// <summary>
        ///  校正图像尺寸
        /// </summary>
        public int Shape_Image_Rectified_Width { set; get; } = 0;


        /// <summary>
        ///  校正图像尺寸
        /// </summary>
        public int Shape_Image_Rectified_Heigth { set; get; } = 0;


        /// <summary>
        /// 模型图像校正后世界坐标转换系数
        /// </summary>
        public double Shape_Image_Rectified_Ratio { set; get; } = 0;

        /// <summary>
        /// 匹配模型平面位置
        /// </summary>
        public Point_Model Shape_PlaneInBase_Pos { set; get; } = new ();

        /// <summary>
        /// 图像原点位置
        /// </summary>
        public Point_Model Shape_Model_2D_Origin { set; get; } = new ();

        /// <summary>
        /// 标定路径坐标列表
        /// </summary>
        public ObservableCollection<Point_Model> Shape_Calibration_PathInBase_List { set; get; } = new();



        /// <summary>
        /// 选择标定路径
        /// </summary>
        public Point_Model? Selected_Shape_Calibration_PathInBase { set; get; }

        /// <summary>
        /// 模型ID号
        /// </summary>
        public int ID { set; get; } = -1;

        /// <summary>
        /// 模型创建日期
        /// </summary>
        public string Creation_Date { set; get; } = string.Empty;



        //public void Dispose()
        //{
        //    foreach (var _Model in Shape_Handle_List)
        //    {
        //        _Model.Dispose();
        //    }
        //    foreach (var _Model in Shape_XLD_Handle_List)
        //    {
        //        _Model.Dispose();
        //    }
        //    Shape_Image_Rectified.Dispose();
         
        //    GC.SuppressFinalize(this);
        //}

    }



    /// <summary>
    /// 相机标定
    /// </summary>
    public enum Halocn_Camera_Calibration_Enum
    {
        /// <summary>
        /// 面扫相机_畸形式
        /// </summary>
        [Description("面扫描_普通式")]
        area_scan_division,

        /// <summary>
        /// 面扫相机_多项式
        /// </summary>
        [Description("面扫描_多项式")]
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
        [Description("圆形")]
        circle,

        [Description("正方形")]
        square
    }

    /// <summary>
    /// 中值滤波器边境处理类型枚举。
    /// </summary>
    public enum MedianImage_Margin_Enum
    {
        [Description("边缘反射")]
        mirrored,

        [Description("边缘循环延续")]
        cyclic,

        [Description("边缘延续")]
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
        ///
        [Description("一般形状模板")]
        shape_model,

        /// <summary>
        /// 线性变形匹配模板
        /// </summary>
        [Description("线性变形匹配模板")]
        planar_deformable_model,

        /// <summary>
        /// 局部可变形模板
        /// </summary>
        [Description("局部可变形模板")]
        local_deformable_model,

        /// <summary>
        /// 和比例缩放模板
        /// </summary>
        [Description("比例缩放模板")]
        Scale_model,
        /// <summary>
        /// 各向异性缩放的形状模型
        /// </summary>
        [Description("异性缩放的匹配模型")]
        Aniso_Model,
        /// <summary>
        /// 相关性匹配模板
        /// </summary>
        [Description("相关性匹配模板")]
        Ncc_Model,

        /// <summary>
        /// Halcon通用保存格式
        /// </summary>
        [Description("通用保存格式")]
        Halcon_DXF
    }

    /// <summary>
    /// 匹配模型焊接工艺轮廓
    /// </summary>
    public enum Match_Model_Craft_Type_Enum
    {
        [Description("请选择模型工艺！")]
        请选择模型工艺,
        [Description("焊接盆胆R角！")]
        焊接盆胆R角,
        [Description("焊接面板围边！")]
        焊接面板围边
    }





    public enum Model_2D_Origin_Type_Enum
    {
        [Description("图像原点")]
        Origin_Imag,
        [Description("当前位置")]
        Tool_In_Base,
        [Description("模型平面")]
        Plan_In_Base,
        [Description("标定路径")]
        Calin_PathInBase



    }


    /// <summary>
    /// 画画类型枚举
    /// </summary>
    public enum Drawing_Type_Enme
    {
        [Description("直线")]
        Draw_Lin,

        [Description("圆弧")]
        Draw_Cir,
        [Description("原点")]
        Draw_Origin,

        [Description("...")]
        Draw_None
    }


    /// <summary>
    /// XLD轮廓创建状态枚举
    /// </summary>
    public enum XLD_Contours_Creation_Status
    {
        [Description("未创建特征！")]
        None,
        [Description("创建特征成功！")]
        Creation_OK,

    }


    /// <summary>
    /// 盆胆R角焊接工艺
    /// </summary>
    public enum Sink_Basin_R_Welding
    {
        [Description("0：模型原点位置")]
        模型原点位置,
        [Description("1：R角中线轮廓")]
        R角中线轮廓,
        [Description("2：盆胆左侧线")]
        盆胆左侧线轮廓,
        [Description("3：盆胆左侧线")]
        盆胆右侧线轮廓,
    }


    public enum Sink_Board_R_Welding
    {
        [Description("0：模型原点位置")]
        模型原点位置,
        [Description("1：面板横直线轮廓")]
        面板横直线轮廓,
        [Description("2：面板圆弧轮廓")]
        面板圆弧轮廓,
        [Description("3：面板竖直线轮廓")]
        面板竖直线轮廓,
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
        [Description("区域 1")]
        F_45,

        [Description("区域 2")]
        F_135,

        [Description("区域 3")]
        F_225,

        [Description("区域 4")]
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
        [Description("自动")]
        auto,

        [Description("不预生成")]
        no_pregeneration,

        [Description("无")]
        none,

        [Description("生成点数减少高")]
        point_reduction_high,

        [Description("生成点数减少底")]
        point_reduction_low,

        [Description("生成点数减少中")]
        point_reduction_medium,

        [Description("预生成")]
        pregeneration
    }

    /// <summary>
    /// 参数metric确定在图像中识别模型的条件。
    /// </summary>
    public enum Metric_Enum
    {
        [Description("忽略颜色极性")]
        ignore_color_polarity,

        [Description("忽略全局极性")]
        ignore_global_polarity,

        [Description("忽略本地极性")]
        ignore_local_polarity,

        [Description("使用极性")]
        use_polarity
    }

    /// <summary>
    /// 亚像素精度枚举
    /// </summary>
    public enum Subpixel_Values_Enum
    {
        [Description("无")]
        none,

        [Description("插值")]
        interpolation,

        [Description("最小二乘法")]
        least_squares,

        [Description("最小二乘法高")]
        least_squares_high,

        [Description("最小二乘法非常高")]
        least_squares_very_high,
    }

    /// <summary>
    /// 获得采集图片方式
    /// </summary>
    public enum Get_Image_Model_Enum
    {
        相机采集,
        图像采集,
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
        标定图像获得相机模型错误,
        创建标定相机错误,
        获得标定结果失败,
        设置相机初始内参错误,
        获得相机内参参数错误,
        保存相机标定文件错误,
        取消覆盖保存相机标定文件,
        未进行相机标定无法保存,
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
        Draw,
        Path
    }

    /// <summary>
    /// 匹配模型类型枚举
    /// </summary>
    public enum Shape_HObject_Type_Enum
    {
        Shape_Handle,
        Shape_XLD,
        Shape_Image_Rectified,
        shape_Model_PlanePos
    }


    public enum Find_Shape_Results_State_Enum
    {
        [Description("模型等待匹配")]
        Match_None,
        [Description("模型匹配成功")]
        Match_Success,
        [Description("模型匹配中...")]
        Matching,
        [Description("模型匹配失败")]
        Match_Failed,
        [Description("模型匹配错误异常")]
        Match_Erroe
    }


    /// <summary>
    /// 窗口显示变量
    /// </summary>
    public enum Window_Show_Name_Enum
    {
        [Description("相机视角页面")]
        Live_Window,

        [Description("图像特征页面")]
        Features_Window,
        Features_3D_Results,
        Results_Window_1,
        Results_Window_2,
        Results_Window_3,
        Results_Window_4,
        Calibration_Window_1,
        Calibration_Window_2,
        Calibration_3D_Results,
        HandEye_Window_1,
        HandEye_Window_2,
        HandEye_Results_Window_1,
        HandEye_Results_Window_2,
        HandEye_3D_Results
    }

    /// <summary>
    /// 区域显示设置枚举
    /// </summary>
    public enum DisplaySetDraw_Enum
    {
        fill,
        margin
    }

    /// <summary>
    /// 手眼标定设备模型状态枚举
    /// </summary>
    public enum Image_Diver_Model_Enum
    {
        /// <summary>
        /// 在线模型
        /// </summary>
        Online,

        /// <summary>
        /// 本地模式
        /// </summary>
        Local
    }

    /// <summary>
    /// 相机内参标定状态，用来判断保存文件
    /// </summary>
    public enum Camera_Calinration_Process_Enum
    {
        /// <summary>
        /// 未标定
        /// </summary>
        Uncalibrated,

        /// <summary>
        /// 标定中状态
        /// </summary>
        Calibrationing,

        /// <summary>
        /// 标定成功
        /// </summary>
        Calibration_Successful
    }

    /// <summary>
    /// 图像标定检测状态
    /// </summary>
    public enum Camera_Calibration_Image_State_Enum
    {
        [Description("...")]
        None,

        [Description("图像加载...")]
        Image_Loading,

        [Description("图像检测中...")]
        Image_Detectioning,

        [Description("图像检测成功...")]
        Image_Successful,

        [Description("图像检测异常...")]
        Image_UnSuccessful,
    }

    /// <summary>
    /// 位置点旋转角度类型
    /// </summary>
    public enum Halcon_Pose_Type_Enum
    {
        /// <summary>
        /// "Rp+T"	"gba"	"point"
        /// </summary>
        [Description("X-Y-Z")]
        gba = 0,

        /// <summary>
        /// "Rp+T"	"abg"	"point"
        /// </summary>
        [Description("Z-Y-X")]
        abg = 2,
    }

    /// <summary>
    /// 手眼标定过程状态枚举
    /// </summary>
    public enum HandEye_Calibration_Type_Enum
    {
        Calibration_Start,
        Calibration_Progress,
        Calibration_End
    }

    /// <summary>
    /// 创建模型带你类型
    /// </summary>
    public enum Vision_Creation_Model_Pos_Enum
    {
        Camer_Pos,
        Model_Pos,
    }

    /// <summary>
    /// 视觉识别功能
    /// </summary>
    public enum Vision_Model_Enum
    {
        Calibration_New,
        Calibration_Text,
        Calibration_Add,
        Find_Model,
        Vision_Ini_Data,
        HandEye_Calib_Date,
        Vision_Creation_Model,
    }



    public enum Robot_Type_Enum
    {
        [Description("KUKA -->(Z-Y''-X'')")]
        KUKA,

        [Description("ABB -->(Z-Y'-X'')")]
        ABB,
        [Description("川崎 -->(X-Y'-Z'')")]
        川崎,
        [Description("通用 -->(X-Y'-Z'')")]

        通用
    }




}