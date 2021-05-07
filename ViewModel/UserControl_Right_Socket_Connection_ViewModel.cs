using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.View.User_Control;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Right_Socket_Connection_ViewModel
    {



        public UserControl_Right_Socket_Connection_ViewModel()
        {

        }



        public ICommand Socket_Connection_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Connection);
        }
        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        private void Socket_Connection(UserControl_Right_Socket_Connection Sm)
        {
            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;

       Socket_Connect Soceket_KUKA_Client = new Socket_Connect();

            Soceket_KUKA_Client.Socket_Client_KUKA(Sm.TB1.Text, int.Parse(Sm.TB2.Text));





        }







    }
}
