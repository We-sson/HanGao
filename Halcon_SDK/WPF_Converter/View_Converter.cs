using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Halcon_SDK_DLL.WPF_Converter
{
    public class View_Converter
    {
    }




    /// <summary>
    /// View 枚举转换器
    /// </summary>
    public class Halcon_StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string @String)
            {
                if (@String.ToLower() == "auto")
                {
                    return (double)0;
                }
                else
                {
                    return double.Parse(@String);
                }
            }
            else
            {
                return value;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value.ToString();


        }

    }


    /// <summary>
    /// View 枚举转换器
    /// </summary>
    public class Halcon_EnumConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int enumValue = 0;
            if (parameter is Type type)
            {
                enumValue = (int)Enum.Parse(type, value.ToString()!);
            }
            return enumValue;
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Enum? returnValue = default;
            if (parameter is Type @type)
            {
                returnValue = (Enum)Enum.Parse(@type, value.ToString()!);
            }
            return returnValue;

        }
    }








    /// <summary>
    /// View 枚举转换器
    /// </summary>
    public class ValueToObject_Converter : IValueConverter
    {

    

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToType(value, targetType);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            return ConvertToType(value, targetType);
        }

        private static object? ConvertToType(object value, Type targetType)
        {
            try
            {
                if (value != null && (object)value != string.Empty)
                {

                if (targetType == typeof(object)) return value;
                var converter = TypeDescriptor.GetConverter(targetType);
                return converter.ConvertFrom(value.ToString()!);
                }
                return 0;

            }
            catch
            {
                var errorValue = value;
                throw new Exception(
                    $"在 TrueFalseValues 类中使用 TypeConverter 尝试将 { errorValue } 转换为 { targetType.Name}  类型时失败");
        }
}


    }

    public class SliderValue_Converter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        { 
            // 检查输入参数的有效性
            if (values == null )
                return DependencyProperty.UnsetValue;

            // 解析 Slider 的 Value 和 Slider 实例
            var currentValue = values as double?;

            // 如果 Slider 或 Value 为空，返回默认显示值
            if (currentValue == null || values is not Slider slider)
                return "N/A"; // 可根据需求更改为默认显示值

            // 判断是否为 Min 或 Max
            if (Math.Abs(currentValue.Value - slider.Minimum) < 0.0001)
                return "Min";
            else if (Math.Abs(currentValue.Value - slider.Maximum) < 0.0001)
                return "Max";
            else
                return currentValue.Value.ToString("F2"); // 格式化为两位小数
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 获取 Slider 实例
            var slider = parameter as Slider;
            if (slider != null && value is string input)
            {
                // 处理特殊值
                if (input.Equals("Min", StringComparison.OrdinalIgnoreCase))
                    return slider.Minimum;
                if (input.Equals("Max", StringComparison.OrdinalIgnoreCase))
                    return slider.Maximum;

                // 尝试将字符串转换为数值
                if (double.TryParse(input, out double result))
                {
                    // 确保值在范围内
                    result = Math.Max(slider.Minimum, Math.Min(result, slider.Maximum));
                    return result;
                }
            }
            if (slider != null && value is double input1)
            {
                return input1;
            }

            // 默认返回 Slider 的当前值
            return value;
        }
    }




    public class MultiTypeValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return Binding.DoNothing;

            if (values[0] is double currentValue &&
                values[1] is double minValue &&
                values[2] is double maxValue)
            {
                // 如果当前值等于最小值或最大值，返回 "min" 或 "max"
                if (Math.Abs(currentValue - minValue) < double.Epsilon)
                    return "min";
                if (Math.Abs(currentValue - maxValue) < double.Epsilon)
                    return "max";

                // 否则返回当前值
                return currentValue.ToString();
            }
            return Binding.DoNothing;
        }

        public  double minValue;
        public double maxValue
;


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string input && targetTypes.Length >= 3)
            {
                
                // 确保绑定值中包含有效的最小值和最大值
                if (targetTypes[1] == typeof(double) && targetTypes[2] == typeof(double))
                {
                    //double minValue = System.Convert.ToDouble(targetTypes[1]);
                    //double maxValue = System.Convert.ToDouble(targetTypes[2]);

                    var  _minValue = System.Convert.ToString(targetTypes[1]);

                    // 如果输入是 "min" 或 "max"，返回对应的最小值或最大值
                    if (input.Equals("min", StringComparison.OrdinalIgnoreCase))
                        return new object[] { minValue, Binding.DoNothing, Binding.DoNothing };
                    if (input.Equals("max", StringComparison.OrdinalIgnoreCase))
                        return new object[] { maxValue, Binding.DoNothing, Binding.DoNothing };

                    // 尝试将字符串解析为数字
                    if (double.TryParse(input, out double parsedValue))
                    {
                        // 限制值在范围内
                        if (parsedValue < minValue)
                            return new object[] { minValue, Binding.DoNothing, Binding.DoNothing };
                        if (parsedValue > maxValue)
                            return new object[] { maxValue, Binding.DoNothing, Binding.DoNothing };

                        return new object[] { parsedValue, Binding.DoNothing, Binding.DoNothing };
                    }
                }
            }

            return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }







    /// <summary>
    /// 控件状态枚举转换器
    /// </summary>
    public class Radio_CheckedToEnumConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
   
            return Enum.Parse(targetType, parameter!.ToString()!);
  
        }


    }



    /// <summary>
    /// 枚举特性文本显示装欢去
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {

        public  static   string? GetEnumDescription(object enumObj)
        {
            try
            {

                if (enumObj == null) { return enumObj?.ToString()!; }


                FieldInfo? fieldInfo = enumObj.GetType().GetField(enumObj.ToString()!);


                if (fieldInfo == null) { return enumObj?.ToString()!; }


                object[] attribArray = fieldInfo.GetCustomAttributes(false);

                if (attribArray!.Length == 0 || attribArray == null)
                {
                    return enumObj.ToString()!;
                }
                else
                {
                    DescriptionAttribute? attrib = attribArray[0] as DescriptionAttribute;
                    return attrib!.Description;
                }
            }
            catch (Exception)
            {
                return enumObj?.ToString()!;


            }
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Enum _Enum = value as Enum;
            //Enum myEnum = (Enum)value;
            string? description = new(GetEnumDescription(value));
            return description;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }




    /// <summary>
    /// 拓展标记枚举转换方法
    /// </summary>
    public class EnumBindingSourceExtension : MarkupExtension
    {


        public Type? Enum_List { set; get; } 

        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("类型必须是枚举类型");
            Enum_List = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            //返回枚举集合
            return Enum.GetValues(Enum_List!).Cast<Enum>();

        }


    }






}
