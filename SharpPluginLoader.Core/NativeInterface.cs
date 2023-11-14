using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    internal unsafe class NativeInterface
    {
        private delegate void ShutdownDelegate();
        private delegate void ReloadPluginsDelegate();
        private delegate void ReloadPluginDelegate(string pluginName);
        private delegate void UploadInternalCallsDelegate(InternalCall* internalCalls, uint internalCallsCount);
        private delegate nint FindCoreMethodDelegate(string typeName, string methodName);

        private readonly struct RetrievedMethod
        {
            private readonly string _typeName;
            private readonly string _methodName;
            public readonly nint FunctionPointer;

            private string FullName => $"{_typeName}.{_methodName}";
            public int Hash => FullName.GetHashCode();

            public RetrievedMethod(string typeName, string methodName, nint functionPointer)
            {
                _typeName = typeName;
                _methodName = methodName;
                FunctionPointer = functionPointer;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ManagedFunctionPointers
        {
            public nint ShutdownPtr;
            public nint LoadPluginsPtr;
            public nint ReloadPluginsPtr;
            public nint ReloadPluginPtr;
            public nint UploadInternalCallsPtr;
            public nint FindCoreMethodPtr;
        }

        private static readonly Dictionary<int, RetrievedMethod> _retrievedMethods = new();

        public static void Initialize(delegate* unmanaged<int, nint, void> logFunc, nint pointers)
        {
            Log.Initialize(logFunc);
            Gui.Initialize();
            Quest.Initialize();
            ResourceManager.Initialize();
            ActionController.Initialize();
            AnimationLayerComponent.Initialize();

            GetManagedFunctionPointers((ManagedFunctionPointers*)pointers);
        }

        public static void GetManagedFunctionPointers(ManagedFunctionPointers* pointers)
        {
            pointers->ShutdownPtr = Marshal.GetFunctionPointerForDelegate(new ShutdownDelegate(Shutdown));
            pointers->LoadPluginsPtr = Marshal.GetFunctionPointerForDelegate(new ReloadPluginsDelegate(LoadPlugins));
            pointers->ReloadPluginsPtr = Marshal.GetFunctionPointerForDelegate(new ReloadPluginsDelegate(ReloadPlugins)); ;
            pointers->ReloadPluginPtr = Marshal.GetFunctionPointerForDelegate(new ReloadPluginDelegate(ReloadPlugin));
            pointers->UploadInternalCallsPtr = Marshal.GetFunctionPointerForDelegate(new UploadInternalCallsDelegate(InternalCallManager.UploadInternalCalls));
            pointers->FindCoreMethodPtr = Marshal.GetFunctionPointerForDelegate(new FindCoreMethodDelegate(FindCoreMethod));
            Log.Debug("[Core] Retrieved Function pointers");
        }

        public static void Shutdown()
        {
            PluginManager.Instance.UnloadAllPlugins();
        }
        
        public static void LoadPlugins()
        {
            PluginManager.Instance.LoadPlugins(PluginManager.DefaultPluginDirectory);
        }

        public static void ReloadPlugins()
        {
            PluginManager.Instance.ReloadPlugins(PluginManager.DefaultPluginDirectory);
        }

        public static void ReloadPlugin([MarshalAs(UnmanagedType.LPStr)] string pluginName)
        {
            PluginManager.Instance.ReloadPlugin(pluginName);
        }

        public static nint FindCoreMethod(string typeName, string methodName)
        {
            try
            {
                var hash = $"{typeName}.{methodName}".GetHashCode();
                if (_retrievedMethods.TryGetValue(hash, out var retrievedMethod))
                {
                    Log.Info($"[Core] Found method {typeName}.{methodName} in cache");
                    return retrievedMethod.FunctionPointer;
                }

                var type = Type.GetType(typeName);
                if (type == null)
                {
                    Log.Error($"[Core] Failed to find type {typeName}");
                    return 0;
                }

                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                {
                    Log.Error($"[Core] Failed to find method {methodName} in type {typeName}");
                    return 0;
                }

                if (!method.IsStatic)
                {
                    Log.Error($"[Core] Method {typeName}.{methodName} is not static");
                    return 0;
                }

                if (method.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() == null)
                {
                    Log.Error($"[Core] Method {typeName}.{methodName} is not marked with UnmanagedCallersOnly");
                    return 0;
                }

                var ptr = method.MethodHandle.GetFunctionPointer();
                _retrievedMethods.Add(hash, new RetrievedMethod(typeName, methodName, ptr));
                return ptr;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return 0;
            }
        }

        [UnmanagedCallersOnly]
        public static void OnUpdate(float deltaTime)
        {
            PluginManager.Instance.InvokeOnUpdate(deltaTime);
        }
    }
}
