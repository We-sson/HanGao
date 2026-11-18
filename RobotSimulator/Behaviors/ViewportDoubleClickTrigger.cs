using System.Windows.Input;
using Macad.Interaction.Panels;
using Microsoft.Xaml.Behaviors;

namespace RobotSimulator.Behaviors;

/// <summary>将视口中的左键双击交给适合窗口命令。</summary>
public sealed class ViewportDoubleClickTrigger : TriggerBase<ViewportPanel>
{
    // Macad 视口拥有独立的名称作用域，这里由宿主直接提供命令。
    internal static void SetCommand(ViewportPanel panel, ICommand command)
    {
        var trigger = Interaction.GetTriggers(panel).OfType<ViewportDoubleClickTrigger>().Single();
        trigger.Actions.OfType<InvokeCommandAction>().Single().Command = command;
    }

    protected override void OnAttached() => AssociatedObject.PreviewMouseLeftButtonDown += OnClick;

    protected override void OnDetaching() => AssociatedObject.PreviewMouseLeftButtonDown -= OnClick;

    private void OnClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 2) return;
        e.Handled = true;
        InvokeActions(e);
    }
}
