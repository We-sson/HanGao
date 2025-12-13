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
    public  class View_Converter
    {
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



    ///// <summary>
    ///// 枚举特性文本显示装欢去
    ///// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {

        public static string? GetEnumDescription(object enumObj)
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

}
