# PluginData

Namespace: SharpPluginLoader.Core

```csharp
public class PluginData
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [PluginData](./SharpPluginLoader.Core.PluginData.md)

## Fields

### **OnPreMain**

```csharp
public bool OnPreMain;
```

### **OnWinMain**

```csharp
public bool OnWinMain;
```

### **OnUpdate**

```csharp
public bool OnUpdate;
```

### **OnSave**

```csharp
public bool OnSave;
```

### **OnSelectSaveSlot**

```csharp
public bool OnSelectSaveSlot;
```

### **OnResourceLoad**

```csharp
public bool OnResourceLoad;
```

### **OnChatMessageSent**

```csharp
public bool OnChatMessageSent;
```

### **OnQuestAccept**

```csharp
public bool OnQuestAccept;
```

### **OnQuestCancel**

```csharp
public bool OnQuestCancel;
```

### **OnQuestDepart**

```csharp
public bool OnQuestDepart;
```

### **OnQuestEnter**

```csharp
public bool OnQuestEnter;
```

### **OnQuestLeave**

```csharp
public bool OnQuestLeave;
```

### **OnQuestComplete**

```csharp
public bool OnQuestComplete;
```

### **OnQuestFail**

```csharp
public bool OnQuestFail;
```

### **OnQuestReturn**

```csharp
public bool OnQuestReturn;
```

### **OnQuestAbandon**

```csharp
public bool OnQuestAbandon;
```

### **OnMonsterCreate**

```csharp
public bool OnMonsterCreate;
```

### **OnMonsterInitialized**

```csharp
public bool OnMonsterInitialized;
```

### **OnMonsterAction**

```csharp
public bool OnMonsterAction;
```

### **OnMonsterFlinch**

```csharp
public bool OnMonsterFlinch;
```

### **OnMonsterEnrage**

```csharp
public bool OnMonsterEnrage;
```

### **OnMonsterUnenrage**

```csharp
public bool OnMonsterUnenrage;
```

### **OnMonsterDeath**

```csharp
public bool OnMonsterDeath;
```

### **OnMonsterDestroy**

```csharp
public bool OnMonsterDestroy;
```

### **OnPlayerAction**

```csharp
public bool OnPlayerAction;
```

### **OnWeaponChange**

```csharp
public bool OnWeaponChange;
```

### **OnEntityAction**

```csharp
public bool OnEntityAction;
```

### **OnEntityAnimation**

```csharp
public bool OnEntityAnimation;
```

### **OnEntityAnimationUpdate**

```csharp
public bool OnEntityAnimationUpdate;
```

### **OnSendPacket**

```csharp
public bool OnSendPacket;
```

### **OnReceivePacket**

```csharp
public bool OnReceivePacket;
```

### **OnRender**

```csharp
public bool OnRender;
```

### **OnImGuiRender**

```csharp
public bool OnImGuiRender;
```

### **OnImGuiFreeRender**

```csharp
public bool OnImGuiFreeRender;
```

### **IsDebugPlugin**

```csharp
public bool IsDebugPlugin;
```

### **ImGuiWrappedInTreeNode**

When set to true, any ImGui widgets rendered inside [IPlugin.OnImGuiRender()](./SharpPluginLoader.Core.IPlugin.md#onimguirender) will be wrapped in a TreeNode.

```csharp
public bool ImGuiWrappedInTreeNode;
```

## Constructors

### **PluginData()**

```csharp
public PluginData()
```
