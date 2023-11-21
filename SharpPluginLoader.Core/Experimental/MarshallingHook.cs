using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;

namespace SharpPluginLoader.Core.Experimental
{
    public class MarshallingHook
    {
        public static MarshallingHook<TFunction> Create<TFunction>(TFunction hook, long functionAddress) 
            where TFunction : Delegate
        {
            return new MarshallingHook<TFunction>(hook, functionAddress);
        }

        #region Internal
        internal static IHook? CreateHook(Forwarder forwarder, long functionAddress)
        {
            var createHook = CreateHookMethod.MakeGenericMethod(forwarder.Delegate.Type);
            var hook = createHook.Invoke(ReloadedHooks.Instance,
                new object?[] { forwarder.Delegate.Delegate, functionAddress });

            if (hook is IHook hookT)
                return hookT;
            
            return null;
        }

        static MarshallingHook()
        {
            var createHookMethods = ReloadedHooks.Instance
                .GetType()
                .GetMethods()
                .Where(m => m is { Name: "CreateHook", IsGenericMethodDefinition: true });

            CreateHookMethod = createHookMethods.FirstOrDefault(m =>
            {
                var parameters = m.GetParameters();
                var genericArguments = m.GetGenericArguments();

                return parameters.Length == 2 &&
                       parameters[0].ParameterType == genericArguments[0] &&
                       parameters[1].ParameterType == typeof(long);
            }, null!);

            if (CreateHookMethod == null)
                throw new InvalidOperationException("Reloaded.Hooks is not loaded");
        }

        private static readonly MethodInfo CreateHookMethod;
        #endregion
    }

    public class MarshallingHook<TFunction> : IDisposable where TFunction : Delegate
    {
        public void Enable() => _hook.Enable();

        public void Disable() => _hook.Disable();

        public bool IsEnabled => _hook.IsHookEnabled;

        public TFunction Original => (TFunction)_managedToNative.Delegate.Delegate;

        #region Internal
        internal MarshallingHook(TFunction function, long functionAddress)
        {
            _nativeToManaged = new Forwarder(function, ForwarderType.NativeToManaged);

            _hook = MarshallingHook.CreateHook(_nativeToManaged, functionAddress) ??
                    throw new InvalidOperationException("Failed to create hook");

            if (GetGenericHookType(_nativeToManaged.Delegate.Type)
                    .GetProperty("OriginalFunction")
                    ?.GetValue(_hook) is not Delegate original)
                throw new InvalidOperationException("Failed to get original function");
            
            _managedToNative = new Forwarder(original, ForwarderType.ManagedToNative, function);

            _hook.Activate();
        }

        private static Type GetGenericHookType(Type delegateType)
        {
            return typeof(IHook<>).MakeGenericType(delegateType);
        }

        private readonly Forwarder _nativeToManaged;
        private readonly Forwarder _managedToNative;
        private readonly IHook _hook;

        private void ReleaseUnmanagedResources()
        {
            if (IsEnabled)
                Disable();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~MarshallingHook()
        {
            ReleaseUnmanagedResources();
        }
        #endregion
    }
}
