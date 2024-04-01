using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Bootstrapper
{
    public unsafe class EntryPoint
    {
        private static CoreLoadContext? _loadContext;
        private static Assembly? _coreAssembly;
        private static delegate* unmanaged<int, nint, void> _logFunc;

        private delegate void PreInitializeDelegate(delegate* unmanaged<int, nint, void> logFunc, nint pointers);

        [UnmanagedCallersOnly]
        public static void Initialize(delegate* unmanaged<int, nint, void> logFunc, nint outData)
        {
            _logFunc = logFunc;
            Log(LogLevel.Info, "[Bootstrapper] Initializing SharpPluginLoader.Core");
            
            try
            {
#if DEBUG
                var fullPath = Path.GetFullPath("nativePC/plugins/CSharp/Loader/SharpPluginLoader.Core.Debug.dll");
                _loadContext = new CoreLoadContext(fullPath);
                _coreAssembly = _loadContext.LoadNoLock(fullPath);
                Log(LogLevel.Debug, $"[Bootstrapper] {_coreAssembly.FullName}");
#else
                var fullPath = Path.GetFullPath("nativePC/plugins/CSharp/Loader/SharpPluginLoader.Core.dll");
                _loadContext = new CoreLoadContext(fullPath);
                _coreAssembly = _loadContext.LoadNoLock(fullPath);
#endif
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, $"[Bootstrapper] Failed to load SharpPluginLoader.Core.dll: {e.GetType().Name}: {e.Message}, Stacktrace:\n{e.StackTrace}");
                Log(LogLevel.Error, $"[Bootstrapper] Inner Exception: " + (e.InnerException != null ? e.InnerException.Message : "N/A"));
                Log(LogLevel.Error, $"[Bootstrapper] Inner Exception Stracktrace: " + (e.InnerException != null ? e.InnerException.StackTrace : "N/A"));
                return;
            }

            Log(LogLevel.Info, "[Bootstrapper] SharpPluginLoader.Core loaded");
            var init = _coreAssembly.GetType("SharpPluginLoader.Core.NativeInterface")?.GetMethod("PreInitialize");
            if (init == null)
            {
                Log(LogLevel.Error, "[Bootstrapper] Failed to find SharpPluginLoader.Core.NativeInterface.Initialize");
                return;
            }

            init.CreateDelegate<PreInitializeDelegate>()(_logFunc, outData);
        }

        [UnmanagedCallersOnly]
        public static void Shutdown()
        {
            if (_loadContext == null)
            {
                Log(LogLevel.Warn, "[Bootstrapper] Shutdown called before Initialize");
                return;
            }

            _loadContext.Unload();
            _loadContext = null;
            _coreAssembly = null;
        }

        internal static void Log(LogLevel level, string message)
        {
            var str = Marshal.StringToHGlobalAnsi(message);
            _logFunc((int)level, str);
            Marshal.FreeHGlobal(str);
        }

        public enum LogLevel
        {
            Debug = 10,
            Info = 7,
            Warn = 14,
            Error = 12
        }
    }
}
