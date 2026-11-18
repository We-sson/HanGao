using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Macad.Occt;
using RobotSimulator.Models;
using RobotSimulator.ViewModels;

namespace RobotSimulator.Services;

/// <summary>
/// 三维场景内容：Base/J1～J6、Tool、Work 的 BREP 直接用 AIS 显示对象表示。
/// AIS 没有场景节点树，因此这里显式累积绝对世界变换：
/// 关节世界 = 前置关节旋转连乘，法兰 = 关节世界 × 法兰位姿，Tool 挂法兰，TCP/焊接光挂法兰×TCP。
/// </summary>
internal sealed class RobotSceneContent : IDisposable
{
    const int JointCount = 6;
    const double BaseAxisLength = 64.0;
    const double FlangeAxisLength = 48.0;
    const double TcpAxisLength = 64.0;

    readonly AIS_InteractiveContext _context;
    readonly CadModelPart?[] _parts = new CadModelPart?[JointCount + 1];
    readonly AIS_ColoredShape?[] _partDisplays = new AIS_ColoredShape?[JointCount + 1];
    readonly Trsf[] _partWorlds = new Trsf[JointCount + 1];

    readonly AIS_Trihedron _baseAxes;
    readonly AIS_Trihedron _flangeAxes;
    readonly AIS_Trihedron _tcpAxes;
    readonly AIS_ColoredShape _weldSpot;
    readonly Trsf _flangePose;

    CadModelPart? _toolPart;
    CadModelPart? _workPart;
    AIS_ColoredShape? _toolDisplay;
    AIS_ColoredShape? _workDisplay;

    IReadOnlyList<JointViewModel>? _joints;
    Trsf _tcpPose = Trsf.Identity;
    Trsf _workPose = Trsf.Identity;
    Trsf _flangeWorld = Trsf.Identity;
    Trsf _tcpWorld = Trsf.Identity;

    bool _baseAxesVisible = true;
    bool _flangeAxesVisible = true;
    bool _tcpAxesVisible = true;
    bool _weldVisible;
    double _unitsPerPixel = 1.0;
    double _appliedAxisScale = double.NaN;
    bool _disposed;

    RobotSceneContent(AIS_InteractiveContext context, IReadOnlyList<CadModelPart> parts, RobotConfiguration configuration)
    {
        _context = context;
        var origin = configuration.FlangeOrigin;
        var orientation = configuration.FlangeOrientation;
        _flangePose = PoseTransform(new PoseConfiguration
        {
            X = origin.X,
            Y = origin.Y,
            Z = origin.Z,
            A = orientation.A,
            B = orientation.B,
            C = orientation.C
        });

        for (var i = 0; i < _parts.Length && i < parts.Count; i++) _parts[i] = parts[i];

        for (var i = 0; i < _parts.Length; i++)
        {
            if (_parts[i] is null) continue;
            _partDisplays[i] = CreatePartDisplay(_parts[i]!);
            _context.Display(_partDisplays[i]!, 1, -1, false);
        }

        _baseAxes = CreateAxes(BaseAxisLength);
        _flangeAxes = CreateAxes(FlangeAxisLength);
        _tcpAxes = CreateAxes(TcpAxisLength);
        _weldSpot = CreateWeldSpot();

        _context.Display(_baseAxes, 1, -1, false);
        _context.Display(_flangeAxes, 1, -1, false);
        _context.Display(_tcpAxes, 1, -1, false);
        _context.Display(_weldSpot, 1, -1, false);
        _context.Erase(_weldSpot, false);

        ApplyTransforms();
    }

    /// <summary>后台线程：读取 Base + J1～J6 的 .cad 显示缓存。</summary>
    internal static IReadOnlyList<CadModelPart> LoadParts(
        RobotConfiguration configuration,
        string modelId,
        IProgress<RobotLoadProgress>? progress)
    {
        var files = new string[JointCount + 1];
        files[0] = configuration.BaseFile;
        for (var i = 0; i < JointCount; i++) files[i + 1] = configuration.Joints[i].ModelFile;

        var parts = new CadModelPart[files.Length];
        var completed = 0;
        var sync = new object();
        Parallel.For(0, files.Length, new ParallelOptions { MaxDegreeOfParallelism = 4 }, index =>
        {
            using var source = ModelResources.OpenRobot(modelId, files[index]);
            var name = index == 0 ? "Base" : configuration.Joints[index - 1].Name;
            var part = CadModelPart.Load(name, source);
            parts[index] = part;
            lock (sync)
            {
                completed++;
                progress?.Report(new RobotLoadProgress(completed, files.Length, part.LoadMessage));
            }
        });
        return parts;
    }

