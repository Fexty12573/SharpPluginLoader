using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Resources.Collision;

public class AttackParamResource : Resource
{
    public AttackParamResource(nint instance) : base(instance, true) { }
    public AttackParamResource() { }

    public AttackParamType Type => Get<AttackParamType>(0xA8);
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

    public ref ElementType ElementType => ref GetRef<ElementType>(0x38);

    public ref uint AttackAttrLevel => ref GetRef<uint>(0x40);

    public ref float AttackAttrDamage => ref GetRef<float>(0x44);

    public ref uint ConditionLevel => ref GetRef<uint>(0x48);

    public ref float BadConditionRiseRate => ref GetRef<float>(0x4C);

    public ref float PoisonDamage => ref GetRef<float>(0x50);

    public ref float DeadlyPoisonDamage => ref GetRef<float>(0x54);

    public ref float ParalysisDamage => ref GetRef<float>(0x58);

    public ref float SleepDamage => ref GetRef<float>(0x5C);

    public ref float BlastDamage => ref GetRef<float>(0x60);

    public ref float MyxomyBlastDamage => ref GetRef<float>(0x64);

    public ref float StunDamage => ref GetRef<float>(0x68);

    public ref float ExhaustDamage => ref GetRef<float>(0x6C);

    public ref float BleedDamage => ref GetRef<float>(0x70);

    public ref float SyoukiDamage => ref GetRef<float>(0x74);

    public ref bool DefenseDownS => ref GetRef<bool>(0x78);

    public ref bool DefenseDownL => ref GetRef<bool>(0x79);

    public ref bool ElementResistDownS => ref GetRef<bool>(0x7A);

    public ref bool ElementResistDownL => ref GetRef<bool>(0x7B);

    public ref float StageDamageL => ref GetRef<float>(0x7C);

    public ref float StageDamageM => ref GetRef<float>(0x80);

    public ref float StageDamageS => ref GetRef<float>(0x84);

    public ref float StageDamageSS => ref GetRef<float>(0x88);

    public ref float StageDamageXS => ref GetRef<float>(0x8C);

    public ref float StageDamageAccumulate => ref GetRef<float>(0x90);

    public ref KnockbackType KnockbackType => ref GetRef<KnockbackType>(0xA0);

    public ref uint KnockbackLevel => ref GetRef<uint>(0xB0);

    public ref uint DamageAngle => ref GetRef<uint>(0xC0);

    public ref float Power => ref GetRef<float>(0xC8);

    public ref GuardType GuardType => ref GetRef<GuardType>(0xD8);

    public ref bool IsMultiHit => ref GetRef<bool>(0xE0);

    public ref float MultiHitTickInterval => ref GetRef<float>(0xE4);

    public ref uint HitEffect => ref GetRef<uint>(0xF0);

    public ref uint DisableHitEffect => ref GetRef<uint>(0xF8);

    public ref float HitEffectAngle => ref GetRef<float>(0xFC);

    public ref float HitEffectAngleX => ref GetRef<float>(0x100);

    public ref uint PlSkillAffect => ref GetRef<uint>(0x15C);

    public ref float Custom1 => ref GetRef<float>(0x184);

    public ref float Custom2 => ref GetRef<float>(0x188);

    public ref uint DamageUiType => ref GetRef<uint>(0x198);
}

public class AttackParamEm : AttackParam
{
    public AttackParamEm(nint instance) : base(instance) { }
    public AttackParamEm() { }

    public ref uint EmFlinchType => ref GetRef<uint>(0x1A8);
}

public class AttackParamPl : AttackParam
{
    public AttackParamPl(nint instance) : base(instance) { }
    public AttackParamPl() { }

    public ref float TenderizeDamage => ref GetRef<float>(0x1C8);

    public ref float ElementMotionValue => ref GetRef<float>(0x1D0);

    public ref float StatusMotionValue => ref GetRef<float>(0x1D4);

    public ref float MountDamage => ref GetRef<float>(0x1D8);

    public ref HitDelay HitDelayS => ref GetRef<HitDelay>(0x1E8);

    public ref HitDelay HitDelayL => ref GetRef<HitDelay>(0x208);

    public ref byte ShakeType => ref GetRef<byte>(0x230);

    public ref bool UseMindsEye => ref GetRef<bool>(0x232);

    public ref float WeaponCustom1 => ref GetRef<float>(0x260);

    public ref float WeaponCustom2 => ref GetRef<float>(0x264);

    public ref float WeaponCustom3 => ref GetRef<float>(0x268);
}

public class AttackParamPlShell : AttackParam
{
    public AttackParamPlShell(nint instance) : base(instance) { }
    public AttackParamPlShell() { }
}

public class AttackParamOt : AttackParam
{
    public AttackParamOt(nint instance) : base(instance) { }
    public AttackParamOt() { }
}

[StructLayout(LayoutKind.Sequential, Size = 0x14)]
public struct HitDelay
{
    public float Timescale;
    public float FrameAdvance;
    public float ScaledDelay;
    public float ScaleTransitionDelay;
    public float UnscaledDelay;
}

public enum AttackParamType : byte
{
    Default = 0,
    Player = 1,
    Monster = 2,
    PlayerShell = 3,
    MonsterShell = 4,
    Palico = 5,
    PalicoShell = 6,
}

public enum ImpactType
{
    HitZoneIndependent = 0,
    Sever = 1,
    Blunt = 2,
    Shot = 3,
}

public enum KnockbackType : uint
{
    None1,
    PinnedTPose,
    PinnedKneeInfinite,
    TremorOrKnees,
    None2,
    Roar,
    WindPressure,
    Flinch,
    None3,
    None4,
    None5,
    FlashStun,
    None6,
    None7,
    None8,
    NoEleStatus,
    KnockedToButt,
    LaunchRoll,
    BackflipSlide,
    Airborne,
    PinnedMove,
    PinnedNoMove,
    None9,
    None10,
    SmallFlinch,
    NoKnock = 39,
    Vortex = 48,
    VaalDrain = 49,
    JhoUnkn = 51,
}

public enum GuardType : uint
{
    RegularAngled,
    Regular360,
    Unblockable,
    GuardUp360,
    GuardUpAngled,
}

public enum ElementType : uint
{
    None = 0,
    Fire = 1,
    Water = 2,
    Ice = 3,
    Thunder = 4,
    Dragon = 5,
}
