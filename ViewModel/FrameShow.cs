
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.View.FrameShow;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Reflection;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static Soceket_Connect.Socket_Connect;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class FrameShow : ObservableRecipient
    {

        public FrameShow()
        {

            var a = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //App_VerSion = Application.ResourceAssembly.GetName().Version.ToString();

            //通讯延时绑定
            Messenger.Register<dynamic , string>(this, nameof(Meg_Value_Eunm.Connter_Time_Delay_Method), (O, _String) => 
            { 
                Connter_Time_Delay=_String; 
            });

            ///服务器现在状态映射UI
            Messenger.Register<string , string>(this, nameof(Meg_Value_Eunm.Socket_Read_Tpye), (O, _S) =>
            {

                UI_Socket_Type = (Socket_Tpye)Enum.Parse(typeof (Socket_Tpye),_S);

            });
        }

        /// <summary>
        /// 软件启动把页面写入内存
        /// </summary>
        public  static  UserControl HomeOne = new HomeOne();
        public  static UserControl ProgRamEdit = new ProgramEdit();
        public  static UserControl ReadData = new RealData();
        public  static UserControl WeldingGui = new WeldingGui();



        /// <summary>
        /// 用于前段绑定显示页面内容
        /// </summary>


        /// <summary>
        /// 版本显示
        /// </summary>
        public string App_VerSion { set; get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 通讯延时显示
        /// </summary>
        public double  Connter_Time_Delay { set; get; } =0.0;


        /// <summary>
        /// UI连接状态属性
        /// </summary>
        public Socket_Tpye UI_Socket_Type { set; get; } = Socket_Tpye.Connect_Cancel;


        private static  UserControl _User_Show = HomeOne;
        /// <summary>
        /// 界面显示
        /// </summary>
        public static  UserControl User_Show
        {
            get { return _User_Show; }
            set { 
                _User_Show = value; 
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(User_Show)));

            }
        }


        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        private static  bool _ProgramEdit_Enabled = false   ;
        /// <summary>
        /// 程序页面可点击属性
        /// </summary>
        public static  bool ProgramEdit_Enabled
        {
            get { return _ProgramEdit_Enabled; }
            set
            {

                _ProgramEdit_Enabled = value ;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(ProgramEdit_Enabled)));

            }
        }



    



        private static  bool _HomeOne_UI=true;
        /// <summary>
        /// 控制台主页
        /// </summary>
        public static  bool HomeOne_UI
        {
            get { return _HomeOne_UI; }
            set {
                _HomeOne_UI = value;
                if (_HomeOne_UI)
                {
                    User_Show = HomeOne;
                }
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(HomeOne_UI)));

            }
        }


        private static  bool _ProgramEdit_UI;
        /// <summary>
        /// 程序编辑页面
        /// </summary>
        public static  bool ProgramEdit_UI
        {
            get { return _ProgramEdit_UI; }
            set { 
                _ProgramEdit_UI = value;
                if (_ProgramEdit_UI)
                {
                    User_Show = ProgRamEdit;
                }
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(ProgramEdit_UI)));

            }
        }


        private static  bool _RealData_UI;
        /// <summary>
        /// 数据页面
        /// </summary>
        public static  bool RealData_UI
        {
            get { return _RealData_UI; }
            set { 
                _RealData_UI = value;
                if (_RealData_UI)
                {
                    User_Show = ReadData;
                }
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(RealData_UI)));

            }
        }

        private static  bool _WeldingGUI_UI;
        /// <summary>
        /// 震镜页面
        /// </summary>
        public static  bool WeldingGUI_UI
        {
            get { return _WeldingGUI_UI; }
            set {
                
                _WeldingGUI_UI = value;
                if (_WeldingGUI_UI)
                {
                    User_Show = WeldingGui;
                }
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(WeldingGUI_UI)));

            }
        }









        /// <summary>
        /// 主窗口最小化
        /// </summary>
        public ICommand Min_Window
        {
            get => new AsyncRelayCommand<Window>(async (Sm, T) =>
            {
                await Task.Delay(0);

                //窗口最小化
                Sm.WindowState = WindowState.Minimized;
            });
        }
 



        /// <summary>
        /// 主窗口最大化
        /// </summary>
        public ICommand Max_Window
        {
            get => new AsyncRelayCommand<Window>(async (Sm, T) =>
            {
                await Task.Delay(0);

                //窗口最大化
                Sm.WindowState = WindowState.Maximized;
                Sm.WindowStyle = System.Windows.WindowStyle.None;
                Sm.WindowState = System.Windows.WindowState.Maximized;
                Sm.Topmost = true;
                Sm.Left = 0;
                Sm.Top = 0;
                Sm.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                Sm.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                Sm.Hide(); //先调用其隐藏方法 然后再显示出来,这样就会全屏,且任务栏不会出现.如果不加这句 可能会出现假全屏即任务栏还在下面.
                Sm.Show();

            });
        }

        /// <summary>
        /// 主窗口关闭
        /// </summary>
        public ICommand Close_Window
        {
            get => new AsyncRelayCommand<Window>(async (Sm, T) =>
            {
                await Task.Delay(0);

                //窗口关闭
                Sm.Close();
            });
        }


    }
}
