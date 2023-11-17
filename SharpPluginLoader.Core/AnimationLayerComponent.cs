using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

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
        
        public void Pause() => SpeedLocks[Instance] = 0f;

        public void Resume() => SpeedLocks.Remove(Instance);

        public unsafe void RegisterLmt(MotionList lmt, uint index)
        {
            if (index >= 16)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than 16");

            var owner = Owner?.Instance ?? 0;
            if (owner == 0)
                throw new InvalidOperationException("Cannot register LMT on an entity that is not initialized");

            RegisterLmtFunc.Invoke(owner, lmt.Instance, index);
        }

        internal static void Initialize()
        {
            _updateHook = Hook.Create<UpdateDelegate>(UpdateHook, 0x14224c150);
            _doLmtHook = Hook.Create<DoLmtDelegate>(DoLmtHook, 0x141c00720);
        }

        private static void UpdateHook(nint animLayer, int a, uint b, nint c, nint d, nint e, nint f, nint g)
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

            _updateHook.Original(animLayer, a, b, c, d, e, f, g);
        }

        private static void DoLmtHook(nint instance, uint animId, float startFrame, uint b, uint c, float d, int e)
        {
            var entity = new Entity(instance);
            AnimationId animIdObj = animId;

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnEntityAnimation))
                plugin.OnEntityAnimation(entity, ref animIdObj, ref startFrame);

            _doLmtHook.Original(instance, animIdObj, startFrame, b, c, d, e);
        }

        private delegate void UpdateDelegate(nint instance, int unk1, uint unk2, nint unk3, nint unk4, nint unk5, nint unk6, nint unk7);
        private delegate void DoLmtDelegate(nint instance, uint animId, float unk1, uint unk2, uint unk3, float unk4, int unk5);
        private static Hook<UpdateDelegate> _updateHook = null!;
        private static Hook<DoLmtDelegate> _doLmtHook = null!;
        private static readonly NativeAction<nint, nint, uint> RegisterLmtFunc = new(0x142236450);
        private static readonly Dictionary<nint, float> SpeedLocks = new();
    }

    public readonly struct AnimationId
    {
        public uint FullId { get; }

        public uint Lmt => FullId >> 12;
        public uint Id => FullId & 0xFFF;

        public AnimationId(uint fullId) => FullId = fullId;
        public AnimationId(uint lmt, uint id) => FullId = (lmt << 12) | (id & 0xFFF);

        public static implicit operator uint(AnimationId id) => id.FullId;
        public static implicit operator AnimationId(uint id) => new(id);

        public static bool operator ==(AnimationId a, AnimationId b) => a.FullId == b.FullId;
        public static bool operator !=(AnimationId a, AnimationId b) => a.FullId != b.FullId;

        public bool Equals(AnimationId other) => FullId == other.FullId;
        public override bool Equals(object? obj) => obj is AnimationId other && Equals(other);
        public override int GetHashCode() => (int)FullId;
        public override string ToString() => $"{Lmt}.{Id} ({FullId})";
    }
}
