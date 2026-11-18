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
            Console.WriteLine("用法：Robot_Info_Mes.MotionClient [端口] [秒数] [MES 地址]");
            Console.WriteLine("默认连接本机 MES，使用配置中的机器人接收端口（通常为 6000），发送 20 秒。");
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
            var host = args.Length > 2 ? args[2] : "127.0.0.1";
            var port = args.Length > 0 ? int.Parse(args[0], CultureInfo.InvariantCulture) : GetLocalMesPort();
            var seconds = args.Length > 1 ? int.Parse(args[1], CultureInfo.InvariantCulture) : 20;
            if (args.Length > 3 || port is < 1 or > 65535 || seconds is < 1 or > 600 || string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("请指定有效端口（1～65535）、测试秒数（1～600）和 MES 地址。");

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

    static int GetLocalMesPort()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "HanGao", "Robot_Info_Mes", "Configs", "Configs_Data.Xml");
        if (!File.Exists(path)) return 6000;

        var config = XDocument.Load(path).Root;
        var parameters = config?.Element("Mes_Run_Parameters");
        if ((string?)config?.Element("Window_Startup_Type") is "Server"
            || (string?)parameters?.Element("Socket_Robot_Model") is string protocol && protocol != "KUKA")
            throw new InvalidOperationException("请在 MES 中启用采集端和 KUKA 机器人通讯。");
        return (int?)parameters?.Element("Sever_Socket_Port") ?? 6000;
    }
}

/// <summary>用 KUKA XML 持续上报六轴，仅用于测试 MES 三维跟随。</summary>
public static class MesJointSender
{
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
        report($"已连接 {host}:{port}。请打开 MES 的「三维仿真」页。");
        const string endTag = "</Robot_Send>";
        var replyBuffer = new byte[8192];
        var watch = Stopwatch.StartNew();
        var confirmed = 0;
        var lastSecond = -1;
        while (watch.Elapsed < duration)
        {
            // 用五种内置型号通用的行程，平缓地往复运动。
            var wave = Math.Sin(watch.Elapsed.TotalSeconds * Math.Tau / 8);
            var j1 = 45 * wave;
            var j2 = -60 + 15 * wave;
            var j3 = 55 + 15 * wave;
            var j4 = 30 * wave;
            var j5 = 25 * wave;
            var j6 = 60 * wave;
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
}
