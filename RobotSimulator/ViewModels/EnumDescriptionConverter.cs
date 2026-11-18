using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace RobotSimulator.ViewModels;

/// <summary>用 [Description] 显示枚举中文名；没有描述时退回枚举名。</summary>
public sealed class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is Enum item
            ? item.GetType().GetField(item.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? item.ToString()
            : value?.ToString() ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
