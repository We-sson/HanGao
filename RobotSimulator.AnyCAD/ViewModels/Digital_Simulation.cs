using CommunityToolkit.Mvvm.ComponentModel;
using RobotSimulator.Controls;
using RobotSimulator.Models;

namespace RobotSimulator.ViewModels
{
    /// <summary>三维模型控件的全部状态；宿主主 VM 只暴露这一个属性。</summary>
    public sealed class Digital_Simulation : ObservableObject, IRobotViewerModel
    {
        private double[] _joints = [];
        private string _statusMessage = "正在加载三维模型…";

        /// <summary>把固定搭配转换为控件参数。</summary>
        public Digital_Simulation(DigitalSimulationConfiguration simulation)
        {
            Options = CreateOptions(simulation);
        }

        /// <summary>固定机器人、Tool 和工装组合。</summary>
        public RobotViewerOptions Options { get; }

        /// <summary>J1～J6 角度；通信层赋值新数组后自动推送到控件。</summary>
        public double[] Joints
        {
            get => _joints;
            set => SetProperty(ref _joints, value);
        }

        IReadOnlyList<double>? IRobotViewerModel.Joints => Joints;

        /// <summary>加载状态文本，由控件更新。</summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>把配置中的枚举和显示开关映射成控件参数。</summary>
        private static RobotViewerOptions CreateOptions(DigitalSimulationConfiguration simulation) => new()
        {
            Robot = simulation.Robot_Model,
            Tool = simulation.Tool_Model,
            Work = simulation.Work_Model,
            ShowBaseCoordinates = simulation.Show_Base_Axes,
            ShowFlangeCoordinates = simulation.Show_Flange_Axes,
            ShowTcpCoordinates = simulation.Show_Tcp_Axes,
            OrbitSpeedDegreesPerSecond = simulation.Orbit_Speed,
            OrbitStartAngleDegrees = simulation.Orbit_Angle,
            OrbitElevationDegrees = simulation.Orbit_Elevation,
        };
    }
}
