# MtVector4

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtVector4
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)

## Fields

### **X**

```csharp
public float X;
```

### **Y**

```csharp
public float Y;
```

### **Z**

```csharp
public float Z;
```

### **W**

```csharp
public float W;
```

## Properties

### **Length**

```csharp
public float Length { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **LengthSquared**

```csharp
public float LengthSquared { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Normalized**

```csharp
public MtVector4 Normalized { get; }
```

#### Property Value

[MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

## Constructors

### **MtVector4(Single, Single, Single, Single)**

```csharp
MtVector4(float x, float y, float z, float w)
```

#### Parameters

`x` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`y` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`w` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **MtVector4(MtVector3, Single)**

```csharp
MtVector4(MtVector3 v3, float w)
```

#### Parameters

`v3` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`w` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

## Methods

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **SetLength(Single)**

```csharp
MtVector4 SetLength(float length)
```

#### Parameters

`length` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

### **Limit(Single)**

```csharp
MtVector4 Limit(float limit)
```

#### Parameters

`limit` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

### **Dot(MtVector4, MtVector4)**

```csharp
float Dot(MtVector4 a, MtVector4 b)
```

#### Parameters

`a` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

`b` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Lerp(MtVector4, MtVector4, Single)**

```csharp
MtVector4 Lerp(MtVector4 a, MtVector4 b, float t)
```

#### Parameters

`a` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

`b` [MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

`t` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector4](./SharpPluginLoader.Core.MtTypes.MtVector4.md)<br>

### **ToVector3()**

```csharp
MtVector3 ToVector3()
```

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
