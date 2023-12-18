# SpanExtensions

Namespace: SharpPluginLoader.Core

```csharp
public static class SpanExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [SpanExtensions](./SharpPluginLoader.Core.SpanExtensions.md)

## Methods

### **AsNativeArray&lt;T&gt;(Span&lt;T&gt;)**

```csharp
public static NativeArray<T> AsNativeArray<T>(Span<T> span)
```

#### Type Parameters

`T`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

#### Returns

NativeArray&lt;T&gt;<br>

### **FindIndex&lt;T&gt;(Span&lt;T&gt;, SpanMatch&lt;T&gt;)**

```csharp
public static int FindIndex<T>(Span<T> span, SpanMatch<T> match)
```

#### Type Parameters

`T`<br>

#### Parameters

`span` Span&lt;T&gt;<br>

`match` SpanMatch&lt;T&gt;<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
