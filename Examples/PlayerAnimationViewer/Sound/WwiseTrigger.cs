using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Components;
using System.Numerics;

namespace PlayerAnimationViewer.Sound;

internal class WwiseTrigger : Component
{
    public WwiseTrigger(nint instance) : base(instance) { }
    public WwiseTrigger() { }

    public WwiseContainer Container => GetInlineObject<WwiseContainer>(0x60);

    public WwiseContainerAsset? ContainerAsset
    {
        get => GetObject<WwiseContainerAsset>(0x298);
        set => SetObject(0x298, value!);
    }

    public bool CallEventById(MtObject source, int wwevId, short eventId)
    {
        var param = new InternalCalls.EventParams
        {
            V0 = new Vector4(),
            V1 = new Vector4(0f, 0f, 1f, 0f),
            V2 = new Vector4(),
        };

        return InternalCalls.CallEventById(Instance + 0x60, source.Instance, 0, wwevId, eventId, ref param);
    }
}
