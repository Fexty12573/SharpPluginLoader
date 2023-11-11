using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of a rShellParamList resource (a .shll file)
    /// </summary>
    public class ShellParamList : Resource
    {
        public ShellParamList(nint instance) : base(instance) { }
        public ShellParamList() { }

        public unsafe ShellParam? GetShell(uint index)
        {
            // GetShellFunc is a native function that returns a pointer to a cShellParamList object.
            // The actual shell is stored at offset 8 from this pointer.
            var shell = GetShellFunc.Invoke(Instance, index);
            return shell == 0 ? null : new ShellParam(shell.Read<nint>(0x8));
        }

        private static readonly NativeFunction<nint, uint, nint> GetShellFunc = new(0x140f7cf20);
    }
}
