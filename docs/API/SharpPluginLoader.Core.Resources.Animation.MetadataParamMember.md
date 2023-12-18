# MetadataParamMember

Namespace: SharpPluginLoader.Core.Resources.Animation

```csharp
public struct MetadataParamMember
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MetadataParamMember](./SharpPluginLoader.Core.Resources.Animation.MetadataParamMember.md)

## Fields

### **KeyframePtr**

```csharp
public MetadataKeyframe* KeyframePtr;
```

### **KeyframeNum**

```csharp
public int KeyframeNum;
```

### **Hash**

```csharp
public uint Hash;
```

### **KeyframeType**

```csharp
public uint KeyframeType;
```

## Properties

### **Keyframes**

```csharp
public Span<MetadataKeyframe> Keyframes { get; }
```

#### Property Value

[Span&lt;MetadataKeyframe&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

## Methods

### **GetKeyframe(Int32)**

```csharp
MetadataKeyframe& GetKeyframe(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[MetadataKeyframe&](./SharpPluginLoader.Core.Resources.Animation.MetadataKeyframe.md)<br>

### **SortKeyframes()**

```csharp
void SortKeyframes()
```
