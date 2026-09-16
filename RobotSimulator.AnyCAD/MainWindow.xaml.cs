using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AnyCAD.Foundation;
using RobotSimulator.Models;
using RobotSimulator.Services;
using RobotSimulator.ViewModels;

namespace RobotSimulator;

public partial class MainWindow : Window
{
    private readonly IReadOnlyList<RobotModelDefinition> _robotModels;
    private RobotModelDefinition _currentRobot;
    private string _configurationPath;
    private string _modelDirectory;
    private readonly MainViewModel _viewModel;
    private readonly RobotSceneService _sceneService;
    private readonly DispatcherTimer _animationTimer;
    private readonly Stopwatch _animationClock = new();
    private bool _suppressJointUpdates;
    private bool _viewerReady;
    private bool _isClosing;
    private bool _closePending;

    public MainWindow(IReadOnlyList<RobotModelDefinition> robotModels)
    {
        if (robotModels.Count == 0)
        {
            throw new ArgumentException("至少需要一个机器人型号。", nameof(robotModels));
        }

        InitializeComponent();

        _robotModels = robotModels;
        _currentRobot = robotModels[0];
        _configurationPath = _currentRobot.ConfigurationPath;
        _modelDirectory = _currentRobot.ModelDirectory;
        _viewModel = new MainViewModel(_currentRobot.Configuration);
        _sceneService = new RobotSceneService(RenderView);
        DataContext = _viewModel;
        BindJointEvents();

        RobotSelector.ItemsSource = _robotModels;
        RobotSelector.SelectedItem = _currentRobot;
        UpdateWindowTitle();

        _animationTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(40)
        };
        _animationTimer.Tick += AnimationTimer_Tick;
    }

    private async void RenderView_ViewerReady()
    {
        if (_isClosing)
        {
            return;
        }

        _viewerReady = true;
        _sceneService.InitializeViewport();
        await LoadSceneAsync();
    }

    private async Task LoadSceneAsync()
    {
        if (_viewModel.IsBusy || _isClosing)
        {
            return;
        }

        StopAnimation();
        _viewModel.IsBusy = true;
        _viewModel.IsLoaded = false;
        RobotSelector.IsEnabled = false;

        try
        {
            AppLog.Write($"开始载入模型目录：{_modelDirectory}");
            var progress = new Progress<RobotLoadProgress>(state =>
            {
                _viewModel.LoadProgress = state.Percent;
                _viewModel.LoadingMessage = state.Message;
                _viewModel.StatusMessage = state.Message;
                AppLog.Write(state.Message);
            });
            var configuration = _viewModel.ToConfiguration();
            var hasTool = await _sceneService.LoadAsync(configuration, _modelDirectory, progress);
            CompleteSceneLoad(hasTool);
            AppLog.Write("Base、J1-J6 场景节点载入成功，关节层级已建立。");
        }
        catch (Exception exception)
        {
            if (_isClosing)
            {
                return;
            }

            _viewModel.StatusMessage = $"载入失败：{exception.Message}";
            AppLog.Write($"模型载入失败：{exception}");
            System.Windows.MessageBox.Show(this, exception.ToString(), "模型载入失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _viewModel.IsBusy = false;
            if (_closePending)
            {
                _ = Dispatcher.BeginInvoke(Close);
            }
            else if (!_isClosing)
            {
                RobotSelector.IsEnabled = true;
            }
        }
    }

    private async void RobotSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_viewerReady
            || _isClosing
            || RobotSelector.SelectedItem is not RobotModelDefinition selected
            || string.Equals(selected.ConfigurationPath, _configurationPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (_viewModel.IsBusy)
        {
            RobotSelector.SelectedItem = _currentRobot;
            return;
        }

        StopAnimation();
        UnbindJointEvents();
        _currentRobot = selected;
        _configurationPath = selected.ConfigurationPath;
        _modelDirectory = selected.ModelDirectory;
        _viewModel.LoadConfiguration(RobotConfigurationStore.Load(selected.ConfigurationPath));
        BindJointEvents();
        UpdateWindowTitle();
        await LoadSceneAsync();
    }

    private void Joint_StateChanged(object? sender, EventArgs e)
    {
        if (!_suppressJointUpdates && sender is JointViewModel joint && _viewModel.IsLoaded)
        {
            _sceneService.ApplyJoint(joint);
            _viewModel.StatusMessage = $"{joint.ControllerAxis}（{joint.Name}）= {joint.Angle:F1}°";
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        StopAnimation();
        _suppressJointUpdates = true;
        foreach (var joint in _viewModel.Joints)
        {
            joint.Angle = joint.HomeAngle;
        }
        _suppressJointUpdates = false;
        _sceneService.ApplyAll(_viewModel.Joints);
        _viewModel.StatusMessage = "机器人已回到配置初始位。";
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.IsLoaded)
        {
            return;
        }

        if (_viewModel.IsPlaying)
        {
            StopAnimation();
            _viewModel.StatusMessage = "示例动作已暂停。";
            return;
        }

        _viewModel.IsPlaying = true;
        _animationClock.Restart();
        _animationTimer.Start();
        _viewModel.StatusMessage = "正在播放六轴示例动作。";
    }

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        var seconds = _animationClock.Elapsed.TotalSeconds;
        var amplitudes = new[] { 40.0, 28.0, 32.0, 55.0, 38.0, 95.0 };
        var frequencies = new[] { 0.42, 0.31, 0.37, 0.55, 0.48, 0.66 };

        _suppressJointUpdates = true;
        for (var index = 0; index < _viewModel.Joints.Count; index++)
        {
            var joint = _viewModel.Joints[index];
            var phase = index * 0.63;
            joint.Angle = joint.HomeAngle + amplitudes[index] * Math.Sin(seconds * frequencies[index] + phase);
        }
        _suppressJointUpdates = false;
        _sceneService.ApplyAll(_viewModel.Joints);
    }

    private void SaveConfiguration_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            RobotConfigurationStore.Save(_configurationPath, _viewModel.ToConfiguration());
            _viewModel.StatusMessage = $"轴配置已保存：{_configurationPath}";
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(this, exception.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Reload_Click(object sender, RoutedEventArgs e)
    {
        await LoadSceneAsync();
    }

    private void ZoomAll_Click(object sender, RoutedEventArgs e)
    {
        RenderView.ZoomAll(1.15f);
    }

    private void Isometric_Click(object sender, RoutedEventArgs e)
    {
        RenderView.SetStandardView(EnumStandardView.View3D, true);
        RenderView.ZoomAll(1.15f);
    }

    private void Front_Click(object sender, RoutedEventArgs e)
    {
        RenderView.SetStandardView(EnumStandardView.Front, true);
        RenderView.ZoomAll(1.15f);
    }

    private void Top_Click(object sender, RoutedEventArgs e)
    {
        RenderView.SetStandardView(EnumStandardView.Top, true);
        RenderView.ZoomAll(1.15f);
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_viewModel.IsBusy)
        {
            e.Cancel = true;
            _closePending = true;
            StopAnimation();
            RobotSelector.IsEnabled = false;
            _viewModel.StatusMessage = "正在完成后台模型读取，随后安全关闭…";
            return;
        }

        _isClosing = true;
        StopAnimation();
        UnbindJointEvents();
        _sceneService.Dispose();
    }

    private void StopAnimation()
    {
        _animationTimer.Stop();
        _animationClock.Stop();
        _viewModel.IsPlaying = false;
    }

    private void CompleteSceneLoad(bool hasTool)
    {
        _sceneService.ApplyAll(_viewModel.Joints);
        _viewModel.ToolStatus = hasTool
            ? $"已载入 {_viewModel.ToolFile}"
            : $"{_viewModel.ToolFile} 未提供（已预留，加入 Models 后重新载入）";
        _viewModel.IsLoaded = true;
        _viewModel.StatusMessage = "模型与 STEP 原始颜色已载入，关节层级与旋转轴配置就绪。";
    }

    private void BindJointEvents()
    {
        foreach (var joint in _viewModel.Joints)
        {
            joint.StateChanged += Joint_StateChanged;
        }
    }

    private void UnbindJointEvents()
    {
        foreach (var joint in _viewModel.Joints)
        {
            joint.StateChanged -= Joint_StateChanged;
        }
    }

    private void UpdateWindowTitle()
    {
        Title = $"{_viewModel.ModelName} · AnyCAD 运动仿真";
    }
}
