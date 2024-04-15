using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides access to the game's resource manager (sMhResource)
    /// </summary>
    public static class ResourceManager
    {
        /// <summary>
        /// The sMhResource singleton instance.
        /// </summary>
        public static MtObject SingletonInstance { get; private set; } = null!;

        /// <summary>
        /// Gets a resource from the game's resource manager.
        /// 
        /// If the resource is already loaded, it will return the existing instance.
        /// Otherwise, it will attempt to load the resource from the specified path.
        /// Files in nativePC take precedence over files in the chunks.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the resource to get. Use <see cref="Resource"/> if there is no dedicated type in the framework for the resource you're trying to load.
        /// </typeparam>
        /// <param name="path">The path to the file, relative to the chunk root and without the extension</param>
        /// <param name="dti">The dti of the file to load</param>
        /// <param name="flags">Flags to pass on to the function (mostly undocumented)</param>
        /// <returns>The resource if it was found, or null</returns>
        public static unsafe T? GetResource<T>(string path, MtDti dti, uint flags = 1) where T : Resource, new()
        {
            var resource = GetResourceFunc.Invoke(SingletonInstance.Instance, dti.Instance, path, flags);
            return resource == 0 ? null : new T { Instance = resource };
        }

        internal static void Initialize()
        {
            _getResourceHook = Hook.Create<GetResourceDelegate>(GetResourceFunc.NativePointer, GetResourceHook);

            // sMhResource gets special treatment because GetResource is called before the
            // constructor of sMhMain finishes, so we need to hook the sMhResource constructor directly
            // to obtain the singleton instance.
            _resourceManagerCtorHook = Hook.Create<CtorDelegate>(
                AddressRepository.Get("ResourceManager:Ctor"),
                (inst, unk1, unk2, unk3, unk4) =>
                {
                    SingletonInstance = new MtObject(inst);
                    return _resourceManagerCtorHook.Original(inst, unk1, unk2, unk3, unk4);
                }
            );
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
        private static Hook<CtorDelegate> _resourceManagerCtorHook = null!;
        private static readonly NativeFunction<nint, nint, string, uint, nint> GetResourceFunc = new(AddressRepository.Get("ResourceManager:GetResource"));

        private delegate nint GetResourceDelegate(nint resourceMgr, nint dti, string path, uint flags);
        private delegate nint CtorDelegate(nint instance, nint unk1, uint unk2, nint unk3, int unk4);
    }
}
