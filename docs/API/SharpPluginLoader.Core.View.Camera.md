# Camera

Namespace: SharpPluginLoader.Core.View

```csharp
public class Camera : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Camera](./SharpPluginLoader.Core.View.Camera.md)

## Properties

### **Position**

The position of the camera, in world space coordinates.

```csharp
public MtVector3& Position { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Up**

The up vector of the camera, in local coordinates.

```csharp
public MtVector3& Up { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Target**

The target of the camera, in local coordinates.

```csharp
public MtVector3& Target { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **FarClip**

The far clip plane of the camera.

```csharp
public Single& FarClip { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **NearClip**

The near clip plane of the camera.

```csharp
public Single& NearClip { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **AspectRatio**

The aspect ratio of the camera.

```csharp
public Single& AspectRatio { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **FieldOfView**

The field of view of the camera, in radians.

```csharp
public Single& FieldOfView { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Camera(IntPtr)**

```csharp
public Camera(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Camera()**

```csharp
public Camera()
```

## Methods

### **GetTargetWorld()**

Gets the target of the camera in world space coordinates.

```csharp
public MtVector3 GetTargetWorld()
```

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The target of the camera in world space coordinates.

### **GetViewMatrix()**

Computes the view matrix of the camera.

```csharp
public MtMatrix4X4 GetViewMatrix()
```

#### Returns

[MtMatrix4X4](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>
The view matrix of the camera.

**Remarks:**

Warning: This method does not perform any caching. It is recommended to cache the result of this method.

### **GetProjectionMatrix()**

Computes the projection matrix of the camera.

```csharp
public MtMatrix4X4 GetProjectionMatrix()
```

#### Returns

[MtMatrix4X4](./SharpPluginLoader.Core.MtTypes.MtMatrix4X4.md)<br>
The projection matrix of the camera.

**Remarks:**

Warning: This method does not perform any caching. It is recommended to cache the result of this method.
