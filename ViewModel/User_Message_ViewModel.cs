

using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.Model;
using HanGao.View.UserMessage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Message_ViewModel : ObservableRecipient
    {

        /// <summary>
        /// 弹窗显示加工区域已存在型号
        /// </summary>
        public string User_Wrok_Trye { set; get; }



        /// <summary>
        /// 接收列表传来的控件
        /// </summary>
        //public static CheckBox List_Check_Control { set; get; }




        public User_Message_ViewModel()
        {


            // 显示弹窗信息型号
            //WeakReferenceMessenger.Default.Register<string>(this, "User_Message_Work_Type", (Type) => { User_Message_View.User_Wrok_Trye = Type; });



        }







        //参数传递记录
        public List_Show_Models List_Show_Models { set; get; } = new List_Show_Models();






        /// <summary>
        /// 弹窗加载事件命令
        /// </summary>
        public ICommand User_Window_Comm
        {
            get => new RelayCommand<RoutedEventArgs>(User_Window_Co);
        }
        private void User_Window_Co(RoutedEventArgs S)
        {
            //User_Message e = S.Source as User_Message;
            //User_Window = e;
            //MessageBox.Show(User_Window.DialogResult.ToString());
            //User_Message_View.User_Wrok_Trye = "1";



        }





        /// <summary>
        /// 弹出用户确定取消选择
        /// </summary>
        public ICommand Yes_No_Comm
        {
            get => new RelayCommand<RoutedEventArgs>(User_Yes_No);
        }
        /// <summary>
        /// 弹出用户确定取消选择
        /// </summary>
        private void User_Yes_No(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            Button e = Sm.Source as Button;
            //User_Message_ViewModel S = (User_Message_ViewModel)e.DataContext;




            //委托方法放回用户选择值
            List_Show_Models.GetUser_Select(bool.Parse(e.Uid));

            //清空弹窗显示
             Messenger.Send<UserControl,string >(null,nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


            //记录用户选择的是或否
            //List_Show_Models.User_Check = e.Uid.ToString();

            //Messenger.Send<List_Show_Models,string >(List_Show_Models, nameof(Meg_Value_Eunm.List_IsCheck_Show) );




        }


 





















    }
}
