using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Fsm.Weapon;

[StructLayout(LayoutKind.Explicit)]
internal struct TransitionMapping
{
    [FieldOffset(0x00)] public int TransitionId;
    [FieldOffset(0x08)] public nint DtiPtr;

    public MtDti? Dti
    {
        readonly get => DtiPtr == 0 ? null : new MtDti(DtiPtr);
        set => DtiPtr = value?.Instance ?? 0;
    }
}
