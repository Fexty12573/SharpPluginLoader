# CollisionComponent

Namespace: SharpPluginLoader.Core.Components

```csharp
public class CollisionComponent : Component
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Component](./SharpPluginLoader.Core.Components.Component.md) → [CollisionComponent](./SharpPluginLoader.Core.Components.CollisionComponent.md)

## Properties

### **Nodes**

Gets the s of this [CollisionComponent](./SharpPluginLoader.Core.Components.CollisionComponent.md).

```csharp
public MtArray<Node> Nodes { get; }
```

#### Property Value

[MtArray&lt;Node&gt;](./SharpPluginLoader.Core.MtArray-1.md)<br>

### **Model**

Gets the owner of this [CollisionComponent](./SharpPluginLoader.Core.Components.CollisionComponent.md).

```csharp
public Model Model { get; }
```

#### Property Value

[Model](./SharpPluginLoader.Core.Models.Model.md)<br>

### **Next**

Gets the next component in the list.

```csharp
public Component Next { get; }
```

#### Property Value

[Component](./SharpPluginLoader.Core.Components.Component.md)<br>

### **Owner**

Gets the owner of this component.

```csharp
public Model Owner { get; }
```

#### Property Value

[Model](./SharpPluginLoader.Core.Models.Model.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **CollisionComponent(IntPtr)**

```csharp
public CollisionComponent(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CollisionComponent()**

```csharp
public CollisionComponent()
```
