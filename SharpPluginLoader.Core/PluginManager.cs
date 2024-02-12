using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Loader;
using SharpPluginLoader.Core.Configuration;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Memory.Windows;

namespace SharpPluginLoader.Core
{
    internal class PluginManager
    {
        public static PluginManager Instance { get; } = new();
        public static string DefaultPluginDirectory => "nativePC/plugins/CSharp";

        private class PluginContext
        {
            public required PluginLoadContext Context { get; init; }
            public required Assembly Assembly { get; init; }
            public required IPlugin Plugin { get; init; }
            public required PluginData Data { get; init; }
            public required string Path {get; init; }
            public required nint NativePlugin { get; init; }

            public void Dispose()
            {
                Plugin.Dispose();
                if (NativePlugin != 0)
                    WinApi.FreeLibrary(NativePlugin);

                ConfigManager.SaveAndUnloadConfig(Plugin);
                Context.Unload();
            }
        }

        private delegate void UploadInternalCallsDelegate(IDictionary<string, nint> icalls, IDictionary<string, nint> addressCache);
        private static readonly TimeSpan EventCooldown = TimeSpan.FromMilliseconds(500);
        private readonly Dictionary<string, PluginContext> _contexts = [];
        private readonly FileSystemWatcher _watcher;
        private readonly object _lock = new();
        private readonly Dictionary<string, DateTime> _lastEventTimes = [];
#if DEBUG
        private readonly List<FileSystemWatcher> _symlinkWatchers = [];
#endif

        public PluginManager()
        {
            _watcher = new FileSystemWatcher("nativePC/plugins/CSharp");
            _watcher.IncludeSubdirectories = true;

            _watcher.Created += (_, args) =>
            {
                if (IsPlugin(args.FullPath))
                {
                    LoadPlugin(args.FullPath);
                    AddressRepository.SavePluginRecords();
                }
            };
            _watcher.Deleted += (_, args) => { if (IsPlugin(args.FullPath)) UnloadPlugin(args.FullPath); };
            _watcher.Changed += (_, args) =>
            {
                if (!IsPlugin(args.FullPath) || args.ChangeType != WatcherChangeTypes.Changed)
                    return;

                lock (_lock)
                {
                    var lastEventTime = _lastEventTimes.GetValueOrDefault(args.FullPath);
                    var nowTime = File.GetLastWriteTime(args.FullPath);
                    if (nowTime - lastEventTime < EventCooldown)
                        return;

                    _lastEventTimes[args.FullPath] = nowTime;
                }
                
                ReloadPlugin(args.FullPath, true);
            };

            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;

#if DEBUG
            var dirInfo = new DirectoryInfo("nativePC/plugins/CSharp");
            foreach (var info in dirInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                if (info.Attributes.HasFlag(FileAttributes.ReparsePoint | FileAttributes.Directory))
                {
                    var symlinkWatcher = new FileSystemWatcher(info.LinkTarget!);
                    symlinkWatcher.IncludeSubdirectories = true;

                    symlinkWatcher.Created += (_, args) =>
                    {
                        if (IsPlugin(args.FullPath))
                        {
                            LoadPlugin(args.FullPath);
                            AddressRepository.SavePluginRecords();
                        }
                    };
                    symlinkWatcher.Deleted += (_, args) => { if (IsPlugin(args.FullPath)) UnloadPlugin(args.FullPath); };
                    symlinkWatcher.Changed += (_, args) =>
                    {
                        if (!IsPlugin(args.FullPath) || args.ChangeType != WatcherChangeTypes.Changed)
                            return;

                        lock (_lock)
                        {
                            var lastEventTime = _lastEventTimes.GetValueOrDefault(args.FullPath);
                            var nowTime = File.GetLastWriteTime(args.FullPath);
                            if (nowTime - lastEventTime < EventCooldown)
                                return;

                            _lastEventTimes[args.FullPath] = nowTime;
                        }

                        ReloadPlugin(args.FullPath, true);
                    };

                    symlinkWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    symlinkWatcher.EnableRaisingEvents = true;

                    _symlinkWatchers.Add(symlinkWatcher);
                }
            }
#endif
        }

        private static string ResolvePluginPath(string path)
        {
            var file = new FileInfo(path);
            if (file.Attributes.HasFlag(FileAttributes.ReparsePoint | FileAttributes.Directory))
                return file.ResolveLinkTarget(true)?.FullName ?? path;

            var pluginDir = Path.GetDirectoryName(path);
            var pluginDirInfo = new DirectoryInfo(pluginDir!);
            if (pluginDirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint | FileAttributes.Directory))
            {
                var resolvedPluginDir = pluginDirInfo.ResolveLinkTarget(true);
                return resolvedPluginDir is null
                    ? path
                    : Path.Combine(resolvedPluginDir.FullName, Path.GetFileName(path));
            }

            return path;
        }

        private static bool IsPlugin(string path)
        {
            return Path.GetExtension(path) == ".dll"
                   && Path.GetFileName(Path.GetDirectoryName(path)) != "Loader"
                   && WinApi.IsManagedAssembly(path);
        }


