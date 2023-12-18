# MtProperty

Namespace: SharpPluginLoader.Core

Represents a property of a [MtObject](./SharpPluginLoader.Core.MtObject.md).

```csharp
public class MtProperty : NativeWrapper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtProperty](./SharpPluginLoader.Core.MtProperty.md)

## Properties

### **Name**

The name of the property.

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Comment**

An optional comment for the property.

```csharp
public string Comment { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **HashName**

Returns the comment if it exists, otherwise returns the name.

```csharp
public string HashName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

**Remarks:**

Whatever this property returns is used for hash lookups.

### **Hash**

The hash of the property's name.

```csharp
public uint Hash { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Type**

The type of the property.

```csharp
public PropType Type { get; }
```

#### Property Value

[PropType](./SharpPluginLoader.Core.PropType.md)<br>

### **Attr**

The attributes of the property.

```csharp
public uint Attr { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Owner**

The owning object of the property.

```csharp
public MtObject Owner { get; }
```

#### Property Value

[MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

### **Get**

The getter method of the property. (If it has one)

```csharp
public nint Get { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **GetCount**

The GetCount method of the property. (If it is a dynamic array)

```csharp
public nint GetCount { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Set**

The setter method of the property. (If it has one)

```csharp
public nint Set { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Realloc**

The realloc method of the property. (If it is a dynamic array)

```csharp
public nint Realloc { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **Index**

```csharp
public uint Index { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Previous**

The previous property in the object's property list.

```csharp
public MtProperty Previous { get; }
```

#### Property Value

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **Next**

The next property in the object's property list.

```csharp
public MtProperty Next { get; }
```

#### Property Value

[MtProperty](./SharpPluginLoader.Core.MtProperty.md)<br>

### **IsArray**

```csharp
public bool IsArray { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsProperty**

```csharp
public bool IsProperty { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtProperty(IntPtr)**

```csharp
public MtProperty(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtProperty()**

```csharp
public MtProperty()
```
