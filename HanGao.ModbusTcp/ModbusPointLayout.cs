using System.Reflection;

namespace HanGao.ModbusTcp;

/// <summary>
/// 一个逻辑点位在 Modbus 中使用的数据类型。
/// </summary>
/// <remarks>
/// Modbus 物理寄存器始终为 16 位；这里的数据类型决定一个业务值连续占用几个寄存器。
/// </remarks>
public enum ModbusPointDataType
{
    /// <summary>无符号 16 位整数，占用 1 个寄存器，SCADA 常显示为 UDec16。</summary>
    UInt16,

    /// <summary>无符号 32 位整数，占用 2 个连续寄存器，SCADA 常显示为 UDec32。</summary>
    UInt32,
}

/// <summary>
/// 在强类型快照属性上声明默认 Modbus 点位定义。
/// </summary>
/// <remarks>
/// Attribute 只描述稳定的开发契约，不保存运行时地址。实际地址由块起点、顺序和数据类型统一计算，
/// 避免人工同时维护“地址”和“占用长度”造成重叠。
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ModbusPointAttribute : Attribute
{
    /// <summary>创建一个点位定义。</summary>
    /// <param name="order">点位唯一编号及连续寄存器块中的顺序，从 0 开始。</param>
    /// <param name="dataType">决定占用 1 个还是 2 个寄存器。</param>
    public ModbusPointAttribute(int order, ModbusPointDataType dataType)
    {
        if (order < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, "点位顺序不能为负数。");
        }

        if (!Enum.IsDefined(dataType))
        {
            throw new ArgumentOutOfRangeException(nameof(dataType), dataType, "未知的 Modbus 点位数据类型。");
        }

        Order = order;
        DataType = dataType;
    }

    /// <summary>点位唯一编号，同时决定连续寄存器块中的排列顺序。</summary>
    public int Order { get; }

    /// <summary>点位数据类型。</summary>
    public ModbusPointDataType DataType { get; }

    /// <summary>UI 和导出表中显示的中文名称。</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>工程单位，例如“分钟”“秒”“件”。</summary>
    public string Unit { get; init; } = string.Empty;

    /// <summary>提供给现场和二次开发人员的说明。</summary>
    public string Comment { get; init; } = string.Empty;
}

/// <summary>
/// 与 UI、XML 和布局计算解耦的点位定义。
/// </summary>
public sealed record ModbusPointDefinition(
    int Order,
    ModbusPointDataType DataType,
    string DisplayName,
    string Unit,
    string Comment,
    bool Enabled = true);

/// <summary>
/// 从带 <see cref="ModbusPointAttribute"/> 的强类型快照中读取默认点位表。
/// </summary>
public static class ModbusPointCatalog
{
    /// <summary>读取公开实例属性上的点位定义，并检查唯一顺序和 CLR 类型。</summary>
    public static IReadOnlyList<ModbusPointDefinition> FromType<TSnapshot>() =>
        FromType(typeof(TSnapshot));

    /// <summary>读取指定类型的公开实例属性上的点位定义。</summary>
    public static IReadOnlyList<ModbusPointDefinition> FromType(Type snapshotType)
    {
        ArgumentNullException.ThrowIfNull(snapshotType);

        var definitions = new List<ModbusPointDefinition>();
        var orders = new HashSet<int>();

        foreach (PropertyInfo property in snapshotType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            ModbusPointAttribute? attribute = property.GetCustomAttribute<ModbusPointAttribute>(inherit: true);
            if (attribute is null)
            {
                continue;
            }

            Type expectedType = attribute.DataType switch
            {
                ModbusPointDataType.UInt16 => typeof(ushort),
                ModbusPointDataType.UInt32 => typeof(uint),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(attribute.DataType),
                    attribute.DataType,
                    "未知的 Modbus 点位数据类型。"),
            };

            if (property.PropertyType != expectedType)
            {
                throw new InvalidOperationException(
                    $"属性 {snapshotType.Name}.{property.Name} 的 CLR 类型是 {property.PropertyType.Name}，" +
                    $"但 {attribute.DataType} 要求使用 {expectedType.Name}。");
            }

            if (!orders.Add(attribute.Order))
            {
                throw new InvalidOperationException($"点位顺序 {attribute.Order} 重复。");
            }

            definitions.Add(
                new ModbusPointDefinition(
                    attribute.Order,
                    attribute.DataType,
                    string.IsNullOrWhiteSpace(attribute.DisplayName)
                        ? property.Name
                        : attribute.DisplayName,
                    attribute.Unit,
                    attribute.Comment));
        }

