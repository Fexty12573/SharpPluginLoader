# AnimationId

Namespace: SharpPluginLoader.Core.Components

Represents an animation id. This is a combination of the LMT and the actual animation id.

```csharp
public struct AnimationId
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [AnimationId](./SharpPluginLoader.Core.Components.AnimationId.md)

## Properties

### **FullId**

The full id of the animation.

```csharp
public uint FullId { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Lmt**

The LMT the animation targets

```csharp
public uint Lmt { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Id**

The actual id of the animation.

```csharp
public uint Id { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

## Constructors

### **AnimationId(UInt32)**

```csharp
AnimationId(uint fullId)
```

#### Parameters

`fullId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **AnimationId(UInt32, UInt32)**

```csharp
AnimationId(uint lmt, uint id)
```

#### Parameters

`lmt` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

## Methods

### **Equals(AnimationId)**

```csharp
bool Equals(AnimationId other)
```

#### Parameters

`other` [AnimationId](./SharpPluginLoader.Core.Components.AnimationId.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

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
