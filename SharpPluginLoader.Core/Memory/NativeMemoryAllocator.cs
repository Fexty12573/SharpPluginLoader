
namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// The default unmanaged memory allocator for SharpPluginLoader.
/// </summary>
public unsafe class NativeMemoryAllocator : IResizableAllocator
{
    /// <summary>
    /// The singleton instance of this allocator.
    /// </summary>
    /// <remarks>
    /// Use this any time you need to use <see cref="NativeMemoryAllocator"/>.
    /// Don't create new instances of this class.
    /// </remarks>
    public static NativeMemoryAllocator Instance { get; } = new();

    /// <inheritdoc cref="IAllocator.Allocate(nint)"/>
    public void* Allocate(nint size)
    {
        return MemoryUtil.Alloc(size).ToPointer();
    }

    /// <inheritdoc cref="IAllocator.Allocate{TObj}(int)"/>
    public TObj* Allocate<TObj>(int count = 1) where TObj : unmanaged
    {
        return MemoryUtil.Alloc<TObj>(count);
    }

    /// <inheritdoc cref="IResizableAllocator.Reallocate"/>
    public void* Reallocate(void* ptr, nint size)
    {
        return MemoryUtil.Realloc((nint)ptr, size).ToPointer();
    }

    /// <inheritdoc cref="IResizableAllocator.Reallocate{TObj}"/>
    public TObj* Reallocate<TObj>(TObj* ptr, int count) where TObj : unmanaged
    {
        return MemoryUtil.Realloc(ptr, count);
    }

    /// <inheritdoc cref="IAllocator.Free"/>
    public void Free(void* ptr)
    {
        MemoryUtil.Free(ptr);
    }

    // Prevent instantiation
    private NativeMemoryAllocator() { }
}
