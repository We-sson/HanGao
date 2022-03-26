

using PropertyChanged;
using Soceket_Connect;
using Soceket_KUKA;
using Soceket_KUKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HanGao.Errorinfo;
using HanGao.Socket_KUKA;
using HanGao.View.User_Control;
using HanGao.ViewModel;
using static Soceket_Connect.Socket_Connect;
using static Soceket_KUKA.Models.Socket_Eunm;
using static Soceket_KUKA.Models.Socket_Models_Connect;
using static Soceket_KUKA.Models.Socket_Models_Receive;

using static Soceket_KUKA.Socket_Receive;
using static HanGao.Socket_KUKA.Socket_Sever;

using static HanGao.ViewModel.User_Control_Log_ViewModel;
using static HanGao.ViewModel.UserControl_Socket_Setup_ViewModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;

namespace HanGao.Model

{
    [AddINotifyPropertyChangedInterface]
    public class Socket_Setup_Models : ObservableObject
    {

        public Socket_Setup_Models()
        {

        }

        #region 属性

        /// <summary>
        /// 设置IP
        /// </summary>
        private string _IP;

        public  string IP
        {
            get { return _IP; }
            set { _IP = value;  }
        }



        /// <summary>
        /// 设置端口
        /// </summary>
        private string _Port;

        public  string Port
        {
            get { return _Port; }
            set { _Port = value;  }
        }

        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        /// <summary>
        /// IP输入识别内容属性
        /// </summary>
        public IP_Text_Error Text_Error { set; get; } 

        /// <summary>
        /// 连接按钮名称属性
        /// </summary>
        public string Control_Name_String { set; get; } = null!;

        /// <summary>
        /// 连接类型枚举属性
        /// </summary>
        public Socket_Type Connect_Socket_Type { set; get; } = Socket_Type.Null;

        /// <summary>
        /// 连接类型枚举定义
        /// </summary>
        public enum Socket_Type { Null = -1, Client, Server }

        /// <summary>
        /// 写入TCP对象
        /// </summary>
        public Socket_Connect Write { set; get; } 

        /// <summary>
        /// 循环读取TCP对象
        /// </summary>
        public Socket_Connect Read { set; get; }

        /// <summary>
        /// 单次TCP对象
        /// </summary>
        public Socket_Connect One_Read { set; get; } 






        /// <summary>
        /// 服务器属性
        /// </summary>
        public Socket_Sever Sever { set; get; } 


        /// <summary>
        /// 连接按钮连接后禁止重复连接
        /// </summary>
        public bool Connect_Button_IsEnabled { set; get; } = true;

        /// <summary>
        /// 设备连接中状态...
        /// </summary>
        public bool Connect_Socket_Connection { set; get; } = false;


        /// <summary>
        /// 设备成功状态...
        /// </summary>
        public bool Connect_Socket_OK { set; get; } = false;

        #endregion

        #region 方法




        /// <summary>
        /// 客户端连接按钮显示状态
        /// </summary>
        /// <param name="_int"></param>
        public void Client_Button_Show(int _int)
        {
            switch (_int)
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
                    User_Log_Add($"-1网络状态显示，传入错误值");
                    break;
            }
        }



        #endregion


        #region 命令

        /// <summary>
        /// Socket连接事件命令
        /// </summary>
        public ICommand Socket_Client_Connection_Comm
        {
            get => new RelayCommand<UserControl_Socket_Conntec_UI>(async (Sm) =>
          {
              await Task.Run(() =>
              {


                  Application.Current.Dispatcher.Invoke(() =>
                  {
                      //把参数类型转换控件

               
                      //Socket_Connect _Client = new Socket_Connect();
                      //Socket_Sever _Server = new Socket_Sever();


                      switch (Connect_Socket_Type)
                      {
                          case Socket_Type.Client:
               
                              Socket_Client_Setup.Read.Socket_Client_Thread(Read_Write_Enum.Read,IP,Port);
                             
                              break;
                          case Socket_Type.Server:
                              Socket_Server_Setup.Sever.Socket_Server_KUKA();
                              break;

                      }
                        //创建连接

                    });
              });

          });
        }


        /// <summary>
        /// Socket关闭事件命令
        /// </summary>
        public ICommand Socket_Close_Comm
        {
            get => new RelayCommand<UserControl_Socket_Conntec_UI>(async (Sm) =>
           {
               await Task.Run(() =>
              {
                  //把参数类型转换控件
                  //UIElement e = Sm.Source as UIElement;


                  Application.Current.Dispatcher.Invoke(() =>
                  {
                      //把参数类型转换控件


              


                      switch (Connect_Socket_Type)
                      {
                          case Socket_Type.Client:
                              Socket_Client_Setup.Read.Is_Read_Client = false;
                              //User_Log_Add("用户退出读取连接！");
                              break;
                          case Socket_Type.Server:
                              Socket_Server_Setup.Sever.Socket_Server_Stop();
                              break;

                      }
                      //创建连接

                  });
              });
           });
        }



        #endregion

    }
}
