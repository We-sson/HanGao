using System.Buffers.Binary;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Robot_Info_Mes.MotionClient;

internal static class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args is ["--help"] or ["-h"])
        {
            Console.WriteLine("用法：Robot_Info_Mes.MotionClient [端口] [秒数] [MES 地址] [协议: KUKA|ABB]");
            Console.WriteLine("默认连接本机 MES，自动读取配置中的端口与协议（KUKA 或 ABB），发送 20 秒。");
            return 0;
        }

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellation.Cancel();
        };

        try
        {
            var (configPort, configProtocol) = GetLocalMesSettings();
            var port = args.Length > 0 ? int.Parse(args[0], CultureInfo.InvariantCulture) : configPort;
            var seconds = args.Length > 1 ? int.Parse(args[1], CultureInfo.InvariantCulture) : 20;
            var host = args.Length > 2 ? args[2] : "127.0.0.1";
            var protocol = args.Length > 3 ? args[3].Trim().ToUpperInvariant() : configProtocol;
            if (args.Length > 4 || port is < 1 or > 65535 || seconds is < 1 or > 600
                || string.IsNullOrWhiteSpace(host) || protocol is not ("KUKA" or "ABB"))
            {
                throw new ArgumentException("请指定有效端口（1～65535）、测试秒数（1～600）、MES 地址和协议（KUKA 或 ABB）。");
            }

            if (protocol == "ABB")
                await MesJointSender.RunAbbAsync(host, port, TimeSpan.FromSeconds(seconds), Console.WriteLine, cancellation.Token);
            else
                await MesJointSender.RunAsync(host, port, TimeSpan.FromSeconds(seconds), Console.WriteLine, cancellation.Token);

            return 0;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            Console.WriteLine("已停止发送。");
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("连接或等待 MES 确认超时，请检查采集端和端口。");
            return 1;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine($"MES 测试失败：{error.Message}");
            return 1;
        }
    }

    static (int Port, string Protocol) GetLocalMesSettings()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "HanGao", "Robot_Info_Mes", "Configs", "Configs_Data.Xml");
        if (!File.Exists(path)) return (6000, "KUKA");

        var config = XDocument.Load(path).Root;
        var parameters = config?.Element("Mes_Run_Parameters");
        var protocol = ((string?)parameters?.Element("Socket_Robot_Model") ?? "KUKA").Trim().ToUpperInvariant();
        if ((string?)config?.Element("Window_Startup_Type") is "Server"
            || protocol is not ("KUKA" or "ABB"))
            throw new InvalidOperationException("请在 MES 中启用采集端以及 KUKA 或 ABB 机器人通讯。");
        return ((int?)parameters?.Element("Sever_Socket_Port") ?? 6000, protocol);
    }
}

