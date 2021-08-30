using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;
using 悍高软件.ViewModel;

namespace 悍高软件.View.User_Control.User_Control_ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class Socket_Client_Setup: UserControl_Socket_Setup_ViewModel
    {
        public Socket_Client_Setup()
        {

        }



    }
}
