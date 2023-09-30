using System;
using SharpPluginLoader.Core;

namespace TestPlugin
{
    public class Plugin : IPlugin
    {
        public PluginData OnLoad()
        {
            return new PluginData
            {
                OnUpdate = true,
                IsDebugPlugin = true,
            };
        }

        public void OnUpdate(float deltaTime)
        {
            Log.Info($"[TestPlugin] OnUpdate: {deltaTime}s");
        }
    }
}
