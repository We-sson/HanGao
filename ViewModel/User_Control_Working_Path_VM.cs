using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.Extension_Method;
using 悍高软件.Model;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_Path_VM : ViewModelBase
    {
        /// <summary>
        /// 变量名称枚举存放地方
        /// </summary>
        [Flags]
        public enum Value_Name_enum
        {
            [StringValue("$POS_ACT"), UserArea(Work_String_Name)]
            POS_ACT,
            [StringValue("$ACT_TOOL"), UserArea(Work_String_Name)]
            ACT_TOOL,
            [StringValue("$ACT_BASE"), UserArea(Work_String_Name)]
            ACT_BASE
        }


        public User_Control_Working_Path_VM()
        {

            //发送需要读取的变量名枚举值
            foreach (Enum item in Enum.GetValues(typeof(Value_Name_enum)))
            {
                Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(new ObservableCollection<Socket_Models_List>() { new Socket_Models_List() { Val_Name = item.GetStringValue(), Val_ID = Socket_Models_Connect.Number_ID, Send_Area=Work_String_Name, Value_Enum=item},  }, "List_Connect");

            }








            //接收读取集合内的值方法
            Messenger.Default.Register<Socket_Models_List>(this, Work_String_Name, (Name_Val) =>
            {

                switch (Name_Val.Value_Enum)
                {
                    //case Value_Name_enum.POS_ACT:
                    //    Working_Path.KUKA_Now_Point_Show = Name_Val.Val_Var;
                    //    break;
                    case Value_Name_enum.ACT_TOOL:
                        Working_Path.KUKA_TOOL_Number = Name_Val.Val_Var;

                        break;
                    case Value_Name_enum.ACT_BASE:
                        Working_Path.KUKA_Base_Number = Name_Val.Val_Var;

                        break;

                }



            }




            );



        }


        /// <summary>
        /// 传递参数区域名称：重要！
        /// </summary>
        public const string Work_String_Name = "Show_Reveice_Control";



        /// <summary>
        /// 前段绑定显示坐标属性
        /// </summary>
        public User_Working_Path_Models Working_Path { set; get; } = new User_Working_Path_Models() { };


    }


}