        if (definitions.Count == 0)
        {
            throw new InvalidOperationException(
                $"类型 {snapshotType.FullName} 没有声明任何 ModbusPointAttribute。");
        }

        return definitions.OrderBy(item => item.Order).ToArray();
    }

    /// <summary>
    /// 按 Attribute 的 Order 读取一个强类型对象的当前属性值。
    /// </summary>
    /// <remarks>
    /// Order 是唯一通信标识，因此属性名或中文名称变化不会影响寄存器取值。
    /// </remarks>
    public static IReadOnlyDictionary<int, ulong> ReadValues<TSnapshot>(TSnapshot snapshot)
        where TSnapshot : class
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        Type snapshotType = snapshot.GetType();
        _ = FromType(snapshotType);
        var values = new Dictionary<int, ulong>();

        foreach (PropertyInfo property in snapshotType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            ModbusPointAttribute? attribute =
                property.GetCustomAttribute<ModbusPointAttribute>(inherit: true);
            if (attribute is null)
            {
                continue;
            }

            object? rawValue = property.GetValue(snapshot);
            ulong value = rawValue switch
            {
                ushort uint16Value => uint16Value,
                uint uint32Value => uint32Value,
                _ => throw new InvalidOperationException(
                    $"属性 {snapshotType.Name}.{property.Name} 没有可读取的 UInt16/UInt32 值。"),
            };
            values.Add(attribute.Order, value);
        }

        return values;
    }
}

/// <summary>
/// 在六位保持寄存器参考地址与 Modbus 报文零基地址之间转换。
/// </summary>
public static class ModbusHoldingAddressConverter
{
    /// <summary>六位保持寄存器的第一个参考地址。</summary>
    public const int ReferenceBase = 400001;

    /// <summary>
    /// 把 400001～465536 六位参考地址转换为 PDU 中的 0～65535 地址。
    /// </summary>
    public static ushort ToProtocolAddress(int referenceAddress)
    {
        if (referenceAddress is >= ReferenceBase and <= ReferenceBase + ushort.MaxValue)
        {
            return checked((ushort)(referenceAddress - ReferenceBase));
        }

        throw new ArgumentOutOfRangeException(
            nameof(referenceAddress),
            referenceAddress,
            "保持寄存器起点必须使用 400001～465536 六位地址。");
    }

    /// <summary>把零基协议地址转换为六位 SCADA 保持寄存器参考地址。</summary>
    public static int ToReferenceAddress(ushort protocolAddress) =>
        ReferenceBase + protocolAddress;
}

/// <summary>一个已经完成自动排位的运行时点位。</summary>
public sealed record ModbusRegisterPointLayout(
    int Order,
    ModbusPointDataType DataType,
    string DisplayName,
    string Unit,
    string Comment,
    int RelativeAddress,
    ushort ProtocolAddress,
    int ReferenceAddress,
    int RegisterCount);

/// <summary>从一个起始地址开始的完整连续寄存器布局。</summary>
public sealed class ModbusRegisterLayout
{
    internal ModbusRegisterLayout(
        int startReferenceAddress,
        ushort startProtocolAddress,
        int registerCount,
        IReadOnlyList<ModbusRegisterPointLayout> points)
    {
        StartReferenceAddress = startReferenceAddress;
        StartProtocolAddress = startProtocolAddress;
        RegisterCount = registerCount;
        Points = points;
    }

    /// <summary>配置文件中的块起始参考地址。</summary>
    public int StartReferenceAddress { get; }

    /// <summary>块起点在 Modbus PDU 中的零基地址。</summary>
    public ushort StartProtocolAddress { get; }

    /// <summary>连续寄存器数量。</summary>
    public int RegisterCount { get; }

    /// <summary>按最终地址排列的启用点位。</summary>
    public IReadOnlyList<ModbusRegisterPointLayout> Points { get; }
}

