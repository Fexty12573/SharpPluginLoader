using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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

    /// <summary>
    /// Renders a given mesh with the given transform and color.
    /// </summary>
    /// <param name="mesh">The mesh to render. Must have been previously registered via <see cref="SupplyCustomMesh"/>.</param>
    /// <param name="transform">The transform of the mesh</param>
    /// <param name="color">The color of the mesh</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderMesh(MeshHandle mesh, Matrix4x4 transform, MtColor color)
    {
        var index = Interlocked.Increment(ref _meshIndex) - 1;
        Meshes[index] = new ColoredMesh { Transform = transform, Color = color.ToVector4(), Mesh = mesh };
    }

    /// <inheritdoc cref="RenderMesh(MeshHandle,Matrix4x4,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderMesh(MeshHandle mesh, Matrix4x4 transform, Vector4 color)
    {
        var index = Interlocked.Increment(ref _meshIndex) - 1;
        Meshes[index] = new ColoredMesh { Transform = transform, Color = color, Mesh = mesh };
    }

    /// <summary>
    /// Registers a custom mesh to be used with <see cref="RenderMesh(MeshHandle,Matrix4x4,MtColor)"/>.
    /// </summary>
    /// <param name="vertices">The vertices of the mesh with each w component set to 1.</param>
    /// <param name="indices">The indices of the mesh.</param>
    /// <returns>A handle to the registered mesh.</returns>
    public static unsafe MeshHandle SupplyCustomMesh(Span<Vector4> vertices, Span<uint> indices)
    {
        fixed (Vector4* verticesPtr = vertices)
        {
            fixed (uint* indicesPtr = indices)
            {
                var mesh = new CustomMesh
                {
                    Vertices = verticesPtr,
                    Indices = indicesPtr,
                    VertexCount = vertices.Length,
                    IndexCount = indices.Length
                };

                return InternalCalls.SupplyCustomMesh(MemoryUtil.AddressOf(ref mesh));
            }
        }
    }

    /// <summary>
    /// Loads and registers a mesh from a .obj file.
    /// </summary>
    /// <param name="objPath">The path to the .obj file</param>
    /// <returns>A handle to the registered mesh, or <see cref="MeshHandle.Invalid"/> if loading failed.</returns>
    public static unsafe MeshHandle SupplyCustomMesh(string objPath)
    {
        fixed (byte* bytes = Encoding.UTF8.GetBytes(objPath))
        {
            return InternalCalls.SupplyCustomMeshFromFile(bytes);
        }
    }

    [UnmanagedCallersOnly]
    private static unsafe void RetrievePrimitives(
        ColoredSphere** outSpheres, long* sphereCount,
        ColoredObb** outObbs, long* obbCount,
        ColoredCapsule** outCapsules, long* capsuleCount,
        ColoredLine** outLines, long* lineCount,
        ColoredMesh** outMeshes, long* meshCount)
    {
        *outSpheres = Spheres.Pointer;
        *outObbs = Obbs.Pointer;
        *outCapsules = Capsules.Pointer;
        *outLines = Lines.Pointer;
        *outMeshes = Meshes.Pointer;

        *sphereCount = _sphereIndex;
        *obbCount = _obbIndex;
        *capsuleCount = _capsuleIndex;
        *lineCount = _lineIndex;
        *meshCount = _meshIndex;
    }

    [UnmanagedCallersOnly]
    private static void ReleasePrimitives()
    {
        _sphereIndex = 0;
        _obbIndex = 0;
        _capsuleIndex = 0;
        _lineIndex = 0;
        _meshIndex = 0;
    }

    private const int MaxPrimitives = 2048;

    private static readonly NativeArray<ColoredSphere> Spheres = NativeArray<ColoredSphere>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredObb> Obbs = NativeArray<ColoredObb>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredCapsule> Capsules = NativeArray<ColoredCapsule>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredLine> Lines = NativeArray<ColoredLine>.Create(MaxPrimitives);
    private static readonly NativeArray<ColoredMesh> Meshes = NativeArray<ColoredMesh>.Create(MaxPrimitives);

    private static int _sphereIndex;
    private static int _obbIndex;
    private static int _capsuleIndex;
    private static int _lineIndex;
    private static int _meshIndex;
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

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
file unsafe struct CustomMesh
{
    [FieldOffset(0x00)] public Vector4* Vertices;
    [FieldOffset(0x08)] public uint* Indices;
    [FieldOffset(0x10)] public nint VertexCount;
    [FieldOffset(0x18)] public nint IndexCount;
}

[StructLayout(LayoutKind.Explicit, Size = 0x60)]
internal struct ColoredMesh
{
    [FieldOffset(0x00)] public Matrix4x4 Transform;
    [FieldOffset(0x40)] public Vector4 Color;
    [FieldOffset(0x50)] public MeshHandle Mesh;
}
