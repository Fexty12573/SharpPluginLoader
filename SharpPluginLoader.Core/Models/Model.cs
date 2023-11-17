using SharpPluginLoader.Core.MtTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Models
{
    /// <summary>
    /// Represents an instance of a uMhModel class
    /// </summary>
    public class Model : MtObject
    {
        public Model(nint instance) : base(instance) { }
        public Model() { }

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

        public AnimationId CurrentAnimation => Get<AnimationId>(0x55B8);

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
        /// The entity's animation component
        /// </summary>
        public AnimationLayerComponent? AnimationLayer => GetObject<AnimationLayerComponent>(0x468);
    }
}
