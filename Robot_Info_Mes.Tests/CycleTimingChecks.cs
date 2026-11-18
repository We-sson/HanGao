using Robot_Info_Mes.Model;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

internal static class CycleTimingChecks
{
    public static void Run()
    {
        var single = CreateModel();
        single.Robot_Info_Data = Packet(Robot_Process_Int_Enum.Spot_Surround_1, a: true);
        Thread.Sleep(25);
        single.Robot_Info_Data = Packet(Robot_Process_Int_Enum.Spot_Surround_1);
        CheckRecorded(single, single.Robot_Work_A_Cycle.Timer.Elapsed, "单工位");

        var paired = CreateModel();
        paired.Robot_Info_Data = Packet(Robot_Process_Int_Enum.R_Side_7, a: true);
        Thread.Sleep(25);
        paired.Robot_Info_Data = Packet(Robot_Process_Int_Enum.R_Side_7, b: true);
        Thread.Sleep(25);
        paired.Robot_Info_Data = Packet(Robot_Process_Int_Enum.R_Side_7);
        CheckRecorded(paired, paired.Robot_Work_A_Cycle.Timer.Elapsed + paired.Robot_Work_B_Cycle.Timer.Elapsed, "R 边双工位");

        CheckPairedOrder();
        CheckDisconnect();
        CheckShortIdleAndWork();
        CheckLoadFactors();
        CheckModeTimers();
        Console.WriteLine("PASS: exact cycle timing, strict A-B/C-D order, interruption cancellation and finite OEE values.");
    }

    static void CheckPairedOrder()
    {
        const Robot_Process_Int_Enum process = Robot_Process_Int_Enum.R_Side_7;
        var repeated = CreateModel();
        foreach (var packet in new[] { Packet(process, a: true), Packet(process, a: true),
                     Packet(process, b: true), Packet(process, b: true), Packet(process), Packet(process) })
            repeated.Robot_Info_Data = packet;
        Check(repeated.Robot_Work_ABCD_Number == 1, "重复有效电平不应重复计件。");

        (string Name, Robot_Mes_Info_Data_Receive[] Packets)[] cancelled =
        [
            ("单侧后空闲", [Packet(process, a: true), Packet(process), Packet(process, b: true), Packet(process)]),
            ("B先于A", [Packet(process, b: true), Packet(process, a: true), Packet(process)]),
            ("A-B-A回跳", [Packet(process, a: true), Packet(process, b: true), Packet(process, a: true), Packet(process)]),
            ("双侧同时有效", [Packet(process, a: true), Packet(process, a: true, b: true), Packet(process, b: true), Packet(process)]),
            ("故障结束", [Packet(process, a: true), Packet(process, b: true), Packet(process, mode: KUKA_Mode_OP_Enum.Error)]),
            ("模式中断", [Packet(process, a: true), Packet(process, a: true, mode: KUKA_Mode_OP_Enum.T1), Packet(process, b: true), Packet(process)]),
            ("工艺号变化", [Packet(process, a: true), Packet(Robot_Process_Int_Enum.Spot_Surround_1, a: true),
                Packet(process, b: true), Packet(process)])
        ];
        foreach (var scenario in cancelled)
        {
            var model = CreateModel();
            foreach (var packet in scenario.Packets) model.Robot_Info_Data = packet;
            Check(model.Robot_Work_ABCD_Number == 0, $"{scenario.Name}错误计为完整产品。");
            Check(!model.Robot_Work_A_Cycle.Timer.IsRunning && !model.Robot_Work_B_Cycle.Timer.IsRunning,
                $"{scenario.Name}取消后仍在累计节拍。");
            model.Robot_Info_Data = Packet(process);
            model.Robot_Info_Data = Packet(process, a: true);
            model.Robot_Info_Data = Packet(process, b: true);
            model.Robot_Info_Data = Packet(process);
            Check(model.Robot_Work_ABCD_Number == 1, $"{scenario.Name}后下一件正常产品无法恢复计件。");
        }

        var simultaneous = CreateModel();
        simultaneous.Robot_Info_Data = Packet(process, a: true, c: true);
        simultaneous.Robot_Info_Data = Packet(process, b: true, d: true);
        simultaneous.Robot_Info_Data = Packet(process);
        Check(simultaneous.Robot_Work_ABCD_Number == 2 && simultaneous.Robot_Work_ABCD_Cycle_List.Count == 3,
            "AB和CD同包完成应保留两件和两条独立节拍。");
    }

