using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool OnQuestPost;
        public bool OnQuestAccept;
        public bool OnQuestDepart;
        public bool OnQuestArrive;
        public bool OnQuestLeave;
        public bool OnQuestComplete;
        public bool OnQuestFail;
        public bool OnQuestJoin;
        public bool OnQuestReturn;
        public bool OnQuestAbandon;
        public bool OnQuestExit;

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
        public void OnQuestPost(int questId) => throw new NotImplementedException();
        public void OnQuestAccept(int questId) => throw new NotImplementedException();
        public void OnQuestDepart(int questId) => throw new NotImplementedException();
        public void OnQuestArrive(int questId) => throw new NotImplementedException();
        public void OnQuestLeave(int questId) => throw new NotImplementedException();
        public void OnQuestComplete(int questId) => throw new NotImplementedException();
        public void OnQuestFail(int questId) => throw new NotImplementedException();
        public void OnQuestJoin(int questId) => throw new NotImplementedException();
        public void OnQuestReturn(int questId) => throw new NotImplementedException();
        public void OnQuestAbandon(int questId) => throw new NotImplementedException();
        public void OnQuestExit(int questId) => throw new NotImplementedException();
    }
}
