﻿using System.Reflection;
using System.Runtime.Loader;

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

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            Log.Debug($"[{assemblyName}] Resolved Path: {assemblyPath}");
            var assembly = assemblyPath != null ? CustomLoadFromAssemblyPath(assemblyPath) : null;
            if (assembly is not null)
                return assembly;

            try
            {
                assembly = CurrentLoadContext.LoadFromAssemblyName(assemblyName);
                return assembly;
            }
            catch (Exception)
            {
                return null;
            }
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
            if (unmanagedDllName.Contains("cimgui"))
            {
                Log.Debug($"[{unmanagedDllName}] Resolved Path: nativePC/plugins/CSharp/Loader/cimgui.dll");
#if DEBUG
                return LoadUnmanagedDllFromPath("nativePC/plugins/CSharp/Loader/cimgui.debug.dll");
#else
                return LoadUnmanagedDllFromPath("nativePC/plugins/CSharp/Loader/cimgui.dll");
#endif
            }

            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            Log.Debug($"[{unmanagedDllName}] Resolved Path: {libraryPath}");
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : 0;
        }
    }
}
