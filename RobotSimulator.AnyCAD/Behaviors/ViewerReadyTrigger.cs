using AnyCAD.WPF;
using Microsoft.Xaml.Behaviors;

namespace RobotSimulator.Behaviors;

/// <summary>把 AnyCAD 的就绪回调接入 XAML 行为，以 InvokeCommandAction 调用 ViewModel 命令。</summary>
public sealed class ViewerReadyTrigger : TriggerBase<RenderControl>
{
    /// <summary>行为绑定到 RenderControl 时订阅就绪事件；控件若已就绪，则补发一次通知。</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.ViewerReady += OnViewerReady;
        if (AssociatedObject.IsReady)
        {
            OnViewerReady();
        }
    }

    /// <summary>行为移除时取消订阅，避免旧视图继续响应事件或被事件引用。</summary>
    protected override void OnDetaching()
    {
        AssociatedObject.ViewerReady -= OnViewerReady;
        base.OnDetaching();
    }

    /// <summary>把回调排到 UI 队列，使控件完成初始化后再触发绑定的命令。</summary>
    private void OnViewerReady()
    {
        AssociatedObject.Dispatcher.BeginInvoke(() =>
        {
            // 队列执行前行为可能已被移除，因此再次确认关联控件仍存在。
            if (AssociatedObject is not null)
            {
                InvokeActions(null);
            }
        });
    }
}
