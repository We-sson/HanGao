using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HanGao.Model;
using HanGao.View.User_Control.Pop_Ups;
using PropertyChanged;
using System.Windows.Input;
using static HanGao.Model.Sink_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Size_VM : ViewModelBase
    {

        public UC_Sink_Size_VM()
        {


            //接收数据发送到尺寸窗口数据
            Messenger.Default.Register<Sink_Models>(this, "Sink_Size_Value_Load", (_Size) =>
            {

                Sink_Size_Value = _Size;
            });


            Messenger.Default.Register<Photo_Sink_Enum>(this, "Sink_Type_Value_OK", (_T) =>
            {

                Sink_Size_Value.Photo_Sink_Type = _T;
            });

        }




        /// <summary>
        /// 水槽各参数
        /// </summary>
        public   Sink_Models Sink_Size_Value { set; get; }




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

                if (Sink_Size_Value !=null )
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
