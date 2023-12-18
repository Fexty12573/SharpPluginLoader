# Quest

Namespace: SharpPluginLoader.Core

```csharp
public static class Quest
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Quest](./SharpPluginLoader.Core.Quest.md)

## Properties

### **SingletonInstance**

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CurrentQuestId**

```csharp
public static int CurrentQuestId { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **CurrentQuestName**

```csharp
public static string CurrentQuestName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

## Methods

### **GetQuestName(Int32)**

```csharp
public static string GetQuestName(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Initialize()**

```csharp
internal static void Initialize()
```
