using Microsoft.Toolkit.Mvvm.Messaging;

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
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM_2 : User_Control_Common
    {

        public Wroking_Models WM { get; set; } = new Wroking_Models() { Work_NO =int.Parse(Work_NO) };


        //功能开关类
        public User_Features UF { get; set; } = new User_Features();





        /// <summary>
        /// 传递参数区域名称：重要！
        /// </summary>
        public const    string Work_String_Name = Work_String_Name_Global + Work_NO;


        //------------------属性、字段声明------------------------


        /// <summary>
        /// 工作区号
        /// </summary>
        public const  string  Work_NO = "2";




        /// <summary>
        /// 2号加工区域
        /// </summary>
        public User_Control_Working_VM_2()
        {


            //功能属性设置
            Messenger.Default.Register<Sink_Models>(this, UserControl_Function_Set_2, (S) =>
            {


                WM.Work_Run = false;
                WM.Work_Type = S.Model_Number.ToString();
                UF.Work_Connt = S.User_Check_2.Work_Connt;
                UF.Work_Pause = S.User_Check_2.Work_Pause;
                UF.Work_NullRun = S.User_Check_2.Work_NullRun;
                UF.Work_JumpOver = S.User_Check_2.Work_JumpOver;
                User_Log_Add("加载" + S.Wroking_Models_ListBox.Work_Type + "型号到" + WM.Number_Work + "号");



            });



            //功能属性初始化
            Messenger.Default.Register<bool>(this, UserControl_Function_Reset_2, (_Bool) =>
            {
                WM.Work_Run = false;
                WM.Work_Type = "";
                UF.Work_Pause = false;
                UF.Work_Connt = true;
                UF.Work_NullRun = false;
                UF.Work_JumpOver = false;

                User_Log_Add("卸载" + WM.Number_Work + "号的加工型号");

            });




            //接收读取集合内的值方法
            Messenger.Default.Register<Socket_Models_List>(this, Work_String_Name, (Name_Val) =>
            {


            });
        }





     




      
    }


}

