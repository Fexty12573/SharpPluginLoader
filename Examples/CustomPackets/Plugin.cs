using SharpPluginLoader.Core;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Networking;

namespace CustomPackets
{
    public class Plugin : IPlugin
    {
        public string Name => "CustomPackets";

        public PluginData OnLoad()
        {
            return new PluginData
            {
                OnUpdate = true,
                OnReceivePacket = true
            };
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.IsDown(Key.LeftControl) && Input.IsPressed(Key.P))
                Network.SendPacket(new PopupPacket("Shidded farded and camed"));
        }

        public void OnReceivePacket(uint id, PacketType type, SessionIndex session, NetBuffer buffer)
        {
            if (id == PopupPacket.StaticId)
            {
                var packet = new PopupPacket(buffer);
                Gui.DisplayPopup(packet.Message, TimeSpan.FromMilliseconds(100));
            }
        }
    }
}
