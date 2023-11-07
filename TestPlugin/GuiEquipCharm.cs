using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;

namespace TestPlugin
{
    internal class GuiEquipCharm : MtObject
    {
        public GuiEquipCharm(nint instance) : base(instance) { }
        public GuiEquipCharm() { }

        public short GetPendantIdAt(int index)
        {
            return GetObject<MtObject>(0x36A8 + index * 8)?.Get<short>(0x10) ?? -1;
        }

        public EquipWork? SelectedEquipment => GetObject<EquipWork>(0x3DC8);
    }
}
