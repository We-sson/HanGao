using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Input;

namespace Robot_Info_Mes.Model
{
    /// <summary>
    /// 维护界面日志缓冲区、递增序号和 ScrollViewer 自动跟随行为。
    /// </summary>
    /// <remarks>
    /// PropertyChanged.Fody 会为可写属性织入变更通知，使追加日志后 XAML 能立即刷新。
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public class User_Log_Models
    {
        /// <summary>
        /// 创建带默认“系统初始化完成”消息的日志模型。
        /// </summary>
        public User_Log_Models()
        {



        }



        /// <summary>
        /// 记录上一次内容高度，只在高度确实变化时滚动，避免重复布局和无意义跳转。
        /// </summary>
        private double ScrollViewer_Contrn { get; set; } = 0;


        //public Log_Show_Window_Enum Log_Show_Window { set; get; }

        /// <summary>
        /// 由日志 ScrollViewer 的布局更新事件调用，尝试把视口跟随到最新一条记录。
        /// </summary>
        public ICommand Update_Log_Comm
        {
            get => new RelayCommand<ScrollViewer>(Update_Log);
        }
        /// <summary>
        /// 比较内容高度；新增日志使高度变化时才滚动到底部。
        /// </summary>
        private void Update_Log(ScrollViewer? Sm)
        {


            // 高度未变化代表没有新增可视内容，保留用户当前阅读位置。
            if (Sm?.ExtentHeight != ScrollViewer_Contrn)
            {
                ScrollViewer_Contrn = Sm!.ExtentHeight;
                Sm.ScrollToEnd();
                return;


            }

        }

        /// <summary>
        /// 当前日志序号。getter 在取出旧值后自增，供追加日志时生成连续编号。
        /// </summary>
        private int _User_Log_Number = 0;

        /// <summary>
        /// 获取下一条日志编号；赋值主要用于达到容量上限后归零。
        /// </summary>
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



        // 使用解码后的换行符与 XAML 文本控件兼容，避免依赖平台行结束符。
        private string _User_Log = "系统初始化完成！" + HttpUtility.HtmlDecode("&#x000A;");
        /// <summary>
        /// 日志显示缓冲区；写入一条消息时自动补充三位序号、当前时间和换行。
        /// </summary>
        public string User_Log
        {
            get
            {
                // 限制长期运行时的界面文本规模；超过 500 条后从新一轮日志开始。
                if (_User_Log_Number > 500)
                {
                    _User_Log = string.Empty;
                    User_Log_Number = 0;
                }
                return _User_Log;

            }
            set
            {



                // User_Log_Number 的 getter 同时推进序号，因此每次赋值只生成一个编号。
                _User_Log += User_Log_Number.ToString("D3") + " | " + DateTime.Now.ToLongTimeString().ToString() + "——" + value + HttpUtility.HtmlDecode("&#x000A;");

            }
        }

        private string _User_Log_Cont =string.Empty;
        /// <summary>
        /// 兼容旧界面绑定保留的日志输出属性。
        /// </summary>
        /// <remarks>当前 getter 返回独立缓存，而 setter 会替换主日志缓冲区。</remarks>
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
}
