using Reloaded.Memory.Kernel32;
using ReloadedMemory = Reloaded.Memory.Sources.Memory;

namespace SharpPluginLoader.Core.Memory
{
    internal class MemoryProtection : IDisposable
    {
        private readonly nint _address;
        private readonly int _size;
        private readonly Kernel32.MEM_PROTECTION _oldProtect;

        public MemoryProtection(nint address, int size, Kernel32.MEM_PROTECTION protection = Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE)
        {
            _address = address;
            _size = size;
            _oldProtect = ReloadedMemory.CurrentProcess.ChangePermission((nuint)address, size, protection);
        }

        private void ReleaseUnmanagedResources()
        {
            ReloadedMemory.CurrentProcess.ChangePermission((nuint)_address, _size, _oldProtect);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~MemoryProtection()
        {
            ReleaseUnmanagedResources();
        }
    }
}
