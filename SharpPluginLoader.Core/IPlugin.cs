using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public struct PluginData
    {
        // Generic
        public bool OnUpdate;
        public bool OnSave;
        public bool OnSelectSaveSlot;
        public bool OnResourceLoad;
        public bool OnChatMessageSent;

        // Quests
        public bool OnQuestAccept;
        public bool OnQuestCancel;
        public bool OnQuestDepart;
        public bool OnQuestEnter; 
        public bool OnQuestLeave;
        public bool OnQuestComplete;
        public bool OnQuestFail;
        public bool OnQuestReturn;
        public bool OnQuestAbandon;

        // Monster
        public bool OnMonsterCreate;
        public bool OnMonsterInitialized;
        public bool OnMonsterAction;
        public bool OnMonsterFlinch;
        public bool OnMonsterAnimation;
        public bool OnMonsterEnrage;
        public bool OnMonsterUnenrage;
        public bool OnMonsterDeath;
        public bool OnMonsterDestroy;

        // Player
        public bool OnPlayerAction;

        // Network
        public bool OnSendPacket;
        public bool OnReceivePacket;

        // Non-Events
        public bool IsDebugPlugin;
    }

    public interface IPlugin
    {
        public PluginData OnLoad();

        // Generic
        public void OnUpdate(float deltaTime) => throw new MissingEventException();
        public void OnSave() => throw new MissingEventException();
        public void OnSelectSaveSlot(int slot) => throw new MissingEventException();
        public void OnResourceLoad(Resource? resource, MtDti dti, string path, uint flags) => throw new MissingEventException();
        public void OnChatMessageSent(string message) => throw new MissingEventException();

        // Quests
        public void OnQuestAccept(int questId) => throw new MissingEventException();
        public void OnQuestCancel(int questId) => throw new MissingEventException();
        public void OnQuestDepart(int questId) => throw new MissingEventException();
        public void OnQuestEnter(int questId) => throw new MissingEventException();
        public void OnQuestLeave(int questId) => throw new MissingEventException();
        public void OnQuestComplete(int questId) => throw new MissingEventException();
        public void OnQuestFail(int questId) => throw new MissingEventException();
        public void OnQuestReturn(int questId) => throw new MissingEventException();
        public void OnQuestAbandon(int questId) => throw new MissingEventException();

        // Monster
        public void OnMonsterAction(Monster monster, ref int actionId) => throw new MissingEventException();

        // Player
        public void OnPlayerAction(ref ActionInfo action) => throw new MissingEventException();

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
    }

    internal static class TypeExtensions
    {
        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }
    }
}
