# MtColor

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtColor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)

## Fields

### **Rgba**

```csharp
public uint Rgba;
```

### **R**

```csharp
public byte R;
```

### **G**

```csharp
public byte G;
```

### **B**

```csharp
public byte B;
```

### **A**

```csharp
public byte A;
```

## Constructors

### **MtColor(Byte, Byte, Byte, Byte)**

```csharp
MtColor(byte r, byte g, byte b, byte a)
```

#### Parameters

`r` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

`g` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

`b` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

`a` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

## Methods

### **ToVector4()**

```csharp
Vector4 ToVector4()
```

#### Returns

[Vector4](https://docs.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4)<br>

### **FromVector4(Vector4)**

```csharp
MtColor FromVector4(Vector4 color)
```

#### Parameters

`color` [Vector4](https://docs.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4)<br>

#### Returns

[MtColor](./SharpPluginLoader.Core.MtTypes.MtColor.md)<br>
