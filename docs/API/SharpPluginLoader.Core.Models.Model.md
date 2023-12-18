# Model

Namespace: SharpPluginLoader.Core.Models

Represents an instance of a uMhModel class

```csharp
public class Model : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Model](./SharpPluginLoader.Core.Models.Model.md)

## Properties

### **Position**

The position of the model

```csharp
public MtVector3& Position { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Size**

The size of the model

```csharp
public MtVector3& Size { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **CollisionPosition**

The position of the model's collision box

```csharp
public MtVector3& CollisionPosition { get; }
```

#### Property Value

[MtVector3&](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Rotation**

The rotation of the model

```csharp
public MtQuaternion& Rotation { get; }
```

#### Property Value

[MtQuaternion&](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **Forward**

The model's forward vector

```csharp
public MtVector3 Forward { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **AnimationFrame**

The current frame of the model's current animation

```csharp
public float AnimationFrame { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **MaxAnimationFrame**

The frame count of the model's current animation

```csharp
public float MaxAnimationFrame { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **AnimationSpeed**

The speed of the model's current animation. Note, this value gets set every frame.

```csharp
public float AnimationSpeed { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **CurrentAnimation**

Gets the current animation of the model

```csharp
public AnimationId& CurrentAnimation { get; }
```

#### Property Value

[AnimationId&](./SharpPluginLoader.Core.Components.AnimationId.md)<br>

### **AnimationLayer**

The model's animation component

```csharp
public AnimationLayerComponent AnimationLayer { get; }
```

#### Property Value

[AnimationLayerComponent](./SharpPluginLoader.Core.Components.AnimationLayerComponent.md)<br>

### **MotionLists**

The model's motion lists.

```csharp
public IEnumerable<MotionList> MotionLists { get; }
```

#### Property Value

[IEnumerable&lt;MotionList&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Model(IntPtr)**

```csharp
public Model(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Model()**

```csharp
public Model()
```

## Methods

### **Teleport(MtVector3)**

Teleports the model to the given position

```csharp
public void Teleport(MtVector3 position)
```

#### Parameters

`position` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The target position

**Remarks:**

Use this function if you need to move a model and ignore walls.

### **Resize(Single)**

Resizes the model on all axes to the given size

```csharp
public void Resize(float size)
```

#### Parameters

`size` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The new size of the model

### **PauseAnimations()**

Pauses the model's current animation

```csharp
public void PauseAnimations()
```

### **ResumeAnimations()**

Resumes the model's current animation

```csharp
public void ResumeAnimations()
```
