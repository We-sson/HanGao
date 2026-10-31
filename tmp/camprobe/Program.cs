using System;
using System.Linq;
using System.Reflection;
using AnyCAD.Foundation;

var contextType = typeof(Viewer).GetMethod("GetContext")!.ReturnType;
Console.WriteLine($"Context type: {contextType.FullName}");
foreach (var p in contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
    Console.WriteLine($"P {p.PropertyType.Name} {p.Name}");
foreach (var m in contextType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => m.Name.Contains("Size", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Viewport", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Width", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Height", StringComparison.OrdinalIgnoreCase)))
    Console.WriteLine($"M {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(x => x.ParameterType.Name))})");
