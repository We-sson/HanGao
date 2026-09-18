# Robot_Info_Mes 部署 Modbus TCP 示例

## 通信角色

在本方案中：

- `Robot_Info_Mes`：Modbus TCP 服务端（Server），负责监听端口并提供保持寄存器数据；
- SCADA：Modbus TCP 客户端（Client），负责主动建立连接并按周期发送 FC03 读取请求；
- `Robot_Info_Mes` 不主动连接 SCADA，SCADA 客户端也不通过 Modbus 写入业务数据。

## 当前应用启动方式

设备采集界面启动时会读取独立的 `ModbusTcp_Config.Xml`。配置启用时，主界面消息循环就绪后自动启动 Modbus TCP 服务端；窗口退出时释放服务端监听器。

配置文件位置：

```text
C:\ProgramData\HanGao\Robot_Info_Mes\Configs\Configs_Data.Xml
C:\ProgramData\HanGao\Robot_Info_Mes\Configs\ModbusTcp_Config.Xml
```

主界面“参数配置 → Modbus服务端”可以编辑监听 IP、端口、Unit ID、块起点、刷新周期、字序、连接数和点位表。底部“开启服务端/关闭服务端”同时决定下次是否自动启动；点击“保存并重启服务端”后，不需要重启整个软件。

## WPF 窗口生命周期示例

```csharp
private bool _modbusStopped;
private bool _closeInProgress;

public Client_Window()
{
    InitializeComponent();

    Closing += async (_, eventArgs) =>
    {
        if (_modbusStopped)
        {
            return;
        }

        // WPF 不会等待普通 async Closed 处理器。第一次 Closing 先取消关闭，
        // 等端口和连接安全停止后再次 Close，第二次才真正退出。
        eventArgs.Cancel = true;
        if (_closeInProgress)
        {
            return;
        }

        _closeInProgress = true;
        try
        {
            if (DataContext is Robot_Info_VM viewModel)
            {
                await viewModel.DisposeModbusServerAsync();
            }
        }
        finally
        {
            _modbusStopped = true;
            _closeInProgress = false;
            Close();
        }
    };
}
```

不要在窗口构造函数中使用 `.Wait()` 或 `.Result`，否则可能阻塞 WPF UI 线程。

## SCADA 客户端连接参数

| 参数 | 示例值 |
|---|---|
| 协议 | Modbus TCP |
| 设备 IP | Robot_Info_Mes 工控机的固定 IPv4 地址 |
| TCP 端口 | 502 |
| Unit ID | 1 |
| 功能码 | FC03 Read Holding Registers |
| 起始地址 | 默认 400001；程序只接受六位参考地址，若 SCADA 驱动要求 PDU 地址则填写 0 |
| 长度 | 默认 15 个寄存器，字段按 Order 连续占位，无对齐保留字 |
| 16 位字节序 | Big Endian，每个寄存器高字节在前 |
| 32 位字序 | High Word First / ABCD |
| SCADA 轮询周期 | 建议 1000 ms，通常不低于 200 ms |
| 超时 | 建议 3 s |
| 重试 | 建议 2～3 次 |

SCADA 客户端应尽量保持 TCP 长连接，并用一次请求读取 400001～400015；没有必要每次轮询都重新建立连接。Robot_Info_Mes 的 Modbus TCP 服务端会原子发布寄存器快照，一次读取整个块可以获得同一版本的数据。

SCADA 或调试软件中的默认点位表应配置为：

| 地址 | 格式 | 字段 |
|---:|---|---|
| 400001 | UDec32 / ABCD | 当天运行时间 |
| 400003 | UDec32 / ABCD | 累计运行时间 |
| 400005 | UDec32 / ABCD | 停机时间 |
| 400007 | UDec32 / ABCD | 单次停机时间 |
| 400009 | UDec16 | 状态信息 |
| 400010 | UDec32 / ABCD | 当天产量 |
| 400012 | UDec32 / ABCD | 累计产量 |
| 400014 | UDec32 / ABCD | 上电时间 |

注意：`400009` 只占一个寄存器；当天产量从 `400010` 开始并占两个寄存器。整块读取数量必须为 15，不能沿用旧配置中的 14。

## 累计产量说明

当前阶段所有 Modbus 点位直接使用 `RobotInfoModbusRegisterData` 的属性初始化值，尚未读取 `Robot_Work_ABCD_Number` 或其他 OEE 业务字段。累计产量业务字段仍然缺失，正式对接时不能通过求和月度图表猜造累计值。

建议后续在业务模型增加并持久化 `long Robot_Work_All_Number`：

1. 每次产品完成时，与 `Robot_Work_ABCD_Number` 在同一个业务事件中递增；
2. 日切只清空当日产量，不清空累计产量；
3. XML 保存及退出保存都包含累计产量；
4. 发布到 Modbus 前检查不超过 UInt32 最大值，超出时报警并升级寄存器表版本。

## Windows 部署检查

先发布完整 WPF 目录，不要只复制单个 DLL。框架依赖发布示例：

```powershell
dotnet publish Robot_Info_Mes\Robot_Info_Mes.csproj -c Release -p:Platform=x64 -o C:\Deploy\Robot_Info_Mes
```

目标工控机需要安装对应的 .NET Desktop Runtime x64。若现场不能安装运行时，可改用 `-r win-x64 --self-contained true` 生成自包含目录。配置和累计 XML 位于 `C:\ProgramData\HanGao\Robot_Info_Mes`，不在发布目录内，升级程序时不要删除该目录。

1. 确保端口未被占用：`netstat -ano | findstr :502`。
2. 以管理员身份按现场网段创建 Windows 防火墙入站规则，例如：

```powershell
New-NetFirewallRule -DisplayName "Robot_Info_Mes Modbus TCP" -Direction Inbound -Protocol TCP -LocalPort 502 -Action Allow -RemoteAddress 10.30.128.0/24
```

3. 从 SCADA 客户端主机检查服务端端口：`Test-NetConnection <设备IP> -Port 502`。
4. 首次联调可用端口 1502，确认程序和地址表后再切换到 502。
5. Modbus TCP 本身没有登录、加密和权限认证，只允许生产网内指定 SCADA 网段访问，不应暴露到办公网或互联网。

## UI 调试建议

每 500～1000 ms 调用 `_modbusHost.GetStatus()`，显示：

- `State`：Stopped/Running/Faulted；
- `LocalEndpoint`：当前监听 IP 和端口；
- `ConnectedClients`：当前 SCADA 客户端连接数；
- `TotalRequests` 和 `RejectedRequests`；
- `LastRequest`：最后功能码、地址和长度；
- `LastPublish`：最后快照时间和流水号；
- `LastPublishError`：业务值转换或发布错误。
