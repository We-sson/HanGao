using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace 悍高软件.Extension_Method
{

    /// <summary>
    /// 继承特征声明新的方法
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        internal StringValueAttribute( string value)
        {
            this.StringValue = value;
        }

        public string StringValue { set; get; }

   

    }

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
            StringValueAttribute[] attrs =
                fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attrs.Length > 0 ? attrs[0].StringValue : enumValue.ToString();
        }
    }

}
