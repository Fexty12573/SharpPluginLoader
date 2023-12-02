using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Networking
{
    /// <summary>
    /// Represents a packet that can be sent to via the game's networking system.
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// The ID of the packet. This is used to identify the packet type.
        /// </summary>
        /// <remarks>
        /// The game uses the DTI Id of each packet as the packet ID.
        /// You can use <see cref="Utility.Crc32(string, int)"/> to generate a unique Id.<br/>
        /// <b>Note:</b> Do not serialize this value. It is automatically serialized by the framework.
        /// </remarks>
        public uint Id { get; }

        /// <summary>
        /// The session index of the packet.
        /// </summary>
        /// <remarks><b>Note:</b> Do not serialize this value. It is automatically serialized by the framework.</remarks>
        public SessionIndex Session => SessionIndex.Current;
        
        /// <summary>
        /// The type of the packet.
        /// </summary>
        /// <remarks><b>Note:</b> Do not serialize this value. It is automatically serialized by the framework.</remarks>
        public PacketType Type { get; }

        /// <summary>
        /// The number of bytes required for the native representation of the packet.
        /// </summary>
        public int RequiredSize { get; }

        /// <summary>
        /// Serializes the packet into a <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to serialize the packet into.</param>
        public void Serialize(NetBuffer buffer);

        /// <summary>
        /// Deserializes the packet from a <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize the packet from.</param>
        public void Deserialize(NetBuffer buffer);
    }

    public enum PacketType
    {
        Custom = 0,
        System = 1,
        Quest = 2,
        Player = 3,
        Palico = 4,
        Monster = 5,
        Gimmick = 6,
        EnvironmentCreature = 7
    }

    public enum SessionIndex : byte
    {
        Hub = 0,
        Quest = 1,
        Session2 = 2,
        Session3 = 3,
        Current = 0x80
    }
}
