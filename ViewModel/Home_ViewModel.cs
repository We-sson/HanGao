using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using 悍高软件.View.User_Control;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Home_ViewModel : ViewModelBase
    {


        public Home_ViewModel()
        {


            //注册消息接收
            Messenger.Default.Register<double>(this, "Open_Effect", Open_Effect);
            Messenger.Default.Register<Visibility>(this, "Home_Visibility_Show", Home_Visibility_Show);

            Sidebar_Control = new UserControl_Right_Function_Connect() { Uid = "1", DataContext = new UserControl_Right_Function_Connect_ViewModel() { Sidebar_MainTitle = "连接状态", Subtitle_Position = new Thickness() { Top = 100 }, Sidebar_Control = new UserControl_Right_Socket_Connection() { } } };


        }


        /// <summary>
        /// 侧边栏打开主页模糊方法
        /// </summary>
        public void Open_Effect(double E)
        {
            Gird_Effect_Radius = E;
        }


        /// <summary>
        /// 侧边栏打开主页模糊方法
        /// </summary>
        public void Home_Visibility_Show(Visibility V)
        {
            Visibility = V;
        }




        private double _Gird_Effect_Radius = 0;
        /// <summary>
        /// 侧面弹出主页模糊
        /// </summary>
        public double Gird_Effect_Radius
        {
            get
            {
                return _Gird_Effect_Radius;
            }
            set
            {
                _Gird_Effect_Radius = value;
            }
        }

        private Visibility _Visibility = Visibility.Collapsed;
        /// <summary>
        /// 屏蔽主页面操作操作显示
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
            }
        }


        private UserControl _Sidebar_Control;
        /// <summary>
        /// 侧边栏显示
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




    }
}
