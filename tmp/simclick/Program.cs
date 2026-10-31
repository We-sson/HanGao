using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

var exe = @"C:\Users\zhiwei2.he\source\repos\HanGao\RobotSimulator.AnyCAD\bin\Debug\net10.0-windows\win-x64\RobotSimulator.AnyCAD.exe";
var folder = @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\simclick";
var process = Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true })!;
var handle = IntPtr.Zero;
for (var attempt = 0; attempt < 120 && handle == IntPtr.Zero; attempt++)
{
    Thread.Sleep(200);
    if (process.HasExited) { Console.WriteLine("exited early"); return; }
    process.Refresh();
    handle = process.MainWindowHandle;
}
if (handle == IntPtr.Zero) { Console.WriteLine("no window"); return; }
GetWindowRect(handle, out var rect);
Console.WriteLine($"hwnd={handle} rect=({rect.Left},{rect.Top},{rect.Right},{rect.Bottom})");

void Capture(string name)
{
    SetForegroundWindow(handle);
    Thread.Sleep(400);
    using var bitmap = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top, PixelFormat.Format32bppArgb);
    using (var graphics = Graphics.FromImage(bitmap))
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
    bitmap.Save(Path.Combine(folder, name), ImageFormat.Png);
    Console.WriteLine($"saved {name}");
}

void Drag(string name, uint down, uint up, int button)
{
    SetForegroundWindow(handle);
    SetCursorPos(rect.Left + 450, rect.Top + 400);
    Thread.Sleep(200);
    mouse_event(down, 0, 0, 0, UIntPtr.Zero);
    for (var step = 1; step <= 12; step++)
    {
        SetCursorPos(rect.Left + 450 + step * 15, rect.Top + 400 + step * 4);
        Thread.Sleep(30);
    }
    mouse_event(up, 0, 0, 0, UIntPtr.Zero);
    Thread.Sleep(900);
    Capture(name);
}

Thread.Sleep(9000);
Capture("b0-start.png");
Drag("b1-left.png", 0x0002, 0x0004, 0);
Drag("b2-middle.png", 0x0020, 0x0040, 1);
Drag("b3-right.png", 0x0008, 0x0010, 2);
SetForegroundWindow(handle);
SetCursorPos(rect.Left + 600, rect.Top + 400);
Thread.Sleep(200);
mouse_event(0x0800, 0, 0, unchecked((uint)-120), UIntPtr.Zero);
Thread.Sleep(900);
Capture("b4-wheel.png");

process.CloseMainWindow();
if (!process.WaitForExit(15000)) process.Kill();

[DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
[DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
[DllImport("user32.dll")] static extern bool SetCursorPos(int x, int y);
[DllImport("user32.dll")] static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extra);
[StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
