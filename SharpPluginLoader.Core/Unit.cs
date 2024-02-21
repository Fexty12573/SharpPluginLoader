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
public class Unit : MtObject
{
    public Unit(nint instance) : base(instance) { }
    public Unit() { }

    public ref uint Rno => ref GetRef<uint>(0x8);

    public ref uint UnitParam => ref GetRef<uint>(0xC);

    public ref uint LineNo => ref GetRef<uint>(0x10);

    public Unit? Next => GetObject<Unit>(0x30);

    public Unit? Prev => GetObject<Unit>(0x38);

    public ref ulong UnitGroup => ref GetRef<ulong>(0x40);

    public ref float DeltaSec => ref GetRef<float>(0x68);

    public ref float DeltaTime => ref GetRef<float>(0x6C);

    public ref uint DrawMode => ref GetRef<uint>(0x48);

    public ComponentManager ComponentManager => GetInlineObject<ComponentManager>(0x70);

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
