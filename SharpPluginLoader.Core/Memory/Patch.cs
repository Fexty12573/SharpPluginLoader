using Reloaded.Assembler;

namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// Represents a patch in memory.
/// </summary>
public readonly struct Patch : IDisposable
{
    /// <summary>
    /// The address of the patch.
    /// </summary>
    public readonly nint Address;

    /// <summary>
    /// The original bytes at the patch address.
    /// </summary>
    public readonly byte[] OriginalBytes;

    /// <summary>
    /// The bytes that will be written to the patch address.
    /// </summary>
    public readonly byte[] PatchedBytes;

    /// <summary>
    /// Creates a new patch.
    /// </summary>
    /// <param name="address">The address of the patch.</param>
    /// <param name="patchedBytes">The bytes that will be written to the patch address.</param>
    /// <param name="enable">Whether to enable the patch.</param>
    public Patch(nint address, byte[] patchedBytes, bool enable = false)
    {
        Address = address;
        OriginalBytes = MemoryUtil.ReadBytes(address, patchedBytes.Length);
        PatchedBytes = patchedBytes;

        if (enable) Enable();
    }

    /// <summary>
    /// Creates a new patch.
    /// </summary>
    /// <param name="address">The address of the patch.</param>
    /// <param name="asm">The assembly that will be written to the patch address.</param>
    /// <param name="enable">Whether to enable the patch.</param>
    public Patch(nint address, string asm, bool enable = false)
    {
        Address = address;
        PatchedBytes = Assembler.Assemble(asm);
        Ensure.NotNull(PatchedBytes);

        OriginalBytes = MemoryUtil.ReadBytes(Address, PatchedBytes.Length);

        if (enable) Enable();
    }

    /// <summary>
    /// Creates a new patch.
    /// </summary>
    /// <param name="address">The address of the patch.</param>
    /// <param name="asm">The assembly that will be written to the patch address. Each line represented by a string.</param>
    /// <param name="enable">Whether to enable the patch.</param>
    public Patch(nint address, IEnumerable<string> asm, bool enable = false)
    {
        Address = address;
        PatchedBytes = Assembler.Assemble(asm);
        Ensure.NotNull(PatchedBytes);

        OriginalBytes = MemoryUtil.ReadBytes(Address, PatchedBytes.Length);

        if (enable) Enable();
    }

    /// <summary>
    /// Whether the patch is currently enabled.
    /// </summary>
    public bool IsEnabled => MemoryUtil.ReadBytes(Address, PatchedBytes.Length).SequenceEqual(PatchedBytes);

    /// <summary>
    /// Enables the patch.
    /// </summary>
    public void Enable() => MemoryUtil.WriteBytesSafe(Address, PatchedBytes);

    /// <summary>
    /// Disables the patch.
    /// </summary>
    public void Disable() => MemoryUtil.WriteBytesSafe(Address, OriginalBytes);

    public void Dispose()
    {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources()
    {
        if (IsEnabled)
            Disable();
    }

    private static readonly Assembler Assembler = new();
}
