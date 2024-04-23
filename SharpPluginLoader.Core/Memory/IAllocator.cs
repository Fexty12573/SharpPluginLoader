namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// Describes an allocator that can allocate and free unmanaged memory.
/// </summary>
public unsafe interface IAllocator
{
    /// <summary>
    /// Allocates a block of memory of the specified size.
    /// </summary>
    /// <param name="size">The size of the memory block to allocate.</param>
    /// <returns>A pointer to the allocated memory block.</returns>
    public void* Allocate(nint size);

    /// <summary>
    /// Allocates <paramref name="count"/> instances of <typeparamref name="TObj"/>.
    /// </summary>
    /// <typeparam name="TObj">The type of object to allocate.</typeparam>
    /// <param name="count">The number of instances to allocate.</param>
    /// <returns>A pointer to the allocated memory block.</returns>
    public TObj* Allocate<TObj>(int count) where TObj : unmanaged;

    /// <summary>
    /// Frees a previously allocated memory block.
    /// </summary>
    /// <param name="ptr">The pointer to the memory block to free.</param>
    public void Free(void* ptr);
}
