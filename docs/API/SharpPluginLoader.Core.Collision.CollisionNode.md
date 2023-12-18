# CollisionNode

Namespace: SharpPluginLoader.Core.Collision

Represents a cCollisionNode object in the game.

```csharp
public class CollisionNode : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [CollisionNode](./SharpPluginLoader.Core.Collision.CollisionNode.md)

## Properties

### **IsActive**

Whether the node is active.

```csharp
public Boolean& IsActive { get; }
```

#### Property Value

[Boolean&](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean&)<br>

### **GeometryArray**

The geometry array of the node.

```csharp
public MtArray<MtObject> GeometryArray { get; }
```

#### Property Value

[MtArray&lt;MtObject&gt;](./SharpPluginLoader.Core.MtArray-1.md)<br>

### **BoundingBox**

The bounding box of the node.

```csharp
public MtAabb& BoundingBox { get; }
```

#### Property Value

[MtAabb&](./SharpPluginLoader.Core.MtTypes.MtAabb.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **CollisionNode(IntPtr)**

```csharp
public CollisionNode(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CollisionNode()**

```csharp
public CollisionNode()
```
