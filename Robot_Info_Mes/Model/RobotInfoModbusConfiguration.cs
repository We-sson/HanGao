using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Xml.Serialization;
using HanGao.ModbusTcp;
using PropertyChanged;

namespace Robot_Info_Mes.Model;

/// <summary>
/// Robot_Info_Mes 的 Modbus TCP 本地 XML 配置。
/// </summary>
/// <remarks>
/// 配置只保存寄存器块起点，不保存人工计算的每个地址。运行时始终通过
/// <see cref="ModbusRegisterLayoutBuilder"/> 根据顺序和类型重新排位，从源头避免地址重叠。
/// </remarks>
[Serializable]
[AddINotifyPropertyChangedInterface]
public sealed class RobotInfoModbusConfiguration : INotifyPropertyChanged
{
    /// <summary>配置字段变化通知；由 PropertyChanged.Fody 注入各属性 setter。</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    // 显式保留通知入口，既供 Fody 复用，也避免编译器把事件误判为从未使用。
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>程序启动后是否自动开启 Modbus TCP 服务端。</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Modbus TCP 监听端口，标准端口为 502。</summary>
    public int Port { get; set; } = 502;

    /// <summary>Modbus TCP 服务端接受 SCADA 客户端请求时使用的 Unit ID。</summary>
    public byte UnitIdentifier { get; set; } = 1;

    /// <summary>
    /// 第一个点位的 SCADA 六位保持寄存器参考地址，范围为 400001～465536。
    /// </summary>
    public int StartReferenceAddress { get; set; } = 400001;

    /// <summary>数据刷新到寄存器的周期，单位毫秒。</summary>
    public int PublishIntervalMilliseconds { get; set; } = 1000;

    /// <summary>Modbus TCP 服务端最多允许同时连接的 SCADA 客户端数量。</summary>
    public int MaxConnections { get; set; } = 64;

    /// <summary>SCADA 客户端长时间无请求时，服务端主动断开连接的等待时间，单位秒。</summary>
    public int ConnectionTimeoutSeconds { get; set; } = 120;

    /// <summary>UInt32 跨两个寄存器时采用的字顺序。</summary>
    public ModbusWordOrder WordOrder { get; set; } = ModbusWordOrder.LowWordFirst;

    /// <summary>
    /// 点位顺序、类型及说明。ReferenceAddress 不进入 XML，因为它必须由程序重新计算。
    /// </summary>
    [XmlArray("RegisterPoints")]
    [XmlArrayItem("Point")]
    public ObservableCollection<ModbusRegisterPointConfiguration> RegisterPoints { get; set; } = [];

    /// <summary>XML 文件的实际路径，仅供 UI 显示，不参与序列化。</summary>
    [XmlIgnore]
    public string ConfigurationFilePath { get; set; } = string.Empty;

    /// <summary>最近一次布局校验结果，仅供 UI 显示。</summary>
    [XmlIgnore]
    public string LayoutValidationMessage { get; set; } = "尚未校验";

    /// <summary>当前布局总共占用多少个 16 位寄存器。</summary>
    [XmlIgnore]
    public int RuntimeRegisterCount { get; set; }

    /// <summary>根据 Attribute 创建首次运行需要的默认配置。</summary>
    public static RobotInfoModbusConfiguration CreateDefault()
    {
        var configuration = new RobotInfoModbusConfiguration();
        foreach (ModbusPointDefinition definition in
                 ModbusPointCatalog.FromType<RobotInfoModbusRegisterData>())
        {
            configuration.RegisterPoints.Add(
                ModbusRegisterPointConfiguration.FromDefinition(
                    definition,
                    isBuiltIn: true));
        }

        configuration.RecalculateLayout();
        return configuration;
    }

