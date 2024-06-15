using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

namespace SharpPluginLoader.Core.Memory
{
    /// <summary>
    /// Used to mark a method as a hook.
    /// </summary>
    /// <remarks>
    /// Methods marked with this attribute must be in a class marked with <see cref="HookProviderAttribute"/>.
    /// Additionally, the method must be static <b>unless</b> it is inside an <see cref="IPlugin"/> class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HookAttribute : Attribute
    {
        public long Address { get; init; }
        public string? Pattern { get; init; }
        public int Offset { get; init; }
        public bool Cache { get; init; }
    }

    /// <summary>
    /// Used to mark a class as a hook provider. Classes marked with this attribute are allowed
    /// to contain methods marked with <see cref="HookAttribute"/>. All hooks in the class will be
    /// automatically registered when the plugin is loaded, and unregistered when the plugin is unloaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HookProviderAttribute : Attribute;

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
    /// <remarks>
    /// Use <see cref="Hook.Create{TFunction}(long, TFunction)"/> to create a new hook.
    /// </remarks>
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
