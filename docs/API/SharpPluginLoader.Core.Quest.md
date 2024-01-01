# Quest

Namespace: SharpPluginLoader.Core

Exposes various functions and properties related to quests.

```csharp
public static class Quest
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Quest](./SharpPluginLoader.Core.Quest.md)

## Properties

### **SingletonInstance**

The sQuest singleton instance.

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **CurrentQuestId**

Gets the current quest ID, or -1 if there is no current quest.

```csharp
public static int CurrentQuestId { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **CurrentQuestName**

Gets the name of the current quest, or an empty string if there is no current quest.

```csharp
public static string CurrentQuestName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **CurrentQuestStarcount**

Gets the current quest star count.

```csharp
public static Int32& CurrentQuestStarcount { get; }
```

#### Property Value

[Int32&](https://docs.microsoft.com/en-us/dotnet/api/System.Int32&)<br>

### **QuestState**

Gets the current quest state.

```csharp
public static UInt32& QuestState { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **CurrentQuestRewardMoney**

Gets the current quests reward money.

```csharp
public static UInt32& CurrentQuestRewardMoney { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **CurrentQuestRewardHrp**

Gets the current quests reward HRP.

```csharp
public static UInt32& CurrentQuestRewardHrp { get; }
```

#### Property Value

[UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

### **Objectives**

Gets the current quests objectives

```csharp
public static Span<QuestTargetData> Objectives { get; }
```

#### Property Value

[Span&lt;QuestTargetData&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

### **QuestEndTimer**

Gets the quests ending timer.

```csharp
public static Timer& QuestEndTimer { get; }
```

#### Property Value

[Timer&](./SharpPluginLoader.Core.Timer.md)<br>

## Methods

### **GetQuestName(Int32)**

Gets the name of the quest with the specified ID.

```csharp
public static string GetQuestName(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The ID of the quest.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The name of the quest with the specified ID, or an empty string if the quest does not exist.
