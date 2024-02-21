using SharpPluginLoader.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core;

/// <summary>
/// Represents an instance of a cUnit.
/// </summary>
/// <remarks>
/// A unit is basically an object that can act on its own (i.e. has an update method). It doesn't necessarily need to
/// have a physical representation in the game world (e.g. Schedulers, Cameras, Visual Filters are also units).
/// But everything that <i>does</i> have a physical representation in the game world is a unit.
/// </remarks>
public class Unit : MtObject
{
    public Unit(nint instance) : base(instance) { }
    public Unit() { }

    /// <summary>
    /// The RNO of the unit.
    /// </summary>
    public ref uint Rno => ref GetRef<uint>(0x8);

    /// <summary>
    /// The unit parameter of the unit.
    /// </summary>
    public ref uint UnitParam => ref GetRef<uint>(0xC);

    /// <summary>
    /// The line that the unit is in, see <see cref="MoveLine"/>.
    /// </summary>
    public ref uint LineNo => ref GetRef<uint>(0x10);

    /// <summary>
    /// The next unit in the line.
    /// </summary>
    public Unit? Next => GetObject<Unit>(0x30);

    /// <summary>
    /// The previous unit in the line.
    /// </summary>
    public Unit? Prev => GetObject<Unit>(0x38);

    /// <summary>
    /// The unit group of the unit.
    /// </summary>
    public ref ulong UnitGroup => ref GetRef<ulong>(0x40);

    /// <summary>
    /// The time in seconds since the last update.
    /// </summary>
    public ref float DeltaSec => ref GetRef<float>(0x68);

    /// <summary>
    /// The time since the last update.
    /// </summary>
    public ref float DeltaTime => ref GetRef<float>(0x6C);

    /// <summary>
    /// The draw mode of the unit.
    /// </summary>
    public ref uint DrawMode => ref GetRef<uint>(0x48);

    /// <summary>
    /// The unit's component manager.
    /// </summary>
    public ComponentManager ComponentManager => GetInlineObject<ComponentManager>(0x70);

    /// <summary>
    /// The name of the unit.
    /// </summary>
    public unsafe string Name => new NativeFunction<nint, string>(GetVirtualFunction(15)).Invoke(Instance);

    public bool Fix
    {
        get => ((Get<uint>(0x14) >> 3) & 1) != 0;
        set => Set(0x14, (Get<uint>(0x14) & 0xFFFFFFF7) | (value ? 8u : 0u));
    }

    public bool Draw
    {
        get => ((Get<uint>(0x14) >> 1) & 1) != 0;
        set => Set(0x14, (Get<uint>(0x14) & 0xFFFFFFFD) | (value ? 2u : 0u));
    }

    public bool Move
    {
        get => (Get<uint>(0x14) & 1) != 0;
        set => Set(0x14, (Get<uint>(0x14) & 0xFFFFFFFE) | (value ? 1u : 0u));
    }
}
