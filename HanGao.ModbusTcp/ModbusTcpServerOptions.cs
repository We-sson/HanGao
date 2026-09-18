using System.Net;

namespace HanGao.ModbusTcp;

/// <summary>
/// 只读 Modbus TCP 服务端的启动参数。
/// </summary>
/// <remarks>
/// 该记录类型不可变，启动后不会被服务端内部修改。若服务端正在运行，再用不同参数启动会抛出异常，
/// 从而避免 UI 修改配置后出现“界面值已经变化、监听器仍使用旧值”的隐蔽状态。
/// </remarks>
public sealed record ModbusTcpServerOptions
{
    /// <summary>本机监听地址；使用 <see cref="IPAddress.Any"/> 表示监听全部网卡。</summary>
    public IPAddress BindAddress { get; init; } = IPAddress.Any;

    /// <summary>
    /// TCP 监听端口。标准端口为 502；开发调试时建议使用 1502，避免权限或端口占用问题。
    /// </summary>
    public int Port { get; init; } = 502;

    /// <summary>
    /// 允许访问的单元标识。设为 0 时按单设备模式接受任意 Unit ID。
    /// </summary>
    public byte AcceptedUnitIdentifier { get; init; } = 1;

    /// <summary>
    /// 从协议地址 0 开始允许读取的寄存器数量。
    /// </summary>
    public int RegisterCount { get; init; } = 4096;

    /// <summary>
    /// 最大同时 TCP 连接数。设为 0 时不额外限制，由底层协议库和操作系统决定。
    /// </summary>
    public int MaxConnections { get; init; } = 64;

    /// <summary>
    /// Modbus TCP 客户端持续无通信超过此时间后关闭连接，避免异常断网留下永久占用的连接。
    /// </summary>
    public TimeSpan ConnectionTimeout { get; init; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// 内存中最多保留的生命周期和错误诊断条数；超过后从最旧记录开始淘汰。
    /// </summary>
    public int DiagnosticHistoryCapacity { get; init; } = 500;

    /// <summary>
    /// 在打开监听端口前集中检查参数，避免服务端进入“部分初始化”状态。
    /// </summary>
    internal void Validate()
    {
        ArgumentNullException.ThrowIfNull(BindAddress);

        if (Port is <= IPEndPoint.MinPort or > IPEndPoint.MaxPort)
        {
            throw new ArgumentOutOfRangeException(nameof(Port), Port, "端口必须在 1～65535 之间。");
        }

        if (RegisterCount is < 1 or > 65536)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RegisterCount),
                RegisterCount,
                "寄存器数量必须在 1～65536 之间。");
        }

        if (MaxConnections is < 0 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxConnections),
                MaxConnections,
                "最大连接数必须在 0～10000 之间。");
        }

        if (ConnectionTimeout <= TimeSpan.Zero || ConnectionTimeout > TimeSpan.FromDays(1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(ConnectionTimeout),
                ConnectionTimeout,
                "连接超时必须大于零且不能超过一天。");
        }

        if (DiagnosticHistoryCapacity is < 1 or > 100_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(DiagnosticHistoryCapacity),
                DiagnosticHistoryCapacity,
                "诊断记录容量必须在 1～100000 之间。");
        }
    }
}
