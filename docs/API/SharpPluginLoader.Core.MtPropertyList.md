# MtPropertyList

Namespace: SharpPluginLoader.Core

```csharp
public class MtPropertyList : MtObject, System.Collections.Generic.IEnumerable`1[[SharpPluginLoader.Core.MtProperty, SharpPluginLoader.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.IEnumerable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtPropertyList](./SharpPluginLoader.Core.MtPropertyList.md)<br>
Implements [IEnumerable&lt;MtProperty&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1), [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable)

## Properties

### **First**

```csharp
public MtProperty First { get; }
```

#### Property Value

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **Count**

```csharp
public uint Count { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Properties**

```csharp
public MtProperty[] Properties { get; }
```

#### Property Value

[MtProperty[]](./SharpPluginLoader.Core.MtProperty.md)<br>

### **Item**

```csharp
public MtProperty Item { get; }
```

#### Property Value

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtPropertyList(IntPtr)**

```csharp
public MtPropertyList(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtPropertyList()**

```csharp
public MtPropertyList()
```

## Methods

### **Finalize()**

```csharp
protected void Finalize()
```

### **FindProperty(String)**

```csharp
public MtProperty FindProperty(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

#### Returns

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **FindProperty(PropType, String)**

```csharp
public MtProperty FindProperty(PropType type, string name)
```

#### Parameters

`type` [PropType](./SharpPluginLoader.Core.PropType.md)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

#### Returns

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **FindProperty(UInt32)**

```csharp
public MtProperty FindProperty(uint hash)
```

#### Parameters

`hash` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

#### Returns

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **GetEnumerator()**

```csharp
public IEnumerator<MtProperty> GetEnumerator()
```

#### Returns

[IEnumerator&lt;MtProperty&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerator-1)<br>
