

using HanGao.View.FrameShow;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Other_Window_VM : ObservableObject
    {
        public Other_Window_VM()
        {






        }



        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private IntPtr OutWindow { get; set; } = IntPtr.Zero;

        private IntPtr _AppInPtr { get; set; } = IntPtr.Zero;








        public void Initialization()
        {








        }

        private Process _App = new Process();


        /// <summary>
        /// 
        /// </summary>
        public ICommand Loaded_RunApp_Command
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {

                Other_Window _UserControl = Sm.Source as Other_Window;



                OutWindow = _UserControl.Window.Handle;
                Task.Run(() =>
                {

                //是否嵌入成功标志，用作返回值
                bool isEmbedSuccess = false;

                    ProcessStartInfo info = new ProcessStartInfo("C:/Users/H/Desktop/1.txt");
                    info.UseShellExecute = true;
                    info.WindowStyle = ProcessWindowStyle.Minimized;

                    _App = Process.Start(info);

                //_App.StartInfo.FileName = "C:/Users/H/Desktop/Everything.exe";

                //var aa = _App.Start();


                    Task.Delay(1000);
                _App.WaitForInputIdle();


              
                //var helper=new   WindowInteropHelper(OutWindow);

                //var helper = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow);
                //helper.Owner = _AppInPtr;

                 _AppInPtr = _App.MainWindowHandle;

                if (_AppInPtr != (IntPtr)0 && OutWindow != (IntPtr)0)
                {


                    do
                    {
                        //isEmbedSuccess=Win32Api

                        isEmbedSuccess = SetParent(_AppInPtr, OutWindow) != 0;
                        ShowWindowAsync(_AppInPtr, 3);


                        Thread.Sleep(100);

                        //MoveWindow(OutWindow, 0, 0, (int)_UserControl.ActualWidth, (int)_UserControl.ActualHeight, true);


                    } while (!isEmbedSuccess);

                }

                });

            });
        }










    }
}
