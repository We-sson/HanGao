using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Show : ObservableRecipient
    {


        public User_Control_Show()
        {







            // 接收到消息创建对应字符的消息框
            Messenger.Register<UserControl,string >(this, nameof(Meg_Value_Eunm.User_Contorl_Message_Show), (O,_List)=> 
            {

                User_UserControl = _List;


         
            });

        }












        private static  UserControl _User_UserControl=null;
        /// <summary>
        /// 弹窗显示容器
        /// </summary>
        public static  UserControl User_UserControl
        {
            get
            {
                return _User_UserControl;
            }
            set
            {
                _User_UserControl = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(User_UserControl)));
            }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;





    }
}
