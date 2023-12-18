# Utility

Namespace: SharpPluginLoader.Core

Provides a set of utility methods.

```csharp
public static class Utility
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Utility](./SharpPluginLoader.Core.Utility.md)

## Methods

### **Crc32(String, Int32)**

Computes the CRC of the specified string. This is the same CRC used by Monster Hunter World.

```csharp
public static uint Crc32(string str, int crc)
```

#### Parameters

`str` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The string to hash

`crc` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The initial CRC value

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The CRC hash of the string

### **MakeDtiId(String)**

```csharp
public static uint MakeDtiId(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **FindDti(UInt32)**

```csharp
internal static nint FindDti(uint id)
```

#### Parameters

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **GetMonsterDti(UInt32)**

```csharp
internal static nint GetMonsterDti(uint monsterId)
```

#### Parameters

`monsterId` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **ResizeArray&lt;T&gt;(MtArray&lt;T&gt;, UInt32)**

```csharp
internal static void ResizeArray<T>(MtArray<T> array, uint newSize)
```

#### Type Parameters

`T`<br>

#### Parameters

`array` MtArray&lt;T&gt;<br>

`newSize` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

### **GetMonsterName(MonsterType)**

```csharp
internal static string GetMonsterName(MonsterType monsterType)
```

#### Parameters

`monsterType` [MonsterType](./SharpPluginLoader.Core.Entities.MonsterType.md)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
