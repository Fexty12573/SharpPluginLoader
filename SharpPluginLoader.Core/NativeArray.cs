using System.Collections;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;
using static SharpPluginLoader.Core.SpanExtensions;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// A wrapper around a native array. This type acts similar to a <see cref="Span{T}"/>,
    /// but can be stored in a field or property, and can take ownership of the memory it points to.
    /// </summary>
    public unsafe struct NativeArray<T> : IEnumerable<T>, IDisposable where T : unmanaged
    {
        /// <summary>
        /// Creates a new native array from a given address and count.
        /// </summary>
        /// <param name="address">The address of the first element</param>
        /// <param name="length">The number of elements in the array</param>
        public NativeArray(nint address, int length)
        {
            Address = address;
            Length = length;
            _ownsPointer = false;
            _allocator = null!;
        }

        /// <summary>
        /// Creates a new native array from a given address and count.
        /// </summary>
        /// <param name="address">The address of the first element</param>
        /// <param name="length">The number of elements in the array</param>
        /// <param name="allocator">The allocator to use</param>
        /// <remarks>
        /// Use this constructor if you want the native array to take ownership of the memory it points to.
        /// </remarks>
        public NativeArray(nint address, int length, IAllocator allocator)
        {
            Address = address;
            Length = length;
            _ownsPointer = true;
            _allocator = allocator;
        }

        /// <summary>
        /// Creates a new native array from a given address and count.
        /// </summary>
        /// <param name="length">The number of elements</param>
        /// <param name="allocator">The allocator to use. A <see langword="null"/> value uses the default allocator.</param>
        /// <returns>The newly allocated native array</returns>
        /// <remarks>
        /// <b>Warning:</b> The memory allocated by this method is not automatically freed. You must call <see cref="Dispose"/> when you are done with the array.
        /// Alternatively, use a <see langword="using"/> statement to ensure that the array is disposed.
        /// </remarks>
        public static NativeArray<T> Create(int length, IAllocator? allocator = null)
        {
            allocator ??= NativeMemoryAllocator.Instance;
            var ptr = allocator.Allocate<T>(length);
            return new NativeArray<T>((nint)ptr, length, allocator);
        }

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The address of the first element in the array.
        /// </summary>
        public nint Address { get; private set; }

        private readonly bool _ownsPointer;
        private readonly IAllocator _allocator;

        /// <summary>
        /// A pointer to the first element in the array.
        /// </summary>
        public T* Pointer
        {
            readonly get => (T*)Address;
            private set => Address = (nint)value;
        }

        /// <summary>
        /// Gets a reference to the element at the specified index.
        /// </summary>
        /// <param name="index">The index to get the element from</param>
        /// <returns>A reference to the element at the provided index</returns>
        public readonly ref T this[int index] => ref *(T*)(Address + index * sizeof(T));

        public readonly NativeArray<T> Slice(int start, int newLength)
        {
            return new NativeArray<T>(Address + start * sizeof(T), newLength);
        }

        /// <summary>
        /// Creates a span over the native array.
        /// </summary>
        /// <returns>A span over the native array.</returns>
        public readonly Span<T> AsSpan() => new((void*)Address, Length);

        /// <summary>
        /// Resizes the native array.
        /// </summary>
        /// <param name="newLength">The new size</param>
        /// <remarks>
        /// <b>Warning:</b> This method can <i>only</i> be called on NativeArrays that were created using the <see cref="Create"/> method.
        /// </remarks>
        public void Resize(int newLength)
        {
            Ensure.IsTrue(_ownsPointer);

            if (newLength == Length)
                return;

            // TODO: Should probably gradually shrink the array if the difference is large
            if (newLength < Length)
            {
                Length = newLength;
                return;
            }

            if (_allocator is IResizableAllocator resizableAllocator)
            {
                Pointer = resizableAllocator.Reallocate(Pointer, newLength);
            }
            else
            {
                var newPtr = _allocator.Allocate<T>(newLength);
                MemoryUtil.Copy(Pointer, newPtr, Length);
                _allocator.Free(Pointer);
                Pointer = newPtr;
            }

            Length = newLength;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Disposes the native array.
        /// </summary>
        public readonly void Dispose()
        {
            if (_ownsPointer)
                _allocator.Free(Pointer);
        }
    }

    /// <summary>
    /// A wrapper around a native array of pointers.
    /// </summary>
    /// <typeparam name="T">The type of the underlying elements</typeparam>
    public readonly unsafe struct PointerArray<T>(nint address, int length) where T : unmanaged
    {
        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length { get; } = length;

        /// <summary>
        /// The address of the first element in the array.
        /// </summary>
        public nint Address { get; } = address;

        /// <summary>
        /// A pointer to the first element in the array.
        /// </summary>
        public T** Pointer => (T**)Address;

        /// <summary>
        /// Gets a reference to the element at the specified index.
        /// </summary>
        public ref T this[int index] => ref *Pointer[index];

        /// <summary>
        /// Gets a pointer to the element at the specified index.
        /// </summary>
        public T* this[nint index] => Pointer[index];

        /// <summary>
        /// Gets an enumerator for the array.
        /// </summary>
        public readonly Enumerator GetEnumerator() => new(this);

        public struct Enumerator
        {
            private readonly PointerArray<T> _array;
            private int _index;

            internal Enumerator(PointerArray<T> array)
            {
                _array = array;
                _index = -1;
            }

            public readonly ref T Current => ref *_array.Pointer[_index];

            public bool MoveNext()
            {
                _index++;
                return _index < _array.Length;
            }
        }
    }

    public static class SpanExtensions
    {
        public static unsafe NativeArray<T> AsNativeArray<T>(this Span<T> span) where T : unmanaged
        {
            return new NativeArray<T>((nint)Unsafe.AsPointer(ref span[0]), span.Length);
        }

        public delegate bool SpanMatch<T>(ref T value);

        public static unsafe int FindIndex<T>(this Span<T> span, SpanMatch<T> match) where T : unmanaged
        {
            var ptr = MemoryUtil.AsPointer(ref span[0]);
            var end = ptr + span.Length;
            var i = 0;
            while (ptr < end)
            {
                if (match(ref *ptr))
                    return i;

                ptr++;
                i++;
            }

            return -1;
        }
    }
}
