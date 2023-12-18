# Patch

Namespace: SharpPluginLoader.Core.Memory

Represents a patch in memory.

```csharp
public struct Patch
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [Patch](./SharpPluginLoader.Core.Memory.Patch.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

## Fields

### **Address**

The address of the patch.

```csharp
public nint Address;
```

### **OriginalBytes**

The original bytes at the patch address.

```csharp
public Byte[] OriginalBytes;
```

### **PatchedBytes**

The bytes that will be written to the patch address.

```csharp
public Byte[] PatchedBytes;
```

## Properties

### **IsEnabled**

Whether the patch is currently enabled.

```csharp
public bool IsEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

## Constructors

### **Patch(IntPtr, Byte[], Boolean)**

Creates a new patch.

```csharp
Patch(nint address, Byte[] patchedBytes, bool enable)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the patch.

`patchedBytes` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes that will be written to the patch address.

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to enable the patch.

### **Patch(IntPtr, String, Boolean)**

Creates a new patch.

```csharp
Patch(nint address, string asm, bool enable)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the patch.

`asm` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The assembly that will be written to the patch address.

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to enable the patch.

### **Patch(IntPtr, IEnumerable&lt;String&gt;, Boolean)**

Creates a new patch.

```csharp
Patch(nint address, IEnumerable<string> asm, bool enable)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the patch.

`asm` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1)<br>
The assembly that will be written to the patch address. Each line represented by a string.

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to enable the patch.

## Methods

### **Enable()**

Enables the patch.

```csharp
void Enable()
```

### **Disable()**

Disables the patch.

```csharp
void Disable()
```

### **Dispose()**

```csharp
void Dispose()
```
