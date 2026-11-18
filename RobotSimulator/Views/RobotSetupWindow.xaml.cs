using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using RobotSimulator.ViewModels;

namespace RobotSimulator.Views;

/// <summary>开发用配置页；目录选择、覆盖确认和窗口导航由视图负责。</summary>
public partial class RobotSetupWindow : Window
{
    private readonly RobotSetupViewModel _viewModel = new();

    public RobotSetupWindow()
    {
        InitializeComponent();
        MinWidth = Math.Min(MinWidth, SystemParameters.WorkArea.Width);
        MinHeight = Math.Min(MinHeight, SystemParameters.WorkArea.Height);
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        DataContext = _viewModel;
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName is nameof(_viewModel.StepIndex) or nameof(_viewModel.SelectedJoint))
            {
                SourceGuideScroll.ScrollToTop();
                ParameterScroll.ScrollToTop();
            }
            if (e.PropertyName == nameof(_viewModel.ValidationMessage)
                && !string.IsNullOrEmpty(_viewModel.ValidationMessage)) ValidationSummary.Focus();
        };
    }

    private void ChooseDirectory(object sender, RoutedEventArgs e)
    {
        var picker = new OpenFolderDialog { Title = "选择含基座及 J1～J6 的机器人 STEP 源目录", Multiselect = false };
        if (picker.ShowDialog(this) != true) return;
        if (_viewModel.HasUnsavedChanges
            && System.Windows.MessageBox.Show(this, "更换目录会丢弃当前未保存的输入，是否继续？", "更换源目录",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try { _viewModel.SelectDirectory(picker.FolderName); }
        catch (Exception exception) { ShowError(exception); }
    }

    private void SaveConfiguration(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_viewModel.OutputPath)
            && System.Windows.MessageBox.Show(this, $"是否用当前校准参数覆盖源配置？\n{_viewModel.OutputPath}", "覆盖机器人源配置",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try { _viewModel.Save(); }
        catch (Exception exception) { ShowError(exception); }
    }

    private void ShowError(Exception exception) =>
        System.Windows.MessageBox.Show(this, exception.Message, "机器人配置未完成", MessageBoxButton.OK, MessageBoxImage.Error);

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_viewModel.HasUnsavedChanges
            && System.Windows.MessageBox.Show(this, "配置尚未保存，是否丢弃输入并关闭？", "未保存的配置",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) e.Cancel = true;
        base.OnClosing(e);
    }
}
