# Primitives

Namespace: SharpPluginLoader.Core.Rendering

Provides methods for rendering 3D "primitives".

```csharp
public static class Primitives
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Primitives](./SharpPluginLoader.Core.Rendering.Primitives.md)

## Methods

### **RenderSphere(MtVector3, Single, MtColor)**

Renders a sphere at the given position with the given radius and color.

```csharp
public static void RenderSphere(MtVector3 position, float radius, MtColor color)
```

#### Parameters

`position` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The position of the sphere.

`radius` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The radius of the sphere.

`color` [MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)<br>
The color of the sphere.

### **RenderSphere(MtVector3, Single, MtVector4)**

Renders a sphere at the given position with the given radius and color.

```csharp
public static void RenderSphere(MtVector3 position, float radius, MtVector4 color)
```

#### Parameters

`position` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The position of the sphere.

`radius` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The radius of the sphere.

`color` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>
The color of the sphere.

### **RenderSphere(MtSphere, MtColor)**

Renders a sphere at the given position with the given radius and color.

```csharp
public static void RenderSphere(MtSphere sphere, MtColor color)
```

#### Parameters

`sphere` [MtSphere](./SharpPluginLoader.Core.MtTypes.MtSphere.md)<br>
The sphere to render.

`color` [MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)<br>
The color of the sphere.

### **RenderSphere(MtSphere, MtVector4)**

Renders a sphere at the given position with the given radius and color.

```csharp
public static void RenderSphere(MtSphere sphere, MtVector4 color)
```

#### Parameters

`sphere` [MtSphere](./SharpPluginLoader.Core.MtTypes.MtSphere.md)<br>
The sphere to render.

`color` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>
The color of the sphere.

### **RenderObb(MtObb, MtColor)**

Renders an oriented bounding box at the given position with the given size and color.

```csharp
public static void RenderObb(MtObb obb, MtColor color)
```

#### Parameters

`obb` [MtObb](./SharpPluginLoader.Core.MtTypes.MtObb.md)<br>
The oriented bounding box to render.

`color` [MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)<br>
The color of the oriented bounding box.

### **RenderObb(MtObb, MtVector4)**

Renders an oriented bounding box at the given position with the given size and color.

```csharp
public static void RenderObb(MtObb obb, MtVector4 color)
```

#### Parameters

`obb` [MtObb](./SharpPluginLoader.Core.MtTypes.MtObb.md)<br>
The oriented bounding box to render.

`color` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>
The color of the oriented bounding box.

### **RenderCapsule(MtCapsule, MtColor)**

Renders a capsule at the given position with the given radius and color.

```csharp
public static void RenderCapsule(MtCapsule capsule, MtColor color)
```

#### Parameters

`capsule` [MtCapsule](./SharpPluginLoader.Core.MtTypes.MtCapsule.md)<br>
The capsule to render.

`color` [MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)<br>
The color of the capsule.

### **RenderCapsule(MtCapsule, MtVector4)**

Renders a capsule at the given position with the given radius and color.

```csharp
public static void RenderCapsule(MtCapsule capsule, MtVector4 color)
```

#### Parameters

`capsule` [MtCapsule](./SharpPluginLoader.Core.MtTypes.MtCapsule.md)<br>
The capsule to render.

`color` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>
The color of the capsule.
