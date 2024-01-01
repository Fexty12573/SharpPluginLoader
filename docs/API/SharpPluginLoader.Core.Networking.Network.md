# Network

Namespace: SharpPluginLoader.Core.Networking

Provides access to the game's networking system.

```csharp
public static class Network
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Network](./SharpPluginLoader.Core.Networking.Network.md)

## Properties

### **SingletonInstance**

The sMhNetwork singleton instance.

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Methods

### **SendPacket(IPacket, Boolean, UInt32, SessionIndex)**

Sends a packet to other people in the specified session.

```csharp
public static void SendPacket(IPacket packet, bool broadcast, uint memberIndex, SessionIndex targetSession)
```

#### Parameters

`packet` [IPacket](./SharpPluginLoader.Core.Networking.IPacket.md)<br>
The packet to send.

`broadcast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether or not to broadcast the packet to everyone in the session.

`memberIndex` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
If  is false, the index of the member to send the packet to.

`targetSession` [SessionIndex](./SharpPluginLoader.Core.Networking.SessionIndex.md)<br>
The session to send the packet to.
