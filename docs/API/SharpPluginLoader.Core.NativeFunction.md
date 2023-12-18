# NativeFunction

Namespace: SharpPluginLoader.Core

Represents a function pointer to a native function with no return type and no arguments. Equivalent to [NativeAction](./SharpPluginLoader.Core.NativeAction.md).

```csharp
public struct NativeFunction
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [NativeFunction](./SharpPluginLoader.Core.NativeFunction.md)

## Properties

### **NativePointer**

Gets the native function pointer.

```csharp
public nint NativePointer { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **NativeFunction(IntPtr)**

```csharp
NativeFunction(nint funcPtr)
```

#### Parameters

`funcPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Methods

### **Invoke()**

Invokes the function pointer.

```csharp
void Invoke()
```

### **InvokeUnsafe()**

Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.

```csharp
void InvokeUnsafe()
```

**Remarks:**

See
