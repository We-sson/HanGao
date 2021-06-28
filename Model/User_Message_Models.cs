using PropertyChanged;
using System.Windows;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_Models
    {

        public User_Message_Models()
        {




        }


        private string _User_Message_Visibility = "Collapsed";
        /// <summary>
        /// 弹窗是否显示出来
        ///Visible 显示元素。
        ///  
        ///Hidden 不显示元素，但是在布局中为元素保留空间。
        /// 
        ///Collapsed 不显示元素，并且不在布局中为它保留空间。
        ///
        /// </summary>
        public string User_Message_Visibility
        {
            get
            {
                return _User_Message_Visibility;
            }
            set
            {
                _User_Message_Visibility = value;
            }
        }




        private string _User_Wrok_Trye = "000000";
        /// <summary>
        /// 弹窗显示加工区域已存在型号
        /// </summary>
        public string User_Wrok_Trye
        {
            get
            {
                return _User_Wrok_Trye;
            }
            set
            {
                _User_Wrok_Trye = value;
            }
        }




        private Window _User_Window;
        /// <summary>
        /// 弹窗加载存放属性
        /// </summary>
        public Window User_Window
        {
            get
            {
                return _User_Window;
            }
            set
            {
                _User_Window = value;
            }
        }



    }
}
