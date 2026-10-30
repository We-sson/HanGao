using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Automation;

var logPath = "run.log";
void Log(string text) => File.AppendAllText(logPath, text + Environment.NewLine);
File.WriteAllText(logPath, string.Empty);

var exe = @"C:\Users\zhiwei2.he\source\repos\HanGao\RobotSimulator.AnyCAD\bin\Debug\net10.0-windows\win-x64\RobotSimulator.AnyCAD.exe";
var process = Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true })!;
Log($"pid={process.Id}");

AutomationElement? window = null;
for (var i = 0; i < 120 && window is null; i++)
{
    Thread.Sleep(200);
    window = AutomationElement.RootElement.FindFirst(TreeScope.Children,
        new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));
}
if (window is null) { Log("window not found"); return; }
var handle = new IntPtr(window.Current.NativeWindowHandle);
Log($"window='{window.Current.Name}' hwnd={handle}");

for (var wait = 0; wait < 30; wait++)
{
    Thread.Sleep(1000);
    var b = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "重新载入工装"));
    if (b is not null && b.Current.IsEnabled) { Log($"ready after {wait + 1}s"); break; }
}

AutomationElement Find(string name) => window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name))!;
void Click(string name)
{
    var element = Find(name);
    Log($"click '{name}' enabled={element.Current.IsEnabled}");
    ((InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
}

byte[] Capture()
{
    SetForegroundWindow(handle);
    Thread.Sleep(300);
    GetWindowRect(handle, out var rect);
    var width = rect.Right - rect.Left;
    var height = rect.Bottom - rect.Top;
    using var bitmap = new Bitmap(Math.Max(width, 1), Math.Max(height, 1), PixelFormat.Format32bppArgb);
    using (var graphics = Graphics.FromImage(bitmap))
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
    using var small = new Bitmap(bitmap, new Size(64, 64));
    var pixels = new byte[64 * 64];
    for (var y = 0; y < 64; y++)
    for (var x = 0; x < 64; x++)
    {
        var c = small.GetPixel(x, y);
        pixels[y * 64 + x] = (byte)((c.R * 299 + c.G * 587 + c.B * 114) / 1000);
    }
    return pixels;
}

double Difference(byte[] a, byte[] b)
{
    long sum = 0;
    for (var i = 0; i < a.Length; i++) sum += Math.Abs(a[i] - b[i]);
    return sum / (double)a.Length;
}

var h0 = Capture();
Log($"captured start (mean={h0.Average(v => (double)v):F1})");
Click("前视"); Thread.Sleep(2500); var h1 = Capture(); Log($"after 前视 diff={Difference(h0, h1):F2}");
Click("俯视"); Thread.Sleep(2500); var h2 = Capture(); Log($"after 俯视 diff={Difference(h1, h2):F2}");
Click("等轴测"); Thread.Sleep(2500); var h3 = Capture(); Log($"after 等轴测 diff={Difference(h2, h3):F2}");

Click("重新载入工装");
var watch = Stopwatch.StartNew();
for (var i = 0; i < 12; i++)
{
    Thread.Sleep(1000);
    process.Refresh();
    Log($"  loading {watch.Elapsed.TotalSeconds:F0}s responding={process.Responding} ws={process.WorkingSet64 / 1048576}MB");
}
var h4 = Capture(); Log($"after reload diff vs before={Difference(h3, h4):F2}");
Click("前视"); Thread.Sleep(2500); var h5 = Capture(); Log($"post-reload 前视 diff={Difference(h4, h5):F2}");
Click("俯视"); Thread.Sleep(2500); var h6 = Capture(); Log($"post-reload 俯视 diff={Difference(h5, h6):F2}");

try { process.CloseMainWindow(); Thread.Sleep(2000); if (!process.HasExited) process.Kill(); } catch { }

[DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
[DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
[StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
