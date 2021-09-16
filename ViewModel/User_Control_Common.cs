using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Extension_Method;
using 悍高软件.Model;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;


namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Common : ViewModelBase
    {


        #region -----属性、字段声明-----


        public const   string UserControl_Function_Reset = "UserControl_Function_Reset" ;
        public const   string UserControl_Function_Set = "UserControl_Function_Set";
       
        /// <summary>
        /// 传递参数区域全局名称：重要！
        /// </summary>
        public const    string Work_String_Name_Global = "Show_Reveice_method_Bool";


        /// <summary>
        /// 变量名称枚举存放地方
        /// </summary>
        [Flags]
        public enum Value_Name_enum
        {
            [StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, false)]
            VEL_ACT_1,
            [StringValue("$VEL_ACT"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Robot_Speed", Value_Type.Int, false)]
            VEL_ACT_2,
            //程序解释器状态
            [StringValue("$PRO_STATE")]
            PRO_STATE,
            [StringValue("$MODE_OP"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Robot_status", Value_Type.String, false)]
            MODE_OP_1,
            [StringValue("$MODE_OP"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Robot_status", Value_Type.String, false)]
            MODE_OP_2,
            [StringValue("$Run_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, true)]
            Run_Work_1,
            [StringValue("$Run_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Run", Value_Type.Bool, true)]
            Run_Work_2,
            [StringValue("$My_Work_1"), UserArea(User_Control_Working_VM_1.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, true)]
            My_Work_1,
            [StringValue("$My_Work_2"), UserArea(User_Control_Working_VM_2.Work_String_Name), BingdingValue("Work_Type", Value_Type.String, true)]
            My_Work_2,
            [StringValue("$my_work_number")]
            my_work_number,
            //机器人运行倍率
            [StringValue("$OV_PRO")]
            OV_PRO,
            //机器人运动下一个点位置信息
            [StringValue("$POS_BACK")]
            POS_BACK,
            //机器人在轨迹中途停下笛卡尔位置信息
            [StringValue("$POS_RET")]
            POS_RET,
            //机器人是否激活运行
            [StringValue("$PRO_ACT")]
            PRO_ACT,
            //程序当前运行点名称
            [StringValue("$PRO_IP.P_NAME[]")]
            PRO_IP_P_NAME,
            //机器人是否运动状态
            [StringValue("$PRO_MOVE")]
            PRO_MOVE,
            //机器人当前运行程序名
            [StringValue("$PRO_NAME[]")]
            PRO_NAME,
            //机器人中断位置轴位置数据信息
            [StringValue("$AXIS_INT")]
            AXIS_INT,
            //机器人移动下一个点位置距离信息
            [StringValue("$DIST_NEXT")]
            DIST_NEXT,
            //机器人各轴扭矩数据信息
            [StringValue("$TORQUE_AXIS_ACT[]")]
            TORQUE_AXIS_ACT,
            //机器人驱动是否运行
            [StringValue("$PERI_RDY")]
            PERI_RDY
        }

        #endregion



        #region  -----消息通知名称声明-----




        /// <summary>
        /// 工作区1：功能初始化，消息通道字典名称
        /// </summary>
        public const string UserControl_Function_Reset_1 = UserControl_Function_Reset + User_Control_Working_VM_1.Work_NO;


        /// <summary>
        /// 工作区1：功能设置，消息通道字典名称
        /// </summary>
        public const string UserControl_Function_Set_1 = UserControl_Function_Set + User_Control_Working_VM_1.Work_NO;


        /// <summary>
        /// 工作区2：功能初始化，消息通道字典名称
        /// </summary>
        public const string UserControl_Function_Reset_2 = UserControl_Function_Reset + User_Control_Working_VM_2. Work_NO;

        /// <summary>
        /// 工作区2：功能设置，消息通道字典名称
        /// </summary>
        public const string UserControl_Function_Set_2 = UserControl_Function_Set + User_Control_Working_VM_2.Work_NO;


        #endregion





        #region ----初始化、消息通知接收处-----

        public User_Control_Common()
        {

            //初始化



            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
            {

                Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(new ObservableCollection<Socket_Models_List>() { new Socket_Models_List() { Val_Name = item.GetStringValue(), Val_ID = Socket_Models_Connect.Number_ID, Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = item.GetBingdingValue().SetValueType } }, "List_Connect");
            }
        }

        #endregion



        #region -----方法体-----


        /// <summary>
        /// 功能开关日志输出
        /// </summary>
        /// <param name="IsCheck">开关</param>
        /// <param name="User">工作区号码</param>
        /// <param name="Fea">功能名称</param>
        public void User_Features_OnOff_Log(bool? IsCheck, int User, string Fea)
        {
            if (IsCheck == true)
            {
                User_Control_Log_ViewModel.User_Log_Add("NO." + User.ToString() + Fea + "功能开启");
            }
            else if (IsCheck == false)
            {
                User_Control_Log_ViewModel.User_Log_Add("NO." + User.ToString() + Fea + "功能关闭");

            }


        }


        /// <summary>
        /// 传入触发控件，返回对应触发的DataContext，否则空
        /// </summary>
        /// <param name="e">传入控件</param>
        public object  Log_Work_data( RoutedEventArgs _e)
        {


            dynamic  Data = (_e.Source as CheckBox).DataContext;



            if (Data.WM.Work_NO == 1)
            {

                return (User_Control_Working_VM_1)Data;


            }
            else if (Data.WM.Work_NO == 2)
            {
                return (User_Control_Working_VM_2)Data;

            };

            return null;



        }



        /// <summary>
        /// 启动触发事件命令
        /// </summary>
        public ICommand Work_Run_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                //
                dynamic  S = e.DataContext;

                
                var a = this.GetType().GetProperty("WM").GetValue(this );





            });
        }

        /// <summary>
        /// 加工区域加载事件命令
        /// </summary>
        public ICommand User_Loaded_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            {
            
            //把参数类型转换控件
            UserControl e = Sm.Source as UserControl;

                dynamic S = e.DataContext;


                var a = this.GetType().GetProperty("WM").GetValue(this);
            });
        }


        /// <summary>
        /// 加工区域计数事件命令
        /// </summary>
        public ICommand Work_Connt_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                dynamic S = e.DataContext;


                var a = this.GetType().GetProperty("WM").GetValue(this);
            });
        }






        /// <summary>
        /// 加工区域空运事件命令
        /// </summary>
        public ICommand Work_NullRun_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                dynamic S = e.DataContext;


                var a = this.GetType().GetProperty("WM").GetValue(this);
            });
        }







        /// <summary>
        /// 加工区域暂停事件命令
        /// </summary>
        public ICommand Work_Pause_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                dynamic S = e.DataContext;


                var a = this.GetType().GetProperty("WM").GetValue(this);

            });
        }








        /// <summary>
        /// 加工区域跳过事件命令
        /// </summary>
        public ICommand Work_JumpOver_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm)=> 
            {
                //把参数类型转换控件
                CheckBox e = Sm.Source as CheckBox;
                dynamic S = e.DataContext;


                var a = this.GetType().GetProperty("WM").GetValue(this);

            });
        }



        #endregion

    }
}
