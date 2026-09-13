using System.Text;

namespace SharpPluginLoader.Core.SaveData;

public class EquipSet : MtObject
{
    public EquipSet(nint instance) : base(instance) { }
    public EquipSet() { }

    public ref int Index => ref GetRef<int>(0x8);

    public string Name
    {
        get => Encoding.UTF8.GetString(NameBytes);
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, 16);
            Encoding.UTF8.GetBytes(value, NameBytes);
        }
    }

    public ref int Weapon => ref GetRef<int>(0x10C);
    public ref int Head => ref GetRef<int>(0x110);
    public ref int Chest => ref GetRef<int>(0x114);
    public ref int Arms => ref GetRef<int>(0x118);
    public ref int Waist => ref GetRef<int>(0x11C);
    public ref int Legs => ref GetRef<int>(0x120);
    public ref int Charm => ref GetRef<int>(0x124);
    public ref int Mantle1 => ref GetRef<int>(0x128);
    public ref int Mantle2 => ref GetRef<int>(0x12C);

    public unsafe Span<int> WeaponDecos => new(GetPtrInline(0x130), 3);
    public unsafe Span<int> HeadDecos => new(GetPtrInline(0x13C), 3);
    public unsafe Span<int> ChestDecos => new(GetPtrInline(0x148), 3);
    public unsafe Span<int> ArmsDecos => new(GetPtrInline(0x154), 3);
    public unsafe Span<int> WaistDecos => new(GetPtrInline(0x160), 3);
    public unsafe Span<int> LegsDecos => new(GetPtrInline(0x16C), 3);
    public unsafe Span<int> CharmDecos => new(GetPtrInline(0x178), 3);
    public unsafe Span<int> Mantle1Decos => new(GetPtrInline(0x184), 3);
    public unsafe Span<int> Mantle2Decos => new(GetPtrInline(0x190), 3);
    public unsafe Span<int> BowgunMods => new(GetPtrInline(0x19C), 5);

    public ref int HeadLayered => ref GetRef<int>(0x270);
    public ref int ChestLayered => ref GetRef<int>(0x274);
    public ref int ArmsLayered => ref GetRef<int>(0x278);
    public ref int WaistLayered => ref GetRef<int>(0x27C);
    public ref int LegsLayered => ref GetRef<int>(0x280);

    private unsafe Span<byte> NameBytes => new(GetPtrInline(0xC), 16);
}
