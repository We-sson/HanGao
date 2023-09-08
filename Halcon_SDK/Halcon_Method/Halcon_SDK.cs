using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
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
        public HSmartWindowControlWPF Halcon_UserContol { set; get; }




        /// <summary>
        /// 样品图片保存后序号
        /// </summary>
        private static int Sample_Save_Image_Number { set; get; } = 1;

        /// <summary>
        /// 当前匹配文件存档
        /// </summary>
        public static Match_Models_List_Model Match_Models { set; get; }

        /// <summary>
        /// 设置显示参数
        /// </summary>
        public DisplayDrawColor_Model SetDisplay { set; get; } = new DisplayDrawColor_Model();


        /// <summary>
        /// 绑定图像
        /// </summary>
        public HObject DisplayImage { set; get; }
        /// <summary>
        /// 绑定区域显示
        /// </summary>
        public HObject DisplayRegion { set; get; }

        public HObject DisplayXLD { set; get; }
        /// <summary>
        /// 模型存储列表
        /// </summary>
        public static List<Match_Models_List_Model> Match_Models_List { set; get; } = new List<Match_Models_List_Model>();

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
        public static HPR_Status_Model ShowMaxGray_Image(ref HRegion _Region, HImage imgee)
        {
            try
            {
                _Region = new HRegion();

                _Region = imgee.Threshold(new HTuple(254), new HTuple(255));


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.显示最大灰度失败) { Result_Error_Info = e.Message };


            }


        }

        /// <summary>
        /// 显示图像中最小的灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="imgee"></param>
        /// <returns></returns>
        public static HPR_Status_Model ShowMinGray_Image(ref HRegion _Region, HImage imgee)
        {
            try
            {
                _Region = new HRegion();

                _Region = imgee.Threshold(new HTuple(0), new HTuple(1));


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK);
            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.显示最大灰度失败) { Result_Error_Info = e.Message };


            }


        }



        /// <summary>
        /// 海康获取图像指针转换Halcon图像
        /// </summary>
        /// <param name="_Width"></param>
        /// <param name="_Height"></param>
        /// <param name="_pData"></param>
        /// <returns></returns>
        public static HPR_Status_Model Mvs_To_Halcon_Image(ref HImage image, int _Width, int _Height, IntPtr _pData)
        {

            try
            {

                image.Dispose();
                //转换halcon图像格式
                image.GenImage1("byte", _Width, _Height, _pData);

                //HOperatorSet.GenImage1(out image, "byte", _Width, _Height, _pData);

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
        public static HPR_Status_Model HRead_Image(ref HImage _Image, string _Path)
        {

            try
            {

                if (_Path != "")
                {

                    _Image.ReadImage(_Path);
                    //HOperatorSet.ReadImage(out _Image, _Path);



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




















                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件路径读取成功！" };
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return new HPR_Status_Model(HVE_Result_Enum.文件路径提取失败) { Result_Error_Info = Save_Path };

            }

        }


        /// <summary>
        /// 根据用户生产数据生产圆弧XLD 
        /// </summary>
        /// <param name="_Cir"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static HPR_Status_Model Draw_Group_Cir(ref HObject _Cir, List<Point3D> _Point, HWindow _Window)
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
        /// GC回收处理方法
        /// </summary>
        public void Dispose()
        {


            //_Image.Dispose();
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }





    [AddINotifyPropertyChangedInterface]
    public class Halcon_Method : IDisposable
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
        public static HPR_Status_Model FindCalib_3DCoord(ref HXLDCont _CalibXLD, ref HObject _CalibCoord, ref HCalibData _CalibSetup_ID, HImage _HImage, int _CameraID, int _CalibID, double _SigmaVal, int _CalobPosNO = 0)
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
                Halcon_Example.Disp_3d_coord(ref _CalibCoord, _CamerPar, hv_Pose, new HTuple(0.02));

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "标定" };


            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.标定板图像识别错误) { Result_Error_Info = e.Message };
            }




        }





        /// <summary>
        /// 图像预处理
        /// </summary>
        /// <param name="_HWindow"></param>
        /// <param name="_Find_Property"></param>
        /// <returns></returns>
        public HPR_Status_Model Halcon_Image_Pre_Processing(HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
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

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "图像预处理完成！" };


            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.图像预处理错误) { Result_Error_Info = e.Message };


            }





        }



        /// <summary>
        /// 所有xld类型集合一起
        /// </summary>
        /// <param name="_All_XLD"></param>
        /// <param name="_Window"></param>
        /// <param name="_XLD_List"></param>
        /// <returns></returns>
        public HPR_Status_Model Group_All_XLD(HWindow _Window, List<HObject> _XLD_List)
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
        public HPR_Status_Model SetGet_ModelXld_Path(string _Location, FilePath_Type_Model_Enum _FilePath_Type, Shape_Based_Model_Enum _Model_Enum, ShapeModel_Name_Enum _Name, int _ID, int _Number = 0)
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




















                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "文件路径读取成功！" };
            }
            else
            {
                //User_Log_Add("读取模型文件地址错误，请检查设置！");
                return new HPR_Status_Model(HVE_Result_Enum.文件路径提取失败) { Result_Error_Info = Shape_Save_Path };

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
        public HPR_Status_Model ShapeModel_SaveFile(string _Location, Create_Shape_Based_ModelXld _Create_Model)
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

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Create_Model.Shape_Based_Model.ToString() + "模型创建成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.创建匹配模型失败) { Result_Error_Info = e.Message };

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
        public HPR_Status_Model ShapeModel_ReadFile(FileInfo _Path)
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


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Path.Name + "Halcon文件读取成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.Halcon文件类型读取失败) { Result_Error_Info = e.Message };
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
        public HPR_Status_Model Find_Deformable_Model(ref Find_Shape_Results_Model _Find_Out, HWindow _HWindow, Find_Shape_Based_ModelXld _Find_Property)
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

                        return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Find_Property.Shape_Based_Model + "查找XLD模型结果映射成功！" };

                    }
                    else
                    {
                        return new HPR_Status_Model(HVE_Result_Enum.XLD匹配结果映射失败) { Result_Error_Info = "计算结果有误,请检查!" };

                    }


                }





                //if (_Find_Out.FInd_Results)
                //{
                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Find_Property.Shape_Based_Model + "匹配模型查找成功！" };

                //}
                //else
                //{
                //    return new HPR_Status_Model(HVE_Result_Enum.查找模型匹配失败) { Result_Error_Info = _Find_Property.Shape_Based_Model + "模型" };

                //}









            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.查找模型匹配失败) { Result_Error_Info = e.Message };
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
        private HPR_Status_Model ProjectiveTrans_Xld(Shape_Based_Model_Enum _Find_Enum, HTuple Angle, HTuple Row, HTuple Column, HTuple HomMat2D, HWindow _Window)
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


                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "根据结果矩阵偏移XLD对象成功！" };
            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.XLD对象映射失败) { Result_Error_Info = e.Message };
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
        public HPR_Status_Model Match_Model_XLD_Pos(ref Find_Shape_Results_Model _Find_Shape_Results, Shape_Based_Model_Enum Matching_Model, HWindow _Window, HTuple _Math2D)
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


                        return new HPR_Status_Model(HVE_Result_Enum.查找模型匹配失败);


                    }


                }
                else
                {

                    return new HPR_Status_Model(HVE_Result_Enum.提取匹配结果的XLD模型失败);

                }

                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "计算匹配模型结果交点信息成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.根据匹配模型结果计算交点信息失败) { Result_Error_Info = e.Message };

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
        public HPR_Status_Model Get_Model_Match_XLD(ref HObject _Lin_1, ref HObject _Cir_1, ref HObject _Lin_2, ref HObject _Lin_3, ref HObject _Lin_4, Shape_Based_Model_Enum Matching_Model)
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
        /// 获得图片最大灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="_HImage"></param>
        /// <returns></returns>
        public static HPR_Status_Model Get_Image_MaxThreshold(ref HRegion _Region, HImage _HImage)
        {

            try
            {
                _Region = _HImage.Threshold(new HTuple(254), new HTuple(255));
                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "提取过曝区域成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.显示最大灰度失败) { Result_Error_Info = e.Message };

            }


        }


        /// <summary>
        /// 获得图片最小灰度
        /// </summary>
        /// <param name="_Region"></param>
        /// <param name="_HImage"></param>
        /// <returns></returns>
        public static HPR_Status_Model Get_Image_MinThreshold(ref HRegion _Region, HImage _HImage)
        {

            try
            {
                _Region = _HImage.Threshold(new HTuple(0), new HTuple(1));
                return new HPR_Status_Model(HVE_Result_Enum.Run_OK) { Result_Error_Info = "提取过暗区域成功！" };

            }
            catch (Exception e)
            {

                return new HPR_Status_Model(HVE_Result_Enum.显示最小灰度失败) { Result_Error_Info = e.Message };

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
        public static void Disp_3d_coord(ref HObject ho_Arrows, HTuple hv_CamParam, HTuple hv_Pose, HTuple hv_CoordAxesLength)
        {



            // Local iconic variables 

            //HObject ho_Arrows;

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
                    ho_Arrows.Dispose();

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

                    return;
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
                    ho_Arrows.Dispose();

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

                    return;
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

                hv_HeadLength = (((((((hv_Distance.TupleMax()
                    ) / 12.0)).TupleConcat(5.0))).TupleMax())).TupleInt();


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

                return;
            }
            catch (HalconException)
            {
                ho_Arrows.Dispose();

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

                //throw HDevExpDefaultException;
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

            HObject ho_TempArrow = null;

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

                return;
            }
            catch (HalconException)
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

                //throw HDevExpDefaultException;
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

                hv_CameraType.Dispose();
                hv_CameraParamNames.Dispose();
                hv_Index.Dispose();
                hv_ParamNameInd.Dispose();
                hv_I.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_CameraType.Dispose();
                hv_CameraParamNames.Dispose();
                hv_Index.Dispose();
                hv_ParamNameInd.Dispose();
                hv_I.Dispose();

                //throw HDevExpDefaultException;
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

                return;
            }
            catch (HalconException)
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

                //throw HDevExpDefaultException;
            }
        }





        private static HTuple gIsSinglePose;
        private static HTuple gTerminationButtonLabel;
        private static HTuple gInfoDecor;
        private static HTuple gInfoPos;
        private static HTuple gTitlePos;
        private static HTuple gTitleDecor;
        private static HTuple gAlphaDeselected;
        private static HTuple gDispObjOffset;
        private static HTuple gLabelsDecor;
        private static HTuple gUsesOpenGL;



        // Chapter: Graphics / Output
        // Short Description: Display 3D object models 
        public static void visualize_object_model_3d(HTuple hv_WindowHandle, HTuple hv_ObjectModel3D,
            HTuple hv_CamParam, HTuple hv_PoseIn, HTuple hv_GenParamName, HTuple hv_GenParamValue,
            HTuple hv_Title, HTuple hv_Label, HTuple hv_Information, out HTuple hv_PoseOut)
        {



            // Local iconic variables 
            // 本地标志性变量 
            HObject ho_Image = null, ho_ImageDump = null;

            // Local control variables 
            // 本地控制变量 
            //HTuple ExpTmpLocalVar_gDispObjOffset = new HTuple();
            //HTuple ExpTmpLocalVar_gLabelsDecor = new HTuple(), ExpTmpLocalVar_gInfoDecor = new HTuple();
            //HTuple ExpTmpLocalVar_gInfoPos = new HTuple(), ExpTmpLocalVar_gTitlePos = new HTuple();
            //HTuple ExpTmpLocalVar_gTitleDecor = new HTuple(), ExpTmpLocalVar_gTerminationButtonLabel = new HTuple();
            //HTuple ExpTmpLocalVar_gAlphaDeselected = new HTuple();
            //HTuple ExpTmpLocalVar_gIsSinglePose = new HTuple(),;
            //ExpTmpLocalVar_gUsesOpenGL = new HTuple();
            HTuple hv_Scene3DTest = new HTuple(), hv_Scene3D = new HTuple();
            HTuple hv_WindowHandleBuffer = new HTuple(), hv_TrackballSize = new HTuple();
            HTuple hv_VirtualTrackball = new HTuple(), hv_MouseMapping = new HTuple();
            HTuple hv_WaitForButtonRelease = new HTuple(), hv_MaxNumModels = new HTuple();
            HTuple hv_WindowCenteredRotation = new HTuple(), hv_NumModels = new HTuple();
            HTuple hv_SelectedObject = new HTuple(), hv_ClipRegion = new HTuple();
            HTuple hv_CPLength = new HTuple(), hv_RowNotUsed = new HTuple();
            HTuple hv_ColumnNotUsed = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_WPRow1 = new HTuple();
            HTuple hv_WPColumn1 = new HTuple(), hv_WPRow2 = new HTuple();
            HTuple hv_WPColumn2 = new HTuple(), hv_CamParamValue = new HTuple();
            HTuple hv_CamWidth = new HTuple(), hv_CamHeight = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_DispBackground = new HTuple(), hv_Mask = new HTuple();
            HTuple hv_Center = new HTuple(), hv_PoseEstimated = new HTuple();
            HTuple hv_Poses = new HTuple(), hv_HomMat3Ds = new HTuple();
            HTuple hv_Sequence = new HTuple(), hv_Font = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_OpenGLInfo = new HTuple();
            HTuple hv_DummyObjectModel3D = new HTuple(), hv_CameraIndexTest = new HTuple();
            HTuple hv_PoseTest = new HTuple(), hv_InstanceIndexTest = new HTuple();
            HTuple hv_MinImageSize = new HTuple(), hv_TrackballRadiusPixel = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
            HTuple hv_NumChannels = new HTuple(), hv_ColorImage = new HTuple();
            HTuple hv_CameraIndex = new HTuple(), hv_AllInstances = new HTuple();
            HTuple hv_SetLight = new HTuple(), hv_LightParam = new HTuple();
            HTuple hv_LightPosition = new HTuple(), hv_LightKind = new HTuple();
            HTuple hv_LightIndex = new HTuple(), hv_PersistenceParamName = new HTuple();
            HTuple hv_PersistenceParamValue = new HTuple(), hv_AlphaOrig = new HTuple();
            HTuple hv_I = new HTuple(), hv_ParamName = new HTuple();
            HTuple hv_ParamValue = new HTuple(), hv_ParamNameTrunk = new HTuple();
            HTuple hv_Instance = new HTuple(), hv_HomMat3D = new HTuple();
            HTuple hv_Qx = new HTuple(), hv_Qy = new HTuple(), hv_Qz = new HTuple();
            HTuple hv_TBCenter = new HTuple(), hv_TBSize = new HTuple();
            HTuple hv_ButtonHold = new HTuple(), hv_VisualizeTB = new HTuple();
            HTuple hv_MaxIndex = new HTuple(), hv_TrackballCenterRow = new HTuple();
            HTuple hv_TrackballCenterCol = new HTuple(), hv_GraphEvent = new HTuple();
            HTuple hv_Exit = new HTuple(), hv_GraphButtonRow = new HTuple();
            HTuple hv_GraphButtonColumn = new HTuple(), hv_GraphButton = new HTuple();
            HTuple hv_ButtonReleased = new HTuple(), hv_e = new HTuple();
            HTuple hv_CamParam_COPY_INP_TMP = new HTuple(hv_CamParam);
            HTuple hv_GenParamName_COPY_INP_TMP = new HTuple(hv_GenParamName);
            HTuple hv_GenParamValue_COPY_INP_TMP = new HTuple(hv_GenParamValue);
            HTuple hv_Label_COPY_INP_TMP = new HTuple(hv_Label);
            HTuple hv_PoseIn_COPY_INP_TMP = new HTuple(hv_PoseIn);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageDump);
            hv_PoseOut = new HTuple();
            try
            {
                //The procedure visualize_object_model_3d can be used to display
                //one or more 3d object models and to interactively modify
                //the object poses by using the mouse.
                //
                //The pose can be modified by moving the mouse while
                //pressing a mouse button. The default settings are:
                //
                // Rotate: Left mouse button
                // Zoom: Shift + Left mouse button (or Center mouse button)
                // Pan: Ctrl + Left mouse button
                //
                //Furthermore, it is possible to select and deselect objects,
                //to decrease the mouse sensitivity, and to toggle the
                //inspection mode (see the description of the generic parameter
                //'inspection_mode' below):
                //
                // (De-)select object(s): Right mouse button
                // Low mouse sensitivity: Alt + mouse button
                // Toggle inspection mode: Ctrl + Alt + Left mouse button
                //
                //In GenParamName and GenParamValue all generic Parameters
                //of disp_object_model_3d are supported.
                //
                //**********************************************************
                //Define global variables
                //**********************************************************
                //
                //global def tuple gDispObjOffset
                //global def tuple gLabelsDecor
                //global def tuple gInfoDecor
                //global def tuple gInfoPos
                //global def tuple gTitlePos
                //global def tuple gTitleDecor
                //global def tuple gTerminationButtonLabel
                //global def tuple gAlphaDeselected
                //global def tuple gIsSinglePose
                //global def tuple gUsesOpenGL
                //
                //**********************************************************
                //Initialize Handles to enable correct handling in error case
                //**********************************************************
                //存储过程 visualize_object_model_3d 可用于显示
                //显示一个或多个三维物体模型，并通过鼠标交互式地修改
                //使用鼠标交互式修改对象的姿态。
                //
                //通过移动鼠标，同时按下鼠标键，可以修改姿势。
                //按下鼠标键即可修改姿势。默认设置为
                //
                // 旋转： 鼠标左键
                // 缩放：Shift + 鼠标左键（或鼠标中键）
                // 平移 Ctrl + 鼠标左键
                //
                //此外，还可以选择和取消选择对象、
                //降低鼠标灵敏度，以及切换
                //检查模式（参见下面通用参数
                //检查模式）：
                //
                // 取消）选择对象： 鼠标右键
                // 低鼠标灵敏度： Alt + 鼠标按钮
                // 切换检查模式： Ctrl + Alt + 鼠标左键
                //
                //在 GenParamName 和 GenParamValue 中，支持 disp_object_model_3d 的所有通用参数。
                //都支持。
                //
                //**********************************************************
                //定义全局变量
                //**********************************************************
                //定义全局变量 
                //global def tuple gDispObjOffset
                //全局后备元组 gLabelsDecor
                //全局后备元组 gInfoDecor
                //全局变量元组 gInfoPos
                //全局变量元组 gTitlePos
                //全局变量元组 gTitleDecor
                //全局后备元组 gTerminationButtonLabel
                //全局变量元组 gAlphaDeselected
                //全局变量元组 gIsSinglePose
                //global def tuple gUsesOpenGL
                //
                //**********************************************************
                //初始化句柄，以便在出错时正确处理
                //**********************************************************
                hv_Scene3DTest.Dispose();
                hv_Scene3DTest = new HTuple();
                hv_Scene3D.Dispose();
                hv_Scene3D = new HTuple();
                hv_WindowHandleBuffer.Dispose();
                hv_WindowHandleBuffer = new HTuple();

                //**********************************************************
                //Some user defines that may be adapted if desired
                //**********************************************************
                //
                //TrackballSize defines the diameter of the trackball in
                //the image with respect to the smaller image dimension.

                //**********************************************************
                //一些用户定义，可根据需要进行调整
                //**********************************************************
                //
                //轨迹球尺寸（TrackballSize）定义了图像中轨迹球的直径。
                //图像中轨迹球的直径。
                hv_TrackballSize.Dispose();
                hv_TrackballSize = 0.8;
                //
                //VirtualTrackball defines the type of virtual trackball that
                //shall be used ('shoemake' or 'bell').
                //虚拟轨迹球定义了应使用的虚拟轨迹球类型（"鞋模 "或 "铃铛"）。
                //应使用的虚拟轨迹球类型（"鞋形 "或 "钟形"）。
                hv_VirtualTrackball.Dispose();
                hv_VirtualTrackball = "shoemake";
                //VirtualTrackball := 'bell'
                //
                //Functionality of mouse buttons
                //    1: Left mouse button
                //    2: Middle mouse button
                //    4: Right mouse button
                //    5: Left+Right mouse button
                //  8+x: Shift + mouse button
                // 16+x: Ctrl + mouse button
                // 48+x: Ctrl + Alt + mouse button
                //in the order [Translate, Rotate, Scale, ScaleAlternative1, ScaleAlternative2, SelectObjects, ToggleSelectionMode]
                //VirtualTrackball := 'bell' （虚拟轨迹球）。
                //
                //鼠标按钮的功能
                // 1: 鼠标左键
                // 2: 鼠标中键
                // 4: 鼠标右键
                // 5: 鼠标左键+右键
                // 8+x: Shift + 鼠标按钮
                // 16+x: Ctrl + 鼠标按钮
                // 48+x: Ctrl + Alt + 鼠标按钮
                // 按照 [平移、旋转、缩放、ScaleAlternative1、ScaleAlternative2、选择对象、切换选择模式] 的顺序操作
                hv_MouseMapping.Dispose();
                hv_MouseMapping = new HTuple();
                hv_MouseMapping[0] = 17;
                hv_MouseMapping[1] = 1;
                hv_MouseMapping[2] = 2;
                hv_MouseMapping[3] = 5;
                hv_MouseMapping[4] = 9;
                hv_MouseMapping[5] = 4;
                hv_MouseMapping[6] = 49;
                //
                //The labels of the objects appear next to their projected
                //center. With gDispObjOffset a fixed offset is added
                // 对象的标签会显示在其投影的旁边。
                //中心。使用 gDispObjOffset 时，会增加一个固定偏移量。
                //                  R,  C
                gDispObjOffset = new HTuple();
                gDispObjOffset[0] = -30;
                gDispObjOffset[1] = 0;
                //ExpSetGlobalVar_gDispObjOffset(ExpTmpLocalVar_gDispObjOffset);
                //
                //Customize the decoration of the different text elements
                //              Color,   Box
                // 自定义不同文本元素的装饰
                // 颜色、方框
                gInfoDecor = new HTuple();
                gInfoDecor[0] = "white";
                gInfoDecor[1] = "false";
                //ExpSetGlobalVar_gInfoDecor(ExpTmpLocalVar_gInfoDecor);
                gLabelsDecor = new HTuple();
                gLabelsDecor[0] = "white";
                gLabelsDecor[1] = "false";
                //ExpSetGlobalVar_gLabelsDecor(ExpTmpLocalVar_gLabelsDecor);
                gTitleDecor = new HTuple();
                gTitleDecor[0] = "black";
                gTitleDecor[1] = "true";
                //ExpSetGlobalVar_gTitleDecor(ExpTmpLocalVar_gTitleDecor);
                //
                //Customize the position of some text elements
                //  gInfoPos has one of the values
                //  {'UpperLeft', 'LowerLeft', 'UpperRight'}
                //自定义某些文本元素的位置
                // gInfoPos 具有以下值之一
                // {' 左上角'、' 左下角'、' 右上角'}。
                gInfoPos = "LowerLeft";
                //ExpSetGlobalVar_gInfoPos(ExpTmpLocalVar_gInfoPos);
                //  gTitlePos has one of the values
                //  {'UpperLeft', 'UpperCenter', 'UpperRight'}
                // gTitlePos 具有以下值之一
                // {'左上'、'中上'、'右上'}。
                gTitlePos = "UpperLeft";
                //ExpSetGlobalVar_gTitlePos(ExpTmpLocalVar_gTitlePos);
                //Alpha value (=1-transparency) that is used for visualizing
                //the objects that are not selected
                //阿尔法值（=1-透明度），用于可视化
                //未被选中的对象
                gAlphaDeselected = 0.3;
                //ExpSetGlobalVar_gAlphaDeselected(ExpTmpLocalVar_gAlphaDeselected);
                //Customize the label of the continue button
                //自定义继续按钮的标签
                gTerminationButtonLabel = " Continue ";
                //ExpSetGlobalVar_gTerminationButtonLabel(ExpTmpLocalVar_gTerminationButtonLabel);
                //Define if the continue button responds to a single click event or
                //if it responds only if the mouse button is released while being placed
                //over the continue button.
                //'true':  Wait until the continue button has been released.
                //         This should be used to avoid unwanted continuations of
                //         subsequent calls of visualize_object_model_3d, which can
                //         otherwise occur if the mouse button remains pressed while the
                //         next visualization is active.
                //'false': Continue the execution already if the continue button is
                //         pressed. This option allows a fast forwarding through
                //         subsequent calls of visualize_object_model_3d.
                // 定义继续按钮是否响应单击事件或
                //是否仅在鼠标放在继续按钮上时释放按钮时才响应。
                //在继续按钮上。
                //'true'：  等待继续按钮被释放。
                // 使用此方法可以避免在后续调用 visualize_object 时出现不必要的继续。
                // 后续调用 visualize_object_model_3d。
                // 否则，如果鼠标按钮在下一次可视化激活时仍被按下，就会发生这种情况。
                // 下一次可视化处于活动状态。
                //'false'（假）： 如果继续按钮
                // 继续执行。该选项允许在
                // 后续调用 visualize_object_model_3d。
                hv_WaitForButtonRelease.Dispose();
                hv_WaitForButtonRelease = "true";
                //Number of 3D Object models that can be selected and handled individually.
                //If there are more models passed then this number, some calculations
                //are performed differently and the individual selection and handling
                //of models is not supported anymore. Note that the value of MaxNumModels
                //can be overwritten with the generic parameter max_num_selectable_models.
                // 可选择并单独处理的 3D 物体模型数量。
                //如果传入的模型数超过此数，则某些计算将以不同的方式执行，并进行单独选择和处理。
                //将以不同方式执行，并且不再支持单独选择和处理模型。
                //将不再支持单独选择和处理模型。请注意，MaxNumModels 的值
                //可以被通用参数 max_num_selectable_models 改写。
                hv_MaxNumModels.Dispose();
                hv_MaxNumModels = 1000;
                //Defines the default for the initial state of the rotation center:
                //(1) The rotation center is fixed in the center of the image and lies
                //    on the surface of the object.
                //(2) The rotation center lies in the center of the object.
                //定义旋转中心初始状态的默认值：
                //(1)旋转中心固定在图像中心，并位于物体表面上。
                //在对象的表面上。
                //(2)旋转中心位于对象的中心。
                hv_WindowCenteredRotation.Dispose();
                hv_WindowCenteredRotation = 2;
                //
                //**********************************************************
                //
                //Initialize some values
                hv_NumModels.Dispose();

                hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength()
                    );

                hv_SelectedObject.Dispose();

                hv_SelectedObject = HTuple.TupleGenConst(
                    hv_NumModels, 1);

                //
                //Apply some system settings
                // dev_get_preferences(...); only in hdevelop
                // dev_set_preferences(...); only in hdevelop
                // dev_get_preferences(...); only in hdevelop
                // dev_set_preferences(...); only in hdevelop
                hv_ClipRegion.Dispose();
                HOperatorSet.GetSystem("clip_region", out hv_ClipRegion);
                HOperatorSet.SetSystem("clip_region", "false");
                //dev_update_off();
                //
                //Check if GenParamName matches GenParamValue
                //检查 GenParamName 是否与 GenParamValue 匹配
                if ((int)(new HTuple((new HTuple(hv_GenParamName_COPY_INP_TMP.TupleLength()
                    )).TupleNotEqual(new HTuple(hv_GenParamValue_COPY_INP_TMP.TupleLength()
                    )))) != 0)
                {
                    throw new HalconException("Number of generic parameters does not match number of generic parameter values");
                }
                //
                try
                {
                    //
                    //Refactor camera parameters to fit to window size
                    //重新调整摄像机参数，以适应窗口大小
                    //
                    hv_CPLength.Dispose();

                    hv_CPLength = new HTuple(hv_CamParam_COPY_INP_TMP.TupleLength()
                        );

                    hv_RowNotUsed.Dispose(); hv_ColumnNotUsed.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowNotUsed, out hv_ColumnNotUsed,
                        out hv_Width, out hv_Height);
                    hv_WPRow1.Dispose(); hv_WPColumn1.Dispose(); hv_WPRow2.Dispose(); hv_WPColumn2.Dispose();
                    HOperatorSet.GetPart(hv_WindowHandle, out hv_WPRow1, out hv_WPColumn1, out hv_WPRow2,
                        out hv_WPColumn2);

                    HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Height - 1, hv_Width - 1);

                    if ((int)(new HTuple(hv_CPLength.TupleEqual(0))) != 0)
                    {

                        hv_CamParam_COPY_INP_TMP.Dispose();
                        Gen_cam_par_area_scan_division(0.06, 0, 8.5e-6, 8.5e-6, hv_Width / 2, hv_Height / 2,
                            hv_Width, hv_Height, out hv_CamParam_COPY_INP_TMP);

                    }
                    else
                    {
                        hv_CamParamValue.Dispose();
                        Get_cam_par_data(hv_CamParam_COPY_INP_TMP, (((((new HTuple("sx")).TupleConcat(
                            "sy")).TupleConcat("cx")).TupleConcat("cy")).TupleConcat("image_width")).TupleConcat(
                            "image_height"), out hv_CamParamValue);
                        hv_CamWidth.Dispose();

                        hv_CamWidth = ((hv_CamParamValue.TupleSelect(
                            4))).TupleReal();

                        hv_CamHeight.Dispose();

                        hv_CamHeight = ((hv_CamParamValue.TupleSelect(
                            5))).TupleReal();

                        hv_Scale.Dispose();

                        hv_Scale = ((((hv_Width / hv_CamWidth)).TupleConcat(
                            hv_Height / hv_CamHeight))).TupleMin();

                        {

                            HTuple ExpTmpOutVar_0;
                            Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "sx", (hv_CamParamValue.TupleSelect(
                                0)) / hv_Scale, out ExpTmpOutVar_0);
                            hv_CamParam_COPY_INP_TMP.Dispose();
                            hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                        }

                        {

                            HTuple ExpTmpOutVar_0;
                            Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "sy", (hv_CamParamValue.TupleSelect(
                            1)) / hv_Scale, out ExpTmpOutVar_0);
                            hv_CamParam_COPY_INP_TMP.Dispose();
                            hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                        }

                        {
                            HTuple ExpTmpOutVar_0;
                            Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "cx", (hv_CamParamValue.TupleSelect(
                            2)) * hv_Scale, out ExpTmpOutVar_0);
                            hv_CamParam_COPY_INP_TMP.Dispose();
                            hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;

                        }

                        {
                        HTuple ExpTmpOutVar_0;

                        Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "cy", (hv_CamParamValue.TupleSelect(
                            3)) * hv_Scale, out ExpTmpOutVar_0);
                        hv_CamParam_COPY_INP_TMP.Dispose();
                        hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                        }

                        {
                        HTuple ExpTmpOutVar_0;

                        Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "image_width", (((hv_CamParamValue.TupleSelect(
                            4)) * hv_Scale)).TupleInt(), out ExpTmpOutVar_0);
                        hv_CamParam_COPY_INP_TMP.Dispose();
                        hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;

                        }

                        {
                        HTuple ExpTmpOutVar_0;

                        Set_cam_par_data(hv_CamParam_COPY_INP_TMP, "image_height", (((hv_CamParamValue.TupleSelect(
                            5)) * hv_Scale)).TupleInt(), out ExpTmpOutVar_0);
                        hv_CamParam_COPY_INP_TMP.Dispose();
                        hv_CamParam_COPY_INP_TMP = ExpTmpOutVar_0;
                        }

                    }
                    //
                    //Check the generic parameters for max_num_selectable_models
                    //检查 max_num_selectable_models 的通用参数
                    //(Note that the default is set above to MaxNumModels := 1000)
                    hv_Indices.Dispose();

                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "max_num_selectable_models");

                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        if ((int)(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                            0)))).TupleIsNumber()) != 0)
                        {
                            if ((int)(new HTuple(((((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect(0)))).TupleNumber())).TupleInt())).TupleLess(
                                1))) != 0)
                            {
                                //Wrong parameter value: Only integer values greater than 0 are allowed
                                throw new HalconException("Wrong value for parameter 'max_num_selectable_models' (must be an integer value greater than 0)");
                            }
                        }
                        else
                        {
                            //Wrong parameter value: Only integer values greater than 0 are allowed
                            throw new HalconException("Wrong value for parameter 'max_num_selectable_models' (must be an integer value greater than 0)");
                        }
                        hv_MaxNumModels.Dispose();

                        hv_MaxNumModels = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0)))).TupleNumber())).TupleInt();


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                    //
                    //Check the generic parameters for window_centered_rotation
                    //检查窗口居中旋转的通用参数
                    //(Note that the default is set above to WindowCenteredRotation := 2)
                    hv_Indices.Dispose();

                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "inspection_mode");

                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                            0)))).TupleEqual("surface"))) != 0)
                        {
                            hv_WindowCenteredRotation.Dispose();
                            hv_WindowCenteredRotation = 1;
                        }
                        else if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0)))).TupleEqual("standard"))) != 0)
                        {
                            hv_WindowCenteredRotation.Dispose();
                            hv_WindowCenteredRotation = 2;
                        }
                        else
                        {
                            //Wrong parameter value, use default value
                            //参数值错误，使用默认值
                        }

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                    //
                    //Check the generic parameters for disp_background
                    //(The former parameter name 'use_background' is still supported
                    // for compatibility reasons)
                    //检查 disp_background 的通用参数
                    //（出于兼容原因，仍支持以前的参数名称 "use_background
                    // 出于兼容性原因）
                    hv_DispBackground.Dispose();
                    hv_DispBackground = "false";
                    if ((int)(new HTuple((new HTuple(hv_GenParamName_COPY_INP_TMP.TupleLength()
                        )).TupleGreater(0))) != 0)
                    {
                        hv_Mask.Dispose();

                        hv_Mask = ((hv_GenParamName_COPY_INP_TMP.TupleEqualElem(
                            "disp_background"))).TupleOr(hv_GenParamName_COPY_INP_TMP.TupleEqualElem(
                            "use_background"));

                        hv_Indices.Dispose();

                        hv_Indices = hv_Mask.TupleFind(
                            1);

                    }
                    else
                    {
                        hv_Indices.Dispose();
                        hv_Indices = -1;
                    }
                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        hv_DispBackground.Dispose();

                        hv_DispBackground = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0));

                        if ((int)((new HTuple(hv_DispBackground.TupleNotEqual("true"))).TupleAnd(
                            new HTuple(hv_DispBackground.TupleNotEqual("false")))) != 0)
                        {
                            //Wrong parameter value: Only 'true' and 'false' are allowed
                            //参数值错误： 只允许使用 "true "和 "false
                            throw new HalconException("Wrong value for parameter 'disp_background' (must be either 'true' or 'false')");
                        }
                        //Note that the background is handled explicitly in this procedure
                        //and therefore, the parameter is removed from the list of
                        //parameters and disp_background is always set to true (see below)
                        //注意，本过程明确处理了背景信息
                        //因此，该参数将从
                        //参数列表中删除，disp_background 始终设为 true（见下文）

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                    //
                    //Read and check the parameter Label for each object
                    //读取并检查每个对象的参数标签
                    if ((int)(new HTuple((new HTuple(hv_Label_COPY_INP_TMP.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        //no labels set -> leave as []
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Label_COPY_INP_TMP.TupleLength()
                        )).TupleEqual(1))) != 0)
                    {
                        //a single label set for all models
                        //一套标签适用于所有型号
                        {
                            HTuple
                              ExpTmpLocalVar_Label = HTuple.TupleGenConst(
                                hv_NumModels, hv_Label_COPY_INP_TMP);
                            hv_Label_COPY_INP_TMP.Dispose();
                            hv_Label_COPY_INP_TMP = ExpTmpLocalVar_Label;
                        }

                    }
                    else
                    {
                        if ((int)(new HTuple((new HTuple(hv_Label_COPY_INP_TMP.TupleLength())).TupleNotEqual(
                            hv_NumModels))) != 0)
                        {
                            //Number of elements in Label does not match
                            //the number of object models.
                            //标签中元素的数量与对象模型的数量不匹配。
                            //对象模型的数量。
                            throw new HalconException(((new HTuple(new HTuple("Number of elements in Label (") + (new HTuple(hv_Label_COPY_INP_TMP.TupleLength()
                                ))) + ") does not match the number of object models(") + hv_NumModels) + ").");
                        }
                    }
                    //Convert labels into strings
                    //将标签转换成字符串
                    {
                        HTuple
                          ExpTmpLocalVar_Label = "" + hv_Label_COPY_INP_TMP;
                        hv_Label_COPY_INP_TMP.Dispose();
                        hv_Label_COPY_INP_TMP = ExpTmpLocalVar_Label;
                    }


                    //Read and check the parameter PoseIn for each object
                    //读取并检查每个对象的 PoseIn 参数
                    hv_Center.Dispose();
                    Get_object_models_center(hv_ObjectModel3D, out hv_Center);
                    if ((int)(new HTuple(hv_Center.TupleEqual(new HTuple()))) != 0)
                    {
                        hv_Center.Dispose();
                        hv_Center = new HTuple();
                        hv_Center[0] = 0;
                        hv_Center[1] = 0;
                        hv_Center[2] = 0;
                    }
                    if ((int)(new HTuple((new HTuple(hv_PoseIn_COPY_INP_TMP.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        //If no pose was specified by the caller, automatically calculate
                        //a pose that is appropriate for the visualization.
                        //Set the initial model reference pose. The orientation is parallel
                        //to the object coordinate system, the position is at the center
                        //of gravity of all models.
                        //如果调用者没有指定姿势，则自动计算出适合可视化的姿势。
                        //适合可视化的姿势。
                        //设置初始模型参考姿态。方向平行于
                        //与物体坐标系平行，位置位于所有模型的重心。
                        //所有模型的重心。
                        hv_PoseIn_COPY_INP_TMP.Dispose();
                        HOperatorSet.CreatePose(-(hv_Center.TupleSelect(0)), -(hv_Center.TupleSelect(
                            1)), -(hv_Center.TupleSelect(2)), 0, 0, 0, "Rp+T", "gba", "point",
                            out hv_PoseIn_COPY_INP_TMP);

                        hv_PoseEstimated.Dispose();
                        Determine_optimum_pose_distance(hv_ObjectModel3D, hv_CamParam_COPY_INP_TMP,
                            0.9, hv_PoseIn_COPY_INP_TMP, out hv_PoseEstimated);
                        hv_Poses.Dispose();
                        hv_Poses = new HTuple();
                        hv_HomMat3Ds.Dispose();
                        hv_HomMat3Ds = new HTuple();
                        hv_Sequence.Dispose();

                        hv_Sequence = HTuple.TupleGenSequence(
                            0, (hv_NumModels * 7) - 1, 1);

                        hv_Poses.Dispose();

                        hv_Poses = hv_PoseEstimated.TupleSelect(
                            hv_Sequence % 7);

                        gIsSinglePose = 1;
                        //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_PoseIn_COPY_INP_TMP.TupleLength()
                        )).TupleEqual(7))) != 0)
                    {
                        hv_Poses.Dispose();
                        hv_Poses = new HTuple();
                        hv_HomMat3Ds.Dispose();
                        hv_HomMat3Ds = new HTuple();
                        hv_Sequence.Dispose();

                        hv_Sequence = HTuple.TupleGenSequence(
                            0, (hv_NumModels * 7) - 1, 1);

                        hv_Poses.Dispose();

                        hv_Poses = hv_PoseIn_COPY_INP_TMP.TupleSelect(
                            hv_Sequence % 7);

                        gIsSinglePose = 1;
                        //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                    }
                    else
                    {
                        if ((int)(new HTuple((new HTuple(hv_PoseIn_COPY_INP_TMP.TupleLength())).TupleNotEqual(
                            (new HTuple(hv_ObjectModel3D.TupleLength())) * 7))) != 0)
                        {
                            //Wrong number of values of input control parameter 'PoseIn'
                            //输入控制参数 "PoseIn "的值个数有误
                            throw new HalconException("Wrong number of values of input control parameter 'PoseIn'.");
                        }
                        else
                        {
                            hv_Poses.Dispose();
                            hv_Poses = new HTuple(hv_PoseIn_COPY_INP_TMP);
                        }
                        gIsSinglePose = 0;
                        //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                    }
                    //
                    //Open (invisible) buffer window to avoid flickering
                    //打开（不可见）缓冲窗口以避免闪烁
                    hv_WindowHandleBuffer.Dispose();
                    HOperatorSet.OpenWindow(0, 0, hv_Width, hv_Height, 0, "buffer", "", out hv_WindowHandleBuffer);

                    HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, hv_Height - 1, hv_Width - 1);

                    hv_Font.Dispose();
                    HOperatorSet.GetFont(hv_WindowHandle, out hv_Font);
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandleBuffer, hv_Font);
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_Exception);
                    }
                    //
                    // Is OpenGL available and should it be used?
                    //OpenGL 是否可用？
                    gUsesOpenGL = "true";
                    //ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                    hv_Indices.Dispose();

                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "opengl");

                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {

                        gUsesOpenGL = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect(0));

                        //ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue_COPY_INP_TMP.TupleRemove(
                                hv_Indices);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpLocalVar_GenParamValue;
                        }

                        if ((int)((new HTuple(gUsesOpenGL.TupleNotEqual("true"))).TupleAnd(
                            new HTuple(gUsesOpenGL.TupleNotEqual("false")))) != 0)
                        {
                            //Wrong parameter value: Only 'true' and 'false' are allowed
                            throw new HalconException("Wrong value for parameter 'opengl' (must be either 'true' or 'false')");
                        }
                    }
                    if ((int)(new HTuple(gUsesOpenGL.TupleEqual("true"))) != 0)
                    {
                        hv_OpenGLInfo.Dispose();
                        HOperatorSet.GetSystem("opengl_info", out hv_OpenGLInfo);
                        if ((int)(new HTuple(hv_OpenGLInfo.TupleEqual("No OpenGL support included."))) != 0)
                        {
                            gUsesOpenGL = "false";
                            //ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                        }
                        else
                        {
                            hv_DummyObjectModel3D.Dispose();
                            HOperatorSet.GenObjectModel3dFromPoints(0, 0, 0, out hv_DummyObjectModel3D);
                            hv_Scene3DTest.Dispose();
                            HOperatorSet.CreateScene3d(out hv_Scene3DTest);
                            hv_CameraIndexTest.Dispose();
                            HOperatorSet.AddScene3dCamera(hv_Scene3DTest, hv_CamParam_COPY_INP_TMP,
                                out hv_CameraIndexTest);
                            hv_PoseTest.Dispose();
                            Determine_optimum_pose_distance(hv_DummyObjectModel3D, hv_CamParam_COPY_INP_TMP,
                                0.9, ((((((new HTuple(0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(
                                0)).TupleConcat(0)).TupleConcat(0)).TupleConcat(0), out hv_PoseTest);
                            hv_InstanceIndexTest.Dispose();
                            HOperatorSet.AddScene3dInstance(hv_Scene3DTest, hv_DummyObjectModel3D,
                                hv_PoseTest, out hv_InstanceIndexTest);
                            try
                            {
                                HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3DTest,
                                    hv_InstanceIndexTest);
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException2)
                            {
                                HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                gUsesOpenGL = "false";
                                //ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                            }
                            HOperatorSet.ClearScene3d(hv_Scene3DTest);
                            hv_Scene3DTest.Dispose();
                            hv_Scene3DTest = new HTuple();
                            HOperatorSet.ClearObjectModel3d(hv_DummyObjectModel3D);
                        }
                    }
                    //
                    //Compute the trackball
                     // 计算轨迹球
                    hv_MinImageSize.Dispose();

                    hv_MinImageSize = ((hv_Width.TupleConcat(
                        hv_Height))).TupleMin();

                    hv_TrackballRadiusPixel.Dispose();

                    hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;

                    //
                    //Measure the text extents for the continue button in the
                    //graphics window
                    //测量继续按钮的文本长度。
                    //图形窗口
                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth.Dispose(); hv_TextHeight.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandleBuffer, gTerminationButtonLabel + "  ",
                        out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);

                    //
                    //Store background image
                    //存储背景图像
                    if ((int)(new HTuple(hv_DispBackground.TupleEqual("false"))) != 0)
                    {
                        HOperatorSet.ClearWindow(hv_WindowHandle);
                    }
                    ho_Image.Dispose();
                    HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandle);
                    //Special treatment for color background images necessary
                    //必须对彩色背景图像进行特殊处理
                    hv_NumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_Image, out hv_NumChannels);
                    hv_ColorImage.Dispose();

                    hv_ColorImage = new HTuple(hv_NumChannels.TupleEqual(
                        3));

                    //
                    hv_Scene3D.Dispose();
                    HOperatorSet.CreateScene3d(out hv_Scene3D);
                    hv_CameraIndex.Dispose();
                    HOperatorSet.AddScene3dCamera(hv_Scene3D, hv_CamParam_COPY_INP_TMP, out hv_CameraIndex);
                    hv_AllInstances.Dispose();
                    HOperatorSet.AddScene3dInstance(hv_Scene3D, hv_ObjectModel3D, hv_Poses, out hv_AllInstances);
                    //Always set 'disp_background' to true,  because it is handled explicitly
                    //in this procedure (see above)
                    //始终将 "disp_background "设置为 true，因为在此过程中会明确处理它。
                    //在本程序中（见上文）
                    HOperatorSet.SetScene3dParam(hv_Scene3D, "disp_background", "true");
                    //Check if we have to set light specific parameters
                    // 检查是否需要设置灯光特定参数
                    hv_SetLight.Dispose();

                    hv_SetLight = new HTuple(hv_GenParamName_COPY_INP_TMP.TupleRegexpTest(
                        "light_"));

                    if ((int)(hv_SetLight) != 0)
                    {
                        //set position of light source
                        // 设置光源位置
                        hv_Indices.Dispose();

                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_position");

                        if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                            new HTuple())))) != 0)
                        {
                            //If multiple light positions are given, use the last one
                            // 如果给出了多个灯位，则使用最后一个灯位
                            hv_LightParam.Dispose();

                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();

                            if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleNotEqual(
                                4))) != 0)
                            {
                                throw new HalconException("light_position must be given as a string that contains four space separated floating point numbers");
                            }
                            hv_LightPosition.Dispose();

                            hv_LightPosition = hv_LightParam.TupleSelectRange(
                                0, 2);

                            hv_LightKind.Dispose();
                            hv_LightKind = "point_light";
                            if ((int)(new HTuple(((hv_LightParam.TupleSelect(3))).TupleEqual(0))) != 0)
                            {
                                hv_LightKind.Dispose();
                                hv_LightKind = "directional_light";
                            }
                            //Currently, only one light source is supported
                            //目前只支持一种光源
                            HOperatorSet.RemoveScene3dLight(hv_Scene3D, 0);
                            hv_LightIndex.Dispose();
                            HOperatorSet.AddScene3dLight(hv_Scene3D, hv_LightPosition, hv_LightKind,
                                out hv_LightIndex);
                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamName_COPY_INP_TMP.Dispose();
                                hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamValue_COPY_INP_TMP.Dispose();
                                hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                        //set ambient part of light source
                        //设置光源的环境部分
                        hv_Indices.Dispose();

                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_ambient");

                        if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                            new HTuple())))) != 0)
                        {
                            //If the ambient part is set multiple times, use the last setting
                            //如果环境部分已多次设置，则使用最后一次设置
                            hv_LightParam.Dispose();

                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();

                            if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleLess(
                                3))) != 0)
                            {
                                throw new HalconException("light_ambient must be given as a string that contains three space separated floating point numbers");
                            }

                            HOperatorSet.SetScene3dLightParam(hv_Scene3D, 0, "ambient", hv_LightParam.TupleSelectRange(
                                0, 2));

                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamName_COPY_INP_TMP.Dispose();
                                hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamValue_COPY_INP_TMP.Dispose();
                                hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                        }
                        //Set diffuse part of light source
                          // 设置光源的漫射部分

                        hv_Indices.Dispose();

                        hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                            "light_diffuse");

                        if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                            new HTuple())))) != 0)
                        {
                            //If the diffuse part is set multiple times, use the last setting
                            //如果漫反射部分被多次设置，则使用最后一次设置
                            hv_LightParam.Dispose();

                            hv_LightParam = ((((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleSplit(
                                new HTuple(", ")))).TupleNumber();

                            if ((int)(new HTuple((new HTuple(hv_LightParam.TupleLength())).TupleLess(
                                3))) != 0)
                            {
                                throw new HalconException("light_diffuse must be given as a string that contains three space separated floating point numbers");
                            }

                            HOperatorSet.SetScene3dLightParam(hv_Scene3D, 0, "diffuse", hv_LightParam.TupleSelectRange(
                                0, 2));

                            {
                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamName_COPY_INP_TMP.Dispose();
                                hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                            }

                            {

                                HTuple ExpTmpOutVar_0;
                                HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                                hv_GenParamValue_COPY_INP_TMP.Dispose();
                                hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                            }

                        }
                    }
                    //
                    //Handle persistence parameters separately because persistence will
                    //only be activated immediately before leaving the visualization
                    //procedure
                    //单独处理持久化参数，因为持久化参数将
                    //只在离开可视化之前立即激活
                    //程序
                    hv_PersistenceParamName.Dispose();
                    hv_PersistenceParamName = new HTuple();
                    hv_PersistenceParamValue.Dispose();
                    hv_PersistenceParamValue = new HTuple();
                    //Set position of light source
                     // 设置光源位置
                    hv_Indices.Dispose();

                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "object_index_persistence");

                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                            (new HTuple(hv_Indices.TupleLength())) - 1)))).TupleEqual("true"))) != 0)
                        {

                            {
                                HTuple
                                  ExpTmpLocalVar_PersistenceParamName = hv_PersistenceParamName.TupleConcat(
                                    "object_index_persistence");
                                hv_PersistenceParamName.Dispose();
                                hv_PersistenceParamName = ExpTmpLocalVar_PersistenceParamName;
                            }


                            {
                                HTuple
                                  ExpTmpLocalVar_PersistenceParamValue = hv_PersistenceParamValue.TupleConcat(
                                    "true");
                                hv_PersistenceParamValue.Dispose();
                                hv_PersistenceParamValue = ExpTmpLocalVar_PersistenceParamValue;
                            }

                        }
                        else if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleEqual(
                            "false"))) != 0)
                        {
                        }
                        else
                        {
                            throw new HalconException("Wrong value for parameter 'object_index_persistence' (must be either 'true' or 'false')");
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    hv_Indices.Dispose();

                    hv_Indices = hv_GenParamName_COPY_INP_TMP.TupleFind(
                        "depth_persistence");

                    if ((int)((new HTuple(hv_Indices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_Indices.TupleNotEqual(
                        new HTuple())))) != 0)
                    {
                        if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(hv_Indices.TupleSelect(
                            (new HTuple(hv_Indices.TupleLength())) - 1)))).TupleEqual("true"))) != 0)
                        {

                            {
                                HTuple
                                  ExpTmpLocalVar_PersistenceParamName = hv_PersistenceParamName.TupleConcat(
                                    "depth_persistence");
                                hv_PersistenceParamName.Dispose();
                                hv_PersistenceParamName = ExpTmpLocalVar_PersistenceParamName;
                            }


                            {
                                HTuple
                                  ExpTmpLocalVar_PersistenceParamValue = hv_PersistenceParamValue.TupleConcat(
                                    "true");
                                hv_PersistenceParamValue.Dispose();
                                hv_PersistenceParamValue = ExpTmpLocalVar_PersistenceParamValue;
                            }

                        }
                        else if ((int)(new HTuple(((hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1)))).TupleEqual(
                            "false"))) != 0)
                        {
                        }
                        else
                        {
                            throw new HalconException("Wrong value for parameter 'depth_persistence' (must be either 'true' or 'false')");
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamName_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamName_COPY_INP_TMP.Dispose();
                            hv_GenParamName_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleRemove(hv_GenParamValue_COPY_INP_TMP, hv_Indices, out ExpTmpOutVar_0);
                            hv_GenParamValue_COPY_INP_TMP.Dispose();
                            hv_GenParamValue_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    //
                    //Parse the generic parameters
                    //- First, all parameters that are understood by set_scene_3d_instance_param
                    //解析通用参数
                    //- 首先是 set_scene_3d_instance_param 可以理解的所有参数
                    hv_AlphaOrig.Dispose();

                    hv_AlphaOrig = HTuple.TupleGenConst(
                        hv_NumModels, 1);

                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_GenParamName_COPY_INP_TMP.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_ParamName.Dispose();

                        hv_ParamName = hv_GenParamName_COPY_INP_TMP.TupleSelect(
                            hv_I);

                        hv_ParamValue.Dispose();

                        hv_ParamValue = hv_GenParamValue_COPY_INP_TMP.TupleSelect(
                            hv_I);

                        //Check if this parameter is understood by set_scene_3d_param
                        if ((int)(new HTuple(hv_ParamName.TupleEqual("alpha"))) != 0)
                        {
                            hv_AlphaOrig.Dispose();

                            hv_AlphaOrig = HTuple.TupleGenConst(
                                hv_NumModels, hv_ParamValue);

                        }
                        try
                        {
                            HOperatorSet.SetScene3dParam(hv_Scene3D, hv_ParamName, hv_ParamValue);
                            continue;
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException2)
                        {
                            HDevExpDefaultException2.ToHTuple(out hv_Exception);
                            if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1203))).TupleOr(
                                new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1303)))) != 0)
                            {
                                if ((int)((new HTuple((new HTuple((new HTuple(hv_ParamName.TupleEqual(
                                    "color_attrib"))).TupleOr(new HTuple(hv_ParamName.TupleEqual("red_channel_attrib"))))).TupleOr(
                                    new HTuple(hv_ParamName.TupleEqual("green_channel_attrib"))))).TupleOr(
                                    new HTuple(hv_ParamName.TupleEqual("blue_channel_attrib")))) != 0)
                                {
                                    throw new HalconException(((((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue) + ". ") + hv_ParamValue) + " may not be attached to the points of the 3D object model. Compare the parameter AttachExtAttribTo of set_object_model_3d_attrib.");
                                }
                                else
                                {
                                    throw new HalconException((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue);
                                }
                            }
                        }
                        //Check if it is a parameter that is valid for only one instance
                        //and therefore can be set only with set_scene_3d_instance_param
                       // 检查该参数是否只对一个实例有效
                        //因此只能通过 set_scene_3d_instance_param 进行设置
                        hv_ParamNameTrunk.Dispose();

                        hv_ParamNameTrunk = hv_ParamName.TupleRegexpReplace(
                            "_\\d+$", "");

                        if ((int)(new HTuple(hv_ParamName.TupleEqual(hv_ParamNameTrunk))) != 0)
                        {
                            hv_Instance.Dispose();

                            hv_Instance = HTuple.TupleGenSequence(
                                0, hv_NumModels - 1, 1);

                        }
                        else
                        {
                            hv_Instance.Dispose();

                            hv_Instance = ((hv_ParamName.TupleRegexpReplace(
                                ("^" + hv_ParamNameTrunk) + "_(\\d+)$", "$1"))).TupleNumber();

                            if ((int)((new HTuple(hv_Instance.TupleLess(0))).TupleOr(new HTuple(hv_Instance.TupleGreater(
                                hv_NumModels - 1)))) != 0)
                            {
                                throw new HalconException(("Parameter " + hv_ParamName) + " refers to a non existing 3D object model");
                            }
                        }
                        try
                        {
                            HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Instance, hv_ParamNameTrunk,
                                hv_ParamValue);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException2)
                        {
                            HDevExpDefaultException2.ToHTuple(out hv_Exception);
                            if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1204))).TupleOr(
                                new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1304)))) != 0)
                            {
                                if ((int)((new HTuple((new HTuple((new HTuple(hv_ParamNameTrunk.TupleEqual(
                                    "color_attrib"))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                    "red_channel_attrib"))))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                    "green_channel_attrib"))))).TupleOr(new HTuple(hv_ParamNameTrunk.TupleEqual(
                                    "blue_channel_attrib")))) != 0)
                                {
                                    throw new HalconException(((((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue) + ". ") + hv_ParamValue) + " may not be attached to the points of the 3D object model. Compare the parameter AttachExtAttribTo of set_object_model_3d_attrib.");
                                }
                                else
                                {
                                    throw new HalconException((("Wrong type or value for parameter " + hv_ParamName) + ": ") + hv_ParamValue);
                                }
                            }
                            else if ((int)((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                                1203))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                                1303)))) != 0)
                            {
                                throw new HalconException("Wrong parameter name " + hv_ParamName);
                            }
                            else
                            {
                                throw new HalconException(hv_Exception);
                            }
                        }
                        if ((int)(new HTuple(hv_ParamNameTrunk.TupleEqual("alpha"))) != 0)
                        {
                            if (hv_AlphaOrig == null)
                                hv_AlphaOrig = new HTuple();
                            hv_AlphaOrig[hv_Instance] = hv_ParamValue;
                        }
                    }
                    //
                    //Start the visualization loop
                    // 启动可视化循环
                    hv_HomMat3D.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_Poses.TupleSelectRange(0, 6), out hv_HomMat3D);


                    hv_Qx.Dispose(); hv_Qy.Dispose(); hv_Qz.Dispose();
                    HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0), hv_Center.TupleSelect(
                        1), hv_Center.TupleSelect(2), out hv_Qx, out hv_Qy, out hv_Qz);

                    hv_TBCenter.Dispose();

                    hv_TBCenter = new HTuple();
                    hv_TBCenter = hv_TBCenter.TupleConcat(hv_Qx, hv_Qy, hv_Qz);

                    hv_TBSize.Dispose();

                    hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum()
                        )) / hv_NumModels)) * hv_TrackballRadiusPixel;

                    hv_ButtonHold.Dispose();
                    hv_ButtonHold = 0;
                    while ((int)(1) != 0)
                    {
                        hv_VisualizeTB.Dispose();

                        hv_VisualizeTB = new HTuple(((hv_SelectedObject.TupleMax()
                            )).TupleNotEqual(0));

                        hv_MaxIndex.Dispose();

                        hv_MaxIndex = ((((new HTuple(hv_ObjectModel3D.TupleLength()
                            )).TupleConcat(hv_MaxNumModels))).TupleMin()) - 1;

                        //Set trackball fixed in the center of the window
                        //将轨迹球固定在窗口中央
                        hv_TrackballCenterRow.Dispose();

                        hv_TrackballCenterRow = hv_Height / 2;

                        hv_TrackballCenterCol.Dispose();

                        hv_TrackballCenterCol = hv_Width / 2;

                        if ((int)(new HTuple(hv_WindowCenteredRotation.TupleEqual(1))) != 0)
                        {
                            try
                            {

                                hv_TBCenter.Dispose(); hv_TBSize.Dispose();
                                Get_trackball_center_fixed(hv_SelectedObject.TupleSelectRange(0, hv_MaxIndex),
                                    hv_TrackballCenterRow, hv_TrackballCenterCol, hv_TrackballRadiusPixel,
                                    hv_Scene3D, hv_ObjectModel3D.TupleSelectRange(0, hv_MaxIndex), hv_Poses.TupleSelectRange(
                                    0, ((hv_MaxIndex + 1) * 7) - 1), hv_WindowHandleBuffer, hv_CamParam_COPY_INP_TMP,
                                    hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP, out hv_TBCenter,
                                    out hv_TBSize);

                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException2)
                            {
                                HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                Disp_message(hv_WindowHandle, "Surface inspection mode is not available.",
                                    "image", 5, 20, "red", "true");
                                hv_WindowCenteredRotation.Dispose();
                                hv_WindowCenteredRotation = 2;

                                hv_TBCenter.Dispose(); hv_TBSize.Dispose();
                                Get_trackball_center(hv_SelectedObject.TupleSelectRange(0, hv_MaxIndex),
                                    hv_TrackballRadiusPixel, hv_ObjectModel3D.TupleSelectRange(0, hv_MaxIndex),
                                    hv_Poses.TupleSelectRange(0, ((hv_MaxIndex + 1) * 7) - 1), out hv_TBCenter,
                                    out hv_TBSize);

                                HOperatorSet.WaitSeconds(1);
                            }
                        }
                        else
                        {

                            hv_TBCenter.Dispose(); hv_TBSize.Dispose();
                            Get_trackball_center(hv_SelectedObject.TupleSelectRange(0, hv_MaxIndex),
                                hv_TrackballRadiusPixel, hv_ObjectModel3D.TupleSelectRange(0, hv_MaxIndex),
                                hv_Poses.TupleSelectRange(0, ((hv_MaxIndex + 1) * 7) - 1), out hv_TBCenter,
                                out hv_TBSize);

                        }
                        Dump_image_output(ho_Image, hv_WindowHandleBuffer, hv_Scene3D, hv_AlphaOrig,
                            hv_ObjectModel3D, hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP,
                            hv_CamParam_COPY_INP_TMP, hv_Poses, hv_ColorImage, hv_Title, hv_Information,
                            hv_Label_COPY_INP_TMP, hv_VisualizeTB, "true", hv_TrackballCenterRow,
                            hv_TrackballCenterCol, hv_TBSize, hv_SelectedObject, hv_WindowCenteredRotation,
                            hv_TBCenter);
                        ho_ImageDump.Dispose();
                        HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                        HDevWindowStack.SetActive(hv_WindowHandle);
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                        }
                        //
                        //Check for mouse events
                        //检查鼠标事件
                        hv_GraphEvent.Dispose();
                        hv_GraphEvent = 0;
                        hv_Exit.Dispose();
                        hv_Exit = 0;
                        while ((int)(1) != 0)
                        {
                            //
                            //Check graphic event
                            //检查图形事件
                            try
                            {
                                hv_GraphButtonRow.Dispose(); hv_GraphButtonColumn.Dispose(); hv_GraphButton.Dispose();
                                HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_GraphButtonRow,
                                    out hv_GraphButtonColumn, out hv_GraphButton);
                                if ((int)(new HTuple(hv_GraphButton.TupleNotEqual(0))) != 0)
                                {
                                    if ((int)((new HTuple((new HTuple((new HTuple(hv_GraphButtonRow.TupleGreater(
                                        (hv_Height - hv_TextHeight) - 25))).TupleAnd(new HTuple(hv_GraphButtonRow.TupleLess(
                                        hv_Height))))).TupleAnd(new HTuple(hv_GraphButtonColumn.TupleGreater(
                                        (hv_Width - hv_TextWidth) - 15))))).TupleAnd(new HTuple(hv_GraphButtonColumn.TupleLess(
                                        hv_Width)))) != 0)
                                    {
                                        //Wait until the continue button has been released
                                        //等待直到继续按钮被释放
                                        if ((int)(new HTuple(hv_WaitForButtonRelease.TupleEqual("true"))) != 0)
                                        {
                                            while ((int)(1) != 0)
                                            {
                                                hv_GraphButtonRow.Dispose(); hv_GraphButtonColumn.Dispose(); hv_GraphButton.Dispose();
                                                HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_GraphButtonRow,
                                                    out hv_GraphButtonColumn, out hv_GraphButton);
                                                if ((int)((new HTuple(hv_GraphButton.TupleEqual(0))).TupleOr(
                                                    new HTuple(hv_GraphButton.TupleEqual(new HTuple())))) != 0)
                                                {
                                                    if ((int)((new HTuple((new HTuple((new HTuple(hv_GraphButtonRow.TupleGreater(
                                                        (hv_Height - hv_TextHeight) - 25))).TupleAnd(new HTuple(hv_GraphButtonRow.TupleLess(
                                                        hv_Height))))).TupleAnd(new HTuple(hv_GraphButtonColumn.TupleGreater(
                                                        (hv_Width - hv_TextWidth) - 15))))).TupleAnd(new HTuple(hv_GraphButtonColumn.TupleLess(
                                                        hv_Width)))) != 0)
                                                    {
                                                        hv_ButtonReleased.Dispose();
                                                        hv_ButtonReleased = 1;
                                                    }
                                                    else
                                                    {
                                                        hv_ButtonReleased.Dispose();
                                                        hv_ButtonReleased = 0;
                                                    }
                                                    //
                                                    break;
                                                }
                                                //Keep waiting until mouse button is released or moved out of the window
                                                //继续等待，直到鼠标按钮被释放或移出窗口
                                            }
                                        }
                                        else
                                        {
                                            hv_ButtonReleased.Dispose();
                                            hv_ButtonReleased = 1;
                                        }
                                        //Exit the visualization loop
                                        // 退出可视化循环
                                        if ((int)(hv_ButtonReleased) != 0)
                                        {
                                            hv_Exit.Dispose();
                                            hv_Exit = 1;
                                            break;
                                        }
                                    }
                                    hv_GraphEvent.Dispose();
                                    hv_GraphEvent = 1;
                                    break;
                                }
                                else
                                {
                                    hv_ButtonHold.Dispose();
                                    hv_ButtonHold = 0;
                                }
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException2)
                            {
                                HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                //Keep waiting
                                //继续等待
                            }
                        }
                        if ((int)(hv_GraphEvent) != 0)
                        {
                            {
                                HTuple ExpTmpOutVar_0; HTuple ExpTmpOutVar_1; HTuple ExpTmpOutVar_2; HTuple ExpTmpOutVar_3;
                                Analyze_graph_event(ho_Image, hv_MouseMapping, hv_GraphButton, hv_GraphButtonRow,
                                    hv_GraphButtonColumn, hv_WindowHandle, hv_WindowHandleBuffer, hv_VirtualTrackball,
                                    hv_TrackballSize, hv_SelectedObject, hv_Scene3D, hv_AlphaOrig, hv_ObjectModel3D,
                                    hv_CamParam_COPY_INP_TMP, hv_Label_COPY_INP_TMP, hv_Title, hv_Information,
                                    hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP, hv_Poses,
                                    hv_ButtonHold, hv_TBCenter, hv_TBSize, hv_WindowCenteredRotation,
                                    hv_MaxNumModels, out ExpTmpOutVar_0, out ExpTmpOutVar_1, out ExpTmpOutVar_2,
                                    out ExpTmpOutVar_3);
                                hv_Poses.Dispose();
                                hv_Poses = ExpTmpOutVar_0;
                                hv_SelectedObject.Dispose();
                                hv_SelectedObject = ExpTmpOutVar_1;
                                hv_ButtonHold.Dispose();
                                hv_ButtonHold = ExpTmpOutVar_2;
                                hv_WindowCenteredRotation.Dispose();
                                hv_WindowCenteredRotation = ExpTmpOutVar_3;
                            }
                        }
                        if ((int)(hv_Exit) != 0)
                        {
                            break;
                        }
                    }
                    //
                    //Display final state with persistence, if requested
                    //Note that disp_object_model_3d must be used instead of the 3D scene
                    //如果需要，显示具有持久性的最终状态
                    //注意必须使用 disp_object_model_3d，而不是 3D 场景
                    if ((int)(new HTuple((new HTuple(hv_PersistenceParamName.TupleLength())).TupleGreater(
                        0))) != 0)
                    {

                        HOperatorSet.DispObjectModel3d(hv_WindowHandle, hv_ObjectModel3D, hv_CamParam_COPY_INP_TMP,
                            hv_Poses, ((new HTuple("disp_background")).TupleConcat("alpha")).TupleConcat(
                            hv_PersistenceParamName), ((new HTuple("true")).TupleConcat(0.0)).TupleConcat(
                            hv_PersistenceParamValue));

                    }
                    //
                    //Compute the output pose
                    // 计算输出姿势
                    if ((int)(gUsesOpenGL) != 0)
                    {
                        hv_PoseOut.Dispose();

                        hv_PoseOut = hv_Poses.TupleSelectRange(
                            0, 6);

                    }
                    else
                    {
                        hv_PoseOut.Dispose();
                        hv_PoseOut = new HTuple(hv_Poses);
                    }
                    //
                    //Clean up.
                    //清理
                    HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                    // dev_set_preferences(...); only in hdevelop
                    // dev_set_preferences(...); only in hdevelop
                    Dump_image_output(ho_Image, hv_WindowHandleBuffer, hv_Scene3D, hv_AlphaOrig,
                        hv_ObjectModel3D, hv_GenParamName_COPY_INP_TMP, hv_GenParamValue_COPY_INP_TMP,
                        hv_CamParam_COPY_INP_TMP, hv_Poses, hv_ColorImage, hv_Title, new HTuple(),
                        hv_Label_COPY_INP_TMP, 0, "false", hv_TrackballCenterRow, hv_TrackballCenterCol,
                        hv_TBSize, hv_SelectedObject, hv_WindowCenteredRotation, hv_TBCenter);
                    ho_ImageDump.Dispose();
                    HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                    HDevWindowStack.SetActive(hv_WindowHandle);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                    }
                    HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
                    HOperatorSet.SetPart(hv_WindowHandle, hv_WPRow1, hv_WPColumn1, hv_WPRow2,
                        hv_WPColumn2);
                    HOperatorSet.ClearScene3d(hv_Scene3D);
                    hv_Scene3D.Dispose();
                    hv_Scene3D = new HTuple();
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    try
                    {
                        //Try to clean up as much as possible.
                        // 尽可能清理干净。
                        if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_Scene3DTest.TupleLength()
                            )))) != 0)
                        {
                            HOperatorSet.ClearScene3d(hv_Scene3DTest);
                            hv_Scene3DTest.Dispose();
                            hv_Scene3DTest = new HTuple();
                        }
                        if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_Scene3D.TupleLength()
                            )))) != 0)
                        {
                            HOperatorSet.ClearScene3d(hv_Scene3D);
                            hv_Scene3D.Dispose();
                            hv_Scene3D = new HTuple();
                        }
                        if ((int)(new HTuple((new HTuple(0)).TupleLess(new HTuple(hv_WindowHandleBuffer.TupleLength()
                            )))) != 0)
                        {
                            HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
                            hv_WindowHandleBuffer.Dispose();
                            hv_WindowHandleBuffer = new HTuple();
                        }
                    }
                    // catch (e) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_e);
                        //Suppress all further exceptions to return the original exception.
                        //抑制所有其他异常，返回原始异常。
                    }
                    try
                    {
                        //Restore system settings.
                        //恢复系统设置。
                        HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                        // dev_set_preferences(...); only in hdevelop
                        // dev_set_preferences(...); only in hdevelop
                    }
                    // catch (e) 
                    catch (HalconException HDevExpDefaultException2)
                    {
                        HDevExpDefaultException2.ToHTuple(out hv_e);
                        //Suppress all further exceptions to return the original exception.
                        //抑制所有其他异常，返回原始异常。
                    }
                    //
                    throw new HalconException(hv_Exception);
                }
                ho_Image.Dispose();
                ho_ImageDump.Dispose();

                hv_CamParam_COPY_INP_TMP.Dispose();
                hv_GenParamName_COPY_INP_TMP.Dispose();
                hv_GenParamValue_COPY_INP_TMP.Dispose();
                hv_Label_COPY_INP_TMP.Dispose();
                hv_PoseIn_COPY_INP_TMP.Dispose();
                hv_Scene3DTest.Dispose();
                hv_Scene3D.Dispose();
                hv_WindowHandleBuffer.Dispose();
                hv_TrackballSize.Dispose();
                hv_VirtualTrackball.Dispose();
                hv_MouseMapping.Dispose();
                hv_WaitForButtonRelease.Dispose();
                hv_MaxNumModels.Dispose();
                hv_WindowCenteredRotation.Dispose();
                hv_NumModels.Dispose();
                hv_SelectedObject.Dispose();
                hv_ClipRegion.Dispose();
                hv_CPLength.Dispose();
                hv_RowNotUsed.Dispose();
                hv_ColumnNotUsed.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WPRow1.Dispose();
                hv_WPColumn1.Dispose();
                hv_WPRow2.Dispose();
                hv_WPColumn2.Dispose();
                hv_CamParamValue.Dispose();
                hv_CamWidth.Dispose();
                hv_CamHeight.Dispose();
                hv_Scale.Dispose();
                hv_Indices.Dispose();
                hv_DispBackground.Dispose();
                hv_Mask.Dispose();
                hv_Center.Dispose();
                hv_PoseEstimated.Dispose();
                hv_Poses.Dispose();
                hv_HomMat3Ds.Dispose();
                hv_Sequence.Dispose();
                hv_Font.Dispose();
                hv_Exception.Dispose();
                hv_OpenGLInfo.Dispose();
                hv_DummyObjectModel3D.Dispose();
                hv_CameraIndexTest.Dispose();
                hv_PoseTest.Dispose();
                hv_InstanceIndexTest.Dispose();
                hv_MinImageSize.Dispose();
                hv_TrackballRadiusPixel.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv_NumChannels.Dispose();
                hv_ColorImage.Dispose();
                hv_CameraIndex.Dispose();
                hv_AllInstances.Dispose();
                hv_SetLight.Dispose();
                hv_LightParam.Dispose();
                hv_LightPosition.Dispose();
                hv_LightKind.Dispose();
                hv_LightIndex.Dispose();
                hv_PersistenceParamName.Dispose();
                hv_PersistenceParamValue.Dispose();
                hv_AlphaOrig.Dispose();
                hv_I.Dispose();
                hv_ParamName.Dispose();
                hv_ParamValue.Dispose();
                hv_ParamNameTrunk.Dispose();
                hv_Instance.Dispose();
                hv_HomMat3D.Dispose();
                hv_Qx.Dispose();
                hv_Qy.Dispose();
                hv_Qz.Dispose();
                hv_TBCenter.Dispose();
                hv_TBSize.Dispose();
                hv_ButtonHold.Dispose();
                hv_VisualizeTB.Dispose();
                hv_MaxIndex.Dispose();
                hv_TrackballCenterRow.Dispose();
                hv_TrackballCenterCol.Dispose();
                hv_GraphEvent.Dispose();
                hv_Exit.Dispose();
                hv_GraphButtonRow.Dispose();
                hv_GraphButtonColumn.Dispose();
                hv_GraphButton.Dispose();
                hv_ButtonReleased.Dispose();
                hv_e.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_Image.Dispose();
                ho_ImageDump.Dispose();

                hv_CamParam_COPY_INP_TMP.Dispose();
                hv_GenParamName_COPY_INP_TMP.Dispose();
                hv_GenParamValue_COPY_INP_TMP.Dispose();
                hv_Label_COPY_INP_TMP.Dispose();
                hv_PoseIn_COPY_INP_TMP.Dispose();
                hv_Scene3DTest.Dispose();
                hv_Scene3D.Dispose();
                hv_WindowHandleBuffer.Dispose();
                hv_TrackballSize.Dispose();
                hv_VirtualTrackball.Dispose();
                hv_MouseMapping.Dispose();
                hv_WaitForButtonRelease.Dispose();
                hv_MaxNumModels.Dispose();
                hv_WindowCenteredRotation.Dispose();
                hv_NumModels.Dispose();
                hv_SelectedObject.Dispose();
                hv_ClipRegion.Dispose();
                hv_CPLength.Dispose();
                hv_RowNotUsed.Dispose();
                hv_ColumnNotUsed.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WPRow1.Dispose();
                hv_WPColumn1.Dispose();
                hv_WPRow2.Dispose();
                hv_WPColumn2.Dispose();
                hv_CamParamValue.Dispose();
                hv_CamWidth.Dispose();
                hv_CamHeight.Dispose();
                hv_Scale.Dispose();
                hv_Indices.Dispose();
                hv_DispBackground.Dispose();
                hv_Mask.Dispose();
                hv_Center.Dispose();
                hv_PoseEstimated.Dispose();
                hv_Poses.Dispose();
                hv_HomMat3Ds.Dispose();
                hv_Sequence.Dispose();
                hv_Font.Dispose();
                hv_Exception.Dispose();
                hv_OpenGLInfo.Dispose();
                hv_DummyObjectModel3D.Dispose();
                hv_CameraIndexTest.Dispose();
                hv_PoseTest.Dispose();
                hv_InstanceIndexTest.Dispose();
                hv_MinImageSize.Dispose();
                hv_TrackballRadiusPixel.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv_NumChannels.Dispose();
                hv_ColorImage.Dispose();
                hv_CameraIndex.Dispose();
                hv_AllInstances.Dispose();
                hv_SetLight.Dispose();
                hv_LightParam.Dispose();
                hv_LightPosition.Dispose();
                hv_LightKind.Dispose();
                hv_LightIndex.Dispose();
                hv_PersistenceParamName.Dispose();
                hv_PersistenceParamValue.Dispose();
                hv_AlphaOrig.Dispose();
                hv_I.Dispose();
                hv_ParamName.Dispose();
                hv_ParamValue.Dispose();
                hv_ParamNameTrunk.Dispose();
                hv_Instance.Dispose();
                hv_HomMat3D.Dispose();
                hv_Qx.Dispose();
                hv_Qy.Dispose();
                hv_Qz.Dispose();
                hv_TBCenter.Dispose();
                hv_TBSize.Dispose();
                hv_ButtonHold.Dispose();
                hv_VisualizeTB.Dispose();
                hv_MaxIndex.Dispose();
                hv_TrackballCenterRow.Dispose();
                hv_TrackballCenterCol.Dispose();
                hv_GraphEvent.Dispose();
                hv_Exit.Dispose();
                hv_GraphButtonRow.Dispose();
                hv_GraphButtonColumn.Dispose();
                hv_GraphButton.Dispose();
                hv_ButtonReleased.Dispose();
                hv_e.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Chapter: Calibration / Camera Parameters
        // Short Description: Generate a camera parameter tuple for an area scan camera with distortions modeled by the division model. 
        private static void Gen_cam_par_area_scan_division(HTuple hv_Focus, HTuple hv_Kappa, HTuple hv_Sx,
            HTuple hv_Sy, HTuple hv_Cx, HTuple hv_Cy, HTuple hv_ImageWidth, HTuple hv_ImageHeight,
            out HTuple hv_CameraParam)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_CameraParam = new HTuple();
            //Generate a camera parameter tuple for an area scan camera
            //with distortions modeled by the division model.
            //
            hv_CameraParam.Dispose();
            hv_CameraParam = new HTuple();
            hv_CameraParam[0] = "area_scan_division";
            hv_CameraParam = hv_CameraParam.TupleConcat(hv_Focus, hv_Kappa, hv_Sx, hv_Sy, hv_Cx, hv_Cy, hv_ImageWidth, hv_ImageHeight);



            return;
        }


        // Chapter: Calibration / Camera Parameters
        // Short Description: Set the value of a specified camera parameter in the camera parameter tuple. 
        public static void Set_cam_par_data(HTuple hv_CameraParamIn, HTuple hv_ParamName, HTuple hv_ParamValue,
            out HTuple hv_CameraParamOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CameraType = new HTuple(), hv_CameraParamNames = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ParamNameInd = new HTuple();
            HTuple hv_I = new HTuple(), hv_IsTelecentric = new HTuple();
            // Initialize local and output iconic variables 
            hv_CameraParamOut = new HTuple();
            try
            {
                //set_cam_par_data sets the value of the parameter that
                //is given in ParamName in the tuple of camera parameters
                //given in CameraParamIn. The modified camera parameters
                //are returned in CameraParamOut.
                //
                //Check for consistent length of input parameters
                if ((int)(new HTuple((new HTuple(hv_ParamName.TupleLength())).TupleNotEqual(
                    new HTuple(hv_ParamValue.TupleLength())))) != 0)
                {
                    throw new HalconException("Different number of values in ParamName and ParamValue");
                }
                //First, get the parameter names that correspond to the
                //elements in the input camera parameter tuple.
                hv_CameraType.Dispose(); hv_CameraParamNames.Dispose();
                Get_cam_par_names(hv_CameraParamIn, out hv_CameraType, out hv_CameraParamNames);
                //
                //Find the index of the requested camera data and return
                //the corresponding value.
                hv_CameraParamOut.Dispose();
                hv_CameraParamOut = new HTuple(hv_CameraParamIn);
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ParamName.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_ParamNameInd.Dispose();

                    hv_ParamNameInd = hv_ParamName.TupleSelect(
                        hv_Index);

                    hv_I.Dispose();

                    hv_I = hv_CameraParamNames.TupleFind(
                        hv_ParamNameInd);

                    if ((int)(new HTuple(hv_I.TupleNotEqual(-1))) != 0)
                    {
                        if (hv_CameraParamOut == null)
                            hv_CameraParamOut = new HTuple();
                        hv_CameraParamOut[hv_I] = hv_ParamValue.TupleSelect(hv_Index);
                    }
                    else
                    {
                        throw new HalconException("Wrong ParamName " + hv_ParamNameInd);
                    }
                    //Check the consistency of focus and telecentricity
                    if ((int)(new HTuple(hv_ParamNameInd.TupleEqual("focus"))) != 0)
                    {
                        hv_IsTelecentric.Dispose();

                        hv_IsTelecentric = (new HTuple(((hv_CameraType.TupleStrstr(
                            "telecentric"))).TupleNotEqual(-1))).TupleAnd(new HTuple(((hv_CameraType.TupleStrstr(
                            "image_side_telecentric"))).TupleEqual(-1)));

                        if ((int)(hv_IsTelecentric) != 0)
                        {
                            throw new HalconException(new HTuple("Focus for telecentric lenses is always 0, and hence, cannot be changed."));
                        }
                        if ((int)((new HTuple(hv_IsTelecentric.TupleNot())).TupleAnd(new HTuple(((hv_ParamValue.TupleSelect(
                            hv_Index))).TupleEqual(0.0)))) != 0)
                        {
                            throw new HalconException("Focus for non-telecentric lenses must not be 0.");
                        }
                    }
                }

                hv_CameraType.Dispose();
                hv_CameraParamNames.Dispose();
                hv_Index.Dispose();
                hv_ParamNameInd.Dispose();
                hv_I.Dispose();
                hv_IsTelecentric.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_CameraType.Dispose();
                hv_CameraParamNames.Dispose();
                hv_Index.Dispose();
                hv_ParamNameInd.Dispose();
                hv_I.Dispose();
                hv_IsTelecentric.Dispose();

                //throw HDevExpDefaultException;
            }
        }




        // Chapter: Graphics / Text
        // Short Description: Write one or multiple text messages. 
        private static void Disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays text in a graphics window.
                //
                //Input parameters:
                //WindowHandle: The WindowHandle of the graphics window, where
                //   the message should be displayed.
                //String: A tuple of strings containing the text messages to be displayed.
                //CoordSystem: If set to 'window', the text position is given
                //   with respect to the window coordinate system.
                //   If set to 'image', image coordinates are used.
                //   (This may be useful in zoomed images.)
                //Row: The row coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Column: The column coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Color: defines the color of the text as string.
                //   If set to [] or '' the currently set color is used.
                //   If a tuple of strings is passed, the colors are used cyclically
                //   for every text position defined by Row and Column,
                //   or every new text line in case of |Row| == |Column| == 1.
                //Box: A tuple controlling a possible box surrounding the text.
                //   Its entries:
                //   - Box[0]: Controls the box and its color. Possible values:
                //     -- 'true' (Default): An orange box is displayed.
                //     -- 'false': No box is displayed.
                //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
                //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
                //     -- 'true' (Default): A shadow is displayed in
                //               darker orange if Box[0] is not a color and in 'white' otherwise.
                //     -- 'false': No shadow is displayed.
                //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
                //
                //It is possible to display multiple text strings in a single call.
                //In this case, some restrictions apply on the
                //parameters String, Row, and Column:
                //They can only have either 1 entry or n entries.
                //Behavior in the different cases:
                //   - Multiple text positions are specified, i.e.,
                //       - |Row| == n, |Column| == n
                //       - |Row| == n, |Column| == 1
                //       - |Row| == 1, |Column| == n
                //     In this case we distinguish:
                //       - |String| == n: Each element of String is displayed
                //                        at the corresponding position.
                //       - |String| == 1: String is displayed n times
                //                        at the corresponding positions.
                //   - Exactly one text position is specified,
                //      i.e., |Row| == |Column| == 1:
                //      Each element of String is display in a new textline.
                //
                //
                //Convert the parameters for disp_text.
                if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
                {

                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_GenParamName.Dispose();
                    hv_GenParamValue.Dispose();

                    return;
                }
                if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP = 12;
                }
                //
                //Convert the parameter Box to generic parameters.
                hv_GenParamName.Dispose();
                hv_GenParamName = new HTuple();
                hv_GenParamValue.Dispose();
                hv_GenParamValue = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                    {
                        //Display no box

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a color other than the default.

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                }
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                    {
                        //Display no shadow.

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a shadow color other than the default.

                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }


                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }

                    }
                }
                //Restore default CoordSystem behavior.
                if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
                {
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP = "image";
                }
                //
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    //disp_text does not accept an empty string for Color.
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Color_COPY_INP_TMP = new HTuple();
                }
                //
                HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                    hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                    hv_GenParamValue);

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Graphics / Output
        // Short Description: Get the center of the virtual trackback that is used to move the camera. 
        private static void Get_trackball_center(HTuple hv_SelectedObject, HTuple hv_TrackballRadiusPixel,
            HTuple hv_ObjectModel3D, HTuple hv_Poses, out HTuple hv_TBCenter, out HTuple hv_TBSize)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_NumModels = new HTuple(), hv_Diameter = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Center = new HTuple();
            HTuple hv_CurrDiameter = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_MD = new HTuple(), hv_Weight = new HTuple();
            HTuple hv_SumW = new HTuple(), hv_PoseSelected = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_TBCenterCamX = new HTuple();
            HTuple hv_TBCenterCamY = new HTuple(), hv_TBCenterCamZ = new HTuple();
            HTuple hv_InvSum = new HTuple();
            // Initialize local and output iconic variables 
            hv_TBCenter = new HTuple();
            hv_TBSize = new HTuple();
            try
            {
                //
                hv_NumModels.Dispose();

                hv_NumModels = new HTuple(hv_ObjectModel3D.TupleLength()
                    );

                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[0] = 0;
                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[1] = 0;
                if (hv_TBCenter == null)
                    hv_TBCenter = new HTuple();
                hv_TBCenter[2] = 0;
                hv_Diameter.Dispose();

                hv_Diameter = HTuple.TupleGenConst(
                    new HTuple(hv_ObjectModel3D.TupleLength()), 0.0);

                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3D.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    try
                    {

                        hv_Center.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "center", out hv_Center);


                        hv_CurrDiameter.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "diameter_axis_aligned_bounding_box", out hv_CurrDiameter);

                        if (hv_Diameter == null)
                            hv_Diameter = new HTuple();
                        hv_Diameter[hv_Index] = hv_CurrDiameter;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        //3D object model is empty or otherwise malformed -> ignore
                    }
                }
                //Normalize Diameter to use it as weights for a weighted mean of the individual centers
                hv_MD.Dispose();

                hv_MD = hv_Diameter.TupleMean();

                if ((int)(new HTuple(hv_MD.TupleGreater(1e-10))) != 0)
                {
                    hv_Weight.Dispose();

                    hv_Weight = hv_Diameter / hv_MD;

                }
                else
                {
                    hv_Weight.Dispose();
                    hv_Weight = new HTuple(hv_Diameter);
                }
                hv_SumW.Dispose();

                hv_SumW = ((hv_Weight.TupleSelectMask(
                    ((hv_SelectedObject.TupleSgn())).TupleAbs()))).TupleSum();

                if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_Weight = HTuple.TupleGenConst(
                            new HTuple(hv_Weight.TupleLength()), 1.0);
                        hv_Weight.Dispose();
                        hv_Weight = ExpTmpLocalVar_Weight;
                    }

                    hv_SumW.Dispose();

                    hv_SumW = ((hv_Weight.TupleSelectMask(
                        ((hv_SelectedObject.TupleSgn())).TupleAbs()))).TupleSum();

                }
                if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
                {
                    hv_SumW.Dispose();
                    hv_SumW = 1.0;
                }
                HTuple end_val30 = hv_NumModels - 1;
                HTuple step_val30 = 1;
                for (hv_Index = 0; hv_Index.Continue(end_val30, step_val30); hv_Index = hv_Index.TupleAdd(step_val30))
                {
                    if ((int)(((hv_SelectedObject.TupleSelect(hv_Index))).TupleAnd(new HTuple(((hv_Diameter.TupleSelect(
                        hv_Index))).TupleGreater(0)))) != 0)
                    {
                        hv_PoseSelected.Dispose();

                        hv_PoseSelected = hv_Poses.TupleSelectRange(
                            hv_Index * 7, (hv_Index * 7) + 6);

                        hv_HomMat3D.Dispose();
                        HOperatorSet.PoseToHomMat3d(hv_PoseSelected, out hv_HomMat3D);

                        hv_Center.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3D.TupleSelect(hv_Index),
                            "center", out hv_Center);


                        hv_TBCenterCamX.Dispose(); hv_TBCenterCamY.Dispose(); hv_TBCenterCamZ.Dispose();
                        HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0),
                            hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_TBCenterCamX,
                            out hv_TBCenterCamY, out hv_TBCenterCamZ);

                        if (hv_TBCenter == null)
                            hv_TBCenter = new HTuple();
                        hv_TBCenter[0] = (hv_TBCenter.TupleSelect(0)) + (hv_TBCenterCamX * (hv_Weight.TupleSelect(
                            hv_Index)));
                        if (hv_TBCenter == null)
                            hv_TBCenter = new HTuple();
                        hv_TBCenter[1] = (hv_TBCenter.TupleSelect(1)) + (hv_TBCenterCamY * (hv_Weight.TupleSelect(
                            hv_Index)));
                        if (hv_TBCenter == null)
                            hv_TBCenter = new HTuple();
                        hv_TBCenter[2] = (hv_TBCenter.TupleSelect(2)) + (hv_TBCenterCamZ * (hv_Weight.TupleSelect(
                            hv_Index)));
                    }
                }
                if ((int)(new HTuple(((hv_SelectedObject.TupleMax())).TupleNotEqual(0))) != 0)
                {
                    hv_InvSum.Dispose();

                    hv_InvSum = 1.0 / hv_SumW;

                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[0] = (hv_TBCenter.TupleSelect(0)) * hv_InvSum;
                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[1] = (hv_TBCenter.TupleSelect(1)) * hv_InvSum;
                    if (hv_TBCenter == null)
                        hv_TBCenter = new HTuple();
                    hv_TBCenter[2] = (hv_TBCenter.TupleSelect(2)) * hv_InvSum;
                    hv_TBSize.Dispose();

                    hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum()
                        )) / hv_NumModels)) * hv_TrackballRadiusPixel;

                }
                else
                {
                    hv_TBCenter.Dispose();
                    hv_TBCenter = new HTuple();
                    hv_TBSize.Dispose();
                    hv_TBSize = 0;
                }

                hv_NumModels.Dispose();
                hv_Diameter.Dispose();
                hv_Index.Dispose();
                hv_Center.Dispose();
                hv_CurrDiameter.Dispose();
                hv_Exception.Dispose();
                hv_MD.Dispose();
                hv_Weight.Dispose();
                hv_SumW.Dispose();
                hv_PoseSelected.Dispose();
                hv_HomMat3D.Dispose();
                hv_TBCenterCamX.Dispose();
                hv_TBCenterCamY.Dispose();
                hv_TBCenterCamZ.Dispose();
                hv_InvSum.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_NumModels.Dispose();
                hv_Diameter.Dispose();
                hv_Index.Dispose();
                hv_Center.Dispose();
                hv_CurrDiameter.Dispose();
                hv_Exception.Dispose();
                hv_MD.Dispose();
                hv_Weight.Dispose();
                hv_SumW.Dispose();
                hv_PoseSelected.Dispose();
                hv_HomMat3D.Dispose();
                hv_TBCenterCamX.Dispose();
                hv_TBCenterCamY.Dispose();
                hv_TBCenterCamZ.Dispose();
                hv_InvSum.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Graphics / Output
        // Short Description: Render 3D object models in a buffer window. 
        private static void Dump_image_output(HObject ho_BackgroundImage, HTuple hv_WindowHandleBuffer,
            HTuple hv_Scene3D, HTuple hv_AlphaOrig, HTuple hv_ObjectModel3DID, HTuple hv_GenParamName,
            HTuple hv_GenParamValue, HTuple hv_CamParam, HTuple hv_Poses, HTuple hv_ColorImage,
            HTuple hv_Title, HTuple hv_Information, HTuple hv_Labels, HTuple hv_VisualizeTrackball,
            HTuple hv_DisplayContinueButton, HTuple hv_TrackballCenterRow, HTuple hv_TrackballCenterCol,
            HTuple hv_TrackballRadiusPixel, HTuple hv_SelectedObject, HTuple hv_VisualizeRotationCenter,
            HTuple hv_RotationCenter)
        {




            // Local iconic variables 

            HObject ho_ModelContours = null, ho_TrackballContour = null;
            HObject ho_CrossRotCenter = null;

            // Local control variables 

            HTuple ExpTmpLocalVar_gUsesOpenGL = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Exception1 = new HTuple(), hv_DeselectedIdx = new HTuple();
            HTuple hv_DeselectedName = new HTuple(), hv_DeselectedValue = new HTuple();
            HTuple hv_Pose = new HTuple(), hv_HomMat3D = new HTuple();
            HTuple hv_Center = new HTuple(), hv_CenterCamX = new HTuple();
            HTuple hv_CenterCamY = new HTuple(), hv_CenterCamZ = new HTuple();
            HTuple hv_CenterRow = new HTuple(), hv_CenterCol = new HTuple();
            HTuple hv_Label = new HTuple(), hv_Sublabels = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
            HTuple hv_Index2 = new HTuple(), hv_TextWidth2 = new HTuple();
            HTuple hv_TextHeight2 = new HTuple(), hv_RotCenterRow = new HTuple();
            HTuple hv_RotCenterCol = new HTuple(), hv_Orientation = new HTuple();
            HTuple hv_Colors = new HTuple();
            HTuple hv_RotationCenter_COPY_INP_TMP = new HTuple(hv_RotationCenter);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_TrackballContour);
            HOperatorSet.GenEmptyObj(out ho_CrossRotCenter);
            try
            {
                //global tuple gAlphaDeselected
                //global tuple gTerminationButtonLabel
                //global tuple gDispObjOffset
                //global tuple gLabelsDecor
                //global tuple gUsesOpenGL
                //
                //Display background image
                HOperatorSet.ClearWindow(hv_WindowHandleBuffer);
                if ((int)(hv_ColorImage) != 0)
                {
                    HOperatorSet.DispColor(ho_BackgroundImage, hv_WindowHandleBuffer);
                }
                else
                {
                    HOperatorSet.DispImage(ho_BackgroundImage, hv_WindowHandleBuffer);
                }
                //
                //Display objects
                if ((int)(new HTuple(((hv_SelectedObject.TupleSum())).TupleEqual(new HTuple(hv_SelectedObject.TupleLength()
                    )))) != 0)
                {
                    if ((int)(new HTuple(gUsesOpenGL.TupleEqual("true"))) != 0)
                    {
                        try
                        {
                            HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            if ((int)((new HTuple((new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                                5185))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                                5188))))).TupleOr(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(
                                5187)))) != 0)
                            {
                                gUsesOpenGL = "false";
                                //ExpSetGlobalVar_gUsesOpenGL(ExpTmpLocalVar_gUsesOpenGL);
                            }
                            else
                            {
                                throw new HalconException(hv_Exception);
                            }
                        }
                    }
                    if ((int)(new HTuple(gUsesOpenGL.TupleEqual("false"))) != 0)
                    {
                        //* NO OpenGL, use fallback
                        ho_ModelContours.Dispose();
                        Disp_object_model_no_opengl(out ho_ModelContours, hv_ObjectModel3DID, hv_GenParamName,
                            hv_GenParamValue, hv_WindowHandleBuffer, hv_CamParam, hv_Poses);
                    }
                }
                else
                {
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_AlphaOrig.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if ((int)(new HTuple(((hv_SelectedObject.TupleSelect(hv_Index))).TupleEqual(
                            1))) != 0)
                        {

                            HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", hv_AlphaOrig.TupleSelect(
                                hv_Index));

                        }
                        else
                        {
                            HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", gAlphaDeselected);
                        }
                    }
                    try
                    {
                        if ((int)(new HTuple(gUsesOpenGL.TupleEqual("false"))) != 0)
                        {
                            throw new HalconException(new HTuple());
                        }
                        HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                    }
                    // catch (Exception1) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception1);
                        //* NO OpenGL, use fallback
                        hv_DeselectedIdx.Dispose();

                        hv_DeselectedIdx = hv_SelectedObject.TupleFind(
                            0);

                        if ((int)(new HTuple(hv_DeselectedIdx.TupleNotEqual(-1))) != 0)
                        {
                            hv_DeselectedName.Dispose();

                            hv_DeselectedName = "color_" + hv_DeselectedIdx;

                            hv_DeselectedValue.Dispose();

                            hv_DeselectedValue = HTuple.TupleGenConst(
                                new HTuple(hv_DeselectedName.TupleLength()), "gray");

                        }

                        ho_ModelContours.Dispose();
                        Disp_object_model_no_opengl(out ho_ModelContours, hv_ObjectModel3DID, hv_GenParamName.TupleConcat(
                            hv_DeselectedName), hv_GenParamValue.TupleConcat(hv_DeselectedValue),
                            hv_WindowHandleBuffer, hv_CamParam, hv_Poses);

                    }
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_AlphaOrig.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {

                        HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index, "alpha", hv_AlphaOrig.TupleSelect(
                            hv_Index));

                    }
                }
                //
                //Display labels
                if ((int)(new HTuple(hv_Labels.TupleNotEqual(new HTuple()))) != 0)
                {

                    HOperatorSet.SetColor(hv_WindowHandleBuffer, gLabelsDecor.TupleSelect(
                        0));

                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        //Project the center point of the current model
                        hv_Pose.Dispose();

                        hv_Pose = hv_Poses.TupleSelectRange(
                            hv_Index * 7, (hv_Index * 7) + 6);

                        hv_HomMat3D.Dispose();
                        HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);
                        try
                        {

                            hv_Center.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                                "center", out hv_Center);


                            hv_CenterCamX.Dispose(); hv_CenterCamY.Dispose(); hv_CenterCamZ.Dispose();
                            HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_Center.TupleSelect(0),
                                hv_Center.TupleSelect(1), hv_Center.TupleSelect(2), out hv_CenterCamX,
                                out hv_CenterCamY, out hv_CenterCamZ);

                            hv_CenterRow.Dispose(); hv_CenterCol.Dispose();
                            HOperatorSet.Project3dPoint(hv_CenterCamX, hv_CenterCamY, hv_CenterCamZ,
                                hv_CamParam, out hv_CenterRow, out hv_CenterCol);
                            hv_Label.Dispose();

                            hv_Label = hv_Labels.TupleSelect(
                                hv_Index);

                            if ((int)(new HTuple(hv_Label.TupleNotEqual(""))) != 0)
                            {
                                //Work around the fact that get_string_extents() does not handle newlines as we want
                                hv_Sublabels.Dispose();
                                HOperatorSet.TupleSplit(hv_Label, "\n", out hv_Sublabels);

                                hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth.Dispose(); hv_TextHeight.Dispose();
                                HOperatorSet.GetStringExtents(hv_WindowHandleBuffer, hv_Sublabels.TupleSelect(
                                    0), out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);

                                for (hv_Index2 = 1; (int)hv_Index2 <= (int)((new HTuple(hv_Sublabels.TupleLength()
                                    )) - 1); hv_Index2 = (int)hv_Index2 + 1)
                                {

                                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth2.Dispose(); hv_TextHeight2.Dispose();
                                    HOperatorSet.GetStringExtents(hv_WindowHandleBuffer, hv_Sublabels.TupleSelect(
                                        hv_Index2), out hv_Ascent, out hv_Descent, out hv_TextWidth2,
                                        out hv_TextHeight2);

                                    {
                                        HTuple
                                          ExpTmpLocalVar_TextHeight = hv_TextHeight + hv_TextHeight2;
                                        hv_TextHeight.Dispose();
                                        hv_TextHeight = ExpTmpLocalVar_TextHeight;
                                    }

                                    {
                                        HTuple
                                          ExpTmpLocalVar_TextWidth = hv_TextWidth.TupleMax2(
                                            hv_TextWidth2);
                                        hv_TextWidth.Dispose();
                                        hv_TextWidth = ExpTmpLocalVar_TextWidth;
                                    }

                                }

                                Disp_message(hv_WindowHandleBuffer, hv_Label, "window", (hv_CenterRow - (hv_TextHeight / 2)) + (gDispObjOffset.TupleSelect(
                                    0)), (hv_CenterCol - (hv_TextWidth / 2)) + (gDispObjOffset.TupleSelect(
                                    1)), new HTuple(), gLabelsDecor.TupleSelect(1));

                            }
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            //The 3D object model might not have a center because it is empty
                            //-> do not display any label
                        }
                    }
                }
                //
                //Visualize the trackball if desired
                if ((int)(hv_VisualizeTrackball) != 0)
                {
                    HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 1);
                    ho_TrackballContour.Dispose();
                    HOperatorSet.GenEllipseContourXld(out ho_TrackballContour, hv_TrackballCenterRow,
                        hv_TrackballCenterCol, 0, hv_TrackballRadiusPixel, hv_TrackballRadiusPixel,
                        0, 6.28318, "positive", 1.5);
                    HOperatorSet.SetColor(hv_WindowHandleBuffer, "dim gray");
                    HOperatorSet.DispXld(ho_TrackballContour, hv_WindowHandleBuffer);
                }
                //
                //Visualize the rotation center if desired
                if ((int)((new HTuple(hv_VisualizeRotationCenter.TupleNotEqual(0))).TupleAnd(
                    new HTuple((new HTuple(hv_RotationCenter_COPY_INP_TMP.TupleLength())).TupleEqual(
                    3)))) != 0)
                {
                    if ((int)(new HTuple(((hv_RotationCenter_COPY_INP_TMP.TupleSelect(2))).TupleLess(
                        1e-10))) != 0)
                    {
                        if (hv_RotationCenter_COPY_INP_TMP == null)
                            hv_RotationCenter_COPY_INP_TMP = new HTuple();
                        hv_RotationCenter_COPY_INP_TMP[2] = 1e-10;
                    }

                    hv_RotCenterRow.Dispose(); hv_RotCenterCol.Dispose();
                    HOperatorSet.Project3dPoint(hv_RotationCenter_COPY_INP_TMP.TupleSelect(0),
                        hv_RotationCenter_COPY_INP_TMP.TupleSelect(1), hv_RotationCenter_COPY_INP_TMP.TupleSelect(
                        2), hv_CamParam, out hv_RotCenterRow, out hv_RotCenterCol);

                    hv_Orientation.Dispose();

                    hv_Orientation = (new HTuple(90)).TupleRad()
                        ;

                    if ((int)(new HTuple(hv_VisualizeRotationCenter.TupleEqual(1))) != 0)
                    {
                        hv_Orientation.Dispose();

                        hv_Orientation = (new HTuple(45)).TupleRad()
                            ;

                    }

                    ho_CrossRotCenter.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_CrossRotCenter, hv_RotCenterRow, hv_RotCenterCol,
                        hv_TrackballRadiusPixel / 25.0, hv_Orientation);

                    HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 3);
                    hv_Colors.Dispose();
                    HOperatorSet.QueryColor(hv_WindowHandleBuffer, out hv_Colors);
                    HOperatorSet.SetColor(hv_WindowHandleBuffer, "light gray");
                    HOperatorSet.DispXld(ho_CrossRotCenter, hv_WindowHandleBuffer);
                    HOperatorSet.SetLineWidth(hv_WindowHandleBuffer, 1);
                    HOperatorSet.SetColor(hv_WindowHandleBuffer, "dim gray");
                    HOperatorSet.DispXld(ho_CrossRotCenter, hv_WindowHandleBuffer);
                }
                //
                //Display title
                Disp_title_and_information(hv_WindowHandleBuffer, hv_Title, hv_Information);
                //
                //Display the 'Exit' button
                if ((int)(new HTuple(hv_DisplayContinueButton.TupleEqual("true"))) != 0)
                {
                    Disp_continue_button(hv_WindowHandleBuffer);
                }
                //
                ho_ModelContours.Dispose();
                ho_TrackballContour.Dispose();
                ho_CrossRotCenter.Dispose();

                hv_RotationCenter_COPY_INP_TMP.Dispose();
                hv_Exception.Dispose();
                hv_Index.Dispose();
                hv_Exception1.Dispose();
                hv_DeselectedIdx.Dispose();
                hv_DeselectedName.Dispose();
                hv_DeselectedValue.Dispose();
                hv_Pose.Dispose();
                hv_HomMat3D.Dispose();
                hv_Center.Dispose();
                hv_CenterCamX.Dispose();
                hv_CenterCamY.Dispose();
                hv_CenterCamZ.Dispose();
                hv_CenterRow.Dispose();
                hv_CenterCol.Dispose();
                hv_Label.Dispose();
                hv_Sublabels.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv_Index2.Dispose();
                hv_TextWidth2.Dispose();
                hv_TextHeight2.Dispose();
                hv_RotCenterRow.Dispose();
                hv_RotCenterCol.Dispose();
                hv_Orientation.Dispose();
                hv_Colors.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_ModelContours.Dispose();
                ho_TrackballContour.Dispose();
                ho_CrossRotCenter.Dispose();

                hv_RotationCenter_COPY_INP_TMP.Dispose();
                hv_Exception.Dispose();
                hv_Index.Dispose();
                hv_Exception1.Dispose();
                hv_DeselectedIdx.Dispose();
                hv_DeselectedName.Dispose();
                hv_DeselectedValue.Dispose();
                hv_Pose.Dispose();
                hv_HomMat3D.Dispose();
                hv_Center.Dispose();
                hv_CenterCamX.Dispose();
                hv_CenterCamY.Dispose();
                hv_CenterCamZ.Dispose();
                hv_CenterRow.Dispose();
                hv_CenterCol.Dispose();
                hv_Label.Dispose();
                hv_Sublabels.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv_Index2.Dispose();
                hv_TextWidth2.Dispose();
                hv_TextHeight2.Dispose();
                hv_RotCenterRow.Dispose();
                hv_RotCenterCol.Dispose();
                hv_Orientation.Dispose();
                hv_Colors.Dispose();

                //throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Output
        // Short Description: Replace disp_object_model_3d if there is no OpenGL available. 
        private static void Disp_object_model_no_opengl(out HObject ho_ModelContours, HTuple hv_ObjectModel3DID,
            HTuple hv_GenParamName, HTuple hv_GenParamValue, HTuple hv_WindowHandleBuffer,
            HTuple hv_CamParam, HTuple hv_PosesOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Idx = new HTuple(), hv_CustomParamName = new HTuple();
            HTuple hv_CustomParamValue = new HTuple(), hv_Font = new HTuple();
            HTuple hv_IndicesDispBackGround = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_ImageWidth = new HTuple(), hv_HasPolygons = new HTuple();
            HTuple hv_HasTri = new HTuple(), hv_HasPoints = new HTuple();
            HTuple hv_HasLines = new HTuple(), hv_NumPoints = new HTuple();
            HTuple hv_IsPrimitive = new HTuple(), hv_Center = new HTuple();
            HTuple hv_Diameter = new HTuple(), hv_OpenGlHiddenSurface = new HTuple();
            HTuple hv_CenterX = new HTuple(), hv_CenterY = new HTuple();
            HTuple hv_CenterZ = new HTuple(), hv_PosObjectsZ = new HTuple();
            HTuple hv_I = new HTuple(), hv_Pose = new HTuple(), hv_HomMat3DObj = new HTuple();
            HTuple hv_PosObjCenterX = new HTuple(), hv_PosObjCenterY = new HTuple();
            HTuple hv_PosObjCenterZ = new HTuple(), hv_PosObjectsX = new HTuple();
            HTuple hv_PosObjectsY = new HTuple(), hv_Color = new HTuple();
            HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            HTuple hv_J = new HTuple(), hv_Indices3 = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_SampledObjectModel3D = new HTuple();
            HTuple hv_X = new HTuple(), hv_Y = new HTuple(), hv_Z = new HTuple();
            HTuple hv_HomMat3D1 = new HTuple(), hv_Qx = new HTuple();
            HTuple hv_Qy = new HTuple(), hv_Qz = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_ObjectModel3DConvexHull = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            try
            {
                //This procedure allows to use project_object_model_3d to simulate a disp_object_model_3d
                //call for small objects. Large objects are sampled down to display.
                hv_Idx.Dispose();

                hv_Idx = hv_GenParamName.TupleFind(
                    "point_size");

                if ((int)((new HTuple(hv_Idx.TupleLength())).TupleAnd(new HTuple(hv_Idx.TupleNotEqual(
                    -1)))) != 0)
                {
                    hv_CustomParamName.Dispose();
                    hv_CustomParamName = "point_size";
                    hv_CustomParamValue.Dispose();

                    hv_CustomParamValue = hv_GenParamValue.TupleSelect(
                        hv_Idx);
                }
                if ((int)(new HTuple(hv_CustomParamValue.TupleEqual(1))) != 0)
                {
                    hv_CustomParamValue.Dispose();
                    hv_CustomParamValue = 0;
                }

                else
                {
                    hv_CustomParamName.Dispose();
                    hv_CustomParamName = new HTuple();
                    hv_CustomParamValue.Dispose();
                    hv_CustomParamValue = new HTuple();
                }
                hv_Font.Dispose();
                HOperatorSet.GetFont(hv_WindowHandleBuffer, out hv_Font);
                hv_IndicesDispBackGround.Dispose();
                HOperatorSet.TupleFind(hv_GenParamName, "disp_background", out hv_IndicesDispBackGround);
                if ((int)(new HTuple(hv_IndicesDispBackGround.TupleNotEqual(-1))) != 0)
                {

                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_GenParamName.TupleSelect(hv_IndicesDispBackGround),
                        "false", out hv_Indices);

                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        HOperatorSet.ClearWindow(hv_WindowHandleBuffer);
                    }
                }
                Set_display_font(hv_WindowHandleBuffer, 11, "mono", "false", "false");
                hv_ImageWidth.Dispose();
                Get_cam_par_data(hv_CamParam, "image_width", out hv_ImageWidth);

                Disp_message(hv_WindowHandleBuffer, "OpenGL missing!", "image", 5, hv_ImageWidth - 130,
                    "red", "false");

                HOperatorSet.SetFont(hv_WindowHandleBuffer, hv_Font);
                hv_HasPolygons.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_polygons", out hv_HasPolygons);
                hv_HasTri.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_triangles", out hv_HasTri);
                hv_HasPoints.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_points", out hv_HasPoints);
                hv_HasLines.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_lines", out hv_HasLines);
                hv_NumPoints.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "num_points", out hv_NumPoints);
                hv_IsPrimitive.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "has_primitive_data",
                    out hv_IsPrimitive);
                hv_Center.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "center", out hv_Center);
                hv_Diameter.Dispose();
                HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID, "diameter", out hv_Diameter);
                hv_OpenGlHiddenSurface.Dispose();
                HOperatorSet.GetSystem("opengl_hidden_surface_removal_enable", out hv_OpenGlHiddenSurface);
                HOperatorSet.SetSystem("opengl_hidden_surface_removal_enable", "false");
                //Sort the objects by inverse z
                hv_CenterX.Dispose();

                hv_CenterX = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3));

                hv_CenterY.Dispose();

                hv_CenterY = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3) + 1);

                hv_CenterZ.Dispose();

                hv_CenterZ = hv_Center.TupleSelect(
                    HTuple.TupleGenSequence(0, (new HTuple(hv_Center.TupleLength())) - 1, 3) + 2);

                hv_PosObjectsZ.Dispose();
                hv_PosObjectsZ = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreater(7))) != 0)
                {
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_Pose.Dispose();

                        hv_Pose = hv_PosesOut.TupleSelectRange(
                            hv_I * 7, (hv_I * 7) + 6);

                        hv_HomMat3DObj.Dispose();
                        HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3DObj);

                        hv_PosObjCenterX.Dispose(); hv_PosObjCenterY.Dispose(); hv_PosObjCenterZ.Dispose();
                        HOperatorSet.AffineTransPoint3d(hv_HomMat3DObj, hv_CenterX.TupleSelect(
                            hv_I), hv_CenterY.TupleSelect(hv_I), hv_CenterZ.TupleSelect(hv_I),
                            out hv_PosObjCenterX, out hv_PosObjCenterY, out hv_PosObjCenterZ);


                        {
                            HTuple
                              ExpTmpLocalVar_PosObjectsZ = hv_PosObjectsZ.TupleConcat(
                                hv_PosObjCenterZ);
                            hv_PosObjectsZ.Dispose();
                            hv_PosObjectsZ = ExpTmpLocalVar_PosObjectsZ;
                        }

                    }
                }
                else
                {
                    hv_Pose.Dispose();

                    hv_Pose = hv_PosesOut.TupleSelectRange(
                        0, 6);

                    hv_HomMat3DObj.Dispose();
                    HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3DObj);
                    hv_PosObjectsX.Dispose(); hv_PosObjectsY.Dispose(); hv_PosObjectsZ.Dispose();
                    HOperatorSet.AffineTransPoint3d(hv_HomMat3DObj, hv_CenterX, hv_CenterY, hv_CenterZ,
                        out hv_PosObjectsX, out hv_PosObjectsY, out hv_PosObjectsZ);
                }
                hv_Idx.Dispose();

                hv_Idx = (new HTuple(hv_PosObjectsZ.TupleSortIndex()
                    )).TupleInverse();

                hv_Color.Dispose();
                hv_Color = "white";
                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color);
                if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_Indices1.Dispose();
                    HOperatorSet.TupleFind(hv_GenParamName, "colored", out hv_Indices1);
                    hv_Indices2.Dispose();
                    HOperatorSet.TupleFind(hv_GenParamName, "color", out hv_Indices2);
                    if ((int)(new HTuple(((hv_Indices1.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                    {
                        if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                            0)))).TupleEqual(3))) != 0)
                        {
                            hv_Color.Dispose();
                            hv_Color = new HTuple();
                            hv_Color[0] = "red";
                            hv_Color[1] = "green";
                            hv_Color[2] = "blue";
                        }
                        else if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                            0)))).TupleEqual(6))) != 0)
                        {
                            hv_Color.Dispose();
                            hv_Color = new HTuple();
                            hv_Color[0] = "red";
                            hv_Color[1] = "green";
                            hv_Color[2] = "blue";
                            hv_Color[3] = "cyan";
                            hv_Color[4] = "magenta";
                            hv_Color[5] = "yellow";
                        }
                        else if ((int)(new HTuple(((hv_GenParamValue.TupleSelect(hv_Indices1.TupleSelect(
                            0)))).TupleEqual(12))) != 0)
                        {
                            hv_Color.Dispose();
                            hv_Color = new HTuple();
                            hv_Color[0] = "red";
                            hv_Color[1] = "green";
                            hv_Color[2] = "blue";
                            hv_Color[3] = "cyan";
                            hv_Color[4] = "magenta";
                            hv_Color[5] = "yellow";
                            hv_Color[6] = "coral";
                            hv_Color[7] = "slate blue";
                            hv_Color[8] = "spring green";
                            hv_Color[9] = "orange red";
                            hv_Color[10] = "pink";
                            hv_Color[11] = "gold";
                        }
                    }
                    else if ((int)(new HTuple(((hv_Indices2.TupleSelect(0))).TupleNotEqual(
                        -1))) != 0)
                    {
                        hv_Color.Dispose();

                        hv_Color = hv_GenParamValue.TupleSelect(
                            hv_Indices2.TupleSelect(0));

                    }
                }
                for (hv_J = 0; (int)hv_J <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength())) - 1); hv_J = (int)hv_J + 1)
                {
                    hv_I.Dispose();

                    hv_I = hv_Idx.TupleSelect(
                        hv_J);

                    if ((int)((new HTuple((new HTuple((new HTuple(((hv_HasPolygons.TupleSelect(
                        hv_I))).TupleEqual("true"))).TupleOr(new HTuple(((hv_HasTri.TupleSelect(
                        hv_I))).TupleEqual("true"))))).TupleOr(new HTuple(((hv_HasPoints.TupleSelect(
                        hv_I))).TupleEqual("true"))))).TupleOr(new HTuple(((hv_HasLines.TupleSelect(
                        hv_I))).TupleEqual("true")))) != 0)
                    {
                        if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                            0))) != 0)
                        {

                            hv_Indices3.Dispose();
                            HOperatorSet.TupleFind(hv_GenParamName, "color_" + hv_I, out hv_Indices3);

                            if ((int)(new HTuple(((hv_Indices3.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                            {

                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_GenParamValue.TupleSelect(
                                    hv_Indices3.TupleSelect(0)));

                            }
                            else
                            {

                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color.TupleSelect(hv_I % (new HTuple(hv_Color.TupleLength()
                                    ))));

                            }
                        }
                        if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreaterEqual(
                            (hv_I * 7) + 6))) != 0)
                        {
                            hv_Pose.Dispose();

                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                hv_I * 7, (hv_I * 7) + 6);

                        }
                        else
                        {
                            hv_Pose.Dispose();

                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                0, 6);

                        }
                        if ((int)(new HTuple(((hv_NumPoints.TupleSelect(hv_I))).TupleLess(10000))) != 0)
                        {

                            ho_ModelContours.Dispose();
                            HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_ObjectModel3DID.TupleSelect(
                                hv_I), hv_CamParam, hv_Pose, hv_CustomParamName, hv_CustomParamValue);

                            HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                        }
                        else
                        {
                            hv_HomMat3D.Dispose();
                            HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);

                            hv_SampledObjectModel3D.Dispose();
                            HOperatorSet.SampleObjectModel3d(hv_ObjectModel3DID.TupleSelect(hv_I),
                                "fast", 0.01 * (hv_Diameter.TupleSelect(hv_I)), new HTuple(), new HTuple(),
                                out hv_SampledObjectModel3D);

                            ho_ModelContours.Dispose();
                            HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_SampledObjectModel3D,
                                hv_CamParam, hv_Pose, "point_size", 1);
                            hv_X.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_x",
                                out hv_X);
                            hv_Y.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_y",
                                out hv_Y);
                            hv_Z.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_SampledObjectModel3D, "point_coord_z",
                                out hv_Z);
                            hv_HomMat3D1.Dispose();
                            HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D1);
                            hv_Qx.Dispose(); hv_Qy.Dispose(); hv_Qz.Dispose();
                            HOperatorSet.AffineTransPoint3d(hv_HomMat3D1, hv_X, hv_Y, hv_Z, out hv_Qx,
                                out hv_Qy, out hv_Qz);
                            hv_Row.Dispose(); hv_Column.Dispose();
                            HOperatorSet.Project3dPoint(hv_Qx, hv_Qy, hv_Qz, hv_CamParam, out hv_Row,
                                out hv_Column);
                            HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                            HOperatorSet.ClearObjectModel3d(hv_SampledObjectModel3D);
                        }
                    }
                    else
                    {
                        if ((int)(new HTuple((new HTuple(hv_GenParamName.TupleLength())).TupleGreater(
                            0))) != 0)
                        {

                            hv_Indices3.Dispose();
                            HOperatorSet.TupleFind(hv_GenParamName, "color_" + hv_I, out hv_Indices3);

                            if ((int)(new HTuple(((hv_Indices3.TupleSelect(0))).TupleNotEqual(-1))) != 0)
                            {

                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_GenParamValue.TupleSelect(
                                    hv_Indices3.TupleSelect(0)));

                            }
                            else
                            {

                                HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color.TupleSelect(hv_I % (new HTuple(hv_Color.TupleLength()
                                    ))));

                            }
                        }
                        if ((int)(new HTuple((new HTuple(hv_PosesOut.TupleLength())).TupleGreaterEqual(
                            (hv_I * 7) + 6))) != 0)
                        {
                            hv_Pose.Dispose();

                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                hv_I * 7, (hv_I * 7) + 6);

                        }
                        else
                        {
                            hv_Pose.Dispose();

                            hv_Pose = hv_PosesOut.TupleSelectRange(
                                0, 6);

                        }
                        if ((int)(new HTuple(((hv_IsPrimitive.TupleSelect(hv_I))).TupleEqual("true"))) != 0)
                        {
                            try
                            {

                                hv_ObjectModel3DConvexHull.Dispose();
                                HOperatorSet.ConvexHullObjectModel3d(hv_ObjectModel3DID.TupleSelect(
                                    hv_I), out hv_ObjectModel3DConvexHull);

                                if ((int)(new HTuple(((hv_NumPoints.TupleSelect(hv_I))).TupleLess(10000))) != 0)
                                {
                                    ho_ModelContours.Dispose();
                                    HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_ObjectModel3DConvexHull,
                                        hv_CamParam, hv_Pose, hv_CustomParamName, hv_CustomParamValue);
                                    HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                                }
                                else
                                {
                                    hv_HomMat3D.Dispose();
                                    HOperatorSet.PoseToHomMat3d(hv_Pose, out hv_HomMat3D);

                                    hv_SampledObjectModel3D.Dispose();
                                    HOperatorSet.SampleObjectModel3d(hv_ObjectModel3DConvexHull, "fast",
                                        0.01 * (hv_Diameter.TupleSelect(hv_I)), new HTuple(), new HTuple(),
                                        out hv_SampledObjectModel3D);

                                    ho_ModelContours.Dispose();
                                    HOperatorSet.ProjectObjectModel3d(out ho_ModelContours, hv_SampledObjectModel3D,
                                        hv_CamParam, hv_Pose, "point_size", 1);
                                    HOperatorSet.DispObj(ho_ModelContours, hv_WindowHandleBuffer);
                                    HOperatorSet.ClearObjectModel3d(hv_SampledObjectModel3D);
                                }
                                HOperatorSet.ClearObjectModel3d(hv_ObjectModel3DConvexHull);
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            }
                        }
                    }
                }
                HOperatorSet.SetSystem("opengl_hidden_surface_removal_enable", hv_OpenGlHiddenSurface);

                hv_Idx.Dispose();
                hv_CustomParamName.Dispose();
                hv_CustomParamValue.Dispose();
                hv_Font.Dispose();
                hv_IndicesDispBackGround.Dispose();
                hv_Indices.Dispose();
                hv_ImageWidth.Dispose();
                hv_HasPolygons.Dispose();
                hv_HasTri.Dispose();
                hv_HasPoints.Dispose();
                hv_HasLines.Dispose();
                hv_NumPoints.Dispose();
                hv_IsPrimitive.Dispose();
                hv_Center.Dispose();
                hv_Diameter.Dispose();
                hv_OpenGlHiddenSurface.Dispose();
                hv_CenterX.Dispose();
                hv_CenterY.Dispose();
                hv_CenterZ.Dispose();
                hv_PosObjectsZ.Dispose();
                hv_I.Dispose();
                hv_Pose.Dispose();
                hv_HomMat3DObj.Dispose();
                hv_PosObjCenterX.Dispose();
                hv_PosObjCenterY.Dispose();
                hv_PosObjCenterZ.Dispose();
                hv_PosObjectsX.Dispose();
                hv_PosObjectsY.Dispose();
                hv_Color.Dispose();
                hv_Indices1.Dispose();
                hv_Indices2.Dispose();
                hv_J.Dispose();
                hv_Indices3.Dispose();
                hv_HomMat3D.Dispose();
                hv_SampledObjectModel3D.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_HomMat3D1.Dispose();
                hv_Qx.Dispose();
                hv_Qy.Dispose();
                hv_Qz.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_ObjectModel3DConvexHull.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Idx.Dispose();
                hv_CustomParamName.Dispose();
                hv_CustomParamValue.Dispose();
                hv_Font.Dispose();
                hv_IndicesDispBackGround.Dispose();
                hv_Indices.Dispose();
                hv_ImageWidth.Dispose();
                hv_HasPolygons.Dispose();
                hv_HasTri.Dispose();
                hv_HasPoints.Dispose();
                hv_HasLines.Dispose();
                hv_NumPoints.Dispose();
                hv_IsPrimitive.Dispose();
                hv_Center.Dispose();
                hv_Diameter.Dispose();
                hv_OpenGlHiddenSurface.Dispose();
                hv_CenterX.Dispose();
                hv_CenterY.Dispose();
                hv_CenterZ.Dispose();
                hv_PosObjectsZ.Dispose();
                hv_I.Dispose();
                hv_Pose.Dispose();
                hv_HomMat3DObj.Dispose();
                hv_PosObjCenterX.Dispose();
                hv_PosObjCenterY.Dispose();
                hv_PosObjCenterZ.Dispose();
                hv_PosObjectsX.Dispose();
                hv_PosObjectsY.Dispose();
                hv_Color.Dispose();
                hv_Indices1.Dispose();
                hv_Indices2.Dispose();
                hv_J.Dispose();
                hv_Indices3.Dispose();
                hv_HomMat3D.Dispose();
                hv_SampledObjectModel3D.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_HomMat3D1.Dispose();
                hv_Qx.Dispose();
                hv_Qy.Dispose();
                hv_Qz.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_ObjectModel3DConvexHull.Dispose();
                hv_Exception.Dispose();

                //throw HDevExpDefaultException;
            }
        }

        private static void Set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
    HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behavior

                    {
                        HTuple
                          ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                            ;
                        hv_Size_COPY_INP_TMP.Dispose();
                        hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                    }

                }
                else
                {

                    {
                        HTuple
                          ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                            ;
                        hv_Size_COPY_INP_TMP.Dispose();
                        hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                    }

                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_Style = hv_Style + "Bold";
                        hv_Style.Dispose();
                        hv_Style = ExpTmpLocalVar_Style;
                    }

                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_Style = hv_Style + "Italic";
                        hv_Style.Dispose();
                        hv_Style = ExpTmpLocalVar_Style;
                    }

                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();

                    hv_Indices = hv_AvailableFonts.TupleFind(
                        hv_Fonts.TupleSelect(hv_Fdx));

                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();

                            hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                hv_Fdx);

                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }

                {
                    HTuple
                      ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                    hv_Font_COPY_INP_TMP.Dispose();
                    hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                }

                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Graphics / Output
        // Short Description: Get the center of the virtual trackback that is used to move the camera (version for inspection_mode = 'surface'). 
        private static void Get_trackball_center_fixed(HTuple hv_SelectedObject, HTuple hv_TrackballCenterRow,
            HTuple hv_TrackballCenterCol, HTuple hv_TrackballRadiusPixel, HTuple hv_Scene3D,
            HTuple hv_ObjectModel3DID, HTuple hv_Poses, HTuple hv_WindowHandleBuffer, HTuple hv_CamParam,
            HTuple hv_GenParamName, HTuple hv_GenParamValue, out HTuple hv_TBCenter, out HTuple hv_TBSize)
        {



            // Local iconic variables 

            HObject ho_RegionCenter, ho_DistanceImage;
            HObject ho_Domain;

            // Local control variables 

            HTuple hv_NumModels = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_SelectPose = new HTuple();
            HTuple hv_Index1 = new HTuple(), hv_Rows = new HTuple();
            HTuple hv_Columns = new HTuple(), hv_Grayval = new HTuple();
            HTuple hv_IndicesG = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Pos = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionCenter);
            HOperatorSet.GenEmptyObj(out ho_DistanceImage);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            hv_TBCenter = new HTuple();
            hv_TBSize = new HTuple();
            try
            {
                //
                //Determine the trackball center for the fixed trackball
                hv_NumModels.Dispose();

                hv_NumModels = new HTuple(hv_ObjectModel3DID.TupleLength()
                    );

                hv_Width.Dispose();
                Get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
                hv_Height.Dispose();
                Get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
                //
                //Project the selected objects
                hv_SelectPose.Dispose();
                hv_SelectPose = new HTuple();
                for (hv_Index1 = 0; (int)hv_Index1 <= (int)((new HTuple(hv_SelectedObject.TupleLength()
                    )) - 1); hv_Index1 = (int)hv_Index1 + 1)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_SelectPose = hv_SelectPose.TupleConcat(
                            HTuple.TupleGenConst(7, hv_SelectedObject.TupleSelect(hv_Index1)));
                        hv_SelectPose.Dispose();
                        hv_SelectPose = ExpTmpLocalVar_SelectPose;
                    }

                    if ((int)(new HTuple(((hv_SelectedObject.TupleSelect(hv_Index1))).TupleEqual(
                        0))) != 0)
                    {
                        HOperatorSet.SetScene3dInstanceParam(hv_Scene3D, hv_Index1, "visible",
                            "false");
                    }
                }
                HOperatorSet.SetScene3dParam(hv_Scene3D, "depth_persistence", "true");
                HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                HOperatorSet.SetScene3dParam(hv_Scene3D, "visible", "true");
                //
                //determine the depth of the object point that appears closest to the trackball
                //center
                ho_RegionCenter.Dispose();
                HOperatorSet.GenRegionPoints(out ho_RegionCenter, hv_TrackballCenterRow, hv_TrackballCenterCol);
                ho_DistanceImage.Dispose();
                HOperatorSet.DistanceTransform(ho_RegionCenter, out ho_DistanceImage, "chamfer-3-4-unnormalized",
                    "false", hv_Width, hv_Height);
                ho_Domain.Dispose();
                HOperatorSet.GetDomain(ho_DistanceImage, out ho_Domain);
                hv_Rows.Dispose(); hv_Columns.Dispose();
                HOperatorSet.GetRegionPoints(ho_Domain, out hv_Rows, out hv_Columns);
                hv_Grayval.Dispose();
                HOperatorSet.GetGrayval(ho_DistanceImage, hv_Rows, hv_Columns, out hv_Grayval);
                hv_IndicesG.Dispose();
                HOperatorSet.TupleSortIndex(hv_Grayval, out hv_IndicesG);

                hv_Value.Dispose();
                HOperatorSet.GetDisplayScene3dInfo(hv_WindowHandleBuffer, hv_Scene3D, hv_Rows.TupleSelect(
                    hv_IndicesG), hv_Columns.TupleSelect(hv_IndicesG), "depth", out hv_Value);


                hv_Pos.Dispose();
                HOperatorSet.TupleFind(hv_Value.TupleSgn(), 1, out hv_Pos);


                HOperatorSet.SetScene3dParam(hv_Scene3D, "depth_persistence", "false");
                //
                //
                //set TBCenter
                if ((int)(new HTuple(hv_Pos.TupleNotEqual(-1))) != 0)
                {
                    //if the object is visible in the image
                    hv_TBCenter.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TBCenter = new HTuple();
                        hv_TBCenter[0] = 0;
                        hv_TBCenter[1] = 0;
                        hv_TBCenter = hv_TBCenter.TupleConcat(hv_Value.TupleSelect(
                            hv_Pos.TupleSelect(0)));
                    }
                }
                else
                {
                    //if the object is not visible in the image, set the z coordinate to -1
                    //to indicate, that the previous z value should be used instead
                    hv_TBCenter.Dispose();
                    hv_TBCenter = new HTuple();
                    hv_TBCenter[0] = 0;
                    hv_TBCenter[1] = 0;
                    hv_TBCenter[2] = -1;
                }
                //
                if ((int)(new HTuple(((hv_SelectedObject.TupleMax())).TupleNotEqual(0))) != 0)
                {
                    hv_TBSize.Dispose();

                    hv_TBSize = (0.5 + ((0.5 * (hv_SelectedObject.TupleSum()
                        )) / hv_NumModels)) * hv_TrackballRadiusPixel;

                }
                else
                {
                    hv_TBCenter.Dispose();
                    hv_TBCenter = new HTuple();
                    hv_TBSize.Dispose();
                    hv_TBSize = 0;
                }
                ho_RegionCenter.Dispose();
                ho_DistanceImage.Dispose();
                ho_Domain.Dispose();

                hv_NumModels.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_SelectPose.Dispose();
                hv_Index1.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Grayval.Dispose();
                hv_IndicesG.Dispose();
                hv_Value.Dispose();
                hv_Pos.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_RegionCenter.Dispose();
                ho_DistanceImage.Dispose();
                ho_Domain.Dispose();

                hv_NumModels.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_SelectPose.Dispose();
                hv_Index1.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Grayval.Dispose();
                hv_IndicesG.Dispose();
                hv_Value.Dispose();
                hv_Pos.Dispose();

                //throw HDevExpDefaultException;
            }
        }




        // Chapter: Graphics / Output
        // Short Description: Compute the center of all given 3D object models. 
        private static void Get_object_models_center(HTuple hv_ObjectModel3DID, out HTuple hv_Center)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Diameters = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Diameter = new HTuple(), hv_C = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_MD = new HTuple();
            HTuple hv_Weight = new HTuple(), hv_SumW = new HTuple();
            HTuple hv_ObjectModel3DIDSelected = new HTuple(), hv_InvSum = new HTuple();
            // Initialize local and output iconic variables 
            hv_Center = new HTuple();
            try
            {
                //Compute the mean of all model centers (weighted by the diameter of the object models)
                hv_Diameters.Dispose();

                hv_Diameters = HTuple.TupleGenConst(
                    new HTuple(hv_ObjectModel3DID.TupleLength()), 0.0);

                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    try
                    {

                        hv_Diameter.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "diameter_axis_aligned_bounding_box", out hv_Diameter);


                        hv_C.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "center", out hv_C);

                        if (hv_Diameters == null)
                            hv_Diameters = new HTuple();
                        hv_Diameters[hv_Index] = hv_Diameter;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        //Object model is empty, has no center etc. -> ignore it by leaving its diameter at zero
                    }
                }

                if ((int)(new HTuple(((hv_Diameters.TupleSum())).TupleGreater(0))) != 0)
                {
                    //Normalize Diameter to use it as weights for a weighted mean of the individual centers
                    hv_MD.Dispose();

                    hv_MD = ((hv_Diameters.TupleSelectMask(
                        hv_Diameters.TupleGreaterElem(0)))).TupleMean();

                    if ((int)(new HTuple(hv_MD.TupleGreater(1e-10))) != 0)
                    {
                        hv_Weight.Dispose();

                        hv_Weight = hv_Diameters / hv_MD;

                    }
                    else
                    {
                        hv_Weight.Dispose();
                        hv_Weight = new HTuple(hv_Diameters);
                    }
                    hv_SumW.Dispose();

                    hv_SumW = hv_Weight.TupleSum();

                    if ((int)(new HTuple(hv_SumW.TupleLess(1e-10))) != 0)
                    {

                        {
                            HTuple
                              ExpTmpLocalVar_Weight = HTuple.TupleGenConst(
                                new HTuple(hv_Weight.TupleLength()), 1.0);
                            hv_Weight.Dispose();
                            hv_Weight = ExpTmpLocalVar_Weight;
                        }

                        hv_SumW.Dispose();

                        hv_SumW = hv_Weight.TupleSum();

                    }
                    hv_Center.Dispose();
                    hv_Center = new HTuple();
                    hv_Center[0] = 0;
                    hv_Center[1] = 0;
                    hv_Center[2] = 0;
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if ((int)(new HTuple(((hv_Diameters.TupleSelect(hv_Index))).TupleGreater(
                            0))) != 0)
                        {
                            hv_ObjectModel3DIDSelected.Dispose();

                            hv_ObjectModel3DIDSelected = hv_ObjectModel3DID.TupleSelect(
                                hv_Index);

                            hv_C.Dispose();
                            HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DIDSelected, "center",
                                out hv_C);
                            if (hv_Center == null)
                                hv_Center = new HTuple();
                            hv_Center[0] = (hv_Center.TupleSelect(0)) + ((hv_C.TupleSelect(0)) * (hv_Weight.TupleSelect(
                                hv_Index)));
                            if (hv_Center == null)
                                hv_Center = new HTuple();
                            hv_Center[1] = (hv_Center.TupleSelect(1)) + ((hv_C.TupleSelect(1)) * (hv_Weight.TupleSelect(
                                hv_Index)));
                            if (hv_Center == null)
                                hv_Center = new HTuple();
                            hv_Center[2] = (hv_Center.TupleSelect(2)) + ((hv_C.TupleSelect(2)) * (hv_Weight.TupleSelect(
                                hv_Index)));
                        }
                    }
                    hv_InvSum.Dispose();

                    hv_InvSum = 1.0 / hv_SumW;

                    if (hv_Center == null)
                        hv_Center = new HTuple();
                    hv_Center[0] = (hv_Center.TupleSelect(0)) * hv_InvSum;
                    if (hv_Center == null)
                        hv_Center = new HTuple();
                    hv_Center[1] = (hv_Center.TupleSelect(1)) * hv_InvSum;
                    if (hv_Center == null)
                        hv_Center = new HTuple();
                    hv_Center[2] = (hv_Center.TupleSelect(2)) * hv_InvSum;
                }
                else
                {
                    hv_Center.Dispose();
                    hv_Center = new HTuple();
                }

                hv_Diameters.Dispose();
                hv_Index.Dispose();
                hv_Diameter.Dispose();
                hv_C.Dispose();
                hv_Exception.Dispose();
                hv_MD.Dispose();
                hv_Weight.Dispose();
                hv_SumW.Dispose();
                hv_ObjectModel3DIDSelected.Dispose();
                hv_InvSum.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Diameters.Dispose();
                hv_Index.Dispose();
                hv_Diameter.Dispose();
                hv_C.Dispose();
                hv_Exception.Dispose();
                hv_MD.Dispose();
                hv_Weight.Dispose();
                hv_SumW.Dispose();
                hv_ObjectModel3DIDSelected.Dispose();
                hv_InvSum.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Chapter: Graphics / Output
        // Short Description: Determine the optimum distance of the object to obtain a reasonable visualization 
        private static void Determine_optimum_pose_distance(HTuple hv_ObjectModel3DID, HTuple hv_CamParam,
            HTuple hv_ImageCoverage, HTuple hv_PoseIn, out HTuple hv_PoseOut)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Rows = new HTuple(), hv_Cols = new HTuple();
            HTuple hv_MinMinZ = new HTuple(), hv_BB = new HTuple();
            HTuple hv_Index = new HTuple(), hv_CurrBB = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Seq = new HTuple();
            HTuple hv_DXMax = new HTuple(), hv_DYMax = new HTuple();
            HTuple hv_DZMax = new HTuple(), hv_Diameter = new HTuple();
            HTuple hv_ZAdd = new HTuple(), hv_BBX0 = new HTuple();
            HTuple hv_BBX1 = new HTuple(), hv_BBY0 = new HTuple();
            HTuple hv_BBY1 = new HTuple(), hv_BBZ0 = new HTuple();
            HTuple hv_BBZ1 = new HTuple(), hv_X = new HTuple(), hv_Y = new HTuple();
            HTuple hv_Z = new HTuple(), hv_HomMat3DIn = new HTuple();
            HTuple hv_QX_In = new HTuple(), hv_QY_In = new HTuple();
            HTuple hv_QZ_In = new HTuple(), hv_PoseInter = new HTuple();
            HTuple hv_HomMat3D = new HTuple(), hv_QX = new HTuple();
            HTuple hv_QY = new HTuple(), hv_QZ = new HTuple(), hv_Cx = new HTuple();
            HTuple hv_Cy = new HTuple(), hv_DR = new HTuple(), hv_DC = new HTuple();
            HTuple hv_MaxDist = new HTuple(), hv_HomMat3DRotate = new HTuple();
            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_MinImageSize = new HTuple(), hv_Zs = new HTuple();
            HTuple hv_ZDiff = new HTuple(), hv_ScaleZ = new HTuple();
            HTuple hv_ZNew = new HTuple();
            // Initialize local and output iconic variables 
            hv_PoseOut = new HTuple();
            try
            {
                //Determine the optimum distance of the object to obtain
                //a reasonable visualization
                //
                hv_Rows.Dispose();
                hv_Rows = new HTuple();
                hv_Cols.Dispose();
                hv_Cols = new HTuple();
                hv_MinMinZ.Dispose();
                hv_MinMinZ = 1e30;
                hv_BB.Dispose();
                hv_BB = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ObjectModel3DID.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    try
                    {

                        hv_CurrBB.Dispose();
                        HOperatorSet.GetObjectModel3dParams(hv_ObjectModel3DID.TupleSelect(hv_Index),
                            "bounding_box1", out hv_CurrBB);


                        {
                            HTuple
                              ExpTmpLocalVar_BB = hv_BB.TupleConcat(
                                hv_CurrBB);
                            hv_BB.Dispose();
                            hv_BB = ExpTmpLocalVar_BB;
                        }

                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        //3D object model is empty / has no bounding box -> ignore it
                    }
                }
                if ((int)(new HTuple(((((((hv_BB.TupleAbs())).TupleConcat(0))).TupleSum())).TupleEqual(
                    0.0))) != 0)
                {
                    hv_BB.Dispose();

                    hv_BB = new HTuple();
                    hv_BB = hv_BB.TupleConcat(-((new HTuple(HTuple.TupleRand(
                        3) * 1e-20)).TupleAbs()));
                    hv_BB = hv_BB.TupleConcat((new HTuple(HTuple.TupleRand(
                        3) * 1e-20)).TupleAbs());

                }
                //Calculate diameter over all objects to be visualized
                hv_Seq.Dispose();

                hv_Seq = HTuple.TupleGenSequence(
                    0, (new HTuple(hv_BB.TupleLength())) - 1, 6);

                hv_DXMax.Dispose();

                hv_DXMax = (((hv_BB.TupleSelect(
                    hv_Seq + 3))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq))).TupleMin());

                hv_DYMax.Dispose();

                hv_DYMax = (((hv_BB.TupleSelect(
                    hv_Seq + 4))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq + 1))).TupleMin());

                hv_DZMax.Dispose();

                hv_DZMax = (((hv_BB.TupleSelect(
                    hv_Seq + 5))).TupleMax()) - (((hv_BB.TupleSelect(hv_Seq + 2))).TupleMin());

                hv_Diameter.Dispose();

                hv_Diameter = ((((hv_DXMax * hv_DXMax) + (hv_DYMax * hv_DYMax)) + (hv_DZMax * hv_DZMax))).TupleSqrt()
                    ;

                //Allow the visualization of single points or extremely small objects
                hv_ZAdd.Dispose();
                hv_ZAdd = 0.0;
                if ((int)(new HTuple(((hv_Diameter.TupleMax())).TupleLess(1e-10))) != 0)
                {
                    hv_ZAdd.Dispose();
                    hv_ZAdd = 0.01;
                }
                //Set extremely small diameters to 1e-10 to avoid CZ == 0.0, which would lead
                //to projection errors
                if ((int)(new HTuple(((hv_Diameter.TupleMin())).TupleLess(1e-10))) != 0)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_Diameter = hv_Diameter - (((((((hv_Diameter - 1e-10)).TupleSgn()
                            ) - 1)).TupleSgn()) * 1e-10);
                        hv_Diameter.Dispose();
                        hv_Diameter = ExpTmpLocalVar_Diameter;
                    }

                }
                //Move all points in front of the camera
                hv_BBX0.Dispose();

                hv_BBX0 = hv_BB.TupleSelect(
                    hv_Seq + 0);

                hv_BBX1.Dispose();

                hv_BBX1 = hv_BB.TupleSelect(
                    hv_Seq + 3);

                hv_BBY0.Dispose();

                hv_BBY0 = hv_BB.TupleSelect(
                    hv_Seq + 1);

                hv_BBY1.Dispose();

                hv_BBY1 = hv_BB.TupleSelect(
                    hv_Seq + 4);

                hv_BBZ0.Dispose();

                hv_BBZ0 = hv_BB.TupleSelect(
                    hv_Seq + 2);

                hv_BBZ1.Dispose();

                hv_BBZ1 = hv_BB.TupleSelect(
                    hv_Seq + 5);

                hv_X.Dispose();

                hv_X = new HTuple();
                hv_X = hv_X.TupleConcat(hv_BBX0, hv_BBX0, hv_BBX0, hv_BBX0, hv_BBX1, hv_BBX1, hv_BBX1, hv_BBX1);

                hv_Y.Dispose();

                hv_Y = new HTuple();
                hv_Y = hv_Y.TupleConcat(hv_BBY0, hv_BBY0, hv_BBY1, hv_BBY1, hv_BBY0, hv_BBY0, hv_BBY1, hv_BBY1);

                hv_Z.Dispose();

                hv_Z = new HTuple();
                hv_Z = hv_Z.TupleConcat(hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1, hv_BBZ0, hv_BBZ1);

                hv_HomMat3DIn.Dispose();
                HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                hv_QX_In.Dispose(); hv_QY_In.Dispose(); hv_QZ_In.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_HomMat3DIn, hv_X, hv_Y, hv_Z, out hv_QX_In,
                    out hv_QY_In, out hv_QZ_In);

                hv_PoseInter.Dispose();
                HOperatorSet.PoseCompose(((((new HTuple(0)).TupleConcat(0)).TupleConcat((-(hv_QZ_In.TupleMin()
                    )) + (2 * (hv_Diameter.TupleMax()))))).TupleConcat((((new HTuple(0)).TupleConcat(
                    0)).TupleConcat(0)).TupleConcat(0)), hv_PoseIn, out hv_PoseInter);

                hv_HomMat3D.Dispose();
                HOperatorSet.PoseToHomMat3d(hv_PoseInter, out hv_HomMat3D);
                //Determine the maximum extension of the projection
                hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
                HOperatorSet.AffineTransPoint3d(hv_HomMat3D, hv_X, hv_Y, hv_Z, out hv_QX, out hv_QY,
                    out hv_QZ);
                hv_Rows.Dispose(); hv_Cols.Dispose();
                HOperatorSet.Project3dPoint(hv_QX, hv_QY, hv_QZ, hv_CamParam, out hv_Rows,
                    out hv_Cols);
                hv_MinMinZ.Dispose();

                hv_MinMinZ = hv_QZ.TupleMin();

                hv_Cx.Dispose();
                Get_cam_par_data(hv_CamParam, "cx", out hv_Cx);
                hv_Cy.Dispose();
                Get_cam_par_data(hv_CamParam, "cy", out hv_Cy);
                hv_DR.Dispose();

                hv_DR = hv_Rows - hv_Cy;

                hv_DC.Dispose();

                hv_DC = hv_Cols - hv_Cx;


                {
                    HTuple
                      ExpTmpLocalVar_DR = (hv_DR.TupleMax()
                        ) - (hv_DR.TupleMin());
                    hv_DR.Dispose();
                    hv_DR = ExpTmpLocalVar_DR;
                }


                {
                    HTuple
                      ExpTmpLocalVar_DC = (hv_DC.TupleMax()
                        ) - (hv_DC.TupleMin());
                    hv_DC.Dispose();
                    hv_DC = ExpTmpLocalVar_DC;
                }

                hv_MaxDist.Dispose();

                hv_MaxDist = (((hv_DR * hv_DR) + (hv_DC * hv_DC))).TupleSqrt()
                    ;

                //
                if ((int)(new HTuple(hv_MaxDist.TupleLess(1e-10))) != 0)
                {
                    //If the object has no extension in the above projection (looking along
                    //a line), we determine the extension of the object in a rotated view

                    hv_HomMat3DRotate.Dispose();
                    HOperatorSet.HomMat3dRotateLocal(hv_HomMat3D, (new HTuple(90)).TupleRad()
                        , "x", out hv_HomMat3DRotate);

                    hv_QX.Dispose(); hv_QY.Dispose(); hv_QZ.Dispose();
                    HOperatorSet.AffineTransPoint3d(hv_HomMat3DRotate, hv_X, hv_Y, hv_Z, out hv_QX,
                        out hv_QY, out hv_QZ);
                    hv_Rows.Dispose(); hv_Cols.Dispose();
                    HOperatorSet.Project3dPoint(hv_QX, hv_QY, hv_QZ, hv_CamParam, out hv_Rows,
                        out hv_Cols);
                    hv_DR.Dispose();

                    hv_DR = hv_Rows - hv_Cy;

                    hv_DC.Dispose();

                    hv_DC = hv_Cols - hv_Cx;

                    {
                        HTuple
                          ExpTmpLocalVar_DR = (hv_DR.TupleMax()
                            ) - (hv_DR.TupleMin());
                        hv_DR.Dispose();
                        hv_DR = ExpTmpLocalVar_DR;
                    }


                    {
                        HTuple
                          ExpTmpLocalVar_DC = (hv_DC.TupleMax()
                            ) - (hv_DC.TupleMin());
                        hv_DC.Dispose();
                        hv_DC = ExpTmpLocalVar_DC;
                    }


                    {
                        HTuple
                          ExpTmpLocalVar_MaxDist = ((hv_MaxDist.TupleConcat(
                            (((hv_DR * hv_DR) + (hv_DC * hv_DC))).TupleSqrt()))).TupleMax();
                        hv_MaxDist.Dispose();
                        hv_MaxDist = ExpTmpLocalVar_MaxDist;
                    }

                }
                //
                hv_ImageWidth.Dispose();
                Get_cam_par_data(hv_CamParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                Get_cam_par_data(hv_CamParam, "image_height", out hv_ImageHeight);
                hv_MinImageSize.Dispose();

                hv_MinImageSize = ((hv_ImageWidth.TupleConcat(
                    hv_ImageHeight))).TupleMin();

                //
                hv_Z.Dispose();

                hv_Z = hv_PoseInter.TupleSelect(
                    2);

                hv_Zs.Dispose();
                hv_Zs = new HTuple(hv_MinMinZ);
                hv_ZDiff.Dispose();

                hv_ZDiff = hv_Z - hv_Zs;

                hv_ScaleZ.Dispose();

                hv_ScaleZ = hv_MaxDist / (((0.5 * hv_MinImageSize) * hv_ImageCoverage) * 2.0);

                hv_ZNew.Dispose();

                hv_ZNew = ((hv_ScaleZ * hv_Zs) + hv_ZDiff) + hv_ZAdd;

                hv_PoseOut.Dispose();

                hv_PoseOut = hv_PoseInter.TupleReplace(
                    2, hv_ZNew);

                //

                hv_Rows.Dispose();
                hv_Cols.Dispose();
                hv_MinMinZ.Dispose();
                hv_BB.Dispose();
                hv_Index.Dispose();
                hv_CurrBB.Dispose();
                hv_Exception.Dispose();
                hv_Seq.Dispose();
                hv_DXMax.Dispose();
                hv_DYMax.Dispose();
                hv_DZMax.Dispose();
                hv_Diameter.Dispose();
                hv_ZAdd.Dispose();
                hv_BBX0.Dispose();
                hv_BBX1.Dispose();
                hv_BBY0.Dispose();
                hv_BBY1.Dispose();
                hv_BBZ0.Dispose();
                hv_BBZ1.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_HomMat3DIn.Dispose();
                hv_QX_In.Dispose();
                hv_QY_In.Dispose();
                hv_QZ_In.Dispose();
                hv_PoseInter.Dispose();
                hv_HomMat3D.Dispose();
                hv_QX.Dispose();
                hv_QY.Dispose();
                hv_QZ.Dispose();
                hv_Cx.Dispose();
                hv_Cy.Dispose();
                hv_DR.Dispose();
                hv_DC.Dispose();
                hv_MaxDist.Dispose();
                hv_HomMat3DRotate.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_MinImageSize.Dispose();
                hv_Zs.Dispose();
                hv_ZDiff.Dispose();
                hv_ScaleZ.Dispose();
                hv_ZNew.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Rows.Dispose();
                hv_Cols.Dispose();
                hv_MinMinZ.Dispose();
                hv_BB.Dispose();
                hv_Index.Dispose();
                hv_CurrBB.Dispose();
                hv_Exception.Dispose();
                hv_Seq.Dispose();
                hv_DXMax.Dispose();
                hv_DYMax.Dispose();
                hv_DZMax.Dispose();
                hv_Diameter.Dispose();
                hv_ZAdd.Dispose();
                hv_BBX0.Dispose();
                hv_BBX1.Dispose();
                hv_BBY0.Dispose();
                hv_BBY1.Dispose();
                hv_BBZ0.Dispose();
                hv_BBZ1.Dispose();
                hv_X.Dispose();
                hv_Y.Dispose();
                hv_Z.Dispose();
                hv_HomMat3DIn.Dispose();
                hv_QX_In.Dispose();
                hv_QY_In.Dispose();
                hv_QZ_In.Dispose();
                hv_PoseInter.Dispose();
                hv_HomMat3D.Dispose();
                hv_QX.Dispose();
                hv_QY.Dispose();
                hv_QZ.Dispose();
                hv_Cx.Dispose();
                hv_Cy.Dispose();
                hv_DR.Dispose();
                hv_DC.Dispose();
                hv_MaxDist.Dispose();
                hv_HomMat3DRotate.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_MinImageSize.Dispose();
                hv_Zs.Dispose();
                hv_ZDiff.Dispose();
                hv_ScaleZ.Dispose();
                hv_ZNew.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        private static void Disp_title_and_information(HTuple hv_WindowHandle, HTuple hv_Title,
    HTuple hv_Information)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WinRow = new HTuple(), hv_WinColumn = new HTuple();
            HTuple hv_WinWidth = new HTuple(), hv_WinHeight = new HTuple();
            HTuple hv_NumTitleLines = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_TextWidth = new HTuple();
            HTuple hv_NumInfoLines = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple();
            HTuple hv_Information_COPY_INP_TMP = new HTuple(hv_Information);
            HTuple hv_Title_COPY_INP_TMP = new HTuple(hv_Title);

            // Initialize local and output iconic variables 
            try
            {
                //
                //global tuple gInfoDecor
                //global tuple gInfoPos
                //global tuple gTitlePos
                //global tuple gTitleDecor
                //
                hv_WinRow.Dispose(); hv_WinColumn.Dispose(); hv_WinWidth.Dispose(); hv_WinHeight.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_WinRow, out hv_WinColumn,
                    out hv_WinWidth, out hv_WinHeight);

                {
                    HTuple
                      ExpTmpLocalVar_Title = ((("" + hv_Title_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_Title_COPY_INP_TMP.Dispose();
                    hv_Title_COPY_INP_TMP = ExpTmpLocalVar_Title;
                }

                hv_NumTitleLines.Dispose();

                hv_NumTitleLines = new HTuple(hv_Title_COPY_INP_TMP.TupleLength()
                    );

                if ((int)(new HTuple(hv_NumTitleLines.TupleGreater(0))) != 0)
                {
                    hv_Row.Dispose();
                    hv_Row = 12;
                    if ((int)(new HTuple(gTitlePos.TupleEqual("UpperLeft"))) != 0)
                    {
                        hv_Column.Dispose();
                        hv_Column = 12;
                    }
                    else if ((int)(new HTuple(gTitlePos.TupleEqual(
                        "UpperCenter"))) != 0)
                    {
                        hv_TextWidth.Dispose();
                        Max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP, out hv_TextWidth);
                        hv_Column.Dispose();

                        hv_Column = (hv_WinWidth / 2) - (hv_TextWidth / 2);

                    }
                    else if ((int)(new HTuple(gTitlePos.TupleEqual(
                        "UpperRight"))) != 0)
                    {
                        if ((int)(new HTuple(((gTitleDecor.TupleSelect(1))).TupleEqual(
                            "true"))) != 0)
                        {

                            hv_TextWidth.Dispose();
                            Max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP + "  ", out hv_TextWidth);

                        }
                        else
                        {
                            hv_TextWidth.Dispose();
                            Max_line_width(hv_WindowHandle, hv_Title_COPY_INP_TMP, out hv_TextWidth);
                        }
                        hv_Column.Dispose();

                        hv_Column = (hv_WinWidth - hv_TextWidth) - 10;

                    }
                    else
                    {
                        //Unknown position!
                        // stop(...); only in hdevelop
                    }

                    Disp_message(hv_WindowHandle, hv_Title_COPY_INP_TMP, "window", hv_Row, hv_Column,
                        gTitleDecor.TupleSelect(0), gTitleDecor.TupleSelect(
                        1));

                }

                {
                    HTuple
                      ExpTmpLocalVar_Information = ((("" + hv_Information_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_Information_COPY_INP_TMP.Dispose();
                    hv_Information_COPY_INP_TMP = ExpTmpLocalVar_Information;
                }

                hv_NumInfoLines.Dispose();

                hv_NumInfoLines = new HTuple(hv_Information_COPY_INP_TMP.TupleLength()
                    );

                if ((int)(new HTuple(hv_NumInfoLines.TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(gInfoPos.TupleEqual("UpperLeft"))) != 0)
                    {
                        hv_Row.Dispose();
                        hv_Row = 12;
                        hv_Column.Dispose();
                        hv_Column = 12;
                    }
                    else if ((int)(new HTuple(gInfoPos.TupleEqual(
                        "UpperRight"))) != 0)
                    {
                        if ((int)(new HTuple(((gInfoDecor.TupleSelect(1))).TupleEqual(
                            "true"))) != 0)
                        {

                            hv_TextWidth.Dispose();
                            Max_line_width(hv_WindowHandle, hv_Information_COPY_INP_TMP + "  ", out hv_TextWidth);

                        }
                        else
                        {
                            hv_TextWidth.Dispose();
                            Max_line_width(hv_WindowHandle, hv_Information_COPY_INP_TMP, out hv_TextWidth);
                        }
                        hv_Row.Dispose();
                        hv_Row = 12;
                        hv_Column.Dispose();

                        hv_Column = (hv_WinWidth - hv_TextWidth) - 12;

                    }
                    else if ((int)(new HTuple(gInfoPos.TupleEqual(
                        "LowerLeft"))) != 0)
                    {
                        hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                        HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Information_COPY_INP_TMP,
                            out hv_Ascent, out hv_Descent, out hv_Width, out hv_Height);
                        hv_Row.Dispose();

                        hv_Row = (hv_WinHeight - ((((new HTuple(0)).TupleMax2(
                            hv_NumInfoLines - 1)) * (hv_Ascent + hv_Descent)) + hv_Height)) - 12;

                        hv_Column.Dispose();
                        hv_Column = 12;
                    }
                    else
                    {
                        //Unknown position!
                        // stop(...); only in hdevelop
                    }

                    Disp_message(hv_WindowHandle, hv_Information_COPY_INP_TMP, "window", hv_Row,
                        hv_Column, gInfoDecor.TupleSelect(0), gInfoDecor.TupleSelect(
                        1));

                }
                //

                hv_Information_COPY_INP_TMP.Dispose();
                hv_Title_COPY_INP_TMP.Dispose();
                hv_WinRow.Dispose();
                hv_WinColumn.Dispose();
                hv_WinWidth.Dispose();
                hv_WinHeight.Dispose();
                hv_NumTitleLines.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_TextWidth.Dispose();
                hv_NumInfoLines.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Information_COPY_INP_TMP.Dispose();
                hv_Title_COPY_INP_TMP.Dispose();
                hv_WinRow.Dispose();
                hv_WinColumn.Dispose();
                hv_WinWidth.Dispose();
                hv_WinHeight.Dispose();
                hv_NumTitleLines.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_TextWidth.Dispose();
                hv_NumInfoLines.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Graphics / Output
        // Short Description: Get string extends of several lines. 
        private static void Max_line_width(HTuple hv_WindowHandle, HTuple hv_Lines, out HTuple hv_MaxWidth)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_LineWidth = new HTuple();
            HTuple hv_LineHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_MaxWidth = new HTuple();
            try
            {
                //
                hv_MaxWidth.Dispose();
                hv_MaxWidth = 0;
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Lines.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {

                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_LineWidth.Dispose(); hv_LineHeight.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Lines.TupleSelect(hv_Index),
                        out hv_Ascent, out hv_Descent, out hv_LineWidth, out hv_LineHeight);


                    {
                        HTuple
                          ExpTmpLocalVar_MaxWidth = ((hv_LineWidth.TupleConcat(
                            hv_MaxWidth))).TupleMax();
                        hv_MaxWidth.Dispose();
                        hv_MaxWidth = ExpTmpLocalVar_MaxWidth;
                    }

                }

                hv_Index.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_LineWidth.Dispose();
                hv_LineHeight.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_Index.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_LineWidth.Dispose();
                hv_LineHeight.Dispose();

                //throw HDevExpDefaultException;
            }
        }




        // Chapter: Graphics / Output
        // Short Description: Display a continue button. 
        private static void Disp_continue_button(HTuple hv_WindowHandle)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ContinueMessage = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_TextWidth = new HTuple(), hv_TextHeight = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays a 'Continue' text button
                //in the lower right corner of the screen.
                //It uses the procedure disp_message.
                //
                //Input parameters:
                //WindowHandle: The window, where the text shall be displayed
                //
                //Use the continue message set in the global variable gTerminationButtonLabel.
                //If this variable is not defined, set a standard text instead.
                //global tuple gTerminationButtonLabel
                try
                {
                    hv_ContinueMessage.Dispose();
                    hv_ContinueMessage = new HTuple(gTerminationButtonLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ContinueMessage.Dispose();
                    hv_ContinueMessage = "Continue";
                }
                //Display the continue button
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Width,
                    out hv_Height);

                hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_TextWidth.Dispose(); hv_TextHeight.Dispose();
                HOperatorSet.GetStringExtents(hv_WindowHandle, (" " + hv_ContinueMessage) + " ",
                    out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);


                Disp_text_button(hv_WindowHandle, hv_ContinueMessage, "window", (hv_Height - hv_TextHeight) - 22,
                    (hv_Width - hv_TextWidth) - 12, "black", "#f28f26");


                hv_ContinueMessage.Dispose();
                hv_Exception.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_ContinueMessage.Dispose();
                hv_Exception.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Graphics / Text
        // Short Description: Display a text message. 
        private static void Disp_text_button(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_TextColor, HTuple hv_ButtonColor)
        {



            // Local iconic variables 

            HObject ho_UpperLeft, ho_LowerRight, ho_Rectangle;

            // Local control variables 

            HTuple hv_Red = new HTuple(), hv_Green = new HTuple();
            HTuple hv_Blue = new HTuple(), hv_Row1Part = new HTuple();
            HTuple hv_Column1Part = new HTuple(), hv_Row2Part = new HTuple();
            HTuple hv_Column2Part = new HTuple(), hv_RowWin = new HTuple();
            HTuple hv_ColumnWin = new HTuple(), hv_WidthWin = new HTuple();
            HTuple hv_HeightWin = new HTuple(), hv_RGB = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Fac = new HTuple();
            HTuple hv_RGBL = new HTuple(), hv_RGBD = new HTuple();
            HTuple hv_ButtonColorBorderL = new HTuple(), hv_ButtonColorBorderD = new HTuple();
            HTuple hv_MaxAscent = new HTuple(), hv_MaxDescent = new HTuple();
            HTuple hv_MaxWidth = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple();
            HTuple hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_ClipRegion = new HTuple();
            HTuple hv_DrawMode = new HTuple(), hv_BorderWidth = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            HTuple hv_String_COPY_INP_TMP = new HTuple(hv_String);
            HTuple hv_TextColor_COPY_INP_TMP = new HTuple(hv_TextColor);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_UpperLeft);
            HOperatorSet.GenEmptyObj(out ho_LowerRight);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {
                //This procedure displays text in a graphics window.
                //
                //Input parameters:
                //WindowHandle: The WindowHandle of the graphics window, where
                //   the message should be displayed
                //String: A tuple of strings containing the text message to be displayed
                //CoordSystem: If set to 'window', the text position is given
                //   with respect to the window coordinate system.
                //   If set to 'image', image coordinates are used.
                //   (This may be useful in zoomed images.)
                //Row: The row coordinate of the desired text position
                //   If set to -1, a default value of 12 is used.
                //Column: The column coordinate of the desired text position
                //   If set to -1, a default value of 12 is used.
                //Color: defines the color of the text as string.
                //   If set to [], '' or 'auto' the currently set color is used.
                //   If a tuple of strings is passed, the colors are used cyclically
                //   for each new textline.
                //ButtonColor: Must be set to a color string (e.g. 'white', '#FF00CC', etc.).
                //             The text is written in a box of that color.
                //
                //Prepare window.
                hv_Red.Dispose(); hv_Green.Dispose(); hv_Blue.Dispose();
                HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
                hv_Row1Part.Dispose(); hv_Column1Part.Dispose(); hv_Row2Part.Dispose(); hv_Column2Part.Dispose();
                HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part,
                    out hv_Row2Part, out hv_Column2Part);
                hv_RowWin.Dispose(); hv_ColumnWin.Dispose(); hv_WidthWin.Dispose(); hv_HeightWin.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                    out hv_WidthWin, out hv_HeightWin);

                HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);

                //
                //Default settings.
                if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_TextColor_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
                {
                    hv_TextColor_COPY_INP_TMP.Dispose();
                    hv_TextColor_COPY_INP_TMP = "";
                }
                //
                try
                {
                    hv_RGB.Dispose();
                    Color_string_to_rgb(hv_ButtonColor, out hv_RGB);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter ButtonColor (must be a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                hv_Fac.Dispose();
                hv_Fac = 0.4;
                hv_RGBL.Dispose();

                hv_RGBL = hv_RGB + (((((255.0 - hv_RGB) * hv_Fac) + 0.5)).TupleInt()
                    );

                hv_RGBD.Dispose();

                hv_RGBD = hv_RGB - ((((hv_RGB * hv_Fac) + 0.5)).TupleInt()
                    );

                hv_ButtonColorBorderL.Dispose();

                hv_ButtonColorBorderL = "#" + ((("" + (hv_RGBL.TupleString(
                    "02x")))).TupleSum());

                hv_ButtonColorBorderD.Dispose();

                hv_ButtonColorBorderD = "#" + ((("" + (hv_RGBD.TupleString(
                    "02x")))).TupleSum());

                //

                {
                    HTuple
                      ExpTmpLocalVar_String = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit(
                        "\n");
                    hv_String_COPY_INP_TMP.Dispose();
                    hv_String_COPY_INP_TMP = ExpTmpLocalVar_String;
                }

                //
                //Estimate extensions of text depending on font size.
                hv_MaxAscent.Dispose(); hv_MaxDescent.Dispose(); hv_MaxWidth.Dispose(); hv_MaxHeight.Dispose();
                HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                    out hv_MaxWidth, out hv_MaxHeight);
                if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
                {
                    hv_R1.Dispose();
                    hv_R1 = new HTuple(hv_Row_COPY_INP_TMP);
                    hv_C1.Dispose();
                    hv_C1 = new HTuple(hv_Column_COPY_INP_TMP);
                }
                else
                {
                    //Transform image to window coordinates.
                    hv_FactorRow.Dispose();

                    hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);

                    hv_FactorColumn.Dispose();

                    hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);

                    hv_R1.Dispose();

                    hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;

                    hv_C1.Dispose();

                    hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;

                }
                //
                //Display text box depending on text size.
                //
                //Calculate box extents.

                {
                    HTuple
                      ExpTmpLocalVar_String = (" " + hv_String_COPY_INP_TMP) + " ";
                    hv_String_COPY_INP_TMP.Dispose();
                    hv_String_COPY_INP_TMP = ExpTmpLocalVar_String;
                }

                hv_Width.Dispose();
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {

                    hv_Ascent.Dispose(); hv_Descent.Dispose(); hv_W.Dispose(); hv_H.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);


                    {
                        HTuple
                          ExpTmpLocalVar_Width = hv_Width.TupleConcat(
                            hv_W);
                        hv_Width.Dispose();
                        hv_Width = ExpTmpLocalVar_Width;
                    }

                }
                hv_FrameHeight.Dispose();

                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));

                hv_FrameWidth.Dispose();

                hv_FrameWidth = (((new HTuple(0)).TupleConcat(
                    hv_Width))).TupleMax();

                hv_R2.Dispose();

                hv_R2 = hv_R1 + hv_FrameHeight;

                hv_C2.Dispose();

                hv_C2 = hv_C1 + hv_FrameWidth;

                //Display rectangles.
                hv_ClipRegion.Dispose();
                HOperatorSet.GetSystem("clip_region", out hv_ClipRegion);
                HOperatorSet.SetSystem("clip_region", "false");
                hv_DrawMode.Dispose();
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                hv_BorderWidth.Dispose();
                hv_BorderWidth = 2;

                ho_UpperLeft.Dispose();
                HOperatorSet.GenRegionPolygonFilled(out ho_UpperLeft, ((((((((hv_R1 - hv_BorderWidth)).TupleConcat(
                    hv_R1 - hv_BorderWidth))).TupleConcat(hv_R1))).TupleConcat(hv_R2))).TupleConcat(
                    hv_R2 + hv_BorderWidth), ((((((((hv_C1 - hv_BorderWidth)).TupleConcat(hv_C2 + hv_BorderWidth))).TupleConcat(
                    hv_C2))).TupleConcat(hv_C1))).TupleConcat(hv_C1 - hv_BorderWidth));


                ho_LowerRight.Dispose();
                HOperatorSet.GenRegionPolygonFilled(out ho_LowerRight, ((((((((hv_R2 + hv_BorderWidth)).TupleConcat(
                    hv_R1 - hv_BorderWidth))).TupleConcat(hv_R1))).TupleConcat(hv_R2))).TupleConcat(
                    hv_R2 + hv_BorderWidth), ((((((((hv_C2 + hv_BorderWidth)).TupleConcat(hv_C2 + hv_BorderWidth))).TupleConcat(
                    hv_C2))).TupleConcat(hv_C1))).TupleConcat(hv_C1 - hv_BorderWidth));

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColorBorderL);
                HOperatorSet.DispObj(ho_UpperLeft, hv_WindowHandle);
                HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColorBorderD);
                HOperatorSet.DispObj(ho_LowerRight, hv_WindowHandle);
                HOperatorSet.SetColor(hv_WindowHandle, hv_ButtonColor);
                HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandle);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
                HOperatorSet.SetSystem("clip_region", hv_ClipRegion);
                //Write text.
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_CurrentColor.Dispose();

                    hv_CurrentColor = hv_TextColor_COPY_INP_TMP.TupleSelect(
                        hv_Index % (new HTuple(hv_TextColor_COPY_INP_TMP.TupleLength())));

                    if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                        "auto")))) != 0)
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                    }
                    else
                    {
                        HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                    }
                    hv_Row_COPY_INP_TMP.Dispose();

                    hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);

                    HOperatorSet.DispText(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), "window", hv_Row_COPY_INP_TMP, hv_C1, hv_CurrentColor, "box",
                        "false");

                }
                //Reset changed window settings.
                HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                    hv_Column2Part);
                ho_UpperLeft.Dispose();
                ho_LowerRight.Dispose();
                ho_Rectangle.Dispose();

                hv_Column_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_String_COPY_INP_TMP.Dispose();
                hv_TextColor_COPY_INP_TMP.Dispose();
                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Row1Part.Dispose();
                hv_Column1Part.Dispose();
                hv_Row2Part.Dispose();
                hv_Column2Part.Dispose();
                hv_RowWin.Dispose();
                hv_ColumnWin.Dispose();
                hv_WidthWin.Dispose();
                hv_HeightWin.Dispose();
                hv_RGB.Dispose();
                hv_Exception.Dispose();
                hv_Fac.Dispose();
                hv_RGBL.Dispose();
                hv_RGBD.Dispose();
                hv_ButtonColorBorderL.Dispose();
                hv_ButtonColorBorderD.Dispose();
                hv_MaxAscent.Dispose();
                hv_MaxDescent.Dispose();
                hv_MaxWidth.Dispose();
                hv_MaxHeight.Dispose();
                hv_R1.Dispose();
                hv_C1.Dispose();
                hv_FactorRow.Dispose();
                hv_FactorColumn.Dispose();
                hv_Width.Dispose();
                hv_Index.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_W.Dispose();
                hv_H.Dispose();
                hv_FrameHeight.Dispose();
                hv_FrameWidth.Dispose();
                hv_R2.Dispose();
                hv_C2.Dispose();
                hv_ClipRegion.Dispose();
                hv_DrawMode.Dispose();
                hv_BorderWidth.Dispose();
                hv_CurrentColor.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_UpperLeft.Dispose();
                ho_LowerRight.Dispose();
                ho_Rectangle.Dispose();

                hv_Column_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_String_COPY_INP_TMP.Dispose();
                hv_TextColor_COPY_INP_TMP.Dispose();
                hv_Red.Dispose();
                hv_Green.Dispose();
                hv_Blue.Dispose();
                hv_Row1Part.Dispose();
                hv_Column1Part.Dispose();
                hv_Row2Part.Dispose();
                hv_Column2Part.Dispose();
                hv_RowWin.Dispose();
                hv_ColumnWin.Dispose();
                hv_WidthWin.Dispose();
                hv_HeightWin.Dispose();
                hv_RGB.Dispose();
                hv_Exception.Dispose();
                hv_Fac.Dispose();
                hv_RGBL.Dispose();
                hv_RGBD.Dispose();
                hv_ButtonColorBorderL.Dispose();
                hv_ButtonColorBorderD.Dispose();
                hv_MaxAscent.Dispose();
                hv_MaxDescent.Dispose();
                hv_MaxWidth.Dispose();
                hv_MaxHeight.Dispose();
                hv_R1.Dispose();
                hv_C1.Dispose();
                hv_FactorRow.Dispose();
                hv_FactorColumn.Dispose();
                hv_Width.Dispose();
                hv_Index.Dispose();
                hv_Ascent.Dispose();
                hv_Descent.Dispose();
                hv_W.Dispose();
                hv_H.Dispose();
                hv_FrameHeight.Dispose();
                hv_FrameWidth.Dispose();
                hv_R2.Dispose();
                hv_C2.Dispose();
                hv_ClipRegion.Dispose();
                hv_DrawMode.Dispose();
                hv_BorderWidth.Dispose();
                hv_CurrentColor.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Chapter: Graphics / Parameters
        private static void Color_string_to_rgb(HTuple hv_Color, out HTuple hv_RGB)
        {



            // Local iconic variables 

            HObject ho_Rectangle, ho_Image;

            // Local control variables 

            HTuple hv_WindowHandleBuffer = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_RGB = new HTuple();
            try
            {
                hv_WindowHandleBuffer.Dispose();
                HOperatorSet.OpenWindow(0, 0, 1, 1, 0, "buffer", "", out hv_WindowHandleBuffer);
                HOperatorSet.SetPart(hv_WindowHandleBuffer, 0, 0, -1, -1);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, 0, 0);
                try
                {
                    HOperatorSet.SetColor(hv_WindowHandleBuffer, hv_Color);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Color (must be a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandleBuffer);
                ho_Image.Dispose();
                HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandleBuffer);
                HOperatorSet.CloseWindow(hv_WindowHandleBuffer);
                hv_RGB.Dispose();
                HOperatorSet.GetGrayval(ho_Image, 0, 0, out hv_RGB);

                {
                    HTuple
                      ExpTmpLocalVar_RGB = hv_RGB + (
                        (new HTuple(0)).TupleConcat(0)).TupleConcat(0);
                    hv_RGB.Dispose();
                    hv_RGB = ExpTmpLocalVar_RGB;
                }

                ho_Rectangle.Dispose();
                ho_Image.Dispose();

                hv_WindowHandleBuffer.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_Rectangle.Dispose();
                ho_Image.Dispose();

                hv_WindowHandleBuffer.Dispose();
                hv_Exception.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Procedures 
        // External procedures 
        // Chapter: Graphics / Output
        // Short Description: Reflect the pose change that was introduced by the user by moving the mouse 
        private static void Analyze_graph_event(HObject ho_BackgroundImage, HTuple hv_MouseMapping,
            HTuple hv_Button, HTuple hv_Row, HTuple hv_Column, HTuple hv_WindowHandle, HTuple hv_WindowHandleBuffer,
            HTuple hv_VirtualTrackball, HTuple hv_TrackballSize, HTuple hv_SelectedObjectIn,
            HTuple hv_Scene3D, HTuple hv_AlphaOrig, HTuple hv_ObjectModel3DID, HTuple hv_CamParam,
            HTuple hv_Labels, HTuple hv_Title, HTuple hv_Information, HTuple hv_GenParamName,
            HTuple hv_GenParamValue, HTuple hv_PosesIn, HTuple hv_ButtonHoldIn, HTuple hv_TBCenter,
            HTuple hv_TBSize, HTuple hv_WindowCenteredRotationlIn, HTuple hv_MaxNumModels,
            out HTuple hv_PosesOut, out HTuple hv_SelectedObjectOut, out HTuple hv_ButtonHoldOut,
            out HTuple hv_WindowCenteredRotationOut)
        {




            // Local iconic variables 

            HObject ho_ImageDump = null;

            // Local control variables 

            HTuple ExpTmpLocalVar_gIsSinglePose = new HTuple();
            HTuple hv_VisualizeTB = new HTuple(), hv_InvLog2 = new HTuple();
            HTuple hv_Seconds = new HTuple(), hv_ModelIndex = new HTuple();
            HTuple hv_Exception1 = new HTuple(), hv_HomMat3DIdentity = new HTuple();
            HTuple hv_NumModels = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_MinImageSize = new HTuple();
            HTuple hv_TrackballRadiusPixel = new HTuple(), hv_TrackballCenterRow = new HTuple();
            HTuple hv_TrackballCenterCol = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ColorImage = new HTuple(), hv_BAnd = new HTuple();
            HTuple hv_SensFactor = new HTuple(), hv_IsButtonTrans = new HTuple();
            HTuple hv_IsButtonRot = new HTuple(), hv_IsButtonDist = new HTuple();
            HTuple hv_MRow1 = new HTuple(), hv_MCol1 = new HTuple();
            HTuple hv_ButtonLoop = new HTuple(), hv_MRow2 = new HTuple();
            HTuple hv_MCol2 = new HTuple(), hv_PX = new HTuple(), hv_PY = new HTuple();
            HTuple hv_PZ = new HTuple(), hv_QX1 = new HTuple(), hv_QY1 = new HTuple();
            HTuple hv_QZ1 = new HTuple(), hv_QX2 = new HTuple(), hv_QY2 = new HTuple();
            HTuple hv_QZ2 = new HTuple(), hv_Len = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_Translate = new HTuple(), hv_Index = new HTuple();
            HTuple hv_PoseIn = new HTuple(), hv_HomMat3DIn = new HTuple();
            HTuple hv_HomMat3DOut = new HTuple(), hv_PoseOut = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_Sequence = new HTuple();
            HTuple hv_Mod = new HTuple(), hv_SequenceReal = new HTuple();
            HTuple hv_Sequence2Int = new HTuple(), hv_Selected = new HTuple();
            HTuple hv_InvSelected = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_DRow = new HTuple(), hv_TranslateZ = new HTuple();
            HTuple hv_MX1 = new HTuple(), hv_MY1 = new HTuple(), hv_MX2 = new HTuple();
            HTuple hv_MY2 = new HTuple(), hv_RelQuaternion = new HTuple();
            HTuple hv_HomMat3DRotRel = new HTuple(), hv_HomMat3DInTmp1 = new HTuple();
            HTuple hv_HomMat3DInTmp = new HTuple(), hv_PosesOut2 = new HTuple();
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesIn);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            HTuple hv_TBCenter_COPY_INP_TMP = new HTuple(hv_TBCenter);
            HTuple hv_TBSize_COPY_INP_TMP = new HTuple(hv_TBSize);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageDump);
            hv_PosesOut = new HTuple();
            hv_SelectedObjectOut = new HTuple();
            hv_ButtonHoldOut = new HTuple();
            hv_WindowCenteredRotationOut = new HTuple();
            try
            {
                //This procedure reflects
                //- the pose change that was introduced by the user by
                //  moving the mouse
                //- the selection of a single object
                //
                //global tuple gIsSinglePose
                //
                hv_ButtonHoldOut.Dispose();
                hv_ButtonHoldOut = new HTuple(hv_ButtonHoldIn);
                hv_PosesOut.Dispose();
                hv_PosesOut = new HTuple(hv_PosesIn_COPY_INP_TMP);
                hv_SelectedObjectOut.Dispose();
                hv_SelectedObjectOut = new HTuple(hv_SelectedObjectIn);
                hv_WindowCenteredRotationOut.Dispose();
                hv_WindowCenteredRotationOut = new HTuple(hv_WindowCenteredRotationlIn);
                hv_VisualizeTB.Dispose();

                hv_VisualizeTB = new HTuple(((hv_SelectedObjectOut.TupleMax()
                    )).TupleNotEqual(0));

                hv_InvLog2.Dispose();

                hv_InvLog2 = 1.0 / ((new HTuple(2)).TupleLog()
                    );


                if ((int)(new HTuple(hv_Button.TupleEqual(hv_MouseMapping.TupleSelect(6)))) != 0)
                {
                    if ((int)(hv_ButtonHoldOut) != 0)
                    {
                        ho_ImageDump.Dispose();

                        hv_Column_COPY_INP_TMP.Dispose();
                        hv_PosesIn_COPY_INP_TMP.Dispose();
                        hv_Row_COPY_INP_TMP.Dispose();
                        hv_TBCenter_COPY_INP_TMP.Dispose();
                        hv_TBSize_COPY_INP_TMP.Dispose();
                        hv_VisualizeTB.Dispose();
                        hv_InvLog2.Dispose();
                        hv_Seconds.Dispose();
                        hv_ModelIndex.Dispose();
                        hv_Exception1.Dispose();
                        hv_HomMat3DIdentity.Dispose();
                        hv_NumModels.Dispose();
                        hv_Width.Dispose();
                        hv_Height.Dispose();
                        hv_MinImageSize.Dispose();
                        hv_TrackballRadiusPixel.Dispose();
                        hv_TrackballCenterRow.Dispose();
                        hv_TrackballCenterCol.Dispose();
                        hv_NumChannels.Dispose();
                        hv_ColorImage.Dispose();
                        hv_BAnd.Dispose();
                        hv_SensFactor.Dispose();
                        hv_IsButtonTrans.Dispose();
                        hv_IsButtonRot.Dispose();
                        hv_IsButtonDist.Dispose();
                        hv_MRow1.Dispose();
                        hv_MCol1.Dispose();
                        hv_ButtonLoop.Dispose();
                        hv_MRow2.Dispose();
                        hv_MCol2.Dispose();
                        hv_PX.Dispose();
                        hv_PY.Dispose();
                        hv_PZ.Dispose();
                        hv_QX1.Dispose();
                        hv_QY1.Dispose();
                        hv_QZ1.Dispose();
                        hv_QX2.Dispose();
                        hv_QY2.Dispose();
                        hv_QZ2.Dispose();
                        hv_Len.Dispose();
                        hv_Dist.Dispose();
                        hv_Translate.Dispose();
                        hv_Index.Dispose();
                        hv_PoseIn.Dispose();
                        hv_HomMat3DIn.Dispose();
                        hv_HomMat3DOut.Dispose();
                        hv_PoseOut.Dispose();
                        hv_Indices.Dispose();
                        hv_Sequence.Dispose();
                        hv_Mod.Dispose();
                        hv_SequenceReal.Dispose();
                        hv_Sequence2Int.Dispose();
                        hv_Selected.Dispose();
                        hv_InvSelected.Dispose();
                        hv_Exception.Dispose();
                        hv_DRow.Dispose();
                        hv_TranslateZ.Dispose();
                        hv_MX1.Dispose();
                        hv_MY1.Dispose();
                        hv_MX2.Dispose();
                        hv_MY2.Dispose();
                        hv_RelQuaternion.Dispose();
                        hv_HomMat3DRotRel.Dispose();
                        hv_HomMat3DInTmp1.Dispose();
                        hv_HomMat3DInTmp.Dispose();
                        hv_PosesOut2.Dispose();

                        return;
                    }
                    //Ctrl (16) + Alt (32) + left mouse button (1) => Toggle rotation center position
                    //If WindowCenteredRotation is not 1, set it to 1, otherwise, set it to 2
                    hv_Seconds.Dispose();
                    HOperatorSet.CountSeconds(out hv_Seconds);
                    if ((int)(new HTuple(hv_WindowCenteredRotationOut.TupleEqual(1))) != 0)
                    {
                        hv_WindowCenteredRotationOut.Dispose();
                        hv_WindowCenteredRotationOut = 2;
                    }
                    else
                    {
                        hv_WindowCenteredRotationOut.Dispose();
                        hv_WindowCenteredRotationOut = 1;
                    }
                    hv_ButtonHoldOut.Dispose();
                    hv_ButtonHoldOut = 1;
                    ho_ImageDump.Dispose();

                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_PosesIn_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_TBCenter_COPY_INP_TMP.Dispose();
                    hv_TBSize_COPY_INP_TMP.Dispose();
                    hv_VisualizeTB.Dispose();
                    hv_InvLog2.Dispose();
                    hv_Seconds.Dispose();
                    hv_ModelIndex.Dispose();
                    hv_Exception1.Dispose();
                    hv_HomMat3DIdentity.Dispose();
                    hv_NumModels.Dispose();
                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_MinImageSize.Dispose();
                    hv_TrackballRadiusPixel.Dispose();
                    hv_TrackballCenterRow.Dispose();
                    hv_TrackballCenterCol.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ColorImage.Dispose();
                    hv_BAnd.Dispose();
                    hv_SensFactor.Dispose();
                    hv_IsButtonTrans.Dispose();
                    hv_IsButtonRot.Dispose();
                    hv_IsButtonDist.Dispose();
                    hv_MRow1.Dispose();
                    hv_MCol1.Dispose();
                    hv_ButtonLoop.Dispose();
                    hv_MRow2.Dispose();
                    hv_MCol2.Dispose();
                    hv_PX.Dispose();
                    hv_PY.Dispose();
                    hv_PZ.Dispose();
                    hv_QX1.Dispose();
                    hv_QY1.Dispose();
                    hv_QZ1.Dispose();
                    hv_QX2.Dispose();
                    hv_QY2.Dispose();
                    hv_QZ2.Dispose();
                    hv_Len.Dispose();
                    hv_Dist.Dispose();
                    hv_Translate.Dispose();
                    hv_Index.Dispose();
                    hv_PoseIn.Dispose();
                    hv_HomMat3DIn.Dispose();
                    hv_HomMat3DOut.Dispose();
                    hv_PoseOut.Dispose();
                    hv_Indices.Dispose();
                    hv_Sequence.Dispose();
                    hv_Mod.Dispose();
                    hv_SequenceReal.Dispose();
                    hv_Sequence2Int.Dispose();
                    hv_Selected.Dispose();
                    hv_InvSelected.Dispose();
                    hv_Exception.Dispose();
                    hv_DRow.Dispose();
                    hv_TranslateZ.Dispose();
                    hv_MX1.Dispose();
                    hv_MY1.Dispose();
                    hv_MX2.Dispose();
                    hv_MY2.Dispose();
                    hv_RelQuaternion.Dispose();
                    hv_HomMat3DRotRel.Dispose();
                    hv_HomMat3DInTmp1.Dispose();
                    hv_HomMat3DInTmp.Dispose();
                    hv_PosesOut2.Dispose();

                    return;
                }
                if ((int)((new HTuple(hv_Button.TupleEqual(hv_MouseMapping.TupleSelect(5)))).TupleAnd(
                    new HTuple((new HTuple(hv_ObjectModel3DID.TupleLength())).TupleLessEqual(
                    hv_MaxNumModels)))) != 0)
                {
                    if ((int)(hv_ButtonHoldOut) != 0)
                    {
                        ho_ImageDump.Dispose();

                        hv_Column_COPY_INP_TMP.Dispose();
                        hv_PosesIn_COPY_INP_TMP.Dispose();
                        hv_Row_COPY_INP_TMP.Dispose();
                        hv_TBCenter_COPY_INP_TMP.Dispose();
                        hv_TBSize_COPY_INP_TMP.Dispose();
                        hv_VisualizeTB.Dispose();
                        hv_InvLog2.Dispose();
                        hv_Seconds.Dispose();
                        hv_ModelIndex.Dispose();
                        hv_Exception1.Dispose();
                        hv_HomMat3DIdentity.Dispose();
                        hv_NumModels.Dispose();
                        hv_Width.Dispose();
                        hv_Height.Dispose();
                        hv_MinImageSize.Dispose();
                        hv_TrackballRadiusPixel.Dispose();
                        hv_TrackballCenterRow.Dispose();
                        hv_TrackballCenterCol.Dispose();
                        hv_NumChannels.Dispose();
                        hv_ColorImage.Dispose();
                        hv_BAnd.Dispose();
                        hv_SensFactor.Dispose();
                        hv_IsButtonTrans.Dispose();
                        hv_IsButtonRot.Dispose();
                        hv_IsButtonDist.Dispose();
                        hv_MRow1.Dispose();
                        hv_MCol1.Dispose();
                        hv_ButtonLoop.Dispose();
                        hv_MRow2.Dispose();
                        hv_MCol2.Dispose();
                        hv_PX.Dispose();
                        hv_PY.Dispose();
                        hv_PZ.Dispose();
                        hv_QX1.Dispose();
                        hv_QY1.Dispose();
                        hv_QZ1.Dispose();
                        hv_QX2.Dispose();
                        hv_QY2.Dispose();
                        hv_QZ2.Dispose();
                        hv_Len.Dispose();
                        hv_Dist.Dispose();
                        hv_Translate.Dispose();
                        hv_Index.Dispose();
                        hv_PoseIn.Dispose();
                        hv_HomMat3DIn.Dispose();
                        hv_HomMat3DOut.Dispose();
                        hv_PoseOut.Dispose();
                        hv_Indices.Dispose();
                        hv_Sequence.Dispose();
                        hv_Mod.Dispose();
                        hv_SequenceReal.Dispose();
                        hv_Sequence2Int.Dispose();
                        hv_Selected.Dispose();
                        hv_InvSelected.Dispose();
                        hv_Exception.Dispose();
                        hv_DRow.Dispose();
                        hv_TranslateZ.Dispose();
                        hv_MX1.Dispose();
                        hv_MY1.Dispose();
                        hv_MX2.Dispose();
                        hv_MY2.Dispose();
                        hv_RelQuaternion.Dispose();
                        hv_HomMat3DRotRel.Dispose();
                        hv_HomMat3DInTmp1.Dispose();
                        hv_HomMat3DInTmp.Dispose();
                        hv_PosesOut2.Dispose();

                        return;
                    }
                    //Ctrl (16) + left mouse button (1) => Select an object
                    try
                    {
                        HOperatorSet.SetScene3dParam(hv_Scene3D, "object_index_persistence", "true");
                        HOperatorSet.DisplayScene3d(hv_WindowHandleBuffer, hv_Scene3D, 0);
                        hv_ModelIndex.Dispose();
                        HOperatorSet.GetDisplayScene3dInfo(hv_WindowHandleBuffer, hv_Scene3D, hv_Row_COPY_INP_TMP,
                            hv_Column_COPY_INP_TMP, "object_index", out hv_ModelIndex);
                        HOperatorSet.SetScene3dParam(hv_Scene3D, "object_index_persistence", "false");
                    }
                    // catch (Exception1) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception1);
                        //* NO OpenGL, no selection possible
                        ho_ImageDump.Dispose();

                        hv_Column_COPY_INP_TMP.Dispose();
                        hv_PosesIn_COPY_INP_TMP.Dispose();
                        hv_Row_COPY_INP_TMP.Dispose();
                        hv_TBCenter_COPY_INP_TMP.Dispose();
                        hv_TBSize_COPY_INP_TMP.Dispose();
                        hv_VisualizeTB.Dispose();
                        hv_InvLog2.Dispose();
                        hv_Seconds.Dispose();
                        hv_ModelIndex.Dispose();
                        hv_Exception1.Dispose();
                        hv_HomMat3DIdentity.Dispose();
                        hv_NumModels.Dispose();
                        hv_Width.Dispose();
                        hv_Height.Dispose();
                        hv_MinImageSize.Dispose();
                        hv_TrackballRadiusPixel.Dispose();
                        hv_TrackballCenterRow.Dispose();
                        hv_TrackballCenterCol.Dispose();
                        hv_NumChannels.Dispose();
                        hv_ColorImage.Dispose();
                        hv_BAnd.Dispose();
                        hv_SensFactor.Dispose();
                        hv_IsButtonTrans.Dispose();
                        hv_IsButtonRot.Dispose();
                        hv_IsButtonDist.Dispose();
                        hv_MRow1.Dispose();
                        hv_MCol1.Dispose();
                        hv_ButtonLoop.Dispose();
                        hv_MRow2.Dispose();
                        hv_MCol2.Dispose();
                        hv_PX.Dispose();
                        hv_PY.Dispose();
                        hv_PZ.Dispose();
                        hv_QX1.Dispose();
                        hv_QY1.Dispose();
                        hv_QZ1.Dispose();
                        hv_QX2.Dispose();
                        hv_QY2.Dispose();
                        hv_QZ2.Dispose();
                        hv_Len.Dispose();
                        hv_Dist.Dispose();
                        hv_Translate.Dispose();
                        hv_Index.Dispose();
                        hv_PoseIn.Dispose();
                        hv_HomMat3DIn.Dispose();
                        hv_HomMat3DOut.Dispose();
                        hv_PoseOut.Dispose();
                        hv_Indices.Dispose();
                        hv_Sequence.Dispose();
                        hv_Mod.Dispose();
                        hv_SequenceReal.Dispose();
                        hv_Sequence2Int.Dispose();
                        hv_Selected.Dispose();
                        hv_InvSelected.Dispose();
                        hv_Exception.Dispose();
                        hv_DRow.Dispose();
                        hv_TranslateZ.Dispose();
                        hv_MX1.Dispose();
                        hv_MY1.Dispose();
                        hv_MX2.Dispose();
                        hv_MY2.Dispose();
                        hv_RelQuaternion.Dispose();
                        hv_HomMat3DRotRel.Dispose();
                        hv_HomMat3DInTmp1.Dispose();
                        hv_HomMat3DInTmp.Dispose();
                        hv_PosesOut2.Dispose();

                        return;
                    }
                    if ((int)(new HTuple(hv_ModelIndex.TupleEqual(-1))) != 0)
                    {
                        //Background click:
                        if ((int)(new HTuple(((hv_SelectedObjectOut.TupleSum())).TupleEqual(new HTuple(hv_SelectedObjectOut.TupleLength()
                            )))) != 0)
                        {
                            //If all objects are already selected, deselect all
                            hv_SelectedObjectOut.Dispose();

                            hv_SelectedObjectOut = HTuple.TupleGenConst(
                                new HTuple(hv_ObjectModel3DID.TupleLength()), 0);

                        }
                        else
                        {
                            //Otherwise select all
                            hv_SelectedObjectOut.Dispose();

                            hv_SelectedObjectOut = HTuple.TupleGenConst(
                                new HTuple(hv_ObjectModel3DID.TupleLength()), 1);

                        }
                    }
                    else
                    {
                        //Object click:
                        if (hv_SelectedObjectOut == null)
                            hv_SelectedObjectOut = new HTuple();
                        hv_SelectedObjectOut[hv_ModelIndex] = ((hv_SelectedObjectOut.TupleSelect(
                            hv_ModelIndex))).TupleNot();
                    }
                    hv_ButtonHoldOut.Dispose();
                    hv_ButtonHoldOut = 1;
                }
                else
                {
                    //Change the pose
                    hv_HomMat3DIdentity.Dispose();
                    HOperatorSet.HomMat3dIdentity(out hv_HomMat3DIdentity);
                    hv_NumModels.Dispose();

                    hv_NumModels = new HTuple(hv_ObjectModel3DID.TupleLength()
                        );

                    hv_Width.Dispose();
                    Get_cam_par_data(hv_CamParam, "image_width", out hv_Width);
                    hv_Height.Dispose();
                    Get_cam_par_data(hv_CamParam, "image_height", out hv_Height);
                    hv_MinImageSize.Dispose();

                    hv_MinImageSize = ((hv_Width.TupleConcat(
                        hv_Height))).TupleMin();

                    hv_TrackballRadiusPixel.Dispose();

                    hv_TrackballRadiusPixel = (hv_TrackballSize * hv_MinImageSize) / 2.0;

                    //Set trackball fixed in the center of the window
                    hv_TrackballCenterRow.Dispose();

                    hv_TrackballCenterRow = hv_Height / 2;

                    hv_TrackballCenterCol.Dispose();

                    hv_TrackballCenterCol = hv_Width / 2;

                    if ((int)(new HTuple((new HTuple(hv_ObjectModel3DID.TupleLength())).TupleLess(
                        hv_MaxNumModels))) != 0)
                    {
                        if ((int)(new HTuple(hv_WindowCenteredRotationOut.TupleEqual(1))) != 0)
                        {
                            hv_TBCenter_COPY_INP_TMP.Dispose(); hv_TBSize_COPY_INP_TMP.Dispose();
                            Get_trackball_center_fixed(hv_SelectedObjectIn, hv_TrackballCenterRow,
                                hv_TrackballCenterCol, hv_TrackballRadiusPixel, hv_Scene3D, hv_ObjectModel3DID,
                                hv_PosesIn_COPY_INP_TMP, hv_WindowHandleBuffer, hv_CamParam, hv_GenParamName,
                                hv_GenParamValue, out hv_TBCenter_COPY_INP_TMP, out hv_TBSize_COPY_INP_TMP);
                        }
                        else
                        {
                            hv_TBCenter_COPY_INP_TMP.Dispose(); hv_TBSize_COPY_INP_TMP.Dispose();
                            Get_trackball_center(hv_SelectedObjectIn, hv_TrackballRadiusPixel, hv_ObjectModel3DID,
                                hv_PosesIn_COPY_INP_TMP, out hv_TBCenter_COPY_INP_TMP, out hv_TBSize_COPY_INP_TMP);
                        }
                    }
                    if ((int)((new HTuple(((hv_SelectedObjectOut.TupleMin())).TupleEqual(0))).TupleAnd(
                        new HTuple(((hv_SelectedObjectOut.TupleMax())).TupleEqual(1)))) != 0)
                    {
                        //At this point, multiple objects do not necessary have the same
                        //pose any more. Consequently, we have to return a tuple of poses
                        //as output of visualize_object_model_3d
                        gIsSinglePose = 0;
                        //ExpSetGlobalVar_gIsSinglePose(ExpTmpLocalVar_gIsSinglePose);
                    }
                    hv_NumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_BackgroundImage, out hv_NumChannels);
                    hv_ColorImage.Dispose();

                    hv_ColorImage = new HTuple(hv_NumChannels.TupleEqual(
                        3));

                    //Alt (32) => lower sensitivity
                    hv_BAnd.Dispose();
                    HOperatorSet.TupleRsh(hv_Button, 5, out hv_BAnd);
                    if ((int)(hv_BAnd % 2) != 0)
                    {
                        hv_SensFactor.Dispose();
                        hv_SensFactor = 0.1;
                    }
                    else
                    {
                        hv_SensFactor.Dispose();
                        hv_SensFactor = 1.0;
                    }
                    hv_IsButtonTrans.Dispose();

                    hv_IsButtonTrans = (new HTuple(((hv_MouseMapping.TupleSelect(
                        0))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        0)))).TupleEqual(hv_Button)));

                    hv_IsButtonRot.Dispose();

                    hv_IsButtonRot = (new HTuple(((hv_MouseMapping.TupleSelect(
                        1))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        1)))).TupleEqual(hv_Button)));

                    hv_IsButtonDist.Dispose();

                    hv_IsButtonDist = (new HTuple((new HTuple((new HTuple((new HTuple((new HTuple(((hv_MouseMapping.TupleSelect(
                        2))).TupleEqual(hv_Button))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        2)))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((hv_MouseMapping.TupleSelect(
                        3))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        3)))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((hv_MouseMapping.TupleSelect(
                        4))).TupleEqual(hv_Button))))).TupleOr(new HTuple(((32 + (hv_MouseMapping.TupleSelect(
                        4)))).TupleEqual(hv_Button)));

                    if ((int)(hv_IsButtonTrans) != 0)
                    {
                        //Translate in XY-direction
                        hv_MRow1.Dispose();
                        hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                        hv_MCol1.Dispose();
                        hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                        while ((int)(hv_IsButtonTrans) != 0)
                        {
                            try
                            {
                                hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                                HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                                    out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                                hv_IsButtonTrans.Dispose();

                                hv_IsButtonTrans = new HTuple(hv_ButtonLoop.TupleEqual(
                                    hv_Button));

                                hv_MRow2.Dispose();

                                hv_MRow2 = hv_MRow1 + ((hv_Row_COPY_INP_TMP - hv_MRow1) * hv_SensFactor);

                                hv_MCol2.Dispose();

                                hv_MCol2 = hv_MCol1 + ((hv_Column_COPY_INP_TMP - hv_MCol1) * hv_SensFactor);

                                hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX1.Dispose(); hv_QY1.Dispose(); hv_QZ1.Dispose();
                                HOperatorSet.GetLineOfSight(hv_MRow1, hv_MCol1, hv_CamParam, out hv_PX,
                                    out hv_PY, out hv_PZ, out hv_QX1, out hv_QY1, out hv_QZ1);
                                hv_PX.Dispose(); hv_PY.Dispose(); hv_PZ.Dispose(); hv_QX2.Dispose(); hv_QY2.Dispose(); hv_QZ2.Dispose();
                                HOperatorSet.GetLineOfSight(hv_MRow2, hv_MCol2, hv_CamParam, out hv_PX,
                                    out hv_PY, out hv_PZ, out hv_QX2, out hv_QY2, out hv_QZ2);
                                hv_Len.Dispose();

                                hv_Len = ((((hv_QX1 * hv_QX1) + (hv_QY1 * hv_QY1)) + (hv_QZ1 * hv_QZ1))).TupleSqrt()
                                    ;

                                hv_Dist.Dispose();

                                hv_Dist = (((((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    0)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(0))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    1)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(1)))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    2)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(2))))).TupleSqrt();

                                hv_Translate.Dispose();

                                hv_Translate = ((((((hv_QX2 - hv_QX1)).TupleConcat(
                                    hv_QY2 - hv_QY1))).TupleConcat(hv_QZ2 - hv_QZ1)) * hv_Dist) / hv_Len;

                                hv_PosesOut.Dispose();
                                hv_PosesOut = new HTuple();
                                if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                                {
                                    HTuple end_val110 = hv_NumModels - 1;
                                    HTuple step_val110 = 1;
                                    for (hv_Index = 0; hv_Index.Continue(end_val110, step_val110); hv_Index = hv_Index.TupleAdd(step_val110))
                                    {
                                        hv_PoseIn.Dispose();

                                        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                            hv_Index * 7, (hv_Index * 7) + 6);

                                        if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                                        {
                                            hv_HomMat3DIn.Dispose();
                                            HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);

                                            hv_HomMat3DOut.Dispose();
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_Translate.TupleSelect(
                                                0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(
                                                2), out hv_HomMat3DOut);

                                            hv_PoseOut.Dispose();
                                            HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                            HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                                        }
                                        else
                                        {
                                            hv_PoseOut.Dispose();
                                            hv_PoseOut = new HTuple(hv_PoseIn);
                                        }

                                        {
                                            HTuple
                                              ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                                                hv_PoseOut);
                                            hv_PosesOut.Dispose();
                                            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                        }

                                    }
                                }
                                else
                                {
                                    hv_Indices.Dispose();
                                    HOperatorSet.TupleFind(hv_SelectedObjectOut, 1, out hv_Indices);
                                    hv_PoseIn.Dispose();

                                    hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                        (hv_Indices.TupleSelect(0)) * 7, ((hv_Indices.TupleSelect(0)) * 7) + 6);

                                    hv_HomMat3DIn.Dispose();
                                    HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);

                                    hv_HomMat3DOut.Dispose();
                                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_Translate.TupleSelect(
                                        0), hv_Translate.TupleSelect(1), hv_Translate.TupleSelect(2),
                                        out hv_HomMat3DOut);

                                    hv_PoseOut.Dispose();
                                    HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                    hv_Sequence.Dispose();

                                    hv_Sequence = HTuple.TupleGenSequence(
                                        0, (hv_NumModels * 7) - 1, 1);

                                    hv_Mod.Dispose();
                                    HOperatorSet.TupleMod(hv_Sequence, 7, out hv_Mod);
                                    hv_SequenceReal.Dispose();

                                    hv_SequenceReal = HTuple.TupleGenSequence(
                                        0, hv_NumModels - (1.0 / 7.0), 1.0 / 7.0);

                                    hv_Sequence2Int.Dispose();

                                    hv_Sequence2Int = hv_SequenceReal.TupleInt()
                                        ;

                                    hv_Selected.Dispose();
                                    HOperatorSet.TupleSelect(hv_SelectedObjectOut, hv_Sequence2Int, out hv_Selected);
                                    hv_InvSelected.Dispose();

                                    hv_InvSelected = 1 - hv_Selected;

                                    hv_PosesOut.Dispose();
                                    HOperatorSet.TupleSelect(hv_PoseOut, hv_Mod, out hv_PosesOut);

                                    {
                                        HTuple
                                          ExpTmpLocalVar_PosesOut = (hv_PosesOut * hv_Selected) + (hv_PosesIn_COPY_INP_TMP * hv_InvSelected);
                                        hv_PosesOut.Dispose();
                                        hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                    }


                                    HOperatorSet.SetScene3dInstancePose(hv_Scene3D, HTuple.TupleGenSequence(
                                        0, hv_NumModels - 1, 1), hv_PosesOut);

                                }

                                Dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                                    hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                                    hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                                    hv_Labels, hv_VisualizeTB, "true", hv_TrackballCenterRow, hv_TrackballCenterCol,
                                    hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, new HTuple(hv_WindowCenteredRotationOut.TupleEqual(
                                    1)), hv_TBCenter_COPY_INP_TMP);

                                ho_ImageDump.Dispose();
                                HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                                HDevWindowStack.SetActive(hv_WindowHandle);
                                if (HDevWindowStack.IsOpen())
                                {
                                    HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                                }
                                //
                                hv_MRow1.Dispose();
                                hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                                hv_MCol1.Dispose();
                                hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                                hv_PosesIn_COPY_INP_TMP.Dispose();
                                hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                //Keep waiting
                            }
                        }
                    }
                    else if ((int)(hv_IsButtonDist) != 0)
                    {
                        //Change the Z distance
                        hv_MRow1.Dispose();
                        hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                        while ((int)(hv_IsButtonDist) != 0)
                        {
                            try
                            {
                                hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                                HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                                    out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                                hv_IsButtonDist.Dispose();

                                hv_IsButtonDist = new HTuple(hv_ButtonLoop.TupleEqual(
                                    hv_Button));

                                hv_MRow2.Dispose();
                                hv_MRow2 = new HTuple(hv_Row_COPY_INP_TMP);
                                hv_DRow.Dispose();

                                hv_DRow = hv_MRow2 - hv_MRow1;

                                hv_Dist.Dispose();

                                hv_Dist = (((((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    0)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(0))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    1)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(1)))) + ((hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    2)) * (hv_TBCenter_COPY_INP_TMP.TupleSelect(2))))).TupleSqrt();

                                hv_TranslateZ.Dispose();

                                hv_TranslateZ = (((-hv_Dist) * hv_DRow) * 0.003) * hv_SensFactor;

                                if (hv_TBCenter_COPY_INP_TMP == null)
                                    hv_TBCenter_COPY_INP_TMP = new HTuple();
                                hv_TBCenter_COPY_INP_TMP[2] = (hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                    2)) + hv_TranslateZ;
                                hv_PosesOut.Dispose();
                                hv_PosesOut = new HTuple();
                                if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                                {
                                    HTuple end_val164 = hv_NumModels - 1;
                                    HTuple step_val164 = 1;
                                    for (hv_Index = 0; hv_Index.Continue(end_val164, step_val164); hv_Index = hv_Index.TupleAdd(step_val164))
                                    {
                                        hv_PoseIn.Dispose();

                                        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                            hv_Index * 7, (hv_Index * 7) + 6);

                                        if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                                        {
                                            //Transform the whole scene or selected object only
                                            hv_HomMat3DIn.Dispose();
                                            HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                            hv_HomMat3DOut.Dispose();
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, 0, 0, hv_TranslateZ,
                                                out hv_HomMat3DOut);
                                            hv_PoseOut.Dispose();
                                            HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                            HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                                        }
                                        else
                                        {
                                            hv_PoseOut.Dispose();
                                            hv_PoseOut = new HTuple(hv_PoseIn);
                                        }

                                        {
                                            HTuple
                                              ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                                                hv_PoseOut);
                                            hv_PosesOut.Dispose();
                                            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                        }

                                    }
                                }
                                else
                                {
                                    hv_Indices.Dispose();
                                    HOperatorSet.TupleFind(hv_SelectedObjectOut, 1, out hv_Indices);
                                    hv_PoseIn.Dispose();

                                    hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                        (hv_Indices.TupleSelect(0)) * 7, ((hv_Indices.TupleSelect(0)) * 7) + 6);

                                    hv_HomMat3DIn.Dispose();
                                    HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                    hv_HomMat3DOut.Dispose();
                                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, 0, 0, hv_TranslateZ,
                                        out hv_HomMat3DOut);
                                    hv_PoseOut.Dispose();
                                    HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                    hv_Sequence.Dispose();

                                    hv_Sequence = HTuple.TupleGenSequence(
                                        0, (hv_NumModels * 7) - 1, 1);

                                    hv_Mod.Dispose();
                                    HOperatorSet.TupleMod(hv_Sequence, 7, out hv_Mod);
                                    hv_SequenceReal.Dispose();

                                    hv_SequenceReal = HTuple.TupleGenSequence(
                                        0, hv_NumModels - (1.0 / 7.0), 1.0 / 7.0);

                                    hv_Sequence2Int.Dispose();

                                    hv_Sequence2Int = hv_SequenceReal.TupleInt()
                                        ;

                                    hv_Selected.Dispose();
                                    HOperatorSet.TupleSelect(hv_SelectedObjectOut, hv_Sequence2Int, out hv_Selected);
                                    hv_InvSelected.Dispose();

                                    hv_InvSelected = 1 - hv_Selected;

                                    hv_PosesOut.Dispose();
                                    HOperatorSet.TupleSelect(hv_PoseOut, hv_Mod, out hv_PosesOut);

                                    {
                                        HTuple
                                          ExpTmpLocalVar_PosesOut = (hv_PosesOut * hv_Selected) + (hv_PosesIn_COPY_INP_TMP * hv_InvSelected);
                                        hv_PosesOut.Dispose();
                                        hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                    }


                                    HOperatorSet.SetScene3dInstancePose(hv_Scene3D, HTuple.TupleGenSequence(
                                        0, hv_NumModels - 1, 1), hv_PosesOut);

                                }
                                Dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                                    hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                                    hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                                    hv_Labels, hv_VisualizeTB, "true", hv_TrackballCenterRow, hv_TrackballCenterCol,
                                    hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, hv_WindowCenteredRotationOut,
                                    hv_TBCenter_COPY_INP_TMP);
                                ho_ImageDump.Dispose();
                                HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                                HDevWindowStack.SetActive(hv_WindowHandle);
                                if (HDevWindowStack.IsOpen())
                                {
                                    HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                                }
                                //
                                hv_MRow1.Dispose();
                                hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                                hv_PosesIn_COPY_INP_TMP.Dispose();
                                hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                //Keep waiting
                            }
                        }
                    }
                    else if ((int)(hv_IsButtonRot) != 0)
                    {
                        //Rotate the object
                        hv_MRow1.Dispose();
                        hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                        hv_MCol1.Dispose();
                        hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                        while ((int)(hv_IsButtonRot) != 0)
                        {
                            try
                            {
                                hv_Row_COPY_INP_TMP.Dispose(); hv_Column_COPY_INP_TMP.Dispose(); hv_ButtonLoop.Dispose();
                                HOperatorSet.GetMpositionSubPix(hv_WindowHandle, out hv_Row_COPY_INP_TMP,
                                    out hv_Column_COPY_INP_TMP, out hv_ButtonLoop);
                                hv_IsButtonRot.Dispose();

                                hv_IsButtonRot = new HTuple(hv_ButtonLoop.TupleEqual(
                                    hv_Button));

                                hv_MRow2.Dispose();
                                hv_MRow2 = new HTuple(hv_Row_COPY_INP_TMP);
                                hv_MCol2.Dispose();
                                hv_MCol2 = new HTuple(hv_Column_COPY_INP_TMP);
                                //Transform the pixel coordinates to relative image coordinates
                                hv_MX1.Dispose();

                                hv_MX1 = (hv_TrackballCenterCol - hv_MCol1) / (0.5 * hv_MinImageSize);

                                hv_MY1.Dispose();

                                hv_MY1 = (hv_TrackballCenterRow - hv_MRow1) / (0.5 * hv_MinImageSize);

                                hv_MX2.Dispose();

                                hv_MX2 = (hv_TrackballCenterCol - hv_MCol2) / (0.5 * hv_MinImageSize);

                                hv_MY2.Dispose();

                                hv_MY2 = (hv_TrackballCenterRow - hv_MRow2) / (0.5 * hv_MinImageSize);

                                //Compute the quaternion rotation that corresponds to the mouse
                                //movement
                                hv_RelQuaternion.Dispose();
                                Trackball(hv_MX1, hv_MY1, hv_MX2, hv_MY2, hv_VirtualTrackball, hv_TrackballSize,
                                    hv_SensFactor, out hv_RelQuaternion);
                                //Transform the quaternion to a rotation matrix
                                hv_HomMat3DRotRel.Dispose();
                                HOperatorSet.QuatToHomMat3d(hv_RelQuaternion, out hv_HomMat3DRotRel);
                                hv_PosesOut.Dispose();
                                hv_PosesOut = new HTuple();
                                if ((int)(new HTuple(hv_NumModels.TupleLessEqual(hv_MaxNumModels))) != 0)
                                {
                                    HTuple end_val226 = hv_NumModels - 1;
                                    HTuple step_val226 = 1;
                                    for (hv_Index = 0; hv_Index.Continue(end_val226, step_val226); hv_Index = hv_Index.TupleAdd(step_val226))
                                    {
                                        hv_PoseIn.Dispose();

                                        hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                            hv_Index * 7, (hv_Index * 7) + 6);

                                        if ((int)(hv_SelectedObjectOut.TupleSelect(hv_Index)) != 0)
                                        {
                                            //Transform the whole scene or selected object only
                                            hv_HomMat3DIn.Dispose();
                                            HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);
                                            {

                                                HTuple ExpTmpOutVar_0;
                                                HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                    0)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(1)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                    2)), out ExpTmpOutVar_0);
                                                hv_HomMat3DIn.Dispose();
                                                hv_HomMat3DIn = ExpTmpOutVar_0;
                                            }

                                            {

                                                HTuple ExpTmpOutVar_0;
                                                HOperatorSet.HomMat3dCompose(hv_HomMat3DRotRel, hv_HomMat3DIn,
                                                    out ExpTmpOutVar_0);
                                                hv_HomMat3DIn.Dispose();
                                                hv_HomMat3DIn = ExpTmpOutVar_0;
                                            }


                                            hv_HomMat3DOut.Dispose();
                                            HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                0), hv_TBCenter_COPY_INP_TMP.TupleSelect(1), hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                                2), out hv_HomMat3DOut);

                                            hv_PoseOut.Dispose();
                                            HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                            HOperatorSet.SetScene3dInstancePose(hv_Scene3D, hv_Index, hv_PoseOut);
                                        }
                                        else
                                        {
                                            hv_PoseOut.Dispose();
                                            hv_PoseOut = new HTuple(hv_PoseIn);
                                        }

                                        {
                                            HTuple
                                              ExpTmpLocalVar_PosesOut = hv_PosesOut.TupleConcat(
                                                hv_PoseOut);
                                            hv_PosesOut.Dispose();
                                            hv_PosesOut = ExpTmpLocalVar_PosesOut;
                                        }

                                    }
                                }
                                else
                                {
                                    hv_Indices.Dispose();
                                    HOperatorSet.TupleFind(hv_SelectedObjectOut, 1, out hv_Indices);
                                    hv_PoseIn.Dispose();

                                    hv_PoseIn = hv_PosesIn_COPY_INP_TMP.TupleSelectRange(
                                        (hv_Indices.TupleSelect(0)) * 7, ((hv_Indices.TupleSelect(0)) * 7) + 6);

                                    hv_HomMat3DIn.Dispose();
                                    HOperatorSet.PoseToHomMat3d(hv_PoseIn, out hv_HomMat3DIn);

                                    hv_HomMat3DInTmp1.Dispose();
                                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DIn, -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                        0)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(1)), -(hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                        2)), out hv_HomMat3DInTmp1);

                                    hv_HomMat3DInTmp.Dispose();
                                    HOperatorSet.HomMat3dCompose(hv_HomMat3DRotRel, hv_HomMat3DInTmp1,
                                        out hv_HomMat3DInTmp);

                                    hv_HomMat3DOut.Dispose();
                                    HOperatorSet.HomMat3dTranslate(hv_HomMat3DInTmp, hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                        0), hv_TBCenter_COPY_INP_TMP.TupleSelect(1), hv_TBCenter_COPY_INP_TMP.TupleSelect(
                                        2), out hv_HomMat3DOut);
                                }
                                hv_PoseOut.Dispose();
                                HOperatorSet.HomMat3dToPose(hv_HomMat3DOut, out hv_PoseOut);
                                hv_Sequence.Dispose();

                                hv_Sequence = HTuple.TupleGenSequence(
                                    0, (hv_NumModels * 7) - 1, 1);

                                hv_Mod.Dispose();
                                HOperatorSet.TupleMod(hv_Sequence, 7, out hv_Mod);
                                hv_SequenceReal.Dispose();

                                hv_SequenceReal = HTuple.TupleGenSequence(
                                    0, hv_NumModels - (1.0 / 7.0), 1.0 / 7.0);

                                hv_Sequence2Int.Dispose();

                                hv_Sequence2Int = hv_SequenceReal.TupleInt()
                                    ;

                                hv_Selected.Dispose();
                                HOperatorSet.TupleSelect(hv_SelectedObjectOut, hv_Sequence2Int, out hv_Selected);
                                hv_InvSelected.Dispose();

                                hv_InvSelected = 1 - hv_Selected;

                                hv_PosesOut.Dispose();
                                HOperatorSet.TupleSelect(hv_PoseOut, hv_Mod, out hv_PosesOut);
                                hv_PosesOut2.Dispose();

                                hv_PosesOut2 = (hv_PosesOut * hv_Selected) + (hv_PosesIn_COPY_INP_TMP * hv_InvSelected);

                                hv_PosesOut.Dispose();
                                hv_PosesOut = new HTuple(hv_PosesOut2);

                                HOperatorSet.SetScene3dInstancePose(hv_Scene3D, HTuple.TupleGenSequence(
                                    0, hv_NumModels - 1, 1), hv_PosesOut);


                                Dump_image_output(ho_BackgroundImage, hv_WindowHandleBuffer, hv_Scene3D,
                                    hv_AlphaOrig, hv_ObjectModel3DID, hv_GenParamName, hv_GenParamValue,
                                    hv_CamParam, hv_PosesOut, hv_ColorImage, hv_Title, hv_Information,
                                    hv_Labels, hv_VisualizeTB, "true", hv_TrackballCenterRow, hv_TrackballCenterCol,
                                    hv_TBSize_COPY_INP_TMP, hv_SelectedObjectOut, hv_WindowCenteredRotationOut,
                                    hv_TBCenter_COPY_INP_TMP);
                                ho_ImageDump.Dispose();
                                HOperatorSet.DumpWindowImage(out ho_ImageDump, hv_WindowHandleBuffer);
                                HDevWindowStack.SetActive(hv_WindowHandle);
                                if (HDevWindowStack.IsOpen())
                                {
                                    HOperatorSet.DispObj(ho_ImageDump, HDevWindowStack.GetActive());
                                }
                                //
                                hv_MRow1.Dispose();
                                hv_MRow1 = new HTuple(hv_Row_COPY_INP_TMP);
                                hv_MCol1.Dispose();
                                hv_MCol1 = new HTuple(hv_Column_COPY_INP_TMP);
                                hv_PosesIn_COPY_INP_TMP.Dispose();
                                hv_PosesIn_COPY_INP_TMP = new HTuple(hv_PosesOut);
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                //Keep waiting
                            }
                        }
                    }
                    hv_PosesOut.Dispose();
                    hv_PosesOut = new HTuple(hv_PosesIn_COPY_INP_TMP);
                }
                ho_ImageDump.Dispose();

                hv_Column_COPY_INP_TMP.Dispose();
                hv_PosesIn_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_TBCenter_COPY_INP_TMP.Dispose();
                hv_TBSize_COPY_INP_TMP.Dispose();
                hv_VisualizeTB.Dispose();
                hv_InvLog2.Dispose();
                hv_Seconds.Dispose();
                hv_ModelIndex.Dispose();
                hv_Exception1.Dispose();
                hv_HomMat3DIdentity.Dispose();
                hv_NumModels.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MinImageSize.Dispose();
                hv_TrackballRadiusPixel.Dispose();
                hv_TrackballCenterRow.Dispose();
                hv_TrackballCenterCol.Dispose();
                hv_NumChannels.Dispose();
                hv_ColorImage.Dispose();
                hv_BAnd.Dispose();
                hv_SensFactor.Dispose();
                hv_IsButtonTrans.Dispose();
                hv_IsButtonRot.Dispose();
                hv_IsButtonDist.Dispose();
                hv_MRow1.Dispose();
                hv_MCol1.Dispose();
                hv_ButtonLoop.Dispose();
                hv_MRow2.Dispose();
                hv_MCol2.Dispose();
                hv_PX.Dispose();
                hv_PY.Dispose();
                hv_PZ.Dispose();
                hv_QX1.Dispose();
                hv_QY1.Dispose();
                hv_QZ1.Dispose();
                hv_QX2.Dispose();
                hv_QY2.Dispose();
                hv_QZ2.Dispose();
                hv_Len.Dispose();
                hv_Dist.Dispose();
                hv_Translate.Dispose();
                hv_Index.Dispose();
                hv_PoseIn.Dispose();
                hv_HomMat3DIn.Dispose();
                hv_HomMat3DOut.Dispose();
                hv_PoseOut.Dispose();
                hv_Indices.Dispose();
                hv_Sequence.Dispose();
                hv_Mod.Dispose();
                hv_SequenceReal.Dispose();
                hv_Sequence2Int.Dispose();
                hv_Selected.Dispose();
                hv_InvSelected.Dispose();
                hv_Exception.Dispose();
                hv_DRow.Dispose();
                hv_TranslateZ.Dispose();
                hv_MX1.Dispose();
                hv_MY1.Dispose();
                hv_MX2.Dispose();
                hv_MY2.Dispose();
                hv_RelQuaternion.Dispose();
                hv_HomMat3DRotRel.Dispose();
                hv_HomMat3DInTmp1.Dispose();
                hv_HomMat3DInTmp.Dispose();
                hv_PosesOut2.Dispose();

                return;
            }
            catch (HalconException)
            {
                ho_ImageDump.Dispose();

                hv_Column_COPY_INP_TMP.Dispose();
                hv_PosesIn_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_TBCenter_COPY_INP_TMP.Dispose();
                hv_TBSize_COPY_INP_TMP.Dispose();
                hv_VisualizeTB.Dispose();
                hv_InvLog2.Dispose();
                hv_Seconds.Dispose();
                hv_ModelIndex.Dispose();
                hv_Exception1.Dispose();
                hv_HomMat3DIdentity.Dispose();
                hv_NumModels.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MinImageSize.Dispose();
                hv_TrackballRadiusPixel.Dispose();
                hv_TrackballCenterRow.Dispose();
                hv_TrackballCenterCol.Dispose();
                hv_NumChannels.Dispose();
                hv_ColorImage.Dispose();
                hv_BAnd.Dispose();
                hv_SensFactor.Dispose();
                hv_IsButtonTrans.Dispose();
                hv_IsButtonRot.Dispose();
                hv_IsButtonDist.Dispose();
                hv_MRow1.Dispose();
                hv_MCol1.Dispose();
                hv_ButtonLoop.Dispose();
                hv_MRow2.Dispose();
                hv_MCol2.Dispose();
                hv_PX.Dispose();
                hv_PY.Dispose();
                hv_PZ.Dispose();
                hv_QX1.Dispose();
                hv_QY1.Dispose();
                hv_QZ1.Dispose();
                hv_QX2.Dispose();
                hv_QY2.Dispose();
                hv_QZ2.Dispose();
                hv_Len.Dispose();
                hv_Dist.Dispose();
                hv_Translate.Dispose();
                hv_Index.Dispose();
                hv_PoseIn.Dispose();
                hv_HomMat3DIn.Dispose();
                hv_HomMat3DOut.Dispose();
                hv_PoseOut.Dispose();
                hv_Indices.Dispose();
                hv_Sequence.Dispose();
                hv_Mod.Dispose();
                hv_SequenceReal.Dispose();
                hv_Sequence2Int.Dispose();
                hv_Selected.Dispose();
                hv_InvSelected.Dispose();
                hv_Exception.Dispose();
                hv_DRow.Dispose();
                hv_TranslateZ.Dispose();
                hv_MX1.Dispose();
                hv_MY1.Dispose();
                hv_MX2.Dispose();
                hv_MY2.Dispose();
                hv_RelQuaternion.Dispose();
                hv_HomMat3DRotRel.Dispose();
                hv_HomMat3DInTmp1.Dispose();
                hv_HomMat3DInTmp.Dispose();
                hv_PosesOut2.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Chapter: Graphics / Output
        // Short Description: Compute the 3D rotation from the mouse movement 
        private static void Trackball(HTuple hv_MX1, HTuple hv_MY1, HTuple hv_MX2, HTuple hv_MY2,
            HTuple hv_VirtualTrackball, HTuple hv_TrackballSize, HTuple hv_SensFactor, out HTuple hv_QuatRotation)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_P1 = new HTuple(), hv_P2 = new HTuple();
            HTuple hv_RotAxis = new HTuple(), hv_D = new HTuple();
            HTuple hv_T = new HTuple(), hv_RotAngle = new HTuple();
            HTuple hv_Len = new HTuple();
            // Initialize local and output iconic variables 
            hv_QuatRotation = new HTuple();
            try
            {
                //
                //Compute the 3D rotation from the mouse movement
                //
                if ((int)((new HTuple(hv_MX1.TupleEqual(hv_MX2))).TupleAnd(new HTuple(hv_MY1.TupleEqual(
                    hv_MY2)))) != 0)
                {
                    hv_QuatRotation.Dispose();
                    hv_QuatRotation = new HTuple();
                    hv_QuatRotation[0] = 1;
                    hv_QuatRotation[1] = 0;
                    hv_QuatRotation[2] = 0;
                    hv_QuatRotation[3] = 0;

                    hv_P1.Dispose();
                    hv_P2.Dispose();
                    hv_RotAxis.Dispose();
                    hv_D.Dispose();
                    hv_T.Dispose();
                    hv_RotAngle.Dispose();
                    hv_Len.Dispose();

                    return;
                }
                //Project the image point onto the trackball
                hv_P1.Dispose();
                Project_point_on_trackball(hv_MX1, hv_MY1, hv_VirtualTrackball, hv_TrackballSize,
                    out hv_P1);
                hv_P2.Dispose();
                Project_point_on_trackball(hv_MX2, hv_MY2, hv_VirtualTrackball, hv_TrackballSize,
                    out hv_P2);
                //The cross product of the projected points defines the rotation axis
                hv_RotAxis.Dispose();
                Tuple_vector_cross_product(hv_P1, hv_P2, out hv_RotAxis);
                //Compute the rotation angle
                hv_D.Dispose();

                hv_D = hv_P2 - hv_P1;

                hv_T.Dispose();

                hv_T = (((((hv_D * hv_D)).TupleSum()
                    )).TupleSqrt()) / (2.0 * hv_TrackballSize);

                if ((int)(new HTuple(hv_T.TupleGreater(1.0))) != 0)
                {
                    hv_T.Dispose();
                    hv_T = 1.0;
                }
                if ((int)(new HTuple(hv_T.TupleLess(-1.0))) != 0)
                {
                    hv_T.Dispose();
                    hv_T = -1.0;
                }
                hv_RotAngle.Dispose();

                hv_RotAngle = (2.0 * (hv_T.TupleAsin()
                    )) * hv_SensFactor;

                hv_Len.Dispose();

                hv_Len = ((((hv_RotAxis * hv_RotAxis)).TupleSum()
                    )).TupleSqrt();

                if ((int)(new HTuple(hv_Len.TupleGreater(0.0))) != 0)
                {

                    {
                        HTuple
                          ExpTmpLocalVar_RotAxis = hv_RotAxis / hv_Len;
                        hv_RotAxis.Dispose();
                        hv_RotAxis = ExpTmpLocalVar_RotAxis;
                    }

                }

                hv_QuatRotation.Dispose();
                HOperatorSet.AxisAngleToQuat(hv_RotAxis.TupleSelect(0), hv_RotAxis.TupleSelect(
                    1), hv_RotAxis.TupleSelect(2), hv_RotAngle, out hv_QuatRotation);


                hv_P1.Dispose();
                hv_P2.Dispose();
                hv_RotAxis.Dispose();
                hv_D.Dispose();
                hv_T.Dispose();
                hv_RotAngle.Dispose();
                hv_Len.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_P1.Dispose();
                hv_P2.Dispose();
                hv_RotAxis.Dispose();
                hv_D.Dispose();
                hv_T.Dispose();
                hv_RotAngle.Dispose();
                hv_Len.Dispose();

                //throw HDevExpDefaultException;
            }
        }



        // Chapter: Graphics / Output
        // Short Description: Project an image point onto the trackball 
        private static void Project_point_on_trackball(HTuple hv_X, HTuple hv_Y, HTuple hv_VirtualTrackball,
            HTuple hv_TrackballSize, out HTuple hv_V)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_R = new HTuple(), hv_XP = new HTuple();
            HTuple hv_YP = new HTuple(), hv_ZP = new HTuple();
            // Initialize local and output iconic variables 
            hv_V = new HTuple();
            try
            {
                //
                if ((int)(new HTuple(hv_VirtualTrackball.TupleEqual("shoemake"))) != 0)
                {
                    //Virtual Trackball according to Shoemake
                    hv_R.Dispose();

                    hv_R = (((hv_X * hv_X) + (hv_Y * hv_Y))).TupleSqrt()
                        ;

                    if ((int)(new HTuple(hv_R.TupleLessEqual(hv_TrackballSize))) != 0)
                    {
                        hv_XP.Dispose();
                        hv_XP = new HTuple(hv_X);
                        hv_YP.Dispose();
                        hv_YP = new HTuple(hv_Y);
                        hv_ZP.Dispose();

                        hv_ZP = (((hv_TrackballSize * hv_TrackballSize) - (hv_R * hv_R))).TupleSqrt()
                            ;

                    }
                    else
                    {
                        hv_XP.Dispose();

                        hv_XP = (hv_X * hv_TrackballSize) / hv_R;

                        hv_YP.Dispose();

                        hv_YP = (hv_Y * hv_TrackballSize) / hv_R;

                        hv_ZP.Dispose();
                        hv_ZP = 0;
                    }
                }
                else
                {
                    //Virtual Trackball according to Bell
                    hv_R.Dispose();

                    hv_R = (((hv_X * hv_X) + (hv_Y * hv_Y))).TupleSqrt()
                        ;

                    if ((int)(new HTuple(hv_R.TupleLessEqual(hv_TrackballSize * 0.70710678))) != 0)
                    {
                        hv_XP.Dispose();
                        hv_XP = new HTuple(hv_X);
                        hv_YP.Dispose();
                        hv_YP = new HTuple(hv_Y);
                        hv_ZP.Dispose();

                        hv_ZP = (((hv_TrackballSize * hv_TrackballSize) - (hv_R * hv_R))).TupleSqrt()
                            ;

                    }
                    else
                    {
                        hv_XP.Dispose();
                        hv_XP = new HTuple(hv_X);
                        hv_YP.Dispose();
                        hv_YP = new HTuple(hv_Y);
                        hv_ZP.Dispose();

                        hv_ZP = ((0.6 * hv_TrackballSize) * hv_TrackballSize) / hv_R;

                    }
                }
                hv_V.Dispose();

                hv_V = new HTuple();
                hv_V = hv_V.TupleConcat(hv_XP, hv_YP, hv_ZP);


                hv_R.Dispose();
                hv_XP.Dispose();
                hv_YP.Dispose();
                hv_ZP.Dispose();

                return;
            }
            catch (HalconException)
            {

                hv_R.Dispose();
                hv_XP.Dispose();
                hv_YP.Dispose();
                hv_ZP.Dispose();

                //throw HDevExpDefaultException;
            }
        }


        // Chapter: Tuple / Arithmetic
        // Short Description: Calculate the cross product of two vectors of length 3. 
        private static void Tuple_vector_cross_product(HTuple hv_V1, HTuple hv_V2, out HTuple hv_VC)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_VC = new HTuple();
            //The caller must ensure that the length of both input vectors is 3
            hv_VC.Dispose();

            hv_VC = ((hv_V1.TupleSelect(
                1)) * (hv_V2.TupleSelect(2))) - ((hv_V1.TupleSelect(2)) * (hv_V2.TupleSelect(1)));


            {
                HTuple
                  ExpTmpLocalVar_VC = hv_VC.TupleConcat(
                    ((hv_V1.TupleSelect(2)) * (hv_V2.TupleSelect(0))) - ((hv_V1.TupleSelect(0)) * (hv_V2.TupleSelect(
                    2))));
                hv_VC.Dispose();
                hv_VC = ExpTmpLocalVar_VC;
            }


            {
                HTuple
                  ExpTmpLocalVar_VC = hv_VC.TupleConcat(
                    ((hv_V1.TupleSelect(0)) * (hv_V2.TupleSelect(1))) - ((hv_V1.TupleSelect(1)) * (hv_V2.TupleSelect(
                    0))));
                hv_VC.Dispose();
                hv_VC = ExpTmpLocalVar_VC;
            }


            return;
        }





    }

}
