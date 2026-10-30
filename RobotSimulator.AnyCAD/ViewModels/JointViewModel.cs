using CommunityToolkit.Mvvm.ComponentModel;
using RobotSimulator.Models;

namespace RobotSimulator.ViewModels;

/// <summary>单轴运行状态；只有当前角度可编辑，行程、参考角和轴线来自内置固定参数。</summary>
public sealed class JointViewModel : ObservableObject
{
    /// <summary>开发时确定的只读关节参数。</summary>
    private readonly JointConfiguration _configuration;
    /// <summary>当前示教器角度，单位度。</summary>
    private double _angle;

    /// <summary>读取固定参数并将当前角度设为初始角，index 从 0 开始。</summary>
    public JointViewModel(int index, JointConfiguration configuration)
    {
        Index = index;
        _configuration = configuration;
        _angle = configuration.HomeAngle;
    }

    /// <summary>当前角度改变时通知主 ViewModel 更新三维场景。</summary>
    public event EventHandler? StateChanged;
    /// <summary>0～5 的关节索引，与场景六轴顺序一致。</summary>
    public int Index { get; }
    /// <summary>模型关节名称，例如 J1。</summary>
    public string Name => _configuration.Name;
    /// <summary>示教器轴名称，例如 A1。</summary>
    public string ControllerAxis => $"A{Index + 1}";
    /// <summary>便于对应模型与示教器的显示名称。</summary>
    public string DisplayName => $"{ControllerAxis} / {Name}";
    /// <summary>内嵌关节模型文件名。</summary>
    public string ModelFile => _configuration.ModelFile;
    /// <summary>固定最小角度，单位度。</summary>
    public double Minimum => _configuration.Minimum;
    /// <summary>固定最大角度，单位度。</summary>
    public double Maximum => _configuration.Maximum;
    /// <summary>固定初始与复位角度，单位度。</summary>
    public double HomeAngle => _configuration.HomeAngle;
    /// <summary>原始 CAD 姿态的参考角，单位度，运行时不可校准。</summary>
    public double ModelReferenceAngle => _configuration.ModelReferenceAngle;
    /// <summary>固定旋转轴参考点的全局 X 坐标，单位 mm。</summary>
    public double AxisOriginX => _configuration.AxisOrigin.X;
    /// <summary>固定旋转轴参考点的全局 Y 坐标，单位 mm。</summary>
    public double AxisOriginY => _configuration.AxisOrigin.Y;
    /// <summary>固定旋转轴参考点的全局 Z 坐标，单位 mm。</summary>
    public double AxisOriginZ => _configuration.AxisOrigin.Z;
    /// <summary>固定旋转方向的 X 分量。</summary>
    public double AxisDirectionX => _configuration.AxisDirection.X;
    /// <summary>固定旋转方向的 Y 分量。</summary>
    public double AxisDirectionY => _configuration.AxisDirection.Y;
    /// <summary>固定旋转方向的 Z 分量。</summary>
    public double AxisDirectionZ => _configuration.AxisDirection.Z;

    /// <summary>滑块和输入框可修改的示教器角度，限制在固定行程内并拒绝非有限数值。</summary>
    public double Angle
    {
        get => _angle;
        set
        {
            if (!double.IsFinite(value)) return;
            if (SetProperty(ref _angle, Math.Clamp(value, Minimum, Maximum)))
                StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
