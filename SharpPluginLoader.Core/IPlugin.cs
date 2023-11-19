using System.Reflection;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{
    public struct PluginData
    {
        #region Generic
        /// <inheritdoc cref="IPlugin.OnUpdate"/>
        public bool OnUpdate;

        /// <inheritdoc cref="IPlugin.OnSave"/>
        public bool OnSave;

        /// <inheritdoc cref="IPlugin.OnSelectSaveSlot"/>
        public bool OnSelectSaveSlot;

        /// <inheritdoc cref="IPlugin.OnResourceLoad"/>
        public bool OnResourceLoad;

        /// <inheritdoc cref="IPlugin.OnChatMessageSent"/>
        public bool OnChatMessageSent;
        #endregion

        #region Quests
        /// <inheritdoc cref="IPlugin.OnQuestAccept"/>
        public bool OnQuestAccept;

        /// <inheritdoc cref="IPlugin.OnQuestCancel"/>
        public bool OnQuestCancel;

        /// <inheritdoc cref="IPlugin.OnQuestDepart"/>
        public bool OnQuestDepart;

        /// <inheritdoc cref="IPlugin.OnQuestEnter"/>
        public bool OnQuestEnter; 

        /// <inheritdoc cref="IPlugin.OnQuestLeave"/>
        public bool OnQuestLeave;

        /// <inheritdoc cref="IPlugin.OnQuestComplete"/>
        public bool OnQuestComplete;

        /// <inheritdoc cref="IPlugin.OnQuestFail"/>
        public bool OnQuestFail;

        /// <inheritdoc cref="IPlugin.OnQuestReturn"/>
        public bool OnQuestReturn;

        /// <inheritdoc cref="IPlugin.OnQuestAbandon"/>
        public bool OnQuestAbandon;
        #endregion

        #region Monster
        public bool OnMonsterCreate;
        public bool OnMonsterInitialized;

        /// <inheritdoc cref="IPlugin.OnMonsterAction"/>
        public bool OnMonsterAction;

        public bool OnMonsterFlinch;
        public bool OnMonsterAnimation;
        public bool OnMonsterEnrage;
        public bool OnMonsterUnenrage;
        public bool OnMonsterDeath;
        public bool OnMonsterDestroy;
        #endregion

        #region Player
        /// <inheritdoc cref="IPlugin.OnPlayerAction"/>
        public bool OnPlayerAction;

        /// <inheritdoc cref="IPlugin.OnWeaponChange"/>
        public bool OnWeaponChange;
        #endregion

        #region Entity
        /// <inheritdoc cref="IPlugin.OnEntityAction"/>
        public bool OnEntityAction;
        
        /// <inheritdoc cref="IPlugin.OnEntityAnimation"/>
        public bool OnEntityAnimation;

        /// <inheritdoc cref="IPlugin.OnEntityAnimationUpdate"/>
        public bool OnEntityAnimationUpdate;
        #endregion

        #region Network
        public bool OnSendPacket;
        public bool OnReceivePacket;
        #endregion

        #region Rendering
        /// <inheritdoc cref="IPlugin.OnRender"/>
        public bool OnRender;
        #endregion

        // Non-Events
        public bool IsDebugPlugin;
    }

    public interface IPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets called when the plugin is loaded. This is where you should initialize your plugin.
        /// The plugin must return a <see cref="PluginData"/> struct, which tells the framework which events to call.
        /// </summary>
        /// <returns>The filled out PluginData</returns>
        public PluginData OnLoad();

        #region Generic
        /// <summary>
        /// Gets called every frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last time this function was called, in seconds</param>
        public void OnUpdate(float deltaTime) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the game is saved.
        /// </summary>
        public void OnSave() => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player selects a save slot.
        /// </summary>
        /// <param name="slot">The save slot that was selected</param>
        public void OnSelectSaveSlot(int slot) => throw new MissingEventException();

        /// <summary>
        /// Gets called when a resource is requested/loaded.
        /// </summary>
        /// <param name="resource">The loaded resource, or null if the request failed</param>
        /// <param name="dti">The DTI of the resource</param>
        /// <param name="path">The file path of the resource, without its extension</param>
        /// <param name="flags">The flags passed to the request</param>
        public void OnResourceLoad(Resource? resource, MtDti dti, string path, uint flags) => throw new MissingEventException();

        /// <summary>
        /// Gets called when a chat message is sent (on the local side).
        /// </summary>
        /// <param name="message">The contents of the message</param>
        public void OnChatMessageSent(string message) => throw new MissingEventException();
        #endregion

        #region Quests
        /// <summary>
        /// Gets called when a quest is accepted on the quest board.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestAccept(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when a quest is cancelled on the quest board.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestCancel(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player departs on a quest.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestDepart(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player arrives in the quest area.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestEnter(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player leaves the quest area.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestLeave(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when a quest is completed.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestComplete(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when a quest is failed.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestFail(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player selects "Return from Quest" in the menu.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestReturn(int questId) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player selects "Abandon Quest" in the menu.
        /// </summary>
        /// <param name="questId">The id of the quest</param>
        public void OnQuestAbandon(int questId) => throw new MissingEventException();
        #endregion

        #region Monster
        /// <summary>
        /// Gets called when a monster does an action.
        /// </summary>
        /// <param name="monster">The monster doing the action</param>
        /// <param name="actionId">The id of the action to be executed</param>
        /// <remarks>The actionId parameter can be modified to change the executed action</remarks>
        public void OnMonsterAction(Monster monster, ref int actionId) => throw new MissingEventException();
        #endregion

        #region Player
        /// <summary>
        /// Gets called when the player does an action.
        /// </summary>
        /// <param name="player">The player doing the action</param>
        /// <param name="action">The action to be executed</param>
        /// <remarks>The action parameter can be modified to change the executed action</remarks>
        public void OnPlayerAction(Player player, ref ActionInfo action) => throw new MissingEventException();

        /// <summary>
        /// Gets called when the player changes their weapon.
        /// </summary>
        /// <param name="player">The player changing weapons</param>
        /// <param name="weaponType">The new weapon type</param>
        /// <param name="weaponId">The new weapon id</param>
        /// <remarks>This function is called asynchronously.</remarks>
        public void OnWeaponChange(Player player, WeaponType weaponType, int weaponId) => throw new MissingEventException();
        #endregion

        #region Entity
        /// <summary>
        /// Gets called when any entity does an action.
        /// </summary>
        /// <param name="entity">The entity doing the action</param>
        /// <param name="action">The action to be executed</param>
        /// <remarks>The action parameter can be modified to change the executed action</remarks>
        public void OnEntityAction(Entity entity, ref ActionInfo action) => throw new MissingEventException();

        /// <summary>
        /// Gets called when any entity does an animation.
        /// </summary>
        /// <param name="entity">The entity doing the animation</param>
        /// <param name="animationId">The id of the animation to be executed</param>
        /// <param name="startFrame">The starting frame of the animation</param>
        /// <remarks>Both the animationId and the startFrame parameters can be modified to change the executed animation.</remarks>
        public void OnEntityAnimation(Entity entity, ref AnimationId animationId, ref float startFrame) => throw new MissingEventException();

        /// <summary>
        /// Gets called when an entity's animation component is updated.
        /// </summary>
        /// <param name="entity">The entity whos animation component is updated</param>
        /// <param name="currentAnimation">The current active animation</param>
        /// <param name="deltaTime">The time since the last time this entity's animation component was updated</param>
        public void OnEntityAnimationUpdate(Entity entity, AnimationId currentAnimation, float deltaTime) => throw new MissingEventException();
        #endregion

        #region Rendering
        /// <summary>
        /// The user can use this function to render things on the screen.
        /// </summary>
        public void OnRender() => throw new MissingEventException();
        #endregion

        #region Internal
        internal void Dispose()
        {
            // Dispose all IDisposable fields
            // This is called when the plugin is unloaded.
            foreach (var field in GetType().GetAllFields())
            {
                if (field.GetValue(this) is IDisposable disposable)
                    disposable.Dispose();
            }
        }
        #endregion
    }

    internal static class TypeExtensions
    {
        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }
    }
}
