using System.Collections;
using System.Formats.Tar;

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
    public readonly unsafe struct NativeArray<T>(nint pointer, int length) : IEnumerable<T> where T : unmanaged
    {
        public int Length { get; } = length;

        public nint Pointer { get; } = pointer;

        public ref T this[int index] => ref *(T*)(Pointer + index * sizeof(T));

        public Span<T> AsSpan() => new((void*)Pointer, Length);

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
