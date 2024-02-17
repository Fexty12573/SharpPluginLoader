using System.Numerics;
using System.Runtime.CompilerServices;
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
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color4));
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
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color));
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
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="sphere">The sphere to render.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtSphere sphere, Vector4 color)
    {
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color));
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
        InternalCalls.RenderObb(MemoryUtil.AddressOf(ref obb), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders an oriented bounding box at the given position with the given size and color.
    /// </summary>
    /// <param name="obb">The oriented bounding box to render.</param>
    /// <param name="color">The color of the oriented bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderObb(MtObb obb, Vector4 color)
    {
        InternalCalls.RenderObb(MemoryUtil.AddressOf(ref obb), MemoryUtil.AddressOf(ref color));
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
        InternalCalls.RenderCapsule(MemoryUtil.AddressOf(ref capsule), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, Vector4 color)
    {
        InternalCalls.RenderCapsule(MemoryUtil.AddressOf(ref capsule), MemoryUtil.AddressOf(ref color));
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
        InternalCalls.RenderLine(MemoryUtil.AddressOf(ref line), MemoryUtil.AddressOf(ref color4));
    }

    /// <inheritdoc cref="RenderLine(Vector3,Vector3,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(Vector3 start, Vector3 end, Vector4 color)
    {
        var line = new MtLineSegment { Point1 = start, Point2 = end };
        InternalCalls.RenderLine(MemoryUtil.AddressOf(ref line), MemoryUtil.AddressOf(ref color));
    }

    /// <summary>
    /// Renders a line with the given color.
    /// </summary>
    /// <param name="line">The line to render.</param>
    /// <param name="color">The color of the line.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, MtColor color)
    {
        var color4 = (Vector4)color;
        InternalCalls.RenderLine(MemoryUtil.AddressOf(ref line), MemoryUtil.AddressOf(ref color4));
    }

    /// <inheritdoc cref="RenderLine(MtLineSegment,MtColor)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderLine(MtLineSegment line, Vector4 color)
    {
        InternalCalls.RenderLine(MemoryUtil.AddressOf(ref line), MemoryUtil.AddressOf(ref color));
    }
}
