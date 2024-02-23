using Halcon_SDK_DLL.Model;
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

        public Halcon_Image_Preprocessing_Process_SDK()
        {


            //Test
            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.MedianImage, Method_Run_Time = 156, V_1 = 156, V_2 = 123, V_3 = 6556 });

            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.MedianImage, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });

            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.Illuminate, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });
            //Preprocessing_Process_List.Add(new Preprocessing_Process_Lsit_Model() { Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.GrayClosingRect, Method_Run_Time = 356, V_1 = 156, V_2 = 123, V_3 = 6556 });


        }


        public ObservableCollection<Preprocessing_Process_Lsit_Model> Preprocessing_Process_List { set; get; } = new ObservableCollection<Preprocessing_Process_Lsit_Model>();




        public int Preprocessing_Process_List_RunTime { set; get; } = 0;

        public Preprocessing_Process_Lsit_Model? Preprocessing_Process_List_Selete { set; get; }


        //public delegate T ADD_delegate<T>(int _IN);


        //public ADD_delegate<object> ADD_Delegate_Model { set; get; }

        //private List<Action> Preprocessing_Process_Method { set; get; } = new List<Action>();











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

                      var _Index = Preprocessing_Process_List.IndexOf(Preprocessing_Process_List_Selete) ;
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
                        var a = Preprocessing_Process_List.IndexOf(Preprocessing_Process_List_Selete)+1;
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

            Image = _OldImage;

            //计算总时间处理
            DateTime AllstartTime = DateTime.Now;

            foreach (var item in Preprocessing_Process_List)
            {
                //开始单个处理时间
                DateTime startTime = DateTime.Now;
              

              
                Get_Preprocessing_Method(item.Image_Preprocessing_Process_Method, item.V_1, item.V_2, item.V_3, item.V_4, item.V_5, item.E_1, item.E_2, item.E_3, item.E_4, item.E_5).Invoke();

                // 计算时间差
                item.Method_Run_Time=(DateTime.Now - startTime).Milliseconds;
            }

            Preprocessing_Process_List_RunTime= (DateTime.Now - AllstartTime).Milliseconds;

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
        public Action Get_Preprocessing_Method(Image_Preprocessing_Process_Enum _Process, object? V_1 = null, object? V_2 = null, object? V_3 = null, object? V_4 = null, object? V_5 = null, Enum? E_1 = null, Enum? E_2 = null, Enum? E_3 = null, Enum? E_4 = null, Enum? E_5 = null)
        {




            return _Process switch
            {
                Image_Preprocessing_Process_Enum.ScaleImageMax =>()=> ScaleImageMax(),
                Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage((MedianImage_MaskType_Enum)E_1!, Convert.ToInt32(V_1), (MedianImage_Margin_Enum)E_2!),
                Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble(V_3)),
                Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(Convert.ToInt32(V_1), Convert.ToInt32(V_2), Convert.ToDouble (V_3)),
                Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(Convert.ToInt32(V_1), Convert.ToInt32(V_2)),
                _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
            };
        }

        /// <summary>
        /// 处理图像
        /// </summary>
        private  HImage Image { set; get; } = new HImage();



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

            Image= Image.MedianRect(MedianRect_MaskWidth, MedianRect_MaskHeight);

        }


        public void GrayOpeningRect(int maskHeight, int maskWidth)
        {
            maskHeight.ThrowIfNull("参数不能：Null ");
            maskWidth.ThrowIfNull("参数不能：Null ");

            Image= Image.GrayOpeningRect(maskHeight, maskWidth);


        }

        public void MedianImage(MedianImage_MaskType_Enum MaskType_Model, int Median_image_Radius, MedianImage_Margin_Enum Margin_Model)
        {
            MaskType_Model.ThrowIfNull("参数不能：Null ");
            Median_image_Radius.ThrowIfNull("参数不能：Null ");
            Margin_Model.ThrowIfNull("参数不能：Null ");


            Image= Image.MedianImage(MaskType_Model.ToString(), Median_image_Radius, Margin_Model.ToString());
        }


        public void Illuminate(int maskWidth, int maskHeight, double factor)
        {
            maskWidth.ThrowIfNull("参数不能：Null ");
            maskHeight.ThrowIfNull("参数不能：Null ");
            factor.ThrowIfNull("参数不能：Null ");


            Image= Image.Illuminate(maskWidth, maskHeight, factor);
        }



        public void Emphasize(int maskWidth, int maskHeight, double factor)
        {
            maskWidth.ThrowIfNull("参数不能：Null ");
            maskHeight.ThrowIfNull("参数不能：Null ");
            factor.ThrowIfNull("参数不能：Null ");

            Image= Image.Emphasize(maskWidth, maskHeight, factor);
        }


        public void GrayClosingRect(int maskHeight, int maskWidth)
        {
            maskHeight.ThrowIfNull("参数不能：Null ");
            maskWidth.ThrowIfNull("参数不能：Null ");

            Image= Image.GrayClosingRect(maskHeight, maskWidth);
        }


    }

    [AddINotifyPropertyChangedInterface]
    public class Preprocessing_Process_Lsit_Model
    {
        public Preprocessing_Process_Lsit_Model()
        {



        }

      


        private Image_Preprocessing_Process_Enum _Image_Preprocessing_Process_Method = Image_Preprocessing_Process_Enum.ScaleImageMax;

        public Image_Preprocessing_Process_Enum Image_Preprocessing_Process_Method
        {
            get { return _Image_Preprocessing_Process_Method ; }
            set {
                _Image_Preprocessing_Process_Method  = value;
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



                    break;
                case Image_Preprocessing_Process_Enum.MedianRect:



                    V_1 = 9;
                    V_2 = 9;


                    break;
                case Image_Preprocessing_Process_Enum.GrayOpeningRect:
                    V_1 = 9;
                    V_2 = 9;
                    break;
                case Image_Preprocessing_Process_Enum.MedianImage:

                    E_1 = MedianImage_MaskType_Enum.square;
                    V_1 = 0.8;
                    E_2 = MedianImage_Margin_Enum.continued;
                    break;
                case Image_Preprocessing_Process_Enum.Illuminate:
                    V_1 = 9;
                    V_2 = 9;
                    V_3 = 0.8;
                 
                    break;
                case Image_Preprocessing_Process_Enum.Emphasize:
                    V_1 = 9;
                    V_2 = 9;
                    V_3 = 0.8;
                    break;
                case Image_Preprocessing_Process_Enum.GrayClosingRect:
                    V_1 = 9;
                    V_2 = 9;
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


        public object? V_1 { set; get; }
        public object? V_2 { set; get; }
        public object? V_3 { set; get; }
        public object? V_4 { set; get; }
        public object? V_5 { set; get; }
        public Enum? E_1 { set; get; }
        public Enum? E_2 { set; get; }
        public Enum? E_3 { set; get; }
        public Enum? E_4 { set; get; }
        public Enum? E_5 { set; get; }

    }



    public enum Image_Preprocessing_Process_Enum
    {
        [Description("灰度动调分布")]
        ScaleImageMax,
        [Description("中值滤波器")]
        MedianRect,
        [Description("矩形开运算")]
        GrayOpeningRect,
        [Description("矩形闭运算")]
        GrayClosingRect,
        [Description("中值滤波器")]
        MedianImage,
        [Description("高频增强对比")]
        Illuminate,
        [Description("增强边缘")]
        Emphasize,


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




}
