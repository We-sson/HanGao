using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using Generic_Extension;
using HanGao.RobotInfo.Modbus;
using Robot_Info_Mes.Model;
using Robot_Info_Mes.ViewModel;
using RobotSimulator.Models;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>
/// Robot_Info_Mes 的轻量自检入口；沿用仓库现有控制台断言风格，不引入测试框架依赖。
/// </summary>
internal static class Program
{
    private static int _failures;

    [STAThread]
    private static int Main()
    {
        Check_AddDataAt_WritesZero();
        Check_DayReset_SameDayOfMonth();
        Check_ResetDaily_KeepsRunningState();
        Check_TwoProductsInOnePacket_KeepBothSamples();
        Check_JointOnlyPacket_StillAdvancesCycleDisplay();
        Check_WeldState_FromKukaXml();
        Check_ListenerStart_FailureIsReported();
        Check_ReadFailure_DoesNotTouchFile();
        Check_SimulationQuality_RestoresFromXml();
        Check_UserLog_Rollover_KeepsEntry501();
        Check_MesDateInt_ShrinksExcessDays();
        Check_Snapshot_RetainsRobotCycleList();
        Check_ModbusServerHost_PublishesLiveDelegateValues();
        RobotAxisValidationChecks.Run();
        Check_MesViewport();

        Console.WriteLine(_failures == 0 ? "ALL CHECKS PASSED" : $"{_failures} CHECK(S) FAILED");
        return _failures == 0 ? 0 : 1;
    }

    private static void Check_MesViewport()
    {
        try
        {
            CycleTimingChecks.Run();
            MesViewportChecks.Run();
        }
        catch (Exception exception)
        {
            _failures++;
            Console.Error.WriteLine(exception);
        }
    }

    private static void Check_AddDataAt_WritesZero()
    {
        var values = new ObservableCollection<double?>();

        values.AddDataAt(2, 0);

        Check(values.Count == 3, "AddDataAt 应补齐到索引 2。");
        Check(values[2] == 0, "AddDataAt 应写入 0 值。");
    }

    private static void Check_DayReset_SameDayOfMonth()
    {
        var model = new Mes_Robot_Info_Model
        {
            File_Update_Time = new DateTime(2026, 10, 5, 23, 0, 0),
            Robot_Work_ABCD_Number = 12,
            Work_Number_Pallets = 3,
        };
        model.Robot_Run_Time.Timer_UI = TimeSpan.FromHours(5);
        model.Robot_Work_Time.Timer_UI = TimeSpan.FromHours(4);
        model.Robot_Debug_Time.Timer_UI = TimeSpan.FromHours(2);
        model.Robot_Error_Time.Timer_UI = TimeSpan.FromHours(1);
        model.Robot_Time_Outside.Timer_UI = TimeSpan.FromSeconds(11);

        // 10 月 5 日与 11 月 5 日“几号”相同，旧实现会漏掉清空。
        model.Check_Day_Int_Time(new DateTime(2026, 11, 5, 0, 1, 0));

        Check(model.Robot_Work_ABCD_Number == 0, "跨月同日号应清零产量。");
        Check(model.Work_Number_Pallets == 0, "跨月同日号应清零栈板数。");
        Check(model.Robot_Run_Time.Timer_UI == TimeSpan.Zero, "跨月同日号应清零开机时间。");
        Check(model.Robot_Work_Time.Timer_UI == TimeSpan.Zero, "跨月同日号应清零作业时间。");
        Check(model.Robot_Debug_Time.Timer_UI == TimeSpan.Zero, "跨月同日号应清零调试时间。");
        Check(model.Robot_Error_Time.Timer_UI == TimeSpan.Zero, "跨月同日号应清零故障时间。");
        Check(model.Robot_Time_Outside.Timer_UI == TimeSpan.Zero, "跨月同日号应清零节拍外时间。");
    }

    private static void Check_ResetDaily_KeepsRunningState()
    {
        var timer = new Time_Model
        {
            Timer_UI = TimeSpan.FromHours(2),
            Time_Offset = TimeSpan.FromHours(2),
        };
        timer.Start();

        timer.ResetDaily();

        Check(timer.Timer_UI == TimeSpan.Zero, "ResetDaily 应清零显示值。");
        Check(timer.Time_Offset == TimeSpan.Zero, "ResetDaily 应清零偏移量。");
        Check(timer.Timer.IsRunning, "ResetDaily 应保持原来的运行状态。");

        Thread.Sleep(100);
        Check(timer.Timer.Elapsed > TimeSpan.Zero, "ResetDaily 后 Stopwatch 应继续计时。");
        timer.Stop();
    }

