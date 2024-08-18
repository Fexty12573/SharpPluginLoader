using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Entities;

namespace CustomActions;

public class Plugin : IPlugin
{
    public string Name => "Custom Actions";
    public string Author => "Fexty";

    private bool _didSpikes = false;

    public void OnLoad()
    {
        var action = new CustomAction
        {
            Name = "My Custom Action",
            Flags = 0x1,
            BaseActionId = 180,
            OnExecute = (action, parentFunc, baseFunc) =>
            {
                action.GetRef<AnimationId>(0x1B0) = new AnimationId(1, 23);
                _didSpikes = false;
                parentFunc(action.Instance);
            },
            OnUpdate = (action, parentFunc, baseFunc) =>
            {
                if (!_didSpikes && action.Parent?.AnimationFrame > 20f)
                {
                    action.Parent?.CreateEffect(2007, 0);
                    _didSpikes = true;
                }

                return parentFunc(action.Instance);
            }
        };

        ActionCloner.RegisterAction(MonsterType.RuinerNergigante, 5, action);
    }
}
