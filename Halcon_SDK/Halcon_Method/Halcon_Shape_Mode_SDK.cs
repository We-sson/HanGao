using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Shapes;
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


        /// <summary>
        /// 全部工艺模型合并
        /// </summary>
        public HXLDCont ALL_Models_XLD { set; get; } = new HXLDCont();

        /// <summary>
        /// 模型ID
        /// </summary>
        public HTuple Shape_ID = new HTuple();

        /// <summary>
        /// 一般形状模型匹配创建属性
        /// </summary>
        public Create_Shape_Based_ModelXld Create_Shape_ModelXld { set; get; } = new Create_Shape_Based_ModelXld();

        /// <summary>
        /// 全部模型三维原点
        /// </summary>
        public Point_Model Model_Plane_Pos { set; get; } = new Point_Model() { X = 659.792, Y = -308.937, Z = 243.794, Rx = 171.291, Ry = -85.334, Rz = 50.470, HType = Halcon_Pose_Type_Enum.abg };


        /// <summary>
        /// 相机拍摄位置
        /// </summary>
        public Point_Model Model_Camera_Pos { set; get; } = new Point_Model() { X = 704.849, Y = -436.028, Z = 217.698, Rx = 175.625, Ry = -58.682, Rz = 39.613, HType = Halcon_Pose_Type_Enum.abg };

        /// <summary>
        /// 二维平面原点位置
        /// </summary>
        public Point_Model Model_2D_Origin { set; get; } = new Point_Model();

        /// <summary>
        /// 图像校正变量
        /// </summary>
        public HImage Image_Rectified { set; get; } = new HImage();


        /// <summary>
        /// 模型原地设置类型
        /// </summary>
        public Model_2D_Origin_Type_Enum Model_2D_Origin_Type { set; get; } = Model_2D_Origin_Type_Enum.Origin_Camera;


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




        private Match_Model_Craft_Type_Enum _Match_Model_Craft_Type = Match_Model_Craft_Type_Enum.请选择模型工艺;

        /// <summary>
        /// 模型工艺类型
        /// </summary>
        public Match_Model_Craft_Type_Enum Match_Model_Craft_Type
        {
            get { return _Match_Model_Craft_Type; }
            set { _Match_Model_Craft_Type = value; Init_Crafe_Type_List(value); }
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
                        case Sink_Basin_R_Welding.R角中线轮廓:
                            Drawing_Data_List[0] = User_Drawing_Data;




                            break;
                        case Sink_Basin_R_Welding.盆胆左侧线轮廓:
                            Drawing_Data_List[1] = User_Drawing_Data;




                            break;
                        case Sink_Basin_R_Welding.盆胆右侧线轮廓:
                            Drawing_Data_List[2] = User_Drawing_Data;




                            break;
                        default:
                            break;
                    }

                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:

                    //根据工艺模型步骤

                    switch (User_Drawing_Data.Craft_Type_Enum)
                    {
                        case Sink_Board_R_Welding.面板横直线轮廓:
                            Drawing_Data_List[0] = User_Drawing_Data;


                            break;
                        case Sink_Board_R_Welding.面板圆弧轮廓:

                            Drawing_Data_List[1] = User_Drawing_Data;

                            break;
                        case Sink_Board_R_Welding.面板竖直线轮廓:
                            Drawing_Data_List[2] = User_Drawing_Data;


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
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.R角中线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.盆胆左侧线轮廓 });
                    Drawing_Data_List.Add(new Vision_Create_Model_Drawing_Model() { Craft_Type_Enum = Sink_Basin_R_Welding.盆胆右侧线轮廓 });
                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:
                    Drawing_Data_List.Clear();
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



        public void Read_Fold_ShapeModel()
        {













        }



        public List<Shape_Mode_File_Model> Get_ShapeModel()
        {
           List< Shape_Mode_File_Model >_ShapeModel = new List<Shape_Mode_File_Model>();

            //获得保存模板名称
            List<FileInfo> _Model_Location = Get_ShapeModel_Path();


            foreach (var _path in _Model_Location)
            {
                Shape_Mode_File_Model_List.Add ( Get_Shape_HDict(_path));
            }


            return _ShapeModel;
        }


        public void Save_ShapeModel(List<HTuple> _Shape_Handle_List, List<HObject> _Shape_XLD_Handle_List)
        {

            //获得保存模板名称
            string _Model_Location = Save_Path + Set_ShapeModel_Path();

            Set_Shape_HDict(new Shape_Mode_File_Model()
            {
                Shape_Area = Create_Shape_ModelXld.ShapeModel_Name,
                Shape_Craft = Match_Model_Craft_Type,
                Shape_Handle_List = _Shape_Handle_List,
                Shape_XLD_Handle_List = _Shape_XLD_Handle_List,
                Shape_Image_Rectified = Image_Rectified,
                Shape_Model = Create_Shape_ModelXld.Shape_Based_Model,

            }, _Model_Location);


        }


        private Shape_Mode_File_Model Get_Shape_HDict(FileInfo _Shape_File)
        {

            HDict _ModelHDict = new HDict();
           

            Shape_Mode_File_Model _Shape_Mode_File_Model = new Shape_Mode_File_Model();


            _ModelHDict.ReadDict(_Shape_File.FullName, new HTuple(), new HTuple());

            _Shape_Mode_File_Model.ID = int.Parse(_Shape_File.Name.Split(".")[0].Split("_")[1]);

            _Shape_Mode_File_Model.Shape_Craft= Enum.Parse<Match_Model_Craft_Type_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Craft)));

            _Shape_Mode_File_Model.Shape_Model = Enum.Parse<Shape_Based_Model_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Model)));

            _Shape_Mode_File_Model.Shape_Area = Enum.Parse<ShapeModel_Name_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Area)));

            _Shape_Mode_File_Model.Shape_Image_Rectified =new HImage(  _ModelHDict.GetDictObject(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified)));
           


            HTuple a = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Handle_List));


            HTuple b = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_XLD_Handle_List));

            HDict _XLDHDict = new HDict(b.H);


            for (int i = 0; i < a.Length; i++)
            {
                        _Shape_Mode_File_Model.Shape_Handle_List.Add(a.TupleSelect(i));

            }
            
         HTuple aa=   _XLDHDict.GetDictParam("keys",  new HTuple ());


            


            for (int i = 0; i < aa.Length; i++)
            {

              var tr=  _XLDHDict.GetDictParam("key_data_type", i);

                HObject bc = new HObject ( _XLDHDict.GetDictObject(i));

                _Shape_Mode_File_Model.Shape_XLD_Handle_List.Add(new HObject(bc)); ;
            }

            return _Shape_Mode_File_Model;
        }


        private HDict Set_Shape_HDict(Shape_Mode_File_Model _Shape_File, string _Save_Path)
        {

            HDict _ModelHDict = new HDict();
        
            HTuple _Shape_Handle_List = new HTuple();
            HDict _Shape_XLD_List = new HDict();

            ///参数设置到字典中
            _ModelHDict.CreateDict();
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Model), _Shape_File.Shape_Model.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Area), _Shape_File.Shape_Area.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Craft), _Shape_File.Shape_Craft.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Creation_Date), _Shape_File.Creation_Date);

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
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_XLD_Handle_List),_Shape_XLD_List);


            ///添加图像校正图像到字典中
            _ModelHDict.SetDictObject(_Shape_File.Shape_Image_Rectified, nameof(_Shape_File.Shape_Image_Rectified));

            //写入本地
            _ModelHDict.WriteDict(_Save_Path, new HTuple(), new HTuple());



            return _ModelHDict;

        }


        private List<FileInfo> Get_ShapeModel_Path()
        {
            List<FileInfo> _PathList = new List<FileInfo>();
            if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);

            DirectoryInfo _FileInfo = new DirectoryInfo(Save_Path);



            //检查文件夹内文件情况,到集合中
            foreach (FileInfo _FileName in _FileInfo.GetFiles())
            {
                if (_FileName.Extension== ".hdict")
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
        private string Set_ShapeModel_Path()
        {
            ////获得识别位置名称

            string Shape_Save_Path = string.Empty;

            if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);



            Shape_Save_Path = "\\" + "Job_" + Create_Shape_ModelXld.Create_ID.ToString();




            return Shape_Save_Path;

        }


        public HImage ImageRectified(HImage _Image, Halcon_Camera_Calibration_Parameters_Model _Camera_Paramteters, Point_Model HandEye_ToolinCamera)
        {
            //check data
            if (Model_Camera_Pos == new Point_Model()) { throw new Exception("创建模型的相机位置未设定数据，请手动或者机器人通讯获取！"); }
            if (Model_Plane_Pos == new Point_Model()) { throw new Exception("创建模型三维位置未设定数据，请手动或者机器人通讯获取！"); }

            //转换平面在相机的坐标,创建平面Z方向远离相机
            Point_Model BaseInToolPose = new Point_Model(Model_Camera_Pos.HPose.PoseInvert());
            Point_Model BaseInCamPose = new Point_Model(HandEye_ToolinCamera.HPose.PoseCompose(BaseInToolPose.HPose));
            Point_Model PlaneInCamPose = new Point_Model(BaseInCamPose.HPose.PoseCompose(Model_Plane_Pos.HPose));

            //计算平面位置在基坐标
            Point_Model CamInBasePose = new Point_Model(BaseInCamPose.HPose.PoseInvert());
            Point_Model PlaneInBasePose = new Point_Model(CamInBasePose.HPose.PoseCompose(PlaneInCamPose.HPose));


            //计算缩放比例
            HRegion RegionGrid = new HRegion();

            //创建图像布局平均点
            RegionGrid.GenGridRegion(20, 20, "points", _Camera_Paramteters.Image_Width, _Camera_Paramteters.Image_Height);

            //获得区域点
            RegionGrid.GetRegionPoints(out HTuple _Rows, out HTuple _Colums);

            HXLDCont _ContCircle = new HXLDCont();

            _ContCircle.GenCircleContourXld(_Rows, _Colums, HTuple.TupleGenConst(_Rows.Length, 1.0), new HTuple(0), new HTuple(6.28318), new HTuple("positive"), 0.1);

            HXLDCont _ContCircleWorldPlane = new HXLDCont();

            _ContCircleWorldPlane = _ContCircle.ContourToWorldPlaneXld(_Camera_Paramteters.HCamPar, PlaneInCamPose.HPose, "m");

            _ContCircleWorldPlane.FitEllipseContourXld("fitzgibbon", -1, 0, 0, 200, 3, 2, out _, out _, out _, out _, out HTuple _Radius2, out _, out _, out _);

            //得到最小的缩放比例
            HTuple _ScaleRectification = _Radius2.TupleMin();


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

            Image_Rectified.GenImageToWorldPlaneMap(_Camera_Paramteters.HCamPar, PlaneInCamOriginPose.HPose, _Camera_Paramteters.Image_Width, _Camera_Paramteters.Image_Width, _WidthRect, _HeightRect, _ScaleRectification, "bilinear");

            _Image = _Image.MapImage(Image_Rectified);


            return _Image;

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
        public void ShapeModel_Create_Save(HImage? _Image = null, HCamPar? _Select_Camera_Parameter = null, HPose? _ReferencePose = null)
        {

            //初始化匹配类型
            HShapeModel _ShapeModel = new HShapeModel();
            HDeformableModel _DeformableModel = new HDeformableModel();
            HNCCModel _NccModel = new HNCCModel();


            try
            {


                switch (Create_Shape_ModelXld.Shape_Based_Model)
                {
                    case Shape_Based_Model_Enum.shape_model:

                        //获得保存模型文件地址
                        //string _shape_model_Location = Save_Path + SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Create_Shape_ModelXld.Shape_Based_Model, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);


                        //创建模型
                        _ShapeModel.CreateShapeModelXld(ALL_Models_XLD, Create_Shape_ModelXld.NumLevels,
                                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                                Create_Shape_ModelXld.Optimization.ToString(), Create_Shape_ModelXld.Metric.ToString(),
                                                Create_Shape_ModelXld.MinContrast);

                        _ShapeModel.SetShapeModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HObject>() { });
                        ////保存模型文件
                        //_ShapeModel.WriteShapeModel(_shape_model_Location);

                        //清楚内存
                        _ShapeModel.ClearShapeModel();


                        break;

                    case Shape_Based_Model_Enum.planar_deformable_model:

                        //获得保存模型文件地址
                        //string _planar_deformable_model_Location = Save_Path + SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Create_Shape_ModelXld.Shape_Based_Model, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);


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
                                    new HTuple(),
                                    Create_Shape_ModelXld.ScaleRStep,
                                    Create_Shape_ModelXld.ScaleCMin,
                                    new HTuple(),
                                     Create_Shape_ModelXld.ScaleCStep,
                                     Create_Shape_ModelXld.Optimization.ToString(),
                                     Create_Shape_ModelXld.Metric.ToString(),
                                     Create_Shape_ModelXld.MinContrast,
                                     new HTuple(),
                                     new HTuple());
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
                                    new HTuple(),
                                    Create_Shape_ModelXld.ScaleRStep,
                                    Create_Shape_ModelXld.ScaleCMin,
                                    new HTuple(),
                                     Create_Shape_ModelXld.ScaleCStep,
                                     Create_Shape_ModelXld.Optimization.ToString(),
                                     Create_Shape_ModelXld.Metric.ToString(),
                                     Create_Shape_ModelXld.MinContrast,
                                     new HTuple(),
                                     new HTuple());

                        }


                        _DeformableModel.SetDeformableModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _DeformableModel }, new List<HObject>() { });
                        ////保存模型文件
                        //_DeformableModel.WriteDeformableModel(_planar_deformable_model_Location);

                        //清楚模型
                        _DeformableModel.ClearDeformableModel();

                        break;

                    case Shape_Based_Model_Enum.local_deformable_model:

                        //获得保存模型文件地址
                        //string _local_deformable_Location = Save_Path + SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Create_Shape_ModelXld.Shape_Based_Model, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);

                        _DeformableModel.CreateLocalDeformableModelXld(
                                ALL_Models_XLD,
                                Create_Shape_ModelXld.NumLevels,
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.ScaleRMin,
                                new HTuple(),
                                Create_Shape_ModelXld.ScaleRStep,
                                Create_Shape_ModelXld.ScaleCMin,
                                new HTuple(),
                                Create_Shape_ModelXld.ScaleCStep,
                                Create_Shape_ModelXld.Optimization.ToString(),
                                Create_Shape_ModelXld.Metric.ToString(),
                                Create_Shape_ModelXld.MinContrast,
                                new HTuple(), new HTuple());


                        _DeformableModel.SetDeformableModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);

                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _DeformableModel }, new List<HObject>() { });
                        ////保存模型文件
                        //_DeformableModel.WriteDeformableModel(_local_deformable_Location);

                        //清楚模型
                        _DeformableModel.ClearDeformableModel();

                        break;

                    case Shape_Based_Model_Enum.Scale_model:

                        //获得保存模型文件地址
                        //string _Scale_model_Location = Save_Path + SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Create_Shape_ModelXld.Shape_Based_Model, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);

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
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HObject>() { });
                        ////保存模型文件
                        //_ShapeModel.WriteShapeModel(_Scale_model_Location);

                        //清楚内存
                        _ShapeModel.ClearShapeModel();




                        break;

                    case Shape_Based_Model_Enum.Aniso_Model:

                        //获得保存模型文件地址
                        //string _Aniso_Model_Location = Save_Path + SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Create_Shape_ModelXld.Shape_Based_Model, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);

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
                        Save_ShapeModel(new List<HTuple>() { _ShapeModel }, new List<HObject>() { });

                        ////保存模型文件
                        //_ShapeModel.WriteShapeModel(_Aniso_Model_Location);

                        //清楚内存
                        _ShapeModel.ClearShapeModel();


                        break;


                    case Shape_Based_Model_Enum.Ncc_Model:


                        HRegion Polygon_Xld = new HRegion();
                        HXLDPoly Select_Region = new HXLDPoly();
                        HRegion Gen_Region = new HRegion();
                        HRegion Dilation_Region = new HRegion();
                        HTuple _Pos_Row = new HTuple();
                        HTuple _Pos_Col = new HTuple();
                        HDict _Data_Dict = new HDict();

                        Polygon_Xld.GenEmptyObj();

                        //每个xld转换多边形类型
                        for (int X = 0; X < ALL_Models_XLD.CountObj(); X++)
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

                        HImage ImageRegion = new HImage();
                        ImageRegion.GenEmptyObj();



                        //抠图出
                        ImageRegion = _Image.ReduceDomain(Dilation_Region);

                        //创建NCC模板
                        _NccModel.CreateNccModel(
                                 ImageRegion,
                                Create_Shape_ModelXld.NumLevels,
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.Metric.ToString());


                        _NccModel.SetNccModelOrigin(Model_2D_Origin.X, Model_2D_Origin.Y);


                        //保存模型
                        Save_ShapeModel(new List<HTuple>() { _NccModel, _NccModel }, new List<HObject>() { ALL_Models_XLD , ALL_Models_XLD });


                        _NccModel.ClearNccModel();



                        //保存模型文件
                        //HOperatorSet.WriteNccModel(_ModelID, _Path);
                        //模型序列化
                        //HOperatorSet.SerializeNccModel(Shape_ID, out _Serializd);
                        ////打开文件
                        //HOperatorSet.OpenFile(Shape_Save_Path, HFIle_Type_Enum.output_binary.ToString(), out _HFile);
                        ////二进制文件保存
                        //HOperatorSet.FwriteSerializedItem(_HFile, _Serializd);
                        ////关闭文件
                        //HOperatorSet.CloseFile(_HFile);

                        ////清楚模型
                        //HOperatorSet.ClearNccModel(Shape_ID);




                        ////计算区域中心未知
                        //HOperatorSet.AreaCenter(ImageRegion, out HTuple _, out Region_Row, out Region_Col);
                        ////计算移动原点2d矩阵
                        //HOperatorSet.VectorAngleToRigid(Region_Row, Region_Col, 0, 0, 0, 0, out HomMat2D);

                        //HOperatorSet.ProjectiveTransContourXld(All_XLD[i], out DXF_XLD, HomMat2D);



                        //string _XLD_Save_Path_Location = Save_Path+ SetGet_ModelXld_Path(FilePath_Type_Model_Enum.Save, Shape_Based_Model_Enum.Halcon_DXF, Create_Shape_ModelXld.ShapeModel_Name, Create_Shape_ModelXld.Create_ID);
                        //保存描述XLD轮廓
                        //ALL_Models_XLD.WriteContourXldDxf(_XLD_Save_Path_Location);

                        //保存模板xld文件
                        //HOperatorSet.WriteContourXldDxf(DXF_XLD, Shape_Save_Path);

                        //清楚模型
                        //ALL_Models_XLD.Dispose();



                        break;
                }

                //return new HPR_Status_Model<bool>(HVE_Result_Enum.Run_OK) { Result_Error_Info = _Create_Model.Shape_Based_Model.ToString() + "模型创建成功！" };
            }
            catch (Exception e)
            {
                throw new Exception("创建" + Create_Shape_ModelXld.Shape_Based_Model + "模型失败！原因：" + e.Message);

                //return new HPR_Status_Model<bool>(HVE_Result_Enum.创建匹配模型失败) { Result_Error_Info = e.Message };
            }
            finally
            {
                //清楚内存

                //All_XLD.ForEach(_M => _M.Dispose());
                //All_Region.ForEach(_M => _M.Dispose());
                //_ModelsXld.Dispose();

                //_ModelID.Dispose();
                //_Serializd.Dispose();
                //_HFile.Dispose();
                //Gen_Polygons.Dispose();
                //Region_Unio.Dispose();
                //Select_Region.Dispose();
                //Gen_Region.Dispose();
                //Polygon_Xld.Dispose();
                //Region_Dilation.Dispose();
                //All_Reduced.Dispose();
                //XLD_1.Dispose();
                //XLD_2.Dispose();
                //XLD_3.Dispose();
                //Region_Unio_1.Dispose();
                //Region_Unio_2.Dispose();
                //Region_Unio_3.Dispose();
                //ImageRegion.Dispose();
                //Dilation_Region.Dispose();
                //DXF_XLD.Dispose();
                //_Pos_Row.Dispose();
                //_Pos_Col.Dispose();
                //D_Region.Dispose();
                //_Serializd.Dispose();
                //_HFile.Dispose();
                //Region_Row.Dispose();
                //Region_Col.Dispose();
                //HomMat2D.Dispose();

                GC.Collect();
                //GC.SuppressFinalize(this);
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
        }





    }
}