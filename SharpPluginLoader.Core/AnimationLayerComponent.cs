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
            get => Get<float>(0x11C);
            set => Set(0x11C, value);
        }

        public bool Paused
        {
            get => SpeedLocks.ContainsKey(Instance) && SpeedLocks[Instance] == 0f;
            set
            {
                if (value) Pause();
                else Resume();
            }
        }

        public void LockSpeed(float speed) => SpeedLocks[Instance] = speed;

        public void UnlockSpeed() => SpeedLocks.Remove(Instance);
        
        public void Pause() => SpeedLocks.Add(Instance, 0f);

        public void Resume() => SpeedLocks.Remove(Instance);


        static AnimationLayerComponent()
        {
            UpdateHook = new Hook<UpdateDelegate>(UpdateHookFunc, 0x14224c150);
        }

        private static void UpdateHookFunc(nint animLayer, int a, uint b, nint c, nint d, nint e, nint f, nint g)
        {
            if (SpeedLocks.TryGetValue(animLayer, out var speed))
                new AnimationLayerComponent(animLayer).Speed = speed;

            UpdateHook.Original(animLayer, a, b, c, d, e, f, g);
        }

        private delegate void UpdateDelegate(nint instance, int unk1, uint unk2, nint unk3, nint unk4, nint unk5,
            nint unk6, nint unk7);
        private static readonly Hook<UpdateDelegate> UpdateHook = null!;
        private static readonly Dictionary<nint, float> SpeedLocks = new();
    }
}
