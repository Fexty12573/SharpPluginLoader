# ResourceManager

Namespace: SharpPluginLoader.Core

```csharp
public static class ResourceManager
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ResourceManager](./SharpPluginLoader.Core.ResourceManager.md)

## Properties

### **SingletonInstance**

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Methods

### **GetResource&lt;T&gt;(String, MtDti, UInt32)**

```csharp
public static T GetResource<T>(string path, MtDti dti, uint flags)
```

#### Type Parameters

`T`<br>

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`dti` [MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

`flags` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

#### Returns

T<br>
