
using PropertyChanged;
using System;
using System.ComponentModel;
using HanGao.Socket_KUKA;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;

using Soceket_KUKA.Models;
using System.Threading;
using System.Windows;
using static HanGao.Model.Sink_Models;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using static HanGao.Model.User_Read_Xml_Model;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;


namespace HanGao.Model
{
[AddINotifyPropertyChangedInterface]
    public class Working_Area_Data : ObservableRecipient
    {
        public Working_Area_Data()
        {


            //设置UI显示
            Messenger.Register<dynamic, string>(this, nameof(Meg_Value_Eunm.Socket_Read_Thread), (O, _S) =>
            {




            });








        }



       /// <summary>
       /// UI界面水槽尺寸显示
       /// </summary>
       public Sink_Models User_Sink { set; get; }


        public Working_Area_UI_Model Working_Area_UI { set; get; }


    };

    [AddINotifyPropertyChangedInterface]
    public class Working_Area_UI_Model
    {
        public Working_Area_UI_Model()
        {

        }


        /// <summary>
        /// 加载工作区UI的区域
        /// </summary>
        public Work_No_Enum Load_UI_Work { set; get; }

       /// <summary>
       /// UI工作区显示
       /// </summary>
        public Visibility UI_Show { set; get; } = Visibility.Visible;


        /// <summary>
        /// 工作区设置编号
        /// </summary>
        public Work_No_Enum Work_NO { set; get; }

        /// <summary>
        /// 发送数据到机器人显示状态
        /// </summary>
        public UI_Type_Enum UI_Loade { set; get; } = UI_Type_Enum.Ok;


        /// <summary>
        /// 显示机器人速度
        /// </summary>
        public double Robot_Speed { set; get; } = 1;

        public double UI_Robot_Speed
        {
            set
            {
                UI_Robot_Speed = value;
            }
            get
            {
                var a = Robot_Speed / (double)(2.0 / 360);
                return a;
            }
        }
        /// <summary>
        /// 显示机器人焊接功率
        /// </summary>
        public double Welding_Power { set; get; } = 50;


        public double UI_Welding_Power
        {
            set
            {
                UI_Welding_Power = value;
            }
            get
            {
                return Welding_Power / (100.0 / 360);
            }
        }
        /// <summary>
        /// 显示机器人焊接周期时间
        /// </summary>
        public double Welding_Time { set; get; } = 10;

        public double UI_Welding_Time
        {
            set
            {
                UI_Welding_Time = value;
            }
            get
            {
                return Welding_Time / (120.0 / 360);
            }
        }





    }


}







