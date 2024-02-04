using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Resources.Collision;

public class CollNodeResource : Resource
{
    public CollNodeResource(nint instance) : base(instance, true) { }
    public CollNodeResource() { }

    public CollNodeType Type => Get<CollNodeType>(0xA8);

    public MtArray<CollNode> Nodes => GetInlineObject<MtArray<CollNode>>(0xB0);
}

public class CollNode : MtObject
{
    public CollNode(nint instance) : base(instance) { }
    public CollNode() { }

    public CollGeomResource? Geometry => GetObject<CollGeomResource>(0x8);

    public ref short Index => ref GetRef<short>(0x30);

    public ref uint CollNodeFlags => ref GetRef<uint>(0x34);

    public ref uint HitCollisionFlags => ref GetRef<uint>(0x38);

    public ref uint Attr => ref GetRef<uint>(0x50);
}

public class CollNodeEm : CollNode
{
    public CollNodeEm(nint instance) : base(instance) { }
    public CollNodeEm() { }

    public ref uint BaseDamageGroup => ref GetRef<uint>(0x60);
    
    public ref uint PartsDamageGroup => ref GetRef<uint>(0x70);

    public ref uint BaseDamageGroupForChanged => ref GetRef<uint>(0x80);

    public ref uint PartsDamageGroupForChanged => ref GetRef<uint>(0x90);

    public ref uint EmAttr => ref GetRef<uint>(0x98);

    public ref uint RideParts => ref GetRef<uint>(0xA8);

    public ref uint PartsGroup => ref GetRef<uint>(0xB8);

    public ref uint PartsTag => ref GetRef<uint>(0xC8);
}

public enum CollNodeType
{
    Default = 0,
    Player = 1,
    Monster = 2,
    Player3 = 3,
    Default4 = 4,
    Default5 = 5,
    Default6 = 6
}
