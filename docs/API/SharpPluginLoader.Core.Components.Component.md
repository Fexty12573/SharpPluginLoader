# Component

Namespace: SharpPluginLoader.Core.Components

```csharp
public class Component : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Component](./SharpPluginLoader.Core.Components.Component.md)

## Properties

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

### **Component(IntPtr)**

```csharp
public Component(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Component()**

```csharp
public Component()
```
