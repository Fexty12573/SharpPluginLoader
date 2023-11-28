using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of the rEffectProvider class.
    /// </summary>
    public class EffectProvider : Resource
    {
        public EffectProvider(nint instance) : base(instance) { }
        public EffectProvider() { }

        /// <summary>
        /// Gets an effect by its group and id which can be passed to <see cref="Entity.CreateEffect(MtObject)"/>.
        /// </summary>
        /// <param name="group">The group the effect is in</param>
        /// <param name="id">The id of the effect</param>
        /// <returns>The effect or null</returns>
        public unsafe MtObject? GetEffect(uint group, uint id)
        {
            var effectPtr = GetEffectFunc.Invoke(Instance, group, id);
            return effectPtr != 0 ? new MtObject(effectPtr) : null;
        }


        private static readonly NativeFunction<nint, uint, uint, nint> GetEffectFunc = new(AddressRepository.Get("EffectProvider:GetEffect"));
    }
}
