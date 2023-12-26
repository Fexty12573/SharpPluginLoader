using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Loader;
using SharpPluginLoader.Core.Configuration;

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

            public void Dispose()
            {
                ConfigManager.SaveAndUnloadConfig(Plugin);
                Plugin.Dispose();
                Context.Unload();
            }
        }

        private static readonly TimeSpan EventCooldown = TimeSpan.FromMilliseconds(500);
        private readonly Dictionary<string, PluginContext> _contexts = new();
        private readonly FileSystemWatcher _watcher;
        private readonly object _lock = new();
        private readonly Dictionary<string, DateTime> _lastEventTimes = new();
#if DEBUG
        private readonly List<FileSystemWatcher> _symlinkWatchers = new();
#endif

        public PluginManager()
        {
            _watcher = new FileSystemWatcher("nativePC/plugins/CSharp");
            _watcher.IncludeSubdirectories = true;

            _watcher.Created += (_, args) => { if (IsPlugin(args.FullPath)) LoadPlugin(args.FullPath); };
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
                
                ReloadPlugin(args.FullPath);
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

                    symlinkWatcher.Created += (_, args) => { if (IsPlugin(args.FullPath)) LoadPlugin(args.FullPath); };
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

                        ReloadPlugin(args.FullPath);
                    };

                    symlinkWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    symlinkWatcher.EnableRaisingEvents = true;

                    _symlinkWatchers.Add(symlinkWatcher);
                }
            }
#endif
        }

        private static bool IsPlugin(string path)
        {
            return Path.GetExtension(path) == ".dll" && Path.GetFileName(Path.GetDirectoryName(path)) != "Loader";
        }

        public void LoadPlugins(string directory)
        {
            foreach (var pluginPath in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(Path.GetDirectoryName(pluginPath)) == "Loader")
                    continue;

                LoadPlugin(pluginPath);
            }
        }

        public void LoadPlugin(string pluginPath)
        {
            if (!File.Exists(pluginPath))
            {
                Log.Warn($"Plugin {pluginPath} does not exist");
                return;
            }

            Log.Info($"Loading plugin {pluginPath}");
            Log.Debug($"assembly context: {AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!.Name}");

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
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(i => i.ToString() == typeof(IPlugin).ToString()))
                    hasIPluginType = true;

                if (typeof(IPlugin).IsAssignableFrom(type))
                    pluginType = type;
            }

            if (pluginType is null)
            {
                // If there is a type that implements some IPlugin, but it is not assignable to IPlugin, then it is likely
                // that the plugin author made some mistake, (such as combining Debug/Release builds) and we should warn
                // them about it.
                // If there is no type at all that implements IPlugin, then that dll is most likely just a dependency of
                // another plugin, and we should just ignore it.
                if (hasIPluginType)
                    Log.Warn($"Plugin {pluginPath} does not have an entry point");

                context.Unload();
                return;
            }

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

            var pluginData = plugin.OnLoad();

            lock (_contexts)
            {
                _contexts.Add(plugin.Key, new PluginContext
                {
                    Context = context,
                    Assembly = assembly,
                    Plugin = plugin,
                    Data = pluginData,
                    Path = absPath
                });
            }

            return;

            string ResolvePluginPath(string path)
            {
                var file = new FileInfo(path);
                if (file.Attributes.HasFlag(FileAttributes.ReparsePoint | FileAttributes.Directory))
                    return file.ResolveLinkTarget(true)?.FullName ?? path;

                var pluginDir = Path.GetDirectoryName(pluginPath);
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

                UnloadPlugin(pluginPath);
            }

            LoadPlugins(directory);
        }

        public void ReloadPlugin(string pluginPath)
        {
            UnloadPlugin(pluginPath);
            LoadPlugin(pluginPath);
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
