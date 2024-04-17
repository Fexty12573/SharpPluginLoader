
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
}
