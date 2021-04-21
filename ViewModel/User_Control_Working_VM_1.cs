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
using System.Windows.Media;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class User_Control_Working_VM_1 : User_Control_Common
    {
        public static Wroking_Models WM  { get;set; } 
        public static User_Features UF { get; set; }

        /// <summary>
        /// 1号加工区域
        /// </summary>
        public User_Control_Working_VM_1()
        {
            
        WM = new Wroking_Models
            {
                Number_Work = "1",
        

            };
    UF = new User_Features
            {


            };


        }



        public ICommand Work_Run_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(Set_Work_Run);
        }
        /// <summary>
        /// 启动触发事件命令
        /// </summary>
        private void Set_Work_Run(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;


            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());



   




        }




        public ICommand User_Loaded_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Loaded);
        }
        /// <summary>
        /// 加工区域加载事件命令
        /// </summary>
        private void User_Loaded(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            UserControl e = Sm.Source as UserControl;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);



        }

        public ICommand Work_Connt_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_Conn);
        }
        /// <summary>
        /// 加工区域计数事件命令
        /// </summary>
        private void User_Work_Conn(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1,e.Content.ToString());
     

        }




        public ICommand Work_NullRun_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_NullRun);
        }
        /// <summary>
        /// 加工区域空运事件命令
        /// </summary>
        private void User_Work_NullRun(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());




        }






        public ICommand Work_Pause_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_Pause);
        }
        /// <summary>
        /// 加工区域暂停事件命令
        /// </summary>
        private void User_Work_Pause(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());




        }







        public ICommand Work_JumpOver_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(User_Work_JumpOver);
        }
        /// <summary>
        /// 加工区域跳过事件命令
        /// </summary>
        private void User_Work_JumpOver(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            CheckBox e = Sm.Source as CheckBox;
            User_Control_Working_VM_1 S = (User_Control_Working_VM_1)e.DataContext;
            //写入列表中泛型
            User_Check_Write_List(1);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, 1, e.Content.ToString());

        }

    }
}
