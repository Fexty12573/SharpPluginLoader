using System;
using System.Collections.Generic;
using System.Linq;
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
    public ref MtRect Region => ref GetRef<MtRect>(0x28);

    /// <summary>
    /// The view matrix of the viewports camera.
    /// </summary>
    public ref MtMatrix4X4 ViewMatrix => ref GetRef<MtMatrix4X4>(0xA0);

    /// <summary>
    /// The projection matrix of the viewports camera.
    /// </summary>
    public ref MtMatrix4X4 ProjectionMatrix => ref GetRef<MtMatrix4X4>(0xE0);

    /// <summary>
    /// The previous view matrix of the viewports camera.
    /// </summary>
    public ref MtMatrix4X4 PrevViewMatrix => ref GetRef<MtMatrix4X4>(0x120);

    /// <summary>
    /// The previous projection matrix of the viewports camera.
    /// </summary>
    public ref MtMatrix4X4 PrevProjectionMatrix => ref GetRef<MtMatrix4X4>(0x160);
}
