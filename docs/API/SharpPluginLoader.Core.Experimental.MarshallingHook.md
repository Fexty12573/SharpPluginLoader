# MarshallingHook

Namespace: SharpPluginLoader.Core.Experimental

```csharp
public class MarshallingHook
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [MarshallingHook](./SharpPluginLoader.Core.Experimental.MarshallingHook.md)

## Constructors

### **MarshallingHook()**

```csharp
public MarshallingHook()
```

## Methods

### **Create&lt;TFunction&gt;(TFunction, Int64)**

```csharp
public static MarshallingHook<TFunction> Create<TFunction>(TFunction hook, long functionAddress)
```

#### Type Parameters

`TFunction`<br>

#### Parameters

`hook` TFunction<br>

`functionAddress` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

#### Returns

MarshallingHook&lt;TFunction&gt;<br>
