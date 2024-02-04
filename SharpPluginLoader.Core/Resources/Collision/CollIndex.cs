using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources.Collision;

public class CollIndexResource : Resource
{
    public CollIndexResource(nint instance) : base(instance, true) { }
    public CollIndexResource() { }

    public unsafe Span<CollIndex> Indices => new(GetPtr(0xA8), Get<int>(0xB0));
}

[StructLayout(LayoutKind.Explicit, Size = 0x48)]
public unsafe struct CollIndex
{
    [FieldOffset(0x10)] public int NodeIndex;
    [FieldOffset(0x20)] public int AttackParamIndex;
    [FieldOffset(0x30)] public int AppendParamIndex;
    [FieldOffset(0x38)] public short LinkId;
    [FieldOffset(0x3A)] public bool LinkTop;
    [FieldOffset(0x3C)] public uint UniqueId;
    [FieldOffset(0x40)] public MtString* NamePtr;

    public string Name => NamePtr != null ? NamePtr->GetString(Encoding.UTF8) : "";
}
