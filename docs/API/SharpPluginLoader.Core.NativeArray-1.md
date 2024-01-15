# NativeArray&lt;T&gt;

Namespace: SharpPluginLoader.Core

A wrapper around a native array.

```csharp
public struct NativeArray<T>
```

#### Type Parameters

`T`<br>
The type of the underlying elements

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [NativeArray&lt;T&gt;](./SharpPluginLoader.Core.NativeArray-1.md)<br>
Implements IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

**Remarks:**

Note: If you don't need to use the array in an iterator, use a [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1) instead.

## Properties

### **Length**

The number of elements in the array.

```csharp
public int Length { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **Address**

The address of the first element in the array.

```csharp
public nint Address { get; private set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Pointer**

A pointer to the first element in the array.

```csharp
public T* Pointer { get; }
```

#### Property Value

T*<br>

### **Item**

```csharp
public T& Item { get; }
```

#### Property Value

T&<br>

## Constructors

### **NativeArray(IntPtr, Int32, Boolean)**

A wrapper around a native array.

```csharp
NativeArray(nint address, int length, bool ownsPointer)
```

#### Parameters

`address` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The address of the first element

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of elements in the array

`ownsPointer` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether or not the array owns the pointer

**Remarks:**

Note: If you don't need to use the array in an iterator, use a [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1) instead.

## Methods

### **Create(Int32)**

Creates a new native array from a given address and count.

```csharp
NativeArray<T> Create(int length)
```

#### Parameters

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The number of elements

#### Returns

[NativeArray&lt;T&gt;](./SharpPluginLoader.Core.NativeArray-1.md)<br>
The newly allocated native array

**Remarks:**

Warning: The memory allocated by this method is not automatically freed. You must call [NativeArray&lt;T&gt;.Dispose()](./SharpPluginLoader.Core.NativeArray-1.md#dispose) when you are done with the array.
 Alternatively, use a using statement to ensure that the array is disposed.

### **Slice(Int32, Int32)**

```csharp
NativeArray<T> Slice(int start, int newLength)
```

#### Parameters

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

`newLength` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[NativeArray&lt;T&gt;](./SharpPluginLoader.Core.NativeArray-1.md)<br>

### **AsSpan()**

Creates a span over the native array.

```csharp
Span<T> AsSpan()
```

#### Returns

Span&lt;T&gt;<br>
A span over the native array.

### **Resize(Int32)**

Resizes the native array.

```csharp
void Resize(int newLength)
```

#### Parameters

`newLength` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The new size

**Remarks:**

Warning: This method can only be called on NativeArrays that were created using the [NativeArray&lt;T&gt;.Create(Int32)](./SharpPluginLoader.Core.NativeArray-1.md#createint32) method.

### **GetEnumerator()**

```csharp
IEnumerator<T> GetEnumerator()
```

#### Returns

IEnumerator&lt;T&gt;<br>

### **Dispose()**

Disposes the native array.

```csharp
void Dispose()
```
