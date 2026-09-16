namespace HanGao.ModbusTcp;

/// <summary>
/// Robot_Info_Mes 向 SCADA 发布的固定保持寄存器表。
/// </summary>
/// <remarks>
/// 所有 Address 常量都是 Modbus 报文中的零基地址；SCADA 参考地址等于 40001 加 Address。
/// 一个 Modbus 寄存器始终是 16 位。年度累计值使用两个连续寄存器组成 UInt32，默认高字在前。
/// SCADA 应一次读取 40001～40018，以获得同一份快照。
/// </remarks>
public static class RobotInfoRegisterMap
{
    /// <summary>SCADA 中本数据块的第一个参考地址。</summary>
    public const int FirstReferenceAddress = 40001;

    /// <summary>Modbus 报文中的数据块起始地址；40001 在报文中等于地址 0。</summary>
    public const ushort StartAddress = 0;

    /// <summary>当前寄存器表版本。调整字段含义或不兼容布局时必须递增。</summary>
    public const ushort CurrentMapVersion = 1;

    /// <summary>整个数据块占用的 16 位寄存器数量。</summary>
    public const int RegisterCount = 18;

    /// <summary>40001：当天运行时间，单位分钟，UInt16。</summary>
    public const ushort DailyRunMinutesAddress = 0;

    /// <summary>40002～40003：累计运行时间，单位秒，UInt32。</summary>
    public const ushort TotalRunSecondsAddress = 1;

    /// <summary>40004～40005：累计停机时间，单位分钟，UInt32。</summary>
    public const ushort DowntimeMinutesAddress = 3;

    /// <summary>40006～40007：当前单次停机时间，单位分钟，UInt32。</summary>
    public const ushort CurrentDowntimeMinutesAddress = 5;

    /// <summary>40008：设备状态码，UInt16；状态枚举由 Robot_Info_Mes 业务层统一定义。</summary>
    public const ushort StatusCodeAddress = 7;

    /// <summary>40009：当天产量，UInt16。</summary>
    public const ushort DailyProductionAddress = 8;

    /// <summary>40010～40011：跨日累计产量，UInt32。</summary>
    public const ushort TotalProductionAddress = 9;

    /// <summary>40012～40013：本次上电后运行时间，单位分钟，UInt32。</summary>
    public const ushort PowerOnMinutesAddress = 11;

    /// <summary>40014～40015：数据更新时间，UTC Unix 秒，UInt32。</summary>
    public const ushort UpdatedAtUnixSecondsAddress = 13;

    /// <summary>40016：寄存器表版本，UInt16。</summary>
    public const ushort MapVersionAddress = 15;

    /// <summary>40017～40018：快照流水号低 32 位，UInt32。</summary>
    public const ushort SnapshotSequenceAddress = 16;

    /// <summary>
    /// 把一组业务值编码为从协议地址 0 开始的不可变寄存器快照。
    /// </summary>
    /// <param name="values">已经完成单位换算和范围检查的业务值。</param>
    /// <param name="sequence">单调递增的发布流水号；寄存器保存其低 32 位。</param>
    /// <param name="wordOrder">32 位字段占用两个寄存器时的字顺序。</param>
    /// <returns>可直接传给 <see cref="IModbusTcpServer.Publish"/> 的完整快照。</returns>
    /// <remarks>
    /// 编码过程先在私有数组中完成，再构造不可变快照；服务器随后在同一把锁内整体复制，
    /// 因而 SCADA 单次读取整个数据块时不会看到一半新、一半旧的数据。
    /// </remarks>
    public static ModbusRegisterSnapshot CreateSnapshot(
        RobotInfoRegisterValues values,
        long sequence,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst)
    {
        ArgumentNullException.ThrowIfNull(values);

        long unixSeconds = values.UpdatedAtUtc.ToUnixTimeSeconds();
        if (unixSeconds is < uint.MinValue or > uint.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(values),
                values.UpdatedAtUtc,
                "数据更新时间必须位于 UInt32 Unix 秒可表示的范围内。");
        }

