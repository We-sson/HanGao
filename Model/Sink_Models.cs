using HanGao.Extension_Method;
using Nancy.Helpers;
using PropertyChanged;
using System;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models
    {

        public Sink_Models(Photo_Sink_enum _Photo_Sink_Type)
        {

            Photo_Sink_Type = _Photo_Sink_Type;



        }

        /// <summary>
        /// 水槽工艺参数
        /// </summary>
        public Sink_Size_Models Sink_Process { set; get; } = new Sink_Size_Models( _Width:650, _Long:345);






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
            { return Sink_Process.Sink_Long.ToString() + "X" + Sink_Process.Sink_Width.ToString(); }
        }





        //&#xe610;   &#xe60a;   &#xe60b;
        private string _Photo_ico = "&#xe61b;";
        /// <summary>
        /// 列表显示对应水槽类型图片码
        /// </summary>
        public string Photo_ico
        {
            get { return HttpUtility.HtmlDecode(_Photo_ico); }
        }

        /// <summary>
        /// 根据水槽类型显示对于图标
        /// </summary>
        [SuppressPropertyChangedWarnings]
        public Photo_Sink_enum Photo_Sink_Type
        {
            set
            {
                _Photo_ico = value.GetStringValue();
            }

        }


        /// <summary>
        /// 列表显示水槽枚举
        /// </summary>
     
        public enum Photo_Sink_enum
        {
            [StringValue("&#xe61b;") ]
            左右单盆,
            [StringValue("&#xe61d;")]
            普通双盆,
            [StringValue("&#xe61a;")]
            上下单盆
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
       public Sink_Size_Models(double _Width ,double _Long)
        {
            Sink_Width = _Width;
            Sink_Long = _Long;
        }


        /// <summary>
        /// 水槽尺寸宽
        /// </summary>
        public double Sink_Width { set; get; } = 345;

        /// <summary>
        /// 水槽尺寸长
        /// </summary>
        public double Sink_Long { set; get; } = 650;






    }
}