    private static void Check_TwoProductsInOnePacket_KeepBothSamples()
    {
        var model = new Mes_Robot_Info_Model
        {
            Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected,
        };

        model.Robot_Info_Data = new Robot_Mes_Info_Data_Receive
        {
            Mes_Robot_Mode = KUKA_Mode_OP_Enum.Run,
            Robot_Process_Int = Robot_Process_Int_Enum.Spot_Surround_1,
            Mes_Work_A_State = true,
            Mes_Work_C_State = true,
        };

        // 模拟两个工位在当前包之前已累计的节拍。
        model.Robot_Work_A_Cycle.Timer_UI = TimeSpan.FromSeconds(10);
        model.Robot_Work_C_Cycle.Timer_UI = TimeSpan.FromSeconds(12);

        model.Robot_Info_Data = new Robot_Mes_Info_Data_Receive
        {
            Mes_Robot_Mode = KUKA_Mode_OP_Enum.Run,
            Robot_Process_Int = Robot_Process_Int_Enum.Spot_Surround_1,
            Mes_Work_A_State = false,
            Mes_Work_C_State = false,
        };

        Check(model.Robot_Work_ABCD_Number == 2, "同一包内 AB、CD 同时完成时应计 2 件。");
        Check(model.Robot_Work_ABCD_Cycle_List.ElementAtOrDefault(1).HasValue,
            "第一件产品的节拍应写入产品序号 1。");
        Check(model.Robot_Work_ABCD_Cycle_List.ElementAtOrDefault(2).HasValue,
            "同一包内第二件产品应写入产品序号 2，而不是覆盖序号 1。");
    }

    /// <summary>只有 J1～J6 变化的报文也必须推进节拍状态机，否则界面上的生产节拍会冻结一个周期。</summary>
    private static void Check_JointOnlyPacket_StillAdvancesCycleDisplay()
    {
        var model = new Mes_Robot_Info_Model
        {
            Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected,
        };
        var snapshot = new Robot_Mes_Info_Data_Receive
        {
            Mes_Robot_Mode = KUKA_Mode_OP_Enum.Run,
            Robot_Process_Int = Robot_Process_Int_Enum.R_Side_7,
            Mes_Work_A_State = true,
        };
        model.Robot_Info_Data = snapshot;

        // 控制台测试没有 UI 帧循环，直接给 A 区计时器一个已累计值，模拟界面计时器已走过 10 秒。
        model.Robot_Work_A_Cycle.Timer_UI = TimeSpan.FromSeconds(10);
        var notifiedSnapshot = model.Robot_Info_Data;

        // 运动报文每帧只有 J1～J6 变化，界面字段不变，走不替换快照的低开销推进路径。
        model.Advance_Robot_Info_Data(new Robot_Mes_Info_Data_Receive(snapshot));

        Check(model.Robot_Work_AB_Cycle.Timer_UI == TimeSpan.FromSeconds(10),
            "只有关节变化的报文没有推进合成节拍显示。");
        Check(ReferenceEquals(notifiedSnapshot, model.Robot_Info_Data),
            "低开销推进不应替换界面快照，否则每帧都会整批刷新绑定。");
    }

    /// <summary>KUKA 上报的 $OUT[1] 焊接状态应进入快照，并在复制构造时保留。</summary>
    private static void Check_WeldState_FromKukaXml()
    {
        const string xml = "<Robot_Receive Robot_Type=\"KUKA\" Vision_Model=\"Mes_Info_Data\">"
            + "<Mes_Weld_State>true</Mes_Weld_State></Robot_Receive>";
        var data = KUKA_Send_Receive_Xml.String_Xml<Robot_Mes_Info_Data_Receive>(xml);

        Check(data.Mes_Weld_State, "KUKA XML 应解析 $OUT[1] 焊接状态。");
        Check(new Robot_Mes_Info_Data_Receive(data).Mes_Weld_State, "快照复制应保留焊接状态。");
    }

    /// <summary>
    /// 只读校验：正常配置读取不应触发回退日志。
    /// 损坏文件的改名行为只能在人工验证，这里不触碰 ProgramData 下的真实文件。
    /// </summary>
    private static void Check_ReadFailure_DoesNotTouchFile()
    {
        string? error = null;
        File_Int_Model config =
            File_Xml_Model.Read_Xml_File_Or_Default<File_Int_Model>(message => error = message);

        Check(error is null, "正常配置文件读取不应报错：" + error);
        Check(config is not null, "正常配置应返回实例。");
    }

    private static void Check_SimulationQuality_RestoresFromXml()
    {
        var serializer = new XmlSerializer(typeof(File_Int_Model));
        using var previousXml = new StringReader("<File_Int_Model><Mes_Digital_Simulation /></File_Int_Model>");
        var config = (File_Int_Model)serializer.Deserialize(previousXml)!;
        Check(config.Mes_Digital_Simulation.Render_Quality == RenderQuality.High,
            "配置缺少画质时，应使用高清档。");

        config.Mes_Digital_Simulation.Render_Quality = RenderQuality.Balanced;
        using var saved = new StringWriter();
        serializer.Serialize(saved, config);
        using var savedXml = new StringReader(saved.ToString());
        var restored = (File_Int_Model)serializer.Deserialize(savedXml)!;
        Check(restored.Mes_Digital_Simulation.Render_Quality == RenderQuality.Balanced,
            "重新读取 MES 配置时，应恢复已保存的均衡档。");
    }

