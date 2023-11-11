using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    public class AnimationLayerComponent : MtObject
    {
        public AnimationLayerComponent(nint instance) : base(instance) { }
        public AnimationLayerComponent() { }

        public float CurrentFrame
        {
            get => Get<float>(0x10C);
            set => Set(0x10C, value);
        }

        public float MaxFrame
        {
            get => Get<float>(0x114);
            set => Set(0x114, value);
        }

        public float Speed
        {
            get => Get<float>(0x118);
            set => Set(0x118, value);
        }

        public void Pause() => PausedInstances.Add(Instance);

        public void Resume() => PausedInstances.Remove(Instance);


        static AnimationLayerComponent()
        {
            UpdateHook = new Hook<UpdateDelegate>(UpdateHookFunc, 0x14224c150);
        }

        private static void UpdateHookFunc(nint animLayer, int a, uint b, nint c, nint d, nint e, nint f, nint g)
        {
            if (PausedInstances.Contains(animLayer))
                new AnimationLayerComponent(animLayer).Speed = 0;

            UpdateHook.Original(animLayer, a, b, c, d, e, f, g);
        }

        private delegate void UpdateDelegate(nint instance, int unk1, uint unk2, nint unk3, nint unk4, nint unk5,
            nint unk6, nint unk7);
        private static readonly Hook<UpdateDelegate> UpdateHook = null!;
        private static readonly List<nint> PausedInstances = new();
    }
}
