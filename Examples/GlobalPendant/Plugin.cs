using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;

namespace GlobalPendant
{
    public class Plugin : IPlugin
    {
        public string Name => "GlobalPendant";
        public string Author => "Fexty";

        private delegate void ChangePendant(nint equipCharm, int pendantIndex, nint unk);

        private Hook<ChangePendant> _changePendantHook = null!;

        private unsafe void ChangePendantHook(nint equipCharmPtr, int pendantIndex, nint unk)
        {
            var getEquipBox = (delegate* unmanaged<nint, nint>)0x1410f6eb0;
            var equipBox = new GuiEquipBox(getEquipBox(Gui.SingletonInstance));
            var equipCharm = new GuiEquipCharm(equipCharmPtr);
            var pendantId = equipCharm.GetPendantIdAt(pendantIndex);
            var selectedEquip = equipCharm.SelectedEquipment;

            if (selectedEquip == null)
            {
                _changePendantHook.Original(equipCharmPtr, pendantIndex, unk);
                return;
            }

            var weapons = equipBox.Weapons;
            var weaponCount = equipBox.WeaponCount;

            var equippedPendantId = selectedEquip.Pendant;

            Gui.DisplayYesNoDialog("Apply to all Weapons?", r =>
            {
                if (r != DialogResult.Yes)
                    return;
                
                if (equippedPendantId == -1 || equippedPendantId != pendantId)
                {
                    for (var i = 0; i < weaponCount; ++i)
                    {
                        // If we assign this here the game will unequip it immediately after
                        if (weapons[i] != selectedEquip)
                            weapons[i].Pendant = pendantId;
                    }
                }
                else // Unequip all pendants
                {
                    for (var i = 0; i < weaponCount; ++i)
                    {
                        if (weapons[i] != selectedEquip)
                            weapons[i].Pendant = -1;
                    }
                }
            });

            _changePendantHook.Original(equipCharmPtr, pendantIndex, unk);
        }

        public void OnLoad()
        {
            _changePendantHook = Hook.Create<ChangePendant>(0x14050d700, ChangePendantHook);
        }
    }
}
