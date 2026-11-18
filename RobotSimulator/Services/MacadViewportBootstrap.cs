using System;
using System.Windows;
using System.Windows.Media;
using Macad.Core.Topology;
using Macad.Interaction;

namespace RobotSimulator.Services;

/// <summary>
/// 为宿主 WPF 应用准备 Macad 交互上下文。三维视口（ViewportPanel）依赖全局单例
/// <see cref="InteractiveContext.Current"/>，MES 与独立仿真器都通过 RobotViewer 使用视口，
/// 因此这里只在首次需要时初始化一次，宿主 App.xaml 不需要合并 Macad 的资源字典。
/// </summary>
internal static class MacadViewportBootstrap
{
    static SimulatorInteractiveContext? _context;

    internal static InteractiveContext EnsureContext()
    {
        if (_context is not null) return _context;

        var application = Application.Current
            ?? throw new InvalidOperationException("Macad 三维视口需要 WPF Application。");
        EnsureBrushes(application);

        var context = new SimulatorInteractiveContext();
        var parameters = context.Parameters.Get<ViewportParameterSet>();
        parameters.ShowViewCube = false;
        parameters.ShowTrihedron = false;
        context.Document = new Model();
        context.Workspace.GridEnabled = false;
        _context = context;
        return context;
    }

    internal static void Shutdown()
    {
        var context = _context;
        _context = null;
        context?.Dispose();
    }

    static void EnsureBrushes(Application application)
    {
        var resources = application.Resources;
        SetBrush(resources, "Macad.Brushes.Window.Background", 0xCB, 0xCB, 0xCB);
        SetBrush(resources, "Macad.Brushes.HudElement.Border", 0x00, 0x00, 0x00);
        SetBrush(resources, "Macad.Brushes.HudElement.Background", 0x3B, 0x42, 0x4F);
        SetBrush(resources, "Macad.Brushes.Accent", 0xFE, 0xCA, 0x34);
    }

    static void SetBrush(ResourceDictionary resources, string key, byte r, byte g, byte b)
    {
        if (resources.Contains(key)) return;
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        resources[key] = brush;
    }

    sealed class SimulatorInteractiveContext : InteractiveContext;
}
