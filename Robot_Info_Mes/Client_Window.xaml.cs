using System.ComponentModel;
using System.Windows;
using Robot_Info_Mes.ViewModel;

namespace Robot_Info_Mes
{
    /// <summary>
    /// 采集端主窗口；具体交互由 XAML 绑定到 <c>Robot_Info_VM</c>，此处只负责装载视图。
    /// </summary>
    public partial class Client_Window : Window
    {
        private bool _modbusShutdownComplete;
        private bool _modbusShutdownInProgress;

        /// <summary>
        /// 初始化客户端页面及其资源、绑定和行为触发器。
        /// </summary>
        public Client_Window()
        {
            InitializeComponent();
            Closing += ClientWindowOnClosing;
        }

        /// <summary>窗口退出前等待 Modbus TCP 服务端监听和全部 SCADA 客户端连接释放完成。</summary>
        private async void ClientWindowOnClosing(object? sender, CancelEventArgs eventArgs)
        {
            if (_modbusShutdownComplete)
            {
                return;
            }

            eventArgs.Cancel = true;
            if (_modbusShutdownInProgress)
            {
                return;
            }

            _modbusShutdownInProgress = true;
            try
            {
                if (DataContext is Robot_Info_VM viewModel)
                {
                    await viewModel.DisposeModbusServerAsync();
                }
            }
            catch
            {
                // 进程退出仍会由操作系统回收端口；释放异常不能让窗口永久卡在取消关闭状态。
            }
            finally
            {
                _modbusShutdownComplete = true;
                _modbusShutdownInProgress = false;
                Close();
            }
        }
    }
}