    /// <summary>
    /// 严格校验整份当前配置并生成运行时布局；不迁移、不补齐缺失点位。
    /// </summary>
    /// <remarks>
    /// XML 中缺少任一内置 Order 或存在非法参数时直接抛出异常，由调用方整份回退默认配置。
    /// 自定义点位仍可追加；内置点位需要删除时应设为禁用，而不是从 XML 中移除。
    /// </remarks>
    public ModbusRegisterLayout ValidateAndCreateLayout()
    {
        if (Port is <= IPEndPoint.MinPort or > IPEndPoint.MaxPort)
        {
            throw new InvalidOperationException("Modbus TCP 端口必须在 1～65535 之间。");
        }

        if (PublishIntervalMilliseconds is < 100 or > 60_000)
        {
            throw new InvalidOperationException("寄存器刷新周期必须在 100～60000 毫秒之间。");
        }

        if (MaxConnections is < 1 or > 10_000)
        {
            throw new InvalidOperationException("最大连接数必须在 1～10000 之间。");
        }

        if (ConnectionTimeoutSeconds is < 1 or > 86_400)
        {
            throw new InvalidOperationException("连接超时必须在 1～86400 秒之间。");
        }

        if (!Enum.IsDefined(WordOrder))
        {
            throw new InvalidOperationException("配置中的 UInt32 字顺序无效。");
        }

        if (RegisterPoints is null || RegisterPoints.Count == 0)
        {
            throw new InvalidOperationException("Modbus 点位表不能为空。");
        }

        IReadOnlyList<ModbusPointDefinition> defaults =
            ModbusPointCatalog.FromType<RobotInfoModbusRegisterData>();
        var builtInOrders = defaults.Select(item => item.Order).ToHashSet();
        foreach (int order in builtInOrders)
        {
            int count = RegisterPoints.Count(item => item.Order == order);
            if (count == 0)
            {
                throw new InvalidOperationException($"Modbus 点位表缺少内置 Order {order}。");
            }

            if (count > 1)
            {
                throw new InvalidOperationException($"Modbus 点位表中的 Order {order} 重复。");
            }
        }

        foreach (ModbusRegisterPointConfiguration point in RegisterPoints)
        {
            point.IsBuiltIn = builtInOrders.Contains(point.Order);
        }

        return RecalculateLayout();
    }

    /// <summary>
    /// 使用与实际通信完全相同的算法重新计算 UI 地址，并返回可直接编码的运行时布局。
    /// </summary>
    public ModbusRegisterLayout RecalculateLayout()
    {
        IReadOnlyDictionary<int, ulong> propertyDefaults =
            ModbusPointCatalog.ReadValues(new RobotInfoModbusRegisterData());

        foreach (ModbusRegisterPointConfiguration point in RegisterPoints)
        {
            ulong propertyValue = propertyDefaults.GetValueOrDefault(point.Order);
            ulong maximum = point.DataType switch
            {
                ModbusPointDataType.UInt16 => ushort.MaxValue,
                ModbusPointDataType.UInt32 => uint.MaxValue,
                _ => throw new InvalidOperationException(
                    $"顺序 {point.Order} 的数据类型 {point.DataType} 不受支持。"),
            };
            if (propertyValue > maximum)
            {
                throw new InvalidOperationException(
                    $"顺序 {point.Order} 的属性值 {propertyValue} 超出 {point.DataType} 范围。");
            }

            point.ReferenceAddress = 0;
            point.ProtocolAddress = 0;
            point.RegisterCount = ModbusRegisterLayoutBuilder.GetRegisterCount(point.DataType);
            point.FormatText = point.DataType switch
            {
                ModbusPointDataType.UInt16 => "UDec16",
                ModbusPointDataType.UInt32 => "UDec32",
                _ => point.DataType.ToString(),
            };
            point.AddressText = point.Enabled ? "待计算" : "已禁用";
            // 这里只供 WPF 显示无符号整数，不参与协议编码或文本反序列化，直接转换即可。
            point.CurrentValueText = propertyValue.ToString();
        }

        ModbusRegisterLayout layout = ModbusRegisterLayoutBuilder.Build(
            StartReferenceAddress,
            RegisterPoints.Select(item => item.ToDefinition()));

        var pointByOrder = RegisterPoints.ToDictionary(item => item.Order);
        foreach (ModbusRegisterPointLayout runtimePoint in layout.Points)
        {
            ModbusRegisterPointConfiguration point = pointByOrder[runtimePoint.Order];
            point.ReferenceAddress = runtimePoint.ReferenceAddress;
            point.ProtocolAddress = runtimePoint.ProtocolAddress;
            point.RegisterCount = runtimePoint.RegisterCount;
            point.AddressText = runtimePoint.ReferenceAddress.ToString();
        }

        RuntimeRegisterCount = layout.RegisterCount;
        LayoutValidationMessage =
            $"配置有效：{layout.Points.Count} 个点位，占用 {layout.RegisterCount} 个寄存器";
        return layout;
    }

    /// <summary>把 XML 点位转换为独立库使用的定义。</summary>
    public IReadOnlyList<ModbusPointDefinition> CreateDefinitions() =>
        RegisterPoints.Select(item => item.ToDefinition()).ToArray();
}

/// <summary>一个可持久化、可在 UI 中编辑的 Modbus 点位。</summary>
[Serializable]
[AddINotifyPropertyChangedInterface]
public sealed class ModbusRegisterPointConfiguration : INotifyPropertyChanged
{
    /// <summary>点位字段变化通知；用于实时重算地址预览。</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>点位唯一标识及自动排位顺序；不能与其他启用点位重复。</summary>
    public int Order { get; set; }

