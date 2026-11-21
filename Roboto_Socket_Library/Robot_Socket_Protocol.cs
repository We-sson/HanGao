


using Roboto_Socket_Library.Model;
using System.Text;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

namespace Roboto_Socket_Library
{
    /// <summary>
    /// 在业务 DTO 与各机器人厂商线上报文之间进行双向转换。
    /// 构造时确定机器人协议和视觉功能，公开路由方法再把请求分派到对应的专用编码/解码方法。
    /// </summary>
    /// <remarks>
    /// KUKA 使用 UTF-8 XML，ABB 使用带长度字段的二进制布局，FANUC/川崎的 MES 状态使用冒号、逗号和分号分隔文本。
    /// </remarks>
    public class Robot_Socket_Protocol
    {
        /// <summary>
        /// 为接收方向创建协议解析器，并立即从报文头识别视觉功能码。
        /// </summary>
        /// <param name="_robo">发送该报文的机器人协议族。</param>
        /// <param name="_receive">一帧完整的入站报文字节。</param>
        public Robot_Socket_Protocol(Socket_Robot_Protocols_Enum _robo, byte[] _receive)
        {
            // 解码必须同时知道厂商协议和原始帧；功能码随后从各厂商自己的头部格式提取。
            Socket_Robot = _robo;
            Receice_byte = new List<byte>(_receive);
            Vision_Model = Socket_Get_Vision_Model();
        }

        /// <summary>
        /// 为发送方向创建协议编码器；功能码由调用方明确提供，无需解析原始报文。
        /// </summary>
        /// <param name="_robo">目标机器人协议族。</param>
        /// <param name="_model">要编码的业务功能码。</param>
        public Robot_Socket_Protocol(Socket_Robot_Protocols_Enum _robo, Vision_Model_Enum _model)
        {
            // 发送方向只需协议族与功能码，具体 DTO 在 Socket_Send_Set_Data 中传入。
            Socket_Robot = _robo;
            Vision_Model = _model;
        }



        /// <summary>
        /// 当前实例使用的厂商协议族。
        /// </summary>
        public Socket_Robot_Protocols_Enum Socket_Robot { set; get; } = Socket_Robot_Protocols_Enum.通用;


        /// <summary>
        /// 当前报文的业务功能码，决定应使用哪一组 DTO 和专用转换方法。
        /// </summary>
        public Vision_Model_Enum Vision_Model { set; get; } = Vision_Model_Enum.Vision_Ini_Data;



        /// <summary>
        /// 接收方向的完整原始帧；发送方向构造的实例保持为空。
        /// </summary>
        public List<byte> Receice_byte = new();

        private string? _cachedUtf8Xml;

        /// <summary>
        /// 惰性将接收字节按 UTF-8 解码并缓存，避免识别功能码与反序列化同一帧时重复解码。
        /// </summary>
        private string GetUtf8Text()
            => _cachedUtf8Xml ??= Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(Receice_byte));

        /// <summary>
        /// 按厂商协议从报文头提取 <see cref="Vision_Model_Enum"/>。
        /// </summary>
        /// <returns>已识别的功能码；KUKA XML 无法解析时返回 <see cref="Vision_Model_Enum.Unknown"/>。</returns>
        /// <exception cref="Exception">ABB 帧长度不一致，或文本/二进制头中的功能码不存在时抛出。</exception>
        private Vision_Model_Enum Socket_Get_Vision_Model()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    try
                    {
                        // KUKA 报文根元素通过 Vision_Model 属性携带功能码；用 XmlReader 直接读取根节点属性，避免构造整棵 XElement DOM。
                        using StringReader stringReader = new(GetUtf8Text());
                        using XmlReader reader = XmlReader.Create(stringReader);
                        if (reader.MoveToContent() != XmlNodeType.Element)
                        {
                            return Vision_Model_Enum.Unknown;
                        }

                        string? visionModel = reader.GetAttribute("Vision_Model");
                        if (string.IsNullOrEmpty(visionModel))
                        {
                            return Vision_Model_Enum.Unknown;
                        }

                        return Enum.Parse<Vision_Model_Enum>(visionModel);
                    }
                    catch (Exception)
                    {
                        // XML 不完整、缺少属性或枚举值非法时统一标记 Unknown，由上层决定如何上报。
                        return Vision_Model_Enum.Unknown;
                    }


                case Socket_Robot_Protocols_Enum.ABB:
                    ReadOnlySpan<byte> abbFrame = CollectionsMarshal.AsSpan(Receice_byte);
                    // ABB 前两字节声明整帧长度（含该 2 字节头部），后两字节为功能码 ASCII 长度。
                    if (abbFrame.Length < 4)
                    {
                        Receice_byte.Clear();
                        throw new Exception("通讯协议存在丢包，请检查网络！");
                    }

                    int expectedLength = BinaryPrimitives.ReadUInt16LittleEndian(abbFrame[..2]);
                    if (expectedLength != abbFrame.Length)
                    {
                        Receice_byte.Clear();
                        throw new Exception("通讯协议存在丢包，请检查网络！");
                    }

                    int modeOffset = 2;
                    string modelString = ReadAbbLengthPrefixedAscii(abbFrame, ref modeOffset, "Vision_Model");

                    if (!Enum.IsDefined(typeof(Vision_Model_Enum), modelString))
                    {
                        throw new Exception("通讯协议无该功能码，请联系开发者！");
                    }

