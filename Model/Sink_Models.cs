
using HanGao.Extension_Method;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Nancy.Helpers;
using PropertyChanged;
using System;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models 
    {

        public Sink_Models(Photo_Sink_Enum _Photo_Sink_Type)
        {

            Photo_Sink_Type = _Photo_Sink_Type;



        }

        /// <summary>
        /// 水槽工艺参数
        /// </summary>
        public Sink_Size_Models Sink_Process { set; get; } = new Sink_Size_Models() { Sink_Size_Long=640, Sink_Size_Width=345 };






        private string _LIst_Show = "Visible";
        /// <summary>
        ///Visible 显示元素。
        ///  
        ///Hidden 不显示元素，但是在布局中为元素保留空间。
        /// 
        ///Collapsed 不显示元素，并且不在布局中为它保留空间。
        ///
        /// </summary>
        public string List_Show
        {
            get
            { return _LIst_Show; }

            set
            { _LIst_Show = value; }
        }




        /// <summary>
        /// 水槽型号
        /// </summary>
        public int Model_Number { set; get; } = 952154;




        /// <summary>
        /// 水槽尺寸信息
        /// </summary>
        public string Sink_Size
        {
            get
            { return Sink_Process.Sink_Size_Long.ToString() + "X" + Sink_Process.Sink_Size_Width.ToString(); }
        }





        //&#xe610;   &#xe60a;   &#xe60b;
        private string _Photo_ico = "&#xe61b;";
        /// <summary>
        /// 列表显示对应水槽类型图片码
        /// </summary>
        public string Photo_ico
        {
            get { return HttpUtility.HtmlDecode(_Photo_ico); }
            set  { _Photo_ico = value; }
      
        }


        private  Photo_Sink_Enum _Photo_Sink_Type;
        /// <summary>
        /// 根据水槽类型显示对于图标
        /// </summary>
        public Photo_Sink_Enum Photo_Sink_Type
        {
            set
            {
                Photo_ico = value.GetStringValue();

               
                _Photo_Sink_Type = value;
            }
            get
            {
                return _Photo_Sink_Type;
            }

        }


        /// <summary>
        /// 列表显示水槽枚举
        /// </summary>
     
        public enum Photo_Sink_Enum
        {
            [StringValue(" ")]
            空,
            [StringValue("&#xe61b;") ]
            左右单盆,
            [StringValue("&#xe61a;")]
            上下单盆,
            [StringValue("&#xe61d;")]
            普通双盆,
        }





        /// <summary>
        /// 列表中水槽的保存参数
        /// </summary>
        public Wroking_Models Wroking_Models_ListBox = new Wroking_Models() { };



        /// <summary>
        /// 列表中区域1水槽的功能保存
        /// </summary>
        public User_Features User_Check_1 { set; get; } = new User_Features() { };



     
        /// <summary>
        /// 列表中区域2水槽的功能保存
        /// </summary>
        public User_Features User_Check_2 { set; get; } = new User_Features() { };




        /// <summary>
        /// 选定加工区域1按钮
        /// </summary>
        public bool List_IsChecked_1 { set; get; } = false;





        /// <summary>
        /// 选定加工区域2按钮
        /// </summary>
        public bool List_IsChecked_2 { set; get; } = false;
 





        public string Trigger_Work_NO { set; get; } = "0";




    }



    [AddINotifyPropertyChangedInterface]
    public class Sink_Size_Models
    {
       public Sink_Size_Models()
        {
   
        }


        /// <summary>
        /// 水槽_宽
        /// </summary>
        public double Sink_Size_Width { set; get; } = 345;

        /// <summary>
        /// 水槽_长
        /// </summary>
        public double Sink_Size_Long { set; get; } = 630;
        /// <summary>
        /// 水槽_R角半径
        /// </summary>
        public double Sink_Size_R { set; get; } = 10;

        /// <summary>
        /// 水槽短边长度
        /// </summary>
        public double Sink_Size_Short_Side { set; get; } = 23;

        /// <summary>
        /// 水槽盆胆厚度
        /// </summary>
        public double Sink_Size_Thickness { set; get; } = 0.75;






    }
}
