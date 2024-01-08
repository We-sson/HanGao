using System;
using System.Net.Sockets;
using System.Reflection;

namespace Roboto_Socket_Library.Models
{




    public class KUKA_SDK_Models
    {


        /// <summary>
        /// 接收变量基本信息
        /// </summary>
        public object Reveice_Inf { set; get; } = new object();

        /// <summary>
        /// 接收字节原数据
        /// </summary>
        public Socket_Modesl_Byte Receive_Byte { set; get; } = new Socket_Modesl_Byte();


        /// <summary>
        /// 接收空字节流属性
        /// </summary>
        public byte[] Byte_Write_Receive { set; get; } = new byte[1024 * 10];
        public byte[] Byte_Read_Receive { set; get; } = new byte[1024 * 10];

        /// <summary>
        /// 接收字节长度
        /// </summary>
        public int Byte_Leng = 0;

        /// <summary>
        /// 接收到值
        /// </summary>
        public string? Receive_Var { set; get; } = string.Empty;

        /// <summary>
        /// 接收字节组属性
        /// </summary>
        public byte[] Write_Byte { set; get; } = Array.Empty<byte>();
        /// <summary>
        /// 发送字节组属性
        /// </summary>
        public byte[] Send_Byte { set; get; } = Array.Empty<byte>();




        /// <summary>
        /// 写入属性
        /// </summary>
        public Read_Write_Enum Read_Write_Type { set; get; } = Read_Write_Enum.Null;




    }




    public  class Socket_SendInfo_Model
    {

        /// <summary>
        /// 接收变量用户信息
        /// </summary>
        public object Reveice_Inf { set; get; }=new object();


        /// <summary>
        /// 值名称
        /// </summary>
        public string Var_Name { set; get; } = string.Empty;

        /// <summary>
        /// 写入值值
        /// </summary>
        public string Write_Var { set; get; } = string.Empty;
        /// <summary>
        /// 值ID
        /// </summary>
        public int Var_ID { set; get; }

    }

  



    /// <summary>
    /// 机器人端变量属性
    /// </summary>
    public enum Value_Type
    {
        String,
        Int,
        Char,
        Bool,
        Enum,
        Null
    }
    /// <summary>
    /// 写入读取枚举属性
    /// </summary>
    public enum Read_Write_Enum { Null = -1, Read, Write, One_Read }





    public class Socket_Models_Server
    {


        #region 属性

        /// <summary>
        /// 服务器接收字节缓存
        /// </summary>
        public byte[] Server_Recv_Byte { set; get; } = Array.Empty<byte>();

        /// <summary>
        /// 服务器发送数据
        /// </summary>
        public string Server_Send_Data { set; get; } = string.Empty;

        /// <summary>
        /// 库卡连接对象
        /// </summary>
        public Socket? Server_Client { set; get; } 




        #endregion


        #region 方法

        /// <summary>
        /// 初始化接收字节
        /// </summary>
        public void Ini_Byte()
        {
            if (Server_Client != null)
            {
                Server_Recv_Byte = new byte[Server_Client.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 关闭当前连接Socket对象
        /// </summary>
        public void Server_Closer()
        {
            if (Server_Client != null)
            {
                Server_Client.Shutdown(SocketShutdown.Both);
                Server_Client.Close();

            }

        }


        #endregion

    }

    /// <summary>
    /// 接收字节分解属性
    /// </summary>

    public class Socket_Modesl_Byte
    {
        public int Byte_ID { set; get; } = -1;
        public int Byte_Val_Total_Length { set; get; } = -1;
        public int Byte_Return_Tpye { set; get; } = -1;
        public int Byte_Val_Length { set; get; } = -1;
        public string Message_Show { set; get; } = string.Empty;
        public int Byte_Write_Type { set; get; } = -1;
        public byte[] Byte_data { set; get; } = Array.Empty<byte>();


    }

    #region kuka获取指令拓展

    /// <summary>
    /// 设置变量读取类型,默认循环类型
    /// </summary>
    public class SetReadTypeAttribute : Attribute
    {

        public SetReadTypeAttribute(Read_Type_Enum _Read_Type)
        {

            Read_Type = _Read_Type;
        }

        public Read_Type_Enum Read_Type { set; get; } = Read_Type_Enum.Loop_Read;
        /// <summary>
        /// 变量读取类型
        /// </summary>

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
        public Binding_Type Binding_Start { set; get; } = Binding_Type.OneWay;

        /// <summary>
        /// 绑定属性
        /// </summary>

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




    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtensions
    {


        /// <summary>
        /// 获取特性 (DisplayAttribute) 的区域名称；如果未使用，则返回空。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetAreaValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            UserAreaAttribute[]? attrs =
                fieldInfo?.GetCustomAttributes(typeof(UserAreaAttribute), false) as UserAreaAttribute[];

            return attrs!.Length > 0 ? attrs[0].UserArea : string.Empty;
        }

        /// <summary>
        /// 获取特性 (DisplayAttribute) 的区域绑定值名称；如果未使用，则返回空。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static KUKA_ValueType_Model GetBingdingValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            BingdingValueAttribute[]? attrs =
                fieldInfo!.GetCustomAttributes(typeof(BingdingValueAttribute), true) as BingdingValueAttribute[];

            return attrs!.Length > 0 ? attrs[0].KUKA_Value : new KUKA_ValueType_Model() { };


        }
        /// <summary>
        /// 读取库卡值的类型
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static Read_Type_Enum GetValueReadTypeValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            SetReadTypeAttribute[]? attrs =
                fieldInfo!.GetCustomAttributes(typeof(SetReadTypeAttribute), false) as SetReadTypeAttribute[];

            return attrs!.Length > 0 ? attrs[0].Read_Type : Read_Type_Enum.Loop_Read;


        }

    }




    public enum Read_Type_Enum
    {
        Loop_Read,
        One_Read
    }

    public enum Binding_Type
    {
        OneWay,
        TwoWay,
    }

    #endregion
}
