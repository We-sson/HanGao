using System.Reflection;
using AnyCAD.Foundation;

internal static class Reflect
{
    public static void Dump()
    {
        var asm = typeof(TopoShape).Assembly;
        Console.WriteLine("== types with Mesh/Triang in name ==");
        foreach (var t in asm.GetTypes().Where(t => t.Name.Contains("Mesh", StringComparison.OrdinalIgnoreCase)
                                                  || t.Name.Contains("Triang", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(t => t.FullName))
            Console.WriteLine("  " + t.FullName);
        var mesh = asm.GetTypes().FirstOrDefault(t => t.Name == "MeshData" || t.Name == "TriangleMesh" || t.Name == "Triangulation");
        if (mesh is not null)
        {
            Console.WriteLine($"== {mesh.FullName} members ==");
            foreach (var m in mesh.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                Console.WriteLine("  " + m);
        }
        var tools = asm.GetTypes().FirstOrDefault(t => t.Name == "ShapeTools");
        if (tools is not null)
        {
            Console.WriteLine("== ShapeTools triangulation members ==");
            foreach (var m in tools.GetMethods(BindingFlags.Public | BindingFlags.Static)
                         .Where(m => m.Name.Contains("Triang", StringComparison.OrdinalIgnoreCase)
                                  || m.Name.Contains("Mesh", StringComparison.OrdinalIgnoreCase)))
                Console.WriteLine("  " + m);
        }
    }
}
