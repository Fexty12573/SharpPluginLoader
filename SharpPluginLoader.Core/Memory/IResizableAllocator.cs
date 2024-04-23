namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// Describes an allocator that can also reallocate memory blocks.
/// </summary>
public unsafe interface IResizableAllocator : IAllocator
{
    /// <summary>
    /// Reallocates a previously allocated memory block to the specified size.
    /// </summary>
    /// <param name="ptr">The pointer to the memory block to reallocate.</param>
    /// <param name="size">The new size of the memory block.</param>
    /// <returns>A pointer to the reallocated memory block.</returns>
    public void* Reallocate(void* ptr, nint size);

    /// <summary>
    /// Reallocates a previously allocated object array to the specified number of instances.
    /// </summary>
    /// <typeparam name="TObj">The type of object to reallocate.</typeparam>
    /// <param name="ptr">The pointer to the memory block to reallocate.</param>
    /// <param name="count">The new number of instances to allocate.</param>
    /// <returns>A pointer to the reallocated memory block.</returns>
    public TObj* Reallocate<TObj>(TObj* ptr, int count) where TObj : unmanaged;
}
