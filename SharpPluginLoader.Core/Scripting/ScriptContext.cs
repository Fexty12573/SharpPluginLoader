
using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Networking;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.Scripting;

/// <summary>
/// Allows scripts to quickly subscribe to events without having to implement the <see cref="IPlugin"/> interface.
/// </summary>
public class ScriptContext
{
    /// <inheritdoc cref="OnUpdateCallback"/>
    public static event OnUpdateCallback? OnUpdate;

    /// <inheritdoc cref="OnSaveCallback"/>
    public static event OnSaveCallback? OnSave;

    /// <inheritdoc cref="OnSelectSaveSlotCallback"/>
    public static event OnSelectSaveSlotCallback? OnSelectSaveSlot;

    /// <inheritdoc cref="OnResourceLoadCallback"/>
    public static event OnResourceLoadCallback? OnResourceLoad;

    /// <inheritdoc cref="OnChatMessageSentCallback"/>
    public static event OnChatMessageSentCallback? OnChatMessageSent;

    /// <inheritdoc cref="OnQuestAcceptCallback"/>
    public static event OnQuestAcceptCallback? OnQuestAccept;

    /// <inheritdoc cref="OnQuestCancelCallback"/>
    public static event OnQuestCancelCallback? OnQuestCancel;

    /// <inheritdoc cref="OnQuestDepartCallback"/>
    public static event OnQuestDepartCallback? OnQuestDepart;

    /// <inheritdoc cref="OnQuestEnterCallback"/>
    public static event OnQuestEnterCallback? OnQuestEnter;

    /// <inheritdoc cref="OnQuestLeaveCallback"/>
    public static event OnQuestLeaveCallback? OnQuestLeave;

    /// <inheritdoc cref="OnQuestCompleteCallback"/>
    public static event OnQuestCompleteCallback? OnQuestComplete;

    /// <inheritdoc cref="OnQuestFailCallback"/>
    public static event OnQuestFailCallback? OnQuestFail;

    /// <inheritdoc cref="OnQuestReturnCallback"/>
    public static event OnQuestReturnCallback? OnQuestReturn;

    /// <inheritdoc cref="OnQuestAbandonCallback"/>
    public static event OnQuestAbandonCallback? OnQuestAbandon;

    /// <inheritdoc cref="OnMonsterCreateCallback"/>
    public static event OnMonsterCreateCallback? OnMonsterCreate;

    /// <inheritdoc cref="OnMonsterInitializedCallback"/>
    public static event OnMonsterInitializedCallback? OnMonsterInitialized;

    /// <inheritdoc cref="OnMonsterActionCallback"/>
    public static event OnMonsterActionCallback? OnMonsterAction;

    /// <inheritdoc cref="OnMonsterFlinchCallback"/>
    public static event OnMonsterFlinchCallback? OnMonsterFlinch;

    /// <inheritdoc cref="OnMonsterEnrageCallback"/>
    public static event OnMonsterEnrageCallback? OnMonsterEnrage;

    /// <inheritdoc cref="OnMonsterUnenrageCallback"/>
    public static event OnMonsterUnenrageCallback? OnMonsterUnenrage;

    /// <inheritdoc cref="OnMonsterDeathCallback"/>
    public static event OnMonsterDeathCallback? OnMonsterDeath;

    /// <inheritdoc cref="OnMonsterDestroyCallback"/>
    public static event OnMonsterDestroyCallback? OnMonsterDestroy;

    /// <inheritdoc cref="OnPlayerActionCallback"/>
    public static event OnPlayerActionCallback? OnPlayerAction;

    /// <inheritdoc cref="OnWeaponChangeCallback"/>
    public static event OnWeaponChangeCallback? OnWeaponChange;

    /// <inheritdoc cref="OnEntityActionCallback"/>
    public static event OnEntityActionCallback? OnEntityAction;

    /// <inheritdoc cref="OnEntityAnimationCallback"/>
    public static event OnEntityAnimationCallback? OnEntityAnimation;

    /// <inheritdoc cref="OnEntityAnimationUpdateCallback"/>
    public static event OnEntityAnimationUpdateCallback? OnEntityAnimationUpdate;

    /// <inheritdoc cref="OnSendPacketCallback"/>
    public static event OnSendPacketCallback? OnSendPacket;

    /// <inheritdoc cref="OnReceivePacketCallback"/>
    public static event OnReceivePacketCallback? OnReceivePacket;

    /// <inheritdoc cref="OnLobbySearchCallback"/>
    public static event OnLobbySearchCallback? OnLobbySearch;

