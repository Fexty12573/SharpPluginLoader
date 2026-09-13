using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Resources;

namespace PlayerAnimationViewer;

public class EpvElement : MtObject
{
    public EpvElement(nint instance) : base(instance) { }
    public EpvElement() { }

    /// <summary>
    /// Gets a weak reference to the contained effect asset.
    /// </summary>
    public Resource? EffectAsset => Get<nint>(0x8) != 0 ? new Resource(Get<nint>(0x8), true) : null;

    public uint RecordId => Get<ushort>(0x2B0);
}

public static class EpvExtensions
{
    public static EpvElement? GetElement(this EffectProvider epv, uint groupId, uint recordId)
    {
        return epv.GetEffect(groupId, recordId)?.As<EpvElement>();
    }

    public static EpvElement? GetEffect(this Entity entity, uint groupId, uint recordId)
    {
        var effectComponent = entity.GetObject<MtObject>(0xA10);
        return effectComponent?.GetObject<EffectProvider>(0x60)?.GetElement(groupId, recordId);
    }
}
