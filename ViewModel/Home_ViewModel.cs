using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HanGao.Model;
using HanGao.View.FrameShow;
using HanGao.View.User_Control;



namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Home_ViewModel : ViewModelBase
    {


        public Home_ViewModel()
        {


            //注册消息接收
            //Messenger.Default.Register<double>("Open_Effect", Home_Var.Open_Effect);
            Messenger.Default.Register<Visibility>("Home_Visibility_Show", Home_Var.Home_Visibility_Show);

            //Home_Var.Sidebar_Control = new UserControl_Right_Function_Connect() {  };


        }


        public Home_Models Home_Var { get; set; } = new Home_Models();



        public static  List<Sideber_Models> Sideber_List { set; get; } = new List<Sideber_Models>()
        { new Sideber_Models()  { Sidebar_Control = new User_Control_Socket_Setup() { }, Sidebar_MainTitle = "连接设置", Sideber_Open = false  },
           new Sideber_Models() { Sidebar_Control = new UserControl_Socket_Write() { }, Sidebar_MainTitle = "写入功能", Sideber_Open = false  },
           new Sideber_Models() { Sidebar_Control = new UserControl_Value_Show() { }, Sidebar_MainTitle = "读取显示", Sideber_Open = false  } };






        /// <summary>
        /// 启动触发事件命令
        /// </summary>
        public ICommand Sideber_Show
        {
            get => new RelayCommand<RoutedEventArgs>(async (Sm) =>
            {

                await Task.Run(() =>
                {
                    //委托
                    Application.Current.Dispatcher.Invoke(  ()=> 
                    {




                        ////把参数类型转换控件
                        Button e = Sm.Source as Button;

                        Home_ViewModel S = (Home_ViewModel)e.DataContext;

                        StackPanel q = (StackPanel)e.Content;

                        Sideber_Models _Sidber = null;


                        var l = VisualTreeHelper.GetChildrenCount(q);


                        for (int i = 0; i < l; i++)
                        {
                            //查找控件
                            TextBlock r = (TextBlock)VisualTreeHelper.GetChild(q, i);
                            foreach (var item in Sideber_List)
                            {
                                if (r.Text == item.Sidebar_Subtitle)
                                {
                                    item.Sideber_Open = true;
                                    _Sidber = item;

                                }

                            }

                        }

                        Messenger.Default.Send<Sideber_Models>(_Sidber, "Sideber_Show");




                    });

                });

            });






        }




    }
}
