﻿using Microsoft.Toolkit.Mvvm.Messaging;

using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Extension_Method;
using HanGao.Model;
using HanGao.Socket_KUKA;

using static Soceket_KUKA.Models.KUKA_Value_Type;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using static HanGao.Extension_Method.KUKA_ValueType_Model;

namespace HanGao.Model
{
    public  class UC_Working_Models
    {


        /// <summary>
        /// 1号加工区域初始化
        /// </summary>
        public UC_Working_Models()
        {





            ////功能属性设置
            //WeakReferenceMessenger.Default.Register<Sink_Models, string>(this, UserControl_Function_Set_1, (O, S) =>
            //{
            //    var a = S.Get_Surround_Struc_String();

            //    WM.Work_Run = false;
            //    WM.Work_Type = S.Sink_Model.ToString();
            //    UF.Work_Connt = S.User_Check_1.Work_Connt;
            //    UF.Work_Pause = S.User_Check_1.Work_Pause;
            //    UF.Work_NullRun = S.User_Check_1.Work_NullRun;
            //    UF.Work_JumpOver = S.User_Check_1.Work_JumpOver;
            //    User_Log_Add("加载" + S.Wroking_Models_ListBox.Work_Type + "型号到" + WM.Number_Work + "号");



            //    //使用多线程写入
            //    new Thread(new ThreadStart(new Action(() =>
            //    {
            //        Socket_Client_Setup.Write.Cycle_Write_Send(nameof(Value_Name_enum.Surround_Welding_size), a);
            //    })))
            //    { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

            //});



            ////功能属性初始化
            //WeakReferenceMessenger.Default.Register<dynamic, string>(this, UserControl_Function_Reset_1, (O, _Bool) =>
            //{
            //    WM.Work_Run = false;
            //    WM.Work_Type = "";
            //    UF.Work_Pause = false;
            //    UF.Work_Connt = true;
            //    UF.Work_NullRun = false;
            //    UF.Work_JumpOver = false;

            //    User_Log_Add("卸载" + WM.Number_Work + "号的加工型号");

            //});






            ////接收读取集合内的值方法
            //WeakReferenceMessenger.Default.Register<Socket_Models_List, string>(this, Work_String_Name, (O, Name_Val) =>
            //{
            //    //互斥线程锁，保证每次只有一个线程接收消息
            //    Receive_Lock.WaitOne();

            //    try
            //    {



            //        //发送需要读取的变量名枚举值
            //        foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
            //        {
            //            var q = item.GetStringValue();
            //            var x = item.GetBingdingValue().BingdingValue;

            //            //判断读取标记名称相同时
            //            if (Name_Val.Val_Name == item.GetStringValue())
            //            {

            //                //判断是否设置时候双向绑定
            //                if (item.GetBingdingValue().Binding_Start == Binding_Type.TwoWay)
            //                {

            //                    var a = WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).GetValue(WM).ToString();
            //                    //判断读取变量时候bool类型,更改类型
            //                    if (item.GetBingdingValue().SetValueType == Value_Type.Bool)
            //                    {
            //                        //将小写转换大写
            //                        a = a.ToUpper();

            //                    }
            //                    //判断双方属性是否相同
            //                    if (Name_Val.Val_Var != a)
            //                    {

            //                        //属性不相同时，以软件端为首发送更改发送机器端


            //                        //使用多线程写入
            //                        new Thread(new ThreadStart(new Action(() =>
            //                        {
            //                            Socket_Client_Setup.Write.Cycle_Write_Send(item.GetStringValue(), a);
            //                        })))
            //                        { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();



            //                    }






            //                }
            //                else
            //                {


            //                    if (item.GetBingdingValue().BingdingValue != "")
            //                    {

            //                        WM.GetType().GetProperty(item.GetBingdingValue().BingdingValue).SetValue(WM, Name_Val.Val_Var);
            //                    }

            //                }
            //            }




            //        }

            //    }
            //    catch (Exception e)
            //    {

            //        Socket_Client_Setup.Write.Socket_Receive_Error(Read_Write_Enum.Write, "Error: -30 原因:" + e.Message);
            //    }



            //    //接收信息互斥线程锁，保证每次只有一个线程接收消息
            //    Receive_Lock.ReleaseMutex();

            //});






       


        }


        //Ui显示类
        public Wroking_Models WM { get; set; } = new Wroking_Models()
        {


        };



    }
}