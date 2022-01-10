using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.View.User_Control.Pop_Ups;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System;
using HanGao.Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class UC_Sink_Type_VM : ObservableRecipient
    {

    

        public UC_Sink_Type_VM()
        {
            IsActive = true;


            //打开读取用户点击水槽类型
            Messenger.Register<dynamic , string >(this, nameof( Meg_Value_Eunm.Sink_Type_Value_Load),     (O,T) =>
            {


                switch ((Sink_Type_Enum)T)
                {
                    case Sink_Type_Enum.LeftRight_One:
                        Sink_LR_Checked = true;
                        break;
                    case Sink_Type_Enum.UpDown_One:
                        Sink_UpDown_Checked = true;
                        break;
                    case Sink_Type_Enum.LeftRight_Two:
                        Sink_Twin_Checked = true;
                        break;
                    default:
                        break;



                }
                //<Photo_Sink_Enum>
                //UC_Sink_Type_VM.Sink_Type_Load = (Photo_Sink_Enum)_E.Sink_Type_Load;

                //Sink_Type_Load = (Photo_Sink_Enum)_E;


                //MessageBox.Show(Sink_Type_Load.ToString());



            });

            //Messenger.Send(this, Meg_Value_Eunm.Sink_Type_Value_Save.ToString());



            
        }


        public Sink_Models[] Sink { get; set; } 



        private Sink_Type_Enum _Sink_Type_Load;
        /// <summary>
        /// 用户选择的水槽类型
        /// </summary>
        public Sink_Type_Enum Sink_Type_Load
        {
            set
            {
                //if (Sink_Type_Load != Photo_Sink_Enum.空)
                //{

                //if (value != Sink_Type_Load )
                //{
              

                //}
                //}
                _Sink_Type_Load = value;

            }


            get { return _Sink_Type_Load; }
        }





        private bool _Sink_LR_Checked;
        public   bool Sink_LR_Checked
        {
            get { return _Sink_LR_Checked; }
            set
            {
                _Sink_LR_Checked = value;

                if (value)
                {
                    Messenger.Send<dynamic, string>(Sink_Type_Enum.LeftRight_One, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
                }

                //MessageBox.Show("_Sink_LR_Checked=" + value.ToString());

                //SetProperty(ref _Sink_LR_Checked, value);
                //if (value == true || Sink_LR_Checked != _Sink_LR_Checked)
                //{
                //    Sink_Type_Load = Photo_Sink_Enum.左右单盆;

                //}
            }
        }
        private   bool _Sink_UpDown_Checked;
        public   bool Sink_UpDown_Checked
        {
            get { return _Sink_UpDown_Checked; }
            set
            {
                _Sink_UpDown_Checked = value;

                if (value)
                {
                    Messenger.Send<dynamic, string>(Sink_Type_Enum.UpDown_One, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
                }
                //MessageBox.Show("_Sink_UpDown_Checked="+value.ToString());

                //SetProperty(ref _Sink_UpDown_Checked, value);
                //if (value == true || Sink_UpDown_Checked != _Sink_UpDown_Checked)
                //{
                //    Sink_Type_Load = Photo_Sink_Enum.上下单盆;

                //}

            }
        }
        private   bool _Sink_Twin_Checked;
        public   bool Sink_Twin_Checked
        {
            get { return _Sink_Twin_Checked; }
            set
            {
                _Sink_Twin_Checked = value;
                if (value)
                {
                    Messenger.Send<dynamic, string>(Sink_Type_Enum.LeftRight_Two, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
                }
                //MessageBox.Show("Sink_Twin_Checked=" + value.ToString());

                //SetProperty(ref _Sink_Twin_Checked, value);

                //if (value == true || Sink_Twin_Checked != _Sink_Twin_Checked)
                //{
                //    Sink_Type_Load = Photo_Sink_Enum.普通双盆;

                //}
            }
        }
        //public bool Sink_LeftRight_Checked = {set;get;}



        /// <summary>
        /// 现在原属性水槽类型
        /// </summary>
        public ICommand Sink_Type_Unloaded_Comm
        {
            get => new AsyncRelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                UC_Sink_Type e = Sm.Source as UC_Sink_Type;

                return Task.Run(() => 
                {
                
                
                });
                //UC_Sink_Size_VM.Sink_Size_Value.Photo_Sink_Type = Sink_Type_Load;





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
