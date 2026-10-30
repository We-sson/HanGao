using RobotSimulator.Models;
using RobotSimulator.ViewModels;

namespace RobotSimulator.Services;

/// <summary>界面可选择的标准相机视角；不改变机器人关节角度。</summary>
public enum RobotView
{
    /// <summary>同时观察三个方向的等轴测视图。</summary>
    Isometric,
    /// <summary>从正面观察机器人。</summary>
    Front,
    /// <summary>从上方观察机器人。</summary>
    Top
}

/// <summary>ViewModel 操作三维场景的接口，隔开界面状态与 AnyCAD 控件，便于独立测试。</summary>
public interface IRobotSceneService : IDisposable
{
    /// <summary>初始化背景、视角立方体等显示设置；只在渲染控件就绪后调用。</summary>
    void InitializeViewport();
    /// <summary>只加载机器人基座及六轴，固定参数只能来自内置机器人配置。</summary>
    Task LoadRobotAsync(RobotConfiguration configuration, string modelId, IProgress<RobotLoadProgress>? progress = null);
    /// <summary>独立加载并替换 Tool，不重新读取机器人，也不重置关节姿态。</summary>
    Task LoadToolAsync(string toolId, ToolConfiguration configuration, IProgress<RobotLoadProgress>? progress = null);
    /// <summary>批量应用全部关节角度，最后统一刷新一帧，供复位和动画使用。</summary>
    void ApplyAll(IEnumerable<JointViewModel> joints);
    /// <summary>应用单个关节的角度或轴参数变化，同时让所有下游零件随动。</summary>
    void ApplyJoint(JointViewModel joint);
    /// <summary>独立控制基坐标、法兰和 TCP 箭头的显示，不改变模型位置。</summary>
    void SetCoordinateVisibility(bool showBase, bool showFlange, bool showTcp);
    /// <summary>更新相对于法兰的 TCP 坐标轴；不重新解析或移动 Tool 几何。</summary>
    void SetToolTcp(PoseConfiguration pose);
    /// <summary>切换标准相机视角，并将整个模型适配到窗口。</summary>
    void SetView(RobotView view);
    /// <summary>调整相机缩放，使机器人整体位于视图内。</summary>
    void ZoomAll();
}
