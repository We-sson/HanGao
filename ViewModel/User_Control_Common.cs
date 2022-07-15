using Microsoft.Toolkit.Mvvm.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Extension_Method;
using HanGao.Model;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Var_Show_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Extension_Method.KUKA_ValueType_Model;
using static HanGao.Extension_Method.SetReadTypeAttribute;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Common
    {


        public User_Control_Common()
        {

            //初始化



            //发送需要读取的变量名枚举值
            Send_KUKA_Value_List(typeof(Value_Name_enum));

        }






        /// <summary>
        /// 变量名称枚举存放地方
        /// </summary>
        public enum Value_Name_enum
        {




            /// <summary>
            /// 围边工艺焊接尺寸
            /// </summary>
            Surround_Welding_size,


            /// <summary>
            /// 程序解释器Submit状态
            /// </summary>
            [StringValue("$"+nameof(PRO_STATE0)),UserArea(nameof(Meg_Value_Eunm.KUKA_State)),BingdingValue(nameof(KUKA_State_Models.KUKA_Submit_State),Value_Type.Enum, Binding_Type.OneWay )]
            PRO_STATE0,

            /// <summary>
            /// 机器人程序状态
            /// </summary>
            [StringValue("$"+nameof(PRO_STATE1)), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Program_State), Value_Type.Enum, Binding_Type.OneWay)]
            PRO_STATE1,

            ///// <summary>
            ///// 机器人操作模式
            ///// </summary>
            //[StringValue("$"+nameof(MODE_OP)), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Mode_State), Value_Type.Enum, Binding_Type.OneWay)]
            //MODE_OP,

            [StringValue("$POS_ACT"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            POS_ACT,
            [StringValue("$ACT_TOOL"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            ACT_TOOL,
            [StringValue("$ACT_BASE"), UserArea(User_Control_Working_Path_VM.Work_String_Name)]
            ACT_BASE,


            //[StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, Binding_Type.OneWay)]
            //VEL_ACT_1,
            //[StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, Binding_Type.OneWay)]
            //VEL_ACT_2,


            /// <summary>
            ///  机器人驱动状态
            /// </summary>
            [StringValue("$PERI_RDY"), UserArea(nameof(Meg_Value_Eunm.KUKA_State)), BingdingValue(nameof(KUKA_State_Models.KUKA_Drive_State), Value_Type.Bool, Binding_Type.OneWay)]
            PERI_RDY,



            //[StringValue("$Run_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, Binding_Type.TwoWay)]
            //Run_Work_1,
            //[StringValue("$Run_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, Binding_Type.TwoWay)]
            //Run_Work_2,
            //[StringValue("$My_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, Binding_Type.OneWay)]
            //My_Work_1,
            //[StringValue("$My_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, Binding_Type.OneWay)]
            //My_Work_2,


            /// <summary>
            /// 机器人运行倍率
            /// </summary>
            [StringValue("$OV_PRO")]
            OV_PRO,
            /// <summary>
            /// 机器人运动下一个点位置信息
            /// </summary>
            [StringValue("$POS_BACK")]
            POS_BACK,
            /// <summary>
            /// 机器人在轨迹中途停下笛卡尔位置信息
            /// </summary>
            [StringValue("$POS_RET")]
            POS_RET,
            /// <summary>
            /// 机器人是否激活运行
            /// </summary>
            [StringValue("$PRO_ACT")]
            PRO_ACT,
            /// <summary>
            /// 程序当前运行点名称
            /// </summary>
            [StringValue("$PRO_IP.P_NAME[]")]
            PRO_IP_P_NAME,
            /// <summary>
            /// 机器人是否运动状态
            /// </summary>
            [StringValue("$PRO_MOVE")]
            PRO_MOVE,
            /// <summary>
            /// 机器人当前运行程序名
            /// </summary>
            [StringValue("$PRO_NAME[]")]
            PRO_NAME,

            /// <summary>
            /// 机器人移动下一个点位置距离信息
            /// </summary>
            [StringValue("$DIST_NEXT")]
            DIST_NEXT,


        }













        /// <summary>
        /// 发送枚举定义库卡变量到变量显示表
        /// </summary>
        /// <param name="_Enum">定义库卡变量类型枚举</param>
        public static void  Send_KUKA_Value_List(Type _Enum)
        {
            ObservableCollection<Socket_Models_List> _List=new ObservableCollection<Socket_Models_List> ();
            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(_Enum))
            {


                 _List.Add(new Socket_Models_List() { Val_Name = item.GetStringValue(), Val_ID = Read_Number_ID, Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = item.GetBingdingValue().SetValueType,  });

            }
            WeakReferenceMessenger.Default.Send<ObservableCollection<Socket_Models_List>, string>(_List, nameof(Meg_Value_Eunm.List_Connect));

        }




       




    }
}
