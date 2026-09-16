namespace HanGao.ModbusTcp;

/// <summary>
/// 一个逻辑数值跨越多个 16 位寄存器时的字顺序。
/// </summary>
public enum ModbusWordOrder
{
    /// <summary>高 16 位放在较小寄存器地址，常写作 ABCD。</summary>
    HighWordFirst,

    /// <summary>低 16 位放在较小寄存器地址，常写作 CDAB。</summary>
    LowWordFirst,
}

/// <summary>
/// 在业务整数与 16 位 Modbus 寄存器之间转换的无分配工具。
/// </summary>
/// <remarks>
/// Modbus 规定每个寄存器为 16 位；UInt32/Int32 占两个连续寄存器，UInt64 占四个。
/// 报文字节顺序由协议库处理，本类只负责多个寄存器之间的字顺序，避免把“字节序”和“字序”混为一谈。
/// </remarks>
public static class ModbusRegisterCodec
{
    /// <summary>把一个 UInt16 写入指定寄存器地址。</summary>
    public static void WriteUInt16(Span<ushort> destination, int address, ushort value)
    {
        EnsureRange(destination.Length, address, 1);
        destination[address] = value;
    }

    /// <summary>从指定寄存器地址读取一个 UInt16。</summary>
    public static ushort ReadUInt16(ReadOnlySpan<ushort> source, int address)
    {
        EnsureRange(source.Length, address, 1);
        return source[address];
    }

    /// <summary>按指定字顺序把一个 UInt32 拆成两个连续寄存器。</summary>
    public static void WriteUInt32(
        Span<ushort> destination,
        int address,
        uint value,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst)
    {
        EnsureRange(destination.Length, address, 2);
        EnsureWordOrder(wordOrder);
        ushort high = (ushort)(value >> 16);
        ushort low = (ushort)value;
        WriteWords(destination, address, high, low, wordOrder);
    }

    /// <summary>按指定字顺序把两个连续寄存器组合为 UInt32。</summary>
    public static uint ReadUInt32(
        ReadOnlySpan<ushort> source,
        int address,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst)
    {
        EnsureRange(source.Length, address, 2);
        EnsureWordOrder(wordOrder);
        (ushort high, ushort low) = ReadWords(source, address, wordOrder);
        return ((uint)high << 16) | low;
    }

    /// <summary>保留二进制补码位模式，将 Int32 写入两个连续寄存器。</summary>
    public static void WriteInt32(
        Span<ushort> destination,
        int address,
        int value,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst) =>
        WriteUInt32(destination, address, unchecked((uint)value), wordOrder);

    /// <summary>从两个连续寄存器读取二进制补码 Int32。</summary>
    public static int ReadInt32(
        ReadOnlySpan<ushort> source,
        int address,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst) =>
        unchecked((int)ReadUInt32(source, address, wordOrder));

    /// <summary>按指定字顺序把一个 UInt64 拆成四个连续寄存器。</summary>
    public static void WriteUInt64(
        Span<ushort> destination,
        int address,
        ulong value,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst)
    {
        EnsureRange(destination.Length, address, 4);
        EnsureWordOrder(wordOrder);
        Span<ushort> words = stackalloc ushort[4]
        {
            (ushort)(value >> 48),
            (ushort)(value >> 32),
            (ushort)(value >> 16),
            (ushort)value,
        };

        if (wordOrder == ModbusWordOrder.LowWordFirst)
        {
            words.Reverse();
        }

        words.CopyTo(destination[address..]);
    }

    /// <summary>按指定字顺序把四个连续寄存器组合为 UInt64。</summary>
    public static ulong ReadUInt64(
        ReadOnlySpan<ushort> source,
        int address,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst)
    {
        EnsureRange(source.Length, address, 4);
        EnsureWordOrder(wordOrder);
        Span<ushort> words = stackalloc ushort[4];
        source.Slice(address, 4).CopyTo(words);

        if (wordOrder == ModbusWordOrder.LowWordFirst)
        {
            words.Reverse();
        }

        return ((ulong)words[0] << 48)
             | ((ulong)words[1] << 32)
             | ((ulong)words[2] << 16)
             | words[3];
    }

    /// <summary>根据字顺序写入一个高字和一个低字。</summary>
    private static void WriteWords(
        Span<ushort> destination,
        int address,
        ushort high,
        ushort low,
        ModbusWordOrder wordOrder)
    {
        if (wordOrder == ModbusWordOrder.HighWordFirst)
        {
            destination[address] = high;
            destination[address + 1] = low;
        }
        else
        {
            destination[address] = low;
            destination[address + 1] = high;
        }
    }

    /// <summary>根据字顺序读取并归一化为“高字、低字”。</summary>
    private static (ushort High, ushort Low) ReadWords(
        ReadOnlySpan<ushort> source,
        int address,
        ModbusWordOrder wordOrder) =>
        wordOrder == ModbusWordOrder.HighWordFirst
            ? (source[address], source[address + 1])
            : (source[address + 1], source[address]);

    /// <summary>在切片前统一检查缓冲区范围，防止整数值跨出寄存器块。</summary>
    private static void EnsureRange(int bufferLength, int address, int wordCount)
    {
        if (address < 0 || wordCount < 0 || address > bufferLength - wordCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(address),
                address,
                $"该数值需要 {wordCount} 个寄存器，但缓冲区长度只有 {bufferLength}。");
        }
    }

    /// <summary>拒绝强制转换产生的未知枚举值，避免不同方法采用不一致的默认解释。</summary>
    private static void EnsureWordOrder(ModbusWordOrder wordOrder)
    {
        if (!Enum.IsDefined(wordOrder))
        {
            throw new ArgumentOutOfRangeException(nameof(wordOrder), wordOrder, "未知的 Modbus 字顺序。");
        }
    }
}
