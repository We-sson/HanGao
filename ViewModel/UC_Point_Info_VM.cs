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
using HanGao.View.User_Control.Program_Editing.Point_info;

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

                User_Sink = S;


            });


            //接收用户选择的水槽项参数
            Messenger.Register<Xml_Craft_Date, string>(this, nameof(Meg_Value_Eunm.Sink_Surround_Craft_Selected_Value), (O, S) =>
            {


                UI_Craft_Date = S;

            });




        }



        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models User_Sink { get; set; }

        /// <summary>
        /// UI界面数据
        /// </summary>
        public Xml_Craft_Date UI_Craft_Date { set; get; }



        /// <summary>
        /// 保存属性水槽类型到XMl
        /// </summary>
        public ICommand Craft_Point_Info_SaveToXml_Comm
        {
            get => new RelayCommand<UC_Point_Info>((Sm) =>
            {
                //把参数类型转换控件



                XML_Write_Read.Save_Xml();

            });
        }



        /// <summary>
        /// 保存属性水槽类型到XMl
        /// </summary>
        public ICommand Craft_Point_Info_SaveToVal_Comm
        {
            get => new RelayCommand<UC_Point_Info>((Sm) =>
            {
                //把参数类型转换控件


             //XML_Write_Read.SetXml_User_Data(new User_Read_Xml_Model(User_Sink.Sink_Model,  ) );

                //Xml_Sink_Model _Sink_List = XML_Write_Read.GetXml_Sink_Models_Data(User_Sink.Sink_Model);

                //Xml_SInk_Craft a = (Xml_SInk_Craft)_Sink_List.Sink_Craft.GetType().GetProperty(User_Sink.Work_No_Emun.ToString()).GetValue(_Sink_List.Sink_Craft);

                //a.Sink_Surround_Craft.GetType();


                //XML_Write_Read.Save_Xml();

            });
        }

    }




}
