using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Networking
{
    /// <summary>
    /// Represents a game packet.
    /// </summary>
    /// <remarks>
    /// <b>Warning:</b> Do not inherit from this class for the purpose of creating custom packets!
    /// Implement the <see cref="IPacket"/> interface directly instead.
    /// </remarks>
    public class Packet : MtObject, IPacket
    {
        public Packet(nint instance) : base(instance) { }
        public Packet() { }

        /// <inheritdoc />
        public uint Id => Get<uint>(0x8);

        /// <inheritdoc />
        public SessionIndex Session => Get<SessionIndex>(0xC);

        /// <inheritdoc />
        public PacketType Type => Get<PacketType>(0xD);

        /// <inheritdoc />
        public unsafe int RequiredSize => new NativeFunction<nint, int>(GetVirtualFunction(5)).InvokeUnsafe(Instance);

        /// <inheritdoc />
        public unsafe void Serialize(NetBuffer buffer)
        {
            var nativeBuffer = NetBuffer.CreateNative(RequiredSize);
            new NativeAction<nint, nint>(GetVirtualFunction(6)).InvokeUnsafe(Instance, nativeBuffer.Instance);

            buffer.WriteBytes(new NetBuffer(nativeBuffer).Buffer);
            NetBuffer.FreeNative(nativeBuffer);
        }

        /// <inheritdoc />
        public unsafe void Deserialize(NetBuffer buffer)
        {
            var nativeBuffer = NetBuffer.CreateNative(buffer.Buffer);
            new NativeAction<nint, nint>(GetVirtualFunction(7)).InvokeUnsafe(Instance, nativeBuffer.Instance);
            NetBuffer.FreeNative(nativeBuffer);
        }
    }
}
