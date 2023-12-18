# MetadataParam

Namespace: SharpPluginLoader.Core.Resources.Animation

```csharp
public struct MetadataParam
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MetadataParam](./SharpPluginLoader.Core.Resources.Animation.MetadataParam.md)

## Fields

### **MemberPtr**

```csharp
public MetadataParamMember* MemberPtr;
```

### **MemberNum**

```csharp
public int MemberNum;
```

### **Hash**

```csharp
public uint Hash;
```

### **UniqueId**

```csharp
public uint UniqueId;
```

## Properties

### **Members**

```csharp
public Span<MetadataParamMember> Members { get; }
```

#### Property Value

[Span&lt;MetadataParamMember&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

### **Dti**

```csharp
public MtDti Dti { get; }
```

#### Property Value

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

## Methods

### **GetMember(Int32)**

```csharp
MetadataParamMember& GetMember(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[MetadataParamMember&](./SharpPluginLoader.Core.Resources.Animation.MetadataParamMember.md)<br>
