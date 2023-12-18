# ObjCollision

Namespace: SharpPluginLoader.Core.Resources

```csharp
public class ObjCollision : Resource
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Resource](./SharpPluginLoader.Core.Resources.Resource.md) → [ObjCollision](./SharpPluginLoader.Core.Resources.ObjCollision.md)

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

### **ObjCollision(IntPtr)**

```csharp
public ObjCollision(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **ObjCollision()**

```csharp
public ObjCollision()
```