    static void CheckDisconnect()
    {
        foreach (var process in new[] { Robot_Process_Int_Enum.Spot_Surround_1, Robot_Process_Int_Enum.R_Side_7 })
        {
            var model = CreateModel();
            model.Robot_Info_Data = Packet(process, a: true);
            Thread.Sleep(20);
            model.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Disconnected;
            var work = model.Robot_Work_Time.Timer.Elapsed;
            Thread.Sleep(25);
            Check(!model.Robot_Work_A_Cycle.Timer.IsRunning && model.Robot_Work_Time.Timer.Elapsed == work,
                "断线期间继续累计工位或作业时间。");
            model.Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected;
            model.Robot_Info_Data = Packet(process, b: process == Robot_Process_Int_Enum.R_Side_7);
            model.Robot_Info_Data = Packet(process);
            Check(model.Robot_Work_ABCD_Number == 0, "重连后首帧把断线前的半件计入产量。");
            model.Robot_Info_Data = Packet(process, a: true);
            if (process == Robot_Process_Int_Enum.R_Side_7) model.Robot_Info_Data = Packet(process, b: true);
            model.Robot_Info_Data = Packet(process);
            Check(model.Robot_Work_ABCD_Number == 1, "断线重连后的正常产品不能恢复计件。");
        }
    }

    static void CheckShortIdleAndWork()
    {
        const Robot_Process_Int_Enum process = Robot_Process_Int_Enum.Spot_Surround_1;
        var model = CreateModel();
        model.Robot_Info_Data = Packet(process, a: true);
        Thread.Sleep(25);
        model.Robot_Info_Data = Packet(process);
        Check(model.Robot_Work_Time.Timer_UI > TimeSpan.Zero, "短作业未采样实际用时。");
        Thread.Sleep(30);
        model.Robot_Info_Data = Packet(process, a: true);
        Check(model.Robot_Robot_Time_Outside_List.ElementAtOrDefault(1) > 0,
            "两件之间的短空闲被记为零秒。");
        model.CancelActiveCycles();
    }

    static void CheckLoadFactors()
    {
        var factors = new Work_Factor_Seried_Model();
        Check(factors.Get_Work_Availability_Factor(10, 0) == 0
              && factors.Get_Work_Availability_Factor(-1, 10) == 0
              && factors.Get_Work_Availability_Factor(double.NaN, 10) == 0,
            "无效时间稼动率应显示零。");
        Check(factors.Get_Work_Performance_Factor(30, 1, 0) == 0
              && factors.Get_Work_Performance_Factor(0, 1, 30) == 0
              && factors.Get_Work_Performance_Factor(double.PositiveInfinity, 1, 30) == 0,
            "无效性能稼动率应显示零。");
        Check(factors.Get_Work_Availability_Factor(50, 100) == 50
              && factors.Get_Work_Performance_Factor(30, 1, 60) == 50,
            "有效OEE样本计算被改变。");
        var model = CreateModel();
        model.Robot_Info_Data = Packet(Robot_Process_Int_Enum.Spot_Surround_2, a: true);
        model.Robot_Work_A_Cycle.Timer_UI = TimeSpan.FromSeconds(15);
        var value = factors.Get_Work_Cycle_Load_Factor(Robot_Process_Int_Enum.Spot_Surround_2,
            ref model.Robot_Work_A_Cycle, ref model.Robot_Work_B_Cycle,
            ref model.Robot_Work_C_Cycle, ref model.Robot_Work_D_Cycle, 30);
        Check(value == 50, "光华激光围边的实时负荷没有使用当前节拍。");
        value = factors.Get_Work_Cycle_Load_Factor(Robot_Process_Int_Enum.Spot_Surround_2,
            ref model.Robot_Work_A_Cycle, ref model.Robot_Work_B_Cycle,
            ref model.Robot_Work_C_Cycle, ref model.Robot_Work_D_Cycle, 0);
        Check(value == 0, "零标准节拍错误显示为满负荷。");
        model.CancelActiveCycles();
    }

