using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Model;
using 悍高软件.View.User_Control;

namespace 悍高软件.ViewModel
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

     




        /// <summary>
        /// 启动触发事件命令
        /// </summary>
        public ICommand Sideber_Show
        {
            get => new DelegateCommand<RoutedEventArgs>((Sm) =>
            {
                //把参数类型转换控件
                Button e = Sm.Source as Button;
                //
                dynamic S = e.DataContext;


                //var a = this.GetType().GetProperty("WM").GetValue(this);





            });
        }




    }
}
