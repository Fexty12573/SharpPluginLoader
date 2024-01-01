# MtStream

Namespace: SharpPluginLoader.Core.IO

Represents any kind of stream.

```csharp
public class MtStream : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)

## Properties

### **Flags**

The flags of the stream.

```csharp
public UInt32& Flags { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **IsReadable**

Whether the stream is readable.

```csharp
public bool IsReadable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsWritable**

Whether the stream is writable.

```csharp
public bool IsWritable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsSeekable**

Whether the stream is seekable.

```csharp
public bool IsSeekable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Position**

Gets the position of the stream.

```csharp
public long Position { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

### **Length**

Gets or sets the length of the stream.

```csharp
public long Length { get; set; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtStream(IntPtr)**

```csharp
public MtStream(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtStream()**

```csharp
public MtStream()
```

## Methods

### **Close()**

Closes the stream.

```csharp
public void Close()
```

### **Flush()**

Flushes the stream.

```csharp
public void Flush()
```

### **Read(Int64)**

Reads the specified number of bytes from the stream.

```csharp
public Byte[] Read(long count)
```

#### Parameters

`count` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes to read.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes read.

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/System.Exception)<br>
Thrown if the number of bytes read does not match the number of bytes requested.

### **Read(Span&lt;Byte&gt;, Int64)**

Reads the specified number of bytes from the stream.

```csharp
public long Read(Span<byte> buffer, long count)
```

#### Parameters

`buffer` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>
The buffer to read into.

`count` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes to read. If -1, it will read bytes equal to the length of the buffer.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes read.

### **Write(ReadOnlySpan&lt;Byte&gt;)**

Writes the specified bytes to the stream.

```csharp
public long Write(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1)<br>
The bytes to write.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes written.

### **Seek(Int64, SeekOrigin)**

Seeks to the specified position in the stream.

```csharp
public long Seek(long offset, SeekOrigin origin)
```

#### Parameters

`offset` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The offset to seek to.

`origin` [SeekOrigin](https://docs.microsoft.com/en-us/dotnet/api/System.IO.SeekOrigin)<br>
The origin to seek from.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The new position in the stream.

### **Skip(Int64)**

Skips the specified number of bytes in the stream.

```csharp
public void Skip(long count)
```

#### Parameters

`count` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes to skip.
