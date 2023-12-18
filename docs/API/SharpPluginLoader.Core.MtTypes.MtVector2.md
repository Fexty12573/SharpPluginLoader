# MtVector2

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtVector2
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)

## Fields

### **X**

```csharp
public float X;
```

### **Y**

```csharp
public float Y;
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

## Constructors

### **MtVector2(Single, Single)**

```csharp
MtVector2(float x, float y)
```

#### Parameters

`x` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`y` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

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

### **Normalize()**

```csharp
MtVector2 Normalize()
```

#### Returns

[MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

### **SetLength(Single)**

```csharp
MtVector2 SetLength(float length)
```

#### Parameters

`length` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

### **Limit(Single)**

```csharp
MtVector2 Limit(float limit)
```

#### Parameters

`limit` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

### **Dot(MtVector2, MtVector2)**

```csharp
float Dot(MtVector2 a, MtVector2 b)
```

#### Parameters

`a` [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

`b` [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Lerp(MtVector2, MtVector2, Single)**

```csharp
MtVector2 Lerp(MtVector2 a, MtVector2 b, float t)
```

#### Parameters

`a` [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

`b` [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

`t` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>
