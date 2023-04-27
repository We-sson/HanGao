
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using HanGao.Xml_Date.Xml_Write_Read;
using HanGao.View.User_Control.Program_Editing.Point_info;
using HanGao.Xml_Date.Vision_XML.Vision_WriteRead;

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
        public ICommand Craft_Point_Info_Close_Comm
        {
            get => new RelayCommand<MainWindow>((Sm) =>
            {
                //把参数类型转换控件



                //XML_Write_Read.Save_Xml();


                ///清楚程序编辑界面数据选项
                Messenger.Send<dynamic, string>(false, nameof(Meg_Value_Eunm.Surround_Direction_Rest));
                Messenger.Send<dynamic, string>(false, nameof(Meg_Value_Eunm.Direction_Info_Rest));
                UI_Craft_Date = null;

                FrameShow.Home_Console_UI = true;
                FrameShow.Program_Edit_UI = false;


            });
        }





        /// <summary>
        /// 保存属性水槽类型到XMl
        /// </summary>
        public ICommand Craft_Point_Info_SaveToXml_Comm
        {
            get => new RelayCommand<MainWindow>((Sm) =>
            {
                //把参数类型转换控件

                XML_Write_Read.Sink_Date.Date_Last_Modify = DateTime.Now.ToString();


                   Vision_Xml_Method.Save_Xml(XML_Write_Read.Sink_Date);


                ///清楚程序编辑界面数据选项
                Messenger.Send<dynamic, string>(true , nameof(Meg_Value_Eunm.Surround_Direction_Rest));
                Messenger.Send<dynamic, string>(true , nameof(Meg_Value_Eunm.Direction_Info_Rest));
                UI_Craft_Date = null;

                //FrameShow.ProgramEdit_Enabled = false;
                //FrameShow.HomeOne_UI = true;
                FrameShow.Home_Console_UI = true;
                FrameShow.Program_Edit_UI= false ;
             


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


                if (UI_Craft_Date   is null) { return; }
      
                Xml_Craft_Date New_Set_Data = UI_Craft_Date;

                
                New_Set_Data.Welding_Offset.X = Sm.S_X.Value;
                New_Set_Data.Welding_Offset.Y = Sm.S_Y.Value;
                New_Set_Data.Welding_Offset.Z = Sm.S_Z.Value;
                New_Set_Data.Welding_Offset.A = Sm.S_A.Value;
                New_Set_Data.Welding_Offset.B = Sm.S_B.Value;
                New_Set_Data.Welding_Offset.C = Sm.S_C.Value ;
                New_Set_Data.Welding_CDIS= (int)Sm.S_CDIS.Value;
                New_Set_Data.Welding_ACC = (int)Sm.S_ACC.Value ;
                New_Set_Data.Welding_Speed = Sm.S_Speed.Value;
                New_Set_Data.Welding_Angle = Sm.S_Angle.Value;
                New_Set_Data.Welding_Power = (int)Sm.S_Power.Value;





                XML_Write_Read.Set_User_Sink_Data(User_Sink, New_Set_Data);

      



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
