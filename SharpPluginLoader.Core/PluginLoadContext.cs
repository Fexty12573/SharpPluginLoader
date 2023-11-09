using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private static AssemblyLoadContext CurrentLoadContext => GetLoadContext(Assembly.GetExecutingAssembly())!;

        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            Log.Debug($"[{assemblyName}] Resolved Path: {assemblyPath}");
            var assembly = assemblyPath != null ? CustomLoadFromAssemblyPath(assemblyPath) : null;
            return assembly ?? CurrentLoadContext.LoadFromAssemblyName(assemblyName);
        }

        private Assembly CustomLoadFromAssemblyPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                return LoadFromAssemblyPath(assemblyPath);

            var data = File.ReadAllBytes(assemblyPath);
            return LoadFromStream(new MemoryStream(data));
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : 0;
        }
    }
}
