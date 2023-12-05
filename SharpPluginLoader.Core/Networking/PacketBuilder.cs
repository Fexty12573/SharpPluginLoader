using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Networking
{
    public static unsafe class PacketBuilder
    {
        /// <summary>
        /// Builds a packet from the specified <see cref="IPacket"/> instance.
        /// </summary>
        /// <param name="packet">The packet to serialize.</param>
        /// <returns>The serialized packet as a buffer</returns>
        public static NetBuffer Build(IPacket packet)
        {
            // Explanation:
            // This buffer is later sent to the game's networking system.
            // It expects a cPacketBase object, which requires a virtual function table. The 8 extra bytes
            // are for the vtable pointer. The 6 extra bytes are for the packet's header (id, session, type),
            // and finally, the 4 extra bytes are for the packet's size.
            // We structure the packet such that it contains only the vtable pointer and raw, already formatted data.
            // This way we only need to copy the data into the real serializer buffer.
            // One more byte is added to the buffer to leave an empty byte at offset 0xC. The game normally writes the
            // SessionIndex here, but we don't need it since we already write it here, and instead we leave an empty byte and ignore that in
            // the native serializer.

            // Extra Bytes:
            // 8 - vtable pointer
            // 4 - packet size
            // 1 - padding byte
            // 6 - decoy packet header
            // 6 - real packet header
            var buffer = new NetBuffer(packet.RequiredSize + 8 + 4 + 1 + 6 + 6);

            // Write the vtable pointer
            buffer.WriteInt64NoBSwap(VtablePtr);

            // Write the packet's size
            buffer.WriteInt32NoBSwap(packet.RequiredSize);
            
            // Write padding byte at 0xC
            buffer.WriteByte(0);

            // Write the decoy packet header
            buffer.WriteUInt32(Utility.MakeDtiId("cPacketBase"));
            buffer.WriteByte(0);
            buffer.WriteByte(0);

            // Write the packet's header
            buffer.WriteUInt32(packet.Id);
            buffer.WriteByte((byte)packet.Session);
            buffer.WriteByte((byte)packet.Type);

            // Serialize the packet
            packet.Serialize(buffer);

            return buffer;
        }

        /// <summary>
        /// Builds a packet from the specified <see cref="NetBuffer"/> instance.
        /// </summary>
        /// <typeparam name="TPacket">The type of the packet to build.</typeparam>
        /// <param name="buffer">The buffer to deserialize.</param>
        /// <returns>The deserialized packet.</returns>
        public static TPacket Build<TPacket>(NetBuffer buffer) where TPacket : IPacket, new()
        {
            var packet = new TPacket();
            packet.Deserialize(buffer);
            return packet;
        }

        [UnmanagedCallersOnly]
        private static int NativeGetSize(nint packet)
        {
            return packet.Read<int>(0x8);
        }

        [UnmanagedCallersOnly]
        private static void NativeSerializer(nint packet, nint buffer)
        {
            var src = (void*)(packet + 8 + 4 + 1); // Skip vtable + size + padding byte
            var dst = (void*)buffer.Read<nint>(0x48);
            var size = (nuint)packet.Read<int>(0x8);

            NativeMemory.Copy(src, dst, size);
        }

        static PacketBuilder()
        {
            var vtable = new PacketVTable(&NativeGetSize, &NativeSerializer);
            VtablePtr = MemoryUtil.Alloc(sizeof(PacketVTable));
            Marshal.StructureToPtr(vtable, VtablePtr, false);
        }

        private static readonly nint VtablePtr;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PacketVTable(delegate* unmanaged<nint, int> getSize, delegate* unmanaged<nint, nint, void> serialize)
    {
        public fixed long Unused[5];
        public delegate* unmanaged<nint, int> GetSize = getSize;
        public delegate* unmanaged<nint, nint, void> Serialize = serialize;
    }
}
