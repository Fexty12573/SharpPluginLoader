using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Memory
{
    public static unsafe class MemoryUtil
    {
        #region Memory Reading/Writing

        public static T Read<T>(this nint address, long offset = 0) where T : unmanaged
        {
            return *(T*)(address + offset);
        }

        public static ref T ReadRef<T>(this nint address, long offset = 0) where T : unmanaged
        {
            return ref *(T*)(address + offset);
        }

        public static T[] ReadArray<T>(this nint address, long offset = 0, int count = 1) where T : unmanaged
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = address.Read<T>(offset + i * sizeof(T));

            return array;
        }

        public static T ReadStruct<T>(this nint address, long offset = 0) where T : struct
        {
            return Marshal.PtrToStructure<T>(address + (nint)offset);
        }

        public static T[] ReadStructArray<T>(this nint address, long offset = 0, int count = 1) where T : struct
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = address.ReadStruct<T>(offset + i * Marshal.SizeOf<T>());

            return array;
        }

        public static T Read<T>(long address) where T : unmanaged
        {
            return *(T*)address;
        }

        public static  T[] ReadArray<T>(long address, int count = 1) where T : unmanaged
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = Read<T>(address + i * sizeof(T));

            return array;
        }

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

        public static nint Alloc(long size)
        {
            return (nint)NativeMemory.Alloc((nuint)size);
        }

        public static nint Realloc(nint address, long newSize)
        {
            return (nint)NativeMemory.Realloc((void*)address, (nuint)newSize);
        }

        public static T* Alloc<T>(long count = 1) where T : unmanaged
        {
            return (T*)NativeMemory.Alloc((nuint)(count * sizeof(T)));
        }

        public static T* Realloc<T>(T* address, long count) where T : unmanaged
        {
            return (T*)NativeMemory.Realloc(address, (nuint)(count * sizeof(T)));
        }

        public static void Free(nint address)
        {
            NativeMemory.Free((void*)address);
        }

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
    }
}
