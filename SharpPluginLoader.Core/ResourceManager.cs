using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{
    public static class ResourceManager
    {
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x145183f40);

        public static unsafe T? GetResource<T>(string path, MtDti dti, uint flags = 1) where T : Resource, new()
        {
            var resource = GetResourceFunc.Invoke(SingletonInstance, dti.Instance, path, flags);
            return resource == 0 ? null : new T { Instance = resource };
        }

        internal static void Initialize()
        {
            _getResourceHook = Hook.Create<GetResourceDelegate>(GetResourceHook, GetResourceFunc.NativePointer);
        }

        private static nint GetResourceHook(nint resourceMgr, nint dti, string path, uint flags)
        {
            var resource = _getResourceHook.Original(resourceMgr, dti, path, flags);
            var resObj = resource != 0 ? new Resource(resource) : null;
            var dtiObj = new MtDti(dti);

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnResourceLoad))
                plugin.OnResourceLoad(resObj, dtiObj, path, flags);

            return resource;
        }

        private static Hook<GetResourceDelegate> _getResourceHook = null!;
        private static readonly NativeFunction<nint, nint, string, uint, nint> GetResourceFunc = new(0x1422214f0);

        private delegate nint GetResourceDelegate(nint resourceMgr, nint dti, string path, uint flags);
    }
}
