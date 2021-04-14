using GalaSoft.MvvmLight;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class User_Control_Working_VM_1: ViewModelBase
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
        /// <summary>
        /// 选择加工工位触发事件命令
        /// </summary>
        public ICommand Work_Run_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(Set_Work_Run);
        }

        private void Set_Work_Run(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            FrameworkElement e = Sm.Source as FrameworkElement;
            Sink_Models S = (Sink_Models)e.DataContext;

            S.Wroking_Models_ListBox.Work_Run =WM.Work_Run;



        }

    }
}
