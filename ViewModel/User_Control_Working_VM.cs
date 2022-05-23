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
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using static HanGao.Extension_Method.KUKA_ValueType_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM : User_Control_Common
    {


        //------------------属性、字段声明------------------------


        public static Wroking_Models UC_Working_VM_1 { get; set; } = new Wroking_Models() { Work_NO=1, UI_Show= Visibility.Collapsed };

        public static Wroking_Models UC_Working_VM_2 { get; set; }= new Wroking_Models() { Work_NO=2 };







        //------------------方法体------------------------



        //------------------方法体------------------------

    }
}
