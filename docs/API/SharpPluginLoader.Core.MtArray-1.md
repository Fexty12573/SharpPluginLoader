# MtArray&lt;T&gt;

Namespace: SharpPluginLoader.Core

This class is a wrapper for the MtArray object in the game.

```csharp
public class MtArray<T> : MtObject, , System.Collections.IEnumerable
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtArray&lt;T&gt;](./SharpPluginLoader.Core.MtArray-1.md)<br>
Implements IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable)

## Properties

### **Length**

Gets the length of the array.

```csharp
public uint Length { get; private set; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Capacity**

Gets the capacity of the array.

```csharp
public uint Capacity { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **AutoDelete**

Gets whether the array auto-deletes its contents when it is destroyed.

```csharp
public bool AutoDelete { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsEmpty**

Gets whether the array is empty.

```csharp
public bool IsEmpty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Item**

```csharp
public T Item { get; set; }
```

#### Property Value

T<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtArray(IntPtr)**

Constructs a new MtArray instance from a native pointer.

```csharp
public MtArray(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The native pointer

### **MtArray()**

Constructs a new MtArray instance with a nullptr

```csharp
public MtArray()
```

## Methods

### **Push(T)**

Adds an item to the end of the array.

```csharp
public void Push(T item)
```

#### Parameters

`item` T<br>
The item to add

### **Pop()**

Removes the last item from the array and returns it.

```csharp
public T Pop()
```

#### Returns

T<br>
The removed item

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>
Thrown if the array is empty

### **First()**

Gets the first item in the array.

```csharp
public T First()
```

#### Returns

T<br>
The first item in the array

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>
Thrown if the array is empty

### **Last()**

Gets the last item in the array.

```csharp
public T Last()
```

#### Returns

T<br>
The last item in the array

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>
Thrown if the array is empty

### **Contains(T)**

Checks if the array contains the given item.

```csharp
public bool Contains(T item)
```

#### Parameters

`item` T<br>
The item to check for

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the array contains the item

**Remarks:**

This function does a reference comparison.

### **Find(Func&lt;T, Boolean&gt;)**

Finds the first item in the array that matches the given predicate.

```csharp
public T Find(Func<T, bool> predicate)
```

#### Parameters

`predicate` Func&lt;T, Boolean&gt;<br>
The predicate to match

#### Returns

T<br>
The first item that matches the predicate, or null if none match

### **Clear(Boolean)**

Clears the array.

```csharp
public void Clear(bool freeMem)
```

#### Parameters

`freeMem` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to free the memory used by the array

### **Insert(Int32, T)**

Inserts an item into the array at the given index.

```csharp
public void Insert(int index, T item)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index to insert the item at

`item` T<br>
The item to insert

### **Erase(T)**

Erases the given item from the array.

```csharp
public void Erase(T item)
```

#### Parameters

`item` T<br>
The item to erase

### **Erase(Int32)**

Erases the item at the given index from the array.

```csharp
public void Erase(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **GetEnumerator()**

Gets an enumerator for the array.

```csharp
public IEnumerator<T> GetEnumerator()
```

#### Returns

IEnumerator&lt;T&gt;<br>
The enumerator
