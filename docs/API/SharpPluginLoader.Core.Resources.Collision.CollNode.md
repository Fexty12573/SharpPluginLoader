# CollNode

Namespace: SharpPluginLoader.Core.Resources.Collision

```csharp
public class CollNode : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [CollNode](./SharpPluginLoader.Core.Resources.Collision.CollNode.md)

## Properties

### **Geometry**

```csharp
public CollGeomResource Geometry { get; }
```

#### Property Value

[CollGeomResource](./SharpPluginLoader.Core.Resources.Collision.CollGeomResource.md)<br>

### **Index**

```csharp
public Int16& Index { get; }
```

#### Property Value

[Int16&](https://docs.microsoft.com/en-us/dotnet/api/System.Int16&)<br>

### **CollNodeFlags**

```csharp
public UInt32& CollNodeFlags { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **HitCollisionFlags**

```csharp
public UInt32& HitCollisionFlags { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **Attr**

```csharp
public UInt32& Attr { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **CollNode(IntPtr)**

```csharp
public CollNode(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CollNode()**

```csharp
public CollNode()
```
