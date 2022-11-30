using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
}
