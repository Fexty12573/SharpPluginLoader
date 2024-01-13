using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.Entities
{
    /// <summary>
    /// Represents a Monster Hunter World uCharacterModel instance.
    /// </summary>
    public class Entity : Model
    {
        public Entity(nint instance) : base(instance) { }
        public Entity() { }

        /// <summary>
        /// Gets the entity's collision component
        /// </summary>
        public CollisionComponent? CollisionComponent => GetObject<CollisionComponent>(0x6A8);

        /// <summary>
        /// Creates an effect on the entity
        /// </summary>
        /// <param name="groupId">The efx group id</param>
        /// <param name="effectId">The efx id</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CreateEffect(uint groupId, uint effectId)
        {
            var effectComponent = GetObject<MtObject>(0xA10);
            if (effectComponent is null)
                throw new InvalidOperationException("Entity does not have an effect component");

            var effect = effectComponent.GetObject<EffectProvider>(0x60)?.GetEffect(groupId, effectId);
            if (effect is null)
                throw new InvalidOperationException("Requested EFX does not exist in default EPV");

            CreateEffect(effect);
        }

        /// <summary>
        /// Creates an effect on the entity from the given epv file
        /// </summary>
        /// <param name="epv">The EPV file to take the efx from</param>
        /// <param name="groupId">The efx group id</param>
        /// <param name="effectId">The efx id</param>
        /// <remarks><b>Tip:</b> You can load any EPV file using <see cref="ResourceManager.GetResource{T}"/></remarks>
        /// <exception cref="InvalidOperationException"></exception>
        public void CreateEffect(EffectProvider epv, uint groupId, uint effectId)
        {
            var effectComponent = GetObject<MtObject>(0xA10);
            if (effectComponent is null)
                throw new InvalidOperationException("Entity does not have an effect component");

            var effect = epv.GetEffect(groupId, effectId);
            if (effect is null)
                throw new InvalidOperationException("Requested EFX does not exist in given EPV");

            CreateEffect(effect);
        }

        /// <summary>
        /// Creates the given effect on the entity
        /// </summary>
        /// <param name="effect">The effect to create</param>
        /// <exception cref="InvalidOperationException"></exception>
        public unsafe void CreateEffect(MtObject effect)
        {
            var effectComponent = GetObject<MtObject>(0xA10);
            if (effectComponent is null)
                throw new InvalidOperationException("Entity does not have an effect component");

            CreateEffectFunc.Invoke(effectComponent.Instance, 0, effect.Instance, false);
        }

        /// <summary>
        /// Spawns a shell on the entity
        /// </summary>
        /// <param name="shllIndex">The index of the shll in the entities shll list</param>
        /// <param name="shlpIndex">The index of the shlp in the shll</param>
        /// <param name="target">The position the shell should travel towards</param>
        /// <param name="origin">The origin of the shell (or null for the entity itself)</param>
        public virtual unsafe void CreateShell(int shllIndex, int shlpIndex, MtVector3 target, MtVector3? origin = null)
        {
            var shellParams = ShellCreationParams.Create(target, origin ?? Position);
            CreateShellFunc.Invoke(ShllArray.Address, shllIndex, shlpIndex, Instance, Instance, MemoryUtil.AddressOf(ref shellParams));
        }

        /// <summary>
        /// Spawns a shell on the entity
        /// </summary>
        /// <param name="shllIndex">The index of the shll in the entities shll list</param>
        /// <param name="shlpIndex">The index of the shlp in the shll</param>
        public virtual unsafe void CreateShell(int shllIndex, int shlpIndex)
        {
            CreateShellFunc.Invoke(
                ShllArray.Address,
                shllIndex,
                shlpIndex,
                Instance,
                Instance,
                MemoryUtil.AddressOf(ref ShellCreationParams.Default())
            );
        }

        /// <summary>
        /// Spawns a shell on the entity
        /// </summary>
        /// <param name="index">The index of the shell in the entities shell list (shll)</param>
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
        /// <param name="index">The index of the shell in the entities shell list (shll)</param>
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
        /// Registers a shll file on the entity
        /// </summary>
        /// <param name="shll">The shll to register</param>
        /// <param name="index">
        /// The index at which to register, must be less than 8. Passing -1 for this parameter will register the shll at the first free index.
        /// </param>
        /// <param name="force">Whether to force the registration or not (only works when an index is specified)</param>
        /// <returns>True if the shll was successfully registered</returns>
        public bool RegisterShll(ShellParamList shll, int index = -1, bool force = false)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 8);

            var shllArray = ShllArray;
            if (index == -1)
            {
                for (var i = 0; i < shllArray.Length; ++i)
                {
                    if (shllArray[i] == 0)
                    {
                        shll.AddRef();
                        shllArray[i] = shll.Instance;
                        return true;
                    }
                }

                return false;
            }

            if (shllArray[index] != 0)
            {
                if (force)
                {
                    var oldShll = new ShellParamList(shllArray[index]);
                    oldShll.Release();

                    shll.AddRef();
                    shllArray[index] = shll.Instance;
                    return true;
                }

                return false;
            }

            shll.AddRef();
            shllArray[index] = shll.Instance;
            return true;
        }

        /// <summary>
        /// Registers a shll file on the entity
        /// </summary>
        /// <param name="path">The path to the shll file</param>
        /// <param name="index">
        /// The index at which to register, must be less than 8. Passing -1 for this parameter will register the shll at the first free index.
        /// </param>
        /// <returns>True if the shll was successfully registered</returns>
        public unsafe bool RegisterShll(string path, int index = -1)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 8);

            var shllArray = ShllArray;
            if (index == -1)
            {
                for (var i = 0; i < shllArray.Length; ++i)
                {
                    if (shllArray[i] == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index == -1)
                return false;

            RegisterShllFunc.Invoke(index, shllArray.Address, path, 0x801);
            return true;
        }

        /// <summary>
        /// The entity's action controller
        /// </summary>
        public ActionController ActionController => GetInlineObject<ActionController>(0x61C8);


        private NativeArray<nint> ShllArray => new(Instance + 0x56E8, 8);
        private static readonly NativeFunction<nint, byte, nint, bool, nint> CreateEffectFunc = new(AddressRepository.Get("Entity:CreateEffect"));
        private static readonly NativeFunction<nint, int, int, nint, nint, nint, nint> CreateShellFunc = new(AddressRepository.Get("Entity:CreateShell"));
        private static readonly NativeAction<int, nint, string, uint> RegisterShllFunc = new(AddressRepository.Get("Entity:RegisterShll"));

        internal unsafe struct ShellCreationParams
        {
            public static ref ShellCreationParams Default() => ref MemoryUtil.GetRef<ShellCreationParams>(0x143f939e0);
            public static ShellCreationParams Create(MtVector3 target, MtVector3 origin)
            {
                ShellCreationParams defaultParams = Default();
                defaultParams.Populate(target, origin);

                return defaultParams;
            }

            public fixed float Values[74];

            private T* As<T>(long offset = 0) where T : unmanaged
            {
                fixed (float* ptr = Values)
                    return (T*)((nint)ptr + offset);
            }

            public ShellCreationParams(MtVector3 target, MtVector3 origin)
            {
                Values[0] = origin.X;
                Values[1] = origin.Y;
                Values[2] = origin.Z;
                Values[3] = 0f;

                *As<bool>(4 * 4) = true;
                var values = As<float>(0x40);
                values[0] = target.X;
                values[1] = target.Y;
                values[2] = target.Z;
                values[3] = 0f;
                *As<bool>(0x50) = true;

                var ivals = As<int>(0xA0);
                ivals[0] = 0x12;
                ivals[1] = -1;
                ivals[2] = -1;
            }

            private void Populate(MtVector3 target, MtVector3 origin)
            {
                Values[0] = origin.X;
                Values[1] = origin.Y;
                Values[2] = origin.Z;
                Values[3] = 0f;

                *As<bool>(4 * 4) = true;
                var values = As<float>(0x40);
                values[0] = target.X;
                values[1] = target.Y;
                values[2] = target.Z;
                values[3] = 0f;
                *As<bool>(0x50) = true;

                var ivals = As<int>(0xA0);
                ivals[0] = 0x12;
                ivals[1] = -1;
                ivals[2] = -1;
            }
        }
    }
}
