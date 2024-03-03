using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core
{
    public static class ResourceManager
    {
        public static MtObject SingletonInstance { get; private set; } = null!;

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
