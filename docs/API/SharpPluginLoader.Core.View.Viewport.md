# Viewport

Namespace: SharpPluginLoader.Core.View

```csharp
public class Viewport : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Viewport](./SharpPluginLoader.Core.View.Viewport.md)

## Properties

### **Camera**

Gets the camera of the viewport.

```csharp
public Camera Camera { get; }
```

#### Property Value

[Camera](./SharpPluginLoader.Core.View.Camera.md)<br>

### **Visible**

Gets a boolean indicating whether the viewport is visible.

```csharp
public Boolean& Visible { get; }
```

#### Property Value

[Boolean&](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean&)<br>

### **Region**

The region of the viewport.

```csharp
public MtRect& Region { get; }
```

#### Property Value

[MtRect&](./SharpPluginLoader.Core.MtTypes.MtRect.md)<br>

### **ViewMatrix**

The view matrix of the viewports camera.

```csharp
public MtMatrix4X4& ViewMatrix { get; }
```

#### Property Value

[MtMatrix4X4&](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>

### **ProjectionMatrix**

The projection matrix of the viewports camera.

```csharp
public MtMatrix4X4& ProjectionMatrix { get; }
```

#### Property Value

[MtMatrix4X4&](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>

### **PrevViewMatrix**

The previous view matrix of the viewports camera.

```csharp
public MtMatrix4X4& PrevViewMatrix { get; }
```

#### Property Value

[MtMatrix4X4&](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>

### **PrevProjectionMatrix**

The previous projection matrix of the viewports camera.

```csharp
public MtMatrix4X4& PrevProjectionMatrix { get; }
```

#### Property Value

[MtMatrix4X4&](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Viewport(IntPtr)**

```csharp
public Viewport(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Viewport()**

```csharp
public Viewport()
```

## Methods

### **WorldToScreen(MtVector3, MtVector2&)**

Converts a point in world space to screen space.

```csharp
public bool WorldToScreen(MtVector3 worldPosition, MtVector2& screenPos)
```

#### Parameters

`worldPosition` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The point in world space.

`screenPos` [MtVector2&](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>
The point in screen space.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the point is visible, false otherwise.
