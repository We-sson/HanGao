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
        public Pop_Message_Models Pop_Message { set; get; } = new Pop_Message_Models();






        /// <summary>
        /// 弹窗加载事件命令
        /// </summary>
        //public ICommand User_Window_Comm
        //{
        //    get => new RelayCommand<RoutedEventArgs>(User_Window_Co);
        //}
        //private void User_Window_Co(RoutedEventArgs S)
        //{
        //    //User_Message e = S.Source as User_Message;
        //    //User_Window = e;
        //    //MessageBox.Show(User_Window.DialogResult.ToString());
        //    //User_Message_View.User_Wrok_Trye = "1";



        //}





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

            //委托方法放回用户选择值
            if (Pop_Message.GetUser_Select!=null)
            {

            Pop_Message.GetUser_Select(bool.Parse(e.Uid));
            }



            //清空弹窗显示
             Messenger.Send<UserControl,string >(new UserControl(), nameof(Meg_Value_Eunm.User_Contorl_Message_Show));


        


        }


 





















    }
}
