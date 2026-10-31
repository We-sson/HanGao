using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RobotSimulator.Models;
using RobotSimulator.Services;

namespace RobotSimulator.ViewModels;

/// <summary>协调机器人、Tool 和工装；机器人参数固定，Tool TCP 与工装安装位置分别保存。</summary>
public sealed class MainViewModel : ObservableObject
{
    /// <summary>三维操作接口，保持 ViewModel 与控件分离。</summary>
    private readonly IRobotSceneService _sceneService;
    /// <summary>示例动作使用实际经过时间推进。</summary>
    private readonly Stopwatch _animationClock = new();
    /// <summary>当前使用的内置机器人。</summary>
    private RobotModelDefinition _currentRobot;
    /// <summary>机器人下拉框选中项。</summary>
    private RobotModelDefinition? _selectedRobot;
    /// <summary>当前成功安装的独立工具，可以为空。</summary>
    private ToolModelDefinition? _currentTool;
    /// <summary>工具下拉框选中项。</summary>
    private ToolModelDefinition? _selectedTool;
    /// <summary>当前工具配置的名称和模型文件信息。</summary>
    private ToolConfiguration? _toolConfiguration;
    /// <summary>工具 TCP 的可编辑副本，所有机器人共用同一工具的这一组参数。</summary>
    private PoseConfiguration _toolTcp = new();
    private WorkModelDefinition? _currentWork;
    private WorkModelDefinition? _selectedWork;
    private WorkConfiguration? _workConfiguration;
    private PoseConfiguration _workPose = new();
    private string _workStatus = "尚未安装工装";
    /// <summary>六轴运行状态，配置不可编辑，只有当前角度变化。</summary>
    private IReadOnlyList<JointViewModel> _joints = [];
    /// <summary>底部状态文字。</summary>
    private string _statusMessage = "正在初始化三维视图…";
    /// <summary>加载步骤说明。</summary>
    private string _loadingMessage = "正在准备机器人模型…";
    /// <summary>当前加载阶段的进度百分比。</summary>
    private double _loadProgress;
    /// <summary>是否正在加载机器人或工具。</summary>
    private bool _isBusy;
    /// <summary>机器人是否已经成功挂接到视图。</summary>
    private bool _isLoaded;
    /// <summary>示例动作是否播放中。</summary>
    private bool _isPlaying;
    /// <summary>当前工具安装状态。</summary>
    private string _toolStatus = "尚未安装 Tool";
    /// <summary>批量修改角度时跳过逐轴刷新。</summary>
    private bool _suppressJointUpdates;
    /// <summary>渲染器是否已初始化。</summary>
    private bool _viewerReady;
    /// <summary>是否已进入最终关闭流程。</summary>
    private bool _isClosing;
    /// <summary>加载期间收到的延迟关闭请求。</summary>
    private bool _closePending;
    /// <summary>动画更新是否已经排入 UI 队列。</summary>
    private bool _animationUpdatePending;
    /// <summary>基坐标箭头显示状态。</summary>
    private bool _showBaseCoordinates = true;
    /// <summary>法兰坐标箭头显示状态。</summary>
    private bool _showFlangeCoordinates = true;
    /// <summary>工具中心箭头显示状态。</summary>
    private bool _showTcpCoordinates = true;
    /// <summary>相机环绕速度，单位度/秒；0 表示不旋转，加载完成后默认缓慢旋转。</summary>
    private double _orbitSpeed = 15;
    /// <summary>相机环绕起始方位角，单位度。</summary>
    private double _orbitAngle = -45;
    /// <summary>环绕仰角固定为标准等轴测角度。</summary>
    private const double OrbitElevationDegrees = 35.264;

