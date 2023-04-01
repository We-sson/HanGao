using Halcon_SDK_DLL.Model;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;


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
        /// 样品图片保存后序号
        /// </summary>
        private static int Sample_Save_Image_Number { set; get; } = 1;



        /// <summary>
        /// 保存图像到当前文件
        /// </summary>
        /// <param name="_Image"></param>
        /// <returns></returns>
        public static bool Save_Image(HObject _Image)
        {
            try
            {

                string _Path = Environment.CurrentDirectory + "\\Sample_Image";
                string _Name = "";

                //检查存放文件目录
                if (!Directory.Exists(_Path))
                {
                    //创建文件夹
                    Directory.CreateDirectory(_Path);

                }

                DirectoryInfo root = new DirectoryInfo(_Path);
                FileInfo Re;
                do
                {
                    _Name = DateTime.Today.ToLongDateString() + "_" + (Sample_Save_Image_Number += 1).ToString();

                    Re = root.GetFiles().Where(F => F.Name.Contains(_Name)).FirstOrDefault();


                } while (Re != null);








                HOperatorSet.WriteImage(_Image, "tiff", 0, _Path + "\\" + _Name);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;

            }
        }


        /// <summary>
        /// 保存标定矩阵坐标方法
        /// </summary>
        /// <param name="_HomMat2D">需要保存矩阵对象</param>
        /// <param name="_Name">保存名字</param>
        /// <param name="_Path">保存地址</param>
        public static void Save_Mat2d_Method(HTuple _HomMat2D, string _Path)
        {
            HTuple _HomMatID = new HTuple();
            HTuple _FileHandle = new HTuple();


            //矩阵二进制化
            HOperatorSet.SerializeHomMat2d(_HomMat2D, out _HomMatID);


            //打开文件
            HOperatorSet.OpenFile(_Path + ".mat", "output_binary", out _FileHandle);

            //写入矩阵变量
            HOperatorSet.FwriteSerializedItem(_FileHandle, _HomMatID);
            //关闭文件
            HOperatorSet.CloseFile(_FileHandle);


        }


        /// <summary>
        /// 读取矩阵文件方法
        /// </summary>
        /// <param name="_Mat2D"></param>
        /// <param name="_Name"></param>
        /// <param name="_Path"></param>
        public static bool Read_Mat2d_Method(ref HTuple _Mat2D, string Vision_Area, string Work_Area)
        {



            try
            {


                HTuple _FileHandle = new HTuple();

                HTuple _HomMatID = new HTuple();

                //打开文件
                string _Path = Directory.GetCurrentDirectory() + "\\Nine_Calibration\\" + Vision_Area + "_" + Work_Area;


                HOperatorSet.OpenFile(_Path + ".mat", "input_binary", out _FileHandle);

                //从文件中读取序列化项目
                HOperatorSet.FreadSerializedItem(_FileHandle, out _HomMatID);

                //反序列化序列化的同类 2D 转换矩阵
                HOperatorSet.DeserializeHomMat2d(_HomMatID, out _Mat2D);
                //关闭文件
                HOperatorSet.CloseFile(_FileHandle);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }


        }


        /// <summary>
        /// 计算矩阵， 放回结果计算误差方法
        /// </summary>
        /// <param name="Calibration"></param>
        /// <param name="Robot"></param>
        /// <returns></returns>
        public static Point3D Calibration_Results_Compute(List<Point3D> Calibration, List<Point3D> Robot, ref HTuple HomMat2D)
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
            //Point3D


            //计算结果组偏差
            double Calibration_Error_X_UI = double.Parse(Specimen_Error(_Error_List_X));
            double Calibration_Error_Y_UI = double.Parse(Specimen_Error(_Error_List_Y));


            return new Point3D(Calibration_Error_X_UI, Calibration_Error_Y_UI, 0);
        }






        /// <summary>
        /// 计算一组样本标准差值
        /// </summary>
        /// <param name="_EList"></param>
        /// <returns></returns>
        public static string Specimen_Error(List<double> _EList)
        {
            double xSum = 0;//样本总和
            double xAvg = 0;//样本平均值
            double sSum = 0;//方差的分子

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


            return Math.Round(Math.Sqrt((sSum / (_EList.Count - 1))), 3).ToString();//样本标准差

            //double    STDP = Convert.ToDouble(Math.Sqrt((sSum / _EList.Count)).ToString());//总体标准差






        }






        /// <summary>
        /// 海康获取图像指针转换Halcon图像
        /// </summary>
        /// <param name="_Width"></param>
        /// <param name="_Height"></param>
        /// <param name="_pData"></param>
        /// <returns></returns>
        public static HImage Mvs_To_Halcon_Image(int _Width, int _Height, IntPtr _pData)
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
        //public HObject Local_To_Halcon_Image(string _local)
        //{

        //    //新建空属性
        //    HOperatorSet.GenEmptyObj(out HObject ho_Image);

        //    ho_Image.Dispose();
        //    HOperatorSet.ReadImage(out ho_Image, _local);

        //    return ho_Image;


        //}

        /// <summary>
        /// 查找标定图像中的模型位置
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_Input_Image"></param>
        /// <param name="_Filtering_Model"></param>
        /// <param name="_Calibration_Data"></param>
        /// <returns></returns>
        public static bool Find_Calibration(ref List<Point3D> _Calibration_Point, HWindow _Window, HObject _Input_Image, Halcon_Find_Calibration_Model _Calibration_Data)
        {

            HObject _Image = new HObject();

            HTuple _Area, _Row, _Column;





            //图像最大灰度值分布在值范围0到255 中
            HOperatorSet.ScaleImageMax(_Input_Image, out _Image);


            if (_Calibration_Data.Median_image_Enable)
            {

                //进行图像中值滤波器平滑
                HOperatorSet.MedianImage(_Image, out _Image, _Calibration_Data.MaskType_Model.ToString(), _Calibration_Data.Median_image_Radius, _Calibration_Data.Margin_Model.ToString());
                if (_Calibration_Data.Median_image_Disp)
                {
                    _Window.DispObj(_Image);
                }

            }




            if (_Calibration_Data.Emphasize_Enable)
            {
                //增强图像的对比度
                HOperatorSet.Emphasize(_Image, out _Image, _Calibration_Data.Emphasize_MaskWidth, _Calibration_Data.Emphasize_MaskHeight, _Calibration_Data.Emphasize_Factor);
                if (_Calibration_Data.Emphasize_Disp)
                {
                    _Window.DispObj(_Image);

                }

            }

            // 使用全局阈值分割图像
            HOperatorSet.Threshold(_Image, out _Image, _Calibration_Data.MinGray, _Calibration_Data.MaxGray);

            ////区域开运算消除边缘
            HOperatorSet.OpeningCircle(_Image, out _Image, _Calibration_Data.OpeningCircle_Radius);

            //计算区域中连接的组件
            HOperatorSet.Connection(_Image, out _Image);



            //填补区域的漏洞
            HOperatorSet.FillUp(_Image, out _Image);


            //形状特征选择圆形和圆度筛选区域
            HOperatorSet.SelectShape(_Image, out _Image, (new HTuple("area")).TupleConcat("circularity"), "and", (new HTuple(_Calibration_Data.Min_Area)).TupleConcat(0.9), (new HTuple(_Calibration_Data.Max_Area)).TupleConcat(1));

            //区域开运算消除边缘
            HOperatorSet.OpeningCircle(_Image, out _Image, _Calibration_Data.OpeningCircle_Radius);

            if (_Calibration_Data.Gray_Disp)
            {

                //区域显示边框
                HOperatorSet.SetDraw(_Window, "margin");
                HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());


                _Window.DispObj(_Image);

                HOperatorSet.SetColor(_Window, nameof(KnownColor.Red).ToLower());

                //区域显示边框
                HOperatorSet.SetDraw(_Window, "fill");


            }

            // 根据区域的相对位置对区域进行排序。
            HOperatorSet.SortRegion(_Image, out _Image, "character", "true", "row");


            //计算区域中心
            HOperatorSet.AreaCenter(_Image, out _Area, out _Row, out _Column);


            //计算识别特征中心十字形的 XLD 轮廓
            HOperatorSet.GenCrossContourXld(out HObject _Cross, _Row, _Column, 100, (new HTuple(45)).TupleRad());




            //生产xld到窗口控件
            HOperatorSet.DispObj(_Cross, _Window);








            //控件显示识别特征数量
            int _Number = _Image.CountObj();
            _Calibration_Point = new List<Point3D>();

            if (_Number == 9)
            {

                //识别特征坐标存储列表


                for (int i = 1; i < _Number + 1; i++)
                {
                    double _X = Math.Round(_Row.TupleSelect(i - 1).D, 3);
                    double _Y = Math.Round(_Column.TupleSelect(i - 1).D, 3);

                    _Calibration_Point.Add(new Point3D(_X, _Y, 0));

                    //控件窗口显示识别信息
                    HOperatorSet.DispText(_Window, "图号: " + i + " X:" + _X + " Y: " + _Y, "image", _X + 100, _Y - 100, "black", "box", "true");


                }

            }
            else
            {




                return false;
            }





            return true;



        }



        /// <summary>
        /// 更加ID获得Xld对象，显示到控件中
        /// </summary>
        /// <param name="ho_ModelContours"></param>
        /// <param name="_Model"></param>
        /// <param name="_ModelXld_ID"></param>
        /// <param name="_Xld_Number"></param>
        /// <param name="_Window"></param>
        /// <returns></returns>
        //public static bool Get_ModelXld(ref HObject ho_ModelContours, Shape_Based_Model_Enum _Model, HObject _ModelXld_ID, int _Xld_Number)
        //{
        //    ///////弃用
        //    try
        //    {


        //        switch (_Model)
        //        {
        //            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

        //                HTuple _ShapeXld = new HTuple (_ModelXld_ID);

        //                HOperatorSet.GetShapeModelContours(out ho_ModelContours, _ShapeXld, _Xld_Number);


        //                break;
        //            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:

        //                HTuple _DeformableXld = new HTuple(_ModelXld_ID);

        //                HOperatorSet.GetDeformableModelContours(out ho_ModelContours, _DeformableXld, _Xld_Number);


        //                break;

        //            case Shape_Based_Model_Enum _T when _T== Shape_Based_Model_Enum.Ncc_Model:

        //                HTuple _NccXld = new HTuple(_ModelXld_ID);

        //                HOperatorSet.GetNccModelRegion(out ho_ModelContours, _NccXld);

        //                break;
        //            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:

        //                ho_ModelContours = _ModelXld_ID;
        //                break;

        //        }


        //        //_Window.ClearWindow();
        //        //_Window.DispObj(ho_ModelContours);

        //        return true;


        //    }
        //    catch (Exception)
        //    {

        //        return false;

        //        throw;

        //    }




        //}



        /// <summary>
        /// 投影映射XLD对象到图像控件位置,并返回XLD对象
        /// </summary>
        /// <param name="_ModelXld"></param>
        /// <param name="_HomMat2D"></param>
        /// <param name="_Window"></param>
        /// <returns></returns>
        public static void ProjectiveTrans_Xld(ref List<HObject> _Model_objects, Shape_Based_Model_Enum _Find_Enum, Halcon_Find_Shape_Out_Parameter _Find_Results, HWindow _Window)
        {


            HObject _ContoursProjTrans = new HObject();



            _Window.SetColor(nameof(KnownColor.Red).ToLower());
            _Window.SetLineWidth(2);



            for (int i = 0; i < _Model_objects.Count; i++)
            {





                //根据匹配模型类型 读取模板内的xld对象
                switch (_Find_Enum)
                {
                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model || _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model || _T== Shape_Based_Model_Enum.Halcon_DXF:

                        //将xld对象矩阵映射到图像中
                        HOperatorSet.ProjectiveTransContourXld(_Model_objects[i], out  _ContoursProjTrans, _Find_Results.HomMat2D[i]);

                        break;

     

                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:


                        //设置显示模型
                        HOperatorSet.SetDraw(_Window, "fill");
                        HOperatorSet.AffineTransRegion(_Model_objects[i], out _ContoursProjTrans, _Find_Results.HomMat2D[i], "constant");


         
                            //将xld对象矩阵映射到图像中
                            //HOperatorSet.ProjectiveTransContourXld(_Model_objects[i], out _ContoursProjTrans, _Find_Results.HomMat2D[i]);

                            //HOperatorSet.GetNccModelRegion(out HObject Ncc_Results, _Model_objects[i]);
                            break;
                }





  

                //显示到对应的控件窗口
                HOperatorSet.DispObj(_ContoursProjTrans, _Window);

            }


            //清除临时内存
            //_ContoursProjTrans.Dispose();


        }


        /// <summary>
        /// 读取匹配模型文件
        /// </summary>
        /// <param name="_Read_Enum"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static bool Read_ModelsXLD_File(ref HTuple _ModelID, ref HObject _ModelContours, Shape_Based_Model_Enum _Model_Based, string _Path)
        {




            //读取模型文件
            //HTuple _ModelContours = new HTuple();

            try
            {


                switch (_Model_Based)
                {

                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

                        //读取文件

                        HOperatorSet.ReadShapeModel(_Path, out _ModelID);
                        HOperatorSet.GetShapeModelContours(out _ModelContours, _ModelID, 1);

                        break;
                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:
                        //读取文件

                        HOperatorSet.ReadDeformableModel(_Path, out _ModelID);
                        HOperatorSet.GetDeformableModelContours(out _ModelContours, _ModelID, 1);



                        break;

                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:
                        //读取文件

                        HOperatorSet.ReadNccModel(_Path, out _ModelID);
                        HOperatorSet.GetNccModelRegion(out _ModelContours, _ModelID);



                        break;
                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:



                        //读取文件
                        HOperatorSet.ReadContourXldDxf(out _ModelContours, _Path, new HTuple(), new HTuple(), out _);

                        break;

                }

                return true;
            }
            catch (Exception)
            {
                //throw;
                return false;

            }





        }


        /// <summary>
        /// 根据模型类型获得模型文件地址
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_Model_Enum"></param>
        /// <returns></returns>
        public static bool Get_ModelXld_Path<T1>(ref T1 _path, string _Location, FilePath_Type_Model_Enum _FilePath_Type, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID, int _Number = 1)
        {

            ////获得识别位置名称
            //string _Name = ShapeModel_Name.ToString();
            //string _Work = Work_Name.ToString();

            List<string> _ModelIDS = new List<string>();






            switch (_FilePath_Type)
            {
                case FilePath_Type_Model_Enum.Get:


                    DirectoryInfo _FileInfo = new DirectoryInfo(_Location);

                    //var aa = _FileInfo.GetFiles().Where((_W) =>
                    //{
                    //    _W.Name.Contains(_ID.ToString() + "_" + _Name + "_" + ((int)_Model_Enum).ToString());

                    //    return true;
                    //});

                    //根据匹配模型类型 读取模板内的xld对象
                    switch (_Model_Enum)
                    {
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model || _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model :





                            break;
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:



                            //获取文件内名称
                            foreach (FileInfo _FileName in _FileInfo.GetFiles())
                            {

                                if (_FileName.Name.Contains(_ID.ToString() + "_" + _Name + "_" + ((int)_Model_Enum).ToString()))
                                {



                                    _ModelIDS.Add(_FileName.FullName);



                                }


                            }



                            break;

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:




                            break;
                    }









                    //类型转换
                    _path = (T1)(object)_ModelIDS;



                    break;
                case FilePath_Type_Model_Enum.Save:



                    string Save_Path = _Location + "\\" + _ID.ToString() + "_" + _Name + "_" + ((int)_Model_Enum).ToString();

                    //路径添加格式
                    switch (_Model_Enum)
                    {
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

                            Save_Path += ".shm";

                            break;

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:

                            Save_Path += ".dfm";
                            break;

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:

                            Save_Path += "_" + _Number + ".ncm";
                            break;
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:

                            Save_Path += "_" + (int)Shape_Based_Model_Enum.Ncc_Model + ".dxf";
                            break;
                    }


                    _path = (T1)(object)Save_Path;


                    break;

            }


















            if (_Location != "")
            {




                return true;
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return false;

            }

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
        public static void ShapeModel_SaveFile(ref HTuple _ModelID, HObject _Image, string _Location, Create_Shape_Based_ModelXld _Create_Model, HObject _ModelsXLD)
        {

            //开启线保存匹配模型文件
            //new Thread(new ThreadStart(new Action(() =>
            //{




            //lock (_Create_Model)
            //{

            string _Path = "";

            _ModelID.Dispose();

            //获得保存模型文件地址
            Get_ModelXld_Path(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID);



            switch (_Create_Model.Shape_Based_Model)
            {
                case Shape_Based_Model_Enum.shape_model:


                    //创建模型
                    HOperatorSet.CreateShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
       , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
       _Create_Model.MinContrast, out _ModelID);



                    //保存模型文件
                    HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    break;
                case Shape_Based_Model_Enum.planar_deformable_model:

                    //创建模型
                    HOperatorSet.CreatePlanarUncalibDeformableModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                                , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleRMin, new HTuple(), _Create_Model.ScaleRStep, _Create_Model.ScaleCMin, new HTuple(),
                                                                                                 _Create_Model.ScaleCStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(), _Create_Model.MinContrast, new HTuple(), new HTuple(),
                                                                                                out _ModelID);

                    //保存模型文件
                    HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    break;
                case Shape_Based_Model_Enum.local_deformable_model:

                    HOperatorSet.CreateLocalDeformableModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                  , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleRMin, new HTuple(), _Create_Model.ScaleRStep, _Create_Model.ScaleCMin, new HTuple(),
                                                                                 _Create_Model.ScaleCStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(), _Create_Model.MinContrast, new HTuple(), new HTuple(),
                                                                                 out _ModelID);


                    //保存模型文件
                    HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    break;
                case Shape_Based_Model_Enum.Scale_model:

                    //创建模型
                    HOperatorSet.CreateScaledShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                                 , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), _Create_Model.AngleStep, _Create_Model.ScaleMin, _Create_Model.ScaleMax, _Create_Model.ScaleStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
                                                                                                _Create_Model.MinContrast, out _ModelID);

                    //保存模型文件
                    HOperatorSet.WriteShapeModel(_ModelID, _Path);

                    break;

                case Shape_Based_Model_Enum.Ncc_Model:



                    HOperatorSet.GenEmptyObj(out HObject Gen_Polygons);
                    HOperatorSet.GenEmptyObj(out HObject Region_Unio);
                    HOperatorSet.GenEmptyObj(out HObject Select_Region);
                    HOperatorSet.GenEmptyObj(out HObject Gen_Region);
                    HOperatorSet.GenEmptyObj(out HObject Polygon_Xld);
                    HOperatorSet.GenEmptyObj(out HObject Region_Dilation);
                    HOperatorSet.GenEmptyObj(out HObject All_Reduced);
                    HOperatorSet.GenEmptyObj(out HObject XLD_1);
                    HOperatorSet.GenEmptyObj(out HObject XLD_2);
                    HOperatorSet.GenEmptyObj(out HObject XLD_3);
                    HOperatorSet.GenEmptyObj(out HObject Region_Unio_1);
                    HOperatorSet.GenEmptyObj(out HObject Region_Unio_2);
                    HOperatorSet.GenEmptyObj(out HObject Region_Unio_3);




                    //每个xld转换多边形类型 
                    for (int X = 0; X < _ModelsXLD.CountObj(); X++)
                    {
                        //分解xld图像点为多边形
                        HOperatorSet.GenPolygonsXld(_ModelsXLD.SelectObj(X + 1), out Select_Region, "ramer", 0.1);
                        //获得分解点的多边形坐标数据
                        HOperatorSet.GetPolygonXld(Select_Region, out HTuple _Pos_Row, out HTuple _Pos_Col, out HTuple _, out HTuple _);
                        //将多边形转换区域
                        HOperatorSet.GenRegionPolygon(out Gen_Region, _Pos_Row, _Pos_Col);
                        //存入集合中
                        HOperatorSet.ConcatObj(Polygon_Xld, Gen_Region, out Polygon_Xld);


                    }

                    //膨胀全部区域
                    HOperatorSet.DilationCircle(Polygon_Xld, out Region_Dilation, _Create_Model.DilationCircle);


                    //转换区域类型
                    HRegion Region = new HRegion(Region_Dilation);


                    //xld集合
                    List<HObject> All_XLD = new List<HObject>
                    {
                        new HObject(_ModelsXLD.SelectObj(1)).ConcatObj(_ModelsXLD.SelectObj(2)),
                        new HObject(_ModelsXLD.SelectObj(3)).ConcatObj(_ModelsXLD.SelectObj(2)),
                        new HObject(_ModelsXLD.SelectObj(4)).ConcatObj(_ModelsXLD.SelectObj(5)),

                    };


                    //区域集合
                    List<HObject> All_Region = new List<HObject>
                    {
                        new HObject(Region.SelectObj(1).Union2(Region.SelectObj(2))),
                        new HObject(Region.SelectObj(3).Union2(Region.SelectObj(2))),
                        new HObject(Region.SelectObj(4).Union2(Region.SelectObj(5))),

                    };


                    //创建并保存模型文件
                    for (int i = 0; i < All_XLD.Count; i++)
                    {
                        //抠图出
                        HOperatorSet.ReduceDomain(_Image, All_Region[i], out HObject ImageRegion);

                        HOperatorSet.CreateNccModel(ImageRegion, _Create_Model.NumLevels, _Create_Model.AngleStart, _Create_Model.AngleExtent, _Create_Model.AngleStep, _Create_Model.Metric.ToString(), out _ModelID);



                        Get_ModelXld_Path<string>(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i + 1);


                        //保存模型文件
                        HOperatorSet.WriteNccModel(_ModelID, _Path);


                        HOperatorSet.AreaCenter(ImageRegion, out HTuple _, out HTuple Region_Row, out HTuple Region_Col);

                        HOperatorSet.VectorAngleToRigid(Region_Row, Region_Col, 0, 0, 0, 0, out HTuple HomMat2D);



                        HOperatorSet.ProjectiveTransContourXld(All_XLD[i], out HObject XLD, HomMat2D);


                        Get_ModelXld_Path<string>(ref _Path, _Location, FilePath_Type_Model_Enum.Save, Shape_Based_Model_Enum.Halcon_DXF, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i + 1);
                        //保存模板xld文件
                        HOperatorSet.WriteContourXldDxf(XLD, _Path);


                    }





                    break;

            }





        }




        /// <summary>
        /// 图像提前预处理
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_HWindow"></param>
        /// <param name="_Find_Property"></param>
        public static void Halcon_Image_Pre_Processing(ref HObject _Image, HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
        {


            _HWindow.DispObj(_Image);


            if (_Find_Property.ScaleImageMax_Enable)
            {
                //图像最大灰度值分布在值范围0到255 中
                HOperatorSet.ScaleImageMax(_Image, out _Image);
                if (_Find_Property.ScaleImageMax_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);
                }
            }


            if (_Find_Property.Median_image_Enable)
            {
                //进行图像中值滤波器平滑
                HOperatorSet.MedianImage(_Image, out _Image, _Find_Property.MaskType_Model.ToString(), _Find_Property.Median_image_Radius, _Find_Property.Margin_Model.ToString());
                if (_Find_Property.Median_image_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);

                }

            }



            if (_Find_Property.MedianRect_Enable)
            {
                //进行图像中值滤波器平滑
                HOperatorSet.MedianRect(_Image, out _Image, _Find_Property.MedianRect_MaskWidth, _Find_Property.MedianRect_MaskHeight);
                if (_Find_Property.MedianRect_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);
                }
            }




            if (_Find_Property.Illuminate_Enable)
            {
                //高频增强图像的对比度
                HOperatorSet.Illuminate(_Image, out _Image, _Find_Property.Illuminate_MaskWidth, _Find_Property.Illuminate_MaskHeight, _Find_Property.Illuminate_Factor);
                if (_Find_Property.Illuminate_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);
                }

            }



            if (_Find_Property.Emphasize_Enable)
            {
                //增强图像的对比度
                HOperatorSet.Emphasize(_Image, out _Image, _Find_Property.Emphasize_MaskWidth, _Find_Property.Emphasize_MaskHeight, _Find_Property.Emphasize_Factor);
                if (_Find_Property.Emphasize_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);


                }

            }

            if (_Find_Property.GrayOpeningRect_Enable)
            {
                //进行图像灰度开运算
                HOperatorSet.GrayOpeningRect(_Image, out _Image, _Find_Property.GrayOpeningRect_MaskHeight, _Find_Property.GrayOpeningRect_MaskWidth);
                if (_Find_Property.GrayOpeningRect_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);
                }
            }


            if (_Find_Property.GrayClosingRect_Enable)
            {
                //进行图像灰度开运算
                HOperatorSet.GrayClosingRect(_Image, out _Image, _Find_Property.GrayClosingRect_MaskHeight, _Find_Property.GrayClosingRect_MaskWidth);
                if (_Find_Property.GrayClosingRect_Disp)
                {
                    HOperatorSet.DispObj(_Image, _HWindow);
                }
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
        public static Halcon_Find_Shape_Out_Parameter Find_Deformable_Model(HWindow _HWindow, HObject _Image, List<HTuple> _ModelID, Find_Shape_Based_ModelXld _Find_Property)
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

                Halcon_Find_Shape_Out_Parameter _Find_Out = new Halcon_Find_Shape_Out_Parameter() { DispWiindow = _HWindow };



                //图像预处理
                Halcon_Image_Pre_Processing(ref _Image, _HWindow, _Find_Property);



                switch (_Find_Property.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:

                        foreach (var _ModelXld in _ModelID)
                        {


                            HOperatorSet.FindShapeModel(_Image,
                                _ModelXld,
                                (new HTuple(_Find_Property.AngleStart)).TupleRad(),
                                (new HTuple(_Find_Property.AngleExtent)).TupleRad(),
                                _Find_Property.MinScore,
                                _Find_Property.NumMatches,
                                _Find_Property.MaxOverlap,
                                _Find_Property.SubPixel.ToString(),
                                _Find_Property.NumLevels,
                                _Find_Property.Greediness,
                                out hv_row, 
                                out hv_column,
                                out hv_angle, 
                                out hv_score);

                            // 查找模型成功保存结果数据
                            if (hv_score.Length != 0)
                            {
                           
                                _Find_Out.Row.Add(hv_row);
                                _Find_Out.Column.Add(hv_column);
                                _Find_Out.Angle.Add(hv_angle);
                                _Find_Out.Score.Add(hv_score);
                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_row, hv_column, hv_angle, out HTuple HMat2D);
                                _Find_Out.HomMat2D.Add(HMat2D);
                            }
                            else
                            { 
                                _Find_Out.Row.Add(0);
                                _Find_Out.Column.Add(0);
                                _Find_Out.Angle.Add(0);
                                _Find_Out.Score.Add(0);
                            }



                        }


                        //_Find_Out = new Halcon_Find_Shape_Out_Parameter() { Row = new List<double>() { hv_row.D }, Column = new List<double>() { hv_column.D }, Angle = new List<double>() { hv_angle.D }, Score = new List<double>() { hv_score.D }, Find_Time = (DateTime.Now - RunTime).Milliseconds };

                        break;



                    case Shape_Based_Model_Enum.planar_deformable_model:



                        foreach (var _ModelXld in _ModelID)
                        {


                            //查找模型
                            HOperatorSet.FindPlanarUncalibDeformableModel
                                (_Image,
                                _ModelXld,
                                (new HTuple(_Find_Property.AngleStart)).TupleRad(),
                                (new HTuple(_Find_Property.AngleExtent)).TupleRad(),
                                _Find_Property.ScaleRMin,
                                _Find_Property.ScaleRMax,
                                _Find_Property.ScaleCMin,
                                _Find_Property.ScaleCMax,
                                _Find_Property.MinScore,
                                _Find_Property.NumMatches,
                                _Find_Property.MaxOverlap,
                                _Find_Property.NumLevels,
                                _Find_Property.Greediness,
                                ((new HTuple("subpixel")).TupleConcat("aniso_scale_change_restriction")).TupleConcat("angle_change_restriction"),
                                ((new HTuple(_Find_Property.SubPixel.ToString())).TupleConcat(_Find_Property.Aniso_scale_change_restriction)).TupleConcat(_Find_Property.Angle_change_restriction),
                                out hv_HomMat2D,
                                out hv_score);


                        }

                        if (hv_score.Length != 0)
                        {

                            //_Find_Out = new Halcon_Find_Shape_Out_Parameter() { DispWiindow = _HWindow, HomMat2D = hv_HomMat2D, Score = new List<double>() { hv_score.D }, Find_Time = new List<double>() { (DateTime.Now - RunTime).TotalMilliseconds } };
                            _Find_Out.HomMat2D.Add(hv_HomMat2D);
                            _Find_Out.Score.Add(hv_score);
                            _Find_Out.Find_Time = (DateTime.Now - RunTime).TotalMilliseconds;
                        }
                        else
                        {
                            //_Find_Out = new Halcon_Find_Shape_Out_Parameter() { DispWiindow = _HWindow, Score = new List<double>() { 0}, Find_Time = new List<double>() { (DateTime.Now - RunTime).TotalMilliseconds } };
                            _Find_Out.Score.Add(0);
                            _Find_Out.Find_Time = (DateTime.Now - RunTime).TotalMilliseconds;
                        }


                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:



                        break;
                    case Shape_Based_Model_Enum.Scale_model:



                        break;


                    case Shape_Based_Model_Enum.Ncc_Model:

                        //识别结果对象初始化
                        List<bool> Find_Results = new List<bool>();
                        for (int i = 0; i < _ModelID.Count; i++)
                        {
                            Find_Results.Add(false);
                        }
                        _Find_Out = new Halcon_Find_Shape_Out_Parameter() { DispWiindow = _HWindow };




                        for (int i = 0; i < _ModelID.Count; i++)
                        {

                            HOperatorSet.FindNccModel
                                (_Image,
                                _ModelID[i],
                                (new HTuple(_Find_Property.AngleStart)).TupleRad(),
                                (new HTuple(_Find_Property.AngleExtent)).TupleRad(),
                                _Find_Property.MinScore,
                                _Find_Property.NumMatches,
                                _Find_Property.MaxOverlap,
                                "true",
                                _Find_Property.NumLevels,
                                out hv_row,
                                out hv_column,
                                out hv_angle,
                                out hv_score
                                );



                            // 查找模型成功保存结果数据
                            if (hv_score.Length != 0)
                            {
                                Find_Results[i] = true;
                                _Find_Out.Row.Add(hv_row);
                                _Find_Out.Column.Add(hv_column);
                                _Find_Out.Angle.Add(hv_angle);
                                _Find_Out.Score.Add(hv_score);
                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_row, hv_column, hv_angle, out HTuple HMat2D);
                                _Find_Out.HomMat2D.Add(HMat2D);
                            }
                            else
                            {
                                Find_Results[i] = false;
                                _Find_Out.Row.Add(0);
                                _Find_Out.Column.Add(0);
                                _Find_Out.Angle.Add(0);
                                _Find_Out.Score.Add(0);
                            }

                        }













                        break;
                }


                //清除临时内存
                hv_row.Dispose();
                hv_column.Dispose();
                hv_angle.Dispose();
                hv_score.Dispose();
                hv_HomMat2D.Dispose();
                _Image1.Dispose();
                _Image2.Dispose();

                return _Find_Out;





            }

        }







    }




}
