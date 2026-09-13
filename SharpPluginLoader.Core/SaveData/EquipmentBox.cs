namespace SharpPluginLoader.Core.SaveData;

public class EquipmentBox : NativeWrapper
{
    public EquipmentBox(nint instance) : base(instance) { }
    public EquipmentBox() { }

    public Equipment this[int index] => new(Get<nint>(index * 0x98));
}
