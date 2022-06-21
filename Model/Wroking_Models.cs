
using PropertyChanged;
using System;
using System.ComponentModel;
using HanGao.Socket_KUKA;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Soceket_KUKA.Models;
using System.Threading;
using System.Windows;
using static HanGao.Model.Sink_Models;
using static HanGao.ViewModel.UC_Surround_Direction_VM;
using static HanGao.Xml_Date.Xml_WriteRead.User_Read_Xml_Model;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Wroking_Models : ObservableObject
    {
        public Wroking_Models()
        {
         
        }


        public Visibility UI_Show { set; get; } =Visibility.Visible;


        /// <summary>
        /// 工作区设置编号
        /// </summary>
        public Work_No_Enum Work_NO { set; get; }

       /// <summary>
       /// UI界面水槽尺寸显示
       /// </summary>
       public Sink_Models UI_Sink_Show { set; get; }

        /// <summary>
        /// 发送数据到机器人显示状态
        /// </summary>
        public UI_Type_Enum UI_Loade { set; get; } = UI_Type_Enum.Ok;


        /// <summary>
        /// 显示机器人速度
        /// </summary>
        public double Robot_Speed { set; get; } = 1;

        public double UI_Robot_Speed
        {
            set
            {
                UI_Robot_Speed = value;
            }
            get
            {
                var a = Robot_Speed/(double)(2.0/360);
                return a;
            }
        }
        /// <summary>
        /// 显示机器人焊接功率
        /// </summary>
        public double  Welding_Power { set; get; } = 50;


        public double UI_Welding_Power
        {
            set { 
                UI_Welding_Power = value; 
            }
            get
            {
                return Welding_Power/(100.0/360);
            }
        }
        /// <summary>
        /// 显示机器人焊接周期时间
        /// </summary>
        public double Welding_Time { set; get; } = 10;

        public double UI_Welding_Time
        {
            set
            {
                UI_Welding_Time = value;
            }
            get
            {
                return Welding_Time/(120.0/360);
            }
        }


        ////#E3E3E3背景白色,#22AB38运行绿色,#F4D160待机黄色
        //private string _Work_Type = "";
        ///// <summary>
        ///// 显示加工水槽型号
        ///// </summary>
        //public string Work_Type
        //{
        //    get
        //    {

        //        if (_Work_Type != "") { return "#P" + _Work_Type; }
        //        return " ";


        //    }
        //    set
        //    {
        //        _Work_Type = value;


        //        //加工区号非空时
        //        if (Work_NO != 0)
        //        {
        //            //将显示的水槽型号发送到kuka端，同时修改显示颜色
        //            if (Work_Type == " ")
        //            {


        //                //使用多线程读取
        //                new Thread(new ThreadStart(new Action(() =>
        //                {
        //                Socket_Client_Setup.Write.Cycle_Write_Send("$my_work_" + Work_NO, " ");
        //                })))
        //                { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();



        //                Work_back = "#E3E3E3";
        //            }
        //            else if (Work_Type != " ")
        //            {
        //                //使用多线程读取
        //                new Thread(new ThreadStart(new Action(() =>
        //                {
        //                 Socket_Client_Setup.Write.Cycle_Write_Send("$my_work_" + Work_NO, " ");
        //                })))
        //                { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

        //                Work_back = "#F4D160";

        //            }
        //        }
        //    }
        //}









        //private string _Robot_status = "#T1";
        ///// <summary>
        ///// 显示机器人当前状态
        ///// </summary>
        //public string Robot_status
        //{
        //    get
        //    {
        //        return _Robot_status.IndexOf('#') != -1 ? _Robot_status.Replace("#", "") : _Robot_status;
        //    }
        //    set
        //    {
        //        if (value.IndexOf('#') == -1)
        //        {
        //            _Robot_status = value.Replace("#", "");
        //        }
        //        else
        //        {
        //            _Robot_status = value;
        //        }



        //    }
        //}
        /// <summary>
        /////  显示机器人当前状态枚举
        ///// </summary>
        //public enum Robot_status_enum
        //{
        //    空闲中,
        //    运行中,
        //    焊接中,
        //}


        ////#E3E3E3背景白色,#22AB38运行绿色,#F4D160待机黄色

        //private string _Work_back = "#E3E3E3";
        ///// <summary>
        ///// 加工区背景颜色
        ///// </summary>
        //public string Work_back
        //{
        //    get
        //    {
        //        if (Work_Type !=" ")
        //        {
        //            if (Work_Run)
        //            {
        //                _Work_back = "#22AB38";
        //            }
        //            else
        //            {
        //                _Work_back = "#F4D160";
        //            }

        //        }
        //        else
        //        {
        //            _Work_back = "#E3E3E3";

        //        }


        //        return _Work_back;
        //    }




        //    set
        //    {
        //        _Work_back = value;

        //    }
        //}

        //private bool _Work_Run = false;
        ///// <summary>
        ///// 显示是否启动加工区域
        ///// </summary>
        //public bool Work_Run
        //{
        //    get
        //    {


        //        return _Work_Run;
        //    }
        //    set
        //    {

        //        //值相同返回，减少UI更新占用资源
        //        if (_Work_Run == value) { return; }

        //        _Work_Run = value;
        //        //属性发送更改发送机器端

        //        //使用多线程读取
        //        new Thread(new ThreadStart(new Action(() =>
        //        {
        //        Socket_Client_Setup.Write.Cycle_Write_Send("$Run_Work_" + Work_NO.ToString(), value.ToString());
        //        })))
        //        { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

        //    }
        //}








    };
}







