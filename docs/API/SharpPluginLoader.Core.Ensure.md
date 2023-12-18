# Ensure

Namespace: SharpPluginLoader.Core

Provides methods to ensure that arguments are not null or default

```csharp
public static class Ensure
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Ensure](./SharpPluginLoader.Core.Ensure.md)

## Methods

### **NotNull&lt;T&gt;(T, String)**

```csharp
public static void NotNull<T>(T value, string name)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **NotNull&lt;T&gt;(T*, String)**

```csharp
public static void NotNull<T>(T* value, string name)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` T*<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **NotNullOrEmpty(String, String)**

```csharp
public static void NotNullOrEmpty(string value, string name)
```

#### Parameters

`value` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **NotNullOrDefault&lt;T&gt;(Nullable&lt;T&gt;, String)**

```csharp
public static void NotNullOrDefault<T>(Nullable<T> value, string name)
```

#### Type Parameters

`T`<br>

#### Parameters

`value` Nullable&lt;T&gt;<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **IsTrue(Boolean, String)**

```csharp
public static void IsTrue(bool value, string name)
```

#### Parameters

`value` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
