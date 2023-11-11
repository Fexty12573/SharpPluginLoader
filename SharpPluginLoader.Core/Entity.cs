using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents a Monster Hunter World uCharacterModel instance.
    /// </summary>
    public class Entity : MtObject
    {
        public Entity(nint instance) : base(instance) { }
        public Entity() { }

        /// <summary>
        /// The position of the entity
        /// </summary>
        public MtVector3 Position
        {
            get => GetMtType<MtVector3>(0x160);
            set => SetMtType(0x160, value);
        }

        /// <summary>
        /// The size of the entity
        /// </summary>
        public MtVector3 Size
        {
            get => GetMtType<MtVector3>(0x180);
            set => SetMtType(0x180, value);
        }

        /// <summary>
        /// The position of the entity's collision box
        /// </summary>
        public MtVector3 CollisionPosition
        {
            get => GetMtType<MtVector3>(0xA50);
            set => SetMtType(0xA50, value);
        }

        /// <summary>
        /// The rotation of the entity
        /// </summary>
        public MtQuaternion Rotation
        {
            get => GetMtType<MtQuaternion>(0x170);
            set => SetMtType(0x170, value);
        }

        /// <summary>
        /// The entity's forward vector
        /// </summary>
        public MtVector3 Forward => Rotation * MtVector3.Forward;

        /// <summary>
        /// Teleports the entity to the given position
        /// </summary>
        /// <remarks>Use this function if you need to move a entity and ignore walls.</remarks>
        /// <param name="position">The target position</param>
        public void Teleport(MtVector3 position)
        {
            Position = position;
            CollisionPosition = position;
        }

        /// <summary>
        /// Resizes the entity on all axes to the given size
        /// </summary>
        /// <param name="size">The new size of the entity</param>
        public void Resize(float size)
        {
            Size = new MtVector3(size, size, size);
        }

        /// <summary>
        /// The current frame of the entity's current animation
        /// </summary>
        public float AnimationFrame
        {
            get => AnimationLayer?.CurrentFrame ?? 0;
            set
            {
                var animLayer = AnimationLayer;
                if (animLayer != null)
                    animLayer.CurrentFrame = value;
            }
        }

        /// <summary>
        /// The frame count of the entity's current animation
        /// </summary>
        public float MaxAnimationFrame
        {
            get => AnimationLayer?.MaxFrame ?? 0;
            set
            {
                var animLayer = AnimationLayer;
                if (animLayer != null)
                    animLayer.MaxFrame = value;
            }
        }

        /// <summary>
        /// The speed of the entity's current animation. Note, this value gets set every frame.
        /// </summary>
        public float AnimationSpeed
        {
            get => AnimationLayer?.Speed ?? 0;
            set
            {
                var animLayer = AnimationLayer;
                if (animLayer != null)
                    animLayer.Speed = value;
            }
        }

        /// <summary>
        /// Pauses the entity's current animation
        /// </summary>
        public void PauseAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Pause();
        }

        /// <summary>
        /// Resumes the entity's current animation
        /// </summary>
        public void ResumeAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Resume();
        }

        /// <summary>
        /// Creates an effect on the entity
        /// </summary>
        /// <param name="groupId">The efx group id</param>
        /// <param name="effectId">The efx id</param>
        public unsafe void CreateEffect(uint groupId, uint effectId)
        {
            var effectComponent = GetObject<MtObject>(0xA10);
            if (effectComponent == null)
                throw new InvalidOperationException("Monster does not have an effect component");

            var effect = effectComponent.GetObject<EffectProvider>(0x60)?.GetEffect(groupId, effectId);
            if (effect == null)
                throw new InvalidOperationException("Requested EFX does not exist in default EPV");

            CreateEffectFunc.Invoke(effectComponent.Instance, 0, effect.Instance, false);
        }

        /// <summary>
        /// Creates an effect on the entity from the given epv file
        /// </summary>
        /// <param name="epv">The EPV file to take the efx from</param>
        /// <param name="groupId">The efx group id</param>
        /// <param name="effectId">The efx id</param>
        /// <remarks><b>Tip:</b> You can load any EPV file using <see cref="ResourceManager.GetResource{T}"/></remarks>
        public unsafe void CreateEffect(EffectProvider epv, uint groupId, uint effectId)
        {
            var effectComponent = GetObject<MtObject>(0xA10);
            if (effectComponent == null)
                throw new InvalidOperationException("Monster does not have an effect component");

            var effect = epv.GetEffect(groupId, effectId);
            if (effect == null)
                throw new InvalidOperationException("Requested EFX does not exist in given EPV");

            CreateEffectFunc.Invoke(effectComponent.Instance, 0, effect.Instance, false);
        }

        /// <summary>
        /// Spawns a shell on the entity
        /// </summary>
        /// <param name="index">The index of the shell in the monsters shell list (shll)</param>
        /// <param name="target">The position the shell should travel towards</param>
        /// <param name="origin">The origin of the shell (or null for the entity itself)</param>
        public virtual void CreateShell(uint index, MtVector3 target, MtVector3? origin = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spawns a shell on the entity from the given shll file
        /// </summary>
        /// <param name="shll">The shll file to take the shell from</param>
        /// <param name="index">The index of the shell in the monsters shell list (shll)</param>
        /// <param name="target">The position the shell should travel towards</param>
        /// <param name="origin">The origin of the shell (or null for the entity itself)</param>
        /// <remarks><b>Tip:</b> You can load any shll file using <see cref="ResourceManager.GetResource{T}"/></remarks>
        public virtual void CreateShell(ShellParamList shll, uint index, MtVector3 target, MtVector3? origin = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spawns the given shell on the entity
        /// </summary>
        /// <param name="shell">The shell to spawn</param>
        /// <param name="target">The target position of the shell</param>
        /// <param name="origin">The origin position of the shell (or null for the entity itself)</param>
        public virtual void CreateShell(ShellParam shell, MtVector3 target, MtVector3? origin = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The entity's action controller
        /// </summary>
        public ActionController ActionController => GetInlineObject<ActionController>(0x61C8);

        /// <summary>
        /// The entity's animation component
        /// </summary>
        public AnimationLayerComponent? AnimationLayer => GetObject<AnimationLayerComponent>(0x468);


        private static readonly NativeFunction<nint, byte, nint, bool, nint> CreateEffectFunc = new(0x1412c5ee0);
    }
}
