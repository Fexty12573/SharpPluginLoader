using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Networking;

namespace CustomPackets
{
    public class PopupPacket : IPacket
    {
        public PopupPacket(string message)
        {
            if (message.Length > RequiredSize)
                throw new ArgumentException("Message cannot be longer than 128 characters.");
            Message = message;
        }

        public PopupPacket(NetBuffer buffer)
        {
            Deserialize(buffer);
        }

        public static uint StaticId => Utility.Crc32("PopupPacket");

        public uint Id => StaticId;

        public PacketType Type => PacketType.Custom;

        public int RequiredSize => 4 + 128; // 4 bytes for the string length, 128 bytes for the string itself

        public string Message { get; set; } = "";

        public void Serialize(NetBuffer buffer)
        {
            buffer.WriteInt32(Message.Length);
            buffer.WriteString(Message);
        }

        public void Deserialize(NetBuffer buffer)
        {
            var length = buffer.ReadInt32();
            Message = buffer.ReadString(length);
        }
    }
}