        private PluginContext? GetPluginContextFromPath(string pluginPath)
        {
            var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            var relPath = Path.GetRelativePath(".", pluginPath);
            var resolvedPath = ResolvePluginPath(pluginPath);
            var absPath = Path.GetFullPath(resolvedPath);

            lock (_contexts)
            {
                List<PluginContext> pluginContexts = _contexts.Values.Where(ctx => ctx.Path == absPath).ToList();
                if (pluginContexts.Count != 1)
                {
                    return null;
                }

                return pluginContexts.First();
            }
        }

        public void PreloadPlugins(string pluginDirectory)
        {
            // All SPL plugins will be loaded during pre-main.
            LoadPlugins(pluginDirectory);

            // Call OnPreMain if subscribed.
            InvokeOnPreMain();
        }

        public void InvokeOnPreMain()
        {
            lock (_contexts)
            {
                foreach (var context in _contexts.Values.Where(context => context.Data.OnPreMain))
                {
                    context.Plugin.OnPreMain();
                }
            }
        }

        public void InvokeOnWinMain()
        {
            lock (_contexts)
            {
                foreach (var context in _contexts.Values.Where(context => context.Data.OnWinMain))
                {
                    context.Plugin.OnWinMain();
                }
            }
        }

        public void InvokeOnLoad()
        {
            lock (_contexts)
            {
                foreach (var context in _contexts.Values)
                {
                    context.Plugin.OnLoad();
                }
            }
        }

        public void LoadPlugins(string directory)
        {
            foreach (var pluginPath in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                if (IsPlugin(pluginPath))
                    LoadPlugin(pluginPath);
            }

            // After all plugins are loaded, we can save the plugin records cache.
            AddressRepository.SavePluginRecords();
        }

        /// <summary>
        /// Loads a plugin assembly into _contexts, with plugin-defined callback data (via `IPlugin.Initialize`).
        /// </summary>
        /// <param name="pluginPath"></param>
        public void LoadPlugin(string pluginPath)
        {
            if (!File.Exists(pluginPath))
            {
                Log.Warn($"Plugin {pluginPath} does not exist");
                return;
            }

            Log.Debug($"Attempting to load {pluginPath}");

            var pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            var relPath = Path.GetRelativePath(".", pluginPath);
            var resolvedPath = ResolvePluginPath(pluginPath);
            var absPath = Path.GetFullPath(resolvedPath);

            lock (_contexts)
            {
                if (_contexts.Values.Any(ctx => ctx.Path == absPath))
                {
                    Log.Warn($"Plugin {pluginPath} is already loaded");
                    return;
                }
            }

            Log.Debug($"Creating ALC for assembly {pluginName}");
            var context = new PluginLoadContext(pluginPath);
            Assembly? assembly;

            try
            {
                assembly = context.LoadFromAssemblyName(new AssemblyName(pluginName));
            }
            catch (Exception e)
            {
                if (e is BadImageFormatException ex) // Most likely a native dll
                    Log.Debug(ex.ToString());
                else
                    Log.Error(e.ToString());
                
                context.Unload();
                return;
            }

            var hasIPluginType = false;
            Type? pluginType = null;

            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetInterfaces().Any(i => i.ToString() == typeof(IPlugin).ToString()))
                        hasIPluginType = true;

