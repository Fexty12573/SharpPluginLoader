using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    internal static class InternalCalls
    {
        public static unsafe delegate* unmanaged<void> TestInternalCallPtr;

        public static unsafe void TestInternalCall() => TestInternalCallPtr();
    }
}
