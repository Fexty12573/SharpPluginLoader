﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.View;

/// <summary>
/// Represents an instance of a uCamera class.
/// </summary>
public class Camera : Unit
{
    public Camera(nint instance) : base(instance) { }
    public Camera() { }

    /// <summary>
    /// The position of the camera, in world space coordinates.
    /// </summary>
    public ref Vector3 Position => ref GetRef<Vector3>(0x150);

    /// <summary>
    /// The up vector of the camera, in local coordinates.
    /// </summary>
    public ref Vector3 Up => ref GetRef<Vector3>(0x160);

    /// <summary>
    /// The target of the camera, in local coordinates.
    /// </summary>
    public ref Vector3 Target => ref GetRef<Vector3>(0x170);

    /// <summary>
    /// Gets the target of the camera in world space coordinates.
    /// </summary>
    /// <returns>The target of the camera in world space coordinates.</returns>
    public unsafe Vector3 GetTargetWorld()
    {
        Vector3 result = new();
        new NativeAction<nint, nint>(GetVirtualFunction(33)).Invoke(Instance, MemoryUtil.AddressOf(ref result));
        return result;
    }

    /// <summary>
    /// The far clip plane of the camera.
    /// </summary>
    public ref float FarClip => ref GetRef<float>(0x138);

    /// <summary>
    /// The near clip plane of the camera.
    /// </summary>
    public ref float NearClip => ref GetRef<float>(0x13C);

    /// <summary>
    /// The aspect ratio of the camera.
    /// </summary>
    public ref float AspectRatio => ref GetRef<float>(0x140);

    /// <summary>
    /// The field of view of the camera, in radians.
    /// </summary>
    public ref float FieldOfView => ref GetRef<float>(0x144);

    /// <summary>
    /// Computes the view matrix of the camera.
    /// </summary>
    /// <returns>The view matrix of the camera.</returns>
    /// <remarks>
    /// <b>Warning:</b> This method does not perform any caching. It is recommended to cache the result of this method.
    /// </remarks>
    public unsafe Matrix4x4 GetViewMatrix()
    {
        Matrix4x4 result = new();
        new NativeAction<nint, nint>(GetVirtualFunction(34)).Invoke(Instance, MemoryUtil.AddressOf(ref result));
        return result;
    }

    /// <summary>
    /// Computes the projection matrix of the camera.
    /// </summary>
    /// <returns>The projection matrix of the camera.</returns>
    /// <remarks>
    /// <b>Warning:</b> This method does not perform any caching. It is recommended to cache the result of this method.
    /// </remarks>
    public unsafe Matrix4x4 GetProjectionMatrix()
    {
        Matrix4x4 result = new();
        new NativeAction<nint, nint>(GetVirtualFunction(35)).Invoke(Instance, MemoryUtil.AddressOf(ref result));
        return result;
    }
}
