# NativeWrapper

Namespace: SharpPluginLoader.Core

A wrapper around a native pointer.

```csharp
public class NativeWrapper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md)

## Properties

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **NativeWrapper(IntPtr)**

```csharp
public NativeWrapper(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **NativeWrapper()**

```csharp
public NativeWrapper()
```

## Methods

### **Get&lt;T&gt;(IntPtr)**

Gets a value at the specified offset.

```csharp
public T Get<T>(nint offset)
```

#### Type Parameters

`T`<br>
The type of the value, must be an unmanaged type

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset at which to retrieve the value

#### Returns

T<br>
The value at the offset

### **Set&lt;T&gt;(IntPtr, T)**

Sets a value at the specified offset.

```csharp
public void Set<T>(nint offset, T value)
```

#### Type Parameters

`T`<br>
The type of the value, must be an unmanaged type

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset at which to set the value

`value` T<br>
The value to set

### **GetPtr&lt;T&gt;(IntPtr)**

```csharp
public T* GetPtr<T>(nint offset)
```

#### Type Parameters

`T`<br>

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

#### Returns

T*<br>

### **GetPtr(IntPtr)**

```csharp
public Void* GetPtr(nint offset)
```

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

#### Returns

[Void*](https://docs.microsoft.com/en-us/dotnet/api/System.Void*)<br>

### **SetPtr&lt;T&gt;(IntPtr, T*)**

```csharp
public void SetPtr<T>(nint offset, T* value)
```

#### Type Parameters

`T`<br>

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

`value` T*<br>

### **GetRef&lt;T&gt;(IntPtr)**

```csharp
public T& GetRef<T>(nint offset)
```

#### Type Parameters

`T`<br>

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

#### Returns

T&<br>

### **GetObject&lt;T&gt;(IntPtr)**

Gets an object at the specified offset.

```csharp
public T GetObject<T>(nint offset)
```

#### Type Parameters

`T`<br>
The type of the object, must inherit from [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md)

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset at which to retrieve the object

#### Returns

T<br>
The object at the offset

### **GetInlineObject&lt;T&gt;(IntPtr)**

Retrieves an inlined object at the specified offset.

```csharp
public T GetInlineObject<T>(nint offset)
```

#### Type Parameters

`T`<br>
The type of the object, must inherit from [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md)

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset at which to retrieve the object

#### Returns

T<br>
The object at the offset

### **SetObject&lt;T&gt;(IntPtr, T)**

Sets an object at the specified offset.

```csharp
public void SetObject<T>(nint offset, T value)
```

#### Type Parameters

`T`<br>
The type of the object, must inherit from [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md)

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset at which to set the object

`value` T<br>
The object to set

### **GetRefInline&lt;T&gt;(IntPtr)**

Gets a reference to a value in the object at the specified offset.

```csharp
public T& GetRefInline<T>(nint offset)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset of the value

#### Returns

T&<br>
A reference to the value at the offset

### **GetPtrInline&lt;T&gt;(IntPtr)**

Gets a pointer to a value in the object at the specified offset.

```csharp
public T* GetPtrInline<T>(nint offset)
```

#### Type Parameters

`T`<br>
The type of the value

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The offset of the value

#### Returns

T*<br>
A pointer to the value at the offset

### **GetPtrInline(IntPtr)**

```csharp
public Void* GetPtrInline(nint offset)
```

#### Parameters

`offset` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

#### Returns

[Void*](https://docs.microsoft.com/en-us/dotnet/api/System.Void*)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
