# Monster

Namespace: SharpPluginLoader.Core.Entities

```csharp
public class Monster : Entity
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Model](./SharpPluginLoader.Core.Models.Model.md) → [Entity](./SharpPluginLoader.Core.Entities.Entity.md) → [Monster](./SharpPluginLoader.Core.Entities.Monster.md)

## Properties

### **SingletonInstance**

The sEnemy singleton instance

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Type**

The id of the monster

```csharp
public MonsterType Type { get; }
```

#### Property Value

[MonsterType](./SharpPluginLoader.Core.Entities.MonsterType.md)<br>

### **Variant**

The variant (sub id) of the monster

```csharp
public uint Variant { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Name**

The name of the monster

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Health**

The current health of the monster

```csharp
public Single& Health { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **MaxHealth**

The maximum health of the monster

```csharp
public Single& MaxHealth { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Speed**

The speed of the monster (1.0 is normal speed)

```csharp
public Single& Speed { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **DespawnTime**

The despawn time of the monster (in seconds)

```csharp
public Single& DespawnTime { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **DifficultyIndex**

The index of the monster in the difficulty table (dtt_dif)

```csharp
public UInt32& DifficultyIndex { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **Frozen**

Freezes the monster and pauses all ai processing

```csharp
public bool Frozen { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **AiData**

The AI data of the monster

```csharp
public nint AiData { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

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

### **Monster(IntPtr)**

Constructs a new Monster from a native pointer

```csharp
public Monster(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>
The native pointer

### **Monster()**

Constructs a new Monster from a nullptr

```csharp
public Monster()
```

## Methods

### **GetAllMonsters()**

Gets a list of all monsters in the game

```csharp
public static IEnumerable<Monster> GetAllMonsters()
```

#### Returns

[IEnumerable&lt;Monster&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1)<br>

### **ForceAction(Int32)**

Forces the monster to do a given action. This will interrupt the current action.

```csharp
public void ForceAction(int id)
```

#### Parameters

`id` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the action to execute

### **Enrage()**

Tries to enrage the monster

```csharp
public bool Enrage()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Unenrage()**

Unenrages the monster

```csharp
public void Unenrage()
```

### **SetTarget(IntPtr)**

Sets the monster's target

```csharp
public void SetTarget(nint target)
```

#### Parameters

`target` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CreateShell(UInt32, MtVector3, Nullable&lt;MtVector3&gt;)**

```csharp
public void CreateShell(uint index, MtVector3 target, Nullable<MtVector3> origin)
```

#### Parameters

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`target` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`origin` [Nullable&lt;MtVector3&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>

### **ToString(MonsterType)**

Gets the human readable name of the given monster id

```csharp
public static string ToString(MonsterType type)
```

#### Parameters

`type` [MonsterType](./SharpPluginLoader.Core.Entities.MonsterType.md)<br>
The monster id

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The human readable name of the monster

**Remarks:**

This is equivalent to the [Monster.Name](./SharpPluginLoader.Core.Entities.Monster.md#name) property on a monster with this

### **DisableSpeedReset()**

Disables the periodic speed reset that the game does

```csharp
public static void DisableSpeedReset()
```

**Remarks:**

Call this before changing the [Monster.Speed](./SharpPluginLoader.Core.Entities.Monster.md#speed) property on a monster. This setting applies globally

### **EnableSpeedReset()**

Reenables the periodic speed reset that the game does

```csharp
public static void EnableSpeedReset()
```

### **Initialize()**

```csharp
internal static void Initialize()
```
