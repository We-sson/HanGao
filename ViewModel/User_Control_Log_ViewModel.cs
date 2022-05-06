using Microsoft.Toolkit.Mvvm.Messaging;
using Nancy.Helpers;
 
using PropertyChanged;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Errorinfo;
using HanGao.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Log_ViewModel : ObservableObject
    {
        ///// <summary>
        ///// 初始化输出信息
        ///// </summary>
        //public   User_Control_Log_ViewModel()
        //{

        //    User_UI_Log.Add(new User_Log_Models() {  User_Log= "软件启动" });

        //}

        private static User_Log_Models _User_UI_Log = new User_Log_Models();
        /// <summary>
        /// 显示状态信息输出
        /// </summary>
        public  static User_Log_Models User_UI_Log
        {
            get
            {
                return _User_UI_Log;
            }
            set
            {
                _User_UI_Log = value;
            }
        }

        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock = new Mutex();

        /// <summary>
        /// 全局使用输出方法
        /// </summary>
        public static void User_Log_Add(string Log)
        {



            lock (User_UI_Log)
            {

       

            User_UI_Log.User_Log += User_UI_Log.User_Log_Number+" | " + DateTime.Now.ToShortTimeString().ToString() + "——" + Log + HttpUtility.HtmlDecode("&#x000A;");

            }





            //LogManager.WriteProgramLog(Log);

            //显示前增加时间戳 





        }



        /// <summary>
        /// 记录添加日志前高度值
        /// </summary>
        private double ScrollViewer_Contrn { get; set; } = 0;


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
                Sm.PageDown();
                return;


            }

        }



    }
}
