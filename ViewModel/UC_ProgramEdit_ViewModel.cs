using HanGao.Extension_Method;
using HanGao.Model;
using HanGao.View.User_Control.Program_Editing.Direction_UI;
using HanGao.Xml_Date.Xml_Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

                //赋值选择工艺图形
                Distance_UI= S.Sink_UI.Sink_Craft.First(X => X.Craft_Type == S.User_Picking_Craft.User_Welding_Craft ).Craft_UI_Direction;


                

                //查找到用户选择工艺后赋值到窗口显示
                //foreach (var _Craft in S.Sink_UI.Sink_Craft)
                //{
                //    if (_Craft.Craft_Type== S.User_Picking_Craft.User_Craft.User_Welding_Craft)
                //    {
                //        Distance_UI = _Craft.Craft_UI_Direction;
                //    }
                //}


      



                //Sink_Craft_Type = S.Craft_Type;


            });




            ///接收工艺参数属性显示对应UI控件
            //Messenger.Register<Sink_Models, string>(this, nameof(Meg_Value_Eunm.UC_Pop_Sink_Value_Load), (O, S) =>
            //{
            //    User_Sink = S;

            //});


        }

        /// <summary>
        /// 临时存放用户选择水槽属性
        /// </summary>
        public Sink_Models User_Sink { get; set; }


        ///// <summary>
        ///// 加载焊接方向界面
        ///// </summary>
        //public UserControl UI_Direction { get; set; }= new UC_Surround_Direction();
        ///// <summary>
        ///// 加载短边工艺界面
        ///// </summary>
        //public UserControl UI_Short_Size { get; set; }= new UC_Short_Side();



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
