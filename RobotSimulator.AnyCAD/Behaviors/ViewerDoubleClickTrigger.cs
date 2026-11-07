using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using AnyCAD.WPF;
using Microsoft.Xaml.Behaviors;

namespace RobotSimulator.Behaviors;

/// <summary>
/// 把原生视口的左键双击接入 XAML，由 InvokeCommandAction 执行居中命令。
/// AnyCAD 虽提供 ZoomByDoubleClick 配置，但 HwndHost 视口下实际不会触发，因此保留消息钩子。
/// </summary>
public sealed class ViewerDoubleClickTrigger : TriggerBase<RenderControl>
{
    /// <summary>AnyCAD 的原生窗口宿主；WPF 鼠标事件无法覆盖此窗口的输入。</summary>
    private HwndHost? _host;

    /// <summary>在加载或渲染器就绪时连接原生窗口，支持视口卸载后重新加载。</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
        AssociatedObject.ViewerReady += AttachHost;
        if (AssociatedObject.IsLoaded) AttachHost();
    }

    /// <summary>移除行为时释放所有订阅，避免旧视口继续执行命令。</summary>
    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnloaded;
        AssociatedObject.ViewerReady -= AttachHost;
        DetachHost();
        base.OnDetaching();
    }

    /// <summary>视口进入可视树后，连接其内部的原生宿主。</summary>
    private void OnLoaded(object sender, RoutedEventArgs e) => AttachHost();

    /// <summary>视口离开可视树后，不再监听原生窗口。</summary>
    private void OnUnloaded(object sender, RoutedEventArgs e) => DetachHost();

    /// <summary>只使用公开的 HwndHost 接口，不依赖 SDK 内部类型或字段。</summary>
    private void AttachHost()
    {
        if (_host is not null || AssociatedObject is null) return;
        if (!AssociatedObject.Dispatcher.CheckAccess())
        {
            AssociatedObject.Dispatcher.BeginInvoke(AttachHost);
            return;
        }
        if (!AssociatedObject.IsLoaded) return;
        _host = FindHost(AssociatedObject);
        if (_host is not null) _host.MessageHook += OnMessage;
    }

    /// <summary>解除窗口消息订阅，可重复执行。</summary>
    private void DetachHost()
    {
        if (_host is null) return;
        _host.MessageHook -= OnMessage;
        _host = null;
    }

    /// <summary>查找 AnyCAD 可视树中的原生窗口宿主。</summary>
    private static HwndHost? FindHost(DependencyObject parent)
    {
        if (parent is HwndHost host) return host;
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var found = FindHost(VisualTreeHelper.GetChild(parent, index));
            if (found is not null) return found;
        }
        return null;
    }

    /// <summary>只响应左键双击；排队执行命令，避免在原生输入处理中修改相机。</summary>
    private IntPtr OnMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int leftDoubleClick = 0x0203;
        if (message != leftDoubleClick) return IntPtr.Zero;
        var control = AssociatedObject;
        var host = _host;
        if (control is null || !control.IsReady) return IntPtr.Zero;
        handled = true;
        control.Dispatcher.BeginInvoke(() =>
        {
            if (ReferenceEquals(control, AssociatedObject) && ReferenceEquals(host, _host)
                && control.IsLoaded && control.IsReady)
                InvokeActions(null);
        });
        return IntPtr.Zero;
    }
}
