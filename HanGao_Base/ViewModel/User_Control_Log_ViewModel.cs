


using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualBasic.Logging;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static MVS_SDK_Base.Model.MVS_Model;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Log_ViewModel : ObservableRecipient
    {



       public User_Control_Log_ViewModel()
        {




            //报错信息传递
            //Socket_Client_Setup.Read.Socket_ErrorInfo_delegate += User_Log_Add;







            //接收其他地方传送数据
            Messenger.Register<string , string>(this, nameof(Meg_Value_Eunm.UI_Log_Show_1), (O, _S) =>
            {


                User_UI_Log.User_Log += User_UI_Log.User_Log_Number.ToString("D3") + " | " + DateTime.Now.ToShortTimeString().ToString() + "——" + _S + HttpUtility.HtmlDecode("&#x000A;");



            });




        }






        public   User_Log_Models User_UI_Log = new User_Log_Models();
        /// <summary>
        /// 显示状态信息输出
        /// </summary>
        //public   User_Log_Models User_UI_Log
        //{
        //    get
        //    {
        //        return _User_UI_Log;
        //    }
        //    set
        //    {
        //        _User_UI_Log = value;
        //    }
        //}

        /// <summary>
        /// 资源互锁
        /// </summary>
        public static Mutex Receive_Lock = new Mutex();

        /// <summary>
        /// 全局使用输出方法
        /// </summary>
        public static   void User_Log_Add(string Log)
        {

            

            //lock (User_UI_Log)
            //{
                Task.Run(() => {
       


                });
            //}





            //LogManager.WriteProgramLog(Log);

            //显示前增加时间戳 





        }


        /// <summary>
        /// 获得算法状态显示UI
        /// </summary>
        /// <param name="_Result_Status"></param>
        /// <returns></returns>
        public static HPR_Status_Model Display_Status(HPR_Status_Model _Result_Status) 
        {


       
                User_Log_Add(_Result_Status.GetResult_Info());



            return _Result_Status;
        }


        /// <summary>
        /// 获得海康算法状态显示UI
        /// </summary>
        /// <param name="_Result_Status"></param>
        /// <returns></returns>
        //public static MPR_Status_Model Display_Status(MPR_Status_Model _Result_Status)
        //{

        //        User_Log_Add(_Result_Status.GetResult_Info());


        //    return _Result_Status;
        //}


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
                Sm.ScrollToEnd();
                return;


            }

        }



    }
}
