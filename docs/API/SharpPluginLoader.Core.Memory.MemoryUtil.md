# MemoryUtil

Namespace: SharpPluginLoader.Core.Memory

Provides low-level memory reading and writing methods.

```csharp
public static class MemoryUtil
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [MemoryUtil](./SharpPluginLoader.Core.Memory.MemoryUtil.md)

## Methods

### **Read&lt;T&gt;(IntPtr)**

Reads a value of type  from a given address.

```csharp
public static T Read<T>(nint address)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

#### Returns

T<br>
The value read

### **ReadPointer&lt;T&gt;(IntPtr)**

Reads a pointer of type * from a given address.

```csharp
public static T* ReadPointer<T>(nint address)
```

#### Type Parameters

`T`<br>
The type of the pointer

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

#### Returns

T*<br>
The pointer read

### **GetRef&lt;T&gt;(IntPtr)**

Gets a reference to a value of type  from a given address.

```csharp
public static T& GetRef<T>(nint address)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

#### Returns

T&<br>
A reference to the value read

### **ReadArray&lt;T&gt;(IntPtr, Int32)**

Reads an array of type [] from a given address.

```csharp
public static T[] ReadArray<T>(nint address, int count)
```

#### Type Parameters

`T`<br>
The type of the array

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of elements to read

#### Returns

T[]<br>
The array read

**Remarks:**

This method copies the data into managed memory.

### **ReadStruct&lt;T&gt;(IntPtr)**

Reads a struct of type  from a given address.

```csharp
public static T ReadStruct<T>(nint address)
```

#### Type Parameters

`T`<br>
The type of the struct

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

#### Returns

T<br>
The struct read

**Remarks:**

This method copies the data into managed memory.

### **ReadStructArray&lt;T&gt;(IntPtr, Int32)**

Reads an array of structs of type [] from a given address.

```csharp
public static T[] ReadStructArray<T>(nint address, int count)
```

#### Type Parameters

`T`<br>
The type of the struct

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of elements to read

#### Returns

T[]<br>
The array read

**Remarks:**

This method copies the data into managed memory.

### **Read&lt;T&gt;(Int64)**

```csharp
public static T Read<T>(long address)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

#### Returns

T<br>

### **ReadPointer&lt;T&gt;(Int64)**

```csharp
public static T* ReadPointer<T>(long address)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

#### Returns

T*<br>

### **GetRef&lt;T&gt;(Int64)**

```csharp
public static T& GetRef<T>(long address)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

#### Returns

T&<br>

### **ReadArray&lt;T&gt;(Int64, Int32)**

```csharp
public static T[] ReadArray<T>(long address, int count)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

T[]<br>

### **ReadStruct&lt;T&gt;(Int64)**

```csharp
public static T ReadStruct<T>(long address)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

#### Returns

T<br>

### **ReadStructArray&lt;T&gt;(Int64, Int32)**

```csharp
public static T[] ReadStructArray<T>(long address, int count)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

T[]<br>

### **ReadBytes(IntPtr, Int32)**

Reads a specified number of bytes from a given address.

```csharp
public static Byte[] ReadBytes(nint address, int count)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to read from

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of bytes to read

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes read

### **WriteBytes(IntPtr, Byte[])**

Writes a specified number of bytes to a given address.

```csharp
public static void WriteBytes(nint address, Byte[] bytes)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to write to

`bytes` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes to write

**Remarks:**

Do not write to EXE regions using this method. Use [MemoryUtil.WriteBytesSafe(IntPtr, Byte[])](./SharpPluginLoader.Core.Memory.MemoryUtil.md#writebytessafeintptr-byte) instead.

### **WriteBytesSafe(IntPtr, Byte[])**

Writes a specified number of bytes to a given address.

```csharp
public static void WriteBytesSafe(nint address, Byte[] bytes)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address to write to

`bytes` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>
The bytes to write

**Remarks:**

This method changes the memory protection of the page prior to writing, and is thus safe to use on EXE regions.

