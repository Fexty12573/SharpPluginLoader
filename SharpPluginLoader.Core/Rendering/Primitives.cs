using System.Buffers;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Rendering;


/// <summary>
/// Provides methods for rendering 3D "primitives".
/// </summary>
public static class Primitives
{
    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="position">The position of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(Vector3 position, float radius, MtColor color)
    {
        var sphere = new MtSphere { Center = position, Radius = radius };
        var color4 = (Vector4)color;
        var index = Interlocked.Increment(ref _sphereIndex) - 1;
        Spheres[index] = new ColoredSphere { Sphere = sphere, Color = color4 };
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="position">The position of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(Vector3 position, float radius, Vector4 color)
    {
        var sphere = new MtSphere { Center = position, Radius = radius };
        var index = Interlocked.Increment(ref _sphereIndex) - 1;
        Spheres[index] = new ColoredSphere { Sphere = sphere, Color = color };
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="sphere">The sphere to render.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtSphere sphere, MtColor color)
    {
        var color4 = (Vector4)color;
        var index = Interlocked.Increment(ref _sphereIndex) - 1;
        Spheres[index] = new ColoredSphere { Sphere = sphere, Color = color4 };
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="sphere">The sphere to render.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtSphere sphere, Vector4 color)
    {
        var index = Interlocked.Increment(ref _sphereIndex) - 1;
        Spheres[index] = new ColoredSphere { Sphere = sphere, Color = color };
    }


    /// <summary>
    /// Renders an oriented bounding box at the given position with the given size and color.
    /// </summary>
    /// <param name="obb">The oriented bounding box to render.</param>
    /// <param name="color">The color of the oriented bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderObb(MtObb obb, MtColor color)
    {
        var color4 = (Vector4)color;
        var index = Interlocked.Increment(ref _obbIndex) - 1;
        Obbs[index] = new ColoredObb { Obb = obb, Color = color4 };
    }

    /// <summary>
    /// Renders an oriented bounding box at the given position with the given size and color.
    /// </summary>
    /// <param name="obb">The oriented bounding box to render.</param>
    /// <param name="color">The color of the oriented bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderObb(MtObb obb, Vector4 color)
    {
        var index = Interlocked.Increment(ref _obbIndex) - 1;
        Obbs[index] = new ColoredObb { Obb = obb, Color = color };
    }


    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, MtColor color)
    {
        var color4 = (Vector4)color;
        var index = Interlocked.Increment(ref _capsuleIndex) - 1;
        Capsules[index] = new ColoredCapsule { Capsule = capsule, Color = color4 };
    }

    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, Vector4 color)
    {
        var index = Interlocked.Increment(ref _capsuleIndex) - 1;
        Capsules[index] = new ColoredCapsule { Capsule = capsule, Color = color };
    }

    /// <summary>
    /// Renders a line at the given start and end position with the given color.
    /// </summary>
    /// <param name="start">The start position of the line.</param>
    /// <param name="end">The end position of the line.</param>
    /// <param name="color">The color of the line.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(Vector3 start, Vector3 end, MtColor color)
    {
        var line = new MtLineSegment { Point1 = start, Point2 = end };
        var color4 = (Vector4)color;
        var index = Interlocked.Increment(ref _lineIndex) - 1;
        Lines[index] = new ColoredLine { Line = line, Color = color4 };
    }

    /// <inheritdoc cref="RenderLine(Vector3,Vector3,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(Vector3 start, Vector3 end, Vector4 color)
    {
        var line = new MtLineSegment { Point1 = start, Point2 = end };
        var index = Interlocked.Increment(ref _lineIndex) - 1;
        Lines[index] = new ColoredLine { Line = line, Color = color };
    }

    /// <summary>
    /// Renders a line with the given color.
    /// </summary>
    /// <param name="line">The line to render.</param>
    /// <param name="color">The color of the line.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, MtColor color)
    {
        var index = Interlocked.Increment(ref _lineIndex) - 1;
        Lines[index] = new ColoredLine { Line = line, Color = color.ToVector4() };
    }

    /// <inheritdoc cref="RenderLine(MtLineSegment,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, Vector4 color)
    {
        var index = Interlocked.Increment(ref _lineIndex) - 1;
        Lines[index] = new ColoredLine { Line = line, Color = color };
    }

    [UnmanagedCallersOnly]
    private static unsafe void RetrievePrimitives(
        ColoredSphere** outSpheres, long* sphereCount,
        ColoredObb** outObbs, long* obbCount,
        ColoredCapsule** outCapsules, long* capsuleCount,
        ColoredLine** outLines, long* lineCount)
    {
        *outSpheres = Spheres.Pointer;
        *outObbs = Obbs.Pointer;
        *outCapsules = Capsules.Pointer;
        *outLines = Lines.Pointer;

        *sphereCount = _sphereIndex;
        *obbCount = _obbIndex;
        *capsuleCount = _capsuleIndex;
        *lineCount = _lineIndex;
    }

    [UnmanagedCallersOnly]
    private static void ReleasePrimitives()
    {
        _sphereIndex = 0;
        _obbIndex = 0;
        _capsuleIndex = 0;
        _lineIndex = 0;
    }

    private const int MaxPrimitives = 2048;

    private static readonly NativeArray<ColoredSphere> Spheres = NativeArray<ColoredSphere>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredObb> Obbs = NativeArray<ColoredObb>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredCapsule> Capsules = NativeArray<ColoredCapsule>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredLine> Lines = NativeArray<ColoredLine>.Create(MaxPrimitives);

    private static int _sphereIndex;
    private static int _obbIndex;
    private static int _capsuleIndex;
    private static int _lineIndex;
}

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
internal struct ColoredSphere
{
    [FieldOffset(0x00)] public MtSphere Sphere;
    [FieldOffset(0x10)] public Vector4 Color;
}

[StructLayout(LayoutKind.Explicit, Size = 0x60)]
internal struct ColoredObb
{
    [FieldOffset(0x00)] public MtObb Obb;
    [FieldOffset(0x50)] public Vector4 Color;
}

[StructLayout(LayoutKind.Explicit, Size = 0x40)]
internal struct ColoredCapsule
{
    [FieldOffset(0x00)] public MtCapsule Capsule;
    [FieldOffset(0x30)] public Vector4 Color;
}

[StructLayout(LayoutKind.Explicit, Size = 0x30)]
internal struct ColoredLine
{
    [FieldOffset(0x00)] public MtLineSegment Line;
    [FieldOffset(0x20)] public Vector4 Color;
}
