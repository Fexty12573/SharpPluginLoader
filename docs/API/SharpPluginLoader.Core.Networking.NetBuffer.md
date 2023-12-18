# NetBuffer

Namespace: SharpPluginLoader.Core.Networking

Represents a network buffer used for reading and writing data.

```csharp
public class NetBuffer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)

## Properties

### **Buffer**

Gets the buffer.

```csharp
public ReadOnlySpan<byte> Buffer { get; }
```

#### Property Value

[ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1)<br>

### **Size**

Gets the size of the buffer.

```csharp
public int Size { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

## Constructors

### **NetBuffer(ReadOnlySpan&lt;Byte&gt;)**

Initializes a new instance of the [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md) class with the specified buffer.

```csharp
public NetBuffer(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1)<br>
The buffer to use.

### **NetBuffer(Byte[])**

Initializes a new instance of the [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md) class with the specified buffer.

```csharp
public NetBuffer(Byte[] buffer)
```

#### Parameters

`buffer` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The buffer to use.

### **NetBuffer(Int32)**

Initializes a new instance of the [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md) class with the specified size.

```csharp
public NetBuffer(int size)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The size of the buffer.

## Methods

### **WriteByte(Byte)**

Writes a byte to the buffer and advances the position by 1.

```csharp
public void WriteByte(byte value)
```

#### Parameters

`value` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The byte value to write.

### **WriteUInt16(UInt16)**

Writes an unsigned 16-bit integer to the buffer and advances the position by 2.

```csharp
public void WriteUInt16(ushort value)
```

#### Parameters

`value` [UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>
The unsigned 16-bit integer value to write.

### **WriteUInt32(UInt32)**

Writes an unsigned 32-bit integer to the buffer and advances the position by 4.

```csharp
public void WriteUInt32(uint value)
```

#### Parameters

`value` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The unsigned 32-bit integer value to write.

### **WriteUInt64(UInt64)**

Writes an unsigned 64-bit integer to the buffer and advances the position by 8.

```csharp
public void WriteUInt64(ulong value)
```

#### Parameters

`value` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/System.UInt64)<br>
The unsigned 64-bit integer value to write.

### **WriteSByte(SByte)**

Writes a signed byte to the buffer and advances the position by 1.

```csharp
public void WriteSByte(sbyte value)
```

#### Parameters

`value` [SByte](https://docs.microsoft.com/en-us/dotnet/api/System.SByte)<br>
The signed byte value to write.

### **WriteInt16(Int16)**

Writes a signed 16-bit integer to the buffer and advances the position by 2.

```csharp
public void WriteInt16(short value)
```

#### Parameters

`value` [Int16](https://docs.microsoft.com/en-us/dotnet/api/System.Int16)<br>
The signed 16-bit integer value to write.

### **WriteInt32(Int32)**

Writes a signed 32-bit integer to the buffer and advances the position by 4.

```csharp
public void WriteInt32(int value)
```

#### Parameters

`value` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The signed 32-bit integer value to write.

### **WriteInt64(Int64)**

Writes a signed 64-bit integer to the buffer and advances the position by 8.

```csharp
public void WriteInt64(long value)
```

#### Parameters

`value` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The signed 64-bit integer value to write.

### **WriteFloat(Single)**

Writes a single-precision floating-point value to the buffer and advances the position by 4.

```csharp
public void WriteFloat(float value)
```

#### Parameters

`value` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The single-precision floating-point value to write.

### **WriteDouble(Double)**

Writes a double-precision floating-point value to the buffer and advances the position by 8.

```csharp
public void WriteDouble(double value)
```

#### Parameters

`value` [Double](https://docs.microsoft.com/en-us/dotnet/api/System.Double)<br>
The double-precision floating-point value to write.

### **WriteBoolean(Boolean)**

Writes a boolean value to the buffer and advances the position by 1.

```csharp
public void WriteBoolean(bool value)
```

#### Parameters

`value` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
The boolean value to write.

### **WriteBytes(ReadOnlySpan&lt;Byte&gt;)**

Writes a byte array to the buffer and advances the position by the length of the array.

```csharp
public void WriteBytes(ReadOnlySpan<byte> bytes)
```

#### Parameters

`bytes` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1)<br>
The byte array to write.

### **WriteString(String)**

Writes a string encoded in UTF-8 to the buffer and advances the position by the length of the string.

```csharp
public void WriteString(string value)
```

#### Parameters

`value` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string value to write.

### **WriteString(String, Encoding)**

Writes a string encoded in the specified encoding to the buffer and advances the position by the length of the string.

```csharp
public void WriteString(string value, Encoding encoding)
```

#### Parameters

`value` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string value to write.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Encoding)<br>
The encoding to use.

### **ReadByte()**

Reads a byte from the buffer and advances the position by 1.

```csharp
public byte ReadByte()
```

#### Returns

[Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The byte value read from the buffer.

### **ReadUInt16()**

Reads an unsigned 16-bit integer from the buffer and advances the position by 2.

```csharp
public ushort ReadUInt16()
```

#### Returns

[UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>
The unsigned 16-bit integer value read from the buffer.

### **ReadUInt32()**

Reads an unsigned 32-bit integer from the buffer and advances the position by 4.

```csharp
public uint ReadUInt32()
```

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The unsigned 32-bit integer value read from the buffer.

### **ReadUInt64()**

Reads an unsigned 64-bit integer from the buffer and advances the position by 8.

```csharp
public ulong ReadUInt64()
```

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/System.UInt64)<br>
The unsigned 64-bit integer value read from the buffer.

### **ReadSByte()**

Reads a signed byte from the buffer and advances the position by 1.

```csharp
public sbyte ReadSByte()
```

#### Returns

[SByte](https://docs.microsoft.com/en-us/dotnet/api/System.SByte)<br>
The signed byte value read from the buffer.

### **ReadInt16()**

Reads a signed 16-bit integer from the buffer and advances the position by 2.

```csharp
public short ReadInt16()
```

#### Returns

[Int16](https://docs.microsoft.com/en-us/dotnet/api/System.Int16)<br>
The signed 16-bit integer value read from the buffer.

### **ReadInt32()**

Reads a signed 32-bit integer from the buffer and advances the position by 4.

```csharp
public int ReadInt32()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The signed 32-bit integer value read from the buffer.

### **ReadInt64()**

Reads a signed 64-bit integer from the buffer and advances the position by 8.

```csharp
public long ReadInt64()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The signed 64-bit integer value read from the buffer.

### **ReadFloat()**

Reads a single-precision floating-point value from the buffer and advances the position by 4.

```csharp
public float ReadFloat()
```

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The single-precision floating-point value read from the buffer.

### **ReadDouble()**

Reads a double-precision floating-point value from the buffer and advances the position by 8.

```csharp
public double ReadDouble()
```

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/System.Double)<br>
The double-precision floating-point value read from the buffer.

### **ReadBoolean()**

Reads a boolean value from the buffer and advances the position by 1.

```csharp
public bool ReadBoolean()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
The boolean value read from the buffer.

### **ReadBytes(Int32)**

Reads a specified number of bytes from the buffer and advances the position by the number of bytes read.

```csharp
public Byte[] ReadBytes(int count)
```

#### Parameters

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of bytes to read.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
A byte array containing the bytes read from the buffer.

### **ReadString(Int32)**

Reads a string from the buffer encoded in UTF-8 and advances the position by the length of the string.

```csharp
public string ReadString(int count)
```

#### Parameters

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of bytes to read.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string value read from the buffer.

### **ReadString(Int32, Encoding)**

Reads a string from the buffer encoded in the specified encoding and advances the position by the length of the string.

```csharp
public string ReadString(int count, Encoding encoding)
```

#### Parameters

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of bytes to read.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Encoding)<br>
The encoding to use.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string value read from the buffer.

### **WriteInt32NoBSwap(Int32)**

```csharp
internal void WriteInt32NoBSwap(int value)
```

#### Parameters

`value` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **WriteInt64NoBSwap(Int64)**

```csharp
internal void WriteInt64NoBSwap(long value)
```

#### Parameters

`value` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

### **CreateNative(Int32)**

```csharp
internal static MtObject CreateNative(int size)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

### **CreateNative(ReadOnlySpan&lt;Byte&gt;)**

```csharp
internal static MtObject CreateNative(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1)<br>

#### Returns

[MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

### **FreeNative(MtObject)**

```csharp
internal static void FreeNative(MtObject instance)
```

#### Parameters

`instance` [MtObject](./SharpPluginLoader.Core.MtObject.md)<br>
