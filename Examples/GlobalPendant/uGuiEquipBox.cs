using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;

namespace GlobalPendant
{
    internal class GuiEquipBox : MtObject
    {
        public GuiEquipBox(nint instance) : base(instance) { }
        public GuiEquipBox() { }

        public EquipWork[] Weapons
        {
            get
            {
                var weapons = new EquipWork[WeaponCount];
                for (var i = 0; i < WeaponCount; i++)
                    weapons[i] = GetObject<EquipWork>(0x4408 + i * 8)!;

                return weapons;
            }
        }

        public int WeaponCount => Get<int>(0x9228);
    }
}