    private static void Check_ListenerStart_FailureIsReported()
    {
        var occupied = new TcpListener(IPAddress.Loopback, 0);
        occupied.Start();
        try
        {
            int port = ((IPEndPoint)occupied.LocalEndpoint).Port;
            var listener = new Socket_Receive(IPAddress.Loopback.ToString(), port.ToString());

            Check(!listener.ServerStarted, "端口被占用时 ServerStarted 应为 false。");
            Check(!string.IsNullOrWhiteSpace(listener.ServerStartError), "端口被占用时应记录启动失败原因。");
        }
        finally
        {
            occupied.Stop();
        }
    }

    /// <summary>日志达到 500 条回卷清空后应保留第 501 条记录，且读取序号本身不产生副作用。</summary>
    private static void Check_UserLog_Rollover_KeepsEntry501()
    {
        var log = new User_Log_Models();
        int initialNumber = log.User_Log_Number;
        Check(initialNumber == 0 && log.User_Log_Number == 0,
            "读取 User_Log_Number 不应改变当前序号。");

        for (int i = 0; i < 500; i++)
        {
            log.User_Log = $"msg-{i}";
        }
        Check(log.User_Log_Number == 500, "连续写入 500 条日志后序号应为 500。");

        log.User_Log = "entry-501";
        Check(log.User_Log_Number == 1, "第 501 条日志写入触发回卷后序号应推进为 1。");
        string rendered = log.User_Log;
        Check(rendered.Contains("000 | ") && rendered.Contains("entry-501"),
            "满 500 条回卷后应保留第 501 条新日志，而不是在 getter 中直接清空。");
    }

    /// <summary>当月天数少于已有月度槽位时，Mes_Date_Int 应一次性裁剪全部多余尾部元素。</summary>
    private static void Check_MesDateInt_ShrinksExcessDays()
    {
        var series = new Work_Factor_Seried_Model();
        for (int i = 0; i < 35; i++)
        {
            series.Robot_Work_ABCD_Number_List.Add(i);
            series.Work_Availability_Factor_List.Add(i);
            series.Work_Performance_Factor_List.Add(i);
            series.Robot_Work_Time_List.Add(i);
            series.Robot_Work_ABCD_Cycle_Mean_List.Add(i);
        }

        DateTime now = DateTime.Now;
        int expectedDays = DateTime.DaysInMonth(now.Year, now.Month);
        series.Mes_Date_Int();

        Check(series.Robot_Work_ABCD_Number_List.Count == expectedDays,
            $"Mes_Date_Int 应将月度产量集合裁剪为当月 {expectedDays} 天。");
        Check(series.Work_Availability_Factor_List.Count == expectedDays
            && series.Work_Performance_Factor_List.Count == expectedDays
            && series.Robot_Work_Time_List.Count == expectedDays
            && series.Robot_Work_ABCD_Cycle_Mean_List.Count == expectedDays,
            "Mes_Date_Int 应将其余月度趋势集合一并裁剪到当月天数。");
    }

    /// <summary>Client 上报快照应携带机器人逐件节拍样本，而不是空的 Work_Factor_Seried 集合。</summary>
    private static void Check_Snapshot_RetainsRobotCycleList()
    {
        var vm = new Robot_Info_VM(new File_Int_Model());
        try
        {
            vm.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Cycle_List.AddDataAt(1, 18.5);
            var snapshot = vm.Create_Mes_Server_Info_Snapshot();
            Check(snapshot.Mes_Server_Date.Robot_Work_ABCD_Cycle_List.ElementAtOrDefault(1) == 18.5,
                "Create_Mes_Server_Info_Snapshot 应复制设备模型中的逐件节拍样本列表。");
        }
        finally
        {
            vm.Digital_Simulation?.Detach();
        }
    }

    /// <summary>Modbus TCP 托管宿主应通过实时读取委托刷新各内置寄存器值文本。</summary>
    private static void Check_ModbusServerHost_PublishesLiveDelegateValues()
    {
        var probe = new TcpListener(IPAddress.Any, 0);
        probe.Start();
        int port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();

        var config = RobotInfoModbusConfiguration.CreateDefault();
        config.Port = port;

        uint production = 321;
        ushort status = 1;
        var host = new RobotInfoModbusServerHost(() => new RobotInfoModbusRegisterData
        {
            DailyProduction = production,
            StatusCode = status,
        });

        try
        {
            host.StartAsync(config).GetAwaiter().GetResult();
            var statusPoint = config.RegisterPoints.First(p => p.Order == 4);
            var productionPoint = config.RegisterPoints.First(p => p.Order == 5);
            Check(statusPoint.CurrentValueText == "1", "Modbus 启动首帧应发布实时状态值。");
            Check(productionPoint.CurrentValueText == "321", "Modbus 启动首帧应发布实时当天产量。");

            production = 654;
            status = 0;
            host.PublishNow();
            Check(statusPoint.CurrentValueText == "0", "PublishNow 应刷新实时状态值。");
            Check(productionPoint.CurrentValueText == "654", "PublishNow 应刷新实时当天产量。");
        }
        finally
        {
            host.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    private static void Check(bool condition, string message)
    {
        if (condition)
        {
            return;
        }

        _failures++;
        Console.WriteLine("FAIL: " + message);
    }
}
