using System.Text;

namespace SharpPluginLoader.Core.SaveData;

public class ItemSet : MtObject
{
    public ItemSet(nint instance) : base(instance) { }
    public ItemSet() { }

    public string Name
    {
        get => Encoding.UTF8.GetString(NameBytes);
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, 16);
            Encoding.UTF8.GetBytes(value, NameBytes);
        }
    }

    public unsafe Span<Item> Items => new(GetPtrInline(0x28), 24);
    public unsafe Span<Item> Ammo => new(GetPtrInline(0x1A8), 16);

    private unsafe Span<byte> NameBytes => new(GetPtrInline(0x8), 16);
}