                    return Enum.Parse<Vision_Model_Enum>(modelString);


                case Socket_Robot_Protocols_Enum.FANUC or Socket_Robot_Protocols_Enum.川崎:
                    // FANUC 与川崎文本帧统一以“功能码:字段列表;”开头，冒号前即枚举名称。
                    string delimitedModelString = Encoding.ASCII.GetString(Receice_byte.ToArray()).Split(':')[0];

                    if (!Enum.IsDefined(typeof(Vision_Model_Enum), delimitedModelString))
                    {
                        throw new Exception("通讯协议无该功能码，请联系开发者！");
                    }

                    return Enum.Parse<Vision_Model_Enum>(delimitedModelString);


       




                case Socket_Robot_Protocols_Enum.通用:
                    // 通用协议尚未定义可解析的报文头，沿用方法末尾的兼容默认值。
                    break;



            }
            // 兼容旧版“通用”调用：没有厂商头时按最早的标定新增功能处理。
            return Vision_Model_Enum.Calibration_New;

        }






        /// <summary>
        /// 按当前功能码选择专用解码器，将入站报文转换为业务 DTO。
        /// </summary>
        /// <typeparam name="T1">调用方期望的 DTO 类型，必须与 <see cref="Vision_Model"/> 对应。</typeparam>
        /// <returns>解码后的 DTO；尚未实现的旧标定功能返回默认值。</returns>
        /// <exception cref="Exception">功能码未知或不在接收路由表时抛出。</exception>
        /// <remarks>内部转换通过运行时强制类型转换完成，错误的 <typeparamref name="T1"/> 会导致类型转换异常。</remarks>
        public T1? Socket_Receive_Get_Date<T1>()
        {


            switch (Vision_Model)
            {
                case Vision_Model_Enum.Calibration_New:
                    // 兼容占位：旧标定新增协议尚无 DTO 解码实现。
                    break;
                case Vision_Model_Enum.Calibration_Text:
                    // 兼容占位：旧标定测试协议尚无 DTO 解码实现。
                    break;
                case Vision_Model_Enum.Calibration_Add:
                    // 兼容占位：旧标定追加协议尚无 DTO 解码实现。
                    break;
                case Vision_Model_Enum.Find_Model:
                    // 查找请求包含相机、平面和路径位姿。
                    return (T1)(Object)Vision_Find_Receive_Protocol();

                case Vision_Model_Enum.Vision_Ini_Data:

                    return (T1)(Object)Vision_Ini_Receive_Protocol();

                case Vision_Model_Enum.HandEye_Calib_Date:

                    return (T1)(Object)HandEye_Calibration_Receive_Protocol();

                case Vision_Model_Enum.Vision_Creation_Model:

                    return (T1)(Object)Vision_Creation_Model_Receive_Protocol();

                case Vision_Model_Enum.Mes_Info_Data:

                    return (T1)(Object)Mes_Robot_Info_Receive_Protocol();


                case Vision_Model_Enum.Mes_Server_Info_Send_Data:
                    // Client -> Server: Client 上传看板快照，Server 解析为 Mes_Server_Info_Data_Receive。

                    return (T1)(Object)Mes_Server_Info_Receive_Protocol();

                case Vision_Model_Enum.Mes_Server_Info_Rece_Data:

                    // Server -> Client: Server 对看板快照的回执，Client 解析为 Mes_Server_Info_Data_Send。

                    return (T1)(Object)Mes_Server_Info_Send_Protocol();

                default:
                    throw new Exception("现有通讯协议无法解析，请联系开发者！");

            }


            return default;


        }


        /// <summary>
        /// 按当前功能码选择专用编码器，将业务 DTO 转换为目标厂商报文。
        /// </summary>
        /// <typeparam name="T1">发送 DTO 类型，必须与 <see cref="Vision_Model"/> 对应。</typeparam>
        /// <param name="_Propertie">要编码的业务应答或上行数据。</param>
        /// <returns>完整报文字节；尚未实现的旧标定功能返回 <see langword="null"/>。</returns>
        /// <exception cref="Exception">功能码未知或不在发送路由表时抛出。</exception>
        public byte[]? Socket_Send_Set_Data<T1>(T1 _Propertie)
        {

            switch (Vision_Model)
            {
                case Vision_Model_Enum.Calibration_New:
                    // 兼容占位：尚未定义发送帧。
                    break;
                case Vision_Model_Enum.Calibration_Text:
                    // 兼容占位：尚未定义发送帧。
                    break;
                case Vision_Model_Enum.Calibration_Add:
                    // 兼容占位：尚未定义发送帧。
                    break;
                case Vision_Model_Enum.Find_Model:

                    return Vision_Find_Send_Protocol((_Propertie as Vision_Find_Data_Send)!);

                case Vision_Model_Enum.Vision_Ini_Data:

                    return Vision_Ini_Send_Procotol((_Propertie as Vision_Ini_Data_Send)!);

                case Vision_Model_Enum.HandEye_Calib_Date:

                    return HandEye_Calibration_Send_Protocol((_Propertie as HandEye_Calibration_Send)!);


                case Vision_Model_Enum.Vision_Creation_Model:
                    return Vision_Creation_Model_Send_Procotol((_Propertie as Vision_Creation_Model_Send)!);

                case Vision_Model_Enum.Mes_Info_Data:


                    return Mes_Robot_Info_Send_Procotol((_Propertie as Robot_Mes_Info_Data_Send)!);

                case Vision_Model_Enum.Mes_Server_Info_Send_Data:

                    // Server -> Client: Server 回执使用 Mes_Server_Info_Data_Send，保留原枚举名以兼容旧包。
                    return Mes_Server_Info_Send_Procotol((_Propertie as Mes_Server_Info_Data_Send)!);


                case Vision_Model_Enum.Mes_Server_Info_Rece_Data:


                    // Client -> Server: Client 上传 Mes_Server_Info_Data_Receive，保留原枚举名以兼容旧调用。
                    return Mes_Server_Info_Receive_Procotol((_Propertie as Mes_Server_Info_Data_Receive)!);



                default:
                    throw new Exception("现有通讯协议无法解析，请联系开发者！");

            }


            return default;
        }



        /// <summary>
        /// 解码机器人提交的手眼标定状态与当前位姿。
        /// </summary>
        /// <returns>统一的手眼标定请求 DTO。</returns>
        private HandEye_Calibration_Receive HandEye_Calibration_Receive_Protocol()
        {

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 直接把 XML 根元素映射到 DTO。
                    HandEye_Calibration_Receive _Kuka_HandEye_Calib_Rece = KUKA_Send_Receive_Xml.String_Xml<HandEye_Calibration_Receive>(GetUtf8Text());


                    return _Kuka_HandEye_Calib_Rece;


                case Socket_Robot_Protocols_Enum.ABB:
                    HandEye_Calibration_Receive _ABB_HandEye_Calib_Rece = new();

                    // ABB 固定布局：偏移 5 为 4 字节标定阶段，其后依次为 X/Y/Z/Rx/Ry/Rz 六个 Single。
                    int _Calib_Model = BitConverter.ToInt32(Receice_byte.Skip(5).Take(4).ToArray());
                    var xx = Receice_byte.Skip(9).Take(4).ToArray();
                    var yy = Receice_byte.Skip(13).Take(4).ToArray();
                    var zz = Receice_byte.Skip(17).Take(4).ToArray();
                    var Rxx = Receice_byte.Skip(21).Take(4).ToArray();
                    var Ryy = Receice_byte.Skip(25).Take(4).ToArray();
                    var Rzz = Receice_byte.Skip(29).Take(4).ToArray();
                    // BitConverter 沿用 Windows 小端序；机器人端必须使用相同字节序。
                    double x = BitConverter.ToSingle(xx);
                    double y = BitConverter.ToSingle(yy);
                    double z = BitConverter.ToSingle(zz);
                    double Rx = BitConverter.ToSingle(Rxx);
                    double Ry = BitConverter.ToSingle(Ryy);
                    double Rz = BitConverter.ToSingle(Rzz);
                    // 对外模型以字符串保存坐标；统一保留四位精度，兼顾 XML 与二进制协议。
                    _ABB_HandEye_Calib_Rece.Vision_Model = Vision_Model;
                    _ABB_HandEye_Calib_Rece.Calibration_Model = (HandEye_Calibration_Type_Enum)_Calib_Model;
                    _ABB_HandEye_Calib_Rece.ACT_Point.X = Math.Round(x, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Y = Math.Round(y, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Z = Math.Round(z, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Rx = Math.Round(Rx, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Ry = Math.Round(Ry, 4).ToString();
                    _ABB_HandEye_Calib_Rece.ACT_Point.Rz = Math.Round(Rz, 4).ToString();



                    return _ABB_HandEye_Calib_Rece;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎手眼标定二进制/文本布局尚未实现，返回默认 DTO。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用协议没有专用字段映射，返回默认 DTO。
                    break;

            }




            return new HandEye_Calibration_Receive();

        }


        /// <summary>
        /// 编码视觉侧返回给机器人的手眼标定结果。
        /// </summary>
        /// <param name="_Propertie">状态、错误消息和结果位姿。</param>
        /// <returns>目标机器人可读取的完整应答帧；未实现的兼容分支返回空数组。</returns>
        private byte[] HandEye_Calibration_Send_Protocol(HandEye_Calibration_Send _Propertie)
        {
            List<byte> _Send_Byte = new();

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 应答使用紧凑 XML，并在网络层按 UTF-8 发送。
                    _Send_Byte = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<HandEye_Calibration_Send>(_Propertie)));

                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // 先把每个逻辑字段转换为其线上字节表示。
                    var st = BitConverter.GetBytes(_Propertie.IsStatus);
                    var mes = Encoding.UTF8.GetBytes(_Propertie.Message_Error);
                    var mes_num = BitConverter.GetBytes(mes.Length);
                    var xx = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.X)));
                    var yy = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Y)));
                    var zz = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Z)));
                    var Rxx = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Rx)));
                    var Ryy = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Ry)));
                    var Rzz = BitConverter.GetBytes((float.Parse(_Propertie.Result_Pos.Rz)));
                    // ABB 负载顺序：状态(Int32) | 消息长度(Int32) | UTF-8 消息 | 六个 Single 位姿值。
                    _Send_Byte.AddRange(st);
                    _Send_Byte.AddRange(mes_num);
                    _Send_Byte.AddRange(mes);
                    _Send_Byte.AddRange(xx);
                    _Send_Byte.AddRange(yy);
                    _Send_Byte.AddRange(zz);
                    _Send_Byte.AddRange(Rxx);
                    _Send_Byte.AddRange(Ryy);
                    _Send_Byte.AddRange(Rzz);
                    // 最前面再插入 4 字节负载总长；长度值不包含自身。
                    var _num = BitConverter.GetBytes(_Send_Byte.Count);

                    _Send_Byte.InsertRange(0, _num);
                    return _Send_Byte.ToArray();
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎手眼标定应答尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用手眼标定应答尚未实现。
                    break;

                default:
                    throw new Exception("发送协议错误！");

            }








            return _Send_Byte.ToArray();
        }




        /// <summary>
        /// 编码视觉建模完成后的状态、消息和创建点位。
        /// </summary>
        /// <param name="_Propertie">建模处理结果。</param>
        /// <returns>目标机器人协议应答帧；未实现的兼容分支返回空数组。</returns>
        private byte[] Vision_Creation_Model_Send_Procotol(Vision_Creation_Model_Send _Propertie)
        {

            List<byte> _Send_Byte = new();


            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 与其他视觉业务一致，直接序列化紧凑 XML。
                    _Send_Byte = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<Vision_Creation_Model_Send>(_Propertie)));


                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 建模应答的状态是 Bool（1 字节），随后是长度前缀消息与六个 Single。
                    var st = BitConverter.GetBytes(_Propertie.IsStatus);
                    var mes = Encoding.UTF8.GetBytes(_Propertie.Message_Error);
                    var mes_num = BitConverter.GetBytes(mes.Length);
                    var xx = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.X)));
                    var yy = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Y)));
                    var zz = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Z)));
                    var Rxx = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Rx)));
                    var Ryy = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Ry)));
                    var Rzz = BitConverter.GetBytes((float.Parse(_Propertie.Creation_Point.Rz)));
                    // 按协议固定顺序拼接，位置字段仍使用 X/Y/Z/Rx/Ry/Rz。
                    _Send_Byte.AddRange(st);
                    _Send_Byte.AddRange(mes_num);
                    _Send_Byte.AddRange(mes);
                    _Send_Byte.AddRange(xx);
                    _Send_Byte.AddRange(yy);
                    _Send_Byte.AddRange(zz);
                    _Send_Byte.AddRange(Rxx);
                    _Send_Byte.AddRange(Ryy);
                    _Send_Byte.AddRange(Rzz);
                    // 帧首的 Int32 表示后续负载长度，不包含这四个长度字节。
                    var _num = BitConverter.GetBytes(_Send_Byte.Count);

                    _Send_Byte.InsertRange(0, _num);
                    return _Send_Byte.ToArray();
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎建模应答尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用建模应答尚未实现。
                    break;
                default:
                    throw new Exception("发送协议错误！");


            }

            return _Send_Byte.ToArray();
        }

        /// <summary>
        /// 解码机器人发起的视觉建模请求。
        /// </summary>
        /// <returns>包含机器人类型、相机位姿和模型原点位姿的统一 DTO。</returns>
        private Vision_Creation_Model_Receive Vision_Creation_Model_Receive_Protocol()
        {
            Vision_Creation_Model_Receive _Robot_Creation_Model_Rece = new();
            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 请求由 XML 序列化特性直接映射。
                    _Robot_Creation_Model_Rece = KUKA_Send_Receive_Xml.String_Xml<Vision_Creation_Model_Receive>(GetUtf8Text());



                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 固定布局从偏移 4 开始：Robot_Type(Int16) + Camera 六个 Single + Origin 六个 Single。
                    int _Robot_Type = BitConverter.ToInt16(Receice_byte.Skip(4).Take(2).ToArray());

                    if (!Enum.IsDefined((Robot_Type_Enum)_Robot_Type))
                    {
                        throw new Exception("通讯协议无该机器人类型，请联系开发者！");
                    }
                    // 先按固定偏移切片，避免不同字段解码时互相推进游标造成错位。
                    var xx = Receice_byte.Skip(6).Take(4).ToArray();
                    var yy = Receice_byte.Skip(10).Take(4).ToArray();
                    var zz = Receice_byte.Skip(14).Take(4).ToArray();
                    var Rxx = Receice_byte.Skip(18).Take(4).ToArray();
                    var Ryy = Receice_byte.Skip(22).Take(4).ToArray();
                    var Rzz = Receice_byte.Skip(26).Take(4).ToArray();
                    var xxx = Receice_byte.Skip(30).Take(4).ToArray();
                    var yyy = Receice_byte.Skip(34).Take(4).ToArray();
                    var zzz = Receice_byte.Skip(38).Take(4).ToArray();
                    var Rxxx = Receice_byte.Skip(42).Take(4).ToArray();
                    var Ryyy = Receice_byte.Skip(46).Take(4).ToArray();
                    var Rzzz = Receice_byte.Skip(50).Take(4).ToArray();
                    // 前六个浮点数为相机位姿，后六个为模型原点位姿。
                    double cx = BitConverter.ToSingle(xx);
                    double cy = BitConverter.ToSingle(yy);
                    double cz = BitConverter.ToSingle(zz);
                    double cRx = BitConverter.ToSingle(Rxx);
                    double cRy = BitConverter.ToSingle(Ryy);
                    double cRz = BitConverter.ToSingle(Rzz);
                    double ox = BitConverter.ToSingle(xxx);
                    double oy = BitConverter.ToSingle(yyy);
                    double oz = BitConverter.ToSingle(zzz);
                    double oRx = BitConverter.ToSingle(Rxxx);
                    double oRy = BitConverter.ToSingle(Ryyy);
                    double oRz = BitConverter.ToSingle(Rzzz);

                    // DTO 坐标统一使用字符串，并将二进制浮点值四舍五入到四位小数。
                    _Robot_Creation_Model_Rece.Vision_Model = Vision_Model;
                    _Robot_Creation_Model_Rece.Robot_Type = (Robot_Type_Enum)_Robot_Type;
                    _Robot_Creation_Model_Rece.Camera_Pos.X = Math.Round(cx, 4).ToString();
                    _Robot_Creation_Model_Rece.Camera_Pos.Y = Math.Round(cy, 4).ToString();
                    _Robot_Creation_Model_Rece.Camera_Pos.Z = Math.Round(cz, 4).ToString();
                    _Robot_Creation_Model_Rece.Camera_Pos.Rx = Math.Round(cRx, 4).ToString();
                    _Robot_Creation_Model_Rece.Camera_Pos.Ry = Math.Round(cRy, 4).ToString();
                    _Robot_Creation_Model_Rece.Camera_Pos.Rz = Math.Round(cRz, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.X = Math.Round(ox, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.Y = Math.Round(oy, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.Z = Math.Round(oz, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.Rx = Math.Round(oRx, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.Ry = Math.Round(oRy, 4).ToString();
                    _Robot_Creation_Model_Rece.Origin_Pos.Rz = Math.Round(oRz, 4).ToString();


                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎建模请求尚未定义字段布局。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用建模请求返回默认 DTO。
                    break;
                default:
                    throw new Exception("发送协议错误！");
            }


            return _Robot_Creation_Model_Rece;

        }


        /// <summary>
        /// 解码视觉程序初始化请求。
        /// </summary>
        /// <returns>初始化请求 DTO；当前仅 KUKA XML 分支填充数据。</returns>
        private Vision_Ini_Data_Receive Vision_Ini_Receive_Protocol()
        {
            Vision_Ini_Data_Receive _Ini_Data_Receive = new();
            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // 初始化请求的 XML 通常只包含 Vision_Model 属性。
                    _Ini_Data_Receive = KUKA_Send_Receive_Xml.String_Xml<Vision_Ini_Data_Receive>(GetUtf8Text());


                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 初始化请求尚未实现，返回默认 DTO。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎初始化请求尚未实现，返回默认 DTO。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用初始化请求尚未实现；保持现有落入 default 的报错行为。

                default:
                    throw new Exception("发送协议错误！");


            }

            return _Ini_Data_Receive;


        }

        /// <summary>
        /// 解码机器人周期上传的生产/MES 状态。
        /// </summary>
        /// <returns>统一的机器人类型、模式、程序、工艺和 A-D 工位状态 DTO。</returns>
        private Robot_Mes_Info_Data_Receive Mes_Robot_Info_Receive_Protocol()
        {
            Robot_Mes_Info_Data_Receive _Mes_Robot_Data_Receive = new();
            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 使用 XML 属性和元素直接映射全部状态字段。
                    _Mes_Robot_Data_Receive = KUKA_Send_Receive_Xml.String_Xml<Robot_Mes_Info_Data_Receive>(GetUtf8Text());


                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    ParseAbbMesRobotInfo(CollectionsMarshal.AsSpan(Receice_byte), _Mes_Robot_Data_Receive);
                    break;
       
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用 MES 状态没有定义具体正文，返回默认 DTO。
                    break;


                case Socket_Robot_Protocols_Enum.FANUC or Socket_Robot_Protocols_Enum.川崎:
                    // 文本布局：Vision_Model:Robot,Mode,Program,Process,A,B,C,D;。
                    _Mes_Robot_Data_Receive.Vision_Model = Enum.Parse<Vision_Model_Enum>(Encoding.ASCII.GetString(Receice_byte.ToArray()).Split(':')[0]);


                    string Receice_byte_Start = Encoding.ASCII.GetString(Receice_byte.ToArray()).Split(':')[1];
                    string Receice_byte_Date = Receice_byte_Start.Split(';')[0];


                    List<string> Receice_byte_List = new List<string>(Receice_byte_Date.Split(','));

                    // 后续映射固定依赖八个字段；先校验数量再按索引转换。
                    if (Receice_byte_List.Count<8)
                    { throw new Exception("协议内容缺失！");}

                    _Mes_Robot_Data_Receive.Robot_Type = Enum.Parse<Robot_Type_Enum>( Receice_byte_List[0]);
                    _Mes_Robot_Data_Receive.Mes_Robot_Mode = Enum.Parse<KUKA_Mode_OP_Enum>(Receice_byte_List[1]);
                    _Mes_Robot_Data_Receive.Mes_Programs_Name = Receice_byte_List[2];
                    _Mes_Robot_Data_Receive.Robot_Process_Int = Enum.Parse<Robot_Process_Int_Enum>(Receice_byte_List[3]);
                    _Mes_Robot_Data_Receive.Mes_Work_A_State = bool.Parse(Receice_byte_List[4]);
                    _Mes_Robot_Data_Receive.Mes_Work_B_State = bool.Parse(Receice_byte_List[5]);
                    _Mes_Robot_Data_Receive.Mes_Work_C_State = bool.Parse(Receice_byte_List[6]);
                    _Mes_Robot_Data_Receive.Mes_Work_D_State = bool.Parse(Receice_byte_List[7]);
                    break;

                default:
                    throw new Exception("发送协议错误！");


            }

            return _Mes_Robot_Data_Receive;


        }

        /// <summary>
        /// 从 ABB 长度前缀帧的指定偏移处读取一段 UInt16 长度前缀的 ASCII 文本并推进游标。
        /// </summary>
        private static string ReadAbbLengthPrefixedAscii(ReadOnlySpan<byte> frame, ref int offset, string fieldName)
        {
            if (offset + 2 > frame.Length)
            {
                throw new InvalidDataException($"ABB 报文缺少字段 {fieldName} 的长度头。");
            }

            int length = BinaryPrimitives.ReadUInt16LittleEndian(frame.Slice(offset, 2));
            offset += 2;
            if (offset + length > frame.Length)
            {
                throw new InvalidDataException($"ABB 报文字段 {fieldName} 长度越界（声明 {length} 字节）。");
            }

            string value = Encoding.ASCII.GetString(frame.Slice(offset, length));
            offset += length;
            return value;
        }

        /// <summary>
        /// 解码 ABB 生产/MES 二进制报文：前 9 段兼容旧版 ABB 协议，末尾可选追加焊接开关与六轴 Float4 角度。
        /// </summary>
        private static void ParseAbbMesRobotInfo(ReadOnlySpan<byte> frame, Robot_Mes_Info_Data_Receive target)
        {
            const int AxisPayloadBytes = 6 * sizeof(float);
            int offset = 2;

            target.Vision_Model = Enum.Parse<Vision_Model_Enum>(ReadAbbLengthPrefixedAscii(frame, ref offset, "Vision_Model"));
            target.Robot_Type = Enum.Parse<Robot_Type_Enum>(ReadAbbLengthPrefixedAscii(frame, ref offset, "Robot_Type"));
            target.Mes_Robot_Mode = Enum.Parse<KUKA_Mode_OP_Enum>(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Robot_Mode"));
            target.Mes_Programs_Name = ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Programs_Name");
            target.Mes_Work_A_State = bool.Parse(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Work_A_State"));
            target.Mes_Work_B_State = bool.Parse(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Work_B_State"));
            target.Mes_Work_C_State = bool.Parse(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Work_C_State"));
            target.Mes_Work_D_State = bool.Parse(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Work_D_State"));
            target.Robot_Process_Int = Enum.Parse<Robot_Process_Int_Enum>(ReadAbbLengthPrefixedAscii(frame, ref offset, "Robot_Process_Int"));

            // 兼容旧版 9 字段 ABB 报文：无扩展尾部时焊接状态为 false、轴组保持 null。
            if (offset == frame.Length)
            {
                return;
            }

            // 扩展字段 10：焊接/激光开关状态（与工位状态一致的长度前缀 TRUE/FALSE）。
            target.Mes_Weld_State = bool.Parse(ReadAbbLengthPrefixedAscii(frame, ref offset, "Mes_Weld_State"));

            int remaining = frame.Length - offset;
            if (remaining == 0)
            {
                return;
            }

            // 扩展字段 11：J1～J6 小端序单精度浮点角度（6 × 4 = 24 字节）。
            if (remaining == AxisPayloadBytes)
            {
                target.Robot_Axis = new Robot_Axis_Model
                {
                    J1 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset, 4)),
                    J2 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset + 4, 4)),
                    J3 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset + 8, 4)),
                    J4 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset + 12, 4)),
                    J5 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset + 16, 4)),
                    J6 = BinaryPrimitives.ReadSingleLittleEndian(frame.Slice(offset + 20, 4)),
                };
                return;
            }

            if (remaining < AxisPayloadBytes)
            {
                // 轴组字节不足 6 轴时保留缺轴对象，由上层按缺轴帧拒绝并维持最近有效姿态。
                target.Robot_Axis = new Robot_Axis_Model();
                return;
            }

            throw new InvalidDataException($"ABB MES 报文包含未识别的尾部数据（剩余 {remaining} 字节）。");
        }

        /// <summary>
        /// 解码客户端上传给看板服务器的完整快照。
        /// </summary>
        /// <returns>机器人状态与看板统计数据组成的上传 DTO。</returns>
        private Mes_Server_Info_Data_Receive Mes_Server_Info_Receive_Protocol()
        {
            Mes_Server_Info_Data_Receive _Mes_Robot_Data_Receive = new();
            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // 上位机看板链路复用 KUKA XML 序列化格式，与实际机器人品牌无关。
                    _Mes_Robot_Data_Receive = KUKA_Send_Receive_Xml.String_Xml<Mes_Server_Info_Data_Receive>(GetUtf8Text());


                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // 尚未定义 ABB 二进制看板快照。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 尚未定义川崎文本看板快照。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用分支尚未实现，并沿用落入 default 的异常行为。
                default:
                    throw new Exception("发送协议错误！");


            }

            return _Mes_Robot_Data_Receive;


        }
        /// <summary>
        /// 解码看板服务器返回给客户端的确认回执。
        /// </summary>
        /// <returns>包含服务器时间和回执功能码的 DTO。</returns>
        private Mes_Server_Info_Data_Send Mes_Server_Info_Send_Protocol()
        {
            Mes_Server_Info_Data_Send _Mes_Robot_Data_Receive = new();
            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // 看板回执当前使用紧凑 XML。
                    _Mes_Robot_Data_Receive = KUKA_Send_Receive_Xml.String_Xml<Mes_Server_Info_Data_Send>(GetUtf8Text());


                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 看板回执格式尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎看板回执格式尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用分支尚未实现，并沿用落入 default 的异常行为。
                default:
                    throw new Exception("发送协议错误！");


            }

            return _Mes_Robot_Data_Receive;


        }
        /// <summary>
        /// 编码视觉初始化结果。
        /// </summary>
        /// <param name="_Propertie">状态、错误消息以及初始化参数。</param>
        /// <returns>目标协议的应答帧；当前仅 KUKA 分支生成内容。</returns>
        private byte[]? Vision_Ini_Send_Procotol(Vision_Ini_Data_Send _Propertie)
        {

            List<byte> _byte_List = new();

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 初始化结果按 DTO 的 XML 特性序列化。
                    _byte_List = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<Vision_Ini_Data_Send>(_Propertie)));

                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 初始化应答尚未实现，返回空数组。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎初始化应答尚未实现，返回空数组。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用初始化应答尚未实现，返回空数组。
                    break;
                default:

                    throw new Exception("标定发送协议错误！");


            }

            return _byte_List.ToArray();
        }
        /// <summary>
        /// 编码视觉查找结果点位。
        /// </summary>
        /// <param name="_Propertie">识别状态、错误消息和最多八个结果位姿。</param>
        /// <returns>目标协议的查找应答帧；当前仅 KUKA 分支生成内容。</returns>
        private byte[] Vision_Find_Send_Protocol(Vision_Find_Data_Send _Propertie)
        {

            List<byte> _byte_List = new();

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // Point_List_Model 会按固定 Pos_1 到 Pos_8 节点顺序写入 XML。
                    _byte_List = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<Vision_Find_Data_Send>(_Propertie)));

                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 查找结果二进制布局尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎查找结果文本布局尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用查找应答尚未实现。
                    break;
                default:

                    throw new Exception("标定发送协议错误！");
            }

            return _byte_List.ToArray();
        }



        /// <summary>
        /// 编码服务器对机器人生产状态上报的确认及下一轮询周期。
        /// </summary>
        /// <param name="_Propertie">接受状态和毫秒轮询周期。</param>
        /// <returns>KUKA XML，或 ABB/FANUC/川崎共用的 8 字节二进制应答。</returns>
        /// <exception cref="Exception">机器人协议不在支持范围时抛出。</exception>
        private byte[]? Mes_Robot_Info_Send_Procotol(Robot_Mes_Info_Data_Send _Propertie)
        {

            List<byte> _byte_List = new();

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 返回 Robot_Send XML。
                    _byte_List = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<Robot_Mes_Info_Data_Send>(_Propertie)));

                    break;
                case Socket_Robot_Protocols_Enum.ABB or Socket_Robot_Protocols_Enum.FANUC or Socket_Robot_Protocols_Enum.川崎:
                    // 三类控制器在此应答方向共用固定 8 字节小端序：状态 Int32 + 轮询秒数 Int32。
                    byte[] ack = new byte[8];
                    BinaryPrimitives.WriteInt32LittleEndian(ack.AsSpan(0, 4), _Propertie.IsStatus ? 1 : 0);
                    // DTO 使用毫秒，线上协议使用整秒，整数除法会舍弃不足一秒部分。
                    BinaryPrimitives.WriteInt32LittleEndian(ack.AsSpan(4, 4), _Propertie.Socket_Polling_Time / 1000);
                    return ack;




             



          
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用生产状态应答尚未定义。
                    break;
                default:

                    throw new Exception("标定发送协议错误！");


            }

            return _byte_List.ToArray();
        }


        /// <summary>
        /// 编码看板服务器对客户端上传快照的确认回执。
        /// </summary>
        /// <param name="_Propertie">包含服务器处理时间和回执功能码的数据。</param>
        /// <returns>当前协议的回执帧；只有 KUKA/XML 分支已实现。</returns>
        /// <exception cref="Exception">机器人协议不在支持范围时抛出。</exception>
        private byte[]? Mes_Server_Info_Send_Procotol(Mes_Server_Info_Data_Send _Propertie)
        {

            List<byte> _byte_List = new();

            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // 上位机间通信沿用无声明、无命名空间的 XML 格式。
                    _byte_List = new List<byte>(Encoding.UTF8.GetBytes(new  KUKA_Send_Receive_Xml().Property_Xml<Mes_Server_Info_Data_Send>(_Propertie)));

                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 专用看板回执尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎专用看板回执尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用专用回执尚未实现。
                    break;
                default:

                    throw new Exception("标定发送协议错误！");


            }

            return _byte_List.ToArray();
        }


        /// <summary>
        /// 编码看板客户端周期上传给服务器的完整快照。
        /// </summary>
        /// <param name="_Propertie">机器人实时状态与看板统计数据。</param>
        /// <returns>UTF-8 XML 上传帧。</returns>
        /// <remarks>这是上位机到上位机的通用链路，故意不按 <see cref="Socket_Robot"/> 分支。</remarks>
        private byte[]? Mes_Server_Info_Receive_Procotol(Mes_Server_Info_Data_Receive _Propertie)
        {

            List<byte> _byte_List = new();
            // 上位机之间统一使用 XML，避免机器人厂商协议影响看板数据交换。
            _byte_List = new List<byte>(Encoding.UTF8.GetBytes(new KUKA_Send_Receive_Xml().Property_Xml<Mes_Server_Info_Data_Receive>(_Propertie)));

            return _byte_List.ToArray();
        }






        /// <summary>
        /// 解码机器人发起的视觉查找请求。
        /// </summary>
        /// <returns>相机位姿、平面位姿、预设路径、机器人类型及模型 ID。</returns>
        private Vision_Find_Data_Receive Vision_Find_Receive_Protocol()
        {
            Vision_Find_Data_Receive _Find_Data_Receive = new();


            switch (Socket_Robot)
            {
                case Socket_Robot_Protocols_Enum.KUKA:
                    // KUKA 查找请求使用 Robot_Receive XML。
                    _Find_Data_Receive = KUKA_Send_Receive_Xml.String_Xml<Vision_Find_Data_Receive>(GetUtf8Text());

                    break;
                case Socket_Robot_Protocols_Enum.ABB:
                    // ABB 查找请求二进制布局尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.川崎:
                    // 川崎查找请求文本布局尚未实现。
                    break;
                case Socket_Robot_Protocols_Enum.通用:
                    // 通用查找请求返回默认 DTO。
                    break;
                default:

                    throw new Exception("标定发送协议错误！");
            }


            return _Find_Data_Receive;

        }

        /// <summary>
        /// 将监视窗口原始字节统一按协议格式解码为可读文本（不区分收发方向）；ABB 二进制帧统一按小端序解码，避免 Float4 字节产生 UTF-8 乱码。
        /// </summary>
        /// <param name="protocol">当前监听器使用的机器人协议族。</param>
        /// <param name="rawData">完整原始报文字节。</param>
        /// <returns>适合在只读监视面板显示的明文或十六进制文本。</returns>
        public static string FormatMonitorData(Socket_Robot_Protocols_Enum protocol, byte[]? rawData)
        {
            if (rawData is null || rawData.Length == 0)
            {
                return string.Empty;
            }

            if (rawData.Length == 8
                && protocol is Socket_Robot_Protocols_Enum.ABB
                    or Socket_Robot_Protocols_Enum.FANUC
                    or Socket_Robot_Protocols_Enum.川崎)
            {
                int status = BinaryPrimitives.ReadInt32LittleEndian(rawData.AsSpan(0, 4));
                int pollingSeconds = BinaryPrimitives.ReadInt32LittleEndian(rawData.AsSpan(4, 4));
                return string.Create(
                    CultureInfo.InvariantCulture,
                    $"IsStatus: {status}\r\nSocket_Polling_Time: {pollingSeconds}");
            }

            if (protocol != Socket_Robot_Protocols_Enum.ABB)
            {
                return Encoding.UTF8.GetString(rawData);
            }

            try
            {
                var parsed = new Robot_Socket_Protocol(Socket_Robot_Protocols_Enum.ABB, rawData);
                if (parsed.Vision_Model == Vision_Model_Enum.Mes_Info_Data
                    && parsed.Socket_Receive_Get_Date<Robot_Mes_Info_Data_Receive>() is { } dto)
                {
                    var sb = new StringBuilder(256);
                    sb.Append("Vision_Model: ").Append(dto.Vision_Model).Append("\r\n");
                    sb.Append("Robot_Type: ").Append(dto.Robot_Type).Append("\r\n");
                    sb.Append("Mes_Robot_Mode: ").Append(dto.Mes_Robot_Mode).Append("\r\n");
                    sb.Append("Mes_Programs_Name: ").Append(dto.Mes_Programs_Name).Append("\r\n");
                    sb.Append("Mes_Work_A_State: ").Append(dto.Mes_Work_A_State).Append("\r\n");
                    sb.Append("Mes_Work_B_State: ").Append(dto.Mes_Work_B_State).Append("\r\n");
                    sb.Append("Mes_Work_C_State: ").Append(dto.Mes_Work_C_State).Append("\r\n");
                    sb.Append("Mes_Work_D_State: ").Append(dto.Mes_Work_D_State).Append("\r\n");
                    sb.Append("Robot_Process_Int: ").Append(dto.Robot_Process_Int).Append("\r\n");
                    sb.Append("Mes_Weld_State: ").Append(dto.Mes_Weld_State);

                    if (dto.Robot_Axis is { } axes)
                    {
                        if (axes.HasCompleteFiniteAxes())
                        {
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J1: {axes.J1:F2}"));
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J2: {axes.J2:F2}"));
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J3: {axes.J3:F2}"));
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J4: {axes.J4:F2}"));
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J5: {axes.J5:F2}"));
                            sb.Append("\r\n").Append(string.Create(CultureInfo.InvariantCulture, $"J6: {axes.J6:F2}"));
                        }
                        else
                        {
                            sb.Append("\r\nRobot_Axis: Invalid");
                        }
                    }

                    return sb.ToString();
                }
            }
            catch
            {
                // 半包或非 MES 二进制帧统一退回纯十六进制视图，避免输出替换字符乱码。
            }

            return BitConverter.ToString(rawData).Replace('-', ' ');
        }







    }




}
