using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Pop_Ups_VM:ViewModelBase
    {
        public UC_Pop_Ups_VM()
        {





        }


        

        //public UserControl Pop_UserControl { set; get; } = new UC_Sink_Size() { };
        public UserControl Pop_UserControl { set; get; } 



        /// <summary>
        /// 获取用户选择的水槽列
        /// </summary>
        public Sink_Models SM { set; get; }









        /// <summary>
        /// 标题枚举
        /// </summary>
        public enum RadioButton_Name
        {
            水槽类型选择,
            水槽尺寸调节,
            工艺参数调节
        }



        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        public ICommand RB_Check_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                RadioButton e = Sm.Source as RadioButton;

                switch (int.Parse((string)e.Tag))
                {
                    case (int)RadioButton_Name.水槽类型选择:
                        Pop_UserControl= new Sink_Type() { };
                        break;
                    case (int)RadioButton_Name.水槽尺寸调节:
                        Pop_UserControl = new UC_Sink_Size() { };
                        break;
                    case (int)RadioButton_Name.工艺参数调节:
                        Pop_UserControl = new UC_Sink_Craft_List() { };

                        break;
                    default:
                        break;
                }




            });
        }


        /// <summary>
        /// 水槽类型选择事件
        /// </summary>
        public ICommand Sink_Loaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                FrameworkElement e = Sm.Source as FrameworkElement;




            });
        }










        /// <summary>
        /// 水槽类型选择事件
        /// </summary>
        public ICommand Sink_Type_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                FrameworkElement e = Sm.Source as FrameworkElement;

               


                switch (int.Parse((String)e.Tag))
                {
                    case (int)Photo_Sink_enum.左右单盆:

                        break;
                    case (int)Photo_Sink_enum.上下单盆:

                        break;
                    case (int)Photo_Sink_enum.普通双盆:

                        break;
                    default:
                        break;
                }




            });
        }
    }
}
