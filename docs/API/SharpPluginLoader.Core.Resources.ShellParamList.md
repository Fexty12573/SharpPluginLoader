# ShellParamList

Namespace: SharpPluginLoader.Core.Resources

Represents an instance of a rShellParamList resource (a .shll file)

```csharp
public class ShellParamList : Resource
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [NativeWrapper](./SharpPluginLoader.Core.NativeWrapper.md) → [MtObject](./SharpPluginLoader.Core.MtObject.md) → [Resource](./SharpPluginLoader.Core.Resources.Resource.md) → [ShellParamList](./SharpPluginLoader.Core.Resources.ShellParamList.md)

## Properties

### **ShellCount**

Gets the number of shells in this list.

```csharp
public int ShellCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **Shells**

Gets all shells in this list.

```csharp
public ShellParam[] Shells { get; }
```

#### Property Value

[ShellParam[]](./SharpPluginLoader.Core.Resources.ShellParam.md)<br>

### **FilePath**

Gets the file path of this resource without the extension.

```csharp
public string FilePath { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **FileExtension**

Gets the file extension of this resource.

```csharp
public string FileExtension { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **RefCount**

Gets the reference count of this resource. If the reference count reaches 0, the resource is unloaded.

```csharp
public uint RefCount { get; }
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

### **ShellParamList(IntPtr)**

```csharp
public ShellParamList(nint instance)
```

#### Parameters

`instance` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **ShellParamList()**

```csharp
public ShellParamList()
```

## Methods

### **GetShell(UInt32)**

Gets a shell by its index.

```csharp
public ShellParam GetShell(uint index)
```

#### Parameters

`index` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

#### Returns

[ShellParam](./SharpPluginLoader.Core.Resources.ShellParam.md)<br>
The shell at the given index, or null
