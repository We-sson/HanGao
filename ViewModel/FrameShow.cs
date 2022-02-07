
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HanGao.View.FrameShow;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class FrameShow : ObservableObject
    {

        public FrameShow()
        {

 
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



        private UserControl _User_Show = HomeOne;

        public UserControl User_Show
        {
            get { return _User_Show; }
            set { _User_Show = value; }
        }


        private bool _HomeOne_UI=true;
        /// <summary>
        /// 控制台主页
        /// </summary>
        public bool HomeOne_UI
        {
            get { return _HomeOne_UI; }
            set {
                _HomeOne_UI = value;
                if (_HomeOne_UI)
                {
                    User_Show = HomeOne;
                }
            }
        }


        private bool _ProgramEdit_UI;
        /// <summary>
        /// 程序编辑页面
        /// </summary>
        public bool ProgramEdit_UI
        {
            get { return _ProgramEdit_UI; }
            set { 
                _ProgramEdit_UI = value;
                if (_ProgramEdit_UI)
                {
                    User_Show = ProgRamEdit;
                }
            }
        }


        private bool _RealData_UI;
        /// <summary>
        /// 数据页面
        /// </summary>
        public bool RealData_UI
        {
            get { return _RealData_UI; }
            set { 
                _RealData_UI = value;
                if (_RealData_UI)
                {
                    User_Show = ReadData;
                }
            }
        }

        private bool _WeldingGUI_UI;
        /// <summary>
        /// 震镜页面
        /// </summary>
        public bool WeldingGUI_UI
        {
            get { return _WeldingGUI_UI; }
            set {
                
                _WeldingGUI_UI = value;
                if (_WeldingGUI_UI)
                {
                    User_Show = WeldingGui;
                }
            }
        }









        public ICommand Min_Window
        {
            get => new RelayCommand<Window>(Min_Window_Comm);
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
            get => new RelayCommand<Window>(Max_Window_Comm);
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
            get => new RelayCommand<Window>(Close_Window_Comm);
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
