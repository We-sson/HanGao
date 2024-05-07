using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using Point = System.Windows.Point;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Shape_Mode_SDK
    {
        public Halcon_Shape_Mode_SDK()
        {

        }

        public ObservableCollection<Vision_Create_Model_Drawing_Model> Drawing_Data_List { get; set; } = new ObservableCollection<Vision_Create_Model_Drawing_Model>();


        /// <summary>
        /// 模型文件列表
        /// </summary>
        public ObservableCollection<Shape_Mode_File_Model> Shape_Mode_File_Model_List { set; get; } = new ObservableCollection<Shape_Mode_File_Model>();


        public Shape_Mode_File_Model? Selected_Shape_Model { set; get; } = null;
        /// <summary>
        /// 全部工艺模型合并
        /// </summary>
        public HXLDCont ALL_Models_XLD { set; get; } = new();

        /// <summary>
        /// 模型ID
        /// </summary>
        public HTuple Shape_ID = new();

        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Create_Shape_ModelXld { set; get; } = new Create_Shape_Based_ModelXld();

        /// <summary>
        /// 全部模型三维原点
        /// </summary>
        public Point_Model Plane_In_BasePose { set; get; } = new Point_Model() { };


        /// <summary>
        /// 相机拍摄位置
        /// </summary>
        public Point_Model Tool_In_BasePos { set; get; } = new Point_Model() { };

        /// <summary>
        /// 标定路径在Base坐标下数据
        /// </summary>
        public ObservableCollection<Point_Model> Calib_PathInBase_List { set; get; } = new();

        /// <summary>
        /// 用户选择数据显示
        /// </summary>
        public Point_Model? Selected_Calib_PathInBase { set; get; }

        /// <summary>
        /// 二维平面原点位置
        /// </summary>
        public Point_Model Model_2D_Origin { set; get; } = new Point_Model();

        /// <summary>
        /// 图像校正变量
        /// </summary>


        public static HImage Image_Rectified = new HImage();

        //public static HImage Image_Rectified
        //{
        //    get { return _Image_Rectified; }
        //    set
        //    {
        //        _Image_Rectified = value;
        //        //if (value != null && value.IsInitialized())
        //        //{

        //        //    //_Image_Rectified?.Dispose();
        //        //    _Image_Rectified = value.CopyObj(1, -1);
        //        //}
        //    }
        //}


        /// <summary>
        /// 在手动步骤操作自动校正开关
        /// </summary>
        //public bool Auto_Image_Rectified { set; get; } = true;


        /// <summary>
        /// 强制执行图像校正
        /// </summary>
        //public bool Compulsory_Image_Rectified { set; get; } = false;


        /// <summary>
        /// 图像校正坐标转换系数
        /// </summary>
        public double Image_Rectified_Ratio { set; get; } = 0;

        /// <summary>
        /// 图像校正尺寸
        /// </summary>
        public int Image_Rectified_Width { set; get; } = 0;

        /// <summary>
        /// 图像校正尺寸
        /// </summary>
        public int Image_Rectified_Height { set; get; } = 0;


        /// <summary>
        /// 模型原地设置类型
        /// </summary>
        public Model_2D_Origin_Type_Enum Model_2D_Origin_Type { set; get; } = Model_2D_Origin_Type_Enum.Tool_In_Base;

        /// <summary>
        /// 机器人类型
        /// </summary>
        public Robot_Type_Enum Robot_Type { set; get; } = Robot_Type_Enum.通用;


        //private string Shape_Save_Path { set; get; } 

        private string Save_Path { set; get; } = Environment.CurrentDirectory + "\\ShapeModel";

        /// <summary>
        /// 用户点击位置
        /// </summary>
        public Point Chick_Position { set; get; } = new Point(0, 0);



        /// <summary>
        /// 鼠标当前灰度值
        /// </summary>
        public int Chick_Position_Gray { set; get; } = -1;

        /// <summary>
        /// 保存单次生产手动描述特征点
        /// </summary>
        public Vision_Create_Model_Drawing_Model User_Drawing_Data { set; get; } = new Vision_Create_Model_Drawing_Model();



        //public Find_Shape_Based_ModelXld Find_Shape_Model { get; set; } = new Find_Shape_Based_ModelXld();




        private Match_Model_Craft_Type_Enum _Match_Model_Craft_Type = Match_Model_Craft_Type_Enum.请选择模型工艺;

        /// <summary>
        /// 模型工艺类型
        /// </summary>
        public Match_Model_Craft_Type_Enum Match_Model_Craft_Type
        {
            get { return _Match_Model_Craft_Type; }
            set { _Match_Model_Craft_Type = value; Init_Crafe_Type_List(value); }
        }


        public HImage Shape_Match_Map(HImage _image, bool _Auto_Image_Rectified, bool _Compulsory_Image_Rectified = false)
        {
            HImage Res_Image = new HImage();
            HImage Map_Image = new HImage();


            /// <summary>
            /// 如果Auto_Image_Rectified属性为false，则直接返回_image
            /// </summary>
            if (!_Auto_Image_Rectified)
            {
                return _image;
            }
            _image.GetImageSize(out HTuple _witch, out HTuple _height);
            Selected_Shape_Model.ThrowIfNull("请选择需要校正的模型号！");

            //检查校正图像是否存在
            if (Selected_Shape_Model.Shape_Image_Rectified.IsInitialized())
            {
                //检查图像是否符合校正尺寸
                _image.GetImageSize(out HTuple _w, out HTuple _h);


                if (_Compulsory_Image_Rectified)
                {

                    if (_w != Selected_Shape_Model.Shape_Image_Rectified_Width || _h != Selected_Shape_Model.Shape_Image_Rectified_Heigth)
                    {
                        _image = _image.ZoomImageSize(Selected_Shape_Model.Shape_Image_Rectified_Width, Selected_Shape_Model.Shape_Image_Rectified_Heigth, "bilinear");
                    }
                    //拷贝Map图进行校正
                    Map_Image = Selected_Shape_Model.Shape_Image_Rectified.CopyImage();
                    Res_Image = _image.MapImage(Map_Image);
                    Map_Image.Dispose();
                    _image.Dispose();

                }
                else
                {

                    if (_w == Selected_Shape_Model.Shape_Image_Rectified_Width || _h == Selected_Shape_Model.Shape_Image_Rectified_Heigth)
                    {

                        //拷贝Map图进行校正
                        Map_Image = Selected_Shape_Model.Shape_Image_Rectified.CopyImage();
                        Res_Image = _image.MapImage(Map_Image);
                        Map_Image.Dispose();
                        _image.Dispose();


                    }

                    else
                    {
                        throw new Exception($"图像尺寸：{_w} X {_h} 校正的图像与模型文件中的尺寸：{Selected_Shape_Model.Shape_Image_Rectified_Width} X {Selected_Shape_Model.Shape_Image_Rectified_Heigth} 不匹配，可以强行缩放图像尺寸校正！");


                    }

                }






            }
            else
            {
                throw new Exception($"{Selected_Shape_Model.ID}号模型内未存在Map校正图像，请检查模型可靠性！");

            }




            return Res_Image;
        }

        public List<HXLDCont> Get_Match_Path_XLD()
        {
            List<HXLDCont> _Result_XLD = new List<HXLDCont>();








            return _Result_XLD;
        }




        public Find_Shape_Results_Model Find_Shape_Model_Results(Find_Shape_Based_ModelXld Find_Shape_Model, HImage _image, Halcon_Camera_Calibration_Parameters_Model? _camera_Param = null, Point_Model? _toolinCamera = null)
        {

            //初始化匹配类型
            HShapeModel _ShapeModel = new();
            HDeformableModel _DeformableModel = new();
            //HNCCModel _NccModel = new HNCCModel();
            Find_Shape_Results_Model _Results = new();
            HHomMat2D _HomMat3D = new();


            _image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized());


            //提取模型库中的文件
            Shape_Mode_File_Model? _Model = Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Find_Shape_Model.FInd_ID);

            _Model.ThrowIfNull(Find_Shape_Model.FInd_ID + "号模型，无法在模型库中找到！");

            Find_Shape_Model.Shape_Based_Model.Throw(@Find_Shape_Model.FInd_ID + "号模型，与模型库中的" + _Model.Shape_Model + "模型类型不一致，无法进行匹配！").IfNotEquals(_Model.Shape_Model);



            //进行图像校正后识别
            //_image = _image.MapImage(_Model.Shape_Image_Rectified);


            switch (Find_Shape_Model.Shape_Based_Model)
            {
                case Shape_Based_Model_Enum.shape_model:


                    break;
                case Shape_Based_Model_Enum.planar_deformable_model:


                    break;
                case Shape_Based_Model_Enum.local_deformable_model:


                    break;
                case Shape_Based_Model_Enum.Scale_model:


                    break;
                case Shape_Based_Model_Enum.Aniso_Model:



                    break;
                case Shape_Based_Model_Enum.Ncc_Model:


                    HTuple _row = new();
                    HTuple _column = new();
                    HTuple _angle = new();
                    HTuple _score = new();
                    HHomMat2D _HomMat2D = new();
                    List<HNCCModel> _NccModel = new();
                    HXLDCont _Origin_XLD = new();
                    _Origin_XLD.GenCrossContourXld(new HTuple(0), new HTuple(0), 100, 0.78);

                    ///查找匹配模型文件里模型
                    for (int i = 0; i < _Model.Shape_Handle_List.Count; i++)
                    {
                        //计时
                        DateTime _Run = DateTime.Now;
                        HNCCModel _ncc = new(_Model.Shape_Handle_List[i].H);
                        HRegion _Ncc_Region = new();
                        HXLDCont _Ncc_Xld = new();
                        _ncc.SetNccModelParam("timeout", Find_Shape_Model.Time_Out);
                        //查找模型
                        _ncc.FindNccModel(
                        _image,
                        Find_Shape_Model.AngleStart,
                        Find_Shape_Model.AngleExtent,
                        Find_Shape_Model.MinScore,
                        Find_Shape_Model.NumMatches,
                        Find_Shape_Model.MaxOverlap,
                        Find_Shape_Model.NCC_SubPixel.ToString().ToLower(),
                         Find_Shape_Model.NumLevels,
                        out _row,
                        out _column,
                        out _angle,
                        out _score);

                        ///查找成功添加数据
                        if (_score > 0)
                        {
                            //(col = x, row = y)
                            _Results.Find_Score.Add(_score);
                            _Results.Find_Column.Add(_column);
                            _Results.Find_Row.Add(_row);
                            _Results.Find_Angle.Add(_angle);
                            ///查看细节部分
                            _Ncc_Region = _ncc.GetNccModelRegion();
                            _Ncc_Xld = _Ncc_Region.GenContourRegionXld("border_holes");
                            _Ncc_Xld = _Ncc_Xld.AffineTransContourXld(_HomMat2D);
                        }
                        else
                        {
                            _Results.Find_Score.Add(0);
                            _Results.Find_Column.Add(0);
                            _Results.Find_Row.Add(0);
                            _Results.Find_Angle.Add(0);

                        }



                        ///记录时间
                        _Results.Find_Time.Add((DateTime.Now - _Run).TotalSeconds);



                    }







                    ///根据工艺分析匹配文件对应计算结果位置
                    switch (_Model.Shape_Craft)
                    {
                        case Match_Model_Craft_Type_Enum.请选择模型工艺:

                            throw new Exception("创建的模型工艺文件，工艺未选择！");


                        case Match_Model_Craft_Type_Enum.焊接盆胆R角 or Match_Model_Craft_Type_Enum.焊接面板围边:

                            ///把匹配结果计算偏移特征
                            for (int i = 0; i < _Results.Find_Score.Count; i++)
                            {


                                HXLDCont _Xld = new();
                                HHomMat2D _results_HomMat2D = new();


                                if (_Results.Find_Score[i] > 0)
                                {
                                    //(col = x, row = y)

                                    ///偏移原点模型文件到匹配位置
                                    _results_HomMat2D.VectorAngleToRigid(0, 0, 0, _Results.Find_Row[i], _Results.Find_Column[i], _Results.Find_Angle[i]);

                                    ///偏移模型库模型
                                    _Xld = _Model.Shape_XLD_Handle_List[i];
                                    _Xld = _Model.Shape_XLD_Handle_List[i].AffineTransContourXld(_results_HomMat2D);
                                    //偏移原点XLD

                                    //识别成功保存结果
                                    _Results.Results_HomMat2D_List.Add(_results_HomMat2D);
                                    _Results.Results_HXLD_List.Add(_Xld);
                                    //_Results.Results_HXLD_List.Add(_Origin_XLD);

                                }

                            }
                            ///全部特征识别成功后计算结果位置，非全部识别输出坐标0
                            if (_Results.Find_Score.Where((_) => _ != 0).ToList().Count != 0)
                            {
                                //(col = x, row = y)
                                ///转换相机坐标，必须参数输入，否则输出图像像素坐标
                                if (_camera_Param != null && Tool_In_BasePos.HPose != new Point_Model().HPose && _Model.Shape_PlaneInBase_Pos.HPose != new Point_Model().HPose && _Model.Shape_Image_Rectified_Ratio != 0 && _toolinCamera != null)
                                {



                                    //*Col = x, Row = y.
                                    //转换平面在相机的坐标,创建平面Z方向远离相机
                                    Point_Model BaseInToolPose = new(Tool_In_BasePos.HPose.PoseInvert());
                                    Point_Model BaseInCamPose = new(_toolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                                    Point_Model PlaneInCamPose = new(BaseInCamPose.HPose.PoseCompose(_Model.Shape_PlaneInBase_Pos.HPose));



                                    //按匹配平面位置计算位置
                                    HHomMat3D _Model_Plane_Pos_Mat3D = PlaneInCamPose.HPose.PoseToHomMat3d();

                                    ///转换位置点在相机坐标下的位置,ncc特殊，需要手工减去，图像原点位置 x=col,y=row
                                    //double _qx = _Model_Plane_Pos_Mat3D.AffineTransPoint3d((_Results.Find_Row[0] - _Model.Shape_Model_2D_Origin.Y) * _Model.Shape_Image_Rectified_Ratio, 0, 0, out double _qy, out double _qz);

                                    ///计算模型在平面上的偏移值
                                    var _X = ((_Results.Find_Column[0] - _Model.Shape_Model_2D_Origin.Y) * _Model.Shape_Image_Rectified_Ratio) * 1000;
                                    var _Y = ((_Results.Find_Row[0] - _Model.Shape_Model_2D_Origin.X) * _Model.Shape_Image_Rectified_Ratio) * 1000;

                                    var RRz = -((180 / Math.PI) * _Results.Find_Angle[0]);
                                    ///生产模型在平面的位置,
                                    Point_Model _ResultInPlanPose = new()
                                    {
                                        X = _X,
                                        Y = _Y,
                                        Rz = -((180 / Math.PI) * _Results.Find_Angle[0]),
                                        HType = _Model.Shape_PlaneInBase_Pos.HType
                                    };



                                    ///把计算结果的用户坐标合并便宜结果
                                    Point_Model _BaseInResult = new(_Model.Shape_PlaneInBase_Pos.HPose.PoseCompose(_ResultInPlanPose.HPose));

                                    ///偏移原点位置
                                    //HHomMat3D _ResultInBase_HomMat3D = _Model.Shape_PlaneInBase_Pos.HPose.PoseToHomMat3d();
                                    //_ResultInBase_HomMat3D = _ResultInBase_HomMat3D.HomMat3dRotate(_Results.Find_Angle[0], "z", _Model.Shape_PlaneInBase_Pos.HPose[0], _Model.Shape_PlaneInBase_Pos.HPose[1], _Model.Shape_PlaneInBase_Pos.HPose[2]);
                                    //_ResultInBase_HomMat3D = _ResultInBase_HomMat3D.HomMat3dTranslate(_ResultInPlanPose.HPose[0], _ResultInPlanPose.HPose[1], new HTuple(0));
                                    //Point_Model _BaseInResult = new Point_Model(_ResultInBase_HomMat3D.HomMat3dToPose());
                                    //Point_Model _ResultInBase1= new Point_Model(_BaseInResult.HPose.PoseInvert());



                                    ///初始化结果坐标
                                    _Results.Results_PathInBase_Pos.Clear();

                                    //计算实际坐标下位置
                                    foreach (var PathInBase_Pos in _Model.Shape_Calibration_PathInBase_List)
                                    {
                                        ///计算路径位置点在标定的用户坐标下
                                        Point_Model TcpInPlan = new Point_Model(_Model.Shape_PlaneInBase_Pos.HPose.PoseInvert().PoseCompose(PathInBase_Pos.HPose));


                                        Point_Model PathInbase = new Point_Model(new Point_Model(_BaseInResult.HPose.PoseCompose(TcpInPlan.HPose)));
                                        ///把位置点在结果用户坐标下转换回Base坐标下
                                        _Results.Results_PathInBase_Pos.Add(new Point_Model(PathInbase.X, PathInbase.Y, PathInbase.Z, PathInbase.Rx, PathInbase.Ry, PathInbase.Rz, _Model.Shape_Robot_Type));


                                    }




                                    ///计算出模型到相机的位置
                                    _Results.Results_ModelInCam_Pos = new(PlaneInCamPose.HPose.PoseCompose(_ResultInPlanPose.HPose));

                                    ///计算出结果位置
                                    //_Results.Results_ModelInCam_Pos = new Point_Model() { X = _qx * 1000, Y = _qy * 1000, Z = _qz * 1000, Rx = 0, Ry = 0, Rz = 0, HType =  Halcon_Pose_Type_Enum.abg };
                                    _Results.Results_Image_Pos = new Point_Model() { X = _Results.Find_Column[0], Y = _Results.Find_Row[0], Z = 0, Rz = (180 / Math.PI) * _Results.Find_Angle[0] };



                                    // pose_invert(ToolInBasePose, BaseInToolPose)
                                    //pose_compose(ToolInCamPose, BaseInToolPose, BaseInCamPose)
                                    //pose_invert(BaseInCamPose, CamInBasePose)
                                    //pose_compose(CamInBasePose, ModelInCamPose, ModelInBasePose)


                                    //Point_Model BaseInToolPose = new Point_Model(Tool_In_BasePos.HPose.PoseInvert());
                                    //Point_Model BaseInCamPose = new Point_Model(_toolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                                    //Point_Model CamInBasePose = new Point_Model(BaseInCamPose.HPose.PoseInvert());


                                    ///计算出模型在Base原点位置坐标下位置
                                    Point_Model CamInToolPose = new(_toolinCamera.HPose.PoseInvert());
                                    Point_Model CamInBasePose = new(Tool_In_BasePos.HPose.PoseCompose(CamInToolPose.HPose));
                                    Point_Model ModelInBasePose = new(CamInBasePose.HPose.PoseCompose(_Results.Results_ModelInCam_Pos.HPose));
                                    ///计算出模型在bace坐标下
                                    ///
                                    //   Point_Model PathTcpInBase = new () { X = 46.89, Y = -780.67, Z = 222.34 };
                                    //   Point_Model BaseInCamPos = new(CamInBasePose.HPose.PoseInvert());
                                    //   HHomMat3D BaseInCamPos_HomMat3D= BaseInCamPos.HPose.PoseToHomMat3d();
                                    //double _PB_x=   BaseInCamPos_HomMat3D.AffineTransPoint3d(PathTcpInBase.X, PathTcpInBase.Y, PathTcpInBase.Z, out double _PB_y, out double _PB_z);

                                    //   _camera_Param.HCamPar.Project3dPoint(_PB_x, _PB_y, _PB_z, out HTuple _Cam_row,  out HTuple _Cam_col);




                                    _Results.Results_ModelInBase_Pos = new(ModelInBasePose);
                                    _Results.Results_PlanOffset_Pos = new(_BaseInResult);
                                    _Results.Find_Shape_Results_State = Find_Shape_Results_State_Enum.Match_Success;


                                }
                                else
                                {

                                    //计算条件未全，出图像像素坐标
                                    _Results.Results_Image_Pos = new Point_Model() { X = _Results.Find_Column[0], Y = _Results.Find_Row[0], Z = 0, Rz = (180 / Math.PI) * _Results.Find_Angle[0] };
                                    _Results.Find_Shape_Results_State = Find_Shape_Results_State_Enum.Match_Failed;

                                }
                            }
                            else
                            {
                                //识别成功保存结果
                                _Results.Results_Image_Pos = new Point_Model() { X = _Results.Find_Column[0], Y = _Results.Find_Row[0], Z = 0, Rz = (180 / Math.PI) * _Results.Find_Angle[0] };
                                _Results.Find_Shape_Results_State = Find_Shape_Results_State_Enum.Match_Failed;

                            }


                            break;


                    }

                    ///把所有计算后图像合并一起显示
                    _Results.HXLD_Results_All.GenEmptyObj();
                    foreach (var _xld in _Results.Results_HXLD_List)
                    {
                        _Results.HXLD_Results_All = _Results.HXLD_Results_All.ConcatObj(_xld);
                    }



                    ///显示匹配相关信息结果
                    _Results.Set_Results_Data_List();



                    break;


            }







            return _Results;
        }



        public void Shape_Model_Crafe_Processing(Find_Shape_Results_Model _Results)
        {










        }



        /// <summary>
        /// 加载工艺模型加载集合中
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Load_Crafe_XLD_ToList()
        {
            //更加模型工艺选择
            switch (Match_Model_Craft_Type)
            {
                case Match_Model_Craft_Type_Enum.请选择模型工艺:

                    throw new Exception("请选择需要添加模型工艺！");


                case Match_Model_Craft_Type_Enum.焊接盆胆R角:

                    //根据工艺模型步骤
                    switch (User_Drawing_Data.Craft_Type_Enum)
                    {
                        case Sink_Basin_R_Welding.模型原点位置:
                            Drawing_Data_List[0] = User_Drawing_Data;

                            break;
                        case Sink_Basin_R_Welding.R角中线轮廓:
                            Drawing_Data_List[1] = User_Drawing_Data;




                            break;
                        case Sink_Basin_R_Welding.盆胆左侧线轮廓:
                            Drawing_Data_List[2] = User_Drawing_Data;




                            break;
                        case Sink_Basin_R_Welding.盆胆右侧线轮廓:
                            Drawing_Data_List[3] = User_Drawing_Data;


                            break;

                        default:
                            break;
                    }

                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:

                    //根据工艺模型步骤

                    switch (User_Drawing_Data.Craft_Type_Enum)
                    {
                        case Sink_Basin_R_Welding.模型原点位置:
                            Drawing_Data_List[0] = User_Drawing_Data;

                            break;
                        case Sink_Board_R_Welding.面板横直线轮廓:
                            Drawing_Data_List[1] = User_Drawing_Data;


                            break;
                        case Sink_Board_R_Welding.面板圆弧轮廓:

                            Drawing_Data_List[2] = User_Drawing_Data;

                            break;
                        case Sink_Board_R_Welding.面板竖直线轮廓:
                            Drawing_Data_List[3] = User_Drawing_Data;


                            break;
                        default:
                            break;
                    }

                    break;

            }

            //集合创建模型
            Group_Model_XLD();

        }

        /// <summary>
        ///  切换工艺更改工艺轮廓特征
        /// </summary>
        /// <param name="_Craft"></param>
        public void Init_Crafe_Type_List(Match_Model_Craft_Type_Enum _Craft)
        {

            switch (_Craft)
            {
                case Match_Model_Craft_Type_Enum.焊接盆胆R角:

                    Drawing_Data_List.Clear();
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.模型原点位置 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.R角中线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.盆胆左侧线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.盆胆右侧线轮廓 });
                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:
                    Drawing_Data_List.Clear();
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.模型原点位置 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板横直线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板圆弧轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Board_R_Welding.面板竖直线轮廓 });

                    break;
                case Match_Model_Craft_Type_Enum.请选择模型工艺:
                    Drawing_Data_List.Clear();

                    break;

            }



        }




        /// <summary>
        /// 获得位置点的灰度值
        /// </summary>
        /// <param name="_Image"></param>
        public void Get_Pos_Gray(HImage _Image)
        {

            try
            {


                Chick_Position_Gray = _Image.GetGrayval(Chick_Position.X, Chick_Position.Y);


            }
            catch (Exception)
            {
                Chick_Position_Gray = -1;

            }

        }



        /// <summary>
        /// 所有xld类型集合一起
        /// </summary>
        /// <param name="_All_XLD"></param>
        /// <param name="_Window"></param>
        /// <param name="_XLD_List"></param>
        /// <returns></returns>
        public void Group_Model_XLD()
        {

            try
            {
                ALL_Models_XLD = new HXLDCont();
                ALL_Models_XLD.GenEmptyObj();

                if (Drawing_Data_List.Count > 1)
                {
                    foreach (var _Xld in Drawing_Data_List)
                    {
                        if (_Xld.Model_XLD.IsInitialized())
                        {
                            //集合一起
                            ALL_Models_XLD = ALL_Models_XLD.ConcatObj(_Xld.Model_XLD);

                        }
                    }


                }

            }
            catch (Exception _e)
            {
                throw new Exception("合并模型工艺轮廓失败！原因：" + _e.Message);

            }

        }



        /// <summary>
        /// 模型文件集合
        /// </summary>
        public List<FileInfo> File_ModelIDS { set; get; } = new List<FileInfo>();




        /// <summary>
        /// 获得匹配模型区域
        /// </summary>
        /// <param name="_Shape_Model"></param>
        /// <param name="_ShapeHandl"></param>
        /// <returns></returns>
        public HObject Get_Shape_Model_Image(Shape_Based_Model_Enum _Shape_Model, HTuple _ShapeHandl)
        {
            HObject _Image = new HObject();


            switch (_Shape_Model)
            {
                case Shape_Based_Model_Enum.shape_model:
                    break;
                case Shape_Based_Model_Enum.planar_deformable_model:
                    break;
                case Shape_Based_Model_Enum.local_deformable_model:
                    break;
                case Shape_Based_Model_Enum.Scale_model:
                    break;
                case Shape_Based_Model_Enum.Aniso_Model:
                    break;
                case Shape_Based_Model_Enum.Ncc_Model:

                    HNCCModel _NccModel = new HNCCModel(_ShapeHandl.H);

                    _Image = new HObject(_NccModel.GetNccModelRegion());

                    break;


            }

            return _Image;

        }



        /// <summary>
        /// 显示匹配模型对象
        /// </summary>
        /// <param name="_HObject_Type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HObject Show_Shape_Model_HObject(Shape_HObject_Type_Enum _HObject_Type)
        {
            HObject _HObject = new HObject();

            Selected_Shape_Model.ThrowIfNull("请选择显示模型号！");



            switch (_HObject_Type)
            {
                case Shape_HObject_Type_Enum.Shape_Handle:

                    Selected_Shape_Model.Selected_Shape_Handle.ThrowIfNull("没有选择模型对象！");
                    if (Selected_Shape_Model.Selected_Shape_Handle.Length == 0)
                    {
                        throw new Exception(Selected_Shape_Model.ID + "号模型加载显示详情失败！");
                    }

                    _HObject = Get_Shape_Model_Image(Selected_Shape_Model.Shape_Model, Selected_Shape_Model.Selected_Shape_Handle);


                    break;
                case Shape_HObject_Type_Enum.Shape_XLD:

                    Selected_Shape_Model.Selected_Shape_XLD_Handle.ThrowIfNull("没有选择XLD模型对象！");
                    if (!Selected_Shape_Model.Selected_Shape_XLD_Handle.IsInitialized())
                    {
                        throw new Exception(Selected_Shape_Model.ID + "号模型XLD变量不存在！");
                    }
                    _HObject = Selected_Shape_Model.Selected_Shape_XLD_Handle;


                    break;
                case Shape_HObject_Type_Enum.Shape_Image_Rectified:


                    //判断图像校正是否存在
                    if (!Selected_Shape_Model.Shape_Image_Rectified.IsInitialized())
                    {
                        throw new Exception(Selected_Shape_Model.ID + "号模型校正图变量不存在！");
                    }

                    _HObject = new HObject(Selected_Shape_Model.Shape_Image_Rectified);
                    break;

            }




            return _HObject;
        }


        /// <summary>
        /// 清楚模型集合中内存
        /// </summary>
        public void Clear_ShapeModel_Lsit()
        {

            foreach (var _List in Shape_Mode_File_Model_List)
            {
                _List.Shape_Image_Rectified.Dispose();
                foreach (var item in _List.Shape_Handle_List)
                {
                    HNCCModel _ncc = new(item.H);
                    _ncc.ClearNccModel();
                    _ncc.ClearHandle();
                    //HOperatorSet.ClearShapeModel(item);
                    item.Dispose();
                }
                foreach (var item in _List.Shape_XLD_Handle_List)
                {
                    
                    item.Dispose();
                }

            }
            Shape_Mode_File_Model_List.Clear();
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForFullGCApproach();
        }

        /// <summary>
        /// 获得本地匹配模型文件对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Shape_Mode_File_Model> Get_ShapeModel()
        {


            List<Shape_Mode_File_Model> _ShapeModel = new List<Shape_Mode_File_Model>();


            try
            {
                //获得保存模板名称
                List<FileInfo> _Model_Location = Get_ShapeModel_Path();
                foreach (var _path in _Model_Location)
                {
                    try
                    {

                        _ShapeModel.Add(Get_Shape_HDict(_path));

                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"模型文件：{_path.Name}  文件读取错误，跳过该文件 !", "标定提示", MessageBoxButton.OK, MessageBoxImage.Error);


                    }

                }
                //排序赋值
                Application.Current.Dispatcher.BeginInvoke(() =>
                        {

                            Shape_Mode_File_Model_List = new ObservableCollection<Shape_Mode_File_Model>(_ShapeModel.OrderBy(o => o.ID).ToList());


                        });


                return _ShapeModel;
            }
            catch (Exception e)
            {
                throw new Exception("读取模型文件夹失败！原因：" + e.Message);


            }
        }


        /// <summary>
        /// 保存匹配模型文件
        /// </summary>
        /// <param name="_Shape_Handle_List"></param>
        /// <param name="_Shape_XLD_Handle_List"></param>
        public void Save_ShapeModel(List<HTuple> _Shape_Handle_List, List<HXLDCont> _Shape_XLD_Handle_List)
        {

            //获得保存模板名称
            string _Model_Location = Set_ShapeModel_Path(Create_Shape_ModelXld.Create_ID);

            //检查模型文件是否覆盖
            if (File.Exists(_Model_Location))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    if (!(MessageBox.Show("模型文件：" + Create_Shape_ModelXld.Create_ID + " 号已存在，是否覆盖？", "标定提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)) return;

                });

            }

            Set_Shape_HDict(new Shape_Mode_File_Model()
            {
                Shape_Area = Create_Shape_ModelXld.ShapeModel_Name,
                Shape_Craft = Match_Model_Craft_Type,
                Shape_Handle_List = _Shape_Handle_List,
                Shape_XLD_Handle_List = _Shape_XLD_Handle_List,
                Shape_Image_Rectified = Image_Rectified,
                Shape_Model = Create_Shape_ModelXld.Shape_Based_Model,
                Shape_PlaneInBase_Pos = Plane_In_BasePose,
                Shape_Image_Rectified_Ratio = Image_Rectified_Ratio,
                Shape_Image_Rectified_Heigth = Image_Rectified_Height,
                Shape_Image_Rectified_Width = Image_Rectified_Width,
                Shape_Model_2D_Origin = Model_2D_Origin,
                Shape_Calibration_PathInBase_List = Calib_PathInBase_List,
                Shape_Robot_Type = Robot_Type

            }, _Model_Location);


        }

        public void Reset_Calibration_Data_ShapeModel(int _ID)
        {
            Selected_Shape_Model.ThrowIfNull("匹配模型未能正常选择！");

            //对应标定参数修改
            //Selected_Shape_Model.Shape_Area = Create_Shape_ModelXld.ShapeModel_Name;
            Selected_Shape_Model.Shape_Image_Rectified = Image_Rectified;
            Selected_Shape_Model.Shape_PlaneInBase_Pos = Plane_In_BasePose;
            Selected_Shape_Model.Shape_Image_Rectified_Ratio = Image_Rectified_Ratio;
            Selected_Shape_Model.Shape_Calibration_PathInBase_List = Calib_PathInBase_List;
            Selected_Shape_Model.Shape_Robot_Type = Robot_Type;
            Selected_Shape_Model.Shape_Image_Rectified_Heigth = Image_Rectified_Height;
            Selected_Shape_Model.Shape_Image_Rectified_Width = Image_Rectified_Width;
            Set_Shape_HDict(Selected_Shape_Model, Set_ShapeModel_Path(_ID));



        }


        /// <summary>
        /// 获得匹配模型文件字典
        /// </summary>
        /// <param name="_Shape_File"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Shape_Mode_File_Model Get_Shape_HDict(FileInfo _Shape_File)
        {

            HDict _ModelHDict = new();
            Shape_Mode_File_Model _Shape_Mode_File_Model = new();
            HTuple _HShape_Handle_List = new();
            HDict _Shape_Calibration_PathInBase_HDict = new();
            HTuple _HShape_XLD_Handle_List = new();
            HDict _XLDHDict = new();
            try
            {




                //读取模型文件字典
                _ModelHDict.ReadDict(_Shape_File.FullName, new HTuple(), new HTuple());
                //根据文件名称设置id
                _Shape_Mode_File_Model.ID = int.Parse(_Shape_File.Name.Split(".")[0].Split("_")[1]);

                //获取模型字典参数
                _Shape_Mode_File_Model.Shape_Craft = Enum.Parse<Match_Model_Craft_Type_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Craft)));
                _Shape_Mode_File_Model.Shape_Model = Enum.Parse<Shape_Based_Model_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Model)));
                _Shape_Mode_File_Model.Shape_Area = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Area));
                _Shape_Mode_File_Model.Shape_Image_Rectified = new HImage(_ModelHDict.GetDictObject(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified)));
                _Shape_Mode_File_Model.Shape_Robot_Type = Enum.Parse<Robot_Type_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Robot_Type)));
                _Shape_Mode_File_Model.Creation_Date = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Creation_Date));
                _Shape_Mode_File_Model.Shape_PlaneInBase_Pos = new Point_Model(new HPose(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_PlaneInBase_Pos))));
                _Shape_Mode_File_Model.Shape_Model_2D_Origin = new Point_Model(new HPose(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Model_2D_Origin))));
                _Shape_Mode_File_Model.Shape_Image_Rectified_Ratio = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified_Ratio));
                _Shape_Mode_File_Model.Shape_Image_Rectified_Width = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified_Width));
                _Shape_Mode_File_Model.Shape_Image_Rectified_Heigth = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified_Heigth));

                //读取模型集合
                _HShape_Handle_List = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Handle_List));
                //添加到变量中
                for (int i = 0; i < _HShape_Handle_List.Length; i++)
                {
                    _Shape_Mode_File_Model.Shape_Handle_List.Add(_HShape_Handle_List.TupleSelect(i));

                }

                //获得标定位置点
                HTuple _Shape_Calibration_PathInBase = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Calibration_PathInBase_List));
                _Shape_Calibration_PathInBase_HDict = new HDict(_Shape_Calibration_PathInBase.H);

                for (int i = 0; i < _Shape_Calibration_PathInBase_HDict.GetDictParam("keys", new HTuple()).Length; i++)
                {
                    _Shape_Mode_File_Model.Shape_Calibration_PathInBase_List.Add(new Point_Model(new HPose(_Shape_Calibration_PathInBase_HDict.GetDictTuple(i))));

                }


                //获得模型xld集合
                _HShape_XLD_Handle_List = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_XLD_Handle_List));

                //创建模型字典
                _XLDHDict = new HDict(_HShape_XLD_Handle_List.H);

                //添加到变量里面
                for (int i = 0; i < _XLDHDict.GetDictParam("keys", new HTuple()).Length; i++)
                {
                    _Shape_Mode_File_Model.Shape_XLD_Handle_List.Add(new HXLDCont(_XLDHDict.GetDictObject(i)));

                }
                return _Shape_Mode_File_Model;
            }
            catch (Exception e)
            {
                //读取错误释放内存
                _Shape_Mode_File_Model.Shape_Image_Rectified.Dispose();
                foreach (var item in _Shape_Mode_File_Model.Shape_Handle_List)
                {
                    HOperatorSet.ClearShapeModel(item);
                    item.Dispose();
                }
                foreach (var item in _Shape_Mode_File_Model.Shape_XLD_Handle_List)
                {

                    item.Dispose();
                }
                _Shape_Calibration_PathInBase_HDict.Dispose();
                _HShape_Handle_List.Dispose();
                _HShape_XLD_Handle_List.Dispose();
                _XLDHDict.Dispose();
                _ModelHDict.Dispose();
                throw new Exception("读取模型文件: " + _Shape_File.Name + "失败！原因：" + e.Message);

            }
            finally
            {

                //for (int i = 0; i < _HShape_Handle_List.Length; i++)
                //{
                //    _HShape_Handle_List.TupleSelect(i).Dispose();

                //}
                _HShape_Handle_List.Dispose();
                _ModelHDict.Dispose();
            }
        }


        /// <summary>
        /// 设置匹配模型文件字典
        /// </summary>
        /// <param name="_Shape_File"></param>
        /// <param name="_Save_Path"></param>
        /// <returns></returns>
        private HDict Set_Shape_HDict(Shape_Mode_File_Model _Shape_File, string _Save_Path)
        {

            HDict _ModelHDict = new HDict();

            HTuple _Shape_Handle_List = new();
            HDict _Shape_XLD_List = new HDict();
            HDict _Shape_Calibration_PathInBase_HDict = new HDict();
            HTuple _Shape_Calibration_PathInBase_HTuple = new();

            ///参数设置到字典中
            _ModelHDict.CreateDict();
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Model), _Shape_File.Shape_Model.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Area), _Shape_File.Shape_Area.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Craft), _Shape_File.Shape_Craft.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Robot_Type), _Shape_File.Shape_Robot_Type.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_PlaneInBase_Pos), _Shape_File.Shape_PlaneInBase_Pos.HPose);
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Model_2D_Origin), _Shape_File.Shape_Model_2D_Origin.HPose);
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Creation_Date), DateTime.Now.ToString("F"));
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Image_Rectified_Ratio), _Shape_File.Shape_Image_Rectified_Ratio);
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Image_Rectified_Heigth), _Shape_File.Shape_Image_Rectified_Heigth);
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Image_Rectified_Width), _Shape_File.Shape_Image_Rectified_Width);

            //添加匹配模型集合中
            foreach (var _handle in _Shape_File.Shape_Handle_List)
            {
                _Shape_Handle_List = _Shape_Handle_List.TupleConcat(_handle);
            }
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Handle_List), _Shape_Handle_List);


            ///添加xld模型到集合
            _Shape_XLD_List.CreateDict();
            for (int i = 0; i < _Shape_File.Shape_XLD_Handle_List.Count; i++)
            {

                _Shape_XLD_List.SetDictObject(_Shape_File.Shape_XLD_Handle_List[i], i);
            }
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_XLD_Handle_List), _Shape_XLD_List);




            //t添加标定路径坐标
            _Shape_Calibration_PathInBase_HDict.CreateDict();

            for (int i = 0; i < _Shape_File.Shape_Calibration_PathInBase_List.Count; i++)
            {
                _Shape_Calibration_PathInBase_HDict.SetDictTuple(i, _Shape_File.Shape_Calibration_PathInBase_List[i].HPose);
            }
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Calibration_PathInBase_List), _Shape_Calibration_PathInBase_HDict);



            ///添加图像校正图像到字典中
            _ModelHDict.SetDictObject(_Shape_File.Shape_Image_Rectified, nameof(_Shape_File.Shape_Image_Rectified));

            //写入本地
            _ModelHDict.WriteDict(_Save_Path, new HTuple(), new HTuple());



            return _ModelHDict;

        }


        /// <summary>
        /// 获得匹配文件文件夹的名称
        /// </summary>
        /// <returns></returns>
        private List<FileInfo> Get_ShapeModel_Path()
        {
            List<FileInfo> _PathList = new List<FileInfo>();
            if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);

            DirectoryInfo _FileInfo = new DirectoryInfo(Save_Path);



            //检查文件夹内文件情况,到集合中
            foreach (FileInfo _FileName in _FileInfo.GetFiles())
            {
                if (_FileName.Extension == ".hdict")
                {
                    _PathList.Add(_FileName);
                }
            }


            return _PathList;
        }

        /// <summary>
        /// 根据模型类型获得模型文件地址
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_Model_Enum"></param>
        /// <returns></returns>
        private string Set_ShapeModel_Path(int _ID)
        {
            ////获得识别位置名称

            string Shape_Save_Path = string.Empty;

            if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);



            Shape_Save_Path = Save_Path + "\\" + "Job_" + _ID + ".hdict";




            return Shape_Save_Path;

        }




        public HImage Set_ImageRectified(HImage _image)
        {


            HImage _ResultImage = new HImage();
            HImage _MapImage = new HImage();
            _image.ThrowIfNull("窗口图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized());
            Image_Rectified.ThrowIfNull("未创建模型校正，不能校正！").Throw().IfFalse(_ => _.IsInitialized());

            //Shape_Mode_File_Model? _Model = Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Find_Shape_Model.FInd_ID).Shape_Image_Rectified;
            //_Model.ThrowIfNull(Find_Shape_Model.FInd_ID + "号模型，无法在模型库中找到校正图像！");
            try
            {



                _MapImage = Image_Rectified.CopyImage();
                //进行图像校正后识别
                _ResultImage = _image.MapImage(_MapImage).CopyImage();
                _image.Dispose();
                _MapImage.Dispose();

            }
            catch (HalconException e)
            {
                _image.Dispose();
                _MapImage.Dispose();
                throw new Exception("图像校正错误！原因：" + e.Message);
            }

            return _ResultImage;
        }






        /// <summary>
        /// 进行图像相机校正
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_Camera_Paramteters"></param>
        /// <param name="HandEye_ToolinCamera"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HImage Get_ImageRectified(HImage _Image, Halcon_Camera_Calibration_Parameters_Model _Camera_Paramteters, Point_Model HandEye_ToolinCamera)
        {
            //check data
            Tool_In_BasePos.HPose.Throw("创建模型的相机位置未设定数据，请手动或者机器人通讯获取！").IfEquals(new Point_Model().HPose);
            Plane_In_BasePose.HPose.Throw("创建模型三维位置未设定数据，请手动或者机器人通讯获取！").IfEquals(new Point_Model().HPose);
            _Image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized());

            try
            {


                //转换平面在相机的坐标,创建平面Z方向远离相机
                Point_Model BaseInToolPose = new Point_Model(Tool_In_BasePos.HPose.PoseInvert());
                Point_Model BaseInCamPose = new Point_Model(HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
                Point_Model PlaneInCamPose = new Point_Model(BaseInCamPose.HPose.PoseCompose(Plane_In_BasePose.HPose));



                //计算平面位置在基坐标
                //Point_Model CamInBasePose = new Point_Model(BaseInCamPose.HPose.PoseInvert());
                //Point_Model PlaneInBasePose = new Point_Model(CamInBasePose.HPose.PoseCompose(PlaneInCamPose.HPose));


                //计算缩放比例
                HRegion RegionGrid = new HRegion();

                //创建图像布局平均点
                RegionGrid.GenGridRegion(20, 20, "points", _Camera_Paramteters.Image_Width, _Camera_Paramteters.Image_Height);

                //获得区域点
                RegionGrid.GetRegionPoints(out HTuple _Rows, out HTuple _Colums);

                HXLDCont _ContCircle = new HXLDCont();

                _ContCircle.GenCircleContourXld(_Rows, _Colums, HTuple.TupleGenConst(_Rows.Length, 1.0), new HTuple(0), new HTuple(6.28318), new HTuple("positive"), 0.05);

                HXLDCont _ContCircleWorldPlane = new HXLDCont();

                _ContCircleWorldPlane = _ContCircle.ContourToWorldPlaneXld(_Camera_Paramteters.HCamPar, PlaneInCamPose.HPose, "m");

                _ContCircleWorldPlane.FitEllipseContourXld("fitzgibbon", -1, 0, 0, 20, 5, 2, out _, out _, out _, out HTuple _Radius1, out HTuple _Radius2, out _, out _, out _);

                //得到最小的缩放比例
                HTuple _ScaleRectification = _Radius2.TupleMin();
                HTuple _ScaleRectification1 = _Radius1.TupleMin();


                //计算平面边界
                HRegion ImageArea = new HRegion();

                ImageArea.GenRectangle1(new HTuple(0), new HTuple(0), new HTuple(_Camera_Paramteters.Image_Height), new HTuple(_Camera_Paramteters.Image_Width));

                HRegion RegionBorder = ImageArea.Boundary("inner_filled");

                RegionBorder.GetRegionPoints(out HTuple _BorderRows, out HTuple _BorderColumns);

                //根据相机平面坐标，生产最小位置
                _Camera_Paramteters.HCamPar.ImagePointsToWorldPlane(PlaneInCamPose.HPose, _BorderRows, _BorderColumns, "m", out HTuple _BorderX, out HTuple _BorderY);

                //设置查找平面原点
                Point_Model PlaneInCamOriginPose = new Point_Model(PlaneInCamPose.HPose.SetOriginPose(_BorderX.TupleMin(), _BorderY.TupleMin(), 0));

                //比例缩放下最大图像尺寸
                int _WidthRect = ((_BorderX.TupleMax() - _BorderX.TupleMin()) / _ScaleRectification + 0.5).TupleInt();
                int _HeightRect = ((_BorderY.TupleMax() - _BorderY.TupleMin()) / _ScaleRectification + 0.5).TupleInt();

                //计算校正图像
                Image_Rectified.Dispose();
                Image_Rectified = new HImage();
                Image_Rectified.GenEmptyObj();
                Image_Rectified.GenImageToWorldPlaneMap(_Camera_Paramteters.HCamPar, PlaneInCamOriginPose.HPose, _Camera_Paramteters.Image_Width, _Camera_Paramteters.Image_Height, _WidthRect, _HeightRect, _ScaleRectification, "bilinear");

                //_Image_Rectified = _Image.MapImage(Image_Rectified);

                //保存校正图像后的比例
                Image_Rectified_Ratio = double.Parse(_ScaleRectification.D.ToString());
                ///保存校正图像原尺寸
                _Image.GetImageSize(out HTuple _w, out HTuple _h);
                Image_Rectified_Height = _h;
                Image_Rectified_Width = _w;
                //Image_Rectified = new HImage(_Image_Rectified);
                return Image_Rectified;


            }
            catch (Exception e)
            {

                throw new Exception("创建校正图像失败！原因：" + e.Message);
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
        public void ShapeModel_Create_Save(HImage _Image, HCamPar? _Select_Camera_Parameter = null, HPose? _ReferencePose = null)
        {

            //初始化匹配类型
            HShapeModel _ShapeModel = new HShapeModel();
            HDeformableModel _DeformableModel = new HDeformableModel();
            HNCCModel _NccModel = new HNCCModel();

            _Image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized()); ;

            Match_Model_Craft_Type.Throw("请选择创建模型的工艺！").IfEquals(Match_Model_Craft_Type_Enum.请选择模型工艺);


            ALL_Models_XLD.CountObj().Throw("请检查创建模型工艺部位状态！").IfNotEquals(Drawing_Data_List.Count);







            try
            {


                switch (Create_Shape_ModelXld.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:



                        //创建模型
                        _ShapeModel.CreateShapeModelXld(ALL_Models_XLD, Create_Shape_ModelXld.NumLevels,
                                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                                Create_Shape_ModelXld.Optimization.ToString(), Create_Shape_ModelXld.Metric.ToString(),
                                                Create_Shape_ModelXld.MinContrast);

                        _ShapeModel.SetShapeModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HXLDCont>() { });



                        break;

                    case Shape_Based_Model_Enum.planar_deformable_model:


                        if (_Select_Camera_Parameter == null && _ReferencePose == null)
                        {

                            //创建模型
                            _DeformableModel.CreatePlanarUncalibDeformableModelXld(
                                    ALL_Models_XLD,
                                    Create_Shape_ModelXld.NumLevels,
                                    (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                    (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                    (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                    Create_Shape_ModelXld.ScaleRMin,
                                    new(),
                                    Create_Shape_ModelXld.ScaleRStep,
                                    Create_Shape_ModelXld.ScaleCMin,
                                    new(),
                                     Create_Shape_ModelXld.ScaleCStep,
                                     Create_Shape_ModelXld.Optimization.ToString(),
                                     Create_Shape_ModelXld.Metric.ToString(),
                                     Create_Shape_ModelXld.MinContrast,
                                     new(),
                                     new());
                        }
                        else
                        {


                            //创建模型，需要机器人提高原点
                            _DeformableModel.CreatePlanarCalibDeformableModelXld(
                                    ALL_Models_XLD,
                                    _Select_Camera_Parameter, _ReferencePose,
                                    Create_Shape_ModelXld.NumLevels,
                                    (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                    (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                    (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                    Create_Shape_ModelXld.ScaleRMin,
                                    new(),
                                    Create_Shape_ModelXld.ScaleRStep,
                                    Create_Shape_ModelXld.ScaleCMin,
                                    new(),
                                     Create_Shape_ModelXld.ScaleCStep,
                                     Create_Shape_ModelXld.Optimization.ToString(),
                                     Create_Shape_ModelXld.Metric.ToString(),
                                     Create_Shape_ModelXld.MinContrast,
                                     new(),
                                     new());

                        }


                        _DeformableModel.SetDeformableModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _DeformableModel }, new List<HXLDCont>() { });


                        break;

                    case Shape_Based_Model_Enum.local_deformable_model:


                        _DeformableModel.CreateLocalDeformableModelXld(
                                ALL_Models_XLD,
                                Create_Shape_ModelXld.NumLevels,
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.ScaleRMin,
                                new(),
                                Create_Shape_ModelXld.ScaleRStep,
                                Create_Shape_ModelXld.ScaleCMin,
                                new(),
                                Create_Shape_ModelXld.ScaleCStep,
                                Create_Shape_ModelXld.Optimization.ToString(),
                                Create_Shape_ModelXld.Metric.ToString(),
                                Create_Shape_ModelXld.MinContrast,
                                new(), new());


                        _DeformableModel.SetDeformableModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);

                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _DeformableModel }, new List<HXLDCont>() { });


                        break;

                    case Shape_Based_Model_Enum.Scale_model:

                        //创建模型
                        _ShapeModel.CreateScaledShapeModelXld(ALL_Models_XLD,
                                Create_Shape_ModelXld.NumLevels,
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.ScaleMin,
                                Create_Shape_ModelXld.ScaleMax,
                                Create_Shape_ModelXld.ScaleStep,
                                Create_Shape_ModelXld.Optimization.ToString(),
                                Create_Shape_ModelXld.Metric.ToString(),
                                 Create_Shape_ModelXld.MinContrast);


                        _ShapeModel.SetShapeModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);

                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HXLDCont>() { });





                        break;

                    case Shape_Based_Model_Enum.Aniso_Model:


                        //创建模型
                        _ShapeModel.CreateAnisoShapeModelXld(ALL_Models_XLD,
                                Create_Shape_ModelXld.NumLevels,
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                 Create_Shape_ModelXld.ScaleRMin,
                                 Create_Shape_ModelXld.ScaleRMax,
                                Create_Shape_ModelXld.ScaleRStep,
                                  Create_Shape_ModelXld.ScaleCMin,
                                 Create_Shape_ModelXld.ScaleCMax,
                                 Create_Shape_ModelXld.ScaleCStep,
                                Create_Shape_ModelXld.Optimization.ToString(),
                                Create_Shape_ModelXld.Metric.ToString(),
                                 Create_Shape_ModelXld.MinContrast);

                        _ShapeModel.SetShapeModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HXLDCont>() { });






                        break;


                    case Shape_Based_Model_Enum.Ncc_Model:


                        HRegion Polygon_Xld = new();
                        HXLDPoly Select_Region = new();
                        HRegion Gen_Region = new();
                        HRegion Dilation_Region = new();
                        HTuple _Pos_Row = new();
                        HTuple _Pos_Col = new();
                        HDict _Data_Dict = new();

                        Polygon_Xld.GenEmptyObj();







                        //每个xld转换多边形类型，从第一个原点显示开始，避开生成
                        for (int X = 1; X < ALL_Models_XLD.CountObj(); X++)
                        {
                            //分解xld图像点为多边形
                            Select_Region = ALL_Models_XLD.SelectObj(X + 1).GenPolygonsXld("ramer", 0.1);
                            //HOperatorSet.GenPolygonsXld(ALL_Models_XLD.SelectObj(X + 1), out Select_Region, "ramer", 0.1);
                            //HOperatorSet.GetPolygonXld(Select_Region, out _Pos_Row, out _Pos_Col, out HTuple _, out HTuple _);

                            //获得分解点的多边形坐标数据
                            Select_Region.GetPolygonXld(out _Pos_Row, out _Pos_Col, out HTuple _, out HTuple _);

                            //将多边形转换区域
                            Gen_Region.GenRegionPolygon(_Pos_Row, _Pos_Col);
                            //HOperatorSet.GenRegionPolygon(out Gen_Region, _Pos_Row, _Pos_Col);
                            //HOperatorSet.ConcatObj(Polygon_Xld, Gen_Region, out Polygon_Xld);

                            //存入集合中
                            Polygon_Xld = Polygon_Xld.ConcatObj(Gen_Region);
                        }

                        //膨胀全部区域
                        Dilation_Region = Polygon_Xld.DilationCircle(Create_Shape_ModelXld.DilationCircle);
                        //HOperatorSet.DilationCircle(Polygon_Xld, out Dilation_Region, Create_Shape_ModelXld.DilationCircle);
                        //区域合并
                        Dilation_Region = Dilation_Region.Union1();

                        HImage ImageRegion = new();
                        ImageRegion.GenEmptyObj();



                        //抠图出
                        ImageRegion = _Image.ReduceDomain(Dilation_Region);

                        //创建NCC模板
                        _NccModel.CreateNccModel(
                                 ImageRegion,
                                new HTuple(Create_Shape_ModelXld.NumLevels),
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.Metric.ToString());



                        //计算区域中心，设置模型中心点
                        Dilation_Region.AreaCenter(out double _row, out double _col);
                        _NccModel.SetNccModelOrigin(Model_2D_Origin.X - _row, Model_2D_Origin.Y - _col);


                        HHomMat2D _Tran = new();
                        var bb = ALL_Models_XLD;
                        ///xld模型偏移
                        _Tran.VectorAngleToRigid(Model_2D_Origin.X, Model_2D_Origin.Y, 0, 0, 0, 0);
                        var aa = ALL_Models_XLD = ALL_Models_XLD.AffineTransContourXld(_Tran);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _NccModel }, new List<HXLDCont>() { ALL_Models_XLD });

                        break;
                }


            }
            catch (Exception e)
            {
                throw new Exception("创建" + Create_Shape_ModelXld.Shape_Based_Model + "模型失败！原因：" + e.Message);

                //return new HPR_Status_Model<bool>(HVE_Result_Enum.创建匹配模型失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                _ShapeModel.ClearShapeModel();
                _DeformableModel.ClearDeformableModel();
                _NccModel.ClearNccModel();

                _ShapeModel.Dispose();
                _DeformableModel.Dispose();
                _NccModel.Dispose();
                //GC.Collect();

            }
        }



        //public void Dispose()
        //{

        //    foreach (var _Shape in Shape_Mode_File_Model_List)
        //    {
        //        _Shape.Dispose();
        //    }
        //    //GC.Collect();
        //    //GC.SuppressFinalize(this);

        //}
    }


    public class Map_Image_Fun
    {
        public Map_Image_Fun(HImage _Image, HImage _map)
        {
            Result_Image.Dispose();
            Result_Image.GenEmptyObj();
            _Image = _Image.ZoomImageSize(4024, 3036, "bilinear");

            Result_Image = _Image.MapImage(_map);
            _map.Dispose();
            _Image.Dispose();
        }



        public HImage Result_Image { set; get; } = new HImage();



    }


}