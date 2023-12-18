# NativeAction&lt;T1, T2, T3, T4, T5, T6, T7&gt;

Namespace: SharpPluginLoader.Core

Represents a function pointer to a native function with the given return type and arguments.

```csharp
public struct NativeAction<T1, T2, T3, T4, T5, T6, T7>
```

#### Type Parameters

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

`T6`<br>

`T7`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [NativeAction&lt;T1, T2, T3, T4, T5, T6, T7&gt;](./SharpPluginLoader.Core.NativeAction-7.md)

## Properties

### **NativePointer**

Gets the native function pointer.

```csharp
public nint NativePointer { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Invoke**

Invokes the function pointer.

```csharp
public  Invoke { get; }
```

#### Property Value

<br>

### **InvokeUnsafe**

Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.

```csharp
public  InvokeUnsafe { get; }
```

#### Property Value

<br>

**Remarks:**

See SuppressGCTransitionAttribute

## Constructors

### **NativeAction(IntPtr)**

```csharp
NativeAction(nint funcPtr)
```

#### Parameters

`funcPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **NativeAction(Int64)**

```csharp
NativeAction(long funcPtr)
```

#### Parameters

`funcPtr` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
