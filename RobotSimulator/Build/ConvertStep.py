import argparse
import io
from pathlib import Path
import struct
import sys
import tempfile


parser = argparse.ArgumentParser(description="将 STEP 的 BREP 和 XDE 原色转换为 .cad 显示缓存")
parser.add_argument("--site-dir", type=Path, help="开发端 Python 依赖目录")
parser.add_argument("--job", type=Path, required=True, help="UTF-8 清单，每行 源STEP|目标CAD")
options = parser.parse_args()
if options.site_dir:
    sys.path.insert(0, str(options.site_dir))

import brotli
from OCP.BinTools import BinTools, BinTools_FormatVersion_VERSION_4
from OCP.Bnd import Bnd_Box
from OCP.BRep import BRep_Builder
from OCP.BRepBndLib import BRepBndLib
from OCP.BRepMesh import BRepMesh_IncrementalMesh
from OCP.BRepTools import BRepTools_ReShape
from OCP.IFSelect import IFSelect_RetDone
from OCP.Quantity import Quantity_TypeOfColor
from OCP.STEPCAFControl import STEPCAFControl_Reader
from OCP.TCollection import TCollection_ExtendedString
from OCP.TDocStd import TDocStd_Document
from OCP.TopAbs import TopAbs_FACE
from OCP.TopExp import TopExp
from OCP.TopLoc import TopLoc_Location
from OCP.TopoDS import TopoDS_Compound, TopoDS_Iterator
from OCP.XCAFApp import XCAFApp_Application
from OCP.XCAFDoc import XCAFDoc_DocumentTool, XCAFDoc_ShapeTool
from OCP.XCAFPrs import XCAFPrs
from OCP.collections import (
    IndexedDataMap_TopoDS_Shape_XCAFPrs_Style_TopTools_ShapeMapHasher,
    IndexedMap_TopoDS_Shape_TopTools_ShapeMapHasher,
    Sequence_TDF_Label,
)


def open_step(source):
    reader = STEPCAFControl_Reader()
    reader.SetColorMode(True)
    # 流式入口由 Python 打开中文路径，OCCT 只接收 STEP 的内容。
    if reader.ReadStream(source.name, io.BytesIO(source.read_bytes())) != IFSelect_RetDone:
        raise ValueError("无法读取 STEP")
    reader.ChangeReader().SetSystemLengthUnit(1.0)
    document = TDocStd_Document(TCollection_ExtendedString("BinXCAF"))
    XCAFApp_Application.GetApplication_s().InitDocument(document)
    if not reader.Transfer(document):
        raise ValueError("无法导入 STEP 装配")
    return document


def face_map(shape):
    faces = IndexedMap_TopoDS_Shape_TopTools_ShapeMapHasher()
    TopExp.MapShapes_s(shape, TopAbs_FACE, faces)
    return faces


def surface_colors(shape, root, faces):
    styles = IndexedDataMap_TopoDS_Shape_XCAFPrs_Style_TopTools_ShapeMapHasher()
    XCAFPrs.CollectStyleSettings_s(root, TopLoc_Location(), styles)
    colors = {}

    def visit(node, inherited):
        index = styles.FindIndex(node)
        if index:
            style = styles.FindFromIndex(index)
            material = style.Material()
            color = (style.GetColorSurfRGBA() if style.IsSetColorSurf()
                     else material.BaseColor() if material is not None and not material.IsEmpty() else None)
            if color is not None:
                rgb = color.GetRGB().Values(Quantity_TypeOfColor.Quantity_TOC_sRGB)
                inherited = bytes(round(max(0.0, min(1.0, value)) * 255) for value in rgb)

        if node.ShapeType() == TopAbs_FACE:
            colors[faces.FindIndex(node)] = inherited
            return

        children = TopoDS_Iterator(node)
        while children.More():
            visit(children.Value(), inherited)
            children.Next()

    # .cad 沿用逐面 RGB。未指定样式的面以默认白色显示。
    visit(shape, b"\xff\xff\xff")
    return colors


