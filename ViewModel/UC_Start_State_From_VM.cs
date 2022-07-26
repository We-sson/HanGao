using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static HanGao.ViewModel.UC_KUKA_State_VM;
using static Soceket_Connect.Socket_Connect;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class UC_Start_State_From_VM : ObservableRecipient
    {

        public UC_Start_State_From_VM()
        {


        }


        public UC_Start_State_From_Model UI_Data { set; get; } = new UC_Start_State_From_Model();



    }


    public class UC_Start_State_From_Model
    {
        /// <summary>
        /// 主控件显示属性
        /// </summary>
        public Visibility UI_Show { set; get; } = Visibility.Visible;

        /// <summary>
        /// UI网络连接状态
        /// </summary>
        public Socket_Tpye UI_Socket_State { set; get; } = Socket_Tpye.Connect_Cancel;

        /// <summary>
        /// UI机器人运作状态
        /// </summary>
        public bool UI_Robot_State { set; get; } = true   ;


        /// <summary>
        /// UI人员操作模式
        /// </summary>
        public KUKA_State_Enum UI_Mode_State { set; get; } = KUKA_State_Enum.Null;
    }
}
