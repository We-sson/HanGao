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
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static HanGao.Model.Sink_Models;
using static HanGao.Xml_Date.Xml_WriteRead.User_Read_Xml_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_VM : ObservableRecipient
    {


        //------------------属性、字段声明------------------------


        public User_Control_Working_VM()
        {

            //接收修改参数属性
            Messenger.Register<Wroking_Models, string>(this, nameof(Meg_Value_Eunm.UI_Work_No), (O, S) =>
            {


                if (S.UI_Sink_Show.User_Picking_Craft.User_Work_Area  == Work_No_Enum.N_1)
                {


                    UC_Working_VM_1 = S;


                }
                else if (S.UI_Sink_Show.User_Picking_Craft.User_Work_Area == Work_No_Enum.N_2)
                {

                    UC_Working_VM_2 = S;


                }




            });



        }




        public  Wroking_Models UC_Working_VM_1 { get; set; } = new Wroking_Models() { Work_NO= Work_No_Enum.N_1,  };

        public  Wroking_Models UC_Working_VM_2 { get; set; }= new Wroking_Models() { Work_NO= Work_No_Enum.N_2 };







        //------------------方法体------------------------



        //------------------方法体------------------------

    }
}
