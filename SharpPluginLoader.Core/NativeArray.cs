using System.Collections;
using System.Formats.Tar;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// A wrapper around a native array.
    /// </summary>
    /// <typeparam name="T">The type of the underlying elements</typeparam>
    /// <param name="pointer">The address of the first element</param>
    /// <param name="length">The number of elements in the array</param>
    /// <remarks>
    /// <b>Note:</b> If you don't need to use the array in an iterator, use a <see cref="Span{T}"/> instead.
    /// </remarks>
    public readonly unsafe struct NativeArray<T>(nint pointer, int length, bool ownsPointer = false) : IEnumerable<T>, IDisposable where T : unmanaged
    {
        /// <summary>
        /// Creates a new native array from a given address and count.
        /// </summary>
        /// <param name="length">The number of elements</param>
        /// <returns>The newly allocated native array</returns>
        /// <remarks>
        /// <b>Warning:</b> The memory allocated by this method is not automatically freed. You must call <see cref="Dispose"/> when you are done with the array.
        /// Alternatively, use a using statement to ensure that the array is disposed.
        /// </remarks>
        public static NativeArray<T> Create(int length)
        {
            var ptr = MemoryUtil.Alloc<T>(length);
            return new NativeArray<T>((nint)ptr, length, true);
        }

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length { get; } = length;

        /// <summary>
        /// The address of the first element in the array.
        /// </summary>
        public nint Pointer { get; } = pointer;

        /// <summary>
        /// Gets a reference to the element at the specified index.
        /// </summary>
        /// <param name="index">The index to get the element from</param>
        /// <returns>A reference to the element at the provided index</returns>
        public ref T this[int index] => ref *(T*)(Pointer + index * sizeof(T));

        /// <summary>
        /// Creates a span over the native array.
        /// </summary>
        /// <returns>A span over the native array.</returns>
        public Span<T> AsSpan() => new((void*)Pointer, Length);

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Disposes the native array.
        /// </summary>
        public void Dispose()
        {
            if (ownsPointer)
                MemoryUtil.Free(Pointer);
        }
    }
}
