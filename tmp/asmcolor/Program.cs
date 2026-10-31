using AnyCAD.WPF;

var type = typeof(RenderControl);
Console.WriteLine($"base={type.BaseType?.FullName}");
for (var current = type.BaseType; current is not null; current = current.BaseType)
    Console.WriteLine("  -> " + current.FullName);
