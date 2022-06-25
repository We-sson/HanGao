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

                Xml_Craft_Date New_Set_Data = UI_Craft_Date;
                New_Set_Data.Welding_Offset.X = double.Parse(Sm.X.Text);
                New_Set_Data.Welding_Offset.Y = double.Parse(Sm.Y.Text);
                New_Set_Data.Welding_Offset.Z = double.Parse(Sm.Z.Text);
                New_Set_Data.Welding_Offset.A = double.Parse(Sm.A.Text);
                New_Set_Data.Welding_Offset.B = double.Parse(Sm.B.Text);
                New_Set_Data.Welding_Offset.C = double.Parse(Sm.C.Text);
                New_Set_Data.Welding_CDIS= int.Parse(Sm.CDIS.Text);
                New_Set_Data.Welding_ACC = int.Parse(Sm.ACC.Text);
                New_Set_Data.Welding_Speed = double.Parse(Sm.Speed.Text);
                New_Set_Data.Welding_Angle = double .Parse(Sm.Angle.Text);
                New_Set_Data.Welding_Power = int.Parse(Sm.Power.Text);





                XML_Write_Read.SetXml_User_Data(User_Sink, New_Set_Data);

      



            });
        }



        /// <summary>
        /// 复位界面水槽工艺显示数据
        /// </summary>
        public ICommand Craft_Point_Info_Restoration_Comm
        {
            get => new RelayCommand<UC_Point_Info>((Sm) =>
            {
          
               
            });
        }


    }




}
