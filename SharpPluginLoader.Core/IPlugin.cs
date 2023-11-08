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
        public void OnUpdate(float deltaTime) => throw new NotImplementedException();
        public void OnSave() => throw new NotImplementedException();
        public void OnSelectSaveSlot(int slot) => throw new NotImplementedException();
        // public void OnResourceLoad() => throw new NotImplementedException();
        public void OnChatMessageSent(string message) => throw new NotImplementedException();

        // Quests
        public void OnQuestAccept(int questId) => throw new NotImplementedException();
        public void OnQuestCancel(int questId) => throw new NotImplementedException();
        public void OnQuestDepart(int questId) => throw new NotImplementedException();
        public void OnQuestEnter(int questId) => throw new NotImplementedException();
        public void OnQuestLeave(int questId) => throw new NotImplementedException();
        public void OnQuestComplete(int questId) => throw new NotImplementedException();
        public void OnQuestFail(int questId) => throw new NotImplementedException();
        public void OnQuestReturn(int questId) => throw new NotImplementedException();
        public void OnQuestAbandon(int questId) => throw new NotImplementedException();

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
