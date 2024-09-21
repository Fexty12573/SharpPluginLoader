using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;

namespace BackgroundOptimize
{
    internal class AppExt : MtObject
    {
        public AppExt(nint instance) : base(instance) { }
        public AppExt() { }

        public bool GameFocused => Get<bool>(0x29AD);
        public ref bool BackgroundMove => ref GetRef<bool>(0x38);
        public ref bool BackgroundLowPower => ref GetRef<bool>(0x3A);
    }
}
