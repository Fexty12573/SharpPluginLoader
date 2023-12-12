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

        private readonly struct PluginContext
        {
            public required PluginLoadContext Context { get; init; }
            public required Assembly Assembly { get; init; }
            public required IPlugin Plugin { get; init; }
            public required PluginData Data { get; init; }
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

            lock (_contexts)
            {
                if (_contexts.ContainsKey(relPath))
                    return;
            }
            
            var context = new PluginLoadContext(pluginPath);

            var assembly = context.LoadFromAssemblyName(new AssemblyName(pluginName));
            var pluginType = assembly.GetTypes().FirstOrDefault(type => typeof(IPlugin).IsAssignableFrom(type));

            if (pluginType is null)
            {
                Log.Warn($"Plugin {pluginPath} does not have an entry point");
                return;
            }

            if (Activator.CreateInstance(pluginType) is not IPlugin plugin)
            {
                Log.Warn($"Failed to create instance of {pluginType.FullName}");
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
                _contexts.Add(relPath, new PluginContext
                {
                    Context = context,
                    Assembly = assembly,
                    Plugin = plugin,
                    Data = pluginData
                });
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
                    context.Context.Unload();

                _contexts.Clear();
            }
        }

        public string? GetPluginConfigPath(IPlugin plugin)
        {
            lock (_contexts)
            {
                foreach (var (path, context) in _contexts)
                {
                    if (plugin.Key == context.Plugin.Key)
                        return Path.ChangeExtension(Path.GetRelativePath(".", path), ".json");
                }

                return null;
            }
        }

        private void UnloadPlugin(string pluginPath)
        {
            lock (_contexts)
            {
                var relPath = Path.GetRelativePath(".", pluginPath);
                if (!_contexts.TryGetValue(relPath, out var context))
                    return;

                ConfigManager.SaveAndUnloadConfig(context.Plugin);
                context.Plugin.Dispose();
                context.Context.Unload();
                _contexts.Remove(relPath);
            }
        }
    }
}
