using System.Drawing;

var path = args.Length > 0 ? args[0] : @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\simclick\b0-start.png";
using var bitmap = new Bitmap(path);
int vLeft = int.MaxValue, vTop = int.MaxValue, vRight = -1, vBottom = -1;
for (var y = 0; y < bitmap.Height; y++)
{
    for (var x = 0; x < bitmap.Width; x++)
    {
        var c = bitmap.GetPixel(x, y);
        var brightness = (c.R + c.G + c.B) / 3;
        if (c.B - c.R > 15 && brightness > 150)
        {
            vLeft = Math.Min(vLeft, x); vTop = Math.Min(vTop, y);
            vRight = Math.Max(vRight, x); vBottom = Math.Max(vBottom, y);
        }
    }
}
Console.WriteLine($"viewport=({vLeft},{vTop})-({vRight},{vBottom})");
int minX = int.MaxValue, minY = int.MaxValue, maxX = -1, maxY = -1;
for (var y = vTop + 4; y < vBottom - 4; y++)
{
    for (var x = vLeft + 4; x < vRight - 4; x++)
    {
        if (x > vRight - 110 && y < vTop + 110) continue; // 视图立方体
        var c = bitmap.GetPixel(x, y);
        var brightness = (c.R + c.G + c.B) / 3;
        var saturation = Math.Max(c.R, Math.Max(c.G, c.B)) - Math.Min(c.R, Math.Min(c.G, c.B));
        if (brightness < 140 || saturation > 60)
        {
            minX = Math.Min(minX, x); minY = Math.Min(minY, y);
            maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
        }
    }
}
var w = maxX - minX + 1; var h = maxY - minY + 1;
var vw = vRight - vLeft + 1; var vh = vBottom - vTop + 1;
Console.WriteLine($"model=({minX},{minY})-({maxX},{maxY}) size={w}x{h} width%={w * 100.0 / vw:F1} height%={h * 100.0 / vh:F1}");
