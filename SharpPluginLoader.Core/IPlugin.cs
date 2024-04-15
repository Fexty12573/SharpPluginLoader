using System.Reflection;
using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Configuration;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Networking;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{

#pragma warning disable CS0649

    /// <summary>
    /// Contains some configuration data for a plugin. This is returned by <see cref="IPlugin.Initialize"/>.
    /// </summary>
    public class PluginData
    {
        #region Pre-Main Events
        /// <inheritdoc cref="IPlugin.OnPreMain"/>
        internal bool OnPreMain;

        /// <inheritdoc cref="IPlugin.OnWinMain"/>
        internal bool OnWinMain;
        #endregion

        #region Generic
        /// <inheritdoc cref="IPlugin.OnUpdate"/>
        internal bool OnUpdate;

        /// <inheritdoc cref="IPlugin.OnSave"/>
        internal bool OnSave;

        /// <inheritdoc cref="IPlugin.OnSelectSaveSlot"/>
        internal bool OnSelectSaveSlot;

        /// <inheritdoc cref="IPlugin.OnResourceLoad"/>
        internal bool OnResourceLoad;

        /// <inheritdoc cref="IPlugin.OnChatMessageSent"/>
        internal bool OnChatMessageSent;
        #endregion

        #region Quests
        /// <inheritdoc cref="IPlugin.OnQuestAccept"/>
        internal bool OnQuestAccept;

        /// <inheritdoc cref="IPlugin.OnQuestCancel"/>
        internal bool OnQuestCancel;

        /// <inheritdoc cref="IPlugin.OnQuestDepart"/>
        internal bool OnQuestDepart;

        /// <inheritdoc cref="IPlugin.OnQuestEnter"/>
        internal bool OnQuestEnter; 

        /// <inheritdoc cref="IPlugin.OnQuestLeave"/>
        internal bool OnQuestLeave;

        /// <inheritdoc cref="IPlugin.OnQuestComplete"/>
        internal bool OnQuestComplete;

        /// <inheritdoc cref="IPlugin.OnQuestFail"/>
        internal bool OnQuestFail;

        /// <inheritdoc cref="IPlugin.OnQuestReturn"/>
        internal bool OnQuestReturn;

        /// <inheritdoc cref="IPlugin.OnQuestAbandon"/>
        internal bool OnQuestAbandon;
        #endregion

        #region Monster
        /// <inheritdoc cref="IPlugin.OnMonsterCreate"/>
        internal bool OnMonsterCreate;

        /// <inheritdoc cref="IPlugin.OnMonsterInitialized"/>
        internal bool OnMonsterInitialized;

        /// <inheritdoc cref="IPlugin.OnMonsterAction"/>
        internal bool OnMonsterAction;

        /// <inheritdoc cref="IPlugin.OnMonsterFlinch"/>
        internal bool OnMonsterFlinch;

        /// <inheritdoc cref="IPlugin.OnMonsterEnrage"/>
        internal bool OnMonsterEnrage;

        /// <inheritdoc cref="IPlugin.OnMonsterUnenrage"/>
        internal bool OnMonsterUnenrage;

        /// <inheritdoc cref="IPlugin.OnMonsterDeath"/>
        internal bool OnMonsterDeath;

        /// <inheritdoc cref="IPlugin.OnMonsterDestroy"/>
        internal bool OnMonsterDestroy;
        #endregion

        #region Player
        /// <inheritdoc cref="IPlugin.OnPlayerAction"/>
        internal bool OnPlayerAction;

        /// <inheritdoc cref="IPlugin.OnWeaponChange"/>
        internal bool OnWeaponChange;
        #endregion

        #region Entity
        /// <inheritdoc cref="IPlugin.OnEntityAction"/>
        internal bool OnEntityAction;
        
        /// <inheritdoc cref="IPlugin.OnEntityAnimation"/>
        internal bool OnEntityAnimation;

        /// <inheritdoc cref="IPlugin.OnEntityAnimationUpdate"/>
        internal bool OnEntityAnimationUpdate;
        #endregion

        #region Network
        internal bool OnSendPacket;
        internal bool OnReceivePacket;
        internal bool OnLobbySearch;
        #endregion

        #region Rendering
        /// <inheritdoc cref="IPlugin.OnRender"/>
        internal bool OnRender;
        /// <inheritdoc cref="IPlugin.OnImGuiRender"/>
        internal bool OnImGuiRender;
        /// <inheritdoc cref="IPlugin.OnImGuiFreeRender"/>
        internal bool OnImGuiFreeRender;
        #endregion

        // Non-Events
        public bool IsDebugPlugin; // Currently unused

        /// <summary>
        /// When set to true, any ImGui widgets rendered inside <see cref="IPlugin.OnImGuiRender"/> will be wrapped in a TreeNode.
        /// </summary>
        public bool ImGuiWrappedInTreeNode = true;
    }

#pragma warning restore CS0649

    /// <summary>
    /// The base interface for all plugins. This is where you define the events that your plugin listens to.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The author of the plugin.
        /// </summary>
        public string Author { get; }


        /// <summary>
        /// Gets called when the plugin is loaded. This is where you can optionally configure your plugin within the framework.
        /// 
        /// Default event, always called once per plugin [re]load.
        /// </summary>
        /// <returns>The filled out PluginData</returns>
        public PluginData Initialize() => new();

        /// <summary>
        /// Gets called after the game has initialized it's singletons. This is you initialize anything in your plugin
        /// that uses the game state (e.g. reading pointers, accessing singletons, etc).
        /// 
        /// Default event, always called once per plugin [re]load.
        /// </summary>
        public void OnLoad() { } // Not marked as a plugin event because it's always called

        /// <summary>
        /// Gets called when the plugin is unloaded. This is where you can clean up any resources used by your plugin.
        /// For example, threads, file handles, etc.
        /// </summary>
        /// <remarks>
        /// Note: IDisposable fields are automatically disposed when the plugin is unloaded.
        /// </remarks>
        public void OnUnload() { }


        #region Pre-Main Events
        /// <summary>
        /// Called before any of the game's code runs (including static initializers).
        /// This is only used for special cases, and is not applicable to most plugins.
        /// This will NOT be called during hot-reloading.
        /// </summary>
        [PluginEvent]
        public void OnPreMain() => throw new NotImplementedException();

        /// <summary>
        /// Called after game's static initializers, but before WinMain.
        /// This is only for special cases, and is not applicable to most plugins.
        /// This will NOT be called during hot-reloading.
        /// </summary>
        [PluginEvent]
        public void OnWinMain() => throw new NotImplementedException();
        #endregion

        #region Generic
        /// <summary>
        /// Gets called every frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last time this function was called, in seconds</param>
        [PluginEvent]
        public void OnUpdate(float deltaTime) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the game is saved.
        /// </summary>
        [PluginEvent]
        public void OnSave() => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player selects a save slot.
        /// </summary>
        /// <param name="slot">The save slot that was selected</param>
        [PluginEvent]
        public void OnSelectSaveSlot(int slot) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a resource is requested/loaded.
        /// </summary>
        /// <param name="resource">The loaded resource, or null if the request failed</param>
        /// <param name="dti">The DTI of the resource</param>
        /// <param name="path">The file path of the resource, without its extension</param>
        /// <param name="flags">The flags passed to the request</param>
        [PluginEvent]
        public void OnResourceLoad(Resource? resource, MtDti dti, string path, uint flags) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a chat message is sent (on the local side).
        /// </summary>
        /// <param name="message">The contents of the message</param>
        [PluginEvent]
        public void OnChatMessageSent(string message) => throw new NotImplementedException();
        #endregion

        #region Quests
        /// <summary>
        /// Gets called when a quest is accepted on the quest board.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestAccept(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a quest is cancelled on the quest board.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestCancel(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player departs on a quest.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestDepart(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player arrives in the quest area.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestEnter(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player leaves the quest area.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestLeave(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a quest is completed.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestComplete(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a quest is failed.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestFail(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player selects "Return from Quest" in the menu.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestReturn(int questId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player selects "Abandon Quest" in the menu.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        [PluginEvent]
        public void OnQuestAbandon(int questId) => throw new NotImplementedException();
        #endregion

        #region Monster
        /// <summary>
        /// Gets called when a monster is created.
        /// </summary>
        /// <param name="monster">The monster being created</param>
        /// <remarks>
        /// This function is called immediately after the monsters constructor is run,
        /// most of its data is not yet initialized by this point.
        /// </remarks>
        [PluginEvent]
        public void OnMonsterCreate(Monster monster) => throw new NotImplementedException();

        /// <summary>
        /// Gets called after a monster is initialized.
        /// </summary>
        /// <param name="monster">The monster that was initialized</param>
        /// <remarks>Most data in the monster is ready to be used by this point.</remarks>
        [PluginEvent]
        public void OnMonsterInitialized(Monster monster) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster does an action.
        /// </summary>
        /// <param name="monster">The monster doing the action</param>
        /// <param name="actionId">The id of the action to be executed</param>
        /// <remarks>The actionId parameter can be modified to change the executed action</remarks>
        [PluginEvent]
        public void OnMonsterAction(Monster monster, ref int actionId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster gets flinched.
        /// </summary>
        /// <param name="monster">The monster getting flinched</param>
        /// <param name="actionId">The flinch action it will perform</param>
        /// <returns>False to cancel the flinch</returns>
        [PluginEvent]
        public bool OnMonsterFlinch(Monster monster, ref int actionId) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster gets enraged.
        /// </summary>
        /// <param name="monster">The monster getting enraged</param>
        [PluginEvent]
        public void OnMonsterEnrage(Monster monster) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster leaves its enraged state.
        /// </summary>
        /// <param name="monster">The monster leaving its enraged state</param>
        [PluginEvent]
        public void OnMonsterUnenrage(Monster monster) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster dies.
        /// </summary>
        /// <param name="monster">The monster that died</param>
        [PluginEvent]
        public void OnMonsterDeath(Monster monster) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a monster is destroyed (its destructor is called).
        /// </summary>
        /// <param name="monster">The monster that is about to be destroyed</param>
        [PluginEvent]
        public void OnMonsterDestroy(Monster monster) => throw new NotImplementedException();
        #endregion

        #region Player
        /// <summary>
        /// Gets called when the player does an action.
        /// </summary>
        /// <param name="player">The player doing the action</param>
        /// <param name="action">The action to be executed</param>
        /// <remarks>The action parameter can be modified to change the executed action</remarks>
        [PluginEvent]
        public void OnPlayerAction(Player player, ref ActionInfo action) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the player changes their weapon.
        /// </summary>
        /// <param name="player">The player changing weapons</param>
        /// <param name="weaponType">The new weapon type</param>
        /// <param name="weaponId">The new weapon id</param>
        /// <remarks>This function is called asynchronously.</remarks>
        [PluginEvent]
        public void OnWeaponChange(Player player, WeaponType weaponType, int weaponId) => throw new NotImplementedException();
        #endregion

        #region Entity
        /// <summary>
        /// Gets called when any entity does an action.
        /// </summary>
        /// <param name="entity">The entity doing the action</param>
        /// <param name="action">The action to be executed</param>
        /// <remarks>The action parameter can be modified to change the executed action</remarks>
        [PluginEvent]
        public void OnEntityAction(Entity entity, ref ActionInfo action) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when any entity does an animation.
        /// </summary>
        /// <param name="entity">The entity doing the animation</param>
        /// <param name="animationId">The id of the animation to be executed</param>
        /// <param name="startFrame">The starting frame of the animation</param>
        /// <param name="interFrame">The number of frames to use for interpolation between animations</param>
        /// <remarks>Both the animationId and the startFrame parameters can be modified to change the executed animation.</remarks>
        [PluginEvent]
        public void OnEntityAnimation(Entity entity, ref AnimationId animationId, ref float startFrame, ref float interFrame) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when an entity's animation component is updated.
        /// </summary>
        /// <param name="entity">The entity whos animation component is updated</param>
        /// <param name="currentAnimation">The current active animation</param>
        /// <param name="deltaTime">The time since the last time this entity's animation component was updated</param>
        [PluginEvent]
        public void OnEntityAnimationUpdate(Entity entity, AnimationId currentAnimation, float deltaTime) => throw new NotImplementedException();
        #endregion

        #region Network
        /// <summary>
        /// Gets called when a packet is sent.
        /// </summary>
        /// <param name="packet">The packet being sent</param>
        /// <param name="isBroadcast">Whether the packet is broadcasted to all players in the session or not</param>
        /// <param name="session">The session the packet is sent to</param>
        [PluginEvent]
        public void OnSendPacket(Packet packet, bool isBroadcast, SessionIndex session) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when a packet is received.
        /// </summary>
        /// <param name="id">The id of the packet</param>
        /// <param name="type">The type of the packet</param>
        /// <param name="sourceSession">The session the packet was sent from</param>
        /// <param name="data">The data of the packet</param>
        [PluginEvent]
        public void OnReceivePacket(uint id, PacketType type, SessionIndex sourceSession, NetBuffer data) => throw new NotImplementedException();

        /// <summary>
        /// Gets called when the game is about to search for lobbies.
        /// </summary>
        /// <param name="maxResults">The maximum number of results to return.</param>
        /// <remarks>
        /// To modify the lobby search, take a look at the <see cref="Steam.Matchmaking"/> class
        /// </remarks>
        [PluginEvent]
        public void OnLobbySearch(ref int maxResults) => throw new NotImplementedException();
        #endregion

        #region Rendering
        /// <summary>
        /// The user can use this function to render arbitrary things on the screen (after the game has rendered).
        /// </summary>
        [PluginEvent]
        public void OnRender() => throw new NotImplementedException();

        /// <summary>
        /// The user can use this function to render ImGui widgets on the screen. Widgets rendered here will be inside the main SPL ImGui window.
        /// </summary>
        [PluginEvent]
        public void OnImGuiRender() => throw new NotImplementedException();

        /// <summary>
        /// The user can use this function to render their own ImGui windows.
        /// </summary>
        /// <remarks>
        /// <b>Note:</b> To render any ImGui widgets inside this function, you <i>must</i> create your own ImGui window.
        /// </remarks>
        [PluginEvent]
        public void OnImGuiFreeRender() => throw new NotImplementedException();
        #endregion

        #region Internal
        internal void Dispose()
        {
            // Dispose all IDisposable fields
            // This is called when the plugin is unloaded.
            foreach (var field in GetType().GetAllFields())
            {
                if (field.GetValue(this) is IDisposable disposable)
                {
                    if (disposable is null)
                        continue;

                    disposable.Dispose();
                }
            }
        }

        internal PluginData PluginData => PluginManager.Instance.GetPluginData(this);

        internal string Key => $"{Author}:{Name}";
        internal string? ConfigPath => PluginManager.Instance.GetPluginConfigPath(this);
        #endregion
    }

    internal static class TypeExtensions
    {
        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }

        public static bool OverridesMethod(this Type type, string methodName)
        {
            return type.GetMethod(methodName)?.DeclaringType == type;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class PluginEventAttribute : Attribute;
}
