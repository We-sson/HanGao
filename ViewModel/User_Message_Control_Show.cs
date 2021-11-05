using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using HanGao.Model;
using HanGao.View.UserMessage;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Show : ViewModelBase
    {


        public User_Control_Show()
        {







            // 接收到消息创建对应字符的消息框
            Messenger.Default.Register<UserControl>(this, "User_Contorl_Message_Show", (_List)=> 
            {

                User_UserControl = _List;

                //if (_List.List_Show_Bool==Visibility.Visible)
                //{
                //    User_Message_UserControl_Show = new User_Message() {  DataContext= new User_Message_ViewModel() { User_Wrok_Trye= _List.List_Show_Name, List_Show_Models=_List } };
                //    User_Visibility = _List.List_Show_Bool;

                //}
                //else
                //{
                //    User_Message_UserControl_Show = null;
                //}
         
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




        private UserControl _User_UserControl=null;
        /// <summary>
        /// 弹窗显示加工区域已存在型号=
        /// </summary>
        public UserControl User_UserControl
        {
            get
            {
                return _User_UserControl;
            }
            set
            {
                _User_UserControl = value;
            }
        }







    }
}
