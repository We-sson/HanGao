namespace HanGao.ModbusTcp;

/// <summary>
/// 作为一个原子操作发布的不可变寄存器数据。
/// </summary>
/// <remarks>
/// 构造时复制调用方数组，之后仅暴露只读内存。这样业务层可以继续复用自己的缓冲区，
/// 而不会在服务发布过程中意外修改正在读取的数据。
/// </remarks>
public sealed class ModbusRegisterSnapshot
{
    // 私有数组只在构造函数中写入，发布期间不再变化。
    private readonly ushort[] _registers;

    /// <summary>
    /// 创建快照并复制传入的全部寄存器值。
    /// </summary>
    /// <param name="startAddress">Modbus 报文中的零基起始地址。</param>
    /// <param name="registers">需要连续发布的 16 位寄存器。</param>
    /// <param name="sequence">由业务层提供的单调递增流水号。</param>
    /// <param name="capturedAtUtc">业务值的 UTC 采集时间；省略时使用当前 UTC 时间。</param>
    public ModbusRegisterSnapshot(
        ushort startAddress,
        ReadOnlySpan<ushort> registers,
        long sequence,
        DateTimeOffset? capturedAtUtc = null)
    {
        if (registers.IsEmpty)
        {
            throw new ArgumentException("寄存器快照至少必须包含一个寄存器。", nameof(registers));
        }

        int endExclusive = startAddress + registers.Length;
        if (endExclusive > 65536)
        {
            throw new ArgumentOutOfRangeException(
                nameof(registers),
                "寄存器快照超出了 Modbus 0～65535 地址空间。");
        }

        StartAddress = startAddress;
        Sequence = sequence;
        CapturedAtUtc = capturedAtUtc ?? DateTimeOffset.UtcNow;
        _registers = registers.ToArray();
    }

    /// <summary>
    /// Modbus 报文中的零基起始地址；例如 SCADA 参考地址 40001 对应此处的 0。
    /// </summary>
    public ushort StartAddress { get; }

    /// <summary>
    /// 调用方提供的发布流水号，用于诊断数据新旧和检查快照一致性。
    /// </summary>
    public long Sequence { get; }

    /// <summary>
    /// 该快照所代表业务数据的 UTC 采集时间。
    /// </summary>
    public DateTimeOffset CapturedAtUtc { get; }

    /// <summary>
    /// 连续寄存器内容；内部数组为构造时的副本，普通调用方无法修改。
    /// </summary>
    public ReadOnlyMemory<ushort> Registers => _registers;
}
