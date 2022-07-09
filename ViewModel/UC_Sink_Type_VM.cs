using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using HanGao.View.User_Control.Pop_Ups;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;
using System.Threading.Tasks;
using HanGao.Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.SInk_UI_Models;
using System;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Type_VM : ObservableRecipient
    {

    

        public UC_Sink_Type_VM()
        {
            IsActive = true;





            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {


                _Sink = S;
                //Sink_Type_Load = _Sink.Sink_Process.Sink_Type;
                switch (_Sink.Sink_Process.Sink_Type)
                {
                    case Sink_Type_Enum.LeftRight_One:
                        Sink_LR_Checked = true;
                        break;
                    case Sink_Type_Enum.UpDown_One:
                        Sink_UpDown_Checked = true;
                        break;
                    case Sink_Type_Enum.LeftRight_Two:
                         Sink_Two_Checked = true;
                        break;
                    default:
                        break;



                }


            });





            
        }

        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models _Sink { get; set; } 



 
        /// <summary>
        /// 用户选择的水槽类型
        /// </summary>
        public Sink_Type_Enum Sink_Type_Load { set; get; }





        //private bool _Sink_LR_Checked=true;
        public   bool Sink_LR_Checked { set; get; }
        //{
        //    get { return _Sink_LR_Checked; }
        //    set
        //    {
        //        _Sink_LR_Checked = value;

        //        //if (value)
        //        //{
        //        //    Messenger.Send<dynamic, string>(Sink_Type_Enum.LeftRight_One, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
        //        //}

        //    }
        //}
        //private   bool _Sink_UpDown_Checked;
        public   bool Sink_UpDown_Checked { set; get; }
        //{
        //    get { return _Sink_UpDown_Checked; }
        //    set
        //    {
        //        _Sink_UpDown_Checked = value;

        //        //if (value)
        //        //{
        //        //    Messenger.Send<dynamic, string>(Sink_Type_Enum.UpDown_One, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
        //        //}


        //    }
        //}
        //private   bool _Sink_Twin_Checked;
        public   bool Sink_Two_Checked { set; get; }
        //{
        //    get { return _Sink_Twin_Checked; }
        //    set
        //    {
        //        _Sink_Twin_Checked = value;
        //        //if (value)
        //        //{
        //        //    Messenger.Send<dynamic, string>(Sink_Type_Enum.LeftRight_Two, nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
        //        //}

        //    }
        //}



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



            });
        }










        /// <summary>
        /// 修改水槽类型选择事件
        /// </summary>
        public ICommand Sink_Type_Set_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                FrameworkElement e = Sm.Source as FrameworkElement;

                //用户设置水槽属性
                //Sink_Type_Load.Photo_Sink_Type = (Photo_Sink_Enum)int.Parse((String)e.Tag);

                   Messenger.Send<dynamic, string>(Enum.Parse(typeof(Sink_Type_Enum), e.Name), nameof(Meg_Value_Eunm.Sink_Type_Value_OK));
                

                



            });
        }


    }
}
