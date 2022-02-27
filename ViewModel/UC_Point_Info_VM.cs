using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using PropertyChanged;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using HanGao.Model;
using HanGao.Xml_Date.Xml_WriteRead;
using Microsoft.Toolkit.Mvvm.Input;
using HanGao.View.User_Control.Pop_Ups;
using HanGao.Xml_Date.Xml_Models;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_Point_Info_VM: ObservableRecipient
    {

        public UC_Point_Info_VM()
        {
            IsActive = true;
            //接收用户选择的水槽项参数
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            {

                _Sink = S;


            });


            //接收用户选择的水槽项参数
            Messenger.Register<Xml_Craft_Date, string>(this, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value), (O, S) =>
            {


                UI_Craft_Date = S;

            });

           //获得工艺数据回传给工艺列表保存对应方向
            Messenger.Register<dynamic  ,string>(this, nameof(Meg_Value_Eunm.Sink_Craft_Data_Save), (O, S) =>
            {

                //传送用户选择工艺
                Messenger.Send<Xml_Craft_Date, string>(UI_Craft_Date, nameof(Meg_Value_Eunm.Sink_Craft_Data_OK));


            });
            



        }


        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models _Sink { get; set; }

        /// <summary>
        /// UI界面数据
        /// </summary>
        public Xml_Craft_Date UI_Craft_Date { set; get; }=new Xml_Craft_Date ();



        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Craft_Point_Loaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;

    




            });
        }


    }




}
