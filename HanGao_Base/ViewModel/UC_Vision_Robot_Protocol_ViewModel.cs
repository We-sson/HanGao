

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
        public class UC_Vision_Robot_Protocol_ViewModel : ObservableRecipient
    {

        public UC_Vision_Robot_Protocol_ViewModel()
        {

        }


        private static  string  _Send_Socket_String="....";

        public static  string Send_Socket_String
        {
            get { return _Send_Socket_String; }
            set { _Send_Socket_String = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Send_Socket_String)));
            }
        }



        private static string  _Receive_Socket_String="...";

        public static string Receive_Socket_String
        {
            get { return _Receive_Socket_String; }
            set { _Receive_Socket_String = value;



                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Receive_Socket_String)));
            }
        }



        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;




        /// <summary>
        /// 上位机使用通讯协助
        /// </summary>
        public int Communication_Protocol { set; get; } = 0;

        /// <summary>
        /// 报文显示格式
        /// </summary>
        public int Content_Coding { set; get; } = 0;
    }
}
