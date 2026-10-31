using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanGao.ModbusTcp;

namespace HanGao.RobotInfo.Modbus;

/// <summary>Robot_Info_Mes 的 Modbus TCP 功能：配置、服务端状态、命令和点位编辑。</summary>
/// <remarks>
/// 宿主只需暴露一个该类型的属性；文件读写、IP 枚举和日志通过构造参数注入，
/// 功能库本身不依赖具体应用。
/// </remarks>
public sealed class RobotModbusFeature : ObservableObject, IAsyncDisposable
{
    private readonly string _configurationPath;
    private readonly Func<RobotInfoModbusConfiguration> _loadConfiguration;
    private readonly Action<RobotInfoModbusConfiguration> _saveConfiguration;
    private readonly Func<IReadOnlyList<string>> _discoverLocalAddresses;
    private readonly Action<string, MessageBoxImage> _log;
    private readonly SemaphoreSlim _operationGate = new(1, 1);
    private readonly DispatcherTimer _uiStatusTimer = new();

    private RobotInfoModbusConfiguration _configuration = RobotInfoModbusConfiguration.CreateDefault();
    private ModbusRegisterPointConfiguration? _selectedPoint;
    private RobotInfoModbusServerHost? _serverHost;
    private bool _initialized;
    private bool _disposed;

    private string _serverStateText = "未启动";
    private string _endpointText = "—";
    private string _listeningAddressesToolTip = "尚未枚举本机 IPv4 地址";
    private string _scadaStateText = "等待连接";
    private string _scadaDetailText = "0 个连接";
    private string _lastErrorText = "无";
    private bool _serverIsRunning;
    private bool _scadaIsConnected;

    private IAsyncRelayCommand? _saveAndRestartCommand;
    private IAsyncRelayCommand? _toggleCommand;
    private IRelayCommand? _refreshLayoutCommand;
    private IRelayCommand? _addUInt16Command;
    private IRelayCommand? _addUInt32Command;
    private IRelayCommand? _deletePointCommand;

    /// <summary>创建功能对象；实际初始化在 Initialize 中完成。</summary>
    /// <param name="configurationPath">XML 配置路径，仅用于显示和默认值回退。</param>
    /// <param name="loadConfiguration">读取并返回已校验的配置。</param>
    /// <param name="saveConfiguration">保存配置到 XML。</param>
    /// <param name="discoverLocalAddresses">枚举本机 IPv4 地址文本。</param>
    /// <param name="log">写入宿主运行日志；第二个参数为消息级别。</param>
    public RobotModbusFeature(
        string configurationPath,
        Func<RobotInfoModbusConfiguration> loadConfiguration,
        Action<RobotInfoModbusConfiguration> saveConfiguration,
        Func<IReadOnlyList<string>> discoverLocalAddresses,
        Action<string, MessageBoxImage> log)
    {
        _configurationPath = configurationPath;
        _loadConfiguration = loadConfiguration;
        _saveConfiguration = saveConfiguration;
        _discoverLocalAddresses = discoverLocalAddresses;
        _log = log;
    }

    /// <summary>与 Configs_Data.Xml 同目录的 Modbus TCP 独立配置。</summary>
    public RobotInfoModbusConfiguration ModbusTcpConfiguration
    {
        get => _configuration;
        private set => SetProperty(ref _configuration, value);
    }

    /// <summary>点位类型下拉框使用的固定选项。</summary>
    public IReadOnlyList<ModbusPointDataType> ModbusPointDataTypeOptions { get; } =
        Enum.GetValues<ModbusPointDataType>();

    /// <summary>UInt32 字序下拉框选项。</summary>
    public IReadOnlyList<ModbusWordOrder> ModbusWordOrderOptions { get; } =
        Enum.GetValues<ModbusWordOrder>();

    /// <summary>寄存器表当前选中行，用于删除 XML 自定义点位。</summary>
    public ModbusRegisterPointConfiguration? SelectedModbusRegisterPoint
    {
        get => _selectedPoint;
        set => SetProperty(ref _selectedPoint, value);
    }