    /// <inheritdoc cref="OnRenderCallback"/>
    public static event OnRenderCallback? OnRender;

    /// <inheritdoc cref="OnImGuiRenderCallback"/>
    public static event OnImGuiRenderCallback? OnImGuiRender;

    /// <inheritdoc cref="OnImGuiFreeRenderCallback"/>
    public static event OnImGuiFreeRenderCallback? OnImGuiFreeRender;

    internal static void InvokeOnUpdate(float deltaTime) => OnUpdate?.Invoke(deltaTime);
    internal static void InvokeOnSave() => OnSave?.Invoke();
    internal static void InvokeOnSelectSaveSlot(int slot) => OnSelectSaveSlot?.Invoke(slot);
    internal static void InvokeOnResourceLoad(Resource? r, MtDti d, string p, LoadFlags l) => OnResourceLoad?.Invoke(r, d, p, l);
    internal static void InvokeOnChatMessageSent(string message) => OnChatMessageSent?.Invoke(message);
    internal static void InvokeOnQuestAccept(int id) => OnQuestAccept?.Invoke(id);
    internal static void InvokeOnQuestCancel(int id) => OnQuestCancel?.Invoke(id);
    internal static void InvokeOnQuestDepart(int id) => OnQuestDepart?.Invoke(id);
    internal static void InvokeOnQuestEnter(int id) => OnQuestEnter?.Invoke(id);
    internal static void InvokeOnQuestLeave(int id) => OnQuestLeave?.Invoke(id);
    internal static void InvokeOnQuestComplete(int id) => OnQuestComplete?.Invoke(id);
    internal static void InvokeOnQuestFail(int id) => OnQuestFail?.Invoke(id);
    internal static void InvokeOnQuestReturn(int id) => OnQuestReturn?.Invoke(id);
    internal static void InvokeOnQuestAbandon(int id) => OnQuestAbandon?.Invoke(id);
    internal static void InvokeOnMonsterCreate(Monster m) => OnMonsterCreate?.Invoke(m);
    internal static void InvokeOnMonsterInitialized(Monster m) => OnMonsterInitialized?.Invoke(m);
    internal static void InvokeOnMonsterAction(Monster m, ref int actionId) => OnMonsterAction?.Invoke(m, ref actionId);
    internal static bool InvokeOnMonsterFlinch(Monster m, ref int actionId) => OnMonsterFlinch?.Invoke(m, ref actionId) ?? true;
    internal static void InvokeOnMonsterEnrage(Monster m) => OnMonsterEnrage?.Invoke(m);
    internal static void InvokeOnMonsterUnenrage(Monster m) => OnMonsterUnenrage?.Invoke(m);
    internal static void InvokeOnMonsterDeath(Monster m) => OnMonsterDeath?.Invoke(m);
    internal static void InvokeOnMonsterDestroy(Monster m) => OnMonsterDestroy?.Invoke(m);
    internal static void InvokeOnPlayerAction(Player p, ref ActionInfo actionInfo) => OnPlayerAction?.Invoke(p, ref actionInfo);
    internal static void InvokeOnWeaponChange(Player p, WeaponType type, int id) => OnWeaponChange?.Invoke(p, type, id);
    internal static void InvokeOnEntityAction(Entity e, ref ActionInfo actionInfo) => OnEntityAction?.Invoke(e, ref actionInfo);
    internal static void InvokeOnEntityAnimation(Entity e, ref AnimationId id, ref float s, ref float i) => OnEntityAnimation?.Invoke(e, ref id, ref s, ref i);
    internal static void InvokeOnEntityAnimationUpdate(Entity e, AnimationId id, float d) => OnEntityAnimationUpdate?.Invoke(e, id, d);
    internal static void InvokeOnSendPacket(Packet p, bool b, SessionIndex s) => OnSendPacket?.Invoke(p, b, s);
    internal static void InvokeOnReceivePacket(uint id, PacketType t, SessionIndex s, NetBuffer b) => OnReceivePacket?.Invoke(id, t, s, b);
    internal static void InvokeOnLobbySearch(ref int maxResults) => OnLobbySearch?.Invoke(ref maxResults);
    internal static void InvokeOnRender() => OnRender?.Invoke();
    internal static void InvokeOnImGuiRender() => OnImGuiRender?.Invoke();
    internal static void InvokeOnImGuiFreeRender() => OnImGuiFreeRender?.Invoke();

    internal static bool AnyOnImGuiRender() => OnImGuiRender is not null;
}
