
using static HanGao.Model.Socket_Setup_Models;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static Soceket_Connect.Socket_Connect;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ObservableRecipient
    {



        public UserControl_Socket_Setup_ViewModel()
        {




            //得到变量值后发送到其他所需区域
            Socket_Client_Setup.Read.Socket_Receive_Delegate= Socket_Client_Setup.One_Read.Socket_Receive_Delegate += (Socket_Models_Receive _Receive) =>
            {
                Messenger.Send<Socket_Models_List, string>(_Receive.Reveice_Inf, _Receive.Reveice_Inf.Send_Area);

            };

            //
            Socket_Client_Setup.Read.Socket_Connect_State_delegate = (bool _Connect_State) =>
            {
                if (_Connect_State)
                {
                    Messenger.Send<string, string>(Socket_Tpye.Connect_OK.ToString(), Meg_Value_Eunm.Socket_Read_Tpye.ToString());
                }
                else
                {
                    Messenger.Send<string, string>(Socket_Tpye.Connect_Cancel.ToString(), Meg_Value_Eunm.Socket_Read_Tpye.ToString());
                }

            };




            //连接控制柜，网络连接状态显示方法
            Messenger.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Connect_Client_Socketing_Button_Show) , (O,_int) =>
            {
                Socket_Client_Setup.Client_Button_Show(_int);
            });








            //客户端连接数量
            Messenger.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.ClientCount) , (O,_int )=> { ClientCount = _int; });


            //显示
            Messenger.Register<dynamic ,string >(this, nameof(Meg_Value_Eunm.Socket_Countion_Show) , (O,_Visibility )=> { Socket_Countion_Show = _Visibility; });



        }



        /// <summary>
        /// 客户端IP
        /// </summary>
        private static  string _IP_Client = "192.168.153.150";

        public static string IP_Client
        {
            get { return _IP_Client; }
            set { _IP_Client = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(IP_Client))); }
        }



        /// <summary>
        /// 客户端端口
        /// </summary>
        private static string _Port_Client = "7000";

        public static string Port_Client
        {
            get { return _Port_Client; }
            set { _Port_Client = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Port_Client))); }
        }



        /// <summary>
        /// 服务器IP
        /// </summary>
        private static string _IP_Sever = "192.168.153.1";

        public static string IP_Sever
        {
            get { return _IP_Sever; }
            set { _IP_Sever = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(IP_Sever))); }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        private static string _Port_Sever = "5000";

        public static string Port_Sever
        {
            get { return _Port_Sever; }
            set { _Port_Sever = value; StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Port_Sever))); }
        }
   



        private static Socket_Setup_Models _Socket_Client_Setup = new ()
        {
            IP = IP_Client,
            Port = Port_Client,
            One_Read =  new Socket_Connect(),
            Read = new Socket_Connect(),
            Write = new Socket_Connect(),
            Connect_Socket_Type = Socket_Type.Client,
            Control_Name_String = "连接控制柜",
        };
        /// <summary>
        /// 客户端静态属性
        /// </summary>
        //public static Socket_Setup_Models Socket_Client_Setup
        //{
        //    get => _Socket_Client_Setup;
        //    set
        //    {

        //        _Socket_Client_Setup = value;
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Client_Setup)));

        //    }
        //}


        private static Socket_Setup_Models _Socket_Server_Setup = new () 
        {
            IP = IP_Sever,
            Port = Port_Sever,
            Connect_Socket_Type = Socket_Type.Server,
            Sever = new Socket_Sever(IP_Sever, Port_Sever),
            Control_Name_String = "监听控制柜",
        };
        /// <summary>
        /// 服务器静态属性
        /// </summary>
        //public static Socket_Setup_Models Socket_Server_Setup
        //{
        //    get => _Socket_Server_Setup;
        //    set
        //    {

        //        _Socket_Server_Setup = value;
        //        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Socket_Server_Setup)));

        //    }
        //}

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
         public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;





        /// <summary>
        /// 客户端连接数量
        /// </summary>
        public int ClientCount { set; get; } = 0;


        /// <summary>
        /// 连接按钮显示
        /// </summary>
        public Visibility Socket_Countion_Show { set; get; } = Visibility.Hidden;


        /// <summary>
        /// Socket连接UI显示枚举
        /// </summary>
        public enum Socket_UI_State_Enum
        {
            Connecting,
            Connection_Failed,
            Connect_OK
        }

    }
}