    /// <summary>当前协议服务原始状态；尚未创建时为空。</summary>
    public ModbusTcpServerStatus? ModbusTcpStatus => _serverHost?.GetStatus();

    /// <summary>主界面和配置弹窗显示的服务状态。</summary>
    public string ModbusServerStateText
    {
        get => _serverStateText;
        private set => SetProperty(ref _serverStateText, value);
    }

    /// <summary>当前连接实际使用的本机端点；无连接时显示首个可访问的非回环端点。</summary>
    public string ModbusEndpointText
    {
        get => _endpointText;
        private set => SetProperty(ref _endpointText, value);
    }

    /// <summary>当前可供 SCADA 访问的本机 IPv4 地址。</summary>
    public ObservableCollection<string> ModbusListeningIpAddresses { get; } = [];

    /// <summary>右上角地址卡的提示文本。</summary>
    public string ModbusListeningAddressesToolTip
    {
        get => _listeningAddressesToolTip;
        private set => SetProperty(ref _listeningAddressesToolTip, value);
    }

    /// <summary>SCADA 客户端当前连接状态。</summary>
    public string ModbusScadaStateText
    {
        get => _scadaStateText;
        private set => SetProperty(ref _scadaStateText, value);
    }

    /// <summary>连接数和最近一次 FC03 请求摘要。</summary>
    public string ModbusScadaDetailText
    {
        get => _scadaDetailText;
        private set => SetProperty(ref _scadaDetailText, value);
    }

    /// <summary>服务或发布过程最近错误。</summary>
    public string ModbusLastErrorText
    {
        get => _lastErrorText;
        private set => SetProperty(ref _lastErrorText, value);
    }

    /// <summary>供 XAML 状态颜色触发器使用。</summary>
    public bool ModbusServerIsRunning
    {
        get => _serverIsRunning;
        private set => SetProperty(ref _serverIsRunning, value);
    }

    /// <summary>至少有一个 SCADA 客户端连接到 Modbus TCP 服务端时为 true。</summary>
    public bool ModbusScadaIsConnected
    {
        get => _scadaIsConnected;
        private set => SetProperty(ref _scadaIsConnected, value);
    }

    /// <summary>保存 ModbusTcp_Config.Xml，并只重启 Modbus TCP 服务端使新参数立即生效。</summary>
    public IAsyncRelayCommand SaveAndRestartModbusServerCommand =>
        _saveAndRestartCommand ??= new AsyncRelayCommand(SaveAndRestartAsync);

    /// <summary>手动启动或停止 Modbus TCP 服务端。</summary>
    public IAsyncRelayCommand ToggleModbusServerCommand =>
        _toggleCommand ??= new AsyncRelayCommand(ToggleAsync);

    /// <summary>使用公共布局算法刷新 UI 中的自动地址。</summary>
    public IRelayCommand RefreshModbusLayoutCommand =>
        _refreshLayoutCommand ??= new RelayCommand(RefreshLayoutPreview);

    /// <summary>向 XML 增加一个 UInt16 自定义点位。</summary>
    public IRelayCommand AddModbusUInt16PointCommand =>
        _addUInt16Command ??= new RelayCommand(() => AddCustomPoint(ModbusPointDataType.UInt16));

    /// <summary>向 XML 增加一个 UInt32 自定义点位。</summary>
    public IRelayCommand AddModbusUInt32PointCommand =>
        _addUInt32Command ??= new RelayCommand(() => AddCustomPoint(ModbusPointDataType.UInt32));

    /// <summary>删除选中的自定义点；内置点位改为禁用。</summary>
    public IRelayCommand DeleteModbusPointCommand =>
        _deletePointCommand ??= new RelayCommand(DeleteSelectedPoint);

