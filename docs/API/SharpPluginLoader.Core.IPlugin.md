# IPlugin

Namespace: SharpPluginLoader.Core

```csharp
public interface IPlugin
```

## Properties

### **Name**

The name of the plugin.

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Author**

The author of the plugin.

```csharp
public string Author { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

## Methods

### **OnLoad()**

Gets called when the plugin is loaded. This is where you should initialize your plugin.
 The plugin must return a [PluginData](./SharpPluginLoader.Core.PluginData.md) struct, which tells the framework which events to call.

```csharp
PluginData OnLoad()
```

#### Returns

[PluginData](./SharpPluginLoader.Core.PluginData.md)<br>
The filled out PluginData

### **OnUpdate(Single)**

Gets called every frame.

```csharp
void OnUpdate(float deltaTime)
```

#### Parameters

`deltaTime` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The time elapsed since the last time this function was called, in seconds

### **OnSave()**

Gets called when the game is saved.

```csharp
void OnSave()
```

### **OnSelectSaveSlot(Int32)**

Gets called when the player selects a save slot.

```csharp
void OnSelectSaveSlot(int slot)
```

#### Parameters

`slot` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The save slot that was selected

### **OnResourceLoad(Resource, MtDti, String, UInt32)**

Gets called when a resource is requested/loaded.

```csharp
void OnResourceLoad(Resource resource, MtDti dti, string path, uint flags)
```

#### Parameters

`resource` [Resource](./SharpPluginLoader.Core.Resources.Resource.md)<br>
The loaded resource, or null if the request failed

`dti` [MtDti](./SharpPluginLoader.Core.MtDti.md)<br>
The DTI of the resource

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The file path of the resource, without its extension

`flags` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The flags passed to the request

### **OnChatMessageSent(String)**

Gets called when a chat message is sent (on the local side).

```csharp
void OnChatMessageSent(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The contents of the message

### **OnQuestAccept(Int32)**

Gets called when a quest is accepted on the quest board.

```csharp
void OnQuestAccept(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestCancel(Int32)**

Gets called when a quest is cancelled on the quest board.

```csharp
void OnQuestCancel(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestDepart(Int32)**

Gets called when the player departs on a quest.

```csharp
void OnQuestDepart(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestEnter(Int32)**

Gets called when the player arrives in the quest area.

```csharp
void OnQuestEnter(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestLeave(Int32)**

Gets called when the player leaves the quest area.

```csharp
void OnQuestLeave(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestComplete(Int32)**

Gets called when a quest is completed.

```csharp
void OnQuestComplete(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestFail(Int32)**

Gets called when a quest is failed.

```csharp
void OnQuestFail(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestReturn(Int32)**

Gets called when the player selects "Return from Quest" in the menu.

```csharp
void OnQuestReturn(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnQuestAbandon(Int32)**

Gets called when the player selects "Abandon Quest" in the menu.

```csharp
void OnQuestAbandon(int questId)
```

#### Parameters

`questId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The id of the quest

### **OnMonsterCreate(Monster)**

Gets called when a monster is created.

```csharp
void OnMonsterCreate(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster being created

**Remarks:**

This function is called immediately after the monsters constructor is run,
 most of its data is not yet initialized by this point.

### **OnMonsterInitialized(Monster)**

Gets called after a monster is initialized.

```csharp
void OnMonsterInitialized(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster that was initialized

**Remarks:**

Most data in the monster is ready to be used by this point.

### **OnMonsterAction(Monster, Int32&)**

Gets called when a monster does an action.

```csharp
void OnMonsterAction(Monster monster, Int32& actionId)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster doing the action

`actionId` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/System.Int32&)<br>
The id of the action to be executed

**Remarks:**

The actionId parameter can be modified to change the executed action

### **OnMonsterFlinch(Monster, Int32&)**

Gets called when a monster gets flinched.

```csharp
void OnMonsterFlinch(Monster monster, Int32& actionId)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster getting flinched

`actionId` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/System.Int32&)<br>
The flinch action it will perform

### **OnMonsterEnrage(Monster)**

Gets called when a monster gets enraged.

```csharp
void OnMonsterEnrage(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster getting enraged

### **OnMonsterUnenrage(Monster)**

Gets called when a monster leaves its enraged state.

```csharp
void OnMonsterUnenrage(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster leaving its enraged state

### **OnMonsterDeath(Monster)**

Gets called when a monster dies.

```csharp
void OnMonsterDeath(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster that died

### **OnMonsterDestroy(Monster)**

Gets called when a monster is destroyed (its destructor is called).

```csharp
void OnMonsterDestroy(Monster monster)
```

#### Parameters

`monster` [Monster](./SharpPluginLoader.Core.Entities.Monster.md)<br>
The monster that is about to be destroyed

### **OnPlayerAction(Player, ActionInfo&)**

Gets called when the player does an action.

```csharp
void OnPlayerAction(Player player, ActionInfo& action)
```

#### Parameters

`player` [Player](./SharpPluginLoader.Core.Entities.Player.md)<br>
The player doing the action

`action` [ActionInfo&](./SharpPluginLoader.Core.ActionInfo.md)<br>
The action to be executed

**Remarks:**

The action parameter can be modified to change the executed action

### **OnWeaponChange(Player, WeaponType, Int32)**

Gets called when the player changes their weapon.

```csharp
void OnWeaponChange(Player player, WeaponType weaponType, int weaponId)
```

#### Parameters

`player` [Player](./SharpPluginLoader.Core.Entities.Player.md)<br>
The player changing weapons

`weaponType` [WeaponType](./SharpPluginLoader.Core.Entities.WeaponType.md)<br>
The new weapon type

`weaponId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The new weapon id

**Remarks:**

This function is called asynchronously.

### **OnEntityAction(Entity, ActionInfo&)**

Gets called when any entity does an action.

```csharp
void OnEntityAction(Entity entity, ActionInfo& action)
```

#### Parameters

`entity` [Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>
The entity doing the action

`action` [ActionInfo&](./SharpPluginLoader.Core.ActionInfo.md)<br>
The action to be executed

**Remarks:**

The action parameter can be modified to change the executed action

### **OnEntityAnimation(Entity, AnimationId&, Single&, Single&)**

Gets called when any entity does an animation.

```csharp
void OnEntityAnimation(Entity entity, AnimationId& animationId, Single& startFrame, Single& interFrame)
```

#### Parameters

`entity` [Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>
The entity doing the animation

`animationId` [AnimationId&](./SharpPluginLoader.Core.Components.AnimationId.md)<br>
The id of the animation to be executed

`startFrame` [Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>
The starting frame of the animation

`interFrame` [Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>
The number of frames to use for interpolation between animations

**Remarks:**

Both the animationId and the startFrame parameters can be modified to change the executed animation.

### **OnEntityAnimationUpdate(Entity, AnimationId, Single)**

Gets called when an entity's animation component is updated.

```csharp
void OnEntityAnimationUpdate(Entity entity, AnimationId currentAnimation, float deltaTime)
```

#### Parameters

`entity` [Entity](./SharpPluginLoader.Core.Entities.Entity.md)<br>
The entity whos animation component is updated

`currentAnimation` [AnimationId](./SharpPluginLoader.Core.Components.AnimationId.md)<br>
The current active animation

`deltaTime` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The time since the last time this entity's animation component was updated

### **OnSendPacket(Packet, Boolean, SessionIndex)**

Gets called when a packet is sent.

```csharp
void OnSendPacket(Packet packet, bool isBroadcast, SessionIndex session)
```

#### Parameters

`packet` [Packet](./SharpPluginLoader.Core.Networking.Packet.md)<br>
The packet being sent

`isBroadcast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether the packet is broadcasted to all players in the session or not

`session` [SessionIndex](./SharpPluginLoader.Core.Networking.SessionIndex.md)<br>
The session the packet is sent to

### **OnReceivePacket(UInt32, PacketType, SessionIndex, NetBuffer)**

Gets called when a packet is received.

```csharp
void OnReceivePacket(uint id, PacketType type, SessionIndex sourceSession, NetBuffer data)
```

#### Parameters

`id` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>
The id of the packet

`type` [PacketType](./SharpPluginLoader.Core.Networking.PacketType.md)<br>
The type of the packet

`sourceSession` [SessionIndex](./SharpPluginLoader.Core.Networking.SessionIndex.md)<br>
The session the packet was sent from

`data` [NetBuffer](./SharpPluginLoader.Core.Networking.NetBuffer.md)<br>
The data of the packet

### **OnRender()**

The user can use this function to render arbitrary things on the screen (after the game has rendered).

```csharp
void OnRender()
```

### **OnImGuiRender()**

The user can use this function to render ImGui widgets on the screen.

```csharp
void OnImGuiRender()
```

### **Dispose()**

```csharp
void Dispose()
```
