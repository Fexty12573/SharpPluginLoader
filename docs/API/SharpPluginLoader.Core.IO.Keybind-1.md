# Keybind&lt;T&gt;

Namespace: SharpPluginLoader.Core.IO

A hotkey, for either a keyboard or a controller.

```csharp
public struct Keybind<T>
```

#### Type Parameters

`T`<br>
The enum type

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [Keybind&lt;T&gt;](./SharpPluginLoader.Core.IO.Keybind-1.md)

## Fields

### **Key**

```csharp
public T Key;
```

### **Modifiers**

```csharp
public T[] Modifiers;
```

## Constructors

### **Keybind(T, T[])**

A hotkey, for either a keyboard or a controller.

```csharp
Keybind(T key, T[] modifiers)
```

#### Parameters

`key` T<br>
The key

`modifiers` T[]<br>
The modifiers
