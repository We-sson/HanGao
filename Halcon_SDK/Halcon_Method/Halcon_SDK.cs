using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;


namespace Halcon_SDK_DLL
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_SDK : IDisposable
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
        public HSmartWindowControlWPF Halcon_UserContol { set; get; } = new HSmartWindowControlWPF();




        /// <summary>
        /// 样品图片保存后序号
        /// </summary>
        private static int Sample_Save_Image_Number { set; get; } = 1;

        /// <summary>
        /// 当前匹配文件存档
        /// </summary>
        //public static Match_Models_List_Model Match_Models { set; get; }

        /// <summary>
        /// 设置显示参数
        /// </summary>
        public DisplayDrawColor_Model SetDisplay { set; get; } = new DisplayDrawColor_Model();


        /// <summary>
        /// 绑定图像
        /// </summary>
        public HObject? DisplayImage { set; get; }
        /// <summary>
        /// 绑定区域显示
        /// </summary>
        public HObject? DisplayRegion { set; get; }

        public HObject? DisplayXLD { set; get; }







        /// <summary>
        /// 模型存储列表
        /// </summary>
        public static List<Match_Models_List_Model> Match_Models_List { set; get; } = new List<Match_Models_List_Model>();

        /// <summary>
        /// 保存图像到当前文件
        /// </summary>
        /// <param name="_Image"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Save_Image(HObject _Image)
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

                if (!Directory.Exists(_Path += "\\" + DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    //创建文件夹
                    Directory.CreateDirectory(_Path);
                }


                DirectoryInfo root = new DirectoryInfo(_Path);
                FileInfo Re;
                do
                {
                    _Name = DateTime.Today.ToLongDateString() + "_" + (Sample_Save_Image_Number += 1).ToString();

                    Re = root.GetFiles().Where(F => F.Name.Contains(_Name)).FirstOrDefault()!;


                } while (Re != null);



                HOperatorSet.WriteImage(_Image, "tiff", 0, _Path + "\\" + _Name);


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "图像保存成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.图像保存失败) { Result_Error_Info = e.Message };

            }
        }




        /// <summary>
        /// 保存标定矩阵坐标方法
        /// </summary>
        /// <param name="_HomMat2D">需要保存矩阵对象</param>
        /// <param name="_Name">保存名字</param>
        /// <param name="_Path">保存地址</param>
        public static HPR_Status_Model<bool> Save_Mat2d_Method(HTuple _HomMat2D, string _Path)
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

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "保存矩阵文件成功！" };
            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.保存矩阵文件失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                _HomMatID.Dispose();
                _FileHandle.Dispose();
            }


        }


        /// <summary>
        /// 读取矩阵文件方法
        /// </summary>
        /// <param name="_Mat2D"></param>
        /// <param name="_Name"></param>
        /// <param name="_Path"></param>
        public static HPR_Status_Model<bool> Read_Mat2d_Method(ref HTuple _Mat2D, string Vision_Area, string Work_Area)
        {

            HTuple _FileHandle = new HTuple();

            HTuple _HomMatID = new HTuple();


            try
            {



                //打开文件
                string _Path = Directory.GetCurrentDirectory() + "\\Nine_Calibration\\" + Vision_Area + "_" + Work_Area;


                HOperatorSet.OpenFile(_Path + ".mat", "input_binary", out _FileHandle);

                //从文件中读取序列化项目
                HOperatorSet.FreadSerializedItem(_FileHandle, out _HomMatID);

                //反序列化序列化的同类 2D 转换矩阵
                HOperatorSet.DeserializeHomMat2d(_HomMatID, out _Mat2D);
                //关闭文件
                HOperatorSet.CloseFile(_FileHandle);

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "读取矩阵文件成功！" };
            }
            catch (Exception e)
            {
                return new HPR_Status_Model<bool>(HVE_Result_Enum.读取矩阵文件失败) { Result_Error_Info = e.Message };

            }
            finally
            {
                _FileHandle.Dispose();
                _HomMatID.Dispose();
            }


        }


        /// <summary>
        /// 计算矩阵， 放回结果计算误差方法
        /// </summary>
        /// <param name="Calibration"></param>
        /// <param name="Robot"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Calibration_Results_Compute(ref Point3D _Results, List<Point3D> Calibration, List<Point3D> Robot, ref HTuple HomMat2D)
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

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "实际偏差：X " + Calibration_Error_X_UI + ",Y " + Calibration_Error_Y_UI };
            }
            catch (Exception e)
            {
                return new HPR_Status_Model<bool>(HVE_Result_Enum.计算实际误差失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                Calibration_RowLine.Dispose();
                Calibration_ColLine.Dispose();
                Robot_RowLine.Dispose();
                Robot_ColLine.Dispose();
                _Qx.Dispose();
                _Qy.Dispose();
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
        /// 显示图像中最大的灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="imgee"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> ShowMaxGray_Image(ref HRegion _Region, HImage imgee)
        {
            try
            {
                _Region = new HRegion();

                _Region = imgee.Threshold(new HTuple(254), new HTuple(255));


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.显示最大灰度失败) { Result_Error_Info = e.Message };


            }


        }

        /// <summary>
        /// 显示图像中最小的灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="imgee"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> ShowMinGray_Image(ref HRegion _Region, HImage imgee)
        {
            try
            {
                _Region = new HRegion();

                _Region = imgee.Threshold(new HTuple(0), new HTuple(1));


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.显示最大灰度失败) { Result_Error_Info = e.Message };


            }


        }




        /// <summary>
        /// 海康获取图像指针转换Halcon图像
        /// </summary>
        /// <param name="_Width"></param>
        /// <param name="_Height"></param>
        /// <param name="_pData"></param>
        /// <returns></returns>
        public static bool Mvs_To_Halcon_Image(ref HImage image, int _Width, int _Height, IntPtr _pData)
        {

            try
            {

                image.Dispose();
                //转换halcon图像格式
                image.GenImage1("byte", _Width, _Height, _pData);

                //HOperatorSet.GenImage1(out image, "byte", _Width, _Height, _pData);

                return true;

            }
            catch (Exception e)
            {


                throw new Exception("获取图像指针转换失败！" + "原因：" + e.Message);



            }



        }



        /// <summary>
        /// 路径读取图片
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> HRead_Image(ref HImage _Image, string _Path)
        {

            try
            {

                if (_Path != "")
                {

                    _Image.ReadImage(_Path);
                    //HOperatorSet.ReadImage(out _Image, _Path);



                    return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件图像读取成功！" };
                }
                else
                {
                    return new HPR_Status_Model<bool>(HVE_Result_Enum.读取图像文件格式错误);
                }


            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.读取图像文件格式错误) { Result_Error_Info = e.Message };

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
        public static HPR_Status_Model<bool> Find_Calibration(ref List<Point3D> _Calibration_Point, HWindow _Window, HObject _Input_Image, Halcon_Find_Calibration_Model _Calibration_Data)
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
                    HOperatorSet.SetLineWidth(_Window, 3);

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

                    return new HPR_Status_Model<bool>(HVE_Result_Enum.标定查找9点位置区域失败);
                }





                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "查找9点标定区域成功！" };

            }
            catch (Exception e)
            {
                return new HPR_Status_Model<bool>(HVE_Result_Enum.标定查找9点位置区域失败) { Result_Error_Info = e.Message };

            }
            finally
            {
                _Image.Dispose();
            }

        }








        /// <summary>
        /// 根据模型类型获得模型文件地址
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_Model_Enum"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Get_ModelXld_Path<T1>(ref T1 _path, string _Location, FilePath_Type_Model_Enum _FilePath_Type, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID, int _Number = 0)
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

                            string[] NameList = _FileName.Name.Split('.')[0].Split('_');

                            if (NameList[0] == _ID.ToString() && NameList[2] == _Name.ToString().Split('_')[1] && NameList[3] == ((int)_Model_Enum).ToString())
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




















                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件路径读取成功！" };
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return new HPR_Status_Model<bool>(HVE_Result_Enum.文件路径提取失败) { Result_Error_Info = Save_Path };

            }

        }


        /// <summary>
        /// 根据用户生产数据生产圆弧XLD 
        /// </summary>
        /// <param name="_Cir"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Draw_Group_Cir(ref HObject _Cir, List<Point3D> _Point, HWindow _Window)
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

                    return new HPR_Status_Model<bool>(HVE_Result_Enum.添加的圆弧类型不足3点数据_重新添加);

                }






                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "添加圆弧类型特征成功！" };



            }
            catch (Exception e)
            {
                return new HPR_Status_Model<bool>(HVE_Result_Enum.添加圆弧类型失败) { Result_Error_Info = e.Message };

            }
            finally
            {
                _Row.Dispose();
                _Col.Dispose();
            }
        }








        /// <summary>
        /// 集合数据生成直线类型xld
        /// </summary>
        /// <param name="_Lin"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static HPR_Status_Model<bool> Draw_Group_Lin(ref HObject _Lin, List<Point3D> _Point, HWindow _Window)
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

                    return new HPR_Status_Model<bool>(HVE_Result_Enum.添加的直线类型不足2点数据_重新添加);

                }


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "添加直线类型特征成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.添加直线类型失败) { Result_Error_Info = e.Message };

            }
            finally
            {
                _Row.Dispose();
                _Col.Dispose();
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
        public static HPR_Status_Model<bool> Draw_Cross(ref HObject _Corss, HWindow _Window, HTuple _Row, HTuple _Col)
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







                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "X：" + _Row + "，Y：" + _Col + ",添加数据成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.添加数据失败) { Result_Error_Info = e.Message };

            }
        }









        /// <summary>
        /// GC回收处理方法
        /// </summary>
        public void Dispose()
        {


            //_Image.Dispose();
            GC.Collect();
            GC.SuppressFinalize(this);

            HWindow?.Dispose();
            Halcon_UserContol?.Dispose();
            DisplayImage?.Dispose();
            DisplayRegion?.Dispose();
            DisplayXLD?.Dispose();
        }
    }

    [AddINotifyPropertyChangedInterface]

    public class Halcon_Window_Display_Model : IDisposable
    {



        public Halcon_SDK HandEye_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_1 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_Results_Window_2 { set; get; } = new Halcon_SDK();
        public Halcon_SDK HandEye_3DResults { set; get; } = new Halcon_SDK();


        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Live_Window { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Features_Window { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Results_Window_1 { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Results_Window_2 { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Results_Window_3 { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Results_Window_4 { set; get; } = new Halcon_SDK();




        /// <summary>
        /// 可视化三维显示模型
        /// </summary>
        public Halcon_SDK Calibration_3D_Results { set; get; } = new Halcon_SDK();
        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_1 { set; get; } = new Halcon_SDK();


        /// <summary>
        /// 实施相机视角控件
        /// </summary>
        public Halcon_SDK Calibration_Window_2 { set; get; } = new Halcon_SDK();



        /// <summary>
        /// 设置窗口控件显示对象
        /// </summary>
        /// <param name="_HImage"></param>
        /// <param name="_Region"></param>
        /// <param name="_XLD"></param>
        /// <param name="_DrawColor"></param>
        /// <param name="_Show"></param>
        public void Display_HObject(HObject _HImage, HObject _Region, HObject _XLD, string _DrawColor, Window_Show_Name_Enum _Show)
        {
            if (_DrawColor != null)
            {
                SetHDrawColor(_DrawColor, DisplaySetDraw_Enum.fill, _Show);
            }


            if (_HImage != null)
            {
                SetWindowDisoplay(_HImage, Display_HObject_Type_Enum.Image, _Show);

            }
            if (_Region != null)
            {

                SetWindowDisoplay(_Region, Display_HObject_Type_Enum.Region, _Show);
            }

            if (_XLD != null)
            {

                SetWindowDisoplay(_XLD, Display_HObject_Type_Enum.XLD, _Show);
            }



        }



        /// <summary>
        /// 设置窗口显示颜色
        /// </summary>
        /// <param name="HColor"></param>
        /// <param name="HDraw"></param>
        /// <param name="_Window"></param>
        public void SetHDrawColor(string HColor, DisplaySetDraw_Enum HDraw, Window_Show_Name_Enum _Window)
        {
            //根据窗口枚举属性设置
            switch (_Window)
            {

                case Window_Show_Name_Enum.HandEye_Window_1:
                    HandEye_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.HandEye_Window_2:
                    HandEye_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_1:
                    HandEye_Results_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_2:
                    HandEye_Results_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.HandEye_3DResults:
                    HandEye_3DResults.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };

                    break;
                case Window_Show_Name_Enum.Live_Window:
                    Live_Window.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Features_Window:
                    Features_Window.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Results_Window_1:
                    Results_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Results_Window_2:
                    Results_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Results_Window_3:
                    Results_Window_3.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Results_Window_4:
                    Results_Window_4.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Calibration_Window_1:
                    Calibration_Window_1.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Calibration_Window_2:
                    Calibration_Window_2.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
                case Window_Show_Name_Enum.Calibration_3D_Results:
                    Calibration_3D_Results.SetDisplay = new DisplayDrawColor_Model() { SetColor = HColor, SetDraw = HDraw };
                    break;
            }


        }




        /// <summary>
        /// 设置窗口显示对象
        /// </summary>
        /// <param name="_S"></param>
        public void SetWindowDisoplay(HObject _Dispaly, Display_HObject_Type_Enum _Type, Window_Show_Name_Enum _Window)
        {

            HOperatorSet.SetSystem("flush_graphic", "false");
            Halcon_SDK _WindowDisplay = new Halcon_SDK();

            //根据窗口枚举属性设置
            switch (_Window)
            {

                case Window_Show_Name_Enum.HandEye_Window_1:
                    _WindowDisplay = HandEye_Window_1;
                    break;
                case Window_Show_Name_Enum.HandEye_Window_2:
                    _WindowDisplay = HandEye_Window_2;
                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_1:
                    _WindowDisplay = HandEye_Results_Window_1;
                    break;
                case Window_Show_Name_Enum.HandEye_Results_Window_2:
                    _WindowDisplay = HandEye_Results_Window_2;
                    break;
                case Window_Show_Name_Enum.Live_Window:
                    _WindowDisplay = Live_Window;
                    break;
                case Window_Show_Name_Enum.Features_Window:
                    _WindowDisplay = Features_Window;
                    break;
                case Window_Show_Name_Enum.Results_Window_1:
                    _WindowDisplay = Results_Window_1;
                    break;
                case Window_Show_Name_Enum.Results_Window_2:
                    _WindowDisplay = Results_Window_2;
                    break;
                case Window_Show_Name_Enum.Results_Window_3:
                    _WindowDisplay = Results_Window_3;
                    break;
                case Window_Show_Name_Enum.Results_Window_4:
                    _WindowDisplay = Results_Window_4;
                    break;
                case Window_Show_Name_Enum.Calibration_Window_1:
                    _WindowDisplay = Calibration_Window_1;
                    break;
                case Window_Show_Name_Enum.Calibration_Window_2:
                    _WindowDisplay = Calibration_Window_2;
                    break;
                case Window_Show_Name_Enum.Calibration_3D_Results:
                    _WindowDisplay = Calibration_3D_Results;
                    break;
                case Window_Show_Name_Enum.HandEye_3DResults:
                    _WindowDisplay = HandEye_3DResults;
                    break;
            }


            //根据显示类型设置
            switch (_Type)
            {
                case Display_HObject_Type_Enum.Image:


                    _WindowDisplay.DisplayImage = _Dispaly;
                    break;
                case Display_HObject_Type_Enum.Region:
                    _WindowDisplay.DisplayRegion = _Dispaly;

                    break;

                case Display_HObject_Type_Enum.XLD:

                    _WindowDisplay.DisplayXLD = _Dispaly;

                    break;

                case Display_HObject_Type_Enum.SetDrawColor:
                    //_WindowDisplay.SetDisplay = _Dispaly;

                    break;
            }

            HOperatorSet.SetSystem("flush_graphic", "true");
        }





        public void Dispose()
        {

            //_Image.Dispose();
            GC.Collect();
            GC.SuppressFinalize(this);

            HandEye_Window_1?.Dispose();
            HandEye_Window_2?.Dispose();
            HandEye_Results_Window_1?.Dispose();
            HandEye_Results_Window_2?.Dispose();
            HandEye_3DResults?.Dispose();
            Calibration_3D_Results?.Dispose();
            Calibration_Window_1?.Dispose();
            Calibration_Window_2?.Dispose();
        }
    }


    [AddINotifyPropertyChangedInterface]
    public class Halcon_Method_Model : IDisposable
    {
        /// <summary>
        /// 处理图像
        /// </summary>
        public HObject _HImage = new HObject();


        /// <summary>
        /// xld对象组
        /// </summary>
        public List<HObject> All_XLd = new List<HObject>();


        /// <summary>
        /// xld集合对象
        /// </summary>
        public HObject _ModelsXld = new HObject();

        /// <summary>
        /// 模型ID
        /// </summary>
        public HTuple Shape_ID = new HTuple();


        /// <summary>
        /// 模型显示
        /// </summary>
        public HObject Shape_ModelContours = new HObject();


        /// <summary>
        /// 模型文件集合
        /// </summary>
        public List<FileInfo> _ModelIDS { set; get; } = new List<FileInfo>();



        /// <summary>
        /// 模型保存地址
        /// </summary>
        public string Shape_Save_Path { set; get; } = "";



        /// <summary>
        /// 显示标定板位置和坐标系
        /// </summary>
        /// <param name="_CalibXLD"></param>
        /// <param name="_CalibCoord"></param>
        /// <param name="_CalibSetup_ID"></param>
        /// <param name="_HImage"></param>
        /// <param name="_CameraID"></param>
        /// <param name="_CalibID"></param>
        public static void FindCalib_3DCoord(ref HXLDCont _CalibXLD, ref HObject _CalibCoord, ref HCalibData _CalibSetup_ID, HImage _HImage, int _CameraID, int _CalibID, double _SigmaVal, int _CalobPosNO = 0)
        {

            HTuple hv_Row = new HTuple();
            HTuple hv_Column = new HTuple();
            HTuple hv_I = new HTuple();
            HTuple hv_Pose = new HTuple();

            try
            {

                //查找标定板
                _CalibSetup_ID.FindCalibObject(_HImage, _CameraID, _CalibID, _CalobPosNO, new HTuple("sigma"), _SigmaVal);
                //读取标定板轮廓
                _CalibXLD = _CalibSetup_ID.GetCalibDataObservContours("marks", _CameraID, _CalibID, _CalobPosNO);

                //获得标定板位置信息
                _CalibSetup_ID.GetCalibDataObservPoints(_CameraID, _CalibID, _CalobPosNO, out hv_Row, out hv_Column, out hv_I, out hv_Pose);
                //读取初始化相机内参
                HTuple _CamerPar = _CalibSetup_ID.GetCalibData("camera", _CameraID, "init_params");
                //显示标定板三维坐标位置
                _CalibCoord = Halcon_Example.Disp_3d_coord(_CamerPar, hv_Pose, new HTuple(0.02));

                //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) {  };


            }
            catch (HalconException e)
            {
                //错误清空
                _CalibXLD.Dispose();
                _CalibCoord.Dispose();
                throw new HalconException(HVE_Result_Enum.标定板图像识别错误.ToString() + " 原因：" + e.Message);
            }
            finally
            {
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_I.Dispose();
                hv_Pose.Dispose();
            }




        }





        /// <summary>
        /// 图像预处理
        /// </summary>
        /// <param name="_HWindow"></param>
        /// <param name="_Find_Property"></param>
        /// <returns></returns>
        public HPR_Status_Model<bool> Halcon_Image_Pre_Processing(HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
        {

            HObject _Image = new HObject(_HImage);
            try
            {
                _HWindow.DispObj(_HImage);


                if (_Find_Property.ScaleImageMax_Enable)
                {
                    //图像最大灰度值分布在值范围0到255 中
                    HOperatorSet.ScaleImageMax(_HImage, out _HImage);
                    //_HImage.ScaleImageMax();


                    if (_Find_Property.ScaleImageMax_Disp)
                    {
                        _HWindow.DispObj(_HImage);
                    }
                }


                if (_Find_Property.Median_image_Enable)
                {
                    //进行图像中值滤波器平滑
                    //_HImage.MedianImage(_Find_Property.MaskType_Model.ToString(), _Find_Property.Median_image_Radius, _Find_Property.Margin_Model.ToString());
                    HOperatorSet.MedianImage(_HImage, out _HImage, _Find_Property.MaskType_Model.ToString(), _Find_Property.Median_image_Radius, _Find_Property.Margin_Model.ToString());
                    if (_Find_Property.Median_image_Disp)
                    {
                        _HWindow.DispObj(_HImage);


                    }

                }



                if (_Find_Property.MedianRect_Enable)
                {
                    //进行图像中值滤波器平滑

                    //_HImage.MedianRect(_Find_Property.MedianRect_MaskWidth, _Find_Property.MedianRect_MaskHeight);
                    HOperatorSet.MedianRect(_HImage, out _HImage, _Find_Property.MedianRect_MaskWidth, _Find_Property.MedianRect_MaskHeight);
                    if (_Find_Property.MedianRect_Disp)
                    {
                        _HWindow.DispObj(_HImage);

                    }
                }




                if (_Find_Property.Illuminate_Enable)
                {
                    //高频增强图像的对比度
                    //_HImage.Illuminate(_Find_Property.Illuminate_MaskWidth, _Find_Property.Illuminate_MaskHeight, _Find_Property.Illuminate_Factor);
                    HOperatorSet.Illuminate(_HImage, out _HImage, _Find_Property.Illuminate_MaskWidth, _Find_Property.Illuminate_MaskHeight, _Find_Property.Illuminate_Factor);
                    if (_Find_Property.Illuminate_Disp)
                    {
                        _HWindow.DispObj(_HImage);
                    }

                }



                if (_Find_Property.Emphasize_Enable)
                {
                    //增强图像的对比度

                    //_HImage.Emphasize(_Find_Property.Emphasize_MaskWidth, _Find_Property.Emphasize_MaskHeight, _Find_Property.Emphasize_Factor);
                    HOperatorSet.Emphasize(_HImage, out _HImage, _Find_Property.Emphasize_MaskWidth, _Find_Property.Emphasize_MaskHeight, _Find_Property.Emphasize_Factor);
                    if (_Find_Property.Emphasize_Disp)
                    {
                        _HWindow.DispObj(_HImage);



                    }

                }

                if (_Find_Property.GrayOpeningRect_Enable)
                {
                    //_HImage.GrayOpeningRect(_Find_Property.GrayOpeningRect_MaskHeight, _Find_Property.GrayOpeningRect_MaskWidth);
                    //进行图像灰度开运算
                    HOperatorSet.GrayOpeningRect(_HImage, out _HImage, _Find_Property.GrayOpeningRect_MaskHeight, _Find_Property.GrayOpeningRect_MaskWidth);
                    if (_Find_Property.GrayOpeningRect_Disp)
                    {
                        _HWindow.DispObj(_HImage);

                    }
                }


                if (_Find_Property.GrayClosingRect_Enable)
                {
                    //_HImage.GrayClosingRect(_Find_Property.GrayClosingRect_MaskHeight, _Find_Property.GrayClosingRect_MaskWidth);
                    //进行图像灰度开运算
                    HOperatorSet.GrayClosingRect(_HImage, out _HImage, _Find_Property.GrayClosingRect_MaskHeight, _Find_Property.GrayClosingRect_MaskWidth);
                    if (_Find_Property.GrayClosingRect_Disp)
                    {
                        _HWindow.DispObj(_HImage);

                    }
                }



                //_Image = _HImage.CopyObj(1, -1);
                //_HImage.Dispose();
                GC.Collect();

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "图像预处理完成！" };


            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.图像预处理错误) { Result_Error_Info = e.Message };


            }
            finally
            {
                _Image.Dispose();
            }





        }



        /// <summary>
        /// 所有xld类型集合一起
        /// </summary>
        /// <param name="_All_XLD"></param>
        /// <param name="_Window"></param>
        /// <param name="_XLD_List"></param>
        /// <returns></returns>
        public HPR_Status_Model<bool> Group_All_XLD(HWindow _Window, List<HObject> _XLD_List)
        {



            HOperatorSet.GenEmptyObj(out _ModelsXld);
            try
            {


                if (_XLD_List.Count > 1)
                {


                    foreach (HObject _Xld in _XLD_List)
                    {

                        HOperatorSet.ConcatObj(_ModelsXld, _Xld, out _ModelsXld);


                    }


                    //设置显示图像颜色
                    HOperatorSet.SetColor(_Window, nameof(KnownColor.Green).ToLower());
                    HOperatorSet.SetLineWidth(_Window, 3);

                    //把线段显示到控件窗口
                    HOperatorSet.DispXld(_ModelsXld, _Window);

                    return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "XLD类型全部集合成功！" };
                }
                else
                {
                    return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD数据集合不足1组以上);

                }

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD数据集合创建失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                //_ModelsXld.Dispose();
            }

        }

        /// <summary>
        /// 根据模型类型获得模型文件地址
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_Model_Enum"></param>
        /// <returns></returns>
        public HPR_Status_Model<bool> SetGet_ModelXld_Path(string _Location, FilePath_Type_Model_Enum _FilePath_Type, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID, int _Number = 0)
        {

            ////获得识别位置名称
            if (_Location != "")
            {



                switch (_FilePath_Type)
                {
                    case FilePath_Type_Model_Enum.Get:


                        DirectoryInfo _FileInfo = new DirectoryInfo(_Location);


                        foreach (FileInfo _FileName in _FileInfo.GetFiles())
                        {

                            string[] NameList = _FileName.Name.Split('.')[0].Split('_');

                            if (NameList[0] == _ID.ToString() && NameList[2] == _Name.ToString().Split('_')[1] && NameList[3] == ((int)_Model_Enum).ToString())
                            {

                                _ModelIDS.Add(_FileName);
                            }

                        }

                        break;
                    case FilePath_Type_Model_Enum.Save:



                        Shape_Save_Path = _Location + "\\" + _ID.ToString() + "_" + _Name;

                        //路径添加格式后缀
                        switch (_Model_Enum)
                        {
                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:

                                Shape_Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".shm";

                                break;

                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:

                                Shape_Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".dfm";
                                break;

                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:

                                Shape_Save_Path += "_" + ((int)_Model_Enum).ToString() + "_" + _Number + ".ncm";
                                break;
                            case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Halcon_DXF:

                                Shape_Save_Path += "_" + ((int)Shape_Based_Model_Enum.Ncc_Model).ToString() + "_" + _Number + ".dxf";
                                break;
                        }


                        break;

                }



                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件路径读取成功！" };
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return new HPR_Status_Model<bool>(HVE_Result_Enum.文件路径提取失败) { Result_Error_Info = Shape_Save_Path };

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
        public HPR_Status_Model<bool> ShapeModel_SaveFile(string _Location, Create_Shape_Based_ModelXld _Create_Model)
        {


            ///此版本修改序列化读取

            //string _Path = "";
            //_ModelID.Dispose();

            HObject Gen_Polygons = new HObject();
            HObject Region_Unio = new HObject();
            HObject Select_Region = new HObject();
            HObject Gen_Region = new HObject();
            HObject Polygon_Xld = new HObject();
            HObject Region_Dilation = new HObject();
            HObject All_Reduced = new HObject();
            HObject XLD_1 = new HObject();
            HObject XLD_2 = new HObject();
            HObject XLD_3 = new HObject();
            HObject Region_Unio_1 = new HObject();
            HObject Region_Unio_2 = new HObject();
            HObject Region_Unio_3 = new HObject();
            HObject ImageRegion = new HObject();
            HObject Dilation_Region = new HObject();
            HObject DXF_XLD = new HObject();
            HOperatorSet.GenEmptyObj(out Polygon_Xld);



            HTuple _Pos_Row = new HTuple();
            HTuple _Pos_Col = new HTuple();
            HRegion D_Region = new HRegion();
            HTuple _Serializd = new HTuple();
            HTuple _HFile = new HTuple();
            HTuple Region_Row = new HTuple();
            HTuple Region_Col = new HTuple();
            HTuple HomMat2D = new HTuple();

            List<HObject> All_XLD = new List<HObject>();
            List<HObject> All_Region = new List<HObject>();

            try
            {


                switch (_Create_Model.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:



                        //获得保存模型文件地址
                        if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {






                            //创建模型
                            HOperatorSet.CreateShapeModelXld(_ModelsXld, _Create_Model.NumLevels,
                                                    (new HTuple(_Create_Model.AngleStart)).TupleRad(),
                                                    (new HTuple(_Create_Model.AngleExtent)).TupleRad(),
                                                    (new HTuple(_Create_Model.AngleStep)).TupleRad(),
                                                    _Create_Model.Optimization.ToString(), _Create_Model.Metric.ToString(),
                                                    _Create_Model.MinContrast,
                                                    out Shape_ID);



                            //保存模型文件
                            HOperatorSet.WriteShapeModel(Shape_ID, Shape_Save_Path);


                            //清楚内存
                            HOperatorSet.ClearShapeModel(Shape_ID);


                        }

                        break;
                    case Shape_Based_Model_Enum.planar_deformable_model:

                        //获得保存模型文件地址
                        if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            //创建模型
                            HOperatorSet.CreatePlanarUncalibDeformableModelXld(
                                _ModelsXld, _Create_Model.NumLevels,
                                (new HTuple(_Create_Model.AngleStart)).TupleRad(),
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
                                 out Shape_ID);

                            //保存模型文件
                            HOperatorSet.WriteDeformableModel(Shape_ID, Shape_Save_Path);

                            //清楚模型
                            HOperatorSet.ClearDeformableModel(Shape_ID);


                        }
                        break;
                    case Shape_Based_Model_Enum.local_deformable_model:

                        //获得保存模型文件地址
                        if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            HOperatorSet.CreateLocalDeformableModelXld(_ModelsXld, _Create_Model.NumLevels,
                                (new HTuple(_Create_Model.AngleStart)).TupleRad(),
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
                                new HTuple(), new HTuple(),
                                 out Shape_ID);


                            //保存模型文件
                            HOperatorSet.WriteDeformableModel(Shape_ID, Shape_Save_Path);

                            //清楚模型
                            HOperatorSet.ClearDeformableModel(Shape_ID);


                        }
                        break;
                    case Shape_Based_Model_Enum.Scale_model:

                        //获得保存模型文件地址
                        if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID).GetResult())
                        {

                            //创建模型
                            HOperatorSet.CreateScaledShapeModelXld(_ModelsXld,
                                _Create_Model.NumLevels,
                                (new HTuple(_Create_Model.AngleStart)).TupleRad(),
                                (new HTuple(_Create_Model.AngleExtent)).TupleRad(),
                                (new HTuple(_Create_Model.AngleStep)).TupleRad(),
                                _Create_Model.ScaleMin,
                                _Create_Model.ScaleMax,
                                _Create_Model.ScaleStep,
                                _Create_Model.Optimization.ToString(),
                                _Create_Model.Metric.ToString(),
                                 _Create_Model.MinContrast,
                                 out Shape_ID);

                            //保存模型文件
                            HOperatorSet.WriteShapeModel(Shape_ID, Shape_Save_Path);

                            //清楚内存
                            HOperatorSet.ClearShapeModel(Shape_ID);


                        }

                        break;

                    case Shape_Based_Model_Enum.Ncc_Model:






                        //每个xld转换多边形类型 
                        for (int X = 0; X < _ModelsXld.CountObj(); X++)
                        {
                            //分解xld图像点为多边形
                            HOperatorSet.GenPolygonsXld(_ModelsXld.SelectObj(X + 1), out Select_Region, "ramer", 0.1);
                            //获得分解点的多边形坐标数据
                            HOperatorSet.GetPolygonXld(Select_Region, out _Pos_Row, out _Pos_Col, out HTuple _, out HTuple _);
                            //将多边形转换区域
                            HOperatorSet.GenRegionPolygon(out Gen_Region, _Pos_Row, _Pos_Col);
                            //存入集合中
                            HOperatorSet.ConcatObj(Polygon_Xld, Gen_Region, out Polygon_Xld);


                        }

                        //膨胀全部区域
                        HOperatorSet.DilationCircle(Polygon_Xld, out Dilation_Region, _Create_Model.DilationCircle);


                        //转换区域类型
                        D_Region = new HRegion(Dilation_Region);


                        //xld集合
                        All_XLD = new List<HObject>
                    {
                        new HObject(_ModelsXld.SelectObj(1)).ConcatObj(_ModelsXld.SelectObj(2)),
                        new HObject(_ModelsXld.SelectObj(3)).ConcatObj(_ModelsXld.SelectObj(2)),
                        new HObject(_ModelsXld.SelectObj(4)).ConcatObj(_ModelsXld.SelectObj(5).ConcatObj(_ModelsXld.SelectObj(2))),

                    };


                        //区域集合
                        All_Region = new List<HObject>
                    {
                        new HObject(D_Region.SelectObj(1).Union2(D_Region.SelectObj(2))),
                        new HObject(D_Region.SelectObj(3).Union2(D_Region.SelectObj(2))),
                        new HObject(D_Region.SelectObj(4).Union2(D_Region.SelectObj(5)).Union2(D_Region.SelectObj(2))),

                    };






                        //创建并保存模型文件
                        for (int i = 0; i < All_XLD.Count; i++)
                        {
                            //抠图出
                            HOperatorSet.ReduceDomain(_HImage, All_Region[i], out ImageRegion);


                            //创建NCC模板
                            HOperatorSet.CreateNccModel(ImageRegion,
                                _Create_Model.NumLevels,
                                (new HTuple(_Create_Model.AngleStart)).TupleRad(),
                                (new HTuple(_Create_Model.AngleExtent)).TupleRad(),
                                (new HTuple(_Create_Model.AngleStep)).TupleRad(),
                                _Create_Model.Metric.ToString(),
                                out Shape_ID);


                            //获得保存模板名称
                            if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, _Create_Model.Shape_Based_Model, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i).GetResult())
                            {

                                //保存模型文件
                                //HOperatorSet.WriteNccModel(_ModelID, _Path);
                                //模型序列化
                                HOperatorSet.SerializeNccModel(Shape_ID, out _Serializd);
                                //打开文件
                                HOperatorSet.OpenFile(Shape_Save_Path, HFIle_Type_Enum.output_binary.ToString(), out _HFile);
                                //二进制文件保存
                                HOperatorSet.FwriteSerializedItem(_HFile, _Serializd);
                                //关闭文件
                                HOperatorSet.CloseFile(_HFile);


                                //清楚模型
                                HOperatorSet.ClearNccModel(Shape_ID);


                            }

                            //计算区域中心未知
                            HOperatorSet.AreaCenter(ImageRegion, out HTuple _, out Region_Row, out Region_Col);
                            //计算移动原点2d矩阵
                            HOperatorSet.VectorAngleToRigid(Region_Row, Region_Col, 0, 0, 0, 0, out HomMat2D);



                            HOperatorSet.ProjectiveTransContourXld(All_XLD[i], out DXF_XLD, HomMat2D);

                            if (SetGet_ModelXld_Path(_Location, FilePath_Type_Model_Enum.Save, Shape_Based_Model_Enum.Halcon_DXF, _Create_Model.ShapeModel_Name, _Create_Model.Create_ID, i).GetResult())
                            {


                                //保存模板xld文件
                                HOperatorSet.WriteContourXldDxf(DXF_XLD, Shape_Save_Path);


                                //清楚模型
                                DXF_XLD.Dispose();

                            }



                        }





                        break;






                }

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Create_Model.Shape_Based_Model.ToString() + "模型创建成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.创建匹配模型失败) { Result_Error_Info = e.Message };

            }
            finally
            {
                //清楚内存


                All_XLD.ForEach(_M => _M.Dispose());
                All_Region.ForEach(_M => _M.Dispose());
                _ModelsXld.Dispose();

                //_ModelID.Dispose();
                _Serializd.Dispose();
                _HFile.Dispose();
                Gen_Polygons.Dispose();
                Region_Unio.Dispose();
                Select_Region.Dispose();
                Gen_Region.Dispose();
                Polygon_Xld.Dispose();
                Region_Dilation.Dispose();
                All_Reduced.Dispose();
                XLD_1.Dispose();
                XLD_2.Dispose();
                XLD_3.Dispose();
                Region_Unio_1.Dispose();
                Region_Unio_2.Dispose();
                Region_Unio_3.Dispose();
                ImageRegion.Dispose();
                Dilation_Region.Dispose();
                DXF_XLD.Dispose();
                _Pos_Row.Dispose();
                _Pos_Col.Dispose();
                D_Region.Dispose();
                _Serializd.Dispose();
                _HFile.Dispose();
                Region_Row.Dispose();
                Region_Col.Dispose();
                HomMat2D.Dispose();

                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
        }





        /// <summary>
        /// 读取Halcon文件类型
        /// </summary>
        /// <param name="_Model"></param>
        /// <param name="_ModelContours"></param>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public HPR_Status_Model<bool> ShapeModel_ReadFile(FileInfo _Path)
        {

            HTuple _Serialized = new HTuple();
            HTuple _HFile = new HTuple();
            try
            {

                DateTime _Run = DateTime.Now;
                Console.WriteLine("开始:" + (DateTime.Now - _Run).TotalSeconds);





                string[] _Name = _Path.Name.Split('.');
                switch (_Name[1])
                {
                    case "ncm":


                        //反序列化读取模型文件
                        HOperatorSet.OpenFile(_Path.FullName, HFIle_Type_Enum.input_binary.ToString(), out _HFile);

                        HOperatorSet.FreadSerializedItem(_HFile, out _Serialized);


                        HOperatorSet.DeserializeNccModel(_Serialized, out Shape_ID);

                        //HOperatorSet.ReadNccModel(_Path.FullName, out _Model);
                        HOperatorSet.GetNccModelRegion(out Shape_ModelContours, Shape_ID);

                        HOperatorSet.CloseFile(_HFile);

                        HOperatorSet.ClearSerializedItem(_Serialized);

                        //Task.Delay(1000);

                        //HOperatorSet.ClearNccModel(Shape_ID);

                        //Task.Delay(5000);

                        break;
                    case "dxf":

                        //读取文件
                        HOperatorSet.ReadContourXldDxf(out Shape_ModelContours, _Path.FullName, new HTuple(), new HTuple(), out _);


                        break;

                    case "dfm":

                        HOperatorSet.ReadDeformableModel(_Path.FullName, out Shape_ID);
                        HOperatorSet.GetDeformableModelContours(out Shape_ModelContours, Shape_ID, 1);


                        break;


                    case "shm":

                        HOperatorSet.ReadShapeModel(_Path.FullName, out Shape_ID);
                        HOperatorSet.GetShapeModelContours(out Shape_ModelContours, Shape_ID, 1);




                        break;
                }


                //_ModelContours.Dispose();
                //_Model.Dispose();
                //_ModelContours = _Contours.CopyObj(1, -1);
                //_Contours.Dispose();

                Console.WriteLine("结束:" + (DateTime.Now - _Run).TotalSeconds);


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Path.Name + "Halcon文件读取成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Halcon文件类型读取失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                _HFile.Dispose();
                _Serialized.Dispose();

                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
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
        public HPR_Status_Model<bool> Find_Deformable_Model(ref Find_Shape_Results_Model _Find_Out, HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
        {

            //HTuple HMat2D = new HTuple();
            HTuple hv_row = new HTuple();
            HTuple hv_column = new HTuple();
            HTuple hv_angle = new HTuple();
            HTuple hv_score = new HTuple();
            HTuple hv_HomMat2D = new HTuple();


            try
            {


                //图像预处理
                if (Halcon_Image_Pre_Processing(_HWindow, _Find_Property).GetResult())
                {

                    //根据匹配类型进行匹配
                    switch (_Find_Property.Shape_Based_Model)
                    {
                        case Shape_Based_Model_Enum.shape_model:

                            HOperatorSet.FindShapeModel(_HImage,
                                Shape_ID,
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


                            //清楚内存
                            //HOperatorSet.ClearShapeModel(Shape_ID);
                            // 查找模型成功保存结果数据
                            if (hv_score.Length != 0)
                            {

                                _Find_Out.Row.Add(hv_row.Clone());
                                _Find_Out.Column.Add(hv_column.Clone());
                                _Find_Out.Angle.Add(hv_angle.Clone());
                                _Find_Out.Score.Add(hv_score.Clone());
                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_row, hv_column, hv_angle, out hv_HomMat2D);
                                _Find_Out.HomMat2D.Add(hv_HomMat2D.Clone());
                                _Find_Out.FInd_Results.Add(true);
                            }
                            else
                            {
                                _Find_Out.Row.Add(0);
                                _Find_Out.Column.Add(0);
                                _Find_Out.Angle.Add(0);
                                _Find_Out.Score.Add(0);
                                _Find_Out.FInd_Results.Add(false);
                            }



                            //}



                            break;



                        case Shape_Based_Model_Enum.planar_deformable_model:



                            //foreach (var _ModelXld in _ModelID)
                            //{


                            //查找模型
                            HOperatorSet.FindPlanarUncalibDeformableModel
                                (_HImage,
                                Shape_ID,
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

                            //清楚模型
                            //HOperatorSet.ClearDeformableModel(Shape_ID);
                            //}

                            if (hv_score.Length != 0)
                            {

                                _Find_Out.HomMat2D.Add(hv_HomMat2D.Clone());
                                _Find_Out.Score.Add(hv_score.Clone());
                                _Find_Out.FInd_Results.Add(true);
                            }
                            else
                            {
                                _Find_Out.Score.Add(0);
                                _Find_Out.FInd_Results.Add(false);
                            }


                            break;
                        case Shape_Based_Model_Enum.local_deformable_model:



                            break;
                        case Shape_Based_Model_Enum.Scale_model:



                            break;


                        case Shape_Based_Model_Enum.Ncc_Model:

                            //识别结果对象初始化
                            //List<bool> Find_Results = new List<bool>();
                            //for (int i = 0; i < _ModelID.Count; i++)
                            //{
                            //    Find_Results.Add(false);
                            //}

                            //筛选所需要的模型数据
                            DateTime _Run1 = DateTime.Now;
                            Console.WriteLine("ncc开始:" + (DateTime.Now - _Run1).TotalSeconds);



                            //for (int i = 0; i < _ModelID.Count; i++)
                            //{
                            HTuple _AStart = new HTuple(_Find_Property.AngleStart).TupleRad();
                            HTuple _AExtent = new HTuple(_Find_Property.AngleExtent).TupleRad();


                            HOperatorSet.FindNccModel
                                (_HImage,
                                Shape_ID,
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


                            Console.WriteLine("ncc结束:" + (DateTime.Now - _Run1).TotalSeconds);

                            //清楚模型
                            //HOperatorSet.ClearNccModel(Shape_ID);

                            // 查找模型成功保存结果数据
                            if (hv_score.Length != 0)
                            {

                                _Find_Out.FInd_Results.Add(true);
                                _Find_Out.Row.Add(hv_row.Clone());
                                _Find_Out.Column.Add(hv_column.Clone());
                                _Find_Out.Angle.Add(hv_angle.Clone());
                                _Find_Out.Score.Add(hv_score.Clone());

                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_column, hv_row, hv_angle, out hv_HomMat2D);
                                _Find_Out.HomMat2D.Add(hv_HomMat2D.Clone());





                            }
                            else
                            {
                                _Find_Out.FInd_Results.Add(false);
                                _Find_Out.Row.Add(0);
                                _Find_Out.Column.Add(0);
                                _Find_Out.Angle.Add(0);
                                _Find_Out.Score.Add(0);
                            }


                            //}


                            ////检查全部模型查找成功
                            //if (Find_Results.Where((_W) => _W == true).ToList().Count == 3)
                            //{
                            //    _Find_Out.FInd_Results = true;

                            //}
                            //else
                            //{
                            //    _Find_Out.FInd_Results = false;

                            //}


                            break;
                    }

                    DateTime _Run = DateTime.Now;
                    Console.WriteLine("xld放置开始:" + (DateTime.Now - _Run).TotalSeconds);

                    //偏移xld对象到结果位置
                    if (ProjectiveTrans_Xld(_Find_Property.Shape_Based_Model, hv_angle, hv_row, hv_column, hv_HomMat2D, _HWindow).GetResult())
                    {

                        Console.WriteLine("xld放置结束:" + (DateTime.Now - _Run).TotalSeconds);

                        return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Find_Property.Shape_Based_Model + "查找XLD模型结果映射成功！" };

                    }
                    else
                    {
                        return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD匹配结果映射失败) { Result_Error_Info = "计算结果有误,请检查!" };

                    }


                }





                //if (_Find_Out.FInd_Results)
                //{
                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Find_Property.Shape_Based_Model + "匹配模型查找成功！" };

                //}
                //else
                //{
                //    return new HPR_Status_Model(HVE_Result_Enum.查找模型匹配失败) { Result_Error_Info = _Find_Property.Shape_Based_Model + "模型" };

                //}









            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.查找模型匹配失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                //清除临时内存
                hv_row.Dispose();
                hv_column.Dispose();
                hv_angle.Dispose();
                hv_score.Dispose();
                hv_HomMat2D.Dispose();

                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }




        }


        /// <summary>
        /// 投影映射XLD对象到图像控件位置,并返回XLD对象
        /// </summary>
        /// <param name="_ModelXld"></param>
        /// <param name="_HomMat2D"></param>
        /// <param name="_Window"></param>
        /// <returns></returns>
        private HPR_Status_Model<bool> ProjectiveTrans_Xld(Shape_Based_Model_Enum _Find_Enum, HTuple Angle, HTuple Row, HTuple Column, HTuple HomMat2D, HWindow _Window)
        {


            HObject _ContoursProjTrans = new HObject(Shape_ModelContours);
            HObject Ncc_Results = new HObject();
            HTuple Mat2D = new HTuple();


            try
            {








                //for (int i = 0; i < _Model_objects.Count; i++)
                //{





                //根据匹配模型类型 读取模板内的xld对象
                switch (_Find_Enum)
                {
                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:


                        //HOperatorSet.GetShapeModelContours(out _ContoursProjTrans, _ModelID[i], 1);
                        HOperatorSet.ProjectiveTransContourXld(Shape_ModelContours, out Shape_ModelContours, HomMat2D);


                        break;

                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:

                        //将xld对象矩阵映射到图像中
                        //HOperatorSet.GetDeformableModelContours(out _ContoursProjTrans, _ModelID[i], 1);
                        HOperatorSet.ProjectiveTransContourXld(Shape_ModelContours, out _ContoursProjTrans, HomMat2D);

                        break;



                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:


                        //设置显示模型
                        HOperatorSet.SetDraw(_Window, "margin");






                        HOperatorSet.HomMat2dIdentity(out Mat2D);
                        HOperatorSet.HomMat2dRotate(Mat2D, (new HTuple(Angle)).TupleRad(), new HTuple(0), new HTuple(0), out Mat2D);
                        HOperatorSet.HomMat2dTranslate(Mat2D, Row, Column, out Mat2D);

                        //将xld对象矩阵映射到图像中
                        HOperatorSet.ProjectiveTransContourXld(Shape_ModelContours, out Shape_ModelContours, Mat2D);
                        _ContoursProjTrans = Shape_ModelContours;


                        HOperatorSet.GetNccModelRegion(out Ncc_Results, Shape_ID);
                        HOperatorSet.AffineTransRegion(Ncc_Results, out Ncc_Results, Mat2D, "constant");
                        HOperatorSet.ConcatObj(Shape_ModelContours, Ncc_Results, out Shape_ModelContours);

                        break;
                }

                //Shape_ModelContours = _ContoursProjTrans.CopyObj(1,-1);




                _Window.SetColor(nameof(KnownColor.Red).ToLower());
                _Window.SetLineWidth(2);
                //显示到对应的控件窗口
                //HOperatorSet.DispObj(Shape_ModelContours, _Window);




                //}


                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "根据结果矩阵偏移XLD对象成功！" };
            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.XLD对象映射失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                //清楚内存使用
                _ContoursProjTrans.Dispose();
                Mat2D.Dispose();
                Ncc_Results.Dispose();

                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }





        }


        /// <summary>
        /// 计算xld交点位置
        /// </summary>
        /// <param name="Halcon_Find_Shape_Out"></param>
        /// <param name="_ALL_ModelXLD"></param>
        /// <param name="Matching_Model"></param>
        /// <param name="_Window"></param>
        /// <param name="_Math2D"></param>
        /// <returns></returns>
        public HPR_Status_Model<bool> Match_Model_XLD_Pos(ref Find_Shape_Results_Model _Find_Shape_Results, Shape_Based_Model_Enum Matching_Model, HWindow _Window, HTuple _Math2D)
        {






            HTuple C_P_Row = new HTuple();
            HTuple C_P_Col = new HTuple();
            HTuple Row1 = new HTuple();
            HTuple Column1 = new HTuple();
            HTuple L_RP1 = new HTuple();
            HTuple L_CP1 = new HTuple();
            HTuple L_RP2 = new HTuple();
            HTuple L_CP2 = new HTuple();
            HTuple L_RP3 = new HTuple();
            HTuple L_CP3 = new HTuple();
            HTuple hv_Text = new HTuple();
            HTuple IsOverlapping = new HTuple();
            HTuple _Qx = new HTuple();
            HTuple _Qy = new HTuple();
            HTuple Row_1 = new HTuple();
            HTuple Row_2 = new HTuple();
            HTuple Row_3 = new HTuple();
            HTuple Row_4 = new HTuple();
            HTuple Row_5 = new HTuple();
            HTuple Col_1 = new HTuple();
            HTuple Col_2 = new HTuple();
            HTuple Col_3 = new HTuple();
            HTuple Col_4 = new HTuple();
            HTuple Col_5 = new HTuple();
            HTuple _Angle = new HTuple();

            HObject _Cross = new HObject();
            HObject _Line_1 = new HObject();
            HObject _Line_2 = new HObject();
            HObject _Line_3 = new HObject();
            HObject _Line_4 = new HObject();
            HObject _Cir_1 = new HObject();


            try
            {

                //提取结果中的XLD特征
                if (Get_Model_Match_XLD(ref _Line_1, ref _Cir_1, ref _Line_2, ref _Line_3, ref _Line_4, Matching_Model).GetResult())
                {

                    if (_Find_Shape_Results.FInd_Results.Where(_W => _W == true).Count() == _Find_Shape_Results.FInd_Results.Count)
                    {

                        //提出XLD数据特征
                        HOperatorSet.GetContourXld(_Line_1, out Row_1, out Col_1);
                        HOperatorSet.GetContourXld(_Cir_1, out Row_2, out Col_2);
                        HOperatorSet.GetContourXld(_Line_2, out Row_3, out Col_3);
                        HOperatorSet.GetContourXld(_Line_3, out Row_4, out Col_4);
                        HOperatorSet.GetContourXld(_Line_4, out Row_5, out Col_5);

                        //得到圆弧中间点

                        C_P_Row = Row_2.TupleSelect((Row_2.TupleLength() / 2));
                        C_P_Col = Col_2.TupleSelect((Col_2.TupleLength() / 2));

                        Row1 = Row1.TupleConcat(C_P_Row);
                        Column1 = Column1.TupleConcat(C_P_Col);


                        //计算直线角度
                        HOperatorSet.AngleLl(Row_3.TupleSelect(1), Col_3.TupleSelect(1), Row_3.TupleSelect(0),
                                                            Col_3.TupleSelect(0), Row_1.TupleSelect(0), Col_1.TupleSelect(
                                                            0), Row_1.TupleSelect(1), Col_1.TupleSelect(1), out _Angle);

                        //计算直线交点
                        HOperatorSet.IntersectionLines(Row_1.TupleSelect(1), Col_1.TupleSelect(
                                                            1), Row_1.TupleSelect(0), Col_1.TupleSelect(0), Row_3.TupleSelect(
                                                             0), Col_3.TupleSelect(0), Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                            1), out L_RP1, out L_CP1, out IsOverlapping);


                        Row1 = Row1.TupleConcat(L_RP1);
                        Column1 = Column1.TupleConcat(L_CP1);

                        //计算直线交点
                        HOperatorSet.IntersectionLines(Row_3.TupleSelect(1), Col_3.TupleSelect(
                                                            1), Row_3.TupleSelect(0), Col_3.TupleSelect(0), Row_4.TupleSelect(
                                                             0), Col_4.TupleSelect(0), Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                            1), out L_RP2, out L_CP2, out IsOverlapping);

                        Row1 = Row1.TupleConcat(L_RP2);
                        Column1 = Column1.TupleConcat(L_CP2);

                        //计算直线交点
                        HOperatorSet.IntersectionLines(Row_4.TupleSelect(1), Col_4.TupleSelect(
                                                            1), Row_4.TupleSelect(0), Col_4.TupleSelect(0), Row_5.TupleSelect(
                                                             0), Col_5.TupleSelect(0), Row_5.TupleSelect(1), Col_5.TupleSelect(
                                                            1), out L_RP3, out L_CP3, out IsOverlapping);



                        Row1 = Row1.TupleConcat(L_RP3);
                        Column1 = Column1.TupleConcat(L_CP3);


                        //生成十字架
                        HOperatorSet.GenCrossContourXld(out _Cross, Row1, Column1, 80, (new HTuple(45)).TupleRad());



                        //添加图像识时间
                        hv_Text = hv_Text.TupleConcat("识别用时 : " + _Find_Shape_Results.Find_Time + "秒.");



                        //添加图像分析
                        _Find_Shape_Results.Score.ForEach((_S) =>
                        {
                            hv_Text = hv_Text.TupleConcat("图像分数 : " + Math.Round(_S, 3));
                        });


                        //清空集合
                        _Find_Shape_Results.Robot_Pos.Clear();
                        _Find_Shape_Results.Vision_Pos.Clear();

                        //转换图像坐标到机器坐标
                        for (int i = 0; i < Row1.Length; i++)
                        {
                            double _OX = Math.Round(Row1.TupleSelect(i).D, 3);
                            double _OY = Math.Round(Column1.TupleSelect(i).D, 3);

                            //没有矩阵数据跳过转换坐标
                            if (_Math2D != null)
                            {
                                HOperatorSet.AffineTransPoint2d(_Math2D, _OX, _OY, out _Qx, out _Qy);
                            }
                            else
                            {
                                _Qx = 0; _Qy = 0;
                            }

                            //显示图像信息
                            hv_Text = hv_Text.TupleConcat("图像坐标_" + i + " X : " + _OX + " Y : " + _OY + " | 机器坐标_" + "X : " + _Qx + " Y : " + _Qy);
                            _Find_Shape_Results.Robot_Pos.Add(new Point3D(Math.Round(_Qx.D, 3), Math.Round(_Qy.D, 3), 0));
                            _Window.DispText(i + "号", "image", _OX + 50, _OY - 50, "black", "box", "true");
                            _Find_Shape_Results.Vision_Pos.Add(new Point3D(_OX, _OY, 0));
                        }



                        hv_Text = hv_Text.TupleConcat("夹角: " + Math.Round(_Angle.TupleDeg().D, 3));
                        _Find_Shape_Results.Text_Arr_UI = new List<string>(hv_Text.SArr);
                        //_Find_Shape_Results.DispWiindow = _Window;





                        //设置显示图像颜色
                        _Window.SetColor(nameof(KnownColor.Green).ToLower());
                        _Window.SetLineWidth(3);
                        //显示十字架
                        _Window.DispObj(_Cross);
                        //设置显示图像颜色
                        _Window.SetColor(nameof(KnownColor.Red).ToLower());
                        _Window.SetLineWidth(1);
                        _Window.SetPart(0, 0, -2, -2);




                    }
                    else
                    {


                        //查找失败处理
                        hv_Text = hv_Text.TupleConcat("识别用时 : " + _Find_Shape_Results.Find_Time + "毫秒，" + "图像分数 : 未知");
                        _Find_Shape_Results.Text_Arr_UI = new List<string>(hv_Text.SArr);
                        //_Find_Shape_Results.DispWiindow = _Window;


                        return new HPR_Status_Model<bool>(HVE_Result_Enum.查找模型匹配失败);


                    }


                }
                else
                {

                    return new HPR_Status_Model<bool>(HVE_Result_Enum.提取匹配结果的XLD模型失败);

                }

                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "计算匹配模型结果交点信息成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model<bool>(HVE_Result_Enum.根据匹配模型结果计算交点信息失败) { Result_Error_Info = e.Message };

            }
            finally
            {

                C_P_Row.Dispose();
                C_P_Col.Dispose();
                Row1.Dispose();
                Column1.Dispose();
                L_RP1.Dispose();
                L_CP1.Dispose();
                L_RP2.Dispose();
                L_CP2.Dispose();
                L_RP3.Dispose();
                L_CP3.Dispose();
                hv_Text.Dispose();
                IsOverlapping.Dispose();
                _Qx.Dispose();
                _Qy.Dispose();
                Row_1.Dispose();
                Row_2.Dispose();
                Row_3.Dispose();
                Row_4.Dispose();
                Row_5.Dispose();
                Col_1.Dispose();
                Col_2.Dispose();
                Col_3.Dispose();
                Col_4.Dispose();
                Col_5.Dispose();
                _Angle.Dispose();

                _Cross.Dispose();
                _Line_1.Dispose();
                _Line_2.Dispose();
                _Line_3.Dispose();
                _Line_4.Dispose();
                _Cir_1.Dispose();


                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
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
        public HPR_Status_Model<bool> Get_Model_Match_XLD(ref HObject _Lin_1, ref HObject _Cir_1, ref HObject _Lin_2, ref HObject _Lin_3, ref HObject _Lin_4, Shape_Based_Model_Enum Matching_Model)
        {



            //选择出有需要的直线特征

            try
            {

                //根据匹配模型类型 读取模板内的xld对象
                switch (Matching_Model)
                {
                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.shape_model || _T == Shape_Based_Model_Enum.Scale_model:



                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_1, 1);
                        HOperatorSet.SelectObj(All_XLd[0], out _Cir_1, 2);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_2, 3);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_3, 4);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_4, 5);


                        break;

                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.planar_deformable_model || _T == Shape_Based_Model_Enum.local_deformable_model:
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_1, 1);
                        HOperatorSet.SelectObj(All_XLd[0], out _Cir_1, 2);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_2, 3);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_3, 4);
                        HOperatorSet.SelectObj(All_XLd[0], out _Lin_4, 5);


                        break;


                    case Shape_Based_Model_Enum _T when _T == Shape_Based_Model_Enum.Ncc_Model:

                        if (All_XLd.Count == 3)
                        {


                            HOperatorSet.SelectObj(All_XLd[0], out _Lin_1, 1);
                            HOperatorSet.SelectObj(All_XLd[1], out _Cir_1, 2);
                            HOperatorSet.SelectObj(All_XLd[1], out _Lin_2, 1);
                            HOperatorSet.SelectObj(All_XLd[2], out _Lin_3, 1);
                            HOperatorSet.SelectObj(All_XLd[2], out _Lin_4, 2);
                        }
                        else
                        {
                            return new HPR_Status_Model<bool>(HVE_Result_Enum.提取匹配结果的XLD模型数量与计算数量不匹配);
                        }


                        break;
                }







                return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = "提取计算特征XLD成功！" };

            }
            catch (Exception e)
            {
                return new HPR_Status_Model<bool>(HVE_Result_Enum.提取匹配结果的XLD模型失败) { Result_Error_Info = e.Message };
            }
        }



        /// <summary>
        /// 获得图片最大灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="_HImage"></param>
        /// <returns></returns>
        public HRegion Get_Image_MaxThreshold(HImage _HImage)
        {

            try
            {
                HRegion _Region = new HRegion();

                if (_HImage != null)
                {

                    _Region = _HImage.Threshold(new HTuple(254), new HTuple(255));
                }
     
                return _Region;

            }
            catch (Exception)
            {

                throw new Exception("显示最大灰度失败！");


            }


        }


        /// <summary>
        /// 获得图片最小灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="_HImage"></param>
        /// <returns></returns>
        public HRegion Get_Image_MinThreshold( HImage _HImage)
        {

            try
            {

                HRegion _Region = new HRegion();

                if (_HImage != null)
                {

                    _Region = _HImage.Threshold(new HTuple(0), new HTuple(1));
                }
                return _Region;


            }
            catch (Exception )
            {

                throw new Exception("显示最小灰度失败！");

            }


        }














        /// <summary>
        /// GC回收处理方法
        /// </summary>
        public void Dispose()
        {

            _HImage.Dispose();
            _ModelsXld.Dispose();
            Shape_ModelContours.Dispose();
            All_XLd.ForEach(_M => _M.Dispose());
            _ModelIDS.Clear();
            Shape_ID.Dispose();

            GC.Collect();
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
            GC.Collect();

        }
    }



    [AddINotifyPropertyChangedInterface]
    public static class Halcon_Example
    {


        /// <summary>
        /// 显示三维坐标系的坐标轴 
        /// </summary>
        /// <param name="hv_WindowHandle"></param>
        /// <param name="hv_CamParam"></param>
        /// <param name="hv_Pose"></param>
        /// <param name="hv_CoordAxesLength"></param>
        public static HObject Disp_3d_coord(HTuple hv_CamParam, HTuple hv_Pose, HTuple hv_CoordAxesLength)
        {



            // Local iconic variables 

            HObject ho_Arrows;

            // Local control variables 

            HTuple hv_CameraType = new HTuple(), hv_IsTelecentric = new HTuple();
            HTuple hv_TransWorld2Cam = new HTuple(), hv_OrigCamX = new HTuple();
            HTuple hv_OrigCamY = new HTuple(), hv_OrigCamZ = new HTuple();
            HTuple hv_Row0 = new HTuple(), hv_Column0 = new HTuple();
            HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_Z = new HTuple();
            HTuple hv_RowAxX = new HTuple(), hv_ColumnAxX = new HTuple();
            HTuple hv_RowAxY = new HTuple(), hv_ColumnAxY = new HTuple();
            HTuple hv_RowAxZ = new HTuple(), hv_ColumnAxZ = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_HeadLength = new HTuple();
            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrows);
            try
            {
                // 这个存储过程将显示三维坐标系。
                // 它需要存储过程 gen_arrow_contour_xld。
                //
                //Input parameters：
                //WindowHandle： 显示坐标系的窗口
                //CamParam：摄像机参数
                //Pose： 要显示的姿势
                //CoordAxesLength： 以世界坐标为单位的坐标轴长度
                //
                //检查 Pose 是否为正确的姿势元组。
                if ((int)(new HTuple((new HTuple(hv_Pose.TupleLength())).TupleNotEqual(7))) != 0)
                {

                    return ho_Arrows;
                }
                hv_CameraType.Dispose();
                Get_cam_par_data(hv_CamParam, "camera_type", out hv_CameraType);
                hv_IsTelecentric.Dispose();

                hv_IsTelecentric = new HTuple(((hv_CameraType.TupleStrstr(
                    "telecentric"))).TupleNotEqual(-1));

                if ((int)((new HTuple(((hv_Pose.TupleSelect(2))).TupleEqual(0.0))).TupleAnd(
                    hv_IsTelecentric.TupleNot())) != 0)
                {
                    //For projective cameras:
                    //Poses with Z position zero cannot be projected
                    //(that would lead to a division by zero error).
                    //用于投影式摄像机：
                    //不能投射 Z 位置为零的姿势
                    //这将导致除以零的错误）。


                    return ho_Arrows;
                }
                //Convert to pose to a transformation matrix
                //将姿势转换为变换矩阵
                hv_TransWorld2Cam.Dispose();
                HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_TransWorld2Cam);
                //Project the world origin into the image
                //将世界原点投射到图像中
                hv_OrigCamX.Dispose(); hv_OrigCamY.Dispose(); hv_OrigCamZ.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, 0, 0, out hv_OrigCamX,
                    out hv_OrigCamY, out hv_OrigCamZ);
                hv_Row0.Dispose(); hv_Column0.Dispose();
                HOperatorSet.Project3dPoint(hv_OrigCamX, hv_OrigCamY, hv_OrigCamZ, hv_CamParam,
                    out hv_Row0, out hv_Column0);
                //Project the coordinate axes into the image
                //将坐标轴投影到图像中
                hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, hv_CoordAxesLength, 0, 0,
                    out hv_X, out hv_Y, out hv_Z);
                hv_RowAxX.Dispose(); hv_ColumnAxX.Dispose();
                HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxX, out hv_ColumnAxX);
                hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, hv_CoordAxesLength, 0,
                    out hv_X, out hv_Y, out hv_Z);
                hv_RowAxY.Dispose(); hv_ColumnAxY.Dispose();
                HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxY, out hv_ColumnAxY);
                hv_X.Dispose(); hv_Y.Dispose(); hv_Z.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_TransWorld2Cam, 0, 0, hv_CoordAxesLength,
                    out hv_X, out hv_Y, out hv_Z);
                hv_RowAxZ.Dispose(); hv_ColumnAxZ.Dispose();
                HOperatorSet.Project3dPoint(hv_X, hv_Y, hv_Z, hv_CamParam, out hv_RowAxZ, out hv_ColumnAxZ);
                //
                //Generate an XLD contour for each axis
                //为每个轴生成 XLD 等值线
                hv_Distance.Dispose();
                HOperatorSet.DistancePp(((hv_Row0.TupleConcat(hv_Row0))).TupleConcat(hv_Row0),
                    ((hv_Column0.TupleConcat(hv_Column0))).TupleConcat(hv_Column0), ((hv_RowAxX.TupleConcat(
                    hv_RowAxY))).TupleConcat(hv_RowAxZ), ((hv_ColumnAxX.TupleConcat(hv_ColumnAxY))).TupleConcat(
                    hv_ColumnAxZ), out hv_Distance);

                hv_HeadLength.Dispose();

                hv_HeadLength = (((((((hv_Distance.TupleMax()) / 12.0)).TupleConcat(5.0))).TupleMax())).TupleInt();


                ho_Arrows.Dispose();
                Gen_arrow_contour_xld(out ho_Arrows, ((hv_Row0.TupleConcat(hv_Row0))).TupleConcat(
                    hv_Row0), ((hv_Column0.TupleConcat(hv_Column0))).TupleConcat(hv_Column0),
                    ((hv_RowAxX.TupleConcat(hv_RowAxY))).TupleConcat(hv_RowAxZ), ((hv_ColumnAxX.TupleConcat(
                    hv_ColumnAxY))).TupleConcat(hv_ColumnAxZ), hv_HeadLength, hv_HeadLength);

                //
                //Display coordinate system
                //HOperatorSet.DispXld(ho_Arrows, hv_WindowHandle);
                //
                //hv_Red.Dispose(); hv_Green.Dispose(); hv_Blue.Dispose();
                //HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);

                //HOperatorSet.SetRgb(hv_WindowHandle, hv_Red.TupleSelect(0), hv_Green.TupleSelect(
                //    0), hv_Blue.TupleSelect(0));


                //HOperatorSet.SetTposition(hv_WindowHandle, hv_RowAxX + 3, hv_ColumnAxX + 3);

                //HOperatorSet.WriteString(hv_WindowHandle, "X");

                //HOperatorSet.SetRgb(hv_WindowHandle, hv_Red.TupleSelect(1 % (new HTuple(hv_Red.TupleLength()
                //    ))), hv_Green.TupleSelect(1 % (new HTuple(hv_Green.TupleLength()))), hv_Blue.TupleSelect(
                //    1 % (new HTuple(hv_Blue.TupleLength()))));


                //HOperatorSet.SetTposition(hv_WindowHandle, hv_RowAxY + 3, hv_ColumnAxY + 3);

                //HOperatorSet.WriteString(hv_WindowHandle, "Y");

                //HOperatorSet.SetRgb(hv_WindowHandle, hv_Red.TupleSelect(2 % (new HTuple(hv_Red.TupleLength()
                //    ))), hv_Green.TupleSelect(2 % (new HTuple(hv_Green.TupleLength()))), hv_Blue.TupleSelect(
                //    2 % (new HTuple(hv_Blue.TupleLength()))));


                //HOperatorSet.SetTposition(hv_WindowHandle, hv_RowAxZ + 3, hv_ColumnAxZ + 3);

                //HOperatorSet.WriteString(hv_WindowHandle, "Z");
                //HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                //ho_Arrows.Dispose();



                return ho_Arrows;
            }
            catch (Exception _e)
            {

                throw new Exception("计算中心坐标失败！原因：" + _e.Message);



                //throw HDevExpDefaultException;
            }
            finally
            {

                hv_CameraType.Dispose();
                hv_IsTelecentric.Dispose();
                hv_TransWorld2Cam.Dispose();
                hv_OrigCamX.Dispose();
                hv_OrigCamY.Dispose();
                hv_OrigCamZ.Dispose();
                hv_Row0.Dispose();
                hv_Column0.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_RowAxX.Dispose();
                hv_ColumnAxX.Dispose();
                hv_RowAxY.Dispose();
                hv_ColumnAxY.Dispose();
                hv_RowAxZ.Dispose();
                hv_ColumnAxZ.Dispose();
                hv_Distance.Dispose();
                hv_HeadLength.Dispose();
                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();

            }
        }



        /// <summary>
        /// 创建箭头形状的 XLD 轮廓
        /// </summary>
        /// <param name="ho_Arrow"></param>
        /// <param name="hv_Row1"></param>
        /// <param name="hv_Column1"></param>
        /// <param name="hv_Row2"></param>
        /// <param name="hv_Column2"></param>
        /// <param name="hv_HeadLength"></param>
        /// <param name="hv_HeadWidth"></param>
        private static void Gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = new HObject ();

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
            HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
            HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
            HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            try
            {
                //This procedure generates arrow shaped XLD contours,
                //pointing from (Row1, Column1) to (Row2, Column2).
                //If starting and end point are identical, a contour consisting
                //of a single point is returned.
                //
                //input parameters:
                //Row1, Column1: Coordinates of the arrows' starting points
                //Row2, Column2: Coordinates of the arrows' end points
                //HeadLength, HeadWidth: Size of the arrow heads in pixels
                //
                //output parameter:
                //Arrow: The resulting XLD contour
                //
                //The input tuples Row1, Column1, Row2, and Column2 have to be of
                //the same length.
                //HeadLength and HeadWidth either have to be of the same length as
                //Row1, Column1, Row2, and Column2 or have to be a single element.
                //If one of the above restrictions is violated, an error will occur.
                //
                //
                //Initialization.
                ho_Arrow.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Arrow);
                //
                //Calculate the arrow length
                hv_Length.Dispose();
                HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
                //
                //Mark arrows with identical start and end point
                //(set Length to -1 to avoid division-by-zero exception)
                hv_ZeroLengthIndices.Dispose();

                hv_ZeroLengthIndices = hv_Length.TupleFind(
                    0);

                if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
                {
                    if (hv_Length == null)
                        hv_Length = new HTuple();
                    hv_Length[hv_ZeroLengthIndices] = -1;
                }
                //
                //Calculate auxiliary variables.
                hv_DR.Dispose();

                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;

                hv_DC.Dispose();

                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;

                hv_HalfHeadWidth.Dispose();

                hv_HalfHeadWidth = hv_HeadWidth / 2.0;

                //
                //Calculate end points of the arrow head.
                hv_RowP1.Dispose();

                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);

                hv_ColP1.Dispose();

                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);

                hv_RowP2.Dispose();

                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);

                hv_ColP2.Dispose();

                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);

                //
                //Finally create output XLD contour for each input point pair
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                    {
                        //Create_ single points for arrows with identical start and end point

                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                            hv_Index), hv_Column1.TupleSelect(hv_Index));

                    }
                    else
                    {
                        //Create arrow contour

                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));

                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                        ho_Arrow.Dispose();
                        ho_Arrow = ExpTmpOutVar_0;
                    }
                }


                return;
            }
            catch (HalconException _e)
            {

                new Exception("三维坐标系生产错误！原因：" + _e.Message);
                //throw HDevExpDefaultException;
            }
            finally
            {
                ho_TempArrow.Dispose();

                hv_Length.Dispose();
                hv_ZeroLengthIndices.Dispose();
                hv_DR.Dispose();
                hv_DC.Dispose();
                hv_HalfHeadWidth.Dispose();
                hv_RowP1.Dispose();
                hv_ColP1.Dispose();
                hv_RowP2.Dispose();
                hv_ColP2.Dispose();
                hv_Index.Dispose();
            }
        }






        /// <summary>
        /// 从摄像机参数元组中获取指定摄像机参数的值。
        /// </summary>
        /// <param name="hv_CameraParam"></param>
        /// <param name="hv_ParamName"></param>
        /// <param name="hv_ParamValue"></param>
        private static void Get_cam_par_data(HTuple hv_CameraParam, HTuple hv_ParamName, out HTuple hv_ParamValue)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraType = new HTuple(), hv_CameraParamNames = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ParamNameInd = new HTuple();
            HTuple hv_I = new HTuple();
            // Initialize local and output iconic variables 
            hv_ParamValue = new HTuple();
            try
            {
                //get_cam_par_data returns in ParamValue the value of the
                //parameter that is given in ParamName from the tuple of
                //camera parameters that is given in CameraParam.
                //
                //Get the parameter names that correspond to the
                //elements in the input camera parameter tuple.
                hv_CameraType.Dispose(); hv_CameraParamNames.Dispose();
                Get_cam_par_names(hv_CameraParam, out hv_CameraType, out hv_CameraParamNames);
                //
                //Find the index of the requested camera data and return
                //the corresponding value.
                hv_ParamValue.Dispose();
                hv_ParamValue = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ParamName.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_ParamNameInd.Dispose();

                    hv_ParamNameInd = hv_ParamName.TupleSelect(
                        hv_Index);

                    if ((int)(new HTuple(hv_ParamNameInd.TupleEqual("camera_type"))) != 0)
                    {

                        {
                            HTuple
                              ExpTmpLocalVar_ParamValue = hv_ParamValue.TupleConcat(
                                hv_CameraType);
                            hv_ParamValue.Dispose();
                            hv_ParamValue = ExpTmpLocalVar_ParamValue;
                        }

                        continue;
                    }
                    hv_I.Dispose();

                    hv_I = hv_CameraParamNames.TupleFind(
                        hv_ParamNameInd);

                    if ((int)(new HTuple(hv_I.TupleNotEqual(-1))) != 0)
                    {

                        {
                            HTuple
                              ExpTmpLocalVar_ParamValue = hv_ParamValue.TupleConcat(
                                hv_CameraParam.TupleSelect(hv_I));
                            hv_ParamValue.Dispose();
                            hv_ParamValue = ExpTmpLocalVar_ParamValue;
                        }

                    }
                    else
                    {
                        throw new HalconException("Unknown camera parameter " + hv_ParamNameInd);
                    }
                }



                return;
            }
            catch (HalconException _e)
            {

                new Exception("获得相机参数失败！原因：" + _e.Message);
                //throw HDevExpDefaultException;
            }
            finally
            {
                hv_CameraType.Dispose();
                hv_CameraParamNames.Dispose();
                hv_Index.Dispose();
                hv_ParamNameInd.Dispose();
                hv_I.Dispose();
            }
        }

        /// <summary>
        /// 从摄像机参数元组中获取指定摄像机参数的值。
        /// </summary>
        /// <param name="hv_CameraParam"></param>
        /// <param name="hv_CameraType"></param>
        /// <param name="hv_ParamNames"></param>
        private static void Get_cam_par_names(HTuple hv_CameraParam, out HTuple hv_CameraType,
            out HTuple hv_ParamNames)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraParamAreaScanDivision = new HTuple();
            HTuple hv_CameraParamAreaScanPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricDivision = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new HTuple();
            HTuple hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanHypercentricDivision = new HTuple();
            HTuple hv_CameraParamAreaScanHypercentricPolynomial = new HTuple();
            HTuple hv_CameraParamLinesScanDivision = new HTuple();
            HTuple hv_CameraParamLinesScanPolynomial = new HTuple();
            HTuple hv_CameraParamLinesScanTelecentricDivision = new HTuple();
            HTuple hv_CameraParamLinesScanTelecentricPolynomial = new HTuple();
            HTuple hv_CameraParamAreaScanTiltDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTiltPolynomialLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanTelecentricPolynomialLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new HTuple();
            HTuple hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new HTuple();
            // Initialize local and output iconic variables 
            hv_CameraType = new HTuple();
            hv_ParamNames = new HTuple();
            try
            {
                //get_cam_par_names returns for each element in the camera
                //parameter tuple that is passed in CameraParam the name
                //of the respective camera parameter. The parameter names
                //are returned in ParamNames. Additionally, the camera
                //type is returned in CameraType. Alternatively, instead of
                //the camera parameters, the camera type can be passed in
                //CameraParam in form of one of the following strings:
                //  - 'area_scan_division'
                //  - 'area_scan_polynomial'
                //  - 'area_scan_tilt_division'
                //  - 'area_scan_tilt_polynomial'
                //  - 'area_scan_telecentric_division'
                //  - 'area_scan_telecentric_polynomial'
                //  - 'area_scan_tilt_bilateral_telecentric_division'
                //  - 'area_scan_tilt_bilateral_telecentric_polynomial'
                //  - 'area_scan_tilt_object_side_telecentric_division'
                //  - 'area_scan_tilt_object_side_telecentric_polynomial'
                //  - 'area_scan_hypercentric_division'
                //  - 'area_scan_hypercentric_polynomial'
                //  - 'line_scan_division'
                //  - 'line_scan_polynomial'
                //  - 'line_scan_telecentric_division'
                //  - 'line_scan_telecentric_polynomial'
                //
                hv_CameraParamAreaScanDivision.Dispose();
                hv_CameraParamAreaScanDivision = new HTuple();
                hv_CameraParamAreaScanDivision[0] = "focus";
                hv_CameraParamAreaScanDivision[1] = "kappa";
                hv_CameraParamAreaScanDivision[2] = "sx";
                hv_CameraParamAreaScanDivision[3] = "sy";
                hv_CameraParamAreaScanDivision[4] = "cx";
                hv_CameraParamAreaScanDivision[5] = "cy";
                hv_CameraParamAreaScanDivision[6] = "image_width";
                hv_CameraParamAreaScanDivision[7] = "image_height";
                hv_CameraParamAreaScanPolynomial.Dispose();
                hv_CameraParamAreaScanPolynomial = new HTuple();
                hv_CameraParamAreaScanPolynomial[0] = "focus";
                hv_CameraParamAreaScanPolynomial[1] = "k1";
                hv_CameraParamAreaScanPolynomial[2] = "k2";
                hv_CameraParamAreaScanPolynomial[3] = "k3";
                hv_CameraParamAreaScanPolynomial[4] = "p1";
                hv_CameraParamAreaScanPolynomial[5] = "p2";
                hv_CameraParamAreaScanPolynomial[6] = "sx";
                hv_CameraParamAreaScanPolynomial[7] = "sy";
                hv_CameraParamAreaScanPolynomial[8] = "cx";
                hv_CameraParamAreaScanPolynomial[9] = "cy";
                hv_CameraParamAreaScanPolynomial[10] = "image_width";
                hv_CameraParamAreaScanPolynomial[11] = "image_height";
                hv_CameraParamAreaScanTelecentricDivision.Dispose();
                hv_CameraParamAreaScanTelecentricDivision = new HTuple();
                hv_CameraParamAreaScanTelecentricDivision[0] = "magnification";
                hv_CameraParamAreaScanTelecentricDivision[1] = "kappa";
                hv_CameraParamAreaScanTelecentricDivision[2] = "sx";
                hv_CameraParamAreaScanTelecentricDivision[3] = "sy";
                hv_CameraParamAreaScanTelecentricDivision[4] = "cx";
                hv_CameraParamAreaScanTelecentricDivision[5] = "cy";
                hv_CameraParamAreaScanTelecentricDivision[6] = "image_width";
                hv_CameraParamAreaScanTelecentricDivision[7] = "image_height";
                hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
                hv_CameraParamAreaScanTelecentricPolynomial = new HTuple();
                hv_CameraParamAreaScanTelecentricPolynomial[0] = "magnification";
                hv_CameraParamAreaScanTelecentricPolynomial[1] = "k1";
                hv_CameraParamAreaScanTelecentricPolynomial[2] = "k2";
                hv_CameraParamAreaScanTelecentricPolynomial[3] = "k3";
                hv_CameraParamAreaScanTelecentricPolynomial[4] = "p1";
                hv_CameraParamAreaScanTelecentricPolynomial[5] = "p2";
                hv_CameraParamAreaScanTelecentricPolynomial[6] = "sx";
                hv_CameraParamAreaScanTelecentricPolynomial[7] = "sy";
                hv_CameraParamAreaScanTelecentricPolynomial[8] = "cx";
                hv_CameraParamAreaScanTelecentricPolynomial[9] = "cy";
                hv_CameraParamAreaScanTelecentricPolynomial[10] = "image_width";
                hv_CameraParamAreaScanTelecentricPolynomial[11] = "image_height";
                hv_CameraParamAreaScanTiltDivision.Dispose();
                hv_CameraParamAreaScanTiltDivision = new HTuple();
                hv_CameraParamAreaScanTiltDivision[0] = "focus";
                hv_CameraParamAreaScanTiltDivision[1] = "kappa";
                hv_CameraParamAreaScanTiltDivision[2] = "image_plane_dist";
                hv_CameraParamAreaScanTiltDivision[3] = "tilt";
                hv_CameraParamAreaScanTiltDivision[4] = "rot";
                hv_CameraParamAreaScanTiltDivision[5] = "sx";
                hv_CameraParamAreaScanTiltDivision[6] = "sy";
                hv_CameraParamAreaScanTiltDivision[7] = "cx";
                hv_CameraParamAreaScanTiltDivision[8] = "cy";
                hv_CameraParamAreaScanTiltDivision[9] = "image_width";
                hv_CameraParamAreaScanTiltDivision[10] = "image_height";
                hv_CameraParamAreaScanTiltPolynomial.Dispose();
                hv_CameraParamAreaScanTiltPolynomial = new HTuple();
                hv_CameraParamAreaScanTiltPolynomial[0] = "focus";
                hv_CameraParamAreaScanTiltPolynomial[1] = "k1";
                hv_CameraParamAreaScanTiltPolynomial[2] = "k2";
                hv_CameraParamAreaScanTiltPolynomial[3] = "k3";
                hv_CameraParamAreaScanTiltPolynomial[4] = "p1";
                hv_CameraParamAreaScanTiltPolynomial[5] = "p2";
                hv_CameraParamAreaScanTiltPolynomial[6] = "image_plane_dist";
                hv_CameraParamAreaScanTiltPolynomial[7] = "tilt";
                hv_CameraParamAreaScanTiltPolynomial[8] = "rot";
                hv_CameraParamAreaScanTiltPolynomial[9] = "sx";
                hv_CameraParamAreaScanTiltPolynomial[10] = "sy";
                hv_CameraParamAreaScanTiltPolynomial[11] = "cx";
                hv_CameraParamAreaScanTiltPolynomial[12] = "cy";
                hv_CameraParamAreaScanTiltPolynomial[13] = "image_width";
                hv_CameraParamAreaScanTiltPolynomial[14] = "image_height";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision = new HTuple();
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[0] = "focus";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[1] = "kappa";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[2] = "tilt";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[3] = "rot";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[4] = "sx";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[5] = "sy";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[6] = "cx";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[7] = "cy";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[8] = "image_width";
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision[9] = "image_height";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial = new HTuple();
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[0] = "focus";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[1] = "k1";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[2] = "k2";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[3] = "k3";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[4] = "p1";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[5] = "p2";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[6] = "tilt";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[7] = "rot";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[8] = "sx";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[9] = "sy";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[10] = "cx";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[11] = "cy";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[12] = "image_width";
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial[13] = "image_height";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision = new HTuple();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[0] = "magnification";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[1] = "kappa";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[2] = "tilt";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[3] = "rot";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[4] = "sx";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[5] = "sy";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[6] = "cx";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[7] = "cy";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[8] = "image_width";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision[9] = "image_height";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial = new HTuple();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[0] = "magnification";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[1] = "k1";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[2] = "k2";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[3] = "k3";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[4] = "p1";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[5] = "p2";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[6] = "tilt";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[7] = "rot";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[8] = "sx";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[9] = "sy";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[10] = "cx";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[11] = "cy";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[12] = "image_width";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial[13] = "image_height";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision = new HTuple();
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[0] = "magnification";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[1] = "kappa";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[2] = "image_plane_dist";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[3] = "tilt";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[4] = "rot";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[5] = "sx";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[6] = "sy";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[7] = "cx";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[8] = "cy";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[9] = "image_width";
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision[10] = "image_height";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial = new HTuple();
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[0] = "magnification";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[1] = "k1";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[2] = "k2";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[3] = "k3";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[4] = "p1";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[5] = "p2";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[6] = "image_plane_dist";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[7] = "tilt";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[8] = "rot";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[9] = "sx";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[10] = "sy";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[11] = "cx";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[12] = "cy";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[13] = "image_width";
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial[14] = "image_height";
                hv_CameraParamAreaScanHypercentricDivision.Dispose();
                hv_CameraParamAreaScanHypercentricDivision = new HTuple();
                hv_CameraParamAreaScanHypercentricDivision[0] = "focus";
                hv_CameraParamAreaScanHypercentricDivision[1] = "kappa";
                hv_CameraParamAreaScanHypercentricDivision[2] = "sx";
                hv_CameraParamAreaScanHypercentricDivision[3] = "sy";
                hv_CameraParamAreaScanHypercentricDivision[4] = "cx";
                hv_CameraParamAreaScanHypercentricDivision[5] = "cy";
                hv_CameraParamAreaScanHypercentricDivision[6] = "image_width";
                hv_CameraParamAreaScanHypercentricDivision[7] = "image_height";
                hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
                hv_CameraParamAreaScanHypercentricPolynomial = new HTuple();
                hv_CameraParamAreaScanHypercentricPolynomial[0] = "focus";
                hv_CameraParamAreaScanHypercentricPolynomial[1] = "k1";
                hv_CameraParamAreaScanHypercentricPolynomial[2] = "k2";
                hv_CameraParamAreaScanHypercentricPolynomial[3] = "k3";
                hv_CameraParamAreaScanHypercentricPolynomial[4] = "p1";
                hv_CameraParamAreaScanHypercentricPolynomial[5] = "p2";
                hv_CameraParamAreaScanHypercentricPolynomial[6] = "sx";
                hv_CameraParamAreaScanHypercentricPolynomial[7] = "sy";
                hv_CameraParamAreaScanHypercentricPolynomial[8] = "cx";
                hv_CameraParamAreaScanHypercentricPolynomial[9] = "cy";
                hv_CameraParamAreaScanHypercentricPolynomial[10] = "image_width";
                hv_CameraParamAreaScanHypercentricPolynomial[11] = "image_height";
                hv_CameraParamLinesScanDivision.Dispose();
                hv_CameraParamLinesScanDivision = new HTuple();
                hv_CameraParamLinesScanDivision[0] = "focus";
                hv_CameraParamLinesScanDivision[1] = "kappa";
                hv_CameraParamLinesScanDivision[2] = "sx";
                hv_CameraParamLinesScanDivision[3] = "sy";
                hv_CameraParamLinesScanDivision[4] = "cx";
                hv_CameraParamLinesScanDivision[5] = "cy";
                hv_CameraParamLinesScanDivision[6] = "image_width";
                hv_CameraParamLinesScanDivision[7] = "image_height";
                hv_CameraParamLinesScanDivision[8] = "vx";
                hv_CameraParamLinesScanDivision[9] = "vy";
                hv_CameraParamLinesScanDivision[10] = "vz";
                hv_CameraParamLinesScanPolynomial.Dispose();
                hv_CameraParamLinesScanPolynomial = new HTuple();
                hv_CameraParamLinesScanPolynomial[0] = "focus";
                hv_CameraParamLinesScanPolynomial[1] = "k1";
                hv_CameraParamLinesScanPolynomial[2] = "k2";
                hv_CameraParamLinesScanPolynomial[3] = "k3";
                hv_CameraParamLinesScanPolynomial[4] = "p1";
                hv_CameraParamLinesScanPolynomial[5] = "p2";
                hv_CameraParamLinesScanPolynomial[6] = "sx";
                hv_CameraParamLinesScanPolynomial[7] = "sy";
                hv_CameraParamLinesScanPolynomial[8] = "cx";
                hv_CameraParamLinesScanPolynomial[9] = "cy";
                hv_CameraParamLinesScanPolynomial[10] = "image_width";
                hv_CameraParamLinesScanPolynomial[11] = "image_height";
                hv_CameraParamLinesScanPolynomial[12] = "vx";
                hv_CameraParamLinesScanPolynomial[13] = "vy";
                hv_CameraParamLinesScanPolynomial[14] = "vz";
                hv_CameraParamLinesScanTelecentricDivision.Dispose();
                hv_CameraParamLinesScanTelecentricDivision = new HTuple();
                hv_CameraParamLinesScanTelecentricDivision[0] = "magnification";
                hv_CameraParamLinesScanTelecentricDivision[1] = "kappa";
                hv_CameraParamLinesScanTelecentricDivision[2] = "sx";
                hv_CameraParamLinesScanTelecentricDivision[3] = "sy";
                hv_CameraParamLinesScanTelecentricDivision[4] = "cx";
                hv_CameraParamLinesScanTelecentricDivision[5] = "cy";
                hv_CameraParamLinesScanTelecentricDivision[6] = "image_width";
                hv_CameraParamLinesScanTelecentricDivision[7] = "image_height";
                hv_CameraParamLinesScanTelecentricDivision[8] = "vx";
                hv_CameraParamLinesScanTelecentricDivision[9] = "vy";
                hv_CameraParamLinesScanTelecentricDivision[10] = "vz";
                hv_CameraParamLinesScanTelecentricPolynomial.Dispose();
                hv_CameraParamLinesScanTelecentricPolynomial = new HTuple();
                hv_CameraParamLinesScanTelecentricPolynomial[0] = "magnification";
                hv_CameraParamLinesScanTelecentricPolynomial[1] = "k1";
                hv_CameraParamLinesScanTelecentricPolynomial[2] = "k2";
                hv_CameraParamLinesScanTelecentricPolynomial[3] = "k3";
                hv_CameraParamLinesScanTelecentricPolynomial[4] = "p1";
                hv_CameraParamLinesScanTelecentricPolynomial[5] = "p2";
                hv_CameraParamLinesScanTelecentricPolynomial[6] = "sx";
                hv_CameraParamLinesScanTelecentricPolynomial[7] = "sy";
                hv_CameraParamLinesScanTelecentricPolynomial[8] = "cx";
                hv_CameraParamLinesScanTelecentricPolynomial[9] = "cy";
                hv_CameraParamLinesScanTelecentricPolynomial[10] = "image_width";
                hv_CameraParamLinesScanTelecentricPolynomial[11] = "image_height";
                hv_CameraParamLinesScanTelecentricPolynomial[12] = "vx";
                hv_CameraParamLinesScanTelecentricPolynomial[13] = "vy";
                hv_CameraParamLinesScanTelecentricPolynomial[14] = "vz";
                //Legacy parameter names
                hv_CameraParamAreaScanTiltDivisionLegacy.Dispose();
                hv_CameraParamAreaScanTiltDivisionLegacy = new HTuple();
                hv_CameraParamAreaScanTiltDivisionLegacy[0] = "focus";
                hv_CameraParamAreaScanTiltDivisionLegacy[1] = "kappa";
                hv_CameraParamAreaScanTiltDivisionLegacy[2] = "tilt";
                hv_CameraParamAreaScanTiltDivisionLegacy[3] = "rot";
                hv_CameraParamAreaScanTiltDivisionLegacy[4] = "sx";
                hv_CameraParamAreaScanTiltDivisionLegacy[5] = "sy";
                hv_CameraParamAreaScanTiltDivisionLegacy[6] = "cx";
                hv_CameraParamAreaScanTiltDivisionLegacy[7] = "cy";
                hv_CameraParamAreaScanTiltDivisionLegacy[8] = "image_width";
                hv_CameraParamAreaScanTiltDivisionLegacy[9] = "image_height";
                hv_CameraParamAreaScanTiltPolynomialLegacy.Dispose();
                hv_CameraParamAreaScanTiltPolynomialLegacy = new HTuple();
                hv_CameraParamAreaScanTiltPolynomialLegacy[0] = "focus";
                hv_CameraParamAreaScanTiltPolynomialLegacy[1] = "k1";
                hv_CameraParamAreaScanTiltPolynomialLegacy[2] = "k2";
                hv_CameraParamAreaScanTiltPolynomialLegacy[3] = "k3";
                hv_CameraParamAreaScanTiltPolynomialLegacy[4] = "p1";
                hv_CameraParamAreaScanTiltPolynomialLegacy[5] = "p2";
                hv_CameraParamAreaScanTiltPolynomialLegacy[6] = "tilt";
                hv_CameraParamAreaScanTiltPolynomialLegacy[7] = "rot";
                hv_CameraParamAreaScanTiltPolynomialLegacy[8] = "sx";
                hv_CameraParamAreaScanTiltPolynomialLegacy[9] = "sy";
                hv_CameraParamAreaScanTiltPolynomialLegacy[10] = "cx";
                hv_CameraParamAreaScanTiltPolynomialLegacy[11] = "cy";
                hv_CameraParamAreaScanTiltPolynomialLegacy[12] = "image_width";
                hv_CameraParamAreaScanTiltPolynomialLegacy[13] = "image_height";
                hv_CameraParamAreaScanTelecentricDivisionLegacy.Dispose();
                hv_CameraParamAreaScanTelecentricDivisionLegacy = new HTuple();
                hv_CameraParamAreaScanTelecentricDivisionLegacy[0] = "focus";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[1] = "kappa";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[2] = "sx";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[3] = "sy";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[4] = "cx";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[5] = "cy";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[6] = "image_width";
                hv_CameraParamAreaScanTelecentricDivisionLegacy[7] = "image_height";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
                hv_CameraParamAreaScanTelecentricPolynomialLegacy = new HTuple();
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[0] = "focus";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[1] = "k1";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[2] = "k2";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[3] = "k3";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[4] = "p1";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[5] = "p2";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[6] = "sx";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[7] = "sy";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[8] = "cx";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[9] = "cy";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[10] = "image_width";
                hv_CameraParamAreaScanTelecentricPolynomialLegacy[11] = "image_height";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy = new HTuple();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[0] = "focus";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[1] = "kappa";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[2] = "tilt";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[3] = "rot";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[4] = "sx";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[5] = "sy";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[6] = "cx";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[7] = "cy";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[8] = "image_width";
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy[9] = "image_height";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy = new HTuple();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[0] = "focus";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[1] = "k1";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[2] = "k2";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[3] = "k3";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[4] = "p1";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[5] = "p2";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[6] = "tilt";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[7] = "rot";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[8] = "sx";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[9] = "sy";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[10] = "cx";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[11] = "cy";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[12] = "image_width";
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy[13] = "image_height";
                //
                //If the camera type is passed in CameraParam
                if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    if ((int)(((hv_CameraParam.TupleSelect(0))).TupleIsString()) != 0)
                    {
                        hv_CameraType.Dispose();

                        hv_CameraType = hv_CameraParam.TupleSelect(
                            0);

                        if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();
                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricPolynomial);
                        }
                        else if ((int)((new HTuple(hv_CameraType.TupleEqual("line_scan_division"))).TupleOr(
                            new HTuple(hv_CameraType.TupleEqual("line_scan")))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);

                        }
                        else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
                        {
                            hv_ParamNames.Dispose();

                            hv_ParamNames = new HTuple();
                            hv_ParamNames[0] = "camera_type";
                            hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);

                        }
                        else
                        {
                            throw new HalconException(("Unknown camera type '" + hv_CameraType) + "' passed in CameraParam.");
                        }

                        return;
                    }
                }
                //
                //If the camera parameters are passed in CameraParam
                if ((int)(((((hv_CameraParam.TupleSelect(0))).TupleIsString())).TupleNot()) != 0)
                {
                    //Format of camera parameters for HALCON 12 and earlier
                    switch ((new HTuple(hv_CameraParam.TupleLength()
                        )).I)
                    {
                        //
                        //Area Scan
                        case 8:
                            //CameraType: 'area_scan_division' or 'area_scan_telecentric_division'
                            if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanDivision);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_division";
                            }
                            else
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanTelecentricDivisionLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_telecentric_division";
                            }
                            break;
                        case 10:
                            //CameraType: 'area_scan_tilt_division' or 'area_scan_telecentric_tilt_division'
                            if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanTiltDivisionLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_tilt_division";
                            }
                            else
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_tilt_bilateral_telecentric_division";
                            }
                            break;
                        case 12:
                            //CameraType: 'area_scan_polynomial' or 'area_scan_telecentric_polynomial'
                            if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanPolynomial);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_polynomial";
                            }
                            else
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanTelecentricPolynomialLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_telecentric_polynomial";
                            }
                            break;
                        case 14:
                            //CameraType: 'area_scan_tilt_polynomial' or 'area_scan_telecentric_tilt_polynomial'
                            if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanTiltPolynomialLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_tilt_polynomial";
                            }
                            else
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy);
                                hv_CameraType.Dispose();
                                hv_CameraType = "area_scan_tilt_bilateral_telecentric_polynomial";
                            }
                            break;
                        //
                        //Line Scan
                        case 11:
                            //CameraType: 'line_scan' or 'line_scan_telecentric'
                            if ((int)(new HTuple(((hv_CameraParam.TupleSelect(0))).TupleNotEqual(0.0))) != 0)
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamLinesScanDivision);
                                hv_CameraType.Dispose();
                                hv_CameraType = "line_scan_division";
                            }
                            else
                            {
                                hv_ParamNames.Dispose();
                                hv_ParamNames = new HTuple(hv_CameraParamLinesScanTelecentricDivision);
                                hv_CameraType.Dispose();
                                hv_CameraType = "line_scan_telecentric_division";
                            }
                            break;
                        default:
                            throw new HalconException("Wrong number of values in CameraParam.");

                    }
                }
                else
                {
                    //Format of camera parameters since HALCON 13
                    hv_CameraType.Dispose();

                    hv_CameraType = hv_CameraParam.TupleSelect(
                        0);

                    if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            9))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            13))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            9))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_telecentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            13))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTelecentricPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            12))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            16))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanTiltPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            11))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_image_side_telecentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            15))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            11))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_bilateral_telecentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            15))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            12))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_tilt_object_side_telecentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            16))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            9))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("area_scan_hypercentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            13))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamAreaScanHypercentricPolynomial);

                    }
                    else if ((int)((new HTuple(hv_CameraType.TupleEqual("line_scan_division"))).TupleOr(
                        new HTuple(hv_CameraType.TupleEqual("line_scan")))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            12))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            16))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanPolynomial);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_division"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            12))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricDivision);

                    }
                    else if ((int)(new HTuple(hv_CameraType.TupleEqual("line_scan_telecentric_polynomial"))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_CameraParam.TupleLength())).TupleNotEqual(
                            16))) != 0)
                        {
                            throw new HalconException("Wrong number of values in CameraParam.");
                        }
                        hv_ParamNames.Dispose();

                        hv_ParamNames = new HTuple();
                        hv_ParamNames[0] = "camera_type";
                        hv_ParamNames = hv_ParamNames.TupleConcat(hv_CameraParamLinesScanTelecentricPolynomial);

                    }
                    else
                    {
                        throw new HalconException("Unknown camera type in CameraParam.");
                    }
                }



                return;
            }
            catch (HalconException _e)
            {

                new Exception("获得相机参数名称错误！原因：" + _e.Message);

                //throw HDevExpDefaultException;
            }
            finally
            {

                hv_CameraParamAreaScanDivision.Dispose();
                hv_CameraParamAreaScanPolynomial.Dispose();
                hv_CameraParamAreaScanTelecentricDivision.Dispose();
                hv_CameraParamAreaScanTelecentricPolynomial.Dispose();
                hv_CameraParamAreaScanTiltDivision.Dispose();
                hv_CameraParamAreaScanTiltPolynomial.Dispose();
                hv_CameraParamAreaScanImageSideTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanImageSideTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanObjectSideTelecentricTiltDivision.Dispose();
                hv_CameraParamAreaScanObjectSideTelecentricTiltPolynomial.Dispose();
                hv_CameraParamAreaScanHypercentricDivision.Dispose();
                hv_CameraParamAreaScanHypercentricPolynomial.Dispose();
                hv_CameraParamLinesScanDivision.Dispose();
                hv_CameraParamLinesScanPolynomial.Dispose();
                hv_CameraParamLinesScanTelecentricDivision.Dispose();
                hv_CameraParamLinesScanTelecentricPolynomial.Dispose();
                hv_CameraParamAreaScanTiltDivisionLegacy.Dispose();
                hv_CameraParamAreaScanTiltPolynomialLegacy.Dispose();
                hv_CameraParamAreaScanTelecentricDivisionLegacy.Dispose();
                hv_CameraParamAreaScanTelecentricPolynomialLegacy.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltDivisionLegacy.Dispose();
                hv_CameraParamAreaScanBilateralTelecentricTiltPolynomialLegacy.Dispose();
            }
        }


    }

}
