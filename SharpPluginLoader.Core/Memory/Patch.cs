using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    public readonly struct Patch
    {
        public readonly nint Address;
        public readonly byte[] OriginalBytes;
        public readonly byte[] PatchedBytes;

        public Patch(nint address, byte[] patchedBytes, bool enable = false)
        {
            Address = address;
            OriginalBytes = MemoryUtil.ReadBytes(address, patchedBytes.Length);
            PatchedBytes = patchedBytes;

            if (enable) Enable();
        }

        public Patch(long address, byte[] patchedBytes, bool enable = false)
        {
            Address = (nint)address;
            OriginalBytes = MemoryUtil.ReadBytes(Address, patchedBytes.Length);
            PatchedBytes = patchedBytes;

            if (enable) Enable();
        }

        public void Enable() => MemoryUtil.WriteBytesSafe(Address, PatchedBytes);

        public void Disable() => MemoryUtil.WriteBytesSafe(Address, OriginalBytes);
    }
}
