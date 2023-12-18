# EffectProvider

Namespace: SharpPluginLoader.Core.Resources

Represents an instance of the rEffectProvider class.

```csharp
public class EffectProvider : Resource
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Resource](./SharpPluginLoader.Core.Resources.Resource.md) → [EffectProvider](./SharpPluginLoader.Core.Resources.EffectProvider.md)

## Properties

### **FilePath**

Gets the file path of this resource without the extension.

```csharp
public string FilePath { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **FileExtension**

Gets the file extension of this resource.

```csharp
public string FileExtension { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **RefCount**

Gets the reference count of this resource. If the reference count reaches 0, the resource is unloaded.

```csharp
public uint RefCount { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **EffectProvider(IntPtr)**

```csharp
public EffectProvider(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **EffectProvider()**

```csharp
public EffectProvider()
```

## Methods

### **GetEffect(UInt32, UInt32)**

Gets an effect by its group and id which can be passed to [Entity.CreateEffect(MtObject)](./SharpPluginLoader.Core.Entities.Entity.md#createeffectmtobject).

```csharp
public MtObject GetEffect(uint group, uint id)
```

#### Parameters

`group` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The group the effect is in

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The id of the effect

#### Returns

[MtObject](./SharpPluginLoader.Core.MtObject.md)<br>
The effect or null
