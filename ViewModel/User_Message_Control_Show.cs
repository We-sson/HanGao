using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;
using 悍高软件.View.UserMessage;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_Control_Show : ViewModelBase
    {


        public User_Message_Control_Show()
        {

            


            //注册消息接收
            Messenger.Default.Register<bool?>(this, "User_Message_Show", User_UI);
            Messenger.Default.Register<string>(this, "User_Contorl_Message_Show", User_Control);

        }



        /// <summary>
        /// 接收到消息创建对应字符的消息框
        /// </summary>
        public void User_Control( string User)
        {

            if (User== "Use_Message")
            {
            User_Message_UserControl_Show = new User_Message() { };

            }

        }





        /// <summary>
        /// 通知弹窗显示
        /// </summary>
        public void User_UI(bool? m)
        {
            if (m == true)
            {
                User_Visibility = Visibility.Visible;
            }
            else if (m == false)
            {
                User_Visibility = Visibility.Collapsed;
            }
        }




        private UserControl _User_Message_UserControl_Show ;
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
