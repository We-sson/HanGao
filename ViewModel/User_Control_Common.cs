﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_KUKA.Models;
using System.Collections.ObjectModel;
using 悍高软件.Model;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Common : ViewModelBase
    {


        public User_Control_Common()
        {

            //初始化





            Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(_List, "List_Connect");








        }


        /// <summary>
        /// 程序初始化
        /// </summary>
        public void Program_Initialization()
        {





        }




        /// <summary>
        /// 临时变量属性集合
        /// </summary>
        public ObservableCollection<Socket_Models_List> _List { set; get; } = new ObservableCollection<Socket_Models_List>()
        {
            new Socket_Models_List() { Val_Name = "$VEL_ACT", Val_ID = Socket_Models_Connect.Number_ID },
            new Socket_Models_List() { Val_Name = "$PRO_STATE", Val_ID = Socket_Models_Connect.Number_ID },
            new Socket_Models_List() { Val_Name = "$MODE_OP", Val_ID = Socket_Models_Connect.Number_ID }
        };



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






    }
}