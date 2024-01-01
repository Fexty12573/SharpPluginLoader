# Timer

Namespace: SharpPluginLoader.Core

```csharp
public struct Timer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [Timer](./SharpPluginLoader.Core.Timer.md)

## Fields

### **VTable**

```csharp
public nint VTable;
```

### **Time**

```csharp
public float Time;
```

### **MaxTime**

```csharp
public float MaxTime;
```

### **Active**

```csharp
public bool Active;
```

### **LockAtMax**

```csharp
public bool LockAtMax;
```

## Methods

### **AddTime(Single)**

```csharp
bool AddTime(float t)
```

#### Parameters

`t` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Reset()**

```csharp
void Reset()
```

### **Ended()**

```csharp
bool Ended()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **SetToEnd()**

```csharp
void SetToEnd()
```
