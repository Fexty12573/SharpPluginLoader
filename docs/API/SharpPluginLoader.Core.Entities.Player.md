# Player

Namespace: SharpPluginLoader.Core.Entities

```csharp
public class Player : Entity
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Model](./SharpPluginLoader.Core.Models.Model.md) → [Entity](./SharpPluginLoader.Core.Entities.Entity.md) → [Player](./SharpPluginLoader.Core.Entities.Player.md)

## Properties

### **SingletonInstance**

The sPlayer singleton instance

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MainPlayer**

The main player

```csharp
public static Player MainPlayer { get; }
```

#### Property Value

[Player](./SharpPluginLoader.Core.Entities.Player.md)<br>

### **CurrentWeapon**

The currently equipped weapon

```csharp
public Weapon CurrentWeapon { get; }
```

#### Property Value

[Weapon](./SharpPluginLoader.Core.Weapons.Weapon.md)<br>

### **CurrentWeaponType**

The currently equipped weapon type

```csharp
public WeaponType CurrentWeaponType { get; }
```

#### Property Value

[WeaponType](./SharpPluginLoader.Core.Entities.WeaponType.md)<br>

### **CollisionComponent**

Gets the entity's collision component

```csharp
public CollisionComponent CollisionComponent { get; }
```

#### Property Value

[CollisionComponent](./SharpPluginLoader.Core.Components.CollisionComponent.md)<br>

### **ActionController**

The entity's action controller

```csharp
public ActionController ActionController { get; }
```

#### Property Value

[ActionController](./SharpPluginLoader.Core.ActionController.md)<br>

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

### **Player(IntPtr)**

```csharp
public Player(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Player()**

```csharp
public Player()
```

## Methods

### **CreateShell(UInt32, MtVector3, Nullable&lt;MtVector3&gt;)**

```csharp
public void CreateShell(uint index, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>

### **CreateShell(ShellParamList, UInt32, MtVector3, Nullable&lt;MtVector3&gt;)**

```csharp
public void CreateShell(ShellParamList shll, uint index, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`shll` [ShellParamList](./SharpPluginLoader.Core.Resources.ShellParamList.md)<br>

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>

### **CreateShell(ShellParam, MtVector3, Nullable&lt;MtVector3&gt;)**

```csharp
public void CreateShell(ShellParam shell, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`shell` [ShellParam](./SharpPluginLoader.Core.Resources.ShellParam.md)<br>

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>

### **RegisterMbd(Resource, UInt32)**

```csharp
public void RegisterMbd(Resource mbd, uint index)
```

#### Parameters

`mbd` [Resource](./SharpPluginLoader.Core.Resources.Resource.md)<br>

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
