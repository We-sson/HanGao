using System;
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
}
