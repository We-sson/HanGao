using System.Diagnostics;
using System.Net;
using System.Windows.Threading;
using HanGao.ModbusTcp;
using Robot_Info_Mes.Model;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Robot_Info_Mes.Services;

/// <summary>
/// 在 Robot_Info_Mes 进程中托管只读 Modbus TCP 服务，并定时把当前机器人数据发布到 40001 寄存器块。
/// </summary>
/// <remarks>
/// Robot_Info_Mes 在 Modbus TCP 中是 Server（从站/服务端），SCADA 是 Client（主站/客户端）。
/// SCADA 的轮询频率与本类的快照发布频率互相独立：SCADA 可以随时读取服务器最近一次完整快照。
/// 本类使用 DispatcherTimer 在 WPF UI 线程读取模型，避免后台线程同时遍历正在被界面更新的业务对象。
/// </remarks>
public sealed class RobotInfoModbusServerHost : IAsyncDisposable
{
    private readonly Func<Mes_Robot_Info_Model> _modelProvider;
    private readonly IModbusTcpServer _server;
    private readonly DispatcherTimer _publishTimer;
    private readonly Stopwatch _serviceUptime = new();
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);

    private long _sequence;
    private int _started;
    private int _disposed;

    /// <summary>
    /// 创建应用层 Modbus 托管服务。
    /// </summary>
    /// <param name="modelProvider">
    /// 返回当前 <see cref="Mes_Robot_Info_Model"/> 的函数；使用函数而不是保存固定引用，
    /// 是为了兼容 ViewModel 从 XML 恢复后替换整个模型实例。
    /// </param>
    public RobotInfoModbusServerHost(Func<Mes_Robot_Info_Model> modelProvider)
        : this(modelProvider, new FluentModbusTcpServer())
    {
    }

    /// <summary>
    /// 创建可注入协议服务的实例，主要用于单元测试或更换底层 Modbus 实现。
    /// </summary>
    internal RobotInfoModbusServerHost(
        Func<Mes_Robot_Info_Model> modelProvider,
        IModbusTcpServer server)
    {
        ArgumentNullException.ThrowIfNull(modelProvider);
        ArgumentNullException.ThrowIfNull(server);

        _modelProvider = modelProvider;
        _server = server;
        _publishTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        _publishTimer.Tick += PublishTimerOnTick;
    }

    /// <summary>
    /// 定时发布失败时触发。UI 可订阅该事件显示日志，但不能在事件中执行耗时工作。
    /// </summary>
    public event Action<Exception>? PublishFailed;

    /// <summary>最近一次定时发布错误；下一次发布成功后自动清空。</summary>
    public string? LastPublishError { get; private set; }

    /// <summary>
    /// 启动 FC03/40001 只读服务，并开始按固定周期生成业务快照。
    /// </summary>
    /// <param name="bindAddress">本机监听地址；示例可用 Any，生产建议绑定工控机指定网卡地址。</param>
    /// <param name="port">TCP 端口，标准值为 502；调试时可使用 1502。</param>
    /// <param name="unitIdentifier">SCADA 配置的 Unit ID，默认 1。</param>
    /// <param name="publishInterval">从业务模型刷新寄存器的周期，默认 1 秒。</param>
    /// <param name="cancellationToken">取消启动等待。</param>
    public async Task StartAsync(
        IPAddress bindAddress,
        int port = 502,
        byte unitIdentifier = 1,
        TimeSpan? publishInterval = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bindAddress);
        ThrowIfDisposed();

        await _lifecycleGate.WaitAsync(cancellationToken);
        try
        {
            ThrowIfDisposed();
            if (Volatile.Read(ref _started) != 0)
            {
                return;
            }

            TimeSpan interval = publishInterval ?? TimeSpan.FromSeconds(1);
            if (interval < TimeSpan.FromMilliseconds(100) || interval > TimeSpan.FromMinutes(1))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(publishInterval),
                    interval,
                    "寄存器发布周期必须在 100 毫秒到 1 分钟之间。");
            }

            await _server.StartAsync(
                new ModbusTcpServerOptions
                {
                    BindAddress = bindAddress,
                    Port = port,
                    AcceptedUnitIdentifier = unitIdentifier,
                    RegisterArea = ModbusRegisterArea.HoldingRegisters,
                    RegisterCount = 64,
                    MaxConnections = 64,
                    ConnectionTimeout = TimeSpan.FromMinutes(2),
                    DiagnosticHistoryCapacity = 500,
                },
                cancellationToken);

            _publishTimer.Interval = interval;
            _serviceUptime.Restart();
            Volatile.Write(ref _started, 1);

            // 启动完成后立即发布一次，避免 SCADA 在第一个定时周期前读到全零默认值。
            PublishNow();
            _publishTimer.Start();
        }
        catch
        {
            Volatile.Write(ref _started, 0);
            _serviceUptime.Reset();
            await _server.StopAsync(CancellationToken.None);
            throw;
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>
    /// 立即读取当前业务模型并发布一份 40001～40018 快照。
    /// </summary>
    /// <remarks>
    /// 该方法应从 WPF UI 线程调用。编码先在私有数组中完成，随后由底层服务整体复制，
    /// 因此 SCADA 单次 FC03 请求不会读到一半旧值、一半新值。
    /// </remarks>
    public void PublishNow()
    {
        ThrowIfDisposed();
        if (Volatile.Read(ref _started) == 0)
        {
            throw new InvalidOperationException("Modbus TCP 服务尚未启动。");
        }

        Mes_Robot_Info_Model model = _modelProvider() ??
            throw new InvalidOperationException("业务模型提供器返回了空对象。");

        uint totalProduction = 0;
        // TODO：Robot_Info_Mes 当前没有“跨日累计产量”字段，暂时向 40010～40011 发布 0。
        // 增加持久化字段后，可替换为类似下面的代码：
        // totalProduction = ToUInt32(model.Robot_Work_All_Number, "累计产量");

        var values = new RobotInfoRegisterValues(
            DailyRunMinutes: ToUInt16(model.Robot_Run_Time.Timer_UI.TotalMinutes, "当天运行时间"),
            TotalRunSeconds: ToUInt32(model.Robot_Run_All_Time.Timer_UI.TotalSeconds, "累计运行时间"),
            // 示例暂把“停机时间”解释为累计故障时间；若现场定义为离线或非生产时间，应在此替换数据源。
            DowntimeMinutes: ToUInt32(model.Robot_Error_All_Time.Timer_UI.TotalMinutes, "累计停机时间"),
            // 当前模型只有累计故障计时，没有“本次停机起点”，所以单次停机时间暂发 0。
            CurrentDowntimeMinutes: 0,
            StatusCode: ResolveStatus(model),
            DailyProduction: ToUInt16(model.Robot_Work_ABCD_Number, "当天产量"),
            TotalProduction: totalProduction,
            // 当前示例表示本 Modbus 服务启动后的时间；若机器人能上报控制柜上电时间，应替换此数据源。
            PowerOnMinutes: ToUInt32(_serviceUptime.Elapsed.TotalMinutes, "本次上电时间"),
            UpdatedAtUtc: DateTimeOffset.UtcNow);

        long sequence = Interlocked.Increment(ref _sequence);
        _server.Publish(RobotInfoRegisterMap.CreateSnapshot(values, sequence));
        LastPublishError = null;
    }

    /// <summary>取得用于 UI 状态栏显示的 Modbus 服务瞬时状态。</summary>
    public ModbusTcpServerStatus GetStatus() => _server.GetStatus();

    /// <summary>取得最近诊断历史，用于 UI 调试弹窗或日志导出。</summary>
    public IReadOnlyList<ModbusDiagnosticEvent> GetRecentDiagnostics() =>
        _server.GetRecentDiagnostics();

    /// <summary>停止定时发布、关闭监听端口和全部 SCADA 连接。</summary>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            return;
        }

        await _lifecycleGate.WaitAsync(cancellationToken);
        try
        {
            if (Interlocked.Exchange(ref _started, 0) == 0)
            {
                return;
            }

            _publishTimer.Stop();
            _serviceUptime.Stop();
            await _server.StopAsync(cancellationToken);
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>停止服务并释放计时器、套接字和生命周期同步资源。</summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _publishTimer.Stop();
        _publishTimer.Tick -= PublishTimerOnTick;
        Volatile.Write(ref _started, 0);
        await _server.DisposeAsync();
        _lifecycleGate.Dispose();
    }

    /// <summary>DispatcherTimer 的周期入口；错误被记录并上报，不能让异常终止 WPF 消息循环。</summary>
    private void PublishTimerOnTick(object? sender, EventArgs eventArgs)
    {
        try
        {
            PublishNow();
        }
        catch (Exception exception)
        {
            LastPublishError = exception.Message;
            NotifyPublishFailed(exception);
        }
    }

    /// <summary>逐个通知错误订阅者，并隔离订阅者异常，避免 UI 日志代码中断定时器。</summary>
    private void NotifyPublishFailed(Exception exception)
    {
        Action<Exception>? handlers = PublishFailed;
        if (handlers is null)
        {
            return;
        }

        foreach (Action<Exception> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(exception);
            }
            catch
            {
                // 日志/UI 订阅者的异常不能破坏 Modbus 周期发布。
            }
        }
    }

    /// <summary>把当前连接状态和机器人模式转换为与 SCADA 约定的稳定状态码。</summary>
    private static RobotInfoStatusCode ResolveStatus(Mes_Robot_Info_Model model)
    {
        if (model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected)
        {
            return RobotInfoStatusCode.Offline;
        }

        return model.Robot_Info_Data.Mes_Robot_Mode switch
        {
            KUKA_Mode_OP_Enum.Error => RobotInfoStatusCode.Fault,
            KUKA_Mode_OP_Enum.Run => RobotInfoStatusCode.Running,
            KUKA_Mode_OP_Enum.T1 or KUKA_Mode_OP_Enum.T2 => RobotInfoStatusCode.Manual,
            KUKA_Mode_OP_Enum.AUT or KUKA_Mode_OP_Enum.EX => RobotInfoStatusCode.Standby,
            _ => RobotInfoStatusCode.Unknown,
        };
    }

    /// <summary>向下取整并检查 UInt16 范围，禁止溢出后静默回绕为错误的 SCADA 数值。</summary>
    private static ushort ToUInt16(double value, string fieldName)
    {
        if (!double.IsFinite(value) || value < ushort.MinValue || value > ushort.MaxValue)
        {
            throw new OverflowException($"{fieldName}={value} 超出 UInt16 范围。");
        }

        return (ushort)Math.Floor(value);
    }

    /// <summary>检查整数并转换为 UInt16，主要用于当日产量。</summary>
    private static ushort ToUInt16(int value, string fieldName)
    {
        if (value is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new OverflowException($"{fieldName}={value} 超出 UInt16 范围。");
        }

        return (ushort)value;
    }

    /// <summary>向下取整并检查 UInt32 范围，供累计秒数和累计分钟数使用。</summary>
    private static uint ToUInt32(double value, string fieldName)
    {
        if (!double.IsFinite(value) || value < uint.MinValue || value > uint.MaxValue)
        {
            throw new OverflowException($"{fieldName}={value} 超出 UInt32 范围。");
        }

        return (uint)Math.Floor(value);
    }

    /// <summary>对象释放后拒绝再次启动或发布。</summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }
}