/// <summary>ABB 二进制 MES 报文编码与 8 字节回执读取工具，与 ABB HG_Socket.sys 保持逐字节一致。</summary>
public static class AbbMesFrameCodec
{
    /// <summary>组装 ABB 长度前缀二进制 MES 帧；weldState 为 null 时生成旧版 9 字段帧。</summary>
    public static byte[] BuildMesInfoFrame(
        string mode = "Unknown",
        string programName = "T_ROB1",
        string processName = "R_Side_7",
        bool workA = false,
        bool workB = false,
        bool workC = false,
        bool workD = false,
        bool? weldState = false,
        float[]? joints = null,
        string visionModel = "Mes_Info_Data",
        string robotType = "ABB")
    {
        using var payload = new MemoryStream(160);

        WriteAsciiField(payload, visionModel);
        WriteAsciiField(payload, robotType);
        WriteAsciiField(payload, mode);
        WriteAsciiField(payload, programName);
        WriteAsciiField(payload, workA ? "TRUE" : "FALSE");
        WriteAsciiField(payload, workB ? "TRUE" : "FALSE");
        WriteAsciiField(payload, workC ? "TRUE" : "FALSE");
        WriteAsciiField(payload, workD ? "TRUE" : "FALSE");
        WriteAsciiField(payload, processName);

        if (weldState.HasValue)
        {
            WriteAsciiField(payload, weldState.Value ? "TRUE" : "FALSE");
            if (joints is not null)
            {
                Span<byte> floatBytes = stackalloc byte[4];
                foreach (float angle in joints)
                {
                    BinaryPrimitives.WriteSingleLittleEndian(floatBytes, angle);
                    payload.Write(floatBytes);
                }
            }
        }

        int totalLength = checked((int)payload.Length + 2);
        if (totalLength > ushort.MaxValue)
            throw new InvalidOperationException($"ABB 帧长度超出 UInt16 上限：{totalLength}。");

        byte[] frame = new byte[totalLength];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), (ushort)totalLength);
        payload.Position = 0;
        payload.ReadExactly(frame, 2, (int)payload.Length);
        return frame;
    }

    /// <summary>读取服务端返回的固定 8 字节 ABB 回执（IsStatus DINT + Socket_Polling_Time DINT）。</summary>
    public static async Task<(bool IsStatus, int PollingSeconds)> ReadAckAsync(
        NetworkStream stream, CancellationToken cancellation)
    {
        byte[] ack = new byte[8];
        int received = 0;
        while (received < ack.Length)
        {
            int count = await stream.ReadAsync(ack.AsMemory(received, ack.Length - received), cancellation);
            if (count == 0) throw new IOException("MES 在返回 ABB 确认前关闭了连接。");
            received += count;
        }

        int status = BinaryPrimitives.ReadInt32LittleEndian(ack.AsSpan(0, 4));
        int pollingSeconds = BinaryPrimitives.ReadInt32LittleEndian(ack.AsSpan(4, 4));
        return (status == 1, pollingSeconds);
    }

    private static void WriteAsciiField(Stream target, string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        Span<byte> lengthBytes = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(lengthBytes, checked((ushort)bytes.Length));
        target.Write(lengthBytes);
        if (bytes.Length > 0) target.Write(bytes);
    }
}

/// <summary>用 KUKA XML 或 ABB 二进制报文持续上报六轴，仅用于测试 MES 三维跟随。</summary>
public static class MesJointSender
{
    /// <summary>根据各自零位参考和轴向符号生成一致物理摆动轨迹的 J1～J6 测试角度。</summary>
    private static double[] ComputeDemoJoints(double elapsedSeconds, bool isAbb)
    {
        double wave = Math.Sin(elapsedSeconds * Math.Tau / 8);
        return isAbb
            // ABB 零位即 L 型 [0,0,0,0,0,0] 且 J1/J4/J6 正方向与 KUKA 相反。
            ? [-45 * wave, 30 + 15 * wave, -35 + 15 * wave, -30 * wave, 25 * wave, -60 * wave]
            : [45 * wave, -60 + 15 * wave, 55 + 15 * wave, 30 * wave, 25 * wave, 60 * wave];
    }

