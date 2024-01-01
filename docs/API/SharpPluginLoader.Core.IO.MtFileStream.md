# MtFileStream

Namespace: SharpPluginLoader.Core.IO

Represents a file stream.

```csharp
public class MtFileStream : MtStream, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtStream](./SharpPluginLoader.Core.IO.MtStream.md) → [MtFileStream](./SharpPluginLoader.Core.IO.MtFileStream.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

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

### **MtFileStream(IntPtr)**

```csharp
public MtFileStream(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtFileStream()**

```csharp
public MtFileStream()
```

## Methods

### **FromFile(MtFile)**

Creates a new file stream from the specified file.

```csharp
public static MtFileStream FromFile(MtFile file)
```

#### Parameters

`file` [MtFile](./SharpPluginLoader.Core.IO.MtFile.md)<br>
The file to create the stream from.

#### Returns

[MtFileStream](./SharpPluginLoader.Core.IO.MtFileStream.md)<br>
The created file stream, or null if the file stream could not be created.

### **FromPath(String, OpenMode, Boolean)**

Opens a file stream from the specified path.

```csharp
public static MtFileStream FromPath(string path, OpenMode mode, bool createPath)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The path of the file.

`mode` [OpenMode](./SharpPluginLoader.Core.IO.OpenMode.md)<br>
The mode to open the file in.

`createPath` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to create the path if it does not exist.

#### Returns

[MtFileStream](./SharpPluginLoader.Core.IO.MtFileStream.md)<br>
The opened file stream, or null if the file stream could not be opened.

### **Write(String, Encoding)**

Writes the given string to the stream.

```csharp
public long Write(string str, Encoding encoding)
```

#### Parameters

`str` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string to write.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Encoding)<br>
The encoding to use. If null, UTF8 will be used.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The number of bytes written.

### **Dispose()**

```csharp
public void Dispose()
```

### **Finalize()**

```csharp
protected void Finalize()
```
