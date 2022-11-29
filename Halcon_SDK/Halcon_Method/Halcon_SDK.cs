using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Windows.Point;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using Halcon_SDK_DLL.Model;
using System.Xml.Linq;
using System.Threading;
using System.Runtime.InteropServices;

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
        /// 保存标定矩阵坐标方法
        /// </summary>
        /// <param name="_HomMat2D">需要保存矩阵对象</param>
        /// <param name="_Name">保存名字</param>
        /// <param name="_Path">保存地址</param>
        public static void Save_Mat2d_Method(HTuple _HomMat2D,string _Name ,string _Path)
        {
            HTuple _HomMatID = new HTuple();
            HTuple _FileHandle = new HTuple();


            //矩阵二进制化
            HOperatorSet.SerializeHomMat2d(_HomMat2D, out _HomMatID);


            //打开文件
            HOperatorSet.OpenFile(_Path+_Name + ".mat", "output_binary", out _FileHandle);

            //写入矩阵变量
            HOperatorSet.FwriteSerializedItem(_FileHandle, _HomMatID);
            //关闭文件
            HOperatorSet.CloseFile(_FileHandle);


        }


        /// <summary>
        /// 计算矩阵， 放回结果计算误差方法
        /// </summary>
        /// <param name="Calibration"></param>
        /// <param name="Robot"></param>
        /// <returns></returns>
        public static Point Calibration_Results_Compute(List<Point> Calibration, List<Point> Robot, ref HTuple HomMat2D)
        {
            //初始化坐标属性
            HTuple Calibration_RowLine = new HTuple();
            HTuple Calibration_ColLine = new HTuple();
            HTuple Robot_RowLine = new HTuple();
            HTuple Robot_ColLine = new HTuple();
           
            HTuple _Qx = new HTuple();
            HTuple _Qy = new HTuple();
            List<double> _Error_List_X = new List<double>();
            List<double> _Error_List_Y = new List<double>();

            //将点转化为Halcon组
            foreach (var _List in Calibration)
            {


                Calibration_ColLine = Calibration_ColLine.TupleConcat(_List.Y);

                Calibration_RowLine = Calibration_RowLine.TupleConcat(_List.X);

            }
            //将点转化为Halcon组
            foreach (var _List in Robot)
            {
                Robot_ColLine = Robot_ColLine.TupleConcat(_List.Y);     

                Robot_RowLine = Robot_RowLine.TupleConcat(_List.X);


            }

            //根据位置点组计算矩阵坐标
            HOperatorSet.VectorToHomMat2d(Calibration_RowLine, Calibration_ColLine, Robot_RowLine, Robot_ColLine,
      out HomMat2D);


            //转换视觉图像结果
            for (int i = 0; i < Calibration.Count; i++)
            {
                HOperatorSet.AffineTransPoint2d(HomMat2D, Calibration[i].X, Calibration[i].Y, out _Qx, out _Qy);

                double _Ex = Robot_RowLine[i].D - _Qx.D;
                double _Ey = Robot_ColLine[i].D - _Qy.D;

                _Error_List_X.Add(_Ex);
                _Error_List_Y.Add(_Ey);

            }



            //计算结果组偏差
            double   Calibration_Error_X_UI =double.Parse(Specimen_Error(_Error_List_X));
           double   Calibration_Error_Y_UI =double .Parse(  Specimen_Error(_Error_List_Y));


            return new Point(Calibration_Error_X_UI, Calibration_Error_Y_UI);
        }


        /// <summary>
        /// 计算一组样本标准差值
        /// </summary>
        /// <param name="_EList"></param>
        /// <returns></returns>
        public static  string Specimen_Error(List<double  > _EList)
        {
            double  xSum = 0 ;//样本总和
            double  xAvg=0;//样本平均值
            double  sSum =0 ;//方差的分子
                            
             //得到样本数量，分母
            foreach (var _ALsit in _EList)
            {
                xSum += _ALsit;
            }


             //计算得到样本平均值
            xAvg = xSum / _EList.Count;


            foreach (var _SList in _EList)
            {
                sSum += ((_SList - xAvg) * (_SList - xAvg));
            }


            return Math.Round( Math.Sqrt((sSum / (_EList.Count - 1))),3) .ToString();//样本标准差

         //double    STDP = Convert.ToDouble(Math.Sqrt((sSum / _EList.Count)).ToString());//总体标准差





    
        }






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
        public HObject Local_To_Halcon_Image(string _local)
        {

            //新建空属性
            HOperatorSet.GenEmptyObj(out HObject ho_Image);

            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, _local);

            return ho_Image;


        }

        /// <summary>
        /// 查找标定图像中的模型位置
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_Input_Image"></param>
        /// <param name="_Filtering_Model"></param>
        /// <param name="_Calibration_Data"></param>
        /// <returns></returns>
        public List<Point> Find_Calibration(HWindow _Window,HObject _Input_Image ,int _Filtering_Model, Halcon_Find_Calibration_Model _Calibration_Data,bool _Show_Split)
        {

            HObject _Image=new HObject ();
            HObject _Image1, _Image2, _Image3, _Image3_1, _Image4, _Image5, _Image6, _Image7, _Image8;
            HTuple _Area, _Row, _Column;





            //
            switch (_Filtering_Model)
            {
                case 0:

                    //进行图像平均平滑
                    HOperatorSet.MedianRect(_Input_Image, out _Image, _Calibration_Data.MaskWidth, _Calibration_Data.MaskHeight);


                    break;
                case 1:
                    //进行图像中值滤波器平滑
                    HOperatorSet.MedianImage(_Input_Image, out _Image, _Calibration_Data.MaskType_Model.ToString(), _Calibration_Data.Radius, _Calibration_Data.Margin_Model.ToString());

                    break;
                case 2:

                    //进行图像矩形掩码的中值滤波器
                    HOperatorSet.MedianRect(_Input_Image, out _Image, _Calibration_Data.MaskWidth, _Calibration_Data.MaskWidth);

                    break;


            }



            //图像最大灰度值分布在值范围0到255 中
            HOperatorSet.ScaleImageMax(_Image, out _Image1);




            //增强图像的对比度
            HOperatorSet.Emphasize(_Image1, out _Image2, _Calibration_Data.Emphasize_MaskWidth, _Calibration_Data.Emphasize_MaskHeight, _Calibration_Data.Factor);


            // 使用全局阈值分割图像
            HOperatorSet.Threshold(_Image1, out _Image3, _Calibration_Data.MinGray, _Calibration_Data.MaxGray);

            ////区域开运算消除边缘
            HOperatorSet.OpeningCircle(_Image3, out _Image3_1, 5);

            //计算区域中连接的组件
            HOperatorSet.Connection(_Image3_1, out _Image4);



            //填补区域的漏洞
            HOperatorSet.FillUp(_Image4, out _Image5);


            //形状特征选择圆形和圆度筛选区域
            HOperatorSet.SelectShape(_Image5, out _Image6, (new HTuple("area")).TupleConcat("circularity"), "and", (new HTuple(_Calibration_Data.Min_Area)).TupleConcat(0.9), (new HTuple(_Calibration_Data.Max_Area)).TupleConcat(1));

            //区域开运算消除边缘
            HOperatorSet.OpeningCircle(_Image6, out _Image7, 2);


            // 根据区域的相对位置对区域进行排序。
            HOperatorSet.SortRegion(_Image7, out _Image8, "character", "true", "row");


            //计算区域中心
            HOperatorSet.AreaCenter(_Image8, out _Area, out _Row, out _Column);


            //计算识别特征中心十字形的 XLD 轮廓
            HOperatorSet.GenCrossContourXld(out HObject _Cross, _Row, _Column, 100, (new HTuple(45)).TupleRad());




            //生产xld到窗口控件
            HOperatorSet.DispObj(_Cross, _Window);


            if (_Show_Split)
            {
            //生产xld到窗口控件
            HOperatorSet.DispObj(_Image3, _Window);

            }



            //区域显示边框
            HOperatorSet.SetDraw(_Window, "margin");
            HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());

            //生产xld到窗口控件
            HOperatorSet.DispObj(_Image7, _Window);

            HOperatorSet.SetColor(_Window, nameof(KnownColor.Red).ToLower());

            //区域显示边框
            HOperatorSet.SetDraw(_Window, "fill");


            //控件显示识别特征数量
            int _Number=  _Image8.CountObj();

            //识别特征坐标存储列表
            List<Point> _Calibration_Point=new List<Point> ();

            for (int i = 1; i < _Number + 1; i++)
            {
                double _X = Math.Round(_Row.TupleSelect(i - 1).D, 3);
                double _Y = Math.Round(_Column.TupleSelect(i - 1).D, 3);

                _Calibration_Point.Add(new Point(_X, _Y));

                //控件窗口显示识别信息
                HOperatorSet.DispText(_Window, "图号: " + i + " X:" + _X + " Y: " + _Y, "image", _X + 100, _Y - 100, "black", "box", "true");


            }


            return _Calibration_Point;


        }


        public static bool Get_ModelXld( ref HObject ho_ModelContours , Shape_Based_Model_Enum _Model, HTuple _ModelXld_ID,int _Xld_Number, HWindow _Window)
        {


            switch (_Model)
            {
                case Shape_Based_Model_Enum.shape_model & Shape_Based_Model_Enum.Scale_model:

                    HOperatorSet.GetShapeModelContours(out ho_ModelContours, _ModelXld_ID, _Xld_Number);


                    break;
                case Shape_Based_Model_Enum.planar_deformable_model | Shape_Based_Model_Enum.local_deformable_model:

                    HOperatorSet.GetDeformableModelContours(out ho_ModelContours, _ModelXld_ID, _Xld_Number);


                    break;
             
            }


            _Window.ClearWindow();
            _Window.DispObj(ho_ModelContours);

            return true;




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
        public static  HTuple Read_ModelsXLD_File(Shape_Based_Model_Enum _Read_Enum, string _Path)
        {


     
      
                //读取模型文件




            HTuple _ModelID = new HTuple();









            switch (_Read_Enum)
            {
                case Shape_Based_Model_Enum.shape_model & Shape_Based_Model_Enum.Scale_model:

                    HOperatorSet.ReadShapeModel(_Path, out _ModelID);


                    break;
                case Shape_Based_Model_Enum.planar_deformable_model | Shape_Based_Model_Enum.local_deformable_model:

                    HOperatorSet.ReadDeformableModel(_Path, out _ModelID);


                    break;

            }







            //根据匹配模型类型 读取模板内的xld对象
            //switch (_Read_Enum)
            //{
            //    case shape_model _Enum when (_Enum == Shape_Based_Model_Enum.shape_model) || (_Enum == Find_Model_Enum.Scale_Model):

            //        HOperatorSet.ReadShapeModel(_Path, out _ModelID);
            //        break;

            //    case Find_Model_Enum.Shape_Model | Find_Model_Enum.Scale_Model:
            //        break;

            //    case Shape_Based_Model_Enum _Enum when (_Enum == Find_Model_Enum.Planar_Deformable_Model) || (_Enum == Find_Model_Enum.Local_Deformable_Model):

            //        HOperatorSet.ReadDeformableModel(_Path, out _ModelID);
            //        break;

            //}

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
        public static void   ShapeModel_SaveFile( ref HTuple _ModelID, Create_Shape_Based_ModelXld _Create_Model, HObject _ModelsXLD, string _Path)
        {

            //开启线保存匹配模型文件
            //new Thread(new ThreadStart(new Action(() =>
            //{


   

            //lock (_Create_Model)
            //{



                    _ModelID.Dispose();


                switch (_Create_Model.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:


                        //创建模型
                        HOperatorSet.CreateShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
           , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
           _Create_Model.MinContrast, out _ModelID);

                        //写入模型文件
                        HOperatorSet.WriteShapeModel(_ModelID, _Path);



                        break;
                    case Shape_Based_Model_Enum.planar_deformable_model:

                        //创建模型
                        HOperatorSet.CreatePlanarUncalibDeformableModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                                    , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleRMin, new HTuple(), _Create_Model.ScaleRStep, _Create_Model.ScaleCMin, new HTuple(),
                                                                                                     _Create_Model.ScaleCStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(), _Create_Model.MinContrast, new HTuple(), new HTuple(),
                                                                                                    out _ModelID);
                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(_ModelID, _Path);


                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:

                        HOperatorSet.CreateLocalDeformableModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                      , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleRMin, new HTuple(), _Create_Model.ScaleRStep, _Create_Model.ScaleCMin, new HTuple(),
                                                                                     _Create_Model.ScaleCStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(), _Create_Model.MinContrast, new HTuple(), new HTuple(),
                                                                                     out _ModelID);

                        //保存模型文件
                        HOperatorSet.WriteDeformableModel(_ModelID, _Path);


                        break;
                    case Shape_Based_Model_Enum.Scale_model:

                        //创建模型
                        HOperatorSet.CreateScaledShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                                     , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleMin, _Create_Model.ScaleMax, _Create_Model.ScaleStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
                                                                                                    _Create_Model.MinContrast, out _ModelID);

                        //保存模型文件
                        HOperatorSet.WriteShapeModel(_ModelID, _Path);

                        break;
               
                }




      

             

                //}




            //})))
            //{ IsBackground = true, Name = "Create_Shape_Thread" }.Start();

              

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
        public Halcon_Find_Shape_Out_Parameter Find_Deformable_Model(HWindow _HWindow, HObject _Image, HTuple _ModelXld, Find_Shape_Based_ModelXld _Find_Property)
        {




            lock (_Find_Property)
            {

                HTuple hv_row = new HTuple();
                HTuple hv_column = new HTuple();
                HTuple hv_angle = new HTuple();
                HTuple hv_score = new HTuple();
                HTuple hv_HomMat2D = new HTuple();
                DateTime RunTime = DateTime.Now;
                HObject _Image1 = new HObject();
                HObject _Image2 = new HObject();
                HObject _Image3 = new HObject();
                HObject _Image4 = new HObject();
                HObject _Image5 = new HObject();
                HObject _Image6 = new HObject();
                HObject _Image7 = new HObject();
                HObject _Image8 = new HObject();
                HObject _Image9 = new HObject();
                HObject _Image10 = new HObject();
                Halcon_Find_Shape_Out_Parameter _Find_Out = new Halcon_Find_Shape_Out_Parameter();



                switch (_Find_Property.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:

                        HOperatorSet.FindShapeModel(_Image, _ModelXld, (new HTuple(_Find_Property.AngleStart)).TupleRad(), (new HTuple(_Find_Property.AngleExtent)).TupleRad(), _Find_Property.MinScore,
                        _Find_Property.NumMatches, _Find_Property.MaxOverlap, _Find_Property.SubPixel.ToString(), _Find_Property.NumLevels, _Find_Property.Greediness, out hv_row, out hv_column, out hv_angle, out hv_score);


                         _Find_Out=      new Halcon_Find_Shape_Out_Parameter() { Row = hv_row.D, Column = hv_column.D, Angle = hv_angle.D, Score = hv_score.D, Find_Time = (DateTime.Now - RunTime).Milliseconds };

                        break;
                    case Shape_Based_Model_Enum.planar_deformable_model:

                        if (_Find_Property.ScaleImageMax_Enable)
                        {
                        //图像最大灰度值分布在值范围0到255 中
                        HOperatorSet.ScaleImageMax(_Image, out _Image1);
                            if (_Find_Property.ScaleImageMax_Disp)
                            {
                                HOperatorSet.DispObj(_Image1, _HWindow);
                            }
                        }


                        if (_Find_Property.Emphasize_Enable)
                        {
                        //增强图像的对比度
                        HOperatorSet.Emphasize(_Image1, out _Image2, _Find_Property.Emphasize_MaskWidth, _Find_Property.Emphasize_MaskHeight, _Find_Property.Emphasize_Factor);
                            if (_Find_Property.Emphasize_Disp)
                            {
                                HOperatorSet.DispObj(_Image2, _HWindow);
                            }

                        }


                        HOperatorSet.FindPlanarUncalibDeformableModel(_Image, _ModelXld,
                                                                                                   (new HTuple(_Find_Property.AngleStart)).TupleRad(), (new HTuple(_Find_Property.AngleExtent)).TupleRad(), _Find_Property.ScaleRMin, _Find_Property.ScaleRMax, _Find_Property.ScaleCMin, _Find_Property.ScaleCMax, _Find_Property.MinScore,
                                                                                                   _Find_Property.NumMatches, _Find_Property.MaxOverlap, _Find_Property.NumLevels, _Find_Property.Greediness, "subpixel", "least_squares", out hv_HomMat2D, out hv_score);

                        _Find_Out=new Halcon_Find_Shape_Out_Parameter() { HomMat2D = hv_HomMat2D, Score = hv_score.D, Find_Time = (DateTime.Now - RunTime).Milliseconds };


                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:



                        break;
                    case Shape_Based_Model_Enum.Scale_model:



                        break;
                }

                return _Find_Out;





            }

        }







    }




}
