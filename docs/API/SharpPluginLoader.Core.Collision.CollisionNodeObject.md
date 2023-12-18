# CollisionNodeObject

Namespace: SharpPluginLoader.Core.Collision

Represents a cCollisionNodeObject object in the game.

```csharp
public class CollisionNodeObject : CollisionNode
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [CollisionNode](./SharpPluginLoader.Core.Collision.CollisionNode.md) → [CollisionNodeObject](./SharpPluginLoader.Core.Collision.CollisionNodeObject.md)

## Properties

### **Owner**

The object that owns this node.

```csharp
public MtObject Owner { get; }
```

#### Property Value

[MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

### **Attributes**

The attributes of the node.

```csharp
public UInt32& Attributes { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **MoveVector**

The move vector of the node.

```csharp
public MtVector4& MoveVector { get; }
```

#### Property Value

[MtVector4&](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

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

### **CollisionNodeObject(IntPtr)**

```csharp
public CollisionNodeObject(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CollisionNodeObject()**

```csharp
public CollisionNodeObject()
```
