# ActionController

Namespace: SharpPluginLoader.Core

Represents an instance of the cActionController class.

```csharp
public class ActionController : MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [ActionController](./SharpPluginLoader.Core.ActionController.md)

## Properties

### **CurrentAction**

The action that is currently being performed.

```csharp
public ActionInfo& CurrentAction { get; }
```

#### Property Value

[ActionInfo&](./SharpPluginLoader.Core.ActionInfo.md)<br>

### **NextAction**

The action that will be performed next.

```csharp
public ActionInfo& NextAction { get; }
```

#### Property Value

[ActionInfo&](./SharpPluginLoader.Core.ActionInfo.md)<br>

### **PreviousAction**

The action that was performed before the current one.

```csharp
public ActionInfo& PreviousAction { get; }
```

#### Property Value

[ActionInfo&](./SharpPluginLoader.Core.ActionInfo.md)<br>

### **Owner**

The owner of this action controller.

```csharp
public Entity Owner { get; }
```

#### Property Value

[Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **ActionController(IntPtr)**

```csharp
public ActionController(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **ActionController()**

```csharp
public ActionController()
```

## Methods

### **GetActionList(Int32)**

Gets an action list by its index.

```csharp
public ActionList GetActionList(int actionSet)
```

#### Parameters

`actionSet` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the action set

#### Returns

[ActionList](./SharpPluginLoader.Core.ActionList.md)<br>
The list of actions for the requested action set

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException)<br>

### **DoAction(Int32, Int32)**

Makes the entity perform an action.

```csharp
public void DoAction(int actionSet, int actionId)
```

#### Parameters

`actionSet` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The action set to use

`actionId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the action within the action set

**Remarks:**

For monsters use the [Monster.ForceAction(Int32)](./SharpPluginLoader.Core.Entities.Monster.md#forceactionint32) method instead.
