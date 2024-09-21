using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;

namespace BackgroundOptimize
{
    internal class MhMain : MtObject
    {
        public MhMain(nint instance) : base(instance) { }
        public MhMain() { }

        public ref float MaxFps => ref GetRef<float>(0x5C);
    }
}
