using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Resources.Collision;

public class AttackParamResource : Resource
{
    public AttackParamResource(nint instance) : base(instance) { }
    public AttackParamResource() { }

    public MtArray<AttackParam> AttackParams => GetInlineObject<MtArray<AttackParam>>(0xB0);
}


public class AttackParam : MtObject
{
    public AttackParam(nint instance) : base(instance) { }
    public AttackParam() { }

    public ref uint Index => ref GetRef<uint>(0x8);

    public ref uint TargetHitGroup => ref GetRef<uint>(0xC);

    public ref ImpactType ImpactType => ref GetRef<ImpactType>(0x18);

    public ref float Attack => ref GetRef<float>(0x20);

    public ref float FixedAttack => ref GetRef<float>(0x24);

    public ref float PartBreakRate => ref GetRef<float>(0x28);
}

public enum AttackParamType : byte
{
    Default = 0,
    Player = 1,
    Monster = 2,
    PlayerShell = 3,
    MonsterShell = 4,
    Palico = 5,
    PalicoShell = 6
}

public enum ImpactType
{
    HitZoneIndependent = 0,
    Sever = 1,
    Blunt = 2,
    Shot = 3
}
