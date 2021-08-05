using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using 悍高软件.Extension_Method;
using static Soceket_KUKA.Models.KUKA_Value_Type;

namespace 悍高软件.Extension_Method
{


    /// <summary>
    /// 继承特征声明新的方法,用于报错字符串变量名称
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        internal StringValueAttribute( string value)
        {
            StringValue = value;
        }
        public string StringValue { set; get; }
    }

    /// <summary>
    /// 用于保存属性区域
    /// </summary>
    public class UserAreaAttribute : Attribute
    {
        public string  UserArea { set; get; }
        internal UserAreaAttribute(  string  value)
        {
            UserArea = value;
        }
    }

    public class KUKA_ValueType
    {
        public string BingdingValue { set; get; } = "";
        public Value_Type SetValueType { set; get; } = Value_Type.Null;
        public bool Binding_Start { set; get; } = false;
    }

    /// <summary>
    /// 用于保存绑定对呀变量值，锁定俩端值数据：参数1：绑定属性名称，参数2：属性类似枚举
    /// </summary>
    public class BingdingValueAttribute : Attribute
    {

        public KUKA_ValueType KUKA_Value { set; get; } = new KUKA_ValueType();

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="value">绑定属性名称</param>
        /// <param name="_enum">属性类型</param>
        internal BingdingValueAttribute(string value, Value_Type _enum,bool _Start)
        {
            KUKA_Value.BingdingValue = value;
            KUKA_Value.SetValueType = _enum;
            KUKA_Value.Binding_Start = _Start;


        }
         

        }

    }


    /// <summary>
    /// 用于设定库卡端的属性类型
    /// </summary>
    public class SetValueTypeAttribute : Attribute
    {
        public Value_Type SetValueType { set; get; }

        internal SetValueTypeAttribute(Value_Type value)
        {
            SetValueType = value;
        }

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
            StringValueAttribute[] attrs =fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attrs.Length > 0 ? attrs[0].StringValue : enumValue.ToString();
        }

        /// <summary>
        /// 获取特性 (DisplayAttribute) 的区域名称；如果未使用，则返回空。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string   GetAreaValue(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            UserAreaAttribute[] attrs =
                fieldInfo.GetCustomAttributes(typeof(UserAreaAttribute), false) as UserAreaAttribute[];

            return attrs.Length > 0 ? attrs[0].UserArea : string.Empty;
        }

        /// <summary>
        /// 获取特性 (DisplayAttribute) 的区域绑定值名称；如果未使用，则返回空。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static KUKA_ValueType GetBingdingValue(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            BingdingValueAttribute[] attrs =
                fieldInfo.GetCustomAttributes(typeof(BingdingValueAttribute), true) as BingdingValueAttribute[];

            return attrs.Length > 0 ? attrs[0].KUKA_Value : new KUKA_ValueType() { } ;


    }

}


