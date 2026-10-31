using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

var exe = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\simclip\bin\RobotSimulator.AnyCAD.exe";
var folder = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\simclip\shot";
var process = Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true })!;
var handle = IntPtr.Zero;
for (var attempt = 0; attempt < 120 && handle == IntPtr.Zero; attempt++)
{
    Thread.Sleep(200);
    if (process.HasExited) { Console.WriteLine($"exited early, code={process.ExitCode}"); return; }
    process.Refresh();
    handle = process.MainWindowHandle;
}
if (handle == IntPtr.Zero) { Console.WriteLine("no window"); return; }
GetWindowRect(handle, out var rect);
Thread.Sleep(9000);
Capture("1.png");
Thread.Sleep(3000);
Capture("2.png");
Thread.Sleep(3000);
Capture("3.png");
process.CloseMainWindow();
if (!process.WaitForExit(15000)) process.Kill();

void Capture(string name)
{
    SetForegroundWindow(handle);
    Thread.Sleep(300);
    using var bitmap = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top, PixelFormat.Format32bppArgb);
    using (var graphics = Graphics.FromImage(bitmap))
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
    bitmap.Save(Path.Combine(folder, name), ImageFormat.Png);
    Console.WriteLine($"saved {name}");
}

[DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
[DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
[StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
