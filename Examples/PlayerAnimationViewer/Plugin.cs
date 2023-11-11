using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.IO;

namespace PlayerAnimationViewer
{
    public class Plugin : IPlugin
    {
        public PluginData OnLoad()
        {
            return new PluginData
            {
                OnUpdate = true
            };
        }

        public void OnUpdate(float deltaTime)
        {
            var player = Player.MainPlayer;
            if (player == null)
                return;

            for (var i = 0; i < 10; i++)
            {
                if (Input.IsPressed(Key.F1 + i))
                {
                    Log.Info($"Creating shell {i}");
                    player.CreateShell((uint)i, player.Forward * 10);
                }
            }

            if (!Input.IsDown(Button.L2)) 
                return;

            if (Input.IsPressed(Button.R3))
            {
                if (!TogglePause(player))
                    return;
            }

            if (Input.IsPressed(Button.Right))
                player.AnimationLayer!.CurrentFrame += 1;

            if (Input.IsPressed(Button.Left))
                player.AnimationLayer!.CurrentFrame -= 1;

            if (Input.IsPressed(Button.Up))
                player.AnimationLayer!.CurrentFrame += 10;

            if (Input.IsPressed(Button.Down))
                player.AnimationLayer!.CurrentFrame -= 10;

            var actionController = player.ActionController;
            if (Input.IsPressed(Button.Circle))
                Log.Info($"Action: {actionController.CurrentAction} Frame: {player.AnimationLayer!.CurrentFrame}");
        }

        private static bool TogglePause(Entity entity)
        {
            if (entity.AnimationLayer == null)
                return false;

            return entity.AnimationLayer.Paused = !entity.AnimationLayer.Paused;
        }
    }
}
