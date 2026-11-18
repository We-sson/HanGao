using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Macad.Interaction.Panels;

namespace RobotSimulator.Services;

/// <summary>把 Macad 视口面板推迟到宿主窗口显示完成后再挂进可视树。</summary>
/// <remarks>
/// 窗口第一次 Show 期间 WPF 布局会暂停调度器处理，此时 ViewportPanel 创建 OpenCASCADE 原生子窗口，
/// 随后到达的窗口消息会在 HwndSubclass 回调里抛「调度程序处理已暂停，但仍在处理消息」并终止进程。
/// 面板先退出可视树，等宿主加载完成后按空闲优先级挂回，原生视口才在安全的时机建立。
/// </remarks>
internal static class DeferredViewportHost
{
    /// <summary>摘下面板，并在宿主加载完成后挂回；宿主从未加载时面板保持离开可视树。</summary>
    public static void Defer(FrameworkElement owner, Panel host, ViewportPanel panel, Func<bool>? isCancelled = null)
    {
        host.Children.Remove(panel);
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 排到空闲优先级，等第一次 Show 的布局与调度器暂停处理都结束后再建原生视口。
            owner.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (isCancelled?.Invoke() == true)
                {
                    owner.Loaded -= OnLoaded;
                    return;
                }
                // 三维标签可能已被切走。保留订阅，等下一次显示再挂回面板。
                if (!owner.IsLoaded) return;
                if (!host.Children.Contains(panel)) host.Children.Add(panel);
                owner.Loaded -= OnLoaded;
            }));
        }
        owner.Loaded += OnLoaded;
    }
}
