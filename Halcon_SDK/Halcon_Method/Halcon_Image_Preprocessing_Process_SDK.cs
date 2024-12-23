﻿using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            Image = new  HImage( _OldImage);

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
        public Preprocessing_Process_Lsit_Model()
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



        /// <summary>
        /// 选择流程方法参数初始化
        /// </summary>
        /// <param name="_Work_Enum"></param>
        public void Preprocessing_Process_Work_Initialization_Value(Image_Preprocessing_Process_Enum _Work_Enum)
        {

            switch (_Work_Enum)
            {
                case Image_Preprocessing_Process_Enum.ScaleImageMax:

                    V_1 = default;
                    V_2 = default;
                    V_3 = default;
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;

                    break;
                case Image_Preprocessing_Process_Enum.MedianRect:



                    V_1 = "9";
                    V_2 = "9";
                    V_3 = default;
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;
                case Image_Preprocessing_Process_Enum.GrayOpeningRect:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = default;
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;
                case Image_Preprocessing_Process_Enum.MedianImage:

                    V_1 = "1";
                    V_2 = default;
                    V_3 = default;
                    V_4 = default;
                    V_5 = default;
                    E_1 = MedianImage_MaskType_Enum.square.ToString();
                    E_2 = MedianImage_Margin_Enum.continued.ToString();
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;
                case Image_Preprocessing_Process_Enum.Illuminate:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = "0.8";
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;
                case Image_Preprocessing_Process_Enum.Emphasize:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = "0.8";
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;
                case Image_Preprocessing_Process_Enum.GrayClosingRect:
                    V_1 = "9";
                    V_2 = "9";
                    V_3 = default;
                    V_4 = default;
                    V_5 = default;
                    E_1 = default;
                    E_2 = default;
                    E_3 = default;
                    E_4 = default;
                    E_5 = default;
                    break;

            }

        }




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


        public string? E_1 { set; get; } = default;


        public string? E_2 { set; get; } = default;

        public string? E_3 { set; get; } = default;


        public string? E_4 { set; get; } = default;

        public string? E_5 { set; get; } = default;

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
        [Description("连接3D模型_Connection")]
        ConnectionObjectModel3d,
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


    public  enum Preprocessing_Process_2D3D_Switch_Enum
    {

        [Description("2D设备")]
        Camera_2D_Drives,
        [Description("3D设备")]
        Camera_3D_Drives



    }



 

}
