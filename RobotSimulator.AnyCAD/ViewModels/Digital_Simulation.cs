using CommunityToolkit.Mvvm.ComponentModel;
using RobotSimulator.Controls;
using RobotSimulator.Models;

namespace RobotSimulator.ViewModels
{
    /// <summary>三维模型控件的全部状态；宿主主 VM 只暴露这一个属性。</summary>
    public sealed class Digital_Simulation : ObservableObject, IRobotViewerModel
    {
        private double[] _joints = [];
        private readonly double[] _homeJoints;
        private string _statusMessage = "正在加载三维模型…";

        /// <summary>把固定搭配转换为控件参数，并缓存复位用的初始角。</summary>
        public Digital_Simulation(DigitalSimulationConfiguration simulation)
        {
            Options = CreateOptions(simulation);
            _homeJoints = LoadHomeJoints(simulation.Robot_Model);
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

        /// <summary>把 KUKA 报文中的 J1～J6 角度推送到三维控件。</summary>
        public void SetJoints(IReadOnlyList<double> degrees)
        {
            if (degrees.Count < 6) return;
            if (_joints.Length == 6
                && _joints[0] == degrees[0] && _joints[1] == degrees[1]
                && _joints[2] == degrees[2] && _joints[3] == degrees[3]
                && _joints[4] == degrees[4] && _joints[5] == degrees[5])
                return;
            Joints = [degrees[0], degrees[1], degrees[2], degrees[3], degrees[4], degrees[5]];
        }

        /// <summary>没有关节报文时回到所选机器人的初始位。</summary>
        public void ResetJointsToHome()
        {
            if (_joints.Length == 6 && _joints.SequenceEqual(_homeJoints)) return;
            Joints = (double[])_homeJoints.Clone();
        }

        /// <summary>读取内嵌机器人配置的初始角；模型异常由控件加载流程统一提示。</summary>
        private static double[] LoadHomeJoints(RobotModelKind model)
        {
            try
            {
                return RobotConfigurationStore.LoadBuiltIn(ModelCatalog.GetId(model))
                    .Joints.Select(joint => joint.HomeAngle)
                    .ToArray();
            }
            catch
            {
                return [0, 0, 0, 0, 0, 0];
            }
        }

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
