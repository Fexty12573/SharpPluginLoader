using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Resources;

namespace PlayerAnimationViewer.Sound;

internal class WwiseContainer : MtObject
{
    public WwiseContainer(nint instance) : base(instance) { }
    public WwiseContainer() { }
}

internal class WwiseContainerAsset : Resource
{
    public WwiseContainerAsset(nint instance, bool weakRef = false) : base(instance, weakRef) { }
    public WwiseContainerAsset() { }
}

internal static class EntityExtensions
{
    public static WwiseContainer? GetWwiseContainer(this Entity entity)
    {
        var trigger = entity.ComponentManager.Find("cpWwiseTrigger")?.As<WwiseTrigger>();
        return trigger?.Container;
    }

    public static WwiseContainerAsset? GetWwiseContainerAsset(this Entity entity)
    {
        var trigger = entity.ComponentManager.Find("cpWwiseTrigger")?.As<WwiseTrigger>();
        return trigger?.ContainerAsset;
    }

    public static WwiseTrigger? GetWwiseTrigger(this Entity entity)
    {
        return entity.ComponentManager.Find("cpWwiseTrigger")?.As<WwiseTrigger>();
    }

    public static bool PlaySound(this Entity entity, int wwevId, short eventId)
    {
        var trigger = entity.GetWwiseTrigger();
        return trigger is not null && trigger.CallEventById(entity, wwevId, eventId);
    }
}
