# AttackParam

Namespace: SharpPluginLoader.Core.Resources.Collision

```csharp
public class AttackParam : SharpPluginLoader.Core.MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [AttackParam](./SharpPluginLoader.Core.Resources.Collision.AttackParam.md)

## Properties

### **Index**

```csharp
public UInt32& Index { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **TargetHitGroup**

```csharp
public UInt32& TargetHitGroup { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **ImpactType**

```csharp
public ImpactType& ImpactType { get; }
```

#### Property Value

[ImpactType&](./SharpPluginLoader.Core.Resources.Collision.ImpactType.md)<br>

### **Attack**

```csharp
public Single& Attack { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **FixedAttack**

```csharp
public Single& FixedAttack { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **PartBreakRate**

```csharp
public Single& PartBreakRate { get; }
```

#### Property Value

[Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **AttackParam(IntPtr)**

```csharp
public AttackParam(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **AttackParam()**

```csharp
public AttackParam()
```
