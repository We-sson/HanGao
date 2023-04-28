







using static HanGao.Model.Sink_Craft_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;


namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UC_ProgramEdit_ViewModel: ObservableRecipient
    {


        public UC_ProgramEdit_ViewModel()
        {
            IsActive = true;







            ///接收工艺参数属性显示对应UI控件
            Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.Program_UI_Load), (O, S) =>
            {
                Distance_UI = null;
                //赋值选择工艺图形
                Distance_UI = S.Sink_UI.Sink_Craft.First(X => X.Craft_Type == S.User_Picking_Craft.User_Welding_Craft ).Craft_UI_Direction;

                User_Sink = S;


            });




 

        }

        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models User_Sink { get; set; }


        public enum KUKA_Read_Info_Value_Enum
        {
            [StringValue("Welding_Name[]")]
           Welding_Name,
            [StringValue("Welding_Speed")]
            Welding_Speed,
            Welding_Pos,
            [StringValue("Welding_Angle")]
            Welding_Angle,
            Welding_Offset,
        }




        /// <summary>
        /// 方向UI部件
        /// </summary>
        public UserControl Distance_UI { set; get; }





        /// <summary>
        /// 保存用户修改工艺动作
        /// </summary>
        public ICommand Craft_Save_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;


                //Messenger.Send <dynamic ,string>( nameof(Meg_Value_Eunm.Sink_Craft_Data_Save));



            });
        }


        /// <summary>
        /// 读取机器人数据工艺动作
        /// </summary>
        public ICommand Read_Robot_Craft_Data_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                //FrameworkElement e = Sm.Source as FrameworkElement;





            });
        }

    }
}