    /// <summary>UI 线程：把已解析的部件包成 AIS 显示对象。</summary>
    internal static RobotSceneContent Create(
        AIS_InteractiveContext context, IReadOnlyList<CadModelPart> parts, RobotConfiguration configuration)
    {
        if (parts.Count != JointCount + 1)
            throw new ArgumentException("机器人模型需要 Base + J1～J6 共 7 个部件。", nameof(parts));
        return new RobotSceneContent(context, parts, configuration);
    }

    internal bool HasTool => _toolPart is not null;

    internal bool HasWork => _workPart is not null;

    internal void SetJoints(IReadOnlyList<JointViewModel> joints)
    {
        _joints = joints;
        ApplyTransforms();
    }

    internal void ReplaceTool(CadModelPart part)
    {
        if (_toolDisplay is not null) _context.Remove(_toolDisplay, false);
        _toolPart = part;
        _toolDisplay = CreatePartDisplay(part);
        _context.Display(_toolDisplay, 1, -1, false);
        _toolDisplay.SetLocalTransformation(_flangeWorld);
    }

    internal void ReplaceWork(CadModelPart part)
    {
        if (_workDisplay is not null) _context.Remove(_workDisplay, false);
        _workPart = part;
        _workDisplay = CreatePartDisplay(part);
        _context.Display(_workDisplay, 1, -1, false);
        _workDisplay.SetLocalTransformation(_workPose);
    }

    /// <summary>换机器人时把 Tool/Work 及其位姿移交到新场景，旧场景只负责移除自己的显示对象。</summary>
    internal void AdoptAttachmentsTo(RobotSceneContent target)
    {
        if (_toolPart is not null)
        {
            target._toolPart = _toolPart;
            _toolPart = null;
        }
        target._tcpPose = _tcpPose;

        if (_workPart is not null)
        {
            target._workPart = _workPart;
            _workPart = null;
        }
        target._workPose = _workPose;

        target.CreateAttachments();
    }

    void CreateAttachments()
    {
        if (_toolPart is not null && _toolDisplay is null)
        {
            _toolDisplay = CreatePartDisplay(_toolPart);
            _context.Display(_toolDisplay, 1, -1, false);
        }
        if (_workPart is not null && _workDisplay is null)
        {
            _workDisplay = CreatePartDisplay(_workPart);
            _context.Display(_workDisplay, 1, -1, false);
        }
        ApplyTransforms();
    }

    internal void SetToolTcp(PoseConfiguration pose)
    {
        _tcpPose = PoseTransform(pose);
        ApplyTransforms();
    }

    internal void SetWorkPose(PoseConfiguration pose)
    {
        _workPose = PoseTransform(pose);
        ApplyTransforms();
    }

    internal void SetCoordinateVisibility(bool baseAxes, bool flangeAxes, bool tcpAxes)
    {
        SetVisible(_baseAxes, ref _baseAxesVisible, baseAxes);
        SetVisible(_flangeAxes, ref _flangeAxesVisible, flangeAxes);
        SetVisible(_tcpAxes, ref _tcpAxesVisible, tcpAxes);
    }

    internal void SetWeldBeamVisible(bool visible)
    {
        SetVisible(_weldSpot, ref _weldVisible, visible);
    }

    internal void UpdateCoordinateScale(double unitsPerPixel)
    {
        if (!double.IsFinite(unitsPerPixel) || unitsPerPixel <= 0) return;
        _unitsPerPixel = unitsPerPixel;
        ApplyAxisScale();
    }

    /// <summary>模型包围盒（不含坐标轴与焊接光），用于相机适配。</summary>
    internal Bnd_Box GetModelBounds()
    {
        var box = new Bnd_Box();
        for (var i = 0; i < _parts.Length; i++)
        {
            if (_parts[i] is null) continue;
            AddWorldBox(box, _parts[i]!.Bounds, _partWorlds[i]);
        }
        if (_toolPart is not null) AddWorldBox(box, _toolPart.Bounds, _flangeWorld);
        if (_workPart is not null) AddWorldBox(box, _workPart.Bounds, _workPose);
        return box;
    }

