using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Configuration;

namespace PlayerAnimationViewer
{
    internal class Config : IConfig
    {
        public string Name => "PlayerAnimationViewer";
        public string Version => "1.0.0";
        public string DuplicateHotkey { get; set; } = "Ctrl+D";
        public string DeleteHotkey { get; set; } = "Delete";
    }
}
