using SharpPluginLoader.Core;

namespace BackgroundOptimize
{
    public class Plugin : IPlugin
    {
        public string Name => "BackgroundOptimize";
        public string Author => "Fexty";

        private float _lastFps;
        private AppExt _appExt = null!;
        private MhMain _mhMain = null!;

        public void OnLoad()
        {
            var ini = new IniReader("graphics_option.ini");
            var maxFps = ini.Read("FrameRate", "GraphicsOption");
            if (!float.TryParse(maxFps, out _lastFps))
            {
                _lastFps = 165.0f;
            }

            _mhMain = SingletonManager.GetSingleton("sMhMain")!.As<MhMain>();
            _appExt = SingletonManager.GetSingleton("sAppExt")!.As<AppExt>();
            _appExt.BackgroundLowPower = false;
        }

        public void OnUpdate(float deltaTime)
        {
            _mhMain.MaxFps = _appExt.GameFocused ? _lastFps : 10.0f;
        }
    }
}
