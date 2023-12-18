# MtVector3

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtVector3
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)

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

## Properties

### **Zero**

```csharp
public static MtVector3 Zero { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **One**

```csharp
public static MtVector3 One { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Forward**

```csharp
public static MtVector3 Forward { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Up**

```csharp
public static MtVector3 Up { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **UnitX**

```csharp
public static MtVector3 UnitX { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **UnitY**

```csharp
public static MtVector3 UnitY { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **UnitZ**

```csharp
public static MtVector3 UnitZ { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

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
public MtVector3 Normalized { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

## Constructors

### **MtVector3(Single, Single, Single)**

```csharp
MtVector3(float x, float y, float z)
```

#### Parameters

`x` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`y` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

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
MtVector3 SetLength(float length)
```

#### Parameters

`length` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Limit(Single)**

```csharp
MtVector3 Limit(float limit)
```

#### Parameters

`limit` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Dot(MtVector3, MtVector3)**

```csharp
float Dot(MtVector3 a, MtVector3 b)
```

#### Parameters

`a` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`b` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Cross(MtVector3, MtVector3)**

```csharp
MtVector3 Cross(MtVector3 a, MtVector3 b)
```

#### Parameters

`a` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`b` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Lerp(MtVector3, MtVector3, Single)**

```csharp
MtVector3 Lerp(MtVector3 a, MtVector3 b, float t)
```

#### Parameters

`a` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`b` [MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

`t` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>
