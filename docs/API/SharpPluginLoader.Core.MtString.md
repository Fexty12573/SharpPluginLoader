# MtString

Namespace: SharpPluginLoader.Core

Represents a string in memory.

```csharp
public struct MtString
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtString](./SharpPluginLoader.Core.MtString.md)

## Fields

### **RefCount**

```csharp
public int RefCount;
```

### **Length**

```csharp
public int Length;
```

### **Data**

```csharp
public <Data>e__FixedBuffer Data;
```

## Methods

### **GetString(Encoding)**

Gets the string from the data using the specified encoding.

```csharp
string GetString(Encoding encoding)
```

#### Parameters

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Encoding)<br>
The encoding to interpret the string as

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string, interpreted with the given encoding

### **GetString()**

Gets the string from the data using UTF-8 encoding.

```csharp
string GetString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string, interpreted with UTF-8 encoding
