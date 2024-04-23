using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core.Memory;

/// <summary>
/// Represents an instance of an MtAllocator, the default memory allocator for the game.
/// </summary>
public unsafe class MtAllocator : MtObject, IAllocator
{
    public MtAllocator(nint instance) : base(instance) { }
    public MtAllocator() { }

    /// <inheritdoc cref="IAllocator.Allocate(nint)"/>
    /// <remarks>
    /// Memory allocated with this method will always be aligned to 16 bytes.
    /// </remarks>
    public void* Allocate(nint size)
    {
        return Allocate(size, 0x10);
    }

    /// <summary>
    /// Allocates a block of memory of the specified size and alignment.
    /// </summary>
    /// <param name="size">The size of the memory block to allocate.</param>
    /// <param name="alignment">The alignment of the memory block to allocate.</param>
    /// <returns>A pointer to the allocated memory block.</returns>
    public void* Allocate(nint size, int alignment)
    {
        return GetAllocateFunction().Invoke(Instance, size, alignment).ToPointer();
    }

    /// <inheritdoc cref="IAllocator.Allocate{TObj}(int)"/>
    public TObj* Allocate<TObj>(int count = 1) where TObj : unmanaged
    {
        return (TObj*)Allocate(count * sizeof(TObj));
    }

    /// <inheritdoc cref="IAllocator.Free"/>
    public void Free(void* ptr)
    {
        GetFreeFunction().Invoke(Instance, (nint)ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NativeFunction<nint, nint, int, nint> GetAllocateFunction()
    {
        return new NativeFunction<IntPtr, IntPtr, int, IntPtr>(GetVirtualFunction(9));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NativeAction<nint, nint> GetFreeFunction()
    {
        return new NativeAction<IntPtr, IntPtr>(GetVirtualFunction(13));
    }
}
