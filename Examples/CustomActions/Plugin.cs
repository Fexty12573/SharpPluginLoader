using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Actions;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Entities;

namespace CustomActions;

public class Plugin : IPlugin
{
    public string Name => "Custom Actions";
    public string Author => "Fexty";

    public void OnLoad()
    {
        ActionCloner.RegisterAction<SpikeDiveAction>(MonsterType.RuinerNergigante, 5);
    }
}

[CustomAction("My Custom Action", BaseActionId = 180, Flags = 0x1)]
public class SpikeDiveAction : CustomAction
{
    private bool _didSpikes;

    public override void OnExecute()
    {
        GetRef<AnimationId>(0x1B0) = new AnimationId(1, 23);
        _didSpikes = false;

        ParentOnExecute();
    }

    public override bool OnUpdate()
    {
        if (!_didSpikes && Parent?.AnimationFrame > 20f)
        {
            Parent?.CreateEffect(2007, 0);
            _didSpikes = true;
        }

        return ParentOnUpdate();
    }
}
