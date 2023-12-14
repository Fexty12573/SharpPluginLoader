using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;

namespace BackgroundOptimize
{
    internal class AppExt
    {
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x144cb6230);

        public static bool GameFocused => SingletonInstance.Read<bool>(0x29AD);

        public static ref bool BackgroundMove => ref SingletonInstance.GetRef<bool>(0x38);

        public static ref bool BackgroundLowPower => ref SingletonInstance.GetRef<bool>(0x3A);
    }
}
