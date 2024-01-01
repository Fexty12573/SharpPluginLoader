# PatternScanner

Namespace: SharpPluginLoader.Core.Memory

```csharp
public static class PatternScanner
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [PatternScanner](./SharpPluginLoader.Core.Memory.PatternScanner.md)

## Methods

### **Scan(Pattern)**

Scans the entire process for the specified pattern.

```csharp
public static List<nint> Scan(Pattern pattern)
```

#### Parameters

`pattern` [Pattern](./SharpPluginLoader.Core.Memory.Pattern.md)<br>
The pattern to scan for.

#### Returns

[List&lt;IntPtr&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1)<br>
A list of addresses where the pattern was found.

**Remarks:**

It is strongly recommended to cache the results of this method.
