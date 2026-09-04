using PropertyChanged;
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
        /// 控制原始字节以 UTF-8 文本还是十六进制字节序列显示。
        /// </summary>
        public Socket_Data_Type_Enum Socket_Data_Type { set; get; } = Socket_Data_Type_Enum.ASCII;

        /// <summary>
        /// 最近一次转换后的显示文本；尚未转换时为占位符 <c>....</c>。
        /// </summary>
        public string Data_Converts_Str { set; get; } = "....";

        /// <summary>
        /// 按 <see cref="Socket_Data_Type"/> 转换一帧原始数据，并更新 <see cref="Data_Converts_Str"/>。
        /// </summary>
        /// <param name="Raw_Data">本次要展示的完整字节数组。</param>
        /// <remarks>空数组不会覆盖上一次转换结果。</remarks>
        public void Data_Converts_Str_Method(byte[] Raw_Data)
        {

            string _Data_string = string.Empty;

            if (Raw_Data.Length > 0)
            {

                switch (Socket_Data_Type)
                {
                    case Socket_Data_Type_Enum.ASCII:
                        // 协议文本统一按 UTF-8 解码；无效字节由 Encoding 的默认替换策略处理。
                        _Data_string = Encoding.UTF8.GetString(Raw_Data);

                        break;
                    case Socket_Data_Type_Enum.HEX:
                        // X2 保证每个字节固定显示两位，空格便于人工核对报文边界。
                        for (int i = 0; i < Raw_Data.Length; i++)
                        {
                            _Data_string += Raw_Data[i].ToString("X2") + " ";

                        }
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
    
