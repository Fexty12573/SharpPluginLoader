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
        /// Gets the number of shells in this list.
        /// </summary>
        public int ShellCount => Get<int>(0xB0);

        /// <summary>
        /// Gets all shells in this list.
        /// </summary>
        public ShellParam[] Shells
        {
            get
            {
                var count = ShellCount;
                if (count == 0)
                    return [];

                var begin = Get<nint>(0xA8);
                var end = begin + count * 0x10;
                var shells = new ShellParam[count];
                for (var i = 0; begin < end; begin += 0x10)
                {
                    var shell = begin.Read<nint>(0x8);
                    if (shell != 0)
                        shells[i++] = new ShellParam(shell);
                }

                return shells;
            }
        }

        /// <summary>
        /// Gets a shell by its index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The shell at the given index, or null</returns>
        public unsafe ShellParam? GetShell(uint index)
        {
            // GetShellFunc is a native function that returns a pointer to a cShellParamList object.
            // The actual shell is stored at offset 8 from this pointer.
            var shell = GetShellFunc.InvokeUnsafe(Instance, index);
            if (shell == 0)
                return null;

            var shellObj = shell.Read<nint>(0x8);
            return shellObj == 0 ? null : new ShellParam(shellObj);
        }

        private static readonly NativeFunction<nint, uint, nint> GetShellFunc = new(AddressRepository.Get("ShellParamList:GetShell"));
    }
}
