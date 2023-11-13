using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    public class AnimationLayerComponent : MtObject
    {
        public AnimationLayerComponent(nint instance) : base(instance) { }
        public AnimationLayerComponent() { }

        public Entity? Owner => GetObject<Entity>(0x30);

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
            var animLayerComponent = new AnimationLayerComponent(animLayer);
            if (SpeedLocks.TryGetValue(animLayer, out var speed))
                animLayerComponent.Speed = speed;

            var owner = animLayerComponent.Owner;
            if (owner != null)
            {
                foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnEntityAnimationUpdate))
                    plugin.OnEntityAnimationUpdate(owner, owner.CurrentAnimation, animLayerComponent.Get<float>(0xE27C));
            }

            UpdateHook.Original(animLayer, a, b, c, d, e, f, g);
        }

        private delegate void UpdateDelegate(nint instance, int unk1, uint unk2, nint unk3, nint unk4, nint unk5,
            nint unk6, nint unk7);
        private static readonly Hook<UpdateDelegate> UpdateHook;
        private static readonly Dictionary<nint, float> SpeedLocks = new();
    }

    public readonly struct AnimationId
    {
        public uint FullId { get; }

        public uint Lmt => FullId >> 12;
        public uint Id => FullId & 0xFFF;

        public AnimationId(uint fullId) => FullId = fullId;

        public static implicit operator uint(AnimationId id) => id.FullId;
        public static implicit operator AnimationId(uint id) => new(id);

        public static bool operator ==(AnimationId a, AnimationId b) => a.FullId == b.FullId;
        public static bool operator !=(AnimationId a, AnimationId b) => a.FullId != b.FullId;

        public bool Equals(AnimationId other) => FullId == other.FullId;
        public override bool Equals(object? obj) => obj is AnimationId other && Equals(other);
        public override int GetHashCode() => (int)FullId;
    }
}
