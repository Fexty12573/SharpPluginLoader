using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Bootstrapper.Chunk
{
    internal static class BinaryExtensions
    {
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf<T>());
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return result;
        }

        public static void WriteStruct<T>(this BinaryWriter writer, T value)
        {
            Debug.Assert(value != null);

            var bytes = new byte[Marshal.SizeOf<T>()];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            handle.Free();
            writer.Write(bytes);
        }
    }
}
