using System.Net;

namespace HanGao.ModbusTcp;

/// <summary>Modbus TCP 服务的生命周期状态。</summary>
public enum ModbusServerState
{
    /// <summary>服务未监听端口。</summary>
    Stopped,

    /// <summary>正在创建监听器。</summary>
    Starting,

    /// <summary>正在接受连接并处理请求。</summary>
    Running,

    /// <summary>正在关闭监听器和客户端连接。</summary>
    Stopping,

    /// <summary>最近一次生命周期操作失败。</summary>
    Faulted,

    /// <summary>服务已经永久释放，不能再次启动。</summary>
    Disposed,
}

/// <summary>诊断事件严重等级。</summary>
public enum ModbusDiagnosticSeverity
{
    /// <summary>详细跟踪信息。</summary>
    Trace,

    /// <summary>正常生命周期信息。</summary>
    Information,

    /// <summary>可恢复问题或被拒绝的请求。</summary>
    Warning,

    /// <summary>服务生命周期故障。</summary>
    Error,
}

/// <summary>一条保存在有界内存队列中的诊断记录。</summary>
/// <param name="TimestampUtc">事件发生的 UTC 时间。</param>
/// <param name="Severity">严重等级。</param>
/// <param name="Code">便于程序筛选的稳定事件代码。</param>
/// <param name="Message">便于 UI 和日志显示的中文说明。</param>
/// <param name="Exception">关联异常；正常信息和协议拒绝通常为空。</param>
public sealed record ModbusDiagnosticEvent(
    DateTimeOffset TimestampUtc,
    ModbusDiagnosticSeverity Severity,
    string Code,
    string Message,
    Exception? Exception = null);

/// <summary>最近一次经过校验的客户端请求元数据。</summary>
/// <param name="TimestampUtc">收到请求的 UTC 时间。</param>
/// <param name="UnitIdentifier">请求中的 Unit ID。</param>
/// <param name="FunctionCode">请求功能码，例如 FC03 为 3。</param>
/// <param name="StartAddress">报文中的零基起始地址。</param>
/// <param name="Quantity">请求读取或写入的寄存器数量。</param>
/// <param name="Accepted">请求是否通过只读、站号和范围校验。</param>
public sealed record ModbusRequestInfo(
    DateTimeOffset TimestampUtc,
    byte UnitIdentifier,
    byte FunctionCode,
    ushort StartAddress,
    ushort Quantity,
    bool Accepted);

/// <summary>最近一次成功发布的寄存器快照元数据。</summary>
/// <param name="PublishedAtUtc">完成寄存器复制的 UTC 时间。</param>
/// <param name="CapturedAtUtc">业务数据本身的采集时间。</param>
/// <param name="Sequence">业务层提供的快照流水号。</param>
/// <param name="StartAddress">快照的零基起始地址。</param>
/// <param name="RegisterCount">本次快照覆盖的寄存器数量。</param>
public sealed record ModbusPublishInfo(
    DateTimeOffset PublishedAtUtc,
    DateTimeOffset CapturedAtUtc,
    long Sequence,
    ushort StartAddress,
    int RegisterCount);

/// <summary>适合 UI 绑定或健康检查的服务瞬时状态。</summary>
/// <param name="State">当前生命周期状态。</param>
/// <param name="LocalEndpoint">实际监听端点；停止时为空。</param>
/// <param name="ConnectedClients">当前已连接客户端数量。</param>
/// <param name="TotalRequests">启动实例累计收到的请求数。</param>
/// <param name="RejectedRequests">因功能码、站号或地址不合法而拒绝的请求数。</param>
/// <param name="PublishedSnapshots">成功发布的业务快照数量。</param>
/// <param name="LastRequest">最近请求元数据。</param>
/// <param name="LastPublish">最近发布元数据。</param>
/// <param name="LastError">最近生命周期错误文本；没有错误时为空。</param>
public sealed record ModbusTcpServerStatus(
    ModbusServerState State,
    IPEndPoint? LocalEndpoint,
    int ConnectedClients,
    long TotalRequests,
    long RejectedRequests,
    long PublishedSnapshots,
    ModbusRequestInfo? LastRequest,
    ModbusPublishInfo? LastPublish,
    string? LastError);
