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
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Right_Socket_Connection_ViewModel : ViewModelBase
    {



        public UserControl_Right_Socket_Connection_ViewModel()
        {


            //Socket_Read_List.Add(new Socket_Models_List() { Val_Name = "$VEL_ACT", Val_ID = Socket_Models_Connect.Number_ID });



            //注册消息接收


            Messenger.Default.Register<string>(this, "Socket_Message_Show", Socket_Message_Show);

            Messenger.Default.Register<bool>(this, "Connect_Button_IsEnabled_Method", Connect_Button_IsEnabled_Method);
            Messenger.Default.Register<int>(this, "Connect_Socketing_Method", Connect_Socketing_Method);

            Messenger.Default.Register<ObservableCollection<Socket_Models_List>>(this, "List_Connect", List_Connect);





        }



        private bool _Connect_Button_IsEnabled = true;
        /// <summary>
        /// 连接按钮连接后禁止重复连接
        /// </summary>
        public bool Connect_Button_IsEnabled
        {
            get
            {
                return _Connect_Button_IsEnabled;
            }
            set
            {
                _Connect_Button_IsEnabled = value;
            }
        }

        /// <summary>
        /// 设备连接中状态...
        /// </summary>
        public bool Connect_Socket_Connection { set; get; } = false;

        /// <summary>
        /// 设备成功状态...
        /// </summary>
        public bool Connect_Socket_OK { set; get; } = false;


        private string _Socket_Message = "准备接收....";
        /// <summary>
        /// 接收消息属性
        /// </summary>
        public string Socket_Message
        {
            get
            {
                return _Socket_Message;
                //return _Socket_Message;
            }
            set
            {
                _Socket_Message = value;
            }
        }

        private string _User_IP;
        /// <summary>
        /// 用户输入IP
        /// </summary>
        public string User_IP
        {
            get
            {
                return _User_IP;
                //return _Socket_Message;
            }
            set
            {
                _User_IP = value;
            }
        }

        public static ObservableCollection<Socket_Models_List> _Socket_Read_List { set; get; } = new ObservableCollection<Socket_Models_List>() { };
        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        public static ObservableCollection<Socket_Models_List> Socket_Read_List
        {
            get
            {
                return _Socket_Read_List;
            }
            set
            {

                _Socket_Read_List = value;

                //Socket_Read_List_Refresh(value.ToArray());

            }
        }




        /// <summary>
        /// 发送内容集合接收写入
        /// </summary>
        /// <param name="_List">接收数组参数</param>
        public void List_Connect(ObservableCollection<Socket_Models_List> _List)
        {

            //写入集合中
            foreach (var item in _List)
            {

                if (!UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                {

                    UserControl_Right_Socket_Connection_ViewModel.Socket_Read_List.Add(item);
                }

            }


        }






        //连接按钮屏蔽方法
        public void Connect_Button_IsEnabled_Method(bool Bool_Try)
        {

            Connect_Button_IsEnabled = Bool_Try;




        }


        //网络连接成功状态方法
        public void Connect_Socketing_Method(int bool_Try)
        {
            switch (bool_Try)
            {
                case -1:
                    Connect_Socket_Connection = false;
                    Connect_Socket_OK = false;
                    break;
                case 0:
                    Connect_Socket_Connection = true;
                    break;
                case 1:
                    Connect_Socket_OK = true;
                    break;
                default:
                    User_Control_Log_ViewModel.User_Log_Add($"-1网络状态显示，传入错误值");
                    break;
            }

        }


        //接收到信息显示到前端界面方法
        public void Socket_Message_Show(string Message)
        {

            Socket_Message = Message;

        }










        public ICommand Socket_Send_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_SendToKuka);
        }
        /// <summary>
        /// Socket发送事件命令
        /// </summary>
        private void Socket_SendToKuka(UserControl_Right_Socket_Connection Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;











            Socket_Send.Send_Write_Var(Sm.Send_Name.Text, Sm.Send_Val.Text);





        }












        public ICommand Socket_Connection_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Connection);
        }
        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        private async void Socket_Connection(UserControl_Right_Socket_Connection Sm)
        {
            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;

            Socket_Connect Soceket_KUKA_Client = new Socket_Connect();


            //Socket_Receive.


            //创建连接
            await Soceket_KUKA_Client.Socket_Client_KUKA(Sm.TB1.Text, int.Parse(Sm.TB2.Text));












        }



        public ICommand Socket_Close_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Clos);
        }
        /// <summary>
        /// Socket关闭事件命令
        /// </summary>
        private async void Socket_Clos(UserControl_Right_Socket_Connection Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;




            await Socket_Connect.Socket_Close();



        }






    }
}
