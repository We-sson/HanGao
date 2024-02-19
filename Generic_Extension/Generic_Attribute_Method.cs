using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;


namespace Generic_Extension
{

    /// <summary>
    /// 设置字符串变量名称
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string _StringValue)
        {
            StringValue = _StringValue;
        }
        public string StringValue { set; get; }


    }

    /// <summary>
    /// 属性顺序
    /// </summary>
    public   class ParameterOrderAttribute : Attribute
    {
        public int Order { get;  set; }
        public ParameterOrderAttribute(int order)
        {
            Order = order;
        }
        public int GetOrder()
        {
            return Order;
        }
    }

    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// 获取特性 (DisplayAttribute) 的名称；如果未使用该特性，则返回枚举的名称。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetStringValue(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attrs.Length > 0 ? attrs[0].StringValue : enumValue.ToString();
        }

    }


    /// <summary>
    /// 在UI线上Description特征的枚举转换器
    /// </summary>
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type) : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (null != value)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());

                    if (null != fi)
                    {
                        var attributes =
                            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description)))
                            ? attributes[0].Description
                            : value.ToString();
                    }
                }

                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }





}
