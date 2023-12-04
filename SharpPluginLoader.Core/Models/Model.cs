using SharpPluginLoader.Core.MtTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Resources;

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
        /// The position of the model
        /// </summary>
        public ref MtVector3 Position => ref GetRef<MtVector3>(0x160);

        /// <summary>
        /// The size of the model
        /// </summary>
        public ref MtVector3 Size => ref GetRef<MtVector3>(0x180);

        /// <summary>
        /// The position of the model's collision box
        /// </summary>
        public ref MtVector3 CollisionPosition => ref GetRef<MtVector3>(0xA50);

        /// <summary>
        /// The rotation of the model
        /// </summary>
        public ref MtQuaternion Rotation => ref GetRef<MtQuaternion>(0x170);

        /// <summary>
        /// The model's forward vector
        /// </summary>
        public MtVector3 Forward => Rotation * MtVector3.Forward;

        /// <summary>
        /// Teleports the model to the given position
        /// </summary>
        /// <remarks>Use this function if you need to move a model and ignore walls.</remarks>
        /// <param name="position">The target position</param>
        public void Teleport(MtVector3 position)
        {
            Position = position;
            CollisionPosition = position;
        }

        /// <summary>
        /// Resizes the model on all axes to the given size
        /// </summary>
        /// <param name="size">The new size of the model</param>
        public void Resize(float size)
        {
            Size = new MtVector3(size, size, size);
        }

        /// <summary>
        /// The current frame of the model's current animation
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
        /// The frame count of the model's current animation
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
        /// The speed of the model's current animation. Note, this value gets set every frame.
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
        /// Gets the current animation of the model
        /// </summary>
        public ref AnimationId CurrentAnimation => ref GetRef<AnimationId>(0x55B8);

        /// <summary>
        /// Pauses the model's current animation
        /// </summary>
        public void PauseAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Pause();
        }

        /// <summary>
        /// Resumes the model's current animation
        /// </summary>
        public void ResumeAnimations()
        {
            var animLayer = AnimationLayer;
            if (animLayer != null)
                animLayer.Resume();
        }

        /// <summary>
        /// The model's animation component
        /// </summary>
        public AnimationLayerComponent? AnimationLayer => GetObject<AnimationLayerComponent>(0x468);

        /// <summary>
        /// The model's motion lists.
        /// </summary>
        public IEnumerable<MotionList> MotionLists
        {
            get
            {
                var animLayer = AnimationLayer;
                if (animLayer == null)
                    yield break;

                foreach (var lmt in animLayer.MotionLists)
                {
                    if (lmt != 0)
                        yield return new MotionList(lmt);
                }
            }
        }
    }
}
