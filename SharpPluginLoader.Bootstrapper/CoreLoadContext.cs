using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Loader;

namespace SharpPluginLoader.Bootstrapper
{
    public class CoreLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public CoreLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            EntryPoint.Log(EntryPoint.LogLevel.Info, $"[Bootstrapper] Loading {assemblyName.Name}");
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            EntryPoint.Log(EntryPoint.LogLevel.Info, $"[Bootstrapper] AssemblyPath: {assemblyPath}");
            var assembly = assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            if (assembly != null)
            {
                EntryPoint.Log(EntryPoint.LogLevel.Info, $"[Bootstrapper] Assembly Loaded: {assembly.GetName().Name}");
                return assembly;
            }

            EntryPoint.Log(EntryPoint.LogLevel.Info, $"[Bootstrapper] Attempting Default ALC load for {assemblyName.Name}");
            assembly = Default.LoadFromAssemblyName(assemblyName);
            EntryPoint.Log(EntryPoint.LogLevel.Info, $"[Bootstrapper] Default ALC loaded: {assembly.GetName().Name}");
            return assembly;
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : 0;
        }
    }
}
