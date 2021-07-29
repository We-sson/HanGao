using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Extension_Method;
using 悍高软件.Model;
using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Common : ViewModelBase
    {




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
            //机器人在轨迹中途停下位置信息
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

        }




        public User_Control_Common()
        {

            //初始化


            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
            {

                Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(new ObservableCollection<Socket_Models_List>() { new Socket_Models_List() { Val_Name = item.GetStringValue(), Val_ID = Socket_Models_Connect.Number_ID, Send_Area = item.GetAreaValue(), Value_Enum = item, Bingding_Value = item.GetBingdingValue().BingdingValue, KUKA_Value_Enum = item.GetBingdingValue().SetValueType } }, "List_Connect");
            }
        }












        /// <summary>
        /// 把功能状态写入自己泛型中
        /// </summary>
        public static void User_Check_Write_List(int W)
        {

            foreach (Sink_Models it in List_Show.SinkModels)
            {
                if (W == 1)
                {


                    if (it.Model_Number.ToString() == User_Control_Working_VM_1.WM.Work_Type)
                    {

                        it.User_Check_1.Work_Pause = User_Control_Working_VM_1.WM.Work_Pause;
                        it.User_Check_1.Work_Connt = User_Control_Working_VM_1.WM.Work_Connt;
                        it.User_Check_1.Work_NullRun = User_Control_Working_VM_1.WM.Work_NullRun;
                        it.User_Check_1.Work_JumpOver = User_Control_Working_VM_1.WM.Work_JumpOver;

                        return;
                    }
                }
                else if (W == 2)
                {

                    if (it.Model_Number.ToString() == User_Control_Working_VM_2.WM.Work_Type)
                    {

                        it.User_Check_2.Work_Pause = User_Control_Working_VM_2.WM.Work_Pause;
                        it.User_Check_2.Work_Connt = User_Control_Working_VM_2.WM.Work_Connt;
                        it.User_Check_2.Work_NullRun = User_Control_Working_VM_2.WM.Work_NullRun;
                        it.User_Check_2.Work_JumpOver = User_Control_Working_VM_2.WM.Work_JumpOver;

                        return;
                    }



                }

            }
        }

        /// <summary>
        /// 功能开关日志输出
        /// </summary>
        /// <param name="IsCheck"></param>
        /// <param name="User"></param>
        /// <param name="Fea"></param>
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
        /// 把用户的选择写入记录和打印的方法
        /// </summary>
        /// <param name="e">传入控件参数</param>
        public dynamic Log_Work_data(dynamic e)
        {


            //写入列表中泛型
            User_Check_Write_List(e.DataContext.Work_NO);
            //功能开关信息日记输出显示
            User_Features_OnOff_Log(e.IsChecked, e.DataContext.Work_NO, e.Content.ToString());





            if (e.DataContext.Work_NO == 1)
            {

                return (User_Control_Working_VM_1)e.DataContext;


            }
            else if (e.DataContext.Work_NO == 2)
            {
                return (User_Control_Working_VM_2)e.DataContext;

            }

            return null;



        }



        /// <summary>
        /// 启动触发事件命令
        /// </summary>
        public ICommand Work_Run_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                dynamic e = Sm.Source as CheckBox;
                //
                dynamic _data = Log_Work_data(e);
                ;



                if (Socket_Connect.Global_Socket_Write == null) { return; }















            });
        }



    }
}