        ushort[] registers = new ushort[RegisterCount];
        ModbusRegisterCodec.WriteUInt16(registers, DailyRunMinutesAddress, values.DailyRunMinutes);
        ModbusRegisterCodec.WriteUInt32(registers, TotalRunSecondsAddress, values.TotalRunSeconds, wordOrder);
        ModbusRegisterCodec.WriteUInt32(registers, DowntimeMinutesAddress, values.DowntimeMinutes, wordOrder);
        ModbusRegisterCodec.WriteUInt32(registers, CurrentDowntimeMinutesAddress, values.CurrentDowntimeMinutes, wordOrder);
        ModbusRegisterCodec.WriteUInt16(registers, StatusCodeAddress, (ushort)values.StatusCode);
        ModbusRegisterCodec.WriteUInt16(registers, DailyProductionAddress, values.DailyProduction);
        ModbusRegisterCodec.WriteUInt32(registers, TotalProductionAddress, values.TotalProduction, wordOrder);
        ModbusRegisterCodec.WriteUInt32(registers, PowerOnMinutesAddress, values.PowerOnMinutes, wordOrder);
        ModbusRegisterCodec.WriteUInt32(registers, UpdatedAtUnixSecondsAddress, (uint)unixSeconds, wordOrder);
        ModbusRegisterCodec.WriteUInt16(registers, MapVersionAddress, CurrentMapVersion);
        ModbusRegisterCodec.WriteUInt32(registers, SnapshotSequenceAddress, unchecked((uint)sequence), wordOrder);

        return new ModbusRegisterSnapshot(StartAddress, registers, sequence, values.UpdatedAtUtc);
    }

    /// <summary>把零基协议地址转换成 SCADA 中显示的 4xxxx 参考地址。</summary>
    public static int ToReferenceAddress(ushort protocolAddress) =>
        ModbusAddressConverter.ToReferenceAddress(
            protocolAddress,
            ModbusRegisterArea.HoldingRegisters);
}

/// <summary>
/// 编码到 Robot_Info_Mes 固定寄存器表的一组业务值。
/// </summary>
/// <param name="DailyRunMinutes">当天运行分钟数，最大 65535。</param>
/// <param name="TotalRunSeconds">跨日累计运行秒数，UInt32 可覆盖约 136 年。</param>
/// <param name="DowntimeMinutes">累计停机分钟数，UInt32 可覆盖完整年度。</param>
/// <param name="CurrentDowntimeMinutes">当前一次停机持续分钟数，UInt32 可覆盖超过 45 天的长停机。</param>
/// <param name="StatusCode">设备状态码；枚举数值是与 SCADA 约定的一部分，发布后不能随意重排。</param>
/// <param name="DailyProduction">当天产量。</param>
/// <param name="TotalProduction">跨日累计产量。</param>
/// <param name="PowerOnMinutes">本次上电后的分钟数。</param>
/// <param name="UpdatedAtUtc">本组业务数据的 UTC 采集时间。</param>
public sealed record RobotInfoRegisterValues(
    ushort DailyRunMinutes,
    uint TotalRunSeconds,
    uint DowntimeMinutes,
    uint CurrentDowntimeMinutes,
    RobotInfoStatusCode StatusCode,
    ushort DailyProduction,
    uint TotalProduction,
    uint PowerOnMinutes,
    DateTimeOffset UpdatedAtUtc);

/// <summary>
/// 40008 状态寄存器使用的稳定状态码。
/// </summary>
/// <remarks>
/// 枚举底层类型固定为 UInt16。新增状态时只能追加新数值，不能修改已有数值，
/// 否则旧版 SCADA 画面会把同一个数字解释成另一种状态。
/// </remarks>
public enum RobotInfoStatusCode : ushort
{
    /// <summary>0：尚未取得有效数据或状态无法识别。</summary>
    Unknown = 0,

    /// <summary>1：机器人通信断开。</summary>
    Offline = 1,

    /// <summary>2：通信正常，但当前没有执行生产动作。</summary>
    Standby = 2,

    /// <summary>3：机器人正在自动生产。</summary>
    Running = 3,

    /// <summary>4：机器人处于 T1/T2 等手动调试模式。</summary>
    Manual = 4,

    /// <summary>5：机器人或业务状态报告故障。</summary>
    Fault = 5,
}
