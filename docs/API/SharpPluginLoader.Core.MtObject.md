# MtObject

Namespace: SharpPluginLoader.Core

```csharp
public class MtObject : NativeWrapper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md)

## Properties

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtObject(IntPtr)**

```csharp
public MtObject(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtObject()**

```csharp
public MtObject()
```

## Methods

### **As&lt;T&gt;()**

```csharp
public T As<T>()
```

#### Type Parameters

`T`<br>

#### Returns

T<br>

### **Is(String)**

```csharp
public bool Is(string typeName)
```

#### Parameters

`typeName` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **GetVirtualFunction(Int32)**

Gets a virtual function from the vtable of this object.

```csharp
public nint GetVirtualFunction(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the virtual function in the vtable

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The requested virtual function

### **GetProperties()**

Gets the properties of this object.

```csharp
public MtPropertyList GetProperties()
```

#### Returns

[MtPropertyList](./SharpPluginLoader.Core.MtPropertyList.md)<br>
The property list containing all properties

### **GetDti()**

Gets the DTI of this object.

```csharp
public MtDti GetDti()
```

#### Returns

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>
The DTI, or null if there is no DTI

### **Destroy(Boolean)**

Calls the destructor of this object.

```csharp
public void Destroy(bool free)
```

#### Parameters

`free` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether the destructor should deallocate the object or not
