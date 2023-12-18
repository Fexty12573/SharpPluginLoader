# IPacket

Namespace: SharpPluginLoader.Core.Networking

Represents a packet that can be sent to via the game's networking system.

```csharp
public interface IPacket
```

## Properties

### **Id**

The ID of the packet. This is used to identify the packet type.

```csharp
public abstract uint Id { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

**Remarks:**

The game uses the DTI Id of each packet as the packet ID.
 You can use [Utility.Crc32(String, Int32)](./SharpPluginLoader.Core.Utility.md#crc32string-int32) to generate a unique Id.<br>Note: Do not serialize this value. It is automatically serialized by the framework.

### **Session**

The session index of the packet.

```csharp
public SessionIndex Session { get; }
```

#### Property Value

[SessionIndex](./SharpPluginLoader.Core.Networking.SessionIndex.md)<br>

**Remarks:**

Note: Do not serialize this value. It is automatically serialized by the framework.

### **Type**

The type of the packet.

```csharp
public abstract PacketType Type { get; }
```

#### Property Value

[PacketType](./SharpPluginLoader.Core.Networking.PacketType.md)<br>

**Remarks:**

Note: Do not serialize this value. It is automatically serialized by the framework.

### **RequiredSize**

The number of bytes required for the native representation of the packet.

```csharp
public abstract int RequiredSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

## Methods

### **Serialize(NetBuffer)**

Serializes the packet into a [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md).

```csharp
void Serialize(NetBuffer buffer)
```

#### Parameters

`buffer` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
The buffer to serialize the packet into.

### **Deserialize(NetBuffer)**

Deserializes the packet from a [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md).

```csharp
void Deserialize(NetBuffer buffer)
```

#### Parameters

`buffer` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
The buffer to deserialize the packet from.
