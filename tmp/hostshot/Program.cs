using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

var exe = @"C:\Users\zhiwei2.he\source\repos\HanGao\Robot_Info_Mes\bin\x64\Debug\net10.0-windows10.0.19041.0\Robot_Info_Mes.exe";
var folder = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\hostshot";
var process = Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true })!;
Console.WriteLine($"pid={process.Id}");

var handle = IntPtr.Zero;
for (var attempt = 0; attempt < 120 && handle == IntPtr.Zero; attempt++)
{
    Thread.Sleep(200);
    if (process.HasExited) { Console.WriteLine($"exited before window, code={process.ExitCode}"); return; }
    process.Refresh();
    handle = process.MainWindowHandle;
}
if (handle == IntPtr.Zero) { Console.WriteLine("window not found"); return; }
Console.WriteLine($"hwnd={handle}");

void Capture(string name)
{
    SetForegroundWindow(handle);
    Thread.Sleep(300);
    GetWindowRect(handle, out var rect);
    var width = rect.Right - rect.Left;
    var height = rect.Bottom - rect.Top;
    using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
    using (var graphics = Graphics.FromImage(bitmap))
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
    var path = Path.Combine(folder, name);
    bitmap.Save(path, ImageFormat.Png);
    Console.WriteLine($"saved {name}");
}

Thread.Sleep(600);
Capture("shot-loading.png");
Thread.Sleep(6000);
Capture("shot-loaded.png");

var listening502 = IPGlobalProperties.GetIPGlobalProperties()
    .GetActiveTcpListeners()
    .Any(endpoint => endpoint.Port == 502);
process.Refresh();
Console.WriteLine($"responding={process.Responding} ws={process.WorkingSet64 / 1048576} MB modbus502={listening502}");

process.CloseMainWindow();
var exited = process.WaitForExit(12000);
Console.WriteLine(exited ? $"exited normally, code={process.ExitCode}" : "still running, killing");
if (!exited) process.Kill();

[DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
[DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
[StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
