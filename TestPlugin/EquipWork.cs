using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;

namespace TestPlugin
{
    internal class EquipWork : MtObject
    {
        public EquipWork(nint instance) : base(instance) { }
        public EquipWork() { }

        public int Pendant
        {
            get => Get<int>(0x64);
            set => Set(0x64, value);
        }
    }
}
