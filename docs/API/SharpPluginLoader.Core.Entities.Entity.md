# Entity

Namespace: SharpPluginLoader.Core.Entities

Represents a Monster Hunter World uCharacterModel instance.

```csharp
public class Entity : SharpPluginLoader.Core.Models.Model
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Model](./SharpPluginLoader.Core.Models.Model.md) → [Entity](./SharpPluginLoader.Core.Entities.Entity.md)

## Properties

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

[ActionController](./SharpPluginLoader.Core.Actions.ActionController.md)<br>

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

### **Entity(IntPtr)**

```csharp
public Entity(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Entity()**

```csharp
public Entity()
```

## Methods

### **CreateEffect(UInt32, UInt32)**

Creates an effect on the entity

```csharp
public void CreateEffect(uint groupId, uint effectId)
```

#### Parameters

`groupId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The efx group id

`effectId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The efx id

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>

### **CreateEffect(EffectProvider, UInt32, UInt32)**

Creates an effect on the entity from the given epv file

```csharp
public void CreateEffect(EffectProvider epv, uint groupId, uint effectId)
```

#### Parameters

`epv` [EffectProvider](./SharpPluginLoader.Core.Resources.EffectProvider.md)<br>
The EPV file to take the efx from

`groupId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The efx group id

`effectId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The efx id

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>

**Remarks:**

Tip: You can load any EPV file using [ResourceManager.GetResource&lt;T&gt;(String, MtDti, UInt32)](./SharpPluginLoader.Core.ResourceManager.md#getresourcetstring-mtdti-uint32)

### **CreateEffect(MtObject)**

Creates the given effect on the entity

```csharp
public void CreateEffect(MtObject effect)
```

#### Parameters

`effect` [MtObject](./SharpPluginLoader.Core.MtObject.md)<br>
The effect to create

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException)<br>

### **CreateShell(Int32, Int32, MtVector3, Nullable&lt;MtVector3&gt;)**

Spawns a shell on the entity

```csharp
public void CreateShell(int shllIndex, int shlpIndex, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`shllIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the shll in the entities shll list

`shlpIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the shlp in the shll

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The position the shell should travel towards

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The origin of the shell (or null for the entity itself)

### **CreateShell(Int32, Int32)**

Spawns a shell on the entity

```csharp
public void CreateShell(int shllIndex, int shlpIndex)
```

#### Parameters

`shllIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the shll in the entities shll list

`shlpIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the shlp in the shll

### **CreateShell(UInt32, MtVector3, Nullable&lt;MtVector3&gt;)**

Spawns a shell on the entity

```csharp
public void CreateShell(uint index, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The index of the shell in the entities shell list (shll)

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The position the shell should travel towards

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The origin of the shell (or null for the entity itself)

### **CreateShell(ShellParamList, UInt32, MtVector3, Nullable&lt;MtVector3&gt;)**

Spawns a shell on the entity from the given shll file

```csharp
public void CreateShell(ShellParamList shll, uint index, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`shll` [ShellParamList](./SharpPluginLoader.Core.Resources.ShellParamList.md)<br>
The shll file to take the shell from

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The index of the shell in the entities shell list (shll)

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The position the shell should travel towards

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The origin of the shell (or null for the entity itself)

**Remarks:**

Tip: You can load any shll file using [ResourceManager.GetResource&lt;T&gt;(String, MtDti, UInt32)](./SharpPluginLoader.Core.ResourceManager.md#getresourcetstring-mtdti-uint32)

### **CreateShell(ShellParam, MtVector3, Nullable&lt;MtVector3&gt;)**

Spawns the given shell on the entity

```csharp
public void CreateShell(ShellParam shell, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`shell` [ShellParam](./SharpPluginLoader.Core.Resources.ShellParam.md)<br>
The shell to spawn

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
The target position of the shell

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The origin position of the shell (or null for the entity itself)

### **RegisterShll(ShellParamList, Int32, Boolean)**

Registers a shll file on the entity

```csharp
public bool RegisterShll(ShellParamList shll, int index, bool force)
```

#### Parameters

`shll` [ShellParamList](./SharpPluginLoader.Core.Resources.ShellParamList.md)<br>
The shll to register

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index at which to register, must be less than 8. Passing -1 for this parameter will register the shll at the first free index.

`force` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether to force the registration or not (only works when an index is specified)

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the shll was successfully registered

### **RegisterShll(String, Int32)**

Registers a shll file on the entity

```csharp
public bool RegisterShll(string path, int index)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The path to the shll file

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index at which to register, must be less than 8. Passing -1 for this parameter will register the shll at the first free index.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the shll was successfully registered