                    if (typeof(IPlugin).IsAssignableFrom(type))
                        pluginType = type;
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Log.Error($"Failed to load types from {pluginName}:");
                foreach (var loaderException in e.LoaderExceptions)
                    Log.Error(loaderException?.ToString() ?? "Null Exception");
                context.Unload();
                return;
            }

            if (pluginType is null)
            {
                Log.Debug($"Missing IPlugin type in {pluginName}");

                // If there is a type that implements some IPlugin, but it is not assignable to IPlugin, then it is likely
                // that the plugin author made some mistake, (such as combining Debug/Release builds) and we should warn
                // them about it.
                // If there is no type at all that implements IPlugin, then that dll is most likely just a dependency of
                // another plugin, and we should just ignore it.
                if (hasIPluginType)
                    Log.Warn($"Plugin {pluginName} does not have an entry point");

                context.Unload();
                return;
            }

            Log.Info($"Loading plugin {pluginName}");

            if (Activator.CreateInstance(pluginType) is not IPlugin plugin)
            {
                Log.Warn($"Failed to create instance of {pluginType.FullName}");
                context.Unload();
                return;
            }

            lock (_contexts)
            {
                if (_contexts.ContainsKey(relPath))
                {
                    Log.Warn($"Plugin {pluginPath} has the same name as another plugin! The plugin will be loaded but there is a");
                }
            }

            var pluginData = plugin.Initialize();

            var nativePlugin = TryLoadNativePlugin(assembly, plugin, Path.ChangeExtension(absPath, ".Native.dll"));

            lock (_contexts)
            {
                _contexts.Add(plugin.Key, new PluginContext
                {
                    Context = context,
                    Assembly = assembly,
                    Plugin = plugin,
                    Data = pluginData,
                    Path = absPath,
                    NativePlugin = nativePlugin
                });
            }

            return;
        }

        private static unsafe nint TryLoadNativePlugin(Assembly assembly, IPlugin plugin, string path)
        {
            if (!File.Exists(path))
                return 0;

            var nativePlugin = WinApi.LoadLibrary(path);
            if (nativePlugin == 0)
            {
                Log.Error($"Failed to load native plugin for {plugin.Name}");
                return 0;
            }

            var getIcallCount = new NativeFunction<int>(WinApi.GetProcAddress(nativePlugin, "get_internal_call_count"));
            var collectIcalls = new NativeAction<nint>(WinApi.GetProcAddress(nativePlugin, "collect_internal_calls"));

            if (getIcallCount.NativePointer == 0)
            {
                Log.Error($"Native plugin for {plugin.Name} does not export GetInternalCallCount");
                WinApi.FreeLibrary(nativePlugin);

                return 0;
            }

            if (collectIcalls.NativePointer == 0)
            {
                Log.Error($"Native plugin for {plugin.Name} does not export CollectInternalCalls");
                WinApi.FreeLibrary(nativePlugin);

                return 0;
            }

            var icallCount = getIcallCount.Invoke();
            var icalls = NativeArray<InternalCall>.Create(icallCount);

            collectIcalls.Invoke(icalls.Address);

            var icallMap = icalls.ToDictionary(
                icall => icall.Name,
                icall => icall.FunctionPointer
            );

            var icallManager = assembly.GetTypes()
                .FirstOrDefault(type => type.GetCustomAttribute<InternalCallManagerAttribute>() is not null);
            if (icallManager is null)
            {
                Log.Error($"{plugin.Name} has a native component but no InternalCallManager");
                WinApi.FreeLibrary(nativePlugin);

                return 0;
            }

            var uploadInternalCalls = icallManager
                .GetMethod("UploadInternalCalls", BindingFlags.Public | BindingFlags.Static);
            if (uploadInternalCalls is null)
            {
                Log.Error($"{plugin.Name} has an InternalCallManager but is missing an UploadInternalCalls method");
                WinApi.FreeLibrary(nativePlugin);

                return 0;
            }

            try
            {
                uploadInternalCalls.CreateDelegate<UploadInternalCallsDelegate>()(icallMap, AddressRepository.GetPluginRecords());
            }
            catch (Exception e)
            {
                Log.Error($"Failed to upload InternalCalls for {plugin.Name}: {e}");
                WinApi.FreeLibrary(nativePlugin);

                return 0;
            }
            

            return nativePlugin;
        }

        public IPlugin[] GetPlugins()
        {
            lock (_contexts)
            {
                return _contexts.Values.Select(context => context.Plugin).ToArray();
            }
        }

        public IPlugin[] GetPlugins(Func<PluginData, bool> predicate)
        {
            lock (_contexts)
            {
                return _contexts.Values.Where(context => predicate(context.Data)).Select(context => context.Plugin).ToArray();
            }
        }

        public PluginData GetPluginData(IPlugin plugin)
        {
            lock (_contexts)
            {
                return _contexts[plugin.Key].Data;
            }
        }

        public void InvokeOnUpdate(float deltaTime)
        {
            lock (_contexts)
            {
                foreach (var context in _contexts.Values.Where(context => context.Data.OnUpdate))
                {
                    context.Plugin.OnUpdate(deltaTime);
                }
            }
        }

        public void ReloadPlugins(string directory)
        {
            foreach (var pluginPath in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(Path.GetDirectoryName(pluginPath)) == "Loader")
                    continue;

                ReloadPlugin(pluginPath);
            }

            AddressRepository.SavePluginRecords();
        }

        public void ReloadPlugin(string pluginPath, bool updatePluginCache = false)
        {
            UnloadPlugin(pluginPath);
            LoadPlugin(pluginPath);
            var context = GetPluginContextFromPath(pluginPath);
            if (context != null)
            {
                if(context.Data.OnPreMain || context.Data.OnWinMain)
                {
                    Log.Warn("Reloading plugin with (OnPreMain|OnWinMain) event subscriptions. These events WILL NOT be called during hot-reload!");
                }
                context.Plugin.OnLoad();
            }

            if (updatePluginCache)
                AddressRepository.SavePluginRecords();
        }

        public void UnloadAllPlugins()
        {
            lock (_contexts)
            {
                foreach (var context in _contexts.Values)
                    context.Dispose();
                _contexts.Clear();
            }
        }

        public string? GetPluginConfigPath(IPlugin plugin)
        {
            lock (_contexts)
            {
                return _contexts.TryGetValue(plugin.Key, out var context) 
                    ? Path.ChangeExtension(context.Path, ".json")
                    : null;
            }
        }

        private void UnloadPlugin(string pluginPath)
        {
            lock (_contexts)
            {
                var absPath = Path.GetFullPath(pluginPath);
                var key = _contexts
                    .Where(e => e.Value.Path == absPath)
                    .Select(e => e.Key)
                    .FirstOrDefault();

                Ensure.NotNull(key);
                _contexts[key].Dispose();
                _contexts.Remove(key);
            }
        }
    }
}
