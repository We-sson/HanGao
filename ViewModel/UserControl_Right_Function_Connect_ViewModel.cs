using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Nancy.Helpers;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public  class UserControl_Right_Function_Connect_ViewModel: ViewModelBase
    {
        
        public UserControl_Right_Function_Connect_ViewModel()
        {
            //注册消息接收
            Messenger.Default.Register<bool?>(this, "Sidebar_Subtitle_Signal_Method_bool", Sidebar_Subtitle_Signal_Method_bool);




            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Sidebar_MainTitle = "连接状态";


                Subtitle_Position = new Thickness() { Top = 100 };
            }
            else
            {
                // Code runs "for real"
            }


        }

        private  bool _Open = false;
        /// <summary>
        /// 侧边栏打开关闭控制
        /// </summary>
        public bool Open
        {
            get
            {
                return _Open;
            }
            set
            {
                _Open = value;
            }
        }

        private string _Sidebar_MainTitle = null ;
        /// <summary>
        /// 侧边栏主标题
        /// </summary>
        public string  Sidebar_MainTitle
        {
            get
            {

                return _Sidebar_MainTitle;
            }
            set
            {
                //每个标题字符之间添加空格
                Sidebar_Subtitle = value;
                char[] st = value.ToCharArray();
                for (int i = 0; i < st.Length; i++)
                {
                    _Sidebar_MainTitle += st[i] + HttpUtility.HtmlDecode("&#0032;");
                }

                  
               
            }
        }


        private bool _Sidebar_Subtitle_Signal = false;
        /// <summary>
        /// 副标题连接状态指示灯闪烁
        /// </summary>
        public bool Sidebar_Subtitle_Signal
        {
            get
            {
                return _Sidebar_Subtitle_Signal;
            }
            set
            {
                _Sidebar_Subtitle_Signal = value;
            }
        }


        /// <summary>
        /// 副标题连接状态指示灯闪方法
        /// </summary>
        public void Sidebar_Subtitle_Signal_Method_bool(bool? B)
        {
            Sidebar_Subtitle_Signal = (bool)B;
        }





        private string _Sidebar_Subtitle ;
        /// <summary>
        /// 侧边栏副标题
        /// </summary>
        public string Sidebar_Subtitle
        {
            get
            {
                return _Sidebar_Subtitle;
            }
            set
            {


                //每个副标题字符之间添加回车
                char[] st = value.ToCharArray();
                for (int i = 0; i < st.Length; i++)
                {
                    _Sidebar_Subtitle += st[i] + HttpUtility.HtmlDecode("&#10;");
                    
                }
            }
        }

        private UserControl _Sidebar_Control ;
        /// <summary>
        /// 侧边栏内容
        /// </summary>
        public UserControl Sidebar_Control
        {
            get
            {
                return _Sidebar_Control;
            }
            set
            {
                _Sidebar_Control = value;
            }
        }





         private Thickness _Subtitle_Position;
        /// <summary>
        /// 侧边栏副标题高度
        /// </summary>
        public Thickness Subtitle_Position
        {
            get
            {
                return _Subtitle_Position;
            }
            set
            {
                _Subtitle_Position = value;
            }
        }





        public ICommand Click_OPen_Comm
        {
            get => new DelegateCommand<RoutedEventArgs>(Click_OPen);
        }
        /// <summary>
        /// 侧边栏打开关闭事件命令
        /// </summary>
        private void Click_OPen(RoutedEventArgs Sm)
        {
            //把参数类型转换控件
            UIElement e = Sm.Source as UIElement;



            if (e.Uid=="Open")
            {
                Open = true;
                //侧边栏打开主页面模糊
                Messenger.Default.Send<Double>(10,"Open_Effect");
                //侧边栏打开后主页黑化禁止用户操作
                Messenger.Default.Send<Visibility >(Visibility.Visible, "Home_Visibility_Show");

                
            }
            else if (e.Uid=="Close")
            {
                Open = false;
                Messenger.Default.Send<Double>(0, "Open_Effect");
                Messenger.Default.Send<Visibility>(Visibility.Collapsed, "Home_Visibility_Show");

            }

        }


        

        





    }
}
