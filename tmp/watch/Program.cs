var before = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
Environment.SetEnvironmentVariable("LOCALAPPDATA", @"C:\Users\zhiwei2.he\source\repos\HanGao\tmp\fresh-localappdata");
var after = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
Console.WriteLine($"before={before}");
Console.WriteLine($"after ={after}");
