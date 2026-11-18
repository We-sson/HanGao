using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Robot_Info_Mes.MotionClient;
using Robot_Info_Mes.ViewModel;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>通过回环连接模拟 KUKA 上报，验证生产接收回调、六轴更新和节拍。</summary>
internal static class MesRobotCommunicationChecks
{
    public static async Task RunAsync(Robot_Info_VM vm, Func<double[], double, Task> verifyDisplay)
    {
        var simulation = vm.Digital_Simulation
            ?? throw new InvalidOperationException("MES 采集端未初始化三维仿真。");
        var model = vm.Mes_Robot_Info_Model_Data;
        var originalCount = model.Robot_Work_ABCD_Number;
        var server = new Socket_Receive
        {
            Socket_Robot = Socket_Robot_Protocols_Enum.KUKA,
            UseXmlFragmentReceive = true,
            Robot_Info_Model_Data_Delegate = vm.Robot_Mes_Info_Receive_Method,
            Socket_ErrorInfo_delegate = vm.Socket_Robot_ErrorLog_Show
        };
        server.Server_Strat(IPAddress.Loopback.ToString(), "0");
        try
        {
            Check(server.ServerStarted, "本机测试监听启动失败：" + server.ServerStartError);
            using var client = new TcpClient { NoDelay = true };
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var endpoint = (IPEndPoint)server.Socket_Sever!.LocalEndPoint!;
            var confirmed = await MesJointSender.RunAsync(endpoint.Address.ToString(), endpoint.Port,
                TimeSpan.FromSeconds(2), Console.WriteLine, timeout.Token);
            Check(confirmed > 1, "独立客户端未收到连续角度回执。");
            var demoAngles = simulation.Joints;
            Check(demoAngles.Length == 6 && demoAngles[0] > 10 && demoAngles[5] > 10, "独立客户端的六轴测试帧没有到达 MES。");
            Check(model.Robot_Work_ABCD_Number == originalCount && !model.Robot_Work_A_Cycle.Timer.IsRunning,
                "独立客户端不应产生模拟产量或启动工位节拍。");
            await verifyDisplay(demoAngles, model.Robot_Work_AB_Cycle.Timer_Sec);

            await client.ConnectAsync(endpoint, timeout.Token);
            using var stream = client.GetStream();

            double[] start = [12.5, -75.25, 90.5, 14, -10.5, 20];
            await SendAsync(stream, vm, start, stationA: true, stationB: false, timeout.Token, split: true);
            await WaitForAsync(() => simulation.Joints.SequenceEqual(start), timeout.Token);
            Check(model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Connected, "首次机器人包未使 MES 显示在线。");
            Check(model.Robot_Work_A_Cycle.Timer.IsRunning, "A 工位有效信号未启动节拍。");

            var snapshot = model.Robot_Info_Data;
            await WaitForAsync(() => model.Robot_Work_A_Cycle.Timer_Sec >= 1, timeout.Token);
            double[] motion = [42.5, -62.25, 74.5, -28, 23.5, 55];
            await SendAsync(stream, vm, motion, stationA: true, stationB: false, timeout.Token);
            await WaitForAsync(() => simulation.Joints.SequenceEqual(motion), timeout.Token);
            Check(ReferenceEquals(snapshot, model.Robot_Info_Data), "纯六轴报文不应重建生产状态快照。");
            Check(model.Robot_Work_AB_Cycle.Timer_Sec >= 1, "纯六轴报文没有更新当前生产节拍。");
            await verifyDisplay(motion, model.Robot_Work_AB_Cycle.Timer_Sec);

            await SendAsync(stream, vm, motion, stationA: false, stationB: true, timeout.Token);
            await WaitForAsync(() => model.Robot_Work_B_Cycle.Timer_Sec >= 1, timeout.Token);
            await SendAsync(stream, vm, motion, stationA: false, stationB: false, timeout.Token);
            var cycle = model.Robot_Work_AB_Cycle.Timer_Sec;
            Check(model.Robot_Work_ABCD_Number == originalCount + 1, "A/B 工位完成后应累计 1 件。");
            Check(cycle >= 2 && model.Robot_Work_ABCD_Cycle_List.ElementAtOrDefault(originalCount + 1) == cycle,
                "A/B 工位完成后未保留本件节拍。");
            await verifyDisplay(motion, cycle);

            Console.WriteLine($"PASS: KUKA TCP loopback ACK, six-axis follow and production cycle ({cycle:F2} s, 1 completed product)");
            client.Close();
            await WaitForAsync(() => model.Socket_Robot_Connect_State == Socket_Robot_Connect_State_Enum.Disconnected, timeout.Token);
        }
        finally
        {
            server.Sever_End();
        }
    }

    private static async Task SendAsync(
        NetworkStream stream, Robot_Info_VM vm, double[] joints, bool stationA, bool stationB,
        CancellationToken cancellation, bool split = false)
    {
        var packet = string.Create(System.Globalization.CultureInfo.InvariantCulture, $"""
            <Robot_Receive Robot_Type="KUKA" Vision_Model="Mes_Info_Data">
                <Mes_Programs_Name>MES_LOCAL_TEST</Mes_Programs_Name>
                <Mes_Robot_Mode>Run</Mes_Robot_Mode>
                <Robot_Process_Int>R_Side_7</Robot_Process_Int>
                <Mes_Work_A_State>{(stationA ? "true" : "false")}</Mes_Work_A_State>
                <Mes_Work_B_State>{(stationB ? "true" : "false")}</Mes_Work_B_State>
                <Robot_Axis J1="{joints[0]}" J2="{joints[1]}" J3="{joints[2]}" J4="{joints[3]}" J5="{joints[4]}" J6="{joints[5]}" />
            </Robot_Receive>
            """);
        var request = Encoding.UTF8.GetBytes(packet);
        var offset = split ? request.Length / 2 : 0;
        if (split)
        {
            await stream.WriteAsync(request.AsMemory(0, offset), cancellation);
            await Task.Delay(25, cancellation);
        }
        await stream.WriteAsync(request.AsMemory(offset), cancellation);

        var response = new StringBuilder();
        var buffer = new byte[1024];
        while (!response.ToString().EndsWith("</Robot_Send>", StringComparison.Ordinal))
        {
            var count = await stream.ReadAsync(buffer, cancellation);
            if (count == 0) throw new IOException("MES 在收到确认前关闭连接。");
            response.Append(Encoding.UTF8.GetString(buffer, 0, count));
        }
        var ack = KUKA_Send_Receive_Xml.String_Xml<Robot_Mes_Info_Data_Send>(response.ToString());
        var pollingMilliseconds = (int)(vm.File_Int_Parameters.Mes_Run_Parameters.File_Save_Cycle_Time * 1000);
        Check(ack.IsStatus && ack.Socket_Polling_Time == pollingMilliseconds, "MES 上报确认帧的状态或轮询周期错误。");
    }

    private static async Task WaitForAsync(Func<bool> condition, CancellationToken cancellation)
    {
        while (!condition()) await Task.Delay(25, cancellation);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
