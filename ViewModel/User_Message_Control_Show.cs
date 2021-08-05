using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using 悍高软件.Model;
using 悍高软件.View.UserMessage;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_Control_Show : ViewModelBase
    {


        public User_Message_Control_Show()
        {







            // 接收到消息创建对应字符的消息框
            Messenger.Default.Register<List_Show_Models>(this, "User_Contorl_Message_Show", (_List)=> 
            {
                if (_List.List_Show_Bool==Visibility.Visible)
                {
                    User_Message_UserControl_Show = new User_Message() {  DataContext= new User_Message_ViewModel() { User_Wrok_Trye= _List.List_Show_Name, List_Show_Models=_List } };
                    User_Visibility = _List.List_Show_Bool;

                }
                else
                {
                    User_Message_UserControl_Show = null;
                }
         
            });

        }









        /// <summary>
        /// 通知弹窗显示
        /// </summary>
        //public void User_UI(bool? m)
        //{
        //    if (m == true)
        //    {
        //        User_Visibility = Visibility.Visible;
        //    }
        //    else if (m == false)
        //    {
        //        User_Visibility = Visibility.Collapsed;
        //    }
        //}




        private UserControl _User_Message_UserControl_Show;
        /// <summary>
        /// 弹窗显示加工区域已存在型号=
        /// </summary>
        public UserControl User_Message_UserControl_Show
        {
            get
            {
                return _User_Message_UserControl_Show;
            }
            set
            {
                _User_Message_UserControl_Show = value;
            }
        }

        private Visibility _User_Visibility = Visibility.Collapsed;
        /// <summary>
        /// 弹窗显示
        /// </summary>
        public Visibility User_Visibility
        {
            get
            {
                return _User_Visibility;
            }
            set
            {
                _User_Visibility = value;
            }
        }





    }
}
