# Resource

Namespace: SharpPluginLoader.Core.Resources

Represents an instance of the cResource class.

```csharp
public class Resource : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Resource](./SharpPluginLoader.Core.Resources.Resource.md)

## Properties

### **FilePath**

Gets the file path of this resource without the extension.

```csharp
public string FilePath { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **FileExtension**

Gets the file extension of this resource.

```csharp
public string FileExtension { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **RefCount**

Gets the reference count of this resource. If the reference count reaches 0, the resource is unloaded.

```csharp
public uint RefCount { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Resource(IntPtr)**

```csharp
public Resource(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Resource()**

```csharp
public Resource()
```

## Methods

### **Finalize()**

```csharp
protected void Finalize()
```

### **AddRef()**

Increments the reference count of this resource.

```csharp
public void AddRef()
```

**Remarks:**

This class automatically increments and decrements the reference counter when it is created/destroyed.
 You should not have to call this method explicitly.

### **Release()**

Decrements the reference count of this resource. If the reference count reaches 0, the resource is unloaded.

```csharp
public void Release()
```

**Remarks:**

This class automatically increments and decrements the reference counter when it is created/destroyed.
 You should not have to call this method explicitly.

### **LoadFrom(MtStream)**

Deserializes this resource from the specified stream.

```csharp
public bool LoadFrom(MtStream stream)
```

#### Parameters

`stream` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>
The stream to deserialize this resource from.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the resource was deserialized successfully, false otherwise.

### **SaveTo(MtStream)**

Serializes this resource to the specified stream.

```csharp
public bool SaveTo(MtStream stream)
```

#### Parameters

`stream` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>
The stream to serialize this resource to.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the resource was serialized successfully, false otherwise.
