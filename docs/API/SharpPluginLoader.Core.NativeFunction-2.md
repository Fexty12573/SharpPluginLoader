# NativeFunction&lt;T1, TRet&gt;

Namespace: SharpPluginLoader.Core

Represents a function pointer to a native function with the given return type and arguments.

```csharp
public struct NativeFunction<T1, TRet>
```

#### Type Parameters

`T1`<br>

`TRet`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [NativeFunction&lt;T1, TRet&gt;](./SharpPluginLoader.Core.NativeFunction-2.md)

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

### **NativeFunction(IntPtr)**

```csharp
NativeFunction(nint funcPtr)
```

#### Parameters

`funcPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **NativeFunction(Int64)**

```csharp
NativeFunction(long funcPtr)
```

#### Parameters

`funcPtr` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
