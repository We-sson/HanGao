using PropertyChanged;
using Roboto_Socket_Library.Model;
using System.Text;

namespace Roboto_Socket_Library
{
    /// <summary>
    /// 将 Socket 原始字节转换为适合界面展示的文本。
    /// <see cref="AddINotifyPropertyChangedInterfaceAttribute"/> 由 Fody 在编译期注入属性变更通知，供 UI 绑定刷新。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Socket_Data_Converts
    {
        /// <summary>
        /// 创建转换器；编码模式和占位文本使用属性声明处的默认值。
        /// </summary>
        public Socket_Data_Converts()
        {

        }

        // 历史方案曾在 Raw_Data 的 setter 中自动转换；当前改为显式调用
        // Data_Converts_Str_Method，避免仅赋值就执行格式化带来的隐式开销。
        //private byte[] _Raw_Data = Array.Empty<byte>();

        //public byte[] Raw_Data
        //{
        //    get { return _Raw_Data; }
        //    set { _Raw_Data = value; Data_Converts_Str_Method(); }
        //}

        /// <summary>
        /// 控制原始字节以协议文本还是十六进制字节序列显示。
        /// </summary>
        public Socket_Data_Type_Enum Socket_Data_Type { set; get; } = Socket_Data_Type_Enum.ASCII;

        /// <summary>
        /// 最近一次转换后的显示文本；尚未转换时为占位符 <c>....</c>。
        /// </summary>
        public string Data_Converts_Str { set; get; } = "....";

        /// <summary>
        /// 按当前默认文本编码或 HEX 模式转换一帧原始数据；兼容既有无协议参数的事件委托签名。
        /// </summary>
        /// <param name="Raw_Data">本次要展示的完整字节数组。</param>
        /// <remarks>空数组不会覆盖上一次转换结果。</remarks>
        public void Data_Converts_Str_Method(byte[] Raw_Data)
            => Data_Converts_Str_Method(Raw_Data, Socket_Robot_Protocols_Enum.通用);

        /// <summary>
        /// 按 <see cref="Socket_Data_Type"/> 与通信协议转换一帧原始数据（不区分收发方向），并更新 <see cref="Data_Converts_Str"/>。
        /// </summary>
        /// <param name="Raw_Data">本次要展示的完整字节数组。</param>
        /// <param name="protocol">由通讯设置统一传入的当前机器人协议。</param>
        public void Data_Converts_Str_Method(byte[] Raw_Data, Socket_Robot_Protocols_Enum protocol)
        {

            string _Data_string = string.Empty;

            if (Raw_Data.Length > 0)
            {

                switch (Socket_Data_Type)
                {
                    case Socket_Data_Type_Enum.ASCII:
                        // 统一按传入协议格式化：二进制报文小端解码，文本协议按 UTF-8 原文展示。
                        _Data_string = Robot_Socket_Protocol.FormatMonitorData(protocol, Raw_Data);

                        break;
                    case Socket_Data_Type_Enum.HEX:
                        // X2 保证每个字节固定显示两位，空格便于人工核对报文边界；BitConverter 一次生成避免 O(N²) 拼接。
                        _Data_string = BitConverter.ToString(Raw_Data).Replace('-', ' ') + " ";
                        break;
                }


                Data_Converts_Str = _Data_string;

            }
        }

    }

    /// <summary>
    /// 指定 Socket 字节在监视界面中的显示格式。
    /// </summary>
    public enum Socket_Data_Type_Enum
    {
        /// <summary>按 UTF-8 文本显示。</summary>
        ASCII,

        /// <summary>按空格分隔的两位十六进制字节显示。</summary>
        HEX
    }
}
