# Hook

Namespace: SharpPluginLoader.Core.Memory

```csharp
public static class Hook
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Hook](./SharpPluginLoader.Core.Memory.Hook.md)

## Methods

### **Create&lt;TFunction&gt;(Int64, TFunction)**

Creates a new native function hook.

```csharp
public static Hook<TFunction> Create<TFunction>(long address, TFunction hook)
```

#### Type Parameters

`TFunction`<br>
The type of the function to hook

#### Parameters

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The address of the function to hook

`hook` TFunction<br>
The hook function

#### Returns

Hook&lt;TFunction&gt;<br>
The hook object
