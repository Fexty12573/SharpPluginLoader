using SharpPluginLoader.Core;

namespace BackgroundOptimize
{
    public class Plugin : IPlugin
    {
        public string Name => "BackgroundOptimize";

        private float _lastFps;

        public PluginData Initialize()
        {
            return new PluginData
            {
                OnUpdate = true
            };
        }

        public void OnLoad()
        {
            AppExt.BackgroundLowPower = false;
            _lastFps = MhMain.MaxFps;
        }

        public void OnUpdate(float deltaTime)
        {
            MhMain.MaxFps = AppExt.GameFocused ? _lastFps : 10.0f;
        }
    }
}
