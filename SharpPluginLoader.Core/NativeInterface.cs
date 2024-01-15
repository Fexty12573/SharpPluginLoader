using System.Reflection;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Networking;

namespace SharpPluginLoader.Core
{
    internal unsafe class NativeInterface
    {
        private delegate void ShutdownDelegate();
        private delegate void TriggerOnPreMainDelegate();
        private delegate void TriggerOnWinMainDelegate();
        private delegate void TriggerOnMhMainCtorDelegate();
        private delegate void ReloadPluginsDelegate();
        private delegate void ReloadPluginDelegate(string pluginName);
        private delegate void UploadInternalCallsDelegate(InternalCall* internalCalls, uint internalCallsCount);
        private delegate nint FindCoreMethodDelegate(string typeName, string methodName);
        private delegate void InitializeDelegate();

        private readonly struct RetrievedMethod(string typeName, string methodName, nint functionPointer)
        {
            public readonly nint FunctionPointer = functionPointer;

            private string FullName => $"{typeName}.{methodName}";
            public int Hash => FullName.GetHashCode();
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct ManagedFunctionPointers
        {
            public nint ShutdownPtr;
            public nint TriggerOnPreMainPtr;
            public nint TriggerOnWinMainPtr;
            public nint TriggerOnMhMainCtorPtr;
            public nint ReloadPluginsPtr;
            public nint ReloadPluginPtr;
            public nint UploadInternalCallsPtr;
            public nint FindCoreMethodPtr;
            public nint InitializePtr;
        }

        private static readonly Dictionary<int, RetrievedMethod> RetrievedMethods = new();

        public static void PreInitialize(delegate* unmanaged<int, nint, void> logFunc, nint pointers)
        {
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                var e = (Exception)args.ExceptionObject;
                Log.Error($"[Core] Unhandled exception: {e.GetType().Name}: {e.Message}, Stacktrace:\n{e.StackTrace}");
                if (e.InnerException is not null)
                {
                    Log.Error($"[Core] Inner exception: {e.InnerException.GetType().Name}: {e.InnerException.Message}, Stacktrace:\n{e.InnerException.StackTrace}");
                }
            };

            Log.Initialize(logFunc);
            GetManagedFunctionPointers((ManagedFunctionPointers*)pointers);
        }

        public static void Initialize()
        {
            try
            {
                PlaceNativeDlls();
                AddressRepository.Initialize();

                Task.WaitAll([
                    Task.Run(Gui.Initialize),
                    Task.Run(Quest.Initialize),
                    Task.Run(ResourceManager.Initialize),
                    Task.Run(Network.Initialize),
                    Task.Run(Player.Initialize),
                    Task.Run(Monster.Initialize),
                    Task.Run(ActionController.Initialize),
                    Task.Run(AnimationLayerComponent.Initialize)
                ]);
            }
            catch (Exception e)
            {
                Log.Error($"[Core] Failed to initialize: {e.GetType().Name}: {e.Message}, Stacktrace:\n{e.StackTrace}");
                if (e.InnerException != null)
                    Log.Error($"[Core] Inner exception: {e.InnerException.GetType().Name}: {e.InnerException.Message}, Stacktrace:\n{e.InnerException.StackTrace}");
            }
        }

        public static void GetManagedFunctionPointers(ManagedFunctionPointers* pointers)
        {
            pointers->ShutdownPtr = Marshal.GetFunctionPointerForDelegate(new ShutdownDelegate(Shutdown));
            pointers->TriggerOnPreMainPtr = Marshal.GetFunctionPointerForDelegate(new TriggerOnPreMainDelegate(TriggerOnPreMain));
            pointers->TriggerOnWinMainPtr = Marshal.GetFunctionPointerForDelegate(new TriggerOnWinMainDelegate(TriggerOnWinMain));
            pointers->TriggerOnMhMainCtorPtr = Marshal.GetFunctionPointerForDelegate(new TriggerOnMhMainCtorDelegate(TriggerOnMhMainCtor));
            pointers->ReloadPluginsPtr = Marshal.GetFunctionPointerForDelegate(new ReloadPluginsDelegate(ReloadPlugins));
            pointers->ReloadPluginPtr = Marshal.GetFunctionPointerForDelegate(new ReloadPluginDelegate(ReloadPlugin));
            pointers->UploadInternalCallsPtr = Marshal.GetFunctionPointerForDelegate(new UploadInternalCallsDelegate(InternalCallManager.UploadInternalCalls));
            pointers->FindCoreMethodPtr = Marshal.GetFunctionPointerForDelegate(new FindCoreMethodDelegate(FindCoreMethod));
            pointers->InitializePtr = Marshal.GetFunctionPointerForDelegate(new InitializeDelegate(Initialize));
            Log.Debug("[Core] Retrieved Function pointers");
        }

        public static void Shutdown()
        {
            PluginManager.Instance.UnloadAllPlugins();
        }

        public static void TriggerOnPreMain()
        {
            PluginManager.Instance.LoadPlugins(PluginManager.DefaultPluginDirectory);

            // Invoke OnPreMain for even subscribers.
            PluginManager.Instance.InvokeOnPreMain();
        }

        public static void TriggerOnWinMain()
        {
            PluginManager.Instance.InvokeOnWinMain();
        }

        public static void TriggerOnMhMainCtor()
        {
            PluginManager.Instance.InvokeOnLoad();
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
                if (RetrievedMethods.TryGetValue(hash, out var retrievedMethod))
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
                RetrievedMethods.Add(hash, new RetrievedMethod(typeName, methodName, ptr));
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

        private static void PlaceNativeDlls()
        {
            var defaultChunk = InternalCalls.GetDefaultChunk();
            var fasm = InternalCalls.ChunkGetFile(defaultChunk, "/NativeLibraries/FASMX64.dll");
            var fasmBytes = new NativeArray<byte>(
                InternalCalls.FileGetContents(fasm),
                (int)InternalCalls.FileGetSize(fasm)
            );

#if DEBUG
            const string cimguiName = "cimgui.debug";
#else
            const string cimguiName = "cimgui";
#endif
            var cimgui = InternalCalls.ChunkGetFile(defaultChunk, $"/NativeLibraries/{cimguiName}.dll");
            var cimguiBytes = new NativeArray<byte>(
                InternalCalls.FileGetContents(cimgui),
                (int)InternalCalls.FileGetSize(cimgui)
            );

            File.WriteAllBytes("FASMX64.dll", [.. fasmBytes]);
            File.WriteAllBytes($"nativePC/plugins/CSharp/Loader/{cimguiName}.dll", [.. cimguiBytes]);
        }
    }
}
