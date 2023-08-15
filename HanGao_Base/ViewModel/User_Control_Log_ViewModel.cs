


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


            //接收其他地方传送数据
            StrongReferenceMessenger.Default.Register<string , string>(this, nameof(Meg_Value_Eunm.UI_Log_Home), (O, _S) =>
            {


                UI_Home_Log.User_Log = _S ;



            });


            //接收其他地方传送数据
            StrongReferenceMessenger.Default.Register<string, string>(this, nameof(Meg_Value_Eunm.UI_Log_Calibration), (O, _S) =>
            {

                UI_Calibration_Log.User_Log = _S;




            });


        }



        public User_Log_Models UI_Home_Log { set; get; } = new User_Log_Models() { Log_Show_Window= Model.Log_Show_Window_Enum.Home };
        public User_Log_Models UI_Calibration_Log { set; get; } = new User_Log_Models() { Log_Show_Window= Model.Log_Show_Window_Enum.Calibration };


        /// <summary>
        /// 全局使用输出方法
        /// </summary>
        public static   void User_Log_Add(string Log, Log_Show_Window_Enum _ShowLog)
        {

                Task.Run(() => {

                    switch (_ShowLog)
                    {
                        case Log_Show_Window_Enum.Home:

                            StrongReferenceMessenger.Default.Send<string, string>(Log, nameof(Meg_Value_Eunm.UI_Log_Home));
                 

                            break;
                        case Log_Show_Window_Enum.Calibration:
                            StrongReferenceMessenger.Default.Send<string, string>(Log, nameof(Meg_Value_Eunm.UI_Log_Calibration));


                            break;
                    }




                });



        }


        /// <summary>
        /// 获得算法状态显示UI
        /// </summary>
        /// <param name="_Result_Status"></param>
        /// <returns></returns>
        public static HPR_Status_Model Display_Status(HPR_Status_Model _Result_Status) 
        {


       
                User_Log_Add(_Result_Status.GetResult_Info(), Log_Show_Window_Enum.Home);



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






    }



}
