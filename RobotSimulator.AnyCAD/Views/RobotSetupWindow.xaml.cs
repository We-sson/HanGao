using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using RobotSimulator.ViewModels;
using Binding = System.Windows.Data.Binding;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Orientation = System.Windows.Controls.Orientation;

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
            if (e.PropertyName == nameof(_viewModel.StepIndex))
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

    /// <summary>本地内置图示的大图查看器，缩放后仍可滚动检查原始尺寸和文字。</summary>
    private void ShowReferenceImage(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string path } button) return;
        try
        {
            var source = new BitmapImage(new Uri($"pack://application:,,,{path}", UriKind.Absolute));
            var width = Math.Min(1160, SystemParameters.WorkArea.Width);
            var height = Math.Min(860, SystemParameters.WorkArea.Height);
            var scale = new ScaleTransform();
            var image = new Image { Source = source, Stretch = Stretch.None, LayoutTransform = scale };
            var zoom = new Slider
            {
                Minimum = 0.25, Maximum = 3, Width = 260,
                Value = Math.Clamp(Math.Min((width - 60) / source.Width, (height - 120) / source.Height), 0.25, 1)
            };
            System.Windows.Automation.AutomationProperties.SetName(zoom, "图示缩放比例");
            scale.ScaleX = scale.ScaleY = zoom.Value;
            zoom.ValueChanged += (_, _) => scale.ScaleX = scale.ScaleY = zoom.Value;
            var percentage = new TextBlock { Width = 115, VerticalAlignment = VerticalAlignment.Center };
            percentage.SetBinding(TextBlock.TextProperty, new Binding(nameof(Slider.Value)) { Source = zoom, StringFormat = "缩放 {0:P0}" });
            var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(12) };
            toolbar.Children.Add(percentage);
            toolbar.Children.Add(zoom);
            toolbar.Children.Add(new Button { Content = "关闭大图", IsCancel = true, Margin = new Thickness(16, 0, 0, 0) });
            var scroll = new ScrollViewer
            {
                Content = image, Background = Brushes.White,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var layout = new Grid();
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            layout.RowDefinitions.Add(new RowDefinition());
            layout.Children.Add(toolbar);
            Grid.SetRow(scroll, 1);
            layout.Children.Add(scroll);
            new Window
            {
                Owner = this, Title = System.Windows.Automation.AutomationProperties.GetName(button),
                Width = width, Height = height, MinWidth = 520, MinHeight = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = (Brush)FindResource("PanelBrush"), Foreground = Brushes.White, Content = layout
            }.ShowDialog();
        }
        catch (Exception exception) { ShowError(exception); }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_viewModel.HasUnsavedChanges
            && System.Windows.MessageBox.Show(this, "配置尚未保存，是否丢弃输入并关闭？", "未保存的配置",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) e.Cancel = true;
        base.OnClosing(e);
    }
}