    public static async Task<int> RunAsync(
        string host, int port, TimeSpan duration, Action<string> report, CancellationToken cancellation)
    {
        using var client = new TcpClient { NoDelay = true };
        using (var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
        {
            deadline.CancelAfter(TimeSpan.FromSeconds(3));
            await client.ConnectAsync(host, port, deadline.Token);
        }
        using var stream = client.GetStream();
        report($"已连接 {host}:{port}（KUKA XML）。请打开 MES 的「三维仿真」页。");
        const string endTag = "</Robot_Send>";
        var replyBuffer = new byte[8192];
        var watch = Stopwatch.StartNew();
        var confirmed = 0;
        var lastSecond = -1;
        while (watch.Elapsed < duration)
        {
            var joints = ComputeDemoJoints(watch.Elapsed.TotalSeconds, isAbb: false);
            var j1 = joints[0];
            var j2 = joints[1];
            var j3 = joints[2];
            var j4 = joints[3];
            var j5 = joints[4];
            var j6 = joints[5];
            var packet = FormattableString.Invariant($"""
                <Robot_Receive Robot_Type="KUKA" Vision_Model="Mes_Info_Data">
                    <Mes_Programs_Name>MES_MOTION_TEST</Mes_Programs_Name>
                    <Mes_Robot_Mode>Unknown</Mes_Robot_Mode>
                    <Robot_Axis J1="{j1:F2}" J2="{j2:F2}" J3="{j3:F2}" J4="{j4:F2}" J5="{j5:F2}" J6="{j6:F2}" />
                </Robot_Receive>
                """);

            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            deadline.CancelAfter(TimeSpan.FromSeconds(3));
            await stream.WriteAsync(Encoding.UTF8.GetBytes(packet), deadline.Token);
            var received = 0;
            while (true)
            {
                var count = await stream.ReadAsync(replyBuffer.AsMemory(received), deadline.Token);
                if (count == 0) throw new IOException("MES 在返回确认前关闭了连接。");
                received += count;
                var reply = Encoding.UTF8.GetString(replyBuffer, 0, received);
                var end = reply.IndexOf(endTag, StringComparison.Ordinal);
                if (end >= 0)
                {
                    var ack = XElement.Parse(reply[..(end + endTag.Length)].TrimStart('\uFEFF'));
                    if (ack.Name.LocalName != "Robot_Send" || (bool?)ack.Attribute("IsStatus") != true)
                        throw new IOException("MES 没有确认该角度报文。");
                    break;
                }
                if (received == replyBuffer.Length) throw new IOException("MES 没有返回有效的 Robot_Send。");
            }

            confirmed++;
            var second = (int)watch.Elapsed.TotalSeconds;
            if (second != lastSecond)
            {
                lastSecond = second;
                report(FormattableString.Invariant($"MES 已确认 {confirmed} 帧 | J1={j1:F1} J2={j2:F1} J3={j3:F1} J4={j4:F1} J5={j5:F1} J6={j6:F1}"));
            }
            await Task.Delay(100, cancellation);
        }
        report($"测试结束，MES 共确认 {confirmed} 帧。");
        return confirmed;
    }

    /// <summary>用 ABB 二进制报文持续上报六轴，测试 MES ABB 接收链路与三维跟随。</summary>
    public static async Task<int> RunAbbAsync(
        string host, int port, TimeSpan duration, Action<string> report, CancellationToken cancellation)
    {
        using var client = new TcpClient { NoDelay = true };
        using (var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
        {
            deadline.CancelAfter(TimeSpan.FromSeconds(3));
            await client.ConnectAsync(host, port, deadline.Token);
        }
        using var stream = client.GetStream();
        report($"已连接 {host}:{port}（ABB 二进制）。请打开 MES 的「三维仿真」页。");
        var watch = Stopwatch.StartNew();
        var confirmed = 0;
        var lastSecond = -1;
        while (watch.Elapsed < duration)
        {
            var joints = ComputeDemoJoints(watch.Elapsed.TotalSeconds, isAbb: true);
            var j1 = (float)joints[0];
            var j2 = (float)joints[1];
            var j3 = (float)joints[2];
            var j4 = (float)joints[3];
            var j5 = (float)joints[4];
            var j6 = (float)joints[5];
            byte[] packet = AbbMesFrameCodec.BuildMesInfoFrame(
                mode: "Unknown",
                programName: "T_ROB1",
                processName: "R_Side_7",
                weldState: false,
                joints: [j1, j2, j3, j4, j5, j6]);

            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            deadline.CancelAfter(TimeSpan.FromSeconds(3));
            await stream.WriteAsync(packet, deadline.Token);
            var (isStatus, _) = await AbbMesFrameCodec.ReadAckAsync(stream, deadline.Token);
            if (!isStatus) throw new IOException("MES 没有确认该 ABB 角度报文。");

            confirmed++;
            var second = (int)watch.Elapsed.TotalSeconds;
            if (second != lastSecond)
            {
                lastSecond = second;
                report(FormattableString.Invariant($"MES 已确认 ABB {confirmed} 帧 | J1={j1:F1} J2={j2:F1} J3={j3:F1} J4={j4:F1} J5={j5:F1} J6={j6:F1}"));
            }
            await Task.Delay(100, cancellation);
        }
        report($"ABB 测试结束，MES 共确认 {confirmed} 帧。");
        return confirmed;
    }
}
