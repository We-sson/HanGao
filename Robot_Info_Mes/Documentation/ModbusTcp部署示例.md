# Robot_Info_Mes 部署 Modbus TCP 示例

## 通信角色

在本方案中：

- `Robot_Info_Mes`：Modbus TCP Server，也常被称为从站或 Slave；
- SCADA：Modbus TCP Client，也常被称为主站或 Master；
- SCADA 主动建立连接并按周期发送 FC03；`Robot_Info_Mes` 不主动连接 SCADA。

## 在 ViewModel 中启动

项目已经加入 `RobotInfoModbusServerHost`，并在 `Robot_Info_VM` 中提供了以下等价的显式启停入口：

```csharp
using System.Net;
using Robot_Info_Mes.Services;

private RobotInfoModbusServerHost? _modbusHost;

public async Task StartModbusServerAsync()
{
    _modbusHost ??= new RobotInfoModbusServerHost(
        () => Mes_Robot_Info_Model_Data);

    _modbusHost.PublishFailed += exception =>
        User_Log_Add("Modbus 寄存器发布失败：" + exception.Message);

    await _modbusHost.StartAsync(
        bindAddress: IPAddress.Any,
        port: 502,
        unitIdentifier: 1,
        publishInterval: TimeSpan.FromSeconds(1));
}

public async Task StopModbusServerAsync()
{
    if (_modbusHost is not null)
    {
        await _modbusHost.StopAsync();
    }
}
```

生产环境建议把启用开关、绑定 IP、端口、Unit ID 和发布周期放入 `Configs_Data.Xml`，默认关闭；窗口 Loaded 时按配置启动，窗口 Closing 或应用 Exit 时执行 `StopAsync/DisposeAsync`。

## WPF 窗口生命周期示例

```csharp
private bool _modbusStopped;
private bool _closeInProgress;

public Client_Window()
{
    InitializeComponent();

    Loaded += async (_, _) =>
    {
        if (DataContext is Robot_Info_VM viewModel)
        {
            await viewModel.StartModbusServerAsync();
        }
    };

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

## SCADA 连接参数

| 参数 | 示例值 |
|---|---|
| 协议 | Modbus TCP |
| 设备 IP | Robot_Info_Mes 工控机的固定 IPv4 地址 |
| TCP 端口 | 502 |
| Unit ID | 1 |
| 功能码 | FC03 Read Holding Registers |
| 起始地址 | 40001；若驱动启用零基地址则填 0 |
| 长度 | 18 个寄存器 |
| 32 位字序 | High Word First / ABCD |
| SCADA 轮询周期 | 建议 1000 ms，通常不低于 200 ms |
| 超时 | 建议 3 s |
| 重试 | 建议 2～3 次 |

SCADA 应尽量保持 TCP 长连接，并用一次请求读取 40001～40018；没有必要每次轮询都重新建立连接。服务器内部发布是原子的，一次读取整个块可以获得同一版本的数据。

## 累计产量说明

当前 `Robot_Info_Mes` 只有会在日切时清零的 `Robot_Work_ABCD_Number`，没有跨日累计产量。因此示例明确向 40010～40011 发布 `0`，未来映射代码保留为注释，没有通过求和月度图表等方式猜造累计值。

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

3. 从 SCADA 主机检查端口：`Test-NetConnection <设备IP> -Port 502`。
4. 首次联调可用端口 1502，确认程序和地址表后再切换到 502。
5. Modbus TCP 本身没有登录、加密和权限认证，只允许生产网内指定 SCADA 网段访问，不应暴露到办公网或互联网。

## UI 调试建议

每 500～1000 ms 调用 `_modbusHost.GetStatus()`，显示：

- `State`：Stopped/Running/Faulted；
- `LocalEndpoint`：当前监听 IP 和端口；
- `ConnectedClients`：当前 SCADA 连接数；
- `TotalRequests` 和 `RejectedRequests`；
- `LastRequest`：最后功能码、地址和长度；
- `LastPublish`：最后快照时间和流水号；
- `LastPublishError`：业务值转换或发布错误。
