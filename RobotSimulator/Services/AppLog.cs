using System.Globalization;
using System.Text;

namespace RobotSimulator.Services;

/// <summary>记录模型加载、缓存和异常信息，便于定位启动失败或加载缓慢的阶段。</summary>
internal static class AppLog
{
    /// <summary>串行化 UI 线程和后台加载线程的写入，防止同时占用日志文件。</summary>
    private static readonly object SyncRoot = new();

    /// <summary>日志位于程序目录，文件名为 robot-simulator.log。</summary>
    public static string LogPath { get; } = System.IO.Path.Combine(AppContext.BaseDirectory, "robot-simulator.log");

    /// <summary>追加一条带毫秒时间的 UTF-8 日志；记录失败不阻断正常仿真。</summary>
    public static void Write(string message)
    {
        try
        {
            var line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}  {message}{Environment.NewLine}";
            lock (SyncRoot)
            {
                System.IO.File.AppendAllText(LogPath, line, Encoding.UTF8);
            }
        }
        catch
        {
            // 日志不能影响仿真程序运行。
        }
    }
}
