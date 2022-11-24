
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.View.FrameShow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Reflection;
using static HanGao.ViewModel.Messenger_Eunm.Messenger_Name;
using static Soceket_Connect.Socket_Connect;
using static HanGao.ViewModel.UC_Vision_CameraSet_ViewModel;


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



        /// <summary>
        /// 静态属性更新通知事件
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        private static bool _ProgramEdit_Enabled = false;
        /// <summary>
        /// 程序页面可点击属性
        /// </summary>
        public static bool ProgramEdit_Enabled
        {
            get { return _ProgramEdit_Enabled; }
            set
            {

                _ProgramEdit_Enabled = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(ProgramEdit_Enabled)));

            }
        }









        private static bool _Vision_Work_UI;
        /// <summary>
        /// 程序编辑页面
        /// </summary>
        public static bool Vision_Work_UI
        {
            get { return _Vision_Work_UI; }
            set
            {
                _Vision_Work_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Vision_Work_UI)));

            }
        }


        private static bool _Program_Edit_UI;
        /// 数据页面
        /// </summary>
        public static bool Program_Edit_UI
        {
            get { return _Program_Edit_UI; }
            set
            {
                _Program_Edit_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Program_Edit_UI)));

            }
        }

        private static bool _Other_Window_UI;
        /// <summary>
        /// 震镜页面
        /// </summary>
        public static bool Other_Window_UI
        {
            get { return _Other_Window_UI; }
            set
            {

                _Other_Window_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Other_Window_UI)));

            }
        }



        private static bool _Home_Console_UI = true;
        /// <summary>
        /// 控制台主页
        /// </summary>
        public static bool Home_Console_UI
        {
            get { return _Home_Console_UI; }
            set
            {
                _Home_Console_UI = value;
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(nameof(Home_Console_UI)));

            }
        }



        /// <summary>
        /// 软件启动初始化时间
        /// </summary>
        public ICommand Loaded_Window
        {
            get => new AsyncRelayCommand<HanGao.MainWindow>(async (Sm, T) =>
            {
                await Task.Delay(0);


                Messenger.Send<dynamic , string>(true, nameof(Meg_Value_Eunm.Initialization_Camera));



            });
        }

        /// <summary>
        /// 软件启动初始化时间
        /// </summary>
        public ICommand Closed_Window
        {
            get => new AsyncRelayCommand<HanGao.MainWindow>(async (Sm, T) =>
            {
                await Task.Delay(0);


                Messenger.Send<dynamic, string>(true, nameof(Meg_Value_Eunm.Close_Camera));



            });
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
