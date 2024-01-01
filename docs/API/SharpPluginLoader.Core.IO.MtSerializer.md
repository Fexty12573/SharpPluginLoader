# MtSerializer

Namespace: SharpPluginLoader.Core.IO

#### Caution

Types with embedded references are not supported in this version of your compiler.

---

```csharp
public struct MtSerializer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [MtSerializer](./SharpPluginLoader.Core.IO.MtSerializer.md)

## Constructors

### **MtSerializer()**

```csharp
MtSerializer()
```

## Methods

### **DeserializeBinary&lt;T&gt;(MtStream, UInt16, T, SerializerMode)**

```csharp
T DeserializeBinary<T>(MtStream stream, ushort version, T dst, SerializerMode mode)
```

#### Type Parameters

`T`<br>

#### Parameters

`stream` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>

`version` [UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>

`dst` T<br>

`mode` [SerializerMode](./SharpPluginLoader.Core.IO.SerializerMode.md)<br>

#### Returns

T<br>

### **DeserializeXml&lt;T&gt;(MtStream, String, T, SerializerMode, SerializerEncoding)**

```csharp
T DeserializeXml<T>(MtStream stream, string rootName, T dst, SerializerMode mode, SerializerEncoding encoding)
```

#### Type Parameters

`T`<br>

#### Parameters

`stream` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>

`rootName` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`dst` T<br>

`mode` [SerializerMode](./SharpPluginLoader.Core.IO.SerializerMode.md)<br>

`encoding` [SerializerEncoding](./SharpPluginLoader.Core.IO.SerializerEncoding.md)<br>

#### Returns

T<br>

### **SerializeBinary(MtStream, MtObject, UInt16, SerializerMode)**

```csharp
bool SerializeBinary(MtStream dst, MtObject src, ushort version, SerializerMode mode)
```

#### Parameters

`dst` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>

`src` [MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

`version` [UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>

`mode` [SerializerMode](./SharpPluginLoader.Core.IO.SerializerMode.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **SerializeXml(MtStream, MtObject, String, SerializerEncoding)**

```csharp
bool SerializeXml(MtStream dst, MtObject src, string rootName, SerializerEncoding encoding)
```

#### Parameters

`dst` [MtStream](./SharpPluginLoader.Core.IO.MtStream.md)<br>

`src` [MtObject](./SharpPluginLoader.Core.MtObject.md)<br>

`rootName` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`encoding` [SerializerEncoding](./SharpPluginLoader.Core.IO.SerializerEncoding.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
