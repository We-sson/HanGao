using System;
using System.Net.Sockets;
using System.Reflection;

namespace Roboto_Socket_Library.Models
{
    /// <summary>
    /// 保存一次 KUKA 变量读写请求及其响应解析状态。
    /// 同一个对象会依次经过组帧、发送、接收、解帧和委托回传几个阶段。
    /// </summary>
    public class KUKA_SDK_Models
    {


        /// <summary>
        /// 调用方附带的业务对象；响应完成后原样随委托返回，用于定位对应的界面属性或变量定义。
        /// </summary>
        public object Reveice_Inf { set; get; } = new object();

        /// <summary>
        /// 已解析的 KUKA 响应帧字段。
        /// </summary>
        public Socket_Modesl_Byte Receive_Byte { set; get; } = new Socket_Modesl_Byte();


        /// <summary>
        /// 写请求通道的响应接收缓冲。
        /// </summary>
        public byte[] Byte_Write_Receive { set; get; } = new byte[1024 * 10];

        /// <summary>
        /// 读请求通道的响应接收缓冲。
        /// </summary>
        public byte[] Byte_Read_Receive { set; get; } = new byte[1024 * 10];

        /// <summary>
        /// 最近一次 <c>EndReceive</c> 返回的有效字节数。
        /// </summary>
        public int Byte_Leng = 0;

        /// <summary>
        /// 从响应帧中提取出的变量文本值。
        /// </summary>
        public string? Receive_Var { set; get; } = string.Empty;

        /// <summary>
        /// 预留的写入字节数据；当前主流程使用 <see cref="Send_Byte"/> 发送。
        /// </summary>
        public byte[] Write_Byte { set; get; } = Array.Empty<byte>();
        /// <summary>
        /// 已按 KUKA 变量协议组装、等待发送的完整请求帧。
        /// </summary>
        public byte[] Send_Byte { set; get; } = Array.Empty<byte>();




        /// <summary>
        /// 标识本次请求走读取通道、写入通道，还是单次读取模式。
        /// </summary>
        public Read_Write_Enum Read_Write_Type { set; get; } = Read_Write_Enum.Null;




    }
    /// <summary>
    /// 描述一个待读取或待写入的机器人变量。
    /// 批量 API 使用该模型生成协议帧，并用 <see cref="Reveice_Inf"/> 将结果关联回业务对象。
    /// </summary>
    public  class Socket_SendInfo_Model
    {

        /// <summary>
        /// 调用方附带的业务上下文，网络层不会修改其内容。
        /// </summary>
        public object Reveice_Inf { set; get; }=new object();


        /// <summary>
        /// 机器人控制器中的变量名称。
        /// </summary>
        public string Var_Name { set; get; } = string.Empty;

        /// <summary>
        /// 写操作要提交的文本值；读操作不会使用该字段。
        /// </summary>
        public string Write_Var { set; get; } = string.Empty;
        /// <summary>
        /// 请求标识，用于将机器人响应与请求对应起来。
        /// </summary>
        public int Var_ID { set; get; }

    }

  



    /// <summary>
    /// 机器人端变量属性
    /// </summary>
    public enum Value_Type
    {
        /// <summary>字符串变量。</summary>
        String,

        /// <summary>整数变量。</summary>
        Int,

        /// <summary>单字符变量。</summary>
        Char,

        /// <summary>布尔变量。</summary>
        Bool,

        /// <summary>以枚举形式映射的变量。</summary>
        Enum,

        /// <summary>未指定变量类型。</summary>
        Null
    }

    /// <summary>
    /// 指定请求用途以及应使用的 Socket 通道。
    /// </summary>
    public enum Read_Write_Enum
    {
        /// <summary>尚未设置请求类型。</summary>
        Null = -1,

        /// <summary>在持久读连接中循环读取。</summary>
        Read,

        /// <summary>通过独立写连接写入变量。</summary>
        Write,

        /// <summary>建立读连接、执行一批读取后立即关闭。</summary>
        One_Read
    }

