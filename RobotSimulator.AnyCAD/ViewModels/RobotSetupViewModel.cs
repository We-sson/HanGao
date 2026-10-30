using System.ComponentModel;
using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RobotSimulator.Models;
using RobotSimulator.Services;

namespace RobotSimulator.ViewModels;

public sealed partial class RobotSetupViewModel : ObservableObject
{
    private string _sourceDirectory = string.Empty;
    private IReadOnlyList<string> _modelFiles = [];
    private IReadOnlyList<JointSetupViewModel> _joints = CreateJoints();
    private JointSetupViewModel _selectedJoint;
    private int _stepIndex;
    private bool _validated;
    private bool _hasUnsavedChanges;
    private string _validationMessage = string.Empty;
    private string _statusMessage = "选择保存有 Base、J1～J6 的 STEP 源模型目录。";

    [ObservableProperty] private string _modelName = string.Empty;
    [ObservableProperty] private string _baseFile = string.Empty;
    [ObservableProperty] private string _flangeX = string.Empty;
    [ObservableProperty] private string _flangeY = string.Empty;
    [ObservableProperty] private string _flangeZ = string.Empty;
    [ObservableProperty] private string _flangeA = string.Empty;
    [ObservableProperty] private string _flangeB = string.Empty;
    [ObservableProperty] private string _flangeC = string.Empty;

    public RobotSetupViewModel()
    {
        _selectedJoint = _joints[0];
        BindJoints();
        BackCommand = new RelayCommand(() => SetStep(StepIndex - 1), () => CanGoBack);
        NextCommand = new RelayCommand(Next, () => CanGoNext);
        ValidateCommand = new RelayCommand(() => ValidateCurrent(), () => SourceDirectory.Length > 0);
    }

    public string SourceDirectory => _sourceDirectory;
    public string OutputPath => SourceDirectory.Length == 0 ? string.Empty : RobotImportService.GetConfigurationPath(SourceDirectory);
    public IReadOnlyList<string> ModelFiles => _modelFiles;
    public IReadOnlyList<JointSetupViewModel> Joints => _joints;
    public JointSetupViewModel SelectedJoint { get => _selectedJoint; set => SetProperty(ref _selectedJoint, value); }
    public int StepIndex => _stepIndex;
    public string StepTitle => new[] { "1. 导入与模型对应", "2. 六轴关节参数", "3. 法兰坐标参数", "4. 校验与保存" }[StepIndex];
    public string StepDescription => new[]
    {
        "保留装配全局坐标，单位 mm；为基座和六轴分别选择一个 STEP 文件。",
        "逐个选择 J1～J6，依据该型号手册和 CAD 装配测量填写；不要直接套用其他型号。",
        "填写原始 STEP 姿态下的法兰中心及方向，供独立 Tool 安装使用。",
        "校验通过后保存源配置，再重新构建内嵌资源，并在仿真中逐轴验证运动。"
    }[StepIndex];
    public string StatusMessage => _statusMessage;
    public string ValidationMessage => _validationMessage;
    public bool CanGoBack => StepIndex > 0;
    public bool CanGoNext => StepIndex < 3 && SourceDirectory.Length > 0;
    public bool CanSave => StepIndex == 3 && _validated;
    public bool HasUnsavedChanges => _hasUnsavedChanges;
    public IRelayCommand BackCommand { get; }
    public IRelayCommand NextCommand { get; }
    public IRelayCommand ValidateCommand { get; }

    public void SelectDirectory(string path)
    {
        var directory = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        if (new DirectoryInfo(directory).Parent is null)
            throw new InvalidDataException("请选择以机器人型号命名的子目录。");
        var files = RobotImportService.GetModelFiles(directory);
        if (files.Count == 0) throw new InvalidDataException("目录中没有 STEP 模型；请先按同一装配坐标导出模型。");
        var existing = RobotImportService.LoadExisting(directory);
        foreach (var joint in _joints) joint.PropertyChanged -= OnJointChanged;
        _sourceDirectory = directory;
        _modelFiles = files;
        _joints = CreateJoints();
        if (existing is not null)
        {
            for (var i = 0; i < _joints.Count; i++) _joints[i].Load(existing.Joints[i]);
            ModelName = existing.ModelName;
            BaseFile = existing.BaseFile;
            FlangeX = Format(existing.FlangeOrigin.X); FlangeY = Format(existing.FlangeOrigin.Y); FlangeZ = Format(existing.FlangeOrigin.Z);
            FlangeA = Format(existing.FlangeOrientation.A); FlangeB = Format(existing.FlangeOrientation.B); FlangeC = Format(existing.FlangeOrientation.C);
        }
        else
        {
            ModelName = new DirectoryInfo(directory).Name;
            BaseFile = MatchFile("Base.STEP");
            foreach (var joint in _joints) joint.ModelFile = MatchFile($"{joint.Name}.STEP");
            FlangeX = FlangeY = FlangeZ = FlangeA = FlangeB = FlangeC = string.Empty;
        }
        _selectedJoint = _joints[0];
        BindJoints();
        _stepIndex = 0;
        _statusMessage = existing is null
            ? $"发现 {files.Count} 个 STEP；测量参数尚未填写。"
            : $"已读取现有配置：{Path.GetFileName(OutputPath)}。";
        Invalidate();
        _hasUnsavedChanges = false;
        OnPropertyChanged(string.Empty);
        UpdateCommands();
    }

