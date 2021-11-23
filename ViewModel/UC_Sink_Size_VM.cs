using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public   class UC_Sink_Size_VM : ViewModelBase
    {

        public UC_Sink_Size_VM()
        {

        }


        /// <summary>
        /// 水槽各参数
        /// </summary>
        public Sink_Size_Models Sink_Size_Value { set; get; }




        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Value_OK_Comm
        {
            get => new RelayCommand<UserControl>((Sm) =>
            {



                Messenger.Default.Send<Sink_Size_Models>(Sink_Size_Value, "Sink_Size_Value_OK");




            });
        }



        /// <summary>
        /// 现在原属性水槽类型
        /// </summary>
        public ICommand Sink_Size_Loaded_Comm
        {
            get => new RelayCommand<UserControl>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;

         





            });
        }


    }



}
