# MtFile

Namespace: SharpPluginLoader.Core.IO

Represents a file.

```csharp
public class MtFile : SharpPluginLoader.Core.MtObject, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtFile](./SharpPluginLoader.Core.IO.MtFile.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

## Properties

### **Path**

Gets the path of the file.

```csharp
public string Path { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Position**

Gets the file pointer.

```csharp
public long Position { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

### **Size**

Gets the size of the file.

```csharp
public long Size { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

### **IsReadable**

Whether the file is readable.

```csharp
public bool IsReadable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsWritable**

Whether the file is writable.

```csharp
public bool IsWritable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtFile(IntPtr)**

```csharp
public MtFile(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtFile()**

```csharp
public MtFile()
```

## Methods

### **Open(String, OpenMode, Boolean)**

Opens a file.

```csharp
public static MtFile Open(string path, OpenMode mode, bool createPath)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The path of the file.

`mode` [OpenMode](./SharpPluginLoader.Core.IO.OpenMode.md)<br>
The mode to open the file in.

`createPath` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to create the path if it does not exist.

#### Returns

[MtFile](./SharpPluginLoader.Core.IO.MtFile.md)<br>
The opened file, or null if the file could not be opened.

### **Reopen(String, OpenMode, Boolean)**

Reopens the file.

```csharp
public bool Reopen(string path, OpenMode mode, bool createPath)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The path of the file.

`mode` [OpenMode](./SharpPluginLoader.Core.IO.OpenMode.md)<br>
The mode to open the file in.

`createPath` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to create the path if it does not exist.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether the file was reopened successfully.

### **Close()**

Closes the file.

```csharp
public void Close()
```

**Remarks:**

This is called automatically when the file object is destroyed or disposed.

### **Read(Int64)**

Reads the specified number of bytes from the file.

```csharp
public Byte[] Read(long size)
```

#### Parameters

`size` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes to read.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes read.

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/System.Exception)<br>
Thrown if the number of bytes read does not match the number of bytes requested.

### **Read(Span&lt;Byte&gt;, Int64)**

Reads the specified number of bytes from the file.

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

Writes the specified bytes to the file.

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

Seeks to the specified position in the file.

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
The new position in the file.

### **SetLength(Int64)**

Sets the length of the file.

```csharp
public void SetLength(long length)
```

#### Parameters

`length` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The new length of the file.

### **Dispose()**

```csharp
public void Dispose()
```

### **Finalize()**

```csharp
protected void Finalize()
```
