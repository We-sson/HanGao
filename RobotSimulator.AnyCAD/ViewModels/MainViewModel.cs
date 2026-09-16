using System.Collections.ObjectModel;
using RobotSimulator.Models;

namespace RobotSimulator.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private string _modelName = string.Empty;
    private string _baseFile = string.Empty;
    private string _toolFile = string.Empty;
    private ObservableCollection<JointViewModel> _joints = [];
    private PoseConfiguration _toolModelPose = new();
    private PoseConfiguration _toolTcp = new();
    private string _statusMessage = "正在初始化三维视图…";
    private string _loadingMessage = "正在准备机器人模型…";
    private double _loadProgress;
    private bool _isBusy;
    private bool _isLoaded;
    private bool _isPlaying;
    private string _toolStatus = "Tool.STEP 未提供（已预留）";

    public MainViewModel(RobotConfiguration configuration)
    {
        LoadConfiguration(configuration);
    }

    public string ModelName
    {
        get => _modelName;
        private set => SetProperty(ref _modelName, value);
    }

    public string BaseFile
    {
        get => _baseFile;
        private set => SetProperty(ref _baseFile, value);
    }

    public string ToolFile
    {
        get => _toolFile;
        private set => SetProperty(ref _toolFile, value);
    }

    public ObservableCollection<JointViewModel> Joints
    {
        get => _joints;
        private set => SetProperty(ref _joints, value);
    }

    public double ToolModelX { get => _toolModelPose.X; set => SetToolPoseValue(_toolModelPose.X, value, v => _toolModelPose.X = v); }
    public double ToolModelY { get => _toolModelPose.Y; set => SetToolPoseValue(_toolModelPose.Y, value, v => _toolModelPose.Y = v); }
    public double ToolModelZ { get => _toolModelPose.Z; set => SetToolPoseValue(_toolModelPose.Z, value, v => _toolModelPose.Z = v); }
    public double ToolModelA { get => _toolModelPose.A; set => SetToolPoseValue(_toolModelPose.A, value, v => _toolModelPose.A = v); }
    public double ToolModelB { get => _toolModelPose.B; set => SetToolPoseValue(_toolModelPose.B, value, v => _toolModelPose.B = v); }
    public double ToolModelC { get => _toolModelPose.C; set => SetToolPoseValue(_toolModelPose.C, value, v => _toolModelPose.C = v); }

    public double ToolTcpX { get => _toolTcp.X; set => SetToolPoseValue(_toolTcp.X, value, v => _toolTcp.X = v); }
    public double ToolTcpY { get => _toolTcp.Y; set => SetToolPoseValue(_toolTcp.Y, value, v => _toolTcp.Y = v); }
    public double ToolTcpZ { get => _toolTcp.Z; set => SetToolPoseValue(_toolTcp.Z, value, v => _toolTcp.Z = v); }
    public double ToolTcpA { get => _toolTcp.A; set => SetToolPoseValue(_toolTcp.A, value, v => _toolTcp.A = v); }
    public double ToolTcpB { get => _toolTcp.B; set => SetToolPoseValue(_toolTcp.B, value, v => _toolTcp.B = v); }
    public double ToolTcpC { get => _toolTcp.C; set => SetToolPoseValue(_toolTcp.C, value, v => _toolTcp.C = v); }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string ToolStatus
    {
        get => _toolStatus;
        set => SetProperty(ref _toolStatus, value);
    }

    public string LoadingMessage
    {
        get => _loadingMessage;
        set => SetProperty(ref _loadingMessage, value);
    }

    public double LoadProgress
    {
        get => _loadProgress;
        set => SetProperty(ref _loadProgress, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetProperty(ref _isLoaded, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (SetProperty(ref _isPlaying, value))
            {
                OnPropertyChanged(nameof(PlayButtonText));
            }
        }
    }

    public string PlayButtonText => IsPlaying ? "停止动作" : "示例动作";

    public void LoadConfiguration(RobotConfiguration configuration)
    {
        ModelName = configuration.ModelName;
        BaseFile = configuration.BaseFile;
        ToolFile = configuration.ToolFile;
        _toolModelPose = CopyPose(configuration.ToolModelPose);
        _toolTcp = CopyPose(configuration.ToolTcp);
        Joints = new ObservableCollection<JointViewModel>(
            configuration.Joints.Select((joint, index) => new JointViewModel(index, joint)));

        OnPropertyChanged(string.Empty);
    }

    public RobotConfiguration ToConfiguration()
    {
        return new RobotConfiguration
        {
            ModelName = ModelName,
            BaseFile = BaseFile,
            ToolFile = ToolFile,
            ToolModelPose = CopyPose(_toolModelPose),
            ToolTcp = CopyPose(_toolTcp),
            Joints = Joints.Select(joint => joint.ToConfiguration()).ToList()
        };
    }

    private void SetToolPoseValue(double currentValue, double newValue, Action<double> setter,
        [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (currentValue.Equals(newValue))
        {
            return;
        }

        setter(newValue);
        OnPropertyChanged(propertyName);
    }

    private static PoseConfiguration CopyPose(PoseConfiguration pose)
    {
        return new PoseConfiguration
        {
            X = pose.X,
            Y = pose.Y,
            Z = pose.Z,
            A = pose.A,
            B = pose.B,
            C = pose.C
        };
    }
}
