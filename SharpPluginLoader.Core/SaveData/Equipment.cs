using SharpPluginLoader.Core.Entities;

namespace SharpPluginLoader.Core.SaveData;

public class Equipment : MtObject
{
    public Equipment(nint instance) : base(instance) { }
    public Equipment() { }

    public ref int Index => ref GetRef<int>(0x8);

    public ref EquipmentType Type => ref GetRef<EquipmentType>(0x18);
    public bool IsWeapon => Type == EquipmentType.Weapon;
    public bool IsArmor => Type == EquipmentType.Armor;

    public ref int Kind => ref GetRef<int>(0x20);
    public ref int Id => ref GetRef<int>(0x24);
    public ref int Level => ref GetRef<int>(0x28);

    public unsafe Span<int> Decorations => new(GetPtrInline<int>(0x30), 3);
    public unsafe Span<int> BowgunMods => new(GetPtrInline<int>(0x3C), 5);
    public unsafe Span<int> Special => new(GetPtrInline<int>(0x50), 3);
    public unsafe Span<byte> Augments => new(GetPtrInline<byte>(0x68), 8);

    public ref int Pendant => ref GetRef<int>(0x64);

    public int GetAugmentLevel(AugmentType augment) => Get<int>(0x68 + (int)augment);
    public ref WeaponType WeaponType => ref GetRef<WeaponType>(0x20); // Same as Kind
    public ref ArmorType ArmorType => ref GetRef<ArmorType>(0x20);
}

public enum EquipmentType
{
    Armor = 0,
    Weapon,
    Unk2,
    Unk3,
    Unk4
}

public enum AugmentType
{
    ExtraSlots = 0,
    Attack,
    Affinity,
    Defense,
    DecorationSlot,
    Health,
    Element,
    Custom
}

public enum ArmorType
{
    Head = 0,
    Chest,
    Arms,
    Waist,
    Legs
}
