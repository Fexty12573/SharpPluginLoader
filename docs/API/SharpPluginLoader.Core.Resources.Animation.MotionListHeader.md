# MotionListHeader

Namespace: SharpPluginLoader.Core.Resources.Animation

```csharp
public struct MotionListHeader
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MotionListHeader](./SharpPluginLoader.Core.Resources.Animation.MotionListHeader.md)

## Fields

### **Magic**

```csharp
public uint Magic;
```

### **Version**

```csharp
public ushort Version;
```

### **MotionCount**

```csharp
public ushort MotionCount;
```

### **Unknown1**

```csharp
public uint Unknown1;
```

## Methods

### **HasMotion(Int32)**

```csharp
bool HasMotion(int id)
```

#### Parameters

`id` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **GetMotion(Int32)**

```csharp
Motion& GetMotion(int id)
```

#### Parameters

`id` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[Motion&](./SharpPluginLoader.Core.Resources.Animation.Motion.md)<br>
