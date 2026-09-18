namespace HanGao.ModbusTcp;

/// <summary>
/// 提供具有完整生命周期管理的只读 Modbus TCP 服务端。
/// </summary>
/// <remarks>
/// 接口只暴露“启动、停止、发布快照、读取诊断”四类能力，业务层不需要引用 FluentModbus，
/// 后续更换底层协议库或编写模拟服务端时不会影响 Robot_Info_Mes 的 ViewModel。
/// </remarks>
public interface IModbusTcpServer : IAsyncDisposable
{
    /// <summary>
    /// 生命周期变化、请求被拒绝或服务端失败时触发。
    /// </summary>
    event EventHandler<ModbusDiagnosticEvent>? DiagnosticEmitted;

    /// <summary>
    /// 返回当前服务端状态的不可变瞬时副本，可直接用于 UI 绑定或健康检查。
    /// </summary>
    ModbusTcpServerStatus GetStatus();

    /// <summary>
    /// 返回有容量限制的内存诊断历史，顺序为从旧到新。
    /// </summary>
    IReadOnlyList<ModbusDiagnosticEvent> GetRecentDiagnostics();

    /// <summary>
    /// 启动 TCP 监听。使用相同参数重复调用不会重复创建监听器。
    /// </summary>
    /// <param name="options">监听端点、寄存器区域和连接限制。</param>
    /// <param name="cancellationToken">取消等待启动操作，不用于停止已经启动的服务端。</param>
    Task StartAsync(
        ModbusTcpServerOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止监听并关闭当前所有 Modbus TCP 客户端连接；重复停止是安全的。
    /// </summary>
    /// <param name="cancellationToken">取消等待生命周期锁。</param>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 将一份不可变快照整体发布到配置的寄存器区。
    /// </summary>
    /// <param name="snapshot">已经完成业务单位换算和寄存器编码的快照。</param>
    /// <remarks>
    /// 发布使用与底层协议读取相同的同步锁；因此 SCADA 在一次请求中读取完整数据块时，
    /// 只能得到发布前或发布后的整份数据，不会得到混合版本。
    /// </remarks>
    void Publish(ModbusRegisterSnapshot snapshot);
}
