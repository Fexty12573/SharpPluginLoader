# Game Objects

## Introduction
Monster Hunter World runs on the MT Framework engine, which is a proprietary engine developed by Capcom. Both the engine and the game are written in C++. (Officially, it uses World Engine, which is what Capcom calls the heavily modified version of MT Framework that they use for MHW but that's besides the point.)

The main object class in the game is `MtObject`. It is similar to what `object` is in C#. It is the base class for most objects in the game. Anything from a monster to a weapon to a particle effect is an `MtObject`. It is a very simple class, and doesn't contain any fields (aside from the vtable pointer).

In the framework, the `MtObject` class acts as a wrapper around a pointer to an `MtObject` in the game. It provides a couple helper methods to make it easier to work with, but it is mostly just a wrapper around the pointer.

It provides some helper methods to read and write data inside it.
```csharp
public unsafe void MyMethod(MtObject obj)
{
    // Read a float from the object
    var floatValue = obj.Get<float>(0x10);
    // Write a float to the object
    obj.Set<float>(0x10, floatValue * 3f);
    // Get a reference to the float
    obj.GetRef<float>(0x10) *= 3f;
    // Get a pointer to the float
    *obj.GetPtr<float>(0x10) = 3.14f;
    // Read an object
    var otherObject = obj.GetObject<MtObject>(0x20);
}
```
You can get a full breakdown of the `MtObject` class in the [API Reference](../API/SharpPluginLoader.Core.MtObject.md).

## Object Hierarchy
There is an important distinction to make between the object hierarchy of the framework and the one of the game. The framework does not provide a wrapper around every class in the game, only a select few that are generally useful.

These classes also inherit from each other, however you need to be careful when casting between them or checking their type.

For example, take this code:
```csharp
public void OnEntityAction(Entity entity, ref ActionInfo action)
{
    // WRONG
    if (entity is Player player)
    {
        // Do something with the player
    }
}
```

This code will not work. It will not crash, the check will never evaluate to true. While `Player` does inherit from `Entity`, the `entity` passed to the `OnEntityAction` event is always just an `Entity`. If you want to check the games inheritance you can make use of the DTI:
```csharp
public void OnEntityAction(Entity entity, ref ActionInfo action)
{
    // Correct
    if (entity.Is("uPlayer"))
    {
        var player = entity.As<Player>();
        // Do something with the player
    }
}
```

This code will work. The `Is` method checks the DTI of the object. DTI stands for "Data Type Information" and is information embedded into the executable about the different types. You can read up more on the subject [here](https://github.com/Ezekial711/MonsterHunterWorldModding/wiki/The-DTI-and-MtFramework-2.0).

## MtDti
The `MtDti` class is a wrapper around the DTI of an object. You can get the DTI of an object by calling the `GetDti` method on it. The DTI is a struct that contains information about the type of the object. It contains information about the name, size, and sometimes even fields of the type.
```csharp
var dti = entity.GetDti();
Log.Info($"Entity is a {dti.Name} and is {dti.Size} bytes large");
```

You can also get the DTI of any type by using the `MtDti.Find` method.
```csharp
var dti = MtDti.Find("uEm045");
```

## MtArray
The most common type of array in the game is an `MtArray`. The framework provides a wrapper around this type called `MtArray`. It is a generic class, so you can use it with any type. They are also `MtObject`s, so you can do all the same things with them that you can do with any other `MtObject`. `MtArray`s are basically just a resizable array. They are similar to `List` in C#. They can only hold game objects, and not any kind of data like integers or floats. I.e. `MtArray<int>` does not exist, but `MtArray<Entity>` does.
```csharp
public void MyMethod(MtArray<Monster> monsters)
{
    foreach (var monster in monsters)
    {
        if (monster != null)
            Log.Info($"Monsters contains a {monster.Name}");
    }
}
```

## Creating your own MtObject types
If there is a native class that you want to write a wrapper for, you can also inherit from MtObject.

For example, say we want to make a class that wraps the native `cEquipWork` class. We can do this by inheriting from `MtObject` and adding the fields we want to access.
```csharp
public class EquipWork : MtObject
{
    public EquipWork(nint instance) : base(instance) { }
    public EquipWork() { }
}
```
> [!NOTE]
> The parameterless constructor is only required if you want to use this object with `NativeWrapper.GetObject<T>()` (and similar), as those methods
> require a parameterless constructor.

Now we can add the fields we want to access. We can use the methods provided by `NativeWrapper` (which `MtObject` inherits from) to read and write data to the native object.
```csharp
public class EquipWork : MtObject
{
    public EquipWork(nint instance) : base(instance) { }
    public EquipWork() { }

    public int EquipId
    {
        get => Get<int>(0x10);
        set => Set<int>(0x10, value);
    }
}
```

### Accessing value types
There are multiple ways to access value types. Above, using separate `Get`/`Set` calls are being used. Another way is to use the `GetRef` method:
```csharp
public ref int EquipId => ref GetRef<int>(0x10);
```

### Accessing objects
You can access objects in the same manner, using the `GetObject` method:
```csharp
public Entity Owner
{
    get => GetObject<Entity>(0x20);
    set => SetObject<Entity>(0x20, value);
}
```
This however is only if the object is stored as a pointer. If it is stored inline, you can use the `GetInlineObject` method:
```csharp
public Entity Monster => GetInlineObject<Entity>(0x20);
```
In this case it is also unnecessary to provide a setter, as there is no pointer to modify.

### Accessing arrays
There are 3 kinds of arrays in the game. `MtArray`, heap-allocated arrays, and inline arrays. `MtArray` is the only one that is wrapped by the framework. 

#### MtArray
Note that `GetObject` is only used for demonstration purposes here. Usually the actual `MtArray` object is inlined so you would use `GetInlineObject` instead.
```csharp
public MtArray<int> Array => GetObject<MtArray<int>>(0x20);
```

#### Heap-allocated arrays
By 'heap-allocated array' I mean an array where the containing object only holds a pointer to the items. In this case you can simply read a `void*` from the object and wrap that in a `Span<T>`.
```csharp
public Span<int> Array => new Span<int>(GetPtr(0x20), 10);
```
> [!NOTE]
> If you want to use a native array in an async or an iterator method, you can use `NativeArray<T>` instead.

#### Inline arrays
Inline arrays are arrays that are stored directly inside the object. The approach here is similar to the one for heap-allocated arrays, except that you use `GetPtrInline` instead of `GetPtr`.
```csharp
public Span<int> Array => new Span<int>(GetPtrInline(0x20), 10);
```

> [!WARNING]
> Both `Span<T>` and `NativeArray<T>` must **not** be used with any reference types. This includes `MtObject`. Instead use `Span<nint>` or `NativeArray<nint>` and simply construct an object when you need it (e.g. `new MtObject(span[i])`).
