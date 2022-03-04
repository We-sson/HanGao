using HanGao.Extension_Method;
using System;
using System.Reflection;
using static HanGao.Extension_Method.KUKA_ValueType_Model;
using static HanGao.Extension_Method.StringValueAttribute;
using static HanGao.Extension_Method.ValueReadTypeAttribute;
using static Soceket_KUKA.Models.KUKA_Value_Type;

namespace HanGao.Extension_Method
{

    /// <summary>
    /// 设置变量读取类型,默认循环类型
    /// </summary>
    public class ValueReadTypeAttribute : Attribute
    {

        public ValueReadTypeAttribute(Read_Type_Enum _Read_Type)
        {
        
            Read_Type = _Read_Type;
        }

        public Read_Type_Enum Read_Type { set; get; } = Read_Type_Enum.Loop_Read;
        /// <summary>
        /// 变量读取类型
        /// </summary>
        public enum Read_Type_Enum
        {
            Loop_Read,
            One_Read
        }
    }

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
    /// 用于保存属性区域
    /// </summary>
    public class UserAreaAttribute : Attribute
    {
        public string UserArea { set; get; }
        public UserAreaAttribute(string value)
        {
            UserArea = value;
        }
    }

    /// <summary>
    /// 设置库卡变量类型属性
    /// </summary>
    public class KUKA_ValueType_Model
    {
        public string BingdingValue { set; get; } = "";
        public Value_Type SetValueType { set; get; } = Value_Type.Null;
        public Binding_Type Binding_Start { set; get; } =  Binding_Type.OneWay;

        /// <summary>
        /// 绑定属性
        /// </summary>
        public enum Binding_Type
        {
            OneWay,
            TwoWay,
        }
    }

    /// <summary>
    /// 用于保存绑定对呀变量值，锁定俩端值数据：参数1：绑定属性名称，参数2：属性类似枚举, 参数3: 属性双向绑定
    /// </summary>
    public class BingdingValueAttribute : Attribute
    {

        public KUKA_ValueType_Model KUKA_Value { set; get; } = new KUKA_ValueType_Model();

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="value">绑定属性名称</param>
        /// <param name="_enum">属性类型</param>
        /// 
        public BingdingValueAttribute(string value, Value_Type _enum, Binding_Type _Start)
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

    public SetValueTypeAttribute(Value_Type value)
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
        StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

        return attrs.Length > 0 ? attrs[0].StringValue : enumValue.ToString();
    }

    /// <summary>
    /// 获取特性 (DisplayAttribute) 的区域名称；如果未使用，则返回空。
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetAreaValue(this Enum enumValue)
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
    public static KUKA_ValueType_Model GetBingdingValue(this Enum enumValue)
    {
        FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        BingdingValueAttribute[] attrs =
            fieldInfo.GetCustomAttributes(typeof(BingdingValueAttribute), true) as BingdingValueAttribute[];

        return attrs.Length > 0 ? attrs[0].KUKA_Value : new KUKA_ValueType_Model() { };


    }

    public static Read_Type_Enum GetValueReadTypeValue(this Enum enumValue)
    {
        FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        ValueReadTypeAttribute[] attrs =
            fieldInfo.GetCustomAttributes(typeof(ValueReadTypeAttribute), false) as ValueReadTypeAttribute[];

        return attrs.Length > 0 ? attrs[0].Read_Type : Read_Type_Enum.Loop_Read;


    }

}


