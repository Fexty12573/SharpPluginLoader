using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

namespace SharpPluginLoader.Core.Memory
{
    public static class Hook
    {
        /// <summary>
        /// Creates a new native function hook.
        /// </summary>
        /// <typeparam name="TFunction">The type of the function to hook</typeparam>
        /// <param name="address">The address of the function to hook</param>
        /// <param name="hook">The hook function</param>
        /// <returns>The hook object</returns>
        public static Hook<TFunction> Create<TFunction>(long address, TFunction hook)
        {
            return new Hook<TFunction>(hook, address);
        }
    }

    /// <summary>
    /// Represents a native function hook.
    /// </summary>
    /// <typeparam name="TFunction">The type of the hooked function</typeparam>
    public class Hook<TFunction> : IDisposable
    {
        /// <summary>
        /// Enables the hook.
        /// </summary>
        public void Enable() => _hook.Enable();

        /// <summary>
        /// Disables the hook.
        /// </summary>
        public void Disable() => _hook.Disable();

        /// <summary>
        /// Gets the original function.
        /// </summary>
        public TFunction Original => _hook.OriginalFunction;

        /// <summary>
        /// Gets a value indicating whether the hook is enabled.
        /// </summary>
        public bool IsEnabled => _hook.IsHookEnabled;

        #region Internal
        internal Hook(TFunction hook, long address)
        {
            _hook = ReloadedHooks.Instance.CreateHook(hook, address).Activate();
        }

        private void ReleaseUnmanagedResources()
        {
            if (IsEnabled)
                _hook.Disable();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Hook()
        {
            ReleaseUnmanagedResources();
        }

        private readonly IHook<TFunction> _hook;
        #endregion
    }
}