    public string Save()
    {
        var path = RobotImportService.Save(SourceDirectory, BuildConfiguration());
        _hasUnsavedChanges = false;
        _validated = true;
        _statusMessage = $"已保存：{path}。请将此目录放入 Resources/Robots，重新构建并逐轴验证。";
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CanSave));
        return path;
    }

    private void Next()
    {
        try
        {
            if (StepIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(ModelName)) throw new InvalidDataException("请输入机器人型号名称。");
                if (RobotImportService.GetModelFiles(SourceDirectory).Count < 7)
                    throw new InvalidDataException("请先准备 Base 和 J1～J6 共七份独立 STEP 文件。");
                if (!ModelFiles.Contains(BaseFile) || !File.Exists(Path.Combine(SourceDirectory, BaseFile)))
                    throw new InvalidDataException("请选择源目录中的基座 STEP 文件。");
            }
            else if (StepIndex == 1) BuildConfiguration(includeFlange: false);
            else BuildConfiguration();
            SetStep(StepIndex + 1);
            if (StepIndex == 3) ValidateCurrent();
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException or UnauthorizedAccessException)
        {
            SetValidation(exception.Message, false);
        }
    }

    private bool ValidateCurrent()
    {
        try
        {
            BuildConfiguration();
            SetValidation("参数与文件校验通过。保存后仍需在仿真中检查轴心、正方向和参考姿态。", true);
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException or UnauthorizedAccessException)
        {
            SetValidation(exception.Message, false);
            return false;
        }
    }

    private RobotConfiguration BuildConfiguration(bool includeFlange = true)
    {
        ValidateModels();
        var configuration = new RobotConfiguration
        {
            ModelName = ModelName.Trim(), BaseFile = BaseFile,
            Joints = Joints.Select(joint => joint.ToConfiguration()).ToArray(),
            FlangeOrigin = includeFlange ? new VectorConfiguration
            {
                X = Parse(FlangeX, "法兰中心 X"), Y = Parse(FlangeY, "法兰中心 Y"), Z = Parse(FlangeZ, "法兰中心 Z")
            } : new VectorConfiguration(),
            FlangeOrientation = includeFlange ? new OrientationConfiguration
            {
                A = Parse(FlangeA, "法兰 A"), B = Parse(FlangeB, "法兰 B"), C = Parse(FlangeC, "法兰 C")
            } : new OrientationConfiguration()
        };
        RobotConfigurationStore.Validate(configuration);
        return configuration;
    }

    private void ValidateModels()
    {
        if (SourceDirectory.Length == 0) throw new InvalidDataException("请先选择机器人源模型目录。");
        if (string.IsNullOrWhiteSpace(ModelName)) throw new InvalidDataException("请输入机器人型号名称。");
        RobotImportService.ValidateFiles(SourceDirectory, BaseFile, Joints.Select(joint => joint.ModelFile));
    }

    private void SetStep(int index)
    {
        _stepIndex = Math.Clamp(index, 0, 3);
        _validationMessage = string.Empty;
        OnPropertyChanged(nameof(StepIndex)); OnPropertyChanged(nameof(StepTitle)); OnPropertyChanged(nameof(StepDescription));
        OnPropertyChanged(nameof(ValidationMessage));
        UpdateCommands();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(ModelName) or nameof(BaseFile) or nameof(FlangeX) or nameof(FlangeY)
            or nameof(FlangeZ) or nameof(FlangeA) or nameof(FlangeB) or nameof(FlangeC)) Invalidate();
    }

    private void BindJoints()
    {
        foreach (var joint in _joints) joint.PropertyChanged += OnJointChanged;
    }

    private void OnJointChanged(object? sender, PropertyChangedEventArgs e) => Invalidate();
    private void Invalidate()
    {
        _hasUnsavedChanges = SourceDirectory.Length > 0;
        SetValidation(string.Empty, false);
    }

    private void SetValidation(string message, bool valid)
    {
        _validated = valid;
        _validationMessage = message;
        OnPropertyChanged(nameof(ValidationMessage)); OnPropertyChanged(nameof(CanSave));
    }

    private void UpdateCommands()
    {
        OnPropertyChanged(nameof(CanGoBack)); OnPropertyChanged(nameof(CanGoNext)); OnPropertyChanged(nameof(CanSave));
        BackCommand.NotifyCanExecuteChanged(); NextCommand.NotifyCanExecuteChanged(); ValidateCommand.NotifyCanExecuteChanged();
    }

    private string MatchFile(string name) => ModelFiles.FirstOrDefault(file => file.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
    private static IReadOnlyList<JointSetupViewModel> CreateJoints() => Enumerable.Range(1, 6).Select(index => new JointSetupViewModel($"J{index}")).ToArray();
    internal static string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);
    internal static double Parse(string text, string name)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
            throw new InvalidDataException($"{name}：请填写有效数值，使用小数点，单位按字段标注。");
        return value;
    }
}

