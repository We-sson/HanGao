using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA.Models;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 悍高软件.Socket_KUKA;
using 悍高软件.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;
using static 悍高软件.ViewModel.UserControl_Socket_Setup_ViewModel;
using static 悍高软件.ViewModel.UserControl_Socket_Var_Show_ViewModel;

namespace Soceket_KUKA
{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Receive
    {


        /// <summary>
        /// 接收异常处理程序
        /// </summary>
        public static void Socket_Receive_Error(string _Error)
        {



            ////连接失败后允许用户再次点击连接按钮
            //Messenger.Default.Send<bool>(true, "Connect_Client_Button_IsEnabled");
            //Messenger.Default.Send<int>(-1, "Connect_Client_Socketing_Button_Show");
            Socket_Close();
            User_Log_Add(_Error);

        }










    }
}
