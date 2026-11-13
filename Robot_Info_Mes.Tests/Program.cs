using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Generic_Extension;
using Robot_Info_Mes.Model;
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

        Console.WriteLine(_failures == 0 ? "ALL CHECKS PASSED" : $"{_failures} CHECK(S) FAILED");
        return _failures == 0 ? 0 : 1;
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