public sealed partial class JointSetupViewModel(string name) : ObservableObject
{
    public string Name { get; } = name;
    [ObservableProperty] private string _modelFile = string.Empty;
    [ObservableProperty] private string _minimum = string.Empty;
    [ObservableProperty] private string _maximum = string.Empty;
    [ObservableProperty] private string _homeAngle = string.Empty;
    [ObservableProperty] private string _modelReferenceAngle = string.Empty;
    [ObservableProperty] private string _originX = string.Empty;
    [ObservableProperty] private string _originY = string.Empty;
    [ObservableProperty] private string _originZ = string.Empty;
    [ObservableProperty] private string _directionX = string.Empty;
    [ObservableProperty] private string _directionY = string.Empty;
    [ObservableProperty] private string _directionZ = string.Empty;

    internal JointConfiguration ToConfiguration() => new()
    {
        Name = Name, ModelFile = ModelFile,
        Minimum = RobotSetupViewModel.Parse(Minimum, $"{Name} 最小角"),
        Maximum = RobotSetupViewModel.Parse(Maximum, $"{Name} 最大角"),
        HomeAngle = RobotSetupViewModel.Parse(HomeAngle, $"{Name} 初始角"),
        ModelReferenceAngle = RobotSetupViewModel.Parse(ModelReferenceAngle, $"{Name} CAD 参考角"),
        AxisOrigin = new VectorConfiguration
        {
            X = RobotSetupViewModel.Parse(OriginX, $"{Name} 轴心 X"),
            Y = RobotSetupViewModel.Parse(OriginY, $"{Name} 轴心 Y"),
            Z = RobotSetupViewModel.Parse(OriginZ, $"{Name} 轴心 Z")
        },
        AxisDirection = new VectorConfiguration
        {
            X = RobotSetupViewModel.Parse(DirectionX, $"{Name} 方向 X"),
            Y = RobotSetupViewModel.Parse(DirectionY, $"{Name} 方向 Y"),
            Z = RobotSetupViewModel.Parse(DirectionZ, $"{Name} 方向 Z")
        }
    };

    internal void Load(JointConfiguration joint)
    {
        ModelFile = joint.ModelFile;
        Minimum = RobotSetupViewModel.Format(joint.Minimum); Maximum = RobotSetupViewModel.Format(joint.Maximum);
        HomeAngle = RobotSetupViewModel.Format(joint.HomeAngle); ModelReferenceAngle = RobotSetupViewModel.Format(joint.ModelReferenceAngle);
        OriginX = RobotSetupViewModel.Format(joint.AxisOrigin.X); OriginY = RobotSetupViewModel.Format(joint.AxisOrigin.Y); OriginZ = RobotSetupViewModel.Format(joint.AxisOrigin.Z);
        DirectionX = RobotSetupViewModel.Format(joint.AxisDirection.X); DirectionY = RobotSetupViewModel.Format(joint.AxisDirection.Y); DirectionZ = RobotSetupViewModel.Format(joint.AxisDirection.Z);
    }
}