def bounds(shape):
    box = Bnd_Box()
    BRepBndLib.Add_s(shape, box, False)
    return box


def remove_reference_faces(shape, faces):
    if not bounds(shape).IsOpen():
        return shape, 0
    change = BRepTools_ReShape()
    removed = 0
    for index in range(1, faces.Extent() + 1):
        face = faces.FindKey(index)
        if bounds(face).IsOpen():
            change.Remove(face)
            removed += 1
    # 保留未修改面的拓扑标识，因此颜色仍可按原 1-based 面号查找。
    return change.Apply(shape) if removed else shape, removed


def save_package(target, palette, indices, brep):
    target.parent.mkdir(parents=True, exist_ok=True)
    temporary = None
    try:
        with tempfile.NamedTemporaryFile(dir=target.parent, suffix=".tmp", delete=False) as output:
            temporary = Path(output.name)
            compressor = brotli.Compressor(quality=5)
            for chunk in (
                struct.pack("<4i", 0x44334748, 1, len(indices), len(palette)),
                b"".join(palette),
                struct.pack(f"<{len(indices)}i", *indices),
                struct.pack("<i", brep.getbuffer().nbytes),
                brep.getbuffer(),
            ):
                output.write(compressor.process(chunk))
            output.write(compressor.finish())
        temporary.replace(target)
    finally:
        if temporary is not None:
            temporary.unlink(missing_ok=True)


def convert(source, target):
    document = open_step(source)
    tool = XCAFDoc_DocumentTool.ShapeTool_s(document.Main())
    labels = Sequence_TDF_Label()
    tool.GetFreeShapes(labels)

    scene = TopoDS_Compound()
    builder = BRep_Builder()
    builder.MakeCompound(scene)
    original = IndexedMap_TopoDS_Shape_TopTools_ShapeMapHasher()
    colors = {}
    for index in range(1, labels.Length() + 1):
        root = labels.Value(index)
        shape = XCAFDoc_ShapeTool.GetShape_s(root)
        if shape.IsNull():
            continue
        TopExp.MapShapes_s(shape, TopAbs_FACE, original)
        colors.update(surface_colors(shape, root, original))
        builder.Add(scene, shape)

    scene, removed = remove_reference_faces(scene, original)
    faces = face_map(scene)
    box = bounds(scene)
    if not faces.Extent() or box.IsVoid() or box.IsOpen():
        raise ValueError("STEP 不包含可显示的有限几何")

    palette = {}
    indices = []
    for index in range(1, faces.Extent() + 1):
        original_index = original.FindIndex(faces.FindKey(index))
        if original_index not in colors:
            raise ValueError(f"无法查找第 {index} 个面的 XDE 样式")
        color = colors[original_index]
        indices.append(palette.setdefault(color, len(palette)))

    if not BRepMesh_IncrementalMesh(scene, 0.35, False, 0.45, True).IsDone():
        raise ValueError("无法生成三角化")
    brep = io.BytesIO()
    # 格式 4 可由应用随附的 OCCT 7.9.2 读取，包含三角化和法向。
    BinTools.Write_s(scene, brep, True, True, BinTools_FormatVersion_VERSION_4)
    if not brep.getbuffer().nbytes:
        raise ValueError("无法写入 BREP")
    save_package(target, palette, indices, brep)
    print(f"[模型缓存] {source.name}: {len(indices)} 面 / {len(palette)} 色 / 剔除 {removed} 个无限参考面", flush=True)


for line in options.job.read_text(encoding="utf-8-sig").splitlines():
    source, separator, target = line.partition("|")
    if not line.strip():
        continue
    if not separator or not source or not target:
        raise ValueError("模型转换清单必须按 源STEP|目标CAD 填写")
    try:
        convert(Path(source), Path(target))
    except Exception as error:
        raise SystemExit(f"[模型缓存] {source}: {error}") from error
