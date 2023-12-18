# Hook&lt;TFunction&gt;

Namespace: SharpPluginLoader.Core.Memory

Represents a native function hook.

```csharp
public class Hook<TFunction> : System.IDisposable
```

#### Type Parameters

`TFunction`<br>
The type of the hooked function

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Hook&lt;TFunction&gt;](./SharpPluginLoader.Core.Memory.Hook-1.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable)

## Properties

### **Original**

Gets the original function.

```csharp
public TFunction Original { get; }
```

#### Property Value

TFunction<br>

### **IsEnabled**

Gets a value indicating whether the hook is enabled.

```csharp
public bool IsEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

## Methods

### **Enable()**

Enables the hook.

```csharp
public void Enable()
```

### **Disable()**

Disables the hook.

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
