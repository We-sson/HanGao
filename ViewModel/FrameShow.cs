using GalaSoft.MvvmLight;
using Prism.Commands;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 悍高软件.View.FrameShow;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class FrameShow : ViewModelBase
    {
        /// <summary>
        /// 页面初始化内容
        /// </summary>
        public FrameShow()
        {

            //启动首页
            User_Show = HomeOne;
        }
        /// <summary>
        /// 软件启动把页面写入内存
        /// </summary>
        private readonly UserControl HomeOne = new HomeOne();
        private readonly UserControl ProgRamEdit = new ProgramEdit();
        private readonly UserControl ReadData = new RealData();
        private readonly UserControl WeldingGui = new WeldingGui();



        /// <summary>
        /// 用于前段绑定显示页面内容
        /// </summary>
        private UserControl _User_Show;
        public UserControl User_Show
        {

            get { return _User_Show; }
            set
            {
                if (_User_Show == value)
                {
                    return;
                }
                _User_Show = value;
            }
        }




        /// <summary>
        /// 标题栏选择事件触发
        /// </summary>
        public ICommand Set_User_Show
        {
            get => new DelegateCommand<Control>(Set_User);
        }



        /// <summary>
        /// 使用Uid识别码来显示内容
        /// </summary>
        /// <param name="_name"></param>
        private void Set_User(Control _name)
        {

            switch (_name.Uid)
            {
                case "1":
                    User_Show = HomeOne;
                    break;
                case "2":
                    User_Show = ProgRamEdit;
                    break;
                case "3":
                    User_Show = ReadData;
                    break;
                case "4":
                    User_Show = WeldingGui;
                    break;
            }
        }



        public ICommand Min_Window
        {
            get => new DelegateCommand<Window>(Min_Window_Comm);
        }
        /// <summary>
        /// 主窗口最小化
        /// </summary>
        private void Min_Window_Comm(Window Sm)
        {


            //窗口最小化
            Sm.WindowState = WindowState.Minimized;

        }

        public ICommand Max_Window
        {
            get => new DelegateCommand<Window>(Max_Window_Comm);
        }
        /// <summary>
        /// 主窗口最大化
        /// </summary>
        private void Max_Window_Comm(Window Sm)
        {


            //窗口最大化
            Sm.WindowState = WindowState.Maximized;

        }
        public ICommand Close_Window
        {
            get => new DelegateCommand<Window>(Close_Window_Comm);
        }
        /// <summary>
        /// 主窗口关闭
        /// </summary>
        private void Close_Window_Comm(Window Sm)
        {

            //窗口关闭
            Sm.Close();


        }

    }
}
