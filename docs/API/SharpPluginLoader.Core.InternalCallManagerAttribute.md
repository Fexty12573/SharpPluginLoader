# InternalCallManagerAttribute

Namespace: SharpPluginLoader.Core

This attribute is used to mark a class as an internal call manager.
 There can only be one internal call manager per plugin.
 
 The internal call manager is where the plugin keeps all of its internal calls.

```csharp
public class InternalCallManagerAttribute : System.Attribute
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute) → [InternalCallManagerAttribute](./SharpPluginLoader.Core.InternalCallManagerAttribute.md)

## Properties

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object)<br>

## Constructors

### **InternalCallManagerAttribute()**

```csharp
public InternalCallManagerAttribute()
```
