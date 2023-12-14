using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Memory
{
    /// <summary>
    /// Provides low-level memory reading and writing methods.
    /// </summary>
    public static unsafe class MemoryUtil
    {
        #region Memory Reading/Writing

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="address">The address to read from</param>
        /// <returns>The value read</returns>
        public static T Read<T>(nint address) where T : unmanaged
        {
            return *(T*)address;
        }

        /// <summary>
        /// Reads a pointer of type <typeparamref name="T"/>* from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the pointer</typeparam>
        /// <param name="address">The address to read from</param>
        /// <returns>The pointer read</returns>
        public static T* ReadPointer<T>(nint address) where T : unmanaged
        {
            return *(T**)address;
        }

        /// <summary>
        /// Gets a reference to a value of type <typeparamref name="T"/> from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="address">The address to read from</param>
        /// <returns>A reference to the value read</returns>
        public static ref T GetRef<T>(nint address) where T : unmanaged
        {
            return ref *(T*)address;
        }

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/>[] from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="address">The address to read from</param>
        /// <param name="count">The number of elements to read</param>
        /// <returns>The array read</returns>
        /// <remarks>This method copies the data into managed memory.</remarks>
        public static T[] ReadArray<T>(nint address, int count = 1) where T : unmanaged
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = Read<T>(address + i * sizeof(T));

            return array;
        }

        /// <summary>
        /// Reads a struct of type <typeparamref name="T"/> from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="address">The address to read from</param>
        /// <returns>The struct read</returns>
        /// <remarks>This method copies the data into managed memory.</remarks>
        public static T ReadStruct<T>(nint address) where T : struct
        {
            return Marshal.PtrToStructure<T>(address);
        }

        /// <summary>
        /// Reads an array of structs of type <typeparamref name="T"/>[] from a given address.
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="address">The address to read from</param>
        /// <param name="count">The number of elements to read</param>
        /// <returns>The array read</returns>
        /// <remarks>This method copies the data into managed memory.</remarks>
        public static T[] ReadStructArray<T>(nint address, int count = 1) where T : struct
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = ReadStruct<T>(address + i * Marshal.SizeOf<T>());

            return array;
        }

        #region Mirror Methods for long

        /// <inheritdoc cref="Read{T}(nint)"/>
        public static T Read<T>(long address) where T : unmanaged
        {
            return *(T*)address;
        }

        /// <inheritdoc cref="ReadPointer{T}(nint)"/>
        public static T* ReadPointer<T>(long address) where T : unmanaged
        {
            return *(T**)address;
        }

        /// <inheritdoc cref="GetRef{T}(nint)"/>
        public static ref T GetRef<T>(long address) where T : unmanaged
        {
            return ref *(T*)address;
        }

        /// <inheritdoc cref="ReadArray{T}(nint,int)"/>
        public static  T[] ReadArray<T>(long address, int count = 1) where T : unmanaged
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = Read<T>(address + i * sizeof(T));

            return array;
        }

        /// <inheritdoc cref="ReadStruct{T}(nint)"/>
        public static T ReadStruct<T>(long address) where T : struct
        {
            return Marshal.PtrToStructure<T>((nint)address);
        }

        /// <inheritdoc cref="ReadStructArray{T}(nint,int)"/>
        public static T[] ReadStructArray<T>(long address, int count = 1) where T : struct
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = ReadStruct<T>(address + i * Marshal.SizeOf<T>());

            return array;
        }

        #endregion

        /// <summary>
        /// Reads a specified number of bytes from a given address.
        /// </summary>
        /// <param name="address">The address to read from</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes read</returns>
        public static byte[] ReadBytes(nint address, int count)
        {
            var bytes = new byte[count];
            Marshal.Copy(address, bytes, 0, count);
            return bytes;
        }

        /// <summary>
        /// Writes a specified number of bytes to a given address.
        /// </summary>
        /// <remarks>Do not write to EXE regions using this method. Use <see cref="WriteBytesSafe"/> instead.</remarks>
        /// <param name="address">The address to write to</param>
        /// <param name="bytes">The bytes to write</param>
        public static void WriteBytes(nint address, byte[] bytes)
        {
            Marshal.Copy(bytes, 0, address, bytes.Length);
        }

        /// <summary>
        /// Writes a specified number of bytes to a given address.
        /// </summary>
        /// <remarks>This method changes the memory protection of the page prior to writing, and is thus safe to use on EXE regions.</remarks>
        /// <param name="address">The address to write to</param>
        /// <param name="bytes">The bytes to write</param>
        public static void WriteBytesSafe(nint address, byte[] bytes)
        {
            using var protection = new MemoryProtection(address, bytes.Length);
            Marshal.Copy(bytes, 0, address, bytes.Length);
        }

        #endregion

        #region Memory Allocation

        /// <summary>
        /// Allocates a block of native memory of the specified size.
        /// </summary>
        /// <param name="size">The size of the block to allocate</param>
        /// <returns>The address of the allocated block</returns>
        /// <remarks>
        /// Any memory allocated using this method must be freed using either <see cref="Free(nint)"/> or <see cref="Free(void*)"/>.
        /// to avoid memory leaks.
        /// </remarks>
        public static nint Alloc(long size)
        {
            return (nint)NativeMemory.Alloc((nuint)size);
        }

        /// <summary>
        /// Reallocates a block of native memory to the specified size.
        /// </summary>
        /// <param name="address">The address of the block to reallocate</param>
        /// <param name="newSize">The new size of the block</param>
        /// <returns>The address of the reallocated block</returns>
        public static nint Realloc(nint address, long newSize)
        {
            return (nint)NativeMemory.Realloc((void*)address, (nuint)newSize);
        }

        /// <summary>
        /// Allocates an array of type <typeparamref name="T"/>[] of the specified size in native memory.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="count">The size of the array</param>
        /// <returns>The address of the allocated array</returns>
        /// <remarks>
        /// Generally, if you need to allocate an array of native memory, you should use <see cref="NativeArray{T}.Create(int)"/>.
        /// If you <i>do</i> use this method, you must free the memory using <see cref="Free(nint)"/> or <see cref="Free(void*)"/>.
        /// </remarks>
        public static T* Alloc<T>(long count = 1) where T : unmanaged
        {
            return (T*)NativeMemory.Alloc((nuint)(count * sizeof(T)));
        }

        /// <summary>
        /// Reallocates an array of type <typeparamref name="T"/>[] to the specified size in native memory.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="address">The address of the array to reallocate</param>
        /// <param name="count">The new size of the array</param>
        /// <returns>The address of the reallocated array</returns>
        /// <remarks>
        /// Generally, if you need an array in native memory, you should use a <see cref="NativeArray{T}"/>
        /// with <see cref="NativeArray{T}.Resize(int)"/>.
        /// </remarks>
        public static T* Realloc<T>(T* address, long count) where T : unmanaged
        {
            return (T*)NativeMemory.Realloc(address, (nuint)(count * sizeof(T)));
        }

        /// <summary>
        /// Frees a block of native memory.
        /// </summary>
        /// <param name="address">The address of the block to free</param>
        public static void Free(nint address)
        {
            NativeMemory.Free((void*)address);
        }

        /// <summary>
        /// Frees a block of native memory.
        /// </summary>
        /// <param name="address">The address of the block to free</param>
        public static void Free(void* address)
        {
            NativeMemory.Free(address);
        }

        #endregion

        /// <summary>
        /// Creates a span around a native array from a given address and count.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="address">The address of the first element</param>
        /// <param name="count">The length of the array</param>
        /// <returns>A new span that represents the native array</returns>
        public static Span<T> AsSpan<T>(nint address, int count) where T : unmanaged
        {
            return new Span<T>((void*)address, count);
        }

        /// <inheritdoc cref="AsSpan{T}(nint,int)"/>
        public static Span<T> AsSpan<T>(long address, int count) where T : unmanaged
        {
            return new Span<T>((void*)address, count);
        }

        /// <summary>
        /// Converts a given reference to a pointer.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The reference to convert</param>
        /// <returns>A pointer of type <typeparamref name="T"/>* that points to <paramref name="value"/></returns>
        public static T* AsPointer<T>(ref T value) where T : unmanaged
        {
            return (T*)Unsafe.AsPointer(ref value);
        }

        /// <summary>
        /// Converts a given pointer to a reference.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="address">The pointer to convert</param>
        /// <returns>A reference of type <typeparamref name="T"/> that points to <paramref name="address"/></returns>
        public static ref T AsRef<T>(T* address) where T : unmanaged
        {
            return ref Unsafe.AsRef<T>(address);
        }

        /// <summary>
        /// Gets the address of a given reference.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The reference to convert</param>
        /// <returns>An address that points to <paramref name="value"/></returns>
        public static nint AddressOf<T>(ref T value) where T : unmanaged
        {
            return (nint)Unsafe.AsPointer(ref value);
        }
    }
}
