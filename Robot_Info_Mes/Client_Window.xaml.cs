using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
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

        /// <summary>
        /// 第一次关闭只取消窗口关闭并启动异步释放；清理完成后再由 Dispatcher 发起真正关闭。
        /// </summary>
        private void ClientWindowOnClosing(object? sender, CancelEventArgs eventArgs)
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
            _ = ShutdownModbusAndCloseAsync();
        }

        /// <summary>
        /// 释放 Modbus 端口和连接，并把第二次 Close 排入 UI 消息队列，避免重入 Closing 事件。
        /// </summary>
        private async Task ShutdownModbusAndCloseAsync()
        {
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

                // 即使 DisposeModbusServerAsync 同步完成，也不能在原 Closing 调用栈中再次 Close。
                // BeginInvoke 保证原事件先返回，再执行最终关闭。
                _ = Dispatcher.BeginInvoke(
                    DispatcherPriority.ApplicationIdle,
                    new Action(() =>
                    {
                        Closing -= ClientWindowOnClosing;
                        Close();
                    }));
            }
        }
    }
}
