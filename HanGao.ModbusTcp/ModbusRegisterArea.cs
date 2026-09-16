namespace HanGao.ModbusTcp;

/// <summary>
/// Modbus 中用于向 SCADA 发布数据的寄存器区域。
/// </summary>
/// <remarks>
/// 保持寄存器通常以 40001 开始显示并使用功能码 03；输入寄存器通常以 30001 开始显示并使用功能码 04。
/// Modbus 报文中不传输 40001/30001 这类参考编号，而是传输从零开始的协议地址。
/// </remarks>
public enum ModbusRegisterArea
{
    /// <summary>保持寄存器，SCADA 通常显示为 4xxxx，使用功能码 03 读取。</summary>
    HoldingRegisters,

    /// <summary>输入寄存器，SCADA 通常显示为 3xxxx，使用功能码 04 读取。</summary>
    InputRegisters,
}

/// <summary>
/// SCADA 参考地址与 Modbus 报文零基地址之间的转换工具。
/// </summary>
public static class ModbusAddressConverter
{
    /// <summary>第一个保持寄存器的常用参考地址。</summary>
    public const int HoldingRegisterReferenceBase = 40001;

    /// <summary>第一个输入寄存器的常用参考地址。</summary>
    public const int InputRegisterReferenceBase = 30001;

    /// <summary>
    /// 把 SCADA 中显示的 40001/30001 参考地址转换为报文中的零基地址。
    /// </summary>
    /// <param name="referenceAddress">SCADA 参考地址，例如 40001。</param>
    /// <param name="area">寄存器区域。</param>
    /// <returns>Modbus PDU 中使用的 0～65535 地址。</returns>
    public static ushort ToProtocolAddress(int referenceAddress, ModbusRegisterArea area)
    {
        int protocolAddress = referenceAddress - GetReferenceBase(area);
        if (protocolAddress is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(referenceAddress),
                referenceAddress,
                "参考地址无法转换为 0～65535 范围内的 Modbus 协议地址。");
        }

        return (ushort)protocolAddress;
    }

    /// <summary>
    /// 把报文中的零基地址转换为便于 SCADA 工程人员查看的参考地址。
    /// </summary>
    /// <param name="protocolAddress">Modbus PDU 中的零基地址。</param>
    /// <param name="area">寄存器区域。</param>
    /// <returns>保持寄存器返回 40001 起始地址，输入寄存器返回 30001 起始地址。</returns>
    public static int ToReferenceAddress(ushort protocolAddress, ModbusRegisterArea area) =>
        GetReferenceBase(area) + protocolAddress;

    /// <summary>取得指定寄存器区域的常用参考地址基数。</summary>
    private static int GetReferenceBase(ModbusRegisterArea area) => area switch
    {
        ModbusRegisterArea.HoldingRegisters => HoldingRegisterReferenceBase,
        ModbusRegisterArea.InputRegisters => InputRegisterReferenceBase,
        _ => throw new ArgumentOutOfRangeException(nameof(area), area, "未知的 Modbus 寄存器区域。"),
    };
}
