# Hook

Namespace: SharpPluginLoader.Core.Memory

```csharp
public static class Hook
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Hook](./SharpPluginLoader.Core.Memory.Hook.md)

## Methods

### **Create&lt;TFunction&gt;(TFunction, Int64)**

Creates a new native function hook.

```csharp
public static Hook<TFunction> Create<TFunction>(TFunction hook, long address)
```

#### Type Parameters

`TFunction`<br>
The type of the function to hook

#### Parameters

`hook` TFunction<br>
The hook function

`address` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>
The address of the function to hook

#### Returns

Hook&lt;TFunction&gt;<br>
The hook object
