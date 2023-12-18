# Weapon

Namespace: SharpPluginLoader.Core.Weapons

Represents an instance of a uWeapon class

```csharp
public class Weapon : SharpPluginLoader.Core.Models.Model
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Model](./SharpPluginLoader.Core.Models.Model.md) → [Weapon](./SharpPluginLoader.Core.Weapons.Weapon.md)

## Properties

### **Type**

Gets the type of the weapon

```csharp
public WeaponType Type { get; }
```

#### Property Value

[WeaponType](./SharpPluginLoader.Core.Entities.WeaponType.md)<br>

### **Holder**

Gets the holder of the weapon if it has one

```csharp
public Entity Holder { get; }
```

#### Property Value

[Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>

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

### **Weapon(IntPtr)**

```csharp
public Weapon(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Weapon()**

```csharp
public Weapon()
```

## Methods

### **RegisterObjCollision(Resource, UInt32)**

Registers the given rObjCollision (.col file) for the weapon. This col can then be called by an lmt.

```csharp
public void RegisterObjCollision(Resource objCollision, uint index)
```

#### Parameters

`objCollision` [Resource](./SharpPluginLoader.Core.Resources.Resource.md)<br>
The col to register

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The index in which to register the col file. Must be less than 8

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException)<br>
