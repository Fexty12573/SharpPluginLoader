using Reloaded.Memory.Kernel32;
using ReloadedMemory = Reloaded.Memory.Sources.Memory;

namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// Temporarily changes the protection of a given region of memory.
/// </summary>
/// <remarks>
/// This class should always be used in a <see langword="using"/> context.
/// You should only ever have ONE of these active per memory region at a time!
/// </remarks>
/// <example>
/// <code>
/// using (var prot = MemoryProtection(0x141234567, 0x100))
/// {
///     // Make changes to the memory
/// }
/// </code>
/// </example>
public class MemoryProtection : IDisposable
{
    private readonly nint _address;
    private readonly int _size;
    private readonly Kernel32.MEM_PROTECTION _oldProtect;

    /// <param name="address">The start address of the memory region to adjust</param>
    /// <param name="size">The size of the memory region to adjust</param>
    /// <param name="protection">The protection value to change it to.</param>
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
