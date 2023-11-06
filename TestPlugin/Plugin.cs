using System;
using SharpPluginLoader.Core;

namespace TestPlugin
{
    public class Plugin : IPlugin
    {
        private delegate void ChangePendant(nint equipCharm, int pendantIndex, nint unk);

        private Hook<ChangePendant> _changePendantHook = null!;

        private unsafe void ChangePendantHook(nint equipCharmPtr, int pendantIndex, nint unk)
        {
            var getEquipBox = (delegate* unmanaged<nint, nint>)0x1410f6eb0;
            var equipBox = new MtObject(getEquipBox(Gui.SingletonInstance));
            var equipCharm = new MtObject(equipCharmPtr);
            var pendantId = equipCharm.GetObject<MtObject>(0x36A8 + pendantIndex * 8)?.Get<short>(0x20) ?? -1;
            var selectedEquip = equipCharm.GetObject<MtObject>(0x3DC8);

            if (selectedEquip == null)
            {
                _changePendantHook.Original(equipCharmPtr, pendantIndex, unk);
                return;
            }

            var weapons = equipBox.GetPtrInline<nint>(0x4408); // (nint*)(equipBox.Instance + 0x4408);
            var weaponCount = equipBox.Get<int>(0x9228);

            var equippedPendantId = selectedEquip.Get<int>(0x64);
            if (equippedPendantId == -1 || equippedPendantId != pendantId)
            {
                for (var i = 0; i < weaponCount; ++i)
                {
                    // If we assign this here the game will unequip it immediately after
                    if (weapons[i] != selectedEquip)
                        *(int*)(weapons[i] + 0x64) = pendantId;
                }
            }
            else // Unequip all pendants
            {
                for (var i = 0; i < weaponCount; ++i)
                {
                    if (weapons[i] != selectedEquip)
                        *(int*)(weapons[i] + 0x64) = -1;
                }
            }

            _changePendantHook.Original(equipCharmPtr, pendantIndex, unk);
        }

        public PluginData OnLoad()
        {
            _changePendantHook = Hook.Create<ChangePendant>(ChangePendantHook, 0x14050d700);

            return new PluginData
            {
                IsDebugPlugin = true,
            };
        }

        public void OnUpdate(float deltaTime)
        {
            
        }
    }
}
