using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public static HObject Local_To_Halcon_Image(string _local)
        {

            //新建空属性
            HOperatorSet.GenEmptyObj(out HObject ho_Image);

            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, _local);

            return ho_Image;


        }





        /// <summary>
        /// 投影映射XLD对象到图像控件位置,并返回XLD对象
        /// </summary>
        /// <param name="_ModelXld"></param>
        /// <param name="_HomMat2D"></param>
        /// <param name="_Window"></param>
        /// <returns></returns>
        public HObject ProjectiveTrans_Xld(Find_Model_Enum _Find_Enum, HTuple _ModelXld, HTuple _HomMat2D, HWindow _Window)
        {


            HObject _ModelConect = new HObject();


            //根据匹配模型类型 读取模板内的xld对象
            switch (_Find_Enum)
            {
                case Find_Model_Enum _Enum when (_Enum == Find_Model_Enum.Shape_Model) || (_Enum == Find_Model_Enum.Scale_Model):
                    HOperatorSet.GetShapeModelContours(out _ModelConect, _ModelXld, 1);

                    break;

                case Find_Model_Enum.Shape_Model | Find_Model_Enum.Scale_Model:
                    break;

                case Find_Model_Enum _Enum when (_Enum == Find_Model_Enum.Planar_Deformable_Model) || (_Enum == Find_Model_Enum.Local_Deformable_Model):

                    HOperatorSet.GetDeformableModelContours(out _ModelConect, _ModelXld, 1);

                    break;

            }

            //将xld对象矩阵映射到图像中
            HOperatorSet.ProjectiveTransContourXld(_ModelConect, out HObject _ContoursProjTrans, _HomMat2D);

            //显示到对应的控件窗口
            HOperatorSet.DispObj(_ContoursProjTrans, _Window);


            return _ContoursProjTrans;
        }


        /// <summary>
        /// 读取匹配模型文件
        /// </summary>
        /// <param name="_Read_Enum"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public HTuple Read_ModelsXLD_File(Find_Model_Enum _Read_Enum, string _Path)
        {

            HTuple _ModelID = new HTuple();

            //根据匹配模型类型 读取模板内的xld对象
            switch (_Read_Enum)
            {
                case Find_Model_Enum _Enum when (_Enum == Find_Model_Enum.Shape_Model) || (_Enum == Find_Model_Enum.Scale_Model):

                    HOperatorSet.ReadShapeModel(_Path, out _ModelID);
                    break;

                case Find_Model_Enum.Shape_Model | Find_Model_Enum.Scale_Model:
                    break;

                case Find_Model_Enum _Enum when (_Enum == Find_Model_Enum.Planar_Deformable_Model) || (_Enum == Find_Model_Enum.Local_Deformable_Model):

                    HOperatorSet.ReadDeformableModel(_Path, out _ModelID);
                    break;

            }

            return _ModelID;


        }




        /// <summary>
        ///创建匹配模型保存文件
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="_Save_Enum"></param>
        /// <param name="_Create_Model"></param>
        /// <param name="_ModelsXLD"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public HTuple ShapeModel_SaveFile<T1>(Find_Model_Enum _Save_Enum, T1 _Create_Model, HObject _ModelsXLD, string _Path)
        {

            lock (_Create_Model)
            {


            HTuple _ModelID = new HTuple();
            _ModelID.Dispose();
            switch (_Save_Enum)
            {
                case Find_Model_Enum _Enum when _Enum == Find_Model_Enum.Shape_Model:
                    if (_Create_Model is Halcon_Create_Shape_ModelXld)
                    {
                        Halcon_Create_Shape_ModelXld Create_Shap = _Create_Model as Halcon_Create_Shape_ModelXld;

                        //创建模型
                        HOperatorSet.CreateShapeModelXld(_ModelsXLD, Create_Shap.NumLevels, (new HTuple(Create_Shap.AngleStart)).TupleRad()
           , (new HTuple(Create_Shap.AngleExtent)).TupleRad(), Create_Shap.AngleStep, Create_Shap.Optimization.ToString(), Create_Shap.Metric.ToString(),
           Create_Shap.MinContrast, out _ModelID);

                        //写入模型文件
                        HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    }
                    break;
                case Find_Model_Enum _Enum when _Enum == Find_Model_Enum.Scale_Model:

                    if (_Create_Model is Halcon_Create_Planar_Uncalib_Deformable_ModelXld)
                    {

                        Halcon_Create_Planar_Uncalib_Deformable_ModelXld Create_Planar = _Create_Model as Halcon_Create_Planar_Uncalib_Deformable_ModelXld;
                        //创建模型
                        HOperatorSet.CreatePlanarUncalibDeformableModelXld(_ModelsXLD, Create_Planar.NumLevels, (new HTuple(Create_Planar.AngleStart)).TupleRad()
                                                                                                    , (new HTuple(Create_Planar.AngleExtent)).TupleRad(), Create_Planar.AngleStep, Create_Planar.ScaleRMin, new HTuple(), Create_Planar.ScaleRStep, Create_Planar.ScaleCMin, new HTuple(),
                                                                                                     Create_Planar.ScaleCStep, Create_Planar.Optimization.ToString(), Create_Planar.Metric.ToString(), Create_Planar.MinContrast, new HTuple(), new HTuple(),
                                                                                                    out _ModelID);
                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(_ModelID, _Path);

                    }

                    break;
                case Find_Model_Enum _Enum when _Enum == Find_Model_Enum.Planar_Deformable_Model:

                    if (_Create_Model is Halcon_Create_Planar_Uncalib_Deformable_ModelXld)
                    {

                        Halcon_Create_Planar_Uncalib_Deformable_ModelXld Create_Local = _Create_Model as Halcon_Create_Planar_Uncalib_Deformable_ModelXld;
                        //创建模型
                        HOperatorSet.CreateLocalDeformableModelXld(_ModelsXLD, Create_Local.NumLevels, (new HTuple(Create_Local.AngleStart)).TupleRad()
                                                                                                    , (new HTuple(Create_Local.AngleExtent)).TupleRad(), Create_Local.AngleStep, Create_Local.ScaleRMin, new HTuple(), Create_Local.ScaleRStep, Create_Local.ScaleCMin, new HTuple(),
                                                                                                   Create_Local.ScaleCStep, Create_Local.Optimization.ToString(), Create_Local.Metric.ToString(), Create_Local.MinContrast, new HTuple(), new HTuple(),
                                                                                                   out _ModelID);

                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(_ModelID, _Path);
                    }
                    break;
                case Find_Model_Enum _Enum when _Enum == Find_Model_Enum.Local_Deformable_Model:
                    if (_Create_Model is Halcon_Create_Scaled_Shape_ModelXld)
                    {
                        Halcon_Create_Scaled_Shape_ModelXld Create_Scaled = _Create_Model as Halcon_Create_Scaled_Shape_ModelXld;

                        //创建模型
                        HOperatorSet.CreateScaledShapeModelXld(_ModelsXLD, Create_Scaled.NumLevels, (new HTuple(Create_Scaled.AngleStart)).TupleRad()
                                                                                                     , (new HTuple(Create_Scaled.AngleExtent)).TupleRad(), Create_Scaled.AngleStep, Create_Scaled.ScaleMin, Create_Scaled.ScaleMax, Create_Scaled.ScaleStep, Create_Scaled.Optimization.ToString(), Create_Scaled.Metric.ToString(),
                                                                                                    Create_Scaled.MinContrast, out _ModelID);

                        //保存模型文件
                        HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    }

                    break;

            }

            return _ModelID;

            }
        }
   


        /// <summary>
        /// 查找匹配模型方法
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="_Find_Enum"></param>
        /// <param name="_Image"></param>
        /// <param name="_ModelXld"></param>
        /// <param name="_Find_Property"></param>
        /// <returns></returns>
        public T2 Find_Deformable_Model<T1, T2>(Find_Model_Enum _Find_Enum, HObject _Image, HTuple _ModelXld, T1 _Find_Property)
        {




            lock (_Find_Property)
            {

                HTuple hv_row = new HTuple();
                HTuple hv_column = new HTuple();
                HTuple hv_angle = new HTuple();
                HTuple hv_score = new HTuple();
                HTuple hv_HomMat2D = new HTuple();
                DateTime RunTime = DateTime.Now;



                switch (_Find_Enum)
                {
                    case Find_Model_Enum.Shape_Model:


                        if (_Find_Property is Halcon_Find_Shape_ModelXld)
                        {

                            Halcon_Find_Shape_ModelXld Find_Shape = _Find_Property as Halcon_Find_Shape_ModelXld;


                            HOperatorSet.FindShapeModel(_Image, _ModelXld, (new HTuple(Find_Shape.AngleStart)).TupleRad(), (new HTuple(Find_Shape.AngleExtent)).TupleRad(), Find_Shape.MinScore,
                            Find_Shape.NumMatches, Find_Shape.MaxOverlap, Find_Shape.SubPixel.ToString(), Find_Shape.NumLevels, Find_Shape.Greediness, out hv_row, out hv_column, out hv_angle, out hv_score);


                            return (T2)(object)new Halcon_Find_Shape_Out_Parameter() { Row = hv_row.D, Column = hv_column.D, Angle = hv_angle.D, Score = hv_score.D, Find_Time = (DateTime.Now - RunTime).Milliseconds };

                        }

                        break;

                    case Find_Model_Enum.Planar_Deformable_Model:

                        if (_Find_Property is Halcon_Find_Deformable_model)
                        {

                            Halcon_Find_Deformable_model Find_Planar = _Find_Property as Halcon_Find_Deformable_model;


                            HOperatorSet.FindPlanarUncalibDeformableModel(_Image, _ModelXld,
                                                                                                       (new HTuple(Find_Planar.AngleStart)).TupleRad(), (new HTuple(Find_Planar.AngleExtent)).TupleRad(), Find_Planar.ScaleRMin, Find_Planar.ScaleRMax, Find_Planar.ScaleCMin, Find_Planar.ScaleCMax, Find_Planar.MinScore,
                                                                                                       Find_Planar.NumMatches, Find_Planar.MaxOverlap, Find_Planar.NumLevels, Find_Planar.Greediness, "subpixel", "least_squares", out hv_HomMat2D, out hv_score);


                            return (T2)(object)new Halcon_Find_Deformable_Out_Parameter() { HomMat2D = hv_HomMat2D, Score = hv_score.D, Find_Time = (DateTime.Now - RunTime).Milliseconds };

                        }
                        break;
                    case Find_Model_Enum.Local_Deformable_Model:
                        break;
                    case Find_Model_Enum.Scale_Model:
                        break;

                }


                return default;




            }

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
    /// 一般形状匹配查找模型结果参数
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

    }

    /// <summary>
    /// 可变形形状匹配查找模型结果参数
    /// </summary>
    public class Halcon_Find_Deformable_Out_Parameter
    {

        /// <summary>
        /// 模型和找到的实例相似值
        /// </summary>
        public double Score { set; get; } = 0;
        /// <summary>
        /// 找到的模型实例的分数
        /// </summary>
        public HTuple HomMat2D { set; get; } = new HTuple();


        /// <summary>
        /// 查找耗时
        /// </summary>
        public int Find_Time { set; get; } = 0;
    }

    /// <summary>
    ///  一般形状匹配查找模型参数
    /// </summary>
    public class Halcon_Find_Shape_ModelXld
    {

        /// <summary>
        /// 模型的最小旋转。默认值：-0.39,建议值： -3.14， -1.57， -0.79， -0.39， -0.20， 0.0
        /// </summary>
        public double AngleStart { set; get; } = 0;
        /// <summary>
        /// 旋转角度的范围。默认值：0.79,建议值：6.29、3.14、1.57、0.79、0.39、0.0
        /// </summary>
        public double AngleExtent { set; get; } = 360;
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
        /// 亚像素精度（如果不等于）“无”.默认值： “least_squares”
        /// </summary>
        public Subpixel_Values_Enum SubPixel { set; get; } = Subpixel_Values_Enum.least_squares;
        /// <summary>
        /// 匹配中使用的金字塔级别数（如果|，则使用的最低金字塔级别numLevels|= 2）。默认值：0
        /// </summary>
        public int NumLevels { set; get; } = 3;
        /// <summary>
        /// 搜索启发式的“贪婪”（0：安全但慢;1：快但可能会错过匹配）。默认值：0.9
        /// </summary>
        public double Greediness { set; get; } = 0.9;

    }

    /// <summary>
    /// 可变形形状匹配查找模型参数
    /// </summary>
    public class Halcon_Find_Deformable_model
    {
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
    }


    /// <summary>
    /// 可变形形状匹配创建模型参数
    /// </summary>
    public class Halcon_Create_Planar_Uncalib_Deformable_ModelXld
    {
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

}
