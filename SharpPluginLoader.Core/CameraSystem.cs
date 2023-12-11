using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.View;

namespace SharpPluginLoader.Core;

public static class CameraSystem
{
    public static nint SingletonInstance => MemoryUtil.Read<nint>(0x1451c21c0);

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
        return new MtObject(SingletonInstance).GetInlineObject<Viewport>(0x50 + index * 0x1A0);
    }
}
