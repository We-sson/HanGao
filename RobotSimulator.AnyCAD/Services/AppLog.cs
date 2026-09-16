using System.Globalization;
using System.Text;

namespace RobotSimulator.Services;

internal static class AppLog
{
    private static readonly object SyncRoot = new();

    public static string LogPath { get; } = System.IO.Path.Combine(AppContext.BaseDirectory, "robot-simulator.log");

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
