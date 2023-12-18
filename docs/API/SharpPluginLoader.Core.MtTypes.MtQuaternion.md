# MtQuaternion

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtQuaternion
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)

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
public MtQuaternion Normalized { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **NormalizedSafe**

```csharp
public MtQuaternion NormalizedSafe { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **Angle**

```csharp
public float Angle { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Axis**

```csharp
public MtVector3 Axis { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Yaw**

```csharp
public float Yaw { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Pitch**

```csharp
public float Pitch { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **Roll**

```csharp
public float Roll { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

### **EulerAngle**

```csharp
public MtVector3 EulerAngle { get; }
```

#### Property Value

[MtVector3](./SharpPluginLoader.Core.MtTypes.MtVector3.md)<br>

### **Conjugate**

```csharp
public MtQuaternion Conjugate { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **Inverse**

```csharp
public MtQuaternion Inverse { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **Zero**

```csharp
public static MtQuaternion Zero { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **One**

```csharp
public static MtQuaternion One { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **Identity**

```csharp
public static MtQuaternion Identity { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **UnitX**

```csharp
public static MtQuaternion UnitX { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **UnitY**

```csharp
public static MtQuaternion UnitY { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **UnitZ**

```csharp
public static MtQuaternion UnitZ { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

### **UnitW**

```csharp
public static MtQuaternion UnitW { get; }
```

#### Property Value

[MtQuaternion](./SharpPluginLoader.Core.MtTypes.MtQuaternion.md)<br>

## Constructors

### **MtQuaternion(Single, Single, Single, Single)**

```csharp
MtQuaternion(float x, float y, float z, float w)
```

#### Parameters

`x` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`y` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

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

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
