# MtHermiteCurve

Namespace: SharpPluginLoader.Core.MtTypes

```csharp
public struct MtHermiteCurve
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtHermiteCurve](./SharpPluginLoader.Core.MtTypes.MtHermiteCurve.md)

## Fields

### **X**

```csharp
public <X>e__FixedBuffer X;
```

### **Y**

```csharp
public <Y>e__FixedBuffer Y;
```

## Properties

### **PointCount**

```csharp
public static int PointCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **EffectivePointCount**

```csharp
public int EffectivePointCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **Item**

```csharp
public float Item { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

## Constructors

### **MtHermiteCurve(IList&lt;ValueTuple&lt;Single, Single&gt;&gt;)**

Creates a new Hermite curve from a list of points.

```csharp
MtHermiteCurve(IList<ValueTuple<float, float>> points)
```

#### Parameters

`points` [IList&lt;ValueTuple&lt;Single, Single&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1)<br>
A list of at most 8 points, with the first element of each tuple being the X value,
 and the second element being the Y value.
