using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of a rShellParam resource (a .shlp file)
    /// </summary>
    public class ShellParam : Resource
    {
        public ShellParam(nint instance) : base(instance) { }
        public ShellParam() { }
    }
}
