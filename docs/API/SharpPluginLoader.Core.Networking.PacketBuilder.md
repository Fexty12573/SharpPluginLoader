# PacketBuilder

Namespace: SharpPluginLoader.Core.Networking

```csharp
public static class PacketBuilder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [PacketBuilder](./SharpPluginLoader.Core.Networking.PacketBuilder.md)

## Methods

### **Build(IPacket)**

Builds a packet from the specified [IPacket](./SharpPluginLoader.Core.Networking.IPacket.md) instance.

```csharp
public static NetBuffer Build(IPacket packet)
```

#### Parameters

`packet` [IPacket](./SharpPluginLoader.Core.Networking.IPacket.md)<br>
The packet to serialize.

#### Returns

[NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
The serialized packet as a buffer

### **Build&lt;TPacket&gt;(NetBuffer)**

Builds a packet from the specified [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md) instance.

```csharp
public static TPacket Build<TPacket>(NetBuffer buffer)
```

#### Type Parameters

`TPacket`<br>
The type of the packet to build.

#### Parameters

`buffer` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
The buffer to deserialize.

#### Returns

TPacket<br>
The deserialized packet.
