using SharpPluginLoader.Core.Memory;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Exposes various functions and properties related to quests.
    /// </summary>
    public static class Quest
    {
        /// <summary>
        /// The sQuest singleton instance.
        /// </summary>
        public static MtObject SingletonInstance => SingletonManager.GetSingleton("sQuest")!;

        /// <summary>
        /// Gets the current quest ID, or -1 if there is no current quest.
        /// </summary>
        public static int CurrentQuestId => SingletonInstance.Get<int>(0x4C);

        /// <summary>
        /// Gets the name of the current quest, or an empty string if there is no current quest.
        /// </summary>
        public static string CurrentQuestName => GetQuestName(CurrentQuestId);

        /// <summary>
        /// Gets the current quest star count.
        /// </summary>
        public static ref int CurrentQuestStarcount => ref SingletonInstance.GetRef<int>(0x50);

        /// <summary>
        /// Gets the current quest state.
        /// </summary>
        public static ref uint QuestState => ref SingletonInstance.GetRef<uint>(0x54);

        /// <summary>
        /// Gets the current quests reward money.
        /// </summary>
        public static ref uint CurrentQuestRewardMoney => ref SingletonInstance.GetRef<uint>(0x60);

        /// <summary>
        /// Gets the current quests reward HRP.
        /// </summary>
        public static ref uint CurrentQuestRewardHrp => ref SingletonInstance.GetRef<uint>(0x68);

        /// <summary>
        /// Gets the current quests objectives
        /// </summary>
        public static Span<QuestTargetData> Objectives => MemoryUtil.AsSpan<QuestTargetData>(SingletonInstance.Instance + 0x90, 4);

        /// <summary>
        /// Gets the quests ending timer.
        /// </summary>
        public static ref Timer QuestEndTimer => ref MemoryUtil.GetRef<Timer>(SingletonInstance.Instance + 0x13198);

        /// <summary>
        /// Gets the name of the quest with the specified ID.
        /// </summary>
        /// <param name="questId">The ID of the quest.</param>
        /// <returns>The name of the quest with the specified ID, or an empty string if the quest does not exist.</returns>
        public static unsafe string GetQuestName(int questId)
        {
            var ptr = GetQuestNameFunc.Invoke(SingletonInstance.Instance, questId, 0);
            return ptr != 0 ? new string((sbyte*)ptr) : string.Empty;
        }

        internal static void Initialize()
        {
            // AcceptQuest: 141b64be0 (When you accept a quest)
            _acceptQuestHook = Hook.Create<AcceptQuest>(AddressRepository.Get("Quest:AcceptQuest"), AcceptQuestHook);

            // EnterQuest: 141b699a0 (When you arrive in the quest)
            _enterQuestHook = Hook.Create<EnterQuest>(AddressRepository.Get("Quest:EnterQuest"), EnterQuestHook);

            // ReturnFromQuest: 141b6f600 (When you click return from quest)
            _returnFromQuestHook = Hook.Create<ReturnFromQuest>(AddressRepository.Get("Quest:ReturnFromQuest"), ReturnFromQuestHook);

            // LeaveQuest: 141b660d0 (Return/Abandon/Fail/Complete)
            _leaveQuestHook = Hook.Create<LeaveQuest>(AddressRepository.Get("Quest:LeaveQuest"), LeaveQuestHook);

            // AbandonQuest: 141b707a0 (When you click abandon quest)
            _abandonQuestHook = Hook.Create<AbandonQuest>(AddressRepository.Get("Quest:AbandonQuest"), AbandonQuestHook);

            // CancelQuest: 141b655a0 (When you cancel the quest before entering)
            _cancelQuestHook = Hook.Create<CancelQuest>(AddressRepository.Get("Quest:CancelQuest"), CancelQuestHook);

            // EndQuest: 141b646c0 (When you complete/fail the quest)
            _endQuestHook = Hook.Create<EndQuest>(AddressRepository.Get("Quest:EndQuest"), EndQuestHook);

            // DepartOnQuest: 141b69140 (When you click depart on quest)
            _departOnQuestHook = Hook.Create<DepartOnQuest>(AddressRepository.Get("Quest:DepartOnQuest"), DepartOnQuestHook);
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
                    Log.Debug($"Unknown quest end reason: {reason}");
                    break;
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
        private static readonly NativeFunction<nint, int, int, nint> GetQuestNameFunc = new(AddressRepository.Get("Quest:GetQuestName"));

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


    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public struct QuestTargetData
    {
        [FieldOffset(0x08)] public QuestTargetType Type;
        [FieldOffset(0x0C)] public short Id;
        [FieldOffset(0x0E)] public short RequiredCount;
        [FieldOffset(0x10)] public short Count;
    }

    [Flags]
    public enum QuestTargetType
    {
        None = 0,
        Monster = 0x1,
        Deliver = 0x2,
        Capture = 0x10,
        Slay = 0x20,
        Hunt = Slay | Capture,

        CaptureMonster = Monster | Capture,
        SlayMonster = Monster | Slay,
        HuntMonster = Monster | Hunt
    }
}
