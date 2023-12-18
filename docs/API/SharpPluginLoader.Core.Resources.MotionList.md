# MotionList

Namespace: SharpPluginLoader.Core.Resources

Represents an instance of the rMotionList (lmt) class.

```csharp
public class MotionList : Resource
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Resource](./SharpPluginLoader.Core.Resources.Resource.md) → [MotionList](./SharpPluginLoader.Core.Resources.MotionList.md)

## Properties

### **Header**

The header of this motion list.

```csharp
public MotionListHeader& Header { get; }
```

#### Property Value

[MotionListHeader&](./SharpPluginLoader.Core.Resources.Animation.MotionListHeader.md)<br>

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

### **MotionList(IntPtr)**

```csharp
public MotionList(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MotionList()**

```csharp
public MotionList()
```
