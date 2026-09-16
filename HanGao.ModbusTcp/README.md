# HanGao.ModbusTcp

面向 HanGao 项目的独立 Modbus TCP 服务库，不依赖 WPF，也不直接依赖 `Robot_Info_Mes` 的 ViewModel 或数据模型。

## 设计原则

- 默认把 `40001` 作为只读保持寄存器块，SCADA 使用 FC03 读取；所有写功能码均拒绝。
- 也可通过 `RegisterArea` 切换为 `30001`/FC04 输入寄存器模式。
- 业务层先生成不可变快照，服务在协议读写共用锁内整体复制，防止一次读取出现新旧数据混合。
- 启动、停止和释放串行执行；相同参数重复启动、重复停止均安全。
- 诊断历史有容量上限，可供 WPF 显示连接数、请求数、拒绝数和最后错误。
- 所有 SCADA 参考地址与报文零基地址的换算集中在 `ModbusAddressConverter`，避免在业务代码中散落 `40001 - 1`。

## 地址约定

SCADA 常用的 `40001` 是参考地址，不会原样出现在 Modbus 报文中：

```text
SCADA 40001  <=>  Modbus PDU 地址 0  <=>  FC03 保持寄存器
SCADA 40002  <=>  Modbus PDU 地址 1
```

部分 SCADA 驱动要求填写 `40001`，部分驱动要求填写 `0` 或 `1`。联调时必须确认驱动是否启用了“零基地址”。

## Robot_Info_Mes 固定寄存器表（版本 1）

SCADA 建议一次读取 `40001～40018` 共 18 个寄存器。

| SCADA 地址 | PDU 地址 | 字段 | 逻辑类型 | 单位 |
|---|---:|---|---|---|
| 40001 | 0 | 当天运行时间 | UInt16 | 分钟 |
| 40002～40003 | 1～2 | 累计运行时间 | UInt32，高字在前 | 秒 |
| 40004～40005 | 3～4 | 累计停机时间 | UInt32，高字在前 | 分钟 |
| 40006～40007 | 5～6 | 当前单次停机时间 | UInt32，高字在前 | 分钟 |
| 40008 | 7 | 状态信息 | UInt16 | 0 未知、1 离线、2 待机、3 运行、4 手动、5 故障 |
| 40009 | 8 | 当天产量 | UInt16 | 件 |
| 40010～40011 | 9～10 | 累计产量 | UInt32，高字在前 | 件 |
| 40012～40013 | 11～12 | 本次上电时间 | UInt32，高字在前 | 分钟 |
| 40014～40015 | 13～14 | 数据更新时间 | UInt32，高字在前 | UTC Unix 秒 |
| 40016 | 15 | 寄存器表版本 | UInt16 | 当前为 1 |
| 40017～40018 | 16～17 | 快照流水号低 32 位 | UInt32，高字在前 | 次 |

`UInt16` 最大值为 65535：按秒只能保存约 18.2 小时，按分钟只能保存约 45.5 天，因此不能保存一年的累计时间。累计秒、累计分钟和累计产量统一使用两个 UInt16 组成的 UInt32；UInt32 秒约可覆盖 136 年。

## 最小使用示例

```csharp
await using IModbusTcpServer server = new FluentModbusTcpServer();

await server.StartAsync(new ModbusTcpServerOptions
{
    BindAddress = IPAddress.Any,
    Port = 502,
    AcceptedUnitIdentifier = 1,
    RegisterArea = ModbusRegisterArea.HoldingRegisters,
    RegisterCount = 64,
    MaxConnections = 64,
});

var values = new RobotInfoRegisterValues(
    DailyRunMinutes: 480,
    TotalRunSeconds: 31_536_000,
    DowntimeMinutes: 30,
    CurrentDowntimeMinutes: 0,
    StatusCode: RobotInfoStatusCode.Running,
    DailyProduction: 350,
    TotalProduction: 127_750,
    PowerOnMinutes: 525_600,
    UpdatedAtUtc: DateTimeOffset.UtcNow);

server.Publish(RobotInfoRegisterMap.CreateSnapshot(values, sequence: 1));
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
