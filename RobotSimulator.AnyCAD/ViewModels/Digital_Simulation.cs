using System.ComponentModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using RobotSimulator.Controls;
using RobotSimulator.Models;

namespace RobotSimulator.ViewModels
{
    /// <summary>三维模型控件的全部状态；宿主主 VM 只暴露这一个属性。</summary>
    public sealed class Digital_Simulation : ObservableObject, IRobotViewerModel
    {
        private readonly DigitalSimulationConfiguration _simulation;
        private readonly RobotViewerOptions _loadOptions;
        private double[] _joints = [];
        private readonly double[] _homeJoints;
        private string _statusMessage = "正在加载三维模型…";
        private RobotViewerOptions _options;
        /// <summary>机器人激光输出状态；未收到报文前保持显示，收到后由通信实时驱动。</summary>
        private bool _weldActive = true;
        /// <summary>串行化最新目标与排队标记，通信线程和界面线程不会互相覆盖。</summary>
        private readonly object _jointSync = new();
        /// <summary>后台线程留下的最新目标；界面线程刷新时取走。</summary>
        private double[]? _pendingJoints;
        /// <summary>是否已有一次后台优先级刷新在排队，同一时刻只排一个。</summary>
        private bool _jointUpdateQueued;

        /// <summary>把固定搭配转换为控件参数，并缓存复位用的初始角。</summary>
        public Digital_Simulation(DigitalSimulationConfiguration simulation)
        {
            _simulation = simulation;
            _options = CreateOptions(simulation);
            _loadOptions = _options;
            _homeJoints = LoadHomeJoints(simulation.Robot_Model);
            // 配置变化即时通知控件；模型类变化由宿主保存时重建控件模型。
            simulation.PropertyChanged += OnSimulationChanged;
        }

        /// <summary>最近一次交给控件加载的模型组合；配置实时变化不会改写它，供宿主判断是否需要重建。</summary>
        public RobotViewerOptions LoadOptions => _loadOptions;

        /// <summary>固定机器人、Tool 和工装组合。</summary>
        public RobotViewerOptions Options
        {
            get => _options;
            private set => SetProperty(ref _options, value);
        }

        /// <summary>机器人激光输出状态；与配置中的显示开关共同决定焊接光是否可见。</summary>
        public bool WeldActive
        {
            get => _weldActive;
            set
            {
                if (!SetProperty(ref _weldActive, value)) return;
                Options = CreateOptions(_simulation);
            }
        }

        /// <summary>解除配置订阅；宿主替换模型前调用，避免旧对象继续响应。</summary>
        public void Detach() => _simulation.PropertyChanged -= OnSimulationChanged;

        /// <summary>配置变化时刷新参数快照；控件直接读取 Options，不再逐项转发。</summary>
        private void OnSimulationChanged(object? sender, PropertyChangedEventArgs e)
        {
            Options = CreateOptions(_simulation);
        }

        /// <summary>J1～J6 角度；通信层赋值新数组后自动推送到控件。</summary>
        public double[] Joints
        {
            get => _joints;
            set => SetProperty(ref _joints, value);
        }

        IReadOnlyList<double>? IRobotViewerModel.Joints => Joints;

        /// <summary>把 KUKA 报文中的 J1～J6 角度推送到三维控件；可从任意线程调用。</summary>
        public void SetJoints(IReadOnlyList<double> degrees)
        {
            if (degrees.Count < 6) return;
            Publish([degrees[0], degrees[1], degrees[2], degrees[3], degrees[4], degrees[5]]);
        }

        /// <summary>没有关节报文时回到所选机器人的初始位；可从任意线程调用。</summary>
        public void ResetJointsToHome() => Publish((double[])_homeJoints.Clone());

        /// <summary>
        /// 界面线程调用时同步生效；后台线程（通信回调）只保存最新目标并合并成一次后台优先级刷新，
        /// 不再让通信线程等界面。没有 WPF 调度器（单元测试、退出阶段）时直接赋值。
        /// </summary>
        private void Publish(double[] joints)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher is null || dispatcher.HasShutdownStarted || dispatcher.CheckAccess())
            {
                lock (_jointSync) _pendingJoints = null;
                ApplyJoints(joints);
                return;
            }

            lock (_jointSync)
            {
                _pendingJoints = joints;
                if (_jointUpdateQueued) return;
                _jointUpdateQueued = true;
            }

            try
            {
                // 仿真刷新让出界面队列给通信和输入；重复帧只保留最后值。
                dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(FlushJoints));
            }
            catch (InvalidOperationException)
            {
                // 调度器正在退出：丢弃该帧并释放排队标记，异常不能回到 Socket 线程。
                lock (_jointSync) _jointUpdateQueued = false;
            }
        }

        /// <summary>在界面线程取走最新目标；期间到达的帧已经合并进同一份目标。</summary>
        private void FlushJoints()
        {
            double[]? joints;
            lock (_jointSync)
            {
                _jointUpdateQueued = false;
                joints = _pendingJoints;
                _pendingJoints = null;
            }
            if (joints is not null) ApplyJoints(joints);
        }

        /// <summary>写入 Joints；角度未变化时不触发绑定刷新。</summary>
        private void ApplyJoints(double[] joints)
        {
            if (_joints.Length == 6
                && _joints[0] == joints[0] && _joints[1] == joints[1]
                && _joints[2] == joints[2] && _joints[3] == joints[3]
                && _joints[4] == joints[4] && _joints[5] == joints[5])
                return;
            Joints = joints;
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
        private RobotViewerOptions CreateOptions(DigitalSimulationConfiguration simulation) => new()
        {
            Robot = simulation.Robot_Model,
            Tool = simulation.Tool_Model,
            Work = simulation.Work_Model,
            ShowBaseCoordinates = simulation.Show_Base_Axes,
            ShowFlangeCoordinates = simulation.Show_Flange_Axes,
            ShowTcpCoordinates = simulation.Show_Tcp_Axes,
            ShowWeldBeam = simulation.Show_Weld_Beam && _weldActive,
            OrbitSpeedDegreesPerSecond = simulation.Orbit_Speed,
            OrbitStartAngleDegrees = simulation.Orbit_Angle,
            OrbitElevationDegrees = simulation.Orbit_Elevation,
            JointResponseFrequencyHz = simulation.Joint_Response_Frequency,
            JointMaxSpeedDegreesPerSecond = simulation.Joint_Max_Speed,
            JointMaxAccelerationDegreesPerSecondSquared = simulation.Joint_Max_Acceleration,
            JointApplyRateHz = simulation.Joint_Apply_Rate,
        };
    }
}
