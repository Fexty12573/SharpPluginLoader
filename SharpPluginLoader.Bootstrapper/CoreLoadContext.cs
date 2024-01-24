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

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            EntryPoint.Log(EntryPoint.LogLevel.Debug, $"[Bootstrapper] Loading {assemblyName.Name}");
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            var assembly = assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            if (assembly != null)
                return assembly;

            assembly = LoadFromChunk(assemblyName);
            if (assembly != null)
                return assembly;

            try
            {
                assembly = Default.LoadFromAssemblyName(assemblyName);
                return assembly;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads an assembly without locking the file
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <returns>The loaded assembly</returns>
        public Assembly LoadNoLock(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath == null)
                return Default.LoadFromAssemblyName(assemblyName);

            var bytes = File.ReadAllBytes(assemblyPath);
            return LoadFromStream(new MemoryStream(bytes));
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
            if (unmanagedDllName.Contains("cimgui"))
            {
                EntryPoint.Log(EntryPoint.LogLevel.Debug, $"[{unmanagedDllName}] Resolved Path: nativePC/plugins/CSharp/Loader/cimgui.dll");
#if DEBUG
                return LoadUnmanagedDllFromPath(Path.GetFullPath("nativePC/plugins/CSharp/Loader/cimgui.debug.dll"));
#else
                return LoadUnmanagedDllFromPath(Path.GetFullPath("nativePC/plugins/CSharp/Loader/cimgui.dll"));
#endif
            }

            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            EntryPoint.Log(EntryPoint.LogLevel.Debug, $"[{unmanagedDllName}] Resolved Path: {libraryPath}");
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : 0;
        }
    }
}
