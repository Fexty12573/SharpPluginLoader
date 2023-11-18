using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Memory
{
    public static class MemoryUtil
    {
        public static unsafe T Read<T>(this nint address, long offset = 0) where T : unmanaged
        {
            return *(T*)(address + offset);
        }

        public static unsafe ref T ReadRef<T>(this nint address, long offset = 0) where T : unmanaged
        {
            return ref *(T*)(address + offset);
        }

        public static unsafe T[] ReadArray<T>(this nint address, long offset = 0, int count = 1) where T : unmanaged
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

        public static unsafe T Read<T>(long address) where T : unmanaged
        {
            return *(T*)address;
        }

        public static unsafe T[] ReadArray<T>(long address, int count = 1) where T : unmanaged
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
    }
}
