using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using 悍高软件.Errorinfo;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;
using 悍高软件.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;
using static Soceket_KUKA.Socket_Receive;
using static 悍高软件.ViewModel.User_Control_Log_ViewModel;
using static 悍高软件.ViewModel.UserControl_Socket_Setup_ViewModel;

namespace 悍高软件.Model

{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Setup_Models : ViewModelBase
    {

        public Socket_Setup_Models()
        {

        }

        public IP_Text_Error Text_Error { set; get; } = new IP_Text_Error() { };

        public string Control_Name_String { set; get; }





        /// <summary>
        /// 连接按钮连接后禁止重复连接
        /// </summary>
        public bool Connect_Button_IsEnabled { set; get; } = true;

        /// <summary>
        /// 设备连接中状态...
        /// </summary>
        public bool Connect_Socket_Connection { set; get; } = false;

        /// <summary>
        /// 设备成功状态...
        /// </summary>
        public bool Connect_Socket_OK { set; get; } = false;

        public void Client_Button_Show(int _int)
        {
            switch (_int)
            {
                case -1:
                    Connect_Socket_Connection = false;
                    Connect_Socket_OK = false;
                    break;
                case 0:
                    Connect_Socket_Connection = true;
                    break;
                case 1:
                    Connect_Socket_OK = true;
                    break;
                default:
                    User_Log_Add($"-1网络状态显示，传入错误值");
                    break;
            }
        }




        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        public ICommand Socket_Connection_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>((Sm) =>
            {

                //把参数类型转换控件
                UserControl_Socket_Conntec_UI e = Sm.Source as UserControl_Socket_Conntec_UI;


                Socket_Connect _Client = new Socket_Connect() ;


                //创建连接
                _Client.Socket_Client_KUKA(e.TB1.Text, int.Parse(e.TB2.Text));



            });
        }

    }
}
