using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Networking
{
    public static class PacketBuilder
    {
        /// <summary>
        /// Builds a packet from the specified <see cref="IPacket"/> instance.
        /// </summary>
        /// <param name="packet">The packet to serialize.</param>
        /// <returns>The serialized packet as a buffer</returns>
        public static NetBuffer Build(IPacket packet)
        {
            var buffer = new NetBuffer(packet.RequiredSize + 6);
            buffer.WriteUInt32(packet.Id);
            buffer.WriteByte((byte)packet.Session);
            buffer.WriteByte((byte)packet.Type);
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
    }
}
