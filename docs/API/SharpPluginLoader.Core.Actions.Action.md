# Action

Namespace: SharpPluginLoader.Core.Actions

Represents an instance of the cActionBase class.

```csharp
public class Action : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Action](./SharpPluginLoader.Core.Actions.Action.md)

## Properties

### **ActiveTime**

The amount of time the action has been active.

```csharp
public Single& ActiveTime { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **DeltaSec**

The actions delta time.

```csharp
public Single& DeltaSec { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Flags**

The flags of the action.

```csharp
public UInt64& Flags { get; }
```

#### Property Value

[UInt64&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt64&)<br>

### **Name**

The name of the action.

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **Action(IntPtr)**

```csharp
public Action(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Action()**

```csharp
public Action()
```