/// <summary>
/// 根据唯一块起点和点位类型自动计算全部寄存器地址。
/// </summary>
public static class ModbusRegisterLayoutBuilder
{
    /// <summary>
    /// 构建无地址重叠的连续布局；UInt16 占 1 位，UInt32 紧接当前位置占 2 位。
    /// </summary>
    public static ModbusRegisterLayout Build(
        int startReferenceAddress,
        IEnumerable<ModbusPointDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ushort startProtocolAddress =
            ModbusHoldingAddressConverter.ToProtocolAddress(startReferenceAddress);
        ModbusPointDefinition[] allPoints = definitions.ToArray();
        var uniqueOrders = new HashSet<int>();
        foreach (ModbusPointDefinition definition in allPoints)
        {
            ValidateDefinition(definition);
            if (!uniqueOrders.Add(definition.Order))
            {
                throw new InvalidOperationException(
                    $"点位顺序 {definition.Order} 重复；Order 必须全局唯一。");
            }
        }

        ModbusPointDefinition[] enabledPoints = allPoints
            .Where(item => item.Enabled)
            .OrderBy(item => item.Order)
            .ToArray();

        if (enabledPoints.Length == 0)
        {
            throw new InvalidOperationException("至少需要启用一个 Modbus 点位。");
        }

        var points = new List<ModbusRegisterPointLayout>(enabledPoints.Length);
        int relativeAddress = 0;

        foreach (ModbusPointDefinition definition in enabledPoints)
        {
            int wordCount = GetRegisterCount(definition.DataType);
            int absoluteProtocolAddress = startProtocolAddress + relativeAddress;
            int endExclusive = absoluteProtocolAddress + wordCount;
            if (endExclusive > ushort.MaxValue + 1)
            {
                throw new InvalidOperationException(
                    $"顺序 {definition.Order} 的点位超出了 Modbus 0～65535 地址空间。");
            }

            ushort protocolAddress = checked((ushort)absoluteProtocolAddress);
            points.Add(
                new ModbusRegisterPointLayout(
                    definition.Order,
                    definition.DataType,
                    definition.DisplayName,
                    definition.Unit,
                    definition.Comment,
                    relativeAddress,
                    protocolAddress,
                    ModbusHoldingAddressConverter.ToReferenceAddress(protocolAddress),
                    wordCount));

            relativeAddress += wordCount;
        }

        return new ModbusRegisterLayout(
            startReferenceAddress,
            startProtocolAddress,
            relativeAddress,
            points.AsReadOnly());
    }

    /// <summary>返回一个逻辑值需要的 16 位寄存器数量。</summary>
    public static int GetRegisterCount(ModbusPointDataType dataType) => dataType switch
    {
        ModbusPointDataType.UInt16 => 1,
        ModbusPointDataType.UInt32 => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, "未知的点位数据类型。"),
    };

    private static void ValidateDefinition(ModbusPointDefinition definition)
    {
        if (definition.Order < 0)
        {
            throw new InvalidOperationException("点位顺序不能为负数。");
        }

        _ = GetRegisterCount(definition.DataType);
    }
}

/// <summary>把自动布局和一组业务值编码为原子寄存器快照。</summary>
public static class ModbusRegisterLayoutEncoder
{
    /// <summary>
    /// 创建快照；未提供值、UInt16 溢出或布局错误时直接失败，绝不静默截断。
    /// </summary>
    public static ModbusRegisterSnapshot CreateSnapshot(
        ModbusRegisterLayout layout,
        IReadOnlyDictionary<int, ulong> values,
        long sequence,
        ModbusWordOrder wordOrder = ModbusWordOrder.HighWordFirst,
        DateTimeOffset? capturedAtUtc = null)
    {
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(values);

        ushort[] registers = new ushort[layout.RegisterCount];
        foreach (ModbusRegisterPointLayout point in layout.Points)
        {
            if (!values.TryGetValue(point.Order, out ulong value))
            {
                throw new KeyNotFoundException($"没有为顺序 {point.Order} 的点位提供发布值。");
            }

            switch (point.DataType)
            {
                case ModbusPointDataType.UInt16 when value <= ushort.MaxValue:
                    ModbusRegisterCodec.WriteUInt16(registers, point.RelativeAddress, (ushort)value);
                    break;
                case ModbusPointDataType.UInt16:
                    throw new OverflowException($"顺序 {point.Order} 的值 {value} 超出 UInt16 范围。");
                case ModbusPointDataType.UInt32 when value <= uint.MaxValue:
                    ModbusRegisterCodec.WriteUInt32(
                        registers,
                        point.RelativeAddress,
                        (uint)value,
                        wordOrder);
                    break;
                case ModbusPointDataType.UInt32:
                    throw new OverflowException($"顺序 {point.Order} 的值 {value} 超出 UInt32 范围。");
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(point.DataType),
                        point.DataType,
                        "未知的点位数据类型。");
            }
        }

        return new ModbusRegisterSnapshot(
            layout.StartProtocolAddress,
            registers,
            sequence,
            capturedAtUtc);
    }
}
