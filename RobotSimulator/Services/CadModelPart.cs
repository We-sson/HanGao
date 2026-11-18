using System;
using System.IO;
using Macad.Occt;

namespace RobotSimulator.Services;

/// <summary>
/// 一个已解析的模型部件：BREP 形状、逐面颜色缓存和局部包围盒。
/// 读取在后台线程完成，AIS 显示对象由 <see cref="RobotSceneContent"/> 在 UI 线程创建。
/// </summary>
internal sealed class CadModelPart
{
    CadModelPart(string name, CadDisplayPackage package, Bnd_Box bounds)
    {
        Name = name;
        Package = package;
        Bounds = bounds;
    }

    public string Name { get; }

    public CadDisplayPackage Package { get; }

    public Bnd_Box Bounds { get; }

    public string LoadMessage => $"{Name}：{Package.FaceCount} 个面";

    public static CadModelPart Load(string name, Stream source)
    {
        var package = CadDisplayPackage.Read(source);
        var bounds = new Bnd_Box();
        BRepBndLib.Add(package.Shape, bounds);
        return new CadModelPart(name, package, bounds);
    }
}
