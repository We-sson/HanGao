using System.Collections.Concurrent;
using System.Buffers.Binary;
using System.Net;
using FluentModbus;

namespace HanGao.ModbusTcp;

/// <summary>
/// 基于 FluentModbus 的只读 Modbus TCP 服务端实现。
/// </summary>
/// <remarks>
/// 服务端固定开放 FC03 保持寄存器读取；FC04 和所有写功能码都由请求校验器拒绝。
/// 业务线程先构造不可变快照，再在底层服务端的同步锁中整体复制，因此一次协议读取不会取得撕裂数据。
/// 生命周期另由信号量串行化，避免 UI 连续点击启动/停止时创建多个监听器。
/// </remarks>
public sealed class FluentModbusTcpServer : IModbusTcpServer
{
    // 串行化 Start/Stop/Dispose；这些操作包含端口和连接资源变化，不能并发执行。
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);

    // 保护底层服务端引用、选项和监听端点，保证 Publish/Stop/GetStatus 看见一致对象。
    private readonly object _serverGate = new();

    // 多线程请求和 UI 线程都可能写入诊断，因此使用无锁并发队列。
    private readonly ConcurrentQueue<ModbusDiagnosticEvent> _diagnosticHistory = new();

    // 以下三个字段必须在 _serverGate 内成组读取或修改。
    private ModbusTcpServer? _server;
    private ModbusTcpServerOptions? _options;
    private IPEndPoint? _localEndpoint;

    // 最近事件使用原子引用替换，GetStatus 无需持有锁即可取得完整不可变记录。
    private ModbusRequestInfo? _lastRequest;
    private ModbusPublishInfo? _lastPublish;
    private string? _lastError;

    // 枚举按 int 原子读写；计数器使用 Interlocked，支持并发请求和发布线程。
    private int _state = (int)ModbusServerState.Stopped;
    private int _disposed;
    private int _diagnosticCount;
    private long _totalRequests;
    private long _rejectedRequests;
    private long _publishedSnapshots;

    /// <inheritdoc />
    public event EventHandler<ModbusDiagnosticEvent>? DiagnosticEmitted;

    /// <inheritdoc />
    /// <remarks>返回的是不可变状态记录，适合 WPF 定时轮询，不会暴露底层服务端对象。</remarks>
    public ModbusTcpServerStatus GetStatus()
    {
        int connectionCount = 0;
        IPEndPoint? endpoint;

        lock (_serverGate)
        {
            endpoint = _localEndpoint;

            if (_server is not null)
            {
                lock (_server.Lock)
                {
                    connectionCount = _server.ConnectionCount;
                }
            }
        }

        return new ModbusTcpServerStatus(
            State: (ModbusServerState)Volatile.Read(ref _state),
            LocalEndpoint: endpoint,
            ConnectedClients: connectionCount,
            TotalRequests: Interlocked.Read(ref _totalRequests),
            RejectedRequests: Interlocked.Read(ref _rejectedRequests),
            PublishedSnapshots: Interlocked.Read(ref _publishedSnapshots),
            LastRequest: Volatile.Read(ref _lastRequest),
            LastPublish: Volatile.Read(ref _lastPublish),
            LastError: Volatile.Read(ref _lastError));
    }

    /// <inheritdoc />
    public IReadOnlyList<ModbusDiagnosticEvent> GetRecentDiagnostics() =>
        _diagnosticHistory.ToArray();

    /// <inheritdoc />
    /// <remarks>
    /// 先校验全部参数，再创建底层服务端；只有端口成功监听后才发布 Running 状态。
    /// 相同参数重复启动视为幂等操作，不同参数必须先停止再启动。
    /// </remarks>
    public async Task StartAsync(
        ModbusTcpServerOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();
        ThrowIfDisposed();

        await _lifecycleGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();

            if ((ModbusServerState)Volatile.Read(ref _state) == ModbusServerState.Running)
            {
                if (options == _options)
                {
                    return;
                }

                throw new InvalidOperationException("Modbus TCP 服务端已使用另一组参数运行；请先停止再重新启动。");
            }

            SetState(ModbusServerState.Starting, "SERVER_STARTING", "Modbus TCP 服务端正在启动。");

            var endpoint = new IPEndPoint(options.BindAddress, options.Port);
            var server = new ModbusTcpServer(isAsynchronous: true)
            {
                ConnectionTimeout = options.ConnectionTimeout,
                MaxConnections = options.MaxConnections,
            };

            if (options.AcceptedUnitIdentifier != 0)
            {
                // FluentModbus 默认只创建 Unit 0。显式增加现场配置的 Unit，确保站号 1 等固定站号
                // 使用自己的寄存器区，而不是依赖底层库对单设备通配行为的解释。
                server.AddUnit(options.AcceptedUnitIdentifier);
            }

            server.RequestValidator = (unitIdentifier, functionCode, startAddress, quantity) =>
                ValidateRequest(options, unitIdentifier, functionCode, startAddress, quantity);

            try
            {
                lock (_serverGate)
                {
                    server.Start(endpoint);
                    _server = server;
                    _options = options;
                    _localEndpoint = endpoint;
                }

                Volatile.Write(ref _lastError, null);
                SetState(
                    ModbusServerState.Running,
                    "SERVER_STARTED",
                    $"Modbus TCP 服务端已监听 {endpoint}。");
            }
            catch (Exception exception)
            {
                server.Dispose();
                Volatile.Write(ref _lastError, exception.Message);
                SetState(
                    ModbusServerState.Faulted,
                    "SERVER_START_FAILED",
                    $"Modbus TCP 服务端无法在 {endpoint} 启动：{exception.Message}",
                    exception);
                throw;
            }
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <inheritdoc />
    /// <remarks>停止过程会关闭全部 Modbus TCP 客户端连接；相同实例可在停止后再次启动。</remarks>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await StopCoreAsync(cancellationToken, disposing: false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 复制寄存器与更新最后发布元数据都放在同一个服务端锁中，使 UI 状态与 SCADA 客户端实际可读内容保持同一发布顺序。
    /// </remarks>
    public void Publish(ModbusRegisterSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ThrowIfDisposed();

        lock (_serverGate)
        {
            if (_server is null || _options is null ||
                (ModbusServerState)Volatile.Read(ref _state) != ModbusServerState.Running)
            {
                throw new InvalidOperationException("Modbus TCP 服务端尚未运行，不能发布寄存器快照。");
            }

            int endExclusive = snapshot.StartAddress + snapshot.Registers.Length;
            if (endExclusive > _options.RegisterCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(snapshot),
                    $"快照结束地址 {endExclusive - 1} 超出配置的可读范围 0～{_options.RegisterCount - 1}。");
            }

            lock (_server.Lock)
            {
                Span<byte> registerBuffer =
                    _server.GetHoldingRegisterBuffer(_options.AcceptedUnitIdentifier);
                int byteStart = snapshot.StartAddress * sizeof(ushort);
                Span<byte> destination = registerBuffer.Slice(
                    byteStart,
                    snapshot.Registers.Length * sizeof(ushort));
                ReadOnlySpan<ushort> source = snapshot.Registers.Span;

                // FluentModbus 服务端直接把寄存器字节缓冲区写入响应报文。
                // Modbus 规定每个 16 位寄存器在线路上按高字节在前传输，因此不能把本机
                // little-endian 的 ushort/short 直接复制到底层缓冲区。
                for (int index = 0; index < source.Length; index++)
                {
                    BinaryPrimitives.WriteUInt16BigEndian(
                        destination.Slice(index * sizeof(ushort), sizeof(ushort)),
                        source[index]);
                }
            }

            // 寄存器和 UI 可见元数据保持同一发布顺序，避免并发发布后 LastPublish 指向旧快照。
            Interlocked.Increment(ref _publishedSnapshots);
            Volatile.Write(
                ref _lastPublish,
                new ModbusPublishInfo(
                    PublishedAtUtc: DateTimeOffset.UtcNow,
                    CapturedAtUtc: snapshot.CapturedAtUtc,
                    Sequence: snapshot.Sequence,
                    StartAddress: snapshot.StartAddress,
                    RegisterCount: snapshot.Registers.Length));
        }
    }

    /// <inheritdoc />
    /// <remarks>释放是幂等操作；一旦释放，实例不能再次启动。</remarks>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        await StopCoreAsync(CancellationToken.None, disposing: true).ConfigureAwait(false);
        Volatile.Write(ref _state, (int)ModbusServerState.Disposed);
        EmitDiagnostic(
            ModbusDiagnosticSeverity.Information,
            "SERVER_DISPOSED",
            "Modbus TCP 服务端已释放。");
        _lifecycleGate.Dispose();
    }

    /// <summary>
    /// Start、显式 Stop 和 Dispose 共用的停止实现。
    /// </summary>
    /// <param name="cancellationToken">等待生命周期锁时使用的取消标记。</param>
    /// <param name="disposing">为 true 时允许对象已被标记为释放。</param>
    private async Task StopCoreAsync(CancellationToken cancellationToken, bool disposing)
    {
        if (!disposing)
        {
            ThrowIfDisposed();
        }

        await _lifecycleGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ModbusServerState currentState = (ModbusServerState)Volatile.Read(ref _state);
            if (currentState is ModbusServerState.Stopped or ModbusServerState.Disposed)
            {
                return;
            }

            SetState(ModbusServerState.Stopping, "SERVER_STOPPING", "Modbus TCP 服务端正在停止。");

            Exception? stopError = null;
            lock (_serverGate)
            {
                ModbusTcpServer? server = _server;
                _server = null;
                _options = null;
                _localEndpoint = null;

                if (server is not null)
                {
                    try
                    {
                        server.Stop();
                    }
                    catch (Exception exception)
                    {
                        stopError = exception;
                    }
                    finally
                    {
                        server.Dispose();
                    }
                }
            }

            if (stopError is not null)
            {
                Volatile.Write(ref _lastError, stopError.Message);
                SetState(
                    ModbusServerState.Faulted,
                    "SERVER_STOP_FAILED",
                    $"Modbus TCP 服务端停止时发生错误：{stopError.Message}",
                    stopError);
                throw stopError;
            }

            SetState(ModbusServerState.Stopped, "SERVER_STOPPED", "Modbus TCP 服务端已停止。");
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>
    /// 在底层库处理请求前检查 Unit ID、功能码和地址范围。
    /// </summary>
    /// <remarks>
    /// 只允许 FC03；FC04 和写功能码统一返回 IllegalFunction。
    /// Unit ID 或地址错误返回 IllegalDataAddress，避免向外暴露未配置的寄存器区。
    /// </remarks>
    private ModbusExceptionCode ValidateRequest(
        ModbusTcpServerOptions options,
        byte unitIdentifier,
        ModbusFunctionCode functionCode,
        ushort startAddress,
        ushort quantity)
    {
        bool correctUnit = options.AcceptedUnitIdentifier == 0 ||
                           unitIdentifier == options.AcceptedUnitIdentifier;
        bool correctFunction = functionCode == ModbusFunctionCode.ReadHoldingRegisters;
        bool correctRange = quantity > 0 &&
                            (uint)startAddress + quantity <= options.RegisterCount;
        bool accepted = correctUnit && correctFunction && correctRange;

        Interlocked.Increment(ref _totalRequests);
        Volatile.Write(
            ref _lastRequest,
            new ModbusRequestInfo(
                TimestampUtc: DateTimeOffset.UtcNow,
                UnitIdentifier: unitIdentifier,
                FunctionCode: (byte)functionCode,
                StartAddress: startAddress,
                Quantity: quantity,
                Accepted: accepted));

        if (accepted)
        {
            return ModbusExceptionCode.OK;
        }

        Interlocked.Increment(ref _rejectedRequests);

        ModbusExceptionCode error = !correctFunction
            ? ModbusExceptionCode.IllegalFunction
            : ModbusExceptionCode.IllegalDataAddress;

        EmitDiagnostic(
            ModbusDiagnosticSeverity.Warning,
            "REQUEST_REJECTED",
            $"拒绝请求：Unit={unitIdentifier}，功能码={(byte)functionCode}，地址={startAddress}，数量={quantity}，异常码={(byte)error}。");

        return error;
    }

    /// <summary>原子更新生命周期状态，并生成一条可供 UI/日志显示的诊断记录。</summary>
    private void SetState(
        ModbusServerState state,
        string code,
        string message,
        Exception? exception = null)
    {
        Volatile.Write(ref _state, (int)state);
        EmitDiagnostic(
            exception is null ? ModbusDiagnosticSeverity.Information : ModbusDiagnosticSeverity.Error,
            code,
            message,
            exception);
    }

    /// <summary>
    /// 保存有界诊断历史并通知订阅者。
    /// </summary>
    /// <remarks>
    /// 单个 UI 或日志订阅者抛出的异常会被隔离，不能中断协议线程；订阅者仍应保持处理简短。
    /// </remarks>
    private void EmitDiagnostic(
        ModbusDiagnosticSeverity severity,
        string code,
        string message,
        Exception? exception = null)
    {
        var diagnostic = new ModbusDiagnosticEvent(
            DateTimeOffset.UtcNow,
            severity,
            code,
            message,
            exception);

        _diagnosticHistory.Enqueue(diagnostic);
        int count = Interlocked.Increment(ref _diagnosticCount);
        int capacity = _options?.DiagnosticHistoryCapacity ?? 500;

        while (count > capacity && _diagnosticHistory.TryDequeue(out _))
        {
            count = Interlocked.Decrement(ref _diagnosticCount);
        }

        EventHandler<ModbusDiagnosticEvent>? handlers = DiagnosticEmitted;
        if (handlers is null)
        {
            return;
        }

        foreach (EventHandler<ModbusDiagnosticEvent> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, diagnostic);
            }
            catch
            {
                // UI/日志订阅者不能破坏协议服务端。
            }
        }
    }

    /// <summary>对象释放后拒绝所有公开操作，避免访问已经关闭的套接字和信号量。</summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }
}
