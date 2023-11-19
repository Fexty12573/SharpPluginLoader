namespace SharpPluginLoader.Core.Memory
{
    public readonly struct Patch : IDisposable
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

        public bool IsEnabled => MemoryUtil.ReadBytes(Address, PatchedBytes.Length).SequenceEqual(PatchedBytes);

        public void Enable() => MemoryUtil.WriteBytesSafe(Address, PatchedBytes);

        public void Disable() => MemoryUtil.WriteBytesSafe(Address, OriginalBytes);

        private void ReleaseUnmanagedResources()
        {
            if (IsEnabled)
                Disable();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}
