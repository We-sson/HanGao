using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Robot_Info_Mes.Model;
using Robot_Info_Mes.ViewModel;
using RobotSimulator.Controls;
using RobotSimulator.ViewModels;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>隔离回环验证缺轴、异常数值、断线多来源、最新快照及收包前日切。</summary>
internal static class MesClientEdgeChecks
{
    public static async Task RunAsync(Robot_Info_VM vm, RobotViewer viewer)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var token = timeout.Token;
        var joints = (IReadOnlyList<JointViewModel>)typeof(RobotViewer)
            .GetField("_joints", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(viewer)!;
        var simulation = vm.Digital_Simulation!;
        var server = new Socket_Receive
        {
            Socket_Robot = Socket_Robot_Protocols_Enum.KUKA,
            UseXmlFragmentReceive = true,
            Robot_Info_Model_Data_Delegate = vm.Robot_Mes_Info_Receive_Method,
            Socket_ErrorInfo_delegate = vm.Socket_Robot_ErrorLog_Show
        };
        server.Server_Strat("127.0.0.1", "0");
        Check(server.ServerStarted, "边缘测试监听启动失败。");
        var endpoint = (IPEndPoint)server.Socket_Sever!.LocalEndPoint!;
        try
        {
            using var first = new TcpClient { NoDelay = true };
            await first.ConnectAsync(endpoint, token);
            var stream = first.GetStream();
            Check(await SendAsync(stream, Axes("35"), token), "正常角度未被确认。");
            await WaitAsync(() => Near(joints[0].Angle, 35), token);
            Check(!await SendAsync(stream, Axes("NaN"), token), "NaN 被确认成功。");
            Check(!await SendAsync(stream, Axes("INF"), token), "无穷角度被确认成功。");
            Check(!await SendAsync(stream, "<Robot_Axis J1=\"0\"/>", token), "不完整六轴被确认成功。");
            Check(Near(simulation.Joints[0], 35), "无效帧改写了最近一次有效角度。");
            Check(await SendAsync(stream, "", token), "不含轴组的状态包未被接受。");
            Check(Near(simulation.Joints[0], 35), "缺轴组时丢失了最近姿态。");
            Check(await SendAsync(stream, Axes("-20"), token), "异常角度后的正常帧未恢复。");
            await WaitAsync(() => Near(joints[0].Angle, -20), token);

            Check(await SendAsync(stream, Axes("195"), token), "有限超限角度没有沿用行程限制。");
            await WaitAsync(() => Near(joints[0].Angle, 170), token);
            Check(await SendAsync(stream, Axes("0"), token), "合法零角度被错误拒绝。");
            await WaitAsync(() => Near(joints[0].Angle, 0), token);
            var snapshot = typeof(Robot_Info_VM).GetMethod("Create_Mes_Server_Info_Snapshot",
                BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(vm, null) as Mes_Server_Info_Data_Receive;
            Check(snapshot?.Robot_Mes_Info_Data.Robot_Axis?.J1 == 0, "上传看板的轴角不是最新有效值。");

            using var second = new TcpClient { NoDelay = true };
            await second.ConnectAsync(endpoint, token);
            Check(await SendAsync(second.GetStream(), Axes("25"), token), "第二来源报文未被接受。");
            await WaitAsync(() => Near(joints[0].Angle, 25), token);
            first.Close();
            await Task.Delay(100, token);
            Check(vm.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected,
                "旧连接断开把有效新连接误标为离线。");
            second.Close();
            await WaitAsync(() => vm.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State
                == Socket_Robot_Connect_State_Enum.Disconnected, token);
            Check(!vm.Mes_Robot_Info_Model_Data.Robot_Work_Time.Timer.IsRunning,
                "最后一个来源断线后作业时间还在累计。");

            using var reconnect = new TcpClient { NoDelay = true };
            await reconnect.ConnectAsync(endpoint, token);
            Check(await SendAsync(reconnect.GetStream(), Axes("-35"), token), "重连报文未确认。");
            await WaitAsync(() => Near(joints[0].Angle, -35), token);
            var completed = vm.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number;
            Check(await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run", a: true), "R 边首区报文未确认。");
            Check(!await SendAsync(reconnect.GetStream(), Axes("NaN"), token, "Run", a: true), "在途异常帧被接受。");
            await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run", b: true);
            await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run");
            Check(vm.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number == completed,
                "异常角度包前后的半周期被拼接成产品。");
            await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run", a: true);
            await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run", b: true);
            await SendAsync(reconnect.GetStream(), Axes("-35"), token, "Run");
            Check(vm.Mes_Robot_Info_Model_Data.Robot_Work_ABCD_Number == completed + 1,
                "异常帧后完整的 A-B 周期未恢复计件。");
            reconnect.Close();
            await WaitAsync(() => vm.Mes_Robot_Info_Model_Data.Socket_Robot_Connect_State
                == Socket_Robot_Connect_State_Enum.Disconnected, token);
            Console.WriteLine("PASS: rejected missing/NaN/INF axes, retained pose without axes, valid recovery, clamped target, latest upload and multi-source reconnect");
        }
        finally { server.Sever_End(); }

        CheckDailyRollover();
    }

    private static void CheckDailyRollover()
    {
        var vm = new Robot_Info_VM(new File_Int_Model());
        var model = vm.Mes_Robot_Info_Model_Data;
        var previousDay = DateTime.Now.Date.AddDays(-1);
        model.File_Update_Time = previousDay;
        model.Robot_Work_ABCD_Number = 17;
        vm.EnsureClientStatisticsDate(previousDay);
        var start = new Robot_Mes_Info_Data_Receive
        {
            Mes_Robot_Mode = KUKA_Mode_OP_Enum.Run,
            Robot_Process_Int = Robot_Process_Int_Enum.Spot_Surround_1,
            Mes_Work_A_State = true
        };
        vm.Robot_Mes_Info_Receive_Method(start, null);
        vm.Robot_Mes_Info_Receive_Method(new Robot_Mes_Info_Data_Receive(start) { Mes_Work_A_State = false }, null);
        Check(model.Robot_Work_ABCD_Number == 1, "零点后首件未从当天零基数开始累计。");
        vm.EnsureClientStatisticsDate(DateTime.Now);
        vm.EnsureClientStatisticsDate(DateTime.Now);
        Check(model.Robot_Work_ABCD_Number == 1, "日切重复执行清除了当天首件。");
        Console.WriteLine("PASS: first new-day product remains after repeated save-time rollover checks");
    }

    private static string Axes(string first) =>
        $"<Robot_Axis J1=\"{first}\" J2=\"-60\" J3=\"76\" J4=\"0\" J5=\"0\" J6=\"20\"/>";

    private static async Task<bool> SendAsync(NetworkStream stream, string axes, CancellationToken token,
        string mode = "Unknown", bool a = false, bool b = false)
    {
        var packet = $"<Robot_Receive Robot_Type=\"KUKA\" Vision_Model=\"Mes_Info_Data\"><Mes_Robot_Mode>{mode}</Mes_Robot_Mode>"
            + $"<Mes_Work_A_State>{a.ToString().ToLowerInvariant()}</Mes_Work_A_State>"
            + $"<Mes_Work_B_State>{b.ToString().ToLowerInvariant()}</Mes_Work_B_State>{axes}</Robot_Receive>";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(packet), token);
        var buffer = new byte[8192];
        var received = 0;
        const string endTag = "</Robot_Send>";
        while (received < buffer.Length)
        {
            var count = await stream.ReadAsync(buffer.AsMemory(received), token);
            if (count == 0) throw new IOException("MES 在确认角度前断开。");
            received += count;
            var reply = Encoding.UTF8.GetString(buffer, 0, received);
            var end = reply.IndexOf(endTag, StringComparison.Ordinal);
            if (end >= 0)
                return (bool?)XElement.Parse(reply[..(end + endTag.Length)]).Attribute("IsStatus") == true;
        }
        throw new IOException("MES 没有返回完整回执。");
    }

    private static async Task WaitAsync(Func<bool> condition, CancellationToken token)
    {
        while (!condition()) await Task.Delay(25, token);
    }

    private static bool Near(double actual, double expected) => Math.Abs(actual - expected) < 0.1;
    private static void Check(bool ok, string error)
    {
        if (!ok) throw new InvalidOperationException(error);
    }
}
