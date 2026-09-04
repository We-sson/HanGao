using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Robot_Info_Mes.View
{
    /// <summary>
    /// 视图转换器命名空间的占位类型；具体转换逻辑由同文件中的转换器实现。
    /// </summary>
    public  class View_Converter
    {
    }




    /// <summary>
    /// XAML 标记扩展：把一个枚举类型展开成所有枚举值，供 ComboBox 等 ItemsSource 直接绑定。
    /// </summary>
    public class EnumBindingSourceExtension : MarkupExtension
    {


        /// <summary>
        /// 要展开的枚举类型。
        /// </summary>
        public Type? Enum_List { set; get; }

        /// <summary>
        /// 验证传入类型确为枚举，尽早暴露错误的 XAML 参数。
        /// </summary>
        /// <param name="enumType">需要作为选项来源的枚举类型。</param>
        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("类型必须是枚举类型");
            Enum_List = enumType;
        }

        /// <summary>
        /// 在 XAML 解析阶段返回枚举值集合。
        /// </summary>
        /// <param name="serviceProvider">WPF 提供的标记扩展服务上下文；此实现无需使用。</param>
        /// <returns>按枚举声明值组成的可枚举集合。</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            // 返回 Enum 而不是基础整数，使后续 Description 转换器仍能读取字段特性。
            return Enum.GetValues(Enum_List!).Cast<Enum>();

        }


    }



    /// <summary>
    /// 将枚举成员的 <see cref="DescriptionAttribute"/> 转换为面向用户的中文说明。
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        /// <summary>
        /// 读取枚举字段的 Description；未配置特性或反射失败时退回枚举名称。
        /// </summary>
        /// <param name="enumObj">待显示的枚举值。</param>
        /// <returns>描述文本；没有描述时为枚举值自身的字符串。</returns>
        public static string? GetEnumDescription(object enumObj)
        {
            try
            {

                if (enumObj == null) { return enumObj?.ToString()!; }


                // 枚举的特性挂在成员字段上，需先由成员名称找到 FieldInfo。
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

        /// <summary>
        /// 将绑定源枚举转换为界面显示文本。
        /// </summary>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Enum _Enum = value as Enum;
            //Enum myEnum = (Enum)value;
            string? description = new(GetEnumDescription(value));
            return description;
        }

        /// <summary>
        /// 保留传入值；该转换器实际用于单向显示，反向写入不负责解析枚举。
        /// </summary>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 将当前趋势图索引与某个 RadioButton 的固定索引进行双向转换。
    /// </summary>
    public class IndexEqualsConverter : IValueConverter
    {
        /// <summary>
        /// 当前索引等于 ConverterParameter 时选中对应按钮。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int currentIndex &&
                   TryGetIndex(parameter, out int targetIndex) &&
                   currentIndex == targetIndex;
        }

        /// <summary>
        /// 仅在按钮被选中时把其固定索引写回；取消选中不覆盖其他按钮刚写入的索引。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true && TryGetIndex(parameter, out int targetIndex))
            {
                return targetIndex;
            }

            return System.Windows.Data.Binding.DoNothing;
        }

        /// <summary>
        /// 统一解析 XAML 中以字符串形式传入的 ConverterParameter。
        /// </summary>
        private static bool TryGetIndex(object parameter, out int index)
        {
            return int.TryParse(parameter?.ToString(), out index);
        }
    }

}