    static void CheckModeTimers()
    {
        foreach (var previousMode in new[] { KUKA_Mode_OP_Enum.T1, KUKA_Mode_OP_Enum.Error })
        foreach (var nextMode in Enum.GetValues<KUKA_Mode_OP_Enum>())
        {
            if (previousMode == nextMode) continue;
            var model = CreateModel();
            model.Robot_Run_Time.Start();
            model.Robot_Info_Data = Packet(Robot_Process_Int_Enum.R_Side_7, mode: previousMode);
            Thread.Sleep(20);
            model.Robot_Info_Data = Packet(Robot_Process_Int_Enum.R_Side_7, mode: nextMode);

            var stopped = previousMode == KUKA_Mode_OP_Enum.T1 ? model.Robot_Debug_Time : model.Robot_Error_Time;
            var stoppedTotal = previousMode == KUKA_Mode_OP_Enum.T1 ? model.Robot_Debug_All_Time : model.Robot_Error_All_Time;
            Check(stopped.Timer_UI > TimeSpan.Zero && stopped.Timer_UI == stopped.Timer.Elapsed
                  && stoppedTotal.Timer_UI > TimeSpan.Zero && stoppedTotal.Timer_UI == stoppedTotal.Timer.Elapsed,
                $"{previousMode}切到{nextMode}时丢失最后一段模式时长。");
            var elapsed = stopped.Timer.Elapsed;
            Thread.Sleep(20);
            Check(stopped.Timer.Elapsed == elapsed && !stopped.Timer.IsRunning,
                $"{previousMode}切到{nextMode}后仍在累计上一模式。");
            Check(model.Robot_Debug_Time.Timer.IsRunning == (nextMode == KUKA_Mode_OP_Enum.T1)
                  && model.Robot_Error_Time.Timer.IsRunning == (nextMode == KUKA_Mode_OP_Enum.Error),
                $"{nextMode}的调试与故障计时状态错误。");
            Check(model.Robot_Run_Time.Timer.IsRunning && model.Robot_Work_ABCD_Number == 0,
                "模式切换影响软件运行时间或产生了产量。");
            model.CancelActiveCycles();
            model.Robot_Run_Time.Stop();
        }
    }

    static Mes_Robot_Info_Model CreateModel() => new()
    {
        Socket_Robot_Connect_State = Socket_Robot_Connect_State_Enum.Connected
    };

    static Robot_Mes_Info_Data_Receive Packet(Robot_Process_Int_Enum process, bool a = false, bool b = false,
        bool c = false, bool d = false, KUKA_Mode_OP_Enum mode = KUKA_Mode_OP_Enum.Run) => new()
    {
        Mes_Robot_Mode = mode,
        Robot_Process_Int = process,
        Mes_Work_A_State = a,
        Mes_Work_B_State = b,
        Mes_Work_C_State = c,
        Mes_Work_D_State = d
    };

    static void CheckRecorded(Mes_Robot_Info_Model model, TimeSpan elapsed, string station)
    {
        // 同步发送边沿，Dispatcher 没有机会刷新，结果必须来自实际计时而非界面缓存。
        if (model.Robot_Work_ABCD_Number != 1
            || elapsed <= TimeSpan.Zero
            || model.Robot_Work_AB_Cycle.Timer_UI != elapsed
            || model.Robot_Work_ABCD_Cycle_List.ElementAtOrDefault(1) != elapsed.TotalSeconds)
        {
            throw new InvalidOperationException($"{station}未按实际经过时间记录并显示节拍。");
        }
    }

    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
