namespace RobotSimulator.Models
{
    /// <summary>
    /// 数字仿真模型的固定搭配；随宿主配置保存，软件启动时据此初始化三维控件。
    /// </summary>
    [Serializable]
    public class DigitalSimulationConfiguration
    {
        /// <summary>机器人型号，对应控件内的枚举。</summary>
        public RobotModelKind Robot_Model { get; set; } = RobotModelKind.KR_20_R1810_2;

        /// <summary>Tool 型号；未配置时使用第一个。</summary>
        public ToolKind Tool_Model { get; set; } = ToolKind.Default;

        /// <summary>工装型号；未配置时使用第一个。</summary>
        public WorkKind Work_Model { get; set; } = WorkKind.RSideWelding;

        /// <summary>是否显示机器人基坐标箭头。</summary>
        public bool Show_Base_Axes { get; set; } = true;

        /// <summary>是否显示法兰坐标箭头。</summary>
        public bool Show_Flange_Axes { get; set; } = true;

        /// <summary>是否显示 TCP 坐标箭头。</summary>
        public bool Show_Tcp_Axes { get; set; } = true;

        /// <summary>相机环绕速度，度/秒；0 表示不旋转，默认加载完成后缓慢旋转。</summary>
        public double Orbit_Speed { get; set; } = 15;

        /// <summary>相机环绕起始方位角，单位度；-45 对应标准等轴测方向。</summary>
        public double Orbit_Angle { get; set; } = -45;

        /// <summary>相机环绕仰角，单位度；默认值对应标准等轴测方向。</summary>
        public double Orbit_Elevation { get; set; } = 35.264;
    }
}
