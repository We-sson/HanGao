using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Type_VM : ViewModelBase
    {
        public UC_Sink_Type_VM()
        {


            Messenger.Default.Register<Photo_Sink_Enum>(this, "Sink_Type_Value_Load", (_E) =>
            {


                switch (_E)
                {
                    case Photo_Sink_Enum.左右单盆:
                        Sink_LR_Checked = true;
                        break;
                    case Photo_Sink_Enum.上下单盆:
                        Sink_UpDown_Checked = true;
                        break;
                    case Photo_Sink_Enum.普通双盆:
                        Sink_Twin_Checked = true;
                        break;
                    default:
                        break;
                }
            });




        }


        private  Photo_Sink_Enum _Sink_Type_Load;
        /// <summary>
        /// 用户选择的水槽类型
        /// </summary>
        public     Photo_Sink_Enum Sink_Type_Load
        {
            set
            { 
                _Sink_Type_Load = value;
                switch (value)
                {
                    case Photo_Sink_Enum.左右单盆:

                        break;
                    case Photo_Sink_Enum.上下单盆:
                        break;
                    case Photo_Sink_Enum.普通双盆:
                        break;
                    default:
                        break;
                }
            }


            get { return _Sink_Type_Load; } 
        }





        private bool _Sink_LR_Checked;
        public bool Sink_LR_Checked
        {
            get { return _Sink_LR_Checked; }
            set
            {
                _Sink_LR_Checked = value;
                if (value == true)
                {

                    Sink_Type_Load = Photo_Sink_Enum.左右单盆;
                }
            }
        }
        private bool _Sink_UpDown_Checked;
        public bool Sink_UpDown_Checked
        {
            get { return _Sink_UpDown_Checked; }
            set
            {
                _Sink_UpDown_Checked = value;
                if (value == true)
                {
                    Sink_Type_Load = Photo_Sink_Enum.上下单盆;

                }

            }
        }
        private bool _Sink_Twin_Checked;
        public bool Sink_Twin_Checked
        {
            get { return _Sink_Twin_Checked; }
            set
            {
                _Sink_Twin_Checked = value;
                if (value == true)
                {
                    Sink_Type_Load = Photo_Sink_Enum.普通双盆;
                }
            }
        }
        //public bool Sink_LeftRight_Checked = {set;get;}



        /// <summary>
        /// 现在原属性水槽类型
        /// </summary>
        public ICommand Sink_Type_Unloaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                UC_Sink_Type e = Sm.Source as UC_Sink_Type;


                //UC_Sink_Size_VM.Sink_Size_Value.Photo_Sink_Type = Sink_Type_Load;
                //Messenger.Default.Send<Photo_Sink_Enum>(Sink_Type_Load, "Sink_Type_Value_OK");





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

                //用户设置水槽属性
                //Sink_Type_Load.Photo_Sink_Type = (Photo_Sink_Enum)int.Parse((String)e.Tag);








            });
        }



















    }
}
