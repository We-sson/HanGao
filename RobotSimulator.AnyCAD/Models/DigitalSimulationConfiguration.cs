using CommunityToolkit.Mvvm.ComponentModel;
using RobotSimulator.Services;

namespace RobotSimulator.Models
{
    /// <summary>
    /// 数字仿真模型的固定搭配；随宿主配置保存，软件启动时据此初始化三维控件。
    /// </summary>
    [Serializable]
    public class DigitalSimulationConfiguration : ObservableObject
    {
        private RobotModelKind _robotModel = RobotModelKind.KR_20_R1810_2;
        private ToolKind _toolModel = ToolKind.Default;
        private WorkKind _workModel = WorkKind.RSideWelding;
        private bool _showBaseAxes = true;
        private bool _showFlangeAxes = true;
        private bool _showTcpAxes = true;
        private double _orbitSpeed = 15;
        private double _orbitAngle = -45;
        private double _orbitElevation = 35.264;
        private double _jointResponseFrequency = JointMotion.DefaultResponseFrequencyHz;
        private double _jointMaxSpeed = JointMotion.DefaultMaxSpeedDegreesPerSecond;
        private double _jointMaxAcceleration = JointMotion.DefaultMaxAccelerationDegreesPerSecondSquared;

        /// <summary>机器人型号，对应控件内的枚举。</summary>
        public RobotModelKind Robot_Model { get => _robotModel; set => SetProperty(ref _robotModel, value); }

        /// <summary>Tool 型号；未配置时使用第一个。</summary>
        public ToolKind Tool_Model { get => _toolModel; set => SetProperty(ref _toolModel, value); }

        /// <summary>工装型号；未配置时使用第一个。</summary>
        public WorkKind Work_Model { get => _workModel; set => SetProperty(ref _workModel, value); }

        /// <summary>是否显示机器人基坐标箭头。</summary>
        public bool Show_Base_Axes { get => _showBaseAxes; set => SetProperty(ref _showBaseAxes, value); }

        /// <summary>是否显示法兰坐标箭头。</summary>
        public bool Show_Flange_Axes { get => _showFlangeAxes; set => SetProperty(ref _showFlangeAxes, value); }

        /// <summary>是否显示 TCP 坐标箭头。</summary>
        public bool Show_Tcp_Axes { get => _showTcpAxes; set => SetProperty(ref _showTcpAxes, value); }

        /// <summary>相机环绕速度，度/秒；0 表示不旋转，默认加载完成后缓慢旋转。</summary>
        public double Orbit_Speed { get => _orbitSpeed; set => SetProperty(ref _orbitSpeed, value); }

        /// <summary>相机环绕起始方位角，单位度；-45 对应标准等轴测方向。</summary>
        public double Orbit_Angle { get => _orbitAngle; set => SetProperty(ref _orbitAngle, value); }

        /// <summary>相机环绕仰角，单位度；默认值对应标准等轴测方向。</summary>
        public double Orbit_Elevation { get => _orbitElevation; set => SetProperty(ref _orbitElevation, value); }

        /// <summary>关节跟随响应频率，单位 Hz；对应 RobotViewer 的 JointResponseFrequencyHz。</summary>
        public double Joint_Response_Frequency { get => _jointResponseFrequency; set => SetProperty(ref _jointResponseFrequency, value); }

        /// <summary>关节跟随最大显示速度，单位度/秒；对应 RobotViewer 的 JointMaxSpeedDegreesPerSecond。</summary>
        public double Joint_Max_Speed { get => _jointMaxSpeed; set => SetProperty(ref _jointMaxSpeed, value); }

        /// <summary>关节跟随最大显示加速度，单位度/秒²；对应 RobotViewer 的 JointMaxAccelerationDegreesPerSecondSquared。</summary>
        public double Joint_Max_Acceleration { get => _jointMaxAcceleration; set => SetProperty(ref _jointMaxAcceleration, value); }
    }
}
