using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

namespace SharpPluginLoader.Core
{
    public class Hook
    {
        public static Hook<TFunction> Create<TFunction>(TFunction hook, long address)
        {
            return new Hook<TFunction>(hook, address);
        }
    }

    public class Hook<TFunction>
    {
        private readonly IHook<TFunction> _hook;

        internal Hook(TFunction hook, long address)
        {
            _hook = ReloadedHooks.Instance.CreateHook(hook, address).Activate();
        }

        public void Enable() => _hook.Enable();

        public void Disable() => _hook.Disable();

        public TFunction Original => _hook.OriginalFunction;

        public bool IsEnabled => _hook.IsHookEnabled;
    }
}
