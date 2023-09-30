using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    internal unsafe class NativeInterface
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ManagedFunctionPointers
        {
            public nint ShutdownPtr;
            public nint OnUpdatePtr;
            public nint ReloadPluginsPtr;
            public nint ReloadPluginPtr;
        }

        public static void Initialize(delegate* unmanaged<int, nint, void> logFunc, nint pointers)
        {
            Log.Initialize(logFunc);
            Log.Info("[Core] Loading plugins...");
            PluginManager.Instance.LoadPlugins(PluginManager.DefaultPluginDirectory);

            GetManagedFunctionPointers((ManagedFunctionPointers*)pointers);
        }

        public static void GetManagedFunctionPointers(ManagedFunctionPointers* pointers)
        {
            Log.Info("[Core] Retrieving function pointers");
            pointers->ShutdownPtr = Marshal.GetFunctionPointerForDelegate(Shutdown);
            pointers->OnUpdatePtr = Marshal.GetFunctionPointerForDelegate(OnUpdate);
            pointers->ReloadPluginsPtr = Marshal.GetFunctionPointerForDelegate(ReloadPlugins);
            pointers->ReloadPluginPtr = Marshal.GetFunctionPointerForDelegate(ReloadPlugin);
        }

        public static void Shutdown()
        {
            PluginManager.Instance.UnloadAllPlugins();
        }

        public static void OnUpdate(float deltaTime)
        {
            PluginManager.Instance.InvokeOnUpdate(deltaTime);
        }

        public static void ReloadPlugins()
        {
            PluginManager.Instance.ReloadPlugins(PluginManager.DefaultPluginDirectory);
        }

        public static void ReloadPlugin([MarshalAs(UnmanagedType.LPStr)] string pluginName)
        {
            PluginManager.Instance.ReloadPlugin(pluginName);
        }
    }
}
