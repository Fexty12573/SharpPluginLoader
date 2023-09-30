using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public struct PluginData
    {
        public bool OnUpdate;
        public bool IsDebugScript;
    }

    public interface IPlugin
    {
        public PluginData OnLoad();
        public void OnUpdate(float deltaTime) => throw new NotImplementedException();
    }
}
