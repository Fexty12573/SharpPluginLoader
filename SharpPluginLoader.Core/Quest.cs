using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    public static class Quest
    {
        public static unsafe nint SingletonInstance => *(nint*)0x14500caf0;

        public static int CurrentQuestId => SingletonInstance.Read<int>(0x4C);

        public static string CurrentQuestName => GetQuestName(CurrentQuestId);

        public static unsafe string GetQuestName(int questId)
        {
            var ptr = GetQuestNameFunc.Invoke(SingletonInstance, questId, 0);
            return ptr != 0 ? new string((sbyte*)ptr) : string.Empty;
        } 

        internal static void Initialize()
        {
            // AcceptQuest: 141b64be0 (When you accept a quest)
            _acceptQuestHook = Hook.Create<AcceptQuest>(AcceptQuestHook, 0x141b64be0);

            // EnterQuest: 141b699a0 (When you arrive in the quest)
            _enterQuestHook = Hook.Create<EnterQuest>(EnterQuestHook, 0x141b699a0);

            // ReturnFromQuest: 141b6f600 (When you click return from quest)
            _returnFromQuestHook = Hook.Create<ReturnFromQuest>(ReturnFromQuestHook, 0x141b6f600);

            // LeaveQuest: 141b660d0 (Return/Abandon/Fail/Complete)
            _leaveQuestHook = Hook.Create<LeaveQuest>(LeaveQuestHook, 0x141b660d0);

            // AbandonQuest: 141b707a0 (When you click abandon quest)
            _abandonQuestHook = Hook.Create<AbandonQuest>(AbandonQuestHook, 0x141b707a0);

            // CancelQuest: 141b655a0 (When you cancel the quest before entering)
            _cancelQuestHook = Hook.Create<CancelQuest>(CancelQuestHook, 0x141b655a0);

            // EndQuest: 141b646c0 (When you complete/fail the quest)
            _endQuestHook = Hook.Create<EndQuest>(EndQuestHook, 0x141b646c0);

            // DepartOnQuest: 141b69140 (When you click depart on quest)
            _departOnQuestHook = Hook.Create<DepartOnQuest>(DepartOnQuestHook, 0x141b69140);
        }


        private static void AcceptQuestHook(nint questMgr, int questId, bool unk)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestAccept))
                plugin.OnQuestAccept(questId);

            _acceptQuestHook.Original(questMgr, questId, unk);
        }

        private static void EnterQuestHook(nint questMgr)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestEnter))
                plugin.OnQuestEnter(CurrentQuestId);

            _enterQuestHook.Original(questMgr);
        }

        private static void LeaveQuestHook(nint questMgr)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestLeave))
                plugin.OnQuestLeave(CurrentQuestId);

            _leaveQuestHook.Original(questMgr);
        }

        private static void AbandonQuestHook(nint questMgr, uint unk)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestAbandon))
                plugin.OnQuestAbandon(CurrentQuestId);

            _abandonQuestHook.Original(questMgr, unk);
        }

        private static void ReturnFromQuestHook(nint questMgr)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestReturn))
                plugin.OnQuestReturn(CurrentQuestId);

            _returnFromQuestHook.Original(questMgr);
        }

        private static void CancelQuestHook(nint questMgr)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestCancel))
                plugin.OnQuestCancel(CurrentQuestId);

            _cancelQuestHook.Original(questMgr);
        }

        private static void DepartOnQuestHook(nint questMgr, bool unk)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestDepart))
                plugin.OnQuestDepart(CurrentQuestId);

            _departOnQuestHook.Original(questMgr, unk);
        }

        private static void EndQuestHook(nint questMgr, bool unk1, QuestEndReason reason, byte unk3)
        {
            switch (reason)
            {
                case QuestEndReason.Complete:
                {
                    foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestComplete))
                        plugin.OnQuestComplete(CurrentQuestId);
                    break;
                }
                case QuestEndReason.Fail:
                {
                    foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnQuestFail))
                        plugin.OnQuestFail(CurrentQuestId);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
            }

            _endQuestHook.Original(questMgr, unk1, reason, unk3);
        }

        private static Hook<AcceptQuest> _acceptQuestHook = null!;
        private static Hook<EnterQuest> _enterQuestHook = null!;
        private static Hook<LeaveQuest> _leaveQuestHook = null!;
        private static Hook<AbandonQuest> _abandonQuestHook = null!;
        private static Hook<ReturnFromQuest> _returnFromQuestHook = null!;
        private static Hook<CancelQuest> _cancelQuestHook = null!;
        private static Hook<DepartOnQuest> _departOnQuestHook = null!;
        private static Hook<EndQuest> _endQuestHook = null!;
        private static readonly NativeFunction<nint, int, int, nint> GetQuestNameFunc = new(0x141b56e10);

        private delegate void AcceptQuest(nint questMgr, int questId, bool unk);
        private delegate void EnterQuest(nint questMgr);
        private delegate void LeaveQuest(nint questMgr);
        private delegate void AbandonQuest(nint questMgr, uint unk);
        private delegate void ReturnFromQuest(nint questMgr);
        private delegate void CancelQuest(nint questMgr);
        private delegate void DepartOnQuest(nint questMgr, bool unk);
        private delegate void EndQuest(nint questMgr, bool unk1, QuestEndReason reason, byte unk3);

        private enum QuestEndReason : uint
        {
            Complete = 3,
            Fail = 5
        }
    }
}
