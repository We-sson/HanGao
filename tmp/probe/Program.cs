using System.Diagnostics;
using System.IO;
using AnyCAD.Foundation;

GlobalInstance.Initialize(true);

var cacheDir = args.Length > 0
    ? args[0]
    : @"C:\Users\zhiwei2.he\AppData\Local\RobotSimulator.AnyCAD\ModelCache\Work-B4E5A0364CFFC60892B9CCB920169E07012813525A196458A4E604C47847E514-v7-solid-colors-1mm";
var deflection = args.Length > 1 ? double.Parse(args[1]) : 1.0;
var files = Directory.GetFiles(cacheDir, "color-*.brep")
    .OrderBy(f => int.Parse(System.IO.Path.GetFileNameWithoutExtension(f).Split('-')[1])).ToArray();
var process = Process.GetCurrentProcess();
Console.WriteLine($"deflection={deflection} mm, files={files.Length}, workingSetBefore={process.WorkingSet64 / 1048576} MB");

var nodes = new List<BrepSceneNode>();
long totalFaces = 0;
var total = Stopwatch.StartNew();
foreach (var file in files)
{
    var shape = ShapeIO.Open(file) ?? throw new InvalidOperationException($"Open failed: {file}");
    var sw = Stopwatch.StartNew();
    var node = BrepSceneNode.Create(shape, deflection, true) ?? throw new InvalidOperationException($"Create failed: {file}");
    sw.Stop();
    nodes.Add(node);
    totalFaces += node.GetFaceCount();
    Console.WriteLine($"{System.IO.Path.GetFileName(file),-18} {new FileInfo(file).Length / 1024.0,9:F0} KB  faces={node.GetFaceCount(),7}  tessellate={sw.ElapsedMilliseconds,6} ms");
}
total.Stop();
process.Refresh();
Console.WriteLine($"TOTAL faces={totalFaces} elapsed={total.ElapsedMilliseconds} ms workingSetAfter={process.WorkingSet64 / 1048576} MB private={process.PrivateMemorySize64 / 1048576} MB nodes={nodes.Count}");
