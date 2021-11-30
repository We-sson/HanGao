using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using Nancy.Helpers;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Size_VM : ViewModelBase
    {

        public UC_Sink_Size_VM()
        {
            var a = Thread.CurrentThread.ManagedThreadId.ToString();


            //接收数据发送到尺寸窗口数据
            Messenger.Default.Register<Sink_Models>(this, "Sink_Size_Value_Load", (_Size) =>
            {

                Sink_Size_Value = _Size;
            });


            Messenger.Default.Register<Photo_Sink_Enum>(this, "Sink_Type_Value_OK", (_T) =>
            {
                var a = Thread.CurrentThread.ManagedThreadId.ToString();
                Sink_Size_Value.Photo_Sink_Type = _T;
            });

        }



        private   Sink_Models _Sink_Size_Value ;
        /// <summary>
        /// 水槽各参数
        /// </summary>
        public     Sink_Models Sink_Size_Value
        {
            set
            {
                var a = Thread.CurrentThread.ManagedThreadId.ToString();
                _Sink_Size_Value = value;
                //StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Sink_Size_Value)));
            }
            get
            {
                return _Sink_Size_Value;
            }
        }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


















        /// <summary>
        /// 传送用户设置好的参数
        /// </summary>
        public ICommand Sink_Value_OK_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {


                Sink_Size_Value.Sink_Process.Sink_Size_Long = double.Parse(Sm.Sink_Long.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Width = double.Parse(Sm.Sink_Width.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Short_Side = double.Parse(Sm.Sink_Short.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_Thickness = double.Parse(Sm.Sink_Thickness.Text);
                Sink_Size_Value.Sink_Process.Sink_Size_R = double.Parse(Sm.Sink_R.Text);

                //Sink_Size_Value.Photo_Sink_Type = UC_Sink_Type_VM.Sink_Type_Load;


                Messenger.Default.Send<Sink_Models>(Sink_Size_Value, "Sink_Size_Value_OK");




            });
        }



        /// <summary>
        /// 现在原属性水槽类型
        /// </summary>
        public ICommand Sink_Size_Loaded_Comm
        {
            get => new RelayCommand<UC_Sink_Size>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;

                if (Sink_Size_Value != null)
                {


                    Sm.Sink_Long.Text = Sink_Size_Value.Sink_Process.Sink_Size_Long.ToString();
                    Sm.Sink_Width.Text = Sink_Size_Value.Sink_Process.Sink_Size_Width.ToString();
                    Sm.Sink_Short.Text = Sink_Size_Value.Sink_Process.Sink_Size_Short_Side.ToString();
                    Sm.Sink_Thickness.Text = Sink_Size_Value.Sink_Process.Sink_Size_Thickness.ToString();
                    Sm.Sink_R.Text = Sink_Size_Value.Sink_Process.Sink_Size_R.ToString();

                }




            });
        }


    }



}
