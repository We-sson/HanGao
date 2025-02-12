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

        public Halcon_Image_Preprocessing_Process_SDK()
        {

            //Preprocessing_Process_2D3D = _Preprocessing_Process_2D3D;
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





        private bool _IsSingleStep = false;

        /// <summary>
        /// 单步模式
        /// </summary>
        public bool IsSingleStep
        {
            get { return _IsSingleStep; }
            set
            {
                _IsSingleStep = value;
                if (value) { IsSingleStep_Number = 0; }
            }
        }



        /// <summary>
        /// 单步当前步骤
        /// </summary>
        public int IsSingleStep_Number { set; get; } = 0;


        public int IsSingleStep_MaxNumber { set; get; } = 0;


        public Preprocessing_Process_Lsit_Model? Preprocessing_Process_List_Selete { set; get; }


        //public delegate T ADD_delegate<T>(int _IN);


        //public ADD_delegate<object> ADD_Delegate_Model { set; get; }

        //private List<Action> Preprocessing_Process_Method { set; get; } = new List<Action>();





        //public Preprocessing_Process_2D3D_Switch_Enum Preprocessing_Process_2D3D { set; get; } = Preprocessing_Process_2D3D_Switch_Enum.Camera_2D_Drives;





        /// <summary>
        /// 预处理流程插入创建方法
        /// </summary>
        /// <param name="_Work_Enum"></param>
        public void Preprocessing_Process_Work(Enum _Work_Enum)
        {




            switch (_Work_Enum)
            {
                case Image_Preprocessing_Process_Work_Enum.Up_Insertion:
                    Preprocessing_Process_Up_Insertion();
                    break;
                case Image_Preprocessing_Process_Work_Enum.Down_Insertion:

                    Preprocessing_Process_Down_Insertion();

                    break;
                case Image_Preprocessing_Process_Work_Enum.Delete_List:

                    Preprocessing_Process_Lsit_Delete();
                    break;

                case Stereo_3D_Preprocessing_Process_Work_Enum.Up_Insertion:
                    Preprocessing_Process_Up_Insertion();
                    break;
                case Stereo_3D_Preprocessing_Process_Work_Enum.Down_Insertion:

                    Preprocessing_Process_Down_Insertion();

                    break;
                case Stereo_3D_Preprocessing_Process_Work_Enum.Delete_List:

                    Preprocessing_Process_Lsit_Delete();
                    break;
                default:

                    break;

            }
        }


        /// <summary>
        /// 预处理流程上插入
        /// </summary>
        public void Preprocessing_Process_Up_Insertion()
        {
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
        }


        /// <summary>
        /// 预处理流程下插入
        /// </summary>
        public void Preprocessing_Process_Down_Insertion()
        {
            if (Preprocessing_Process_List_Selete != null)
            {
                var a = Preprocessing_Process_List.IndexOf(Preprocessing_Process_List_Selete) + 1;
                Preprocessing_Process_New(a);
            }
            else
            {
                Preprocessing_Process_New(Preprocessing_Process_List.Count);
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
        public HImage Preprocessing_Process_Start(HImage _OldImage, ObservableCollection<Preprocessing_Process_Lsit_Model> _Preprocessing_Process_List)
        {

            //Image = new HImage(_OldImage);




            ///流程状态复位
            _Preprocessing_Process_List.ToList().ForEach(list => list.Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Int);


            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;

            foreach (var item in _Preprocessing_Process_List)
            {
                //开始单个处理时间
                DateTime startTime = DateTime.Now;




                if (IsSingleStep && IsSingleStep_Number > _Preprocessing_Process_List.Count - 1)
                {
                    IsSingleStep_Number = 0;
                    item.Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Int;

                }


                if (IsSingleStep && item.Method_Num > IsSingleStep_Number)
                {


                    break;
                }

                _OldImage = item.Get_23DResults_Method(_OldImage);





                // 计算时间差
                item.Method_Run_Time = (DateTime.Now - startTime).Milliseconds;
            }

            Preprocessing_Process_List_RunTime = (DateTime.Now - AllstartTime).Milliseconds;

            return _OldImage;


        }





        /// <summary>
        /// 预处理流程开始
        /// </summary>
        public (HImage, HImage, HImage, HImage) Preprocessing_Process_Start(HImage _OldImage1, HImage _OldImage2, HImage _OldImage3, HImage _OldImage4, ObservableCollection<Preprocessing_Process_Lsit_Model> _List1, ObservableCollection<Preprocessing_Process_Lsit_Model> _List2, ObservableCollection<Preprocessing_Process_Lsit_Model> _List3, ObservableCollection<Preprocessing_Process_Lsit_Model> _List4, H3DStereo_Image_Type_Enum Image_Type, H3DStereo_CameraDrives_Type_Enum CameraDrives_Type)
        {

            //Image = new HImage(_OldImage);


            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;






            //IsSingleStep_MaxNumber = new[] { _List1.Count, _List2.Count, _List3.Count, _List4.Count }.Max();

            if (IsSingleStep)
            {

                switch (Image_Type)
                {
                    case H3DStereo_Image_Type_Enum.点云图像:

                        switch (CameraDrives_Type)
                        {
                            case H3DStereo_CameraDrives_Type_Enum.Camera_0:

                                _OldImage1 = Preprocessing_Process_Start(_OldImage1, _List1);

                                break;
                            case H3DStereo_CameraDrives_Type_Enum.Camera_1:
                                _OldImage2 = Preprocessing_Process_Start(_OldImage2, _List2);

                                break;

                        }

                        break;
                    case H3DStereo_Image_Type_Enum.深度图像:

                        switch (CameraDrives_Type)
                        {
                            case H3DStereo_CameraDrives_Type_Enum.Camera_0:
                                _OldImage3 = Preprocessing_Process_Start(_OldImage3, _List3);

                                break;
                            case H3DStereo_CameraDrives_Type_Enum.Camera_1:
                                _OldImage4 = Preprocessing_Process_Start(_OldImage4, _List4);

                                break;

                        }
                        break;

                }



            }
            else
            {



                _OldImage1 = Preprocessing_Process_Start(_OldImage1, _List1);
                _OldImage2 = Preprocessing_Process_Start(_OldImage2, _List2);



                _OldImage3 = Preprocessing_Process_Start(_OldImage3, _List3);
                _OldImage4 = Preprocessing_Process_Start(_OldImage4, _List4);


            }


            Preprocessing_Process_List_RunTime = (DateTime.Now - AllstartTime).Milliseconds;
            GC.Collect();


            if (IsSingleStep)
            {
                IsSingleStep_Number++;

            }


            return (_OldImage1, _OldImage2, _OldImage3, _OldImage4);



        }






        public HObjectModel3D[] Preprocessing_Process_Start(HObjectModel3D[] _OldModel, ObservableCollection<Preprocessing_Process_Lsit_Model> _Preprocessing_Process_List)
        {


            ///流程状态复位
            _Preprocessing_Process_List.ToList().ForEach(list => list.Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Int);


            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;



            foreach (var item in _Preprocessing_Process_List)
            {
                //开始单个处理时间
                DateTime startTime = DateTime.Now;



                if (IsSingleStep && IsSingleStep_Number > _Preprocessing_Process_List.Count - 1)
                {
                    IsSingleStep_Number = 0;
                    item.Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Int;

                }


                if (IsSingleStep && item.Method_Num > IsSingleStep_Number)
                {


                    break;
                }



                _OldModel = item.Get_23DResults_Method(_OldModel);


            
                // 计算时间差
                item.Method_Run_Time = (DateTime.Now - startTime).Milliseconds;
            }



            if (IsSingleStep)
            {
                IsSingleStep_Number++;

            }

            Preprocessing_Process_List_RunTime = (DateTime.Now - AllstartTime).Milliseconds;

            GC.Collect();
            return _OldModel;



        }

        /// <summary>
        /// 预处理流程创建位置方法
        /// </summary>
        /// <param name="_List_No"></param>
        public void Preprocessing_Process_New(int _List_No)
        {
            //插入新流程
            Preprocessing_Process_List.Insert(_List_No, new Preprocessing_Process_Lsit_Model() { Preprocessing_Process_2DModel_Method = Image_Preprocessing_Process_Enum.ScaleImageMax, Preprocessing_Process_3DModel_Method = H3DObjectModel_Features_Enum.ConnectionObjectModel3d });

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
        //public Action Get_Preprocessing_Method(Image_Preprocessing_Process_Enum _Process, object? V_1 = null, object? V_2 = null, object? V_3 = null, object? V_4 = null, object? V_5 = null, string? E_1 = null, string? E_2 = null, string? E_3 = null, string? E_4 = null, string? E_5 = null)
        //{


        //    switch (Preprocessing_Process_2D3D)
        //    {
        //        case Preprocessing_Process_2D3D_Switch_Enum.Camera_2D_Drives:

        //            return _Process switch
        //            {
        //                Image_Preprocessing_Process_Enum.ScaleImageMax => () => ScaleImageMax(),
        //                Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage(Enum.Parse<MedianImage_MaskType_Enum>(E_1!), int.Parse((string)V_1!), Enum.Parse<MedianImage_Margin_Enum>(E_2!)),
        //                Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
        //                Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
        //                Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
        //            };

        //        case Preprocessing_Process_2D3D_Switch_Enum.Camera_3D_Drives:
        //            return _Process switch
        //            {
        //                Image_Preprocessing_Process_Enum.ScaleImageMax => () => ScaleImageMax(),
        //                Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage(Enum.Parse<MedianImage_MaskType_Enum>(E_1!), int.Parse((string)V_1!), Enum.Parse<MedianImage_Margin_Enum>(E_2!)),
        //                Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
        //                Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
        //                Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
        //                _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
        //            };
        //        default:
        //            throw new NotSupportedException($"未支持的 2D/3D 预处理类型: {Preprocessing_Process_2D3D}");
        //    }


        //}





        /// <summary>
        /// 处理图像
        /// </summary>
        private HImage Image { set; get; } = new HImage();



        /// <summary>
        /// 三维模型集合
        /// </summary>
        private HObjectModel3D[] H3DModel { set; get; } = [];



    }

    [AddINotifyPropertyChangedInterface]
    [Serializable]

    public class Preprocessing_Process_Lsit_Model
    {
        public Preprocessing_Process_Lsit_Model()
        {

            Preprocessing_Process_Work_Initialization_Value(Preprocessing_Process_2DModel_Method);
            Preprocessing_Process_Work_Initialization_Value(Preprocessing_Process_3DModel_Method);


        }




        //private Image_Preprocessing_Process_Enum _Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.ScaleImageMax;



        //public Image_Preprocessing_Process_Enum Image_Preprocessing_Process_Method
        //{
        //    get { return _Image_Preprocessing_Process_Method; }
        //    set
        //    {
        //        _Image_Preprocessing_Process_Method = value;
        //        Preprocessing_Process_Work_Initialization_Value(_Image_Preprocessing_Process_Method);
        //    }
        //}





        private Image_Preprocessing_Process_Enum _Preprocessing_Process_2DModel_Method = Image_Preprocessing_Process_Enum.ScaleImageMax;

        public Image_Preprocessing_Process_Enum Preprocessing_Process_2DModel_Method
        {
            get { return _Preprocessing_Process_2DModel_Method; }
            set
            {
                _Preprocessing_Process_2DModel_Method = value;
                Preprocessing_Process_Work_Initialization_Value(value);

            }
        }



        private H3DObjectModel_Features_Enum _Preprocessing_Process_3DModel_Method = H3DObjectModel_Features_Enum.ConnectionObjectModel3d;

        public H3DObjectModel_Features_Enum Preprocessing_Process_3DModel_Method
        {
            get { return _Preprocessing_Process_3DModel_Method; }
            set
            {
                _Preprocessing_Process_3DModel_Method = value;
                Preprocessing_Process_Work_Initialization_Value(value);
            }
        }



        /// <summary>
        /// 流程状态
        /// </summary>
        public Preprocessing_Process_Run_State_Enum Preprocessing_Process_Run_State { set; get; } = Preprocessing_Process_Run_State_Enum.Int;



        public HObjectModel3D[] Get_23DResults_Method(HObjectModel3D[] _HObjectModel3D)
        {




            Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Runing;

            try
            {



                HObjectModel3D[] Result ;

                switch (Preprocessing_Process_3DModel_Method)
                {
                    case H3DObjectModel_Features_Enum.ConnectionObjectModel3d:

                        Result = ConnectionObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    case H3DObjectModel_Features_Enum.SelectObjectModel3d:
                        Result = SelectObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    case H3DObjectModel_Features_Enum.SampleObjectModel3d:
                        Result = SampleObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;

                        break;
                    case H3DObjectModel_Features_Enum.SurfaceNormalsObjectModel3d:
                        Result = SurfaceNormalsObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    case H3DObjectModel_Features_Enum.SmoothObjectModel3d:
                        Result = SmoothObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    case H3DObjectModel_Features_Enum.PrepareObjectModel3d:
                        Result = PrepareObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    case H3DObjectModel_Features_Enum.TriangulateObjectModel3d:
                        Result = TriangulateObjectModel3d?.Get_Results(_HObjectModel3D) ?? _HObjectModel3D;
                        break;
                    default:
                        throw new ArgumentException("无效的预处理过程枚举值。", nameof(Preprocessing_Process_3DModel_Method));


                }

                Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.End;
                return Result;


            }
            catch (Exception e)
            {
                Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Error;
                throw new Exception($"图选处理{Preprocessing_Process_3DModel_Method}:方法错误！,原因：{e.Message}");
            }

        }


        public HImage Get_23DResults_Method(HImage _HObjectModel2D)
        {

            Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Runing;

            try
            {
                HImage hImage = new HImage();

                switch (Preprocessing_Process_2DModel_Method)
                {
                    case Image_Preprocessing_Process_Enum.ScaleImageMax:


                        hImage = ScaleImageMax?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.MedianRect:
                        hImage = MedianRect?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.GrayOpeningRect:
                        hImage = GrayOpeningRect?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.GrayClosingRect:
                        hImage = GrayClosingRect?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.MedianImage:
                        hImage = MedianImage?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.Illuminate:
                        hImage = Illuminate?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    case Image_Preprocessing_Process_Enum.Emphasize:
                        hImage = Emphasize?.Get_Results(_HObjectModel2D) ?? _HObjectModel2D;
                        break;
                    default:
                        throw new ArgumentException("无效的预处理过程枚举值。", nameof(Preprocessing_Process_2DModel_Method));
                }

                Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.End;

                return hImage;
            }
            catch (Exception e)
            {
                Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Error;
                throw new Exception($"图选处理{Preprocessing_Process_2DModel_Method}:方法错误！,原因：{e.Message}");
            }




        }



        /// <summary>
        /// 选择流程方法参数初始化
        /// </summary>
        /// <param name="_Work_Enum"></param>
        public void Preprocessing_Process_Work_Initialization_Value(Enum _Work_Enum)
        {


            Preprocessing_Process_Run_State = Preprocessing_Process_Run_State_Enum.Int;


            switch (_Work_Enum)
            {
                case Image_Preprocessing_Process_Enum.ScaleImageMax:


                    ScaleImageMax = new ScaleImageMax_Function_Model();

                    break;
                case Image_Preprocessing_Process_Enum.MedianRect:


                    MedianRect = new MedianRect_Function_Model();


                    break;
                case Image_Preprocessing_Process_Enum.GrayOpeningRect:
                    GrayOpeningRect = new GrayOpeningRect_Function_Model();

                    break;
                case Image_Preprocessing_Process_Enum.MedianImage:

                    MedianImage = new MedianImage_Function_Model();

                    break;
                case Image_Preprocessing_Process_Enum.Illuminate:
                    Illuminate = new Illuminate_Function_Model();

                    break;
                case Image_Preprocessing_Process_Enum.Emphasize:
                    Emphasize = new Emphasize_Function_Model();

                    break;
                case Image_Preprocessing_Process_Enum.GrayClosingRect:
                    GrayClosingRect = new GrayClosingRect_Function_Model();

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


        public Emphasize_Function_Model? Emphasize { set; get; }


        public Illuminate_Function_Model? Illuminate { set; get; }

        public MedianImage_Function_Model? MedianImage { set; get; }

        public GrayOpeningRect_Function_Model? GrayOpeningRect { set; get; }

        public GrayClosingRect_Function_Model? GrayClosingRect { set; get; }

        public MedianRect_Function_Model? MedianRect { set; get; }

        public ScaleImageMax_Function_Model? ScaleImageMax { set; get; }

        public ConnectionObjectModel3d_Function_Model? ConnectionObjectModel3d { set; get; }

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


    }



    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class ScaleImageMax_Function_Model
    {

        public HImage Get_Results(HImage _Model3D)
        {

            HImage _Results = new HImage(_Model3D.ScaleImageMax());
            _Model3D.Dispose();

            return _Results;

        }


    }
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class MedianRect_Function_Model
    {
        [XmlAttribute()]
        public int MaskWidth { set; get; } = 11;
        [XmlAttribute()]

        public int MaskHeight { set; get; } = 11;





        public HImage Get_Results(HImage _Model3D)
        {

            HImage _Results = new HImage(_Model3D.MedianRect(MaskWidth, MaskHeight));

            _Model3D.Dispose();

            return _Results;

        }


    }


    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class MedianImage_Function_Model
    {
        [XmlAttribute()]

        public int Radius { set; get; } = 11;


        [XmlAttribute()]

        public MedianImage_MaskType_Enum MaskType { set; get; } = MedianImage_MaskType_Enum.square;

        [XmlAttribute()]

        public MedianImage_Margin_Enum Margin { set; get; } = MedianImage_Margin_Enum.mirrored;

        public HImage Get_Results(HImage _Model3D)
        {

            HImage _Results = new HImage(_Model3D.MedianImage(MaskType.ToString().ToLower(), Radius, Margin.ToString().ToLower()));
            _Model3D.Dispose();

            return _Results;

        }


    }
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class GrayOpeningRect_Function_Model
    {
        [XmlAttribute()]

        public int MaskWidth { set; get; } = 11;
        [XmlAttribute()]

        public int MaskHeight { set; get; } = 11;





        public HImage Get_Results(HImage _Model3D)
        {

            HImage _Results = new HImage(_Model3D.GrayOpeningRect(MaskWidth, MaskHeight));
            _Model3D.Dispose();

            return _Results;

        }


    }
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class GrayClosingRect_Function_Model
    {
        [XmlAttribute()]

        public int MaskWidth { set; get; } = 11;
        [XmlAttribute()]

        public int MaskHeight { set; get; } = 11;





        public HImage Get_Results(HImage _Model3D)
        {
            HImage _Results = new HImage(_Model3D.GrayClosingRect(MaskWidth, MaskHeight));
            _Model3D.Dispose();

            return _Results;

        }


    }

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Illuminate_Function_Model
    {
        [XmlAttribute()]

        public int MaskWidth { set; get; } = 11;
        [XmlAttribute()]

        public int MaskHeight { set; get; } = 11;

        [XmlAttribute()]

        public double Factor { set; get; } = 0.2;


        public HImage Get_Results(HImage _Model3D)
        {
            HImage _Results = new HImage(_Model3D.Illuminate(MaskWidth, MaskHeight, Factor));
            _Model3D.Dispose();

            return _Results;
        }


    }


    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Emphasize_Function_Model
    {

        public Emphasize_Function_Model() { }

        public int MaskWidth { set; get; } = 11;
        public int MaskHeight { set; get; } = 11;


        public double Factor { set; get; } = 0.2;


        public HImage Get_Results(HImage _Model3D)
        {
            HImage _Results = new HImage(_Model3D.Emphasize(MaskWidth, MaskHeight, Factor));
            _Model3D.Dispose();
            return _Results;

        }


    }

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class ConnectionObjectModel3d_Function_Model
    {


        public ConnectionObjectModel3d_Function_Model() { }


        public ConnectionObjectModel3d_Feature_Enum Feature { set; get; } = ConnectionObjectModel3d_Feature_Enum.distance_3d;


        public double Value { set; get; } = 0.005;

        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {
            try
            {


                switch (Feature)
                {
                    case ConnectionObjectModel3d_Feature_Enum.distance_3d or ConnectionObjectModel3d_Feature_Enum.distance_mapping or ConnectionObjectModel3d_Feature_Enum.lines:




                        return HObjectModel3D.UnionObjectModel3d(_Model3D, "points_surface").ConnectionObjectModel3d(Feature.ToString().ToLower(), Value);




                    case ConnectionObjectModel3d_Feature_Enum.angle:


                        return HObjectModel3D.UnionObjectModel3d(_Model3D, "points_surface").ConnectionObjectModel3d(Feature.ToString().ToLower(), new HTuple(Value).TupleRad());




                    default: throw new ArgumentException("Feature:参数错误！");
                }


            }
            finally
            {
                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

            }


            //return _Model3D[0].ConnectionObjectModel3d(Feature.ToString().ToLower(), Value);

            //return HObjectModel3D.ConnectionObjectModel3d(_Model3D, Feature.ToString().ToLower(), Value);

        }

    }
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class SelectObjectModel3d_Funtion_Model
    {

        public SelectObjectModel3d_Funtion_Model() { }

        public SelectObjectModel3d_Feature_Enum Feature { set; get; } = SelectObjectModel3d_Feature_Enum.num_points;

        public SelectObjectModel3d_Operation_Enum Operation { set; get; } = SelectObjectModel3d_Operation_Enum.and;


        public double minValue { set; get; } = 500;


        public double maxValue { set; get; } = 10000;


        public bool Max { set; get; } = false;
        public bool Min { set; get; } = false;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            try
            {

                // 参数检查
                if (_Model3D == null || _Model3D.Length == 0)
                {
                    throw new ArgumentException("The input model3D array is null or empty.", nameof(_Model3D));
                }

                if (!Max && Min)
                {


                    return HObjectModel3D.SelectObjectModel3d(_Model3D, Feature.ToString().ToLower(), Operation.ToString(), minValue, "max");





                }

                if (!Min && Max)
                {
                    return HObjectModel3D.SelectObjectModel3d(_Model3D, Feature.ToString().ToLower(), Operation.ToString(), "min", maxValue);

                }

                if (!Max && !Min)
                {
                    return HObjectModel3D.SelectObjectModel3d(_Model3D, Feature.ToString().ToLower(), Operation.ToString(), "min", "max");

                }

                return HObjectModel3D.SelectObjectModel3d(_Model3D, Feature.ToString().ToLower(), Operation.ToString(), minValue, maxValue);

            }
            finally
            {

                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

            }
        }


    }

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class SampleObjectModel3d_Function_Model
    {

        public SampleObjectModel3d_Method_Enum Method { set; get; } = SampleObjectModel3d_Method_Enum.fast;

        public double SampleDistance { set; get; } = 0.005;


        public double max_angle_diff { set; get; } = 180;


        public double min_num_points { set; get; } = 5;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            try
            {



                switch (Method)
                {
                    case SampleObjectModel3d_Method_Enum.fast or SampleObjectModel3d_Method_Enum.fast_compute_normals or SampleObjectModel3d_Method_Enum.xyz_mapping or SampleObjectModel3d_Method_Enum.xyz_mapping_compute_normals or SampleObjectModel3d_Method_Enum.furthest_point or SampleObjectModel3d_Method_Enum.furthest_point_compute_normals:


                        return HObjectModel3D.SampleObjectModel3d(_Model3D, Method.ToString(), SampleDistance, new HTuple(), new HTuple());


                    case SampleObjectModel3d_Method_Enum.accurate:
                        return HObjectModel3D.SampleObjectModel3d(_Model3D, Method.ToString(), SampleDistance, new HTuple([nameof(min_num_points)]), new HTuple([min_num_points]));

                    case SampleObjectModel3d_Method_Enum.accurate_use_normals:
                        return HObjectModel3D.SampleObjectModel3d(_Model3D, Method.ToString(), SampleDistance, new HTuple([nameof(max_angle_diff), nameof(min_num_points)]), new HTuple([max_angle_diff, min_num_points]));




                    default: throw new ArgumentException("Method:参数错误！");

                }

            }
            finally
            {

                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

            }
        }

    }
    [Serializable]

    [AddINotifyPropertyChangedInterface]
    public class SurfaceNormalsObjectModel3d_Function_Model
    {

        public SurfaceNormalsObjectModel3d_Method_Enum Method { set; get; } = SurfaceNormalsObjectModel3d_Method_Enum.mls;


        public double mls_kNN { set; get; } = 60;

        public double mls_abs_sigma { set; get; } = 0.001;

        public int mls_order { set; get; } = 2;

        public double mls_relative_sigma { set; get; } = 1;

        public bool mls_force_inwards { set; get; } = false;



        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {
            try
            {

                switch (Method)
                {
                    case SurfaceNormalsObjectModel3d_Method_Enum.mls:


                        if (mls_abs_sigma != 0)
                        {
                            return HObjectModel3D.SurfaceNormalsObjectModel3d(
                      _Model3D,
                      Method.ToString(),
                      new HTuple([nameof(mls_kNN), nameof(mls_abs_sigma), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                      new HTuple([mls_kNN, mls_abs_sigma, mls_order, mls_relative_sigma, mls_force_inwards.ToString().ToLower()]));
                        }
                        else
                        {
                            return HObjectModel3D.SurfaceNormalsObjectModel3d(
                      _Model3D,
                      Method.ToString(),
                      new HTuple([nameof(mls_kNN), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                      new HTuple([mls_kNN, mls_order, mls_relative_sigma, mls_force_inwards.ToString().ToLower()]));

                        }




                    case SurfaceNormalsObjectModel3d_Method_Enum.xyz_mapping or SurfaceNormalsObjectModel3d_Method_Enum.triangles:
                        return HObjectModel3D.SurfaceNormalsObjectModel3d(
                      _Model3D,
                       Method.ToString(),
                       new HTuple(),
                       new HTuple());
                    default: throw new ArgumentException("Method:参数错误！");

                }


            }
            finally
            {

                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

            }

        }

    }
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class SmoothObjectModel3d_Function_Model
    {

        public SmoothObjectModel3d_Method_Enum Method { set; get; } = SmoothObjectModel3d_Method_Enum.mls;


        public double mls_kNN { set; get; } = 60;


        public int mls_order { set; get; } = 2;
        public double mls_abs_sigma { set; get; } = 0.001;

        public double mls_relative_sigma { set; get; } = 0.1;

        public bool mls_force_inwards { set; get; } = false;


        public SmoothObjectModel3d_Xyz_Mapping_Filter_Enum xyz_mapping_filter { set; get; } = SmoothObjectModel3d_Xyz_Mapping_Filter_Enum.median_separate;


        public int xyz_mapping_mask_width { set; get; } = 3;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {

            try
            {

                switch (Method)
                {
                    case SmoothObjectModel3d_Method_Enum.mls:

                        if (mls_abs_sigma != 0)
                        {

                            return HObjectModel3D.SmoothObjectModel3d(
                                _Model3D,
                                Method.ToString().ToLower(),
                                new HTuple([nameof(mls_kNN), nameof(mls_abs_sigma), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                                new HTuple([mls_kNN, mls_abs_sigma, mls_order, mls_relative_sigma, mls_force_inwards.ToString().ToLower()]));

                        }
                        else
                        {
                            return HObjectModel3D.SmoothObjectModel3d(
                        _Model3D,
                        Method.ToString().ToLower(),
                        new HTuple([nameof(mls_kNN), nameof(mls_order), nameof(mls_relative_sigma), nameof(mls_force_inwards)]),
                        new HTuple([mls_kNN, mls_order, mls_relative_sigma, mls_force_inwards.ToString().ToLower()]));
                        }

                    case SmoothObjectModel3d_Method_Enum.xyz_mapping or SmoothObjectModel3d_Method_Enum.xyz_mapping_compute_normals:

                        return HObjectModel3D.SmoothObjectModel3d(
                            _Model3D,
                            Method.ToString().ToLower(),
                            new HTuple([nameof(xyz_mapping_filter), nameof(xyz_mapping_mask_width)]),
                            new HTuple([xyz_mapping_filter.ToString(), xyz_mapping_mask_width]));

                    default: throw new ArgumentException("参数错误！");


                }

            }
            finally
            {

                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

            }

        }

    }
    [Serializable]

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
                   new HTuple([max_area_holes]));
                    break;
                case PrepareObjectModel3d_PurPose_Enum.distance_computation:

                    HObjectModel3D.PrepareObjectModel3d(
                      _Model3D,
                     Purpose.ToString().ToLower(),
                     overwriteData.ToString().ToLower(),
                     new HTuple([nameof(distance_to), nameof(method), nameof(max_distance), nameof(sampling_dist_rel), nameof(sampling_dist_abs)]),
                     new HTuple([distance_to.ToString(), method.GetStringValue(), max_distance, sampling_dist_rel, sampling_dist_abs]));

                    break;
                case PrepareObjectModel3d_PurPose_Enum.gen_xyz_mapping:

                    HObjectModel3D.PrepareObjectModel3d(
                        _Model3D,
                       Purpose.ToString().ToLower(),
                       overwriteData.ToString().ToLower(),
                       new HTuple([nameof(xyz_map_width)]),
                       new HTuple([xyz_map_width]));

                    break;

            }



            return _Model3D;




        }

    }




    [Serializable]
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


        public int greedy_hole_filling { set; get; } = 40;

        public bool greedy_fix_flips { get; set; } = true;

        public bool greedy_prefetch_neighbors { set; get; } = true;

        public int greedy_mesh_erosion { set; get; } = 3;

        public int greedy_mesh_dilation { set; get; } = 2;

        public double greedy_remove_small_surfaces { set; get; } = 0;


        public bool greedy_timeout { set; get; } = false;


        public bool greedy_suppress_timeout_error { set; get; } = false;

        public bool greedy_output_all_points { set; get; } = false;

        public TriangulateObjectModel3d_Information_Enum information { set; get; } = TriangulateObjectModel3d_Information_Enum.verbose;


        public int implicit_octree_depth { set; get; } = 6;


        public int implicit_solver_depth { set; get; } = 6;

        public int implicit_min_num_samples { set; get; } = 1;


        public HObjectModel3D[] Get_Results(HObjectModel3D[] _Model3D)
        {
            try
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
                          greedy_kNN,
                      greedy_radius_type.ToString().ToLower(),
                      greedy_radius_value,
                      greedy_neigh_orient_tol,
                      greedy_neigh_orient_consistent.ToString().ToLower(),
                      greedy_neigh_latitude_tol,
                      greedy_neigh_vertical_tol,
                      greedy_hole_filling,
                      greedy_fix_flips.ToString().ToLower(),
                      greedy_prefetch_neighbors.ToString().ToLower(),
                      greedy_mesh_erosion,
                      greedy_mesh_dilation,
                      greedy_remove_small_surfaces,
                      greedy_timeout.ToString().ToLower(),
                      greedy_suppress_timeout_error.ToString().ToLower(),
                      greedy_output_all_points.ToString().ToLower(),
                      information.ToString()]),
                          out _information);

                        ;
                    case TriangulateObjectModel3d_Method_Enum.Implicit:


                        return HObjectModel3D.TriangulateObjectModel3d(
                    _Model3D,
                    Method.ToString().ToLower(),
                    new HTuple([nameof(implicit_octree_depth), nameof(implicit_solver_depth), nameof(implicit_min_num_samples), nameof(information)]),
                    new HTuple([implicit_octree_depth, implicit_solver_depth, implicit_min_num_samples, information.ToString()]),
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
                         new HTuple([
                         nameof(xyz_mapping_max_area_holes),
                     nameof(xyz_mapping_max_view_angle),
                     nameof(xyz_mapping_max_view_dir_x),
                     nameof(xyz_mapping_max_view_dir_y),
                     nameof(xyz_mapping_max_view_dir_z),
                     nameof(xyz_mapping_output_all_points)]),
                         new HTuple([
                         xyz_mapping_max_area_holes,
                     HTuple.TupleRand(xyz_mapping_max_view_angle),
                     Convert.ToInt32(xyz_mapping_max_view_dir_x).ToString(),
                     Convert.ToInt32(xyz_mapping_max_view_dir_y).ToString(),
                     Convert.ToInt32(xyz_mapping_max_view_dir_z).ToString(),
                     xyz_mapping_output_all_points.ToString().ToLower()]),
                         out _information);




                    default: throw new ArgumentException("参数错误！");
                }



            }
            finally
            {

                foreach (var item in _Model3D) { item.ClearObjectModel3d(); item.Dispose(); };

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


    public enum Stereo_3D_Preprocessing_Process_Work_Enum
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

    public enum Preprocessing_Process_Run_State_Enum
    {
        Int,
        Runing,
        End,
        Error
    }



}
