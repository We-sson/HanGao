using System.Net;
using System.Windows.Threading;
using HanGao.ModbusTcp;
using Robot_Info_Mes.Model;

namespace Robot_Info_Mes.Services;

/// <summary>
/// 在 Robot_Info_Mes 进程中托管只读 Modbus TCP 服务端，并按 XML 自动布局发布数据。
/// </summary>
/// <remarks>
/// 当前阶段读取强类型寄存器数据对象的属性默认值，用于先验证 SCADA 地址、字序、
/// UInt16/UInt32 占位和网络连接；正式接入时替换该对象的赋值来源即可。
/// </remarks>
public sealed class RobotInfoModbusServerHost : IAsyncDisposable
{
    private readonly IModbusTcpServer _server;
    private readonly DispatcherTimer _publishTimer;
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);

    private ModbusRegisterLayout? _layout;
    private IReadOnlyList<RuntimeRegisterPoint>? _runtimePoints;
    private ModbusWordOrder _wordOrder = ModbusWordOrder.HighWordFirst;
    private long _sequence;
    private int _started;
    private int _disposed;

    /// <summary>使用生产协议服务创建应用层托管实例。</summary>
    public RobotInfoModbusServerHost()
        : this(new FluentModbusTcpServer())
    {
    }

    /// <summary>注入协议服务，供测试或后续替换底层实现。</summary>
    internal RobotInfoModbusServerHost(IModbusTcpServer server)
    {
        ArgumentNullException.ThrowIfNull(server);
        _server = server;
        _publishTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        _publishTimer.Tick += PublishTimerOnTick;
    }

    /// <summary>定时发布失败时触发；UI 可记录日志但不应执行耗时操作。</summary>
    public event Action<Exception>? PublishFailed;

    /// <summary>最近一次定时发布错误；下一次成功后自动清空。</summary>
    public string? LastPublishError { get; private set; }

    /// <summary>
    /// 按本地 XML 启动 FC03 只读服务。重复使用相同运行实例启动不会创建第二个监听器。
    /// </summary>
    public async Task StartAsync(
        RobotInfoModbusConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ThrowIfDisposed();

        await _lifecycleGate.WaitAsync(cancellationToken);
        try
        {
            ThrowIfDisposed();
            if (Volatile.Read(ref _started) != 0)
            {
                return;
            }

            ValidateConfiguration(configuration);
            ModbusRegisterLayout layout = configuration.RecalculateLayout();
            IPAddress bindAddress = ParseBindAddress(configuration.BindAddress);
            int readableRegisterCount = checked(layout.StartProtocolAddress + layout.RegisterCount);

            _layout = layout;
            // 复制一份运行期定义；UI 后续编辑 XML 对象时，正在发布的布局保持不变，直到用户保存并重启服务。
            IReadOnlyDictionary<int, ulong> propertyDefaults =
                ModbusPointCatalog.ReadValues(new RobotInfoModbusRegisterData());
            _runtimePoints = configuration.RegisterPoints
                .Where(item => item.Enabled)
                .Select(item => new RuntimeRegisterPoint(
                    item.Order,
                    item.DataType,
                    propertyDefaults.GetValueOrDefault(item.Order),
                    item))
                .ToArray();
            _wordOrder = configuration.WordOrder;
            _publishTimer.Interval = TimeSpan.FromMilliseconds(
                configuration.PublishIntervalMilliseconds);

            await _server.StartAsync(
                new ModbusTcpServerOptions
                {
                    BindAddress = bindAddress,
                    Port = configuration.Port,
                    AcceptedUnitIdentifier = configuration.UnitIdentifier,
                    RegisterCount = readableRegisterCount,
                    MaxConnections = configuration.MaxConnections,
                    ConnectionTimeout = TimeSpan.FromSeconds(
                        configuration.ConnectionTimeoutSeconds),
                    DiagnosticHistoryCapacity = 500,
                },
                cancellationToken);

            Interlocked.Exchange(ref _sequence, 0);
            Volatile.Write(ref _started, 1);

            // 监听成功后立即填充一次，避免 SCADA 在第一个周期前读到整块全零。
            PublishNow();
            _publishTimer.Start();
        }
        catch
        {
            Volatile.Write(ref _started, 0);
            _layout = null;
            _runtimePoints = null;
            await _server.StopAsync(CancellationToken.None);
            throw;
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>立即读取强类型属性默认值并原子发布一份完整寄存器快照。</summary>
    public void PublishNow()
    {
        ThrowIfDisposed();
        if (Volatile.Read(ref _started) == 0 || _runtimePoints is null || _layout is null)
        {
            throw new InvalidOperationException("Modbus TCP 服务端尚未启动。");
        }

        long sequence = Interlocked.Increment(ref _sequence);
        Dictionary<int, ulong> values = CreateRegisterValues(_runtimePoints);
        _server.Publish(
            ModbusRegisterLayoutEncoder.CreateSnapshot(
                _layout,
                values,
                sequence,
                _wordOrder,
                DateTimeOffset.UtcNow));
        LastPublishError = null;
    }

    /// <summary>取得用于右上角状态栏和配置页显示的服务状态。</summary>
    public ModbusTcpServerStatus GetStatus() => _server.GetStatus();

    /// <summary>取得最近诊断历史，供后续日志弹窗或导出使用。</summary>
    public IReadOnlyList<ModbusDiagnosticEvent> GetRecentDiagnostics() =>
        _server.GetRecentDiagnostics();

    /// <summary>停止周期发布、服务端 TCP 监听和当前全部 SCADA 客户端连接。</summary>
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
            await _server.StopAsync(cancellationToken);
            _layout = null;
            _runtimePoints = null;
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>停止并永久释放监听器、定时器和同步资源。</summary>
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
        _layout = null;
        _runtimePoints = null;
        _lifecycleGate.Dispose();
    }

    /// <summary>
    /// 按唯一 Order 生成发布值。Attribute 内置点位取属性初始化值，XML 自定义点位默认发布 0。
    /// </summary>
    private static Dictionary<int, ulong> CreateRegisterValues(
        IReadOnlyList<RuntimeRegisterPoint> runtimePoints)
    {
        var values = new Dictionary<int, ulong>();

        foreach (RuntimeRegisterPoint point in runtimePoints)
        {
            ulong maximum = point.DataType switch
            {
                ModbusPointDataType.UInt16 => ushort.MaxValue,
                ModbusPointDataType.UInt32 => uint.MaxValue,
                _ => throw new InvalidOperationException(
                    $"顺序 {point.Order} 使用了不支持的数据类型 {point.DataType}。"),
            };
            if (point.PropertyDefaultValue > maximum)
            {
                throw new OverflowException(
                    $"顺序 {point.Order} 的属性默认值 {point.PropertyDefaultValue} 超出 {point.DataType} 范围。");
            }

            values.Add(point.Order, point.PropertyDefaultValue);
            point.UiPoint.CurrentValueText = point.PropertyDefaultValue.ToString(
                System.Globalization.CultureInfo.InvariantCulture);
        }

        return values;
    }

    /// <summary>定时入口；异常只写入状态和日志，不能中断 WPF 消息循环。</summary>
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
                // UI/日志订阅者的错误不能破坏 Modbus 发布循环。
            }
        }
    }

    private static void ValidateConfiguration(RobotInfoModbusConfiguration configuration)
    {
        if (configuration.Port is <= IPEndPoint.MinPort or > IPEndPoint.MaxPort)
        {
            throw new InvalidOperationException("Modbus TCP 端口必须在 1～65535 之间。");
        }

        if (configuration.PublishIntervalMilliseconds is < 100 or > 60_000)
        {
            throw new InvalidOperationException("寄存器刷新周期必须在 100～60000 毫秒之间。");
        }

        if (configuration.MaxConnections is < 1 or > 10_000)
        {
            throw new InvalidOperationException("最大连接数必须在 1～10000 之间。");
        }

        if (configuration.ConnectionTimeoutSeconds is < 1 or > 86_400)
        {
            throw new InvalidOperationException("连接超时必须在 1～86400 秒之间。");
        }

        if (!Enum.IsDefined(configuration.WordOrder))
        {
            throw new InvalidOperationException("配置中的 UInt32 字顺序无效。");
        }
    }

    private static IPAddress ParseBindAddress(string? text)
    {
        string candidate = text?.Trim() ?? string.Empty;
        if (candidate is "" or "*" or "0.0.0.0")
        {
            return IPAddress.Any;
        }

        if (!IPAddress.TryParse(candidate, out IPAddress? address))
        {
            throw new InvalidOperationException($"监听地址“{candidate}”不是有效 IP 地址。");
        }

        return address;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }

    /// <summary>服务启动时冻结的点位，确保 UI 编辑在重启前不改变线上寄存器含义。</summary>
    private sealed record RuntimeRegisterPoint(
        int Order,
        ModbusPointDataType DataType,
        ulong PropertyDefaultValue,
        ModbusRegisterPointConfiguration UiPoint);
}
