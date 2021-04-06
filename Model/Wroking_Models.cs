using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public  class Wroking_Models
    {


    private string _Number_Work=null;
        /// <summary>
        /// 显示加工区域号
        /// </summary>
        public string Number_Work
    {
        get
        {
            return "NO."+_Number_Work+":";
        }
        set
        {
                _Number_Work = value;
        }
    }


        private string _Work_Type=null;
        /// <summary>
        /// 显示加工水槽型号
        /// </summary>
        public string Work_Type
        {
            get
            {
                return _Work_Type;
            }
            set
            {
                _Work_Type = value;
            }
        }

        private bool  _Work_Run=false;
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
                _Work_Run = value;
            }
        }

        private bool _Work_Connt =false;
        /// <summary>
        /// 显示是否启动加工区域计数
        /// </summary>
        public bool Work_Connt
        {
            get
            {
                return _Work_Connt;
            }
            set
            {
                _Work_Connt = value;
            }
        }


        private bool _Work_NullRun = false;
        /// <summary>
        /// 显示是否空运行加工区域
        /// </summary>
        public bool Work_NullRun
        {
            get
            {
                return _Work_NullRun;
            }
            set
            {
                _Work_NullRun = value;
            }
        }

        private bool _Work_Pause = false;
        /// <summary>
        /// 显示是否空暂停
        /// </summary>
        public bool Work_Pause
        {
            get
            {
                return _Work_Pause;
            }
            set
            {
                _Work_Pause = value;
            }
        }

        private bool _Work_JumpOver = false;
        /// <summary>
        /// 显示是否跳过加工
        /// </summary>
        public bool Work_JumpOver
        {
            get
            {
                return _Work_JumpOver;
            }
            set
            {
                _Work_JumpOver = value;
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
                return _Robot_Speed+"m/分钟";
            }
            set
            {
                _Robot_Speed = value;
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



        private int  _Robot_status= 0;
        /// <summary>
        /// 显示机器人焊接功率
        /// </summary>
        public string Robot_status
        {
            get
            {
                if (_Robot_status==0)
                {
                return "空闲中...";
                };
                return null;
            }
            set
            {
                //_Robot_status = value();
            }
        }

        public enum Robot_status_enum
        {
            空闲中,
            待机中,
            焊接中,
        }



    }






}
