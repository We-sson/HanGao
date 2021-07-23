using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.Model;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Working_Path_VM : ViewModelBase
    {
        public User_Control_Working_Path_VM()
        {


            //发送需要读取的变量名
            Messenger.Default.Send<ObservableCollection<Socket_Models_List>>(_List, "List_Connect");









            //消息注册

            Messenger.Default.Register<Socket_Models_List>(this, Work_String_Name, Show_Reveice_Control);



        }

        /// <summary>
        /// 传递参数区域名称：重要！
        /// </summary>
        public static string Work_String_Name {  get; } = "Show_Reveice_Control";









        /// <summary>
        /// 前段绑定显示坐标属性
        /// </summary>
        public User_Working_Path_Models Working_Path { set; get; } = new User_Working_Path_Models() { };



        /// <summary>
        /// 临时变量属性集合
        /// </summary>
        public ObservableCollection<Socket_Models_List> _List { set; get; } = new ObservableCollection<Socket_Models_List>()
        {
            new Socket_Models_List() { Val_Name = "$POS_ACT", Val_ID = Socket_Models_Connect.Number_ID, Send_Area=Work_String_Name},
            new Socket_Models_List() { Val_Name = "$ACT_TOOL", Val_ID = Socket_Models_Connect.Number_ID,Send_Area=Work_String_Name },
            new Socket_Models_List() { Val_Name = "$ACT_BASE", Val_ID = Socket_Models_Connect.Number_ID,Send_Area=Work_String_Name },
        };













        /// <summary>
        /// 循环读取集合内的值方法
        /// </summary>
        /// <param name="_Obj"></param>
        public void Show_Reveice_Control(Socket_Models_List Name_Val)
        {


            switch (Name_Val.Val_Name)
            {
                case "$POS_ACT":
                    Working_Path.KUKA_Now_Point_Show = Name_Val.Val_Var;
                    break;
                case "$ACT_TOOL":
                    Working_Path.KUKA_TOOL_Number = Name_Val.Val_Var;
                    break;
                case "$ACT_BASE":
                    Working_Path.KUKA_Base_Number = Name_Val.Val_Var;
                    break;

            }





        }



        /// <summary>
        /// 数据更新字体显示红色
        /// </summary>
        /// <param name="mm">输入显示多少秒</param>
        public void UI_Show_Rad(int mm)
        {
            Working_Path.UI_Point_Color = true;
            Task.Delay(mm);
            Working_Path.UI_Point_Color = false;

        }

    }
}