    /// <summary>读取并严格校验独立 XML；失败时报告错误并整份使用默认配置。</summary>
    public void Initialize()
    {
        if (_initialized) return;

        string? loadError = null;
        try
        {
            RobotInfoModbusConfiguration loaded = _loadConfiguration();
            loaded.ConfigurationFilePath = _configurationPath;
            _ = loaded.ValidateAndCreateLayout();
            ModbusTcpConfiguration = loaded;
        }
        catch (Exception exception)
        {
            RobotInfoModbusConfiguration fallback = RobotInfoModbusConfiguration.CreateDefault();
            fallback.ConfigurationFilePath = _configurationPath;
            ModbusTcpConfiguration = fallback;
            loadError =
                $"Modbus 配置读取或校验失败，已使用默认值。文件：{_configurationPath}；" +
                $"原因：{exception.Message}";
            ModbusLastErrorText = loadError;
        }

        AttachConfigurationEvents();
        RefreshLayoutPreview();
        RefreshListeningAddresses();

        if (loadError is not null)
        {
            ModbusTcpConfiguration.LayoutValidationMessage =
                "本地配置无效，当前使用默认值；保存后将写入当前结构";
            _log(loadError, MessageBoxImage.Error);
        }

        _uiStatusTimer.Interval = TimeSpan.FromMilliseconds(500);
        _uiStatusTimer.Tick += UiStatusTimerOnTick;
        _uiStatusTimer.Start();
        _initialized = true;
    }

    /// <summary>应用启动时按配置安全启动；失败只记录日志和状态。</summary>
    public async Task StartIfEnabledAsync()
    {
        if (!ModbusTcpConfiguration.Enabled)
        {
            RefreshUiStatus();
            return;
        }

        try
        {
            await StartAsync();
        }
        catch (Exception exception)
        {
            ModbusLastErrorText = exception.Message;
            _log("Modbus TCP 服务端自动启动失败：" + exception.Message, MessageBoxImage.Error);
            RefreshUiStatus();
        }
    }

    /// <summary>按当前内存配置启动 FC03 只读服务端和寄存器数据发布。</summary>
    public async Task StartAsync()
    {
        await _operationGate.WaitAsync();
        try
        {
            if (!ModbusTcpConfiguration.Enabled)
            {
                ModbusServerStateText = "已禁用";
                return;
            }

            _ = ModbusTcpConfiguration.RecalculateLayout();
            _serverHost ??= CreateServerHost();
            await _serverHost.StartAsync(ModbusTcpConfiguration);
            RefreshListeningAddresses();
            RefreshUiStatus();
            _log(
                $"Modbus TCP 服务端已监听全部本机 IPv4，端口 {ModbusTcpConfiguration.Port}，" +
                $"Unit={ModbusTcpConfiguration.UnitIdentifier}，FC03，起点={ModbusTcpConfiguration.StartReferenceAddress}。",
                MessageBoxImage.None);
        }
        finally
        {
            _operationGate.Release();
        }
    }

    /// <summary>停止监听并断开当前 SCADA 客户端连接；重复调用安全。</summary>
    public async Task StopAsync()
    {
        await _operationGate.WaitAsync();
        try
        {
            if (_serverHost is not null) await _serverHost.StopAsync();
            RefreshListeningAddresses();
            RefreshUiStatus();
            _log("Modbus TCP 服务端已停止。", MessageBoxImage.None);
        }
        finally
        {
            _operationGate.Release();
        }
    }

    /// <summary>窗口关闭时永久释放 Modbus 资源。</summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        _uiStatusTimer.Stop();
        _uiStatusTimer.Tick -= UiStatusTimerOnTick;

        await _operationGate.WaitAsync();
        try
        {
            if (_serverHost is not null)
            {
                await _serverHost.DisposeAsync();
                _serverHost = null;
            }
        }
        finally
        {
            _operationGate.Release();
            _operationGate.Dispose();
        }

