using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Halcon_SDK_DLL.WPF_Converter
{
    public  class View_Converter
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
                if (@String == "auto")
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value.ToString();


        }

    }


    /// <summary>
    /// View 枚举转换器
    /// </summary>
    public class Halcon_EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int enumValue = 0;
            if (parameter is Type)
            {
                enumValue = (int)Enum.Parse((Type)parameter, value.ToString());
            }
            return enumValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Enum returnValue = default;
            if (parameter is Type @type)
            {
                returnValue = (Enum)Enum.Parse(@type, value.ToString());
            }
            return returnValue;

        }
    }

    /// <summary>
    /// 控件状态枚举转换器
    /// </summary>
    public class Radio_CheckedToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString()== parameter.ToString())
            {
            return true;

            }
            else
            {
                return false;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Boolean.Parse(value.ToString()))
            {
 
                return (Enum)Enum.Parse(targetType, parameter.ToString());
            }
            else
            {
                return null;
            }
        }
    }



    /// <summary>
    /// 枚举特性文本显示装欢去
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public  string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }



}