    void ApplyTransforms()
    {
        UpdateWorlds();
        for (var i = 0; i < _partDisplays.Length; i++)
            _partDisplays[i]?.SetLocalTransformation(_partWorlds[i]);

        _baseAxes.SetLocalTransformation(Trsf.Identity);
        _flangeAxes.SetLocalTransformation(_flangeWorld);
        _tcpAxes.SetLocalTransformation(_tcpWorld);
        _weldSpot.SetLocalTransformation(_tcpWorld);
        _toolDisplay?.SetLocalTransformation(_flangeWorld);
        _workDisplay?.SetLocalTransformation(_workPose);
        ApplyAxisScale();
    }

    void UpdateWorlds()
    {
        var jointWorld = Trsf.Identity;
        _partWorlds[0] = Trsf.Identity;
        for (var i = 0; i < JointCount; i++)
        {
            if (_joints is { Count: >= JointCount })
                jointWorld = jointWorld.Multiplied(JointTransform(_joints[i]));
            _partWorlds[i + 1] = jointWorld;
        }
        _flangeWorld = jointWorld.Multiplied(_flangePose);
        _tcpWorld = _flangeWorld.Multiplied(_tcpPose);
    }

    void ApplyAxisScale()
    {
        var scale = _unitsPerPixel;
        if (!double.IsFinite(scale) || scale <= 0) scale = 1.0;
        if (Math.Abs(scale - _appliedAxisScale) < 1e-12) return;
        _appliedAxisScale = scale;
        _baseAxes.SetSize(BaseAxisLength * scale);
        _flangeAxes.SetSize(FlangeAxisLength * scale);
        _tcpAxes.SetSize(TcpAxisLength * scale);
        if (_baseAxesVisible) _context.Redisplay(_baseAxes, false);
        if (_flangeAxesVisible) _context.Redisplay(_flangeAxes, false);
        if (_tcpAxesVisible) _context.Redisplay(_tcpAxes, false);
    }

    void SetVisible(AIS_InteractiveObject obj, ref bool state, bool visible)
    {
        if (state == visible) return;
        state = visible;
        if (visible) _context.Display(obj, 1, -1, false);
        else _context.Erase(obj, false);
    }

    static void AddWorldBox(Bnd_Box target, Bnd_Box source, Trsf world)
    {
        if (source.IsVoid()) return;
        var min = source.CornerMin();
        var max = source.CornerMax();
        for (var ix = 0; ix < 2; ix++)
        for (var iy = 0; iy < 2; iy++)
        for (var iz = 0; iz < 2; iz++)
        {
            var point = new Pnt(ix == 0 ? min.X : max.X, iy == 0 ? min.Y : max.Y, iz == 0 ? min.Z : max.Z);
            target.Add(point.Transformed(world));
        }
    }

    static AIS_ColoredShape CreatePartDisplay(CadModelPart part)
    {
        var faces = new TopTools_IndexedMapOfShape();
        TopExp.MapShapes(part.Package.Shape, TopAbs_ShapeEnum.FACE, faces);

        var boundary = new Prs3d_LineAspect(CadColors.Srgb(41, 45, 51), Aspect_TypeOfLine.SOLID, 1.0);
        var display = new AIS_ColoredShape(part.Package.Shape);
        display.SetMaterial(new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial.Plastified));
        display.SetColor(CadColors.Srgb(255, 255, 255));
        display.Attributes().SetFaceBoundaryAspect(boundary);
        display.Attributes().SetFaceBoundaryDraw(true);

        var builder = new BRep_Builder();
        var groups = new Dictionary<int, TopoDS_Compound>();
        var faceCount = Math.Min(faces.Size(), part.Package.FaceCount);
        for (var face = 1; face <= faceCount; face++)
        {
            var slot = part.Package.ColorSlot(face);
            if (!groups.TryGetValue(slot, out var compound))
            {
                compound = new TopoDS_Compound();
                builder.MakeCompound(compound);
                groups[slot] = compound;
            }
            builder.Add(compound, faces.FindKey(face));
        }

