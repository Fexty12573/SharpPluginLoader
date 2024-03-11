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
        Spheres.Add(new ColoredSphere { Sphere = sphere, Color = color4 });
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
        Spheres.Add(new ColoredSphere { Sphere = sphere, Color = color });
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
        Spheres.Add(new ColoredSphere { Sphere = sphere, Color = color4 });
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="sphere">The sphere to render.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtSphere sphere, Vector4 color)
    {
        Spheres.Add(new ColoredSphere { Sphere = sphere, Color = color });
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
        Obbs.Add(new ColoredObb { Obb = obb, Color = color4 });
    }

    /// <summary>
    /// Renders an oriented bounding box at the given position with the given size and color.
    /// </summary>
    /// <param name="obb">The oriented bounding box to render.</param>
    /// <param name="color">The color of the oriented bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderObb(MtObb obb, Vector4 color)
    {
        Obbs.Add(new ColoredObb { Obb = obb, Color = color });
    }


    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, MtColor color)
    {
        Capsules.Add(new ColoredCapsule { Capsule = capsule, Color = color.ToVector4() });
    }

    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, Vector4 color)
    {
        Capsules.Add(new ColoredCapsule { Capsule = capsule, Color = color });
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
        Lines.Add(new ColoredLine { Line = line, Color = color4 });
    }

    /// <inheritdoc cref="RenderLine(Vector3,Vector3,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(Vector3 start, Vector3 end, Vector4 color)
    {
        var line = new MtLineSegment { Point1 = start, Point2 = end };
        Lines.Add(new ColoredLine { Line = line, Color = color });
    }

    /// <summary>
    /// Renders a line with the given color.
    /// </summary>
    /// <param name="line">The line to render.</param>
    /// <param name="color">The color of the line.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, MtColor color)
    {
        Lines.Add(new ColoredLine { Line = line, Color = color.ToVector4() });
    }

    /// <inheritdoc cref="RenderLine(MtLineSegment,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, Vector4 color)
    {
        Lines.Add(new ColoredLine { Line = line, Color = color });
    }

    [UnmanagedCallersOnly]
    private static unsafe void RetrievePrimitives(
        ColoredSphere** outSpheres, long* sphereCount,
        ColoredObb** outObbs, long* obbCount,
        ColoredCapsule** outCapsules, long* capsuleCount,
        ColoredLine** outLines, long* lineCount)
    {
        _sphereMemory = new Memory<ColoredSphere>(ListExtensions.UnderlyingArray(Spheres));
        _obbMemory = new Memory<ColoredObb>(ListExtensions.UnderlyingArray(Obbs));
        _capsuleMemory = new Memory<ColoredCapsule>(ListExtensions.UnderlyingArray(Capsules));
        _lineMemory = new Memory<ColoredLine>(ListExtensions.UnderlyingArray(Lines));

        _sphereHandle = _sphereMemory.Pin();
        _obbHandle = _obbMemory.Pin();
        _capsuleHandle = _capsuleMemory.Pin();
        _lineHandle = _lineMemory.Pin();

        *outSpheres = (ColoredSphere*)_sphereHandle.Pointer;
        *outObbs = (ColoredObb*)_obbHandle.Pointer;
        *outCapsules = (ColoredCapsule*)_capsuleHandle.Pointer;
        *outLines = (ColoredLine*)_lineHandle.Pointer;

        *sphereCount = Spheres.Count;
        *obbCount = Obbs.Count;
        *capsuleCount = Capsules.Count;
        *lineCount = Lines.Count;
    }

    [UnmanagedCallersOnly]
    private static void ReleasePrimitives()
    {
        _sphereHandle.Dispose();
        _obbHandle.Dispose();
        _capsuleHandle.Dispose();
        _lineHandle.Dispose();

        Spheres.Clear();
        Obbs.Clear();
        Capsules.Clear();
        Lines.Clear();
    }

    private static readonly List<ColoredSphere> Spheres = [];
    private static readonly List<ColoredObb> Obbs = [];
    private static readonly List<ColoredCapsule> Capsules = [];
    private static readonly List<ColoredLine> Lines = [];

    private static Memory<ColoredSphere> _sphereMemory;
    private static Memory<ColoredObb> _obbMemory;
    private static Memory<ColoredCapsule> _capsuleMemory;
    private static Memory<ColoredLine> _lineMemory;

    private static MemoryHandle _sphereHandle;
    private static MemoryHandle _obbHandle;
    private static MemoryHandle _capsuleHandle;
    private static MemoryHandle _lineHandle;
}

file static class ListExtensions
{
    public static T[] UnderlyingArray<T>(List<T> list) => ArrayAccessor<T>.Get(list);

    private static class ArrayAccessor<T>
    {
        public static readonly Func<List<T>, T[]> Get;

        static ArrayAccessor()
        {
            var dm = new DynamicMethod(
                "get",
                MethodAttributes.Static | MethodAttributes.Public,
                CallingConventions.Standard,
                typeof(T[]),
                [typeof(List<T>)],
                typeof(ArrayAccessor<T>),
                true
            );
            var gen = dm.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, typeof(List<T>)
                .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)!);
            gen.Emit(OpCodes.Ret);
            Get = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
        }
    }
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
