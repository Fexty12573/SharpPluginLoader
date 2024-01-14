using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Experimental;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

namespace PreloadTesting
{
    public class Plugin : IPlugin
    {
        public string Name => "Preload Testing";

        private delegate void StaticinitializerMtObjectDTI();
        private Hook<StaticinitializerMtObjectDTI> _staticinitializerMtObjectDTIHook = null!;

        public PluginData Initialize()
        {
            Log.Debug("PreloadTesting->Initialize called!");
            return new PluginData
            {
                OnPreMain = true,
                OnWinMain = true
            };
        }

        private unsafe void StaticinitializerMtObjectDTIHook()
        {
            Log.Debug("PreloadTesting->StaticinitializerMtObjectDTIHook called!");
            _staticinitializerMtObjectDTIHook.Original();
            return;
        }

        public void OnPreMain()
        {
            Log.Debug("PreloadTesting->OnPreMain called!");

            // Hook a static DTI initializer just to test/verify that we running before that.
            _staticinitializerMtObjectDTIHook = Hook.Create<StaticinitializerMtObjectDTI>(0x14020C1B0, StaticinitializerMtObjectDTIHook);
        }

        public void OnWinMain()
        {
            Log.Debug("PreloadTesting->OnWinMain called!");
        }

        public void OnLoad()
        {
            Log.Debug("PreloadTesting->OnLoad called!");
        }
    }
}