        foreach (var group in groups)
        {
            var color = part.Package.PaletteColor(group.Key);
            display.SetCustomColor(group.Value, color);
            display.CustomAspects(group.Value).SetFaceBoundaryAspect(boundary);
        }
        return display;
    }

    static AIS_Trihedron CreateAxes(double size)
    {
        var trihedron = new AIS_Trihedron(new Geom_Axis2Placement(Pnt.Origin, new Dir(0, 0, 1), new Dir(1, 0, 0)));
        trihedron.SetDatumDisplayMode(Prs3d_DatumMode.Shaded);
        trihedron.SetDrawArrows(true);
        trihedron.Attributes().DatumAspect().SetDrawLabels(false);
        trihedron.SetSize(size);
        trihedron.SetXAxisColor(CadColors.Srgb(255, 26, 26));
        trihedron.SetYAxisColor(CadColors.Srgb(26, 204, 26));
        trihedron.SetAxisColor(CadColors.Srgb(31, 107, 255));
        trihedron.SetOriginColor(CadColors.Srgb(230, 230, 230));
        return trihedron;
    }

    /// <summary>焊接光：橙色半透明锥 + 前端光晕球。</summary>
    static AIS_ColoredShape CreateWeldSpot()
    {
        var builder = new BRep_Builder();
        var compound = new TopoDS_Compound();
        builder.MakeCompound(compound);

        var cone = new BRepPrimAPI_MakeCone(2.0, 0.0, 50.0).Shape();
        var glow = new BRepPrimAPI_MakeSphere(new Pnt(0, 0, 50), 8.0).Shape();
        builder.Add(compound, cone);
        builder.Add(compound, glow);

        var display = new AIS_ColoredShape(compound);
        display.SetMaterial(new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial.Plastified));
        display.SetTransparency(0.35);
        display.SetCustomColor(cone, CadColors.Srgb(255, 89, 31));
        display.SetCustomColor(glow, CadColors.Srgb(255, 140, 26));
        return display;
    }

    /// <summary>关节旋转变换：绕轴心旋转 (当前角 - 模型参考角)。</summary>
    internal static Trsf JointTransform(JointViewModel joint)
    {
        var lengthSquared = joint.AxisDirectionX * joint.AxisDirectionX
                            + joint.AxisDirectionY * joint.AxisDirectionY
                            + joint.AxisDirectionZ * joint.AxisDirectionZ;
        if (lengthSquared < 1e-18) return Trsf.Identity;

        var origin = new Pnt(joint.AxisOriginX, joint.AxisOriginY, joint.AxisOriginZ);
        var direction = new Dir(joint.AxisDirectionX, joint.AxisDirectionY, joint.AxisDirectionZ);
        var angle = (joint.Angle - joint.ModelReferenceAngle) * Math.PI / 180.0;
        return new Trsf(new Ax1(origin, direction), angle);
    }

    /// <summary>姿态变换：与迁移前一致，世界变换 = Rz(A) × Ry(B) × Rx(C) + 平移。</summary>
    internal static Trsf PoseTransform(PoseConfiguration pose)
    {
        var rz = new Trsf(new Ax1(Pnt.Origin, new Dir(0, 0, 1)), pose.A * Math.PI / 180.0);
        var ry = new Trsf(new Ax1(Pnt.Origin, new Dir(0, 1, 0)), pose.B * Math.PI / 180.0);
        var rx = new Trsf(new Ax1(Pnt.Origin, new Dir(1, 0, 0)), pose.C * Math.PI / 180.0);
        var rotation = rz.Multiplied(ry).Multiplied(rx);
        rotation.SetTranslationPart(new Vec(pose.X, pose.Y, pose.Z));
        return rotation;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var display in _partDisplays)
            if (display is not null) _context.Remove(display, false);
        if (_toolDisplay is not null) _context.Remove(_toolDisplay, false);
        if (_workDisplay is not null) _context.Remove(_workDisplay, false);
        _context.Remove(_baseAxes, false);
        _context.Remove(_flangeAxes, false);
        _context.Remove(_tcpAxes, false);
        _context.Remove(_weldSpot, false);
    }
}