        RefreshUiStatus();
    }

    /// <summary>订阅会影响自动排位的 XML 字段。</summary>
    private void AttachConfigurationEvents()
    {
        ModbusTcpConfiguration.RegisterPoints.CollectionChanged += RegisterPointsOnCollectionChanged;

        if (ModbusTcpConfiguration is INotifyPropertyChanged notifier)
            notifier.PropertyChanged += ConfigurationOnPropertyChanged;

        foreach (ModbusRegisterPointConfiguration point in ModbusTcpConfiguration.RegisterPoints)
            AttachPointEvent(point);
    }

    /// <summary>点位增删时维护属性订阅，并立即重算后续自动地址。</summary>
    private void RegisterPointsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        if (eventArgs.OldItems is not null)
        {
            foreach (object item in eventArgs.OldItems)
            {
                if (item is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged -= PointOnPropertyChanged;
            }
        }

        if (eventArgs.NewItems is not null)
        {
            foreach (object item in eventArgs.NewItems)
            {
                if (item is ModbusRegisterPointConfiguration point) AttachPointEvent(point);
            }
        }

        RefreshLayoutPreview();
    }

    /// <summary>为一个点位建立唯一的变更订阅。</summary>
    private void AttachPointEvent(ModbusRegisterPointConfiguration point)
    {
        if (point is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged -= PointOnPropertyChanged;
            notifier.PropertyChanged += PointOnPropertyChanged;
        }
    }

    /// <summary>六位块起点变化时刷新整张寄存器地址预览。</summary>
    private void ConfigurationOnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(RobotInfoModbusConfiguration.StartReferenceAddress))
            RefreshLayoutPreview();
    }

    /// <summary>点位的顺序、启用状态或类型变化时重新排位。</summary>
    private void PointOnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is nameof(ModbusRegisterPointConfiguration.Order)
            or nameof(ModbusRegisterPointConfiguration.Enabled)
            or nameof(ModbusRegisterPointConfiguration.DataType))
        {
            RefreshLayoutPreview();
        }
    }

    /// <summary>刷新地址预览；编辑中的无效值显示为错误，但不会中断 UI。</summary>
    private void RefreshLayoutPreview()
    {
        try
        {
            ModbusTcpConfiguration.RecalculateLayout();
        }
        catch (Exception exception)
        {
            foreach (ModbusRegisterPointConfiguration point in ModbusTcpConfiguration.RegisterPoints)
            {
                point.ReferenceAddress = 0;
                point.ProtocolAddress = 0;
                point.AddressText = point.Enabled ? "配置错误" : "已禁用";
            }

            ModbusTcpConfiguration.LayoutValidationMessage = "配置无效：" + exception.Message;
            ModbusTcpConfiguration.RuntimeRegisterCount = 0;
        }
    }

    /// <summary>分配下一个唯一 Order 并增加一个默认值为 0 的自定义点位。</summary>
    private void AddCustomPoint(ModbusPointDataType dataType)
    {
        int order = ModbusTcpConfiguration.RegisterPoints.Count == 0
            ? 0
            : ModbusTcpConfiguration.RegisterPoints.Max(point => point.Order) + 1;
        var point = new ModbusRegisterPointConfiguration
        {
            Order = order,
            Enabled = true,
            DataType = dataType,
            DisplayName = "自定义点位",
            Comment = "XML 自定义点位；未接入业务时默认发布 0",
        };

        ModbusTcpConfiguration.RegisterPoints.Add(point);
        SelectedModbusRegisterPoint = point;
        _log($"已增加 {dataType} 点位，唯一顺序为 {order}，地址由程序自动排位。", MessageBoxImage.None);
    }

    /// <summary>删除自定义点位；内置点位仅禁用。</summary>
    private void DeleteSelectedPoint()
    {
        ModbusRegisterPointConfiguration? point = SelectedModbusRegisterPoint;
        if (point is null)
        {
            _log("请先在 Modbus 点位表选择一行。", MessageBoxImage.Warning);
            return;
        }

        if (point.IsBuiltIn)
        {
            point.Enabled = false;
            _log($"内置点位 {point.DisplayName} 已改为禁用；保存并重启服务后生效。", MessageBoxImage.None);
            return;
        }

        ModbusTcpConfiguration.RegisterPoints.Remove(point);
        SelectedModbusRegisterPoint = null;
        _log($"已删除顺序 {point.Order} 的自定义 Modbus 点位。", MessageBoxImage.None);
    }

    /// <summary>校验并保存独立 XML，然后只重启 Modbus TCP 服务端。</summary>
    private async Task SaveAndRestartAsync()
    {
        try
        {
            _ = ModbusTcpConfiguration.ValidateAndCreateLayout();
            _saveConfiguration(ModbusTcpConfiguration);

            await _operationGate.WaitAsync();
            try
            {
                if (_serverHost is not null) await _serverHost.StopAsync();

                if (ModbusTcpConfiguration.Enabled)
                {
                    _serverHost ??= CreateServerHost();
                    await _serverHost.StartAsync(ModbusTcpConfiguration);
                }
            }
            finally
            {
                _operationGate.Release();
            }

            RefreshUiStatus();
            _log(
                "Modbus TCP 服务端配置已保存并重新加载：" +
                ModbusTcpConfiguration.ConfigurationFilePath,
                MessageBoxImage.None);
        }
        catch (Exception exception)
        {
            ModbusTcpConfiguration.LayoutValidationMessage = "保存/重启失败：" + exception.Message;
            ModbusLastErrorText = exception.Message;
            _log("保存或重启 Modbus TCP 服务端失败：" + exception.Message, MessageBoxImage.Error);
        }
    }

    /// <summary>按当前状态启动或停止服务端，并同步下次启动时的自动开启标志。</summary>
    private async Task ToggleAsync()
    {
        if (ModbusTcpConfiguration.Enabled)
        {
            ModbusTcpConfiguration.Enabled = false;
            await StopAsync();
        }
        else
        {
            ModbusTcpConfiguration.Enabled = true;
            try
            {
                await StartAsync();
            }
            catch (Exception exception)
            {
                ModbusLastErrorText = exception.Message;
                _log("启动 Modbus TCP 服务端失败：" + exception.Message, MessageBoxImage.Error);
                RefreshUiStatus();
            }
        }
    }

    /// <summary>创建应用层服务端宿主，并把后台发布错误接入统一日志。</summary>
    private RobotInfoModbusServerHost CreateServerHost()
    {
        var host = new RobotInfoModbusServerHost();
        host.PublishFailed += exception =>
        {
            ModbusLastErrorText = exception.Message;
            _log("Modbus TCP 服务端寄存器发布失败：" + exception.Message, MessageBoxImage.Error);
        };
        return host;
    }

    /// <summary>定时把协议诊断快照投影到显示属性。</summary>
    private void UiStatusTimerOnTick(object? sender, EventArgs eventArgs) => RefreshUiStatus();

    /// <summary>刷新 Modbus 可访问的本机 IPv4 地址列表。</summary>
    private void RefreshListeningAddresses()
    {
        IReadOnlyList<string> discoveredAddresses;
        try
        {
            discoveredAddresses = _discoverLocalAddresses();
        }
        catch (Exception exception)
        {
            discoveredAddresses = [IPAddress.Loopback.ToString()];
            ModbusListeningAddressesToolTip =
                "本机 IPv4 枚举失败，当前仅显示回环地址。原因：" + exception.Message;
        }

        string[] normalizedAddresses = discoveredAddresses
            .Where(text => IPAddress.TryParse(text, out IPAddress? address) &&
                           address.AddressFamily == AddressFamily.InterNetwork)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(text => text == IPAddress.Loopback.ToString() ? 1 : 0)
            .ThenBy(text => text, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedAddresses.Length == 0)
            normalizedAddresses = [IPAddress.Loopback.ToString()];

        ModbusListeningIpAddresses.Clear();
        foreach (string address in normalizedAddresses)
            ModbusListeningIpAddresses.Add(address);
    }

    /// <summary>从系统 TCP 表查找当前 SCADA 连接实际使用的本机端点。</summary>
    private static IReadOnlyList<TcpConnectionInformation> GetActiveConnections(int port)
    {
        try
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .Where(connection =>
                    connection.State == TcpState.Established &&
                    connection.LocalEndPoint.Port == port &&
                    connection.LocalEndPoint.Address.AddressFamily == AddressFamily.InterNetwork)
                .OrderBy(connection => connection.LocalEndPoint.Address.ToString(), StringComparer.Ordinal)
                .ThenBy(connection => connection.RemoteEndPoint.Address.ToString(), StringComparer.Ordinal)
                .ThenBy(connection => connection.RemoteEndPoint.Port)
                .ToArray();
        }
        catch
        {
            return [];
        }
    }

    /// <summary>生成界面使用的具体本机端点；从不显示 0.0.0.0。</summary>
    private IReadOnlyList<TcpConnectionInformation> RefreshEndpointPresentation(ModbusTcpServerStatus? status)
    {
        int actualPort = status?.LocalEndpoint?.Port ?? ModbusTcpConfiguration.Port;
        IReadOnlyList<TcpConnectionInformation> activeConnections =
            status?.ConnectedClients > 0 ? GetActiveConnections(actualPort) : [];

        string[] activeLocalAddresses = activeConnections
            .Select(connection => connection.LocalEndPoint.Address.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        string primaryAddress = activeLocalAddresses.FirstOrDefault() ??
                                ModbusListeningIpAddresses.FirstOrDefault(
                                    address => address != IPAddress.Loopback.ToString()) ??
                                ModbusListeningIpAddresses.FirstOrDefault() ??
                                IPAddress.Loopback.ToString();

        ModbusEndpointText = $"{primaryAddress}:{actualPort}";
        if (activeLocalAddresses.Length > 1)
            ModbusEndpointText += $" +{activeLocalAddresses.Length - 1}";

        string stateText = status?.State == ModbusServerState.Running
            ? "服务端正在监听全部本机 IPv4："
            : "服务端启动后监听全部本机 IPv4：";
        string allEndpoints = string.Join(
            Environment.NewLine,
            ModbusListeningIpAddresses.Select(address => $"{address}:{actualPort}"));
        ModbusListeningAddressesToolTip = stateText + Environment.NewLine + allEndpoints;

        if (activeLocalAddresses.Length > 0)
        {
            ModbusListeningAddressesToolTip +=
                Environment.NewLine + "当前连接使用：" +
                string.Join(", ", activeLocalAddresses.Select(address => $"{address}:{actualPort}"));
        }

        return activeConnections;
    }

    /// <summary>把底层诊断快照转换成适合状态卡的短文本。</summary>
    private void RefreshUiStatus()
    {
        if (_serverHost is null)
        {
            ModbusServerIsRunning = false;
            ModbusScadaIsConnected = false;
            ModbusServerStateText = ModbusTcpConfiguration.Enabled ? "未启动" : "已禁用";
            _ = RefreshEndpointPresentation(status: null);
            ModbusScadaStateText = "等待连接";
            ModbusScadaDetailText = "0 个连接";
            return;
        }

        ModbusTcpServerStatus status = _serverHost.GetStatus();
        ModbusServerIsRunning = status.State == ModbusServerState.Running;
        ModbusScadaIsConnected = status.ConnectedClients > 0;
        ModbusServerStateText = !ModbusTcpConfiguration.Enabled && status.State == ModbusServerState.Stopped
            ? "已禁用"
            : status.State switch
            {
                ModbusServerState.Starting => "启动中",
                ModbusServerState.Running => "运行中",
                ModbusServerState.Stopping => "停止中",
                ModbusServerState.Stopped => "已停止",
                ModbusServerState.Faulted => "故障",
                ModbusServerState.Disposed => "已释放",
                _ => status.State.ToString(),
            };
        IReadOnlyList<TcpConnectionInformation> activeConnections =
            RefreshEndpointPresentation(status);
        ModbusScadaStateText = ModbusScadaIsConnected
            ? $"已连接 {status.ConnectedClients}"
            : "等待连接";
        ModbusScadaDetailText = status.LastRequest is null
            ? $"{status.ConnectedClients} 个连接 · 尚无读取"
            : $"{status.ConnectedClients} 个连接 · " +
              $"{status.LastRequest.TimestampUtc.ToLocalTime():HH:mm:ss} " +
              $"FC{status.LastRequest.FunctionCode:D2} PDU {status.LastRequest.StartAddress}";
        if (activeConnections.Count > 0)
        {
            string scadaEndpoints = string.Join(
                ", ",
                activeConnections
                    .Select(connection => connection.RemoteEndPoint.ToString())
                    .Distinct(StringComparer.OrdinalIgnoreCase));
            ModbusScadaDetailText += Environment.NewLine + "客户端：" + scadaEndpoints;
        }

        ModbusLastErrorText = status.LastError ??
                              _serverHost.LastPublishError ??
                              "无";
    }
}
