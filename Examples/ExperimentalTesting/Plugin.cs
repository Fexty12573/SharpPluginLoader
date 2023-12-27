using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Experimental;
using SharpPluginLoader.Core.Resources;

namespace ExperimentalTesting
{
    public class Plugin : IPlugin
    {
        public string Name => "Experimental Testing";

        public delegate void CreateShinyDropDelegate(MtObject a, int b, int c, nint d, long e, uint f);
        public delegate void ReleaseResourceDelegate(MtObject resourceMgr, Resource resource);
        private MarshallingHook<CreateShinyDropDelegate> _createShinyDropHook = null!;
        private MarshallingHook<ReleaseResourceDelegate> _releaseResourceHook = null!;

        public void CreateShinyDropHook(MtObject a, int b, int c, nint d, long e, uint f)
        {
            Log.Info($"CreateShinyDropHook: {a},{b},{c},{d},{e},{f}");
            _createShinyDropHook.Original(a, b, c, d, e, f);
        }

        public void ReleaseResourceHook(MtObject resourceMgr, Resource resource)
        {
            Log.Info($"Releasing Resource: {resource.FilePath}.{resource.FileExtension}" + 
                     (resource.Get<int>(0x5C) == 1 ? " | Unloading..." : ""));
            _releaseResourceHook.Original(resourceMgr, resource);
        }

        public PluginData Initialize()
        {
            return new PluginData();
        }

        public void OnLoad()
        {
            //_createShinyDropHook = MarshallingHook.Create<CreateShinyDropDelegate>(CreateShinyDropHook, 0x1402cb1d0);
            //_releaseResourceHook = MarshallingHook.Create<ReleaseResourceDelegate>(ReleaseResourceHook, 0x142224890);
        }
    }
}
