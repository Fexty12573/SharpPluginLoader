using SharpPluginLoader.Core.MtTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// The position of the entitiy
        /// </summary>
        public MtVector3 Position
        {
            get => GetMtType<MtVector3>(0x160);
            set => SetMtType(0x160, value);
        }

        /// <summary>
        /// The size of the entitiy
        /// </summary>
        public MtVector3 Size
        {
            get => GetMtType<MtVector3>(0x180);
            set => SetMtType(0x180, value);
        }

        /// <summary>
        /// The position of the entitiy's collision box
        /// </summary>
        public MtVector3 CollisionPosition
        {
            get => GetMtType<MtVector3>(0xA50);
            set => SetMtType(0xA50, value);
        }

        /// <summary>
        /// The rotation of the entitiy
        /// </summary>
        public MtVector4 Rotation // TODO: Change to MtQuaternion
        {
            get => GetMtType<MtVector4>(0x170);
            set => SetMtType(0x170, value);
        }

        /// <summary>
        /// Teleports the entitiy to the given position
        /// </summary>
        /// <remarks>Use this function if you need to move a entitiy and ignore walls.</remarks>
        /// <param name="position">The target position</param>
        public void Teleport(MtVector3 position)
        {
            Position = position;
            CollisionPosition = position;
        }

        /// <summary>
        /// Resizes the entitiy on all axes to the given size
        /// </summary>
        /// <param name="size">The new size of the entitiy</param>
        public void Resize(float size)
        {
            Size = new MtVector3(size, size, size);
        }

        /// <summary>
        /// The current frame of the entitiy's current animation
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
        /// The frame count of the entitiy's current animation
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
        /// The speed of the entitiy's current animation. Note, this value gets set every frame.
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
        /// Pauses the entitiy's current animation
        /// </summary>
        public void PauseAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Pause();
        }

        /// <summary>
        /// Resumes the entitiy's current animation
        /// </summary>
        public void ResumeAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Resume();
        }

        public ActionController ActionController => GetInlineObject<ActionController>(0x61C8);

        public AnimationLayerComponent? AnimationLayer => GetObject<AnimationLayerComponent>(0x468);
    }
}