    /// <summary>是否进入实际寄存器块。</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>UInt16 占 1 个字，UInt32 占 2 个字。</summary>
    public ModbusPointDataType DataType { get; set; } = ModbusPointDataType.UInt16;

    /// <summary>UI 和 SCADA 表中显示的名称。</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>工程单位。</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>现场说明。</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>是否来自代码 Attribute；仅供 UI 提示。</summary>
    [XmlIgnore]
    public bool IsBuiltIn { get; set; }

    /// <summary>程序计算出的 SCADA 参考地址，不写入 XML。</summary>
    [XmlIgnore]
    public int ReferenceAddress { get; set; }

    /// <summary>程序计算出的 PDU 零基地址，不写入 XML。</summary>
    [XmlIgnore]
    public ushort ProtocolAddress { get; set; }

    /// <summary>程序计算出的占用字数，不写入 XML。</summary>
    [XmlIgnore]
    public int RegisterCount { get; set; }

    /// <summary>供 UI 展示的自动地址。</summary>
    [XmlIgnore]
    public string AddressText { get; set; } = "待计算";

    /// <summary>供 UI 展示的 UDec16/UDec32 名称。</summary>
    [XmlIgnore]
    public string FormatText { get; set; } = "UDec16";

    /// <summary>最近一次发布的属性值或业务值。</summary>
    [XmlIgnore]
    public string CurrentValueText { get; set; } = "—";

    internal static ModbusRegisterPointConfiguration FromDefinition(
        ModbusPointDefinition definition,
        bool isBuiltIn) => new()
        {
            Order = definition.Order,
            Enabled = definition.Enabled,
            DataType = definition.DataType,
            DisplayName = definition.DisplayName,
            Unit = definition.Unit,
            Comment = definition.Comment,
            IsBuiltIn = isBuiltIn,
        };

    internal ModbusPointDefinition ToDefinition() => new(
        Order,
        DataType,
        DisplayName?.Trim() ?? string.Empty,
        Unit?.Trim() ?? string.Empty,
        Comment?.Trim() ?? string.Empty,
        Enabled);
}

/// <summary>
/// Attribute 默认点位合同。属性初始化值用于当前联调；正式接入业务值时可统一改为 0。
/// </summary>
public sealed class RobotInfoModbusRegisterData
{
    [ModbusPoint(
        0,
        ModbusPointDataType.UInt32,
        DisplayName = "当天运行时间",
        Unit = "分钟",
        Comment = "属性默认值，后续接入当天运行统计")]
    public uint DailyRunMinutes { get; init; } = 1_200;

    [ModbusPoint(
        1,
        ModbusPointDataType.UInt32,
        DisplayName = "累计运行时间",
        Unit = "秒",
        Comment = "属性默认值，后续接入持久化累计运行时间")]
    public uint TotalRunSeconds { get; init; } = 31_536_000;

    [ModbusPoint(
        2,
        ModbusPointDataType.UInt32,
        DisplayName = "停机时间",
        Unit = "分钟",
        Comment = "属性默认值，后续按 SCADA 停机口径统计")]
    public uint DowntimeMinutes { get; init; } = 3_000;

    [ModbusPoint(
        3,
        ModbusPointDataType.UInt32,
        DisplayName = "单次停机时间",
        Unit = "分钟",
        Comment = "属性默认值，后续接入当前停机区间")]
    public uint CurrentDowntimeMinutes { get; init; } = 40;

    [ModbusPoint(
        4,
        ModbusPointDataType.UInt16,
        DisplayName = "状态信息",
        Comment = "属性默认值：0运行、1故障、2待机")]
    public ushort StatusCode { get; init; }

    [ModbusPoint(
        5,
        ModbusPointDataType.UInt32,
        DisplayName = "当天产量",
        Unit = "件",
        Comment = "属性默认值，后续接入当天设备产量")]
    public uint DailyProduction { get; init; } = 350;

    [ModbusPoint(
        6,
        ModbusPointDataType.UInt32,
        DisplayName = "累计产量",
        Unit = "件",
        Comment = "属性默认值，业务累计产量字段尚未建立")]
    public uint TotalProduction { get; init; } = 127_750;

    [ModbusPoint(
        7,
        ModbusPointDataType.UInt32,
        DisplayName = "上电时间",
        Unit = "分钟",
        Comment = "属性默认值，后续接入设备真实上电时间")]
    public uint PowerOnMinutes { get; init; } = 60;
}
