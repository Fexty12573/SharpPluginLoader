# AnimationLayerComponent

Namespace: SharpPluginLoader.Core.Components

Represents an instance of the cpAnimationLayer class.

```csharp
public class AnimationLayerComponent : Component
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Component](./SharpPluginLoader.Core.Components.Component.md) → [AnimationLayerComponent](./SharpPluginLoader.Core.Components.AnimationLayerComponent.md)

## Properties

### **Owner**

The owner of this animation layer.

```csharp
public Entity Owner { get; }
```

#### Property Value

[Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>

### **CurrentFrame**

The current frame of the animation.

```csharp
public Single& CurrentFrame { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **MaxFrame**

The frame count of the animation.

```csharp
public Single& MaxFrame { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Speed**

The speed of the animation.

```csharp
public Single& Speed { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Paused**

Gets or sets whether the animation is paused.

```csharp
public bool Paused { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

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

### **AnimationLayerComponent(IntPtr)**

```csharp
public AnimationLayerComponent(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **AnimationLayerComponent()**

```csharp
public AnimationLayerComponent()
```

## Methods

### **LockSpeed(Single)**

Locks the speed of this animation layer.

```csharp
public void LockSpeed(float speed)
```

#### Parameters

`speed` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The speed to lock it to

### **UnlockSpeed()**

Unlocks the speed of this animation layer.

```csharp
public void UnlockSpeed()
```

### **Pause()**

Pauses the animation layer.

```csharp
public void Pause()
```

### **Resume()**

Resumes the animation layer.

```csharp
public void Resume()
```

### **RegisterLmt(MotionList, UInt32)**

Registers an LMT to this animation layer.

```csharp
public void RegisterLmt(MotionList lmt, uint index)
```

#### Parameters

`lmt` [MotionList](./SharpPluginLoader.Core.Resources.MotionList.md)<br>
The lmt to register

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The index to register the lmt in. Must be less than 16

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException)<br>

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>

### **DoAnimation(AnimationId, Single, Single, Single, UInt32)**

Forces the animation layer to do the given animation. This will interrupt the current animation.

```csharp
public void DoAnimation(AnimationId id, float interpolationFrame, float startFrame, float speed, uint attr)
```

#### Parameters

`id` [AnimationId](./SharpPluginLoader.Core.Components.AnimationId.md)<br>
The id of the animation

`interpolationFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The amount of frames used to interpolate. 0 will auto-calculate the necessary frames

`startFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The starting frame of the animation

`speed` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The initial speed of the animation

`attr` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
Additional flags

### **DoAnimationSafe(AnimationId, Single, Single, Single, UInt32)**

Forces the animation layer to do the given animation. This will interrupt the current animation.

```csharp
public void DoAnimationSafe(AnimationId id, float interpolationFrame, float startFrame, float speed, uint attr)
```

#### Parameters

`id` [AnimationId](./SharpPluginLoader.Core.Components.AnimationId.md)<br>
The id of the animation

`interpolationFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The amount of frames used to interpolate. 0 will auto-calculate the necessary frames

`startFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The starting frame of the animation

`speed` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The initial speed of the animation

`attr` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
Additional flags

**Remarks:**

If the owner object of the animation layer this function is called on is an entity,
 it will instead call the function dedicated to entities. Note that in that case the 
 parameter will be ignored.

### **Initialize()**

```csharp
internal static void Initialize()
```
