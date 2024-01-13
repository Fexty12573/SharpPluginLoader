using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents an instance of the cActionController class.
    /// </summary>
    public class ActionController : MtObject
    {
        public ActionController(nint instance) : base(instance) { }
        public ActionController() { }

        /// <summary>
        /// The action that is currently being performed.
        /// </summary>
        public ref ActionInfo CurrentAction => ref GetRef<ActionInfo>(0xAC);

        /// <summary>
        /// The action that will be performed next.
        /// </summary>
        public ref ActionInfo NextAction => ref GetRef<ActionInfo>(0xBC);

        /// <summary>
        /// The action that was performed before the current one.
        /// </summary>
        public ref ActionInfo PreviousAction => ref GetRef<ActionInfo>(0xC4);

        /// <summary>
        /// The owner of this action controller.
        /// </summary>
        public Entity? Owner => GetObject<Entity>(0x100);

        /// <summary>
        /// Gets an action list by its index.
        /// </summary>
        /// <param name="actionSet">The index of the action set</param>
        /// <returns>The list of actions for the requested action set</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ActionList GetActionList(int actionSet)
        {
            if (actionSet is < 0 or > 3)
                throw new ArgumentOutOfRangeException(nameof(actionSet));

            return Get<ActionList>(0x68 + (actionSet * 0x10));
        }

        /// <summary>
        /// Makes the entity perform an action.
        /// </summary>
        /// <param name="actionSet">The action set to use</param>
        /// <param name="actionId">The id of the action within the action set</param>
        /// <remarks>For monsters use the <see cref="Monster.ForceAction"/> method instead.</remarks>
        public unsafe void DoAction(int actionSet, int actionId)
        {
            var actionInfo = stackalloc float[2] { actionSet, actionId };
            DoActionFunc.Invoke(Instance, (nint)actionInfo);
        }


        internal static void Initialize()
        {
            _doActionHook = Hook.Create<DoActionDelegate>(AddressRepository.Get("ActionController:DoAction"), DoActionHookFunc);
        }

        private static bool DoActionHookFunc(nint instance, ref ActionInfo actionInfo)
        {
            var actionController = new ActionController(instance);
            var owner = actionController.Owner;
            if (owner != null)
            {
                foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnEntityAction))
                    plugin.OnEntityAction(owner, ref actionInfo);
            }

            var player = Player.MainPlayer;
            if (instance != (player?.ActionController.Instance ?? 0))
                return _doActionHook.Original(instance, ref actionInfo);

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnPlayerAction))
                plugin.OnPlayerAction(player!, ref actionInfo);

            return _doActionHook.Original(instance, ref actionInfo);
        }

        private static bool LaunchActionHookFunc(nint instance, int actionId)
        {
            var monster = new Monster(instance);
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterAction))
                plugin.OnMonsterAction(monster, ref actionId);

            return _launchActionHook.Original(instance, actionId);
        }

        private delegate bool DoActionDelegate(nint instance, ref ActionInfo actionInfo);
        private delegate bool LaunchActionDelegate(nint instance, int actionId);
        private static readonly NativeFunction<nint, nint, bool> DoActionFunc = new(AddressRepository.Get("ActionController:DoAction"));
        private static Hook<DoActionDelegate> _doActionHook = null!;
        private static Hook<LaunchActionDelegate> _launchActionHook = null!;
    }

    /// <summary>
    /// Represents an action that can be performed by an entity.
    /// This is a combination of an action set and an action id.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct ActionInfo
    {
        /// <summary>
        /// The action set that this action belongs to.
        /// </summary>
        [FieldOffset(0x0)] public int ActionSet;

        /// <summary>
        /// The id of the action within the action set.
        /// </summary>
        [FieldOffset(0x4)] public int ActionId;


        public ActionInfo(int actionSet, int actionId)
        {
            ActionSet = actionSet;
            ActionId = actionId;
        }

        public static bool operator ==(ActionInfo left, ActionInfo right) => left.Equals(right);
        public static bool operator !=(ActionInfo left, ActionInfo right) => !left.Equals(right);
        public static bool operator ==(ActionInfo left, (int, int) right) => left.ActionSet == right.Item1 && left.ActionId == right.Item2;
        public static bool operator !=(ActionInfo left, (int, int) right) => !(left == right);
        public static bool operator ==((int, int) left, ActionInfo right) => right == left;
        public static bool operator !=((int, int) left, ActionInfo right) => !(right == left);

        public bool Equals(ActionInfo other)
        {
            return ActionSet == other.ActionSet && ActionId == other.ActionId;
        }

        public override bool Equals(object? obj)
        {
            return obj is ActionInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActionSet, ActionId);
        }

        public override string ToString() => $"{ActionSet}:{ActionId}";
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct ActionList
    {
        [FieldOffset(0x0)] public nint Actions;
        [FieldOffset(0x8)] public int Count;

        public readonly unsafe MtObject this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return new MtObject(((nint*)Actions)[index]);
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                ((nint*)Actions)[index] = value.Instance;
            }
        }
    }
}
