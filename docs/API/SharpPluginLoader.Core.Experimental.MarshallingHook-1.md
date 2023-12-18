# MarshallingHook&lt;TFunction&gt;

Namespace: SharpPluginLoader.Core.Experimental

```csharp
public class MarshallingHook<TFunction> : System.IDisposable
```

#### Type Parameters

`TFunction`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [MarshallingHook&lt;TFunction&gt;](./SharpPluginLoader.Core.Experimental.MarshallingHook-1.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

## Properties

### **IsEnabled**

```csharp
public bool IsEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Original**

```csharp
public TFunction Original { get; }
```

#### Property Value

TFunction<br>

## Methods

### **Enable()**

```csharp
public void Enable()
```

### **Disable()**

```csharp
public void Disable()
```

### **Dispose()**

```csharp
public void Dispose()
```

### **Finalize()**

```csharp
protected void Finalize()
```
