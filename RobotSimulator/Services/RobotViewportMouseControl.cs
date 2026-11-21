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
    const double DegreesPerPixel = 1.0 / 6.0;

    readonly Action<bool>? _onPointerInteraction;
    readonly Action<double, double>? _onOrbitRotate;
    ViewportController? _viewportController;
    Point? _lastRotatePosition;
    bool _wasPressed;

    public RobotViewportMouseControl()
    {
    }

    internal RobotViewportMouseControl(Action<bool> onPointerInteraction, Action<double, double>? onOrbitRotate = null)
    {
        _onPointerInteraction = onPointerInteraction ?? throw new ArgumentNullException(nameof(onPointerInteraction));
        _onOrbitRotate = onOrbitRotate;
    }

    /// <summary>由 <see cref="ViewportPanel"/> 在视图控制器就绪或更换时注入。</summary>
    public ViewportController ViewportController
    {
        set => _viewportController = value;
    }

    public void MouseMove(Point pos, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        var pressed = HasActiveMouseButton(mouseDevice);
        if (pressed)
        {
            _wasPressed = true;
            NotifyInteraction(true);
        }
        else if (_wasPressed)
        {
            _wasPressed = false;
            NotifyInteraction(false);
        }

        if (mouseDevice?.LeftButton == MouseButtonState.Pressed && _onOrbitRotate is not null)
        {
            if (_lastRotatePosition is { } previous)
            {
                var yawDegrees = (previous.X - pos.X) * DegreesPerPixel;
                var pitchDegrees = (previous.Y - pos.Y) * DegreesPerPixel;
                if (Math.Abs(yawDegrees) > 1e-6 || Math.Abs(pitchDegrees) > 1e-6)
                {
                    _onOrbitRotate(yawDegrees, pitchDegrees);
                }
            }

            _lastRotatePosition = pos;
            return;
        }

        _lastRotatePosition = null;
        var controller = ResolveController();
        if (controller is null) return;

        // 左键已由 _onOrbitRotate 绕模型中心转盘旋转；右键平移与普通悬停仍交由控制器处理。
        if (mouseDevice?.RightButton == MouseButtonState.Pressed)
            controller.MouseMove(pos, modifierKeys, ViewportController.MouseMoveMode.Panning);
        else
            controller.MouseMove(pos, modifierKeys);
    }

    public void MouseWheel(Point pos, MouseWheel wheel, int delta, InputDevice device, ModifierKeys modifierKeys)
    {
        var controller = ResolveController();
        if (controller is null || delta == 0) return;

        var pressed = HasActiveMouseButton(device as MouseDevice);
        _wasPressed = pressed;
        NotifyInteraction(pressed);

        // Ctrl 缩小滚轮步长，与 Macad 默认控件的精细缩放一致。
        if ((modifierKeys & ModifierKeys.Control) != 0) delta /= 10;

        if (wheel == Macad.Interaction.Panels.MouseWheel.Vertical)
            controller.Zoom(pos, delta / 200.0);
        else if (_onOrbitRotate is not null)
            _onOrbitRotate(delta / -50.0, 0.0);
        else
            controller.Rotate(delta / -50.0, 0.0, 0.0);

        controller.MouseMove(pos, modifierKeys);
    }

    public void MouseDown(Point pos, MouseButton changedButton, int clickCount, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        var wasPressed = _wasPressed;
        var pressed = HasActiveMouseButton(mouseDevice)
            || changedButton is MouseButton.Left or MouseButton.Right or MouseButton.Middle;
        _wasPressed = pressed;
        if (pressed || wasPressed) NotifyInteraction(pressed);

        _lastRotatePosition = changedButton == MouseButton.Left && _onOrbitRotate is not null ? pos : null;

        // 左键只用于拖动旋转，不触发 Macad 的选择/框选，因此不调用 ViewportController.MouseDown；同时同步指针位置供右键平移起算。
        ResolveController()?.MouseMove(pos, modifierKeys);
    }

    public void MouseUp(Point pos, MouseButton changedButton, MouseDevice mouseDevice, ModifierKeys modifierKeys)
    {
        var wasPressed = _wasPressed;
        var stillPressed = mouseDevice is not null && changedButton switch
        {
            MouseButton.Left => mouseDevice.RightButton == MouseButtonState.Pressed || mouseDevice.MiddleButton == MouseButtonState.Pressed,
            MouseButton.Right => mouseDevice.LeftButton == MouseButtonState.Pressed || mouseDevice.MiddleButton == MouseButtonState.Pressed,
            MouseButton.Middle => mouseDevice.LeftButton == MouseButtonState.Pressed || mouseDevice.RightButton == MouseButtonState.Pressed,
            _ => HasActiveMouseButton(mouseDevice),
        };
        _wasPressed = stillPressed;
        if (wasPressed || stillPressed) NotifyInteraction(stillPressed);

        _lastRotatePosition = null;
        // 松开按键后回到普通悬停状态，让控制器结束平移。
        ResolveController()?.MouseMove(pos, modifierKeys);
    }

    public void Cancel()
    {
        var wasPressed = _wasPressed;
        _wasPressed = false;
        _lastRotatePosition = null;
        // ViewportPanel.OnMouseLeave 在指针移出视口时会无条件调用 Cancel；仅在曾按住按键时通知结束交互，避免空闲划过边界误暂停自转。
        if (wasPressed) NotifyInteraction(false);
        ResolveController()?.MouseMove(new Point(-1.0, -1.0));
    }

    static bool HasActiveMouseButton(MouseDevice? mouseDevice)
        => mouseDevice is not null
           && (mouseDevice.LeftButton == MouseButtonState.Pressed
               || mouseDevice.RightButton == MouseButtonState.Pressed
               || mouseDevice.MiddleButton == MouseButtonState.Pressed);

    void NotifyInteraction(bool pointerHeld) => _onPointerInteraction?.Invoke(pointerHeld);

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
