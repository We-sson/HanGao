using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
    //public class EnumToItems_EnumConverter : IValueConverter
    //{


    //    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {

    //        IEnumerable<Enum>? EnumList = default;
    //        if (value != null)
    //        {

    //            EnumList = Enum.GetValues(value.GetType()).Cast<Enum>();

    //        }
    //        return EnumList;
    //        //int enumValue = 0;
    //        //if (parameter is Type)
    //        //{
    //        //    enumValue = (int)Enum.Parse((Type)parameter, value.ToString()!);
    //        //}
    //        //return enumValue;
    //    }
    //    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {

    //        //Enum? returnValue = default;
    //        //if (parameter is Type @type)
    //        //{
    //        //    returnValue = (Enum)Enum.Parse(@type, value.ToString()!);
    //        //}
    //        //return returnValue;

    //        return value;

    //    }
    //}


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





    /// <summary>
    /// 控件状态枚举转换器
    /// </summary>
    public class Radio_CheckedToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == parameter.ToString())
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value==null)
            {
                return Binding.DoNothing;

            }
            if (Boolean.Parse(value.ToString()!) )
            {
                TypeDescriptor.GetConverter(targetType);
                return (Enum)Enum.Parse(targetType, parameter.ToString()!);
            }
            else
            {
                return Binding.DoNothing;
            }
       
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
