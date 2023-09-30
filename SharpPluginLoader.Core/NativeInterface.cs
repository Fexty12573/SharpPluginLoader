using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    internal unsafe class NativeInterface
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ManagedFunctionPointers
        {
            public delegate* unmanaged<void> ShutdownPtr;
            public delegate* unmanaged<float, void> OnUpdatePtr;
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
            pointers->ShutdownPtr = &Shutdown;
            pointers->OnUpdatePtr = &OnUpdate;
        }

        [UnmanagedCallersOnly]
        public static void Shutdown()
        {
            PluginManager.Instance.UnloadAllPlugins();
        }

        [UnmanagedCallersOnly]
        public static void OnUpdate(float deltaTime)
        {
            Log.Info($"OnUpdate: {deltaTime}s");
            PluginManager.Instance.InvokeOnUpdate(deltaTime);
        }
    }
}
