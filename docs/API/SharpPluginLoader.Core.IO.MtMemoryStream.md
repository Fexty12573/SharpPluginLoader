# MtMemoryStream

Namespace: SharpPluginLoader.Core.IO

Represents a memory stream.

```csharp
public class MtMemoryStream : MtStream, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtStream](./SharpPluginLoader.Core.IO.MtStream.md) → [MtMemoryStream](./SharpPluginLoader.Core.IO.MtMemoryStream.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

## Properties

### **Buffer**

The buffer of this memory stream.

```csharp
public nint Buffer { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **IsBufferOverflowed**

Whether this memory stream is overflowed.

```csharp
public bool IsBufferOverflowed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Mode**

Gets the mode this memory stream was created with.

```csharp
public MemoryStreamMode Mode { get; }
```

#### Property Value

[MemoryStreamMode](./SharpPluginLoader.Core.IO.MemoryStreamMode.md)<br>

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

### **MtMemoryStream(IntPtr)**

```csharp
public MtMemoryStream(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtMemoryStream()**

```csharp
public MtMemoryStream()
```

## Methods

### **Create()**

Creates a new expandable memory stream.

```csharp
public static MtMemoryStream Create()
```

#### Returns

[MtMemoryStream](./SharpPluginLoader.Core.IO.MtMemoryStream.md)<br>
The new memory stream or null if it could not be created.

### **Create(Byte[])**

Creates a new memory stream from the specified buffer.

```csharp
public static MtMemoryStream Create(Byte[] buffer)
```

#### Parameters

`buffer` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The buffer to use

#### Returns

[MtMemoryStream](./SharpPluginLoader.Core.IO.MtMemoryStream.md)<br>
The new memory stream or null if it could not be created.

**Remarks:**

Note that the memory stream returned by this method is not expandable.
 It also does not take ownership of the buffer.

### **Dispose()**

```csharp
public void Dispose()
```

### **Finalize()**

```csharp
protected void Finalize()
```
