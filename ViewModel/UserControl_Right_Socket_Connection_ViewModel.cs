using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.Errorinfo;
using 悍高软件.Model;
using 悍高软件.Socket_KUKA;
using 悍高软件.View.User_Control;
using static Soceket_KUKA.Models.Socket_Models_Receive;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class UserControl_Right_Socket_Connection_ViewModel: ViewModelBase  
    {



        public UserControl_Right_Socket_Connection_ViewModel()
        {
        
            //Socket_Read_List.Add(new Socket_Models_List() { Val_ID= Socket_Models_Connect.Number_ID, Val_Name = "$POS_ACT"});
    



            //注册消息接收

            Messenger.Default.Register<string>(this, "Socket_Message_Show", Socket_Message_Show);

            Messenger.Default.Register<bool>(this, "Connect_Button_IsEnabled_Method", Connect_Button_IsEnabled_Method);
            Messenger.Default.Register<bool>(this, "Connect_Socketing_Method", Connect_Socketing_Method);






        }



        private bool _Connect_Button_IsEnabled=true;
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
        public  bool Connect_Socketing { set; get; }



        private   string  _Socket_Message="准备接收...." ;
        /// <summary>
        /// 接收消息属性
        /// </summary>
        public  string Socket_Message
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

        private string _User_IP ;
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

        public static ObservableCollection<Socket_Models_List> _Socket_Read_List=new ObservableCollection<Socket_Models_List>() ;
        /// <summary>
        /// 读取库卡变量列表集合
        /// </summary>
        public static ObservableCollection<Socket_Models_List> Socket_Read_List
        {
            get { return _Socket_Read_List; }
            set {
                
                _Socket_Read_List = value;
               
            }
        }






        //连接按钮屏蔽方法
        public void Connect_Button_IsEnabled_Method(bool Bool_Try)
        {

            Connect_Button_IsEnabled = Bool_Try;

        }


        //网络连接中状态方法
        public void Connect_Socketing_Method(bool bool_Try)
        {
            Connect_Socketing = bool_Try;
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

           //把输入框的消息发送过去
            //Messenger.Default.Send<string>(Sm.Socket_Send.Text, "Socket_Send_Message_Method");




            //Soceket_Send.Socket_Send_Message_Method(Sm.Socket_Send.Text);


            //Socket_Send.Send_Read_Var(Sm.Socket_Send.Text);

      

            Socket_Send.Send_Write_Var(Sm.Send_Name.Text, Sm.Send_Val.Text);
             

   


        }












        public ICommand Socket_Connection_Comm
        {
            get => new DelegateCommand<UserControl_Right_Socket_Connection>(Socket_Connection);
        }
        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        private void Socket_Connection(UserControl_Right_Socket_Connection Sm)
        {
            //把参数类型转换控件
            //UIElement e = Sm.Source as UIElement;
            
       Socket_Connect Soceket_KUKA_Client = new Socket_Connect();


            //Socket_Receive.

 
            //创建连接
            Soceket_KUKA_Client.Socket_Client_KUKA(Sm.TB1.Text, int.Parse(Sm.TB2.Text));


            









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

            //把输入框的消息发送过去
            //Messenger.Default.Send<string>(Sm.Socket_Send.Text, "Socket_Send_Message_Method");

            //Soceket_Send.Socket_Send_Message_Method(Sm.Socket_Send.Text);


            await Socket_Connect.Socket_Close();

            

        }






    }
}
