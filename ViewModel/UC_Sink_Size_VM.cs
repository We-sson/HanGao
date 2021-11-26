using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System.Windows.Input;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Size_VM : ViewModelBase
    {

        public UC_Sink_Size_VM()
        {



            Messenger.Default.Register<Sink_Models>(this, "Sink_Size_Value_Load", (_E) =>
            {



                Sink_Size_Value = _E;
            });


        }


        /// <summary>
        /// 水槽各参数
        /// </summary>
        public Sink_Models Sink_Size_Value { set; get; } 




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

                Sm.Sink_Long.Text = Sink_Size_Value.Sink_Process.Sink_Size_Long.ToString();
                Sm.Sink_Width.Text = Sink_Size_Value.Sink_Process.Sink_Size_Width.ToString();
                Sm.Sink_Short.Text = Sink_Size_Value.Sink_Process.Sink_Size_Short_Side.ToString();
                Sm.Sink_Thickness.Text = Sink_Size_Value.Sink_Process.Sink_Size_Thickness.ToString();
                Sm.Sink_R.Text = Sink_Size_Value.Sink_Process.Sink_Size_R.ToString();





            });
        }


    }



}
