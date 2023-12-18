# MtDti

Namespace: SharpPluginLoader.Core

This class represents a Monster Hunter World: Iceborne data type info.

```csharp
public class MtDti : MtObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [MtDti](./SharpPluginLoader.Core.MtDti.md)

## Properties

### **Name**

Gets the name of the class.

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Next**

Gets the next class in the list.

```csharp
public MtDti Next { get; }
```

#### Property Value

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

### **Child**

Gets the first child class of this class.

```csharp
public MtDti Child { get; }
```

#### Property Value

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

### **Children**

Gets all classes that inherit directly from this class.

```csharp
public MtDti[] Children { get; }
```

#### Property Value

[MtDti[]](./SharpPluginLoader.Core.MtDti.md)<br>

### **AllChildren**

Gets all classes that inherit from this class (directly or indirectly).

```csharp
public MtDti[] AllChildren { get; }
```

#### Property Value

[MtDti[]](./SharpPluginLoader.Core.MtDti.md)<br>

### **Parent**

Gets the parent class of this class.

```csharp
public MtDti Parent { get; }
```

#### Property Value

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

### **Link**

Gets the linked class of this class. This property is used by the game to form a hash table of classes.

```csharp
public MtDti Link { get; }
```

#### Property Value

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>

### **Size**

Gets the size in bytes of the class.

```csharp
public uint Size { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **AllocatorIndex**

Gets the index of the allocator used by this class.

```csharp
public uint AllocatorIndex { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Attributes**

Gets the attributes of the class.

```csharp
public uint Attributes { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Id**

Gets the id of the class.

```csharp
public uint Id { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **Instance**

The native pointer.

```csharp
public nint Instance { get; set; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Constructors

### **MtDti(IntPtr)**

Constructs a new instance of [MtDti](./SharpPluginLoader.Core.MtDti.md) with the specified native pointer.

```csharp
public MtDti(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MtDti()**

Constructs a new instance of [MtDti](./SharpPluginLoader.Core.MtDti.md) with nullptr as the native pointer.

```csharp
public MtDti()
```

## Methods

### **InheritsFrom(UInt32)**

Checks if this class inherits from the class with the specified CRC.

```csharp
public bool InheritsFrom(uint id)
```

#### Parameters

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The CRC hash of the class to check

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the class inherits from the specified class

### **InheritsFrom(String)**

Checks if this class inherits from the class with the specified name.

```csharp
public bool InheritsFrom(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The name of the class to check

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the class inherits from the specified class

### **InheritsFrom(MtDti)**

Checks if this class inherits from the specified class.

```csharp
public bool InheritsFrom(MtDti dti)
```

#### Parameters

`dti` [MtDti](./SharpPluginLoader.Core.MtDti.md)<br>
The class to check for

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
True if the class inherits from the specified class

### **CreateInstance&lt;T&gt;()**

Creates and instantiates a new instance of the of the type represented by this class.

```csharp
public T CreateInstance<T>()
```

#### Type Parameters

`T`<br>
The type of the object

#### Returns

T<br>
The created object

### **Instantiate&lt;T&gt;(T)**

Instantiates the specified object with the type represented by this class.

```csharp
public T Instantiate<T>(T obj)
```

#### Type Parameters

`T`<br>
The type of the object

#### Parameters

`obj` T<br>
The object to instantiate

#### Returns

T<br>
The object if the instantiation was successfull or null

### **MakeId(String)**

Computes the dti id of a class from its name.

```csharp
public static uint MakeId(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The name of the class

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The computed dti id of the class

### **Find(UInt32)**

Finds a DTI by its id.

```csharp
public static MtDti Find(uint id)
```

#### Parameters

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The id of the class

#### Returns

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>
The DTI or null if no DTI was found

### **Find(String)**

Finds a DTI by its name.

```csharp
public static MtDti Find(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The fully qualified name of the class

#### Returns

[MtDti](./SharpPluginLoader.Core.MtDti.md)<br>
The DTI or null if no DTI was found
