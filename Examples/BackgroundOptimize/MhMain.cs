using SharpPluginLoader.Core.Memory;

namespace BackgroundOptimize
{
    internal class MhMain
    {
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x1451c1f08);

        public static ref float MaxFps => ref MemoryUtil.GetRef<float>(SingletonInstance + 0x5C);
    }
}
