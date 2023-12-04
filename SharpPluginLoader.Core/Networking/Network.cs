using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Networking
{
    /// <summary>
    /// Provides access to the game's networking system.
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// The sMhNetwork singleton instance.
        /// </summary>
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x1451c2478);

        /// <summary>
        /// Sends a packet to other people in the specified session.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="broadcast">Whether or not to broadcast the packet to everyone in the session.</param>
        /// <param name="memberIndex">If <paramref name="broadcast"/> is false, the index of the member to send the packet to.</param>
        /// <param name="targetSession">The session to send the packet to.</param>
        public static unsafe void SendPacket(IPacket packet, bool broadcast = true, uint memberIndex = 0, SessionIndex targetSession = SessionIndex.Current)
        {
            var assembledPacket = PacketBuilder.Build(packet);

            fixed (byte* ptr = assembledPacket.Buffer)
            {
                var dst = broadcast ? 0xC0 : memberIndex;
                var option = broadcast ? 0x10u : 0x30u;
                SendPacketFunc.Invoke(SingletonInstance, (nint)ptr, dst, option, (uint)targetSession);
            }
        }

        #region Internal
        private static void SendPacketHook(nint instance, nint packet, uint dst,
            uint option, uint sessionIndex)
        {
            var packetObj = new Packet(packet);
            var isBroadcast = (option & 0x20) == 0 && dst > 0x40;

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnSendPacket))
                plugin.OnSendPacket(packetObj, isBroadcast, (SessionIndex)sessionIndex);
            
            _sendPacketHook.Original(instance, packet, dst, option, sessionIndex);
        }

        private static unsafe void ReceivePacketHook(nint instance, int src, nint data, int dataSize)
        {
            var dataOffset = 6;
            var buffer = new NetBuffer(new ReadOnlySpan<byte>((void*)data, dataSize));
            var id = buffer.ReadUInt32();
            var session = buffer.ReadByte();
            var type = buffer.ReadByte();
            
            if (id == Utility.MakeDtiId("cPacketBase")) // Decoy Header
            {
                id = buffer.ReadUInt32();
                session = buffer.ReadByte();
                type = buffer.ReadByte();
                dataOffset += 6;
            }

            var offsettedData = new ReadOnlySpan<byte>((void*)(data + dataOffset), dataSize - dataOffset);

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnReceivePacket))
                plugin.OnReceivePacket(id, (PacketType)type, (SessionIndex)session, new NetBuffer(offsettedData));

            _receivePacketHook.Original(instance, src, data, dataSize);
        }

        internal static void Initialize()
        {
            _sendPacketHook = new Hook<SendPacketDelegate>(SendPacketHook, AddressRepository.Get("Network:SendPacket"));
            _receivePacketHook = new Hook<ReceivePacketDelegate>(ReceivePacketHook, AddressRepository.Get("Network:ReceivePacket"));
        }

        private delegate void SendPacketDelegate(nint instance, nint packet, uint dst, uint option, uint sessionIndex);
        private delegate void ReceivePacketDelegate(nint instance, int src, nint data, int dataSize);

        private static Hook<SendPacketDelegate> _sendPacketHook = null!;
        private static Hook<ReceivePacketDelegate> _receivePacketHook = null!;
        private static readonly NativeAction<nint, nint, uint, uint, uint> SendPacketFunc = new(AddressRepository.Get("Network:SendPacket"));
        #endregion
    }
}
