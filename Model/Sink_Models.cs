using Nancy.Helpers;
using PropertyChanged;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Sink_Models
    {

        public Sink_Models()
        {



        }


        private int _Model_Number;
        private string _Photo_ico;
        private string _LIst_Show;

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
        public int Model_Number
        {
            get { return _Model_Number; }
            set { _Model_Number = value; }
        }
        /// <summary>
        /// 列表显示对应水槽类型图片
        /// </summary>
        public string Photo_ico
        {
            get { return _Photo_ico; }
            set
            {
                //将字符串转Unicode码在前段显示
                

                _Photo_ico = HttpUtility.HtmlDecode(value);
                




            }

        }
        public enum Ico
        {
            双盆图标,
            左右盆图标,
            单盆图标
        }


    }




}
