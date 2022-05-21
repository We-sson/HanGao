
using PropertyChanged;
using System;
using System.ComponentModel;
using HanGao.Socket_KUKA;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Soceket_KUKA.Models;
using System.Threading;


namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Wroking_Models : ObservableObject
    {
        public Wroking_Models()
        {
         
        }

        

        /// <summary>
        /// 工作区设置编号
        /// </summary>
        public int Work_NO { set; get; }

       
        /// <summary>
        /// 显示加工区域号
        /// </summary>
        public string Number_Work
        {
            get
            {
                return  Work_NO.ToString() + "号 工作区";
            }
    
        }

        //#E3E3E3背景白色,#22AB38运行绿色,#F4D160待机黄色
        private string _Work_Type = "";
        /// <summary>
        /// 显示加工水槽型号
        /// </summary>
        public string Work_Type
        {
            get
            {

                if (_Work_Type != "") { return "#P" + _Work_Type; }
                return " ";


            }
            set
            {
                _Work_Type = value;


                //加工区号非空时
                if (Work_NO != 0)
                {
                    //将显示的水槽型号发送到kuka端，同时修改显示颜色
                    if (Work_Type == " ")
                    {


                        //使用多线程读取
                        new Thread(new ThreadStart(new Action(() =>
                        {
                        Socket_Client_Setup.Write.Cycle_Write_Send("$my_work_" + Work_NO, " ");
                        })))
                        { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

                        

                        Work_back = "#E3E3E3";
                    }
                    else if (Work_Type != " ")
                    {
                        //使用多线程读取
                        new Thread(new ThreadStart(new Action(() =>
                        {
                         Socket_Client_Setup.Write.Cycle_Write_Send("$my_work_" + Work_NO, " ");
                        })))
                        { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

                        Work_back = "#F4D160";

                    }
                }
            }
        }






        private string _Robot_Speed = "0";
        /// <summary>
        /// 显示机器人速度
        /// </summary>
        public string Robot_Speed
        {
            get
            {
                return Convert.ToDouble(_Robot_Speed).ToString("F3");

            }
            set
            {
                //值相同返回，减少UI更新占用资源
                if (value!=String .Empty)
                {
                _Robot_Speed = value;
                }
            }
        }

        private string _Welding_Power = "0";
        /// <summary>
        /// 显示机器人焊接功率
        /// </summary>
        public string Welding_Power
        {
            get
            {
                return _Welding_Power + "%";
            }
            set
            {

                _Welding_Power = value;
            }
        }




        private string _Robot_status = "#T1";
        /// <summary>
        /// 显示机器人当前状态
        /// </summary>
        public string Robot_status
        {
            get
            {
                return _Robot_status.IndexOf('#') != -1 ? _Robot_status.Replace("#", "") : _Robot_status;
            }
            set
            {
                if (value.IndexOf('#') == -1)
                {
                    _Robot_status = value.Replace("#", "");
                }
                else
                {
                    _Robot_status = value;
                }



            }
        }
        /// <summary>
        ///  显示机器人当前状态枚举
        /// </summary>
        public enum Robot_status_enum
        {
            空闲中,
            运行中,
            焊接中,
        }


        //#E3E3E3背景白色,#22AB38运行绿色,#F4D160待机黄色

        private string _Work_back = "#E3E3E3";
        /// <summary>
        /// 加工区背景颜色
        /// </summary>
        public string Work_back
        {
            get
            {
                if (Work_Type !=" ")
                {
                    if (Work_Run)
                    {
                        _Work_back = "#22AB38";
                    }
                    else
                    {
                        _Work_back = "#F4D160";
                    }

                }
                else
                {
                    _Work_back = "#E3E3E3";

                }


                return _Work_back;
            }




            set
            {
                _Work_back = value;

            }
        }

        private bool _Work_Run = false;
        /// <summary>
        /// 显示是否启动加工区域
        /// </summary>
        public bool Work_Run
        {
            get
            {


                return _Work_Run;
            }
            set
            {

                //值相同返回，减少UI更新占用资源
                if (_Work_Run == value) { return; }

                _Work_Run = value;
                //属性发送更改发送机器端

                //使用多线程读取
                new Thread(new ThreadStart(new Action(() =>
                {
                Socket_Client_Setup.Write.Cycle_Write_Send("$Run_Work_" + Work_NO.ToString(), value.ToString());
                })))
                { IsBackground = true, Name = "Cycle_Write—KUKA" }.Start();

            }
        }




        private bool _Work_IsEnabled = true;


        /// <summary>
        /// 显示是否加工功能
        /// </summary>
        public bool Work_IsEnabled
        {
            get
            {
                if (_Work_Type == " " || _Work_Type == "") { return _Work_IsEnabled = false; } else { return _Work_IsEnabled = true; }
                //return _Work_IsEnabled;

            }
            set
            {
                _Work_IsEnabled = value;
            }
        }




    };
}







