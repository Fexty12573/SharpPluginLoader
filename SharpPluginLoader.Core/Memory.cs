using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public static class Memory
    {
        public static unsafe T Read<T>(this nint address, long offset = 0) where T : unmanaged
        {
            return *(T*)(address + offset);
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

        public static unsafe T Read<T>(long address, long offset = 0) where T : unmanaged
        {
            return *(T*)(address + offset);
        }
    }
}