### **Alloc(Int64)**

Allocates a block of native memory of the specified size.

```csharp
public static nint Alloc(long size)
```

#### Parameters

`size` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The size of the block to allocate

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the allocated block

**Remarks:**

Any memory allocated using this method must be freed using either [MemoryUtil.Free(IntPtr)](./SharpPluginLoader.Core.Memory.MemoryUtil.md#freeintptr) or [MemoryUtil.Free(IntPtr)](./SharpPluginLoader.Core.Memory.MemoryUtil.md#freeintptr).
 to avoid memory leaks.

### **Realloc(IntPtr, Int64)**

Reallocates a block of native memory to the specified size.

```csharp
public static nint Realloc(nint address, long newSize)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the block to reallocate

`newSize` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The new size of the block

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the reallocated block

### **Alloc&lt;T&gt;(Int64)**

Allocates an array of type [] of the specified size in native memory.

```csharp
public static T* Alloc<T>(long count)
```

#### Type Parameters

`T`<br>
The type of the array

#### Parameters

`count` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The size of the array

#### Returns

T*<br>
The address of the allocated array

**Remarks:**

Generally, if you need to allocate an array of native memory, you should use [NativeArray&lt;T&gt;.Create(Int32)](./SharpPluginLoader.Core.NativeArray-1.md#createint32).
 If you do use this method, you must free the memory using [MemoryUtil.Free(IntPtr)](./SharpPluginLoader.Core.Memory.MemoryUtil.md#freeintptr) or [MemoryUtil.Free(IntPtr)](./SharpPluginLoader.Core.Memory.MemoryUtil.md#freeintptr).

### **Realloc&lt;T&gt;(T*, Int64)**

Reallocates an array of type [] to the specified size in native memory.

```csharp
public static T* Realloc<T>(T* address, long count)
```

#### Type Parameters

`T`<br>
The type of the array

#### Parameters

`address` T*<br>
The address of the array to reallocate

`count` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The new size of the array

#### Returns

T*<br>
The address of the reallocated array

**Remarks:**

Generally, if you need an array in native memory, you should use a [NativeArray&lt;T&gt;](./SharpPluginLoader.Core.NativeArray-1.md)
 with [NativeArray&lt;T&gt;.Resize(Int32)](./SharpPluginLoader.Core.NativeArray-1.md#resizeint32).

### **Free(IntPtr)**

Frees a block of native memory.

```csharp
public static void Free(nint address)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the block to free

### **Free(Void*)**

Frees a block of native memory.

```csharp
public static void Free(Void* address)
```

#### Parameters

`address` [Void*](https://docs.microsoft.com/en-us/dotnet/api/System.Void*)<br>
The address of the block to free

### **AsSpan&lt;T&gt;(IntPtr, Int32)**

Creates a span around a native array from a given address and count.

```csharp
public static Span<T> AsSpan<T>(nint address, int count)
```

#### Type Parameters

`T`<br>
The type of the array

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the first element

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The length of the array

#### Returns

Span&lt;T&gt;<br>
A new span that represents the native array

### **AsSpan&lt;T&gt;(Int64, Int32)**

```csharp
public static Span<T> AsSpan<T>(long address, int count)
```

#### Type Parameters

`T`<br>

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

Span&lt;T&gt;<br>

### **AsPointer&lt;T&gt;(T&)**

Converts a given reference to a pointer.

```csharp
public static T* AsPointer<T>(T& value)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`value` T&<br>
The reference to convert

#### Returns

T*<br>
A pointer of type * that points to

### **AsRef&lt;T&gt;(T*)**

Converts a given pointer to a reference.

```csharp
public static T& AsRef<T>(T* address)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`address` T*<br>
The pointer to convert

#### Returns

T&<br>
A reference of type  that points to

### **AddressOf&lt;T&gt;(T&)**

Gets the address of a given reference.

```csharp
public static nint AddressOf<T>(T& value)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`value` T&<br>
The reference to convert

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
An address that points to
