using GalaSoft.MvvmLight;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_ViewModel : ViewModelBase
    {

        public User_Message_ViewModel()
        {








        }





        private static string _User_Wrok_Trye = "000000";
        /// <summary>
        /// 弹窗显示加工区域已存在型号
        /// </summary>
        public static string User_Wrok_Trye
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

        /// <summary>
        /// 弹窗加载事件命令
        /// </summary>
        public ICommand User_Window_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Window_Co);
        }
        private void User_Window_Co(RoutedEventArgs S)
        {
            Window e = S.Source as Window;
            User_Window = e;
            //MessageBox.Show(User_Window.DialogResult.ToString());




        }





        /// <summary>
        /// 弹出用户确定取消选择
        /// </summary>
        public ICommand Yes_No_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Yes_No);
        }
        /// <summary>
        /// 加工区域计数事件命令
        /// </summary>
        private void User_Yes_No(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            Button e = Sm.Source as Button;
            User_Message_ViewModel S = (User_Message_ViewModel)e.DataContext;
            




            if (e.Uid.ToString() == "Yes")
            {

                User_Window.DialogResult = true;
            }
            else if (e.Uid.ToString() == "No")
            {
                User_Window.DialogResult = false;

            }


            return;



            //if (e.Content.ToString()=="Yes")
            //{

            //}
            //else if (e.Content.ToString() == "No")
            //{

            //}

            //MessageBox.Show(e.Content.ToString());

        }



    }
}