    /// <summary>接收三个独立资源库，并连接 XAML 使用的所有命令。</summary>
    public MainViewModel(IReadOnlyList<RobotModelDefinition> robotModels, IReadOnlyList<ToolModelDefinition> toolModels,
        IReadOnlyList<WorkModelDefinition> workModels, IRobotSceneService sceneService)
    {
        if (robotModels.Count == 0) throw new ArgumentException("至少需要一个机器人型号。", nameof(robotModels));
        RobotModels = robotModels;
        ToolModels = toolModels;
        WorkModels = workModels;
        _sceneService = sceneService;
        _currentRobot = robotModels[0];
        _selectedRobot = _currentRobot;
        _currentTool = toolModels.FirstOrDefault();
        _selectedTool = _currentTool;
        if (_currentTool is not null) LoadToolConfiguration(_currentTool.Configuration);
        _currentWork = workModels.FirstOrDefault();
        _selectedWork = _currentWork;
        if (_currentWork is not null) LoadWorkConfiguration(_currentWork.Configuration);
        LoadRobotConfiguration();
        ViewerReadyCommand = new AsyncRelayCommand(InitializeSceneAsync);
        SelectRobotCommand = new AsyncRelayCommand<RobotModelDefinition>(SelectRobotAsync);
        SelectToolCommand = new AsyncRelayCommand<ToolModelDefinition>(SelectToolAsync);
        ReloadToolCommand = new AsyncRelayCommand(ReloadToolAsync);
        SaveToolConfigurationCommand = new RelayCommand(SaveToolConfiguration);
        SelectWorkCommand = new AsyncRelayCommand<WorkModelDefinition>(SelectWorkAsync);
        ReloadWorkCommand = new AsyncRelayCommand(ReloadWorkAsync);
        SaveWorkConfigurationCommand = new RelayCommand(SaveWorkConfiguration);
        ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing);
        ResetCommand = new RelayCommand(Reset);
        PlayCommand = new RelayCommand(Play);
        IsometricCommand = new RelayCommand(() => _sceneService.SetView(RobotView.Isometric));
        FrontCommand = new RelayCommand(() => _sceneService.SetView(RobotView.Front));
        TopCommand = new RelayCommand(() => _sceneService.SetView(RobotView.Top));
        ZoomAllCommand = new RelayCommand(_sceneService.ZoomAll);
        UpdateCoordinateVisibilityCommand = new RelayCommand(UpdateCoordinateVisibility);
    }

    /// <summary>请求视图显示错误，参数为详细内容和标题。</summary>
    public event Action<string, string>? ErrorOccurred;
    /// <summary>加载结束后请求视图继续关闭。</summary>
    public event Action? CloseRequested;
    /// <summary>机器人下拉框的数据源，只包含内置固定参数。</summary>
    public IReadOnlyList<RobotModelDefinition> RobotModels { get; }
    /// <summary>独立工具下拉框的数据源，可与任意机器人搭配。</summary>
    public IReadOnlyList<ToolModelDefinition> ToolModels { get; }
    public IReadOnlyList<WorkModelDefinition> WorkModels { get; }
    /// <summary>渲染器就绪后启动第一次加载。</summary>
    public IAsyncRelayCommand ViewerReadyCommand { get; }
    /// <summary>选择机器人，只更换机器人参数与六轴，保留当前工具 TCP。</summary>
    public IAsyncRelayCommand<RobotModelDefinition> SelectRobotCommand { get; }
    /// <summary>选择工具，只替换 Tool 模型和配置，保留当前关节姿态。</summary>
    public IAsyncRelayCommand<ToolModelDefinition> SelectToolCommand { get; }
    /// <summary>重新读取并加载当前工具的外部配置。</summary>
    public IAsyncRelayCommand ReloadToolCommand { get; }
    /// <summary>只保存当前工具的配置，不输出机器人参数。</summary>
    public IRelayCommand SaveToolConfigurationCommand { get; }
    public IAsyncRelayCommand<WorkModelDefinition> SelectWorkCommand { get; }
    public IAsyncRelayCommand ReloadWorkCommand { get; }
    public IRelayCommand SaveWorkConfigurationCommand { get; }
    /// <summary>接收窗口关闭事件参数，协调原生资源安全释放。</summary>
    public IRelayCommand<CancelEventArgs> ClosingCommand { get; }
    /// <summary>将六轴恢复到内置初始角度。</summary>
    public IRelayCommand ResetCommand { get; }
    /// <summary>开始或停止示例动作。</summary>
    public IRelayCommand PlayCommand { get; }
    /// <summary>切换等轴测视角。</summary>
    public IRelayCommand IsometricCommand { get; }
    /// <summary>切换正视角。</summary>
    public IRelayCommand FrontCommand { get; }
    /// <summary>切换俯视角。</summary>
    public IRelayCommand TopCommand { get; }
    /// <summary>让完整模型适配窗口。</summary>
    public IRelayCommand ZoomAllCommand { get; }
    /// <summary>同步三个坐标显示开关。</summary>
    public IRelayCommand UpdateCoordinateVisibilityCommand { get; }
    /// <summary>相机环绕速度，度/秒；0 表示不旋转。</summary>
    public double OrbitSpeed
    {
        get => _orbitSpeed;
        set
        {
            if (SetProperty(ref _orbitSpeed, value)) UpdateOrbit();
        }
    }
    /// <summary>相机环绕起始方位角，单位度。</summary>
    public double OrbitAngle
    {
        get => _orbitAngle;
        set
        {
            if (SetProperty(ref _orbitAngle, value)) UpdateOrbit();
        }
    }

    /// <summary>机器人下拉框选中项，实际加载通过选择命令触发。</summary>
    public RobotModelDefinition? SelectedRobot { get => _selectedRobot; set => SetProperty(ref _selectedRobot, value); }
    /// <summary>工具下拉框选中项，与机器人选择相互独立。</summary>
    public ToolModelDefinition? SelectedTool { get => _selectedTool; set => SetProperty(ref _selectedTool, value); }
    public WorkModelDefinition? SelectedWork { get => _selectedWork; set => SetProperty(ref _selectedWork, value); }
    /// <summary>内置机器人名称。</summary>
    public string ModelName => _currentRobot.Configuration.ModelName;
    /// <summary>窗口标题，不包含可编辑的机器人配置入口。</summary>
    public string WindowTitle => $"{ModelName} · AnyCAD 运动仿真";
    /// <summary>当前工具名称。</summary>
    public string ToolName => _toolConfiguration?.ToolName ?? "未安装 Tool";
    public string WorkName => _workConfiguration?.WorkName ?? "未安装工装";
    /// <summary>六轴运行状态集合，外部只能读取集合。</summary>
    public IReadOnlyList<JointViewModel> Joints { get => _joints; private set => SetProperty(ref _joints, value); }
    /// <summary>是否显示基坐标箭头。</summary>
    public bool ShowBaseCoordinates { get => _showBaseCoordinates; set => SetProperty(ref _showBaseCoordinates, value); }
    /// <summary>是否显示法兰箭头，独立于 TCP。</summary>
    public bool ShowFlangeCoordinates { get => _showFlangeCoordinates; set => SetProperty(ref _showFlangeCoordinates, value); }
    /// <summary>是否显示 TCP 箭头。</summary>
    public bool ShowTcpCoordinates { get => _showTcpCoordinates; set => SetProperty(ref _showTcpCoordinates, value); }
    /// <summary>TCP 相对于法兰的 X 偏移，单位 mm。</summary>
    public double ToolTcpX { get => _toolTcp.X; set => SetToolPoseValue(_toolTcp.X, value, v => _toolTcp.X = v); }
    /// <summary>TCP 相对于法兰的 Y 偏移，单位 mm。</summary>
    public double ToolTcpY { get => _toolTcp.Y; set => SetToolPoseValue(_toolTcp.Y, value, v => _toolTcp.Y = v); }
    /// <summary>TCP 相对于法兰的 Z 偏移，单位 mm。</summary>
    public double ToolTcpZ { get => _toolTcp.Z; set => SetToolPoseValue(_toolTcp.Z, value, v => _toolTcp.Z = v); }
    /// <summary>TCP 相对于法兰的 A 角，单位度。</summary>
    public double ToolTcpA { get => _toolTcp.A; set => SetToolPoseValue(_toolTcp.A, value, v => _toolTcp.A = v); }
    /// <summary>TCP 相对于法兰的 B 角，单位度。</summary>
    public double ToolTcpB { get => _toolTcp.B; set => SetToolPoseValue(_toolTcp.B, value, v => _toolTcp.B = v); }
    /// <summary>TCP 相对于法兰的 C 角，单位度。</summary>
    public double ToolTcpC { get => _toolTcp.C; set => SetToolPoseValue(_toolTcp.C, value, v => _toolTcp.C = v); }
    /// <summary>工装整体相对于机器人基坐标的 XYZ/ABC；全零保留导出位置。</summary>
    public double WorkX { get => _workPose.X; set => SetWorkPoseValue(_workPose.X, value, v => _workPose.X = v); }
    public double WorkY { get => _workPose.Y; set => SetWorkPoseValue(_workPose.Y, value, v => _workPose.Y = v); }
    public double WorkZ { get => _workPose.Z; set => SetWorkPoseValue(_workPose.Z, value, v => _workPose.Z = v); }
    public double WorkA { get => _workPose.A; set => SetWorkPoseValue(_workPose.A, value, v => _workPose.A = v); }
    public double WorkB { get => _workPose.B; set => SetWorkPoseValue(_workPose.B, value, v => _workPose.B = v); }
    public double WorkC { get => _workPose.C; set => SetWorkPoseValue(_workPose.C, value, v => _workPose.C = v); }
    /// <summary>底部操作状态文字。</summary>
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    /// <summary>工具安装状态。</summary>
    public string ToolStatus { get => _toolStatus; set => SetProperty(ref _toolStatus, value); }
    public string WorkStatus { get => _workStatus; set => SetProperty(ref _workStatus, value); }
    /// <summary>当前后台读取步骤。</summary>
    public string LoadingMessage { get => _loadingMessage; set => SetProperty(ref _loadingMessage, value); }
    /// <summary>当前读取阶段的进度，取值 0～100。</summary>
    public double LoadProgress { get => _loadProgress; set => SetProperty(ref _loadProgress, value); }
    /// <summary>加载机器人或 Tool 时为 true，统一禁止重复操作。</summary>
    public bool IsBusy { get => _isBusy; private set { if (SetProperty(ref _isBusy, value)) NotifyAvailability(); } }
    /// <summary>机器人已加载；工具加载失败时仍可操作机器人或重新选择工具。</summary>
    public bool IsLoaded { get => _isLoaded; private set { if (SetProperty(ref _isLoaded, value)) NotifyAvailability(); } }
    /// <summary>加载期间显示浮层和进度，重新载入工装时也有反馈。</summary>
    public bool ShowLoadingOverlay => IsBusy;
    /// <summary>渲染器已就绪且不处于加载和关闭流程时允许选择机器人。</summary>
    public bool CanSelectRobot => _viewerReady && !IsBusy && !_closePending && !_isClosing;
    /// <summary>机器人可操作时允许选择独立 Tool。</summary>
    public bool CanSelectTool => CanOperate && ToolModels.Count > 0;
    public bool CanSelectWork => CanOperate && WorkModels.Count > 0;
    /// <summary>当前允许控制关节、复位或播放动作。</summary>
    public bool CanOperate => IsLoaded && CanSelectRobot;
    /// <summary>有当前工具且没有加载任务时，允许编辑、保存和重载工具配置。</summary>
    public bool CanEditTool => CanOperate && _currentTool is not null;
    public bool CanEditWork => CanOperate && _currentWork is not null;
    /// <summary>示例动作是否播放中，变化时更新按钮文字。</summary>
    public bool IsPlaying { get => _isPlaying; private set { if (SetProperty(ref _isPlaying, value)) OnPropertyChanged(nameof(PlayButtonText)); } }
    /// <summary>根据播放状态显示按钮文字。</summary>
    public string PlayButtonText => IsPlaying ? "停止动作" : "示例动作";

    /// <summary>导出独立工具配置，机器人参数不会进入保存结果。</summary>
    public ToolConfiguration ToToolConfiguration() => new()
    {
        ToolName = _toolConfiguration?.ToolName ?? throw new InvalidOperationException("未选择 Tool。"),
        ModelFile = _toolConfiguration.ModelFile,
        Tcp = CopyPose(_toolTcp)
    };

    public WorkConfiguration ToWorkConfiguration() => new()
    {
        WorkName = _workConfiguration?.WorkName ?? throw new InvalidOperationException("未选择工装。"),
        ModelFile = _workConfiguration.ModelFile,
        Pose = CopyPose(_workPose)
    };

    /// <summary>切换机器人时重新创建六轴状态，固定参数仍取自内置配置。</summary>
    private void LoadRobotConfiguration()
    {
        Joints = _currentRobot.Configuration.Joints.Select((joint, index) => new JointViewModel(index, joint)).ToArray();
        BindJointEvents();
        OnPropertyChanged(string.Empty);
    }

    /// <summary>更新工具可编辑副本，避免与已加载配置共享可变 TCP。</summary>
    private void LoadToolConfiguration(ToolConfiguration configuration)
    {
        _toolConfiguration = configuration;
        _toolTcp = CopyPose(configuration.Tcp);
        OnPropertyChanged(string.Empty);
    }

    private void LoadWorkConfiguration(WorkConfiguration configuration)
    {
        _workConfiguration = configuration;
        _workPose = CopyPose(configuration.Pose);
        OnPropertyChanged(string.Empty);
    }

    /// <summary>控件首次就绪后初始化视口并加载首个机器人与工具。</summary>
    private async Task InitializeSceneAsync()
    {
        if (_viewerReady || _isClosing) return;
        try
        {
            _sceneService.InitializeViewport();
            _viewerReady = true;
            NotifyAvailability();
            await LoadSceneAsync();
        }
        catch (Exception exception) { ReportError(exception, "三维视图初始化失败"); }
    }

    /// <summary>首次安装全部选择项，后续只替换机器人；加载失败保留当前场景与参数。</summary>
    private async Task LoadSceneAsync(RobotModelDefinition? target = null)
    {
        if (IsBusy || _isClosing || !_viewerReady) return;
        StopAnimation();
        IsBusy = true;
        target ??= _currentRobot;
        var installAttachments = !IsLoaded;
        var robotReady = IsLoaded;
        try
        {
            await _sceneService.LoadRobotAsync(target.Configuration, target.Id, CreateProgress());
            UnbindJointEvents();
            _currentRobot = target;
            SelectedRobot = target;
            LoadRobotConfiguration();
            robotReady = true;
            _sceneService.ApplyAll(Joints);
            if (installAttachments)
            {
                ToolStatus = "未安装 Tool";
                WorkStatus = "未安装工装";
            }
            var installationFailed = false;
            if (installAttachments && _currentTool is not null)
            {
                try
                {
                    await _sceneService.LoadToolAsync(_currentTool.Id, ToToolConfiguration(), CreateProgress());
                    ToolStatus = $"已安装：{ToolName}";
                }
                catch (Exception exception)
                {
                    installationFailed = true;
                    ToolStatus = "Tool 加载失败，可重新载入";
                    ReportError(exception, "Tool 载入失败");
                }
            }
            if (installAttachments && _currentWork is not null)
            {
                try
                {
                    await _sceneService.LoadWorkAsync(_currentWork.Id, ToWorkConfiguration(), CreateProgress());
                    WorkStatus = $"已安装：{WorkName}（机器人基坐标）";
                }
                catch (Exception exception)
                {
                    installationFailed = true;
                    WorkStatus = "工装加载失败，可重新载入";
                    ReportError(exception, "工装载入失败");
                }
            }
            if (!installationFailed)
            {
                StatusMessage = installAttachments
                    ? "机器人及所选 Tool、工装已加载；工装固定于机器人基坐标。"
                    : "机器人已切换，Tool 与工装复用现有模型，相机视角保持不变。";
                // 首次安装完成后统一为等轴测视角，并按当前速度/角度启动相机环绕。
                if (installAttachments)
                {
                    _sceneService.SetView(RobotView.Isometric);
                    UpdateOrbit();
                }
            }
        }
        catch (Exception exception)
        {
            SelectedRobot = _currentRobot;
            ReportError(exception, "模型载入失败");
        }
        finally
        {
            IsLoaded = robotReady;
            IsBusy = false;
            UpdateToolTcp();
            UpdateWorkPose();
            UpdateCoordinateVisibility();
            CompletePendingClose();
        }
    }

    /// <summary>响应机器人选择变化，工具选择和当前工具 TCP 保持不变。</summary>
    private async Task SelectRobotAsync(RobotModelDefinition? selected)
    {
        if (selected is null || selected.Id == _currentRobot.Id || !_viewerReady || _isClosing) return;
        if (IsBusy) { SelectedRobot = _currentRobot; return; }
        await LoadSceneAsync(selected);
    }

    /// <summary>读取所选工具的当前外部配置，成功解析模型后才替换旧工具；失败时恢复选择。</summary>
    private async Task SelectToolAsync(ToolModelDefinition? selected)
    {
        if (selected is null || selected.Id == _currentTool?.Id) return;
        if (!CanSelectTool) { SelectedTool = _currentTool; return; }
        try { await ApplyToolAsync(selected, ToolConfigurationStore.Load(selected.ConfigurationPath)); }
        catch (Exception exception) { SelectedTool = _currentTool; ReportError(exception, "切换 Tool 失败"); }
    }

    /// <summary>只重读当前 tool.json 和 Tool 模型，保留机器人及六轴当前姿态。</summary>
    private async Task ReloadToolAsync()
    {
        if (!CanEditTool || _currentTool is null) return;
        try { await ApplyToolAsync(_currentTool, ToolConfigurationStore.Load(_currentTool.ConfigurationPath)); }
        catch (Exception exception) { ReportError(exception, "重新载入 Tool 失败"); }
    }

    /// <summary>独立更换工具的共用流程，原生几何在成功读取后才切换。</summary>
    private async Task ApplyToolAsync(ToolModelDefinition tool, ToolConfiguration configuration)
    {
        StopAnimation();
        IsBusy = true;
        try
        {
            await _sceneService.LoadToolAsync(tool.Id, configuration, CreateProgress());
            _currentTool = tool;
            SelectedTool = tool;
            LoadToolConfiguration(configuration);
            ToolStatus = $"已安装：{ToolName}";
            StatusMessage = "Tool 已更新，机器人当前姿态保持不变。";
        }
        finally
        {
            IsBusy = false;
            UpdateToolTcp();
            UpdateCoordinateVisibility();
            CompletePendingClose();
        }
    }

    /// <summary>只保存当前工具配置，不生成或修改任何机器人配置文件。</summary>
    private void SaveToolConfiguration()
    {
        if (!CanEditTool || _currentTool is null) return;
        try
        {
            ToolConfigurationStore.Save(_currentTool.ConfigurationPath, ToToolConfiguration());
            StatusMessage = $"Tool 配置已保存：{_currentTool.ConfigurationPath}";
        }
        catch (Exception exception) { ReportError(exception, "保存 Tool 失败"); }
    }

    private async Task SelectWorkAsync(WorkModelDefinition? selected)
    {
        if (selected is null || selected.Id == _currentWork?.Id) return;
        if (!CanSelectWork) { SelectedWork = _currentWork; return; }
        try { await ApplyWorkAsync(selected, WorkConfigurationStore.Load(selected.ConfigurationPath)); }
        catch (Exception exception) { SelectedWork = _currentWork; ReportError(exception, "切换工装失败"); }
    }

    private async Task ReloadWorkAsync()
    {
        if (!CanEditWork || _currentWork is null) return;
        try { await ApplyWorkAsync(_currentWork, WorkConfigurationStore.Load(_currentWork.ConfigurationPath)); }
        catch (Exception exception) { ReportError(exception, "重新载入工装失败"); }
    }

    private async Task ApplyWorkAsync(WorkModelDefinition work, WorkConfiguration configuration)
    {
        StopAnimation();
        IsBusy = true;
        try
        {
            await _sceneService.LoadWorkAsync(work.Id, configuration, CreateProgress());
            _currentWork = work;
            SelectedWork = work;
            LoadWorkConfiguration(configuration);
            WorkStatus = $"已安装：{WorkName}（机器人基坐标）";
            StatusMessage = "工装已更新，机器人当前姿态和 Tool 保持不变。";
        }
        finally
        {
            IsBusy = false;
            UpdateWorkPose();
            CompletePendingClose();
        }
    }

    private void SaveWorkConfiguration()
    {
        if (!CanEditWork || _currentWork is null) return;
        try
        {
            WorkConfigurationStore.Save(_currentWork.ConfigurationPath, ToWorkConfiguration());
            StatusMessage = $"工装配置已保存：{_currentWork.ConfigurationPath}";
        }
        catch (Exception exception) { ReportError(exception, "保存工装失败"); }
    }

    /// <summary>在 UI 线程创建进度对象，后台回调自动回到界面线程更新属性。</summary>
    private IProgress<RobotLoadProgress> CreateProgress() => new Progress<RobotLoadProgress>(state =>
    {
        LoadProgress = state.Percent;
        LoadingMessage = state.Message;
        StatusMessage = state.Message;
        AppLog.Write(state.Message);
    });

    /// <summary>统一通知选择、运动和工具编辑控件的可用状态。</summary>
    private void NotifyAvailability()
    {
        OnPropertyChanged(nameof(CanSelectRobot));
        OnPropertyChanged(nameof(CanSelectTool));
        OnPropertyChanged(nameof(CanOperate));
        OnPropertyChanged(nameof(CanEditTool));
        OnPropertyChanged(nameof(CanSelectWork));
        OnPropertyChanged(nameof(CanEditWork));
        OnPropertyChanged(nameof(ShowLoadingOverlay));
    }

    /// <summary>后台读取完成后继续处理此前延迟的关闭请求。</summary>
    private void CompletePendingClose()
    {
        if (_closePending) CloseRequested?.Invoke();
    }

    /// <summary>响应单个关节角度变化；批量更新期间由调用方统一刷新，避免重复绘制。</summary>
    private void Joint_StateChanged(object? sender, EventArgs e)
    {
        if (!_suppressJointUpdates && sender is JointViewModel joint && CanOperate)
        {
            _sceneService.ApplyJoint(joint);
            StatusMessage = $"{joint.ControllerAxis}（{joint.Name}）= {joint.Angle:F1}°";
        }
    }

    /// <summary>停止示例动作，将全部关节恢复到配置的初始角度后统一刷新场景。</summary>
    private void Reset()
    {
        if (!CanOperate)
        {
            return;
        }

        StopAnimation();
        _suppressJointUpdates = true;
        foreach (var joint in Joints)
        {
            joint.Angle = joint.HomeAngle;
        }
        _suppressJointUpdates = false;
        _sceneService.ApplyAll(Joints);
        StatusMessage = "机器人已回到配置初始位。";
    }

    /// <summary>切换示例动作的播放/暂停；播放时使用 WPF 渲染事件驱动，不另建固定频率计时器。</summary>
    private void Play()
    {
        if (!CanOperate)
        {
            return;
        }

        if (IsPlaying)
        {
            StopAnimation();
            StatusMessage = "示例动作已暂停。";
            return;
        }

        IsPlaying = true;
        _animationClock.Restart();
        CompositionTarget.Rendering += AnimationFrame;
        StatusMessage = "正在播放六轴示例动作。";
    }

    /// <summary>每次渲染事件最多排入一个动画更新任务，防止帧回调过快导致队列积压。</summary>
    private void AnimationFrame(object? sender, EventArgs e)
    {
        if (_animationUpdatePending)
        {
            return;
        }

        _animationUpdatePending = true;
        // 以后台优先级更新动画，让鼠标拖动相机等输入先处理，减轻播放时的交互卡顿。
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, UpdateAnimation);
    }

    /// <summary>根据实际经过时间计算六轴正弦示例角度，再一次性应用全部关节。</summary>
    private void UpdateAnimation()
    {
        _animationUpdatePending = false;
        if (!IsPlaying)
        {
            return;
        }

        // 停止后队列中可能仍有一次回调；上面的 IsPlaying 判断使它不会继续改变关节。
        var seconds = _animationClock.Elapsed.TotalSeconds;
        // 每轴使用不同摆幅和频率，只作为显示演示轨迹；Angle 属性仍会执行行程限位。
        var amplitudes = new[] { 40.0, 28.0, 32.0, 55.0, 38.0, 95.0 };
        var frequencies = new[] { 0.42, 0.31, 0.37, 0.55, 0.48, 0.66 };

        _suppressJointUpdates = true;
        for (var index = 0; index < Joints.Count; index++)
        {
            var joint = Joints[index];
            joint.Angle = joint.HomeAngle + amplitudes[index] * Math.Sin(seconds * frequencies[index] + index * 0.63);
        }
        _suppressJointUpdates = false;
        _sceneService.ApplyAll(Joints);
    }

    /// <summary>处理关闭请求：加载中先取消本次关闭；空闲时停止动画、解除事件并释放场景。</summary>
    private void OnClosing(CancelEventArgs? e)
    {
        if (e is null)
        {
            return;
        }

        if (IsBusy)
        {
            e.Cancel = true;
            _closePending = true;
            StopAnimation();
            NotifyAvailability();
            StatusMessage = "正在完成后台模型读取，随后安全关闭…";
            return;
        }

        _isClosing = true;
        StopAnimation();
        UnbindJointEvents();
        _sceneService.Dispose();
    }

    /// <summary>取消渲染事件订阅、停止动画时钟，并将播放状态恢复为 false。</summary>
    private void StopAnimation()
    {
        CompositionTarget.Rendering -= AnimationFrame;
        _animationClock.Stop();
        IsPlaying = false;
    }

    /// <summary>订阅当前六轴的变化通知，让滑块和校准表的修改同步到模型。</summary>
    private void BindJointEvents()
    {
        foreach (var joint in Joints)
        {
            joint.StateChanged += Joint_StateChanged;
        }
    }

    /// <summary>切换、重载或关闭前解除旧六轴事件，避免旧对象继续触发场景更新。</summary>
    private void UnbindJointEvents()
    {
        foreach (var joint in Joints)
        {
            joint.StateChanged -= Joint_StateChanged;
        }
    }

    /// <summary>状态栏显示简短原因，日志保留完整异常，并请求视图弹出错误提示。</summary>
    private void ReportError(Exception exception, string title)
    {
        StatusMessage = $"{title}：{exception.Message}";
        AppLog.Write($"{title}：{exception}");
        ErrorOccurred?.Invoke(exception.ToString(), title);
    }

    /// <summary>统一处理 TCP 单项编辑：值真正改变后才写入、通知界面并更新三维坐标。</summary>
    private void SetToolPoseValue(double currentValue, double newValue, Action<double> setter,
        [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (!currentValue.Equals(newValue))
        {
            setter(newValue);
            OnPropertyChanged(propertyName);
            UpdateToolTcp();
        }
    }

    /// <summary>仅在场景已加载且六个 TCP 数值都有效时更新坐标轴，避免把非法值传给原生引擎。</summary>
    private void UpdateToolTcp()
    {
        if (CanOperate && double.IsFinite(_toolTcp.X) && double.IsFinite(_toolTcp.Y) && double.IsFinite(_toolTcp.Z)
            && double.IsFinite(_toolTcp.A) && double.IsFinite(_toolTcp.B) && double.IsFinite(_toolTcp.C))
        {
            _sceneService.SetToolTcp(CopyPose(_toolTcp));
        }
    }

    private void SetWorkPoseValue(double currentValue, double newValue, Action<double> setter,
        [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (!currentValue.Equals(newValue))
        {
            setter(newValue);
            OnPropertyChanged(propertyName);
            UpdateWorkPose();
        }
    }

    private void UpdateWorkPose()
    {
        if (CanEditWork && double.IsFinite(_workPose.X) && double.IsFinite(_workPose.Y) && double.IsFinite(_workPose.Z)
            && double.IsFinite(_workPose.A) && double.IsFinite(_workPose.B) && double.IsFinite(_workPose.C))
            _sceneService.SetWorkPose(CopyPose(_workPose));
    }

    /// <summary>将三个坐标复选框的状态同步到当前场景；未加载时先保留界面状态。</summary>
    private void UpdateCoordinateVisibility()
    {
        if (CanOperate)
        {
            _sceneService.SetCoordinateVisibility(ShowBaseCoordinates, ShowFlangeCoordinates, ShowTcpCoordinates);
        }
    }

    /// <summary>把当前环绕速度、起始角度和固定仰角同步到场景。</summary>
    private void UpdateOrbit()
    {
        if (!_viewerReady || _isClosing)
        {
            return;
        }

        _sceneService.SetOrbit(_orbitSpeed, _orbitAngle, OrbitElevationDegrees);
    }

    /// <summary>复制位姿的六个数值，避免界面编辑和后台场景读取同一个可变配置对象。</summary>
    private static PoseConfiguration CopyPose(PoseConfiguration pose) => new()
    {
        X = pose.X,
        Y = pose.Y,
        Z = pose.Z,
        A = pose.A,
        B = pose.B,
        C = pose.C
    };

}
