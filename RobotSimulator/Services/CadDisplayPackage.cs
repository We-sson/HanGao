using System;
using System.IO;
using System.IO.Compression;
using Macad.Occt;
using Macad.Occt.Helper;

namespace RobotSimulator.Services;

/// <summary>
/// 运行时显示缓存（.cad）读取器：BREP 形状 + 逐面 sRGB 颜色，整包 Brotli 压缩。
/// 构建脚本写入 Magic、Version、FaceCount、ColorCount、
/// ColorCount×RGB、FaceCount×颜色索引、BrepLength、BREP 字节。
/// </summary>
internal sealed class CadDisplayPackage
{
    const int Magic = 0x44334748; // "HG3D"
    const int Version = 1;

    readonly byte[] _palette;
    readonly int[] _faceColorIndices;

    CadDisplayPackage(TopoDS_Shape shape, byte[] palette, int[] faceColorIndices)
    {
        Shape = shape;
        _palette = palette;
        _faceColorIndices = faceColorIndices;
    }

    public TopoDS_Shape Shape { get; }

    /// <summary>与 BREP 中 <c>TopExp.MapShapes</c> 的面顺序一一对应的面数。</summary>
    public int FaceCount => _faceColorIndices.Length;

    /// <summary>调色板颜色数，用于校验面颜色索引。</summary>
    public int ColorCount => _palette.Length / 3;

    public static CadDisplayPackage Read(Stream source)
    {
        using var brotli = new BrotliStream(source, CompressionMode.Decompress, leaveOpen: true);
        using var buffer = new MemoryStream();
        brotli.CopyTo(buffer);
        buffer.Position = 0;

        using var reader = new BinaryReader(buffer);
        if (reader.ReadInt32() != Magic)
            throw new InvalidDataException("不是有效的 .cad 三维模型文件。");

        var version = reader.ReadInt32();
        if (version != Version)
            throw new InvalidDataException($"不支持的 .cad 三维模型版本：{version}。");

        var faceCount = reader.ReadInt32();
        var colorCount = reader.ReadInt32();
        if (faceCount < 0 || colorCount < 0)
            throw new InvalidDataException(".cad 三维模型的面数无效。");

        var palette = new byte[colorCount * 3];
        for (var i = 0; i < palette.Length; i++) palette[i] = reader.ReadByte();

        var faceColorIndices = new int[faceCount];
        for (var i = 0; i < faceColorIndices.Length; i++) faceColorIndices[i] = reader.ReadInt32();

        var brepLength = reader.ReadInt32();
        if (brepLength <= 0)
            throw new InvalidDataException(".cad 三维模型缺少 BREP 数据。");

        var brep = reader.ReadBytes(brepLength);
        var shape = BRepExchange.ReadBinary(brep)
            ?? throw new InvalidDataException(".cad 三维模型的 BREP 反序列化失败。");
        return new CadDisplayPackage(shape, palette, faceColorIndices);
    }

    /// <summary>取 1-based 面号的颜色槽位；越界返回 0。</summary>
    public int ColorSlot(int faceNumber)
    {
        var slot = faceNumber - 1;
        return slot >= 0 && slot < _faceColorIndices.Length ? _faceColorIndices[slot] : 0;
    }

    public Quantity_Color PaletteColor(int slot)
    {
        if (slot < 0 || slot * 3 + 2 >= _palette.Length) return CadColors.Srgb(255, 255, 255);
        return CadColors.Srgb(_palette[slot * 3], _palette[slot * 3 + 1], _palette[slot * 3 + 2]);
    }
}

/// <summary>sRGB 颜色转换：Macad 的 Quantity_Color 只有在 sRGB 枚举下才与原始面色一致。</summary>
internal static class CadColors
{
    internal static Quantity_Color Srgb(byte r, byte g, byte b) =>
        new(r / 255.0, g / 255.0, b / 255.0, Quantity_TypeOfColor.sRGB);
}
