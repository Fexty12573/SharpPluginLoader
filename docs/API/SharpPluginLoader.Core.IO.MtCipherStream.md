# MtCipherStream

Namespace: SharpPluginLoader.Core.IO

Represents a cipher stream. Can be used to encrypt/decrypt data from a stream.

```csharp
public class MtCipherStream : MtStream, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtStream](./SharpPluginLoader.Core.IO.MtStream.md) → [MtCipherStream](./SharpPluginLoader.Core.IO.MtCipherStream.md)<br>
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

### **MtCipherStream(IntPtr)**

```csharp
public MtCipherStream(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtCipherStream()**

```csharp
public MtCipherStream()
```

## Methods

### **FromStream(MtStream, CipherStreamMode, String)**

Creates a new cipher stream from the specified stream and key.

```csharp
public MtCipherStream FromStream(MtStream stream, CipherStreamMode mode, string key)
```

#### Parameters

`stream` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>
The stream to encrypt/decrypt.

`mode` [CipherStreamMode](./SharpPluginLoader.Core.IO.CipherStreamMode.md)<br>
The mode to use.

`key` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The key to use.

#### Returns

[MtCipherStream](./SharpPluginLoader.Core.IO.MtCipherStream.md)<br>
The cipher stream, or null if it could not be created.

### **Dispose()**

```csharp
public void Dispose()
```

### **Finalize()**

```csharp
protected void Finalize()
```
