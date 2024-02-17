using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.View;

public class Viewport : MtObject
{
    public Viewport(nint instance) : base(instance) { }
    public Viewport() { }

    /// <summary>
    /// Gets the camera of the viewport.
    /// </summary>
    public Camera? Camera => GetObject<Camera>(0x8);

    /// <summary>
    /// Gets a boolean indicating whether the viewport is visible.
    /// </summary>
    public ref bool Visible => ref GetRef<bool>(0x20);

    /// <summary>
    /// The region of the viewport.
    /// </summary>
    public ref Rectangle Region => ref GetRef<Rectangle>(0x28);

    /// <summary>
    /// The view matrix of the viewports camera.
    /// </summary>
    public ref Matrix4x4 ViewMatrix => ref GetRef<Matrix4x4>(0xA0);

    /// <summary>
    /// The projection matrix of the viewports camera.
    /// </summary>
    public ref Matrix4x4 ProjectionMatrix => ref GetRef<Matrix4x4>(0xE0);

    /// <summary>
    /// The previous view matrix of the viewports camera.
    /// </summary>
    public ref Matrix4x4 PrevViewMatrix => ref GetRef<Matrix4x4>(0x120);

    /// <summary>
    /// The previous projection matrix of the viewports camera.
    /// </summary>
    public ref Matrix4x4 PrevProjectionMatrix => ref GetRef<Matrix4x4>(0x160);

    /// <summary>
    /// Converts a point in world space to screen space.
    /// </summary>
    /// <param name="worldPosition">The point in world space.</param>
    /// <param name="screenPos">The point in screen space.</param>
    /// <returns>True if the point is visible, false otherwise.</returns>
    public bool WorldToScreen(Vector3 worldPosition, out Vector2 screenPos)
    {
        Vector4 worldPos = new(worldPosition, 1.0f);
        
        var viewPos = Vector4.Transform(worldPos, ViewMatrix);
        var clipPos = Vector4.Transform(viewPos, ProjectionMatrix);

        if (clipPos.W < (Camera?.NearClip ?? 0.01f))
        {
            screenPos = default;
            return false;
        }

        var ndcPos = clipPos / clipPos.W;

        var hwidth = (Region.Width - Region.X) * 0.5f;
        var hheight = (Region.Height - Region.Y) * 0.5f;

        screenPos = new Vector2(
            (hwidth * ndcPos.X) + hwidth,
            -(hheight * ndcPos.Y) + hheight
        );

        return true;
    }
}
