using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using static HanGao.ViewModel.UC_Pop_Ups_VM;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Type_VM : ViewModelBase
    {
        public UC_Sink_Type_VM()
        {



        }

        /// <summary>
        /// 用户选择的水槽类型
        /// </summary>
        public Photo_Sink_Enum Sink_Type_Enum { set; get; }






        /// <summary>
        /// 现在原属性水槽类型
        /// </summary>
        public ICommand Sink_Type_Loaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                UC_Sink_Type e = Sm.Source as UC_Sink_Type;

                switch (Sink_Type_Enum)
                {
                    case Photo_Sink_Enum.左右单盆:
                        e.R1.IsChecked = true;
                        break;
                    case Photo_Sink_Enum.上下单盆:
                        e.R2.IsChecked = true;
                        break;
                    case Photo_Sink_Enum.普通双盆:
                        e.R3.IsChecked = true;
                        break;
                    default:
                        break;
                }






            });
        }










        /// <summary>
        /// 修改水槽类型选择事件
        /// </summary>
        public ICommand Sink_Type_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                FrameworkElement e = Sm.Source as FrameworkElement;


                Sink_Type_Enum = (Photo_Sink_Enum)int.Parse((String)e.Tag);


                Messenger.Default.Send<Photo_Sink_Enum>(Sink_Type_Enum, "Sink_Type_Value_OK");

                Messenger.Default.Send<RadioButton_Name>(RadioButton_Name.水槽尺寸调节, "Pop_Sink_Size_Show");

                

            });
        }



















    }
}
