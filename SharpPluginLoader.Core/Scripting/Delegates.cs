using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Networking;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.Scripting;

/// <inheritdoc cref="IPlugin.OnUpdate"/>
public delegate void OnUpdateCallback(float deltaTime);

/// <inheritdoc cref="IPlugin.OnSave"/>
public delegate void OnSaveCallback();

/// <inheritdoc cref="IPlugin.OnSelectSaveSlot"/>
public delegate void OnSelectSaveSlotCallback(int slot);

/// <inheritdoc cref="IPlugin.OnResourceLoad"/>
public delegate void OnResourceLoadCallback(Resource? resource, MtDti dti, string path, LoadFlags flags);

/// <inheritdoc cref="IPlugin.OnChatMessageSent"/>
public delegate void OnChatMessageSentCallback(string message);

/// <inheritdoc cref="IPlugin.OnQuestAccept"/>
public delegate void OnQuestAcceptCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestCancel"/>
public delegate void OnQuestCancelCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestDepart"/>
public delegate void OnQuestDepartCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestEnter"/>
public delegate void OnQuestEnterCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestLeave"/>
public delegate void OnQuestLeaveCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestComplete"/>
public delegate void OnQuestCompleteCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestFail"/>
public delegate void OnQuestFailCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestReturn"/>
public delegate void OnQuestReturnCallback(int questId);

/// <inheritdoc cref="IPlugin.OnQuestAbandon"/>
public delegate void OnQuestAbandonCallback(int questId);

/// <inheritdoc cref="IPlugin.OnMonsterCreate"/>
public delegate void OnMonsterCreateCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnMonsterInitialized"/>
public delegate void OnMonsterInitializedCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnMonsterAction"/>
public delegate void OnMonsterActionCallback(Monster monster, ref int actionId);

/// <inheritdoc cref="IPlugin.OnMonsterFlinch"/>
public delegate bool OnMonsterFlinchCallback(Monster monster, ref int actionId);

/// <inheritdoc cref="IPlugin.OnMonsterEnrage"/>
public delegate void OnMonsterEnrageCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnMonsterUnenrage"/>
public delegate void OnMonsterUnenrageCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnMonsterDeath"/>
public delegate void OnMonsterDeathCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnMonsterDestroy"/>
public delegate void OnMonsterDestroyCallback(Monster monster);

/// <inheritdoc cref="IPlugin.OnPlayerAction"/>
public delegate void OnPlayerActionCallback(Player player, ref ActionInfo action);

/// <inheritdoc cref="IPlugin.OnWeaponChange"/>
public delegate void OnWeaponChangeCallback(Player player, WeaponType weaponType, int weaponId);

/// <inheritdoc cref="IPlugin.OnEntityAction"/>
public delegate void OnEntityActionCallback(Entity entity, ref ActionInfo action);

/// <inheritdoc cref="IPlugin.OnEntityAnimation"/>
public delegate void OnEntityAnimationCallback(Entity entity, ref AnimationId animation, ref float startFrame, ref float interFrame);

/// <inheritdoc cref="IPlugin.OnEntityAnimationUpdate"/>
public delegate void OnEntityAnimationUpdateCallback(Entity entity, AnimationId currentAnimation, float deltaTime);

/// <inheritdoc cref="IPlugin.OnSendPacket"/>
public delegate void OnSendPacketCallback(Packet packet, bool isBroadcast, SessionIndex session);

/// <inheritdoc cref="IPlugin.OnReceivePacket"/>
public delegate void OnReceivePacketCallback(uint id, PacketType type, SessionIndex sourceSession, NetBuffer data);

/// <inheritdoc cref="IPlugin.OnLobbySearch"/>
public delegate void OnLobbySearchCallback(ref int maxResults);

/// <inheritdoc cref="IPlugin.OnRender"/>
public delegate void OnRenderCallback();

/// <inheritdoc cref="IPlugin.OnImGuiRender"/>
public delegate void OnImGuiRenderCallback();

/// <inheritdoc cref="IPlugin.OnImGuiFreeRender"/>
public delegate void OnImGuiFreeRenderCallback();
