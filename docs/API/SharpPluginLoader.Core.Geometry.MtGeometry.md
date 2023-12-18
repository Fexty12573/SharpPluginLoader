# MtGeometry

Namespace: SharpPluginLoader.Core.Geometry

```csharp
public class MtGeometry : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtGeometry](./SharpPluginLoader.Core.Geometry.MtGeometry.md)

## Properties

### **Type**

```csharp
public GeometryType Type { get; }
```

#### Property Value

[GeometryType](./SharpPluginLoader.Core.Geometry.GeometryType.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtGeometry(IntPtr)**

```csharp
public MtGeometry(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtGeometry()**

```csharp
public MtGeometry()
```

## Methods

### **GetGeometry&lt;T&gt;()**

```csharp
public T& GetGeometry<T>()
```

#### Type Parameters

`T`<br>

#### Returns

T&<br>
