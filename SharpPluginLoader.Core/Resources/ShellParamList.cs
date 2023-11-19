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

        /// <summary>
        /// Gets a shell by its index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The shell at the given index, or null</returns>
        public unsafe ShellParam? GetShell(uint index)
        {
            // GetShellFunc is a native function that returns a pointer to a cShellParamList object.
            // The actual shell is stored at offset 8 from this pointer.
            var shell = GetShellFunc.Invoke(Instance, index);
            if (shell == 0)
                return null;

            var shellObj = shell.Read<nint>(0x8);
            return shellObj == 0 ? null : new ShellParam(shellObj);
        }

        private static readonly NativeFunction<nint, uint, nint> GetShellFunc = new(0x140f7cf20);
    }
}
