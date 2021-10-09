using Nancy.Helpers;
using PropertyChanged;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models
    {

        public Sink_Models()
        {





        }

        /// <summary>
        /// 水槽工艺参数
        /// </summary>
        public Sink_Process_Models Sink_Process { set; get; } = new Sink_Process_Models();






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
        private string _Photo_ico = HttpUtility.HtmlDecode("&#xe61b;");
        /// <summary>
        /// 列表显示对应水槽类型图片
        /// </summary>
        public string Photo_ico
        {
            get { return _Photo_ico; }
            set
            {
                //将字符串转Unicode码在前端显示
                switch (int.Parse(value))
                {
                    case (int)Photo_enum.普通单盆:
                        _Photo_ico = HttpUtility.HtmlDecode("&#xe61b;");
                        break;
                    case (int)Photo_enum.普通双盆:
                        _Photo_ico = HttpUtility.HtmlDecode("&#xe61d;");
                        break;
                    case (int)Photo_enum.左右单盆:
                        _Photo_ico = HttpUtility.HtmlDecode("&#xe61a;");
                        break;

                }
            }
        }




        /// <summary>
        /// 列表显示水槽枚举
        /// </summary>
        public enum Photo_enum
        {
            普通单盆,
            普通双盆,
            左右单盆
        }





        /// <summary>
        /// 列表中水槽的保存参数
        /// </summary>
        public Wroking_Models Wroking_Models_ListBox = new Wroking_Models() { };


        private User_Features _User_Check_1 = new User_Features() { };
        /// <summary>
        /// 列表中区域1水槽的功能保存
        /// </summary>
        public User_Features User_Check_1
        {
            get
            {
                return _User_Check_1;
            }
            set
            {
                _User_Check_1 = value;
            }
        }


        private User_Features _User_Check_2 = new User_Features() { };
        /// <summary>
        /// 列表中区域2水槽的功能保存
        /// </summary>
        public User_Features User_Check_2
        {
            get
            {
                return _User_Check_2;
            }
            set
            {
                _User_Check_2 = value;
            }
        }



        private bool _List_IsChecked_1 = false;
        /// <summary>
        /// 选定加工区域1按钮
        /// </summary>
        public bool List_IsChecked_1
        {
            get { return _List_IsChecked_1; }
            set { _List_IsChecked_1 = value; }
        }



        private bool _List_IsChecked_2 = false;
        /// <summary>
        /// 选定加工区域2按钮
        /// </summary>
        public bool List_IsChecked_2
        {
            get { return _List_IsChecked_2; }
            set { _List_IsChecked_2 = value; }
        }





        public string Trigger_Work_NO { set; get; } = "0";




    }

    [AddINotifyPropertyChangedInterface]
    public class Sink_Process_Models
    {

        /// <summary>
        /// 水槽尺寸宽
        /// </summary>
        public int Sink_Width { set; get; } = 345;

        /// <summary>
        /// 水槽尺寸长
        /// </summary>
        public int Sink_Long { set; get; } = 650;






    }
}
