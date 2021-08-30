using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using 悍高软件.Errorinfo;
using 悍高软件.Model;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Socket_Setup_ViewModel : ViewModelBase
    {



        public UserControl_Socket_Setup_ViewModel()
        {


            //Socket_Read_List.Add(new Socket_Models_List() { Val_Name = "$VEL_ACT", Val_ID = Socket_Models_Connect.Number_ID });



            //注册消息接收


            //Messenger.Default.Register<string>(this, "Socket_Message_Show", Socket_Message_Show);

            //连接按钮屏蔽方法
            Messenger.Default.Register<bool>(this, "Connect_Client_Button_IsEnabled", (_Bool)=> 
            {
                Socket_Client_Setup.Connect_Button_IsEnabled = _Bool;
            });



            //连接控制柜，网络连接状态显示方法
            Messenger.Default.Register<int>(this, "Connect_Client_Socketing_Button_Show", (_int)=> 
            {
                Socket_Client_Setup.Client_Button_Show(_int);
            });

            //读取变量集合发送
            Messenger.Default.Register<ObservableCollection<Socket_Models_List>>(this, "List_Connect", List_Connect);


            //通讯延时绑定
            Messenger.Default.Register<string>(this, "Connter_Time_Delay_Method",(s)=> { Connter_Time_Delay = s; });







        }


        public Socket_Setup_Models Socket_Client_Setup { set; get; } = new Socket_Setup_Models() { Control_Name_String="连接控制柜" ,Text_Error = new IP_Text_Error() { User_IP = "192.168.159.147", User_Port = "7000" } };
        public Socket_Setup_Models Socket_Server_Setup { set; get; } = new Socket_Setup_Models() { Control_Name_String = "监听控制柜", Text_Error = new IP_Text_Error() { User_IP = "192.168.159.1", User_Port = "5000" } };





        //private string _Socket_Message = "准备接收....";
        ///// <summary>
        ///// 接收消息属性
        ///// </summary>
        //public string Socket_Message
        //{
        //    get
        //    {
        //        return _Socket_Message;
        //        //return _Socket_Message;
        //    }
        //    set
        //    {
        //        _Socket_Message = value;
        //    }
        //}

        /// <summary>
        /// 通讯延时显示
        /// </summary>
        public string Connter_Time_Delay { set; get; } = "0";


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

                if (!Socket_Read_List.Any<Socket_Models_List>(l => l.Val_Name == item.Val_Name))
                {

                    Socket_Read_List.Add(item);
                }

            }


        }











        ////接收到信息显示到前端界面方法
        //public void Socket_Message_Show(string Message)
        //{

        //    Socket_Message = Message;

        //}










        public ICommand Socket_Send_Comm
        {
            get => new DelegateCommand<User_Control_Socket_Setup>(Socket_SendToKuka);
        }
        /// <summary>
        /// Socket发送事件命令
        /// </summary>
        private void Socket_SendToKuka(User_Control_Socket_Setup Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;










            //发输入框内的数值
            //Socket_Send.Send_Write_Var(Sm.Send_Name.Text, Sm.Send_Val.Text);





        }















        public ICommand Socket_Close_Comm
        {
            get => new DelegateCommand<User_Control_Socket_Setup>(Socket_Clos);
        }
        /// <summary>
        /// Socket关闭事件命令
        /// </summary>
        private  void Socket_Clos(User_Control_Socket_Setup Sm)
        {

            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;




             Socket_Connect.Socket_Close();



        }






    }
}
