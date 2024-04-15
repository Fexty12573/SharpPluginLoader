using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.View;

namespace SharpPluginLoader.Core;

/// <summary>
/// Exposes functionality related to the sMhCamera singleton.
/// </summary>
public static class CameraSystem
{
    public static MtObject SingletonInstance => SingletonManager.GetSingleton("sMhCamera")!;

    /// <summary>
    /// Gets the main viewport.
    /// </summary>
    public static Viewport MainViewport => GetViewport(0);

    /// <summary>
    /// Gets the viewport at the specified index.
    /// </summary>
    /// <param name="index">The index of the viewport.</param>
    /// <returns>The viewport at the specified index.</returns>
    public static Viewport GetViewport(int index)
    {
        Ensure.IsTrue(index is >= 0 and < 8);
        return SingletonInstance.GetInlineObject<Viewport>(0x50 + index * 0x1A0);
    }
}
