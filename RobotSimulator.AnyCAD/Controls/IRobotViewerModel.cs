using System.ComponentModel;
using RobotSimulator.Models;

namespace RobotSimulator.Controls;

/// <summary>三维控件所需的宿主数据：固定搭配、关节角度和加载状态。</summary>
public interface IRobotViewerModel : INotifyPropertyChanged
{
    /// <summary>固定机器人、Tool 和工装组合。</summary>
    RobotViewerOptions? Options { get; }

    /// <summary>J1～J6 角度，单位度。</summary>
    IReadOnlyList<double>? Joints { get; }

    /// <summary>加载状态文本，由控件更新。</summary>
    string StatusMessage { get; set; }
}
