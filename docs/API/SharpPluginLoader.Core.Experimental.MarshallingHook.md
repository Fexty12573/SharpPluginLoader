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

### **Create&lt;TFunction&gt;(Int64, TFunction)**

```csharp
public static MarshallingHook<TFunction> Create<TFunction>(long functionAddress, TFunction hook)
```

#### Type Parameters

`TFunction`<br>

#### Parameters

`functionAddress` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`hook` TFunction<br>

#### Returns

MarshallingHook&lt;TFunction&gt;<br>
