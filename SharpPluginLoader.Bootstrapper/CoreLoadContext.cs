using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Loader;
using SharpPluginLoader.Bootstrapper.Chunk;

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
            EntryPoint.Log(EntryPoint.LogLevel.Debug, $"[Bootstrapper] Loading {assemblyName.Name}");
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            var assembly = assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            if (assembly != null)
                return assembly;

            assembly = LoadFromChunk(assemblyName);
            if (assembly != null)
                return assembly;

            assembly = Default.LoadFromAssemblyName(assemblyName);
            return assembly;
        }

        private Assembly? LoadFromChunk(AssemblyName assemblyName)
        {
            var chunk = ChunkManager.GetDefaultChunk();

            try
            {
                var file = chunk.GetFile($"/Assemblies/{assemblyName.Name}.dll");
                return LoadFromStream(new MemoryStream(file.Contents));
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : 0;
        }
    }
}
