using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Roboto_Socket_Library;
using Roboto_Socket_Library.Model;
using static Roboto_Socket_Library.Model.Roboto_Socket_Model;

/// <summary>验证停止监听同时关闭旧连接，以及同一监听对象重启后的容量和收包。</summary>
internal static class SocketLifecycleChecks
{
    public static async Task RunAsync()
    {
        foreach (var streaming in new[] { true, false })
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var cancellation = timeout.Token;
            int callbacks = 0;
            var errors = new ConcurrentQueue<string>();
            var server = new Socket_Receive
            {
                Socket_Robot = Socket_Robot_Protocols_Enum.KUKA,
                UseXmlFragmentReceive = streaming,
                MaxServerConnections = 1,
                Robot_Info_Model_Data_Delegate = (_, _) =>
                {
                    Interlocked.Increment(ref callbacks);
                    return new Robot_Mes_Info_Data_Send { IsStatus = true };
                },
                Socket_ErrorInfo_delegate = (message, _) => errors.Enqueue(message)
            };
            server.Server_Strat("127.0.0.1", "0");
            Check(server.ServerStarted, "生命周期测试监听启动失败。");
            int port = ((IPEndPoint)server.Socket_Sever!.LocalEndPoint!).Port;

            try
            {
                using var oldClient = await ConnectAsync(port, cancellation);
                var oldStream = oldClient.GetStream();
                await SendAsync(oldStream, cancellation);
                using (var excess = await ConnectAsync(port, cancellation))
                    await CheckClosedAsync(excess.GetStream(), cancellation);
                Check(callbacks == 1 && errors.Any(error => error.StartsWith("Error:-17", StringComparison.Ordinal)),
                    "达到连接上限后，新连接仍进入业务处理。");

                server.Sever_End();
                server.Sever_End();
                Check(!server.ServerStarted && server.Socket_Sever is null, "停止后监听状态未清空。");
                await CheckClosedAsync(oldStream, cancellation);
                Check(callbacks == 1, "停止后旧连接仍更新业务。");

                server.Server_Strat("127.0.0.1", port.ToString(System.Globalization.CultureInfo.InvariantCulture));
                Check(server.ServerStarted, "同一对象停止后无法重新启动。");
                using var restarted = await ConnectAsync(port, cancellation);
                var restartedStream = restarted.GetStream();
                await SendAsync(restartedStream, cancellation);
                Check(callbacks == 2, "旧连接计数占用了重启后的唯一连接名额。");
                using (var excess = await ConnectAsync(port, cancellation))
                    await CheckClosedAsync(excess.GetStream(), cancellation);

                // 再次启动会关闭前一轮连接，前一轮异步清理不得释放新一轮的名额。
                server.Server_Strat("127.0.0.1", port.ToString(System.Globalization.CultureInfo.InvariantCulture));
                await CheckClosedAsync(restartedStream, cancellation);
                using var latest = await ConnectAsync(port, cancellation);
                await SendAsync(latest.GetStream(), cancellation);
                using (var excess = await ConnectAsync(port, cancellation))
                    await CheckClosedAsync(excess.GetStream(), cancellation);
                Check(callbacks == 3, "重启后连接上限计数或业务调用次数错误。");

                latest.Close();
                while (!errors.Any(error => error.StartsWith("Error:-9", StringComparison.Ordinal)))
                    await Task.Delay(10, cancellation);
                using var replacement = await ConnectAsync(port, cancellation);
                await SendAsync(replacement.GetStream(), cancellation);
                using (var excess = await ConnectAsync(port, cancellation))
                    await CheckClosedAsync(excess.GetStream(), cancellation);
                Check(callbacks == 4, "正常断线后未正确回收唯一连接名额。");
            }
            finally
            {
                server.Sever_End();
            }
        }
        Console.WriteLine("PASS: XML and fixed-buffer listener stop, accepted connection cleanup, restart and connection limit");
    }

    private static async Task<TcpClient> ConnectAsync(int port, CancellationToken cancellation)
    {
        var client = new TcpClient { NoDelay = true };
        try { await client.ConnectAsync(IPAddress.Loopback, port, cancellation); }
        catch { client.Dispose(); throw; }
        return client;
    }

    private static async Task SendAsync(NetworkStream stream, CancellationToken cancellation)
    {
        const string frame = "<Robot_Receive Robot_Type=\"KUKA\" Vision_Model=\"Mes_Info_Data\" />";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(frame), cancellation);
        var response = new StringBuilder();
        var bytes = new byte[1024];
        while (!response.ToString().Contains("</Robot_Send>", StringComparison.Ordinal))
        {
            int length = await stream.ReadAsync(bytes, cancellation);
            Check(length > 0, "有效连接在应答前被关闭。");
            response.Append(Encoding.UTF8.GetString(bytes, 0, length));
        }
        Check(KUKA_Send_Receive_Xml.String_Xml<Robot_Mes_Info_Data_Send>(response.ToString()).IsStatus,
            "有效连接未得到成功应答。");
    }

    private static async Task CheckClosedAsync(NetworkStream stream, CancellationToken cancellation)
    {
        try { Check(await stream.ReadAsync(new byte[64], cancellation) == 0, "被关闭的连接仍能收到业务数据。"); }
        catch (IOException) { }
    }

    private static void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
    }
}
