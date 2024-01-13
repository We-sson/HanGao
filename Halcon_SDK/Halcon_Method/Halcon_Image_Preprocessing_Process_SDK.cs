using Halcon_SDK_DLL.Model;
using HalconDotNet;
using PropertyChanged;
using System.ComponentModel;

namespace Halcon_SDK_DLL.Halcon_Method
{
    [AddINotifyPropertyChangedInterface]
    public class Halcon_Image_Preprocessing_Process_SDK
    {

        public Halcon_Image_Preprocessing_Process_SDK()
        {








        }


        public List<Preprocessing_Process_Lsit_Model> Preprocessing_Process_Lsit { set; get; } = new List<Preprocessing_Process_Lsit_Model>();



        //public delegate T ADD_delegate<T>(int _IN);


        //public ADD_delegate<object> ADD_Delegate_Model { set; get; }

        public List<Action> Preprocessing_Process_Method { set; get; } = new List<Action>();



        public void Preprocessing_Process_Start()
        {

            foreach (var item in Preprocessing_Process_Lsit)
            {

                Preprocessing_Process_Method.Add(Get_Preprocessing_Method(item.Image_Preprocessing_Process_Method, item.V_1, item.V_2, item.V_3, item.V_4, item.V_5, item.V_6, item.V_7, item.V_8, item.V_9, item.V_10));
          
            
            }

            //执行
            foreach (var item in Preprocessing_Process_Method)
            {
                item.Invoke();
            }



        }




        public Action Get_Preprocessing_Method(Image_Preprocessing_Process_Enum _Process ,dynamic?  V_1=null, dynamic? V_2=null, dynamic? V_3=null, dynamic? V_4=null, dynamic? V_5=null, dynamic? V_6=null, dynamic? V_7=null, dynamic? V_8=null, dynamic? V_9 = null, dynamic? V_10 = null)
        {

            return _Process switch
            {
                Image_Preprocessing_Process_Enum.ScaleImageMax => () => ScaleImageMax(),
                Image_Preprocessing_Process_Enum.MedianRect => () => MedianRect(V_1, V_2),
                Image_Preprocessing_Process_Enum.GrayOpeningRect => () => GrayOpeningRect(V_1, V_2),
                Image_Preprocessing_Process_Enum.MedianImage => () => MedianImage(V_1, V_2, V_3),
                Image_Preprocessing_Process_Enum.Illuminate => () => Illuminate(V_1, V_2, V_3),
                Image_Preprocessing_Process_Enum.Emphasize => () => Emphasize(V_1, V_2, V_3),
                Image_Preprocessing_Process_Enum.GrayClosingRect => () => GrayClosingRect(V_1, V_2),
                _ => throw new ArgumentException("无效的预处理过程枚举值。", nameof(_Process)),// 处理默认情况，或者根据需要抛出异常
            };
        }


        public HImage Image { set; get; } = new HImage();

        public void ScaleImageMax()
        {

            //HImage _Image_Results = new HImage();
            Image.ScaleImageMax();

            //HOperatorSet.ScaleImageMax(_Image, out HImage);
        }


        public void MedianRect(int MedianRect_MaskWidth, int MedianRect_MaskHeight)
        {

            //HOperatorSet.MedianRect(_HImage, out _HImage, _Find_Property.MedianRect_MaskWidth, _Find_Property.MedianRect_MaskHeight);

            Image.MedianRect(MedianRect_MaskWidth, MedianRect_MaskHeight);


        }


        public void GrayOpeningRect(int maskHeight, int maskWidth)
        {

            Image.GrayOpeningRect(maskHeight, maskWidth);


        }

        public void MedianImage(MedianImage_MaskType_Enum MaskType_Model, int Median_image_Radius, MedianImage_Margin_Enum Margin_Model)
        {
            Image.MedianImage(MaskType_Model.ToString(), Median_image_Radius, Margin_Model.ToString());
        }


        public void Illuminate(int maskWidth, int maskHeight, double factor)
        {
            Image.Illuminate(maskWidth, maskHeight, factor);
        }



        public void Emphasize(int maskWidth, int maskHeight, double factor)
        {
            Image.Emphasize(maskWidth, maskHeight, factor);
        }


        public void GrayClosingRect(int maskHeight, int maskWidth)
        {
            Image.GrayClosingRect(maskHeight, maskWidth);
        }


    }


    public class Preprocessing_Process_Lsit_Model
    {
        public Preprocessing_Process_Lsit_Model()
        {



        }

        public Image_Preprocessing_Process_Enum Image_Preprocessing_Process_Method { set; get; } =  Image_Preprocessing_Process_Enum.ScaleImageMax;




        public Action? Action_Method { set; get; }



        /// <summary>
        /// 耗时用毫秒单位
        /// </summary>
        public int   Method_Run_Time { set; get; } = 0;


        public dynamic? V_1 { set; get; } 
        public dynamic? V_2 { set; get; } 
        public dynamic? V_3 { set; get; } 
        public dynamic? V_4 { set; get; } 
        public dynamic? V_5 { set; get; } 
        public dynamic? V_6 { set; get; } 
        public dynamic? V_7 { set; get; } 
        public dynamic? V_8 { set; get; }
        public dynamic? V_9 { set; get; }
        public dynamic? V_10 { set; get; }

    }



        public enum Image_Preprocessing_Process_Enum
        {
            [Description("图像最大最小分布")]
            ScaleImageMax,
            [Description("中值滤波器")]
            MedianRect,
            [Description("矩形开运算")]
            GrayOpeningRect,
            [Description("中值滤波器")]
            MedianImage,
            [Description("增强对比度")]
            Illuminate,
            [Description("增强边缘")]
            Emphasize,
            [Description("矩形闭运算")]
            GrayClosingRect

        }

}
