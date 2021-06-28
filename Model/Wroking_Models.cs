using PropertyChanged;

namespace 悍高软件.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Wroking_Models : User_Features
    {





        private string _Number_Work = "";
        /// <summary>
        /// 显示加工区域号
        /// </summary>
        public string Number_Work
        {
            get
            {
                return "NO." + _Number_Work + ":";
            }
            set
            {
                _Number_Work = value;
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
                return _Work_Type;
            }
            set
            {
                _Work_Type = value;
                if (Work_Type == "")
                {
                    Work_back = "#E3E3E3";
                }
                else if (Work_Type != "")
                {
                    Work_back = "#F4D160";

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
                return _Robot_Speed + "m/分钟";
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




        private Robot_status_enum _Robot_status = (Robot_status_enum)0;
        /// <summary>
        /// 显示机器人当前状态
        /// </summary>
        public Robot_status_enum Robot_status
        {
            get
            {
                return _Robot_status;
            }
            set
            {
                _Robot_status = value;
            }
        }
        /// <summary>
        ///  显示机器人当前状态枚举
        /// </summary>
        public enum Robot_status_enum
        {
            空闲中,
            待机中,
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
                return _Work_back;
            }


            //return new SolidColorBrush((Color)ColorConverter.ConvertFromString(_Work_back));

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
                _Work_Run = value;
                if (_Work_Type != "" && Work_Run == true)
                {
                    Work_back = "#22AB38";
                }
                else if (_Work_Type != "" && Work_Run == false)
                {
                    Work_back = "#F4D160";
                }

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
                if (Work_Type == "")
                {
                    return _Work_IsEnabled = false;

                }
                else
                {
                    return _Work_IsEnabled = true;
                }

            }
            set
            {
                _Work_IsEnabled = value;
            }
        }
    }






}
