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
        public static HPR_Status_Model Save_Image(HObject _Image)
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


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "图像保存成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.图像保存失败) { Result_Error_Info = e.Message };

            }
        }


        /// <summary>
        /// 保存标定矩阵坐标方法
        /// </summary>
        /// <param name="_HomMat2D">需要保存矩阵对象</param>
        /// <param name="_Name">保存名字</param>
        /// <param name="_Path">保存地址</param>
        public static HPR_Status_Model Save_Mat2d_Method(HTuple _HomMat2D, string _Path)
        {
            HTuple _HomMatID = new HTuple();
            HTuple _FileHandle = new HTuple();


            try
            {


                //矩阵二进制化
                HOperatorSet.SerializeHomMat2d(_HomMat2D, out _HomMatID);


                //打开文件
                HOperatorSet.OpenFile(_Path + ".mat", "output_binary", out _FileHandle);

                //写入矩阵变量
                HOperatorSet.FwriteSerializedItem(_FileHandle, _HomMatID);
                //关闭文件
                HOperatorSet.CloseFile(_FileHandle);

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "保存矩阵文件成功！" };
            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.保存矩阵文件失败) { Result_Error_Info = e.Message };
            }


        }


        /// <summary>
        /// 读取矩阵文件方法
        /// </summary>
        /// <param name="_Mat2D"></param>
        /// <param name="_Name"></param>
        /// <param name="_Path"></param>
        public static HPR_Status_Model Read_Mat2d_Method(ref HTuple _Mat2D, string Vision_Area, string Work_Area)
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

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "读取矩阵文件成功！" };
            }
            catch (Exception e)
            {
                return new HPR_Status_Model(HVE_Result_Enum.读取矩阵文件失败) { Result_Error_Info = e.Message };

            }


        }


        /// <summary>
        /// 计算矩阵， 放回结果计算误差方法
        /// </summary>
        /// <param name="Calibration"></param>
        /// <param name="Robot"></param>
        /// <returns></returns>
        public static HPR_Status_Model Calibration_Results_Compute(ref Point3D _Results, List<Point3D> Calibration, List<Point3D> Robot, ref HTuple HomMat2D)
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


            try
            {



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

                _Results = new Point3D(Calibration_Error_X_UI, Calibration_Error_Y_UI, 0);

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "实际偏差：X " + Calibration_Error_X_UI + ",Y " + Calibration_Error_Y_UI };
            }
            catch (Exception e)
            {
                return new HPR_Status_Model(HVE_Result_Enum.计算实际误差失败) { Result_Error_Info = e.Message };
            }


        }










        /// <summary>
        /// 根据不同匹配模式提取不用xld特征顺序，后续可有用户选择提示功能
        /// </summary>
        /// <param name="_Lin_1"></param>
        /// <param name="_Cir_1"></param>
        /// <param name="_Lin_2"></param>
        /// <param name="_Lin_3"></param>
        /// <param name="_Lin_4"></param>
        /// <param name="_ALL_ModelXLD"></param>
        /// <param name="Matching_Model"></param>
        /// <returns></returns>
        public static HPR_Status_Model Get_Model_Match_XLD(ref HObject _Lin_1,ref HObject _Cir_1,ref HObject _Lin_2,ref HObject _Lin_3,ref HObject _Lin_4, List<HObject> _ALL_ModelXLD, Shape_Based_Model_Enum Matching_Model)
        {



            //选择出有需要的直线特征

            try
            {

  
            //根据匹配模型类型 读取模板内的xld对象
            switch (Matching_Model)
            {
                case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:



                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_1, 1);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Cir_1, 2);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_2, 3);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_3, 4);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_4, 5);


                    break;

                case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_1, 1);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Cir_1, 2);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_2, 3);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_3, 4);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_4, 5);


                    break;


                case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:

                        if (_ALL_ModelXLD.Count==3)
                        {


                    HOperatorSet.SelectObj(_ALL_ModelXLD[0], out _Lin_1, 1);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[1], out _Cir_1, 2);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[1], out _Lin_2, 1);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[2], out _Lin_3, 1);
                    HOperatorSet.SelectObj(_ALL_ModelXLD[2], out _Lin_4, 2);
                        }
                        else
                        {
                            return new HPR_Status_Model(HVE_Result_Enum.提取匹配结果的XLD模型数量与计算数量不匹配);
                        }


                    break;
            }


       




            return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "提取计算特征XLD成功！" };

            }
            catch (Exception e)
            {
                return new HPR_Status_Model(HVE_Result_Enum.提取匹配结果的XLD模型失败) { Result_Error_Info = e.Message };
            }
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
        public static HPR_Status_Model Mvs_To_Halcon_Image(ref HObject image, int _Width, int _Height, IntPtr _pData)
        {

            try
            {


                //转换halcon图像格式
                HOperatorSet.GenImage1(out image, "byte", _Width, _Height, _pData);



                return new HPR_Status_Model(HVE_Result_Enum.Run_OK);

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.Halcon转换海康图像错误) { Result_Error_Info = e.Message };

            }



        }



        /// <summary>
        /// 路径读取图片
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static HPR_Status_Model HRead_Image(ref HObject _Image, string _Path)
        {

            try
            {

                if (_Path != "")
                {
                    HOperatorSet.ReadImage(out _Image, _Path);



                    return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件图像读取成功！" };
                }
                else
                {
                    return new HPR_Status_Model(HVE_Result_Enum.读取图像文件格式错误);
                }


            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.读取图像文件格式错误) { Result_Error_Info = e.Message };

            }

        }




        /// <summary>
        /// 查找标定图像中的模型位置
        /// </summary>
        /// <param name="_Window"></param>
        /// <param name="_Input_Image"></param>
        /// <param name="_Filtering_Model"></param>
        /// <param name="_Calibration_Data"></param>
        /// <returns></returns>
        public static HPR_Status_Model Find_Calibration(ref List<Point3D> _Calibration_Point, HWindow _Window, HObject _Input_Image, Halcon_Find_Calibration_Model _Calibration_Data)
        {

            HObject _Image = new HObject();



            try
            {




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
                HOperatorSet.AreaCenter(_Image, out _, out HTuple _Row, out HTuple _Column);


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

                    return new HPR_Status_Model(HVE_Result_Enum.标定查找9点位置区域失败);
                }





                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "查找9点标定区域成功！" };

            }
            catch (Exception e)
            {
                return new HPR_Status_Model(HVE_Result_Enum.标定查找9点位置区域失败) { Result_Error_Info = e.Message };

            }

        }






        /// <summary>
        /// 投影映射XLD对象到图像控件位置,并返回XLD对象
        /// </summary>
        /// <param name="_ModelXld"></param>
        /// <param name="_HomMat2D"></param>
        /// <param name="_Window"></param>
        /// <returns></returns>
        public static HPR_Status_Model ProjectiveTrans_Xld(ref List<HObject> _Model_objects, List<HTuple> _ModelID, Shape_Based_Model_Enum _Find_Enum, Halcon_Find_Shape_Out_Parameter _Find_Results, HWindow _Window)
        {


            HObject _ContoursProjTrans = new HObject();
            HTuple Mat2D = new HTuple();


            try
            {



                _Window.SetColor(nameof(KnownColor.Red).ToLower());
                _Window.SetLineWidth(2);



                for (int i = 0; i < _Model_objects.Count; i++)
                {





                    //根据匹配模型类型 读取模板内的xld对象
                    switch (_Find_Enum)
                    {
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:


                            //HOperatorSet.GetShapeModelContours(out _ContoursProjTrans, _ModelID[i], 1);
                            HOperatorSet.ProjectiveTransContourXld(_Model_objects[i], out _ContoursProjTrans, _Find_Results.HomMat2D[i]);


                            break;

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model :

                            //将xld对象矩阵映射到图像中
                            //HOperatorSet.GetDeformableModelContours(out _ContoursProjTrans, _ModelID[i], 1);
                            HOperatorSet.ProjectiveTransContourXld(_Model_objects[i], out _ContoursProjTrans, _Find_Results.HomMat2D[i]);

                            break;



                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:


                            //设置显示模型
                            HOperatorSet.SetDraw(_Window, "margin");






                            HOperatorSet.HomMat2dIdentity(out Mat2D);
                            HOperatorSet.HomMat2dRotate(Mat2D, (new HTuple(_Find_Results.Angle[i])).TupleRad(), new HTuple(0), new HTuple(0), out Mat2D);
                            HOperatorSet.HomMat2dTranslate(Mat2D, _Find_Results.Row[i], _Find_Results.Column[i], out Mat2D);

                            //将xld对象矩阵映射到图像中
                            HOperatorSet.ProjectiveTransContourXld(_Model_objects[i], out _ContoursProjTrans, Mat2D);


                            HOperatorSet.GetNccModelRegion(out HObject Ncc_Results, _ModelID[i]);
                            HOperatorSet.AffineTransRegion(Ncc_Results, out Ncc_Results, Mat2D, "constant");
                            HOperatorSet.ConcatObj(_ContoursProjTrans, Ncc_Results, out _ContoursProjTrans);

                            break;
                    }

                    _Model_objects[i] = _ContoursProjTrans;





                    //显示到对应的控件窗口
                    HOperatorSet.DispObj(_ContoursProjTrans, _Window);

                }


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.XLD对象映射失败) { Result_Error_Info = e.Message };
            }



            //清除临时内存
            //_ContoursProjTrans.Dispose();
            //Mat2D.Dispose();


        }





        public static HPR_Status_Model Read_Halcon_Type_File(ref HTuple _Model,  ref HObject _ModelContours, FileInfo _Path)
        {


            try
            {

     

            string[] _Name = _Path.Name.Split('.');


            switch (_Name[1])
            {
                case "ncm":

                    HOperatorSet.ReadNccModel(_Path.FullName, out _Model);
                    HOperatorSet.GetNccModelRegion(out  _ModelContours, _Model);

                    break;
                case "dxf":

                    //读取文件
                    HOperatorSet.ReadContourXldDxf(out  _ModelContours, _Path.FullName, new HTuple(), new HTuple(), out _);


                    break;

                case "dfm":

                    HOperatorSet.ReadDeformableModel(_Path.FullName, out  _Model);
                    HOperatorSet.GetDeformableModelContours(out _ModelContours, _Model, 1);


                    break;


                    case "shm":

                        HOperatorSet.ReadShapeModel(_Path.FullName, out _Model);
                        HOperatorSet.GetShapeModelContours(out _ModelContours, _Model, 1);

                        


                        break;
                        }





            return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Path.Name+"，Halcon文件读取成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.Halcon文件类型读取失败) { Result_Error_Info = e.Message };
            }
        }




        /// <summary>
        /// 读取匹配模型文件
        /// </summary>
        /// <param name="_Read_Enum"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static HPR_Status_Model Read_ModelsXLD_File(ref List<HTuple> _ModelID, ref List<HObject> _ModelContours, Shape_Based_Model_Enum _Model_Based, List<FileInfo> _PathInfo)
        {




            //读取模型文件
            //HTuple _ModelContours = new HTuple();

            HTuple _Model = new HTuple();
            HObject Contours = new HObject();


            try
            {
                foreach (FileInfo _Path in _PathInfo)
                {


                    switch (_Model_Based)
                    {

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

                            //读取文件

                            //HOperatorSet.ReadShapeModel(_Path.FullName, out HTuple _Model_Shape);
                            //HOperatorSet.GetShapeModelContours(out HObject _Model_Shape_Contours, _Model_Shape, 1);
                    
                             Read_Halcon_Type_File(ref _Model, ref Contours, _Path);
                            _ModelID.Add(_Model);
                            _ModelContours.Add(Contours);

                            break;
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:
                            //读取文件


                            //HOperatorSet.ReadDeformableModel(_Path.FullName, out HTuple _Model_Deformable);
                            //HOperatorSet.GetDeformableModelContours(out HObject _Model_Deformable_Contours, _Model_Deformable, 1);
                            
                            
                            Read_Halcon_Type_File(ref _Model, ref Contours, _Path);

                            _ModelID.Add(_Model);
                            _ModelContours.Add(Contours);

                            break;

                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:
                            //读取文件


                            //Read_Halcon_Type_File(ref _Model, ref Contours, _Path);

                            //_ModelID.Add(_Model);
                            //_ModelContours.Add(Contours);


                            if (_Path.Name.Contains(".dxf"))
                            {

                                //读取文件
                                Read_Halcon_Type_File(ref _Model, ref Contours, _Path);

                                _ModelContours.Add(Contours);

                            }
                            else
                            {

                                Read_Halcon_Type_File(ref _Model, ref Contours, _Path);

                                //HOperatorSet.ReadNccModel(_Path.FullName, out HTuple _Model_Ncc);
                                //HOperatorSet.GetNccModelRegion(out HObject _Model_Ncc_Contours, _Model_Ncc);
                                _ModelID.Add(_Model);

                            }


                            break;
                        case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:



                            //读取文件
                            //HOperatorSet.ReadContourXldDxf(out _ModelContours, _Path, new HTuple(), new HTuple(), out _);

                            break;

                    }

                }
                return new HPR_Status_Model(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {
                //throw;
                return new HPR_Status_Model(HVE_Result_Enum.读取模型文件失败) { Result_Error_Info = e.Message };

            }





        }


        /// <summary>
        /// 根据模型类型获得模型文件地址
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_Model_Enum"></param>
        /// <returns></returns>
        public static HPR_Status_Model Get_ModelXld_Path<T1>(ref T1 _path, string _Location, FilePath_Type_Model_Enum _FilePath_Type, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID, int _Number = 0)
        {

            ////获得识别位置名称
            //string _Name = ShapeModel_Name.ToString();
            //string _Work = Work_Name.ToString();

            List<FileInfo> _ModelIDS = new List<FileInfo>();
            string Save_Path = "";

            if (_Location != "")
            {







                switch (_FilePath_Type)
                {
                    case FilePath_Type_Model_Enum.Get:


                        DirectoryInfo _FileInfo = new DirectoryInfo(_Location);


                        foreach (FileInfo _FileName in _FileInfo.GetFiles())
                        {

                            if (_FileName.Name.Contains(_ID.ToString() + "_" + _Name + "_" + ((int)_Model_Enum).ToString()))
                            {

                                _ModelIDS.Add(_FileName);
                            }

                        }


                        //类型转换
                        _path = (T1)(object)_ModelIDS;



                        break;
                    case FilePath_Type_Model_Enum.Save:



                        Save_Path = _Location + "\\" + _ID.ToString() + "_" + _Name;

                        //路径添加格式后缀
                        switch (_Model_Enum)
                        {
                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

                                Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".shm";

                                break;

                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:

                                Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".dfm";
                                break;

                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:

                                Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".ncm";
                                break;
                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:

                                Save_Path += "_" + ((int)Shape_Based_Model_Enum.Ncc_Model).ToString() + "_" + _Number + ".dxf";
                                break;
                        }


                        _path = (T1)(object)Save_Path;


                        break;

                }




















                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件路径读取成功！" };
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return new HPR_Status_Model(HVE_Result_Enum.文件路径提取失败) { Result_Error_Info = Save_Path };

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
        public static HPR_Status_Model ShapeModel_SaveFile(ref HTuple _ModelID, HObject _Image, string _Location, Create_Shape_Based_ModelXld _Create_Model, HObject _ModelsXLD)
        {




            string _Path = "";
            _ModelID.Dispose();


            try
            {



                switch (_Create_Model.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:



                        //获得保存模型文件地址
                        if (Get_ModelXld_Path(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {


                            //创建模型
                            HOperatorSet.CreateShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
               , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), (new HTuple(_Create_Model.AngleStep)).TupleRad(), _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
               _Create_Model.MinContrast, out _ModelID);



                            //保存模型文件
                            HOperatorSet.WriteShapeModel(_ModelID, _Path);
                        }

                        break;
                    case Shape_Based_Model_Enum.planar_deformable_model:

                        //获得保存模型文件地址
                        if (Get_ModelXld_Path(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            //创建模型
                            HOperatorSet.CreatePlanarUncalibDeformableModelXld(
                                _ModelsXLD, _Create_Model.NumLevels, 
                                (new HTuple(_Create_Model.AngleStart)).TupleRad() ,
                                (new HTuple(_Create_Model.AngleExtent)).TupleRad(), 
                                (new HTuple(_Create_Model.AngleStep)).TupleRad(),
                                _Create_Model.ScaleRMin,
                                new HTuple(),
                                _Create_Model.ScaleRStep,
                                _Create_Model.ScaleCMin,
                                new HTuple(),
                                 _Create_Model.ScaleCStep,
                                 _Create_Model.Optimization.ToString(),
                                 _Create_Model.Metric.ToString(),
                                 _Create_Model.MinContrast,
                                 new HTuple(),
                                 new HTuple(),
                                 out _ModelID);

                            //保存模型文件
                            HOperatorSet.WriteDeformableModel(_ModelID, _Path);
                        }
                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:

                        //获得保存模型文件地址
                        if (Get_ModelXld_Path(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            HOperatorSet.CreateLocalDeformableModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                      , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), (new HTuple(_Create_Model.AngleStep)).TupleRad(), _Create_Model.ScaleRMin, new HTuple(), _Create_Model.ScaleRStep, _Create_Model.ScaleCMin, new HTuple(),
                                                                                     _Create_Model.ScaleCStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(), _Create_Model.MinContrast, new HTuple(), new HTuple(),
                                                                                     out _ModelID);


                            //保存模型文件
                            HOperatorSet.WriteDeformableModel(_ModelID, _Path);
                        }
                        break;
                    case Shape_Based_Model_Enum.Scale_model:

                        //获得保存模型文件地址
                        if (Get_ModelXld_Path(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            //创建模型
                            HOperatorSet.CreateScaledShapeModelXld(_ModelsXLD, _Create_Model.NumLevels, (new HTuple(_Create_Model.AngleStart)).TupleRad()
                                                                                                     , (new HTuple(_Create_Model.AngleExtent)).TupleRad(), (new HTuple(_Create_Model.AngleStep)).TupleRad(), _Create_Model.ScaleMin, _Create_Model.ScaleMax, _Create_Model.ScaleStep, _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
                                                                                                    _Create_Model.MinContrast, out _ModelID);

                            //保存模型文件
                            HOperatorSet.WriteShapeModel(_ModelID, _Path);
                        }

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
                        new HObject(_ModelsXLD.SelectObj(4)).ConcatObj(_ModelsXLD.SelectObj(5).ConcatObj(_ModelsXLD.SelectObj(2))),

                    };


                        //区域集合
                        List<HObject> All_Region = new List<HObject>
                    {
                        new HObject(Region.SelectObj(1).Union2(Region.SelectObj(2))),
                        new HObject(Region.SelectObj(3).Union2(Region.SelectObj(2))),
                        new HObject(Region.SelectObj(4).Union2(Region.SelectObj(5)).Union2(Region.SelectObj(2))),

                    };






                        //创建并保存模型文件
                        for (int i = 0; i < All_XLD.Count; i++)
                        {
                            //抠图出
                            HOperatorSet.ReduceDomain(_Image, All_Region[i], out HObject ImageRegion);


                            //创建NCC模板
                            HOperatorSet.CreateNccModel(ImageRegion,
                                _Create_Model.NumLevels,
                                (new HTuple(_Create_Model.AngleStart)).TupleRad(),
                                (new HTuple(_Create_Model.AngleExtent)).TupleRad(),
                                (new HTuple(_Create_Model.AngleStep)).TupleRad(),
                                _Create_Model.Metric.ToString(),
                                out _ModelID);


                            //获得保存模板名称
                            if (Get_ModelXld_Path<string>(ref _Path, _Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i).GetResult())
                            {

                                //保存模型文件
                                HOperatorSet.WriteNccModel(_ModelID, _Path);
                            }

                            //计算区域中心未知
                            HOperatorSet.AreaCenter(ImageRegion, out HTuple _, out HTuple Region_Row, out HTuple Region_Col);
                            //计算移动原点2d矩阵
                            HOperatorSet.VectorAngleToRigid(Region_Row, Region_Col, 0, 0, 0, 0, out HTuple HomMat2D);



                            HOperatorSet.ProjectiveTransContourXld(All_XLD[i], out HObject XLD, HomMat2D);

                            if (Get_ModelXld_Path<string>(ref _Path, _Location, FilePath_Type_Model_Enum.Save, Shape_Based_Model_Enum.Halcon_DXF, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i).GetResult())
                            {


                                //保存模板xld文件
                                HOperatorSet.WriteContourXldDxf(XLD, _Path);





                            }



                        }


                        break;






                }

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Create_Model.Shape_Based_Model.ToString() + "模型创建成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.创建匹配模型失败) { Result_Error_Info = e.Message };

            }
        }

        /// <summary>
        /// 图像提前预处理
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_HWindow"></param>
        /// <param name="_Find_Property"></param>
        public static HPR_Status_Model Halcon_Image_Pre_Processing(ref HObject _Image, HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
        {

            try
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







                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "图像预处理完成！" };


            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.图像预处理错误) { Result_Error_Info = e.Message };


            }





        }




        /// <summary>
        /// 根据用户生产数据生产圆弧XLD 
        /// </summary>
        /// <param name="_Cir"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static HPR_Status_Model Draw_Group_Cir(ref HObject _Cir, List<Point3D> _Point,HWindow _Window)
        {


            HTuple _Row = new HTuple();
            HTuple _Col = new HTuple();


            try
            {


                if (_Point.Count >= 3)
                {


                    foreach (var _P in _Point)
                    {
                        HOperatorSet.TupleConcat(_Row, _P.X, out _Row);
                        HOperatorSet.TupleConcat(_Col, _P.Y, out _Col);
                    }

                    HOperatorSet.GenContourPolygonXld(out HObject Cir_Contour, _Row, _Col);

                    //拟合xld圆弧
                    HOperatorSet.FitCircleContourXld(Cir_Contour, "atukey", -1, 2, 0, 5, 2, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder);
                    //显示xld圆弧
                    HOperatorSet.GenCircleContourXld(out _Cir, hv_Row, hv_Column, hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 0.5);

                    //根据描绘点生产线段
                    //设置显示图像颜色
                    HOperatorSet.SetColor(_Window, nameof(KnownColor.Blue).ToLower());
                    HOperatorSet.SetLineWidth(_Window, 3);

                    //把线段显示到控件窗口
                    HOperatorSet.DispXld(_Cir, _Window);

                }
                else
                {

                    return new HPR_Status_Model(HVE_Result_Enum.添加的圆弧类型不足3点数据_重新添加);

                }






                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "添加圆弧类型特征成功！" };



            }
            catch (Exception e)
            {
                return new HPR_Status_Model(HVE_Result_Enum.添加圆弧类型失败) { Result_Error_Info = e.Message };

            }
        }




        /// <summary>
        /// 所有xld类型集合一起
        /// </summary>
        /// <param name="_All_XLD"></param>
        /// <param name="_Window"></param>
        /// <param name="_XLD_List"></param>
        /// <returns></returns>
        public static HPR_Status_Model Group_All_XLD(ref HObject _All_XLD, HWindow _Window, List<HObject> _XLD_List)
        {



            try
            {

                HOperatorSet.GenEmptyObj(out _All_XLD);
           
                if (_XLD_List.Count > 1)
                {


                    foreach (HObject _Xld in _XLD_List)
                    {

                        HOperatorSet.ConcatObj(_All_XLD, _Xld, out _All_XLD);

                    }


                    //设置显示图像颜色
                    HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());
                    HOperatorSet.SetLineWidth(_Window, 3);

                    //把线段显示到控件窗口
                    HOperatorSet.DispXld(_All_XLD, _Window);

                    return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "XLD类型全部集合成功！" };
                }
                else
                {
                    return new HPR_Status_Model(HVE_Result_Enum.XLD数据集合不足1组以上);

                }

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.XLD数据集合创建失败) { Result_Error_Info = e.Message };
            }

        }





        /// <summary>
        /// 集合数据生成直线类型xld
        /// </summary>
        /// <param name="_Lin"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static HPR_Status_Model Draw_Group_Lin(ref HObject _Lin, List<Point3D> _Point, HWindow _Window)
        {


            HTuple _Row = new HTuple();
            HTuple _Col = new HTuple();



            try
            {

                if (_Point.Count >= 2)
                {


                    foreach (var _P in _Point)
                    {
                        HOperatorSet.TupleConcat(_Row, _P.X, out _Row);
                        HOperatorSet.TupleConcat(_Col, _P.Y, out _Col);
                    }


                    //根据描绘点生产线段
                    HOperatorSet.GenContourPolygonXld(out HObject Lin_Contour, _Row, _Col);

                    //拟合直线
                    HOperatorSet.FitLineContourXld(Lin_Contour, "tukey", -1, 0, 5, 2, out HTuple _RowBegin, out HTuple _ColBegin, out HTuple _RowEnd, out HTuple _ColEnd, out _, out _, out _);

                    //生成xld直线
                    HOperatorSet.GenContourPolygonXld(out _Lin, _RowBegin.TupleConcat(_RowEnd), _ColBegin.TupleConcat(_ColEnd));

                    //设置显示图像颜色
                    HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());
                    HOperatorSet.SetLineWidth(_Window, 3);

                    //把线段显示到控件窗口
                    HOperatorSet.DispXld(_Lin, _Window);


                }
                else
                {

                    return new HPR_Status_Model(HVE_Result_Enum.添加的直线类型不足2点数据_重新添加);

                }


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "添加直线类型特征成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.添加直线类型失败) { Result_Error_Info = e.Message };

            }




        }




        /// <summary>
        /// 根据用户点击位置生产十字数据
        /// </summary>
        /// <param name="_Corss"></param>
        /// <param name="_Window"></param>
        /// <param name="_Row"></param>
        /// <param name="_Col"></param>
        /// <returns></returns>
        public static HPR_Status_Model Draw_Cross(ref HObject _Corss, HWindow _Window, HTuple _Row, HTuple _Col)
        {


            try
            {



                //设置显示样式h
                HOperatorSet.SetColor(_Window, nameof(KnownColor.Red).ToLower());
                HOperatorSet.SetLineWidth(_Window, 1);

                //生成十字架
                HOperatorSet.GenCrossContourXld(out _Corss, _Row, _Col, 50, (new HTuple(45)).TupleRad());
                //显示十字架
                HOperatorSet.DispXld(_Corss, _Window);







                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "X：" + _Row + "，Y：" + _Col + ",添加数据成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.添加数据失败) { Result_Error_Info = e.Message };

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
                                _Find_Out.FInd_Results = true;
                            }
                            else
                            {
                                _Find_Out.Row.Add(0);
                                _Find_Out.Column.Add(0);
                                _Find_Out.Angle.Add(0);
                                _Find_Out.Score.Add(0);
                                _Find_Out.FInd_Results = false ;
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
                            _Find_Out.FInd_Results = true;
                        }
                        else
                        {
                            //_Find_Out = new Halcon_Find_Shape_Out_Parameter() { DispWiindow = _HWindow, Score = new List<double>() { 0}, Find_Time = new List<double>() { (DateTime.Now - RunTime).TotalMilliseconds } };
                            _Find_Out.Score.Add(0);
                            _Find_Out.Find_Time = (DateTime.Now - RunTime).TotalMilliseconds;
                            _Find_Out.FInd_Results = false ;
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
                   




                        for (int i = 0; i < _ModelID.Count; i++)
                        {
                            HTuple _AStart = new HTuple(_Find_Property.AngleStart).TupleRad();
                            HTuple _AExtent = new HTuple(_Find_Property.AngleExtent).TupleRad();


                            HOperatorSet.FindNccModel
                                (_Image,
                                _ModelID[i],
                                _AStart,
                                _AExtent,
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






                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_column, hv_row, hv_angle, out HTuple HMat2D);
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










                        //检查全部模型查找成功
                        if (Find_Results.Where((_W) => _W == true).ToList().Count == 3)
                        {
                            _Find_Out.FInd_Results = true;
                        }
                        else
                        {
                            _Find_Out.FInd_Results = false;

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
