# HanGao.ModbusTcp

面向 HanGao 项目的独立 Modbus TCP 服务端库，不依赖 WPF，也不直接依赖 `Robot_Info_Mes` 的 ViewModel 或数据模型。

## 设计原则

- `Robot_Info_Mes` 是 Modbus TCP 服务端，只接受 `400001～465536` 六位保持寄存器参考地址；SCADA 客户端使用 FC03 读取，所有写功能码均拒绝。
- 服务端固定提供 FC03 保持寄存器，不暴露寄存器区域选择；FC04 和写功能码均拒绝。
- 每个 16 位寄存器在线路上固定使用 Modbus 大端字节序；`WordOrder` 只控制 UInt32 两个寄存器之间的高低字顺序。
- 业务层先生成不可变快照，服务端在协议读写共用锁内整体复制，防止 SCADA 客户端一次读取出现新旧数据混合。
- 启动、停止和释放串行执行；相同参数重复启动、重复停止均安全。
- 诊断历史有容量上限，可供 WPF 显示连接数、请求数、拒绝数和最后错误。
- 所有六位参考地址与报文零基地址的换算集中在转换器中，避免业务代码重复换算。

## 地址约定

本项目配置和 UI 只使用六位参考地址；参考地址不会原样出现在 Modbus 报文中：

```text
SCADA 400001 <=> Modbus PDU 地址 0 <=> FC03 保持寄存器
SCADA 400002 <=> Modbus PDU 地址 1
```

配置文件中的块起点必须位于 `400001～465536`。如果 SCADA 驱动的地址栏要求填写 PDU 地址，则首地址填写 `0`；不要在应用配置中填写五位地址。

默认 `HighWordFirst` 对应常见的 UInt32 `ABCD` 排列。例如十六进制值
`0x11223344` 在线路上依次发送 `11 22 33 44`。如果选择 `LowWordFirst`，则只交换两个
16 位寄存器，线路字节为 `33 44 11 22`；单个寄存器内部始终保持高字节在前。

## Attribute 自动布局与本地 XML

`Robot_Info_Mes` 当前使用 `ModbusPointAttribute` 声明点位唯一 `Order`、UInt16/UInt32、单位和说明。运行时只配置寄存器块起点，`ModbusRegisterLayoutBuilder` 负责计算所有后续地址：

- UInt16 占 1 个 16 位寄存器；
- UInt32 占 2 个连续寄存器；
- 所有字段按 Order 紧密连续排列，不插入对齐保留字；
- Order 同时是发布值的唯一标识，XML 不再保存容易误改的字符串 Key；
- 地址、类型、顺序或范围不合法时拒绝启动，不会带着重叠布局继续通信；
- `400001` 自动转换为 PDU 地址 0，`400003` 自动转换为 PDU 地址 2。

默认八个点位的自动结果为：

| 地址 | 类型 | 字段 |
|---:|---|---|
| 400001～400002 | UInt32 | 当天运行时间 |
| 400003～400004 | UInt32 | 累计运行时间 |
| 400005～400006 | UInt32 | 停机时间 |
| 400007～400008 | UInt32 | 单次停机时间 |
| 400009 | UInt16 | 状态信息 |
| 400010～400011 | UInt32 | 当天产量 |
| 400012～400013 | UInt32 | 累计产量 |
| 400014～400015 | UInt32 | 上电时间 |

应用配置保存在 `C:\ProgramData\HanGao\Robot_Info_Mes\Configs\ModbusTcp_Config.Xml`，与 `Configs_Data.Xml` 同目录。UI 修改后会先校验并原子保存 XML，再单独重启 Modbus TCP 服务端。正在运行的服务端使用启动时冻结的布局，编辑中的数据不会提前改变线上寄存器含义。

旧版配置在读取后会自动升级到 SchemaVersion 2 / MapVersion 2，并按 Order 保留现有点位设置；再次保存时会清理不再使用的旧字段。

当前 `Robot_Info_Mes` 直接发布强类型寄存器数据属性的初始化值，尚未与 OEE 业务统计对接。正式接入时可先把属性初始化值改为 0，再替换为真实业务赋值。

`UInt16` 最大值为 65535：按秒只能保存约 18.2 小时，按分钟只能保存约 45.5 天，因此不能保存一年的累计时间。累计秒、累计分钟和累计产量统一使用两个 UInt16 组成的 UInt32；UInt32 秒约可覆盖 136 年。

## 最小使用示例

```csharp
await using IModbusTcpServer server = new FluentModbusTcpServer();

await server.StartAsync(new ModbusTcpServerOptions
{
    BindAddress = IPAddress.Any,
    Port = 502,
    AcceptedUnitIdentifier = 1,
    RegisterCount = 64,
    MaxConnections = 64,
});

ModbusPointDefinition[] definitions =
[
    new(0, ModbusPointDataType.UInt32, "当天运行时间", "分钟", "示例数据"),
    new(1, ModbusPointDataType.UInt16, "状态信息", "", "0运行、1故障、2待机"),
];
ModbusRegisterLayout layout = ModbusRegisterLayoutBuilder.Build(400001, definitions);
var values = new Dictionary<int, ulong>
{
    [0] = 480,
    [1] = 0,
};

server.Publish(ModbusRegisterLayoutEncoder.CreateSnapshot(layout, values, sequence: 1));
```

## UI 显示和调试

WPF 层建议每 500～1000 ms 调用一次 `GetStatus()`，显示：

- 服务状态、监听 IP/端口；
- 当前连接数、累计请求数、拒绝请求数；
- 最近一次请求的 Unit ID、功能码、地址和数量；
- 最近发布流水号、采集时间、发布时间；
- 最近生命周期错误。

`DiagnosticEmitted` 可用于实时日志。事件在产生诊断的调用线程触发，WPF 更新控件时必须通过 Dispatcher 切回 UI 线程，事件处理本身应保持简短。

## 现有数据持久化结论

`Robot_Info_Mes` 当前使用 `TimeSpan`/`int` 写入 ProgramData 下的 XML，默认每 30 秒保存，并通过临时文件替换目标文件：

- 保存累计时间的范围足够，不应把持久化源数据降为 UInt16；
- 允许异常断电时最多丢失一个保存周期数据的场景，可继续使用；
- 当前缺少独立的跨日累计产量字段，不能仅用会在日切时清零的当日产量代替；
- 建议增加 `UInt64/long TotalProduction`、完整日期日切判断、退出时保存和 `.bak` 恢复；
- 若要求每件产品都不可丢失并且需要审计，应改为 SQLite WAL 事务记录，XML 仅保留配置和月度展示快照。

Modbus 寄存器只是当前业务状态的通信投影，不应成为累计数据的唯一持久化来源。

## 压力测试

快速验证：

```powershell
dotnet run --project HanGao.ModbusTcp.Tests -c Release -- --quick
```

持续高并发验证：

```powershell
dotnet run --project HanGao.ModbusTcp.Tests -c Release -- --clients 128 --reads 10000 --publishers 8 --publishes 100000 --registers 64 --timeout-seconds 300
```

测试程序使用异步 TCP 客户端和统一起跑信号；发现快照撕裂、元数据不一致、FC03/写保护错误、超时或客户端异常时返回非零退出码。
