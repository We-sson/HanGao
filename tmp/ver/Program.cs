using System.Diagnostics;

foreach (var path in args)
{
    if (!File.Exists(path)) { Console.WriteLine($"{path}: missing"); continue; }
    var info = FileVersionInfo.GetVersionInfo(path);
    Console.WriteLine($"{path}: file={info.FileVersion} product={info.ProductVersion}");
}
