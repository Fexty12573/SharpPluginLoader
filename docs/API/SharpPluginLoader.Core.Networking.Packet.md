# Packet

Namespace: SharpPluginLoader.Core.Networking

Represents a game packet.

```csharp
public class Packet : SharpPluginLoader.Core.MtObject, IPacket
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Packet](./SharpPluginLoader.Core.Networking.Packet.md)<br>
Implements [IPacket](./SharpPluginLoader.Core.Networking.IPacket.md)

**Remarks:**

Warning: Do not inherit from this class for the purpose of creating custom packets!
 Implement the [IPacket](./SharpPluginLoader.Core.Networking.IPacket.md) interface directly instead.

## Properties

### **Id**

```csharp
public uint Id { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Session**

```csharp
public SessionIndex Session { get; }
```

#### Property Value

[SessionIndex](./SharpPluginLoader.Core.Networking.SessionIndex.md)<br>

### **Type**

```csharp
public PacketType Type { get; }
```

#### Property Value

[PacketType](./SharpPluginLoader.Core.Networking.PacketType.md)<br>

### **RequiredSize**

```csharp
public int RequiredSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Packet(IntPtr)**

```csharp
public Packet(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Packet()**

```csharp
public Packet()
```

## Methods

### **Serialize(NetBuffer)**

```csharp
public void Serialize(NetBuffer buffer)
```

#### Parameters

`buffer` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>

### **Deserialize(NetBuffer)**

```csharp
public void Deserialize(NetBuffer buffer)
```

#### Parameters

`buffer` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
