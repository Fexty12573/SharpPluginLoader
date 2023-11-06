using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public static class Gui
    {
        public delegate void PromptCallback(PromptResult result);

        public static unsafe nint SingletonInstance => *(nint*)0x1451c2400;
    }

    public enum PromptResult
    {
        Yes,
        No,
        Cancel
    }
}
