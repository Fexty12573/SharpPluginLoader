# Metadata

Namespace: SharpPluginLoader.Core.Resources.Animation

```csharp
public struct Metadata
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [Metadata](./SharpPluginLoader.Core.Resources.Animation.Metadata.md)

## Fields

### **ParamPtr**

```csharp
public MetadataParam* ParamPtr;
```

### **ParamNum**

```csharp
public int ParamNum;
```

### **Type1**

```csharp
public uint Type1;
```

### **Type2**

```csharp
public uint Type2;
```

### **FrameCount**

```csharp
public float FrameCount;
```

### **LoopStart**

```csharp
public float LoopStart;
```

### **LoopValue**

```csharp
public uint LoopValue;
```

### **Hash**

```csharp
public uint Hash;
```

## Properties

### **Params**

```csharp
public Span<MetadataParam> Params { get; }
```

#### Property Value

[Span&lt;MetadataParam&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

## Methods

### **GetParam(Int32)**

```csharp
MetadataParam& GetParam(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[MetadataParam&](./SharpPluginLoader.Core.Resources.Animation.MetadataParam.md)<br>