    /// <summary>
    /// 保存旧版多客户端服务器中的单个客户端连接及其收发缓存。
    /// </summary>
    public class Socket_Models_Server
    {


        #region 属性

        /// <summary>
        /// 从该客户端接收数据时使用的缓冲区。
        /// </summary>
        public byte[] Server_Recv_Byte { set; get; } = Array.Empty<byte>();

        /// <summary>
        /// 预留的待发送文本；关闭连接时会清空。
        /// </summary>
        public string Server_Send_Data { set; get; } = string.Empty;

        /// <summary>
        /// 该状态对象对应的客户端 Socket。
        /// </summary>
        public Socket? Server_Client { set; get; } 




        #endregion


        #region 方法

        /// <summary>
        /// 按 Socket 的系统接收缓冲大小重新分配应用层接收缓冲。
        /// </summary>
        public void Ini_Byte()
        {
            if (Server_Client != null)
            {
                Server_Recv_Byte = new byte[Server_Client.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 双向关闭并释放当前客户端连接。
        /// </summary>
        public void Server_Closer()
        {
            if (Server_Client != null)
            {
                // Shutdown 先通知对端不再收发，Close 再释放本地句柄。
                Server_Client.Shutdown(SocketShutdown.Both);
                Server_Client.Close();

            }

        }


        #endregion

    }

    /// <summary>
    /// KUKA 变量协议响应帧的解码结果。
    /// 字段顺序与线上的二进制布局一致，便于核对偏移和长度。
    /// </summary>
    public class Socket_Modesl_Byte
    {
        /// <summary>响应所属的两字节请求 ID。</summary>
        public int Byte_ID { set; get; } = -1;

        /// <summary>协议头声明的后续数据总长度。</summary>
        public int Byte_Val_Total_Length { set; get; } = -1;

        /// <summary>响应操作类型，例如读取或写入。</summary>
        public int Byte_Return_Tpye { set; get; } = -1;

        /// <summary>返回变量值的字节长度。</summary>
        public int Byte_Val_Length { set; get; } = -1;

        /// <summary>按 ASCII 解码后的变量值或错误消息。</summary>
        public string Message_Show { set; get; } = string.Empty;

        /// <summary>写操作的结果标志，协议值 1 表示成功、0 表示失败。</summary>
        public int Byte_Write_Type { set; get; } = -1;

        /// <summary>本次接收的有效响应帧，不包含接收缓冲的未使用空间。</summary>
        public byte[] Byte_data { set; get; } = Array.Empty<byte>();


    }

    #region kuka获取指令拓展

    /// <summary>
    /// 标注枚举成员或其他目标应采用循环读取还是单次读取；未标注时默认为循环读取。
    /// </summary>
    public class SetReadTypeAttribute : Attribute
    {
        /// <summary>
        /// 创建读取策略特性。
        /// </summary>
        /// <param name="_Read_Type">目标变量的读取策略。</param>
        public SetReadTypeAttribute(Read_Type_Enum _Read_Type)
        {

            Read_Type = _Read_Type;
        }
        /// <summary>
        /// 被标注变量的读取策略。
        /// </summary>
        public Read_Type_Enum Read_Type { set; get; } = Read_Type_Enum.Loop_Read;

    }



    /// <summary>
    /// 在枚举成员上保存业务区域名称，供反射读取并用于变量分组。
    /// </summary>
    public class UserAreaAttribute : Attribute
    {
        /// <summary>业务区域名称。</summary>
        public string UserArea { set; get; }

        /// <summary>
        /// 创建区域标记。
        /// </summary>
        /// <param name="value">要附加到枚举成员的区域名称。</param>
        public UserAreaAttribute(string value)
        {
            UserArea = value;
        }
    }

    /// <summary>
    /// 描述业务枚举成员与 KUKA 变量之间的绑定元数据。
    /// </summary>
    public class KUKA_ValueType_Model
    {
        /// <summary>对应的机器人变量名或业务绑定键。</summary>
        public string BingdingValue { set; get; } = "";

        /// <summary>机器人变量的数据类型。</summary>
        public Value_Type SetValueType { set; get; } = Value_Type.Null;

        /// <summary>数据只从机器人流向应用，还是允许双向同步。</summary>
        public Binding_Type Binding_Start { set; get; } = Binding_Type.OneWay;

    }

    /// <summary>
    /// 在枚举成员上声明 KUKA 变量名、值类型和绑定方向。
    /// </summary>
    public class BingdingValueAttribute : Attribute
    {
        /// <summary>由构造参数汇总出的绑定元数据。</summary>
        public KUKA_ValueType_Model KUKA_Value { set; get; } = new KUKA_ValueType_Model();

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="value">绑定属性名称</param>
        /// <param name="_enum">属性类型</param>
        /// <param name="_Start">单向或双向绑定策略。</param>
        public BingdingValueAttribute(string value, Value_Type _enum, Binding_Type _Start)
        {
            KUKA_Value.BingdingValue = value;
            KUKA_Value.SetValueType = _enum;
            KUKA_Value.Binding_Start = _Start;


        }


    }



    /// <summary>
    /// 单独标注 KUKA 端变量的数据类型。
    /// </summary>
    public class SetValueTypeAttribute : Attribute
    {
        /// <summary>标注的数据类型。</summary>
        public Value_Type SetValueType { set; get; }

        /// <summary>
        /// 创建变量类型标记。
        /// </summary>
        /// <param name="value">KUKA 端变量类型。</param>
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
        /// 读取枚举成员的 <see cref="UserAreaAttribute"/>；未标注时返回空字符串。
        /// </summary>
        /// <param name="enumValue">要检查的枚举值。</param>
        /// <returns>区域名称，或空字符串。</returns>
        public static string GetAreaValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            UserAreaAttribute[]? attrs =
                fieldInfo?.GetCustomAttributes(typeof(UserAreaAttribute), false) as UserAreaAttribute[];

            return attrs!.Length > 0 ? attrs[0].UserArea : string.Empty;
        }

        /// <summary>
        /// 读取枚举成员的 <see cref="BingdingValueAttribute"/>。
        /// </summary>
        /// <param name="enumValue">要检查的枚举值。</param>
        /// <returns>绑定元数据；未标注时返回使用默认值的新实例。</returns>
        public static KUKA_ValueType_Model GetBingdingValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            BingdingValueAttribute[]? attrs =
                fieldInfo!.GetCustomAttributes(typeof(BingdingValueAttribute), true) as BingdingValueAttribute[];

            return attrs!.Length > 0 ? attrs[0].KUKA_Value : new KUKA_ValueType_Model() { };


        }
        /// <summary>
        /// 读取枚举成员的 <see cref="SetReadTypeAttribute"/>。
        /// </summary>
        /// <param name="enumValue">要检查的枚举值。</param>
        /// <returns>标注的读取策略；未标注时返回 <see cref="Read_Type_Enum.Loop_Read"/>。</returns>
        public static Read_Type_Enum GetValueReadTypeValue(this Enum enumValue)
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            SetReadTypeAttribute[]? attrs =
                fieldInfo!.GetCustomAttributes(typeof(SetReadTypeAttribute), false) as SetReadTypeAttribute[];

            return attrs!.Length > 0 ? attrs[0].Read_Type : Read_Type_Enum.Loop_Read;


        }

    }

    /// <summary>
    /// 指定变量是在持久循环中读取，还是按需读取一次。
    /// </summary>
    public enum Read_Type_Enum
    {
        /// <summary>连接存续期间重复轮询。</summary>
        Loop_Read,

        /// <summary>执行一次或一批读取后结束。</summary>
        One_Read
    }

    /// <summary>
    /// 指定机器人变量与应用属性之间的数据流向。
    /// </summary>
    public enum Binding_Type
    {
        /// <summary>仅从数据源更新目标。</summary>
        OneWay,

        /// <summary>数据源和目标可互相更新。</summary>
        TwoWay,
    }

    #endregion
}
