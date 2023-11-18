using SharpPluginLoader.Core;

namespace BackgroundOptimize
{
    public class Plugin : IPlugin
    {
        public string Name => "BackgroundOptimize";

        private float _lastFps;

        public PluginData OnLoad()
        {
            AppExt.BackgroundLowPower = false;
            _lastFps = MhMain.MaxFps;

            return new PluginData
            {
                OnUpdate = true
            };
        }

        public void OnUpdate(float deltaTime)
        {
            MhMain.MaxFps = AppExt.GameFocused ? _lastFps : 10.0f;
        }
    }
}
