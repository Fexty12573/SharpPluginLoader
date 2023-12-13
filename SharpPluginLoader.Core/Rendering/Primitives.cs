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
    public static void RenderSphere(MtVector3 position, float radius, MtColor color)
    {
        var sphere = new MtSphere { Center = position, Radius = radius };
        var color4 = (MtVector4)color;
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="position">The position of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtVector3 position, float radius, MtVector4 color)
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
        var color4 = (MtVector4)color;
        InternalCalls.RenderSphere(MemoryUtil.AddressOf(ref sphere), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders a sphere at the given position with the given radius and color.
    /// </summary>
    /// <param name="sphere">The sphere to render.</param>
    /// <param name="color">The color of the sphere.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderSphere(MtSphere sphere, MtVector4 color)
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
        var color4 = (MtVector4)color;
        InternalCalls.RenderObb(MemoryUtil.AddressOf(ref obb), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders an oriented bounding box at the given position with the given size and color.
    /// </summary>
    /// <param name="obb">The oriented bounding box to render.</param>
    /// <param name="color">The color of the oriented bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderObb(MtObb obb, MtVector4 color)
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
        var color4 = (MtVector4)color;
        InternalCalls.RenderCapsule(MemoryUtil.AddressOf(ref capsule), MemoryUtil.AddressOf(ref color4));
    }

    /// <summary>
    /// Renders a capsule at the given position with the given radius and color.
    /// </summary>
    /// <param name="capsule">The capsule to render.</param>
    /// <param name="color">The color of the capsule.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderCapsule(MtCapsule capsule, MtVector4 color)
    {
        InternalCalls.RenderCapsule(MemoryUtil.AddressOf(ref capsule), MemoryUtil.AddressOf(ref color));
    }
}
