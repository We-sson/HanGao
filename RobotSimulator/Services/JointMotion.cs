namespace RobotSimulator.Services;

/// <summary>
/// 六轴显示的临界阻尼速度控制；通信跟随和示例动作共用同一套速度、加速度限制。
/// 三维视口只负责渲染，关节插值在这里按速度/加速度控制，帧推进仍由 WPF 渲染事件驱动。
/// </summary>
public static class JointMotion
{
    /// <summary>默认响应频率，单位 Hz。</summary>
    public const double DefaultResponseFrequencyHz = 2.0;
    /// <summary>默认最大显示速度，单位度/秒。</summary>
    public const double DefaultMaxSpeedDegreesPerSecond = 360;
    /// <summary>默认最大显示加速度，单位度/秒²。</summary>
    public const double DefaultMaxAccelerationDegreesPerSecondSquared = 1440;
    /// <summary>默认到达容差，单位度。</summary>
    public const double DefaultToleranceDegrees = 0.02;
    /// <summary>默认三维位置刷新频率，单位 Hz；渲染仍按界面帧率进行。</summary>
    public const double DefaultApplyRateHz = 20;
    /// <summary>到达目标时的速度判定，单位度/秒。</summary>
    private const double StopSpeedDegreesPerSecond = 0.5;

    /// <summary>按一帧时间推进角度和速度；目标始终从当前位置连续逼近，不瞬移。</summary>
    public static void Step(
        ref double angle,
        ref double velocity,
        double target,
        double deltaSeconds,
        double frequencyHz = DefaultResponseFrequencyHz,
        double maxSpeed = DefaultMaxSpeedDegreesPerSecond,
        double maxAcceleration = DefaultMaxAccelerationDegreesPerSecondSquared,
        double tolerance = DefaultToleranceDegrees)
    {
        if (!double.IsFinite(target) || !double.IsFinite(deltaSeconds)
            || !double.IsFinite(frequencyHz) || !double.IsFinite(maxSpeed)
            || !double.IsFinite(maxAcceleration) || !double.IsFinite(tolerance)
            || deltaSeconds <= 0 || maxSpeed <= 0 || maxAcceleration <= 0)
        {
            return;
        }

        // 有效帧到达后也能恢复旧版本留下的非有限插值状态。
        if (!double.IsFinite(angle)) angle = target;
        if (!double.IsFinite(velocity)) velocity = 0;

        var difference = target - angle;
        if (Math.Abs(difference) <= tolerance && Math.Abs(velocity) <= StopSpeedDegreesPerSecond)
        {
            angle = target;
            velocity = 0;
            return;
        }

        var omega = 2 * Math.PI * Math.Max(frequencyHz, 0.1);
        var acceleration = omega * omega * difference - 2 * omega * velocity;
        acceleration = Math.Clamp(acceleration, -maxAcceleration, maxAcceleration);
        velocity = Math.Clamp(velocity + acceleration * deltaSeconds, -maxSpeed, maxSpeed);
        angle += velocity * deltaSeconds;

        if (Math.Abs(target - angle) <= tolerance && Math.Abs(velocity) <= StopSpeedDegreesPerSecond)
        {
            angle = target;
            velocity = 0;
        }
    }
}
