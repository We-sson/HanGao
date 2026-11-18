using System;
using System.Windows;
using System.Windows.Input;
using Macad.Interaction;
using Macad.Interaction.Panels;

namespace RobotSimulator.Services;

/// <summary>
/// 项目约定的只读视口鼠标手势：左键拖动旋转、右键拖动平移、滚轮缩放。
/// Macad 默认控件是左键框选、中键旋转、右键平移；这里去掉框选与选择，
/// 避免仿真画面被选中高亮，并与其他三维 CAD 软件的操作习惯保持一致。
/// </summary>
public sealed class RobotViewportMouseControl : IViewportMouseControl
{
    ViewportController? _viewportController;

    /// <summary>由 <see cref="ViewportPanel"/> 在视图控制器就绪或更换时注入。</summary>
    public ViewportController ViewportController
    {
        set => _viewportController = value;
    }

    public void MouseMove(Point pos, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        var controller = ResolveController();
        if (controller is null) return;

        // 模式由物理按键状态决定：左键旋转、右键平移；松开后 mode 回到 None，控制器自行结束手势。
        if (mouseDevice?.LeftButton == MouseButtonState.Pressed)
            controller.MouseMove(pos, modifierKeys, ViewportController.MouseMoveMode.Rotating);
        else if (mouseDevice?.RightButton == MouseButtonState.Pressed)
            controller.MouseMove(pos, modifierKeys, ViewportController.MouseMoveMode.Panning);
        else
            controller.MouseMove(pos, modifierKeys);
    }

    public void MouseWheel(Point pos, MouseWheel wheel, int delta, InputDevice device, ModifierKeys modifierKeys)
    {
        var controller = ResolveController();
        if (controller is null || delta == 0) return;

        // Ctrl 缩小滚轮步长，与 Macad 默认控件的精细缩放一致。
        if ((modifierKeys & ModifierKeys.Control) != 0) delta /= 10;

        if (wheel == Macad.Interaction.Panels.MouseWheel.Vertical)
            controller.Zoom(pos, delta / 200.0);
        else
            controller.Rotate(delta / -50.0, 0.0, 0.0);

        controller.MouseMove(pos, modifierKeys);
    }

    public void MouseDown(Point pos, MouseButton changedButton, int clickCount, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        // 左键只用于拖动旋转，不触发 Macad 的选择/框选，因此不调用 ViewportController.MouseDown。
        ResolveController()?.MouseMove(pos, modifierKeys);
    }

    public void MouseUp(Point pos, MouseButton changedButton, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        // 松开按键后回到普通悬停状态，让控制器结束旋转/平移。
        ResolveController()?.MouseMove(pos, modifierKeys);
    }

    public void Cancel()
    {
        ResolveController()?.MouseMove(new Point(-1.0, -1.0));
    }

    // ViewportPanel 可能在控件构造后才注入视图控制器；兜底从全局交互上下文取。
    ViewportController? ResolveController()
    {
        if (_viewportController is not null) return _viewportController;
        try
        {
            return _viewportController = InteractiveContext.Current?.ViewportController;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
