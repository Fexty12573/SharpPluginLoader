using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Resources
{
    public class EffectProvider : Resource
    {
        public EffectProvider(nint instance) : base(instance) { }
        public EffectProvider() { }

        public unsafe MtObject? GetEffect(uint group, uint id)
        {
            var effectPtr = GetEffectFunc.Invoke(Instance, group, id);
            return effectPtr != 0 ? new MtObject(effectPtr) : null;
        }


        private static readonly NativeFunction<nint, uint, uint, nint> GetEffectFunc = new(0x1423621e0);
    }
}
