using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Throw;
using static Halcon_SDK_DLL.Model.Halcon_Data_Model;
using Point = System.Windows.Point;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Shape_Mode_SDK : IDisposable
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



        public Find_Shape_Based_ModelXld Find_Shape_Model { get; set; } = new Find_Shape_Based_ModelXld();



        private Match_Model_Craft_Type_Enum _Match_Model_Craft_Type = Match_Model_Craft_Type_Enum.请选择模型工艺;

        /// <summary>
        /// 模型工艺类型
        /// </summary>
        public Match_Model_Craft_Type_Enum Match_Model_Craft_Type
        {
            get { return _Match_Model_Craft_Type; }
            set { _Match_Model_Craft_Type = value; Init_Crafe_Type_List(value); }
        }




        public Find_Shape_Results_Model Find_Shape_Model_Results(HImage _image, Halcon_Camera_Calibration_Parameters_Model? _camera_Param=null)
        {

            //初始化匹配类型
            HShapeModel _ShapeModel = new HShapeModel();
            HDeformableModel _DeformableModel = new HDeformableModel();
            //HNCCModel _NccModel = new HNCCModel();
            Find_Shape_Results_Model _Results = new Find_Shape_Results_Model();

            HHomMat2D _HomMat3D = new HHomMat2D();


            _image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized());


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


                    HTuple _row = new HTuple();
                    HTuple _column = new HTuple();
                    HTuple _angle = new HTuple();
                    HTuple _score = new HTuple();
                    HHomMat2D _HomMat2D = new HHomMat2D();
                    List<HNCCModel> _NccModel = new List<HNCCModel>();



                    for (int i = 0; i < _Model.Shape_Handle_List.Count; i++)
                    {


                        HNCCModel _ncc = new HNCCModel(_Model.Shape_Handle_List[i].H);
                        HXLDCont _Xld = new HXLDCont();
                        HRegion _Ncc_Region = new HRegion();
                        HXLDCont _Ncc_Xld = new HXLDCont();

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

                        if (_score > 0)
                        {

                            if (_camera_Param != null && Model_Plane_Pos != new Point_Model())
                            {
                                HHomMat3D _Resulye_Mat3D = new HHomMat3D();
                                _camera_Param.HCamPar.ImagePointsToWorldPlane(Model_Plane_Pos.HPose, _row, _column, "mm", out HTuple _x, out HTuple _y);

                                _Resulye_Mat3D.HomMat3dIdentity();
                                _Resulye_Mat3D.HomMat3dRotate(_angle, new HTuple ("z"), new HTuple (0), new HTuple(0), new HTuple(0));



                            }


                            _HomMat2D.VectorAngleToRigid(0, 0, 0, _row, _column, _angle);

                          
                            _Xld = _Model.Shape_XLD_Handle_List[i];
                            _Xld = _Model.Shape_XLD_Handle_List[i].AffineTransContourXld(_HomMat2D);


                            _Ncc_Region = _ncc.GetNccModelRegion();
                            _Ncc_Xld = _Ncc_Region.GenContourRegionXld("border_holes");
                            _Ncc_Xld = _Ncc_Xld.AffineTransContourXld(_HomMat2D);


                            //识别成功保存结果
                            _Results.Results_HomMat2D_List.Add(_HomMat2D);
                            _Results.Results_HXLD_List.Add(_Xld);

                            _Results.Find_Score.Add(_score);
                        }
                        else
                        {
                            //失败存储结果
                            _Results.Results_HomMat2D_List.Add(_HomMat2D);
                            _Results.Results_HXLD_List.Add(_Xld);

                            _Results.Find_Score.Add(0);
                        }
                    }
                    //_Results.Image_Rectified = _image;







                    break;


            }


            switch (_Model.Shape_Craft)
            {
                case Match_Model_Craft_Type_Enum.请选择模型工艺:


                    throw new Exception("请选择需要解析模型工艺！");



                case Match_Model_Craft_Type_Enum.焊接盆胆R角:

                    if (_Results.Find_Score.Where(_ => _ == 0).ToList().Count == 0)
                    {

                        if (_camera_Param!=null && Model_Plane_Pos!=new Point_Model ())
                        {

                            //_camera_Param.HCamPar.ImagePointsToWorldPlane(Model_Plane_Pos,)


                        }
                        




                    }



                    break;
                case Match_Model_Craft_Type_Enum.焊接面板围边:
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


                    if (Selected_Shape_Model.Selected_Shape_Handle.Length == 0)
                    {
                        throw new Exception(Selected_Shape_Model.ID + "号模型加载显示详情失败！");
                    }

                    _HObject = Get_Shape_Model_Image(Selected_Shape_Model.Shape_Model, Selected_Shape_Model.Selected_Shape_Handle);


                    break;
                case Shape_HObject_Type_Enum.Shape_XLD:

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

                    _HObject = Selected_Shape_Model.Shape_Image_Rectified;
                    break;

            }




            return _HObject;
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
                    _ShapeModel.Add(Get_Shape_HDict(_path));
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
            string _Model_Location = Set_ShapeModel_Path();

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
                Shape_Model_Plane_Pos = Model_Plane_Pos,
            }, _Model_Location);


        }

        /// <summary>
        /// 获得匹配模型文件字典
        /// </summary>
        /// <param name="_Shape_File"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Shape_Mode_File_Model Get_Shape_HDict(FileInfo _Shape_File)
        {

            HDict _ModelHDict = new HDict();
            Shape_Mode_File_Model _Shape_Mode_File_Model = new Shape_Mode_File_Model();


            try
            {




                //读取模型文件字典
                _ModelHDict.ReadDict(_Shape_File.FullName, new HTuple(), new HTuple());
                //根据文件名称设置id
                _Shape_Mode_File_Model.ID = int.Parse(_Shape_File.Name.Split(".")[0].Split("_")[1]);

                //获取模型字典参数
                _Shape_Mode_File_Model.Shape_Craft = Enum.Parse<Match_Model_Craft_Type_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Craft)));
                _Shape_Mode_File_Model.Shape_Model = Enum.Parse<Shape_Based_Model_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Model)));
                _Shape_Mode_File_Model.Shape_Area = Enum.Parse<ShapeModel_Name_Enum>(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Area)));
                _Shape_Mode_File_Model.Shape_Image_Rectified = new HImage(_ModelHDict.GetDictObject(nameof(_Shape_Mode_File_Model.Shape_Image_Rectified)));
                _Shape_Mode_File_Model.Creation_Date = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Creation_Date));
                _Shape_Mode_File_Model.Shape_Model_Plane_Pos = new Point_Model(new HPose(_ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Model_Plane_Pos))));
                //读取模型集合
                HTuple _HShape_Handle_List = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_Handle_List));
                //添加到变量中
                for (int i = 0; i < _HShape_Handle_List.Length; i++)
                {
                    _Shape_Mode_File_Model.Shape_Handle_List.Add(_HShape_Handle_List.TupleSelect(i));

                }


                //获得模型xld集合
                HTuple _HShape_XLD_Handle_List = _ModelHDict.GetDictTuple(nameof(_Shape_Mode_File_Model.Shape_XLD_Handle_List));

                //创建模型字典
                HDict _XLDHDict = new HDict(_HShape_XLD_Handle_List.H);

                //添加到变量里面
                for (int i = 0; i < _XLDHDict.GetDictParam("keys", new HTuple()).Length; i++)
                {
                    _Shape_Mode_File_Model.Shape_XLD_Handle_List.Add(new HXLDCont(_XLDHDict.GetDictObject(i)));

                }
                return _Shape_Mode_File_Model;
            }
            catch (Exception e)
            {

                throw new Exception("读取模型文件: " + _Shape_File.Name + "失败！原因：" + e.Message);

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

            HTuple _Shape_Handle_List = new HTuple();
            HDict _Shape_XLD_List = new HDict();

            ///参数设置到字典中
            _ModelHDict.CreateDict();
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Model), _Shape_File.Shape_Model.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Area), _Shape_File.Shape_Area.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Craft), _Shape_File.Shape_Craft.ToString());
            _ModelHDict.SetDictTuple(nameof(_Shape_File.Shape_Model_Plane_Pos), _Shape_File.Shape_Model_Plane_Pos.HPose);

            _ModelHDict.SetDictTuple(nameof(_Shape_File.Creation_Date), DateTime.Now.ToString("F"));

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
        private string Set_ShapeModel_Path()
        {
            ////获得识别位置名称

            string Shape_Save_Path = string.Empty;

            if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);



            Shape_Save_Path = Save_Path + "\\" + "Job_" + Create_Shape_ModelXld.Create_ID.ToString() + ".hdict";




            return Shape_Save_Path;

        }




        public HImage Set_ImageRectified(HImage _image)
        {

            HImage _ResultImage = new HImage();
            _image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized());
            Shape_Mode_File_Model? _Model = Shape_Mode_File_Model_List.FirstOrDefault((w) => w.ID == Find_Shape_Model.FInd_ID);
            _Model.ThrowIfNull(Find_Shape_Model.FInd_ID + "号模型，无法在模型库中找到！");

            //进行图像校正后识别
            _ResultImage = _image.MapImage(_Model.Shape_Image_Rectified);

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
            Model_Camera_Pos.Throw("创建模型的相机位置未设定数据，请手动或者机器人通讯获取！").IfEquals(new Point_Model());
            Model_Plane_Pos.Throw("创建模型三维位置未设定数据，请手动或者机器人通讯获取！").IfEquals(new Point_Model());
            _Image.ThrowIfNull("图像未采集，不能识别！").Throw().IfFalse(_ => _.IsInitialized()); ;


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
                                new HTuple(Create_Shape_ModelXld.NumLevels),
                                (new HTuple(Create_Shape_ModelXld.AngleStart)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleExtent)).TupleRad(),
                                (new HTuple(Create_Shape_ModelXld.AngleStep)).TupleRad(),
                                Create_Shape_ModelXld.Metric.ToString());



                        //计算区域中心，设置模型中心点
                        Dilation_Region.AreaCenter(out double _row, out double _col);
                        _NccModel.SetNccModelOrigin(Model_2D_Origin.X - _row, Model_2D_Origin.Y - _col);


                        HHomMat2D _Tran = new HHomMat2D();
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
                GC.Collect();

            }
        }



        public void Dispose()
        {

            foreach (var _Shape in Shape_Mode_File_Model_List)
            {
                _Shape.Dispose();
            }
            GC.Collect();


        }
    }
}