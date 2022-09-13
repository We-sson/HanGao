using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.View.FrameShow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Reflection;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static Soceket_Connect.Socket_Connect;
using HalconDotNet;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Visal_Function_VM : ObservableRecipient
    {


        public UC_Visal_Function_VM()
        {



            //halcon控件操作
            Messenger.Register<UserControl, string>(this, nameof(Meg_Value_Eunm.User_Contorl_Message_Show), (O, _List) =>
            {






            });


        }
        public HSmartWindowControlWPF Live_Window { set; get; } = new HSmartWindowControlWPF() { };

        // 接收到消息创建对应字符的消息框

        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand Live_Window_Show_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Vision e = Sm.Source as Vision;


              






            });
        }


        /// <summary>
        /// 加载属性水槽类型
        /// </summary>
        public ICommand User_Comm
        {
            get => new RelayCommand<Vision>((Sm) =>
            {
                FrameworkElement e = Sm as FrameworkElement;






            });
        }

    }
}
