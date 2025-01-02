using Generic_Extension;
using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Throw;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Image_Preprocessing_Process_SDK
    {

        public Halcon_Image_Preprocessing_Process_SDK(Preprocessing_Process_2D3D_Switch_Enum _Preprocessing_Process_2D3D)
        {

            Preprocessing_Process_2D3D = _Preprocessing_Process_2D3D;
            //Test
            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.MedianImage, Method_Run_Time = 156, V_1 = 156, V_2 = 123, V_3 = 6556 });

            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.MedianImage, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });

            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.Illuminate, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });
            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.GrayClosingRect, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });


        }
        public Halcon_Image_Preprocessing_Process_SDK(ObservableCollection<Preprocessing_Process_Lsit_Model> _)
        {
            Preprocessing_Process_List = _;

        }

        public ObservableCollection<Preprocessing_Process_Lsit_Model> Preprocessing_Process_List { set; get; } = new ObservableCollection<Preprocessing_Process_Lsit_Model>();




        public int Preprocessing_Process_List_RunTime { set; get; } = 0;



        public Preprocessing_Process_Lsit_Model? Preprocessing_Process_List_Selete { set; get; }


        //public delegate T ADD_delegate<T>(int _IN);


        //public ADD_delegate<object> ADD_Delegate_Model { set; get; }

        //private List<Action> Preprocessing_Process_Method { set; get; } = new List<Action>();





        public Preprocessing_Process_2D3D_Switch_Enum Preprocessing_Process_2D3D { set; get; } = Preprocessing_Process_2D3D_Switch_Enum.Camera_2D_Drives;





        /// <summary>
        /// 预处理流程插入创建方法
        /// </summary>
        /// <param name="_Work_Enum"></param>
        public void Preprocessing_Process_Work(Image_Preprocessing_Process_Work_Enum _Work_Enum)
        {




            switch (_Work_Enum)
            {
                case Image_Preprocessing_Process_Work_Enum.Up_Insertion:
                    if (Preprocessing_Process_List_Selete != null)
                    {

                        var _Index = Preprocessing_Process_List.IndexOf(Preprocessing_Process_List_Selete);
                        if (_Index < 0)
                        { Preprocessing_Process_New(0); }
                        else
                        {
                            Preprocessing_Process_New(_Index);
                        }
                    }
                    else
                    {
                        Preprocessing_Process_New(0);
                    }
                    break;
                case Image_Preprocessing_Process_Work_Enum.Down_Insertion:
                    if (Preprocessing_Process_List_Selete != null)
                    {
                        var a = Preprocessing_Process_List.IndexOf(Preprocessing_Process_List_Selete) + 1;
                        Preprocessing_Process_New(a);
                    }
                    else
                    {
                        Preprocessing_Process_New(Preprocessing_Process_List.Count);
                    }


                    break;
                case Image_Preprocessing_Process_Work_Enum.Delete_List:

                    Preprocessing_Process_Lsit_Delete();
                    break;
                default:

                    break;

            }
        }

        /// <summary>
        /// 图像流程集合删除
        /// </summary>
        public void Preprocessing_Process_Lsit_Delete()
        {
            Preprocessing_Process_List_Selete.ThrowIfNull("请选择需要删除的选项！");
            if (Preprocessing_Process_List_Selete != null)
            {
                Preprocessing_Process_List.Remove(Preprocessing_Process_List_Selete);
            }
        }


        /// <summary>
        /// 预处理流程开始
        /// </summary>
        public HImage Preprocessing_Process_Start(HImage _OldImage)
        {

            Image = new HImage(_OldImage);

            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;

            foreach (var item in Preprocessing_Process_List)
            {
                //开始单个处理时间
                DateTime startTime = DateTime.Now;


                Get_Preprocessing_Method(item.Image_Preprocessing_Process_Method, item.V_1, item.V_2, item.V_3, item.V_4, item.V_5, item.E_1, item.E_2, item.E_3, item.E_4, item.E_5).Invoke();

                // 计算时间差
                item.Method_Run_Time = (DateTime.Now - startTime).Milliseconds;
            }

            Preprocessing_Process_List_RunTime = (DateTime.Now - AllstartTime).Milliseconds;

            return Image;



        }



        public HObjectModel3D[] Preprocessing_Process_Start(HObjectModel3D[] _OldModel)
        {

            //Image = new HImage(_OldImage);

            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;

            foreach (var item in Preprocessing_Process_List)
            {
                //开始单个处理时间
                DateTime startTime = DateTime.Now;

                _OldModel = item.Get_3DResults_Method(_OldModel);

                //Get_Preprocessing_Method(item.Image_Preprocessing_Process_Method, item.V_1, item.V_2, item.V_3, item.V_4, item.V_5, item.E_1, item.E_2, item.E_3, item.E_4, item.E_5).Invoke();

                // 计算时间差
                item.Method_Run_Time = (DateTime.Now - startTime).Milliseconds;
            }

            Preprocessing_Process_List_RunTime = (DateTime.Now - AllstartTime).Milliseconds;

            return _OldModel;



        }

        /// <summary>
        /// 预处理流程创建位置方法
        /// </summary>
        /// <param name="_List_No"></param>
        public void Preprocessing_Process_New(int _List_No)
        {
            //插入新流程
            Preprocessing_Process_List.Insert(_List_No, new Preprocessing_Process_Lsit_Model() { });

            //新建排序
            for (int i = 0; i < Preprocessing_Process_List.Count; i++)
            {
                Preprocessing_Process_List[i].Method_Num = i;
            }
        }


        /// <summary>
        /// 获得对应预处理方法
        /// </summary>
        /// <param name="_Process"></param>
        /// <param name="V_1"></param>
        /// <param name="V_2"></param>
        /// <param name="V_3"></param>
        /// <param name="V_4"></param>
        /// <param name="V_5"></param>
        /// <param name="E_1"></param>
        /// <param name="E_2"></param>
        /// <param name="E_3"></param>
        /// <param name="E_4"></param>
        /// <param name="E_5"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Action Get_Preprocessing_Method(Image_Preprocessing_Process_Enum _Process, object? V_1 = null, object? V_2 = null, object? V_3 = null, object? V_4 = null, object? V_5 = null, string? E_1 = null, string? E_2 = null, string? E_3 = null, string? E_4 = null, string? E_5 = null)
        {


            switch (Preprocessing_Process_2D3D)
            {
                case Preprocessing_Process_2D3D_Switch_Enum.Camera_2D_Drives:

                    return _Process switch
                    {
                        Image_Preprocessing_Process_Enum.ScaleImageMax => () => ScaleImageMax(),
                        Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage(Enum.Parse<MedianImage_MaskType_Enum>(E_1!), int.Parse((string)V_1!), Enum.Parse<MedianImage_Margin_Enum>(E_2!)),
                        Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
                        Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
                        Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
                    };

                case Preprocessing_Process_2D3D_Switch_Enum.Camera_3D_Drives:
                    return _Process switch
                    {
                        Image_Preprocessing_Process_Enum.ScaleImageMax => () => ScaleImageMax(),
                        Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage(Enum.Parse<MedianImage_MaskType_Enum>(E_1!), int.Parse((string)V_1!), Enum.Parse<MedianImage_Margin_Enum>(E_2!)),
                        Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
                        Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
                        Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                        _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
                    };
                default:
                    throw new NotSupportedException($"未支持的 2D/3D 预处理类型: {Preprocessing_Process_2D3D}");
            }


        }





        /// <summary>
        /// 处理图像
        /// </summary>
        private HImage Image { set; get; } = new HImage();



        /// <summary>
        /// 三维模型集合
        /// </summary>
        private HObjectModel3D[] H3DModel { set; get; } = [];









        public void ScaleImageMax()
        {

            //HImage _Image_Results = new HImage();
            Image.ScaleImageMax();

            //HOperatorSet.ScaleImageMax(_Image, out HImage);
        }


        public void MedianRect(int MedianRect_MaskWidth, int MedianRect_MaskHeight)
        {

            DateTime startTime = DateTime.Now;



            MedianRect_MaskWidth.ThrowIfNull("参数不能：Null ");
            MedianRect_MaskHeight.ThrowIfNull("参数不能：Null ");
            //HOperatorSet.MedianRect(_HImage, out _HImage, _Find_Property.MedianRect_MaskWidth, _Find_Property.MedianRect_MaskHeight);

            Image = Image.MedianRect(MedianRect_MaskWidth, MedianRect_MaskHeight);

        }


        public void GrayOpeningRect(int maskHeight, int maskWidth)
        {
            maskHeight.ThrowIfNull("参数不能：Null ");
            maskWidth.ThrowIfNull("参数不能：Null ");

            Image = Image.GrayOpeningRect(maskHeight, maskWidth);


        }

        public void MedianImage(MedianImage_MaskType_Enum MaskType_Model, int Median_image_Radius, MedianImage_Margin_Enum Margin_Model)
        {
            MaskType_Model.ThrowIfNull("参数不能：Null ");
            Median_image_Radius.ThrowIfNull("参数不能：Null ");
            Margin_Model.ThrowIfNull("参数不能：Null ");


            Image = Image.MedianImage(MaskType_Model.ToString(), Median_image_Radius, Margin_Model.ToString());
        }


        public void Illuminate(int maskWidth, int maskHeight, double factor)
        {
            maskWidth.ThrowIfNull("参数不能：Null ");
            maskHeight.ThrowIfNull("参数不能：Null ");
            factor.ThrowIfNull("参数不能：Null ");


            Image = Image.Illuminate(maskWidth, maskHeight, factor);
        }



        public void Emphasize(int maskWidth, int maskHeight, double factor)
        {
            maskWidth.ThrowIfNull("参数不能：Null ");
            maskHeight.ThrowIfNull("参数不能：Null ");
            factor.ThrowIfNull("参数不能：Null ");

            Image = Image.Emphasize(maskWidth, maskHeight, factor);
        }


        public void GrayClosingRect(int maskHeight, int maskWidth)
        {
            maskHeight.ThrowIfNull("参数不能：Null ");
            maskWidth.ThrowIfNull("参数不能：Null ");

            Image = Image.GrayClosingRect(maskHeight, maskWidth);
        }


    }

    [AddINotifyPropertyChangedInterface]
    [Serializable]

    public class Preprocessing_Process_Lsit_Model
    {
        public   Preprocessing_Process_Lsit_Model()
        {



        }




        private Image_Preprocessing_Process_Enum _Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.ScaleImageMax;



        public Image_Preprocessing_Process_Enum Image_Preprocessing_Process_Method
        {
            get { return _Image_Preprocessing_Process_Method; }
            set
            {
                _Image_Preprocessing_Process_Method = value;
                Preprocessing_Process_Work_Initialization_Value(_Image_Preprocessing_Process_Method);
            }
        }

        private H3DObjectModel_Features_Enum _Preprocessing_Process_3DModel_Method = H3DObjectModel_Features_Enum.ConnectionObjectModel3d;



        public H3DObjectModel_Features_Enum Preprocessing_Process_3DModel_Method
        {
            get { return _Preprocessing_Process_3DModel_Method; }
            set
            {
                _Preprocessing_Process_3DModel_Method = value;
                Preprocessing_Process_Work_Initialization_Value(_Preprocessing_Process_3DModel_Method);
            }
        }




        public HObjectModel3D[] Get_3DResults_Method(HObjectModel3D[] _HObjectModel3D)
        {




            return Preprocessing_Process_3DModel_Method switch
            {
                H3DObjectModel_Features_Enum.ConnectionObjectModel3d => ConnectionObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.SelectObjectModel3d => SelectObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.SampleObjectModel3d => SampleObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.SurfaceNormalsObjectModel3d => SurfaceNormalsObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.SmoothObjectModel3d => SmoothObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.PrepareObjectModel3d => PrepareObjectModel3d?.Get_Results(_HObjectModel3D),
                H3DObjectModel_Features_Enum.TriangulateObjectModel3d => TriangulateObjectModel3d?.Get_Results(_HObjectModel3D),

                _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(Preprocessing_Process_3DModel_Method)),// 处理默认情况，或者根据需要抛出异常
            }?? throw new ArgumentException("预处理过程错误");





        }




        /// <summary>
        /// 选择流程方法参数初始化
        /// </summary>
        /// <param name="_Work_Enum"></param>
        public void Preprocessing_Process_Work_Initialization_Value(Enum _Work_Enum)
        {




            switch (_Work_Enum)
            {
                case Image_Preprocessing_Process_Enum.ScaleImageMax:



                    break;
                case Image_Preprocessing_Process_Enum.MedianRect:


                    V_1 = "9";
                    V_2 = "9";


                    break;
                case Image_Preprocessing_Process_Enum.GrayOpeningRect:
                    V_1 = "9";
                    V_2 = "9";

                    break;
                case Image_Preprocessing_Process_Enum.MedianImage:

                    V_1 = "1";

                    E_1 = MedianImage_MaskType_Enum.square.ToString();
                    E_2 = MedianImage_Margin_Enum.continued.ToString();



                    break;
                case Image_Preprocessing_Process_Enum.Illuminate:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = "0.8";

                    break;
                case Image_Preprocessing_Process_Enum.Emphasize:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = "0.8";

                    break;
                case Image_Preprocessing_Process_Enum.GrayClosingRect:
                    V_1 = "9";
                    V_2 = "9";

                    break;

                case H3DObjectModel_Features_Enum.ConnectionObjectModel3d:

                    ConnectionObjectModel3d = new ConnectionObjectModel3d_Function_Model() { };


                    break;
                case H3DObjectModel_Features_Enum.SelectObjectModel3d:

                    SelectObjectModel3d = new SelectObjectModel3d_Funtion_Model() { };



                    break;
                case H3DObjectModel_Features_Enum.SampleObjectModel3d:

                    SampleObjectModel3d = new SampleObjectModel3d_Function_Model() { };



                    break;

                case H3DObjectModel_Features_Enum.SurfaceNormalsObjectModel3d:

                    SurfaceNormalsObjectModel3d = new SurfaceNormalsObjectModel3d_Function_Model() { };

                    break;


                case H3DObjectModel_Features_Enum.SmoothObjectModel3d:

                    SmoothObjectModel3d = new SmoothObjectModel3d_Function_Model() { };

                    break;


                case H3DObjectModel_Features_Enum.PrepareObjectModel3d:

                    PrepareObjectModel3d = new PrepareObjectModel3d_Function_Model() { };

                    break;
                case H3DObjectModel_Features_Enum.TriangulateObjectModel3d:

                    TriangulateObjectModel3d = new TriangulateObjectModel3d_Function_Model() { };

                    break;
                    
            }

        }





        public   ConnectionObjectModel3d_Function_Model? ConnectionObjectModel3d { set; get; }

        public SelectObjectModel3d_Funtion_Model? SelectObjectModel3d { set; get; }



        public SampleObjectModel3d_Function_Model? SampleObjectModel3d { set; get; }


        public SurfaceNormalsObjectModel3d_Function_Model? SurfaceNormalsObjectModel3d { set; get; }



        public SmoothObjectModel3d_Function_Model? SmoothObjectModel3d { set; get; }


        public PrepareObjectModel3d_Function_Model? PrepareObjectModel3d { set; get; }



        public TriangulateObjectModel3d_Function_Model? TriangulateObjectModel3d { set; get; }


        //public Action? Action_Method { set; get; }

        /// <summary>
        /// 预处理方法运行序号
        /// </summary>
        public int Method_Num { set; get; } = 0;
        /// <summary>
        /// 耗时用毫秒单位
        /// </summary>
        public int Method_Run_Time { set; get; } = 0;


        public string? V_1 { set; get; } = default;
        public string? V_2 { set; get; } = default;
        public string? V_3 { set; get; } = default;
        public string? V_4 { set; get; } = default;
        public string? V_5 { set; get; } = default;
        public string? V_6 { set; get; } = default;
        public string? V_7 { set; get; } = default;
        public string? V_8 { set; get; } = default;
        public string? V_9 { set; get; } = default;
        public string? V_10 { set; get; } = default;
        public string? V_11 { set; get; } = default;
        public string? V_12 { set; get; } = default;
        public string? V_13 { set; get; } = default;
        public string? V_14 { set; get; } = default;
        public string? V_15 { set; get; } = default;


        public string? E_1 { set; get; } = default;
        public string? E_2 { set; get; } = default;
        public string? E_3 { set; get; } = default;
        public string? E_4 { set; get; } = default;
        public string? E_5 { set; get; } = default;
        public string? E_6 { set; get; } = default;
        public string? E_7 { set; get; } = default;
        public string? E_8 { set; get; } = default;
        public string? E_9 { set; get; } = default;
        public string? E_10 { set; get; } = default;

    }


    [AddINotifyPropertyChangedInterface]
    public class ConnectionObjectModel3d_Function_Model
    {



        public ConnectionObjectModel3d_Feature_Enum Feature { set; get; } = ConnectionObjectModel3d_Feature_Enum.distance_3d;


        public string Value { set; get; } = 0.005.ToString();

        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            return HObjectModel3D.ConnectionObjectModel3d(_Model3D, Feature.ToString().ToLower(), Value);

        }

    }

    [AddINotifyPropertyChangedInterface]
    public class SelectObjectModel3d_Funtion_Model
    {
        public SelectObjectModel3d_Feature_Enum Feature { set; get; } = SelectObjectModel3d_Feature_Enum.num_points;

        public SelectObjectModel3d_Operation_Enum Operation { set; get; } = SelectObjectModel3d_Operation_Enum.and;
      
        [XmlElement("minValue")]
        public string minValue { set; get; } = 500.ToString();

        [XmlElement("maxValue")]
        public string maxValue { set; get; } = "max";


        public int Max { set; get; } = 100000;
        public int Min { set; get; } = 0;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            return HObjectModel3D.SelectObjectModel3d(_Model3D, Feature.ToString().ToLower(), Operation.ToString().ToLower(), minValue.ToString(), maxValue.ToString());

        }


    }


    [AddINotifyPropertyChangedInterface]
    public class SampleObjectModel3d_Function_Model
    {

        public SampleObjectModel3d_Method_Enum Method { set; get; } = SampleObjectModel3d_Method_Enum.fast;

        public double SampleDistance { set; get; } = 0.05;


        public double max_angle_diff { set; get; } = 180;


        public double min_num_points { set; get; } = 5;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            return HObjectModel3D.SampleObjectModel3d(_Model3D, Method.ToString().ToLower(), SampleDistance, new HTuple([nameof(max_angle_diff), nameof(min_num_points)]), new HTuple([max_angle_diff.ToString(), min_num_points.ToString()]));

        }

    }

    [AddINotifyPropertyChangedInterface]
    public class SurfaceNormalsObjectModel3d_Function_Model
    {

        public SurfaceNormalsObjectModel3d_Method_Enum Method { set; get; } = SurfaceNormalsObjectModel3d_Method_Enum.mls;


        public double mls_kNN { set; get; } = 60;

        public double mls_abs_sigma { set; get; } = 0.001;

        public int mls_order { set; get; } = 2;

        public double mls_relative_sigma { set; get; } = 1;

        public bool mls_force_inwards { set; get; } = true;



        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            return HObjectModel3D.SurfaceNormalsObjectModel3d(
                _Model3D,
                Method.ToString().ToLower(),
                new HTuple([nameof(mls_kNN), nameof(mls_abs_sigma), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                new HTuple([mls_kNN.ToString(), mls_abs_sigma.ToString(), mls_order.ToString(), mls_relative_sigma.ToString(), mls_force_inwards.ToString().ToLower()]));

        }

    }

    [AddINotifyPropertyChangedInterface]
    public class SmoothObjectModel3d_Function_Model
    {

        public SmoothObjectModel3d_Method_Enum Method { set; get; } = SmoothObjectModel3d_Method_Enum.mls;


        public double mls_kNN { set; get; } = 60;


        public int mls_order { set; get; } = 2;
        public double mls_abs_sigma { set; get; } = 0.001;

        public double mls_relative_sigma { set; get; } = 1;

        public bool mls_force_inwards { set; get; } = true;


        public SmoothObjectModel3d_Xyz_Mapping_Filter_Enum xyz_mapping_filter { set; get; } = SmoothObjectModel3d_Xyz_Mapping_Filter_Enum.median_separate;


        public int xyz_mapping_mask_width { set; get; } = 3;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {


            switch (Method)
            {
                case SmoothObjectModel3d_Method_Enum.mls:



                    return HObjectModel3D.SmoothObjectModel3d(
                        _Model3D,
                        Method.ToString().ToLower(),
                        new HTuple([nameof(mls_kNN), nameof(mls_abs_sigma), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                        new HTuple([mls_kNN.ToString(), mls_abs_sigma.ToString(), mls_order.ToString(), mls_relative_sigma.ToString(), mls_force_inwards.ToString().ToLower()]));

                case SmoothObjectModel3d_Method_Enum.xyz_mapping or SmoothObjectModel3d_Method_Enum.xyz_mapping_compute_normals:

                    return HObjectModel3D.SmoothObjectModel3d(
                        _Model3D,
                        Method.ToString().ToLower(),
                        new HTuple([nameof(xyz_mapping_filter), nameof(xyz_mapping_mask_width)]),
                        new HTuple([xyz_mapping_filter.ToString(), xyz_mapping_mask_width.ToString()]));

                default: throw new ArgumentException("参数错误！");


            }


        }

    }

    [AddINotifyPropertyChangedInterface]
    public class PrepareObjectModel3d_Function_Model
    {

        public PrepareObjectModel3d_PurPose_Enum Purpose { set; get; } = PrepareObjectModel3d_PurPose_Enum.segmentation;


        public bool overwriteData { set; get; } = true;

        public PrepareObjectModel3d_DistanceTo_Enum distance_to { set; get; } = PrepareObjectModel3d_DistanceTo_Enum.auto;


        public PrepareObjectModel3d_Method_Enum method { set; get; } = PrepareObjectModel3d_Method_Enum.auto;


        public int max_distance { set; get; } = 0;

        public double sampling_dist_rel { set; get; } = 0.03;

        public int sampling_dist_abs { set; get; } = 100;

        public int xyz_map_width { set; get; } = 4024;

        public int max_area_holes { set; get; } = 100;



        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {


            switch (Purpose)
            {
                case PrepareObjectModel3d_PurPose_Enum.shape_based_matching_3d:



                    HObjectModel3D.PrepareObjectModel3d(
                          _Model3D,
                         Purpose.ToString().ToLower(),
                         overwriteData.ToString().ToLower(),
                         new HTuple(),
                         new HTuple());
                    break;


                case PrepareObjectModel3d_PurPose_Enum.segmentation:

                    HObjectModel3D.PrepareObjectModel3d(
                    _Model3D,
                   Purpose.ToString().ToLower(),
                   overwriteData.ToString().ToLower(),
                   new HTuple([nameof(max_area_holes)]),
                   new HTuple([max_area_holes.ToString()]));
                    break;
                case PrepareObjectModel3d_PurPose_Enum.distance_computation:

                    HObjectModel3D.PrepareObjectModel3d(
                      _Model3D,
                     Purpose.ToString().ToLower(),
                     overwriteData.ToString().ToLower(),
                     new HTuple([nameof(distance_to), nameof(method), nameof(max_distance), nameof(sampling_dist_rel), nameof(sampling_dist_abs)]),
                     new HTuple([distance_to.ToString(), method.GetStringValue(), max_distance.ToString(), sampling_dist_rel.ToString(), sampling_dist_abs.ToString()]));

                    break;
                case PrepareObjectModel3d_PurPose_Enum.gen_xyz_mapping:

                    HObjectModel3D.PrepareObjectModel3d(
                        _Model3D,
                       Purpose.ToString().ToLower(),
                       overwriteData.ToString().ToLower(),
                       new HTuple([nameof(xyz_map_width)]),
                       new HTuple([xyz_map_width.ToString()]));

                    break;

            }



            return _Model3D;



        }

    }





    [AddINotifyPropertyChangedInterface]
    public class TriangulateObjectModel3d_Function_Model
    {

        public TriangulateObjectModel3d_Method_Enum Method { set; get; } = TriangulateObjectModel3d_Method_Enum.greedy;




        public int xyz_mapping_max_area_holes { set; get; } = 10;

        public int xyz_mapping_max_view_angle { set; get; } = 90;

        public bool xyz_mapping_max_view_dir_x { set; get; } = false;
        public bool xyz_mapping_max_view_dir_y { set; get; } = false;
        public bool xyz_mapping_max_view_dir_z { set; get; } = true;
        public bool xyz_mapping_output_all_points { set; get; } = false;


        public int greedy_kNN { set; get; } = 40;

        public TriangulateObjectModel3d_Greedy_Radius_Type_Enum greedy_radius_type { set; get; } = TriangulateObjectModel3d_Greedy_Radius_Type_Enum.auto;

        public double greedy_radius_value { set; get; } = 0.01;

        public int greedy_neigh_orient_tol { set; get; } = 30;

        public bool greedy_neigh_orient_consistent { set; get; } = false;

        public int greedy_neigh_latitude_tol { set; get; } = 30;

        public double greedy_neigh_vertical_tol { set; get; } = 0.1;


        public object greedy_hole_filling { set; get; } = 40;

        public bool greedy_fix_flips { get; set; } = true;

        public bool greedy_prefetch_neighbors { set; get; } = true;

        public int greedy_mesh_erosion { set; get; } = 3;

        public int greedy_mesh_dilation { set; get; } = 2;

        public object greedy_remove_small_surfaces { set; get; } = false;


        public object greedy_timeout { set; get; } = false;


        public bool greedy_suppress_timeout_error { set; get; } = false;

        public bool greedy_output_all_points { set; get; } = false;

        public TriangulateObjectModel3d_Information_Enum information { set; get; } = TriangulateObjectModel3d_Information_Enum.num_triangles;


        public int implicit_octree_depth { set; get; } = 6;


        public int implicit_solver_depth { set; get; } = 6;

        public int implicit_min_num_samples { set; get; } = 1;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            HTuple _information;


            switch (Method)
            {
                case TriangulateObjectModel3d_Method_Enum.greedy:


                    return HObjectModel3D.TriangulateObjectModel3d(
  _Model3D,
  Method.ToString().ToLower(),
  new HTuple([
                      nameof(greedy_kNN),
                      nameof(greedy_radius_type),
                      nameof(greedy_radius_value),
                      nameof(greedy_neigh_orient_tol),
                      nameof(greedy_neigh_orient_consistent),
                      nameof(greedy_neigh_latitude_tol),
                      nameof(greedy_neigh_vertical_tol),
                      nameof(greedy_hole_filling),
                      nameof(greedy_fix_flips),
                      nameof(greedy_prefetch_neighbors),
                      nameof(greedy_mesh_erosion),
                      nameof(greedy_mesh_dilation),
                      nameof(greedy_remove_small_surfaces),
                      nameof(greedy_timeout),
                      nameof(greedy_suppress_timeout_error),
                      nameof(greedy_output_all_points),
                      nameof(information)
                      ]),
  new HTuple([
                      greedy_kNN.ToString(),
                      greedy_radius_type.ToString().ToLower(),
                      greedy_radius_value.ToString(),
                      greedy_neigh_orient_tol.ToString(),
                      greedy_neigh_orient_consistent.ToString().ToLower(),
                      greedy_neigh_latitude_tol.ToString(),
                      greedy_neigh_vertical_tol.ToString(),
                      greedy_hole_filling?.ToString()?.ToLower(),
                      greedy_fix_flips.ToString(),
                      greedy_prefetch_neighbors.ToString(),
                      greedy_mesh_erosion.ToString(),
                      greedy_mesh_dilation.ToString(),
                      greedy_remove_small_surfaces?.ToString()?.ToLower(),
                      greedy_timeout?.ToString()?.ToLower(),
                      greedy_suppress_timeout_error.ToString().ToLower(),
                      greedy_output_all_points.ToString(),
                      information.ToString()]),
  out _information);

                    ;
                case TriangulateObjectModel3d_Method_Enum.Implicit:


                    return HObjectModel3D.TriangulateObjectModel3d(
                _Model3D,
                Method.ToString().ToLower(),
                new HTuple([nameof(implicit_octree_depth), nameof(implicit_solver_depth), nameof(implicit_min_num_samples), nameof(information)]),
                new HTuple([implicit_octree_depth.ToString(), implicit_solver_depth.ToString(), implicit_min_num_samples.ToString(), information.ToString()]),
                out _information);


                case TriangulateObjectModel3d_Method_Enum.polygon_triangulation:


                    return HObjectModel3D.TriangulateObjectModel3d(
             _Model3D,
             Method.ToString().ToLower(),
             new HTuple([nameof(information)]),
             new HTuple([information.ToString()]),
             out _information);


                case TriangulateObjectModel3d_Method_Enum.xyz_mapping:


                    return HObjectModel3D.TriangulateObjectModel3d(
          _Model3D,
          Method.ToString().ToLower(),
          new HTuple([nameof(xyz_mapping_max_area_holes), nameof(xyz_mapping_max_view_angle), nameof(xyz_mapping_max_view_dir_x), nameof(xyz_mapping_max_view_dir_y), nameof(xyz_mapping_max_view_dir_z), nameof(xyz_mapping_output_all_points)]),
          new HTuple([xyz_mapping_max_area_holes.ToString(), HTuple.TupleRand(xyz_mapping_max_view_angle).ToString(), Convert.ToInt32(xyz_mapping_max_view_dir_x).ToString(), Convert.ToInt32(xyz_mapping_max_view_dir_y).ToString(), Convert.ToInt32(xyz_mapping_max_view_dir_z).ToString(), xyz_mapping_output_all_points.ToString().ToLower()]),
          out _information);




                default: throw new ArgumentException("参数错误！");
            }





        }

    }



    public enum Image_Preprocessing_Process_Enum
    {
        [Description("灰度动调分布_ScaleImageMax")]
        ScaleImageMax,
        [Description("中值滤波器_MedianRect")]
        MedianRect,
        [Description("矩形开运算_GrayOpeningRect")]
        GrayOpeningRect,
        [Description("矩形闭运算_GrayClosingRect")]
        GrayClosingRect,
        [Description("中值滤波器_MedianImage")]
        MedianImage,
        [Description("高频增强对比_Illuminate")]
        Illuminate,
        [Description("增强边缘_Emphasize")]
        Emphasize,

    }


    public enum H3DObjectModel_Features_Enum
    {
        [Description("连通3D模型_Connection")]
        ConnectionObjectModel3d,
        [Description("筛选3D模型_Select")]
        SelectObjectModel3d,
        [Description("重采样3D模型_Sample")]
        SampleObjectModel3d,
        [Description("计算法线3D模型_SurfaceNormals")]
        SurfaceNormalsObjectModel3d,
        [Description("平滑3D模型_Smooth")]
        SmoothObjectModel3d,
        [Description("预准备3D模型_Prepare")]
        PrepareObjectModel3d,
        [Description("三角化3D模型_Triangulate")]
        TriangulateObjectModel3d
    }




    public enum Image_Preprocessing_Process_Work_Enum
    {
        [Description("上方插入")]
        Up_Insertion,
        [Description("上方插入")]
        Down_Insertion,
        [Description("删除选择")]
        Delete_List,

    }


    public enum Preprocessing_Process_2D3D_Switch_Enum
    {

        [Description("2D设备")]
        Camera_2D_Drives,
        [Description("3D设备")]
        Camera_3D_Drives



    }





}
