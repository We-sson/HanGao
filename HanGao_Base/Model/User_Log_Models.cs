

using HanGao.ViewModel;

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class User_Log_Models
    {
        public User_Log_Models()
        {



        }



        /// <summary>
        /// 记录添加日志前高度值
        /// </summary>
        private double ScrollViewer_Contrn { get; set; } = 0;


        public Log_Show_Window_Enum Log_Show_Window { set; get; }

        /// <summary>
        /// 添加消息时触发事件
        /// </summary>
        public ICommand Update_Log_Comm
        {
            get => new RelayCommand<ScrollViewer>(Update_Log);
        }
        /// <summary>
        /// 添加消息时触发事件方法
        /// </summary>
        private void Update_Log(ScrollViewer Sm)
        {


            //如果原来的高度就不翻页
            if (Sm.ExtentHeight != ScrollViewer_Contrn)
            {
                ScrollViewer_Contrn = Sm.ExtentHeight;
                Sm.ScrollToEnd();
                return;


            }

        }

        private int _User_Log_Number = 0;
        public int User_Log_Number
        {
            set
            {
                _User_Log_Number = value;
            }
            get
            {
                return _User_Log_Number++;
            }
        }



        private string _User_Log="系统初始化完成！"+ HttpUtility.HtmlDecode("&#x000A;");
        /// <summary>
        /// 显示状态信息添加时间戳
        /// </summary>
        public string User_Log
        {
            get
            {
                if (_User_Log_Number > 500)
                {
                    _User_Log = string.Empty;
                    User_Log_Number = 0;
                }
                return _User_Log;

            }
            set
            {



                //_User_Log = value;

                _User_Log += User_Log_Number.ToString("D3") + " | " + DateTime.Now.ToShortTimeString().ToString() + "——" + value + HttpUtility.HtmlDecode("&#x000A;");

            }
        }

        private string _User_Log_Cont = null;
        /// <summary>
        /// 显示状态信息输出
        /// </summary>
        public string User_Log_Cont
        {
            get
            {
              
                return _User_Log_Cont;
            }
            set
            {

                _User_Log = value;
            }
        }

    }
    /// <summary>
    /// 日志调试显示窗口
    /// </summary>
    public enum Log_Show_Window_Enum
    {
        Home,
        Calibration,
        HandEye

    }
}
