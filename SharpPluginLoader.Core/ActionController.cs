using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    public class ActionController : MtObject
    {
        public ActionController(nint instance) : base(instance) { }
        public ActionController() { }

        public ActionInfo CurrentAction
        {
            get => GetMtType<ActionInfo>(0xAC);
            set => SetMtType(0xAC, value);
        }

        public ActionInfo NextAction
        {
            get => GetMtType<ActionInfo>(0xBC);
            set => SetMtType(0xBC, value);
        }

        public ActionInfo PreviousAction
        {
            get => GetMtType<ActionInfo>(0xC4);
            set => SetMtType(0xC4, value);
        }

        public ActionList GetActionList(int actionSet)
        {
            if (actionSet is < 0 or > 3)
                throw new ArgumentOutOfRangeException(nameof(actionSet));

            return GetMtType<ActionList>(0x68 + (actionSet * 0x10));
        }

        public unsafe void DoAction(int actionSet, int actionId)
        {
            var actionInfo = stackalloc float[2] { actionSet, actionId };
            DoActionFunc.Invoke(Instance, (nint)actionInfo);
        }

        internal static void Initialize()
        {
            _doActionHook = new Hook<DoActionDelegate>(DoActionHook, 0x140269c90);
        }

        private static bool DoActionHook(nint instance, ref ActionInfo actionInfo)
        {
            if (instance != (Player.MainPlayer?.ActionController.Instance ?? 0))
                return _doActionHook.Original(instance, ref actionInfo);

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnPlayerAction))
                plugin.OnPlayerAction(ref actionInfo);

            return _doActionHook.Original(instance, ref actionInfo);
        }

        private delegate bool DoActionDelegate(nint instance, ref ActionInfo actionInfo);
        private static readonly NativeFunction<nint, nint, bool> DoActionFunc = new(0x140269c90);
        private static Hook<DoActionDelegate> _doActionHook = null!;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct ActionInfo : IMtType
    {
        [FieldOffset(0x0)] public int ActionSet;
        [FieldOffset(0x4)] public int ActionId;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct ActionList : IMtType
    {
        [FieldOffset(0x8)] public nint Actions;
        [FieldOffset(0x0)] public int Count;

        public unsafe MtObject this[int index]
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
